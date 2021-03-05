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
    public partial class PacketEditor : Form
    {
        private byte[] packetData = null;

        public PacketEditor(byte[] vs, bool editible)
        {
            InitializeComponent();

            hexEditor.ForegroundSecondColor = System.Windows.Media.Brushes.Blue;
            hexEditor.StatusBarVisibility = System.Windows.Visibility.Hidden;

            hexEditor.ReadOnlyMode = !editible;
            packetData = vs;
            //hexEditor.Stream = new MemoryStream(new byte[] { 0x10, 0x20, 0x30, 0x40 });
            //hexEditor.HeaderVisibility = Visibility.Hidden;
            //hexEditor.LineInfoVisibility = Visibility.Hidden;
        }

        private void PacketEditor_Load(object sender, EventArgs e)
        {
            hexEditor.Stream = new MemoryStream(packetData);
        }

        private void confirmButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;

        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
