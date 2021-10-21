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
        private Stack<Tuple<ListViewItem, int>> hiddenProcessListItems; // <processListViewItem, listViewIndex>
        private bool isElevated;

        private int oldSearchLength = 0;

        public ProcessDialog(bool isElevated)
        {
            InitializeComponent();

            this.isElevated = isElevated;

            processes = new SortedDictionary<int, Process>();
            hiddenProcessListItems = new Stack<Tuple<ListViewItem, int>>();

            processesListView.Columns[0].Width = 400;//processesView.Width;
            is64bitText.Text = Environment.Is64BitProcess.ToString();

            this.processesListView
            .GetType()
            .GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
            .SetValue(processesListView, true, null);
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
            hiddenProcessListItems.Clear();
            oldSearchLength = 0;
            this.searchTextBox.Text = "";

            Process[] ps = Process.GetProcesses();
            foreach (Process p in ps)
                processes.Add(p.Id, p);

            this.processesListView.BeginUpdate();
            this.processesListView.Items.Clear();

            if (objectCache.Contains("PROCESS_SMALL_IMAGE_LIST"))
                this.processesListView.SmallImageList = (ImageList)objectCache.Get("PROCESS_SMALL_IMAGE_LIST");
            else
            {
                ImageList imageList = new ImageList
                {
                    ColorDepth = ColorDepth.Depth32Bit
                };
                this.processesListView.SmallImageList = imageList;
                objectCache.Add("PROCESS_SMALL_IMAGE_LIST", imageList, DateTime.Now.AddMinutes(30));
            }

            foreach (KeyValuePair<int, Process> kv in processes)
            { 
                Process p = kv.Value;
                int pid = p.Id;
                
                string cacheKey = $"PROCESS_BLACKLIST_{pid}_{p.ProcessName}";
                if ((!isElevated && p.SessionId == 0) || objectCache.Contains(cacheKey))
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
                    }));
                }
                catch (Win32Exception ex)
                {
                    objectCache.Add(cacheKey, true, DateTime.Now.AddMinutes(30));
                }
                catch (Exception ex)
                {

                }
            }
            this.processesListView.EndUpdate();

            updateProcessListLabel();

            stopwatch.Stop();

            this.Text = $"Process Selector - Loaded in {stopwatch.Elapsed.Milliseconds}ms";
        }

        private void updateProcessListLabel()
        {
            int hiddenListCount = hiddenProcessListItems.Count;
            int processesListCount = this.processesListView.Items.Count;
            this.processListTotalLabel.Text = $"{processesListCount}/{processesListCount + hiddenListCount}";
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
            string searchQuery = this.searchTextBox.Text;

            this.processesListView.BeginUpdate();

            if (searchTextBox.Text.Length > oldSearchLength)
            {
                for (int i = this.processesListView.Items.Count - 1; i >= 0; i--)
                {
                    ListViewItem processItem = this.processesListView.Items[i];
                    Process process = processes[(int)processItem.Tag];
                    if (process.ProcessName.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) == -1)
                    {
                        hiddenProcessListItems.Push(new Tuple<ListViewItem, int>(processItem, processItem.Index));
                        processItem.Remove();
                    }
                }
            }
            else if (searchTextBox.Text.Length < oldSearchLength)
            {
                while (hiddenProcessListItems.Count > 0)
                {
                    Tuple<ListViewItem, int> tuple = hiddenProcessListItems.Pop();

                    Process process = processes[(int)tuple.Item1.Tag];
                    if (process.ProcessName.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) != -1)
                    {
                        if (tuple.Item2 < this.processesListView.Items.Count)
                            this.processesListView.Items.Insert(tuple.Item2, tuple.Item1);
                        else
                        {
                            this.processesListView.Items.Add(tuple.Item1);
                            Debug.WriteLine("Could not add processListViewItem back to its original position");
                        }
                    }
                    else
                    {
                        hiddenProcessListItems.Push(tuple);
                        break;
                    }
                }
            }

            this.processesListView.EndUpdate();

            updateProcessListLabel();

            oldSearchLength = searchTextBox.Text.Length;
        }
    }
}
