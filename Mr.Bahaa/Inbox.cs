﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer_Online_Server.Client;
using Conquer_Online_Server.Network;
using Conquer_Online_Server.Network.GamePackets;
using Conquer_Online_Server.Database;
using System.IO;

namespace Conquer_Online_Server.MaTrix
{
    public class Inbox : Network.Writer, Interfaces.IPacket
    {
        public static void SendInbox(GameClient client, bool On)
        {

            byte[] test = new byte[12 + 8];
            Writer.WriteUshort((ushort)(test.Length - 8), 0, test);
            Writer.WriteUshort(1047, 2, test);
            if (On)
                test[4] = 3;
            else
                test[4] = 3;
            client.Send(test);
        }

        public class PrizeInfo
        {
            public void WriteItem(BinaryWriter writer)
            {
                writer.Write(ID); //= reader.ToUInt1632();
                writer.Write(Time);
                writer.Write(Sender);
                writer.Write(Subject);
                writer.Write(Message);
                writer.Write(goldprize);
                writer.Write(cpsprize);
            }
            public PrizeInfo ReadItem(BinaryReader reader)
            {
                ID = reader.ReadUInt32();//4
                Time = reader.ReadUInt32();//8
                Sender = reader.ReadString();//10
                Subject = reader.ReadString();//12
                Message = reader.ReadString();//14
                goldprize = reader.ReadUInt32();//18
                cpsprize = reader.ReadUInt32();//22            
                return this;
            }
            public uint ID;
            public uint Time;
            public string Sender;
            public string Subject;
            public string Message;
            public bool MessageOrGift;
            public Action<GameClient> itemprize;
            public uint goldprize;
            public uint cpsprize;
        }

        ///////////////////////////////////////////////////
        ///////////////////////////////////////////////////
        public static void Load(Client.GameClient client)
        {
            using (var cmd = new MySqlCommand(MySqlCommandType.SELECT))
            {
                cmd.Select("prizes").Where("UID", client.Entity.UID);
                using (MySqlReader rdr = new MySqlReader(cmd))
                {
                    if (rdr.Read())
                    {
                        byte[] data = rdr.ReadBlob("Prizes");
                        if (data.Length > 0)
                        {
                            using (var stream = new MemoryStream(data))
                            using (var reader = new BinaryReader(stream))
                            {
                                int count = reader.ReadByte();
                                for (uint x = 0; x < count; x++)
                                {
                                    PrizeInfo item = new PrizeInfo();
                                    item = item.ReadItem(reader);
                                    client.Prizes.Add(item.ID, item);
                                }
                            }
                        }
                    }
                    else
                    {
                        using (var command = new MySqlCommand(MySqlCommandType.INSERT))
                        {
                            command.Insert("prizes").Insert("UID", client.Entity.UID).Insert("Name", client.Entity.Name);
                            command.Execute();
                        }
                    }
                }
            }
        }
        ///////////////////////////////////////////////////
        ///////////////////////////////////////////////////
        public static void Save(Client.GameClient client)
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write((byte)client.Prizes.Count);
            foreach (var prize in client.Prizes.Values)
                prize.WriteItem(writer);
            string SQL = "UPDATE `prizes` SET Prizes=@Prizes where UID = " + client.Entity.UID + " ;";
            byte[] rawData = stream.ToArray();
            using (var conn = DataHolder.MySqlConnection)
            {
                conn.Open();
                using (var cmd = new MySql.Data.MySqlClient.MySqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = SQL;
                    cmd.Parameters.AddWithValue("@Prizes", rawData);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        ///////////////////////////////////////////////////
        ///////////////////////////////////////////////////       


        byte[] Buffer;
        public Inbox()
        {
            Buffer = new byte[20];
            WriteUshort((ushort)(Buffer.Length - 8), 0, Buffer);
            WriteUshort(1046, 2, Buffer);
        }

        public uint Count { get { return Conquer_Online_Server.BitConverter.ToUInt16(Buffer, 4); } set { WriteUint(value, 4, Buffer); } }

        public uint Page { get { return Conquer_Online_Server.BitConverter.ToUInt16(Buffer, 8); } set { WriteUint(value, 8, Buffer); } }

        public uint unknown { get { return Conquer_Online_Server.BitConverter.ToUInt16(Buffer, 12); } set { WriteUint(value, 12, Buffer); } }

        List<MaTrix.Inbox.PrizeInfo> list = new List<MaTrix.Inbox.PrizeInfo>();
        public void check(SafeDictionary<uint, MaTrix.Inbox.PrizeInfo> Prizes, uint page)
        {
            List<MaTrix.Inbox.PrizeInfo> prizes = Prizes.Values.ToList();
            list.Clear();
            for (int i = (int)page; i < page + 7; i++)
            {
                if (i < prizes.Count)
                {


                    list.Add(prizes[i]);
                }
            }
            if (list.Count > 0)
            {
                Buffer = new byte[20 + 92 * list.Count];
                WriteUshort((ushort)(Buffer.Length - 8), 0, Buffer);
                WriteUshort(1046, 2, Buffer);
                Count = (uint)list.Count;
                Page = page;
                unknown = (uint)prizes.Count;
                for (int i = 0; i < list.Count; i++)
                    Apend(list[i]);
            }

        }
        ushort offset = 16;
        public void Apend(PrizeInfo prize)
        {
            Uint(prize.ID, offset, Buffer);//uid 
            offset += 4;
            WriteString(prize.Sender, offset, Buffer);//sender
            offset += 32;
            WriteString(prize.Subject, offset, Buffer);//Subject
            offset += 32;

            Uint(prize.goldprize, offset, Buffer);//attachment
            offset += 4;

            Uint(prize.cpsprize, offset, Buffer);//attachment
            offset += 4;

            Uint(prize.Time, offset, Buffer);//Time
            offset += 4;

            Uint(prize.MessageOrGift == true ? (byte)1 : (byte)0, offset, Buffer);// image
            offset += 4;

            Uint(prize.itemprize != null ? (byte)1 : (byte)0, offset, Buffer);//attachment
            offset += 4;

        }

        private void Uint(uint p, ushort offset, byte[] Buffer)
        {
            throw new NotImplementedException();
        }
        public void Send(GameClient client)
        {
            client.Send(this.ToArray());
        }

        public void Deserialize(byte[] Data)
        {
            Buffer = Data;
        }

        public byte[] ToArray()
        {
            return Buffer;
        }


    }
}
