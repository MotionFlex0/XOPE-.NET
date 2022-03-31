using System;
using System.ComponentModel;
using XOPE_UI.Model;
using XOPE_UI.Presenter;

namespace XOPE_UI.View
{
    public interface IFilterViewTab
    {
        BindingList<FilterEntry> Filters { get; }
        FilterEntry SelectedItem { get; }

        void AddFilter(FilterEntry entry);
        void RemoveFilter(FilterEntry entry);

        FilterEntry ShowFilterEditorDialog();
        FilterEntry ShowFilterEditorDialog(FilterEntry filterEntry);

        // true = clear filter list
        bool ShowFilterListClearConfirmation();
        
        void ShowFailedToAddFilterMessage(string errorMessage);
    }
}
