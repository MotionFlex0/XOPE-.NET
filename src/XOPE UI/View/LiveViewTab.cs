using System;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using WpfHexaEditor;
using XOPE_UI.Model;
using XOPE_UI.Presenter;

namespace XOPE_UI.View
{
    public partial class LiveViewTab : UserControl, ILiveViewTab
    {
        ElementHost _elementHost;
        HexEditor _hexEditor;

        LiveViewTabPresenter _presenter;
        MemoryStream _internalStream;

        SpyManager _spyManager;

        public byte[] BytesInEditor => _internalStream.ToArray();
        public string JobId 
        {
            get => jobIdLabel.Text; 
            private set => jobIdLabel.Text = value; 
        }

        public LiveViewTab()
        {
            InitializeComponent();
            InitializeHexEditor();

            _presenter = new LiveViewTabPresenter(this);
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

        public void ClearEditor()
        {
            _hexEditor.DeleteBytesAtPosition(0);
            forwardButton.Enabled = false;
            dropPacketButton.Enabled = false;
            jobIdLabel.Text = "~";
        }

        public void UpdateEditor(Guid jobId, byte[] packet)
        {
            JobId = jobId.ToString();
            _internalStream = new MemoryStream(packet);
            _internalStream.Write(packet, 0, packet.Length);
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
