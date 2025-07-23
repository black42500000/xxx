namespace Conquer_Online_Server.Network.GamePackets
{
    using Conquer_Online_Server;
    using Conquer_Online_Server.Network;
    using System;

    public class JiangHuUpdate : Conquer_Online_Server.Network.Writer
    {
        private byte[] packet = new byte[0x1b];

        public JiangHuUpdate()
        {
            Conquer_Online_Server.Network.Writer.WriteUInt16(0x13, 0, this.packet);
            Conquer_Online_Server.Network.Writer.WriteUInt16(0xa8e, 2, this.packet);
        }

        public byte[] ToArray()
        {
            return this.packet;
        }

        public ushort Atribute
        {
            get
            {
                return Conquer_Online_Server.BitConverter.ToUInt16(this.packet, 12);
            }
            set
            {
                Conquer_Online_Server.Network.Writer.WriteUInt16(value, 12, this.packet);
            }
        }

        public uint FreeCourse
        {
            get
            {
                return Conquer_Online_Server.BitConverter.ToUInt32(this.packet, 4);
            }
            set
            {
                Conquer_Online_Server.Network.Writer.WriteUInt32(value, 4, this.packet);
            }
        }

        public byte FreeTimeTodeyUsed
        {
            get
            {
                return this.packet[14];
            }
            set
            {
                Conquer_Online_Server.Network.Writer.WriteByte(value, 14, this.packet);
            }
        }

        public uint RoundBuyPoints
        {
            get
            {
                return Conquer_Online_Server.BitConverter.ToUInt32(this.packet, 15);
            }
            set
            {
                Conquer_Online_Server.Network.Writer.WriteUInt32(value, 15, this.packet);
            }
        }

        public byte Stage
        {
            get
            {
                return this.packet[11];
            }
            set
            {
                Conquer_Online_Server.Network.Writer.WriteByte(value, 11, this.packet);
            }
        }

        public byte Star
        {
            get
            {
                return this.packet[10];
            }
            set
            {
                Conquer_Online_Server.Network.Writer.WriteByte(value, 10, this.packet);
            }
        }
    }
}

