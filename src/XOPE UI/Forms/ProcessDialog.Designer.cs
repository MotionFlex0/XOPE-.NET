namespace XOPE_UI.Forms
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
            this.processesView = new System.Windows.Forms.ListView();
            this.dummyHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label1 = new System.Windows.Forms.Label();
            this.is64bitText = new System.Windows.Forms.TextBox();
            this.processDialogBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.cancelButton = new System.Windows.Forms.Button();
            this.confirmButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.processDialogBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // processesView
            // 
            this.processesView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.processesView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.dummyHeader});
            this.processesView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.processesView.HideSelection = false;
            this.processesView.Location = new System.Drawing.Point(12, 12);
            this.processesView.MultiSelect = false;
            this.processesView.Name = "processesView";
            this.processesView.Size = new System.Drawing.Size(644, 306);
            this.processesView.TabIndex = 1;
            this.processesView.UseCompatibleStateImageBehavior = false;
            this.processesView.View = System.Windows.Forms.View.Details;
            this.processesView.SelectedIndexChanged += new System.EventHandler(this.processesView_SelectedIndexChanged);
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
            this.label1.Location = new System.Drawing.Point(9, 330);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Is64BitProcess:";
            // 
            // is64bitText
            // 
            this.is64bitText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.is64bitText.Location = new System.Drawing.Point(95, 326);
            this.is64bitText.Name = "is64bitText";
            this.is64bitText.ReadOnly = true;
            this.is64bitText.Size = new System.Drawing.Size(77, 20);
            this.is64bitText.TabIndex = 3;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.Location = new System.Drawing.Point(581, 325);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 4;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // confirmButton
            // 
            this.confirmButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.confirmButton.Enabled = false;
            this.confirmButton.Location = new System.Drawing.Point(500, 325);
            this.confirmButton.Name = "confirmButton";
            this.confirmButton.Size = new System.Drawing.Size(75, 23);
            this.confirmButton.TabIndex = 5;
            this.confirmButton.Text = "Confirm";
            this.confirmButton.UseVisualStyleBackColor = true;
            this.confirmButton.Click += new System.EventHandler(this.confirmButton_Click);
            // 
            // ProcessDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(668, 359);
            this.Controls.Add(this.confirmButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.is64bitText);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.processesView);
            this.Name = "ProcessDialog";
            this.Text = "Process Selector";
            this.Load += new System.EventHandler(this.ProcessDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.processDialogBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ListView processesView;
        private System.Windows.Forms.BindingSource processDialogBindingSource;
        private System.Windows.Forms.ColumnHeader dummyHeader;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox is64bitText;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button confirmButton;
    }
}