using XOPE_UI.Settings;
using XOPE_UI.View;

namespace XOPE_UI.Presenter
{
    public class SettingsDialogPresenter
    {
        ISettingsDialog _view;

        public SettingsDialogPresenter(ISettingsDialog view)
        {
            _view = view;
        }

        public void LoadSettings(IUserSettings settings)
        {
            settings.SettingsEntries.ForEach(s => _view.Settings.Add(s));
        }
    }
}
