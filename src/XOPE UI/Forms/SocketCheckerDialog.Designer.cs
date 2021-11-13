
namespace XOPE_UI.Forms
{
    partial class SocketCheckerDialog
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
            this.socketCheckBtn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.socketIdTextBox = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.socketIdTextBox)).BeginInit();
            this.SuspendLayout();
            // 
            // socketCheckBtn
            // 
            this.socketCheckBtn.Location = new System.Drawing.Point(63, 38);
            this.socketCheckBtn.Name = "socketCheckBtn";
            this.socketCheckBtn.Size = new System.Drawing.Size(75, 23);
            this.socketCheckBtn.TabIndex = 1;
            this.socketCheckBtn.Text = "Check";
            this.socketCheckBtn.UseVisualStyleBackColor = true;
            this.socketCheckBtn.Click += new System.EventHandler(this.socketCheckBtn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(35, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Socket ID";
            // 
            // socketIdTextBox
            // 
            this.socketIdTextBox.Location = new System.Drawing.Point(96, 12);
            this.socketIdTextBox.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.socketIdTextBox.Name = "socketIdTextBox";
            this.socketIdTextBox.Size = new System.Drawing.Size(79, 20);
            this.socketIdTextBox.TabIndex = 19;
            // 
            // SocketChecker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(213, 72);
            this.Controls.Add(this.socketIdTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.socketCheckBtn);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SocketChecker";
            this.Text = "Socket Checker";
            ((System.ComponentModel.ISupportInitialize)(this.socketIdTextBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button socketCheckBtn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown socketIdTextBox;
    }
}