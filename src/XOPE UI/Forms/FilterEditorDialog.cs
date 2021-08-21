using System;
using System.IO;
using System.Windows.Forms;
using XOPE_UI.Definitions;

namespace XOPE_UI.Forms
{
    public partial class FilterEditorDialog : Form
    {
        public FilterEntry Filter { get; set; }

        public FilterEditorDialog()
        {
            InitializeComponent();

            beforeHexEditor.ForegroundSecondColor = System.Windows.Media.Brushes.Blue;
            beforeHexEditor.StatusBarVisibility = System.Windows.Visibility.Hidden;
            beforeHexEditor.StringByteWidth = 8;
            beforeHexEditor.CanInsertAnywhere = true;

            afterHexEditor.ForegroundSecondColor = System.Windows.Media.Brushes.Blue;
            afterHexEditor.StatusBarVisibility = System.Windows.Visibility.Hidden;
            afterHexEditor.StringByteWidth = 8;
            afterHexEditor.CanInsertAnywhere = true;
        }

        private void FilterEditorDialog_Load(object sender, EventArgs e)
        {
            beforeHexEditor.Stream = new MemoryStream(new byte[] { 0x01 });
            afterHexEditor.Stream = new MemoryStream(new byte[] { 0x02 });

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
            Filter = new FilterEntry
            {
                Name = nameTextBox.Text,
                Before = beforeHexEditor.GetAllBytes(true),
                After = afterHexEditor.GetAllBytes(true),
                SocketId = (int)socketIdTextBox.Value
            };
            DialogResult = DialogResult.OK;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
