using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class GenericRanking : Writer, Interfaces.IPacket
    {
        public const uint
            Ranking = 1,
            QueryCount = 2,
            InformationRequest = 5;
        public const uint
            RoseFairy = 30000002,
            LilyFairy = 30000102,
            OrchidFairy = 30000202,
            TulipFairy = 30000302,
            Chi = 60000000,
            DragonChi = 60000001,
            PhoenixChi = 60000002,
            TigerChi = 60000003,
            TurtleChi = 60000004;

        byte[] Buffer;
        int current;

        public GenericRanking(bool Create, uint entries = 1)
        {
            if (Create)
            {
                Buffer = new byte[32 + entries * 56];
                WriteUInt16((ushort)(24 + entries * 56), 0, Buffer);
                WriteUInt16(1151, 2, Buffer);
            }
        }

        public uint Mode
        {
            get { return BitConverter.ToUInt32(Buffer, 4); }
            set { WriteUInt32(value, 4, Buffer); }
        }

        public uint RankingType
        {
            get { return BitConverter.ToUInt32(Buffer, 8); }
            set { WriteUInt32(value, 8, Buffer); }
        }

        public ushort RegisteredCount
        {
            get { return BitConverter.ToUInt16(Buffer, 12); }
            set { WriteUInt16(value, 12, Buffer); }
        }

        public ushort Page
        {
            get { return BitConverter.ToUInt16(Buffer, 14); }
            set { WriteUInt16(value, 14, Buffer); }
        }

        public uint Count
        {
            get { return BitConverter.ToUInt32(Buffer, 16); }
            set { WriteUInt32(value, 16, Buffer); }
        }

        public void Append(uint rank, uint amount, uint uid, string name)
        {
            int offset = current * 56 + 24;
            if (offset + 56 <= Buffer.Length)
            {
                current++;
                Count = (uint)current;
                WriteUInt32(rank, offset, Buffer); offset += 8;
                WriteUInt32(amount, offset, Buffer); offset += 8;
                WriteUInt32(uid, offset, Buffer); offset += 4;
                WriteUInt32(uid, offset, Buffer); offset += 4;
                WriteString(name, offset, Buffer); offset += 16;
                WriteString(name, offset, Buffer); offset += 16;
            }
        }
        public void Reset()
        {
            current = 0;
        }

        public void Send(Client.GameClient client)
        {
            client.Send(Buffer);
        }

        public byte[] ToArray()
        {
            return Buffer;
        }

        public void Deserialize(byte[] _buffer)
        {
            Buffer = _buffer;
            if (Count == 0)
            {
                byte[] buffer = new byte[88];
                Buffer.CopyTo(buffer, 0);
                WriteUInt16(80, 0, buffer);
                Buffer = buffer;
            }
        }
    }
}
