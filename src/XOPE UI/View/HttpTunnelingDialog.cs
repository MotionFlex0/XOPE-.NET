using System;
using System.Windows.Forms;
using XOPE_UI.Presenter;

namespace XOPE_UI.View
{
    public partial class HttpTunnelingDialog : Form, IHttpTunnelingDialog
    {
        HttpTunnelingDialogPresenter _presenter;

        public string IPAddress 
        { 
            get => this.ipTextBox.Text;
            set => this.ipTextBox.Text = value;
        }
        public string Port 
        { 
            get => this.portTextBox.Text; 
            set => this.portTextBox.Text = value;
        }

        public HttpTunnelingDialog(SpyManager spyManager)
        {
            InitializeComponent();

            _presenter = new HttpTunnelingDialogPresenter(this, spyManager);

            if (spyManager.IsTunneling)
            {
                IPAddress = spyManager.TunnelIp.ToString();
                Port = spyManager.TunnelPort.ToString();
                ShowUiConnectedToProxy();
            }
            else
                ShowUiDisconnectedFromProxy();
        }

        public void ShowErrorMessage(string message)
        {
            MessageBox.Show(this, message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public bool ShowUnstableWarningYesNo(string message)
        {
            return MessageBox.Show(this, message, "Instability warning", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes;
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            _presenter.StartHttpTunneling();
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            _presenter.StopHttpTunneling();
        }

        public void ShowUiConnectedToProxy()
        {
            this.ipTextBox.Enabled = false;
            this.portTextBox.Enabled = false;
            this.startButton.Enabled = false;
            this.stopButton.Enabled = true;
        }

        public void ShowUiDisconnectedFromProxy()
        {
            this.ipTextBox.Enabled = true;
            this.portTextBox.Enabled = true;
            this.startButton.Enabled = true;
            this.stopButton.Enabled = false;
        }
    }
}
