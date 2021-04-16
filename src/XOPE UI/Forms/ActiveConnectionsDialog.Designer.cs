namespace XOPE_UI.Forms
{
    partial class ActiveConnectionsDialog
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
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("test");
            this.closeButton = new System.Windows.Forms.Button();
            this.connectionListView = new System.Windows.Forms.ListView();
            this.socket = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ip = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.refreshButton = new System.Windows.Forms.Button();
            this.port = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ipFamily = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.status = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.Location = new System.Drawing.Point(311, 357);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
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
            this.socket,
            this.ipFamily,
            this.ip,
            this.port,
            this.status});
            this.connectionListView.FullRowSelect = true;
            this.connectionListView.HideSelection = false;
            this.connectionListView.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1});
            this.connectionListView.Location = new System.Drawing.Point(12, 12);
            this.connectionListView.MultiSelect = false;
            this.connectionListView.Name = "connectionListView";
            this.connectionListView.Size = new System.Drawing.Size(374, 339);
            this.connectionListView.TabIndex = 1;
            this.connectionListView.UseCompatibleStateImageBehavior = false;
            this.connectionListView.View = System.Windows.Forms.View.Details;
            // 
            // socket
            // 
            this.socket.Text = "Socket ID";
            // 
            // ip
            // 
            this.ip.Text = "IP Address";
            this.ip.Width = 67;
            // 
            // refreshButton
            // 
            this.refreshButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.refreshButton.Location = new System.Drawing.Point(230, 357);
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(75, 23);
            this.refreshButton.TabIndex = 2;
            this.refreshButton.Text = "Refresh";
            this.refreshButton.UseVisualStyleBackColor = true;
            this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
            // 
            // port
            // 
            this.port.Text = "Port";
            // 
            // ipFamily
            // 
            this.ipFamily.Text = "IP Family";
            // 
            // status
            // 
            this.status.Text = "Status";
            this.status.Width = 120;
            // 
            // ActiveConnectionsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(398, 392);
            this.Controls.Add(this.refreshButton);
            this.Controls.Add(this.connectionListView);
            this.Controls.Add(this.closeButton);
            this.Name = "ActiveConnectionsDialog";
            this.Text = "Active Connections";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.ListView connectionListView;
        private System.Windows.Forms.ColumnHeader socket;
        private System.Windows.Forms.ColumnHeader ip;
        private System.Windows.Forms.Button refreshButton;
        private System.Windows.Forms.ColumnHeader ipFamily;
        private System.Windows.Forms.ColumnHeader port;
        private System.Windows.Forms.ColumnHeader status;
    }
}