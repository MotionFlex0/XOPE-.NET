using Newtonsoft.Json.Linq;
using System;
using System.Windows.Forms;
using XOPE_UI.Spy;
using XOPE_UI.Spy.DispatcherMessageType;
using XOPE_UI.Spy.Type;

namespace XOPE_UI.Forms
{
    public partial class SocketCheckerDialog : Form
    {
        IMessageDispatcher messageDispatcher;

        public SocketCheckerDialog(IMessageDispatcher messageDispatcher)
        {
            InitializeComponent();
            this.messageDispatcher = messageDispatcher;
        }

        private void socketCheckBtn_Click(object sender, EventArgs e)
        {
            if (!messageDispatcher.IsConnected)
            {
                MessageBox.Show("Cannot check socket because the Spy is no longer available.", "Spy not available");
                return;
            }

            SocketInfo socketInfo = new SocketInfo
            {
                SocketId = Convert.ToInt32(this.socketIdTextBox.Value)
            };

            socketInfo.OnResponse += (object s, IncomingMessage response) =>
            {
                MessageBox.Show(this, $"Response:\n\n{response.Json}");
            };

            messageDispatcher.Send(socketInfo);
        }
    }
}
