using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace netdecode
{
    public partial class MainForm : Form
    {
        DemoFile _currentFile;
        GameStringTable _stringTable;

        public MainForm()
        {
            InitializeComponent();

            var file = "tf_english.txt";
            if (File.Exists(file))
            {
                var stream = File.OpenText(file);

                _stringTable = new GameStringTable(stream);
                if (!_stringTable.Parse())
                {
                    MessageBox.Show("Could not read TF2 text strings.");
                    return;
                }
            }
        }

        private TreeNode ParseIntoTree(DemoFile.DemoMessage msg)
        {
            var node = new TreeNode(String.Format("{0}, tick {1}, {2} bytes",
                msg.Type, msg.Tick, msg.Data.Length));
            node.Expand();
            node.BackColor = DemoMessageItem.GetTypeColor(msg.Type);

            Packet packet = new Packet();
            packet.StringTable = _stringTable;
            packet.List = eventsList;

            if (_currentFile.GameEvents.Count != 0)
                packet.GameEvents = _currentFile.GameEvents;

            switch (msg.Type)
            {
                case DemoFile.MessageType.ConsoleCmd:
                    node.Nodes.Add(new TreeNode(Encoding.ASCII.GetString(msg.Data)));
                    break;
                case DemoFile.MessageType.UserCmd:
                    UserCmd.ParseIntoTreeNode(msg.Data, node);
                    break;
                case DemoFile.MessageType.Signon:
                case DemoFile.MessageType.Packet:
                    try
                    {
                        packet.Parse(msg.Data, node);
                    }
                    catch (IndexOutOfRangeException ex)
                    {
                        // There was some error reading the buffers
                        node.Nodes.Add(new TreeNode("Error reading message."));
                        MessageBox.Show("Error reading message: " + msg.Tick);
                        break;
                    }

                    if (packet.GameEvents.Count > 0 && _currentFile.GameEvents.Count == 0)
                        _currentFile.GameEvents = packet.GameEvents;
                    break;
                case DemoFile.MessageType.DataTables:
                    DataTables.Parse(msg.Data, node);
                    break;
                default:
                    node.Nodes.Add(new TreeNode("Unhandled demo message type."));
                    break;
            }

            return node;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Filter = "Demo files|*.dem",
                RestoreDirectory = true
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                messageList.Items.Clear();

                Stream f = ofd.OpenFile();
                _currentFile = new DemoFile(f);
                f.Close();

                var Items = new List<DemoMessageItem>();
                foreach (var msg in _currentFile.Messages)
                {
                    Items.Add(new DemoMessageItem(msg));
                }

                messageList.Items.AddRange(Items.ToArray());
            }

            SimulateDemo();

            propertiesToolStripMenuItem.Enabled = true;
            simulateToolStripMenuItem.Enabled = true;
        }

        private void messageList_SelectedIndexChanged(object sender, EventArgs e)
        {
            messageTree.Nodes.Clear();

            messageTree.BeginUpdate();
            foreach(DemoMessageItem item in messageList.SelectedItems)
            {
                var node = ParseIntoTree(item.Msg);
                var nodeId = messageTree.Nodes.Add(node);
                //messageTree.ExpandAll();
            }
            messageTree.EndUpdate();

            tabControl1.SelectedIndex = 1;
        }

        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Demo protocol: " + _currentFile.Info.DemoProtocol + "\n"
                + "Net protocol: " + _currentFile.Info.NetProtocol + "\n"
                + "Server name: " + _currentFile.Info.ServerName + "\n"
                + "Client name: " + _currentFile.Info.ClientName + "\n"
                + "Map name: " + _currentFile.Info.MapName + "\n"
                + "Game directory: " + _currentFile.Info.GameDirectory + "\n"
                + "Length in seconds: " + _currentFile.Info.Seconds + "\n"
                + "Tick count: " + _currentFile.Info.TickCount + "\n"
                + "Frame count: " + _currentFile.Info.FrameCount);
        }

        private void simulateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SimulateDemo();
        }

        private void SimulateDemo()
        {
            if (_currentFile == null)
                return;

            eventsList.Items.Clear();

            eventsList.BeginUpdate();
            foreach (var msg in _currentFile.Messages)
            {
                var node = ParseIntoTree(msg);
            }
            eventsList.EndUpdate();
        }

        private void messageTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag == null)
                return;

            var tag = e.Node.Tag as Packet.PacketNodeData;
            var data = tag.Data.Skip(tag.Offset).ToArray();
            hexBox1.ByteProvider = new Be.Windows.Forms.DynamicByteProvider(data);
        }

        void updateHexOffset()
        {
            var curPos = (hexBox1.BytesPerLine * (hexBox1.CurrentLine - 1) +
                hexBox1.CurrentPositionInLine) - 1;
            toolStripStatusLabel1.Text = "Offset: " + curPos;
        }

        private void hexBox1_CurrentPositionInLineChanged(object sender, EventArgs e)
        {
            updateHexOffset();
        }

        private void hexBox1_CurrentLineChanged(object sender, EventArgs e)
        {
            updateHexOffset();
        }

        private void messageTree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                dumpToFileToolStripMenuItem.Tag = e.Node;
                contextMenuStrip1.Show(messageTree.PointToScreen(e.Location));
            }
        }

        private void dumpToFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var node = dumpToFileToolStripMenuItem.Tag as TreeNode;
            var data = node.Tag as Packet.PacketNodeData;

            if (data == null)
                return;

            var sfd = new SaveFileDialog();
         
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllBytes(sfd.FileName, data.Data.Skip(data.Offset).ToArray());
            }
        }
    }

    class DemoMessageItem : ListViewItem
    {
        public DemoFile.DemoMessage Msg;

        public DemoMessageItem(DemoFile.DemoMessage msg)
        {
            Msg = msg;

            BackColor = GetTypeColor(msg.Type);
            Text = Msg.Tick.ToString();
            SubItems.Add(Msg.Type.ToString());
            SubItems.Add(Msg.Data.Length.ToString());
        }

        public static Color GetTypeColor(DemoFile.MessageType type)
        {
            switch(type) {
                case DemoFile.MessageType.Signon:
                case DemoFile.MessageType.Packet:
                    return Color.LightBlue;
                case DemoFile.MessageType.UserCmd:
                    return Color.LightGreen;
                case DemoFile.MessageType.ConsoleCmd:
                    return Color.LightPink;
                default:
                    return Color.White;
            }
        }
    }
}
