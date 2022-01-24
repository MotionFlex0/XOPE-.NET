namespace XOPE_UI.View
{
    partial class SettingsDialog
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
            this.listView1 = new System.Windows.Forms.ListView();
            this.settingsDataGridView = new System.Windows.Forms.DataGridView();
            this.SettingsName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.descriptionTextBox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.settingsDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listView1.Location = new System.Drawing.Point(12, 12);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(156, 378);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // settingsDataGridView
            // 
            this.settingsDataGridView.AllowUserToAddRows = false;
            this.settingsDataGridView.AllowUserToDeleteRows = false;
            this.settingsDataGridView.AllowUserToResizeRows = false;
            this.settingsDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.settingsDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.SettingsName,
            this.Value});
            this.settingsDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.settingsDataGridView.Location = new System.Drawing.Point(174, 12);
            this.settingsDataGridView.MultiSelect = false;
            this.settingsDataGridView.Name = "settingsDataGridView";
            this.settingsDataGridView.RowHeadersVisible = false;
            this.settingsDataGridView.RowTemplate.Height = 25;
            this.settingsDataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.settingsDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.settingsDataGridView.Size = new System.Drawing.Size(528, 310);
            this.settingsDataGridView.TabIndex = 1;
            this.settingsDataGridView.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.settingsDataGridView_CellBeginEdit);
            this.settingsDataGridView.CurrentCellDirtyStateChanged += new System.EventHandler(this.settingsDataGridView_CurrentCellDirtyStateChanged);
            this.settingsDataGridView.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.settingsDataGridView_EditingControlShowing);
            this.settingsDataGridView.SelectionChanged += new System.EventHandler(this.settingsDataGridView_SelectionChanged);
            // 
            // SettingsName
            // 
            this.SettingsName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.SettingsName.DataPropertyName = "Name";
            this.SettingsName.HeaderText = "(Name)";
            this.SettingsName.Name = "SettingsName";
            this.SettingsName.ReadOnly = true;
            // 
            // Value
            // 
            this.Value.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Value.DataPropertyName = "Value";
            this.Value.HeaderText = "Value";
            this.Value.Name = "Value";
            // 
            // descriptionTextBox
            // 
            this.descriptionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.descriptionTextBox.Location = new System.Drawing.Point(174, 328);
            this.descriptionTextBox.Multiline = true;
            this.descriptionTextBox.Name = "descriptionTextBox";
            this.descriptionTextBox.Size = new System.Drawing.Size(528, 62);
            this.descriptionTextBox.TabIndex = 2;
            // 
            // SettingsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(714, 402);
            this.Controls.Add(this.descriptionTextBox);
            this.Controls.Add(this.settingsDataGridView);
            this.Controls.Add(this.listView1);
            this.Name = "SettingsDialog";
            this.Text = "SettingsDialog";
            ((System.ComponentModel.ISupportInitialize)(this.settingsDataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.DataGridView settingsDataGridView;
        private System.Windows.Forms.TextBox descriptionTextBox;
        private System.Windows.Forms.DataGridViewTextBoxColumn SettingsName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Value;
    }
}