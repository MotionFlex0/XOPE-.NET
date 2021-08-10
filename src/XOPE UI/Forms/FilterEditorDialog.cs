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
using XOPE_UI.Definitions;

namespace XOPE_UI.Forms
{
    public partial class FilterEditorDialog : Form
    {
        //public bool EditMode { get; set; } = false;
        //public FilterEntry Filter { get; set; }

        public FilterEditorDialog()
        {
            InitializeComponent();

            //beforeHexEditor.ForegroundSecondColor = System.Windows.Media.Brushes.Blue;
            //beforeHexEditor.StatusBarVisibility = System.Windows.Visibility.Hidden;
            //beforeHexEditor.StringByteWidth = 8;


            //afterHexEditor.ForegroundSecondColor = System.Windows.Media.Brushes.Blue;
            //afterHexEditor.StatusBarVisibility = System.Windows.Visibility.Hidden;
            //afterHexEditor.StringByteWidth = 8;
        }

        private void FilterEditorDialog_Load(object sender, EventArgs e)
        {
            beforeHexEditor.Stream = new MemoryStream(new byte[] { 0x11, 0x22 });
            hexEditor1.Stream = new MemoryStream(new byte[] { 0x55, 0x22 });

            //if (Filter == null)
            //    Filter = new FilterEntry();
            //else
            //{
            //    //beforeHexEditor.SetBy(Filter.Before);
            //    //afterHexEditor.SetBytes(Filter.After);
            //}

        }

        private void acceptButton_Click(object sender, EventArgs e)
        {
            //Filter = new FilterEntry
            //{
            //    Name = "test",
            //    Before = beforeHexEditor.GetAllBytes(true),
            //    After = afterHexEditor.GetAllBytes(true),
            //    SocketId = 0
            //};
            //DialogResult = DialogResult.OK;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
