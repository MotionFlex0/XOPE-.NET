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
            //this.settingsDataGridView.Sort(this.settingsDataGridView.Columns["SettingsName"], 
                //ListSortDirection.Ascending);

            
        }

        private void settingsDataGridView_CurrentCellDirtyStateChanged(object sender, System.EventArgs e)
        {
            if (this.settingsDataGridView.CurrentCell is DataGridViewComboBoxCell)
            {
                DataGridViewRow selectedRow = this.settingsDataGridView.SelectedRows[0];

                this.settingsDataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);  
                this.settingsDataGridView.CurrentCell = selectedRow.Cells["SettingsName"];
            }
        }

        private void settingsDataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.PreviewKeyDown -= settingsDataGridView_EditingControl_PreviewKeyDown;
            e.Control.PreviewKeyDown += settingsDataGridView_EditingControl_PreviewKeyDown;

            DataGridViewTextBoxEditingControl editingControl = e.Control as DataGridViewTextBoxEditingControl;
            if (editingControl == null)
                return;

            // Clicking on a cell's value results in the caret appearing at that location
            this.BeginInvoke(() =>
            {
                editingControl.SelectionLength = 0;

                Point mousePosRelToControl = editingControl.PointToClient(MousePosition);
                mousePosRelToControl.X = mousePosRelToControl.X < 0 ? 0 : mousePosRelToControl.X;
                mousePosRelToControl.Y = mousePosRelToControl.Y < 0 ? 0 : mousePosRelToControl.Y;

                int closestCharIndex = editingControl.GetCharIndexFromPosition(mousePosRelToControl);
                Point closestCharPoint = editingControl.GetPositionFromCharIndex(closestCharIndex);

                // TODO: Could calculate the pixel length of the last character instead of a magic number
                if (closestCharIndex == editingControl.TextLength-1 && mousePosRelToControl.X > closestCharPoint.X + 7) 
                    closestCharIndex += 1;

                editingControl.SelectionStart = closestCharIndex;
            });
        }

        private void settingsDataGridView_EditingControl_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter &&
                this.settingsDataGridView.IsCurrentCellInEditMode)
            {
                DataGridViewRow selectedRow = this.settingsDataGridView.SelectedRows[0];
                this.settingsDataGridView.CurrentCell = selectedRow.Cells["SettingsName"];
            }
        }

        private void settingsDataGridView_SelectionChanged(object sender, System.EventArgs e)
        {
            if (this.settingsDataGridView.SelectedRows.Count < 1)
                return;

            SettingsEntry settingsEntry = this.settingsDataGridView.SelectedRows[0].DataBoundItem as SettingsEntry;
            
            descriptionTextBox.Text = $"{settingsEntry.Description}\r\nDefault value: {settingsEntry.DefaultValue}";
        }

        private void settingsDataGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            DataGridViewRow row = this.settingsDataGridView.Rows[e.RowIndex];
            if (row.Cells["Value"] is DataGridViewComboBoxCell)
                return;

            SettingsEntry settingsEntry = row.DataBoundItem as SettingsEntry;
            if (settingsEntry == null)
                return;

            if (settingsEntry.ValueType == typeof(bool))
            {
                DataGridViewComboBoxCell comboBoxCell = new DataGridViewComboBoxCell() { Value = settingsEntry.Value };
                comboBoxCell.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
                comboBoxCell.FlatStyle = FlatStyle.Popup;

                comboBoxCell.DataSource = new bool[] { true, false };
                comboBoxCell.ValueType = typeof(bool);

                row.Cells["Value"] = comboBoxCell;
            }
        }
    }
}
