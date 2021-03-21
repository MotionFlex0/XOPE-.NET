using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XOPE_UI.Forms
{
    public partial class PacketEditorReplayDialog : Form
    {
        public byte[] Data { get; set; } = null;
        public bool Editible { get; set; } = false;
        public int SocketId { get; set; } = 0;

        public PacketEditorReplayDialog()
        {
            InitializeComponent();

            hexEditor.ForegroundSecondColor = System.Windows.Media.Brushes.Blue;
            hexEditor.StatusBarVisibility = System.Windows.Visibility.Hidden;
            
        }

        private void PacketEditorReplayDialog_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                hexEditor.ReadOnlyMode = !Editible;
                if (Data != null)
                {
                    hexEditor.Stream = new MemoryStream(Data);
                    socketIdTextBox.Text = SocketId.ToString(); //Technically not a TextBox but a NumericUpDown instead

                }
                else
                    hexEditor.Stream = new MemoryStream(Encoding.ASCII.GetBytes("___NO DATA___"));
            }
        }
    }
}
