using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using XOPE_UI.Model;
using XOPE_UI.Presenter;
using XOPE_UI.Settings;

namespace XOPE_UI.View
{
    public partial class FilterViewTab : UserControl, IFilterViewTab
    {
        public FilterViewTabPresenter Presenter { get; }

        public BindingList<FilterEntry> Filters { get; } = new BindingList<FilterEntry>();
        public FilterEntry SelectedItem => 
            filterDataGridView.SelectedRows?[0]?.DataBoundItem as FilterEntry;

        SpyManager _spyManager;
        IUserSettings _settings;

        public FilterViewTab()
        {
            InitializeComponent();
            Presenter = new FilterViewTabPresenter(this);

            this.filterDataGridView.AutoGenerateColumns = false;

            this.filterDataGridView.DataSource = Filters;
            Filters.ListChanged += Filters_ListChanged;
        }

        public void AttachSpyManager(SpyManager spyManager) => 
            Presenter.SpyManager = _spyManager = spyManager;

        public void AttachSettings(IUserSettings settings)
        {
            Presenter.Settings = settings;
            _settings = settings;
            _settings.Get(UserSettingsKey.MAX_BYTES_SHOWN_FOR_FILTER).PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(SettingsEntry.Value))
                {
                    DataGridViewRowCollection rows = this.filterDataGridView.Rows;
                    foreach (DataGridViewRow row in rows)
                        UpdateRowFilterColumnText(row.Index);
                }
            };
        }

        void IFilterViewTab.AddFilter(FilterEntry entry)
        {
            if (this.InvokeRequired)
                this.Invoke(() => Filters.Add(entry));
            else
                Filters.Add(entry);
        }

        void IFilterViewTab.RemoveFilter(FilterEntry entry)
        {
            if (this.InvokeRequired)
                this.Invoke(() => Filters.Remove(entry));
            else
                Filters.Remove(entry);
        }

        void IFilterViewTab.ShowFailedToAddFilterMessage(string errorMessage)
        {
            MessageBox.Show($"Failed to add filter.\nMessage: " +
                        $"{errorMessage}");
        }

        FilterEntry IFilterViewTab.ShowFilterEditorDialog()
        {
            return (this as IFilterViewTab).ShowFilterEditorDialog(null);
        }

        FilterEntry IFilterViewTab.ShowFilterEditorDialog(FilterEntry filter)
        {
            using (FilterEditorDialog dialog = new FilterEditorDialog(_spyManager))
            {
                if (filter != null)
                    dialog.SetFilterEntry(filter);
                DialogResult dialogResult = dialog.ShowDialog();

                if (dialogResult != DialogResult.OK)
                    return null;
                
                return dialog.Filter;
            }
        }
        
        bool IFilterViewTab.ShowFilterListClearConfirmation()
        {
            return MessageBox.Show(this, "You're about to load filters from a file.\n" +
                "This will erase your current filters.\n" +
                "Are you sure you want to continue?", "Clearing list",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes;
        }

        private void UpdateRowFilterColumnText(int rowIndex)
        {
            DataGridViewRow row = this.filterDataGridView.Rows[rowIndex];
            if (row == null)
                return;

            FilterEntry filter = row.DataBoundItem as FilterEntry;
            if (filter == null)
                return;
            
            if (filter.DropPacket)
            {
                row.Cells["Filter"].Value = $"N/A - This packet will be blocked.";
                return;
            }

            int maxBytesShown = _settings.GetValue<int>(UserSettingsKey.MAX_BYTES_SHOWN_FOR_FILTER);

            string beforeStr = BitConverter.ToString(filter.OldValue, 0).Replace("-", " ");
            string beforeFormatted = beforeStr.Substring(0, Math.Min((maxBytesShown * 3) - 1, beforeStr.Length));
            beforeFormatted += beforeStr.Length > (maxBytesShown * 3) - 1 ? "..." : "";

            string afterStr = BitConverter.ToString(filter.NewValue, 0).Replace("-", " ");
            string afterFormatted = afterStr.Substring(0, Math.Min((maxBytesShown * 3) - 1, afterStr.Length));
            afterFormatted += afterStr.Length > (maxBytesShown * 3) - 1 ? "..." : "";

            row.Cells["Filter"].Value = $"{beforeFormatted} --> {afterFormatted}";
        }

        private void addFilterButton_Click(object sender, EventArgs e)
        {
            Presenter.AddFilterButtonClicked();
        }

        private void deleteFilterButton_Click(object sender, EventArgs e)
        {
            Presenter.DeleteFilterButtonClicked(SelectedItem);
        }

        private void filterDataGridView_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.RowIndex < 0)
                return;

            var checkbox = this.filterDataGridView.Rows[e.RowIndex].Cells["Activated"] as DataGridViewCheckBoxCell;

            if (e.Button == MouseButtons.Left && !checkbox.ContentBounds.Contains(e.Location))
            {
                Presenter.ShowFilterEditor(SelectedItem);
                this.UpdateRowFilterColumnText(e.RowIndex);
            }
        }

        private void filterDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (this.filterDataGridView.Columns[e.ColumnIndex].Name == "Activated")
            {
                var filter = this.filterDataGridView.Rows[e.RowIndex].DataBoundItem as FilterEntry;
                Presenter.ToggleFilterActivated(filter, filter.Activated);
            }
        }

        private void filterDataGridView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            UpdateRowFilterColumnText(e.RowIndex);
        }

        private void filterDataGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            int currentCellColumnIndex = this.filterDataGridView.CurrentCell.ColumnIndex;
            string currentCellHeaderName = this.filterDataGridView.Columns[currentCellColumnIndex].Name;
            if (currentCellHeaderName == "Activated" && this.filterDataGridView.IsCurrentCellDirty)
                this.filterDataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
                    
        }

        private void Filters_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType != ListChangedType.ItemChanged)
                return;

            if (e.PropertyDescriptor.Name == nameof(FilterEntry.OldValue) || 
                e.PropertyDescriptor.Name == nameof(FilterEntry.NewValue))
            {
                UpdateRowFilterColumnText(e.NewIndex);
            }
        }

        private void filterDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            this.deleteFilterButton.Enabled = filterDataGridView.SelectedRows.Count > 0;
        }

    }
}
