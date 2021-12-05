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

            this.spyManager.ConnectionEstablished += SpyManager_OnNewConnection;
            this.spyManager.ConnectionClosed += SpyManager_OnCloseConnection;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);

            this.spyManager.ConnectionEstablished -= SpyManager_OnNewConnection;
            this.spyManager.ConnectionClosed -= SpyManager_OnCloseConnection;
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
                item.Tag = c.SocketId;
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
            this.Invoke(() =>
            {

                ListViewItem item = null;
                foreach (ListViewItem lvi in connectionListView.Items)
                {
                    if (lvi.Tag is int socketId && socketId == c.SocketId)
                    {
                        item = lvi;
                        item.SubItems[0].Text = c.IPFamily == AddressFamily.InterNetwork ? "IPv4" : "IPv6";
                        item.SubItems[1].Text = c.IP.ToString();
                        item.SubItems[2].Text = c.Port.ToString();
                        item.SubItems[3].Text = c.SocketStatus.ToString();
                        break;
                    }
                }

                if (item == null)
                {
                    item = new ListViewItem(c.SocketId.ToString());
                    item.Name = c.SocketId.ToString();
                    item.SubItems.Add(c.IPFamily == AddressFamily.InterNetwork ? "IPv4" : "IPv6");
                    item.SubItems.Add(c.IP.ToString());
                    item.SubItems.Add(c.Port.ToString());
                    item.SubItems.Add(c.SocketStatus.ToString());
                    connectionListView.Items.Add(item);
                }

                item.BackColor = Color.LightGreen;

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

            });
        }

        private void SpyManager_OnCloseConnection(object sender, Connection c)
        {
            this.Invoke(() =>
            {
                ListViewItem item = connectionListView.Items[c.SocketId.ToString()];
                if (item == null)
                    return;

                item.BackColor = Color.Red;

                Timer timer = new Timer();
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
            });
        }
    }
}
