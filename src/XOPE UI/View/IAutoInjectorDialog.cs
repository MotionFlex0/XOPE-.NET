using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOPE_UI.Model;

namespace XOPE_UI.View
{
    internal interface IAutoInjectorDialog
    {
        AutoInjectorEntry SelectedItem { get; }

        string ShowOpenFileDialogForDLL();
        void ClearListView();
        void AddItemToListView(AutoInjectorEntry entry);
        void RemoveItemFromListView(AutoInjectorEntry entry);
    }
}
