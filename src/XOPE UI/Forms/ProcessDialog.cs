using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.Caching;
using System.Windows.Forms;
using XOPE_UI.Native;

namespace XOPE_UI.Forms
{
    public partial class ProcessDialog : Form
    {
        public Process SelectedProcess { get; private set; }
        public string SelectedProcessName { get; private set; }

        private SortedDictionary<int, Process> processes;

        private List<ListViewItem> shadowProcessListItems;

        private bool isElevated;

        public ProcessDialog(bool isElevated)
        {
            InitializeComponent();

            this.isElevated = isElevated;

            processes = new SortedDictionary<int, Process>();
            shadowProcessListItems = new List<ListViewItem>();

            processesListView.Columns[0].Width = 400;//processesView.Width;
            is64bitText.Text = Environment.Is64BitProcess.ToString();

            this.processesListView.SmallImageList = getImageListFromCache();

            this.processesListView
            .GetType()
            .GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
            .SetValue(processesListView, true, null);

            this.searchTextBox.PlaceholderText = "Search here or F5 to refresh";
        }

        /// <summary>
        /// Should be called once, at the earlier possible time.
        /// </summary>
        public static void PrecacheResources()
        {
            ObjectCache objectCache = MemoryCache.Default;
            ImageList imageList = getImageListFromCache();

            Process[] ps = Process.GetProcesses();
            foreach (Process p in ps)
            {
                try
                {
                    string processFilePath = NativeMethods.GetFullProcessName(p.Handle, 0);
                    string processName = Path.GetFileName(processFilePath);
                    imageList.Images.Add(processName, Icon.ExtractAssociatedIcon(processFilePath));
                }
                catch (Win32Exception)
                {
                    string cacheKey = $"PROCESS_BLACKLIST_{p.Id}_{p.ProcessName}";
                    objectCache.Add(cacheKey, true, DateTime.Now.AddMinutes(30));
                }
                catch(InvalidOperationException) { }
            }
        }

        //TODO: fix threading issue https://stackoverflow.com/questions/38423472/what-is-the-difference-between-task-run-and-task-factory-startnew
        private void ProcessDialog_Load(object sender, EventArgs e)
        {
            updateProcessListView();
        }

        private void updateProcessListView()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            ObjectCache objectCache = MemoryCache.Default;

            this.confirmButton.Enabled = false;

            processes.Clear();
            shadowProcessListItems.Clear();

            Process[] ps = Process.GetProcesses();
            foreach (Process p in ps)
                processes.Add(p.Id, p);

            this.processesListView.BeginUpdate();
            this.processesListView.Items.Clear();

            foreach (KeyValuePair<int, Process> kv in processes)
            { 
                Process p = kv.Value;
                int pid = p.Id;
                
                string cacheKey = $"PROCESS_BLACKLIST_{pid}_{p.ProcessName}";
                if (objectCache.Contains(cacheKey) || (!isElevated && p.SessionId == 0))
                    continue;
                
                try
                {
                    string arch = NativeMethods.IsWow64Process(p.Handle) ? "x64" : "x32";

                    string processFilePath = NativeMethods.GetFullProcessName(p.Handle, 0);
                    string processName = Path.GetFileName(processFilePath);
                    ListViewItem listViewItem = new ListViewItem($"[{pid}] {processName} ({arch})", processName)
                    {
                        Tag = p.Id
                    };

                    this.Invoke(new Action(() =>
                    {
                        if (!this.processesListView.SmallImageList.Images.ContainsKey(processName))
                            this.processesListView.SmallImageList.Images.Add(processName, Icon.ExtractAssociatedIcon(processFilePath));
                        this.processesListView.Items.Add(listViewItem);
                        shadowProcessListItems.Add(listViewItem);
                    }));
                }
                catch (Win32Exception)
                {
                    objectCache.Add(cacheKey, true, DateTime.Now.AddMinutes(30));
                }
                catch (Exception)
                {

                }
            }
            this.processesListView.EndUpdate();

            UpdateProcessListLabel();

            if (this.searchTextBox.Text != "")
                UpdateListViewWithSearchQuery();

            stopwatch.Stop();

            this.Text = $"Process Selector - Loaded in {stopwatch.Elapsed.Milliseconds}ms";
        }

        private static ImageList getImageListFromCache()
        {
            ObjectCache objectCache = MemoryCache.Default;
            if (objectCache.Contains("PROCESS_SMALL_IMAGE_LIST"))
                return (ImageList)objectCache.Get("PROCESS_SMALL_IMAGE_LIST");
            else
            {
                ImageList imageList = new ImageList
                {
                    ColorDepth = ColorDepth.Depth32Bit
                };
                objectCache.Add("PROCESS_SMALL_IMAGE_LIST", imageList, DateTime.Now.AddMinutes(30));
                return imageList;
            }
        }

        private void UpdateListViewWithSearchQuery()
        {
            string searchQuery = this.searchTextBox.Text;

            this.processesListView.BeginUpdate();
            this.processesListView.Items.Clear();

            foreach (ListViewItem processItem in shadowProcessListItems)
            {
                Process process = processes[(int)processItem.Tag];
                if (process.ProcessName.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) != -1)
                {
                    this.processesListView.Items.Add(processItem);
                }
            }

            this.processesListView.EndUpdate();

            if (searchQuery != "")
            {
                if (this.processesListView.Items.Count > 0)
                    this.searchTextBox.BackColor = Color.LightGreen;
                else
                    this.searchTextBox.BackColor = Color.FromArgb(255, 100, 100);
            }
            else
                this.searchTextBox.BackColor = Color.White;

            UpdateProcessListLabel();
        }

        private void UpdateProcessListLabel()
        {
            int totalProcessCount = shadowProcessListItems.Count;
            int processesListCount = this.processesListView.Items.Count;
            this.processListTotalLabel.Text = $"{processesListCount}/{totalProcessCount}";
        }

        private void processesView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (processesListView.SelectedItems.Count == 0)
                confirmButton.Enabled = false;
            else
            {
                if (!confirmButton.Enabled)
                    confirmButton.Enabled = true;
            }
                
        }

        private void confirmButton_Click(object sender, EventArgs e)
        {
            SelectedProcessName = processesListView.SelectedItems[0].ImageKey;
            SelectedProcess = processes[(int)processesListView.SelectedItems[0].Tag];
            this.DialogResult = DialogResult.OK;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void processesView_DoubleClick(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection selectedItems = this.processesListView.SelectedItems;
            if (selectedItems.Count > 0)
                this.confirmButton.PerformClick();
        }

        private void searchTextBox_TextChanged(object sender, EventArgs e)
        {
            UpdateListViewWithSearchQuery();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F5)
            {
                updateProcessListView();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
         
    }
}
