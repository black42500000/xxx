using System;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class Attack : Interfaces.IPacket
    {
        public const uint Melee = 2,
                          MarriageRequest = 8,
                          MarriageAccept = 9,
                          Kill = 14,
                          Magic = 24,
                          Reflect = 26,
                          Dash = 27,
                          Ranged = 28,
                          MonkMelee = 34,
                          MerchantAccept = 40,
                          MerchantRefuse = 41,
                          MerchantProgress = 42,
                          Scapegoat = 43,
                          CounterKillSwitch = 44,
                          FatalStrike = 45,
                          ShowUseSpell = 52,
                          InteractionRequest = 46,
                          InteractionAccept = 47,
                          InteractionRefuse = 48,
                          InteractionEffect = 49,
                          InteractionStopEffect = 50,
                          SkillMove = 53;


        byte[] Buffer;

        public Attack(bool Create)
        {
            if (Create)
            {
                Buffer = new byte[8 + 36];
                Writer.WriteUInt16(36, 0, Buffer);
                Writer.WriteUInt16(1022, 2, Buffer);
                Writer.WriteUInt32((uint)Time32.timeGetTime().GetHashCode(), 4, Buffer);
            }
        }

        public uint Attacker
        {
            get { return BitConverter.ToUInt32(Buffer, 12); }
            set { Writer.WriteUInt32(value, 12, Buffer); }
        }

        public uint Attacked
        {
            get { return BitConverter.ToUInt32(Buffer, 16); }
            set { Writer.WriteUInt32(value, 16, Buffer); }
        }

        public ushort X
        {
            get { return BitConverter.ToUInt16(Buffer, 20); }
            set { Writer.WriteUInt16(value, 20, Buffer); }
        }

        public ushort Y
        {
            get { return BitConverter.ToUInt16(Buffer, 22); }
            set { Writer.WriteUInt16(value, 22, Buffer); }
        }

        public uint AttackType
        {
            get { return BitConverter.ToUInt32(Buffer, 24); }
            set { Writer.WriteUInt32(value, 24, Buffer); }
        }

        public uint Damage
        {
            get { return BitConverter.ToUInt32(Buffer, 28); }
            set { Writer.WriteUInt32(value, 28, Buffer); }
        }

        public ushort KOCount
        {
            get { return BitConverter.ToUInt16(Buffer, 30); }
            set { Writer.WriteUInt16(value, 30, Buffer); }
        }

        public uint ResponseDamage
        {
            get { return BitConverter.ToUInt32(Buffer, 32); }
            set { Writer.WriteUInt32(value, 32, Buffer); }
        }
        public AttackEffects1 Effect1
        {
            get { return (AttackEffects1)Buffer[36]; }
            set { Buffer[36] = (Byte)value; }
        }

        public AttackEffects2 Effect2
        {
            get { return (AttackEffects2)Buffer[37]; }
            set { Buffer[37] = (Byte)value; }
        }
        [Flags]
        public enum AttackEffects1 : byte
        {
            None = 0x0,
            Block = 0x1,
            Penetration = 0x2,
            CriticalStrike = 0x4,
            Immu = 0x08,
            MetalResist = 0x10,
            WoodResist = 0x20,
            WaterResist = 0x40,
            FireResist = 0x80,
        }

        [Flags]
        public enum AttackEffects2 : byte
        {
            None = 0x0,
            EarthResist = 0x1,
            StudyPoints = 0x2,
        }
        public bool Decoded = false;
        public ushort MagicType
        {
            get { return (ushort)Damage; }
            set { Damage = (uint)((MagicLevel << 16) | value); }
        }
        public ushort MagicLevel
        {
            get { return (ushort)(Damage >> 16); }
            set { Damage = (uint)((value << 16) | MagicType); }
        } 
        public void Deserialize(byte[] buffer)
        {
            this.Buffer = buffer;
        }
        public byte[] ToArray()
        {
            return Buffer;
        }
        public void Send(Client.GameClient client)
        {
            client.Send(Buffer);
        }

        public uint dwParam
        {
            get
            {
                return BitConverter.ToUInt32(Buffer, 28);
            }
            set
            {
                Writer.WriteUInt32(value, 28, Buffer);
            }
        }
    }
}
