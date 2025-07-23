using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class TitlePacket2 : Writer, Interfaces.IPacket
    {
        private Byte[] mData;
        private Int32 Offset = 11;
        public byte Prof;
        public TitlePacket2(Boolean create)
        {
            if (create)
            {
                mData = new Byte[24 + 8];
                Writer.WriteUInt16((UInt16)(mData.Length - 8), 0, mData);
                Writer.WriteUInt16((UInt16)1130, 2, mData);
            }
        }
        public UInt32 Identifier
        {
            get { return BitConverter.ToUInt32(mData, 4); }
            set { Writer.WriteUInt32((UInt32)value, 4, mData); }
        }
        public Titles Title
        {
            get { return (Titles)mData[8]; }
            set { mData[8] = (Byte)value; }
        }
        public Types Type
        {
            get { return (Types)mData[9]; }
            set { Writer.WriteUInt16((UInt16)value, 9, mData); }
        }
        public Byte Count
        {
            get { return mData[10]; }
            set { mData[10] = value; }
        }
        public void Add(Byte id)
        {
            Byte[] tmp = new Byte[mData.Length];
            mData.CopyTo(tmp, 0);

            mData = new Byte[tmp.Length + 1 + 8];
            Buffer.BlockCopy(tmp, 0, mData, 0, tmp.Length);

            Writer.WriteUInt16((UInt16)(mData.Length - 8), 0, mData);

            Writer.WriteByte(id, Offset, mData);

            Offset++;
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
        public enum Types : ushort
        {
            Switch = 3,
            List = 4
        }
        public enum Titles : ushort
        {
            None = 0,

            GoldenRacer = 11,

            ElitePKChamption_Low = 12,
            ElitePK2ndPlace_Low = 13,
            ElitePK3ndPlace_Low = 14,
            ElitePKTopEight_Low = 15,
            VipMember = 7,
            ElitePKChamption_High = 16,
            ElitePK2ndPlace_High = 17,
            ElitePK3ndPlace_High = 18,
            ElitePKTopEight_High = 19,
        }
    }
}

