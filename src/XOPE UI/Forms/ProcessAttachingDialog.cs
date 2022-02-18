using System;
using System.Threading;
using System.Windows.Forms;

namespace XOPE_UI.View
{
    public partial class ProcessAttachingDialog : Form
    {
        string _processName;

        public CancellationTokenSource CancellationToken { get; set; } = new();

        public ProcessAttachingDialog(string processName)
        {
            InitializeComponent();
            _processName = processName;
        }

        // Called when script has finished loading
        public void CloseDialog()
        {
            CancellationToken.Cancel();
            this.DialogResult = DialogResult.OK;
        }

        private void ScriptLoadingDialog_Load(object sender, EventArgs e)
        {
            this.statusTextbox.Text = $"Attaching to {_processName}.exe...";
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            CancellationToken.Cancel();
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
