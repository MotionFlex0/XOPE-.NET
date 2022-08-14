using System;
using System.Windows.Forms;
using XOPE_UI.Model;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace XOPE_UI.View.Component
{
    //TODO: Use MVP
    public partial class PacketListView : UserControl
    {
        public event EventHandler<Packet> ItemSelectedChanged;
        public event EventHandler<Packet> ItemDoubleClicked;

        public int Count { get => captureListView.Items.Count; }

        int _packetCounter = 0;

        int _minAutoScrollOffset = 20;

        int _maxPacketLength = 30;

        Timer _batchListViewUpdateTimer;
        ConcurrentQueue<ListViewItem> _batchListViewItemQueue;
        Object _batchQueueLock = new Object();


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

            _batchListViewItemQueue = new ConcurrentQueue<ListViewItem>();
            _batchListViewUpdateTimer = new Timer();
            _batchListViewUpdateTimer.Interval = 300;
            _batchListViewUpdateTimer.Tick += batchListViewUpdateTimer_Tick;
            _batchListViewUpdateTimer.Start();
        }

        public int Add(Packet packet)
        {
            ListViewItem lvi = CreateListViewItem(packet,
                packet.Type, packet.Socket, packet.Data, packet.Modified, packet.Tunneled);

            if (lvi == null)
                return -1;

            EnqueueListViewItem(lvi);
            return _packetCounter++;
        }

        public void Clear()
        {
            lock(_batchQueueLock) _batchListViewItemQueue.Clear();
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
                    byte[] packet = (item.Tag as Packet).Data;

                    string formattedPacket = BitConverter.ToString(packet, 0, Math.Min(newLen, packet.Length));
                    formattedPacket = formattedPacket.Replace("-", " ");
                    formattedPacket += packet.Length > newLen ? "..." : "";

                    item.SubItems[3].Text = formattedPacket;
                }
            });
        }

        private ListViewItem CreateListViewItem(Packet packet, HookedFuncType type, int socketId, byte[] data, 
            bool modified = false, bool tunneled = false)
        {
            if (type != HookedFuncType.SEND && type != HookedFuncType.RECV && 
                type != HookedFuncType.WSASEND && type != HookedFuncType.WSARECV)
                return null;

            string formattedPacket = BitConverter.ToString(data, 0, Math.Min(_maxPacketLength, data.Length));
            formattedPacket = formattedPacket.Replace("-", " ");
            formattedPacket += data.Length > _maxPacketLength ? "..." : "";

            ListViewItem listViewItem = new ListViewItem(_packetCounter.ToString());
            listViewItem.Tag = packet;
            listViewItem.SubItems.Add(type.ToString());
            listViewItem.SubItems.Add(data.Length.ToString());
            listViewItem.SubItems.Add(formattedPacket);
            listViewItem.SubItems.Add(socketId.ToString());
            if (modified)
                listViewItem.SubItems.Add("modified");
            else if (tunneled)
                listViewItem.SubItems.Add("tunneled");

            return listViewItem;
        }

        private void EnqueueListViewItem(ListViewItem lvi)
        {
            lock (_batchQueueLock) _batchListViewItemQueue.Enqueue(lvi);
        }

        private void batchListViewUpdateTimer_Tick(object sender, EventArgs e)
        {
            if (_batchListViewItemQueue.Count < 1)
                return;

            //Stopwatch sw = Stopwatch.StartNew();

            ListViewItem[] lvis = null;
            lock (_batchQueueLock)
            {
                lvis = _batchListViewItemQueue.ToArray();
                _batchListViewItemQueue.Clear();
            }

            //this.captureListView.BeginUpdate();

            this.captureListView.Items.AddRange(lvis);
            if (captureListView.TopItem.Index >= (captureListView.Items.Count-lvis.Length) - _minAutoScrollOffset)
                lvis[lvis.Length - 1].EnsureVisible();

            //this.captureListView.EndUpdate();
        }

        private void captureListView_DoubleClick(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection selectedItems = this.captureListView.SelectedItems;
            if (selectedItems.Count > 0)
                ItemDoubleClicked?.Invoke(this, selectedItems[0].Tag as Packet);
        }

        private void captureListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
                ItemSelectedChanged?.Invoke(this, e.Item.Tag as Packet);
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

        private void underlyingEventToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.captureListView.SelectedItems.Count < 1)
                return;

            ListViewItem lvi = this.captureListView.SelectedItems[0];

            MessageBox.Show(this.ParentForm, 
                $"Message Event Sent by Spy\n{(lvi.Tag as Packet).UnderlyingEvent.ToString()}",
                "Event message sent by Spy", 
                MessageBoxButtons.OK, MessageBoxIcon.None);
        }

        private void captureListView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (this.captureListView.SelectedItems.Count < 1)
                    return;

                this.packetItemContextMenuStrip.Show(this.captureListView, e.Location);
            }
        }
    }
}
