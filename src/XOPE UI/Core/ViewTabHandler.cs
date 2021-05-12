using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Text;
using System.Threading.Tasks;

namespace XOPE_UI.Core
{

    public class ViewTabHandler
    {
        class ViewTabHandlerItem
        {
            public Button Button { get; set; }
            public TabPage TabPage { get; set; }
        }

        TabControl tabControl;
        Dictionary<Button, TabPage> viewTabs = new Dictionary<Button, TabPage>();

        public ViewTabHandler(TabControl tabControl) 
        { 
            this.tabControl = tabControl;
            this.tabControl.SelectedIndexChanged += TabControl_SelectedIndexChanged;
        }


        public void AddView(Button btn, TabPage tabPage)
        {
            btn.Click += Btn_Click;

            viewTabs.Add(btn, tabPage );
        }

        private void Btn_Click(object sender, EventArgs e)
        {
            tabControl.SelectedTab = viewTabs[(Button)sender];
        }

        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (KeyValuePair<Button, TabPage> entry in viewTabs)
            {
                entry.Key.BackColor = entry.Value == tabControl.SelectedTab ?
                    entry.Key.BackColor = System.Drawing.SystemColors.MenuHighlight 
                    : System.Drawing.SystemColors.Control;
            }
        }
    }
}
