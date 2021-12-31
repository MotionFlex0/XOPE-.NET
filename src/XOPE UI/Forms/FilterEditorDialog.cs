using System;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using WpfHexaEditor;
using XOPE_UI.Definitions;

namespace XOPE_UI.Forms
{
    public partial class FilterEditorDialog : Form
    {
        public FilterEntry Filter { get; set; }

        static int filterEntryCounter = 0;

        ElementHost elementHost;
        HexEditor beforeHexEditor;
        MemoryStream beforeHexStream;

        ElementHost elementHost2;
        HexEditor afterHexEditor;
        MemoryStream afterHexStream;

        public FilterEditorDialog()
        {
            InitializeComponent();
            InitializeHexEditor();

            nameTextBox.Text = $"Filter {filterEntryCounter++}";

            packetTypeComboBox.DataSource = Enum.GetValues<ReplayableFunction>();

            beforeHexEditor.ForegroundSecondColor = System.Windows.Media.Brushes.Blue;
            beforeHexEditor.StatusBarVisibility = System.Windows.Visibility.Hidden;
            beforeHexEditor.StringByteWidth = 8;
            beforeHexEditor.CanInsertAnywhere = true;
            beforeHexEditor.AllowExtend = true;
            beforeHexEditor.HideByteDeleted = true;
            beforeHexEditor.AppendNeedConfirmation = false;

            afterHexEditor.ForegroundSecondColor = System.Windows.Media.Brushes.Blue;
            afterHexEditor.StatusBarVisibility = System.Windows.Visibility.Hidden;
            afterHexEditor.StringByteWidth = 8;
            afterHexEditor.CanInsertAnywhere = true;
            afterHexEditor.AllowExtend = true;
            afterHexEditor.HideByteDeleted = true;
            afterHexEditor.AppendNeedConfirmation = false;

            beforeHexStream = new MemoryStream(1);
            afterHexStream = new MemoryStream(1);

            beforeHexStream.WriteByte(0x01);
            afterHexStream.WriteByte(0x02);

            beforeHexEditor.Stream = beforeHexStream;
            afterHexEditor.Stream = afterHexStream;
        }

        private void InitializeHexEditor()
        {
            beforeHexEditor = new HexEditor();
            elementHost = new ElementHost();
            elementHost.Anchor = ~AnchorStyles.None;
            elementHost.Location = this.beforeHexEditorPlaceholder.Location;
            elementHost.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            elementHost.Name = "elementHost1";
            elementHost.Size = this.beforeHexEditorPlaceholder.Size;
            elementHost.TabIndex = 8;
            elementHost.Text = "elementHost1";
            elementHost.Child = beforeHexEditor;
            this.beforeGroupBox.Controls.Remove(beforeHexEditorPlaceholder);
            this.beforeGroupBox.Controls.Add(elementHost);

            afterHexEditor = new HexEditor();
            elementHost2 = new ElementHost();
            elementHost2.Anchor = ~AnchorStyles.None;
            elementHost2.Location = this.afterHexEditorPlaceholder.Location;
            elementHost2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            elementHost2.Name = "elementHost2";
            elementHost2.Size = this.afterHexEditorPlaceholder.Size;
            elementHost2.TabIndex = 8;
            elementHost2.Text = "elementHost2";
            elementHost2.Child = afterHexEditor;
            this.afterGroupBox.Controls.Remove(afterHexEditorPlaceholder);
            this.afterGroupBox.Controls.Add(elementHost2);
        }

        private void FilterEditorDialog_Load(object sender, EventArgs e)
        {

        }

        private void acceptButton_Click(object sender, EventArgs e)
        {
            Filter = new FilterEntry
            {
                Name = nameTextBox.Text,
                OldValue = beforeHexEditor.GetAllBytes(true),
                NewValue = afterHexEditor.GetAllBytes(true),
                SocketId = (int)socketIdTextBox.Value,
                PacketType = (ReplayableFunction)packetTypeComboBox.SelectedItem
            };
            DialogResult = DialogResult.OK;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
