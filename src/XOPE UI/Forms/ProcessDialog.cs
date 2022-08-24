using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Management;
using System.Runtime.Caching;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using XOPE_UI.Native;
using XOPE_UI.View.Component;

namespace XOPE_UI.View
{
    public partial class ProcessDialog : Form
    {
        public Process SelectedProcess { get; private set; }
        public string SelectedProcessName { get; private set; }

        private SearchTextBox _searchTextBox;

        private SortedDictionary<int, Process> _processes;

        // Use this if you need to go through the process list, instead of "processes"
        //   as "processes" contains elevated processes
        // <processId, listViewItem>
        private Dictionary<int, ListViewItem> _shadowProcessListItems;

        private bool _isElevated;

        private Guid _currentListId;

        public ProcessDialog(bool isElevated)
        {
            InitializeComponent();

            _searchTextBox = new SearchTextBox()
            {
                Anchor = (AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom),
                Location = searchTextBoxPlaceholder.Location,
                Margin = new Padding(4, 3, 4, 3),
                Name = "searchTextBox",
                PlaceholderText = "Search here or F5 to refresh | Args: /c",
                Size = searchTextBoxPlaceholder.Size,
                TabIndex = 0,
                Text = ""
            };
            _searchTextBox.TextChanged += new EventHandler(this.searchTextBox_TextChanged);
            this.Controls.Add(_searchTextBox);
            this.Controls.Remove(searchTextBoxPlaceholder);

            this._isElevated = isElevated;

            _processes = new SortedDictionary<int, Process>();
            _shadowProcessListItems = new Dictionary<int, ListViewItem>();

            this.processesListView.Columns[0].Width = 400;//processesView.Width;
            is64bitText.Text = Environment.Is64BitProcess.ToString();

            this.processesListView.SmallImageList = GetImageListFromCache();

            this.processesListView
                .GetType()
                .GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                .SetValue(this.processesListView, true, null);

            _searchTextBox.AddQueryOperator("/c", "Search commandline arguments");
        }

        /// <summary>
        /// Should be called once, at the earlier possible time.
        /// </summary>
        public static void PrecacheResources()
        {
            ObjectCache objectCache = MemoryCache.Default;
            ImageList imageList = GetImageListFromCache();

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
            UpdateProcessListView();
        }

        private void UpdateProcessListView()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            ObjectCache objectCache = MemoryCache.Default;

            this.confirmButton.Enabled = false;

            _currentListId = Guid.NewGuid();
            _processes.Clear();
            _shadowProcessListItems.Clear();

            Process[] ps = Process.GetProcesses();
            foreach (Process p in ps)
                _processes.Add(p.Id, p);

            this.processesListView.BeginUpdate();
            this.processesListView.Items.Clear();

            // Get command-line arguments on sepearate thread due to delay of >100ms
            Task.Factory.StartNew(() =>
            {
                Guid listToUpdate = _currentListId;
                using (ManagementObjectSearcher mos =
                    new($"SELECT CommandLine, ProcessId FROM Win32_Process"))
                {
                    if (this.IsDisposed || listToUpdate != _currentListId)
                        return;


                    foreach (ManagementObject mo in mos.Get())
                    {
                        if (listToUpdate != _currentListId)
                            break;

                        int pid = Convert.ToInt32(mo["ProcessId"]);
                        if (!_shadowProcessListItems.ContainsKey(pid))
                            continue;

                        this.Invoke(() => _shadowProcessListItems[pid].ToolTipText = mo["CommandLine"] as string);
                    }
                }
            });

            foreach (KeyValuePair<int, Process> kv in _processes)
            { 
                Process p = kv.Value;
                int pid = p.Id;
                
                string cacheKey = $"PROCESS_BLACKLIST_{pid}_{p.ProcessName}";
                if (objectCache.Contains(cacheKey) || (!_isElevated && p.SessionId == 0))
                    continue;
                
                try
                {
                    string arch = NativeMethods.IsWow64Process(p.Handle) ? "x32" : "x64";

                    string processFilePath = NativeMethods.GetFullProcessName(p.Handle, 0);
                    string processName = Path.GetFileName(processFilePath);

                    ListViewItem listViewItem = new ListViewItem($"[{pid}] {processName} ({arch})", processName)
                    {
                        Tag = p.Id,
                    };



                    this.Invoke(new Action(() =>
                    {
                        if (!this.processesListView.SmallImageList.Images.ContainsKey(processName))
                            this.processesListView.SmallImageList.Images.Add(processName, Icon.ExtractAssociatedIcon(processFilePath));
                        this.processesListView.Items.Add(listViewItem);
                        _shadowProcessListItems.Add(p.Id, listViewItem);
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

            //Add a * if process was the first to start with that name
            HashSet<string> processNamesChecked = new HashSet<string>();
            for (int i = 0; i < this.processesListView.Items.Count; i++)
            {
                ListViewItem listViewItem = this.processesListView.Items[i];
                Process process = _processes[(int)listViewItem.Tag];

                if (processNamesChecked.Contains(process.ProcessName))
                    continue;

                bool wasEarliestStartedProcess = true;
                bool singleInstance = true;
                foreach (ListViewItem processItem in _shadowProcessListItems.Values)
                {
                    Process p = _processes[(int)processItem.Tag];
                    if (p.Id == process.Id || p.ProcessName != process.ProcessName)
                        continue;

                    if (singleInstance)
                        singleInstance = false;

                    // process started after another p with the same name
                    if (process.StartTime >= p.StartTime)
                    {
                        wasEarliestStartedProcess = false;
                        break;
                    }
                }

                if (wasEarliestStartedProcess && !singleInstance)
                {
                    listViewItem.Text += " *";
                    processNamesChecked.Add(process.ProcessName);
                }

            }

            this.processesListView.EndUpdate();

            UpdateProcessListLabel();

            if (_searchTextBox.Text != "")
                UpdateListViewWithSearchQuery();

            stopwatch.Stop();

            this.Text = $"Process Selector - Loaded in {stopwatch.Elapsed.TotalMilliseconds:F0}ms";
        }

        private static ImageList GetImageListFromCache()
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
            string processNameQuery = _searchTextBox.FormattedText;
            string commandlineQuery = _searchTextBox.GetQueryOperatorValue("/c");
            
            this.processesListView.BeginUpdate();
            this.processesListView.Items.Clear();

            foreach (ListViewItem processItem in _shadowProcessListItems.Values)
            {
                Process process = _processes[(int)processItem.Tag];
                if (process.ProcessName.IndexOf(processNameQuery, StringComparison.OrdinalIgnoreCase) != -1 &&
                    (commandlineQuery is null || processItem.ToolTipText.IndexOf(commandlineQuery, StringComparison.OrdinalIgnoreCase) != -1))
                {
                    this.processesListView.Items.Add(processItem);
                }
            }

            this.processesListView.EndUpdate();

            if (_searchTextBox.Text != "")
            {
                if (this.processesListView.Items.Count > 0)
                    _searchTextBox.BackColor = Color.LightGreen;
                else
                    _searchTextBox.BackColor = Color.FromArgb(255, 100, 100);
            }
            else
                _searchTextBox.BackColor = Color.White;

            UpdateProcessListLabel();
        }

        private void UpdateProcessListLabel()
        {
            int totalProcessCount = _shadowProcessListItems.Count;
            int processesListCount = this.processesListView.Items.Count;
            this.processListTotalLabel.Text = $"{processesListCount}/{totalProcessCount}";
        }

        private void processesView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.processesListView.SelectedItems.Count == 0)
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
            SelectedProcess = _processes[(int)processesListView.SelectedItems[0].Tag];
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
                UpdateProcessListView();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
         
    }
}
