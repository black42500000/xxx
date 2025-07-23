using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Network.GamePackets
{
    public unsafe class AutoShow
    {
        byte[] Buffer;
        public byte[] ToArray()
        {
            return Buffer;
        }
        public AutoShow()
        {
            Buffer = new byte[46];
            fixed (byte* ptr = Buffer)
            {
                *((ushort*)(ptr)) = 38;
                *((ushort*)(ptr + 2)) = 1070;
                *((ushort*)(ptr + 4)) = 0;
                *((ushort*)(ptr + 6)) = 341;
            }
        }
    }
    public unsafe class AutoStart
    {
        byte[] Buffer;
        public byte[] ToArray()
        {
            return Buffer;
        }
        public AutoStart()
        {
            Buffer = new byte[46];
            fixed (byte* ptr = Buffer)
            {
                *((ushort*)(ptr)) = 38;
                *((ushort*)(ptr + 2)) = 1070;
                *((ushort*)(ptr + 4)) = 1;
            }
        }
    }
    public unsafe class AutoEnding
    {
        byte[] Buffer;
        public byte[] ToArray()
        {
            return Buffer;
        }
        public AutoEnding()
        {
            Buffer = new byte[46];
            fixed (byte* ptr = Buffer)
            {
                *((ushort*)(ptr)) = 38;
                *((ushort*)(ptr + 2)) = 1070;
                *((ushort*)(ptr + 4)) = 2;
            }
        }
    }
    public unsafe class AutoEnd
    {
        byte[] Buffer;
        public byte[] ToArray()
        {
            return Buffer;
        }
        public AutoEnd()
        {
            Buffer = new byte[46];
            fixed (byte* ptr = Buffer)
            {
                *((ushort*)(ptr)) = 38;
                *((ushort*)(ptr + 2)) = 1070;
                *((ushort*)(ptr + 4)) = 3;
            }
        }
    }
    public class AutoHunt : Writer, Interfaces.IPacket
    {
        public const ushort
        Icon = 0,
        Start = 1,
        Gui = 2,
        End = 3;
        private byte[] Buffer;
        public AutoHunt(bool Create)
        {
            if (Create)
            {
                Buffer = new byte[38 + 8];
                WriteUInt16(38, 0, Buffer);
                WriteUInt16(1070, 2, Buffer);
            }
        }

        public ushort Type
        {
            get { return BitConverter.ToUInt16(Buffer, 4); }
            set { WriteUInt32(value, 4, Buffer); }
        }
        public ushort Show
        {
            get { return BitConverter.ToUInt16(Buffer, 6); }
            set { WriteUInt32(value, 6, Buffer); }
        }
        public ushort EXP
        {
            get { return BitConverter.ToUInt16(Buffer, 14); }
            set { WriteUInt32(value, 14, Buffer); }
        }

        public void Deserialize(byte[] Data)
        {
            Buffer = Data;
        }
        public byte[] ToArray()
        {
            return Buffer;
        }
        public void Send(Client.GameClient client)
        {
            client.Send(Buffer);
        }
    }
}
