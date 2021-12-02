using System;
using System.Diagnostics;
using System.Windows.Forms;
using XOPE_UI.Core;
using XOPE_UI.Forms;
using XOPE_UI.Forms.Component;
using XOPE_UI.Injection;
using XOPE_UI.Native;
using XOPE_UI.Util;
using XOPE_UI.Script;
using XOPE_UI.Definitions;
using XOPE_UI.Spy.DispatcherMessageType;
using System.Security.Principal;
using System.ComponentModel;
using XOPE_UI.Spy.Type;

namespace XOPE_UI
{
    public partial class MainWindow : Form
    {
        const string XOPE_SPY_32 = "XOPESpy32.dll";
        const string XOPE_SPY_64 = "XOPESpy64.dll";

        bool IsAdmin 
        { 
            get => (new WindowsPrincipal(WindowsIdentity.GetCurrent())).IsInRole(WindowsBuiltInRole.Administrator); 
        }

        string windowTitle = "XOPE";

        int captureIndex = 0;
        int filterIndex = 0;

        Process attachedProcess = null;

        SpyManager spyManager;
        LogDialog logDialog;
        ViewTabHandler viewTabHandler;
        ScriptManager scriptManager;

        SDK.Environment environment;

        Timer resizeTimer = new Timer();
        bool isResizing = false;

        public MainWindow()
        {
            InitializeComponent();

            // Initialise Logger and add a event handler to update status bar
            Logger logger = new Logger();
            logger.OnFlush += (object sender, string value) =>
            {
                if (this.IsHandleCreated && value.Trim() != "")
                    this.BeginInvoke(new Action(() => this.toolStripStatusLabel.Text = value.ReplaceLineEndings(" | ")));
            };
            logDialog = new LogDialog(logger);
            Console.WriteLine($"Program started at: {DateTime.Now}");

            //Credit: https://www.codeproject.com/Articles/317695/Detecting-If-An-Application-is-Running-as-An-Eleva
            if (IsAdmin)
                windowTitle += " [Elevated as Admin]";

            viewTabHandler = new ViewTabHandler(viewTab);
            viewTabHandler.AddView(captureViewButton, captureViewTabPage);
            viewTabHandler.AddView(filterViewButton, filterViewTabPage);
            viewTabHandler.AddView(replayViewButton, replayViewTabPage);

            scriptManager = new ScriptManager();

            livePacketListView.OnItemDoubleClick += LivePacketListView_OnItemDoubleClick;
            livePacketListView.OnItemSelectedChanged += LivePacketListView_OnItemSelectedChanged;

            spyManager = new SpyManager();

            spyManager.OnNewPacket += (object sender, Packet e) =>
            {
                if (recordToolStripButton.Tag != null && !recordToolStripButton.Enabled) // caoturing and not paused
                {
                    PacketListView listView = (PacketListView)captureTabControl.TabPages[(string)recordToolStripButton.Tag].Controls[0];
                    listView.Invoke(new Action(() => listView.Add(e)));

                }
                livePacketListView.Invoke(new Action(() => livePacketListView.Add(e)));
            };

            captureTabControl.MouseClick += captureTabControl_MouseClick;

            environment = SDK.Environment.GetEnvironment();
            spyManager.OnNewPacket += (object sender, Definitions.Packet e) =>
                environment.NotifyNewPacket(e.Data);

            resizeTimer.Interval = 100;
            resizeTimer.Tick += (object sender, EventArgs e) =>
            {
                this.ResumeLayout(true);
                this.SuspendLayout();
                resizeTimer.Stop();
            };

            this.Text = windowTitle;

            //Temp solution for reducing first-time loading time of Packet dialog
            using (PacketEditorReplayDialog p = new PacketEditorReplayDialog(null))
                p.Show();
        }

        public void AttachToProcess()
        {
            Process selectedProcess;
            using (ProcessDialog processDialog = new ProcessDialog(IsAdmin))
            {
                DialogResult result = processDialog.ShowDialog();
                if (result != DialogResult.OK)
                    return;
                else
                    selectedProcess = processDialog.SelectedProcess;
            }


            if (livePacketListView.Count > 0)
            {
                DialogResult shouldClearList = MessageBox.Show("Attaching to a new process will" +
                    " clear your packet list(s)\n" +
                    "Are ypu sure you want to do this?", "Warning",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (shouldClearList != DialogResult.Yes)
                    return;

                livePacketListView.Clear();
                this.packetCaptureHexPreview.ClearBytes();
            }

            if (attachedProcess != null)
                DetachFromProcess();

            bool spyAlreadyAttached = false;
            if (!Environment.Is64BitProcess || NativeMethods.IsWow64Process(selectedProcess.Handle))
                spyAlreadyAttached = NativeMethods.GetModuleHandle(selectedProcess.Handle, XOPE_UI.Config.Spy.ModuleName32) != IntPtr.Zero;
            else
                spyAlreadyAttached = NativeMethods.GetModuleHandle(selectedProcess.Handle, XOPE_UI.Config.Spy.ModuleName64) != IntPtr.Zero;

            if (spyAlreadyAttached)
            {
                DialogResult shouldFreePreviousSpy = MessageBox.Show("The Spy is already injected within this process.\n" +
                    "This may be due to another instance of XOPE.\n" +
                    "Would you like to allow this instance to remove the already existing Spy from this process?",
                    "Another XOPE instance?",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (shouldFreePreviousSpy != DialogResult.Yes)
                    return;

                // TODO: Attempt to removed existing Spy. 
                CreateRemoteThread.FreeSpy(selectedProcess.Handle);
            }

            Console.WriteLine($"Injecting into ${selectedProcess.ProcessName}.exe - {selectedProcess.Id}");

            spyManager.RunAsync(selectedProcess);

            bool res = CreateRemoteThread.InjectSpy(selectedProcess.Handle);

            if (res)
            {
                attachedProcess = selectedProcess;

                SetUiToAttachedState();
                attachedProcess.EnableRaisingEvents = true;
                attachedProcess.Exited += attachedProcess_Exited;
                environment.NotifyProcessAttached(attachedProcess);
            }
            else
                MessageBox.Show($"Error when AttachToProcess");
        }

        public void DetachFromProcess(bool alreadyFreed = false)
        {
            if (attachedProcess == null)
                return;

            spyManager.Shutdown();
            // This gives the Spy enough time to properly shutdown and dispose of its running threads (temp fix)
            System.Threading.Thread.Sleep(2000);

            if (!alreadyFreed)
            {
                bool res;
                if (!Environment.Is64BitProcess || NativeMethods.IsWow64Process(attachedProcess.Handle))
                    res = CreateRemoteThread.Free32(attachedProcess.Handle, XOPE_UI.Config.Spy.ModuleName32);
                else
                    res = CreateRemoteThread.Free64(attachedProcess.Handle, XOPE_UI.Config.Spy.ModuleName64);

                if (!res)
                    MessageBox.Show("Failed to free XOPESpy from the attached process");
            }
            ListViewItem listViewItem = new ListViewItem();
            SetUiToDetachedState();
            environment.NotifyProcessDetached(attachedProcess);
            attachedProcess = null;
        }

        private bool IsAttached()
        {
            if (attachedProcess == null)
            {
                MessageBox.Show("Not attached to a process");
                return false;
            }
            return true;
        }

        private void SetUiToAttachedState()
        {
            this.Invoke(new Action(() =>
            {
                string processName = attachedProcess.MainModule.ModuleName;

                Console.WriteLine($"Successfully injected into ${processName}.exe - {attachedProcess.Id}");
                this.Text = $"{windowTitle} - [{attachedProcess.Id}] {attachedProcess.MainModule.ModuleName}";
                detachToolStripButton.Enabled = true;
                detachToolStripMenuItem.Enabled = true;
                recordToolStripButton.Enabled = true;
            }));
        }

        private void SetUiToDetachedState()
        {
            this.Invoke(new Action(() =>
            {
                Console.WriteLine($"Successfully freed Spy from process");
                if (stopRecToolStripButton.Enabled)
                    stopRecToolStripButton.PerformClick();

                this.Text = windowTitle;
                detachToolStripButton.Enabled = false;
                detachToolStripMenuItem.Enabled = false;
                recordToolStripButton.Enabled = false;
                pauseRecToolStripButton.Enabled = false;
                stopRecToolStripButton.Enabled = false;
            }));
        }

        private void CreditsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("Icon(s) made by Google from www.flaticon.com", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Console.WriteLine($"Current Directory: {Environment.CurrentDirectory}");
        }

        private void attachToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AttachToProcess();
        }

        private void attachToolStripButton_Click(object sender, EventArgs e)
        {
            AttachToProcess();
        }

        private void detachToolStripButton_Click(object sender, EventArgs e)
        {
            DetachFromProcess();
        }

        private void detachToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DetachFromProcess();
        }

        private void attachedProcess_Exited(object sender, EventArgs e)
        {
            DetachFromProcess(true);
        }

        private void closeTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            captureTabControl.TabPages.Remove((TabPage)tabContextMenu.Tag);
        }

        private void captureTabControl_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                for (int i = 0; i < captureTabControl.TabCount; i++)
                {
                    if (captureTabControl.GetTabRect(i).Contains(e.Location))
                    {
                        if (i == 0)
                            return;

                        tabContextMenu.Tag = captureTabControl.TabPages[i];
                        break;
                    }
                }
                tabContextMenu.Show(this.captureTabControl, e.Location);
            }

        }

        private void recordToolStripButton_Click(object sender, EventArgs e)
        {
            recordToolStripButton.Enabled = false;
            pauseRecToolStripButton.Enabled = true;
            stopRecToolStripButton.Enabled = true;
            if (recordToolStripButton.Tag == null)
            {
                string newCaptureKey = "Capture " + captureIndex++;
                TabPage tabPage = new TabPage(newCaptureKey + " [Capturing]");
                captureTabControl.TabPages.Add(tabPage);
                tabPage.Name = newCaptureKey;
                tabPage.Controls.Add(new PacketListView());
                tabPage.Controls[0].Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                tabPage.Controls[0].Size = tabPage.Size;// - new System.Drawing.Size(2, 0);
                Console.WriteLine($"before: {tabPage.Size}"); ;

                Console.WriteLine($"after: {tabPage.Controls[0].Size}");

                //Creates a new tabpage, based on the default tab page
                //captureTabControl.TabPages.Add(tabPage);
                //((ListView)tabPage.Controls[0].Controls["captureListView"]).Items.Add("abc123");
                captureTabControl.SelectedTab = captureTabControl.TabPages[newCaptureKey];
                recordToolStripButton.Tag = newCaptureKey;
            }
            else
            {
                var t = captureTabControl.TabPages[(string)recordToolStripButton.Tag];

                t.Text = t.Text.Replace("[Paused]", "[Capturing]");
            }
        }

        private void pauseRecToolStripButton_Click(object sender, EventArgs e)
        {
            recordToolStripButton.Enabled = true;
            pauseRecToolStripButton.Enabled = false;
            stopRecToolStripButton.Enabled = true;
            var t = captureTabControl.TabPages[(string)recordToolStripButton.Tag];

            t.Text = t.Text.Replace("[Capturing]", "[Paused]");
        }

        private void stopRecToolStripButton_Click(object sender, EventArgs e)
        {
            recordToolStripButton.Enabled = true;
            pauseRecToolStripButton.Enabled = false;
            stopRecToolStripButton.Enabled = false;

            var t = captureTabControl.TabPages[(string)recordToolStripButton.Tag];
            t.Text = t.Text.Remove(t.Text.IndexOf(" ["));
            Console.WriteLine(t.Text);
            recordToolStripButton.Tag = null;
        }

        private void connectionsListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!IsAttached())
                return;
                
            using (ActiveConnectionsDialog dialog = new ActiveConnectionsDialog(spyManager))
            {
                dialog.UpdateActiveList();
                dialog.ShowDialog();
            }
        }

        private void LivePacketListView_OnItemDoubleClick(object sender, ListViewItem e)
        {
            using (PacketEditorReplayDialog packetEditorReplay = new PacketEditorReplayDialog(spyManager))
            {
                //[4] is the socket id. TODO: Store info about a packet in a proper structure
                packetEditorReplay.SocketId = Convert.ToInt32(e.SubItems[4].Text);
                packetEditorReplay.Data = (byte[])e.Tag;
                packetEditorReplay.Editible = true;
                packetEditorReplay.ShowDialog();
            }
        }

        private void LivePacketListView_OnItemSelectedChanged(object sender, ListViewItem e)
        {
            this.packetCaptureHexPreview.SetBytes((byte[])e.Tag);
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (attachedProcess != null)
                DetachFromProcess();
        }

        private void newPacketToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void logToolStripMenuItem_Click(object sender, EventArgs e)
        {
            logDialog.Show();
        }

        private void runScriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "C# File (*.cs)|*.cs";
            DialogResult result = openFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                scriptManager.AddCSScript(openFileDialog.FileName);

            }
        }

        private void scriptManagerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (ScriptManagerDialog scriptManagerDialog = new ScriptManagerDialog(scriptManager))
            {
                scriptManagerDialog.ShowDialog();

            }
        }

        private void filterListView_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = true;
        }

        private void filterListView_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            Debug.WriteLine($"e.Header.Text: {e.Header.Text}");
            if (e.Header.Text == "Activated")
            {
                //CheckBoxRenderer.DrawCheckBox(e.Graphics, e.Bounds.Location, CheckBoxState.CheckedNormal);
                //e.DrawDefault = false;
            }
        }

        private void filterListView_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;

        }

        private void addFilterButton_Click(object sender, EventArgs e)
        {
            const int MAX_FORMAT_LENGTH = 15;

            using (FilterEditorDialog filterEditorDialog = new FilterEditorDialog())
            {
                DialogResult result = filterEditorDialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    ListViewItem listViewItem = new ListViewItem();
                    FilterEntry filter = filterEditorDialog.Filter;

                    string beforeStr = BitConverter.ToString(filter.OldValue, 0).Replace("-", " ");
                    string beforeFormatted = beforeStr.Substring(0, Math.Min(MAX_FORMAT_LENGTH, beforeStr.Length));
                    beforeFormatted += beforeStr.Length > MAX_FORMAT_LENGTH ? "..." : "";

                    string afterStr = BitConverter.ToString(filter.NewValue, 0).Replace("-", " ");
                    string afterFormatted = afterStr.Substring(0, Math.Min(MAX_FORMAT_LENGTH, afterStr.Length));
                    afterFormatted += afterStr.Length > MAX_FORMAT_LENGTH ? "..." : "";

                    listViewItem.Text = $"{filterIndex++}";
                    listViewItem.SubItems.Add(filter.Name);
                    listViewItem.SubItems.Add($"{beforeFormatted} --> {afterFormatted}");
                    filterListView.Items.Add(listViewItem);

                    EventHandler<IncomingMessage> addSendFilterCallback = (object sender, IncomingMessage response) =>
                        Console.WriteLine(response.Json);

                    spyManager.MessageDispatcher.Send(new AddSendFilter(addSendFilterCallback)
                    {
                        SocketId = filter.SocketId,
                        OldValue = filter.OldValue,
                        NewValue = filter.NewValue,
                        ReplaceEntirePacket = false
                    });
                }

            }
        }

        private void deleteFilterButton_Click(object sender, EventArgs e)
        {
            if (filterListView.SelectedItems.Count > 0)
                filterListView.Items.Remove(filterListView.SelectedItems[0]);
        }

        private void filterListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            deleteFilterButton.Enabled = filterListView.SelectedItems.Count > 0;
        }

        private void pingTestSpyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!IsAttached())
                return;

            Stopwatch roundTripStopwatch = new Stopwatch();
            Ping ping = new Ping();
            ping.OnResponse += (ev, response) =>
            {
                roundTripStopwatch.Stop();
                Console.WriteLine($"Received response from Ping.\r\n" +
                    $"Round trip time: {roundTripStopwatch.ElapsedMilliseconds}ms\r\n" +
                    $"Response:\r\n{response.Json}");
            };

            roundTripStopwatch.Start();
            spyManager.MessageDispatcher.Send(ping);
        }

        private void socketCheckerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (attachedProcess == null && spyManager.MessageDispatcher != null)
            {
                MessageBox.Show("Not currently attached to a process.");
                return;
            }

            using (SocketCheckerDialog socketCheckerDialog = new SocketCheckerDialog(spyManager.MessageDispatcher))
            {
                socketCheckerDialog.ShowDialog();
            }
        }

        private void restartAsAdminToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (IsAdmin)
                MessageBox.Show("This process is already running as admin.", "Already Admin", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
            {
                if (MessageBox.Show("Are you sure you would like to restart process as Admin?\nYour progress will NOT be saved.",
                    "Warning",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning) == DialogResult.No)
                {
                    return;
                }

                Process process = new Process();
                process.StartInfo.FileName = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.Verb = "runas";
                
                try
                {
                    process.Start();
                    Environment.Exit(0);
                }
                catch (Win32Exception ex)
                {
                    MessageBox.Show($"Failed to start as Admin.\n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            if (isResizing)
            {
                resizeTimer.Stop();
                resizeTimer.Start();
            }
            base.OnResize(e);
        }

        protected override void OnResizeBegin(EventArgs e)
        {
            SuspendLayout();
            resizeTimer.Start();
            isResizing = true;

            base.OnResizeBegin(e);
        }

        protected override void OnResizeEnd(EventArgs e)
        {
            base.OnResizeEnd(e);
            resizeTimer.Stop();
            ResumeLayout();
            isResizing = false;
        }
    }

}
