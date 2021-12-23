
namespace XOPE_UI.Forms
{
    partial class FilterEditorDialog
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
            this.beforeGroupBox = new System.Windows.Forms.GroupBox();
            this.beforeHexEditorPlaceholder = new System.Windows.Forms.Label();
            this.afterGroupBox = new System.Windows.Forms.GroupBox();
            this.afterHexEditorPlaceholder = new System.Windows.Forms.Label();
            this.acceptButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.socketIdTextBox = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.instructionLabel = new System.Windows.Forms.Label();
            this.packetTypeComboBox = new System.Windows.Forms.ComboBox();
            this.beforeGroupBox.SuspendLayout();
            this.afterGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.socketIdTextBox)).BeginInit();
            this.SuspendLayout();
            // 
            // beforeGroupBox
            // 
            this.beforeGroupBox.Controls.Add(this.beforeHexEditorPlaceholder);
            this.beforeGroupBox.Location = new System.Drawing.Point(14, 62);
            this.beforeGroupBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.beforeGroupBox.Name = "beforeGroupBox";
            this.beforeGroupBox.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.beforeGroupBox.Size = new System.Drawing.Size(685, 156);
            this.beforeGroupBox.TabIndex = 1;
            this.beforeGroupBox.TabStop = false;
            this.beforeGroupBox.Text = "Before";
            // 
            // beforeHexEditorPlaceholder
            // 
            this.beforeHexEditorPlaceholder.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.beforeHexEditorPlaceholder.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.beforeHexEditorPlaceholder.Location = new System.Drawing.Point(7, 19);
            this.beforeHexEditorPlaceholder.Name = "beforeHexEditorPlaceholder";
            this.beforeHexEditorPlaceholder.Size = new System.Drawing.Size(671, 134);
            this.beforeHexEditorPlaceholder.TabIndex = 21;
            this.beforeHexEditorPlaceholder.Text = "WpfHexaEditor.HexEditor\r\n### DO NOT REMOVE ###";
            this.beforeHexEditorPlaceholder.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // afterGroupBox
            // 
            this.afterGroupBox.Controls.Add(this.afterHexEditorPlaceholder);
            this.afterGroupBox.Location = new System.Drawing.Point(14, 224);
            this.afterGroupBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.afterGroupBox.Name = "afterGroupBox";
            this.afterGroupBox.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.afterGroupBox.Size = new System.Drawing.Size(685, 156);
            this.afterGroupBox.TabIndex = 2;
            this.afterGroupBox.TabStop = false;
            this.afterGroupBox.Text = "After";
            // 
            // afterHexEditorPlaceholder
            // 
            this.afterHexEditorPlaceholder.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.afterHexEditorPlaceholder.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.afterHexEditorPlaceholder.Location = new System.Drawing.Point(7, 19);
            this.afterHexEditorPlaceholder.Name = "afterHexEditorPlaceholder";
            this.afterHexEditorPlaceholder.Size = new System.Drawing.Size(671, 134);
            this.afterHexEditorPlaceholder.TabIndex = 22;
            this.afterHexEditorPlaceholder.Text = "WpfHexaEditor.HexEditor\r\n### DO NOT REMOVE ###";
            this.afterHexEditorPlaceholder.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // acceptButton
            // 
            this.acceptButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.acceptButton.Location = new System.Drawing.Point(508, 407);
            this.acceptButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.acceptButton.Name = "acceptButton";
            this.acceptButton.Size = new System.Drawing.Size(88, 27);
            this.acceptButton.TabIndex = 3;
            this.acceptButton.Text = "Accept";
            this.acceptButton.UseVisualStyleBackColor = true;
            this.acceptButton.Click += new System.EventHandler(this.acceptButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.Location = new System.Drawing.Point(604, 407);
            this.cancelButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(88, 27);
            this.cancelButton.TabIndex = 4;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 27);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 15);
            this.label1.TabIndex = 6;
            this.label1.Text = "Name";
            // 
            // nameTextBox
            // 
            this.nameTextBox.Location = new System.Drawing.Point(75, 22);
            this.nameTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Size = new System.Drawing.Size(116, 23);
            this.nameTextBox.TabIndex = 8;
            // 
            // socketIdTextBox
            // 
            this.socketIdTextBox.Location = new System.Drawing.Point(290, 22);
            this.socketIdTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.socketIdTextBox.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.socketIdTextBox.Name = "socketIdTextBox";
            this.socketIdTextBox.Size = new System.Drawing.Size(92, 23);
            this.socketIdTextBox.TabIndex = 20;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(223, 27);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 15);
            this.label2.TabIndex = 19;
            this.label2.Text = "SocketID";
            // 
            // instructionLabel
            // 
            this.instructionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.instructionLabel.Location = new System.Drawing.Point(21, 392);
            this.instructionLabel.Name = "instructionLabel";
            this.instructionLabel.Size = new System.Drawing.Size(262, 42);
            this.instructionLabel.TabIndex = 21;
            this.instructionLabel.Text = "Insert bytes by pressing any character at EOF\r\nPress DELETE to remove a byte";
            // 
            // packetTypeComboBox
            // 
            this.packetTypeComboBox.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.packetTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.packetTypeComboBox.FormattingEnabled = true;
            this.packetTypeComboBox.Location = new System.Drawing.Point(571, 22);
            this.packetTypeComboBox.Name = "packetTypeComboBox";
            this.packetTypeComboBox.Size = new System.Drawing.Size(121, 23);
            this.packetTypeComboBox.TabIndex = 22;
            // 
            // FilterEditorDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(704, 447);
            this.Controls.Add(this.packetTypeComboBox);
            this.Controls.Add(this.instructionLabel);
            this.Controls.Add(this.socketIdTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.nameTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.acceptButton);
            this.Controls.Add(this.afterGroupBox);
            this.Controls.Add(this.beforeGroupBox);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "FilterEditorDialog";
            this.Text = "Filter Dialog";
            this.Load += new System.EventHandler(this.FilterEditorDialog_Load);
            this.beforeGroupBox.ResumeLayout(false);
            this.afterGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.socketIdTextBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.GroupBox beforeGroupBox;
        private System.Windows.Forms.GroupBox afterGroupBox;
        private System.Windows.Forms.Button acceptButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.NumericUpDown socketIdTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label beforeHexEditorPlaceholder;
        private System.Windows.Forms.Label afterHexEditorPlaceholder;
        private System.Windows.Forms.Label instructionLabel;
        private System.Windows.Forms.ComboBox packetTypeComboBox;
    }
}