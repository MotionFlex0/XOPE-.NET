﻿namespace XOPE_UI
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
            this.captureTabControl = new System.Windows.Forms.TabControl();
            this.liveView = new System.Windows.Forms.TabPage();
            this.logOutput = new System.Windows.Forms.TextBox();
            this.liveCapture = new System.Windows.Forms.TabPage();
            this.livePacketListView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.processToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.attachToToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.detachToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.connectionsListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.packetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newPacketToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.elementHost1 = new System.Windows.Forms.Integration.ElementHost();
            this.packetCaptureHexPreview = new WpfHexaEditor.HexEditor();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.captureViewButton = new System.Windows.Forms.Button();
            this.replayViewButton = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.viewTab = new System.Windows.Forms.TabControl();
            this.captureViewTabPage = new System.Windows.Forms.TabPage();
            this.replayViewTabPage = new System.Windows.Forms.TabPage();
            this.captureTabControl.SuspendLayout();
            this.liveView.SuspendLayout();
            this.liveCapture.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.tabContextMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.viewTab.SuspendLayout();
            this.captureViewTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // captureTabControl
            // 
            this.captureTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.captureTabControl.Controls.Add(this.liveView);
            this.captureTabControl.Controls.Add(this.liveCapture);
            this.captureTabControl.Location = new System.Drawing.Point(4, 0);
            this.captureTabControl.Name = "captureTabControl";
            this.captureTabControl.SelectedIndex = 0;
            this.captureTabControl.Size = new System.Drawing.Size(674, 298);
            this.captureTabControl.TabIndex = 0;
            // 
            // liveView
            // 
            this.liveView.Controls.Add(this.logOutput);
            this.liveView.Location = new System.Drawing.Point(4, 22);
            this.liveView.Name = "liveView";
            this.liveView.Padding = new System.Windows.Forms.Padding(3);
            this.liveView.Size = new System.Drawing.Size(666, 272);
            this.liveView.TabIndex = 0;
            this.liveView.Text = "Log";
            this.liveView.UseVisualStyleBackColor = true;
            // 
            // logOutput
            // 
            this.logOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logOutput.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.logOutput.Location = new System.Drawing.Point(3, 2);
            this.logOutput.Multiline = true;
            this.logOutput.Name = "logOutput";
            this.logOutput.ReadOnly = true;
            this.logOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.logOutput.Size = new System.Drawing.Size(452, 267);
            this.logOutput.TabIndex = 0;
            this.logOutput.TabStop = false;
            // 
            // liveCapture
            // 
            this.liveCapture.Controls.Add(this.livePacketListView);
            this.liveCapture.Location = new System.Drawing.Point(4, 22);
            this.liveCapture.Name = "liveCapture";
            this.liveCapture.Padding = new System.Windows.Forms.Padding(3);
            this.liveCapture.Size = new System.Drawing.Size(666, 272);
            this.liveCapture.TabIndex = 1;
            this.liveCapture.Text = "Live Capture";
            this.liveCapture.UseVisualStyleBackColor = true;
            // 
            // livePacketListView
            // 
            this.livePacketListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.livePacketListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader5,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.livePacketListView.FullRowSelect = true;
            this.livePacketListView.HideSelection = false;
            this.livePacketListView.Location = new System.Drawing.Point(0, 0);
            this.livePacketListView.MultiSelect = false;
            this.livePacketListView.Name = "livePacketListView";
            this.livePacketListView.Size = new System.Drawing.Size(666, 272);
            this.livePacketListView.TabIndex = 0;
            this.livePacketListView.UseCompatibleStateImageBehavior = false;
            this.livePacketListView.View = System.Windows.Forms.View.Details;
            this.livePacketListView.SelectedIndexChanged += new System.EventHandler(this.livePacketListView_SelectedIndexChanged);
            this.livePacketListView.DoubleClick += new System.EventHandler(this.livePacketListView_DoubleClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "#";
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Type";
            this.columnHeader5.Width = 80;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Length";
            this.columnHeader2.Width = 59;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Data";
            this.columnHeader3.Width = 278;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "SocketID";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.processToolStripMenuItem,
            this.packetToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(808, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.logToolStripMenuItem,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // logToolStripMenuItem
            // 
            this.logToolStripMenuItem.Name = "logToolStripMenuItem";
            this.logToolStripMenuItem.Size = new System.Drawing.Size(94, 22);
            this.logToolStripMenuItem.Text = "Log";
            this.logToolStripMenuItem.Click += new System.EventHandler(this.logToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(91, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(94, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // processToolStripMenuItem
            // 
            this.processToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.attachToToolStripMenuItem,
            this.detachToolStripMenuItem,
            this.toolStripSeparator1,
            this.connectionsListToolStripMenuItem});
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
            // packetToolStripMenuItem
            // 
            this.packetToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newPacketToolStripMenuItem});
            this.packetToolStripMenuItem.Name = "packetToolStripMenuItem";
            this.packetToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
            this.packetToolStripMenuItem.Text = "Packet";
            // 
            // newPacketToolStripMenuItem
            // 
            this.newPacketToolStripMenuItem.Name = "newPacketToolStripMenuItem";
            this.newPacketToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.newPacketToolStripMenuItem.Text = "New Packet";
            this.newPacketToolStripMenuItem.Click += new System.EventHandler(this.newPacketToolStripMenuItem_Click);
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
            this.toolStrip1.Size = new System.Drawing.Size(808, 25);
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
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.captureTabControl);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.elementHost1);
            this.splitContainer1.Size = new System.Drawing.Size(677, 390);
            this.splitContainer1.SplitterDistance = 297;
            this.splitContainer1.TabIndex = 3;
            // 
            // elementHost1
            // 
            this.elementHost1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.elementHost1.Location = new System.Drawing.Point(3, 3);
            this.elementHost1.Name = "elementHost1";
            this.elementHost1.Size = new System.Drawing.Size(674, 81);
            this.elementHost1.TabIndex = 5;
            this.elementHost1.Child = this.packetCaptureHexPreview;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 452);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(808, 22);
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // captureViewButton
            // 
            this.captureViewButton.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.captureViewButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.captureViewButton.Location = new System.Drawing.Point(3, 5);
            this.captureViewButton.Name = "captureViewButton";
            this.captureViewButton.Size = new System.Drawing.Size(104, 23);
            this.captureViewButton.TabIndex = 6;
            this.captureViewButton.Text = "Capture View";
            this.captureViewButton.UseVisualStyleBackColor = false;
            // 
            // replayViewButton
            // 
            this.replayViewButton.BackColor = System.Drawing.SystemColors.Control;
            this.replayViewButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.replayViewButton.Location = new System.Drawing.Point(3, 34);
            this.replayViewButton.Name = "replayViewButton";
            this.replayViewButton.Size = new System.Drawing.Size(104, 23);
            this.replayViewButton.TabIndex = 7;
            this.replayViewButton.Text = "Replay View";
            this.replayViewButton.UseVisualStyleBackColor = false;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.panel1.Controls.Add(this.captureViewButton);
            this.panel1.Controls.Add(this.replayViewButton);
            this.panel1.Location = new System.Drawing.Point(0, 52);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(111, 390);
            this.panel1.TabIndex = 8;
            // 
            // viewTab
            // 
            this.viewTab.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.viewTab.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.viewTab.Controls.Add(this.captureViewTabPage);
            this.viewTab.Controls.Add(this.replayViewTabPage);
            this.viewTab.ItemSize = new System.Drawing.Size(0, 1);
            this.viewTab.Location = new System.Drawing.Point(111, 52);
            this.viewTab.Name = "viewTab";
            this.viewTab.SelectedIndex = 0;
            this.viewTab.Size = new System.Drawing.Size(685, 397);
            this.viewTab.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.viewTab.TabIndex = 9;
            // 
            // captureViewTabPage
            // 
            this.captureViewTabPage.Controls.Add(this.splitContainer1);
            this.captureViewTabPage.Location = new System.Drawing.Point(4, 5);
            this.captureViewTabPage.Name = "captureViewTabPage";
            this.captureViewTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.captureViewTabPage.Size = new System.Drawing.Size(677, 388);
            this.captureViewTabPage.TabIndex = 0;
            this.captureViewTabPage.Text = "tabPage1";
            this.captureViewTabPage.UseVisualStyleBackColor = true;
            // 
            // replayViewTabPage
            // 
            this.replayViewTabPage.Location = new System.Drawing.Point(4, 5);
            this.replayViewTabPage.Name = "replayViewTabPage";
            this.replayViewTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.replayViewTabPage.Size = new System.Drawing.Size(677, 388);
            this.replayViewTabPage.TabIndex = 1;
            this.replayViewTabPage.Text = "tabPage2";
            this.replayViewTabPage.UseVisualStyleBackColor = true;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(808, 474);
            this.Controls.Add(this.viewTab);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainWindow";
            this.Text = "XOPE";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindow_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.captureTabControl.ResumeLayout(false);
            this.liveView.ResumeLayout(false);
            this.liveView.PerformLayout();
            this.liveCapture.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tabContextMenu.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.viewTab.ResumeLayout(false);
            this.captureViewTabPage.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TabControl captureTabControl;
        private System.Windows.Forms.TabPage liveView;
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
        private System.Windows.Forms.TextBox logOutput;
        private System.Windows.Forms.ContextMenuStrip tabContextMenu;
        private System.Windows.Forms.ToolStripMenuItem closeTabToolStripMenuItem;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem connectionsListToolStripMenuItem;
        private System.Windows.Forms.TabPage liveCapture;
        private System.Windows.Forms.ListView livePacketListView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ToolStripMenuItem packetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newPacketToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem logToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Button captureViewButton;
        private System.Windows.Forms.Button replayViewButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TabControl viewTab;
        private System.Windows.Forms.TabPage captureViewTabPage;
        private System.Windows.Forms.TabPage replayViewTabPage;
        private WpfHexaEditor.HexEditor packetCaptureHexPreview;
        private System.Windows.Forms.Integration.ElementHost elementHost1;
    }
}
