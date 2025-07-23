namespace Conquer_Online_Server.Network.GamePackets
{
    using Conquer_Online_Server.Client;
    using Conquer_Online_Server.Network;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class JiangHu : Conquer_Online_Server.Network.Writer
    {
        public const byte IconBar = 0;
        public const byte InfoStauts = 7;
        public byte Mode = 0;
        public const byte OpenStage = 12;
        private byte[] packet;
        public const byte SetName = 14;
        public List<string> Texts = new List<string>();
        public const byte UpdateStar = 11;
        public const byte UpdateTalent = 5;
        public const byte UpdateTime = 13;

        public void Clear()
        {
            this.Texts.Clear();
        }

        public void CreateArray()
        {
            this.packet = new byte[(this.Leng(this.Texts.ToArray()) + 7) + 8];
            Conquer_Online_Server.Network.Writer.WriteUInt16((ushort) (this.packet.Length - 8), 0, this.packet);
            Conquer_Online_Server.Network.Writer.WriteUInt16(0xa8c, 2, this.packet);
            Conquer_Online_Server.Network.Writer.WriteByte(this.Mode, 4, this.packet);
            Conquer_Online_Server.Network.Writer.WriteByte((byte) this.Texts.Count, 5, this.packet);
            ushort offset = 6;
            for (ushort i = 0; i < this.Texts.Count; i = (ushort) (i + 1))
            {
                string arg = this.Texts[i];
                Conquer_Online_Server.Network.Writer.WriteByte((byte) arg.Length, offset, this.packet);
                Conquer_Online_Server.Network.Writer.WriteString(arg, (ushort) (offset + 1), this.packet);
                offset = (ushort) (offset + ((ushort) (arg.Length + 1)));
            }
        }

        private uint Leng(string[] dat)
        {
            uint num = 0;
            foreach (string str in dat)
            {
                num += (byte) str.Length;
            }
            return (num + ((uint) dat.Length));
        }

        public void Send(Conquer_Online_Server.Client.GameClient client)
        {
            if (this.packet != null)
            {
                client.Send(this.packet.ToArray<byte>());
            }
        }
    }
}

