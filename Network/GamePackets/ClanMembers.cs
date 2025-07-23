﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer_Online_Server.Interfaces;


namespace Conquer_Online_Server.Network.GamePackets
{
    public class ClanMembers : Writer, IPacket
    {
        private Byte[] mData;
        private Int32 Offset = 16;

        public ClanMembers(Clan clan)
        {
            mData = new Byte[56 + (36 * clan.Members.Count) + 8];
            WriteUInt16((UInt16)(mData.Length - 8), 0, mData);
            WriteUInt16((UInt16)1312, 2, mData);

            UInt32 Count = (UInt32)clan.Members.Count();
            foreach (ClanMember member in clan.Members.Values)
            {
                WriteUInt32(Count, Offset, mData); Offset += 4;
                WriteString(member.Name, Offset, mData); Offset += 16;
                WriteUInt32((UInt32)member.Level, Offset, mData); Offset += 4;
                WriteUInt16(Convert.ToUInt16(member.Rank), Offset, mData); Offset += 2;
                WriteUInt16(Convert.ToUInt16(Kernel.GamePool.ContainsKey(member.Identifier)), Offset, mData); Offset += 2;
                WriteUInt32((UInt32)member.Class, Offset, mData); Offset += 4;
                WriteUInt32((UInt32)member.Donation, Offset, mData); Offset += 4;

                Count -= 1;
            }
            Offset = 16;
            Type = Clan.Types.Members;
        }
        public Clan.Types Type
        {
            get { return (Clan.Types)mData[4]; }
            set { WriteByte((Byte)value, 4, mData); }
        }
        public void Deserialize(byte[] buffer)
        {
            this.mData = buffer;
        }
        public byte[] ToArray()
        {
            return mData;
        }
        public void Send(Client.GameClient client)
        {
            client.Send(mData);
        }
    }
}
