using System;
using System.Collections.Generic;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class Update : Writer, Interfaces.IPacket
    {
        public struct UpdateStruct
        {
            public uint Type;
            public ulong Value1;
            public ulong Value2;
        }

        public class Flags
        {

            public const ulong
                Cursed = 1UL << 32,
                Normal = 0x0,
                Senior = 1UL << 183,
             VIP6Effect = 1UL << 52,
             GMEffect = 1UL << 53,
                FlashingName = 0x1,
                Poisoned = 0x2,
                Invisible = 0x4,
                XPList = 0x10,
                Dead = 0x20,
                TeamLeader = 0x40,
                StarOfAccuracy = 0x80,
                MagicShield = 0x100,
                Stigma = 0x200,
                Ghost = 0x400,
                FadeAway = 0x800,
                RedName = 0x4000,
                BlackName = 0x8000,
                ReflectMelee = 0x20000,
                Superman = 0x40000,
                Ball = 0x80000,
                Ball2 = 0x100000,
                Invisibility = 0x400000,
                Cyclone = 0x800000,
                Dodge = 0x4000000,
                Fly = 0x8000000,
                Intensify = 0x10000000,
                CastPray = 0x40000000,
                Praying = 0x80000000,
                HeavenBlessing = 0x200000000,
                TopGuildLeader = 0x400000000,
                TopDeputyLeader = 0x800000000,
                MonthlyPKChampion = 0x1000000000,
                WeeklyPKChampion = 0x2000000000,
                TopWarrior = 0x4000000000,
                TopTrojan = 0x8000000000,
                TopArcher = 0x10000000000,
                TopWaterTaoist = 0x20000000000,
                TopFireTaoist = 0x40000000000,
                TopNinja = 0x80000000000,
                ShurikenVortex = 0x400000000000,
                FatalStrike = 0x800000000000,
                Flashy = 0x1000000000000,
                Ride = 0x4000000000000,
               TopSpouse = 1UL << 51,
            OrangeSparkles = 1UL << 52,
            PurpleSparkles = 1UL << 53,
            Frightened = 1UL << 54,
            SpeedIncreased = 1UL << 55,
            MoveSpeedRecovered = 1UL << 56,
             TopMonk = 0x4000000000000L,
            DivineShield = 1UL << 57,
            Dizzy = 1UL << 58,
            Freeze = 1UL << 59,
            Confused = 1UL << 60;
        }
        public class Flags2
        {

            public const ulong
               WeeklyTop8Pk = 0x01,
            WeeklyTop2PkGold = 0x2,
            WeeklyTop2PkBlue = 0x4,
            MonthlyTop8Pk = 0x8,
            MontlyTop2Pk = 0x10,
            MontlyTop3Pk = 0x20,
            Top8Fire = 0x40,
            TopPirate2 = 1UL << 123,
            Top2Fire = 0x80,
            Top3Fire = 0x100,
            Top8Water = 0x200,
            Top2Water = 0x400,
            Top3Water = 0x800,
            Top8Ninja = 0x1000,
            Top2Ninja = 0x2000,
            Top3Ninja = 0x4000,
            Top8Warrior = 0x8000,
            Top2Warrior = 0x10000,
            Top3Warrior = 0x20000,
            Top8Trojan = 0x40000,
            Top2Trojan = 0x80000,
            Top3Trojan = 0x100000,
            Top8Archer = 0x200000,
            Top2Archer = 0x400000,
            Top3Archer = 0x800000,
            Top3SpouseBlue = 0x1000000,
            Top2SpouseBlue = 0x2000000,
            Top3SpouseYellow = 0x4000000,
            Contestant = 0x8000000,
            ChainBoltActive = 0x10000000,
            AzureShield = 0x20000000,
            AzureShieldFade = 0x40000000,
            CarryingFlag = 0x80000000,//blank next one?
            TyrantAura = 0x400000000,
            FendAura = 0x1000000000,
            MetalAura = 0x4000000000,
            WoodAura = 0x10000000000,
            WaterAura = 0x40000000000,
            FireAura = 17592186044416,
            EarthAura = 0x400000000000,
            SoulShackle = 140737488355328,
            Oblivion = 0x1000000000000,
            Top8Monk = 0x8000000000000,
            Top2Monk = 0x10000000000000,
            LionShield = 0x200000000000000,
            OrangeHaloGlow = 1125899906842624,
            LowVigorUnableToJump = 1125899906842624,
            TopSpouse = 2251799813685248,
            GoldSparkle = 4503599627370496,
            VioletSparkle = 9007199254740992,
            Dazed = 18014398509481984,//no movement
            BlueRestoreAura = 36028797018963968,
            MoveSpeedRecovered = 72057594037927936,
            SuperShieldHalo = 144115188075855872,
            HUGEDazed = 288230376151711744,//no movement
            IceBlock = 576460752303423488, //no movement
            Confused = 1152921504606846976,//reverses movement
            Top3Monk = 0x20000000000000,
            CannonBarrage = 1UL << 120,
            BlackbeardsRage = 1UL << 121,
            Fatigue = 1UL << 126,
            TopMonk = 0x4000000000000L,
            TopPirate = 1UL << 58;
        }
        public class Flags3
        {
            public const uint
             DragonFury = (uint)1UL << 30,
            DragonCyclone = (uint)1UL << 159,
            DragonSwing = (uint)1UL << 160,
            DragonFlow = 148,
            DragonRoar = (uint)1UL << 29,
                SuperCyclone = 0x400000,
              MagicDefender = 0x01,
              Assassin = 0x20000,
              AutoHunting = (uint)1UL << 20,
              BladeFlurry = 0x40000,
              KineticSpark = 0x80000;



            public static uint DragonFLow { get; set; }
        }
        public const byte
                Hitpoints = 0,
                MaxHitpoints = 1,
                Mana = 2,
                MaxMana = 3,
                Money = 4,
                Experience = 5,
                PKPoints = 6,
                Class = 7,
                Stamina = 8,
                WHMoney = 9,
                Atributes = 10,
                Mesh = 11,
                Level = 12,
                Spirit = 13,
                Vitality = 14,
                Strength = 15,
                Agility = 16,
                HeavensBlessing = 17,
                DoubleExpTimer = 18,
                CursedTimer = 20,
                Reborn = 22,
                StatusFlag = 25,
                HairStyle = 26,
                XPCircle = 27,
                LuckyTimeTimer = 28,
                ConquerPoints = 29,
                OnlineTraining = 31,
                MentorBattlePower = 36,
                Merchant = 38,
                VIPLevel = 39,
                QuizPoints = 40,
                EnlightPoints = 41,
                HonorPoints = 42,
                GuildBattlepower = 44,
                BoundConquerPoints = 45,
                RaceShopPoints = 47,
                MagicDefenderIcone = 49,
                DefensiveStance = 56;

        byte[] Buffer;
        const byte minBufferSize = 40;
        public Update(bool Create)
        {
            if (Create)
            {
                Buffer = new byte[minBufferSize + 8];
                WriteUInt16(minBufferSize, 0, Buffer);
                WriteUInt16(10017, 2, Buffer);
                WriteUInt32((uint)Time32.timeGetTime().GetHashCode(), 4, Buffer);
            }
        }

        public uint UID
        {
            get { return BitConverter.ToUInt32(Buffer, 8); }
            set { WriteUInt32(value, 8, Buffer); }
        }
        public uint self
        {
            get { return BitConverter.ToUInt32(Buffer, 12); }
            set { WriteUInt32(value, 12, Buffer); }
        }

        public uint UpdateCount
        {
            get { return BitConverter.ToUInt32(Buffer, 12); }
            set
            {
                byte[] buffer = new byte[minBufferSize + 8 + 24 * value];
                Buffer.CopyTo(buffer, 0);
                WriteUInt16((ushort)(minBufferSize + 24 * value), 0, buffer);
                Buffer = buffer;
                WriteUInt32(value, 12, Buffer);
            }
        }
        public void Append(byte type, byte value)
        {
            this.UpdateCount++;
            ushort offset = (ushort)(16 + ((this.UpdateCount - 1) * 28));
            Writer.WriteUInt32(type, offset, this.Buffer);
            Writer.WriteUInt64((ulong)value, offset + 4, this.Buffer);
            Writer.WriteUInt64(value, offset + 8, Buffer);
        }
        public void Append(byte type, ushort value)
        {
            this.UpdateCount++;
            ushort offset = (ushort)(12 + 4 + ((this.UpdateCount - 1) * 20));
            Writer.WriteUInt32(type, offset, this.Buffer);
            Writer.WriteUInt64((ulong)value, offset + 4, this.Buffer);
            Writer.WriteUInt64(value, offset + 4, Buffer);
        }

        public void AppendFull(byte type, ulong val1, ulong val2, uint val3)
        {
            UpdateCount = UpdateCount + 1;
            ushort offset = (ushort)(16 + ((UpdateCount - 1) * 20));
            WriteUInt32(type, offset, Buffer);
            WriteUInt64(val1, offset + 4, Buffer);
            WriteUInt64(val2, offset + 12, Buffer);
            WriteUInt32(val3, offset + 20, Buffer);
        }

        public void PoPAppend(byte type, ulong val1, ulong val2)
        {
            UpdateCount = UpdateCount + 1;
            ushort offset = (ushort)(16 + ((UpdateCount - 1) * 24));
            WriteUInt32(type, offset, Buffer);
            WriteUInt64(val1, offset + 8, Buffer);
            WriteUInt64(val2, offset + 16, Buffer);
        }
        public void Append(byte type, uint[] value)
        {
            UpdateCount = UpdateCount + 1;
            ushort offset = (ushort)(12 + 4 + (UpdateCount - 1) * 24);
            WriteUInt32(type, offset, Buffer);
            for (byte x = 0; x < value.Length; x++)
                WriteUInt32(value[x], (ushort)((offset + 4) + x * 4), Buffer);
        }
        public void Append(byte type, uint value)
        {
            UpdateCount = UpdateCount + 1;
            ushort offset = (ushort)(16 + ((UpdateCount - 1) * 28));
            WriteUInt32(type, offset, Buffer);
            WriteUInt64(value, offset + 4, Buffer);
        }

        public void Append(byte type, ulong value)
        {
            this.UpdateCount++;
            ushort offset = (ushort)(12 + 4 + ((this.UpdateCount - 1) * 24));
            Writer.WriteUInt32(type, offset, this.Buffer);
            Writer.WriteUInt64(value, offset + 4, this.Buffer);
        }

        public void Append2(byte type, byte value)
        {
            this.UpdateCount++;
            ushort offset = (ushort)(12 + 4 + ((this.UpdateCount - 1) * 24));
            Writer.WriteUInt32(type, offset, this.Buffer);
            Writer.WriteUInt64((ulong)value, offset + 12, this.Buffer);
        }
        public void Append2(byte type, ushort value)
        {
            this.UpdateCount++;
            ushort offset = (ushort)(12 + 4 + ((this.UpdateCount - 1) * 24));
            Writer.WriteUInt32(type, offset, this.Buffer);
            Writer.WriteUInt64((ulong)value, offset + 12, this.Buffer);
        }

        public void Append2(byte type, uint value)
        {
            this.UpdateCount++;
            ushort offset = (ushort)(12 + 4 + ((this.UpdateCount - 1) * 24));
            Writer.WriteUInt32(type, offset, this.Buffer);
            Writer.WriteUInt64((ulong)value, offset + 12, this.Buffer);
        }

        public void Append2(byte type, ulong value)
        {
            this.UpdateCount++;
            ushort offset = (ushort)(12 + 4 + ((this.UpdateCount - 1) * 24));
            Writer.WriteUInt32(type, offset, this.Buffer);
            Writer.WriteUInt64(value, offset + 12, this.Buffer);
        }

        public void Clear()
        {
            byte[] buffer = new byte[minBufferSize + 8];
            WriteUInt16(minBufferSize, 0, Buffer);
            WriteUInt16(10017, 2, Buffer);
            WriteUInt32(UID, 8, buffer);
            WriteUInt32((uint)Time32.timeGetTime().GetHashCode(), 4, Buffer);
            Buffer = buffer;
        }

        public List<UpdateStruct> Updates
        {
            get
            {
                List<UpdateStruct> structs = new List<UpdateStruct>();
                ushort offset = 16;
                if (UpdateCount > 0)
                {
                    for (int c = 0; c < UpdateCount; c++)
                    {
                        UpdateStruct st = new UpdateStruct();
                        st.Type = BitConverter.ToUInt32(Buffer, offset); offset += 4;
                        st.Value1 = BitConverter.ToUInt64(Buffer, offset); offset += 8;
                        st.Value2 = BitConverter.ToUInt64(Buffer, offset); offset += 8;
                        structs.Add(st);
                    }
                }
                return structs;
            }
        }

        public byte[] ToArray()
        {
            return Buffer;
        }

        public void Deserialize(byte[] buffer)
        {
            Buffer = buffer;
        }

        public void Send(Client.GameClient client)
        {
            client.Send(Buffer);
        }
        public void Append(byte type, byte value, byte second, byte second2, byte second3, byte second4, byte second5, byte second6, byte second7)
        {
            UpdateCount = UpdateCount + 1;
            ushort offset = (ushort)(UpdateCount * 16);//12
            WriteUInt32(type, offset, Buffer);
            WriteUInt64(value, offset + 4, Buffer);
            WriteByte(second, offset + 12, Buffer);
            WriteByte(second2, offset + 13, Buffer);
            WriteByte(second3, offset + 14, Buffer);
            WriteByte(second4, offset + 15, Buffer);
            WriteByte(second5, offset + 16, Buffer);
            WriteByte(second6, offset + 17, Buffer);
            WriteByte(second7, offset + 18, Buffer);
        }
        public void Append(ulong val1, ulong val2, uint val3, uint val4, uint val5, uint val6, uint val7)
        {
            UpdateCount = 2;
            WriteUInt32(0x19, 16, Buffer);
            WriteUInt64(val1, 20, Buffer);
            WriteUInt64(val2, 28, Buffer);
            WriteUInt32(val3, 36, Buffer);
            WriteUInt32(val4, 40, Buffer);
            WriteUInt32(val5, 44, Buffer);
            WriteUInt32(val6, 48, Buffer);
            WriteUInt32(val7, 52, Buffer);
        }
    }
}
