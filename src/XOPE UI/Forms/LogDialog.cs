using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using XOPE_UI.Util;

namespace XOPE_UI.View
{
    public partial class LogDialog : Form
    {
        Logger logger;
        static LogDialog instance = null;

        public static void ShowOrBringToFront(Logger logger)
        {
            if (instance == null && logger != null)
            {
                instance = new LogDialog(logger);
                instance.Show();
            }
            else if (instance != null)
            {
                if (instance.WindowState == FormWindowState.Minimized)
                    instance.WindowState = FormWindowState.Normal;
                instance.BringToFront();
            }
            else
                throw new ArgumentException("logger can only be null if an instance of the LogDialog already exists", "logger");
        }

        private LogDialog(Logger logger)
        {
            InitializeComponent();

            this.ActiveControl = null;

            this.logger = logger;
            this.logger.TextWritten += Logger_TextWritten; 

            this.logTextBox.Text = this.logger.ToString();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);

            this.logger.TextWritten -= Logger_TextWritten;
            instance = null;
        }

        private void Logger_TextWritten(object sender, string value)
        {
            if (this.IsHandleCreated && this.Visible)
            {
                //Fixes deadlocked related to Invoke but not great for efficiency on UI thread
                logTextBox.BeginInvoke(new Action(() =>
                {
                    int lastCharVisible = logTextBox.GetCharIndexFromPosition((Point)logTextBox.Size);
                    int bottomMostVisibleLine = logTextBox.GetLineFromCharIndex(lastCharVisible);

                    bool shouldScroll = (bottomMostVisibleLine >= logTextBox.Lines.Length - 2);

                    logTextBox.AppendText(value);
                    if (shouldScroll)
                        logTextBox.ScrollToCaret();
                }));
            }
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
