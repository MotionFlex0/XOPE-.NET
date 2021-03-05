using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using XOPE_UI.Core;
using XOPE_UI.Forms;
using XOPE_UI.Forms.Component;
using XOPE_UI.Injection;
using XOPE_UI.Native;
using XOPE_UI.Spy;

namespace XOPE_UI
{
    public partial class MainWindow : Form
    {
        const string XOPE_SPY_32 = "XOPESpy32.dll";
        const string XOPE_SPY_64 = "XOPESpy64.dll";

        int captureIndex = 0;

        ProcessDialog processDialog;
        ActiveConnectionsDialog activeConnectionsDialog;

        Process attachedProcess = null;

        Spy.Server server;
        Spy.SpyData spyData;

        public MainWindow()
        {
            //hexEditor = new WpfHexaEditor.HexEditor();
           //hexEditor.ForegroundSecondColor = System.Windows.Media.Brushes.Blue;
            //elementHost1.Child = hexEditor;

            InitializeComponent();

            //Using reflection to modify the protected DoubleBuffered property for ListView. DoubleBuffered is used to prevent flickering
            livePacketListView
                .GetType()
                .GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                .SetValue(livePacketListView, true, null);

            if (!File.Exists("XOPESpy32.dll"))
                System.Windows.Forms.MessageBox.Show("Canont find XOPESpy32.dll\nMake sure it is in the current directory\nWithout it, you cannot attach to 32-bit processes", "Missing DLL",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

            if (!File.Exists("XOPESpy64.dll") && Environment.Is64BitProcess)
                System.Windows.Forms.MessageBox.Show("Canont find XOPESpy64.dll\nMake sure it is in the current directory\nWithout it, you cannot attach to 64-bit processes", "Missing DLL",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

            if (!File.Exists("helper32.exe") && Environment.Is64BitProcess)
                System.Windows.Forms.MessageBox.Show("Canont find helper32.exe\nMake sure it is in the current directory\nWithout it, you cannot attach to 32-bit processes", "Missing helper executable",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

            spyData = new SpyData();

            processDialog = new ProcessDialog();
            activeConnectionsDialog = new ActiveConnectionsDialog(spyData);

            server = new Spy.Server(new PacketList(this.livePacketListView), liveViewOutput, spyData);
            server.runASync();
            captureTabControl.MouseClick += captureTabControl_MouseClick;
        }

        private void AttachToProcess()
        {
            DialogResult result = processDialog.ShowDialog(); //Maybe make processDialog a local var and dispose of dialog here
            if (result == DialogResult.OK)
            {
                //Console.WriteLine($"Successfully written int: {NativeMethods.WPM(processDialog.SelectedProcess.Handle, (IntPtr)0x008FFD34, 5000)}");
                //Console.WriteLine($"Successfully written byte: {NativeMethods.WPM(processDialog.SelectedProcess.Handle, (IntPtr)0x008FFD38, "Override partly")}");
                if (attachedProcess != null)
                    DetachFromProcess();

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

                    attachedProcess = processDialog.SelectedProcess;
                    attachedProcess.EnableRaisingEvents = true;
                    attachedProcess.Exited += attachedProcess_Exited;
                    recordToolStripButton.Enabled = true;
                }
                //int val = NativeMethods.RPM<int>(processDialog.SelectedProcess.Handle, (IntPtr)0x0133F934);
                //string strVal = NativeMethods.RPM(processDialog.SelectedProcess.Handle, (IntPtr)0x009861C0, 10);
                
            }
        }

        private void DetachFromProcess()
        {
            if (attachedProcess == null)
                return;

            bool res;
            if (!Environment.Is64BitProcess || NativeMethods.IsWow64Process(attachedProcess.Handle))
                res = CreateRemoteThread.Free32(attachedProcess.Handle, "XOPESpy32.dll");
            else
                res = CreateRemoteThread.Free64(attachedProcess.Handle, "XOPESpy64.dll");

            this.Text = "XOPE";
            attachedProcess = null;
            detachToolStripButton.Enabled = false;
            detachToolStripMenuItem.Enabled = false;
            recordToolStripButton.Enabled = false;
            pauseRecToolStripButton.Enabled = false;
            stopRecToolStripButton.Enabled = false;
        }

        private void CreditsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("Icon(s) made by Google from www.flaticon.com", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Console.WriteLine(Environment.CurrentDirectory);
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
            this.Text = $"XOPE";
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
                tabPage.Controls.Add(new CaptureTabPage());
                tabPage.Controls[0].Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                tabPage.Controls[0].Size = tabPage.Size - new System.Drawing.Size(2, 0);
                Console.WriteLine($"before: {tabPage.Size}");;
                
                //tabPage.Controls[0].Size = new System.Drawing.Size(150, 100);
                
                Console.WriteLine($"after: {tabPage.Controls[0].Size}");

                //Creates a new tabpage, based on the default tab page
                //captureTabControl.TabPages.Add(tabPage);
                ((System.Windows.Forms.ListView)tabPage.Controls[0].Controls["captureListView"]).Items.Add("abc123");
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

        private void livePacketListView_DoubleClick(object sender, EventArgs e)
        {
            var selectedItems = livePacketListView.SelectedItems;

            if (selectedItems.Count > 0)
            {
                using (PacketEditor packetEditor = new PacketEditor((byte[])selectedItems[0].Tag, false))
                {
                    DialogResult result = packetEditor.ShowDialog();
                    if (result == DialogResult.OK)
                    {

                    }
                }
            }
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (attachedProcess != null)
                DetachFromProcess();
        }
    }
}
