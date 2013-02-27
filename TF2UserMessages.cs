using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace netdecode
{
    partial class Packet
    {
        class OutputItem : ListViewItem
        {
            public OutputItem(string key, string msg)
            {
                Text = key;
                SubItems.Add(msg);
            }
        }

        void AddItem(string key, string msg, params string[] args)
        {
            List.Items.Add(new OutputItem(key, string.Format(msg, args)));
        }

        void HandleSayText(BitBuffer bb, TreeNode node, uint lengthBits)
        {
            var client = bb.ReadBits(8);
            var msg = bb.ReadString();
            var unk1 = bb.ReadBits(7);
            var unk2 = bb.ReadBool();

            AddItem("SayText", msg);
            node.Nodes.Add("Text: " + msg);
        }

        static string CleanChatMessage(string msg)
        {
            // = 0x02 (STX) - Use team color up to the end of the player name.
            // = 0x03 (ETX) - Use team color from this point forward
            // = 0x04 (EOT) - Use location color from this point forward
            // = 0x05 (ENQ) - Use achievement color from this point forward
            // = 0x01 (SOH) - Use normal color from this point forward
            msg = msg.Replace("\x02", string.Empty);
            msg = msg.Replace("\x03", string.Empty);
            msg = msg.Replace("\x04", string.Empty);
            msg = msg.Replace("\x05", string.Empty);
            msg = msg.Replace("\x01", string.Empty);

            return msg;
        }

        void HandleSayText2(BitBuffer bb, TreeNode node, uint lengthBits)
        {
            var endBit = bb._pos + lengthBits;

            var client = bb.ReadBits(8);

            // 0 - raw text, 1 - sets CHAT_FILTER_PUBLICCHAT 
            var isRaw = bb.ReadBits(8) != 0;

            // \x03 in the message for the team color of the specified clientid

            var kind = bb.ReadString();
            node.Nodes.Add("Kind: " + kind);

            var from = bb.ReadString();
            node.Nodes.Add("From: " + from);

            var msg = bb.ReadString();
            node.Nodes.Add("Text: " + msg);

            // This message can have two optional string parameters.
            var args = new List<string>();
            while (bb._pos < endBit)
            {
                var arg = bb.ReadString();
                args.Add(arg);
            }

            if (msg.StartsWith("#"))
                msg = msg.Substring(1);

            string s;
            if (StringTable.LookupString(kind, new List<string>() { from, msg }, out s))
            {
                s = CleanChatMessage(s);

                node.Nodes.Add("Full: " + s);
                AddItem(kind, s);
            }
        }

        void HandleTextMsg(BitBuffer bb, TreeNode node, uint lengthBits)
        {
            var endBit = bb._pos + lengthBits;
            var destType = bb.ReadBits(8);

            var msg = bb.ReadString();
            var textNode = node.Nodes.Add("Text:" + msg);
            
            var args = new List<string>();
            
            while (bb._pos < endBit)
            {
                var arg = bb.ReadString();
                args.Add(arg);
                if (!string.IsNullOrEmpty(arg))
                  textNode.Nodes.Add(arg);
            }

            if (msg.StartsWith("#"))
            {
                msg = msg.Substring(1);

                string s;
                if (StringTable.LookupString(msg, args, out s))
                {
                    node.Nodes.Add("Full: " + s);
                    AddItem("TextMsg", s);
                }
            }
            else
            {
                AddItem("TextMsg", msg);
            }
        }

        void player_spawn(BitBuffer bb, TreeNode node, uint lengthBits)
        {
            var userid = bb.ReadBits(16);
            node.Nodes.Add("User Id: " + userid);

            var team = (TFTeam)bb.ReadBits(16);
            node.Nodes.Add("Team: " + team);
            
            var @class = (TFClassType)bb.ReadBits(16);
            node.Nodes.Add("Class: " + @class);

            //Console.WriteLine("[player_spawn] user {0} class {1} team {2}",
            //    userid, @class, team); 
        }

        void player_changeclass(BitBuffer bb, TreeNode node, uint lengthBits)
        {
            var userid = bb.ReadBits(16);
            node.Nodes.Add("User Id: " + userid);

            var @class = bb.ReadBits(16);
            node.Nodes.Add("Class: " + @class);

            //Console.WriteLine("[player_changeclass] user {0} class {1}", userid, @class); 
        }

        void teamplay_round_start(BitBuffer bb, TreeNode node, uint lengthBits)
        {
            var fullRestart = bb.ReadBool();
            node.Nodes.Add("Full Restart: " + fullRestart);

            //Console.WriteLine("[teamplay_round_start] full restart {0}", fullRestart);
        }

        void teamplay_round_active(BitBuffer bb, TreeNode node, uint lengthBits)
        {
            //Console.WriteLine("[teamplay_round_active]");
        }

        void teamplay_round_win(BitBuffer bb, TreeNode node, uint lengthBits)
        {
            bb.Seek(lengthBits);
            //Console.WriteLine("[teamplay_round_win]");
        }

        void player_builtobject(BitBuffer bb, TreeNode node, uint lengthBits)
        {
            var userid = bb.ReadBits(16);
            var @object = bb.ReadBits(8);
            var index = bb.ReadBits(16);
            //Console.WriteLine("[player_builtobject]");
        }

        void player_death(BitBuffer bb, TreeNode node, uint lengthBits)
        {
            long endBit = bb._pos + lengthBits;

            var userid = bb.ReadBits(16);
            var victim_entindex = bb.ReadBits(32);
            var inflictor_entindex = bb.ReadBits(32);
            var attacker = bb.ReadBits(16);
            var weapon = bb.ReadString();
            var weaponid = bb.ReadBits(16);
            var damagebits = bb.ReadBits(32);
            var customkill = bb.ReadBits(16);
            var assister = (short)bb.ReadBits(16);
            var weapon_logclassname = bb.ReadString();
            var stun_flags = bb.ReadBits(16);
            var death_flags = bb.ReadBits(16);
            var silent_kill = bb.ReadBool();
            var playerpenetratecount = bb.ReadBits(16);
            string assister_fallback;
            if (assister == -1)
                assister_fallback = bb.ReadString();
            else
            {
                var unk = bb.ReadBits(8);
            }
                
            if (bb._pos != endBit) Debugger.Break();

            //Console.WriteLine("[player_death]");
        }

        void teamplay_broadcast_audio(BitBuffer bb, TreeNode node, uint lengthBits)
        {
            var team = (TFTeam)bb.ReadBits(8);
            var sound = bb.ReadString();

            AddItem("teamplay_broadcast_audio", "{0}: {1}", team.ToString(), sound);
        }

        void hltv_status(BitBuffer bb, TreeNode node, uint lengthBits)
        {
            //var clients = bb.ReadBits(16);
            bb.Seek(lengthBits);
        }
    }
}
