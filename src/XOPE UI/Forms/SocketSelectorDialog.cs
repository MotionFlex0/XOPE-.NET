using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XOPE_UI.Definitions;

namespace XOPE_UI.Forms
{
    public partial class SocketSelectorDialog : Form
    {
        public int SelectedSocketId { get; set; } = 0;

        SpyManager spyManager;

        public SocketSelectorDialog(SpyManager spyManager)
        {
            InitializeComponent();

            this.spyManager = spyManager;
        }

        private void UpdateActiveList()
        {
            connectionListView.Items.Clear();
            foreach (KeyValuePair<int, Connection> kvp in spyManager.SpyData.Connections)
            {
                Connection c = kvp.Value;

                if (c.SocketStatus == Connection.Status.CLOSED)
                    continue;

                ListViewItem item = new ListViewItem(c.SocketId.ToString());
                item.SubItems.Add(c.IPFamily == AddressFamily.InterNetwork ? "IPv4" : "IPv6");
                item.SubItems.Add(c.IP.ToString());
                item.SubItems.Add(c.Port.ToString());
                item.SubItems.Add(c.SocketStatus.ToString());
                item.Tag = c.SocketId;
                connectionListView.Items.Add(item);
            }
        }

        private void SocketSelectorDialog_Load(object sender, EventArgs e)
        {
            UpdateActiveList();
        }

        private void confirmButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            SelectedSocketId = (int)this.connectionListView.SelectedItems[0].Tag;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            UpdateActiveList();
        }

        private void connectionListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (connectionListView.SelectedItems.Count > 0)
                confirmButton.Enabled = true;
            else
                confirmButton.Enabled = false;
        }

        private void connectionListView_DoubleClick(object sender, EventArgs e)
        {
            var selectedItems = this.connectionListView.SelectedItems;
            if (selectedItems.Count > 0)
                this.confirmButton.PerformClick();
        }
    }
}
