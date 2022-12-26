using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Security.Principal;
using System.ComponentModel;
using XOPE_UI.Core;
using XOPE_UI.Injection;
using XOPE_UI.Model;
using XOPE_UI.Native;
using XOPE_UI.Script;
using XOPE_UI.Spy.DispatcherMessageType;
using XOPE_UI.Util;
using XOPE_UI.View;
using XOPE_UI.View.Component;
using System.Threading.Tasks;
using System.Threading;
using XOPE_UI.Settings;
using Newtonsoft.Json;
using System.Runtime.Caching;
using System.Collections.Generic;
using System.IO;

namespace XOPE_UI
{
    // TODO: A lot of refactoring and decoupling 
    public partial class MainWindow : Form
    {
        bool IsAdmin => 
            (new WindowsPrincipal(WindowsIdentity.GetCurrent())).IsInRole(WindowsBuiltInRole.Administrator);

        string _windowTitle = "XOPE";

        int _captureIndex = 0;

        Process _attachedProcess = null;

        Logger _logger;
        ViewTabHandler _viewTabHandler;
        ScriptManager _scriptManager;
        IUserSettings _settings;
        SpyManager _spyManager;

        SDK.Environment _environment;

        System.Windows.Forms.Timer _resizeTimer = new System.Windows.Forms.Timer();
        bool _isResizing = false;

        public MainWindow(IUserSettings settings)
        {
            InitializeComponent();

            _settings = settings;
            this.livePacketListView.ChangeBytesLength(
                _settings.GetValue<int>(UserSettingsKey.MAX_BYTES_SHOWN_FOR_PACKET_VIEW));
            _settings.Get(UserSettingsKey.MAX_BYTES_SHOWN_FOR_PACKET_VIEW).PropertyChanged += (o, e) =>
            {
                if (e.PropertyName == nameof(SettingsEntry.Value))
                {
                    this.livePacketListView.ChangeBytesLength((int)(o as SettingsEntry).Value);
                }
            };

            // Initialise Logger and add a event handler to update status bar
            _logger = new Logger();
            _logger.TextWritten += (object sender, string value) =>
            {
                if (this.IsHandleCreated && value.Trim() != "")
                    this.BeginInvoke(new Action(() => this.toolStripStatusLabel.Text = value.ReplaceLineEndings(" | ")));
            };

            Console.WriteLine($"Program started at: {DateTime.Now}");

            //Credit: https://www.codeproject.com/Articles/317695/Detecting-If-An-Application-is-Running-as-An-Eleva
            if (IsAdmin)
                _windowTitle += " [Elevated as Admin]";

            _viewTabHandler = new ViewTabHandler(viewTab);
            _viewTabHandler.AddView(captureViewButton, captureViewTabPage);
            _viewTabHandler.AddView(filterViewButton, filterViewTabPage);
            _viewTabHandler.AddView(replayViewButton, replayViewTabPage);

            _scriptManager = new ScriptManager();

            livePacketListView.ItemDoubleClicked += PacketListView_ItemDoubleClicked;
            livePacketListView.ItemSelectedChanged += PacketListView_ItemSelectedChanged;
            
            _spyManager = new SpyManager();
            filterViewTab.AttachSpyManager(_spyManager);
            filterViewTab.AttachSettings(_settings);

            _spyManager.NewPacket += (object sender, Packet e) =>
            {
                if (recordToolStripButton.Tag != null && !recordToolStripButton.Enabled) // capturing and not paused
                {
                    PacketListView listView = (PacketListView)captureTabControl.TabPages[(string)recordToolStripButton.Tag].Controls[0];
                    listView.Invoke(new Action(() => listView.Add(e)));

                }
                livePacketListView.Invoke(new Action(() => livePacketListView.Add(e)));
            };

            captureTabControl.MouseClick += captureTabControl_MouseClick;

            _environment = SDK.Environment.GetEnvironment();
            _spyManager.NewPacket += (object sender, Model.Packet e) =>
                _environment.NotifyNewPacket(e.Data);

            _resizeTimer.Interval = 100;
            _resizeTimer.Tick += (object sender, EventArgs e) =>
            {
                this.ResumeLayout(true);
                this.SuspendLayout();
                _resizeTimer.Stop();
            };

            this.Text = _windowTitle;

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
                    "Are you sure you want to do this?", "Warning",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (shouldClearList != DialogResult.Yes)
                    return;

                livePacketListView.Clear();
                this.packetCaptureHexPreview.ClearBytes();
            }

            if (_attachedProcess != null)
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
            }

            using (ProcessAttachingDialog loadingDialog = new ProcessAttachingDialog(selectedProcess.ProcessName))
            {
                Task.Run(() =>
                {
                    if (spyAlreadyAttached)
                    {
                        // TODO: Attempt to removed existing Spy. 
                        //CreateRemoteThread.FreeSpy(selectedProcess.Handle);
                        
                        while (spyAlreadyAttached)
                        {
                            if (!Environment.Is64BitProcess || NativeMethods.IsWow64Process(selectedProcess.Handle))
                                spyAlreadyAttached = NativeMethods.GetModuleHandle(selectedProcess.Handle, 
                                    XOPE_UI.Config.Spy.ModuleName32) != IntPtr.Zero;
                            else
                                spyAlreadyAttached = NativeMethods.GetModuleHandle(selectedProcess.Handle, 
                                    XOPE_UI.Config.Spy.ModuleName64) != IntPtr.Zero;

                            if (loadingDialog.CancellationToken.IsCancellationRequested || selectedProcess.HasExited)
                            {
                                Console.WriteLine($"Cancelling injecting into {selectedProcess.ProcessName}.");
                                loadingDialog.CloseDialog();
                                return;
                            }

                            Thread.Sleep(1000);
                        }
                    }

                    Console.WriteLine($"Injecting into [{selectedProcess.Id}] - {selectedProcess.ProcessName}.exe...");

                    _spyManager.RunAsync($"{Config.Spy.ReceiverPipeNamePrefix}{selectedProcess.Id}");

                    bool res = CreateRemoteThread.InjectSpy(selectedProcess.Handle);

                    if (res)
                    {
                        _attachedProcess = selectedProcess;

                        SetUiToAttachedState();
                        _attachedProcess.EnableRaisingEvents = true;
                        _attachedProcess.Exited += attachedProcess_Exited;
                        _spyManager.AttachedToProcess(_attachedProcess);
                        _environment.NotifyProcessAttached(_attachedProcess);

                        // Auto-inject other dlls | TODO: temporary solution
                        ObjectCache objectCache = MemoryCache.Default;
                        Dictionary<string, AutoInjectorEntry> dllsToInject = objectCache.Get(
                            XOPE_UI.Presenter.AutoInjectorDialogPresenter.CACHE_KEY) as Dictionary<string, AutoInjectorEntry>;
                        if (dllsToInject != null)
                        {
                            foreach (AutoInjectorEntry entry in dllsToInject.Values)
                            {
                                if (entry.IsActivated)
                                    CreateRemoteThread.Inject(_attachedProcess.Handle, entry.FilePath);
                            }
                        }
                    }
                    else
                        MessageBox.Show($"Error when AttachToProcess");

                    loadingDialog.CloseDialog();
                }, loadingDialog.CancellationToken.Token);

                loadingDialog.ShowDialog();
            }
        }

        public void DetachFromProcess(bool alreadyFreed = false)
        {
            if (!_spyManager.IsAttached)
                return;

            _spyManager.Shutdown();
            // This gives the Spy enough time to properly shutdown and dispose of its running threads (temp fix)
            //System.Threading.Thread.Sleep(2000);

            if (!alreadyFreed)
            {
                //bool res;
                //if (!Environment.Is64BitProcess || NativeMethods.IsWow64Process(_attachedProcess.Handle))
                //    res = CreateRemoteThread.Free32(_attachedProcess.Handle, XOPE_UI.Config.Spy.ModuleName32);
                //else
                //    res = CreateRemoteThread.Free64(_attachedProcess.Handle, XOPE_UI.Config.Spy.ModuleName64);

                //if (!res)
                //    MessageBox.Show("Failed to free XOPESpy from the attached process");
            }

            SetUiToDetachedState();
            _spyManager.DetachedFromProcess();
            _environment.NotifyProcessDetached(_attachedProcess);
            _attachedProcess = null;
        }

        private bool IsAttached(bool showMessage = true)
        {
            if (!_spyManager.IsAttached)
            {
                if (showMessage)
                    MessageBox.Show("Not attached to a process");
                return false;
            }
            return true;
        }

        private void SetUiToAttachedState()
        {
            this.Invoke(new Action(() =>
            {
                string processName = _attachedProcess.MainModule.ModuleName;

                Console.WriteLine($"Successfully injected into [{_attachedProcess.Id}]{processName}. Waiting for Spy to connect to Receiver...");
                this.Text = $"{_windowTitle} - [{_attachedProcess.Id}] {_attachedProcess.MainModule.ModuleName}";
                detachToolStripButton.Enabled = true;
                detachToolStripMenuItem.Enabled = true;
                recordToolStripButton.Enabled = true;
                filterViewTab.Enabled = true;
            }));
        }

        private void SetUiToDetachedState()
        {
            this.Invoke(new Action(() =>
            {
                Console.WriteLine("Detached from process.");
                if (stopRecToolStripButton.Enabled)
                    stopRecToolStripButton.PerformClick();

                this.Text = _windowTitle;
                this.detachToolStripButton.Enabled = false;
                this.detachToolStripMenuItem.Enabled = false;
                this.recordToolStripButton.Enabled = false;
                this.pauseRecToolStripButton.Enabled = false;
                this.stopRecToolStripButton.Enabled = false;
                this.filterViewTab.Enabled = false;
                this.httpTunnelingModeToolStripMenuItem.Checked = false;
            }));
        }

        /*
         * Events
         */

        private void CreditsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Icon(s) made by Google from www.flaticon.com", "Credits", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                string newCaptureKey = "Capture " + _captureIndex++;

                PacketListView packetListView = new PacketListView();
                packetListView.Anchor = ~AnchorStyles.None;
                packetListView.Size = captureTabControl.TabPages[0].Size;
                packetListView.ItemDoubleClicked += PacketListView_ItemDoubleClicked;
                packetListView.ItemSelectedChanged += PacketListView_ItemSelectedChanged;

                //Creates a new tabpage, based on the default tab page
                TabPage tabPage = new TabPage(newCaptureKey + " [Capturing]");
                tabPage.Name = newCaptureKey;
                tabPage.Controls.Add(packetListView);
                captureTabControl.TabPages.Add(tabPage);

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
                
            using (ActiveConnectionsDialog dialog = new ActiveConnectionsDialog(_spyManager))
            {
                dialog.ShowDialog();
            }
        }

        private void PacketListView_ItemDoubleClicked(object sender, Packet e)
        {
            using (PacketEditorReplayDialog packetEditorReplay = new PacketEditorReplayDialog(_spyManager))
            {
                //[4] is the socket id. TODO: Store info about a packet in a proper structure
                packetEditorReplay.SocketId = e.Socket;
                packetEditorReplay.Data = e.Data;
                packetEditorReplay.Editible = true;
                packetEditorReplay.ShowDialog();
            }
        }

        private void PacketListView_ItemSelectedChanged(object sender, Packet e)
        {
            this.packetCaptureHexPreview.SetBytes(e.Data);
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_attachedProcess != null)
                DetachFromProcess();
        }

        private void newPacketToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!IsAttached())
                return;

            using (PacketEditorReplayDialog packetEditorReplayDialog = 
                new PacketEditorReplayDialog(_spyManager)) 
            {
                packetEditorReplayDialog.Data = new byte[] { 0x00 };
                packetEditorReplayDialog.Editible = true;
                packetEditorReplayDialog.ShowDialog();
            }
        }

        private void logToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LogDialog.ShowOrBringToFront(_logger);
        }

        private void runScriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "C# File (*.cs)|*.cs";
            DialogResult result = openFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                _scriptManager.AddCSScript(openFileDialog.FileName);

            }
        }

        private void scriptManagerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (ScriptManagerDialog scriptManagerDialog = new ScriptManagerDialog(_scriptManager))
            {
                scriptManagerDialog.ShowDialog();
            }
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
            _spyManager.MessageDispatcher.Send(ping);
        }

        private void socketCheckerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!_spyManager.IsAttached || _spyManager.MessageDispatcher == null)
            {
                MessageBox.Show("Not currently attached to a process.");
                return;
            }

            using (SocketCheckerDialog socketCheckerDialog = 
                new SocketCheckerDialog(_spyManager.MessageDispatcher))
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
                if (MessageBox.Show("Are you sure you would like to restart XOPE as an Admin?\nYour progress will NOT be saved.\n\n" +
                    "Note: The target process needs to also be run as an admin.",
                    "Warning",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning) == DialogResult.No)
                {
                    return;
                }

                Process process = new Process();
                process.StartInfo.FileName = Process.GetCurrentProcess().MainModule.FileName;
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.Verb = "runas";
                
                try
                {
                    process.Start();
                    Environment.Exit(0);
                }
                catch (Win32Exception ex)
                {
                    MessageBox.Show($"Failed to start as Admin.\n\n{ex.Message}", 
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            if (_isResizing)
            {
                _resizeTimer.Stop();
                _resizeTimer.Start();
            }
            base.OnResize(e);
        }

        protected override void OnResizeBegin(EventArgs e)
        {
            SuspendLayout();
            _resizeTimer.Start();
            _isResizing = true;

            base.OnResizeBegin(e);
        }

        protected override void OnResizeEnd(EventArgs e)
        {
            base.OnResizeEnd(e);
            _resizeTimer.Stop();
            ResumeLayout();
            _isResizing = false;
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SettingsDialog settingsDialog = new SettingsDialog(_settings))
            {
                settingsDialog.ShowDialog();
            }
        }

        private void loadFiltersToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void saveFiltersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "XOPE Filters (*.xof)|*.xof";
                DialogResult dialogResult = saveFileDialog.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    string content = filterViewTab.Presenter.GetFiltersAsJson();
                    MessageBox.Show($"File name: {saveFileDialog.FileName}\nJson:\n{content}");
                }
            }
        }

        private void autoInjectorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (AutoInjectorDialog dialog = new AutoInjectorDialog())
            {
                dialog.ShowDialog();
                this.autoInjectorToolStripMenuItem.Checked = dialog.IsAnyActive;
            }
        }

        private void httpTunnelingModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!IsAttached())
                return;

            using (HttpTunnelingDialog dialog = new HttpTunnelingDialog(_spyManager))
            {
                dialog.ShowDialog();
                if (_spyManager.IsTunneling)
                    this.httpTunnelingModeToolStripMenuItem.Checked = true;
                else
                    this.httpTunnelingModeToolStripMenuItem.Checked = false;
            }
        }
    }

}
