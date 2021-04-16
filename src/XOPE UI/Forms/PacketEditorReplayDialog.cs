using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private IServer server;

        public PacketEditorReplayDialog(IServer server)
        {
            InitializeComponent();

            this.server = server;

            hexEditor.ForegroundSecondColor = System.Windows.Media.Brushes.Blue;
            hexEditor.StatusBarVisibility = System.Windows.Visibility.Hidden;
            
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
            EventHandler func = (object s, EventArgs _) =>
            {
                if (s != null && s is Timer) ((Timer)s).Dispose(); 

                IMessage message = new InjectSendPacket
                {
                    Data = hexEditor.GetAllBytes(),
                    SocketId = Convert.ToInt32(socketIdTextBox.Value)
                };
                Console.WriteLine("Sending data.. in replayButton_Click");
                server.Send(message);
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
