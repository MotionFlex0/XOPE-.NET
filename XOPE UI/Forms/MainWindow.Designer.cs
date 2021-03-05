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
            this.captureTabControl = new System.Windows.Forms.TabControl();
            this.liveView = new System.Windows.Forms.TabPage();
            this.liveViewOutput = new System.Windows.Forms.TextBox();
            this.liveCapture = new System.Windows.Forms.TabPage();
            this.livePacketListView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.processToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.attachToToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.detachToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.connectionsListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.captureTabControl.SuspendLayout();
            this.liveView.SuspendLayout();
            this.liveCapture.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.tabContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // captureTabControl
            // 
            this.captureTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.captureTabControl.Controls.Add(this.liveView);
            this.captureTabControl.Controls.Add(this.liveCapture);
            this.captureTabControl.Location = new System.Drawing.Point(12, 52);
            this.captureTabControl.Name = "captureTabControl";
            this.captureTabControl.SelectedIndex = 0;
            this.captureTabControl.Size = new System.Drawing.Size(776, 386);
            this.captureTabControl.TabIndex = 0;
            // 
            // liveView
            // 
            this.liveView.Controls.Add(this.liveViewOutput);
            this.liveView.Location = new System.Drawing.Point(4, 22);
            this.liveView.Name = "liveView";
            this.liveView.Padding = new System.Windows.Forms.Padding(3);
            this.liveView.Size = new System.Drawing.Size(768, 360);
            this.liveView.TabIndex = 0;
            this.liveView.Text = "Log";
            this.liveView.UseVisualStyleBackColor = true;
            // 
            // liveViewOutput
            // 
            this.liveViewOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.liveViewOutput.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.liveViewOutput.Location = new System.Drawing.Point(3, 2);
            this.liveViewOutput.Multiline = true;
            this.liveViewOutput.Name = "liveViewOutput";
            this.liveViewOutput.ReadOnly = true;
            this.liveViewOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.liveViewOutput.Size = new System.Drawing.Size(569, 355);
            this.liveViewOutput.TabIndex = 0;
            this.liveViewOutput.TabStop = false;
            // 
            // liveCapture
            // 
            this.liveCapture.Controls.Add(this.livePacketListView);
            this.liveCapture.Location = new System.Drawing.Point(4, 22);
            this.liveCapture.Name = "liveCapture";
            this.liveCapture.Padding = new System.Windows.Forms.Padding(3);
            this.liveCapture.Size = new System.Drawing.Size(768, 360);
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
            this.livePacketListView.Location = new System.Drawing.Point(3, 3);
            this.livePacketListView.MultiSelect = false;
            this.livePacketListView.Name = "livePacketListView";
            this.livePacketListView.Size = new System.Drawing.Size(762, 354);
            this.livePacketListView.TabIndex = 0;
            this.livePacketListView.UseCompatibleStateImageBehavior = false;
            this.livePacketListView.View = System.Windows.Forms.View.Details;
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
            this.columnHeader3.Width = 432;
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
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
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
            this.toolStrip1.Size = new System.Drawing.Size(800, 25);
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
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.captureTabControl);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainWindow";
            this.Text = "XOPE";
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
        private System.Windows.Forms.TextBox liveViewOutput;
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
    }
}

