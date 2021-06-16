using System;
using System.Windows.Forms;
using XOPE_UI.Script;

namespace XOPE_UI.Forms
{
    public partial class ScriptManagerDialog : Form
    {
        private ScriptManager scriptManager;
        private Timer infoRefreshTimer = new Timer();

        public ScriptManagerDialog(ScriptManager scriptManager)
        {
            InitializeComponent();
            this.scriptManager = scriptManager;

            this.scriptInfoListView
                .GetType()
                .GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                .SetValue(scriptInfoListView, true, null);

            // Given Name to each item allowing access via a key
            scriptInfoListView.Items[0].Name = "name";
            scriptInfoListView.Items[1].Name = "status";
            scriptInfoListView.Items[2].Name = "running_time";
            scriptInfoListView.Items[3].Name = "started_at";
            scriptInfoListView.Items[4].Name = "file_location";

            foreach (ListViewItem item in scriptInfoListView.Items)
                item.SubItems.Add(new ListViewItem.ListViewSubItem());

            infoRefreshTimer.Interval = 1000;
            infoRefreshTimer.Tick += (object sender, EventArgs e) =>
            {
                if (scriptInfoListView.Tag == null || !(scriptInfoListView.Tag is Guid))
                    return;

                Guid guid = (Guid)scriptInfoListView.Tag;
                if (guid != null)
                {
                    ScriptData scriptData = scriptManager.GetScript(guid);

                    TimeSpan runningTime = DateTime.Now - scriptData.StartedAt;
                    scriptInfoListView.Items["running_time"].SubItems[1].Text = runningTime.ToString(@"hh\:mm\:ss");
                }
            };
            infoRefreshTimer.Start();
        }

        public void Reload()
        {
            this.runningScriptListView.Items.Clear();
            this.stoppedScriptListView.Items.Clear();
            this.infoRefreshTimer.Tag = null;

            foreach (Guid g in scriptManager.GetGuids())
            {
                ScriptData scriptData = scriptManager.GetScript(g);
                if (scriptData.Status == ScriptStatus.RUNNING)
                {
                    this.runningScriptListView.Items.Add(scriptData.Name).Tag = g;
                }
                else if (scriptData.Status == ScriptStatus.STOPPED)
                {
                    this.stoppedScriptListView.Items.Add(scriptData.Name).Tag = g;
                }
            }
        }

        private void ScriptManagerDialog_Load(object sender, EventArgs e)
        {
            Reload();
        }

        private void scriptListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListView selectedListView = (ListView)sender;

            (selectedListView == runningScriptListView ? stoppedScriptListView : runningScriptListView)
                .SelectedItems
                .Clear();

            //Debug.WriteLine($"selectedListView.SelectedItems[0].Name: {selectedListView.SelectedItems[0].Name}");

            if (selectedListView.SelectedItems.Count < 1)
                return;

            Guid selectedScriptGuid = (Guid)selectedListView.SelectedItems[0].Tag;
            ScriptData scriptData = scriptManager.GetScript(selectedScriptGuid);

            TimeSpan runningTime = DateTime.Now - scriptData.StartedAt;

            scriptInfoListView.Items["name"].SubItems[1].Text = scriptData.Name;
            scriptInfoListView.Items["status"].SubItems[1].Text = scriptData.Status.ToString();
            scriptInfoListView.Items["running_time"].SubItems[1].Text = runningTime.ToString(@"hh\:mm\:ss");
            scriptInfoListView.Items["started_at"].SubItems[1].Text = scriptData.StartedAt.ToString();
            scriptInfoListView.Items["file_location"].SubItems[1].Text = scriptData.FileLocation;

            scriptInfoListView.Tag = selectedScriptGuid;

            //scriptInfoListView.SelectedItems[]
        }


    }
}
