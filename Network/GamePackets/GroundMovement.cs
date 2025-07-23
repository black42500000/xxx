using System;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class GroundMovement:Writer, Interfaces.IPacket
    {
        public const uint Walk = 0,
                          Run = 1,
                          Slide = 9;

        private byte[] Buffer;

        public GroundMovement(bool CreateInstance)
        {
            if (CreateInstance)
            {
                Buffer = new byte[32];
                WriteUInt32(24, 0, Buffer);
                WriteUInt32(10005, 2, Buffer);
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

        public Conquer_Online_Server.Game.Enums.ConquerAngle Direction
        {
            get { return (Conquer_Online_Server.Game.Enums.ConquerAngle)(Buffer[4] % 24); }
            set { Buffer[4] = (byte)value; }
        }

        public uint UID
        {
            get { return BitConverter.ToUInt32(Buffer, 8); }
            set { WriteUInt32(value, 8, Buffer); }
        }
        static readonly sbyte[] DeltaX = new sbyte[] { 0, -1, -1, -1, 0, 1, 1, 1, 0 };
        static readonly sbyte[] DeltaY = new sbyte[] { 1, 1, 0, -1, -1, -1, 0, 1, 0 };
        public static MapPoint CreateDirectionPoint(ushort X, ushort Y, byte dir)
        {
            return new MapPoint(0, (ushort)(X + DeltaX[dir]), (ushort)(Y + DeltaY[dir]));
        }
        public uint GroundMovementType
        {
            get { return BitConverter.ToUInt32(Buffer, 12); }
            set { WriteUInt32(value, 12, Buffer); }
        }

        public uint TimeStamp
        {
            get { return BitConverter.ToUInt32(Buffer, 16); }
            set { WriteUInt32(value, 16, Buffer); }
        }

        public uint MapID
        {
            get { return BitConverter.ToUInt32(Buffer, 20); }
            set { WriteUInt32(value, 20, Buffer); }
        }

        public void Send(Client.GameClient client)
        {
            client.Send(Buffer);
        }
    }
}
