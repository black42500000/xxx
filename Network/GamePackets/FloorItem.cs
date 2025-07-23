using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class FloorItem : Writer, Interfaces.IPacket, Interfaces.IMapObject
    {
 
        public const ushort Drop = 1,
            Remove = 2,
            Animation = 3,
            DropDetain = 4,
            Effect = 10,
            RemoveEffect = 12;
        public const uint
            Twilight = 40,
            DaggerStorm = 1027,
        FuryofEgg = 1041,
        ShacklingIce = 1042;  
        public static Counter FloorUID = new Counter(0);
        byte[] Buffer;
        Client.GameClient owner;
        ushort mapid;
        public Time32 OnFloor, UseTime;
        public bool PickedUpAlready = false;
        public uint Value;
        public bool Shake = false;
        public bool Zoom = false;
        public bool Darkness = false;
        public FloorValueType ValueType;
        private ConquerItem item;
        public FloorItem(bool Create)
        {
            if (Create)
            {
                Buffer = new byte[28 + 8];
                WriteUInt16(28, 0, Buffer);
                WriteUInt16(1101, 2, Buffer);
                WriteUInt32((uint)Time32.timeGetTime().GetHashCode(), 4, Buffer);
                Value = 0;
                ValueType = FloorValueType.Item;
            }
        }
        public void AppendFlags()
        {
            WriteInt32((Shake ? 1 : 0) | (Zoom ? 2 : 0) | (Darkness ? 4 : 0), 8, Buffer);
        }
        public uint UID
        {
            get { return BitConverter.ToUInt32(Buffer, 8); }
            set { WriteUInt32(value, 8, Buffer); }
        }
        public uint ItemID
        {
            get { return BitConverter.ToUInt32(Buffer, 12); }
            set { WriteUInt32(value, 12, Buffer); }
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
        public Game.Enums.Color ItemColor
        {
            get { return (Game.Enums.Color)BitConverter.ToUInt16(Buffer, 20); }
            set { WriteUInt16((ushort)value, 20, Buffer); }
        }
        public ushort Type
        {
            get { return BitConverter.ToUInt16(Buffer, 22); }
            set { WriteUInt32(value, 22, Buffer); }
        }
        public Conquer_Online_Server.Game.MapObjectType MapObjType
        {
            get { return Conquer_Online_Server.Game.MapObjectType.Item; }
            set { }
        }
        public void Send(Client.GameClient client)
        {
            client.Send(Buffer);
        }
        public void SendSpawn(Client.GameClient client)
        {
            SendSpawn(client, false);
        }
        public Client.GameClient Owner
        {
            get { return owner; }
            set { owner = value; }
        }
        public ConquerItem Item
        {
            get { return item; }
            set { item = value; }
        }
        public ushort MapID
        {
            get { return mapid; }
            set { mapid = value; }
        }
        public void SendSpawn(Client.GameClient client, bool checkScreen)
        {
            if (client.Screen.Add(this) || !checkScreen)
            {
                client.Send(Buffer);
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
        public enum FloorValueType { Item, Money, ConquerPoints }
    }
}
