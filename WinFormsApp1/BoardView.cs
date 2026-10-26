using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace YgxViewer
{
    public enum ColorMode { None = 0, Block = 1, Part = 2, Nozzle = 3, Kind = 4 }
    public enum LabelMode { Auto = 0, All = 1, None = 2 }
    public enum ThemeKind { Classic = 0, Pcb = 1, Dark = 2 }

    /// <summary>Kotak pembatas satu blok (dipakai untuk bingkai blok dan zoom ke blok).</summary>
    public class BlockBox
    {
        public string Label = "";
        public double X0, Y0, X1, Y1;
        public int Count;
        public int SortKey;
    }

    internal class Theme
    {
        public Color Back, Board, BoardLine, GridMinor, GridMajor, GridText;
        public Color CompLine, CompFill, Label, Dim, Hover, Select, BlockLine, Fid;
        public Color OverlayBg, OverlayText;
    }

    internal class LegendItem
    {
        public Color Color;
        public string Text = "";
        public int Count;
    }

    /// <summary>
    /// Kontrol penampil PCB.
    /// Scroll = zoom, drag kiri = geser, klik = pilih, klik ganda = zoom ke komponen.
    /// </summary>
    public class BoardView : Control
    {
        private static readonly Color[] Palette = new Color[]
        {
            Color.FromArgb(31, 119, 180), Color.FromArgb(255, 127, 14), Color.FromArgb(44, 160, 44),
            Color.FromArgb(214, 39, 40), Color.FromArgb(148, 103, 189), Color.FromArgb(140, 86, 75),
            Color.FromArgb(227, 119, 194), Color.FromArgb(127, 127, 127), Color.FromArgb(188, 189, 34),
            Color.FromArgb(23, 190, 207), Color.FromArgb(0, 128, 96), Color.FromArgb(255, 187, 120)
        };

        private YgxData _data;
        private double _scale = 1.0;
        private double _offX, _offY;
        private bool _autoFit = true;

        private bool _flipY = true;
        private bool _mirror = false;
        private int _viewRot = 0;
        private bool _showGrid = true;
        private bool _showBlocks = true;
        private bool _showFid = true;
        private LabelMode _labelMode = LabelMode.Auto;
        private ColorMode _colorMode = ColorMode.None;
        private Theme _theme = MakeTheme(ThemeKind.Classic);

        private MountItem _selected;
        private MountItem _hover;
        private HashSet<MountItem> _highlight;

        private List<BlockBox> _blocks = new List<BlockBox>();
        private Dictionary<MountItem, string> _blockOf = new Dictionary<MountItem, string>();
        private Dictionary<MountItem, Color> _colors = new Dictionary<MountItem, Color>();
        private List<LegendItem> _legend = new List<LegendItem>();

        private bool _dragging;
        private bool _moved;
        private Point _dragStart;
        private Point _last;
        private Point _mouse;

        private bool _measure;
        private bool _hasA;
        private bool _hasB;
        private double _ax, _ay, _bx, _by;

        public event Action<MountItem> MountClicked;
        public event Action<MountItem> MountHover;
        public event Action<double, double> CursorMoved;
        public event Action<string> MeasureChanged;
        public event Action<double> ZoomChanged;

        public BoardView()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw, true);
            BackColor = _theme.Back;
        }

        // ================= Properti =================

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public YgxData Data
        {
            get { return _data; }
            set
            {
                _data = value;
                _selected = null;
                _hover = null;
                _highlight = null;
                ClearMeasure();
                RebuildBlocks();
                RebuildColors();
                FitToView();
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public MountItem Selected
        {
            get { return _selected; }
            set { _selected = value; Invalidate(); }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public HashSet<MountItem> Highlight
        {
            get { return _highlight; }
            set { _highlight = value; Invalidate(); }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IList<BlockBox> Blocks
        {
            get { return _blocks; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public LabelMode LabelMode
        {
            get { return _labelMode; }
            set { _labelMode = value; Invalidate(); }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ColorMode ColorMode
        {
            get { return _colorMode; }
            set { _colorMode = value; RebuildColors(); Invalidate(); }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ThemeKind ThemeKind
        {
            set
            {
                _theme = MakeTheme(value);
                BackColor = _theme.Back;
                Invalidate();
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ShowGrid
        {
            get { return _showGrid; }
            set { _showGrid = value; Invalidate(); }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ShowBlocks
        {
            get { return _showBlocks; }
            set { _showBlocks = value; Invalidate(); }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ShowFiducials
        {
            get { return _showFid; }
            set { _showFid = value; Invalidate(); }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool FlipY
        {
            get { return _flipY; }
            set { _flipY = value; ClearMeasure(); FitToView(); }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Mirror
        {
            get { return _mirror; }
            set { _mirror = value; FitToView(); }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int ViewRotation
        {
            get { return _viewRot; }
            set
            {
                int v = ((value % 360) + 360) % 360;
                _viewRot = (v / 90) * 90;
                FitToView();
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool MeasureMode
        {
            get { return _measure; }
            set
            {
                _measure = value;
                Cursor = value ? Cursors.Cross : Cursors.Default;
                ClearMeasure();
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public double ViewScale
        {
            get { return _scale; }
        }

        public string BlockLabel(MountItem m)
        {
            string s;
            return _blockOf.TryGetValue(m, out s) ? s : "";
        }

        // ================= Tema =================

        private static Theme MakeTheme(ThemeKind kind)
        {
            Theme t = new Theme();
            switch (kind)
            {
                case ThemeKind.Pcb:
                    t.Back = Color.FromArgb(30, 34, 38);
                    t.Board = Color.FromArgb(20, 100, 62);
                    t.BoardLine = Color.FromArgb(214, 180, 70);
                    t.GridMinor = Color.FromArgb(28, 114, 72);
                    t.GridMajor = Color.FromArgb(44, 140, 94);
                    t.GridText = Color.FromArgb(150, 215, 180);
                    t.CompLine = Color.FromArgb(235, 235, 235);
                    t.CompFill = Color.FromArgb(60, 255, 255, 255);
                    t.Label = Color.FromArgb(255, 240, 150);
                    t.Dim = Color.FromArgb(52, 130, 92);
                    t.Hover = Color.FromArgb(80, 200, 255);
                    t.Select = Color.FromArgb(255, 70, 70);
                    t.BlockLine = Color.FromArgb(255, 200, 0);
                    t.Fid = Color.FromArgb(255, 130, 255);
                    t.OverlayBg = Color.FromArgb(215, 15, 35, 28);
                    t.OverlayText = Color.FromArgb(240, 240, 240);
                    break;

                case ThemeKind.Dark:
                    t.Back = Color.FromArgb(18, 18, 20);
                    t.Board = Color.FromArgb(38, 40, 46);
                    t.BoardLine = Color.FromArgb(120, 120, 135);
                    t.GridMinor = Color.FromArgb(48, 50, 58);
                    t.GridMajor = Color.FromArgb(68, 71, 82);
                    t.GridText = Color.FromArgb(125, 130, 145);
                    t.CompLine = Color.FromArgb(205, 205, 215);
                    t.CompFill = Color.FromArgb(40, 255, 255, 255);
                    t.Label = Color.FromArgb(130, 190, 255);
                    t.Dim = Color.FromArgb(78, 80, 92);
                    t.Hover = Color.FromArgb(80, 220, 255);
                    t.Select = Color.FromArgb(255, 80, 80);
                    t.BlockLine = Color.FromArgb(255, 190, 60);
                    t.Fid = Color.FromArgb(255, 130, 255);
                    t.OverlayBg = Color.FromArgb(220, 24, 24, 28);
                    t.OverlayText = Color.FromArgb(230, 230, 235);
                    break;

                default: // Classic (putih seperti tampilan Yamaha)
                    t.Back = Color.Black;
                    t.Board = Color.White;
                    t.BoardLine = Color.FromArgb(120, 120, 120);
                    t.GridMinor = Color.FromArgb(238, 238, 238);
                    t.GridMajor = Color.FromArgb(214, 214, 214);
                    t.GridText = Color.FromArgb(150, 150, 150);
                    t.CompLine = Color.Black;
                    t.CompFill = Color.Transparent;
                    t.Label = Color.FromArgb(0, 0, 220);
                    t.Dim = Color.FromArgb(205, 205, 205);
                    t.Hover = Color.FromArgb(0, 120, 215);
                    t.Select = Color.FromArgb(220, 30, 30);
                    t.BlockLine = Color.FromArgb(0, 140, 70);
                    t.Fid = Color.FromArgb(200, 0, 200);
                    t.OverlayBg = Color.FromArgb(225, 255, 255, 255);
                    t.OverlayText = Color.FromArgb(40, 40, 40);
                    break;
            }
            return t;
        }

        // ================= Transformasi koordinat =================
        // Urutan: mirror X -> balik Y -> rotasi tampilan (searah jarum jam).

        private void Rot(double x, double y, out double ru, out double rv)
        {
            double u = _mirror ? -x : x;
            double v = _flipY ? -y : y;
            switch (_viewRot)
            {
                case 90: ru = -v; rv = u; break;
                case 180: ru = -u; rv = -v; break;
                case 270: ru = v; rv = -u; break;
                default: ru = u; rv = v; break;
            }
        }

        private PointF ToScreen(double x, double y)
        {
            double ru, rv;
            Rot(x, y, out ru, out rv);
            return new PointF((float)(_offX + ru * _scale), (float)(_offY + rv * _scale));
        }

        private void ToBoard(double px, double py, out double x, out double y)
        {
            double ru = (px - _offX) / _scale;
            double rv = (py - _offY) / _scale;
            double u, v;
            switch (_viewRot)
            {
                case 90: u = rv; v = -ru; break;
                case 180: u = -ru; v = -rv; break;
                case 270: u = -rv; v = ru; break;
                default: u = ru; v = rv; break;
            }
            x = _mirror ? -u : u;
            y = _flipY ? -v : v;
        }

        private float AngleFor(MountItem m)
        {
            double a = _flipY ? -m.R : m.R;
            if (_mirror) a = 180.0 - a;
            return (float)(a + _viewRot);
        }

        private void PixelSize(MountItem m, out float w, out float h)
        {
            double bx = m.BodyX > 0 ? m.BodyX : 1.0;
            double by = m.BodyY > 0 ? m.BodyY : 1.0;
            w = Math.Max((float)(bx * _scale), 2f);
            h = Math.Max((float)(by * _scale), 2f);
        }

        // ================= Zoom / Fit =================

        private void NotifyZoom()
        {
            if (ZoomChanged != null) ZoomChanged(_scale);
        }

        private void FitRect(double x0, double y0, double x1, double y1, int margin)
        {
            if (_data == null || Width < 40 || Height < 40)
            {
                Invalidate();
                return;
            }

            double[] xs = { x0, x1, x0, x1 };
            double[] ys = { y0, y0, y1, y1 };
            double minU = double.MaxValue, maxU = double.MinValue;
            double minV = double.MaxValue, maxV = double.MinValue;
            for (int i = 0; i < 4; i++)
            {
                double ru, rv;
                Rot(xs[i], ys[i], out ru, out rv);
                if (ru < minU) minU = ru;
                if (ru > maxU) maxU = ru;
                if (rv < minV) minV = rv;
                if (rv > maxV) maxV = rv;
            }

            double bw = Math.Max(maxU - minU, 0.001);
            double bh = Math.Max(maxV - minV, 0.001);
            double sc = Math.Min((Width - 2.0 * margin) / bw, (Height - 2.0 * margin) / bh);
            if (sc < 0.05) sc = 0.05;
            if (sc > 500) sc = 500;

            _scale = sc;
            _offX = Width / 2.0 - ((minU + maxU) / 2.0) * sc;
            _offY = Height / 2.0 - ((minV + maxV) / 2.0) * sc;
            NotifyZoom();
            Invalidate();
        }

        public void FitToView()
        {
            _autoFit = true;
            if (_data != null)
                FitRect(0, 0, _data.Board.SizeX, _data.Board.SizeY, 16);
            else
                Invalidate();
        }

        public void ZoomToMount(MountItem m)
        {
            if (_data == null || m == null) return;
            _autoFit = false;
            double ext = Math.Max(Math.Max(m.BodyX, m.BodyY), 2.0);
            double pad = Math.Max(ext * 3.0, 8.0);
            FitRect(m.X - pad, m.Y - pad, m.X + pad, m.Y + pad, 20);
        }

        public void ZoomToBlock(int index)
        {
            if (_data == null || index < 0 || index >= _blocks.Count) return;
            _autoFit = false;
            BlockBox b = _blocks[index];
            FitRect(b.X0 - 4, b.Y0 - 4, b.X1 + 4, b.Y1 + 4, 16);
        }

        /// <summary>Jika komponen berada di luar layar, geser tampilan agar komponen di tengah.</summary>
        public void EnsureVisible(MountItem m)
        {
            if (_data == null || m == null) return;
            PointF p = ToScreen(m.X, m.Y);
            if (p.X < 30 || p.X > Width - 30 || p.Y < 30 || p.Y > Height - 30)
            {
                _autoFit = false;
                _offX += Width / 2.0 - p.X;
                _offY += Height / 2.0 - p.Y;
                Invalidate();
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (_autoFit && _data != null) FitToView();
        }

        // ================= Blok, warna, legenda =================

        private void RebuildBlocks()
        {
            _blocks.Clear();
            _blockOf.Clear();
            if (_data == null) return;

            List<string> files = new List<string>();
            foreach (MountItem m in _data.Mounts)
            {
                string f = m.SourceFile ?? "";
                if (!files.Contains(f)) files.Add(f);
            }

            Dictionary<string, BlockBox> map = new Dictionary<string, BlockBox>();
            foreach (MountItem m in _data.Mounts)
            {
                if (m.OrgBlk <= 0) continue;

                int fi = files.IndexOf(m.SourceFile ?? "");
                string label = (files.Count > 1 ? "F" + (fi + 1) + " " : "") + m.BlockText;
                string key = fi + "|" + m.RepeatNo + "|" + m.OrgBlk;
                _blockOf[m] = label;

                double ext = Math.Max(Math.Max(m.BodyX, m.BodyY) / 2.0, 0.5);
                BlockBox box;
                if (!map.TryGetValue(key, out box))
                {
                    box = new BlockBox();
                    box.Label = label;
                    box.X0 = m.X - ext; box.X1 = m.X + ext;
                    box.Y0 = m.Y - ext; box.Y1 = m.Y + ext;
                    box.SortKey = m.RepeatNo * 1000000 + fi * 1000 + m.OrgBlk;
                    map[key] = box;
                    _blocks.Add(box);
                }
                else
                {
                    if (m.X - ext < box.X0) box.X0 = m.X - ext;
                    if (m.X + ext > box.X1) box.X1 = m.X + ext;
                    if (m.Y - ext < box.Y0) box.Y0 = m.Y - ext;
                    if (m.Y + ext > box.Y1) box.Y1 = m.Y + ext;
                }
                box.Count++;
            }

            foreach (BlockBox b in _blocks)
            {
                b.X0 -= 1.5; b.Y0 -= 1.5; b.X1 += 1.5; b.Y1 += 1.5;
            }
            _blocks.Sort((a, b) => a.SortKey.CompareTo(b.SortKey));
        }

        private string ColorKey(MountItem m)
        {
            switch (_colorMode)
            {
                case ColorMode.Block: return m.OrgBlk > 0 ? BlockLabel(m) : "-";
                case ColorMode.Part: return (m.SourceFile ?? "") + "|" + m.Comp;
                case ColorMode.Nozzle: return m.Nozzle.ToString();
                case ColorMode.Kind: return m.Kind;
                default: return "";
            }
        }

        private string ColorText(MountItem m)
        {
            switch (_colorMode)
            {
                case ColorMode.Block: return "Blok " + (m.OrgBlk > 0 ? BlockLabel(m) : "-");
                case ColorMode.Part:
                    return string.IsNullOrEmpty(m.PartName) ? "Part #" + m.Comp : m.PartName;
                case ColorMode.Nozzle: return "Nozzle " + m.Nozzle;
                case ColorMode.Kind: return "Jenis " + m.Kind;
                default: return "";
            }
        }

        private string LegendTitle()
        {
            switch (_colorMode)
            {
                case ColorMode.Block: return "Warna: per blok";
                case ColorMode.Part: return "Warna: per Part";
                case ColorMode.Nozzle: return "Warna: per nozzle";
                case ColorMode.Kind: return "Warna: per jenis";
                default: return "";
            }
        }

        private void RebuildColors()
        {
            _colors.Clear();
            _legend.Clear();
            if (_data == null || _colorMode == ColorMode.None) return;

            Dictionary<string, int> index = new Dictionary<string, int>();
            Dictionary<string, LegendItem> items = new Dictionary<string, LegendItem>();

            foreach (MountItem m in _data.Mounts)
            {
                string key = ColorKey(m);
                int idx;
                if (!index.TryGetValue(key, out idx))
                {
                    idx = index.Count;
                    index[key] = idx;

                    LegendItem li = new LegendItem();
                    li.Color = Palette[idx % Palette.Length];
                    li.Text = ColorText(m);
                    items[key] = li;
                    _legend.Add(li);
                }
                items[key].Count++;
                _colors[m] = items[key].Color;
            }

            _legend.Sort((a, b) => b.Count.CompareTo(a.Count));
        }

        // ================= Menggambar =================

        protected override void OnPaint(PaintEventArgs e)
        {
            DrawScene(e.Graphics, Width, Height, false);
        }

        private static double NiceStep(double minMm)
        {
            double[] steps = { 0.5, 1, 2, 5, 10, 20, 50, 100, 200, 500, 1000 };
            foreach (double s in steps)
                if (s >= minMm) return s;
            return 1000;
        }

        private static GraphicsPath RoundRect(RectangleF r, float rad)
        {
            float d = rad * 2f;
            GraphicsPath gp = new GraphicsPath();
            gp.AddArc(r.X, r.Y, d, d, 180, 90);
            gp.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            gp.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            gp.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            gp.CloseFigure();
            return gp;
        }

        private void DrawScene(Graphics g, int w, int h, bool export)
        {
            Theme th = _theme;
            g.Clear(th.Back);

            if (_data == null)
            {
                using (Font f = new Font("Segoe UI", 12f))
                {
                    g.DrawString("Buka file .ygx (tombol Buka, Ctrl+O) atau seret file ke jendela ini.",
                        f, Brushes.Gray, 24f, 24f);
                }
                return;
            }

            g.SmoothingMode = _data.Mounts.Count < 6000 ? SmoothingMode.AntiAlias : SmoothingMode.HighSpeed;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            // ---- PCB ----
            PointF p0 = ToScreen(0, 0);
            PointF p1 = ToScreen(_data.Board.SizeX, _data.Board.SizeY);
            RectangleF br = new RectangleF(Math.Min(p0.X, p1.X), Math.Min(p0.Y, p1.Y),
                                           Math.Abs(p1.X - p0.X), Math.Abs(p1.Y - p0.Y));
            using (Brush bb = new SolidBrush(th.Board))
                g.FillRectangle(bb, br);

            if (_showGrid) DrawGrid(g, w, h, th);
            if (_showBlocks) DrawBlocks(g, th);
            if (_showFid) DrawFiducials(g, th);

            // ---- Komponen ----
            float fs = Math.Max(7f, Math.Min(11f, 6f + (float)(_scale / 6.0)));
            using (Pen linePen = new Pen(th.CompLine, 1f))
            using (Pen tickPen = new Pen(Color.FromArgb(150, th.CompLine), 1f))
            using (Pen dimPen = new Pen(th.Dim, 1f))
            using (Pen selPen = new Pen(th.Select, 2f))
            using (Pen hovPen = new Pen(th.Hover, 2f))
            using (Brush selBrush = new SolidBrush(Color.FromArgb(200, th.Select)))
            using (Brush hovBrush = new SolidBrush(Color.FromArgb(90, th.Hover)))
            using (Brush fillBrush = new SolidBrush(th.CompFill))
            using (Brush labBrush = new SolidBrush(th.Label))
            using (Brush selLabBrush = new SolidBrush(th.Select))
            using (Font font = new Font("Segoe UI", fs))
            using (Font boldFont = new Font("Segoe UI", fs, FontStyle.Bold))
            {
                // state: 0 normal, 1 diredupkan, 2 hover, 3 terpilih
                Action<MountItem, int> drawMount = (m, state) =>
                {
                    PointF p = ToScreen(m.X, m.Y);
                    float pw, ph;
                    PixelSize(m, out pw, out ph);
                    float hw = pw / 2f, hh = ph / 2f;
                    float ext = Math.Max(hw, hh);
                    if (p.X < -2 * ext - 4 || p.Y < -2 * ext - 4 ||
                        p.X > w + 80 + 2 * ext || p.Y > h + 2 * ext + 4) return;

                    Color col;
                    bool colored = _colors.TryGetValue(m, out col);

                    GraphicsState st = g.Save();
                    g.TranslateTransform(p.X, p.Y);
                    g.RotateTransform(AngleFor(m));

                    if (state == 3)
                    {
                        g.FillRectangle(selBrush, -hw, -hh, pw, ph);
                        g.DrawRectangle(selPen, -hw, -hh, pw, ph);
                    }
                    else if (state == 2)
                    {
                        g.FillRectangle(hovBrush, -hw, -hh, pw, ph);
                        g.DrawRectangle(hovPen, -hw, -hh, pw, ph);
                    }
                    else if (state == 1)
                    {
                        g.DrawRectangle(dimPen, -hw, -hh, pw, ph);
                    }
                    else if (m.Fid != 0)
                    {
                        g.FillRectangle(selBrush, -hw, -hh, pw, ph);
                        g.DrawRectangle(linePen, -hw, -hh, pw, ph);
                    }
                    else if (colored)
                    {
                        using (Brush cb = new SolidBrush(Color.FromArgb(120, col)))
                            g.FillRectangle(cb, -hw, -hh, pw, ph);
                        using (Pen cp = new Pen(col, 1.3f))
                            g.DrawRectangle(cp, -hw, -hh, pw, ph);
                    }
                    else
                    {
                        if (th.CompFill.A > 0) g.FillRectangle(fillBrush, -hw, -hh, pw, ph);
                        g.DrawRectangle(linePen, -hw, -hh, pw, ph);
                    }

                    // garis penanda arah rotasi (0 derajat = sumbu X body)
                    if (state != 1 && pw >= 10f)
                        g.DrawLine(tickPen, 0f, 0f, hw, 0f);

                    g.Restore(st);

                    bool label;
                    if (state >= 2) label = true;
                    else if (state == 1) label = false;
                    else if (_labelMode == LabelMode.All) label = true;
                    else if (_labelMode == LabelMode.Auto) label = (_scale >= 8.0) || (_highlight != null);
                    else label = false;

                    if (label && !string.IsNullOrEmpty(m.Comment))
                    {
                        Brush lb = state == 3 ? selLabBrush : labBrush;
                        g.DrawString(m.Comment, state >= 2 ? boldFont : font, lb, p.X + ext + 1f, p.Y - fs);
                    }
                };

                // 1) diredupkan, 2) normal, 3) hover, 4) terpilih
                if (_highlight != null)
                    foreach (MountItem m in _data.Mounts)
                        if (!_highlight.Contains(m) && !ReferenceEquals(m, _selected) && !ReferenceEquals(m, _hover))
                            drawMount(m, 1);

                foreach (MountItem m in _data.Mounts)
                {
                    if (_highlight != null && !_highlight.Contains(m)) continue;
                    if (ReferenceEquals(m, _selected) || ReferenceEquals(m, _hover)) continue;
                    drawMount(m, 0);
                }

                if (_hover != null && !ReferenceEquals(_hover, _selected)) drawMount(_hover, 2);
                if (_selected != null) drawMount(_selected, 3);
            }

            // ---- Garis tepi PCB ----
            using (Pen bp = new Pen(th.BoardLine, 2f))
                g.DrawRectangle(bp, br.X, br.Y, br.Width, br.Height);

            DrawMeasure(g, th);

            // ---- Overlay ----
            DrawLegend(g, w, h, th);
            DrawScaleBar(g, w, h, th);
            DrawGizmo(g, w, th);
            if (!export) DrawTooltip(g, w, h, th);
        }

        private void DrawGrid(Graphics g, int w, int h, Theme th)
        {
            double major = NiceStep(80.0 / _scale);
            double minor = major / 5.0;
            bool showMinor = minor * _scale >= 12.0;
            double step = showMinor ? minor : major;
            int per = showMinor ? 5 : 1;
            double sx = _data.Board.SizeX;
            double sy = _data.Board.SizeY;

            using (Pen pMinor = new Pen(th.GridMinor, 1f))
            using (Pen pMajor = new Pen(th.GridMajor, 1f))
            using (Brush tb = new SolidBrush(th.GridText))
            using (Font font = new Font("Segoe UI", 7.5f))
            {
                int nx = (int)Math.Floor(sx / step + 1e-9);
                for (int i = 0; i <= nx; i++)
                {
                    double x = i * step;
                    bool isMajor = (i % per) == 0;
                    DrawGridLine(g, x, 0, x, sy, isMajor ? pMajor : pMinor, isMajor, x, tb, font, w, h);
                }

                int ny = (int)Math.Floor(sy / step + 1e-9);
                for (int j = 0; j <= ny; j++)
                {
                    double y = j * step;
                    bool isMajor = (j % per) == 0;
                    DrawGridLine(g, 0, y, sx, y, isMajor ? pMajor : pMinor, isMajor, y, tb, font, w, h);
                }
            }
        }

        private void DrawGridLine(Graphics g, double xa, double ya, double xb, double yb, Pen pen,
                                  bool label, double value, Brush tb, Font font, int w, int h)
        {
            PointF a = ToScreen(xa, ya);
            PointF b = ToScreen(xb, yb);
            bool vertical = Math.Abs(b.X - a.X) < Math.Abs(b.Y - a.Y);

            if (vertical)
            {
                if (a.X < -1 || a.X > w + 1) return;
            }
            else
            {
                if (a.Y < -1 || a.Y > h + 1) return;
            }

            g.DrawLine(pen, a, b);
            if (!label) return;

            string s = value.ToString("0.##");
            if (vertical)
            {
                float top = Math.Min(a.Y, b.Y);
                float bot = Math.Max(a.Y, b.Y);
                float ly = Math.Max(top + 2f, 2f);
                if (ly + 12f > bot) return;
                g.DrawString(s, font, tb, a.X + 2f, ly);
            }
            else
            {
                float left = Math.Min(a.X, b.X);
                float right = Math.Max(a.X, b.X);
                float lx = Math.Max(left + 2f, 2f);
                if (lx + 24f > right) return;
                g.DrawString(s, font, tb, lx, a.Y + 1f);
            }
        }

        private void DrawBlocks(Graphics g, Theme th)
        {
            if (_blocks.Count == 0) return;

            using (Pen pen = new Pen(th.BlockLine, 1.5f))
            using (Brush tb = new SolidBrush(th.BlockLine))
            using (Font font = new Font("Segoe UI", 9f, FontStyle.Bold))
            {
                pen.DashStyle = DashStyle.Dash;
                foreach (BlockBox bb in _blocks)
                {
                    PointF a = ToScreen(bb.X0, bb.Y0);
                    PointF b = ToScreen(bb.X1, bb.Y1);
                    float x = Math.Min(a.X, b.X);
                    float y = Math.Min(a.Y, b.Y);
                    float rw = Math.Abs(b.X - a.X);
                    float rh = Math.Abs(b.Y - a.Y);
                    g.DrawRectangle(pen, x, y, rw, rh);
                    g.DrawString(bb.Label, font, tb, x + 3f, y + 2f);
                }
            }
        }

        private void DrawFiducials(Graphics g, Theme th)
        {
            if (_data.Fiducials.Count == 0) return;

            using (Pen pen = new Pen(th.Fid, 1.5f))
            using (Brush tb = new SolidBrush(th.Fid))
            using (Font font = new Font("Segoe UI", 8f, FontStyle.Bold))
            {
                foreach (FiducialInfo f in _data.Fiducials)
                {
                    PointF p = ToScreen(f.X, f.Y);
                    float r = Math.Min(Math.Max(7f, (float)(1.5 * _scale)), 40f);
                    g.DrawEllipse(pen, p.X - r, p.Y - r, 2f * r, 2f * r);
                    g.DrawLine(pen, p.X - r * 1.6f, p.Y, p.X + r * 1.6f, p.Y);
                    g.DrawLine(pen, p.X, p.Y - r * 1.6f, p.X, p.Y + r * 1.6f);
                    g.DrawString(f.Label, font, tb, p.X + r + 2f, p.Y + r);
                }
            }
        }

        private void DrawMeasure(Graphics g, Theme th)
        {
            if (!_hasA) return;

            PointF a = ToScreen(_ax, _ay);
            using (Pen pen = new Pen(th.Hover, 2f))
            using (Brush br = new SolidBrush(th.Hover))
            using (Brush bg = new SolidBrush(th.OverlayBg))
            using (Brush tb = new SolidBrush(th.OverlayText))
            using (Font font = new Font("Segoe UI", 9f, FontStyle.Bold))
            {
                g.FillEllipse(br, a.X - 4f, a.Y - 4f, 8f, 8f);
                if (!_hasB) return;

                PointF b = ToScreen(_bx, _by);
                pen.DashStyle = DashStyle.Dash;
                g.DrawLine(pen, a, b);
                g.FillEllipse(br, b.X - 4f, b.Y - 4f, 8f, 8f);

                double dx = _bx - _ax, dy = _by - _ay;
                string s = Math.Sqrt(dx * dx + dy * dy).ToString("0.000") + " mm";
                SizeF sz = g.MeasureString(s, font);
                float mx = (a.X + b.X) / 2f + 8f;
                float my = (a.Y + b.Y) / 2f - sz.Height - 4f;
                using (GraphicsPath gp = RoundRect(new RectangleF(mx - 4f, my - 2f, sz.Width + 8f, sz.Height + 4f), 4f))
                    g.FillPath(bg, gp);
                g.DrawString(s, font, tb, mx, my);
            }
        }

        private void DrawLegend(Graphics g, int w, int h, Theme th)
        {
            if (_legend.Count == 0) return;

            using (Font font = new Font("Segoe UI", 8.5f))
            using (Font bold = new Font("Segoe UI", 8.5f, FontStyle.Bold))
            using (Brush bg = new SolidBrush(th.OverlayBg))
            using (Brush tb = new SolidBrush(th.OverlayText))
            {
                int rows = Math.Min(_legend.Count, 12);
                string title = LegendTitle();
                float wmax = g.MeasureString(title, bold).Width;
                for (int i = 0; i < rows; i++)
                {
                    string s = _legend[i].Text + "  (" + _legend[i].Count + ")";
                    wmax = Math.Max(wmax, g.MeasureString(s, font).Width + 18f);
                }

                float rowH = 16f;
                float bw = wmax + 18f;
                bool more = _legend.Count > rows;
                float bh = rowH * (rows + 1) + 12f + (more ? rowH : 0f);
                float x = 10f;
                float y = h - bh - 10f;

                using (GraphicsPath gp = RoundRect(new RectangleF(x, y, bw, bh), 6f))
                    g.FillPath(bg, gp);

                g.DrawString(title, bold, tb, x + 8f, y + 6f);
                for (int i = 0; i < rows; i++)
                {
                    float ry = y + 6f + rowH * (i + 1);
                    using (Brush cb = new SolidBrush(_legend[i].Color))
                        g.FillRectangle(cb, x + 8f, ry + 3f, 11f, 10f);
                    g.DrawString(_legend[i].Text + "  (" + _legend[i].Count + ")", font, tb, x + 24f, ry);
                }
                if (more)
                {
                    float ry = y + 6f + rowH * (rows + 1);
                    g.DrawString("+" + (_legend.Count - rows) + " lainnya", font, tb, x + 8f, ry);
                }
            }
        }

        private void DrawScaleBar(Graphics g, int w, int h, Theme th)
        {
            double mm = NiceStep(90.0 / _scale);
            float len = (float)(mm * _scale);
            if (len < 20f || len > w * 0.7f) return;

            using (Font font = new Font("Segoe UI", 8.5f, FontStyle.Bold))
            using (Brush bg = new SolidBrush(th.OverlayBg))
            using (Brush tb = new SolidBrush(th.OverlayText))
            using (Pen pen = new Pen(th.OverlayText, 2f))
            {
                string s = mm.ToString("0.##") + " mm";
                float bw = len + 24f;
                float bh = 34f;
                float x = w - bw - 10f;
                float y = h - bh - 10f;
                using (GraphicsPath gp = RoundRect(new RectangleF(x, y, bw, bh), 6f))
                    g.FillPath(bg, gp);

                float lx = x + 12f;
                float ly = y + 24f;
                g.DrawLine(pen, lx, ly, lx + len, ly);
                g.DrawLine(pen, lx, ly - 4f, lx, ly + 4f);
                g.DrawLine(pen, lx + len, ly - 4f, lx + len, ly + 4f);
                g.DrawString(s, font, tb, lx, y + 4f);
            }
        }

        /// <summary>Penunjuk arah sumbu X dan Y board (berguna saat rotasi / mirror / balik Y).</summary>
        private void DrawGizmo(Graphics g, int w, Theme th)
        {
            float cx = w - 48f;
            float cy = 48f;
            float len = 24f;
            double ux, uy, vx, vy;
            Rot(1, 0, out ux, out uy);
            Rot(0, 1, out vx, out vy);

            using (Brush bg = new SolidBrush(th.OverlayBg))
            using (Font font = new Font("Segoe UI", 8.5f, FontStyle.Bold))
            using (Pen px = new Pen(Color.FromArgb(230, 60, 60), 2f))
            using (Pen py = new Pen(Color.FromArgb(40, 170, 80), 2f))
            using (Brush bx = new SolidBrush(Color.FromArgb(230, 60, 60)))
            using (Brush by = new SolidBrush(Color.FromArgb(40, 170, 80)))
            {
                g.FillEllipse(bg, cx - 38f, cy - 38f, 76f, 76f);
                px.EndCap = LineCap.ArrowAnchor;
                py.EndCap = LineCap.ArrowAnchor;
                g.DrawLine(px, cx, cy, cx + (float)ux * len, cy + (float)uy * len);
                g.DrawLine(py, cx, cy, cx + (float)vx * len, cy + (float)vy * len);
                g.DrawString("X", font, bx, cx + (float)ux * (len + 5f) - 4f, cy + (float)uy * (len + 5f) - 7f);
                g.DrawString("Y", font, by, cx + (float)vx * (len + 5f) - 4f, cy + (float)vy * (len + 5f) - 7f);
            }
        }

        private void DrawTooltip(Graphics g, int w, int h, Theme th)
        {
            if (_hover == null || _dragging) return;
            MountItem m = _hover;

            List<string> lines = new List<string>();
            lines.Add(m.Comment + (string.IsNullOrEmpty(m.PartName) ? "" : "   " + m.PartName));
            lines.Add(string.Format("X {0:0.000}   Y {1:0.000}   R {2:0.#}", m.X, m.Y, m.R));
            lines.Add("Body " + (m.HasBody ? m.BodyText + " mm" : "(data Part tidak ada)"));
            lines.Add("Blok " + m.BlockText + "   Head " + m.Head + "   Nozzle " + m.Nozzle);

            using (Font font = new Font("Segoe UI", 8.5f))
            using (Font bold = new Font("Segoe UI", 9f, FontStyle.Bold))
            using (Brush bg = new SolidBrush(Color.FromArgb(232, 25, 30, 38)))
            using (Brush tb = new SolidBrush(Color.FromArgb(245, 245, 245)))
            {
                float bw = 0f;
                for (int i = 0; i < lines.Count; i++)
                    bw = Math.Max(bw, g.MeasureString(lines[i], i == 0 ? bold : font).Width);
                bw += 16f;
                float lh = 16f;
                float bh = lh * lines.Count + 10f;

                float x = _mouse.X + 16f;
                float y = _mouse.Y + 16f;
                if (x + bw > w - 4f) x = _mouse.X - bw - 12f;
                if (y + bh > h - 4f) y = _mouse.Y - bh - 12f;
                if (x < 4f) x = 4f;
                if (y < 4f) y = 4f;

                using (GraphicsPath gp = RoundRect(new RectangleF(x, y, bw, bh), 6f))
                    g.FillPath(bg, gp);
                for (int i = 0; i < lines.Count; i++)
                    g.DrawString(lines[i], i == 0 ? bold : font, tb, x + 8f, y + 5f + lh * i);
            }
        }

        // ================= Ekspor gambar =================

        /// <summary>Gambar seluruh PCB ke bitmap (lebar piksel ditentukan).</summary>
        public Bitmap RenderToBitmap(int widthPx)
        {
            if (_data == null) return null;

            double sx = _data.Board.SizeX;
            double sy = _data.Board.SizeY;
            double[] xs = { 0, sx, 0, sx };
            double[] ys = { 0, 0, sy, sy };
            double minU = double.MaxValue, maxU = double.MinValue;
            double minV = double.MaxValue, maxV = double.MinValue;
            for (int i = 0; i < 4; i++)
            {
                double ru, rv;
                Rot(xs[i], ys[i], out ru, out rv);
                if (ru < minU) minU = ru;
                if (ru > maxU) maxU = ru;
                if (rv < minV) minV = rv;
                if (rv > maxV) maxV = rv;
            }

            int margin = 30;
            double bw = Math.Max(maxU - minU, 0.001);
            double bh = Math.Max(maxV - minV, 0.001);
            double sc = (widthPx - 2.0 * margin) / bw;
            int heightPx = (int)Math.Ceiling(bh * sc + 2.0 * margin);

            Bitmap bmp = new Bitmap(widthPx, heightPx);
            double oldScale = _scale, oldX = _offX, oldY = _offY;
            try
            {
                _scale = sc;
                _offX = margin - minU * sc;
                _offY = margin - minV * sc;
                using (Graphics g = Graphics.FromImage(bmp))
                    DrawScene(g, widthPx, heightPx, true);
            }
            finally
            {
                _scale = oldScale;
                _offX = oldX;
                _offY = oldY;
            }
            return bmp;
        }

        // ================= Ukur jarak =================

        public void ClearMeasure()
        {
            bool had = _hasA;
            _hasA = false;
            _hasB = false;
            Invalidate();
            if (had && MeasureChanged != null) MeasureChanged("");
        }

        private void MeasureClick(Point pt)
        {
            double x, y;
            MountItem hit = HitTest(pt, 8.0);
            if (hit != null)
            {
                x = hit.X;
                y = hit.Y;
            }
            else
            {
                ToBoard(pt.X, pt.Y, out x, out y);
            }

            if (!_hasA || _hasB)
            {
                _ax = x; _ay = y;
                _hasA = true;
                _hasB = false;
                if (MeasureChanged != null)
                    MeasureChanged(string.Format("Titik A: X {0:0.000}  Y {1:0.000}  - klik titik B", x, y));
            }
            else
            {
                _bx = x; _by = y;
                _hasB = true;
                double dx = _bx - _ax, dy = _by - _ay;
                if (MeasureChanged != null)
                    MeasureChanged(string.Format("Jarak {0:0.000} mm   (dX {1:0.000}   dY {2:0.000})",
                        Math.Sqrt(dx * dx + dy * dy), dx, dy));
            }
            Invalidate();
        }

        // ================= Mouse =================

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            Focus();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (_hover != null)
            {
                _hover = null;
                Invalidate();
                if (MountHover != null) MountHover(null);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                _dragging = true;
                _moved = false;
                _dragStart = e.Location;
                _last = e.Location;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            _mouse = e.Location;
            if (_data == null) return;

            double x, y;
            ToBoard(e.X, e.Y, out x, out y);
            if (CursorMoved != null) CursorMoved(x, y);

            if (_dragging)
            {
                if (!_moved &&
                    (Math.Abs(e.X - _dragStart.X) > 3 || Math.Abs(e.Y - _dragStart.Y) > 3))
                {
                    _moved = true;
                    Cursor = Cursors.SizeAll;
                }
                if (_moved)
                {
                    _autoFit = false;
                    _offX += e.X - _last.X;
                    _offY += e.Y - _last.Y;
                    _last = e.Location;
                    Invalidate();
                }
                return;
            }

            MountItem hit = _measure ? null : HitTest(e.Location, 4.0);
            bool changed = !ReferenceEquals(hit, _hover);
            if (changed)
            {
                _hover = hit;
                Cursor = hit != null ? Cursors.Hand : (_measure ? Cursors.Cross : Cursors.Default);
                if (MountHover != null) MountHover(hit);
            }
            // gambar ulang saat hover berubah, atau agar tooltip mengikuti kursor
            if (changed || _hover != null) Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (e.Button == MouseButtons.Right && _measure)
            {
                ClearMeasure();
                return;
            }
            if (e.Button != MouseButtons.Left || !_dragging) return;

            _dragging = false;
            Cursor = _measure ? Cursors.Cross : Cursors.Default;

            if (!_moved && _data != null)
            {
                if (_measure)
                {
                    MeasureClick(e.Location);
                }
                else
                {
                    MountItem hit = HitTest(e.Location, 4.0);
                    _selected = hit;
                    Invalidate();
                    if (MountClicked != null) MountClicked(hit);
                }
            }
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            if (_data == null || _measure || e.Button != MouseButtons.Left) return;
            MountItem hit = HitTest(e.Location, 4.0);
            if (hit != null) ZoomToMount(hit);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            if (_data == null) return;

            _autoFit = false;
            double factor = e.Delta > 0 ? 1.2 : 1.0 / 1.2;
            double newScale = _scale * factor;
            if (newScale < 0.05) newScale = 0.05;
            if (newScale > 500) newScale = 500;
            double f = newScale / _scale;

            _offX = e.X - (e.X - _offX) * f;
            _offY = e.Y - (e.Y - _offY) * f;
            _scale = newScale;
            NotifyZoom();
            Invalidate();
        }

        private MountItem HitTest(Point pt, double extra)
        {
            if (_data == null) return null;

            double best = double.MaxValue;
            MountItem found = null;
            foreach (MountItem m in _data.Mounts)
            {
                if (_highlight != null && !_highlight.Contains(m)) continue;

                PointF p = ToScreen(m.X, m.Y);
                float pw, ph;
                PixelSize(m, out pw, out ph);
                double limit = Math.Max(pw, ph) / 2.0 + extra;

                double dx = p.X - pt.X;
                double dy = p.Y - pt.Y;
                double d2 = dx * dx + dy * dy;
                if (d2 <= limit * limit && d2 < best)
                {
                    best = d2;
                    found = m;
                }
            }
            return found;
        }
    }
}