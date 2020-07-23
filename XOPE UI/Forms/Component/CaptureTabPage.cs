using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XOPE_UI.Forms.Component
{
    public partial class CaptureTabPage : UserControl
    {
        PacketEditor packetEditor = null;

        public CaptureTabPage()
        {
            InitializeComponent();
            packetEditor = new PacketEditor();
        }

        private void captureListView_DoubleClick(object sender, EventArgs e)
        {
            packetEditor.ShowDialog();
        }
    }
}
