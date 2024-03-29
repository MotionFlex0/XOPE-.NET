﻿using System;
using System.Threading;
using System.Windows.Forms;

namespace XOPE_UI.View
{
    public partial class ScriptLoadingDialog : Form
    {
        CancellationTokenSource _cancellationTokenSource;
        public ScriptLoadingDialog(CancellationTokenSource cancellationTokenSource)
        {
            InitializeComponent();
            _cancellationTokenSource = cancellationTokenSource;
        }

        // Called when script has finished loading
        public void ScriptLoaded()
        {
            this.DialogResult = DialogResult.OK;
        }

        private void ScriptLoadingDialog_Load(object sender, EventArgs e)
        {
            this.statusTextbox.Text = "Loading script...";
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            _cancellationTokenSource.Cancel();
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
