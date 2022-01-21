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

        SettingsEntry Get(Keys key);
        T GetValue<T>(Keys key);


        enum Keys
        {
            FILTER_ENABLED_BY_DEFAULT,
            MAX_BYTES_SHOWN_FOR_PACKET_VIEW,
            MAX_BYTES_SHOWN_FOR_FILTER
        }
    }
}
