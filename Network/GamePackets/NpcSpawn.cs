using System;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class NpcSpawn : Writer, Interfaces.IPacket, Interfaces.INpc, Interfaces.IMapObject
    {
        private byte[] Buffer;
        private ushort _MapID;
        public string _Name;
        
        public NpcSpawn()
        {
            Buffer = new byte[36];
            WriteUInt16(28, 0, Buffer);
            WriteUInt16(2030, 2, Buffer);
            WriteUInt32((uint)Time32.timeGetTime().GetHashCode(), 4, Buffer);
           // WriteUInt16(1, 22, Buffer);
        }
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;

                byte[] buffer = new byte[90];
                Buffer.CopyTo(buffer, 0);
                WriteUInt16((ushort)(buffer.Length - 8), 0, buffer);
                buffer[26] = 1;
                WriteStringWithLength(value, 27, buffer);
                Buffer = buffer;
            }
        }  
        public uint UID
        {
            get { return BitConverter.ToUInt32(Buffer, 8); }
            set { WriteUInt32(value, 8, Buffer); }
        }

        public ushort X
        {
            get { return BitConverter.ToUInt16(Buffer, 16); }
            set { WriteUInt16(value, 16, Buffer); }
        }

        public ushort Y
        {
            get { return BitConverter.ToUInt16(Buffer, 18); }
            set { WriteUInt16(value, 18, Buffer); }
        }

        public ushort Mesh
        {
            get { return BitConverter.ToUInt16(Buffer, 20); }
            set { WriteUInt16(value, 20, Buffer); }
        }

        public Conquer_Online_Server.Game.Enums.NpcType Type
        {
            get { return (Conquer_Online_Server.Game.Enums.NpcType)Buffer[22]; }
            set { Buffer[22] = (byte)value; }
        }

        public ushort MapID { get { return _MapID; } set { _MapID = value; } }

        public Conquer_Online_Server.Game.MapObjectType MapObjType { get { return Conquer_Online_Server.Game.MapObjectType.Npc; } }
        
        public Client.GameClient Owner { get { return null; } }

        public void SendSpawn(Client.GameClient client, bool checkScreen)
        {
            if (client.Screen.Add(this) || !checkScreen)
            {
                client.Send(Buffer);
            }
        }
        public void SendSpawn(Client.GameClient client)
        {
            SendSpawn(client, false);
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
            SendSpawn(client, false);
        }
    }
}
