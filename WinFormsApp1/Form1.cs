using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using YgxViewer;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace YgxViewer
{
    public partial class Form1 : Form
    {
        private YgxData _data;
        private List<MountItem> _view = new List<MountItem>();
        private List<PartRow> _partRows = new List<PartRow>();
        private List<string> _fileInfo = new List<string>();
        private List<string> _fileLines = new List<string>();
        private int _partTotal;
        private bool _init = true;       // true selama konstruksi / pengisian combo
        private bool _syncGrid;          // mencegah putaran event grid <-> kanvas
        private string _sortProp;
        private bool _sortAsc = true;

        public Form1()
        {
            InitializeComponent();

            StyleGrid(dgvMounts);
            StyleGrid(dgvParts);
            BuildMountColumns();
            BuildPartColumns();

            cmbLabel.SelectedIndex = 0;
            cmbColor.SelectedIndex = 0;
            cmbTheme.SelectedIndex = 0;
            cmbRotPcb.SelectedIndex = 0;
            cmbRotMat.SelectedIndex = 0;
            cmbZoomBlock.Items.Add("Semua PCB");
            cmbZoomBlock.SelectedIndex = 0;
            cmbBlockFilter.Items.Add("Semua blok");
            cmbBlockFilter.SelectedIndex = 0;

            boardView.MountClicked += BoardView_MountClicked;
            boardView.CursorMoved += BoardView_CursorMoved;
            boardView.MeasureChanged += BoardView_MeasureChanged;
            boardView.ZoomChanged += BoardView_ZoomChanged;

            _init = false;
            ApplyViewOptions();
            ShowDetail(null);
            UpdateSummary();
        }

        // =====================================================================
        //  Tampilan tabel
        // =====================================================================

        private static void StyleGrid(DataGridView g)
        {
            g.AutoGenerateColumns = false;
            g.BorderStyle = BorderStyle.None;
            g.EnableHeadersVisualStyles = false;
            g.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            g.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(45, 62, 80);
            g.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            g.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(45, 62, 80);
            g.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            g.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(244, 247, 251);
            g.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 120, 215);
            g.DefaultCellStyle.SelectionForeColor = Color.White;
            g.GridColor = Color.FromArgb(225, 230, 236);
            g.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            g.RowTemplate.Height = 22;
        }

        private static void AddCol(DataGridView g, string header, string prop, int width, string format)
        {
            DataGridViewTextBoxColumn c = new DataGridViewTextBoxColumn();
            c.HeaderText = header;
            c.DataPropertyName = prop;
            c.Name = "col" + prop;
            c.Width = width;
            c.ReadOnly = true;
            c.SortMode = DataGridViewColumnSortMode.Programmatic;
            if (!string.IsNullOrEmpty(format))
            {
                c.DefaultCellStyle.Format = format;
                c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
            g.Columns.Add(c);
        }

        private void BuildMountColumns()
        {
            AddCol(dgvMounts, "Ref", "Comment", 70, null);
            AddCol(dgvMounts, "Part", "PartName", 140, null);
            AddCol(dgvMounts, "Comp", "Comp", 48, null);
            AddCol(dgvMounts, "X (mm)", "X", 66, "0.000");
            AddCol(dgvMounts, "Y (mm)", "Y", 66, "0.000");
            AddCol(dgvMounts, "R", "R", 46, "0.##");
            AddCol(dgvMounts, "Body", "BodyText", 80, null);
            AddCol(dgvMounts, "Blok", "BlockText", 56, null);
            AddCol(dgvMounts, "Head", "Head", 44, null);
            AddCol(dgvMounts, "Nozzle", "Nozzle", 54, null);
            AddCol(dgvMounts, "File", "SourceFile", 130, null);
        }

        private void BuildPartColumns()
        {
            AddCol(dgvParts, "Part", "PartName", 160, null);
            AddCol(dgvParts, "Comp", "Comp", 50, null);
            AddCol(dgvParts, "Jumlah", "Qty", 60, null);
            AddCol(dgvParts, "Ukuran (mm)", "BodyText", 90, null);
            AddCol(dgvParts, "Jenis", "Kind", 50, null);
            AddCol(dgvParts, "Contoh Ref", "SampleRef", 80, null);
        }

        // =====================================================================
        //  Opsi tampilan
        // =====================================================================

        private void ApplyViewOptions()
        {
            boardView.LabelMode = (LabelMode)Math.Max(0, cmbLabel.SelectedIndex);
            boardView.ColorMode = (ColorMode)Math.Max(0, cmbColor.SelectedIndex);
            boardView.ThemeKind = (ThemeKind)Math.Max(0, cmbTheme.SelectedIndex);
            boardView.ShowGrid = btnGrid.Checked;
            boardView.ShowBlocks = btnBlocks.Checked;
            boardView.ShowFiducials = btnFid.Checked;
            boardView.Mirror = btnMirror.Checked;
            boardView.ViewRotation = Math.Max(0, cmbRotPcb.SelectedIndex) * 90;
            boardView.FlipY = btnFlipY.Checked;
        }

        private void btnFit_Click(object sender, EventArgs e)
        {
            boardView.FitToView();
        }

        private void cmbZoomBlock_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_init) return;
            int i = cmbZoomBlock.SelectedIndex;
            if (i == 0) boardView.FitToView();
            else if (i > 0) boardView.ZoomToBlock(i - 1);
        }

        private void btnMeasure_CheckedChanged(object sender, EventArgs e)
        {
            boardView.MeasureMode = btnMeasure.Checked;
            if (btnMeasure.Checked)
                lblMeasure.Text = "Mode ukur: klik titik A";
            else
                lblMeasure.Text = "";
        }

        private void cmbLabel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_init) return;
            boardView.LabelMode = (LabelMode)Math.Max(0, cmbLabel.SelectedIndex);
        }

        private void cmbColor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_init) return;
            boardView.ColorMode = (ColorMode)Math.Max(0, cmbColor.SelectedIndex);
        }

        private void cmbTheme_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_init) return;
            boardView.ThemeKind = (ThemeKind)Math.Max(0, cmbTheme.SelectedIndex);
        }

        private void btnGrid_CheckedChanged(object sender, EventArgs e)
        {
            if (_init) return;
            boardView.ShowGrid = btnGrid.Checked;
        }

        private void btnBlocks_CheckedChanged(object sender, EventArgs e)
        {
            if (_init) return;
            boardView.ShowBlocks = btnBlocks.Checked;
        }

        private void btnFid_CheckedChanged(object sender, EventArgs e)
        {
            if (_init) return;
            boardView.ShowFiducials = btnFid.Checked;
        }

        private void btnFlipY_CheckedChanged(object sender, EventArgs e)
        {
            if (_init) return;
            boardView.FlipY = btnFlipY.Checked;
        }

        private void btnMirror_CheckedChanged(object sender, EventArgs e)
        {
            if (_init) return;
            boardView.Mirror = btnMirror.Checked;
        }

        private void cmbRotPcb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_init) return;
            boardView.ViewRotation = Math.Max(0, cmbRotPcb.SelectedIndex) * 90;
        }

        private void cmbRotMat_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_init || _data == null) return;
            ApplyMaterialRotation(Math.Max(0, cmbRotMat.SelectedIndex) * 90);
        }

        /// <summary>
        /// Memutar SEMUA material sebesar deg derajat dari rotasi aslinya (R di file).
        /// Posisi tidak berubah; 0 = kembali ke data asli.
        /// </summary>
        private void ApplyMaterialRotation(int deg)
        {
            foreach (MountItem m in _data.Mounts)
            {
                if (deg == 0)
                    m.R = m.OriginalR;
                else
                    m.R = (((m.OriginalR + deg) % 360.0) + 360.0) % 360.0;
            }
            boardView.Invalidate();
            dgvMounts.Refresh();
            ShowDetail(boardView.Selected);
        }

        // =====================================================================
        //  Event dari kanvas
        // =====================================================================

        private void BoardView_MountClicked(MountItem m)
        {
            ShowDetail(m);
            SelectInGrid(m);
        }

        private void BoardView_CursorMoved(double x, double y)
        {
            lblCursor.Text = string.Format("X: {0:0.000}    Y: {1:0.000}", x, y);
        }

        private void BoardView_MeasureChanged(string text)
        {
            lblMeasure.Text = text;
        }

        private void BoardView_ZoomChanged(double scale)
        {
            lblZoom.Text = string.Format("Zoom: {0:0.0} px/mm", scale);
        }

        // =====================================================================
        //  Membuka file (satu atau banyak, digabung dalam satu kanvas)
        // =====================================================================

        private string[] PickFiles()
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "Pilih satu atau beberapa file Yamaha";
                dlg.Filter = "Yamaha file (*.ygx)|*.ygx|Semua file (*.*)|*.*";
                dlg.Multiselect = true;
                if (dlg.ShowDialog(this) == DialogResult.OK)
                    return dlg.FileNames;
            }
            return null;
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            string[] files = PickFiles();
            if (files != null) LoadFiles(files, false);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string[] files = PickFiles();
            if (files != null) LoadFiles(files, true);
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (files != null && files.Length > 0)
                LoadFiles(files, false);
        }

        private void LoadFiles(string[] paths, bool append)
        {
            YgxData merged = new YgxData();
            List<string> info = new List<string>();
            List<string> lines = new List<string>();
            int partTotal = 0;
            bool haveBoard = false;

            if (append && _data != null)
            {
                merged.Mounts.AddRange(_data.Mounts);
                merged.Fiducials.AddRange(_data.Fiducials);
                merged.Board.OriginX = _data.Board.OriginX;
                merged.Board.OriginY = _data.Board.OriginY;
                merged.Board.SizeX = _data.Board.SizeX;
                merged.Board.SizeY = _data.Board.SizeY;
                merged.SkippedMasters = _data.SkippedMasters;
                info.AddRange(_fileInfo);
                lines.AddRange(_fileLines);
                partTotal = _partTotal;
                haveBoard = true;
            }
            else
            {
                append = false;
            }

            StringBuilder errors = new StringBuilder();
            int loaded = 0;

            foreach (string path in paths)
            {
                try
                {
                    YgxData d = YgxParser.Load(path);
                    string name = Path.GetFileName(path);
                    foreach (MountItem m in d.Mounts) m.SourceFile = name;
                    merged.Mounts.AddRange(d.Mounts);

                    if (!haveBoard)
                    {
                        merged.Board.OriginX = d.Board.OriginX;
                        merged.Board.OriginY = d.Board.OriginY;
                        merged.Board.SizeX = d.Board.SizeX;
                        merged.Board.SizeY = d.Board.SizeY;
                        merged.Fiducials.AddRange(d.Fiducials);
                        haveBoard = true;
                    }
                    else
                    {
                        // Beda ukuran board antar file: pakai yang terbesar agar semua muat.
                        merged.Board.SizeX = Math.Max(merged.Board.SizeX, d.Board.SizeX);
                        merged.Board.SizeY = Math.Max(merged.Board.SizeY, d.Board.SizeY);
                    }

                    partTotal += d.Parts.Count;
                    merged.SkippedMasters += d.SkippedMasters;
                    info.Add(name);

                    string line = name + "  |  " + d.Mounts.Count + " mount";
                    if (d.SkippedMasters > 0) line += " (+" + d.SkippedMasters + " master dilewati)";
                    if (!string.IsNullOrEmpty(d.MachineVersion)) line += "  |  " + d.MachineVersion;
                    if (!string.IsNullOrEmpty(d.LastEdit)) line += "  |  edit " + d.LastEdit;
                    lines.Add(line);
                    loaded++;
                }
                catch (Exception ex)
                {
                    errors.AppendLine(Path.GetFileName(path) + " - " + ex.Message);
                }
            }

            if (errors.Length > 0)
                MessageBox.Show(this, "File berikut gagal dibaca:\n\n" + errors,
                    "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            if (loaded == 0) return;

            _data = merged;
            _partTotal = partTotal;
            _fileInfo = info;
            _fileLines = lines;
            _sortProp = null;
            foreach (DataGridViewColumn col in dgvMounts.Columns)
                col.HeaderCell.SortGlyphDirection = SortOrder.None;

            boardView.Data = _data;

            if (!append)
            {
                _init = true;
                cmbRotMat.SelectedIndex = 0;     // rotasi material kembali ke data asli file
                _init = false;
            }
            else
            {
                int deg = Math.Max(0, cmbRotMat.SelectedIndex) * 90;
                if (deg > 0) ApplyMaterialRotation(deg);
            }

            RebuildBlockCombos();
            BuildParts();
            ApplyFilter();
            ShowDetail(boardView.Selected);
            UpdateSummary();
            UpdateStatus();

            Text = info.Count == 1
                ? "Yamaha YGX PCB Viewer - " + info[0]
                : "Yamaha YGX PCB Viewer - " + info.Count + " file digabung";
        }

        private void RebuildBlockCombos()
        {
            _init = true;
            cmbZoomBlock.Items.Clear();
            cmbZoomBlock.Items.Add("Semua PCB");
            cmbBlockFilter.Items.Clear();
            cmbBlockFilter.Items.Add("Semua blok");
            foreach (BlockBox b in boardView.Blocks)
            {
                cmbZoomBlock.Items.Add(b.Label);
                cmbBlockFilter.Items.Add(b.Label);
            }
            cmbZoomBlock.SelectedIndex = 0;
            cmbBlockFilter.SelectedIndex = 0;
            _init = false;
        }

        private void BuildParts()
        {
            Dictionary<string, PartRow> map = new Dictionary<string, PartRow>();
            List<PartRow> rows = new List<PartRow>();

            foreach (MountItem m in _data.Mounts)
            {
                string key = (m.SourceFile ?? "") + "|" + m.Comp;
                PartRow pr;
                if (!map.TryGetValue(key, out pr))
                {
                    pr = new PartRow();
                    pr.Key = key;
                    pr.Comp = m.Comp;
                    pr.PartName = string.IsNullOrEmpty(m.PartName) ? "(data Part tidak ada)" : m.PartName;
                    pr.BodyText = m.BodyText;
                    pr.Kind = m.Kind;
                    pr.SampleRef = m.Comment;
                    map[key] = pr;
                    rows.Add(pr);
                }
                pr.Qty++;
                pr.Mounts.Add(m);
            }

            rows.Sort(delegate (PartRow a, PartRow b) { return b.Qty.CompareTo(a.Qty); });
            _partRows = rows;

            dgvParts.DataSource = null;
            dgvParts.DataSource = _partRows;
            dgvParts.ClearSelection();
        }

        // =====================================================================
        //  Filter, urutan, dan sinkron tabel <-> kanvas
        // =====================================================================

        private bool FilterActive()
        {
            return txtFilter.Text.Trim().Length > 0 || cmbBlockFilter.SelectedIndex > 0;
        }

        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            if (_init) return;
            ApplyFilter();
        }

        private void cmbBlockFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_init) return;
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            List<MountItem> list = new List<MountItem>();

            if (_data != null)
            {
                string[] terms = txtFilter.Text.Split(new char[] { ',', ';', ' ' },
                                                      StringSplitOptions.RemoveEmptyEntries);
                string blk = cmbBlockFilter.SelectedIndex > 0 ? cmbBlockFilter.SelectedItem as string : null;

                foreach (MountItem m in _data.Mounts)
                {
                    if (blk != null && boardView.BlockLabel(m) != blk) continue;

                    if (terms.Length > 0)
                    {
                        bool ok = false;
                        foreach (string t in terms)
                        {
                            if ((m.Comment != null && m.Comment.IndexOf(t, StringComparison.OrdinalIgnoreCase) >= 0) ||
                                (m.PartName != null && m.PartName.IndexOf(t, StringComparison.OrdinalIgnoreCase) >= 0))
                            {
                                ok = true;
                                break;
                            }
                        }
                        if (!ok) continue;
                    }
                    list.Add(m);
                }
            }

            _view = list;
            if (_sortProp != null) SortView();
            BindMounts();
            lblCount.Text = _view.Count + " / " + (_data == null ? 0 : _data.Mounts.Count);
            RefreshHighlight();
        }

        private void BindMounts()
        {
            _syncGrid = true;
            try
            {
                dgvMounts.DataSource = null;
                dgvMounts.DataSource = _view;
                dgvMounts.ClearSelection();
            }
            finally
            {
                _syncGrid = false;
            }
            SelectInGrid(boardView.Selected);
        }

        private void SelectInGrid(MountItem m)
        {
            _syncGrid = true;
            try
            {
                dgvMounts.ClearSelection();
                if (m != null)
                {
                    int idx = _view.IndexOf(m);
                    if (idx >= 0 && idx < dgvMounts.Rows.Count)
                    {
                        dgvMounts.CurrentCell = dgvMounts.Rows[idx].Cells[0];
                        dgvMounts.Rows[idx].Selected = true;
                        try { dgvMounts.FirstDisplayedScrollingRowIndex = idx; } catch { }
                    }
                }
            }
            finally
            {
                _syncGrid = false;
            }
        }

        private void dgvMounts_SelectionChanged(object sender, EventArgs e)
        {
            if (_syncGrid) return;

            MountItem m = null;
            if (dgvMounts.SelectedRows.Count > 0)
                m = dgvMounts.SelectedRows[0].DataBoundItem as MountItem;

            boardView.Selected = m;
            ShowDetail(m);
            if (m != null) boardView.EnsureVisible(m);
        }

        private void dgvMounts_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= _view.Count) return;
            boardView.ZoomToMount(_view[e.RowIndex]);
        }

        private void dgvMounts_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridViewColumn col = dgvMounts.Columns[e.ColumnIndex];
            string prop = col.DataPropertyName;
            if (_sortProp == prop)
                _sortAsc = !_sortAsc;
            else
            {
                _sortProp = prop;
                _sortAsc = true;
            }

            SortView();
            BindMounts();

            foreach (DataGridViewColumn c in dgvMounts.Columns)
                c.HeaderCell.SortGlyphDirection = SortOrder.None;
            col.HeaderCell.SortGlyphDirection = _sortAsc ? SortOrder.Ascending : SortOrder.Descending;
        }

        private void SortView()
        {
            System.Reflection.PropertyInfo pi = typeof(MountItem).GetProperty(_sortProp);
            if (pi == null) return;
            int dir = _sortAsc ? 1 : -1;

            _view.Sort(delegate (MountItem a, MountItem b)
            {
                object va = pi.GetValue(a, null);
                object vb = pi.GetValue(b, null);
                string sa = va as string;
                string sb = vb as string;
                int c;
                if (sa != null && sb != null)
                {
                    c = NatCmp(sa, sb);
                }
                else
                {
                    IComparable ca = va as IComparable;
                    c = (ca != null && vb != null) ? ca.CompareTo(vb) : 0;
                }
                return c * dir;
            });
        }

        /// <summary>Urutan alami: C2 &lt; C10.</summary>
        private static int NatCmp(string a, string b)
        {
            int i = 0, j = 0;
            while (i < a.Length && j < b.Length)
            {
                if (char.IsDigit(a[i]) && char.IsDigit(b[j]))
                {
                    int si = i;
                    while (i < a.Length && char.IsDigit(a[i])) i++;
                    int sj = j;
                    while (j < b.Length && char.IsDigit(b[j])) j++;

                    string da = a.Substring(si, i - si).TrimStart('0');
                    string db = b.Substring(sj, j - sj).TrimStart('0');
                    if (da.Length != db.Length) return da.Length.CompareTo(db.Length);
                    int cn = string.CompareOrdinal(da, db);
                    if (cn != 0) return cn;
                }
                else
                {
                    int cc = char.ToUpperInvariant(a[i]).CompareTo(char.ToUpperInvariant(b[j]));
                    if (cc != 0) return cc;
                    i++;
                    j++;
                }
            }
            return (a.Length - i).CompareTo(b.Length - j);
        }

        // =====================================================================
        //  Sorotan (highlight) di kanvas
        // =====================================================================

        private void tabMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshHighlight();
        }

        private void dgvParts_SelectionChanged(object sender, EventArgs e)
        {
            RefreshHighlight();
        }

        /// <summary>
        /// Tab "Ringkasan Part": semua komponen Part terpilih disorot.
        /// Tab lain: hasil filter disorot (jika filter aktif).
        /// </summary>
        private void RefreshHighlight()
        {
            if (_data == null)
            {
                boardView.Highlight = null;
                return;
            }

            if (tabMain.SelectedTab == tabParts && dgvParts.SelectedRows.Count > 0)
            {
                PartRow pr = dgvParts.SelectedRows[0].DataBoundItem as PartRow;
                boardView.Highlight = pr != null ? new HashSet<MountItem>(pr.Mounts) : null;
                if (pr != null)
                    lblInfo.Text = pr.PartName + "  |  " + pr.Qty + " komponen disorot  |  ukuran " + pr.BodyText + " mm";
                return;
            }

            boardView.Highlight = FilterActive() ? new HashSet<MountItem>(_view) : null;
            UpdateStatus();
        }

        // =====================================================================
        //  Panel detail, status, ringkasan
        // =====================================================================

        private void AddRow(string prop, string val)
        {
            lvDetail.Items.Add(new ListViewItem(new string[] { prop, val }));
        }

        private void ShowDetail(MountItem m)
        {
            lvDetail.BeginUpdate();
            lvDetail.Items.Clear();

            if (m == null)
            {
                lblRef.Text = "-";
                lblPartName.Text = "Klik komponen di gambar atau di tabel";
            }
            else
            {
                lblRef.Text = string.IsNullOrEmpty(m.Comment) ? "(tanpa nama)" : m.Comment;
                lblPartName.Text = string.IsNullOrEmpty(m.PartName)
                    ? "Part #" + m.Comp + "  (data Part tidak ditemukan)"
                    : m.PartName + "   (Part #" + m.Comp + ")";

                AddRow("Jenis", m.Kind);
                AddRow("Posisi X", m.X.ToString("0.000") + " mm");
                AddRow("Posisi Y", m.Y.ToString("0.000") + " mm");

                string rot = m.R.ToString("0.###") + " derajat";
                if (Math.Abs(m.R - m.OriginalR) > 0.0001)
                    rot += "   (asli " + m.OriginalR.ToString("0.###") + ")";
                AddRow("Rotasi (R)", rot);

                AddRow("Ukuran body",
                    m.HasBody
                        ? m.BodyX.ToString("0.###") + " x " + m.BodyY.ToString("0.###") + " mm  (X x Y)"
                        : "tidak ada data - digambar 1 x 1 mm");

                AddRow("Blok", m.BlockText);
                AddRow("Pola ulang (Repeat)", m.RepeatNo > 0 ? m.RepeatNo.ToString() : "-");
                AddRow("Head", m.Head.ToString());
                AddRow("Nozzle", m.Nozzle.ToString());
                AddRow("Group", m.Group.ToString());
                AddRow("Mount No", m.No.ToString());
                AddRow("Fid / Bad / Exec", m.Fid + " / " + m.Bad + " / " + m.Exec);
                AddRow("File sumber", m.SourceFile ?? "");
            }

            lvDetail.EndUpdate();
        }

        private void UpdateStatus()
        {
            if (_data == null) return;

            int noPart = 0;
            foreach (MountItem m in _data.Mounts)
                if (!m.HasBody) noPart++;

            string files = _fileInfo.Count <= 3 ? string.Join(", ", _fileInfo.ToArray()) : _fileInfo.Count + " file";
            int blocks = boardView.Blocks.Count;

            lblInfo.Text = string.Format("{0}  |  PCB {1:0.###} x {2:0.###} mm  |  {3} komponen{4}{5}",
                files, _data.Board.SizeX, _data.Board.SizeY, _data.Mounts.Count,
                blocks > 0 ? "  |  " + blocks + " blok" : "",
                noPart > 0 ? "  |  " + noPart + " tanpa ukuran" : "");
        }

        private void UpdateSummary()
        {
            StringBuilder sb = new StringBuilder();

            if (_data == null)
            {
                sb.AppendLine("Belum ada file dimuat.");
                sb.AppendLine();
            }
            else
            {
                sb.AppendLine("BERKAS");
                foreach (string s in _fileLines) sb.AppendLine("  " + s);
                sb.AppendLine();

                sb.AppendLine("PCB");
                sb.AppendLine(string.Format("  Ukuran       : {0:0.###} x {1:0.###} mm", _data.Board.SizeX, _data.Board.SizeY));
                sb.AppendLine("  Total Mount  : " + _data.Mounts.Count);
                sb.AppendLine("  Jenis Part   : " + _partTotal);
                if (_data.SkippedMasters > 0)
                    sb.AppendLine("  Master dilewati: " + _data.SkippedMasters + " (template blok 1, posisinya sama)");
                sb.AppendLine();

                if (boardView.Blocks.Count > 0)
                {
                    sb.AppendLine("BLOK  (R = pola ulang / baris, B = nomor blok)");
                    foreach (BlockBox b in boardView.Blocks)
                        sb.AppendLine(string.Format("  {0,-9}: {1} komponen", b.Label, b.Count));
                    sb.AppendLine();
                }

                Dictionary<string, int> kinds = new Dictionary<string, int>();
                Dictionary<int, int> missing = new Dictionary<int, int>();
                foreach (MountItem m in _data.Mounts)
                {
                    int n;
                    kinds.TryGetValue(m.Kind, out n);
                    kinds[m.Kind] = n + 1;

                    if (!m.HasBody)
                    {
                        int k;
                        missing.TryGetValue(m.Comp, out k);
                        missing[m.Comp] = k + 1;
                    }
                }

                List<KeyValuePair<string, int>> klist = new List<KeyValuePair<string, int>>(kinds);
                klist.Sort(delegate (KeyValuePair<string, int> a, KeyValuePair<string, int> b)
                {
                    return b.Value.CompareTo(a.Value);
                });
                sb.AppendLine("JENIS KOMPONEN (huruf awal reference)");
                foreach (KeyValuePair<string, int> kv in klist)
                    sb.AppendLine(string.Format("  {0,-6}: {1}", kv.Key, kv.Value));
                sb.AppendLine();

                if (missing.Count > 0)
                {
                    sb.AppendLine("PERINGATAN");
                    sb.AppendLine("  Mount tanpa ukuran Part (digambar 1 x 1 mm, garis tepi biasa):");
                    foreach (KeyValuePair<int, int> kv in missing)
                        sb.AppendLine("    Comp " + kv.Key + " : " + kv.Value + " komponen");
                    sb.AppendLine();
                }
            }

            sb.AppendLine("KONTROL");
            sb.AppendLine("  Scroll mouse      zoom ke posisi kursor");
            sb.AppendLine("  Drag kiri         geser tampilan");
            sb.AppendLine("  Klik              pilih komponen");
            sb.AppendLine("  Klik ganda        zoom ke komponen");
            sb.AppendLine("  Arahkan kursor    tooltip detail komponen");
            sb.AppendLine("  Ukur (Ctrl+M)     klik titik A lalu B; klik kanan = hapus");
            sb.AppendLine("  Ctrl+O            buka file");
            sb.AppendLine("  Ctrl+S            simpan gambar PNG");
            sb.AppendLine("  Ctrl+F            cari komponen / Part");
            sb.AppendLine("  Ctrl+G / B / L    grid / bingkai blok / mode label");
            sb.AppendLine("  Ctrl+0            pas ke layar");
            sb.AppendLine("  Esc               hapus pilihan / ukuran");
            sb.AppendLine();
            sb.AppendLine("TIPS");
            sb.AppendLine("  - Kolom Cari bisa banyak kata: C94 C98 IC11");
            sb.AppendLine("  - Tab Ringkasan Part: klik satu Part, semua");
            sb.AppendLine("    komponennya disorot di gambar.");
            sb.AppendLine("  - Warna per blok / Part / nozzle / jenis ada di toolbar.");
            sb.AppendLine("  - Kotak merah di gambar = komponen terpilih atau Fid.");

            txtSummary.Text = sb.ToString();
        }

        // =====================================================================
        //  Ekspor
        // =====================================================================

        private void btnPng_Click(object sender, EventArgs e)
        {
            if (_data == null)
            {
                MessageBox.Show(this, "Buka file .ygx terlebih dahulu.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Filter = "Gambar PNG (*.png)|*.png";
                dlg.FileName = "pcb_view.png";
                if (dlg.ShowDialog(this) != DialogResult.OK) return;

                try
                {
                    using (Bitmap bmp = boardView.RenderToBitmap(3200))
                    {
                        if (bmp == null) return;
                        bmp.Save(dlg.FileName, ImageFormat.Png);
                    }
                    MessageBox.Show(this, "Gambar tersimpan:\n" + dlg.FileName, "Selesai",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "Gagal menyimpan gambar:\n" + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private static string Csv(string s)
        {
            if (s == null) return "";
            if (s.IndexOfAny(new char[] { ',', '"', '\n', '\r' }) >= 0)
                return "\"" + s.Replace("\"", "\"\"") + "\"";
            return s;
        }

        private void btnCsv_Click(object sender, EventArgs e)
        {
            if (_data == null || _view.Count == 0)
            {
                MessageBox.Show(this, "Tidak ada data untuk diekspor.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Filter = "CSV (*.csv)|*.csv";
                dlg.FileName = "komponen.csv";
                if (dlg.ShowDialog(this) != DialogResult.OK) return;

                try
                {
                    CultureInfo inv = CultureInfo.InvariantCulture;
                    using (StreamWriter w = new StreamWriter(dlg.FileName, false, new UTF8Encoding(true)))
                    {
                        w.WriteLine("File,Blok,Ref,Part,Comp,X,Y,R,BodyX,BodyY,Head,Nozzle,Group");
                        foreach (MountItem m in _view)
                        {
                            w.WriteLine(string.Join(",", new string[]
                            {
                                Csv(m.SourceFile), Csv(m.BlockText), Csv(m.Comment), Csv(m.PartName),
                                m.Comp.ToString(inv),
                                m.X.ToString("0.000", inv), m.Y.ToString("0.000", inv), m.R.ToString("0.###", inv),
                                m.BodyX.ToString("0.###", inv), m.BodyY.ToString("0.###", inv),
                                m.Head.ToString(inv), m.Nozzle.ToString(inv), m.Group.ToString(inv)
                            }));
                        }
                    }
                    MessageBox.Show(this, _view.Count + " baris tersimpan:\n" + dlg.FileName, "Selesai",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "Gagal menyimpan CSV:\n" + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // =====================================================================
        //  Pintasan keyboard
        // =====================================================================

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Control | Keys.O:
                    btnOpen_Click(this, EventArgs.Empty);
                    return true;
                case Keys.Control | Keys.S:
                    btnPng_Click(this, EventArgs.Empty);
                    return true;
                case Keys.Control | Keys.F:
                    tabMain.SelectedTab = tabList;
                    txtFilter.Focus();
                    txtFilter.SelectAll();
                    return true;
                case Keys.Control | Keys.G:
                    btnGrid.Checked = !btnGrid.Checked;
                    return true;
                case Keys.Control | Keys.B:
                    btnBlocks.Checked = !btnBlocks.Checked;
                    return true;
                case Keys.Control | Keys.L:
                    cmbLabel.SelectedIndex = (cmbLabel.SelectedIndex + 1) % 3;
                    return true;
                case Keys.Control | Keys.M:
                    btnMeasure.Checked = !btnMeasure.Checked;
                    return true;
                case Keys.Control | Keys.D0:
                    boardView.FitToView();
                    return true;
                case Keys.Escape:
                    boardView.ClearMeasure();
                    boardView.Selected = null;
                    dgvMounts.ClearSelection();
                    ShowDetail(null);
                    return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}