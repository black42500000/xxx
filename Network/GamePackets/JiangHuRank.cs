namespace Conquer_Online_Server.Network.GamePackets
{
    using Conquer_Online_Server;
    using Conquer_Online_Server.Network;
    using System;
    using System.Runtime.InteropServices;

    public class JiangHuRank : Conquer_Online_Server.Network.Writer
    {
        private byte[] packet;
        private ushort Position = 8;

        public JiangHuRank(byte entry = 1)
        {
            this.packet = new byte[0x10 + (entry * 0x29)];
            Conquer_Online_Server.Network.Writer.WriteUInt16((ushort)(this.packet.Length - 8), 0, this.packet);
            Conquer_Online_Server.Network.Writer.WriteUInt16(0xa8f, 2, this.packet);
            Conquer_Online_Server.Network.Writer.WriteByte(entry, 6, this.packet);
        }

        public void Appren(byte Rank, uint Inner_Strength, uint Level, string Name, string CustomizedName)
        {
            Conquer_Online_Server.Network.Writer.WriteByte(Rank, this.Position, this.packet);
            this.Position = (ushort)(this.Position + 1);
            Conquer_Online_Server.Network.Writer.WriteUInt32(Inner_Strength, this.Position, this.packet);
            this.Position = (ushort)(this.Position + 4);
            Conquer_Online_Server.Network.Writer.WriteUInt32(Level, this.Position, this.packet);
            this.Position = (ushort)(this.Position + 4);
            Conquer_Online_Server.Network.Writer.WriteString(Name, this.Position, this.packet);
            this.Position = (ushort)(this.Position + 0x10);
            Conquer_Online_Server.Network.Writer.WriteString(CustomizedName, this.Position, this.packet);
            this.Position = (ushort)(this.Position + 0x10);
        }

        public byte[] ToArray()
        {
            return this.packet;
        }

        public ushort Page
        {
            get
            {
                return Conquer_Online_Server.BitConverter.ToUInt16(this.packet, 4);
            }
            set
            {
                Conquer_Online_Server.Network.Writer.WriteUInt16(value, 4, this.packet);
            }
        }

        public byte RegisteredCount
        {
            get
            {
                return this.packet[7];
            }
            set
            {
                Conquer_Online_Server.Network.Writer.WriteByte(value, 7, this.packet);
            }
        }
    }
}

