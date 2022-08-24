using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using WpfHexaEditor;
using XOPE_UI.Spy.DispatcherMessageType;

namespace XOPE_UI.View
{
    public partial class PacketEditorReplayDialog : Form
    {
        public byte[] Data { get; set; } = null;
        public bool Editible { get; set; } = false;
        public int SocketId { get; set; } = 0;

        private ElementHost _elementHost;
        private HexEditor _hexEditor;

        private SpyManager _spyManager;

        private Stream _internalStream = null;

        private Timer _replayTimer = null;

        public PacketEditorReplayDialog(SpyManager spyManager)
        {
            InitializeComponent();
            InitializeHexEditor();

            this._spyManager = spyManager;
            _hexEditor.ForegroundSecondColor = System.Windows.Media.Brushes.Blue;
            _hexEditor.StatusBarVisibility = System.Windows.Visibility.Hidden;
            _hexEditor.StringByteWidth = 8;
            _hexEditor.CanInsertAnywhere = true;
            _hexEditor.AllowExtend = true;
            _hexEditor.HideByteDeleted = true;
            _hexEditor.AppendNeedConfirmation = false;

            _hexEditor.KeyDown += (object sender, System.Windows.Input.KeyEventArgs e) =>
            {
                if (e.Key == System.Windows.Input.Key.Insert)
                {

                }
            };

            packetTypeComboBox.SelectedIndex = 0;
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

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);

            if (_replayTimer != null)
            {
                _replayTimer.Stop();
                _replayTimer.Dispose();
            }
        }

        private void PacketEditorReplayDialog_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                _hexEditor.ReadOnlyMode = !Editible;
                if (Data != null)
                {
                    _internalStream = new MemoryStream(Data.Length);
                    _internalStream.Write(Data, 0, Data.Length);
                    _hexEditor.Stream = _internalStream;

                    socketIdTextBox.Text = SocketId.ToString(); //Technically not a TextBox but a NumericUpDown instead
                }
                else
                {
                    _internalStream = new MemoryStream(1);
                    _internalStream.WriteByte(0x00);
                    _hexEditor.Stream = _internalStream;
                }
            }
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void replayButton_Click(object sender, EventArgs e)
        {
            if (_spyManager.MessageDispatcher == null)
            {
                MessageBox.Show("Cannot replay packet because the UI is not connected to the Spy.");
                return;
            }

            byte[] data = _hexEditor.GetAllBytes(true);
            int socketId = Convert.ToInt32(socketIdTextBox.Value);
            double waitTimer = Convert.ToDouble(this.delayTimerTextBox.Value);
            DateTime timeToReplay = DateTime.Now + TimeSpan.FromMilliseconds(waitTimer);

            EventHandler func = (object s, EventArgs _) =>
            {
                TimeSpan timeDifference = timeToReplay - DateTime.Now;
                if (timeDifference.TotalMilliseconds > 0)
                {
                    replayProgressLabel.Text = $"{timeDifference.TotalSeconds:F2}s";
                    return;
                }

                replayProgressLabel.Text = "Sending...";

                if (s != null && s is Timer) ((Timer)s).Dispose();
                _replayTimer = null;

                IMessage message = packetTypeComboBox.SelectedIndex == 0 ?
                    new InjectSendPacket { Data = data, SocketId = socketId } :
                    new InjectRecvPacket { Data = data, SocketId = socketId };

                _spyManager.MessageDispatcher.Send(message);
                preventUiInteraction(false);
            };

            // Forms.Timer is only accurate to ~55ms
            if (this.delayTimerTextBox.Value > 55)
            {
                replayProgressLabel.Text = $"{waitTimer/100:F2}s";
                _replayTimer = new Timer();
                _replayTimer.Tick += func;
                _replayTimer.Interval = 100;
                _replayTimer.Start();
                preventUiInteraction(true);
            }
            else
                func(this, EventArgs.Empty); 
        }

        private void preventUiInteraction(bool isReplaying)
        {
            replayButton.Enabled = !isReplaying;
            stopButton.Enabled = isReplaying;
            replayProgressLabel.Visible = isReplaying;
            socketIdTextBox.Enabled = !isReplaying;
            delayTimerTextBox.Enabled = !isReplaying;
            _hexEditor.IsEnabled = !isReplaying;
            socketSelectorButton.Enabled = !isReplaying;
        }

        private void addToListButton_Click(object sender, EventArgs e)
        {

        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            _replayTimer.Stop();
            _replayTimer = null;
            preventUiInteraction(false);
        }

        private void socketSelectorButton_Click(object sender, EventArgs e)
        {
            using (SocketSelectorDialog socketSelectorDialog = 
                new SocketSelectorDialog(_spyManager))
            {
                DialogResult result = socketSelectorDialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    socketIdTextBox.Value = socketSelectorDialog.SelectedSocketId;
                }
            }
        }
    }
}
