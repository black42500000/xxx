using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class PokerTable : Writer, Interfaces.IPacket, Interfaces.table, Interfaces.IMapObject
    {
        private byte[] Buffer;
        private ushort _MapID;

        public PokerTable()
        {
            //  WriteUInt16(52, 0, Pack);
            //  WriteUInt16(2172, 2, Pack);
            //   WriteUInt32((uint)101915, 4, Pack);//table uid
            //   WriteUInt16(C.X, 16, Pack);
            //   WriteUInt16(C.Y, 18, Pack);
            //  WriteUInt32((uint)7217967, 20, Pack);//table mesh with chairs.. NEEDED
            //  WriteUInt32(3, 26, Pack);//table id... it's auto boosted.. 0 = 1, 1 = 2, etc
            //WriteUInt16(1, 30, Pack);//odd... interferes with pot count. MUST BE 1
            // WriteUInt32(10000, 38, Pack);
            Buffer = new byte[60];
            WriteUInt16(52, 0, Buffer);
            WriteUInt16(2172, 2, Buffer);
            // WriteUInt16(1, 22, Buffer);
        }

        public uint UID
        {
            get { return BitConverter.ToUInt32(Buffer, 4); }
            set { WriteUInt32(value, 4, Buffer); }
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
        public uint Mesh
        {
            get { return BitConverter.ToUInt32(Buffer, 20); }
            set { WriteUInt32(value, 20, Buffer); }
        }
        public uint TableUID
        {
            get { return BitConverter.ToUInt32(Buffer, 26); }
            set { WriteUInt32(value, 26, Buffer); }
        }
        public ushort BE
        {
            get { return BitConverter.ToUInt16(Buffer, 30); }
            set { WriteUInt16(value, 30, Buffer); }
        }
        public uint Other
        {
            get { return BitConverter.ToUInt32(Buffer, 38); }
            set { WriteUInt32(value, 38, Buffer); }
        }
        public Conquer_Online_Server.Game.Enums.NpcType Type
        {
            get { return (Conquer_Online_Server.Game.Enums.NpcType)Buffer[18]; }
            set { Buffer[18] = (byte)value; }
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
