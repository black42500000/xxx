﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class GameUpdates : Writer, Interfaces.IPacket
    {
        private byte[] Buffer;
        
        public const byte
            Header = 0,
            Body = 1,
            Footer = 2;

        public GameUpdates(byte _Type, string _String)
        {
            this.Buffer = new byte[29 + _String.Length];
            WriteUInt16((ushort)(Buffer.Length - 8), 0, Buffer);
            WriteUInt16(2032, 2, Buffer);
            WriteUInt32((uint)Time32.timeGetTime().GetHashCode(), 4, Buffer);
            Buffer[15] = 112;
            Buffer[16] = 1;
            Type = _Type;
            String = _String;
        }

        public byte Type
        {
            get { return Buffer[14]; }
            set { Buffer[14] = value; }
        }

        public string String
        {
            get { return System.BitConverter.ToString(Buffer, 18, Buffer[17]); }
            set { WriteStringWithLength(value, 17, Buffer); }
        }
        public void Send(Client.GameClient client)
        {
            client.Send(Buffer);
        }
        public void Deserialize(byte[] buffer)
        {
            this.Buffer = buffer;
        }

        public byte[] ToArray()
        {
            return Buffer;
        }
    }
}
