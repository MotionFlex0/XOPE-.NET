using System;
using System.Windows.Forms;
using XOPE_UI.Model;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace XOPE_UI.View.Component
{
    //TODO: Use MVP
    public partial class PacketListView : UserControl
    {
        public event EventHandler<Packet> ItemSelectedChanged;
        public event EventHandler<Packet> ItemDoubleClicked;

        public int Count { get => _listViewItemStore.Count; }

        int _packetCounter = 0;

        int _minAutoScrollOffset = 20;

        int _maxPacketLength = 30;

        ConcurrentDictionary<int, ListViewItem> _listViewItemStore;
        ListViewItem _defaultListViewItem;

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


            _listViewItemStore = new ConcurrentDictionary<int, ListViewItem>();
            _defaultListViewItem = new ListViewItem();
            _defaultListViewItem.SubItems.AddRange(new string[5]);

            _batchListViewItemQueue = new ConcurrentQueue<ListViewItem>();
            _batchListViewUpdateTimer = new Timer();
            _batchListViewUpdateTimer.Interval = 100;
            _batchListViewUpdateTimer.Tick += batchListViewUpdateTimer_Tick;
            _batchListViewUpdateTimer.Start();
        }

        public int Add(Packet packet)
        {
            ListViewItem lvi = CreateListViewItem(packet,
                packet.Type, packet.Socket, packet.Data, packet.Modified, packet.Tunneled);

            if (lvi == null)
                return -1;

            //EnqueueListViewItem(lvi);
            _listViewItemStore.TryAdd(_packetCounter, lvi);
            
            //if (captureListView.TopItem.Index >= captureListView.Items.Count - _minAutoScrollOffset)
            //    lvi.EnsureVisible();
            return _packetCounter++;
        }

        public void Clear()
        {
            lock(_batchQueueLock) _batchListViewItemQueue.Clear();
            _packetCounter = 0;
            _listViewItemStore.Clear();
            this.captureListView.VirtualListSize = 0;
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

            List<KeyValuePair<string, int>> packetFlags = new List<KeyValuePair<string, int>>();
            if (packet.Intercepted) packetFlags.Add(new("intercepted", 5));
            if (modified) packetFlags.Add(new("modified", 3));
            if(tunneled) packetFlags.Add(new("tunneled", 4));
            if (packet.DropPacket) packetFlags.Add(new("dropped", 4));

            if (packetFlags.Count == 1)
            {
                listViewItem.SubItems.Add(packetFlags[0].Key);
            }
            else if (packetFlags.Count> 1)
            {
                List<string> flagsToJoin = new List<string>();
                foreach (var kv in packetFlags)
                    flagsToJoin.Add(kv.Key.Substring(0, kv.Value));
                listViewItem.SubItems.Add(String.Join("/", flagsToJoin));
            }
            else
                listViewItem.SubItems.Add("");

            return listViewItem;
        }

        private void EnqueueListViewItem(ListViewItem lvi)
        {
            lock (_batchQueueLock) _batchListViewItemQueue.Enqueue(lvi);
        }

        private ListViewItem GetFirstSelectedCaptureItem()
        {
            var selectedIndics = this.captureListView.SelectedIndices;
            if (selectedIndics.Count == 0)
                return null;
            return _listViewItemStore[selectedIndics[0]];
        }

        private void batchListViewUpdateTimer_Tick(object sender, EventArgs e)
        {
            //if (_batchListViewItemQueue.Count < 1)
            //    return;

            //ListViewItem[] lvis = null;
            //lock (_batchQueueLock)
            //{
            //    lvis = _batchListViewItemQueue.ToArray();
            //    _batchListViewItemQueue.Clear();
            //}

            ////this.captureListView.BeginUpdate();

            //this.captureListView.Items.AddRange(lvis);


            if (_listViewItemStore.Count != this.captureListView.VirtualListSize)
            {
                int additionalItemsCount = _listViewItemStore.Count - this.captureListView.VirtualListSize;

                this.captureListView.VirtualListSize = _listViewItemStore.Count;
                if (this.captureListView.TopItem.Index >= (_listViewItemStore.Count - additionalItemsCount) - _minAutoScrollOffset)
                    this.captureListView.EnsureVisible(_listViewItemStore.Count - 1);

            }

            //this.captureListView.EndUpdate();
        }

        private void captureListView_DoubleClick(object sender, EventArgs e)
        {
            ListViewItem selectedItem = GetFirstSelectedCaptureItem();
            if (selectedItem != null)
                ItemDoubleClicked?.Invoke(this, selectedItem.Tag as Packet);
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

            if (_listViewItemStore.Count > 0 && _listViewItemStore[0] != null)
            {
                int itemHeight = _listViewItemStore[0].Bounds.Height;
                if (itemHeight < 1)
                    return;

                int itemsVisible = (int)Math.Floor((decimal)captureListView.Height / itemHeight - 1);

                _minAutoScrollOffset = itemsVisible + 2;
            }
            else
            {
                // Adds an ListViewItem, to calculate the bounds of each item.
                //_listViewItemStore.TryAdd(0, _defaultListViewItem);
                //int itemsVisible = (int)Math.Floor((decimal)captureListView.Height / _defaultListViewItem.Bounds.Height - 1);
                //_minAutoScrollOffset = itemsVisible + 2;
                //_defaultListViewItem.Remove();
            }
        }

        private void underlyingEventToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListViewItem lvi = GetFirstSelectedCaptureItem();
            if (lvi == null)
                return;

            MessageBox.Show(this.ParentForm, 
                $"Message Event Sent by Spy\n{(lvi.Tag as Packet).UnderlyingEvent}",
                "Event message sent by Spy", 
                MessageBoxButtons.OK, MessageBoxIcon.None);
        }

        private void captureListView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (GetFirstSelectedCaptureItem() == null)
                    return;

                this.packetItemContextMenuStrip.Show(this.captureListView, e.Location);
            }
        }

        private void captureListView_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            if (e.ItemIndex < 0 || e.ItemIndex > _listViewItemStore.Count - 1)
            {
                e.Item = _defaultListViewItem;
                return;
            }

            
            e.Item = _listViewItemStore[e.ItemIndex];

        }

        private void replayDoubleClickToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListViewItem selectedItem = GetFirstSelectedCaptureItem();
            if (selectedItem != null)
                ItemDoubleClicked?.Invoke(this, selectedItem.Tag as Packet);
        }
    }
}
