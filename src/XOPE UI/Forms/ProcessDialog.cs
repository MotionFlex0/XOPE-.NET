using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using XOPE_UI.Native;

namespace XOPE_UI.Forms
{
    public partial class ProcessDialog : Form
    {
        private Process[] Processes { get; set; }
        public Process SelectedProcess { get; private set; }
        public string SelectedProcessName { get; private set; }

        public ProcessDialog()
        {
            InitializeComponent();
            processesView.Columns[0].Width = 400;//processesView.Width;
            is64bitText.Text = Environment.Is64BitProcess.ToString();
        }

        //TODO: fix threading issue https://stackoverflow.com/questions/38423472/what-is-the-difference-between-task-run-and-task-factory-startnew
        private void ProcessDialog_Load(object sender, EventArgs e)
        {
            this.confirmButton.Enabled = false;

            Processes = Process.GetProcesses();

            this.processesView.SmallImageList = new ImageList();
            this.processesView.SmallImageList.ColorDepth = ColorDepth.Depth32Bit;

            Processes = Processes.OrderBy(p => p.Id).Where(p => p.ProcessName.Contains("PacketSender") || p.ProcessName.Contains("RuneLite") || p.ProcessName.Contains("SimpleClient")).ToArray();

            var context = TaskScheduler.FromCurrentSynchronizationContext();

            Task.Run(() =>
            {
                this.processesView.Items.Clear();

                List<ListViewItem> listViewItemsQueue = new List<ListViewItem>(); //Stagger the adds to the listView or else it becomes a flicking mess
                foreach (Process p in Processes)
                {
                    try
                    {
                        //p.
                        string processFilePath = NativeMethods.GetFullProcessName(p.Handle, 0);
                        string processName = Path.GetFileName(processFilePath);
                        ListViewItem listViewItem = new ListViewItem($"[{p.Id}] {processName}", processName)
                        {
                            Tag = p.Id
                        };

                        //Task.Run<string>()
                        Task.Factory.StartNew(() =>
                        {
                            this.processesView.SmallImageList.Images.Add(processName, Icon.ExtractAssociatedIcon(processFilePath));

                            listViewItemsQueue.Add(listViewItem);
                            if (listViewItemsQueue.Count() >= 6)
                            {
                                processesView.Items.AddRange(listViewItemsQueue.ToArray());
                                listViewItemsQueue.Clear();
                            }
                        }, Task.Factory.CancellationToken, TaskCreationOptions.None, context).Wait();
                        
                    }
                    catch (Win32Exception ex)
                    {

                    }
                    catch (Exception ex)
                    {

                    }
                }

                if (listViewItemsQueue.Count > 0)
                {
                    Task.Factory.StartNew(() =>
                    {
                        processesView.Items.AddRange(listViewItemsQueue.ToArray());
                    }, Task.Factory.CancellationToken, TaskCreationOptions.None, context).Wait();
                }
                    
            });
        }

        private void processesView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (processesView.SelectedItems.Count == 0)
                confirmButton.Enabled = false;
            else
            {
                if (!confirmButton.Enabled)
                    confirmButton.Enabled = true;
            }
                
        }

        private void confirmButton_Click(object sender, EventArgs e)
        {
            SelectedProcessName = processesView.SelectedItems[0].ImageKey;
            SelectedProcess = Processes.Single(p => p.Id == (int)processesView.SelectedItems[0].Tag);
            this.DialogResult = DialogResult.OK;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
