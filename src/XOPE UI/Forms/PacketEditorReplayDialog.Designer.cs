
namespace XOPE_UI.Forms
{
    partial class PacketEditorReplayDialog
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
            this.elementHost1 = new System.Windows.Forms.Integration.ElementHost();
            this.hexEditor = new WpfHexaEditor.HexEditor();
            this.closeButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.addToListButton = new System.Windows.Forms.Button();
            this.replayButton = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.waitTimerTextBox = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.socketIdTextBox = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.waitTimerTextBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.socketIdTextBox)).BeginInit();
            this.SuspendLayout();
            // 
            // elementHost1
            // 
            this.elementHost1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.elementHost1.Location = new System.Drawing.Point(12, 44);
            this.elementHost1.Name = "elementHost1";
            this.elementHost1.Size = new System.Drawing.Size(632, 269);
            this.elementHost1.TabIndex = 8;
            this.elementHost1.Text = "elementHost1";
            this.elementHost1.Child = this.hexEditor;
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.Location = new System.Drawing.Point(569, 319);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 9;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "SocketID";
            // 
            // addToListButton
            // 
            this.addToListButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.addToListButton.Location = new System.Drawing.Point(12, 319);
            this.addToListButton.Name = "addToListButton";
            this.addToListButton.Size = new System.Drawing.Size(84, 23);
            this.addToListButton.TabIndex = 12;
            this.addToListButton.Text = "Add to List";
            this.addToListButton.UseVisualStyleBackColor = true;
            // 
            // replayButton
            // 
            this.replayButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.replayButton.Location = new System.Drawing.Point(488, 14);
            this.replayButton.Name = "replayButton";
            this.replayButton.Size = new System.Drawing.Size(75, 23);
            this.replayButton.TabIndex = 14;
            this.replayButton.Text = "Replay";
            this.replayButton.UseVisualStyleBackColor = true;
            this.replayButton.Click += new System.EventHandler(this.replayButton_Click);
            // 
            // button2
            // 
            this.button2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.button2.Location = new System.Drawing.Point(569, 14);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 15;
            this.button2.Text = "Stop";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // waitTimerTextBox
            // 
            this.waitTimerTextBox.Location = new System.Drawing.Point(231, 14);
            this.waitTimerTextBox.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.waitTimerTextBox.Name = "waitTimerTextBox";
            this.waitTimerTextBox.Size = new System.Drawing.Size(62, 20);
            this.waitTimerTextBox.TabIndex = 16;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(170, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "Wait Time";
            // 
            // socketIdTextBox
            // 
            this.socketIdTextBox.Location = new System.Drawing.Point(70, 14);
            this.socketIdTextBox.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.socketIdTextBox.Name = "socketIdTextBox";
            this.socketIdTextBox.Size = new System.Drawing.Size(79, 20);
            this.socketIdTextBox.TabIndex = 18;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(294, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(20, 13);
            this.label3.TabIndex = 19;
            this.label3.Text = "ms";
            // 
            // PacketEditorReplayDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(656, 354);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.socketIdTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.waitTimerTextBox);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.replayButton);
            this.Controls.Add(this.addToListButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.elementHost1);
            this.Controls.Add(this.closeButton);
            this.Name = "PacketEditorReplayDialog";
            this.Text = "PacketEditorReplayDialog";
            this.VisibleChanged += new System.EventHandler(this.PacketEditorReplayDialog_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.waitTimerTextBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.socketIdTextBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Integration.ElementHost elementHost1;
        private WpfHexaEditor.HexEditor hexEditor;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button addToListButton;
        private System.Windows.Forms.Button replayButton;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.NumericUpDown waitTimerTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown socketIdTextBox;
        private System.Windows.Forms.Label label3;
    }
}