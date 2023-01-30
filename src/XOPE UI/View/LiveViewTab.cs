﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Markup;
using WpfHexaEditor;
using XOPE_UI.Model;
using XOPE_UI.Presenter;

namespace XOPE_UI.View
{
    public partial class LiveViewTab : UserControl, ILiveViewTab
    {
        ElementHost _elementHost;
        HexEditor _hexEditor;

        LiveViewTabPresenter _presenter;
        MemoryStream _internalStream;

        public Packet BytesInEditor => throw new NotImplementedException();

        public LiveViewTab()
        {
            InitializeComponent();
            InitializeHexEditor();

            _presenter = new LiveViewTabPresenter(this);
        }

        private void InitializeHexEditor()
        {
            _hexEditor = new HexEditor();
            _elementHost = new ElementHost();
            _elementHost.Anchor = ~AnchorStyles.None;
            _elementHost.Location = this.hexEditorPlaceholder.Location;
            _elementHost.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            _elementHost.Name = "elementHost1";
            _elementHost.Size = this.hexEditorPlaceholder.Size;
            _elementHost.TabIndex = 8;
            _elementHost.Text = "elementHost1";
            _elementHost.Child = _hexEditor;
            this.Controls.Remove(hexEditorPlaceholder);
            this.Controls.Add(_elementHost);
        }
        
        public void ClearEditor()
        {
            _hexEditor.DeleteBytesAtPosition(0);
        }

        public void UpdateEditor(byte[] bytes)
        {
            _internalStream = new MemoryStream(bytes.Length);
            _internalStream.Write(bytes, 0, bytes.Length);
        }

        private void forwardButton_Click(object sender, EventArgs e)
        {
            _presenter.ForwardButtonClicked();
        }

        private void dropPacketButton_Click(object sender, EventArgs e)
        {
            _presenter.DropPacketButtonClicked();
        }
    }
}