
namespace XOPE_UI.View
{
    partial class ScriptManagerDialog
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
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("Name");
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("Status");
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("Running Time");
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("Started At");
            System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem("File Location");
            this.runningScriptListView = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.stopButton = new System.Windows.Forms.Button();
            this.startButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.stoppedScriptListView = new System.Windows.Forms.ListView();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.scriptInfoListView = new System.Windows.Forms.ListView();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // runningScriptListView
            // 
            this.runningScriptListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.runningScriptListView.Location = new System.Drawing.Point(7, 22);
            this.runningScriptListView.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.runningScriptListView.Name = "runningScriptListView";
            this.runningScriptListView.Size = new System.Drawing.Size(336, 211);
            this.runningScriptListView.TabIndex = 0;
            this.runningScriptListView.UseCompatibleStateImageBehavior = false;
            this.runningScriptListView.View = System.Windows.Forms.View.Details;
            this.runningScriptListView.SelectedIndexChanged += new System.EventHandler(this.scriptListView_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Scripts";
            this.columnHeader1.Width = 277;
            // 
            // stopButton
            // 
            this.stopButton.Location = new System.Drawing.Point(622, 136);
            this.stopButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(88, 27);
            this.stopButton.TabIndex = 1;
            this.stopButton.Text = "Stop";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // startButton
            // 
            this.startButton.Enabled = false;
            this.startButton.Location = new System.Drawing.Point(448, 136);
            this.startButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(88, 27);
            this.startButton.TabIndex = 3;
            this.startButton.Text = "Start";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.runningScriptListView);
            this.groupBox1.Location = new System.Drawing.Point(14, 14);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox1.Size = new System.Drawing.Size(354, 240);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Running Scripts";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.stoppedScriptListView);
            this.groupBox2.Location = new System.Drawing.Point(14, 277);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox2.Size = new System.Drawing.Size(354, 240);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Stopped Scripts";
            // 
            // stoppedScriptListView
            // 
            this.stoppedScriptListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2});
            this.stoppedScriptListView.Location = new System.Drawing.Point(7, 22);
            this.stoppedScriptListView.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.stoppedScriptListView.Name = "stoppedScriptListView";
            this.stoppedScriptListView.Size = new System.Drawing.Size(336, 211);
            this.stoppedScriptListView.TabIndex = 0;
            this.stoppedScriptListView.UseCompatibleStateImageBehavior = false;
            this.stoppedScriptListView.View = System.Windows.Forms.View.Details;
            this.stoppedScriptListView.SelectedIndexChanged += new System.EventHandler(this.scriptListView_SelectedIndexChanged);
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Scripts";
            this.columnHeader2.Width = 277;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.scriptInfoListView);
            this.groupBox3.Location = new System.Drawing.Point(374, 170);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox3.Size = new System.Drawing.Size(404, 227);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Script Information";
            // 
            // scriptInfoListView
            // 
            this.scriptInfoListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader4});
            this.scriptInfoListView.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3,
            listViewItem4,
            listViewItem5});
            this.scriptInfoListView.Location = new System.Drawing.Point(7, 22);
            this.scriptInfoListView.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.scriptInfoListView.Name = "scriptInfoListView";
            this.scriptInfoListView.Size = new System.Drawing.Size(387, 198);
            this.scriptInfoListView.TabIndex = 0;
            this.scriptInfoListView.UseCompatibleStateImageBehavior = false;
            this.scriptInfoListView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "(Name)";
            this.columnHeader3.Width = 88;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Value";
            this.columnHeader4.Width = 232;
            // 
            // ScriptManagerDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 546);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.stopButton);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "ScriptManagerDialog";
            this.Text = "ScriptManagerDialog";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ScriptManagerDialog_FormClosed);
            this.Load += new System.EventHandler(this.ScriptManagerDialog_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView runningScriptListView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListView stoppedScriptListView;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ListView scriptInfoListView;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
    }
}