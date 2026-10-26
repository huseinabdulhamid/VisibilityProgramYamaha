using System;
using System.Collections.Generic;

namespace YgxViewer
{
    /// <summary>Ukuran PCB dari tag Board_000.</summary>
    public class BoardInfo
    {
        public double OriginX { get; set; }
        public double OriginY { get; set; }
        public double SizeX { get; set; }
        public double SizeY { get; set; }
    }

    /// <summary>Fiducial PCB (tag PcbFid).</summary>
    public class FiducialInfo
    {
        public string Label = "";
        public double X;
        public double Y;
    }

    /// <summary>Data material dari tag Part (PartsName + BodyX/BodyY).</summary>
    public class PartInfo
    {
        public string Name = "";
        public double BodyX;
        public double BodyY;
    }

    /// <summary>Satu komponen (tag Mount) beserta data Part-nya.</summary>
    public class MountItem
    {
        public string Comment { get; set; }      // Reference designator (mis. C297)
        public string PartName { get; set; }
        public int Comp { get; set; }            // nomor Part
        public double X { get; set; }
        public double Y { get; set; }
        public double R { get; set; }
        public double OriginalR { get; set; }
        public double BodyX { get; set; }
        public double BodyY { get; set; }
        public int Head { get; set; }
        public int Nozzle { get; set; }
        public int Group { get; set; }
        public int Fid { get; set; }
        public int Bad { get; set; }
        public int No { get; set; }
        public int OrgBlk { get; set; }
        public int Exec { get; set; }
        public int RepeatNo { get; set; }
        public string SourceFile { get; set; }

        public bool HasBody
        {
            get { return BodyX > 0 && BodyY > 0; }
        }

        public string BodyText
        {
            get { return HasBody ? string.Format("{0:0.###} x {1:0.###}", BodyX, BodyY) : "-"; }
        }

        /// <summary>Contoh: B1 (satu pola) atau R2-B3 (pola ulang ke-2, blok 3).</summary>
        public string BlockText
        {
            get
            {
                if (OrgBlk <= 0) return "-";
                return (RepeatNo > 0 ? "R" + RepeatNo + "-" : "") + "B" + OrgBlk;
            }
        }

        /// <summary>Huruf awal reference: C, R, IC, T, D, OSC, L ...</summary>
        public string Kind
        {
            get
            {
                if (string.IsNullOrEmpty(Comment)) return "?";
                int i = 0;
                while (i < Comment.Length && char.IsLetter(Comment[i])) i++;
                return i == 0 ? "?" : Comment.Substring(0, i).ToUpperInvariant();
            }
        }

        public MountItem Clone()
        {
            return (MountItem)MemberwiseClone();
        }
    }

    /// <summary>Baris ringkasan Part (jumlah komponen per jenis Part).</summary>
    public class PartRow
    {
        public string Key = "";
        public string PartName { get; set; }
        public int Comp { get; set; }
        public int Qty { get; set; }
        public string BodyText { get; set; }
        public string Kind { get; set; }
        public string SampleRef { get; set; }
        public List<MountItem> Mounts = new List<MountItem>();
    }

    /// <summary>Hasil baca satu file .ygx.</summary>
    public class YgxData
    {
        public BoardInfo Board = new BoardInfo();
        public List<MountItem> Mounts = new List<MountItem>();
        public Dictionary<int, PartInfo> Parts = new Dictionary<int, PartInfo>();
        public List<FiducialInfo> Fiducials = new List<FiducialInfo>();
        public int SkippedMasters;
        public string MachineVersion = "";
        public string LastEdit = "";
    }
}