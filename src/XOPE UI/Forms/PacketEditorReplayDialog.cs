﻿using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using WpfHexaEditor;
using XOPE_UI.Spy.DispatcherMessageType;

namespace XOPE_UI.Forms
{
    public partial class PacketEditorReplayDialog : Form
    {
        public byte[] Data { get; set; } = null;
        public bool Editible { get; set; } = false;
        public int SocketId { get; set; } = 0;

        private ElementHost elementHost;
        private HexEditor hexEditor;

        private SpyManager spyManager;

        private Timer replayTimer = null;

        public PacketEditorReplayDialog(SpyManager spyManager)
        {
            InitializeComponent();
            InitializeHexEditor();

            this.spyManager = spyManager;
            hexEditor.ForegroundSecondColor = System.Windows.Media.Brushes.Blue;
            hexEditor.StatusBarVisibility = System.Windows.Visibility.Hidden;
            hexEditor.StringByteWidth = 8;
            hexEditor.CanInsertAnywhere = true;
        }

        private void InitializeHexEditor()
        {
            hexEditor = new HexEditor();
            elementHost = new ElementHost();
            //elementHost
            elementHost.Anchor = ~AnchorStyles.None;
            elementHost.Location = this.hexEditorPlaceholder.Location;
            elementHost.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            elementHost.Name = "elementHost1";
            elementHost.Size = this.hexEditorPlaceholder.Size;
            elementHost.TabIndex = 8;
            elementHost.Text = "elementHost1";
            elementHost.Child = hexEditor;
            this.Controls.Remove(hexEditorPlaceholder);
            this.Controls.Add(elementHost);

            hexEditor.CanInsertAnywhere = true;
            hexEditor.KeyDown += (object sender, System.Windows.Input.KeyEventArgs e) =>
            {
                if (e.Key == System.Windows.Input.Key.Insert)
                {
                    //hexEditor.InsertByte(0x00, hexEditor.SelectionStart);
                }
            };
        }

        private void PacketEditorReplayDialog_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                hexEditor.ReadOnlyMode = !Editible;
                if (Data != null)
                {
                    hexEditor.Stream = new MemoryStream(Data);
                    socketIdTextBox.Text = SocketId.ToString(); //Technically not a TextBox but a NumericUpDown instead

                }
                else
                    hexEditor.Stream = new MemoryStream(Encoding.ASCII.GetBytes("___NO_DATA___"));
            }
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void replayButton_Click(object sender, EventArgs e)
        {
            if (spyManager.MessageDispatcher == null)
            {
                MessageBox.Show("Cannot replay packet because the UI is not connected to the Spy.");
                return;
            }

            byte[] data = hexEditor.GetAllBytes(true);
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
                replayTimer = null;

                IMessage message = new InjectSendPacket
                {
                    Data = data,
                    SocketId = socketId
                };

                spyManager.MessageDispatcher.Send(message);
                setUiToReplayState(false);
            };

            if (this.delayTimerTextBox.Value >= 1)
            {
                replayProgressLabel.Text = $"{waitTimer/100:F2}s";
                replayTimer = new Timer();
                replayTimer.Tick += func;
                replayTimer.Interval = 100;
                replayTimer.Start();
                setUiToReplayState(true);
            }
            else
                func(this, null); 
        }

        private void setUiToReplayState(bool isReplaying)
        {
            replayButton.Enabled = !isReplaying;
            stopButton.Enabled = isReplaying;
            replayProgressLabel.Visible = isReplaying;
            socketIdTextBox.Enabled = !isReplaying;
            delayTimerTextBox.Enabled = !isReplaying;
            hexEditor.IsEnabled = !isReplaying;
        }

        private void addToListButton_Click(object sender, EventArgs e)
        {

        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            replayTimer.Stop();
            replayTimer = null;
            setUiToReplayState(false);
        }
    }
}
