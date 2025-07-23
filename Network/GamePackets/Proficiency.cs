﻿using System;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class Proficiency : Writer, Interfaces.IPacket, Interfaces.IProf
    {
        byte[] Buffer;
        private byte _PreviousLevel;
        private bool _Available;
        public Proficiency(bool Create)
        {
            Buffer = new byte[28];
            WriteUInt16(20, 0, Buffer);
            WriteUInt16(1025, 2, Buffer);
            _Available = false;
        }
        public ushort ID
        {
            get { return (ushort)BitConverter.ToUInt16(Buffer, 4); }
            set { WriteUInt16(value, 4, Buffer); }
        }

        public byte Level
        {
            get { return (byte)BitConverter.ToUInt16(Buffer, 8); }
            set { WriteUInt16(value, 8, Buffer); }
        }

        public byte PreviousLevel
        {
            get { return _PreviousLevel; }
            set { _PreviousLevel = value; }
        }

        public uint Experience
        {
            get { return BitConverter.ToUInt32(Buffer, 12); }
            set { WriteUInt32(value, 12, Buffer); }
        }

        public uint NeededExperience
        {
            get { return BitConverter.ToUInt32(Buffer, 16); }
            set { WriteUInt32(value, 16, Buffer); }
        }
        public byte TempLevel
        {
            get { return (byte)BitConverter.ToUInt16(Buffer, 16); }
            set { WriteUInt32(value, 16, Buffer); }
        }
        public byte[] ToArray()
        {
            return Buffer;
        }

        public void Deserialize(byte[] buffer)
        {
            throw new NotImplementedException();
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
        public void Send(Client.GameClient client)
        {
            client.Send(Buffer);
        }
    }
}
