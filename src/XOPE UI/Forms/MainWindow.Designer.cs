namespace XOPE_UI
{
    partial class MainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadFiltersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFiltersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.logToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.restartAsAdminToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.processToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.attachToToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.detachToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.connectionsListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.pingTestSpyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.packetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newPacketToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.socketCheckerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.httpTunnelingModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scriptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runScriptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.scriptManagerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripSeparator();
            this.attachToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.detachToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton5 = new System.Windows.Forms.ToolStripSeparator();
            this.recordToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.pauseRecToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.stopRecToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.tabContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.closeTabToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.captureViewButton = new System.Windows.Forms.Button();
            this.filterViewButton = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.replayViewButton = new System.Windows.Forms.Button();
            this.viewTab = new System.Windows.Forms.TabControl();
            this.captureViewTabPage = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.captureTabControl = new System.Windows.Forms.TabControl();
            this.liveCapture = new System.Windows.Forms.TabPage();
            this.livePacketListView = new XOPE_UI.View.Component.PacketListView();
            this.hexPreviewPanel = new System.Windows.Forms.Panel();
            this.packetCaptureHexPreview = new XOPE_UI.View.Component.HexEditor();
            this.filterViewTabPage = new System.Windows.Forms.TabPage();
            this.filterViewTab = new XOPE_UI.View.FilterViewTab();
            this.replayViewTabPage = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.autoInjectorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.tabContextMenu.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.viewTab.SuspendLayout();
            this.captureViewTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.captureTabControl.SuspendLayout();
            this.liveCapture.SuspendLayout();
            this.hexPreviewPanel.SuspendLayout();
            this.filterViewTabPage.SuspendLayout();
            this.replayViewTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.processToolStripMenuItem,
            this.packetToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.scriptToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(1038, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadFiltersToolStripMenuItem,
            this.saveFiltersToolStripMenuItem,
            this.toolStripSeparator6,
            this.logToolStripMenuItem,
            this.restartAsAdminToolStripMenuItem,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // loadFiltersToolStripMenuItem
            // 
            this.loadFiltersToolStripMenuItem.Name = "loadFiltersToolStripMenuItem";
            this.loadFiltersToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.loadFiltersToolStripMenuItem.Text = "Load Filters";
            this.loadFiltersToolStripMenuItem.Click += new System.EventHandler(this.loadFiltersToolStripMenuItem_Click);
            // 
            // saveFiltersToolStripMenuItem
            // 
            this.saveFiltersToolStripMenuItem.Name = "saveFiltersToolStripMenuItem";
            this.saveFiltersToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.saveFiltersToolStripMenuItem.Text = "Save Filters";
            this.saveFiltersToolStripMenuItem.Click += new System.EventHandler(this.saveFiltersToolStripMenuItem_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(160, 6);
            // 
            // logToolStripMenuItem
            // 
            this.logToolStripMenuItem.Name = "logToolStripMenuItem";
            this.logToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.logToolStripMenuItem.Text = "Log";
            this.logToolStripMenuItem.Click += new System.EventHandler(this.logToolStripMenuItem_Click);
            // 
            // restartAsAdminToolStripMenuItem
            // 
            this.restartAsAdminToolStripMenuItem.Image = global::XOPE_UI.Properties.Resources.windows_admin_icon;
            this.restartAsAdminToolStripMenuItem.Name = "restartAsAdminToolStripMenuItem";
            this.restartAsAdminToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.restartAsAdminToolStripMenuItem.Text = "Restart as Admin";
            this.restartAsAdminToolStripMenuItem.Click += new System.EventHandler(this.restartAsAdminToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(160, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            this.editToolStripMenuItem.Visible = false;
            // 
            // processToolStripMenuItem
            // 
            this.processToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.attachToToolStripMenuItem,
            this.detachToolStripMenuItem,
            this.toolStripSeparator1,
            this.connectionsListToolStripMenuItem,
            this.toolStripSeparator4,
            this.pingTestSpyToolStripMenuItem});
            this.processToolStripMenuItem.Name = "processToolStripMenuItem";
            this.processToolStripMenuItem.Size = new System.Drawing.Size(59, 20);
            this.processToolStripMenuItem.Text = "Process";
            // 
            // attachToToolStripMenuItem
            // 
            this.attachToToolStripMenuItem.Name = "attachToToolStripMenuItem";
            this.attachToToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.attachToToolStripMenuItem.Text = "Attach to...";
            this.attachToToolStripMenuItem.Click += new System.EventHandler(this.attachToToolStripMenuItem_Click);
            // 
            // detachToolStripMenuItem
            // 
            this.detachToolStripMenuItem.Enabled = false;
            this.detachToolStripMenuItem.Name = "detachToolStripMenuItem";
            this.detachToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.detachToolStripMenuItem.Text = "Detach";
            this.detachToolStripMenuItem.Click += new System.EventHandler(this.detachToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(156, 6);
            // 
            // connectionsListToolStripMenuItem
            // 
            this.connectionsListToolStripMenuItem.Name = "connectionsListToolStripMenuItem";
            this.connectionsListToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.connectionsListToolStripMenuItem.Text = "Connections list";
            this.connectionsListToolStripMenuItem.Click += new System.EventHandler(this.connectionsListToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(156, 6);
            // 
            // pingTestSpyToolStripMenuItem
            // 
            this.pingTestSpyToolStripMenuItem.Name = "pingTestSpyToolStripMenuItem";
            this.pingTestSpyToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.pingTestSpyToolStripMenuItem.Text = "Ping Test Spy";
            this.pingTestSpyToolStripMenuItem.Click += new System.EventHandler(this.pingTestSpyToolStripMenuItem_Click);
            // 
            // packetToolStripMenuItem
            // 
            this.packetToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newPacketToolStripMenuItem,
            this.toolStripSeparator5,
            this.socketCheckerToolStripMenuItem});
            this.packetToolStripMenuItem.Name = "packetToolStripMenuItem";
            this.packetToolStripMenuItem.Size = new System.Drawing.Size(64, 20);
            this.packetToolStripMenuItem.Text = "Network";
            // 
            // newPacketToolStripMenuItem
            // 
            this.newPacketToolStripMenuItem.Name = "newPacketToolStripMenuItem";
            this.newPacketToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.newPacketToolStripMenuItem.Text = "New Packet";
            this.newPacketToolStripMenuItem.Click += new System.EventHandler(this.newPacketToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(152, 6);
            // 
            // socketCheckerToolStripMenuItem
            // 
            this.socketCheckerToolStripMenuItem.Name = "socketCheckerToolStripMenuItem";
            this.socketCheckerToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.socketCheckerToolStripMenuItem.Text = "Socket Checker";
            this.socketCheckerToolStripMenuItem.Click += new System.EventHandler(this.socketCheckerToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.autoInjectorToolStripMenuItem,
            this.httpTunnelingModeToolStripMenuItem,
            this.toolStripSeparator7,
            this.settingsToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // httpTunnelingModeToolStripMenuItem
            // 
            this.httpTunnelingModeToolStripMenuItem.Name = "httpTunnelingModeToolStripMenuItem";
            this.httpTunnelingModeToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.httpTunnelingModeToolStripMenuItem.Text = "HTTP Tunneling Mode";
            this.httpTunnelingModeToolStripMenuItem.Click += new System.EventHandler(this.httpTunnelingModeToolStripMenuItem_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(189, 6);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.settingsToolStripMenuItem.Text = "Settings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // scriptToolStripMenuItem
            // 
            this.scriptToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.runScriptToolStripMenuItem,
            this.toolStripSeparator3,
            this.scriptManagerToolStripMenuItem});
            this.scriptToolStripMenuItem.Name = "scriptToolStripMenuItem";
            this.scriptToolStripMenuItem.Size = new System.Drawing.Size(49, 20);
            this.scriptToolStripMenuItem.Text = "Script";
            // 
            // runScriptToolStripMenuItem
            // 
            this.runScriptToolStripMenuItem.Name = "runScriptToolStripMenuItem";
            this.runScriptToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.runScriptToolStripMenuItem.Text = "Run Script";
            this.runScriptToolStripMenuItem.Click += new System.EventHandler(this.runScriptToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(151, 6);
            // 
            // scriptManagerToolStripMenuItem
            // 
            this.scriptManagerToolStripMenuItem.Name = "scriptManagerToolStripMenuItem";
            this.scriptManagerToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.scriptManagerToolStripMenuItem.Text = "Script Manager";
            this.scriptManagerToolStripMenuItem.Click += new System.EventHandler(this.scriptManagerToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(111, 22);
            this.aboutToolStripMenuItem.Text = "Credits";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.CreditsToolStripMenuItem_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton2,
            this.attachToolStripButton,
            this.detachToolStripButton,
            this.toolStripButton5,
            this.recordToolStripButton,
            this.pauseRecToolStripButton,
            this.stopRecToolStripButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1038, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(6, 25);
            // 
            // attachToolStripButton
            // 
            this.attachToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.attachToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("attachToolStripButton.Image")));
            this.attachToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.attachToolStripButton.Name = "attachToolStripButton";
            this.attachToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.attachToolStripButton.Text = "Attach to process";
            this.attachToolStripButton.Click += new System.EventHandler(this.attachToolStripButton_Click);
            // 
            // detachToolStripButton
            // 
            this.detachToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.detachToolStripButton.Enabled = false;
            this.detachToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("detachToolStripButton.Image")));
            this.detachToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.detachToolStripButton.Name = "detachToolStripButton";
            this.detachToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.detachToolStripButton.Text = "toolStripButton6";
            this.detachToolStripButton.ToolTipText = "Detach from process";
            this.detachToolStripButton.Click += new System.EventHandler(this.detachToolStripButton_Click);
            // 
            // toolStripButton5
            // 
            this.toolStripButton5.Name = "toolStripButton5";
            this.toolStripButton5.Size = new System.Drawing.Size(6, 25);
            // 
            // recordToolStripButton
            // 
            this.recordToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.recordToolStripButton.Enabled = false;
            this.recordToolStripButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.recordToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("recordToolStripButton.Image")));
            this.recordToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.recordToolStripButton.Name = "recordToolStripButton";
            this.recordToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.recordToolStripButton.Text = "Record Packets";
            this.recordToolStripButton.Click += new System.EventHandler(this.recordToolStripButton_Click);
            // 
            // pauseRecToolStripButton
            // 
            this.pauseRecToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.pauseRecToolStripButton.Enabled = false;
            this.pauseRecToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("pauseRecToolStripButton.Image")));
            this.pauseRecToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.pauseRecToolStripButton.Name = "pauseRecToolStripButton";
            this.pauseRecToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.pauseRecToolStripButton.Text = "toolStripButton3";
            this.pauseRecToolStripButton.Click += new System.EventHandler(this.pauseRecToolStripButton_Click);
            // 
            // stopRecToolStripButton
            // 
            this.stopRecToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.stopRecToolStripButton.Enabled = false;
            this.stopRecToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("stopRecToolStripButton.Image")));
            this.stopRecToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.stopRecToolStripButton.Name = "stopRecToolStripButton";
            this.stopRecToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.stopRecToolStripButton.Text = "toolStripButton4";
            this.stopRecToolStripButton.Click += new System.EventHandler(this.stopRecToolStripButton_Click);
            // 
            // tabContextMenu
            // 
            this.tabContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeTabToolStripMenuItem});
            this.tabContextMenu.Name = "tabContextMenu";
            this.tabContextMenu.Size = new System.Drawing.Size(125, 26);
            // 
            // closeTabToolStripMenuItem
            // 
            this.closeTabToolStripMenuItem.Name = "closeTabToolStripMenuItem";
            this.closeTabToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.closeTabToolStripMenuItem.Text = "Close Tab";
            this.closeTabToolStripMenuItem.Click += new System.EventHandler(this.closeTabToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 525);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
            this.statusStrip1.Size = new System.Drawing.Size(1038, 22);
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(10, 17);
            this.toolStripStatusLabel.Text = "!";
            // 
            // captureViewButton
            // 
            this.captureViewButton.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.captureViewButton.FlatAppearance.BorderSize = 0;
            this.captureViewButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.captureViewButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.captureViewButton.Location = new System.Drawing.Point(1, 6);
            this.captureViewButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.captureViewButton.Name = "captureViewButton";
            this.captureViewButton.Size = new System.Drawing.Size(130, 27);
            this.captureViewButton.TabIndex = 6;
            this.captureViewButton.Text = "Capture View";
            this.captureViewButton.UseVisualStyleBackColor = false;
            // 
            // filterViewButton
            // 
            this.filterViewButton.BackColor = System.Drawing.SystemColors.Control;
            this.filterViewButton.FlatAppearance.BorderSize = 0;
            this.filterViewButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.filterViewButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.filterViewButton.Location = new System.Drawing.Point(1, 39);
            this.filterViewButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.filterViewButton.Name = "filterViewButton";
            this.filterViewButton.Size = new System.Drawing.Size(130, 27);
            this.filterViewButton.TabIndex = 7;
            this.filterViewButton.Text = "Filter View";
            this.filterViewButton.UseVisualStyleBackColor = false;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.replayViewButton);
            this.panel1.Controls.Add(this.captureViewButton);
            this.panel1.Controls.Add(this.filterViewButton);
            this.panel1.Location = new System.Drawing.Point(13, 60);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(130, 450);
            this.panel1.TabIndex = 8;
            // 
            // replayViewButton
            // 
            this.replayViewButton.BackColor = System.Drawing.SystemColors.Control;
            this.replayViewButton.FlatAppearance.BorderSize = 0;
            this.replayViewButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.replayViewButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.replayViewButton.Location = new System.Drawing.Point(1, 73);
            this.replayViewButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.replayViewButton.Name = "replayViewButton";
            this.replayViewButton.Size = new System.Drawing.Size(130, 27);
            this.replayViewButton.TabIndex = 8;
            this.replayViewButton.Text = "Replay View";
            this.replayViewButton.UseVisualStyleBackColor = false;
            // 
            // viewTab
            // 
            this.viewTab.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.viewTab.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.viewTab.Controls.Add(this.captureViewTabPage);
            this.viewTab.Controls.Add(this.filterViewTabPage);
            this.viewTab.Controls.Add(this.replayViewTabPage);
            this.viewTab.ItemSize = new System.Drawing.Size(0, 1);
            this.viewTab.Location = new System.Drawing.Point(135, 60);
            this.viewTab.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.viewTab.Name = "viewTab";
            this.viewTab.SelectedIndex = 0;
            this.viewTab.Size = new System.Drawing.Size(890, 454);
            this.viewTab.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.viewTab.TabIndex = 9;
            // 
            // captureViewTabPage
            // 
            this.captureViewTabPage.Controls.Add(this.splitContainer1);
            this.captureViewTabPage.Location = new System.Drawing.Point(4, 5);
            this.captureViewTabPage.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.captureViewTabPage.Name = "captureViewTabPage";
            this.captureViewTabPage.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.captureViewTabPage.Size = new System.Drawing.Size(882, 445);
            this.captureViewTabPage.TabIndex = 0;
            this.captureViewTabPage.Text = "tabPage1";
            this.captureViewTabPage.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.captureTabControl);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.hexPreviewPanel);
            this.splitContainer1.Size = new System.Drawing.Size(878, 449);
            this.splitContainer1.SplitterDistance = 326;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 3;
            // 
            // captureTabControl
            // 
            this.captureTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.captureTabControl.Controls.Add(this.liveCapture);
            this.captureTabControl.Location = new System.Drawing.Point(5, 0);
            this.captureTabControl.Margin = new System.Windows.Forms.Padding(0);
            this.captureTabControl.Name = "captureTabControl";
            this.captureTabControl.SelectedIndex = 0;
            this.captureTabControl.Size = new System.Drawing.Size(864, 326);
            this.captureTabControl.TabIndex = 0;
            // 
            // liveCapture
            // 
            this.liveCapture.Controls.Add(this.livePacketListView);
            this.liveCapture.Location = new System.Drawing.Point(4, 24);
            this.liveCapture.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.liveCapture.Name = "liveCapture";
            this.liveCapture.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.liveCapture.Size = new System.Drawing.Size(856, 298);
            this.liveCapture.TabIndex = 1;
            this.liveCapture.Text = "Live Capture";
            this.liveCapture.UseVisualStyleBackColor = true;
            // 
            // livePacketListView
            // 
            this.livePacketListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.livePacketListView.Location = new System.Drawing.Point(0, 0);
            this.livePacketListView.Margin = new System.Windows.Forms.Padding(0);
            this.livePacketListView.Name = "livePacketListView";
            this.livePacketListView.Size = new System.Drawing.Size(856, 298);
            this.livePacketListView.TabIndex = 0;
            // 
            // hexPreviewPanel
            // 
            this.hexPreviewPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hexPreviewPanel.Controls.Add(this.packetCaptureHexPreview);
            this.hexPreviewPanel.Location = new System.Drawing.Point(5, 1);
            this.hexPreviewPanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.hexPreviewPanel.Name = "hexPreviewPanel";
            this.hexPreviewPanel.Size = new System.Drawing.Size(869, 135);
            this.hexPreviewPanel.TabIndex = 6;
            // 
            // packetCaptureHexPreview
            // 
            this.packetCaptureHexPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.packetCaptureHexPreview.CellBackColor = System.Drawing.Color.White;
            this.packetCaptureHexPreview.CellHoverBackColor = System.Drawing.Color.Cyan;
            this.packetCaptureHexPreview.Location = new System.Drawing.Point(0, 5);
            this.packetCaptureHexPreview.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.packetCaptureHexPreview.Name = "packetCaptureHexPreview";
            this.packetCaptureHexPreview.SelectionBackColor = System.Drawing.Color.Blue;
            this.packetCaptureHexPreview.SelectionForeColor = System.Drawing.Color.White;
            this.packetCaptureHexPreview.Size = new System.Drawing.Size(864, 90);
            this.packetCaptureHexPreview.TabIndex = 0;
            // 
            // filterViewTabPage
            // 
            this.filterViewTabPage.Controls.Add(this.filterViewTab);
            this.filterViewTabPage.Location = new System.Drawing.Point(4, 5);
            this.filterViewTabPage.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.filterViewTabPage.Name = "filterViewTabPage";
            this.filterViewTabPage.Size = new System.Drawing.Size(882, 445);
            this.filterViewTabPage.TabIndex = 2;
            this.filterViewTabPage.Text = "tabPage1";
            this.filterViewTabPage.UseVisualStyleBackColor = true;
            // 
            // filterViewTab
            // 
            this.filterViewTab.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.filterViewTab.Enabled = false;
            this.filterViewTab.Location = new System.Drawing.Point(4, 3);
            this.filterViewTab.Name = "filterViewTab";
            this.filterViewTab.Size = new System.Drawing.Size(875, 421);
            this.filterViewTab.TabIndex = 0;
            // 
            // replayViewTabPage
            // 
            this.replayViewTabPage.Controls.Add(this.label2);
            this.replayViewTabPage.Location = new System.Drawing.Point(4, 5);
            this.replayViewTabPage.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.replayViewTabPage.Name = "replayViewTabPage";
            this.replayViewTabPage.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.replayViewTabPage.Size = new System.Drawing.Size(882, 445);
            this.replayViewTabPage.TabIndex = 1;
            this.replayViewTabPage.Text = "tabPage2";
            this.replayViewTabPage.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(408, 218);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(112, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "NOT IMPLEMENTED";
            // 
            // autoInjectorToolStripMenuItem
            // 
            this.autoInjectorToolStripMenuItem.Name = "autoInjectorToolStripMenuItem";
            this.autoInjectorToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.autoInjectorToolStripMenuItem.Text = "Auto DLL Injector";
            this.autoInjectorToolStripMenuItem.Click += new System.EventHandler(this.autoInjectorToolStripMenuItem_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1038, 547);
            this.Controls.Add(this.viewTab);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "MainWindow";
            this.Text = "XOPE";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindow_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tabContextMenu.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.viewTab.ResumeLayout(false);
            this.captureViewTabPage.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.captureTabControl.ResumeLayout(false);
            this.liveCapture.ResumeLayout(false);
            this.hexPreviewPanel.ResumeLayout(false);
            this.filterViewTabPage.ResumeLayout(false);
            this.replayViewTabPage.ResumeLayout(false);
            this.replayViewTabPage.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem processToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem attachToToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem detachToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton recordToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripButton2;
        private System.Windows.Forms.ToolStripButton pauseRecToolStripButton;
        private System.Windows.Forms.ToolStripButton stopRecToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripButton5;
        private System.Windows.Forms.ToolStripButton attachToolStripButton;
        private System.Windows.Forms.ToolStripButton detachToolStripButton;
        private System.Windows.Forms.ContextMenuStrip tabContextMenu;
        private System.Windows.Forms.ToolStripMenuItem closeTabToolStripMenuItem;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem connectionsListToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem packetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newPacketToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripMenuItem logToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Button captureViewButton;
        private System.Windows.Forms.Button filterViewButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TabControl viewTab;
        private System.Windows.Forms.TabPage captureViewTabPage;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TabControl captureTabControl;
        private System.Windows.Forms.TabPage liveCapture;
        private View.Component.PacketListView livePacketListView;
        private System.Windows.Forms.Panel hexPreviewPanel;
        private System.Windows.Forms.TabPage replayViewTabPage;
        private System.Windows.Forms.ToolStripMenuItem scriptToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem runScriptToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem scriptManagerToolStripMenuItem;
        private View.Component.HexEditor packetCaptureHexPreview;
        private System.Windows.Forms.Button replayViewButton;
        private System.Windows.Forms.TabPage filterViewTabPage;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem pingTestSpyToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem socketCheckerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem restartAsAdminToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private View.FilterViewTab filterViewTab;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadFiltersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveFiltersToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripMenuItem httpTunnelingModeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autoInjectorToolStripMenuItem;
    }
}

