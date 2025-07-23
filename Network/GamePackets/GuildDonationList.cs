using Conquer_Online_Server.Database;
using Conquer_Online_Server.Game.ConquerStructures.Society;
using System;
using System.IO;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class GuildDonationList
    {
        public ushort Size;
        public ushort Type;
        public ushort SubType;
        public ushort PageNumber;
        public Guild g;
        private byte[] byte_0;
        public GuildDonationList(byte[] packet)
        {
            System.IO.BinaryReader binaryReader = new System.IO.BinaryReader(new System.IO.MemoryStream(packet));
            this.Size = binaryReader.ReadUInt16();
            this.Type = binaryReader.ReadUInt16();
            this.SubType = binaryReader.ReadUInt16();
            this.PageNumber = binaryReader.ReadUInt16();
        }
        public byte[] Build()
        {
            this.byte_0 = new byte[696];
            Writer.WriteUInt16(688, 0, this.byte_0);
            Writer.WriteUInt16(2101, 2, this.byte_0);
            Writer.WriteUInt16(this.SubType, 4, this.byte_0);
            Writer.WriteUInt16((ushort)this.g.Members.Count, 6, this.byte_0);
            Writer.WriteUInt32(100u, 8, this.byte_0);
            int num = 0;
            int num2 = 10;
            int num3 = 0;
            ushort num4 = 12;
            foreach (Guild.Member current in this.g.Members.Values)
            {
                if (num >= num3 && num < num2)
                {
                    if (current.IsOnline)
                    {
                        Writer.WriteUInt32(current.ID, (int)num4, this.byte_0);
                        num4 += 4;
                        Writer.WriteUInt32((uint)current.Rank, (int)num4, this.byte_0);
                        num4 += 4;
                        Writer.WriteUInt32((uint)((ushort)num), (int)num4, this.byte_0);
                        num4 += 4;
                        Writer.WriteUInt32((uint)current.SilverDonation, (int)num4, this.byte_0);
                        num4 += 4;
                        Writer.WriteUInt32((uint)current.ConquerPointDonation, (int)num4, this.byte_0);
                        num4 += 4;
                        Writer.WriteUInt32(0u, (int)num4, this.byte_0);
                        num4 += 4;
                        Writer.WriteUInt32(0u, (int)num4, this.byte_0);
                        num4 += 4;
                        Writer.WriteUInt32((uint)current.Client.Entity.ARSDON, (int)num4, this.byte_0);
                        num4 += 4;
                        Writer.WriteUInt32(current.Client.Entity.Flowers.RedRoses, (int)num4, this.byte_0);
                        num4 += 4;
                        Writer.WriteUInt32(current.Client.Entity.Flowers.Orchads, (int)num4, this.byte_0);
                        num4 += 4;
                        Writer.WriteUInt32(current.Client.Entity.Flowers.Lilies, (int)num4, this.byte_0);
                        num4 += 4;
                        Writer.WriteUInt32(current.Client.Entity.Flowers.Tulips, (int)num4, this.byte_0);
                        num4 += 4;
                        Writer.WriteUInt32((uint)current.TotalDoantion, (int)num4, this.byte_0);
                        num4 += 4;
                        Writer.WriteString(current.Name, (int)num4, this.byte_0);
                        num4 += 16;
                    }
                    else
                    {
                        Writer.WriteUInt32(current.ID, (int)num4, this.byte_0);
                        num4 += 4;
                        Writer.WriteUInt32((uint)current.Rank, (int)num4, this.byte_0);
                        num4 += 4;
                        Writer.WriteUInt32((uint)((ushort)num), (int)num4, this.byte_0);
                        num4 += 4;
                        Writer.WriteUInt32((uint)current.SilverDonation, (int)num4, this.byte_0);
                        num4 += 4;
                        Writer.WriteUInt32((uint)current.ConquerPointDonation, (int)num4, this.byte_0);
                        num4 += 4;
                        Writer.WriteUInt32(0u, (int)num4, this.byte_0);
                        num4 += 4;
                        Writer.WriteUInt32(0u, (int)num4, this.byte_0);
                        num4 += 4;
                        ulong num5 = 0uL;
                        MySqlCommand mySqlCommand = new MySqlCommand(MySqlCommandType.SELECT);
                        mySqlCommand.Select("guild_arsenalsdonation").Where("guild_uid", (long)((ulong)current.GuildID)).And("d_uid", (long)((ulong)current.ID));
                        MySqlReader mySqlReader = new MySqlReader(mySqlCommand);
                        while (mySqlReader.Read())
                        {
                            ulong num6 = (ulong)mySqlReader.ReadUInt32("item_donation");
                            num5 += num6;
                        }
                        Writer.WriteUInt32((uint)num5, (int)num4, this.byte_0);
                        num4 += 4;
                        ulong num7 = 0;
                        ulong num8 = 0;
                        ulong num9 = 0;
                        ulong num10 = 0;
                        MySqlCommand mySqlCommand2 = new MySqlCommand(MySqlCommandType.SELECT);
                        mySqlCommand2.Select("flowers").Where("id", (long)((ulong)current.ID));
                        MySqlReader mySqlReader2 = new MySqlReader(mySqlCommand2);
                        while (mySqlReader2.Read())
                        {
                            num7 = (ulong)mySqlReader2.ReadUInt32("redroses");
                            num8 = (ulong)mySqlReader2.ReadUInt32("lilies");
                            num9 = (ulong)mySqlReader2.ReadUInt32("orchads");
                            num10 = (ulong)mySqlReader2.ReadUInt32("tulips");
                        }
                        Writer.WriteUInt32((uint)num7, (int)num4, this.byte_0);
                        num4 += 4;
                        Writer.WriteUInt32((uint)num9, (int)num4, this.byte_0);
                        num4 += 4;
                        Writer.WriteUInt32((uint)num8, (int)num4, this.byte_0);
                        num4 += 4;
                        Writer.WriteUInt32((uint)num10, (int)num4, this.byte_0);
                        num4 += 4;
                        Writer.WriteUInt32((uint)current.TotalDoantion, (int)num4, this.byte_0);
                        num4 += 4;
                        Writer.WriteString(current.Name, (int)num4, this.byte_0);
                        num4 += 16;
                    }
                    num++;
                }
            }
            return this.byte_0;
        }
    }
}
