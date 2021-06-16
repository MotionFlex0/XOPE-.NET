using CSScriptLib;
using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using XOPE_UI.Core;
using XOPE_UI.Forms;
using XOPE_UI.Forms.Component;
using XOPE_UI.Injection;
using XOPE_UI.Native;
using XOPE_UI.Definitions;
using XOPE_UI.Util;
using XOPE_UI.Spy;
using XOPE_UI.Script;
using System.Threading;

namespace XOPE_UI
{
    public partial class MainWindow : Form
    {
        const string XOPE_SPY_32 = "XOPESpy32.dll";
        const string XOPE_SPY_64 = "XOPESpy64.dll";

        public event EventHandler<IntPtr> OnProcessAttached;
        public event EventHandler<IntPtr> OnProcessDetached;

        int captureIndex = 0;
        
        ProcessDialog processDialog;
        ActiveConnectionsDialog activeConnectionsDialog;
        /*
         * Not a big fan of this. Unfortunately, due to performance issues with WPFHexaEditor,
         * this is required. TODO: Fix in the future
         */
        PacketEditorReplayDialog packetEditorReplayDialog; 

        Process attachedProcess = null;

        IServer server;

        LogDialog logDialog;
        ViewTabHandler viewTabHandler;
        ScriptManager scriptManager;

        ByteViewer packetCaptureHexPreview;

        SDK.Environment environment;

        public MainWindow(IServer server)
        {
            InitializeComponent();

            this.server = server;

            logDialog = new LogDialog(new Logger());
            Console.WriteLine($"Program started at: {DateTime.Now}");

            viewTabHandler = new ViewTabHandler(viewTab);
            viewTabHandler.AddView(captureViewButton, captureViewTabPage);
            viewTabHandler.AddView(replayViewButton, replayViewTabPage);

            scriptManager = new ScriptManager();

            // For WpfHexaEditor
            //packetCaptureHexPreview.ForegroundSecondColor = System.Windows.Media.Brushes.Blue;
            //packetCaptureHexPreview.StatusBarVisibility = System.Windows.Visibility.Hidden;
            //packetCaptureHexPreview.Focusable = false;

            packetCaptureHexPreview = new ByteViewer();
            packetCaptureHexPreview.Size = new Size(hexPreviewPanel.Width, hexPreviewPanel.Height);
            packetCaptureHexPreview.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            hexPreviewPanel.Controls.Add(packetCaptureHexPreview);

            processDialog = new ProcessDialog();
            activeConnectionsDialog = new ActiveConnectionsDialog(server.Data);
            packetEditorReplayDialog = new PacketEditorReplayDialog(server);

            livePacketListView.OnItemDoubleClick += LivePacketListView_OnItemDoubleClick;
            livePacketListView.OnItemSelectedChanged += LivePacketListView_OnItemSelectedChanged;

            server.OnNewPacket += (object sender, Definitions.Packet e) =>
                livePacketListView.Invoke((MethodInvoker)(() => livePacketListView.Add(e)));

            captureTabControl.MouseClick += captureTabControl_MouseClick;

            environment = SDK.Environment.GetEnvironment();
            server.OnNewPacket += (object sender, Definitions.Packet e) =>
                environment.EmitNewPacket(e.Data);
        }

        public void AttachToProcess()
        {
            DialogResult result = processDialog.ShowDialog(); //Maybe make processDialog a local var and dispose of dialog here
            if (result == DialogResult.OK)
            {
                //Console.WriteLine($"Successfully written int: {NativeMethods.WPM(processDialog.SelectedProcess.Handle, (IntPtr)0x008FFD34, 5000)}");
                //Console.WriteLine($"Successfully written byte: {NativeMethods.WPM(processDialog.SelectedProcess.Handle, (IntPtr)0x008FFD38, "Override partly")}");
                if (attachedProcess != null)
                    DetachFromProcess();

                server.RunAsync();

                bool res;
                if (!Environment.Is64BitProcess || NativeMethods.IsWow64Process(processDialog.SelectedProcess.Handle))
                    res = CreateRemoteThread.Inject32(processDialog.SelectedProcess.Handle, $@"{Environment.CurrentDirectory}\XOPESpy32.dll");
                else
                    res = CreateRemoteThread.Inject64(processDialog.SelectedProcess.Handle, $@"{Environment.CurrentDirectory}\XOPESpy64.dll");

                if (res)
                {
                    this.Text = $"XOPE - [{processDialog.SelectedProcess.Id}] {processDialog.SelectedProcessName}";
                    detachToolStripButton.Enabled = true;
                    detachToolStripMenuItem.Enabled = true;
                    recordToolStripButton.Enabled = true;

                    attachedProcess = processDialog.SelectedProcess;
                    attachedProcess.EnableRaisingEvents = true;
                    attachedProcess.Exited += attachedProcess_Exited;
                    environment.EmitProcessAttached(attachedProcess);
                }
                else
                    MessageBox.Show($"Error when AttachToProcess");
                //int val = NativeMethods.RPM<int>(processDialog.SelectedProcess.Handle, (IntPtr)0x0133F934);
                //string strVal = NativeMethods.RPM(processDialog.SelectedProcess.Handle, (IntPtr)0x009861C0, 10);
                
            }
        }

        public void DetachFromProcess()
        {
            if (attachedProcess == null)
                return;

            bool res;
            if (!Environment.Is64BitProcess || NativeMethods.IsWow64Process(attachedProcess.Handle))
                res = CreateRemoteThread.Free32(attachedProcess.Handle, "XOPESpy32.dll");
            else
                res = CreateRemoteThread.Free64(attachedProcess.Handle, "XOPESpy64.dll");

            this.Text = "XOPE";
            detachToolStripButton.Enabled = false;
            detachToolStripMenuItem.Enabled = false;
            recordToolStripButton.Enabled = false;
            pauseRecToolStripButton.Enabled = false;
            stopRecToolStripButton.Enabled = false;
            environment.EmitProcessDetached(attachedProcess);
            attachedProcess = null;
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
            this.Invoke((MethodInvoker)(() => this.Text = "XOPE" ));
            attachedProcess = null;
            detachToolStripButton.Enabled = false;
            detachToolStripMenuItem.Enabled = false;
            recordToolStripButton.Enabled = false;
            pauseRecToolStripButton.Enabled = false;
            stopRecToolStripButton.Enabled = false;
        }

        private void closeTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //ToolStripMenuItem tab ((ToolStripMenuItem)sender)
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
                        if (i == 0 || i == 1)
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
                Console.WriteLine($"before: {tabPage.Size}");;
                
                //tabPage.Controls[0].Size = new System.Drawing.Size(150, 100);
                
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
            activeConnectionsDialog.UpdateActiveList();
            activeConnectionsDialog.ShowDialog();
        }

        private void LivePacketListView_OnItemDoubleClick(object sender, ListViewItem e)
        {
            //[4] is the socket id. TODO: Store info about a packet in a proper structure
            packetEditorReplayDialog.SocketId = Convert.ToInt32(e.SubItems[4].Text);
            packetEditorReplayDialog.Data = (byte[])e.Tag;
            packetEditorReplayDialog.Editible = true;
            DialogResult result = packetEditorReplayDialog.ShowDialog();
            if (result == DialogResult.OK)
            {

            }
        }

        private void LivePacketListView_OnItemSelectedChanged(object sender, ListViewItem e)
        {
            packetCaptureHexPreview.SetBytes((byte[])e.Tag);
            packetCaptureHexPreview.SetDisplayMode(DisplayMode.Hexdump);
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
    }
}
