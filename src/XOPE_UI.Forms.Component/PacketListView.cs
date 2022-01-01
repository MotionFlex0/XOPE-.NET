using System;
using System.Windows.Forms;
using XOPE_UI.Definitions;

namespace XOPE_UI.Forms.Component
{
    public partial class PacketListView : UserControl
    {
        public event EventHandler<ListViewItem> ItemSelectedChanged;
        public event EventHandler<ListViewItem> ItemDoubleClicked;

        public int Count { get => captureListView.Items.Count; }

        int packetCounter = 0;

        int minAutoScrollOffset = 20;

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

        public int Add(HookedFuncType type, int socketId, byte[] packet, bool modified = false)
        {
            const int MAX_PACKET_LENGTH = 30;

            if (type == HookedFuncType.SEND || 
                type == HookedFuncType.RECV ||
                type == HookedFuncType.WSASEND || 
                type == HookedFuncType.WSARECV)
            {
                string formattedPacket = BitConverter.ToString(packet, 0, Math.Min(MAX_PACKET_LENGTH, packet.Length));
                formattedPacket = formattedPacket.Replace("-", " ");
                formattedPacket += packet.Length > MAX_PACKET_LENGTH ? "..." : "";

                ListViewItem listViewItem = new ListViewItem(packetCounter.ToString());
                listViewItem.Tag = packet;
                listViewItem.SubItems.Add(type.ToString());
                listViewItem.SubItems.Add(packet.Length.ToString());
                listViewItem.SubItems.Add(formattedPacket);
                listViewItem.SubItems.Add(socketId.ToString());
                if (modified)
                    listViewItem.SubItems.Add("modified");

                captureListView.Invoke(new Action(() => 
                { 
                    captureListView.Items.Add(listViewItem);

                    if (captureListView.TopItem.Index >= captureListView.Items.Count - minAutoScrollOffset)
                        listViewItem.EnsureVisible();
                }));
            }

            return packetCounter++;
        }
        
        public int Add(Packet packet)
        {
            return Add(packet.Type, packet.Socket, packet.Data, packet.Modified);
        }

        public void Clear()
        {
            captureListView.Items.Clear();
            packetCounter = 0;
        }
        
        private void captureListView_DoubleClick(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection selectedItems = this.captureListView.SelectedItems;
            if (selectedItems.Count > 0)
                ItemDoubleClicked?.Invoke(this, selectedItems[0]);
        }

        private void captureListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
                ItemSelectedChanged?.Invoke(this, e.Item);
        }

        private void captureListView_Resize(object sender, EventArgs e)
        {
            if (!captureListView.Visible)
                return;

            if (captureListView.Items.Count > 0 && captureListView.Items[0] != null)
            {
                int itemsVisible = (int)Math.Floor((decimal)captureListView.Height / captureListView.Items[0].Bounds.Height - 1);

                minAutoScrollOffset = itemsVisible + 2;
            }
            else
            {
                // Adds an ListViewItem, to calculate the bounds of each item.
                ListViewItem item = new ListViewItem("DEFAULT_ITEM");
                captureListView.Items.Add(item);
                int itemsVisible = (int)Math.Floor((decimal)captureListView.Height / item.Bounds.Height - 1);
                minAutoScrollOffset = itemsVisible + 2;
                item.Remove();
            }
        }
    }
}
