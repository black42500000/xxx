using System;
using System.Collections.Generic;
using Conquer_Online_Server.Network.GamePackets;
using Conquer_Online_Server.Interfaces;

namespace Conquer_Online_Server.Game.ConquerStructures
{
    public class Equipment
    {
        ConquerItem[] objects;
        Client.GameClient Owner;
        public Equipment(Client.GameClient client)
        {
            Owner = client;
            objects = new ConquerItem[40];
        }

        public void UpdateEntityPacket()
        {
            for (byte Position = 1; Position < 29; Position++)
            {
                if (Free(Position))
                {
                    ClearItemview(Position);
                }
                else
                {
                    var item = TryGetItem(Position);
                    UpdateItemview(item);
                }
            }

            if (Owner.ArmorLook != 0)
            {
                Network.Writer.WriteUInt32(0, ArmorSoul, Owner.Entity.SpawnPacket);
                Network.Writer.WriteUInt32(Owner.ArmorLook, Armor, Owner.Entity.SpawnPacket);
            }
            if (Owner.HeadgearLook > 0)
            {
                Network.Writer.WriteUInt32(0, HeadSoul, Owner.Entity.SpawnPacket);
                Network.Writer.WriteUInt32(Owner.HeadgearLook, Head, Owner.Entity.SpawnPacket);
            }

            Owner.SendScreen(Owner.Entity.SpawnPacket, false);
        }
        public uint GetGear(byte Position, Client.GameClient C)
        {
            ConquerItem I = C.Equipment.TryGetItem(Position);
            if (I == null)
            {
                return 0;
            }
            return I.UID;
        }
        public bool Add(ConquerItem item)
        {
            if (objects.Length < item.Position)
                return false;

            if (objects[item.Position - 1] == null)
            {
                item.IsWorn = true;
                UpdateItemview(item);
                objects[item.Position - 1] = item;
                item.Position = item.Position;
                item.Send(Owner);

                Owner.LoadItemStats();
                Owner.SendScreenSpawn(Owner.Entity, false);

                return true;
            }
            else return false;
        }
        public bool Add(ConquerItem item, Enums.ItemUse use)
        {
            if (objects[item.Position - 1] == null)
            {
                objects[item.Position - 1] = item;
                item.Mode = Enums.ItemMode.Default;

                if (use != Enums.ItemUse.None)
                {
                    item.IsWorn = true;
                    UpdateItemview(item);

                    item.Send(Owner);
                    Owner.LoadItemStats();
                }
                return true;
            }
            else return false;
        }

        #region Offsets
        public const int
            Head = 52,
            Garment = 56,
            Armor = 60,
            LeftWeapon = 64,
            RightWeapon = 68,
            LeftWeaponAccessory = 72,
            RightWeaponAccessory = 76,
            Steed = 80,
            MountArmor = 84,
            ArmorColor = 145,
            LeftWeaponColor = 147,
            HeadColor = 149,
            SteedPlus = 155,
            SteedColor = 161,
            HeadSoul = 204,
            ArmorSoul = 208,
            LeftWeaponSoul = 212,
            RightWeaponSoul = 216;
        #endregion

        public void ClearItemview(uint Position)
        {
            switch ((ushort)Position)
            {
                case Network.GamePackets.ConquerItem.Head:
                    if (Owner.HeadgearLook != 0)
                    {
                        Network.Writer.WriteUInt32(0, HeadSoul, Owner.Entity.SpawnPacket);
                        Network.Writer.WriteUInt32(Owner.HeadgearLook, Head, Owner.Entity.SpawnPacket);
                    }
                    else
                    {
                        Network.Writer.WriteUInt32(0, HeadSoul, Owner.Entity.SpawnPacket);
                        Network.Writer.WriteUInt32(0, Head, Owner.Entity.SpawnPacket);
                        Network.Writer.WriteUInt16(0, HeadColor, Owner.Entity.SpawnPacket);
                    }
                    break;
                case Network.GamePackets.ConquerItem.Garment:
                    if (Owner.Entity.MapID != 1081)
                        Network.Writer.WriteUInt32(0, Garment, Owner.Entity.SpawnPacket);
                    break;
                case Network.GamePackets.ConquerItem.Armor:
                    if (Owner.ArmorLook != 0)
                    {
                        Network.Writer.WriteUInt32(0, ArmorSoul, Owner.Entity.SpawnPacket);
                        Network.Writer.WriteUInt32(Owner.ArmorLook, Armor, Owner.Entity.SpawnPacket);
                    }
                    else
                    {
                        Network.Writer.WriteUInt32(0, ArmorSoul, Owner.Entity.SpawnPacket);
                        Network.Writer.WriteUInt32(0, Armor, Owner.Entity.SpawnPacket);
                        Network.Writer.WriteUInt16(0, ArmorColor, Owner.Entity.SpawnPacket);
                    }
                    break;
                case Network.GamePackets.ConquerItem.RightWeapon:
                    Network.Writer.WriteUInt32(0, RightWeaponSoul, Owner.Entity.SpawnPacket);
                    Network.Writer.WriteUInt32(0, RightWeapon, Owner.Entity.SpawnPacket);
                    break;
                case Network.GamePackets.ConquerItem.LeftWeapon:
                    Network.Writer.WriteUInt32(0, LeftWeaponSoul, Owner.Entity.SpawnPacket);
                    Network.Writer.WriteUInt32(0, LeftWeapon, Owner.Entity.SpawnPacket);
                    Network.Writer.WriteUInt16(0, LeftWeaponColor, Owner.Entity.SpawnPacket);
                    break;
                case Network.GamePackets.ConquerItem.RightWeaponAccessory:
                    Network.Writer.WriteUInt32(0, RightWeaponAccessory, Owner.Entity.SpawnPacket);
                    break;
                case Network.GamePackets.ConquerItem.LeftWeaponAccessory:
                    Network.Writer.WriteUInt32(0, LeftWeaponAccessory, Owner.Entity.SpawnPacket);
                    break;
                case Network.GamePackets.ConquerItem.Steed:
                    Network.Writer.WriteUInt32(0, Steed, Owner.Entity.SpawnPacket);
                    Network.Writer.WriteUInt16(0, SteedPlus, Owner.Entity.SpawnPacket);
                    Network.Writer.WriteUInt32(0, SteedColor, Owner.Entity.SpawnPacket);
                    break;
                case Network.GamePackets.ConquerItem.SteedArmor:
                    Network.Writer.WriteUInt32(0, MountArmor, Owner.Entity.SpawnPacket);
                    break;
            }
        }
        public void UpdateItemview(ConquerItem item)
        {
            if (item == null) return;
            if (!item.IsWorn) return;
            switch ((ushort)item.Position)
            {
                case Network.GamePackets.ConquerItem.AlternateHead:
                case Network.GamePackets.ConquerItem.Head:
                    if (Owner.HeadgearLook != 0)
                    {
                        Network.Writer.WriteUInt32(0, HeadSoul, Owner.Entity.SpawnPacket);
                        Network.Writer.WriteUInt32(Owner.HeadgearLook, Head, Owner.Entity.SpawnPacket);
                    }
                    else
                    {
                        if (item.Purification.Available)
                            Network.Writer.WriteUInt32(item.Purification.PurificationItemID, HeadSoul, Owner.Entity.SpawnPacket);
                        else Network.Writer.WriteUInt32(0, HeadSoul, Owner.Entity.SpawnPacket);
                        Network.Writer.WriteUInt32(item.ID, Head, Owner.Entity.SpawnPacket);
                    }
                    Network.Writer.WriteUInt16((byte)item.Color, HeadColor, Owner.Entity.SpawnPacket);
                    break;
                case Network.GamePackets.ConquerItem.AlternateGarment:
                case Network.GamePackets.ConquerItem.Garment:
                        Network.Writer.WriteUInt32(item.ID, Garment, Owner.Entity.SpawnPacket);
                        break;
                case Network.GamePackets.ConquerItem.AlternateArmor:
                case Network.GamePackets.ConquerItem.Armor:
                    if (Owner.ArmorLook != 0)
                    {
                        Network.Writer.WriteUInt32(0, ArmorSoul, Owner.Entity.SpawnPacket);
                        Network.Writer.WriteUInt32(Owner.ArmorLook, Armor, Owner.Entity.SpawnPacket);
                    }
                    else
                    {
                        if (item.Purification.Available)
                            Network.Writer.WriteUInt32(item.Purification.PurificationItemID, ArmorSoul, Owner.Entity.SpawnPacket);
                        else Network.Writer.WriteUInt32(0, ArmorSoul, Owner.Entity.SpawnPacket);
                        Network.Writer.WriteUInt32(item.ID, Armor, Owner.Entity.SpawnPacket);
                    }
                    Network.Writer.WriteUInt16((byte)item.Color, ArmorColor, Owner.Entity.SpawnPacket);
                    break;
                case Network.GamePackets.ConquerItem.AlternateRightWeapon:
                case Network.GamePackets.ConquerItem.RightWeapon:
                    if (item.Purification.Available)
                        Network.Writer.WriteUInt32(item.Purification.PurificationItemID, RightWeaponSoul, Owner.Entity.SpawnPacket);
                    else Network.Writer.WriteUInt32(0, RightWeaponSoul, Owner.Entity.SpawnPacket);
                    Network.Writer.WriteUInt32(item.ID, RightWeapon, Owner.Entity.SpawnPacket);
                    break;
                case Network.GamePackets.ConquerItem.RightWeaponAccessory:
                    Network.Writer.WriteUInt32(item.ID, RightWeaponAccessory, Owner.Entity.SpawnPacket);
                    break;
                case Network.GamePackets.ConquerItem.AlternateLeftWeapon:
                case Network.GamePackets.ConquerItem.LeftWeapon:
                    if (item.Purification.Available)
                        Network.Writer.WriteUInt32(item.Purification.PurificationItemID, LeftWeaponSoul, Owner.Entity.SpawnPacket);
                    else Network.Writer.WriteUInt32(0, LeftWeaponSoul, Owner.Entity.SpawnPacket);
                    Network.Writer.WriteUInt16((byte)item.Color, LeftWeaponColor, Owner.Entity.SpawnPacket);
                    Network.Writer.WriteUInt32(item.ID, LeftWeapon, Owner.Entity.SpawnPacket);
                    break;
                case Network.GamePackets.ConquerItem.LeftWeaponAccessory:
                    Network.Writer.WriteUInt32(item.ID, LeftWeaponAccessory, Owner.Entity.SpawnPacket);
                    break;
                case Network.GamePackets.ConquerItem.Steed:
                    Network.Writer.WriteUInt32(item.ID, Steed, Owner.Entity.SpawnPacket);
                    Network.Writer.WriteUInt16((byte)item.Plus, SteedPlus, Owner.Entity.SpawnPacket);
                    Network.Writer.WriteUInt32(item.SocketProgress, SteedColor, Owner.Entity.SpawnPacket);
                    break;
                case Network.GamePackets.ConquerItem.SteedArmor:
                    Network.Writer.WriteUInt32(item.ID, MountArmor, Owner.Entity.SpawnPacket);
                    break;
            }
        }

        public bool Remove(byte Position)
        {
            if (objects[Position - 1] != null)
            {
                if (Owner.Inventory.Count <= 39)
                {
                    if (Owner.Inventory.Add(objects[Position - 1], Enums.ItemUse.Move))
                    {
                        objects[Position - 1].Position = Position;
                        objects[Position - 1].IsWorn = false;
                        //Owner.UnloadItemStats(objects[Position - 1], false);
                        objects[Position - 1].Position = 0;
                        if (Position == 12)
                            Owner.Entity.RemoveFlag(Network.GamePackets.Update.Flags.Ride);
                        if (Position == 4)
                            Owner.Entity.RemoveFlag(Network.GamePackets.Update.Flags.Fly);
                        Network.GamePackets.ItemUsage iu = new Network.GamePackets.ItemUsage(true);
                        iu.UID = objects[Position - 1].UID;
                        iu.dwParam = Position;
                        iu.ID = Network.GamePackets.ItemUsage.UnequipItem;
                        Owner.Send(iu);
                        ClearItemview(Position);
                        objects[Position - 1] = null;
                        Owner.SendScreenSpawn(Owner.Entity, false);
                        return true;
                    }
                }
                else
                {
                    Owner.Send(new Network.GamePackets.Message("Not enough room in your inventory.", System.Drawing.Color.Red, Network.GamePackets.Message.TopLeft));
                }
            }
            return false;
        }
        public bool DestroyArrow(uint Position)
        {
            if (objects[Position - 1] != null)
            {
                objects[Position - 1].Position = (ushort)Position;
                if (objects[Position - 1].ID == 0)
                {
                    objects[Position - 1].Position = 0;
                    Database.ConquerItemTable.DeleteItem(objects[Position - 1].UID);
                    objects[Position - 1] = null;
                    return true;
                }
                if (!Network.PacketHandler.IsArrow(objects[Position - 1].ID))
                    return false;

                //Owner.UnloadItemStats(objects[Position - 1], false);
                objects[Position - 1].IsWorn = false;
                Database.ConquerItemTable.DeleteItem(objects[Position - 1].UID);
                Network.GamePackets.ItemUsage iu = new Network.GamePackets.ItemUsage(true);
                iu.UID = objects[Position - 1].UID;
                iu.dwParam = Position;
                iu.ID = Network.GamePackets.ItemUsage.UnequipItem;
                Owner.Send(iu);
                iu.dwParam = 0;
                iu.ID = Network.GamePackets.ItemUsage.RemoveInventory;
                Owner.Send(iu);
                ClearItemview(Position);
                objects[Position - 1].Position = 0;
                objects[Position - 1] = null;
                return true;
            }
            return false;
        }
        public bool RemoveToGround(uint Position)
        {
            if (Position == 0 || Position > 19)//zawdt dey? no dy asln 13
                return true;
            if (objects[Position - 1] != null)
            {
                objects[Position - 1].Position = (ushort)Position;
                objects[Position - 1].IsWorn = false;
                //Owner.UnloadItemStats(objects[Position - 1], false);
                objects[Position - 1].Position = 0;
                Database.ConquerItemTable.RemoveItem(objects[Position - 1].UID);
                Network.GamePackets.ItemUsage iu = new Network.GamePackets.ItemUsage(true);
                iu.UID = objects[Position - 1].UID;
                iu.dwParam = Position;
                iu.ID = Network.GamePackets.ItemUsage.UnequipItem;
                Owner.Send(iu);
                iu.dwParam = 0;
                iu.ID = Network.GamePackets.ItemUsage.RemoveInventory;
                Owner.Send(iu);

                ClearItemview(Position);
                objects[Position - 1] = null;
                return true;
            }
            return false;
        }
        public ConquerItem[] Objects
        {
            get
            {
                return objects;
            }
        }
        public byte Count
        {
            get
            {
                byte count = 0; foreach (ConquerItem i in objects)
                    if (i != null)
                        count++; return count;
            }
        }
        public bool Free(byte Position)
        {
            return TryGetItem(Position) == null;
        }
        public bool Free(uint Position)
        {
            return TryGetItem((byte)Position) == null;
        }
        public ConquerItem TryGetItem(byte Position)
        {
            ConquerItem item = null;
            if (Position < 1 || Position > 39)
                return item;
            item = objects[Position - 1];
            return item;
        }
        public ConquerItem TryGetItem(uint uid)
        {
            try
            {
                foreach (ConquerItem item in objects)
                {
                    if (item != null)
                        if (item.UID == uid)
                            return item;
                }
            }
            catch (Exception e)
            {
                Program.SaveException(e);
                //Console.WriteLine(e);
            }
            return TryGetItem((byte)uid);
        }

        public bool IsArmorSuper()
        {
            if (TryGetItem(3) != null)
                return TryGetItem(3).ID % 10 == 9;
            return false;
        }
        public bool IsAllSuper()
        {
            for (byte count = 1; count < 12; count++)
            {
                if (count == 5)
                {
                    if (Owner.Entity.Class > 100)
                        continue;
                    if (TryGetItem(count) != null)
                    {
                        if (Network.PacketHandler.IsArrow(TryGetItem(count).ID))
                            continue;
                        if (Network.PacketHandler.IsTwoHand(TryGetItem(4).ID))
                            continue;
                        if (TryGetItem(count).ID % 10 != 9)
                            return false;
                    }
                }
                else
                {
                    if (TryGetItem(count) != null)
                    {
                        if (count != Network.GamePackets.ConquerItem.Bottle && count != Network.GamePackets.ConquerItem.Garment)
                            if (TryGetItem(count).ID % 10 != 9)
                                return false;
                    }
                    else
                        if (count != Network.GamePackets.ConquerItem.Bottle && count != Network.GamePackets.ConquerItem.Garment)
                            return false;
                }
            }
            return true;
        }
    }
}
