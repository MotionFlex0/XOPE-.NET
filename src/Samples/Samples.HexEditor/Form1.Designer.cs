
namespace Samples.HexEditor
{
    partial class Form1
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
            this.hexEditor = new XOPE_UI.Forms.Component.HexEditor();
            this.randomiseData = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // hexEditor
            // 
            this.hexEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hexEditor.CellBackColor = System.Drawing.Color.White;
            this.hexEditor.CellHoverBackColor = System.Drawing.Color.Cyan;
            this.hexEditor.Location = new System.Drawing.Point(12, 26);
            this.hexEditor.Name = "hexEditor";
            this.hexEditor.SelectionBackColor = System.Drawing.Color.Blue;
            this.hexEditor.SelectionForeColor = System.Drawing.Color.White;
            this.hexEditor.Size = new System.Drawing.Size(804, 185);
            this.hexEditor.TabIndex = 0;
            // 
            // randomiseData
            // 
            this.randomiseData.Location = new System.Drawing.Point(362, 231);
            this.randomiseData.Name = "randomiseData";
            this.randomiseData.Size = new System.Drawing.Size(111, 23);
            this.randomiseData.TabIndex = 1;
            this.randomiseData.Text = "Randomise Data";
            this.randomiseData.UseVisualStyleBackColor = true;
            this.randomiseData.Click += new System.EventHandler(this.randomiseData_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(828, 291);
            this.Controls.Add(this.randomiseData);
            this.Controls.Add(this.hexEditor);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private XOPE_UI.Forms.Component.HexEditor hexEditor;
        private System.Windows.Forms.Button randomiseData;
    }
}

