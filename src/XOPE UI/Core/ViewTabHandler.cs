using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;

namespace XOPE_UI.Core
{
    public class ViewTabHandler
    {
        const byte ANIM_ALPHA_CHANGE_BY = 50;
        public Color AlertColour { get; set; } = Color.Orange;

        TabControl _tabControl;
        Dictionary<Guid, ViewTabHandlerItem> _viewTabs = new Dictionary<Guid, ViewTabHandlerItem>();

        public ViewTabHandler(TabControl tabControl) 
        { 
            _tabControl = tabControl;
            _tabControl.SelectedIndexChanged += TabControl_SelectedIndexChanged;
        }

        // NOTE: Uses .Tag for both the Button and TabPage. Do not alert!
        public void AddView(Button btn, TabPage tabPage)
        {
            Guid guid = Guid.NewGuid();

            btn.Click += Btn_Click;
            btn.Tag = guid;
            tabPage.Tag = guid;

            _viewTabs.Add(guid, new ViewTabHandlerItem { Button = btn, TabPage = tabPage });
        }

        public void ShowAlertForViewTab(TabPage tabPage) 
        {
            Guid viewGuid = (Guid)tabPage.Tag;
            ViewTabHandlerItem item = _viewTabs[viewGuid];

            if (item.AlertAnimTimer != null)
                return;

            item.Button.BackColor = AlertColour;
            item.AlertAnimOnFadeIn = false;

            item.AlertAnimTimer = new Timer();
            item.AlertAnimTimer.Tick += (s, ev) =>
            {
                byte newAlpha = item.Button.BackColor.A;
                if (newAlpha == 0)
                {
                    item.AlertAnimOnFadeIn = true;
                    newAlpha += ANIM_ALPHA_CHANGE_BY;
                }
                else if (newAlpha == 255)
                {
                    item.AlertAnimOnFadeIn = false;
                    newAlpha -= ANIM_ALPHA_CHANGE_BY;
                }
                else if (item.AlertAnimOnFadeIn)
                {
                    newAlpha = (byte)Math.Min(newAlpha + ANIM_ALPHA_CHANGE_BY, 255);
                }
                else
                {
                    newAlpha = (byte)Math.Max(newAlpha - ANIM_ALPHA_CHANGE_BY, 0);
                }

                
                item.Button.BackColor = Color.FromArgb(newAlpha, AlertColour);
            };
            item.AlertAnimTimer.Interval = 100;
            item.AlertAnimTimer.Start();
        }

        public void RemoveAlertForViewTab(TabPage tabPage)
        {
            Guid viewGuid = (Guid)tabPage.Tag;
            ViewTabHandlerItem item = _viewTabs[viewGuid];

            if (item.AlertAnimTimer == null)
                return;

            item.AlertAnimTimer.Stop();
            item.AlertAnimTimer.Dispose();

            item.Button.BackColor = SystemColors.Control;

            item.AlertAnimTimer = null;
        }

        private void Btn_Click(object sender, EventArgs e)
        {
            Guid viewGuid = (Guid)(sender as Button).Tag;

            _tabControl.SelectedTab = _viewTabs[viewGuid].TabPage;
        }

        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (KeyValuePair<Guid, ViewTabHandlerItem> entry in _viewTabs)
            {
                entry.Value.Button.BackColor = entry.Value.TabPage == _tabControl.SelectedTab ?
                    entry.Value.Button.BackColor = System.Drawing.SystemColors.MenuHighlight 
                    : System.Drawing.SystemColors.Control;
            }
        }

        class ViewTabHandlerItem
        {
            public Button Button { get; set; }
            public TabPage TabPage { get; set; }
            public Timer AlertAnimTimer { get; set; }
            public bool AlertAnimOnFadeIn { get; set; }
        }
    }
}
