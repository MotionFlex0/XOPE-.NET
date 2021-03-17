using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XOPE_UI.Util;

namespace XOPE_UI.Forms
{
    public partial class LogDialog : Form
    {
        Logger logger;
        string preHandleCreateBuffer;
        public LogDialog(Logger logger)
        {
            InitializeComponent();

            this.ActiveControl = null;

            this.logger = logger;
            preHandleCreateBuffer = "";

            this.logger.OnFlush += (object sender, char c) =>
            {
                if (this.IsHandleCreated)
                {
                    logTextbox.Invoke((MethodInvoker)(() =>
                    {
                        logTextbox.Text += c;
                        logTextbox.ScrollToCaret();
                    }));
                }
                else
                {
                    preHandleCreateBuffer += c;
                }
            };
        }
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            logTextbox.Invoke((MethodInvoker)(() => logTextbox.Text = preHandleCreateBuffer));
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
    }
}
