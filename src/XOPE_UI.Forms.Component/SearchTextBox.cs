using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using XOPE_UI.Native;

namespace XOPE_UI.View.Component
{
    public partial class SearchTextBox : TextBox
    {
        private const int WM_PASTE = 0x0302;

        private string _placeholderText;
        public override string PlaceholderText
        {
            get => _placeholderText;
            set
            {
                NativeMethods.SendMessage(this.Handle, Win32API.WindowsMessage.EM_SETCUEBANNER, (IntPtr)1, value);
                _placeholderText = value;
            }
        }


        public SearchTextBox()
        {
            InitializeComponent();
        }

        public SearchTextBox(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
