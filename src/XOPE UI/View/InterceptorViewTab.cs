using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using WpfHexaEditor;
using XOPE_UI.Model;
using XOPE_UI.Presenter;

namespace XOPE_UI.View
{
    public partial class InterceptorViewTab : UserControl, IInterceptorViewTab
    {
        public event EventHandler WaitingForAction;
        public event EventHandler InterceptorCleared;

        ElementHost _elementHost;
        HexEditor _hexEditor;

        InterceptorViewTabPresenter _presenter;

        MemoryStream _packetStream = null;

        ConcurrentQueue<KeyValuePair<Guid, Packet>> _liveViewQueue;

        public int QueueCount => _liveViewQueue.Count;
        public bool PacketInEditor => _packetStream != null;

        public byte[] BytesInEditor => _packetStream?.ToArray();
        public string JobId 
        {
            get => jobIdLabel.Text; 
            private set => jobIdLabel.Text = value; 
        }

        SpyManager _spyManager;

        public InterceptorViewTab()
        {
            InitializeComponent();
            InitializeHexEditor();

            _presenter = new InterceptorViewTabPresenter(this);
            _liveViewQueue = new ConcurrentQueue<KeyValuePair<Guid, Packet>>();
        }

        private void InitializeHexEditor()
        {
            _hexEditor = new HexEditor();
            _elementHost = new ElementHost();
            _elementHost.Anchor = ~AnchorStyles.None;
            _elementHost.Location = this.hexEditorPlaceholder.Location;
            _elementHost.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            _elementHost.Name = "elementHost1";
            _elementHost.Size = this.hexEditorPlaceholder.Size;
            _elementHost.TabIndex = 8;
            _elementHost.Text = "elementHost1";
            _elementHost.Child = _hexEditor;
            this.Controls.Remove(hexEditorPlaceholder);
            this.Controls.Add(_elementHost);
        }

        public void AttachSpyManager(SpyManager spyManager) =>
            _presenter.SpyManager = _spyManager = spyManager;

        public void AddPacketToLiveViewQueue(Guid jobId, Packet packet)
        {
            if (_packetStream != null)
            {
                _liveViewQueue.Enqueue(new KeyValuePair<Guid, Packet> (jobId, packet));
                return;
            }
            
            _presenter.UpdateEditor(jobId, packet);
        }

        public void ForwardAllPacket()
        {
            _presenter.ForwardAllPackets();
        }

        void IInterceptorViewTab.MoveToNextPacket()
        {
            if (QueueCount < 1)
            {
                if (_packetStream != null)
                    ClearEditor();
                return;
            }

            ClearEditor();
            _liveViewQueue.TryDequeue(out var nextPacket);
            _presenter.UpdateEditor(nextPacket.Key, nextPacket.Value);
        }

        void IInterceptorViewTab.UpdateEditor(Guid jobId, Packet packet)
        {
            this.Invoke(() =>
            {
                _packetStream = new MemoryStream(packet.Data);
                _packetStream.Write(packet.Data, 0, packet.Length);
                _hexEditor.Stream = _packetStream;
                JobId = jobId.ToString();
                packetTypeLabel.Text = packet.Type.ToString();
                if (_spyManager.SpyData.Connections.ContainsKey(packet.Socket))
                {
                    Connection conn = _spyManager.SpyData.Connections[packet.Socket];
                    string sourceIp = conn.SourceAddress != null ? conn.ConvertSourceToString() : "";

                    if (packet.Type == HookedFuncType.SEND || packet.Type == HookedFuncType.WSASEND)
                        ipAddrLabel.Text = $"{sourceIp} ---> {conn.ConvertDestToString()}";
                    else if (packet.Type == HookedFuncType.RECV || packet.Type == HookedFuncType.WSARECV)
                        ipAddrLabel.Text = $"{conn.ConvertDestToString()} ---> {sourceIp}";
                }

                forwardButton.Enabled = true;
                dropPacketButton.Enabled = true;
                WaitingForAction?.Invoke(this, new EventArgs());
            });
        }

        private void ClearEditor()
        {
            this.Invoke(() =>
            {
                _hexEditor.DeleteBytesAtPosition(0);
                _hexEditor.Stream = null;
                _packetStream = null;
                JobId = "~";
                packetTypeLabel.Text = "~";
                ipAddrLabel.Text = "~";
                forwardButton.Enabled = false;
                dropPacketButton.Enabled = false;
                InterceptorCleared?.Invoke(this, new EventArgs());
            });
        }

        private void forwardButton_Click(object sender, EventArgs e)
        {
            _presenter.ForwardButtonClicked();
        }

        private void dropPacketButton_Click(object sender, EventArgs e)
        {
            _presenter.DropPacketButtonClicked();
        }
    }
}
