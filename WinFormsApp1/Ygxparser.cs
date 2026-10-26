using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace YgxViewer
{
    public static class YgxParser
    {
        // Pola tahan terhadap tanda ">" di dalam nilai atribut.
        private const string AttrBody = "((?:[^>\"]|\"[^\"]*\")*?)/?>";

        private static readonly Regex BoardTag = MakeTag("Board_000");
        private static readonly Regex MountTag = MakeTag("Mount");
        private static readonly Regex RepeatTag = MakeTag("Repeat");
        private static readonly Regex PcbFidTag = MakeTag("PcbFid");
        private static readonly Regex MachineTag = MakeTag("Machine");
        private static readonly Regex EditTag = MakeTag("LastEditing");
        private static readonly Regex Part001Tag = MakeTag("Part_001");
        private static readonly Regex Part013Tag = MakeTag("Part_013");

        private static readonly Regex AttrPair = new Regex("([\\w:\\-]+)\\s*=\\s*\"([^\"]*)\"");

        private static readonly Regex PartBlock = new Regex(
            "<Part\\s+No\\s*=\\s*\"\\s*(\\d+)\\s*\"[^>]*>(.*?)</Part>",
            RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private static readonly Regex BodyXAttr = new Regex("\\bBodyX\\s*=\\s*\"([^\"]*)\"", RegexOptions.IgnoreCase);
        private static readonly Regex BodyYAttr = new Regex("\\bBodyY\\s*=\\s*\"([^\"]*)\"", RegexOptions.IgnoreCase);

        private class Rep
        {
            public int No;
            public double X, Y, R;
            public int OrgBlk;
        }

        private static Regex MakeTag(string name)
        {
            return new Regex("<" + name + "\\s+" + AttrBody,
                RegexOptions.IgnoreCase | RegexOptions.Singleline);
        }

        public static YgxData Load(string path)
        {
            string text = File.ReadAllText(path);
            YgxData data = new YgxData();

            // ---- Info mesin ----
            Match mm = MachineTag.Match(text);
            if (mm.Success)
            {
                Dictionary<string, string> am = ParseAttrs(mm.Groups[1].Value);
                string ver;
                if (am.TryGetValue("Version", out ver)) data.MachineVersion = ver.Trim();
            }
            Match me = EditTag.Match(text);
            if (me.Success)
            {
                Dictionary<string, string> ae = ParseAttrs(me.Groups[1].Value);
                string d1, t1;
                ae.TryGetValue("Date", out d1);
                ae.TryGetValue("Time", out t1);
                data.LastEdit = ((d1 ?? "") + " " + (t1 ?? "")).Trim();
            }

            // ---- Board ----
            Match bm = BoardTag.Match(text);
            if (bm.Success)
            {
                Dictionary<string, string> ab = ParseAttrs(bm.Groups[1].Value);
                data.Board.OriginX = GetD(ab, "OriginX");
                data.Board.OriginY = GetD(ab, "OriginY");
                data.Board.SizeX = GetD(ab, "SizeX");
                data.Board.SizeY = GetD(ab, "SizeY");
            }

            // ---- Fiducial PCB ----
            Match fm = PcbFidTag.Match(text);
            if (fm.Success)
            {
                Dictionary<string, string> af = ParseAttrs(fm.Groups[1].Value);
                if (GetI(af, "Mark1") > 0 || GetD(af, "X1") != 0 || GetD(af, "Y1") != 0)
                {
                    FiducialInfo f1 = new FiducialInfo();
                    f1.Label = "FID1";
                    f1.X = GetD(af, "X1");
                    f1.Y = GetD(af, "Y1");
                    data.Fiducials.Add(f1);
                }
                if (GetI(af, "Mark2") > 0 || GetD(af, "X2") != 0 || GetD(af, "Y2") != 0)
                {
                    FiducialInfo f2 = new FiducialInfo();
                    f2.Label = "FID2";
                    f2.X = GetD(af, "X2");
                    f2.Y = GetD(af, "Y2");
                    data.Fiducials.Add(f2);
                }
            }

            // ---- Parts (ukuran aktual material) ----
            foreach (Match pm in PartBlock.Matches(text))
            {
                int no;
                if (!int.TryParse(pm.Groups[1].Value, out no)) continue;
                string body = pm.Groups[2].Value;

                PartInfo pi = new PartInfo();
                Match m13 = Part013Tag.Match(body);
                if (m13.Success)
                {
                    Dictionary<string, string> a13 = ParseAttrs(m13.Groups[1].Value);
                    pi.BodyX = GetD(a13, "BodyX");
                    pi.BodyY = GetD(a13, "BodyY");
                }
                // BodyX/BodyY bisa ada di Part_012, Part_013, Part_015, Part_016, dst.
                if (pi.BodyX <= 0)
                {
                    Match mx = BodyXAttr.Match(body);
                    if (mx.Success) pi.BodyX = ParseNum(mx.Groups[1].Value);
                }
                if (pi.BodyY <= 0)
                {
                    Match my = BodyYAttr.Match(body);
                    if (my.Success) pi.BodyY = ParseNum(my.Groups[1].Value);
                }
                Match m01 = Part001Tag.Match(body);
                if (m01.Success)
                {
                    Dictionary<string, string> a01 = ParseAttrs(m01.Groups[1].Value);
                    string nm;
                    if (a01.TryGetValue("PartsName", out nm)) pi.Name = WebUtility.HtmlDecode(nm).Trim();
                }

                PartInfo prev;
                if (data.Parts.TryGetValue(no, out prev))
                {
                    if (pi.BodyX <= 0 || pi.BodyY <= 0)
                    {
                        pi.BodyX = prev.BodyX;
                        pi.BodyY = prev.BodyY;
                    }
                    if (string.IsNullOrEmpty(pi.Name)) pi.Name = prev.Name;
                }
                data.Parts[no] = pi;
            }

            // ---- Mounts ----
            foreach (Match m in MountTag.Matches(text))
            {
                Dictionary<string, string> a = ParseAttrs(m.Groups[1].Value);
                if (!a.ContainsKey("X") || !a.ContainsKey("Y")) continue;

                MountItem it = new MountItem();
                it.No = GetI(a, "No");
                it.X = GetD(a, "X");
                it.Y = GetD(a, "Y");
                it.R = GetD(a, "R");
                it.OriginalR = it.R;
                it.Fid = GetI(a, "Fid");
                it.Bad = GetI(a, "Bad");
                it.Comp = GetI(a, "Comp");
                it.Group = GetI(a, "Group");
                it.Head = GetI(a, "Head");
                it.Nozzle = GetI(a, "Nozzle");
                it.OrgBlk = GetI(a, "OrgBlk");
                it.Exec = GetI(a, "Exec");
                string c;
                it.Comment = a.TryGetValue("Comment", out c) ? WebUtility.HtmlDecode(c).Trim() : "";

                PartInfo part;
                if (data.Parts.TryGetValue(it.Comp, out part))
                {
                    it.BodyX = part.BodyX;
                    it.BodyY = part.BodyY;
                    it.PartName = part.Name;
                }
                data.Mounts.Add(it);
            }

            if (data.Mounts.Count == 0)
                throw new InvalidDataException("Tidak ditemukan tag <Mount> pada file ini.");

            // ---- Offset / Repeat ----
            // Repeat dengan OrgBlk > 0 = offset per BLOK (koordinat Mount di file sudah memuatnya).
            // Repeat dengan OrgBlk = 0 = pengulangan seluruh pola (mis. baris bawah dan baris atas).
            List<Rep> patterns = new List<Rep>();
            bool hasBlockRepeats = false;
            foreach (Match rm in RepeatTag.Matches(text))
            {
                Dictionary<string, string> ra = ParseAttrs(rm.Groups[1].Value);
                Rep rp = new Rep();
                rp.No = GetI(ra, "No");
                rp.X = GetD(ra, "X");
                rp.Y = GetD(ra, "Y");
                rp.R = GetD(ra, "R");
                rp.OrgBlk = GetI(ra, "OrgBlk");
                if (rp.OrgBlk > 0) hasBlockRepeats = true;
                else patterns.Add(rp);
            }

            // Data master (OrgBlk = 0) hanya template blok 1 dan posisinya sama dengan blok 1.
            if (hasBlockRepeats)
            {
                List<MountItem> kept = new List<MountItem>();
                foreach (MountItem mi in data.Mounts)
                    if (mi.OrgBlk > 0) kept.Add(mi);

                if (kept.Count > 0)
                {
                    data.SkippedMasters = data.Mounts.Count - kept.Count;
                    data.Mounts = kept;
                }
            }

            if (patterns.Count > 0)
            {
                bool identityOnly = patterns.Count == 1 &&
                                    patterns[0].X == 0 && patterns[0].Y == 0 && patterns[0].R == 0;
                if (!identityOnly)
                {
                    List<MountItem> expanded = new List<MountItem>();
                    foreach (Rep pat in patterns)
                    {
                        double rad = pat.R * Math.PI / 180.0;
                        double cs = Math.Cos(rad);
                        double sn = Math.Sin(rad);
                        foreach (MountItem src in data.Mounts)
                        {
                            MountItem cp = src.Clone();
                            cp.X = src.X * cs - src.Y * sn + pat.X;
                            cp.Y = src.X * sn + src.Y * cs + pat.Y;
                            cp.R = src.R + pat.R;
                            cp.OriginalR = cp.R;
                            cp.RepeatNo = pat.No;
                            expanded.Add(cp);
                        }
                    }
                    data.Mounts = expanded;
                }
            }

            // Jika ukuran board tidak ada / tidak cukup, sesuaikan dengan batas data Mount.
            double maxX = 0, maxY = 0;
            foreach (MountItem it2 in data.Mounts)
            {
                if (it2.X > maxX) maxX = it2.X;
                if (it2.Y > maxY) maxY = it2.Y;
            }
            if (data.Board.SizeX <= 0 || maxX + 2 > data.Board.SizeX) data.Board.SizeX = maxX + 5;
            if (data.Board.SizeY <= 0 || maxY + 2 > data.Board.SizeY) data.Board.SizeY = maxY + 5;

            return data;
        }

        private static Dictionary<string, string> ParseAttrs(string s)
        {
            Dictionary<string, string> d = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (Match m in AttrPair.Matches(s))
                d[m.Groups[1].Value] = m.Groups[2].Value;
            return d;
        }

        private static double ParseNum(string v)
        {
            double r;
            if (double.TryParse(v.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out r))
                return r;
            return 0;
        }

        private static double GetD(Dictionary<string, string> a, string key)
        {
            string v;
            return a.TryGetValue(key, out v) ? ParseNum(v) : 0;
        }

        private static int GetI(Dictionary<string, string> a, string key)
        {
            return (int)Math.Round(GetD(a, key));
        }
    }
}