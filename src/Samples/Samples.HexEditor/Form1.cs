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

namespace Samples.HexEditor
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
            this.hexEditor.SetBytes(new byte[] { 0x01, 0x02, 0xFF });
        }

 
    }
}
