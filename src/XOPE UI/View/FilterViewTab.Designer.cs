namespace XOPE_UI.View
{
    partial class FilterViewTab
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.filterDataGridView = new System.Windows.Forms.DataGridView();
            this.FilterNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FilterName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PacketType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Filter = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SocketId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Activated = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.deleteFilterButton = new System.Windows.Forms.Button();
            this.addFilterButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.filterDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // filterDataGridView
            // 
            this.filterDataGridView.AllowUserToAddRows = false;
            this.filterDataGridView.AllowUserToDeleteRows = false;
            this.filterDataGridView.AllowUserToResizeRows = false;
            this.filterDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.filterDataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SunkenVertical;
            this.filterDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.filterDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.filterDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.FilterNumber,
            this.FilterName,
            this.PacketType,
            this.Filter,
            this.SocketId,
            this.Activated});
            this.filterDataGridView.Location = new System.Drawing.Point(1, 0);
            this.filterDataGridView.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.filterDataGridView.Name = "filterDataGridView";
            this.filterDataGridView.RowHeadersVisible = false;
            this.filterDataGridView.RowTemplate.Height = 20;
            this.filterDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.filterDataGridView.Size = new System.Drawing.Size(873, 367);
            this.filterDataGridView.TabIndex = 0;
            this.filterDataGridView.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.filterDataGridView_CellMouseDoubleClick);
            this.filterDataGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.filterDataGridView_CellValueChanged);
            this.filterDataGridView.CurrentCellDirtyStateChanged += new System.EventHandler(this.filterDataGridView_CurrentCellDirtyStateChanged);
            this.filterDataGridView.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.filterDataGridView_RowsAdded);
            this.filterDataGridView.SelectionChanged += new System.EventHandler(this.filterDataGridView_SelectionChanged);
            // 
            // FilterNumber
            // 
            this.FilterNumber.DataPropertyName = "FilterNumber";
            this.FilterNumber.HeaderText = "#";
            this.FilterNumber.Name = "FilterNumber";
            this.FilterNumber.ReadOnly = true;
            this.FilterNumber.Width = 60;
            // 
            // FilterName
            // 
            this.FilterName.DataPropertyName = "Name";
            this.FilterName.HeaderText = "Name";
            this.FilterName.Name = "FilterName";
            this.FilterName.ReadOnly = true;
            this.FilterName.Width = 80;
            // 
            // PacketType
            // 
            this.PacketType.DataPropertyName = "PacketType";
            this.PacketType.HeaderText = "Type";
            this.PacketType.Name = "PacketType";
            this.PacketType.ReadOnly = true;
            this.PacketType.Width = 90;
            // 
            // Filter
            // 
            this.Filter.DataPropertyName = "Filter";
            this.Filter.HeaderText = "Filter";
            this.Filter.Name = "Filter";
            this.Filter.ReadOnly = true;
            this.Filter.Width = 470;
            // 
            // SocketId
            // 
            this.SocketId.DataPropertyName = "SocketId";
            this.SocketId.HeaderText = "Socket Id";
            this.SocketId.Name = "SocketId";
            this.SocketId.ReadOnly = true;
            this.SocketId.Width = 80;
            // 
            // Activated
            // 
            this.Activated.DataPropertyName = "Activated";
            this.Activated.HeaderText = "Activated";
            this.Activated.Name = "Activated";
            this.Activated.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Activated.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Activated.Width = 70;
            // 
            // deleteFilterButton
            // 
            this.deleteFilterButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.deleteFilterButton.Enabled = false;
            this.deleteFilterButton.Location = new System.Drawing.Point(791, 371);
            this.deleteFilterButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.deleteFilterButton.Name = "deleteFilterButton";
            this.deleteFilterButton.Size = new System.Drawing.Size(83, 27);
            this.deleteFilterButton.TabIndex = 4;
            this.deleteFilterButton.Text = "Delete Filter";
            this.deleteFilterButton.UseVisualStyleBackColor = true;
            this.deleteFilterButton.Click += new System.EventHandler(this.deleteFilterButton_Click);
            // 
            // addFilterButton
            // 
            this.addFilterButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.addFilterButton.Location = new System.Drawing.Point(0, 371);
            this.addFilterButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.addFilterButton.Name = "addFilterButton";
            this.addFilterButton.Size = new System.Drawing.Size(88, 27);
            this.addFilterButton.TabIndex = 3;
            this.addFilterButton.Text = "Add filter";
            this.addFilterButton.UseVisualStyleBackColor = true;
            this.addFilterButton.Click += new System.EventHandler(this.addFilterButton_Click);
            // 
            // FilterViewTab
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.deleteFilterButton);
            this.Controls.Add(this.addFilterButton);
            this.Controls.Add(this.filterDataGridView);
            this.Name = "FilterViewTab";
            this.Size = new System.Drawing.Size(875, 421);
            ((System.ComponentModel.ISupportInitialize)(this.filterDataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView filterDataGridView;
        private System.Windows.Forms.Button deleteFilterButton;
        private System.Windows.Forms.Button addFilterButton;
        private System.Windows.Forms.DataGridViewTextBoxColumn FilterNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn FilterName;
        private System.Windows.Forms.DataGridViewTextBoxColumn PacketType;
        private System.Windows.Forms.DataGridViewTextBoxColumn Filter;
        private System.Windows.Forms.DataGridViewTextBoxColumn SocketId;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Activated;
    }
}
