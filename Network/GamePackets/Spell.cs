using System;
using System.Collections.Generic;
using System.Linq;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class Spell : Writer, Interfaces.IPacket, Interfaces.ISkill
    {
        byte[] Buffer;
        private byte _PreviousLevel;
        private bool _Available;
        public Spell(bool Create)
        {
            Buffer = new byte[32 + 8];
            WriteUInt16(32, 0, Buffer);
            WriteUInt16(1103, 2, Buffer);
            WriteUInt32((uint)Time32.timeGetTime().GetHashCode(), 4, Buffer);
            Buffer[24] = 1;
            _Available = false;
        }
        public ushort ID
        {
            get { return (ushort)BitConverter.ToUInt16(Buffer, 12); }
            set { WriteUInt16(value, 12, Buffer); }
        }

        public byte Level
        {
            get { return (byte)BitConverter.ToUInt32(Buffer, 16); }
            set
            {
                if (Database.SpellTable.SpellInformations != null)
                    if (Database.SpellTable.SpellInformations.ContainsKey(ID))
                        if (!Database.SpellTable.SpellInformations[ID].ContainsKey(value))
                            value = Database.SpellTable.SpellInformations[ID].Keys.LastOrDefault();
                WriteUInt16(value, 16, Buffer);
            }
        }

        byte _Levelhu = 0;
        public byte LevelHu
        {
            get { return _Levelhu; }
            set
            {
                _Levelhu = value;
                /* if (value >= 1)
                     Buffer[25] = 1;
                 if (value >= 2)
                     Buffer[26] = 1;
                 if (value >= 3)
                     Buffer[27] = 1;
                 if (value >= 4)
                     Buffer[28] = 1;*/
                if (value == 1)
                    Buffer[24] = 3;
                if (value == 2)
                    Buffer[24] = 7;
                if (value == 3)
                    Buffer[24] = 15;
                if (value == 4)
                    Buffer[24] = 31;
                Buffer[28] = Buffer[24];
            }
        }

        public byte LevelHu2
        {
            get;
            set;
        }

        public byte PreviousLevel
        {
            get { return _PreviousLevel; }
            set { _PreviousLevel = value; }
        }

        public uint Experience
        {
            get { return BitConverter.ToUInt32(Buffer, 8); }
            set { WriteUInt32(value, 8, Buffer); }
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
            client.Send(this);
        }

        public bool Available
        {
            get
            {
                return _Available;
            }
            set
            {
                _Available = value;
            }
        }

        public static uint power { get; set; }
    }
}