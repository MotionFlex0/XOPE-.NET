using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using WpfHexaEditor;

namespace XOPE_UI.Forms
{
    public partial class PacketEditorDialog : Form
    {
        private ElementHost elementHost;
        private HexEditor hexEditor;

        public byte[] Data { get; set; } = null;
        public bool Editible { get; set; } = false;

        public PacketEditorDialog()
        {
            InitializeComponent();
            InitialiseHexEditor();

            hexEditor.ForegroundSecondColor = System.Windows.Media.Brushes.Blue;
            hexEditor.StatusBarVisibility = System.Windows.Visibility.Hidden;

            
            //hexEditor.Stream = new MemoryStream(new byte[] { 0x10, 0x20, 0x30, 0x40 });
            //hexEditor.HeaderVisibility = Visibility.Hidden;
            //hexEditor.LineInfoVisibility = Visibility.Hidden;
        }

        private void InitialiseHexEditor()
        {
            this.hexEditor = new HexEditor();
            this.elementHost = new ElementHost();

            // elementHost1
            this.elementHost.Anchor = ~AnchorStyles.None;
            this.elementHost.Location = hexEditorPlaceholder.Location;
            this.elementHost.Size = hexEditorPlaceholder.Size;
            this.elementHost.Name = "elementHost1";
            this.elementHost.TabIndex = 4;
            this.elementHost.Text = "elementHost1";
            this.elementHost.Child = this.hexEditor;
            this.Controls.Remove(this.hexEditorPlaceholder);
            this.Controls.Add(this.elementHost);
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
                    hexEditor.Stream = new MemoryStream(Encoding.ASCII.GetBytes("___NO_DATA___"));
            }
        }
    }
}
