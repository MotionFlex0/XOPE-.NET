using System;
using System.Collections.Generic;
using System.Linq;
using XOPE_UI.Model;

namespace XOPE_UI.Settings
{
    public class UserSettings : IUserSettings
    {
        Dictionary<IUserSettings.Keys, SettingsEntry> _settings = new();
        
        public List<SettingsEntry> SettingsEntries => _settings.Values.ToList();

        public UserSettings()
        {
            //AddSettings(IUserSettings.Keys.FILTER_ENABLED_BY_DEFAULT,
            //    new SettingsEntry("Filter Enabled by default", "Affects whether filter are enabled by default or not.",
            //    SettingsType.BOOLEAN, true));

            AddSettings(IUserSettings.Keys.MAX_BYTES_SHOWN_FOR_FILTER,
                new SettingsEntry("Max bytes shown in Filter list", "The maximum number of bytes shown within the Filter column in the Filter List.",
                SettingsType.NUMBER, 8));

            //AddSettings(IUserSettings.Keys.MAX_BYTES_SHOWN_FOR_PACKET_VIEW,
            //    new SettingsEntry("Max bytes in the packet list", "Empty",
            //    SettingsType.NUMBER, 15));
        }

        public SettingsEntry Get(IUserSettings.Keys key) => _settings[key];

        public T GetValue<T>(IUserSettings.Keys key) => (T)_settings[key].Value;

        private void AddSettings(IUserSettings.Keys key, SettingsEntry settingsEntry)
        {
            if (_settings.ContainsKey(key))
                throw new ArgumentException($"Cannot add 2 settings with the same name {settingsEntry.Name}");

            _settings.Add(key, settingsEntry);
        }


    }
}
