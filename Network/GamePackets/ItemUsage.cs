using System;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class ItemUsage : Writer, Interfaces.IPacket
    {
        public const ushort
            BuyFromNPC = 1,
            SellToNPC = 2,
            RemoveInventory = 3,
            EquipItem = 4,
            UnequipItem = 6,
            ArrowReload = 8,
            ViewWarehouse = 9,
            WarehouseDeposit = 10,
            WarehouseWithdraw = 11,
            Repair = 14,
            VIPRepair = 15,
            DragonBallUpgrade = 19,
            MeteorUpgrade = 20,
            ShowBoothItems = 21,
            AddItemOnBoothForSilvers = 22,
            RemoveItemFromBooth = 23,
            BuyFromBooth = 24,
            UpdateDurability = 25,
            AddItemOnBoothForConquerPoints = 29,
            Ping = 27,
            Enchant = 28,
            RedeemGear = 32,
            ClaimGear = 33,
            SocketTalismanWithItem = 35,
            SocketTalismanWithCPs = 36,
            DropItem = 37,
            DropMoney = 38,
            GemCompose = 39,
            Bless = 40,
            Accessories = 41,
            MainEquipment = 44,
            AlternateEquipment = 45,
            ToristSuper = 51,
            SocketerMan = 43,
            MergeStackableItems = 48,
            SplitStack = 49;

        byte[] Buffer;
        public ItemUsage(bool Create)
        {
            if (Create)
            {
                Buffer = new byte[92];
                WriteUInt16(84, 0, Buffer);
                WriteUInt16(1009, 2, Buffer);
                WriteUInt32((uint)Time32.timeGetTime().GetHashCode(), 4, Buffer);
            }
        }
        public uint UID
        {
            get { return BitConverter.ToUInt32(Buffer, 8); }
            set { WriteUInt32(value, 8, Buffer); }
        }
        public uint dwParam
        {
            get { return BitConverter.ToUInt32(Buffer, 12); }
            set { WriteUInt32(value, 12, Buffer); }
        }

        public uint ID
        {
            get { return BitConverter.ToUInt32(Buffer, 16); }
            set { WriteUInt32(value, 16, Buffer); }
        }

        public uint TimeStamp
        {
            get { return BitConverter.ToUInt32(Buffer, 20); }
            set { WriteUInt32(value, 20, Buffer); }
        }

        public uint dwExtraInfo
        {
            get { return BitConverter.ToUInt32(Buffer, 24); }
            set { WriteUInt32(value, 24, Buffer); }
        }
        public uint dwExtraInfo2
        {
            get { return BitConverter.ToUInt32(Buffer, 28); }
            set { WriteUInt32(value, 28, Buffer); }
        }
        public uint dwExtraInfo3
        {
            get { return BitConverter.ToUInt32(Buffer, 32); }
            set { WriteUInt32(value, 32, Buffer); }
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
            client.Send(Buffer);
        }
    }
}