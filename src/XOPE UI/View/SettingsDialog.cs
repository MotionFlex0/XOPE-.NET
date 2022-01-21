using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using XOPE_UI.Model;
using XOPE_UI.Presenter;
using XOPE_UI.Settings;

namespace XOPE_UI.View
{
    public partial class SettingsDialog : Form, ISettingsDialog
    {
        public BindingList<SettingsEntry> Settings { get; } = new();

        SettingsDialogPresenter _presenter;

        public SettingsDialog(IUserSettings settings)
        {
            InitializeComponent();

            _presenter = new SettingsDialogPresenter(this);
            _presenter.LoadSettings(settings);

            this.settingsDataGridView.AutoGenerateColumns = false;
            this.settingsDataGridView.DataSource = Settings;
        }

        private void settingsDataGridView_CurrentCellDirtyStateChanged(object sender, System.EventArgs e)
        {
            this.settingsDataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);

            DataGridViewRow selectedRow = this.settingsDataGridView.SelectedRows[0];
            if ((selectedRow.DataBoundItem as SettingsEntry).ValueType == SettingsType.BOOLEAN)
                this.settingsDataGridView.CurrentCell = selectedRow.Cells["SettingsName"];
        }

        private void settingsDataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            DataGridViewTextBoxEditingControl editingControl = e.Control as DataGridViewTextBoxEditingControl;
            if (editingControl == null)
                return;

            BeginInvoke(() =>
            {
                editingControl.SelectionLength = 0;

                Point mousePosRelToControl = editingControl.PointToClient(MousePosition);
                mousePosRelToControl.X = mousePosRelToControl.X < 0 ? 0 : mousePosRelToControl.X;
                mousePosRelToControl.Y = mousePosRelToControl.Y < 0 ? 0 : mousePosRelToControl.Y;

                int closestCharIndex = editingControl.GetCharIndexFromPosition(mousePosRelToControl);
                Point closestCharPoint = editingControl.GetPositionFromCharIndex(closestCharIndex);

                // Could calculate the length of the last character instead of a magic number
                if (closestCharIndex == editingControl.TextLength-1 && mousePosRelToControl.X > closestCharPoint.X + 7) 
                    closestCharIndex += 1;

                editingControl.SelectionStart = closestCharIndex;
            });
        }

        private void settingsDataGridView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {

        }

        private void settingsDataGridView_SelectionChanged(object sender, System.EventArgs e)
        {
            if (this.settingsDataGridView.SelectedRows.Count < 1)
                return;

            SettingsEntry settingsEntry = this.settingsDataGridView.SelectedRows[0].DataBoundItem as SettingsEntry;
            
            descriptionTextBox.Text = $"{settingsEntry.Description}\r\nDefault value: {settingsEntry.DefaultValue}";
        }

        private void settingsDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //if (e.RowIndex < 0 || e.ColumnIndex < 0)
            //    return;

            ////if (this.settingsDataGridView.Columns[e.ColumnIndex].DataPropertyName != "Value")
            ////    return;

            //DataGridViewRow row = this.settingsDataGridView.Rows[e.RowIndex];
            //SettingsEntry settingsEntry = row.DataBoundItem as SettingsEntry;
            //if (settingsEntry == null)
            //    return;

            //if (settingsEntry.ValueType == SettingsType.BOOLEAN)
            //{
            //    DataGridViewComboBoxCell comboBoxCell = new DataGridViewComboBoxCell() { Value = settingsEntry.Value };
            //    comboBoxCell.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            //    comboBoxCell.FlatStyle = FlatStyle.Popup;

            //    comboBoxCell.DataSource = new bool[] { true, false };
            //    comboBoxCell.ValueType = typeof(bool);

            //    row.Cells["Value"] = comboBoxCell;
            //    Invalidate();
            //}
        }

        private void settingsDataGridView_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            //if (e.RowIndex < 0 || e.ColumnIndex < 0)
            //    return;

            //if (this.settingsDataGridView.Columns[e.ColumnIndex].DataPropertyName != "Value")
            //    return;

            //DataGridViewRow row = this.settingsDataGridView.Rows[e.RowIndex];
            //SettingsEntry settingsEntry = row.DataBoundItem as SettingsEntry;
            //if (settingsEntry == null)
            //    return;

            //if (settingsEntry.ValueType == SettingsType.BOOLEAN)
            //{
            //    DataGridViewTextBoxCell textBoxCell = new DataGridViewTextBoxCell() { Value = settingsEntry.Value };
            //    textBoxCell.ValueType = typeof(bool);
            //    row.Cells["Value"] = textBoxCell;
            //}
        }

        private void settingsDataGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            //DataGridViewRow row = this.settingsDataGridView.Rows[e.RowIndex];
            //SettingsEntry settingsEntry = row.DataBoundItem as SettingsEntry;
            //if (settingsEntry == null)
            //    return;

            //if (settingsEntry.ValueType == SettingsType.BOOLEAN)
            //{
            //    DataGridViewComboBoxCell comboBoxCell = new DataGridViewComboBoxCell() { Value = settingsEntry.Value };
            //    comboBoxCell.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            //    comboBoxCell.FlatStyle = FlatStyle.Popup;

            //    comboBoxCell.DataSource = new bool[] { true, false };
            //    comboBoxCell.ValueType = typeof(bool);

            //    row.Cells["Value"] = comboBoxCell;
            //    //Invalidate();
            //}
        }
    }
}
