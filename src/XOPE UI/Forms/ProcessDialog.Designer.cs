namespace XOPE_UI.View
{
    partial class ProcessDialog
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
            this.processesListView = new System.Windows.Forms.ListView();
            this.dummyHeader = new System.Windows.Forms.ColumnHeader();
            this.label1 = new System.Windows.Forms.Label();
            this.is64bitText = new System.Windows.Forms.TextBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.confirmButton = new System.Windows.Forms.Button();
            this.processListTotalLabel = new System.Windows.Forms.Label();
            this.processDialogBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.searchTextBoxPlaceholder = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.processDialogBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // processesListView
            // 
            this.processesListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.processesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.dummyHeader});
            this.processesListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.processesListView.Location = new System.Drawing.Point(14, 14);
            this.processesListView.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.processesListView.MultiSelect = false;
            this.processesListView.Name = "processesListView";
            this.processesListView.ShowItemToolTips = true;
            this.processesListView.Size = new System.Drawing.Size(751, 352);
            this.processesListView.TabIndex = 1;
            this.processesListView.UseCompatibleStateImageBehavior = false;
            this.processesListView.View = System.Windows.Forms.View.Details;
            this.processesListView.SelectedIndexChanged += new System.EventHandler(this.processesView_SelectedIndexChanged);
            this.processesListView.DoubleClick += new System.EventHandler(this.processesView_DoubleClick);
            // 
            // dummyHeader
            // 
            this.dummyHeader.Text = "";
            this.dummyHeader.Width = 40;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 381);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "Is64BitProcess:";
            // 
            // is64bitText
            // 
            this.is64bitText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.is64bitText.Location = new System.Drawing.Point(111, 376);
            this.is64bitText.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.is64bitText.Name = "is64bitText";
            this.is64bitText.ReadOnly = true;
            this.is64bitText.Size = new System.Drawing.Size(89, 23);
            this.is64bitText.TabIndex = 3;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.Location = new System.Drawing.Point(678, 375);
            this.cancelButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(88, 27);
            this.cancelButton.TabIndex = 4;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // confirmButton
            // 
            this.confirmButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.confirmButton.Enabled = false;
            this.confirmButton.Location = new System.Drawing.Point(583, 375);
            this.confirmButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.confirmButton.Name = "confirmButton";
            this.confirmButton.Size = new System.Drawing.Size(88, 27);
            this.confirmButton.TabIndex = 5;
            this.confirmButton.Text = "Confirm";
            this.confirmButton.UseVisualStyleBackColor = true;
            this.confirmButton.Click += new System.EventHandler(this.confirmButton_Click);
            // 
            // processListTotalLabel
            // 
            this.processListTotalLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.processListTotalLabel.BackColor = System.Drawing.Color.White;
            this.processListTotalLabel.Location = new System.Drawing.Point(644, 327);
            this.processListTotalLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.processListTotalLabel.Name = "processListTotalLabel";
            this.processListTotalLabel.Size = new System.Drawing.Size(82, 15);
            this.processListTotalLabel.TabIndex = 7;
            this.processListTotalLabel.Text = "0/0";
            this.processListTotalLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // searchTextBoxPlaceholder
            // 
            this.searchTextBoxPlaceholder.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.searchTextBoxPlaceholder.Font = new System.Drawing.Font("Segoe UI", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.searchTextBoxPlaceholder.Location = new System.Drawing.Point(260, 376);
            this.searchTextBoxPlaceholder.Name = "searchTextBoxPlaceholder";
            this.searchTextBoxPlaceholder.Size = new System.Drawing.Size(270, 23);
            this.searchTextBoxPlaceholder.TabIndex = 8;
            this.searchTextBoxPlaceholder.Text = "Component.SearchTextBox\r\n### DO NOT REMOVE ###";
            this.searchTextBoxPlaceholder.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ProcessDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(779, 414);
            this.Controls.Add(this.searchTextBoxPlaceholder);
            this.Controls.Add(this.processListTotalLabel);
            this.Controls.Add(this.confirmButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.is64bitText);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.processesListView);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "ProcessDialog";
            this.Text = "Process Selector";
            this.Load += new System.EventHandler(this.ProcessDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.processDialogBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ListView processesListView;
        private System.Windows.Forms.BindingSource processDialogBindingSource;
        private System.Windows.Forms.ColumnHeader dummyHeader;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox is64bitText;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button confirmButton;
        private System.Windows.Forms.Label processListTotalLabel;
        private System.Windows.Forms.Label searchTextBoxPlaceholder;
    }
}