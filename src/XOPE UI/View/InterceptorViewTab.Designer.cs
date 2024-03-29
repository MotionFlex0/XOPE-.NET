﻿namespace XOPE_UI.View
{
    partial class InterceptorViewTab
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
            this.forwardButton = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.dropPacketButton = new System.Windows.Forms.Button();
            this.hexEditorPlaceholder = new System.Windows.Forms.Label();
            this.jobIdLabel = new System.Windows.Forms.Label();
            this.packetTypeLabel = new System.Windows.Forms.Label();
            this.ipAddrLabel = new System.Windows.Forms.Label();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // forwardButton
            // 
            this.forwardButton.Enabled = false;
            this.forwardButton.Location = new System.Drawing.Point(3, 3);
            this.forwardButton.Name = "forwardButton";
            this.forwardButton.Size = new System.Drawing.Size(75, 23);
            this.forwardButton.TabIndex = 0;
            this.forwardButton.Text = "Forward";
            this.forwardButton.UseVisualStyleBackColor = true;
            this.forwardButton.Click += new System.EventHandler(this.forwardButton_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.flowLayoutPanel1.Controls.Add(this.forwardButton);
            this.flowLayoutPanel1.Controls.Add(this.dropPacketButton);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(327, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(176, 31);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // dropPacketButton
            // 
            this.dropPacketButton.Enabled = false;
            this.dropPacketButton.Location = new System.Drawing.Point(84, 3);
            this.dropPacketButton.Name = "dropPacketButton";
            this.dropPacketButton.Size = new System.Drawing.Size(88, 23);
            this.dropPacketButton.TabIndex = 1;
            this.dropPacketButton.Text = "Drop Packet";
            this.dropPacketButton.UseVisualStyleBackColor = true;
            this.dropPacketButton.Click += new System.EventHandler(this.dropPacketButton_Click);
            // 
            // hexEditorPlaceholder
            // 
            this.hexEditorPlaceholder.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hexEditorPlaceholder.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.hexEditorPlaceholder.Location = new System.Drawing.Point(3, 37);
            this.hexEditorPlaceholder.Name = "hexEditorPlaceholder";
            this.hexEditorPlaceholder.Size = new System.Drawing.Size(869, 433);
            this.hexEditorPlaceholder.TabIndex = 22;
            this.hexEditorPlaceholder.Text = "WpfHexaEditor.HexEditor\r\n### DO NOT REMOVE ###";
            this.hexEditorPlaceholder.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // jobIdLabel
            // 
            this.jobIdLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.jobIdLabel.Location = new System.Drawing.Point(3, 470);
            this.jobIdLabel.Name = "jobIdLabel";
            this.jobIdLabel.Size = new System.Drawing.Size(869, 19);
            this.jobIdLabel.TabIndex = 23;
            this.jobIdLabel.Text = "~";
            this.jobIdLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // packetTypeLabel
            // 
            this.packetTypeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.packetTypeLabel.Location = new System.Drawing.Point(3, 489);
            this.packetTypeLabel.Name = "packetTypeLabel";
            this.packetTypeLabel.Size = new System.Drawing.Size(869, 19);
            this.packetTypeLabel.TabIndex = 24;
            this.packetTypeLabel.Text = "~";
            this.packetTypeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ipAddrLabel
            // 
            this.ipAddrLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ipAddrLabel.Location = new System.Drawing.Point(3, 508);
            this.ipAddrLabel.Name = "ipAddrLabel";
            this.ipAddrLabel.Size = new System.Drawing.Size(869, 19);
            this.ipAddrLabel.TabIndex = 25;
            this.ipAddrLabel.Text = "~";
            this.ipAddrLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // InterceptorViewTab
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ipAddrLabel);
            this.Controls.Add(this.packetTypeLabel);
            this.Controls.Add(this.jobIdLabel);
            this.Controls.Add(this.hexEditorPlaceholder);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "InterceptorViewTab";
            this.Size = new System.Drawing.Size(875, 527);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button forwardButton;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button dropPacketButton;
        private System.Windows.Forms.Label hexEditorPlaceholder;
        private System.Windows.Forms.Label jobIdLabel;
        private System.Windows.Forms.Label packetTypeLabel;
        private System.Windows.Forms.Label ipAddrLabel;
    }
}
