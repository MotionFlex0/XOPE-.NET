using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using XOPE_UI.Spy;
using XOPE_UI.Spy.ServerType;

namespace XOPE_UI.Forms
{
    public partial class PacketEditorReplayDialog : Form
    {
        public byte[] Data { get; set; } = null;
        public bool Editible { get; set; } = false;
        public int SocketId { get; set; } = 0;

        private SpyManager spyManager;

        public PacketEditorReplayDialog(SpyManager spyManager)
        {
            InitializeComponent();

            this.spyManager = spyManager;

            hexEditor.ForegroundSecondColor = System.Windows.Media.Brushes.Blue;
            hexEditor.StatusBarVisibility = System.Windows.Visibility.Hidden;
            hexEditor.StringByteWidth = 8;
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

            EventHandler func = (object s, EventArgs _) =>
            {
                if (s != null && s is Timer) ((Timer)s).Dispose(); 

                IMessage message = new InjectSendPacket
                {
                    Data = hexEditor.GetAllBytes(true),
                    SocketId = Convert.ToInt32(socketIdTextBox.Value)
                };
                //hexEditor.Stream.
                Console.WriteLine("Sending data.. in replayButton_Click");
                spyManager.MessageDispatcher.Send(message);
            };

            if (this.waitTimerTextBox.Value >= 1)
            {
                Timer timer = new Timer();
                timer.Tick += func;
                timer.Interval = Convert.ToInt32(this.waitTimerTextBox.Value);
                timer.Start();
            }
            else
                func(null, null); 
        }
    }
}
