using System;
using System.Windows.Forms;
using XOPE_UI.Util;

namespace XOPE_UI.Forms
{
    public partial class LogDialog : Form
    {
        Logger logger;
        public LogDialog(Logger logger)
        {
            InitializeComponent();

            this.ActiveControl = null;

            this.logger = logger;

            this.logger.OnFlush += (object sender, string value) =>
            {
                if (this.IsHandleCreated && this.Visible)
                {
                    //Fixes deadlocked related to Invoke but not great for efficiency on UI thread
                    logTextbox.BeginInvoke(new Action(() => 
                    {
                        logTextbox.AppendText(value);
                        //logTextbox.SelectionStart = logTextbox.Text.Length;
                        //logTextbox.ScrollToCaret();
                    }));
                }
            };
        }
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        //TODO: Store logs more permenantly, like in a file and load from that instead.
        private void LogDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        private void LogDialog_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                logTextbox.ResetText();
                logTextbox.AppendText(logger.ToString());
            }
        }
    }
}
