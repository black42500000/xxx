using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer_Online_Server.Client;
using Conquer_Online_Server.Interfaces;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class ClientEquip : Interfaces.IPacket
    {
        private Byte[] mData;

        public ClientEquip()
        {
            this.mData = new Byte[88 + 8];
            Writer.WriteUInt16((UInt16)(this.mData.Length - 8), 0, mData);
            Writer.WriteUInt16((UInt16)1009, 2, mData);
            Writer.WriteUInt32((uint)Time32.timeGetTime().GetHashCode(), 4, mData);
            Writer.WriteUInt16((UInt16)0x2E, 16, mData);
        }

        public ClientEquip(GameClient c)
        {
            this.mData = new Byte[88 + 8];
            Writer.WriteUInt16((UInt16)(this.mData.Length - 8), 0, mData);
            Writer.WriteUInt16((UInt16)1009, 2, mData);
            Writer.WriteUInt32((uint)Time32.timeGetTime().GetHashCode(), 4, mData);
            Writer.WriteUInt16((UInt16)0x2E, 16, mData);

            DoEquips(c);
        }

        public void DoEquips(GameClient client)
        {
            if (client.Equipment == null) return;
            ConquerItem[] Items = client.Equipment.Objects;
            Writer.WriteUInt32((uint)Time32.timeGetTime().GetHashCode(), 4, mData);
            Writer.WriteUInt32(client.Entity.UID, 8, mData);
            AlternativeEquipment = client.AlternateEquipment;
            foreach (var Item in client.Equipment.Objects)
            {
                if (Item == null)
                    continue;
                if (Item.IsWorn)
                {
                    switch (Item.Position)
                    {
                        #region Equipment
                        case ConquerItem.Head:
                            this.Helm = Item.UID; break;
                        case ConquerItem.Necklace:
                            this.Necklace = Item.UID;
                            break;
                        case ConquerItem.Armor:
                            this.Armor = Item.UID;
                            break;
                        case ConquerItem.RightWeapon:
                            this.RHand = Item.UID;
                            break;
                        case ConquerItem.LeftWeapon:
                            this.LHand = Item.UID;
                            break;
                        case ConquerItem.Ring:
                            this.Ring = Item.UID;
                            break;
                        case ConquerItem.Boots:
                            this.Boots = Item.UID;
                            break;
                        case ConquerItem.Garment:
                            this.Garment = Item.UID;
                            break;
                        case ConquerItem.Bottle:
                            this.Talisman = Item.UID; break;
                        case ConquerItem.RightWeaponAccessory:
                            AccessoryOne = Item.UID;
                            break;
                        case ConquerItem.LeftWeaponAccessory:
                            AccessoryTwo = Item.UID;
                            break;
                        case ConquerItem.SteedArmor:
                            SteedArmor = Item.UID;
                            break;
                        case ConquerItem.SteedCrop:
                            SteedTalisman = Item.UID;
                            break;
                        #endregion
                        #region AlternateEquipment
                        case ConquerItem.AlternateArmor:
                            Armor = Item.UID;
                            break;
                        case ConquerItem.AlternateHead:
                            Helm = Item.UID;
                            break;
                        case ConquerItem.AlternateNecklace:
                            Necklace = Item.UID;
                            break;
                        case ConquerItem.AlternateRing:
                            Ring = Item.UID;
                            break;
                        case ConquerItem.AlternateBoots:
                            Boots = Item.UID;
                            break;
                        case ConquerItem.AlternateBottle:
                            Talisman = Item.UID;
                            break;
                        case ConquerItem.AlternateGarment:
                            this.Garment = Item.UID;
                            break;
                        case ConquerItem.AlternateLeftWeapon:
                            LHand = Item.UID;
                            break;
                        case ConquerItem.AlternateRightWeapon:
                            RHand = Item.UID;
                            break;
                        #endregion
                    }
                }
            }
            if (client.ArmorLook > 0) Garment = Armor = uint.MaxValue - 1;
            if (client.HeadgearLook > 0) Helm = uint.MaxValue - 2;
        }

        public void Deserialize(byte[] buffer) { this.mData = buffer; }
        public byte[] ToArray()
        { return mData; }
        public void Send(Client.GameClient client) { client.Send(mData); }


        public bool AlternativeEquipment
        {
            get { return this.mData[12] == 1 ? true : false; }
            set { this.mData[12] = value ? (byte)1 : (byte)0; }
        }

        public UInt32 Helm
        {
            get { return BitConverter.ToUInt32(this.mData, 36); }
            set { Writer.WriteUInt32(value, 36, mData); }
        }

        public UInt32 Necklace
        {
            get { return BitConverter.ToUInt32(this.mData, 40); }
            set { Writer.WriteUInt32(value, 40, mData); }
        }

        public UInt32 Armor
        {
            get { return BitConverter.ToUInt32(this.mData, 44); }
            set { Writer.WriteUInt32(value, 44, mData); }
        }

        public UInt32 RHand
        {
            get { return BitConverter.ToUInt32(this.mData, 48); }
            set { Writer.WriteUInt32(value, 48, mData); }
        }

        public UInt32 LHand
        {
            get { return BitConverter.ToUInt32(this.mData, 52); }
            set { Writer.WriteUInt32(value, 52, mData); }
        }

        public UInt32 Ring
        {
            get { return BitConverter.ToUInt32(this.mData, 56); }
            set { Writer.WriteUInt32(value, 56, mData); }
        }

        public UInt32 Talisman
        {
            get { return BitConverter.ToUInt32(this.mData, 60); }
            set { Writer.WriteUInt32(value, 60, mData); }
        }

        public UInt32 Boots
        {
            get { return BitConverter.ToUInt32(this.mData, 64); }
            set { Writer.WriteUInt32(value, 64, mData); }
        }

        public UInt32 Garment
        {
            get { return BitConverter.ToUInt32(this.mData, 68); }
            set { Writer.WriteUInt32(value, 68, mData); }
        }

        public uint AccessoryOne
        {
            get { return BitConverter.ToUInt32(mData, 72); }
            set { Writer.WriteUInt32(value, 72, mData); }
        }

        public uint AccessoryTwo
        {
            get { return BitConverter.ToUInt32(mData, 76); }
            set { Writer.WriteUInt32(value, 76, mData); }
        }
        public UInt32 SteedArmor
        {
            get { return BitConverter.ToUInt32(this.mData, 80); }
            set { Writer.WriteUInt32(value, 80, mData); }
        }

        public UInt32 SteedTalisman
        {
            get { return BitConverter.ToUInt32(this.mData, 84); }
            set { Writer.WriteUInt32(value, 84, mData); }
        }
    }
}

