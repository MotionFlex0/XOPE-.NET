namespace XOPE_UI.View
{
    partial class ActiveConnectionsDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("test");
            this.closeButton = new System.Windows.Forms.Button();
            this.connectionListView = new System.Windows.Forms.ListView();
            this.socket_id = new System.Windows.Forms.ColumnHeader();
            this.ip_family = new System.Windows.Forms.ColumnHeader();
            this.ip_address = new System.Windows.Forms.ColumnHeader();
            this.port = new System.Windows.Forms.ColumnHeader();
            this.status = new System.Windows.Forms.ColumnHeader();
            this.refreshButton = new System.Windows.Forms.Button();
            this.connectionContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.dNSLookupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.copyIPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyIPPortToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyPortToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copySocketIdToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectionContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.Location = new System.Drawing.Point(363, 412);
            this.closeButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(88, 27);
            this.closeButton.TabIndex = 0;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // connectionListView
            // 
            this.connectionListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.connectionListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.socket_id,
            this.ip_family,
            this.ip_address,
            this.port,
            this.status});
            this.connectionListView.FullRowSelect = true;
            this.connectionListView.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1});
            this.connectionListView.Location = new System.Drawing.Point(14, 14);
            this.connectionListView.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.connectionListView.MultiSelect = false;
            this.connectionListView.Name = "connectionListView";
            this.connectionListView.Size = new System.Drawing.Size(436, 391);
            this.connectionListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.connectionListView.TabIndex = 1;
            this.connectionListView.UseCompatibleStateImageBehavior = false;
            this.connectionListView.View = System.Windows.Forms.View.Details;
            this.connectionListView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.connectionListView_MouseClick);
            // 
            // socket_id
            // 
            this.socket_id.Tag = "socket_id";
            this.socket_id.Text = "Socket ID";
            // 
            // ip_family
            // 
            this.ip_family.Tag = "ip_family";
            this.ip_family.Text = "IP Family";
            // 
            // ip_address
            // 
            this.ip_address.Tag = "ip_address";
            this.ip_address.Text = "IP Address";
            this.ip_address.Width = 67;
            // 
            // port
            // 
            this.port.Tag = "port";
            this.port.Text = "Port";
            // 
            // status
            // 
            this.status.Tag = "status";
            this.status.Text = "Status";
            this.status.Width = 120;
            // 
            // refreshButton
            // 
            this.refreshButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.refreshButton.Location = new System.Drawing.Point(268, 412);
            this.refreshButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(88, 27);
            this.refreshButton.TabIndex = 2;
            this.refreshButton.Text = "Refresh";
            this.refreshButton.UseVisualStyleBackColor = true;
            this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
            // 
            // connectionContextMenu
            // 
            this.connectionContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dNSLookupToolStripMenuItem,
            this.toolStripSeparator1,
            this.copyIPToolStripMenuItem,
            this.copyIPPortToolStripMenuItem,
            this.copyPortToolStripMenuItem,
            this.copySocketIdToolStripMenuItem});
            this.connectionContextMenu.Name = "connectionContextMenu";
            this.connectionContextMenu.Size = new System.Drawing.Size(154, 120);
            // 
            // dNSLookupToolStripMenuItem
            // 
            this.dNSLookupToolStripMenuItem.Name = "dNSLookupToolStripMenuItem";
            this.dNSLookupToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.dNSLookupToolStripMenuItem.Text = "DNS Lookup";
            this.dNSLookupToolStripMenuItem.Click += new System.EventHandler(this.dNSLookupToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(150, 6);
            // 
            // copyIPToolStripMenuItem
            // 
            this.copyIPToolStripMenuItem.Name = "copyIPToolStripMenuItem";
            this.copyIPToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.copyIPToolStripMenuItem.Text = "Copy IP";
            this.copyIPToolStripMenuItem.Click += new System.EventHandler(this.copyIPToolStripMenuItem_Click);
            // 
            // copyIPPortToolStripMenuItem
            // 
            this.copyIPPortToolStripMenuItem.Name = "copyIPPortToolStripMenuItem";
            this.copyIPPortToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.copyIPPortToolStripMenuItem.Text = "Copy IP:Port";
            this.copyIPPortToolStripMenuItem.Click += new System.EventHandler(this.copyIPPortToolStripMenuItem_Click);
            // 
            // copyPortToolStripMenuItem
            // 
            this.copyPortToolStripMenuItem.Name = "copyPortToolStripMenuItem";
            this.copyPortToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.copyPortToolStripMenuItem.Text = "Copy Port";
            this.copyPortToolStripMenuItem.Click += new System.EventHandler(this.copyPortToolStripMenuItem_Click);
            // 
            // copySocketIdToolStripMenuItem
            // 
            this.copySocketIdToolStripMenuItem.Name = "copySocketIdToolStripMenuItem";
            this.copySocketIdToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.copySocketIdToolStripMenuItem.Text = "Copy Socket Id";
            this.copySocketIdToolStripMenuItem.Click += new System.EventHandler(this.copySocketIdToolStripMenuItem_Click);
            // 
            // ActiveConnectionsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(464, 452);
            this.Controls.Add(this.refreshButton);
            this.Controls.Add(this.connectionListView);
            this.Controls.Add(this.closeButton);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "ActiveConnectionsDialog";
            this.Text = "Active Connections";
            this.connectionContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.ListView connectionListView;
        private System.Windows.Forms.ColumnHeader socket_id;
        private System.Windows.Forms.ColumnHeader ip_address;
        private System.Windows.Forms.Button refreshButton;
        private System.Windows.Forms.ColumnHeader ip_family;
        private System.Windows.Forms.ColumnHeader port;
        private System.Windows.Forms.ColumnHeader status;
        private System.Windows.Forms.ContextMenuStrip connectionContextMenu;
        private System.Windows.Forms.ToolStripMenuItem dNSLookupToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem copyIPToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyIPPortToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyPortToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copySocketIdToolStripMenuItem;
    }
}