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
using XOPE_UI.Spy;

namespace XOPE_UI.Forms
{
    public partial class ActiveConnectionsDialog : Form
    {
        SpyData spyData;
        public ActiveConnectionsDialog(SpyData sd)
        {
            InitializeComponent();
            spyData = sd;
        }

        public void UpdateActiveList()
        {
            connectionListView.Items.Clear();
            foreach (Connection c in spyData.Connections)
            {
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
    }
}
