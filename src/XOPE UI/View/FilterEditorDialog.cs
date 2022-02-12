using System;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using WpfHexaEditor;
using XOPE_UI.Model;
using XOPE_UI.Presenter;

namespace XOPE_UI.View
{
    public partial class FilterEditorDialog : Form, IFilterEditorDialog
    {
        FilterEditorDialogPresenter _presenter;
        
        static int _filterEntryCounter = 0;

        ElementHost _elementHost;
        HexEditor _beforeHexEditor;
        MemoryStream _beforeHexStream;

        ElementHost _elementHost2;
        HexEditor _afterHexEditor;
        MemoryStream _afterHexStream;

        int _oldSocketId = 0;

        SpyManager _spyManager;
        FilterEntry _filter;

        #region Inheritied Props
        public string FilterName
        {
            get => nameTextBox.Text;
            set => nameTextBox.Text = value;
        }

        public byte[] OldValue
        {
            get => _beforeHexEditor.GetAllBytes(true);
            set
            {
                _beforeHexStream.SetLength(0);
                _beforeHexStream.Write(value);
            }
        }

        public byte[] NewValue
        {
            get => _afterHexEditor.GetAllBytes(true);
            set
            {
                _afterHexStream.SetLength(0);
                _afterHexStream.Write(value);
            }
        }

        public int SocketId
        {
            get => (int)socketIdTextBox.Value;
            set => socketIdTextBox.Value = value;
        }

        public ReplayableFunction PacketType
        {
            get => (ReplayableFunction)packetTypeComboBox.SelectedItem;
            set => packetTypeComboBox.SelectedItem = value;
        }

        public bool ShouldRecursiveReplace
        {
            get => recursiveReplaceCheckBox.Checked;
            set => recursiveReplaceCheckBox.Checked = value;
        }

        public bool AllSockets
        {
            get => allSocketsCheckBox.Checked;
            set => allSocketsCheckBox.Checked = value;
        }

        /// <summary>
        /// Discard this value if DialogResult is not equal to DialogResult.OK
        /// </summary>
        public FilterEntry Filter
        {
            get => _filter;
            set => _filter = value;
        }
        #endregion

        public FilterEditorDialog(SpyManager spyManager)
        {
            InitializeComponent();
            InitializeHexEditor();

            _spyManager = spyManager;

            _presenter = new FilterEditorDialogPresenter(this);

            nameTextBox.Text = $"Filter {_filterEntryCounter++}";

            packetTypeComboBox.DataSource = Enum.GetValues<ReplayableFunction>();

            foreach (HexEditor hexEditor in
                new HexEditor[] { _beforeHexEditor, _afterHexEditor })
            {
                hexEditor.ForegroundSecondColor = System.Windows.Media.Brushes.Blue;
                hexEditor.StatusBarVisibility = System.Windows.Visibility.Hidden;
                hexEditor.StringByteWidth = 8;
                hexEditor.CanInsertAnywhere = true;
                hexEditor.AllowExtend = true;
                hexEditor.HideByteDeleted = true;
                hexEditor.AppendNeedConfirmation = false;
            }

            _beforeHexStream = new MemoryStream(1);
            _afterHexStream = new MemoryStream(1);

            _beforeHexStream.WriteByte(0x01);
            _afterHexStream.WriteByte(0x01);

            _beforeHexEditor.Stream = _beforeHexStream;
            _afterHexEditor.Stream = _afterHexStream;

        }

        private void InitializeHexEditor()
        {
            _beforeHexEditor = new HexEditor();
            _elementHost = new ElementHost();
            _elementHost.Anchor = ~AnchorStyles.None;
            _elementHost.Location = this.beforeHexEditorPlaceholder.Location;
            _elementHost.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            _elementHost.Name = "elementHost1";
            _elementHost.Size = this.beforeHexEditorPlaceholder.Size;
            _elementHost.TabIndex = 8;
            _elementHost.Text = "elementHost1";
            _elementHost.Child = _beforeHexEditor;
            this.beforeGroupBox.Controls.Remove(beforeHexEditorPlaceholder);
            this.beforeGroupBox.Controls.Add(_elementHost);

            _afterHexEditor = new HexEditor();
            _elementHost2 = new ElementHost();
            _elementHost2.Anchor = ~AnchorStyles.None;
            _elementHost2.Location = this.afterHexEditorPlaceholder.Location;
            _elementHost2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            _elementHost2.Name = "elementHost2";
            _elementHost2.Size = this.afterHexEditorPlaceholder.Size;
            _elementHost2.TabIndex = 8;
            _elementHost2.Text = "elementHost2";
            _elementHost2.Child = _afterHexEditor;
            this.afterGroupBox.Controls.Remove(afterHexEditorPlaceholder);
            this.afterGroupBox.Controls.Add(_elementHost2);
        }

        public void SetFilterEntry(FilterEntry filter)
        {
            Filter = filter;
            _presenter.SetFilter(filter);
        }

        private void acceptButton_Click(object sender, EventArgs e)
        {
            _presenter.SaveFilter();
            this.DialogResult = DialogResult.OK;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void allSocketsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (allSocketsCheckBox.Checked)
            {
                _oldSocketId = (int)socketIdTextBox.Value;
                SocketId = -1;
                socketIdTextBox.Enabled = false;
                socketSelectorButton.Enabled = false;
            }    
            else
            {
                SocketId = _oldSocketId;
                socketIdTextBox.Enabled = true;
                socketSelectorButton.Enabled = true;
            }
        }

        private void socketSelectorButton_Click(object sender, EventArgs e)
        {
            using (SocketSelectorDialog socketSelectorDialog =
                new SocketSelectorDialog(_spyManager))
            {
                DialogResult result = socketSelectorDialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    socketIdTextBox.Value = socketSelectorDialog.SelectedSocketId;
                }
            }
        }
    }
}
