using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace XOPE_UI.Forms
{
    public partial class PacketEditorDialog : Form
    {
        public byte[] Data { get; set; } = null;
        public bool Editible { get; set; } = false;

        public PacketEditorDialog()
        {
            InitializeComponent();

            hexEditor.ForegroundSecondColor = System.Windows.Media.Brushes.Blue;
            hexEditor.StatusBarVisibility = System.Windows.Visibility.Hidden;

            
            //hexEditor.Stream = new MemoryStream(new byte[] { 0x10, 0x20, 0x30, 0x40 });
            //hexEditor.HeaderVisibility = Visibility.Hidden;
            //hexEditor.LineInfoVisibility = Visibility.Hidden;
        }

        private void confirmButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;

        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void PacketEditorDialog_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                if (Data != null)
                    hexEditor.Stream = new MemoryStream(Data);
                else
                    hexEditor.Stream = new MemoryStream(Encoding.ASCII.GetBytes("___NO DATA___"));
            }
        }
    }
}
