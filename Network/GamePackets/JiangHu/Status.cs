namespace Conquer_Online_Server.Network.GamePackets
{
    using Conquer_Online_Server;
    using Conquer_Online_Server.Game;
    using Conquer_Online_Server.Network;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public class JiangHuStatus : Conquer_Online_Server.Network.Writer
    {
        private byte[] packet;
        private ushort Position = 0x30;

        public JiangHuStatus(uint count = 1)
        {
            this.packet = new byte[0x38 + (count * 0x12)];
            Conquer_Online_Server.Network.Writer.WriteUInt16((ushort) (0x30 + (count * 0x12)), 0, this.packet);
            Conquer_Online_Server.Network.Writer.WriteUInt16(0xa8d, 2, this.packet);
            Conquer_Online_Server.Network.Writer.WriteUInt32(0x98967f, 0x27, this.packet);
        }

        public void Apprend(ICollection<Conquer_Online_Server.Game.JiangHu.JiangStages> val)
        {
            foreach (Conquer_Online_Server.Game.JiangHu.JiangStages stages in val)
            {
                if (!stages.Activate)
                {
                    break;
                }
                for (byte i = 0; i < stages.Stars.Length; i = (byte) (i + 1))
                {
                    if (this.packet.Length < (this.Position + 2))
                    {
                        break;
                    }
                    Conquer_Online_Server.Game.JiangHu.JiangStages.Star star = stages.Stars[i];
                    if (!star.Activate)
                    {
                        break;
                    }
                    Conquer_Online_Server.Network.Writer.WriteUInt16(star.UID, this.Position, this.packet);
                    this.Position = (ushort) (this.Position + 2);
                }
            }
            this.FreeTimeTodeyUsed = 0;
        }

        public byte[] ToArray()
        {
            return this.packet;
        }

        public uint FreeTimeTodey
        {
            get
            {
                return Conquer_Online_Server.BitConverter.ToUInt32(this.packet, 0x23);
            }
            set
            {
                Conquer_Online_Server.Network.Writer.WriteUInt32(value, 0x23, this.packet);
            }
        }

        public byte FreeTimeTodeyUsed
        {
            get
            {
                return this.packet[0x2b];
            }
            set
            {
                Conquer_Online_Server.Network.Writer.WriteByte(value, 0x2b, this.packet);
            }
        }

        public string Name
        {
            set
            {
                Conquer_Online_Server.Network.Writer.WriteString(value, 4, this.packet);
            }
        }

        public uint RoundBuyPoints
        {
            get
            {
                return Conquer_Online_Server.BitConverter.ToUInt32(this.packet, 0x2c);
            }
            set
            {
                Conquer_Online_Server.Network.Writer.WriteUInt32(value, 0x2c, this.packet);
            }
        }

        public byte Stage
        {
            get
            {
                return this.packet[20];
            }
            set
            {
                Conquer_Online_Server.Network.Writer.WriteByte(value, 20, this.packet);
            }
        }

        public ulong StudyPoints
        {
            get
            {
                return Conquer_Online_Server.BitConverter.ToUInt64(this.packet, 0x1b);
            }
            set
            {
                Conquer_Online_Server.Network.Writer.WriteUInt64(value, 0x1b, this.packet);
            }
        }

        public byte Talent
        {
            get
            {
                return this.packet[0x15];
            }
            set
            {
                Conquer_Online_Server.Network.Writer.WriteByte(value, 0x15, this.packet);
            }
        }

        public uint Timer
        {
            get
            {
                return Conquer_Online_Server.BitConverter.ToUInt32(this.packet, 0x16);
            }
            set
            {
                Conquer_Online_Server.Network.Writer.WriteUInt32(value, 0x16, this.packet);
            }
        }
    }
}

