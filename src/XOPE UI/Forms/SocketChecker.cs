using Newtonsoft.Json.Linq;
using System;
using System.Windows.Forms;
using XOPE_UI.Spy;
using XOPE_UI.Spy.ServerType;

namespace XOPE_UI.Forms
{
    public partial class SocketChecker : Form
    {
        IServer server;

        public SocketChecker(IServer server)
        {
            InitializeComponent();
            this.server = server;
        }

        private void socketCheckBtn_Click(object sender, EventArgs e)
        {
            SocketInfo socketInfo = new SocketInfo
            {
                SocketId = Convert.ToInt32(this.socketIdTextBox.Value)
            };

            socketInfo.OnResponse += (object s, JObject json) =>
            {
                Console.WriteLine($"Socket Check: {json.ToString()}");
            };

            server.Send(socketInfo);
        }
    }
}
