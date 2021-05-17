using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XOPE_UI.Definitions;

namespace XOPE_UI.Forms.Component
{
    public partial class PacketListView : UserControl
    {
        public event EventHandler<ListViewItem> OnItemSelectedChanged;
        public event EventHandler<ListViewItem> OnItemDoubleClick;

        private int packetCounter = 0;

        public PacketListView()
        {
            InitializeComponent();

            // TODO: change 'captureLiveView' to something else
            // Using reflection to modify the protected DoubleBuffered property for ListView.
            //  DoubleBuffered is used to prevent flickering
            this.captureListView
                .GetType()
                .GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                .SetValue(captureListView, true, null);
        }

        public int Add(HookedFuncType type, int socketId, byte[] packet)
        {
            if (type == HookedFuncType.SEND || type == HookedFuncType.RECV || type == HookedFuncType.WSASEND || type == HookedFuncType.WSARECV)
            {
                ListViewItem listViewItem = new ListViewItem(packetCounter.ToString());
                listViewItem.Tag = packet;
                listViewItem.SubItems.Add(type.ToString());
                listViewItem.SubItems.Add(packet.Length.ToString());
                listViewItem.SubItems.Add(BitConverter.ToString(packet).Replace("-", " "));
                listViewItem.SubItems.Add(socketId.ToString());
                captureListView.Invoke((MethodInvoker)(() => { captureListView.Items.Add(listViewItem); }));
            }

            return packetCounter++;
        }
        
        public int Add(Packet packet)
        {
            return Add(packet.Type, packet.Socket, packet.Data);
        }

        
        private void captureListView_DoubleClick(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection selectedItems = this.captureListView.SelectedItems;
            if (selectedItems.Count > 0)
            {
                Console.WriteLine("captureListView_DoubleClick");
                OnItemDoubleClick?.Invoke(this, selectedItems[0]);
            }
        }

        private void captureListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
            {
                Console.WriteLine("captureListView_ItemSelectionChanged");
                OnItemSelectedChanged?.Invoke(this, e.Item);
            }
        }
    }
}
