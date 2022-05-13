using System;
using System.Windows.Forms;
using XOPE_UI.Model;
using XOPE_UI.Presenter;

namespace XOPE_UI.View
{
    public partial class AutoInjectorDialog : Form, IAutoInjectorDialog
    {
        public AutoInjectorEntry SelectedItem => 
            this.dllListView.SelectedItems.Count > 0 ? this.dllListView.SelectedItems[0].Tag as AutoInjectorEntry : null;

        public bool IsAnyActive
        {
            get
            {
                for (int i = 0; i < this.dllListView.Items.Count; i++)
                    if (this.dllListView.Items[i].Checked)
                        return true;
                return false;
            }
        }

        AutoInjectorDialogPresenter _presenter;

        public AutoInjectorDialog()
        {
            InitializeComponent();
            
            _presenter = new AutoInjectorDialogPresenter(this);
            _presenter.ReloadDllListView();
        }

        string IAutoInjectorDialog.ShowOpenFileDialogForDLL()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "DLL File (*.dll)|*.dll";
                openFileDialog.Title = "Select DLL to auto inject into attached processes...";
                DialogResult dialogResult = openFileDialog.ShowDialog();
                if (dialogResult != DialogResult.OK)
                    return null;

                return openFileDialog.FileName;
            }
        }

        void IAutoInjectorDialog.ClearListView()
        {
            this.dllListView.Items.Clear();
        }

        void IAutoInjectorDialog.AddItemToListView(AutoInjectorEntry entry)
        {
            ListViewItem listViewItem = new ListViewItem();
            listViewItem.SubItems.Add(entry.Name);
            listViewItem.SubItems.Add(entry.FilePath);
            listViewItem.Checked = entry.IsActivated;
            listViewItem.Tag = entry;

            this.dllListView.Items.Add(listViewItem);
        }

        void IAutoInjectorDialog.RemoveItemFromListView(AutoInjectorEntry entry)
        {
            for (int i = 0; i < this.dllListView.Items.Count; i++)
            {
                ListViewItem item = this.dllListView.Items[i];
                if (item.SubItems[2].Text != entry.FilePath)
                    continue;
                item.Remove();
                break;
            }
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            _presenter.AddButtonClicked();
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            _presenter.RemoveButtonClicked();
        }

        private void dllListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.removeButton.Enabled = this.dllListView.SelectedItems.Count > 0;
        }

        private void dllListView_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            _presenter.ToggledDllActive(e.Item.Tag as AutoInjectorEntry, e.Item.Checked);
        }
    }
}
