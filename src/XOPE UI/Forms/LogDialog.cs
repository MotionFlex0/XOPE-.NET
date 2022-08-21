using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using XOPE_UI.Util;

namespace XOPE_UI.View
{
    public partial class LogDialog : Form
    {
        static LogDialog instance = null;
        Logger _logger;

        Timer _updateTimer;
        StringBuilder _textStreamtQueued;

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

            _textStreamtQueued = new StringBuilder();

            _logger = logger;
            _logger.TextWritten += Logger_TextWritten; 

            this.logTextBox.Text = this._logger.ToString();
            _updateTimer = new Timer();
            _updateTimer.Interval = 100;
            _updateTimer.Tick += (s, e) =>
            {
                if (_textStreamtQueued.Length < 1)
                    return;

                int lastCharVisible = logTextBox.GetCharIndexFromPosition((Point)logTextBox.Size);
                int bottomMostVisibleLine = logTextBox.GetLineFromCharIndex(lastCharVisible);

                bool shouldScroll = (bottomMostVisibleLine >= logTextBox.Lines.Length - 2);

                logTextBox.AppendText(_textStreamtQueued.ToString());
                if (shouldScroll)
                    logTextBox.ScrollToCaret();

                _textStreamtQueued.Clear();
            };
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);

            _logger.TextWritten -= Logger_TextWritten;
            _updateTimer.Stop();
            instance = null;
        }

        private void Logger_TextWritten(object sender, string value)
        {
            _textStreamtQueued.Append(value);
        }

        private void LogDialog_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                this.logTextBox.Select(logTextBox.Text.Length - 1, 1);
                this.logTextBox.ScrollToCaret();
                _updateTimer.Start();
            }
            else
                _updateTimer.Stop();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
