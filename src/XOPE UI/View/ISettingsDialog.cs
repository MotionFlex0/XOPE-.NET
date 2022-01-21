using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOPE_UI.Model;

namespace XOPE_UI.View
{
    public interface ISettingsDialog
    {
        public BindingList<SettingsEntry> Settings { get; }
    }
}
