using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using XOPE_UI.Model;
using XOPE_UI.Spy;
using XOPE_UI.Spy.DispatcherMessageType;

namespace XOPE_UI.View
{
    public partial class ActiveConnectionsDialog : Form
    {
        SpyManager _spyManager;
        public ActiveConnectionsDialog(SpyManager spyManager)
        {
            InitializeComponent();
            this._spyManager = spyManager;

            this.HandleCreated += ActiveConnectionsDialog_HandleCreated;

            connectionListView
                .GetType()
                .GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                .SetValue(connectionListView, true, null);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);

            this._spyManager.ConnectionConnecting -= SpyManager_ConnectionConnecting;
            this._spyManager.ConnectionEstablished -= SpyManager_ConnectionEstablished;
            this._spyManager.ConnectionClosed -= SpyManager_ConnectionClosed;
        }

        public ListViewItem AddOrUpdateConnectionInList(Connection c)
        {
            ListViewItem item = GetListViewItemFromSocketId(c.SocketId);

            if (item != null)
            {
                item.SubItems["ip_family"].Text = c.IPFamily == AddressFamily.InterNetwork ? "IPv4" : "IPv6";
                item.SubItems["ip_address"].Text = c.DestAddress.ToString();
                item.SubItems["port"].Text = c.DestPort.ToString();
                item.SubItems["status"].Text = c.SocketStatus.ToString();
            }
            else
            {
                item = new ListViewItem(c.SocketId.ToString());
                
                item.SubItems.Add(c.IPFamily == AddressFamily.InterNetwork ? "IPv4" : "IPv6");
                item.SubItems.Add(c.DestAddress.ToString());
                item.SubItems.Add(c.DestPort.ToString());
                item.SubItems.Add(c.SocketStatus.ToString());

                // Even though connectionListView.Columns[i].Name is set in the designer, the string is empty at run-time.
                // This seems to be a bug that days back 12+ years and has yet to be fixed. Use .Tag instead
                for (int i = 0; i < item.SubItems.Count; i++)
                    item.SubItems[i].Name = connectionListView.Columns[i].Tag as string;

                connectionListView.Items.Add(item);
            }

            return item;
        }

        private void AddSingleTimerToListItem(ListViewItem item,  EventHandler tick, int interval)
        {
            Timer timer = item.Tag as Timer ?? new Timer();
            item.Tag = item.Tag ?? timer;

            if (timer.Enabled)
                timer.Stop();

            timer.Interval = interval;
            timer.Tick += (object sender, EventArgs e) =>
            {
                tick(sender, e);
                timer.Stop();
                item.Tag = null;
            };

            timer.Start();
        }

        private ListViewItem GetListViewItemFromSocketId(int socketId)
        {
            foreach (ListViewItem item in connectionListView.Items)
                if (item.Text == socketId.ToString())
                    return item;

            return null;
        }

        private void UpdateActiveList()
        {
            connectionListView.BeginUpdate();
            connectionListView.Items.Clear();
            foreach (KeyValuePair<int, Connection> kvp in _spyManager.SpyData.Connections)
            {
                Connection c = kvp.Value;
                AddOrUpdateConnectionInList(c);
            }
            connectionListView.EndUpdate();
        }

        private void ActiveConnectionsDialog_HandleCreated(object sender, EventArgs e)
        {
            this._spyManager.ConnectionConnecting += SpyManager_ConnectionConnecting;
            this._spyManager.ConnectionEstablished += SpyManager_ConnectionEstablished;
            this._spyManager.ConnectionClosed += SpyManager_ConnectionClosed;

            UpdateActiveList();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            UpdateActiveList();
        }

        private void SpyManager_ConnectionConnecting(object sender, Connection c)
        {
            this.Invoke(() =>
            {
                ListViewItem item = AddOrUpdateConnectionInList(c);

                item.BackColor = Color.Yellow;

                EventHandler tick = (object sender, EventArgs e) =>
                    item.BackColor = Color.White;

                AddSingleTimerToListItem(item, tick, 5000);
            });
        }

        private void SpyManager_ConnectionEstablished(object sender, Connection c)
        {
            this.Invoke(() =>
            {
                ListViewItem item = AddOrUpdateConnectionInList(c);

                item.BackColor = Color.LightGreen;

                EventHandler tick = (object sender, EventArgs e) =>
                    item.BackColor = Color.White;

                AddSingleTimerToListItem(item, tick, 5000);
            });
        }

        private void SpyManager_ConnectionClosed(object sender, Connection c)
        {
            this.Invoke(() =>
            {
                ListViewItem item = GetListViewItemFromSocketId(c.SocketId);
                if (item == null)
                    return;

                item.BackColor = Color.FromArgb(255, 100, 100);
                item.SubItems["status"].Text = c.SocketStatus.ToString();

                EventHandler tick = (object sender, EventArgs e) =>
                    item.Remove();

                AddSingleTimerToListItem(item, tick, 5000);
            });
        }

        private void connectionListView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                foreach (ListViewItem item in connectionListView.Items)
                {
                    if (item.Bounds.Contains(e.Location))
                    {
                        connectionContextMenu.Tag = item;
                        connectionContextMenu.Show(connectionListView, e.Location);
                        break;
                    }
                }
            }
        }

        private void dNSLookupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string padding = new string(' ', 4);

            ListViewItem item = connectionContextMenu.Tag as ListViewItem;
            IPAddress ip = IPAddress.Parse(item.SubItems["ip_address"].Text);

            if (ip.ToString() == "0.0.0.0")
                return;

            try
            {
                IPHostEntry hostEntry = Dns.GetHostEntry(ip);
                string addressList = hostEntry.AddressList.Length > 0 ? 
                    $"Address List:\n{padding}{string.Join<IPAddress>($"\n{padding}", hostEntry.AddressList)}\n\n" : 
                    "";
                string alias = hostEntry.Aliases.Length > 0 ?
                    $"Alias :\n{padding}{ string.Join($"\n{padding}", hostEntry.Aliases)}" : 
                    "";

                MessageBox.Show($"Hostname: {hostEntry.HostName}\n{addressList}{alias}");
            }
            catch (SocketException ex)
            {
                MessageBox.Show($"Error when performing DNS lookup on {ip}\n\n{ex.Message}");
            }
        }

        private void copyIPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListViewItem item = connectionContextMenu.Tag as ListViewItem;
            Clipboard.SetText(item.SubItems["ip_address"].Text);
        }

        private void copyIPPortToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListViewItem item = connectionContextMenu.Tag as ListViewItem;
            Clipboard.SetText($"{item.SubItems["ip_address"].Text}:{item.SubItems["port"].Text}");
        }

        private void copyPortToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListViewItem item = connectionContextMenu.Tag as ListViewItem;
            Clipboard.SetText(item.SubItems["port"].Text);
        }

        private void copySocketIdToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListViewItem item = connectionContextMenu.Tag as ListViewItem;
            Clipboard.SetText(item.SubItems["socket_id"].Text);
        }

        private void closeConnectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show(this, 
                "Are you sure you want to close this connection?\nThis may cause instability if the socket is in active use.",
                "Closing Socket", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (dialogResult == DialogResult.Yes)
            {
                ListViewItem item = connectionContextMenu.Tag as ListViewItem;
                _spyManager.MessageDispatcher.Send(new CloseSocket()
                {
                    SocketId = int.Parse(item.SubItems["socket_id"].Text)
                });
                this.connectionListView.SelectedItems.Clear();
            }
        }
    }
}
