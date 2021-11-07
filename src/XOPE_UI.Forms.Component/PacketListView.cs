﻿using System;
using System.Windows.Forms;
using XOPE_UI.Definitions;

namespace XOPE_UI.Forms.Component
{
    public partial class PacketListView : UserControl
    {
        public event EventHandler<ListViewItem> OnItemSelectedChanged;
        public event EventHandler<ListViewItem> OnItemDoubleClick;

        public int Count { get => captureListView.Items.Count; }

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
                listViewItem.SubItems.Add(BitConverter.ToString(packet, 0, Math.Min(30, packet.Length)).Replace("-", " "));
                listViewItem.SubItems.Add(socketId.ToString());
                captureListView.Invoke(new Action(() => { captureListView.Items.Add(listViewItem); }));
            }

            return packetCounter++;
        }
        
        public int Add(Packet packet)
        {
            return Add(packet.Type, packet.Socket, packet.Data);
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
                OnItemDoubleClick?.Invoke(this, selectedItems[0]);
        }

        private void captureListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
                OnItemSelectedChanged?.Invoke(this, e.Item);
        }
    }
}
