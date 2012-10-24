﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

namespace netdecode
{
    class Packet
    {
        struct MsgHandler
        {
            public string Name;
            public Action<BitBuffer, TreeNode> Handler;
        }

        private static readonly Dictionary<uint, MsgHandler> Handlers = new Dictionary<uint, MsgHandler>
        {
            {0, new MsgHandler { Name = "net_nop", Handler = (_, node) => { node.ForeColor = Color.Gray; }}},
            {1, new MsgHandler { Name = "net_disconnect", Handler = net_disconnect }},
            {2, new MsgHandler { Name = "net_file", Handler = net_file }},
            {3, new MsgHandler { Name = "net_tick", Handler = net_tick }},
            {4, new MsgHandler { Name = "net_stringcmd", Handler = net_stringcmd }},
            {5, new MsgHandler { Name = "net_setconvar", Handler = net_setconvar }},
            {6, new MsgHandler { Name = "net_signonstate", Handler = net_signonstate }},

            {7, new MsgHandler { Name = "svc_print", Handler = svc_print }},
            {8, new MsgHandler { Name = "svc_serverinfo", Handler = svc_serverinfo }},
            {9, new MsgHandler { Name = "svc_sendtable", Handler = svc_sendtable }},
            {10, new MsgHandler { Name = "svc_classinfo", Handler = svc_classinfo }},
            {11, new MsgHandler { Name = "svc_setpause", Handler = svc_setpause }},
            {12, new MsgHandler { Name = "svc_createstringtable", Handler = svc_createstringtable }},
            {13, new MsgHandler { Name = "svc_updatestringtable", Handler = svc_updatestringtable }},
            {14, new MsgHandler { Name = "svc_voiceinit", Handler = svc_voiceinit }},
            {15, new MsgHandler { Name = "svc_voicedata", Handler = svc_voicedata }},
            {17, new MsgHandler { Name = "svc_sounds", Handler = svc_sounds }},
            {18, new MsgHandler { Name = "svc_setview", Handler = svc_setview }},
            {19, new MsgHandler { Name = "svc_fixangle", Handler = svc_fixangle }},
            {20, new MsgHandler { Name = "svc_crosshairangle", Handler = svc_crosshairangle }},
            {21, new MsgHandler { Name = "svc_bspdecal", Handler = svc_bspdecal }},
            {23, new MsgHandler { Name = "svc_usermessage", Handler = svc_usermessage }},
            {24, new MsgHandler { Name = "svc_entitymessage", Handler = svc_entitymessage }},
            {25, new MsgHandler { Name = "svc_gameevent", Handler = svc_gameevent }},
            {26, new MsgHandler { Name = "svc_packetentities", Handler = svc_packetentities }},
            {27, new MsgHandler { Name = "svc_tempentities", Handler = svc_tempentities }},
            {28, new MsgHandler { Name = "svc_prefetch", Handler = svc_prefetch }},
            {29, new MsgHandler { Name = "svc_menu", Handler = svc_menu }},
            {30, new MsgHandler { Name = "svc_gameeventlist", Handler = svc_gameeventlist }},
            {31, new MsgHandler { Name = "svc_getcvarvalue", Handler = svc_getcvarvalue }},
            {32, new MsgHandler { Name = "svc_cmdkeyvalues", Handler = svc_cmdkeyvalues }}
        };

        public static void Parse(byte[] data, TreeNode node)
        {
            var bb = new BitBuffer(data);

            while (bb.BitsLeft() > 6)
            {
                var type = bb.ReadBits(6);
                MsgHandler handler;
                if (Handlers.TryGetValue(type, out handler))
                {
                    var sub = new TreeNode(handler.Name);
                    node.Nodes.Add(sub);
                    if (handler.Handler != null)
                    {
                        handler.Handler(bb, sub);
                    }
                    else
                        break;
                }
                else
                {
                    node.Nodes.Add("unknown message type " + type).ForeColor = Color.Crimson;
                    break;
                }
            }
        }

        // do we even encounter these in demo files?
        static void net_disconnect(BitBuffer bb, TreeNode node) 
        {
            node.Nodes.Add("Reason: " + bb.ReadString());
        }

        static void net_file(BitBuffer bb, TreeNode node)
        {
            node.Nodes.Add("Transfer ID: " + bb.ReadBits(32));
            node.Nodes.Add("Filename: " + bb.ReadString());
            node.Nodes.Add("Requested: " + bb.ReadBool());
        }

        static void net_tick(BitBuffer bb, TreeNode node)
        {
            node.Nodes.Add("Tick: " + (int)bb.ReadBits(32));
            node.Nodes.Add("Host frametime: " + bb.ReadBits(16));
            node.Nodes.Add("Host frametime StdDev: " + bb.ReadBits(16));
        }

        static void net_stringcmd(BitBuffer bb, TreeNode node)
        {
            node.Nodes.Add("Command: " + bb.ReadString());
        }

        static void net_setconvar(BitBuffer bb, TreeNode node)
        {
            var n = bb.ReadBits(8);
            while (n-- > 0)
                node.Nodes.Add(bb.ReadString() + ": " + bb.ReadString());
        }

        static void net_signonstate(BitBuffer bb, TreeNode node)
        {
            node.Nodes.Add("Signon state: " + bb.ReadBits(8));
            node.Nodes.Add("Spawn count: " + (int)bb.ReadBits(32));
        }

        static void svc_print(BitBuffer bb, TreeNode node)
        {
            node.Nodes.Add(bb.ReadString());
        }

        static void svc_serverinfo(BitBuffer bb, TreeNode node)
        {
            short version = (short)bb.ReadBits(16);
            node.Nodes.Add("Version: " + version);
            node.Nodes.Add("Server count: " + (int)bb.ReadBits(32));
            node.Nodes.Add("SourceTV: " + bb.ReadBool());
            node.Nodes.Add("Dedicated: " + bb.ReadBool());
            node.Nodes.Add("Server client CRC: 0x" + bb.ReadBits(32).ToString("X8"));
            node.Nodes.Add("Max classes: " + bb.ReadBits(16));
            if (version < 18)
                node.Nodes.Add("Server map CRC: 0x" + bb.ReadBits(32).ToString("X8"));
            else
                bb.Seek(128); // TODO: display out map md5 hash
            node.Nodes.Add("Current player count: " + bb.ReadBits(8));
            node.Nodes.Add("Max player count: " + bb.ReadBits(8));
            node.Nodes.Add("Interval per tick: " + bb.ReadFloat());
            node.Nodes.Add("Platform: " + (char)bb.ReadBits(8));
            node.Nodes.Add("Game directory: " + bb.ReadString());
            node.Nodes.Add("Map name: " + bb.ReadString());
            node.Nodes.Add("Skybox name: " + bb.ReadString());
            node.Nodes.Add("Hostname: " + bb.ReadString());
            node.Nodes.Add("Has replay: " + bb.ReadBool()); // ???: protocol version
        }

        static void svc_sendtable(BitBuffer bb, TreeNode node)
        {
            node.Nodes.Add("Needs decoder: " + bb.ReadBool());
            var n = bb.ReadBits(16);
            node.Nodes.Add("Length in bits: " + n);
            bb.Seek(n);
        }

        static void svc_classinfo(BitBuffer bb, TreeNode node)
        {
            var n = bb.ReadBits(16);
            node.Nodes.Add("Number of server classes: " + n);
            var cc = bb.ReadBool();
            node.Nodes.Add("Create classes on client: " + cc);
            if (!cc)
                while (n-- > 0)
                {
                    node.Nodes.Add("Class ID: " + bb.ReadBits((uint)Math.Log(n, 2) + 1));
                    node.Nodes.Add("Class name: " + bb.ReadString());
                    node.Nodes.Add("Datatable name: " + bb.ReadString());
                }
        }

        static void svc_setpause(BitBuffer bb, TreeNode node)
        {
            node.Nodes.Add(bb.ReadBool().ToString());
        }

        static void svc_createstringtable(BitBuffer bb, TreeNode node)
        {
            node.Nodes.Add("Table name: " + bb.ReadString());
            var m = bb.ReadBits(16);
            node.Nodes.Add("Max entries: " + m);
            node.Nodes.Add("Number of entries: " + bb.ReadBits((uint)Math.Log(m, 2) + 1));
            var n = bb.ReadBits(20);
            node.Nodes.Add("Length in bits: " + n);
            var f = bb.ReadBool();
            node.Nodes.Add("Userdata fixed size: " + f);
            if (f)
            {
                node.Nodes.Add("Userdata size: " + bb.ReadBits(12));
                node.Nodes.Add("Userdata bits: " + bb.ReadBits(4));
            }

            // ???: this is not in Source 2007 netmessages.h/cpp it seems. protocol version?
            node.Nodes.Add("Compressed: " + bb.ReadBool());
            bb.Seek(n);
        }

        static void svc_updatestringtable(BitBuffer bb, TreeNode node)
        {
            node.Nodes.Add("Table ID: " + bb.ReadBits(5));
            node.Nodes.Add("Changed entries: " + (bb.ReadBool() ? bb.ReadBits(16) : 1));
            var b = bb.ReadBits(20);
            node.Nodes.Add("Length in bits: " + b);
            bb.Seek(b);
        }

        static void svc_voiceinit(BitBuffer bb, TreeNode node)
        {
            node.Nodes.Add("Codec: " + bb.ReadString());
            node.Nodes.Add("Quality: " + bb.ReadBits(8));
        }

        static void svc_voicedata(BitBuffer bb, TreeNode node)
        {
            node.Nodes.Add("Client: " + bb.ReadBits(8));
            node.Nodes.Add("Proximity: " + bb.ReadBits(8));
            var b = bb.ReadBits(16);
            node.Nodes.Add("Length in bits: " + b);
            bb.Seek(b);
        }

        static void svc_sounds(BitBuffer bb, TreeNode node)
        {
            var r = bb.ReadBool();
            node.Nodes.Add("Reliable: " + r);
            node.Nodes.Add("Number of sounds: " + (r ? 1 : bb.ReadBits(8)));
            uint b = r ? bb.ReadBits(8) : bb.ReadBits(16);
            node.Nodes.Add("Length in bits: " + b);
            bb.Seek(b);
        }

        static void svc_setview(BitBuffer bb, TreeNode node)
        {
            node.Nodes.Add("Entity index: " + bb.ReadBits(11));
        }

        static void svc_fixangle(BitBuffer bb, TreeNode node)
        {
            node.Nodes.Add("Relative: " + bb.ReadBool());
            // TODO: handle properly
            bb.Seek(48);
        }

        static void svc_crosshairangle(BitBuffer bb, TreeNode node)
        {
            // TODO: see above
            bb.Seek(48);
        }

        static void svc_bspdecal(BitBuffer bb, TreeNode node)
        {
            node.Nodes.Add("Position: " + bb.ReadVecCoord());
            node.Nodes.Add("Decal texture index: " + bb.ReadBits(9));
            if (bb.ReadBool())
            {
                node.Nodes.Add("Entity index: " + bb.ReadBits(11));
                node.Nodes.Add("Model index: " + bb.ReadBits(12));
            }
            node.Nodes.Add("Low priority: " + bb.ReadBool());
        }

        static void svc_usermessage(BitBuffer bb, TreeNode node)
        {
            node.Nodes.Add("Message type: " + bb.ReadBits(8));
            var b = bb.ReadBits(11);
            node.Nodes.Add("Length in bits: " + b);
            bb.Seek(b);
        }

        static void svc_entitymessage(BitBuffer bb, TreeNode node)
        {
            node.Nodes.Add("Entity index: " + bb.ReadBits(11));
            node.Nodes.Add("Class ID: " + bb.ReadBits(9));
            var b = bb.ReadBits(11);
            node.Nodes.Add("Length in bits: " + b);
            bb.Seek(b);
        }

        static void svc_gameevent(BitBuffer bb, TreeNode node)
        {
            var b = bb.ReadBits(11);
            node.Nodes.Add("Length in bits: " + b);
            bb.Seek(b);
        }

        static void svc_packetentities(BitBuffer bb, TreeNode node)
        {
            node.Nodes.Add("Max entries: " + bb.ReadBits(11));
            bool d = bb.ReadBool();
            node.Nodes.Add("Is delta: " + d);
            if (d)
                node.Nodes.Add("Delta from: " + bb.ReadBits(32));
            node.Nodes.Add("Baseline: " + bb.ReadBool());
            node.Nodes.Add("Updated entries: " + bb.ReadBits(11));
            var b = bb.ReadBits(20);
            node.Nodes.Add("Length in bits: " + b);
            node.Nodes.Add("Update baseline: " + bb.ReadBool());
            bb.Seek(b);
        }

        static void svc_tempentities(BitBuffer bb, TreeNode node)
        {
            node.Nodes.Add("Number of entries: " + bb.ReadBits(8));
            var b = bb.ReadBits(17);
            node.Nodes.Add("Length in bits: " + b);
            bb.Seek(b);
        }

        static void svc_prefetch(BitBuffer bb, TreeNode node)
        {
            node.Nodes.Add("Sound index: " + bb.ReadBits(13));
        }

        static void svc_menu(BitBuffer bb, TreeNode node)
        {
            node.Nodes.Add("Menu type: " + bb.ReadBits(16));
            var b = bb.ReadBits(16);
            node.Nodes.Add("Length in bytes: " + b);
            bb.Seek(b << 3);
        }

        static void svc_gameeventlist(BitBuffer bb, TreeNode node)
        {
            node.Nodes.Add("Number of events: " + bb.ReadBits(9));
            var b = bb.ReadBits(20);
            node.Nodes.Add("Length in bits: " + b);
            bb.Seek(b);
        }

        static void svc_getcvarvalue(BitBuffer bb, TreeNode node)
        {
            node.Nodes.Add("Cookie: 0x" + bb.ReadBits(32).ToString("X8"));
            node.Nodes.Add(bb.ReadString());
        }

        static void svc_cmdkeyvalues(BitBuffer bb, TreeNode node)
        {
            var b = bb.ReadBits(32);
            node.Nodes.Add("Length in bits: " + b);
            bb.Seek(b);
        }
    }
}
