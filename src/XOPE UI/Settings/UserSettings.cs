using System;
using System.Collections.Generic;
using System.Linq;
using XOPE_UI.Model;

namespace XOPE_UI.Settings
{
    // TODO: Potential move to using ApplicationSettingsBase instead
    public class UserSettings : IUserSettings
    {
        readonly Dictionary<UserSettingsKey, SettingsEntry> _settings = new();
        
        public List<SettingsEntry> SettingsEntries => _settings.Values.ToList();

        public UserSettings()
        {
            AddSettings(UserSettingsKey.FILTER_ENABLED_BY_DEFAULT,
                new SettingsEntry("Filter Enabled by default", 
                "Affects whether filter are enabled by default or not.",
                true));

            AddSettings(UserSettingsKey.MAX_BYTES_SHOWN_FOR_FILTER,
                new SettingsEntry("Max bytes shown in Filter list", 
                "The maximum number of bytes shown within the Filter column in the Filter List.",
                8, 1));

            AddSettings(UserSettingsKey.MAX_BYTES_SHOWN_FOR_PACKET_VIEW,
                new SettingsEntry("Max bytes in the packet list", 
                "The maximum number of bytes shown within the Data column in the Capture List.",
                30, 1));
        }

        public SettingsEntry Get(UserSettingsKey key) => _settings[key];

        public T GetValue<T>(UserSettingsKey key) => (T)_settings[key].Value;

        private void AddSettings(UserSettingsKey key, SettingsEntry settingsEntry)
        {
            if (_settings.ContainsKey(key))
                throw new ArgumentException($"Cannot add 2 settings with the same name {key}");

            _settings.Add(key, settingsEntry);
        }
    }
}
