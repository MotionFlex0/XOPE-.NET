﻿
namespace XOPE_UI.View
{
    partial class PacketEditorReplayDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.closeButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.addToListButton = new System.Windows.Forms.Button();
            this.replayButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.delayTimerTextBox = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.socketIdTextBox = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.hexEditorPlaceholder = new System.Windows.Forms.Label();
            this.replayProgressLabel = new System.Windows.Forms.Label();
            this.socketSelectorButton = new System.Windows.Forms.Button();
            this.instructionLabel = new System.Windows.Forms.Label();
            this.packetTypeComboBox = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.delayTimerTextBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.socketIdTextBox)).BeginInit();
            this.SuspendLayout();
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.Location = new System.Drawing.Point(664, 368);
            this.closeButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(88, 27);
            this.closeButton.TabIndex = 9;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 20);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 15);
            this.label1.TabIndex = 11;
            this.label1.Text = "SocketID";
            // 
            // addToListButton
            // 
            this.addToListButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.addToListButton.Enabled = false;
            this.addToListButton.Location = new System.Drawing.Point(14, 368);
            this.addToListButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.addToListButton.Name = "addToListButton";
            this.addToListButton.Size = new System.Drawing.Size(98, 27);
            this.addToListButton.TabIndex = 12;
            this.addToListButton.Text = "Add to List";
            this.addToListButton.UseVisualStyleBackColor = true;
            this.addToListButton.Click += new System.EventHandler(this.addToListButton_Click);
            // 
            // replayButton
            // 
            this.replayButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.replayButton.Location = new System.Drawing.Point(568, 12);
            this.replayButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.replayButton.Name = "replayButton";
            this.replayButton.Size = new System.Drawing.Size(88, 27);
            this.replayButton.TabIndex = 14;
            this.replayButton.Text = "Replay";
            this.replayButton.UseVisualStyleBackColor = true;
            this.replayButton.Click += new System.EventHandler(this.replayButton_Click);
            // 
            // stopButton
            // 
            this.stopButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.stopButton.Enabled = false;
            this.stopButton.Location = new System.Drawing.Point(663, 12);
            this.stopButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(88, 27);
            this.stopButton.TabIndex = 15;
            this.stopButton.Text = "Stop";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // delayTimerTextBox
            // 
            this.delayTimerTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.delayTimerTextBox.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.delayTimerTextBox.Location = new System.Drawing.Point(356, 16);
            this.delayTimerTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.delayTimerTextBox.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.delayTimerTextBox.Name = "delayTimerTextBox";
            this.delayTimerTextBox.Size = new System.Drawing.Size(72, 23);
            this.delayTimerTextBox.TabIndex = 16;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(304, 20);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 15);
            this.label2.TabIndex = 17;
            this.label2.Text = "Delay by";
            // 
            // socketIdTextBox
            // 
            this.socketIdTextBox.Location = new System.Drawing.Point(69, 16);
            this.socketIdTextBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.socketIdTextBox.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.socketIdTextBox.Name = "socketIdTextBox";
            this.socketIdTextBox.Size = new System.Drawing.Size(92, 23);
            this.socketIdTextBox.TabIndex = 18;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(426, 20);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(23, 15);
            this.label3.TabIndex = 19;
            this.label3.Text = "ms";
            // 
            // hexEditorPlaceholder
            // 
            this.hexEditorPlaceholder.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.hexEditorPlaceholder.Location = new System.Drawing.Point(14, 51);
            this.hexEditorPlaceholder.Name = "hexEditorPlaceholder";
            this.hexEditorPlaceholder.Size = new System.Drawing.Size(737, 310);
            this.hexEditorPlaceholder.TabIndex = 20;
            this.hexEditorPlaceholder.Text = "WpfHexaEditor.HexEditor\r\n### DO NOT REMOVE ###";
            this.hexEditorPlaceholder.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // replayProgressLabel
            // 
            this.replayProgressLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.replayProgressLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.replayProgressLabel.Location = new System.Drawing.Point(460, 13);
            this.replayProgressLabel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.replayProgressLabel.Name = "replayProgressLabel";
            this.replayProgressLabel.Size = new System.Drawing.Size(100, 25);
            this.replayProgressLabel.TabIndex = 21;
            this.replayProgressLabel.Text = "0.00ms";
            this.replayProgressLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.replayProgressLabel.Visible = false;
            // 
            // socketSelectorButton
            // 
            this.socketSelectorButton.Location = new System.Drawing.Point(163, 16);
            this.socketSelectorButton.Name = "socketSelectorButton";
            this.socketSelectorButton.Size = new System.Drawing.Size(30, 23);
            this.socketSelectorButton.TabIndex = 22;
            this.socketSelectorButton.Text = "lll";
            this.socketSelectorButton.UseVisualStyleBackColor = true;
            this.socketSelectorButton.Click += new System.EventHandler(this.socketSelectorButton_Click);
            // 
            // instructionLabel
            // 
            this.instructionLabel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.instructionLabel.BackColor = System.Drawing.SystemColors.Control;
            this.instructionLabel.Location = new System.Drawing.Point(247, 363);
            this.instructionLabel.Name = "instructionLabel";
            this.instructionLabel.Size = new System.Drawing.Size(262, 42);
            this.instructionLabel.TabIndex = 23;
            this.instructionLabel.Text = "Insert bytes by pressing any character at EOF\r\nPress DELETE to remove a byte";
            this.instructionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // packetTypeComboBox
            // 
            this.packetTypeComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.packetTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.packetTypeComboBox.FormattingEnabled = true;
            this.packetTypeComboBox.Items.AddRange(new object[] {
            "[WSA]SEND",
            "[WSA]RECV"});
            this.packetTypeComboBox.Location = new System.Drawing.Point(119, 370);
            this.packetTypeComboBox.Name = "packetTypeComboBox";
            this.packetTypeComboBox.Size = new System.Drawing.Size(121, 23);
            this.packetTypeComboBox.TabIndex = 24;
            // 
            // PacketEditorReplayDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(765, 408);
            this.Controls.Add(this.packetTypeComboBox);
            this.Controls.Add(this.instructionLabel);
            this.Controls.Add(this.replayProgressLabel);
            this.Controls.Add(this.hexEditorPlaceholder);
            this.Controls.Add(this.socketIdTextBox);
            this.Controls.Add(this.delayTimerTextBox);
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.replayButton);
            this.Controls.Add(this.addToListButton);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.socketSelectorButton);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "PacketEditorReplayDialog";
            this.Text = "Packet Replay";
            this.VisibleChanged += new System.EventHandler(this.PacketEditorReplayDialog_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.delayTimerTextBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.socketIdTextBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button addToListButton;
        private System.Windows.Forms.Button replayButton;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.NumericUpDown delayTimerTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown socketIdTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label hexEditorPlaceholder;
        private System.Windows.Forms.Label replayProgressLabel;
        private System.Windows.Forms.Button socketSelectorButton;
        private System.Windows.Forms.Label instructionLabel;
        private System.Windows.Forms.ComboBox packetTypeComboBox;
    }
}