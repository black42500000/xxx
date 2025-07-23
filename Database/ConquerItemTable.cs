using System;
using System.Linq;
using Conquer_Online_Server.Network.GamePackets;
using System.Collections.Generic;

namespace Conquer_Online_Server.Database
{
    public class ConquerItemTable
    {
        public static void LoadItems(Client.GameClient client)
        {
            using (var cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("items").Where("EntityID", client.Entity.UID))
            using (var reader = new MySqlReader(cmd))
            {
                while (reader.Read())
                {
                    var item = deserialzeItem(reader);
                    if (!Database.ConquerItemInformation.BaseInformations.ContainsKey(item.ID))
                        continue;  
                    HandleInscribing(item, client);
                    ItemAddingTable.GetAddingsForItem(item);
                    if (item.Warehouse == 0)
                    {
                        switch (item.Position)
                        {
                            case 0: client.Inventory.Add(item, Game.Enums.ItemUse.None); break;
                            default:
                                if (item.Position > 40) continue;
                                if (client.Equipment.Free((byte)item.Position))
                                    client.Equipment.Add(item, Game.Enums.ItemUse.None);
                                else
                                {
                                    if (client.Inventory.Count < 40)
                                    {
                                        item.Position = 0;
                                        client.Inventory.Add(item, Game.Enums.ItemUse.None);
                                        if (client.Warehouses[Conquer_Online_Server.Game.ConquerStructures.Warehouse.WarehouseID.StoneCity].Count < 20)
                                            client.Warehouses[Conquer_Online_Server.Game.ConquerStructures.Warehouse.WarehouseID.StoneCity].Add(item);
                                        UpdatePosition(item);
                                    }
                                }
                                break;
                        }
                    }
                    else
                    {
                        if (item != null)
                        {
                            Conquer_Online_Server.Game.ConquerStructures.Warehouse.WarehouseID whID = (Game.ConquerStructures.Warehouse.WarehouseID)item.Warehouse;
                            if (client.Warehouses.ContainsKey(whID))
                            {
                                client.Warehouses[whID].Add(item);
                            }
                        }
                    }
                }
            }
        }
        private static ConquerItem deserialzeItem(MySqlReader reader)
        {
            ConquerItem item = new Network.GamePackets.ConquerItem(true);
            item.ID = reader.ReadUInt32("Id");
            item.UID = reader.ReadUInt32("Uid");
            //item.Durability = reader.ReadUInt16("Durability");
            item.MaximDurability = reader.ReadUInt16("MaximDurability");
            item.Durability = item.MaximDurability;
            item.Position = reader.ReadUInt16("Position");
            item.Agate = reader.ReadString("Agate");
            item.SocketProgress = reader.ReadUInt32("SocketProgress");
            item.PlusProgress = reader.ReadUInt32("PlusProgress");
            item.SocketOne = (Game.Enums.Gem)reader.ReadUInt16("SocketOne");
            item.SocketTwo = (Game.Enums.Gem)reader.ReadUInt16("SocketTwo");
            item.Effect = (Game.Enums.ItemEffect)reader.ReadUInt16("Effect");
            item.Mode = Game.Enums.ItemMode.Default;
            item.Plus = reader.ReadByte("Plus");
            item.Bless = reader.ReadByte("Bless");
            item.Bound = reader.ReadBoolean("Bound");
            item.Enchant = reader.ReadByte("Enchant");
            item.Lock = reader.ReadByte("Locked");
            item.UnlockEnd = DateTime.FromBinary(reader.ReadInt64("UnlockEnd"));
            item.Suspicious = reader.ReadBoolean("Suspicious");
            item.SuspiciousStart = DateTime.FromBinary(reader.ReadInt64("SuspiciousStart"));
            item.Color = (Game.Enums.Color)reader.ReadUInt32("Color");
            item.Warehouse = reader.ReadUInt16("Warehouse");
            item.StackSize = reader.ReadUInt16("StackSize");
            item.RefineItem = reader.ReadUInt32("RefineryItem");

            if (item.ID == 300000)
            {
                uint NextSteedColor = reader.ReadUInt32("NextSteedColor");
                item.NextGreen = (byte)(NextSteedColor & 0xFF);
                item.NextBlue = (byte)((NextSteedColor >> 8) & 0xFF);
                item.NextRed = (byte)((NextSteedColor >> 16) & 0xFF);
            }

            Int64 rTime = reader.ReadInt64("RefineryTime");
            if (item.RefineItem > 0 && rTime != 0)
            {
                item.RefineryTime = DateTime.FromBinary(rTime);
                if (DateTime.Now > item.RefineryTime)
                {
                    item.RefineryTime = new DateTime(0);
                    item.RefineItem = 0;
                }
            }
            if (item.Lock == 2)
                if (DateTime.Now >= item.UnlockEnd)
                    item.Lock = 0;
            return item;
        }
        public static void HandleInscribing(ConquerItem item, Client.GameClient client, bool detained = false)
        {
            if (client.Entity.GuildID != 0)
            {
                if (client.Guild != null)
                {
                    int itemPosition = Network.PacketHandler.ArsenalPosition(item.ID);
                    if (itemPosition != -1)
                    {
                        var arsenal = client.Guild.Arsenals[itemPosition];
                        if (arsenal.Unlocked)
                        {
                            if (arsenal.ItemDictionary.ContainsKey(item.UID))
                            {
                                var arsenalItem = arsenal.ItemDictionary[item.UID];
                                arsenalItem.Update(item, client);
                                item.Inscribed = true;
                                client.ArsenalDonations[itemPosition] += arsenalItem.DonationWorth;
                            }
                        }
                    }
                }
            }
        }
        public static ConquerItem LoadItem(uint UID)
        {
            ConquerItem item = new ConquerItem(true);
            ConquerItemInformation information = new ConquerItemInformation(item.ID, item.Plus);
            MySqlCommand command = new MySqlCommand(MySqlCommandType.SELECT);
            command.Select("items").Where("UID", (long)UID);
            MySqlReader reader = new MySqlReader(command);
            if (reader.Read())
            {
                item.ID = reader.ReadUInt32("ID");
                item.UID = reader.ReadUInt32("UID");
                item.MaximDurability = reader.ReadUInt16("MaximDurability");
                item.Durability = item.MaximDurability;
                item.Position = reader.ReadUInt16("Position");
                item.SocketProgress = reader.ReadUInt32("SocketProgress");
                item.PlusProgress = reader.ReadUInt32("PlusProgress");
                item.SocketOne = (Game.Enums.Gem)reader.ReadByte("SocketOne");
                item.SocketTwo = (Game.Enums.Gem)reader.ReadByte("SocketTwo");
                item.Effect = (Game.Enums.ItemEffect)reader.ReadByte("Effect");
                item.Mode = Game.Enums.ItemMode.Default;
                item.Plus = reader.ReadByte("Plus");
                item.Bless = reader.ReadByte("Bless");
                item.Bound = reader.ReadBoolean("Bound");
                item.Enchant = reader.ReadByte("Enchant");
                item.Lock = reader.ReadByte("Locked");
                item.UnlockEnd = DateTime.FromBinary(reader.ReadInt64("UnlockEnd"));
                item.Suspicious = reader.ReadBoolean("Suspicious");
                item.SuspiciousStart = DateTime.FromBinary(reader.ReadInt64("SuspiciousStart"));
                item.Color = (Game.Enums.Color)reader.ReadByte("Color");
                item.Inscribed = reader.ReadByte("Inscribed") == 1;
                item.StackSize = reader.ReadUInt16("StackSize");
                item.Warehouse = reader.ReadUInt16("Warehouse");
                item.RefineItem = reader.ReadUInt32("RefineryItem");
                Int64 rTime = reader.ReadInt64("RefineryTime");
                if (item.RefineItem > 0 && rTime != 0)
                {
                    item.RefineryTime = DateTime.FromBinary(rTime);
                    if (DateTime.Now > item.RefineryTime)
                    {
                        item.RefineryTime = new DateTime(0);
                        item.RefineItem = 0;
                    }
                }
                if ((item.Lock == 2) && (DateTime.Now >= item.UnlockEnd))
                {
                    item.Lock = 0;
                }
                ItemAddingTable.GetAddingsForItem(item);
            }
            return item;
        }

        public static void AddItem(ref ConquerItem Item, Client.GameClient client)
        {
            try
            {
                using (var cmd = new MySqlCommand(MySqlCommandType.INSERT).Insert("items"))
                    cmd.Insert("ID", Item.ID).Insert("UID", Item.UID)
                        .Insert("Plus", Item.Plus).Insert("Bless", Item.Bless)
                        .Insert("Enchant", Item.Enchant).Insert("SocketOne", (byte)Item.SocketOne)
                        .Insert("SocketTwo", (byte)Item.SocketTwo).Insert("Durability", Item.Durability)
                        .Insert("MaximDurability", Item.MaximDurability).Insert("SocketProgress", Item.SocketProgress)
                        .Insert("PlusProgress", Item.PlusProgress).Insert("Effect", (ushort)Item.Effect)
                        .Insert("Bound", Item.Bound).Insert("Locked", Item.Lock).Insert("UnlockEnd", Item.UnlockEnd.Ticks)
                        .Insert("Suspicious", Item.Suspicious).Insert("SuspiciousStart", Item.SuspiciousStart.Ticks).Insert("NextSteedColor", 0)
                        .Insert("Color", (ushort)Item.Color).Insert("Position", Item.Position).Insert("StackSize", Item.StackSize)
                        .Insert("RefineryItem", Item.RefineItem).Insert("RefineryTime", Item.RefineryTime.Ticks).Insert("EntityID", client.Entity.UID)
                        .Execute();
            }
            catch (Exception )
            {
                ////Console.WriteLine(e);
                DeleteItem(Item.UID);
                AddItem(ref Item, client);
            }
        }
        private static void UpdateData(ConquerItem Item, string column, object value)
        {
            UpdateData(Item.UID, column, value);
        }
        private static void UpdateData(uint UID, string column, object value)
        {
            using (var cmd = new MySqlCommand(MySqlCommandType.UPDATE))
                cmd.Update("items").Set(column, value.ToString())
                    .Where("UID", UID).Execute();
        }
        public static void UpdateBless(ConquerItem Item)
        {
            UpdateData(Item, "Bless", Item.Bless);
        }
        public static void UpdateItemAgate(ConquerItem Item)
        {
            string agate = "";
            if (Item.ID == 720828)
            {
                foreach (string coord in Item.Agate_map.Values)
                {
                    agate += coord + "#";
                    UpdateData(Item, "agate", agate);
                }
            }
        }
        public static void UpdateColor(ConquerItem Item)
        {
            UpdateData(Item, "Color", (uint)Item.Color);
        }
        public static void UpdateStack(ConquerItem Item)
        {
            UpdateData(Item, "StackSize", Item.StackSize);
        }
        public static void UpdateEnchant(ConquerItem Item)
        {
            UpdateData(Item, "Enchant", Item.Enchant);
        }
        public static void UpdateLock(ConquerItem Item)
        {
            UpdateData(Item, "Locked", Item.Lock);
            UpdateData(Item, "UnlockEnd", Item.UnlockEnd.ToBinary());
        }
        public static void UpdateSockets(ConquerItem Item)
        {
            UpdateData(Item, "SocketOne", (byte)Item.SocketOne);
            UpdateData(Item, "SocketTwo", (byte)Item.SocketTwo);
        }
        public static void UpdateSocketProgress(ConquerItem Item)
        {
            UpdateData(Item, "SocketProgress", Item.SocketProgress);
        }
        public static void UpdateNextSteedColor(ConquerItem Item)
        {
            UpdateData(Item, "NextSteedColor", Item.NextGreen | (Item.NextBlue << 8) | (Item.NextRed << 16));
        }
        public static void UpdateRefineryItem(ConquerItem Item)
        {
            UpdateData(Item, "RefineryItem", Item.RefineItem);
        }
        public static void UpdateRefineryTime(ConquerItem Item)
        {
            UpdateData(Item, "RefineryTime", Item.RefineryTime.Ticks);
        }
        public static void UpdateDurabilityItem(ConquerItem Item)
        {
        }
        public static void UpdateLocation(ConquerItem Item, Client.GameClient client)
        {
            using (var cmd = new MySqlCommand(MySqlCommandType.UPDATE))
                cmd.Update("items").Set("EntityID", client.Entity.UID)
                    .Set("Position", Item.Position).Set("Warehouse", Item.Warehouse)
                    .Where("UID", Item.UID).Execute();
        }
        public static void UpdatePosition(ConquerItem Item)
        {
            using (var cmd = new MySqlCommand(MySqlCommandType.UPDATE))
                cmd.Update("items").Set("Position", Item.Position).Set("Warehouse", Item.Warehouse)
                    .Where("UID", Item.UID).Execute();
        }
        public static void UpdatePlus(ConquerItem Item)
        {
            UpdateData(Item, "Plus", Item.Plus);
        }
        public static void UpdateBound(ConquerItem Item)
        {
            UpdateData(Item, "Bound", 0);
        }
        public static void UpdatePlusProgress(ConquerItem Item)
        {
            UpdateData(Item, "PlusProgress", Item.PlusProgress);
        }
        public static void UpdateItemID(ConquerItem Item, Client.GameClient client)
        {
            UpdateData(Item, "ID", Item.ID);
        }
        public static void RemoveItem(uint UID)
        {
            using (var cmd = new MySqlCommand(MySqlCommandType.UPDATE))
                cmd.Update("items").Set("EntityID", 0)
                    .Set("Position", 0).Where("UID", UID).Execute();
        }
        public static void DeleteItem(uint UID)
        {
            using (var cmd = new MySqlCommand(MySqlCommandType.DELETE))
                cmd.Delete("items", "UID", UID).Execute();
        }
        public static void ClearPosition(uint EntityID, byte position)
        {
            using (var cmd = new MySqlCommand(MySqlCommandType.UPDATE))
                cmd.Update("items").Set("EntityID", 0).Set("Position", 0)
                    .Where("EntityID", EntityID).And("Position", position).Execute();
        }
        public static void RefineryUpdate(ConquerItem Item, Client.GameClient client)
        {
        }

        public static void ClearNulledItems()
        {
            Dictionary<uint, int> dict = new Dictionary<uint, int>();
            using (var c = new MySqlCommand(MySqlCommandType.SELECT).Select("detaineditems"))
            using (var r = c.CreateReader())
                while (r.Read())
                    dict[r.ReadUInt32("ItemUID")] = 0;
            //cmd.Where("UID", r.ReadUInt32("ItemUID")); 

            using (var c = new MySqlCommand(MySqlCommandType.SELECT).Select("claimitems"))
            using (var r = c.CreateReader())
                while (r.Read())
                    dict[r.ReadUInt32("ItemUID")] = 0;
            //cmd.Where("UID", r.ReadUInt32("ItemUID")); 
            var array = dict.Keys.ToArray();
            foreach (var item in array)
                using (var cmd = new MySqlCommand(MySqlCommandType.UPDATE).Update("items")
                    .Set("entityid", 1).Where("entityid", 0).And("uid", item))
                    cmd.Execute();

            using (var cmd = new MySqlCommand(MySqlCommandType.DELETE).Delete("items", "entityid", 0))
                cmd.Execute();
        }  
    }
}
