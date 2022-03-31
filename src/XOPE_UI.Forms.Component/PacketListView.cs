using System;
using System.Windows.Forms;
using XOPE_UI.Model;

namespace XOPE_UI.View.Component
{
    //TODO: Use MVP
    public partial class PacketListView : UserControl
    {
        public event EventHandler<ListViewItem> ItemSelectedChanged;
        public event EventHandler<ListViewItem> ItemDoubleClicked;

        public int Count { get => captureListView.Items.Count; }

        int _packetCounter = 0;

        int _minAutoScrollOffset = 20;

        int _maxPacketLength = 30;

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

        public int Add(HookedFuncType type, int socketId, byte[] packet, bool modified = false, bool tunneled = false)
        {
            if (type == HookedFuncType.SEND || 
                type == HookedFuncType.RECV ||
                type == HookedFuncType.WSASEND || 
                type == HookedFuncType.WSARECV)
            {
                string formattedPacket = BitConverter.ToString(packet, 0, Math.Min(_maxPacketLength, packet.Length));
                formattedPacket = formattedPacket.Replace("-", " ");
                formattedPacket += packet.Length > _maxPacketLength ? "..." : "";

                ListViewItem listViewItem = new ListViewItem(_packetCounter.ToString());
                listViewItem.Tag = packet;
                listViewItem.SubItems.Add(type.ToString());
                listViewItem.SubItems.Add(packet.Length.ToString());
                listViewItem.SubItems.Add(formattedPacket);
                listViewItem.SubItems.Add(socketId.ToString());
                if (modified)
                    listViewItem.SubItems.Add("modified");
                else if (tunneled)
                    listViewItem.SubItems.Add("tunneled");

                captureListView.Invoke(new Action(() => 
                { 
                    captureListView.Items.Add(listViewItem);

                    if (captureListView.TopItem.Index >= captureListView.Items.Count - _minAutoScrollOffset)
                        listViewItem.EnsureVisible();
                }));
            }

            return _packetCounter++;
        }
        
        public int Add(Packet packet)
        {
            return Add(packet.Type, packet.Socket, packet.Data, packet.Modified);
        }

        public void Clear()
        {
            captureListView.Items.Clear();
            _packetCounter = 0;
        }
        
        public void ChangeBytesLength(int newLen)
        {
            _maxPacketLength = newLen;
            if (!this.IsHandleCreated)
                return;

            this.Invoke(() =>
            {
                ListView.ListViewItemCollection items = this.captureListView.Items;
                foreach (ListViewItem item in items)
                {
                    byte[] packet = item.Tag as byte[];

                    string formattedPacket = BitConverter.ToString(packet, 0, Math.Min(newLen, packet.Length));
                    formattedPacket = formattedPacket.Replace("-", " ");
                    formattedPacket += packet.Length > newLen ? "..." : "";

                    item.SubItems[3].Text = formattedPacket;
                }
            });
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
                int itemHeight = captureListView.Items[0].Bounds.Height;
                if (itemHeight < 1)
                    return;

                int itemsVisible = (int)Math.Floor((decimal)captureListView.Height / itemHeight - 1);

                _minAutoScrollOffset = itemsVisible + 2;
            }
            else
            {
                // Adds an ListViewItem, to calculate the bounds of each item.
                ListViewItem item = new ListViewItem("DEFAULT_ITEM");
                captureListView.Items.Add(item);
                int itemsVisible = (int)Math.Floor((decimal)captureListView.Height / item.Bounds.Height - 1);
                _minAutoScrollOffset = itemsVisible + 2;
                item.Remove();
            }
        }
    }
}
