
namespace XOPE_UI.Forms
{
    partial class ScriptLoadingDialog
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
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.statusTextbox = new System.Windows.Forms.TextBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(38, 38);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(122, 23);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar.TabIndex = 0;
            // 
            // statusTextbox
            // 
            this.statusTextbox.Location = new System.Drawing.Point(38, 12);
            this.statusTextbox.Name = "statusTextbox";
            this.statusTextbox.ReadOnly = true;
            this.statusTextbox.Size = new System.Drawing.Size(122, 20);
            this.statusTextbox.TabIndex = 1;
            this.statusTextbox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(58, 67);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(82, 23);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // ScriptLoadingDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(211, 100);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.statusTextbox);
            this.Controls.Add(this.progressBar);
            this.Name = "ScriptLoadingDialog";
            this.Text = "Loading Script";
            this.Load += new System.EventHandler(this.ScriptLoadingDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.TextBox statusTextbox;
        private System.Windows.Forms.Button cancelButton;
    }
}