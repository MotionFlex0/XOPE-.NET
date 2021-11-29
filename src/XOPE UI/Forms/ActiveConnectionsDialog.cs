using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Sockets;
using System.Windows.Forms;
using XOPE_UI.Definitions;
using XOPE_UI.Spy;

namespace XOPE_UI.Forms
{
    public partial class ActiveConnectionsDialog : Form
    {
        SpyManager spyManager;
        public ActiveConnectionsDialog(SpyManager spyManager)
        {
            InitializeComponent();
            this.spyManager = spyManager;

            this.spyManager.OnNewConnection += (s, e) => this.Invoke(() => SpyManager_OnNewConnection(s, e));
            this.spyManager.OnCloseConnection += (s, e) => this.Invoke(() => SpyManager_OnCloseConnection(s, e));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);

            this.spyManager.OnNewConnection -= SpyManager_OnNewConnection;
            this.spyManager.OnCloseConnection -= SpyManager_OnCloseConnection;
        }

        public void UpdateActiveList()
        {
            connectionListView.Items.Clear();
            foreach (KeyValuePair<int, Connection> kvp in spyManager.SpyData.Connections)
            {
                Connection c = kvp.Value;

                ListViewItem item = new ListViewItem(c.SocketId.ToString());
                item.SubItems.Add(c.IPFamily == AddressFamily.InterNetwork ? "IPv4" : "IPv6");
                item.SubItems.Add(c.IP.ToString());
                item.SubItems.Add(c.Port.ToString());
                item.SubItems.Add(c.SocketStatus.ToString());
                connectionListView.Items.Add(item);
            }
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            UpdateActiveList();
        }

        private void SpyManager_OnNewConnection(object sender, Connection c)
        {
            ListViewItem item = new ListViewItem(c.SocketId.ToString());
            item.Name = c.SocketId.ToString();
            item.SubItems.Add(c.IPFamily == AddressFamily.InterNetwork ? "IPv4" : "IPv6");
            item.SubItems.Add(c.IP.ToString());
            item.SubItems.Add(c.Port.ToString());
            item.SubItems.Add(c.SocketStatus.ToString());

            item.BackColor = Color.LightGreen;
            connectionListView.Items.Add(item);

            System.Windows.Forms.Timer timer = new Timer();
            timer.Interval = 5000;
            timer.Tick += (object sender, EventArgs e) =>
            {
                if (this.Visible)
                {
                    timer.Stop();
                    item.BackColor = Color.White;
                }
            };

            timer.Start();

        }

        private void SpyManager_OnCloseConnection(object sender, Connection c)
        {
            ListViewItem item = connectionListView.Items[c.SocketId.ToString()];
            if (item == null)
                return;

            item.BackColor = Color.Red;

            System.Windows.Forms.Timer timer = new Timer();
            timer.Interval = 5000;
            timer.Tick += (object sender, EventArgs e) =>
            {
                if (this.Visible)
                {
                    timer.Stop();
                    item.Remove();
                }
            };

            timer.Start();
        }
    }
}
