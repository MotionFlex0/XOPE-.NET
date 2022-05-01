using System.Collections.Generic;
using System.IO;
using System.Runtime.Caching;
using XOPE_UI.Model;
using XOPE_UI.View;

namespace XOPE_UI.Presenter
{
    internal class AutoInjectorDialogPresenter
    {
        public static readonly string CACHE_KEY = "AutoInjector_DLL_List";

        private IAutoInjectorDialog _view;
        private Dictionary<string, AutoInjectorEntry> _dllEntries; // <filePath,AutoInjectorEntry>

        public AutoInjectorDialogPresenter(IAutoInjectorDialog view)
        {
            _view = view;

            ObjectCache objectCache = MemoryCache.Default;
            if (!objectCache.Contains(CACHE_KEY))
                objectCache.Add(CACHE_KEY, new Dictionary<string, AutoInjectorEntry>(), ObjectCache.InfiniteAbsoluteExpiration);

            _dllEntries = objectCache.Get(CACHE_KEY) as Dictionary<string, AutoInjectorEntry>;
        }

        public void ReloadDllListView()
        {
            _view.ClearListView();
            foreach (var entry in _dllEntries.Values)
                _view.AddItemToListView(entry);
        }

        public void AddButtonClicked()
        {
            string dllFilePath = _view.ShowOpenFileDialogForDLL();
            if (dllFilePath == null)
                return;

            if (_dllEntries.ContainsKey(dllFilePath))
                return;

            AutoInjectorEntry entry = new AutoInjectorEntry()
            {
                Name = Path.GetFileName(dllFilePath),
                FilePath = dllFilePath,
                IsActivated = true
            };

            _dllEntries.Add(dllFilePath, entry);
            _view.AddItemToListView(entry);
        }

        public void RemoveButtonClicked()
        {
            AutoInjectorEntry entry = _view.SelectedItem;
            if (entry != null)
            {
                _view.RemoveItemFromListView(entry);
                _dllEntries.Remove(entry.FilePath);
            }
        }

        public void ToggledDllActive(AutoInjectorEntry entry, bool toggle)
        {
            entry.IsActivated = toggle;
        }
    }
}
