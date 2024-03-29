﻿namespace XOPE_UI.View.Component
{
    partial class PacketListView
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
            this.components = new System.ComponentModel.Container();
            this.captureListView = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader6 = new System.Windows.Forms.ColumnHeader();
            this.packetItemContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.replayDoubleClickToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.underlyingEventToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.packetItemContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // captureListView
            // 
            this.captureListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.captureListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.captureListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader5,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader6});
            this.captureListView.FullRowSelect = true;
            this.captureListView.GridLines = true;
            this.captureListView.Location = new System.Drawing.Point(0, 0);
            this.captureListView.Margin = new System.Windows.Forms.Padding(0);
            this.captureListView.MultiSelect = false;
            this.captureListView.Name = "captureListView";
            this.captureListView.Size = new System.Drawing.Size(840, 316);
            this.captureListView.TabIndex = 1;
            this.captureListView.UseCompatibleStateImageBehavior = false;
            this.captureListView.View = System.Windows.Forms.View.Details;
            this.captureListView.VirtualMode = true;
            this.captureListView.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.captureListView_ItemSelectionChanged);
            this.captureListView.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.captureListView_RetrieveVirtualItem);
            this.captureListView.DoubleClick += new System.EventHandler(this.captureListView_DoubleClick);
            this.captureListView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.captureListView_MouseClick);
            this.captureListView.Resize += new System.EventHandler(this.captureListView_Resize);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "#";
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Type";
            this.columnHeader5.Width = 80;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Length";
            this.columnHeader2.Width = 110;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Data";
            this.columnHeader3.Width = 400;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "SocketID";
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Modified";
            this.columnHeader6.Width = 120;
            // 
            // packetItemContextMenuStrip
            // 
            this.packetItemContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.replayDoubleClickToolStripMenuItem,
            this.toolStripSeparator1,
            this.underlyingEventToolStripMenuItem});
            this.packetItemContextMenuStrip.Name = "packetItemContextMenuStrip";
            this.packetItemContextMenuStrip.Size = new System.Drawing.Size(188, 54);
            // 
            // replayDoubleClickToolStripMenuItem
            // 
            this.replayDoubleClickToolStripMenuItem.Name = "replayDoubleClickToolStripMenuItem";
            this.replayDoubleClickToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.replayDoubleClickToolStripMenuItem.Text = "Replay [Double Click]";
            this.replayDoubleClickToolStripMenuItem.Click += new System.EventHandler(this.replayDoubleClickToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(184, 6);
            // 
            // underlyingEventToolStripMenuItem
            // 
            this.underlyingEventToolStripMenuItem.Name = "underlyingEventToolStripMenuItem";
            this.underlyingEventToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.underlyingEventToolStripMenuItem.Text = "Underlying Event";
            this.underlyingEventToolStripMenuItem.Click += new System.EventHandler(this.underlyingEventToolStripMenuItem_Click);
            // 
            // PacketListView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.captureListView);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "PacketListView";
            this.Size = new System.Drawing.Size(840, 316);
            this.packetItemContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView captureListView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ContextMenuStrip packetItemContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem underlyingEventToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem replayDoubleClickToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    }
}
