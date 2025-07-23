using System;
using System.Collections.Generic;
using System.Text;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class _String : Writer, Interfaces.IPacket
    {
        public const byte GuildName = 3,
        Spouse = 6,
        Effect = 10,
        GuildList = 11,
        Unknown = 13,
        ViewEquipSpouse = 16,
        StartGamble = 17,
        EndGamble = 18,
        Sound = 20,
        GuildAllies = 21,
        GuildEnemies = 22,
        WhisperDetails = 26,
         MapEffect = 9;

        byte[] Buffer;

        public _String(bool Create)
        {
            if (Create)
            {
                Buffer = new byte[23];
                WriteUInt16(15, 0, Buffer);
                WriteUInt16(1015, 2, Buffer);
                WriteUInt32((uint)Time32.timeGetTime().GetHashCode(), 4, Buffer);
                Texts = new List<string>();
            }
        }
        public uint UID
        {
            get { return BitConverter.ToUInt32(Buffer, 8); }
            set { WriteUInt32(value, 8, Buffer); }
        }
        public ushort PositionX
        {
            get { return (ushort)UID; }
            set { UID = (uint)((PositionY << 16) | value); }
        }

        public ushort PositionY
        {
            get { return (ushort)(UID >> 16); }
            set { UID = (uint)((value << 16) | PositionX); }
        }
        public byte Type
        {
            get { return Buffer[12]; }
            set { Buffer[12] = value; }
        }
        public byte TextsCount
        {
            get { return Buffer[13]; }
            set { Buffer[13] = value; }
        }
        public List<string> Texts;

        public void Send(Client.GameClient client)
        {
            client.Send(Buffer);
        }
       
        public byte[] ToArray()
        {
            ushort entirelength = 23;
            foreach (string list in Texts)
                entirelength += (ushort)list.Length;
            byte[] buffer = new byte[entirelength];
            WriteUInt16((ushort)(entirelength - 8), 0, buffer);
            WriteUInt16(1015, 2, buffer);
            WriteUInt32(UID, 8, buffer);
            WriteUInt32((uint)Time32.timeGetTime().GetHashCode(), 4, buffer);
            buffer[12] = Type;
            Buffer = buffer;
            WriteStringList(Texts, 13, Buffer);
            return Buffer;
        }
        public void Deserialize(byte[] buffer)
        {
            Buffer = buffer;
            Texts = new List<string>(buffer[13]);
            ushort offset = 14;
            byte count = 0;
            while (count != TextsCount)
            {
                ushort textlength = buffer[offset]; offset++;
                string text = Encoding.Default.GetString(buffer, offset, textlength); offset += textlength;
                Texts.Add(text);
                count++;
            }            
        }
    }
}
