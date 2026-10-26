namespace YgxViewer
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.toolStripMain = new System.Windows.Forms.ToolStrip();
            this.btnOpen = new System.Windows.Forms.ToolStripButton();
            this.btnAdd = new System.Windows.Forms.ToolStripButton();
            this.sep1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnFit = new System.Windows.Forms.ToolStripButton();
            this.tslZoom = new System.Windows.Forms.ToolStripLabel();
            this.cmbZoomBlock = new System.Windows.Forms.ToolStripComboBox();
            this.btnMeasure = new System.Windows.Forms.ToolStripButton();
            this.sep2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnPng = new System.Windows.Forms.ToolStripButton();
            this.btnCsv = new System.Windows.Forms.ToolStripButton();
            this.sep3 = new System.Windows.Forms.ToolStripSeparator();
            this.tslLabel = new System.Windows.Forms.ToolStripLabel();
            this.cmbLabel = new System.Windows.Forms.ToolStripComboBox();
            this.tslColor = new System.Windows.Forms.ToolStripLabel();
            this.cmbColor = new System.Windows.Forms.ToolStripComboBox();
            this.tslTheme = new System.Windows.Forms.ToolStripLabel();
            this.cmbTheme = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripView = new System.Windows.Forms.ToolStrip();
            this.btnGrid = new System.Windows.Forms.ToolStripButton();
            this.btnBlocks = new System.Windows.Forms.ToolStripButton();
            this.btnFid = new System.Windows.Forms.ToolStripButton();
            this.btnFlipY = new System.Windows.Forms.ToolStripButton();
            this.btnMirror = new System.Windows.Forms.ToolStripButton();
            this.sep4 = new System.Windows.Forms.ToolStripSeparator();
            this.tslRotPcb = new System.Windows.Forms.ToolStripLabel();
            this.cmbRotPcb = new System.Windows.Forms.ToolStripComboBox();
            this.tslRotMat = new System.Windows.Forms.ToolStripLabel();
            this.cmbRotMat = new System.Windows.Forms.ToolStripComboBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblInfo = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblMeasure = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblZoom = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblCursor = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitMain = new System.Windows.Forms.SplitContainer();
            this.boardView = new YgxViewer.BoardView();
            this.tabMain = new System.Windows.Forms.TabControl();
            this.tabList = new System.Windows.Forms.TabPage();
            this.dgvMounts = new System.Windows.Forms.DataGridView();
            this.pnlFilter = new System.Windows.Forms.Panel();
            this.lblCount = new System.Windows.Forms.Label();
            this.cmbBlockFilter = new System.Windows.Forms.ComboBox();
            this.txtFilter = new System.Windows.Forms.TextBox();
            this.lblFilter = new System.Windows.Forms.Label();
            this.tabParts = new System.Windows.Forms.TabPage();
            this.dgvParts = new System.Windows.Forms.DataGridView();
            this.tabSummary = new System.Windows.Forms.TabPage();
            this.txtSummary = new System.Windows.Forms.TextBox();
            this.pnlDetail = new System.Windows.Forms.Panel();
            this.lvDetail = new System.Windows.Forms.ListView();
            this.colProp = new System.Windows.Forms.ColumnHeader();
            this.colVal = new System.Windows.Forms.ColumnHeader();
            this.pnlDetailHeader = new System.Windows.Forms.Panel();
            this.lblPartName = new System.Windows.Forms.Label();
            this.lblRef = new System.Windows.Forms.Label();
            this.toolStripMain.SuspendLayout();
            this.toolStripView.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            this.tabMain.SuspendLayout();
            this.tabList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMounts)).BeginInit();
            this.pnlFilter.SuspendLayout();
            this.tabParts.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvParts)).BeginInit();
            this.tabSummary.SuspendLayout();
            this.pnlDetail.SuspendLayout();
            this.pnlDetailHeader.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripMain
            // 
            this.toolStripMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnOpen,
            this.btnAdd,
            this.sep1,
            this.btnFit,
            this.tslZoom,
            this.cmbZoomBlock,
            this.btnMeasure,
            this.sep2,
            this.btnPng,
            this.btnCsv,
            this.sep3,
            this.tslLabel,
            this.cmbLabel,
            this.tslColor,
            this.cmbColor,
            this.tslTheme,
            this.cmbTheme});
            this.toolStripMain.Location = new System.Drawing.Point(0, 0);
            this.toolStripMain.Name = "toolStripMain";
            this.toolStripMain.Padding = new System.Windows.Forms.Padding(6, 2, 6, 2);
            this.toolStripMain.Size = new System.Drawing.Size(1360, 29);
            this.toolStripMain.TabIndex = 3;
            // 
            // btnOpen
            // 
            this.btnOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(58, 22);
            this.btnOpen.Text = "Buka...";
            this.btnOpen.ToolTipText = "Buka satu atau beberapa file .ygx (Ctrl+O)";
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(66, 22);
            this.btnAdd.Text = "Tambah...";
            this.btnAdd.ToolTipText = "Tambahkan file .ygx ke data yang sudah tampil";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // sep1
            // 
            this.sep1.Name = "sep1";
            this.sep1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnFit
            // 
            this.btnFit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnFit.Name = "btnFit";
            this.btnFit.Size = new System.Drawing.Size(27, 22);
            this.btnFit.Text = "Fit";
            this.btnFit.ToolTipText = "Pas ke layar (Ctrl+0)";
            this.btnFit.Click += new System.EventHandler(this.btnFit_Click);
            // 
            // tslZoom
            // 
            this.tslZoom.Name = "tslZoom";
            this.tslZoom.Size = new System.Drawing.Size(50, 22);
            this.tslZoom.Text = "Zoom ke:";
            // 
            // cmbZoomBlock
            // 
            this.cmbZoomBlock.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbZoomBlock.Name = "cmbZoomBlock";
            this.cmbZoomBlock.Size = new System.Drawing.Size(100, 25);
            this.cmbZoomBlock.ToolTipText = "Zoom ke salah satu blok";
            this.cmbZoomBlock.SelectedIndexChanged += new System.EventHandler(this.cmbZoomBlock_SelectedIndexChanged);
            // 
            // btnMeasure
            // 
            this.btnMeasure.CheckOnClick = true;
            this.btnMeasure.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnMeasure.Name = "btnMeasure";
            this.btnMeasure.Size = new System.Drawing.Size(40, 22);
            this.btnMeasure.Text = "Ukur";
            this.btnMeasure.ToolTipText = "Ukur jarak: klik titik A lalu titik B (klik kanan = hapus) (Ctrl+M)";
            this.btnMeasure.CheckedChanged += new System.EventHandler(this.btnMeasure_CheckedChanged);
            // 
            // sep2
            // 
            this.sep2.Name = "sep2";
            this.sep2.Size = new System.Drawing.Size(6, 25);
            // 
            // btnPng
            // 
            this.btnPng.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnPng.Name = "btnPng";
            this.btnPng.Size = new System.Drawing.Size(74, 22);
            this.btnPng.Text = "Simpan PNG";
            this.btnPng.ToolTipText = "Simpan gambar PCB beresolusi tinggi (Ctrl+S)";
            this.btnPng.Click += new System.EventHandler(this.btnPng_Click);
            // 
            // btnCsv
            // 
            this.btnCsv.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnCsv.Name = "btnCsv";
            this.btnCsv.Size = new System.Drawing.Size(72, 22);
            this.btnCsv.Text = "Ekspor CSV";
            this.btnCsv.ToolTipText = "Ekspor daftar komponen (sesuai filter) ke CSV";
            this.btnCsv.Click += new System.EventHandler(this.btnCsv_Click);
            // 
            // sep3
            // 
            this.sep3.Name = "sep3";
            this.sep3.Size = new System.Drawing.Size(6, 25);
            // 
            // tslLabel
            // 
            this.tslLabel.Name = "tslLabel";
            this.tslLabel.Size = new System.Drawing.Size(37, 22);
            this.tslLabel.Text = "Label:";
            // 
            // cmbLabel
            // 
            this.cmbLabel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLabel.Items.AddRange(new object[] {
            "Otomatis",
            "Semua",
            "Tidak ada"});
            this.cmbLabel.Name = "cmbLabel";
            this.cmbLabel.Size = new System.Drawing.Size(85, 25);
            this.cmbLabel.ToolTipText = "Otomatis = label muncul saat di-zoom (Ctrl+L)";
            this.cmbLabel.SelectedIndexChanged += new System.EventHandler(this.cmbLabel_SelectedIndexChanged);
            // 
            // tslColor
            // 
            this.tslColor.Name = "tslColor";
            this.tslColor.Size = new System.Drawing.Size(40, 22);
            this.tslColor.Text = "Warna:";
            // 
            // cmbColor
            // 
            this.cmbColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbColor.Items.AddRange(new object[] {
            "Polos",
            "Per blok",
            "Per Part",
            "Per nozzle",
            "Per jenis (C/R/IC...)"});
            this.cmbColor.Name = "cmbColor";
            this.cmbColor.Size = new System.Drawing.Size(130, 25);
            this.cmbColor.SelectedIndexChanged += new System.EventHandler(this.cmbColor_SelectedIndexChanged);
            // 
            // tslTheme
            // 
            this.tslTheme.Name = "tslTheme";
            this.tslTheme.Size = new System.Drawing.Size(37, 22);
            this.tslTheme.Text = "Tema:";
            // 
            // cmbTheme
            // 
            this.cmbTheme.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTheme.Items.AddRange(new object[] {
            "Klasik (putih)",
            "PCB hijau",
            "Gelap"});
            this.cmbTheme.Name = "cmbTheme";
            this.cmbTheme.Size = new System.Drawing.Size(100, 25);
            this.cmbTheme.SelectedIndexChanged += new System.EventHandler(this.cmbTheme_SelectedIndexChanged);
            // 
            // toolStripView
            // 
            this.toolStripView.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripView.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnGrid,
            this.btnBlocks,
            this.btnFid,
            this.btnFlipY,
            this.btnMirror,
            this.sep4,
            this.tslRotPcb,
            this.cmbRotPcb,
            this.tslRotMat,
            this.cmbRotMat});
            this.toolStripView.Location = new System.Drawing.Point(0, 29);
            this.toolStripView.Name = "toolStripView";
            this.toolStripView.Padding = new System.Windows.Forms.Padding(6, 2, 6, 2);
            this.toolStripView.Size = new System.Drawing.Size(1360, 29);
            this.toolStripView.TabIndex = 2;
            // 
            // btnGrid
            // 
            this.btnGrid.Checked = true;
            this.btnGrid.CheckOnClick = true;
            this.btnGrid.CheckState = System.Windows.Forms.CheckState.Checked;
            this.btnGrid.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnGrid.Name = "btnGrid";
            this.btnGrid.Size = new System.Drawing.Size(34, 22);
            this.btnGrid.Text = "Grid";
            this.btnGrid.ToolTipText = "Garis grid dan skala mm (Ctrl+G)";
            this.btnGrid.CheckedChanged += new System.EventHandler(this.btnGrid_CheckedChanged);
            // 
            // btnBlocks
            // 
            this.btnBlocks.Checked = true;
            this.btnBlocks.CheckOnClick = true;
            this.btnBlocks.CheckState = System.Windows.Forms.CheckState.Checked;
            this.btnBlocks.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnBlocks.Name = "btnBlocks";
            this.btnBlocks.Size = new System.Drawing.Size(65, 22);
            this.btnBlocks.Text = "Bingkai blok";
            this.btnBlocks.ToolTipText = "Bingkai putus-putus dan nama tiap blok (Ctrl+B)";
            this.btnBlocks.CheckedChanged += new System.EventHandler(this.btnBlocks_CheckedChanged);
            // 
            // btnFid
            // 
            this.btnFid.Checked = true;
            this.btnFid.CheckOnClick = true;
            this.btnFid.CheckState = System.Windows.Forms.CheckState.Checked;
            this.btnFid.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnFid.Name = "btnFid";
            this.btnFid.Size = new System.Drawing.Size(51, 22);
            this.btnFid.Text = "Fiducial";
            this.btnFid.ToolTipText = "Tanda fiducial PCB";
            this.btnFid.CheckedChanged += new System.EventHandler(this.btnFid_CheckedChanged);
            // 
            // btnFlipY
            // 
            this.btnFlipY.Checked = true;
            this.btnFlipY.CheckOnClick = true;
            this.btnFlipY.CheckState = System.Windows.Forms.CheckState.Checked;
            this.btnFlipY.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnFlipY.Name = "btnFlipY";
            this.btnFlipY.Size = new System.Drawing.Size(106, 22);
            this.btnFlipY.Text = "Origin kiri-bawah";
            this.btnFlipY.ToolTipText = "Aktif: Y naik ke atas (seperti PCB). Nonaktif: Y turun ke bawah";
            this.btnFlipY.CheckedChanged += new System.EventHandler(this.btnFlipY_CheckedChanged);
            // 
            // btnMirror
            // 
            this.btnMirror.CheckOnClick = true;
            this.btnMirror.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnMirror.Name = "btnMirror";
            this.btnMirror.Size = new System.Drawing.Size(45, 22);
            this.btnMirror.Text = "Mirror";
            this.btnMirror.ToolTipText = "Cermin horizontal (melihat dari sisi seberang)";
            this.btnMirror.CheckedChanged += new System.EventHandler(this.btnMirror_CheckedChanged);
            // 
            // sep4
            // 
            this.sep4.Name = "sep4";
            this.sep4.Size = new System.Drawing.Size(6, 25);
            // 
            // tslRotPcb
            // 
            this.tslRotPcb.Name = "tslRotPcb";
            this.tslRotPcb.Size = new System.Drawing.Size(70, 22);
            this.tslRotPcb.Text = "Rotasi PCB:";
            // 
            // cmbRotPcb
            // 
            this.cmbRotPcb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRotPcb.Items.AddRange(new object[] {
            "0",
            "90",
            "180",
            "270"});
            this.cmbRotPcb.Name = "cmbRotPcb";
            this.cmbRotPcb.Size = new System.Drawing.Size(55, 25);
            this.cmbRotPcb.ToolTipText = "Memutar seluruh tampilan PCB (searah jarum jam)";
            this.cmbRotPcb.SelectedIndexChanged += new System.EventHandler(this.cmbRotPcb_SelectedIndexChanged);
            // 
            // tslRotMat
            // 
            this.tslRotMat.Name = "tslRotMat";
            this.tslRotMat.Size = new System.Drawing.Size(88, 22);
            this.tslRotMat.Text = "Rotasi material:";
            // 
            // cmbRotMat
            // 
            this.cmbRotMat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRotMat.Items.AddRange(new object[] {
            "0",
            "90",
            "180",
            "270"});
            this.cmbRotMat.Name = "cmbRotMat";
            this.cmbRotMat.Size = new System.Drawing.Size(55, 25);
            this.cmbRotMat.ToolTipText = "Menambah rotasi semua material dari nilai R di file (0 = asli)";
            this.cmbRotMat.SelectedIndexChanged += new System.EventHandler(this.cmbRotMat_SelectedIndexChanged);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblInfo,
            this.lblMeasure,
            this.lblZoom,
            this.lblCursor});
            this.statusStrip1.Location = new System.Drawing.Point(0, 778);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1360, 22);
            this.statusStrip1.TabIndex = 1;
            // 
            // lblInfo
            // 
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(900, 17);
            this.lblInfo.Spring = true;
            this.lblInfo.Text = "Belum ada file. Klik Buka... atau seret file .ygx ke jendela ini.";
            this.lblInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblMeasure
            // 
            this.lblMeasure.Name = "lblMeasure";
            this.lblMeasure.Size = new System.Drawing.Size(0, 17);
            // 
            // lblZoom
            // 
            this.lblZoom.Name = "lblZoom";
            this.lblZoom.Size = new System.Drawing.Size(90, 17);
            this.lblZoom.Text = "Zoom: -";
            // 
            // lblCursor
            // 
            this.lblCursor.Name = "lblCursor";
            this.lblCursor.Size = new System.Drawing.Size(150, 17);
            this.lblCursor.Text = "X: -    Y: -";
            // 
            // splitMain
            // 
            this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitMain.Location = new System.Drawing.Point(0, 58);
            this.splitMain.Name = "splitMain";
            // 
            // splitMain.Panel1
            // 
            this.splitMain.Panel1.Controls.Add(this.boardView);
            // 
            // splitMain.Panel2
            // 
            this.splitMain.Panel2.Controls.Add(this.tabMain);
            this.splitMain.Panel2.Controls.Add(this.pnlDetail);
            this.splitMain.Size = new System.Drawing.Size(1360, 720);
            this.splitMain.SplitterDistance = 890;
            this.splitMain.TabIndex = 0;
            // 
            // boardView
            // 
            this.boardView.BackColor = System.Drawing.Color.Black;
            this.boardView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.boardView.Location = new System.Drawing.Point(0, 0);
            this.boardView.Name = "boardView";
            this.boardView.Size = new System.Drawing.Size(890, 720);
            this.boardView.TabIndex = 0;
            // 
            // tabMain
            // 
            this.tabMain.Controls.Add(this.tabList);
            this.tabMain.Controls.Add(this.tabParts);
            this.tabMain.Controls.Add(this.tabSummary);
            this.tabMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabMain.Location = new System.Drawing.Point(0, 276);
            this.tabMain.Name = "tabMain";
            this.tabMain.SelectedIndex = 0;
            this.tabMain.Size = new System.Drawing.Size(466, 444);
            this.tabMain.TabIndex = 1;
            this.tabMain.SelectedIndexChanged += new System.EventHandler(this.tabMain_SelectedIndexChanged);
            // 
            // tabList
            // 
            this.tabList.Controls.Add(this.dgvMounts);
            this.tabList.Controls.Add(this.pnlFilter);
            this.tabList.Location = new System.Drawing.Point(4, 22);
            this.tabList.Name = "tabList";
            this.tabList.Size = new System.Drawing.Size(458, 418);
            this.tabList.TabIndex = 0;
            this.tabList.Text = "Daftar komponen";
            this.tabList.UseVisualStyleBackColor = true;
            // 
            // dgvMounts
            // 
            this.dgvMounts.AllowUserToAddRows = false;
            this.dgvMounts.AllowUserToDeleteRows = false;
            this.dgvMounts.AllowUserToResizeRows = false;
            this.dgvMounts.BackgroundColor = System.Drawing.Color.White;
            this.dgvMounts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMounts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvMounts.Location = new System.Drawing.Point(0, 38);
            this.dgvMounts.MultiSelect = false;
            this.dgvMounts.Name = "dgvMounts";
            this.dgvMounts.ReadOnly = true;
            this.dgvMounts.RowHeadersVisible = false;
            this.dgvMounts.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvMounts.Size = new System.Drawing.Size(458, 380);
            this.dgvMounts.TabIndex = 1;
            this.dgvMounts.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvMounts_CellDoubleClick);
            this.dgvMounts.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvMounts_ColumnHeaderMouseClick);
            this.dgvMounts.SelectionChanged += new System.EventHandler(this.dgvMounts_SelectionChanged);
            // 
            // pnlFilter
            // 
            this.pnlFilter.Controls.Add(this.lblCount);
            this.pnlFilter.Controls.Add(this.cmbBlockFilter);
            this.pnlFilter.Controls.Add(this.txtFilter);
            this.pnlFilter.Controls.Add(this.lblFilter);
            this.pnlFilter.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlFilter.Location = new System.Drawing.Point(0, 0);
            this.pnlFilter.Name = "pnlFilter";
            this.pnlFilter.Size = new System.Drawing.Size(458, 38);
            this.pnlFilter.TabIndex = 0;
            // 
            // lblCount
            // 
            this.lblCount.Location = new System.Drawing.Point(330, 8);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(120, 22);
            this.lblCount.TabIndex = 3;
            this.lblCount.Text = "0 / 0";
            this.lblCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cmbBlockFilter
            // 
            this.cmbBlockFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBlockFilter.FormattingEnabled = true;
            this.cmbBlockFilter.Location = new System.Drawing.Point(206, 9);
            this.cmbBlockFilter.Name = "cmbBlockFilter";
            this.cmbBlockFilter.Size = new System.Drawing.Size(118, 21);
            this.cmbBlockFilter.TabIndex = 2;
            this.cmbBlockFilter.SelectedIndexChanged += new System.EventHandler(this.cmbBlockFilter_SelectedIndexChanged);
            // 
            // txtFilter
            // 
            this.txtFilter.Location = new System.Drawing.Point(50, 9);
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.Size = new System.Drawing.Size(150, 20);
            this.txtFilter.TabIndex = 1;
            this.txtFilter.TextChanged += new System.EventHandler(this.txtFilter_TextChanged);
            // 
            // lblFilter
            // 
            this.lblFilter.Location = new System.Drawing.Point(6, 8);
            this.lblFilter.Name = "lblFilter";
            this.lblFilter.Size = new System.Drawing.Size(42, 22);
            this.lblFilter.TabIndex = 0;
            this.lblFilter.Text = "Cari:";
            this.lblFilter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tabParts
            // 
            this.tabParts.Controls.Add(this.dgvParts);
            this.tabParts.Location = new System.Drawing.Point(4, 22);
            this.tabParts.Name = "tabParts";
            this.tabParts.Size = new System.Drawing.Size(458, 418);
            this.tabParts.TabIndex = 1;
            this.tabParts.Text = "Ringkasan Part";
            this.tabParts.UseVisualStyleBackColor = true;
            // 
            // dgvParts
            // 
            this.dgvParts.AllowUserToAddRows = false;
            this.dgvParts.AllowUserToDeleteRows = false;
            this.dgvParts.AllowUserToResizeRows = false;
            this.dgvParts.BackgroundColor = System.Drawing.Color.White;
            this.dgvParts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvParts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvParts.Location = new System.Drawing.Point(0, 0);
            this.dgvParts.MultiSelect = false;
            this.dgvParts.Name = "dgvParts";
            this.dgvParts.ReadOnly = true;
            this.dgvParts.RowHeadersVisible = false;
            this.dgvParts.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvParts.Size = new System.Drawing.Size(458, 418);
            this.dgvParts.TabIndex = 0;
            this.dgvParts.SelectionChanged += new System.EventHandler(this.dgvParts_SelectionChanged);
            // 
            // tabSummary
            // 
            this.tabSummary.Controls.Add(this.txtSummary);
            this.tabSummary.Location = new System.Drawing.Point(4, 22);
            this.tabSummary.Name = "tabSummary";
            this.tabSummary.Size = new System.Drawing.Size(458, 418);
            this.tabSummary.TabIndex = 2;
            this.tabSummary.Text = "Info & Panduan";
            this.tabSummary.UseVisualStyleBackColor = true;
            // 
            // txtSummary
            // 
            this.txtSummary.BackColor = System.Drawing.Color.White;
            this.txtSummary.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtSummary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSummary.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSummary.Location = new System.Drawing.Point(0, 0);
            this.txtSummary.Multiline = true;
            this.txtSummary.Name = "txtSummary";
            this.txtSummary.ReadOnly = true;
            this.txtSummary.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtSummary.Size = new System.Drawing.Size(458, 418);
            this.txtSummary.TabIndex = 0;
            // 
            // pnlDetail
            // 
            this.pnlDetail.Controls.Add(this.lvDetail);
            this.pnlDetail.Controls.Add(this.pnlDetailHeader);
            this.pnlDetail.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlDetail.Location = new System.Drawing.Point(0, 0);
            this.pnlDetail.Name = "pnlDetail";
            this.pnlDetail.Size = new System.Drawing.Size(466, 276);
            this.pnlDetail.TabIndex = 0;
            // 
            // lvDetail
            // 
            this.lvDetail.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colProp,
            this.colVal});
            this.lvDetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvDetail.FullRowSelect = true;
            this.lvDetail.GridLines = true;
            this.lvDetail.HideSelection = false;
            this.lvDetail.Location = new System.Drawing.Point(0, 62);
            this.lvDetail.MultiSelect = false;
            this.lvDetail.Name = "lvDetail";
            this.lvDetail.Size = new System.Drawing.Size(466, 214);
            this.lvDetail.TabIndex = 1;
            this.lvDetail.UseCompatibleStateImageBehavior = false;
            this.lvDetail.View = System.Windows.Forms.View.Details;
            // 
            // colProp
            // 
            this.colProp.Text = "Properti";
            this.colProp.Width = 140;
            // 
            // colVal
            // 
            this.colVal.Text = "Nilai";
            this.colVal.Width = 290;
            // 
            // pnlDetailHeader
            // 
            this.pnlDetailHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(52)))), ((int)(((byte)(71)))));
            this.pnlDetailHeader.Controls.Add(this.lblPartName);
            this.pnlDetailHeader.Controls.Add(this.lblRef);
            this.pnlDetailHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlDetailHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlDetailHeader.Name = "pnlDetailHeader";
            this.pnlDetailHeader.Size = new System.Drawing.Size(466, 62);
            this.pnlDetailHeader.TabIndex = 0;
            // 
            // lblPartName
            // 
            this.lblPartName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(200)))), ((int)(((byte)(225)))));
            this.lblPartName.Location = new System.Drawing.Point(12, 38);
            this.lblPartName.Name = "lblPartName";
            this.lblPartName.Size = new System.Drawing.Size(440, 18);
            this.lblPartName.TabIndex = 1;
            this.lblPartName.Text = "Klik komponen di gambar atau di tabel";
            // 
            // lblRef
            // 
            this.lblRef.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRef.ForeColor = System.Drawing.Color.White;
            this.lblRef.Location = new System.Drawing.Point(10, 4);
            this.lblRef.Name = "lblRef";
            this.lblRef.Size = new System.Drawing.Size(440, 32);
            this.lblRef.TabIndex = 0;
            this.lblRef.Text = "-";
            // 
            // Form1
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1360, 800);
            this.Controls.Add(this.splitMain);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStripView);
            this.Controls.Add(this.toolStripMain);
            this.MinimumSize = new System.Drawing.Size(1000, 600);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Yamaha YGX PCB Viewer";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form1_DragEnter);
            this.toolStripMain.ResumeLayout(false);
            this.toolStripMain.PerformLayout();
            this.toolStripView.ResumeLayout(false);
            this.toolStripView.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).EndInit();
            this.splitMain.ResumeLayout(false);
            this.tabMain.ResumeLayout(false);
            this.tabList.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMounts)).EndInit();
            this.pnlFilter.ResumeLayout(false);
            this.pnlFilter.PerformLayout();
            this.tabParts.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvParts)).EndInit();
            this.tabSummary.ResumeLayout(false);
            this.tabSummary.PerformLayout();
            this.pnlDetail.ResumeLayout(false);
            this.pnlDetailHeader.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStripMain;
        private System.Windows.Forms.ToolStripButton btnOpen;
        private System.Windows.Forms.ToolStripButton btnAdd;
        private System.Windows.Forms.ToolStripSeparator sep1;
        private System.Windows.Forms.ToolStripButton btnFit;
        private System.Windows.Forms.ToolStripLabel tslZoom;
        private System.Windows.Forms.ToolStripComboBox cmbZoomBlock;
        private System.Windows.Forms.ToolStripButton btnMeasure;
        private System.Windows.Forms.ToolStripSeparator sep2;
        private System.Windows.Forms.ToolStripButton btnPng;
        private System.Windows.Forms.ToolStripButton btnCsv;
        private System.Windows.Forms.ToolStripSeparator sep3;
        private System.Windows.Forms.ToolStripLabel tslLabel;
        private System.Windows.Forms.ToolStripComboBox cmbLabel;
        private System.Windows.Forms.ToolStripLabel tslColor;
        private System.Windows.Forms.ToolStripComboBox cmbColor;
        private System.Windows.Forms.ToolStripLabel tslTheme;
        private System.Windows.Forms.ToolStripComboBox cmbTheme;
        private System.Windows.Forms.ToolStrip toolStripView;
        private System.Windows.Forms.ToolStripButton btnGrid;
        private System.Windows.Forms.ToolStripButton btnBlocks;
        private System.Windows.Forms.ToolStripButton btnFid;
        private System.Windows.Forms.ToolStripButton btnFlipY;
        private System.Windows.Forms.ToolStripButton btnMirror;
        private System.Windows.Forms.ToolStripSeparator sep4;
        private System.Windows.Forms.ToolStripLabel tslRotPcb;
        private System.Windows.Forms.ToolStripComboBox cmbRotPcb;
        private System.Windows.Forms.ToolStripLabel tslRotMat;
        private System.Windows.Forms.ToolStripComboBox cmbRotMat;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblInfo;
        private System.Windows.Forms.ToolStripStatusLabel lblMeasure;
        private System.Windows.Forms.ToolStripStatusLabel lblZoom;
        private System.Windows.Forms.ToolStripStatusLabel lblCursor;
        private System.Windows.Forms.SplitContainer splitMain;
        private BoardView boardView;
        private System.Windows.Forms.TabControl tabMain;
        private System.Windows.Forms.TabPage tabList;
        private System.Windows.Forms.DataGridView dgvMounts;
        private System.Windows.Forms.Panel pnlFilter;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.ComboBox cmbBlockFilter;
        private System.Windows.Forms.TextBox txtFilter;
        private System.Windows.Forms.Label lblFilter;
        private System.Windows.Forms.TabPage tabParts;
        private System.Windows.Forms.DataGridView dgvParts;
        private System.Windows.Forms.TabPage tabSummary;
        private System.Windows.Forms.TextBox txtSummary;
        private System.Windows.Forms.Panel pnlDetail;
        private System.Windows.Forms.ListView lvDetail;
        private System.Windows.Forms.ColumnHeader colProp;
        private System.Windows.Forms.ColumnHeader colVal;
        private System.Windows.Forms.Panel pnlDetailHeader;
        private System.Windows.Forms.Label lblPartName;
        private System.Windows.Forms.Label lblRef;
    }
}