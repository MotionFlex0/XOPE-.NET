
namespace XOPE_UI.Test.UI
{
    partial class HexEditorForm
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
            this.hexEditor1 = new XOPE_UI.Forms.Component.HexEditor();
            this.SuspendLayout();
            // 
            // hexEditor1
            // 
            this.hexEditor1.Location = new System.Drawing.Point(127, 22);
            this.hexEditor1.Name = "hexEditor1";
            this.hexEditor1.Size = new System.Drawing.Size(549, 404);
            this.hexEditor1.TabIndex = 0;
            // 
            // HexEditorFormTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.hexEditor1);
            this.Name = "HexEditorFormTest";
            this.Text = "HexEditorFormTest";
            this.ResumeLayout(false);

        }

        #endregion

        private Forms.Component.HexEditor hexEditor1;
    }
}