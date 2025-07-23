using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class ChiRetreatRequest : Writer, Interfaces.IPacket
    {
        byte[] Buffer;
        public enum RetreatType
        {
            Info = 0,
            RequestRetreat = 1,
            Retreat = 2,
            RequestRestore = 3,
            Restore = 4,
            RequestExtend = 5,
            Extend = 6,
            RequestPayoff = 7,
            Payoff = 8,
            RequestAbondan = 9,
            Abondan = 10,
            RequestUpdate = 11,
            Update = 12,
            RequestExtend2 = 13,
            Extend2 = 14,
        }

        public ChiRetreatRequest(bool Create)
        {
            if (Create)
            {
                Buffer = new byte[7 + 8];
                Writer.WriteUInt16(7, 0, Buffer);
                Writer.WriteUInt16(2536, 2, Buffer);
            }
        }

        public RetreatType Type
        {
            get { return (RetreatType)BitConverter.ToUInt16(Buffer, 4); }
            set { WriteUInt16((ushort)value, 4, Buffer); }
        }

        public Game.Enums.ChiPowerType Mode
        {
            get { return (Game.Enums.ChiPowerType)Buffer[6]; }
            set { Buffer[6] = (byte)value; }
        }
        public void Send(Client.GameClient client)
        {
            client.Send(Buffer);
        }

        public byte[] ToArray()
        {
            return Buffer;
        }

        public void Deserialize(byte[] buffer)
        {
            this.Buffer = buffer;
        }
    }
}