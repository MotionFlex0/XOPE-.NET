using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XOPE_UI.Forms.Component
{
    public partial class SearchTextBox : TextBox
    {
        private const int WM_PASTE = 0x0302;

        public SearchTextBox()
        {
            InitializeComponent();
        }

        public SearchTextBox(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_PASTE:
                    this.Text = "";
                    break;
            }
            base.WndProc(ref m);
        }
    }
}
