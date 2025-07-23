using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class poker : Writer, Interfaces.IPacket
    {
        public const uint NotSignedUp = 0,
                          WaitingForOpponent = 1,
                          WaitingInactive = 2;
        byte[] Buffer;
        public poker(bool Create)
        {
            Buffer = new byte[28];
            WriteUInt16(20, 0, Buffer);
            WriteUInt16(2090, 2, Buffer);
        }

        public ushort show
        {
            get { return BitConverter.ToUInt16(Buffer, 4); }
            set { WriteUInt16(value, 4, Buffer); }
        }

        public ushort show1
        {
            get { return BitConverter.ToUInt16(Buffer, 6); }
            set { WriteUInt16(value, 6, Buffer); }
        }

        public uint show2
        {
            get { return BitConverter.ToUInt32(Buffer, 8); }
            set { WriteUInt32(value, 8, Buffer); }
        }

        public uint show3
        {
            get { return BitConverter.ToUInt32(Buffer, 12); }
            set { WriteUInt32(value, 12, Buffer); }
        }

        public uint show4
        {
            get { return BitConverter.ToUInt32(Buffer, 16); }
            set { WriteUInt32(value, 16, Buffer); }
        }
        public void Send(Client.GameClient client)
        {
            client.Send(ToArray());
        }

        public byte[] ToArray()
        {
            return Buffer;
        }

        public void Deserialize(byte[] buffer)
        {
            Buffer = buffer;
        }
    }
    public class poker2 : Writer, Interfaces.IPacket
    {
        public const uint NotSignedUp = 0,
                          WaitingForOpponent = 1,
                          WaitingInactive = 2;
        byte[] Buffer;
        public poker2(bool Create)
        {
            Buffer = new byte[24];
            WriteUInt16(16, 0, Buffer);
            WriteUInt16(2096, 2, Buffer);
        }

        public uint show
        {
            get { return BitConverter.ToUInt32(Buffer, 4); }
            set { WriteUInt32(value, 4, Buffer); }
        }

        public uint show1
        {
            get { return BitConverter.ToUInt32(Buffer, 8); }
            set { WriteUInt32(value, 8, Buffer); }
        }

        public uint show2
        {
            get { return BitConverter.ToUInt32(Buffer, 12); }
            set { WriteUInt32(value, 12, Buffer); }
        }
        public void Send(Client.GameClient client)
        {
            client.Send(ToArray());
        }

        public byte[] ToArray()
        {
            return Buffer;
        }

        public void Deserialize(byte[] buffer)
        {
            Buffer = buffer;
        }
    }
    public class poker2171 : Writer, Interfaces.IPacket
    {
        public const uint NotSignedUp = 0,
                          WaitingForOpponent = 1,
                          WaitingInactive = 2;
        byte[] Buffer;
        public poker2171(bool Create)
        {
            Buffer = new byte[28];
            WriteUInt16(20, 0, Buffer);
            WriteUInt16(2171, 2, Buffer);
        }

        public uint show
        {
            get { return BitConverter.ToUInt32(Buffer, 4); }
            set { WriteUInt32(value, 4, Buffer); }
        }

        public uint show1
        {
            get { return BitConverter.ToUInt32(Buffer, 8); }
            set { WriteUInt32(value, 8, Buffer); }
        }

        public uint show2
        {
            get { return BitConverter.ToUInt32(Buffer, 12); }
            set { WriteUInt32(value, 12, Buffer); }
        }
        public uint show3
        {
            get { return BitConverter.ToUInt32(Buffer, 16); }
            set { WriteUInt32(value, 16, Buffer); }
        }
        public void Send(Client.GameClient client)
        {
            client.Send(ToArray());
        }

        public byte[] ToArray()
        {
            return Buffer;
        }

        public void Deserialize(byte[] buffer)
        {
            Buffer = buffer;
        }
    }
    public class poker2090 : Writer, Interfaces.IPacket
    {
        public const uint NotSignedUp = 0,
                          WaitingForOpponent = 1,
                          WaitingInactive = 2;
        byte[] Buffer;
        public poker2090(bool Create)
        {
            Buffer = new byte[28];
            WriteUInt16(20, 0, Buffer);
            WriteUInt16(2090, 2, Buffer);
        }

        public ushort show
        {
            get { return BitConverter.ToUInt16(Buffer, 4); }
            set { WriteUInt16(value, 4, Buffer); }
        }

        public ushort show1
        {
            get { return BitConverter.ToUInt16(Buffer, 6); }
            set { WriteUInt16(value, 6, Buffer); }
        }

        public uint show2
        {
            get { return BitConverter.ToUInt32(Buffer, 8); }
            set { WriteUInt32(value, 8, Buffer); }
        }

        public uint show3
        {
            get { return BitConverter.ToUInt32(Buffer, 12); }
            set { WriteUInt32(value, 12, Buffer); }
        }

        public uint show4
        {
            get { return BitConverter.ToUInt32(Buffer, 16); }
            set { WriteUInt32(value, 16, Buffer); }
        }
        public void Send(Client.GameClient client)
        {
            client.Send(ToArray());
        }

        public byte[] ToArray()
        {
            return Buffer;
        }

        public void Deserialize(byte[] buffer)
        {
            Buffer = buffer;
        }
    }
    public class poker2091 : Writer, Interfaces.IPacket
    {
        public const uint NotSignedUp = 0,
                          WaitingForOpponent = 1,
                          WaitingInactive = 2;
        byte[] Buffer;
        public poker2091(bool Create)
        {
            Buffer = new byte[52];
            WriteUInt16(44, 0, Buffer);
            WriteUInt16(2091, 2, Buffer);
        }

        public ushort show
        {
            get { return BitConverter.ToUInt16(Buffer, 4); }
            set { WriteUInt16(value, 4, Buffer); }
        }

        public ushort show1
        {
            get { return BitConverter.ToUInt16(Buffer, 6); }
            set { WriteUInt16(value, 6, Buffer); }
        }

        public uint show2
        {
            get { return BitConverter.ToUInt32(Buffer, 8); }
            set { WriteUInt32(value, 8, Buffer); }
        }

        public uint show3
        {
            get { return BitConverter.ToUInt32(Buffer, 12); }
            set { WriteUInt32(value, 12, Buffer); }
        }

        public uint show4
        {
            get { return BitConverter.ToUInt32(Buffer, 16); }
            set { WriteUInt32(value, 16, Buffer); }
        }
        public uint show5
        {
            get { return BitConverter.ToUInt32(Buffer, 20); }
            set { WriteUInt32(value, 20, Buffer); }
        }
        public uint show6
        {
            get { return BitConverter.ToUInt32(Buffer, 24); }
            set { WriteUInt32(value, 24, Buffer); }
        }
        public uint show7
        {
            get { return BitConverter.ToUInt32(Buffer, 26); }
            set { WriteUInt32(value, 26, Buffer); }
        }
        public uint show8
        {
            get { return BitConverter.ToUInt32(Buffer, 30); }
            set { WriteUInt32(value, 30, Buffer); }
        }
        public uint show9
        {
            get { return BitConverter.ToUInt32(Buffer, 34); }
            set { WriteUInt32(value, 34, Buffer); }
        }
        public uint show10
        {
            get { return BitConverter.ToUInt32(Buffer, 38); }
            set { WriteUInt32(value, 38, Buffer); }
        }
        public void Send(Client.GameClient client)
        {
            client.Send(ToArray());
        }

        public byte[] ToArray()
        {
            return Buffer;
        }

        public void Deserialize(byte[] buffer)
        {
            Buffer = buffer;
        }
    }
    public class poker2092 : Writer, Interfaces.IPacket
    {
        public const uint NotSignedUp = 0,
                          WaitingForOpponent = 1,
                          WaitingInactive = 2;
        byte[] Buffer;
        public poker2092(bool Create)
        {
            Buffer = new byte[28];
            WriteUInt16(20, 0, Buffer);
            WriteUInt16(2092, 2, Buffer);
        }

        public ushort show
        {
            get { return BitConverter.ToUInt16(Buffer, 4); }
            set { WriteUInt16(value, 4, Buffer); }
        }

        public ushort show1
        {
            get { return BitConverter.ToUInt16(Buffer, 6); }
            set { WriteUInt16(value, 6, Buffer); }
        }

        public uint show2
        {
            get { return BitConverter.ToUInt32(Buffer, 8); }
            set { WriteUInt32(value, 8, Buffer); }
        }

        public uint show3
        {
            get { return BitConverter.ToUInt32(Buffer, 12); }
            set { WriteUInt32(value, 12, Buffer); }
        }

        public uint show4
        {
            get { return BitConverter.ToUInt32(Buffer, 16); }
            set { WriteUInt32(value, 16, Buffer); }
        }
        public void Send(Client.GameClient client)
        {
            client.Send(ToArray());
        }

        public byte[] ToArray()
        {
            return Buffer;
        }

        public void Deserialize(byte[] buffer)
        {
            Buffer = buffer;
        }
    }
    public class poker2093 : Writer, Interfaces.IPacket
    {
        public const uint NotSignedUp = 0,
                          WaitingForOpponent = 1,
                          WaitingInactive = 2;
        byte[] Buffer;
        public poker2093(bool Create)
        {
            Buffer = new byte[28];
            WriteUInt16(20, 0, Buffer);
            WriteUInt16(2093, 2, Buffer);
        }

        public ushort show
        {
            get { return BitConverter.ToUInt16(Buffer, 4); }
            set { WriteUInt16(value, 4, Buffer); }
        }

        public ushort show1
        {
            get { return BitConverter.ToUInt16(Buffer, 6); }
            set { WriteUInt16(value, 6, Buffer); }
        }

        public uint show2
        {
            get { return BitConverter.ToUInt32(Buffer, 8); }
            set { WriteUInt32(value, 8, Buffer); }
        }

        public uint show3
        {
            get { return BitConverter.ToUInt32(Buffer, 12); }
            set { WriteUInt32(value, 12, Buffer); }
        }

        public uint show4
        {
            get { return BitConverter.ToUInt32(Buffer, 16); }
            set { WriteUInt32(value, 16, Buffer); }
        }
        public void Send(Client.GameClient client)
        {
            client.Send(ToArray());
        }

        public byte[] ToArray()
        {
            return Buffer;
        }

        public void Deserialize(byte[] buffer)
        {
            Buffer = buffer;
        }
    }
    public class poker2094 : Writer, Interfaces.IPacket
    {
        public const uint NotSignedUp = 0,
                          WaitingForOpponent = 1,
                          WaitingInactive = 2;
        byte[] Buffer;
        public poker2094(bool Create)
        {
            Buffer = new byte[40];
            WriteUInt16(32, 0, Buffer);
            WriteUInt16(2094, 2, Buffer);
        }
        public ushort show
        {
            get { return BitConverter.ToUInt16(Buffer, 4); }
            set { WriteUInt16(value, 4, Buffer); }
        }

        public ushort show1
        {
            get { return BitConverter.ToUInt16(Buffer, 6); }
            set { WriteUInt16(value, 6, Buffer); }
        }

        public uint show2
        {
            get { return BitConverter.ToUInt32(Buffer, 8); }
            set { WriteUInt32(value, 8, Buffer); }
        }

        public uint show3
        {
            get { return BitConverter.ToUInt32(Buffer, 12); }
            set { WriteUInt32(value, 12, Buffer); }
        }

        public uint show4
        {
            get { return BitConverter.ToUInt32(Buffer, 16); }
            set { WriteUInt32(value, 16, Buffer); }
        }
        public uint show5
        {
            get { return BitConverter.ToUInt32(Buffer, 20); }
            set { WriteUInt32(value, 20, Buffer); }
        }
        public uint show6
        {
            get { return BitConverter.ToUInt32(Buffer, 24); }
            set { WriteUInt32(value, 24, Buffer); }
        }
        public uint show7
        {
            get { return BitConverter.ToUInt32(Buffer, 26); }
            set { WriteUInt32(value, 26, Buffer); }
        }
        public uint show8
        {
            get { return BitConverter.ToUInt32(Buffer, 30); }
            set { WriteUInt32(value, 30, Buffer); }
        }
        public uint show9
        {
            get { return BitConverter.ToUInt32(Buffer, 34); }
            set { WriteUInt32(value, 34, Buffer); }
        }
        public void Send(Client.GameClient client)
        {
            client.Send(ToArray());
        }

        public byte[] ToArray()
        {
            return Buffer;
        }

        public void Deserialize(byte[] buffer)
        {
            Buffer = buffer;
        }
    }
    public class poker2095 : Writer, Interfaces.IPacket
    {
        public const uint NotSignedUp = 0,
                          WaitingForOpponent = 1,
                          WaitingInactive = 2;
        byte[] Buffer;
        public poker2095(bool Create)
        {
            Buffer = new byte[46];
            WriteUInt16(38, 0, Buffer);
            WriteUInt16(2095, 2, Buffer);
        }

        public ushort show
        {
            get { return BitConverter.ToUInt16(Buffer, 4); }
            set { WriteUInt16(value, 4, Buffer); }
        }

        public ushort show1
        {
            get { return BitConverter.ToUInt16(Buffer, 6); }
            set { WriteUInt16(value, 6, Buffer); }
        }

        public uint show2
        {
            get { return BitConverter.ToUInt32(Buffer, 8); }
            set { WriteUInt32(value, 8, Buffer); }
        }

        public uint show3
        {
            get { return BitConverter.ToUInt32(Buffer, 12); }
            set { WriteUInt32(value, 12, Buffer); }
        }

        public uint show4
        {
            get { return BitConverter.ToUInt32(Buffer, 16); }
            set { WriteUInt32(value, 16, Buffer); }
        }
        public void Send(Client.GameClient client)
        {
            client.Send(ToArray());
        }

        public byte[] ToArray()
        {
            return Buffer;
        }

        public void Deserialize(byte[] buffer)
        {
            Buffer = buffer;
        }
    }
    public class poker2099 : Writer, Interfaces.IPacket
    {
        public const uint NotSignedUp = 0,
                          WaitingForOpponent = 1,
                          WaitingInactive = 2;
        byte[] Buffer;
        public poker2099(bool Create)
        {
            Buffer = new byte[20];
            WriteUInt16(12, 0, Buffer);
            WriteUInt16(2099, 2, Buffer);
        }
        public ushort show
        {
            get { return BitConverter.ToUInt16(Buffer, 4); }
            set { WriteUInt16(value, 4, Buffer); }
        }

        public ushort show1
        {
            get { return BitConverter.ToUInt16(Buffer, 6); }
            set { WriteUInt16(value, 6, Buffer); }
        }

        public uint show2
        {
            get { return BitConverter.ToUInt32(Buffer, 8); }
            set { WriteUInt32(value, 8, Buffer); }
        }

        public uint show3
        {
            get { return BitConverter.ToUInt32(Buffer, 12); }
            set { WriteUInt32(value, 12, Buffer); }
        }

        public uint show4
        {
            get { return BitConverter.ToUInt32(Buffer, 16); }
            set { WriteUInt32(value, 16, Buffer); }
        }
        public void Send(Client.GameClient client)
        {
            client.Send(ToArray());
        }

        public byte[] ToArray()
        {
            return Buffer;
        }

        public void Deserialize(byte[] buffer)
        {
            Buffer = buffer;
        }
    }
}
