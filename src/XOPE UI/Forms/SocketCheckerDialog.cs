using Newtonsoft.Json.Linq;
using System;
using System.Windows.Forms;
using XOPE_UI.Spy;
using XOPE_UI.Spy.ServerType;

namespace XOPE_UI.Forms
{
    public partial class SocketCheckerDialog : Form
    {
        IServer server;

        public SocketCheckerDialog(IServer server)
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
                MessageBox.Show(this, $"Response:\n\n{json}");
            };

            server.Send(socketInfo);
        }
    }
}
