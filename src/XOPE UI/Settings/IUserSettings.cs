using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOPE_UI.Model;

namespace XOPE_UI.Settings
{
    public interface IUserSettings
    {
        List<SettingsEntry> SettingsEntries { get; }

        SettingsEntry Get(UserSettingsKey key);
        T GetValue<T>(UserSettingsKey key);
    }
}
