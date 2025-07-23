using System;
using System.Text;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class NpcRequest : Writer, Interfaces.IPacket
    {
        private byte[] Buffer;

        public void Deserialize(byte[] buffer)
        {
            Buffer = buffer;
        }
        public byte[] ToArray()
        {
            throw new NotImplementedException();
        }

        public uint NpcID
        {
            get { return BitConverter.ToUInt32(Buffer, 8); }
            set { WriteUInt32(value, 8, Buffer); }
        }

        public byte OptionID
        {
            get { return Buffer[14]; }
            set { Buffer[14] = value; }
        }

        public byte InteractType
        {
            get { return Buffer[15]; }
        }

        public string Input
        {
            get { return Encoding.Default.GetString(Buffer, 18, Buffer[17]); }
        }

        public void Send(Client.GameClient client)
        {
            client.Send(Buffer);
        }
    }
}
