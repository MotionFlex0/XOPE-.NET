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
        public PacketEditor()
        {
            InitializeComponent();

            hexEditor.ForegroundSecondColor = System.Windows.Media.Brushes.Blue;
            hexEditor.StatusBarVisibility = System.Windows.Visibility.Hidden;
            hexEditor.Stream = new MemoryStream(new byte[] { 0x10, 0x20, 0x30, 0x40 });
            //hexEditor.HeaderVisibility = Visibility.Hidden;
            //hexEditor.LineInfoVisibility = Visibility.Hidden;
        }
    }
}
