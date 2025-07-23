using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer_Online_Server.Client;
using Conquer_Online_Server.Network;
using System.IO;
using Conquer_Online_Server.Database;

namespace Conquer_Online_Server.MaTrix
{
    public class Way2Heroes : Network.Writer, Interfaces.IPacket
    {
        public static void SendDone(GameClient client)
        {
            byte[] Buffer = new byte[106 + 8];
            WriteUInt16((ushort)(Buffer.Length - 8), 0, Buffer);
            WriteUInt16(2832, 2, Buffer);
            Ushort(10, 4, Buffer);//count
            int offset = 6;
            for (uint i = 0; i < 10; i++)
            {
                Uint(i + 1, offset, Buffer); offset += 5;//id
                Buffer[offset++] = 0;//has something to claim
                Uint(4, offset, Buffer); offset += 4;//items done
            }
            client.Send(Buffer);
        }

        private static void Uint(uint p, int offset, byte[] Buffer)
        {
            throw new NotImplementedException();
        }

        private static void Ushort(int p, int p_2, byte[] Buffer)
        {
            throw new NotImplementedException();
        }
        public struct Stage
        {
            public uint UID;
            public uint Progress;
            public uint[] Prize;
        }
        public static Dictionary<uint, Stage> Stages;

        public class StageInfo
        {
            public uint UID;
            public bool Done;
            public SafeDictionary<uint, SubStageInfo> SubStages = new SafeDictionary<uint, SubStageInfo>();
        }
        public class SubStageInfo
        {
            public uint UID;
            public bool Done;
            public uint points;
        }
        public static void Load()
        {
            Stages = new Dictionary<uint, Stage>();
            string[] text = File.ReadAllLines("database\\Reward\\stagegoal.txt");
            Stage Stage = new Stage();
            for (int x = 0; x < text.Length; x++)
            {
                string line = text[x];
                string[] split = line.Split('=');
                if (line.StartsWith("[") && line.EndsWith("]") && !line.Contains("StageGoal"))
                {
                    string[] numbers = line.Replace("[", "").Replace("]", "").Split('-');
                    uint id = uint.Parse(numbers[0]) * 100;
                    if (numbers.Length > 1)
                        id += uint.Parse(numbers[1]);
                    if (Stages.ContainsKey(id))
                        Stage = Stages[uint.Parse(split[1])];
                    else
                    {
                        Stage = new Stage();
                        Stage.UID = id;
                        Stage.Prize = new uint[3];
                        Stages.Add(id, Stage);
                    }
                }
                else
                {
                    if (split[0] == "Progress")
                        Stage.Progress = uint.Parse(split[1]);
                    else if (split[0] == "Prize1")
                    {
                        string[] PrizeLine = split[1].Split(new string[] { "@@" }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < PrizeLine.Length; i++)
                        {
                            Stage.Prize[i] = uint.Parse(PrizeLine[i]);
                        }

                    }
                }
            }

            Conquer_Online_Server.Console.WriteLine("stagegoal loaded.");
        }
        /// <summary>
        ///
        /// </summary>

        byte[] buffer;
        public Way2Heroes(uint page)
        {
            buffer = new byte[11 + 8];
            Writer.Ushort((ushort)(buffer.Length - 8), 0, buffer);
            Writer.Ushort(2831, 2, buffer);
            Page = page;
        }

        private uint Page
        {
            get { return Conquer_Online_Server.BitConverter.ReadUint(buffer, 4); }
            set { WriteUint(value, 4, buffer); }
        }
        private bool AllDone
        {
            get { return buffer[8] == 1; }
            set { buffer[8] = value == true ? (byte)1 : (byte)0; }
        }
        private ushort Count
        {
            get { return Conquer_Online_Server.BitConverter.ReadUshort(buffer, 9); }
            set { Ushort(value, 9, buffer); }
        }

        int offset = 11;
        public void check(StageInfo stages)
        {
            if (stages.SubStages.Count > 0)
            {
                byte[] bytes = new byte[11 + 8 + stages.SubStages.Count * 9];
                buffer.CopyTo(bytes, 0);
                WriteUInt16((ushort)(bytes.Length - 8), 0, bytes);
                buffer = bytes;


                Count = (ushort)stages.SubStages.Count;
                AllDone = stages.Done;

                var items = stages.SubStages.Values.ToArray();
                for (uint i = 0; i < items.Length; i++)
                {
                    Uint(items[i].UID, offset, buffer); offset += 8;//id                    
                    buffer[offset++] = items[i].Done == true ? (byte)1 : (byte)0; // //has something to claim    

                }
            }
        }
        public void Send(GameClient client)
        {
            client.Send(this.ToArray());
        }
        public void Deserialize(byte[] Data)
        {
            buffer = Data;
        }
        public byte[] ToArray()
        {
            return buffer;
        }
    }
}