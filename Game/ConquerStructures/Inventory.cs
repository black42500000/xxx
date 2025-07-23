using System;
using System.Linq;
using System.Collections.Generic;
using Conquer_Online_Server.Network.GamePackets;

namespace Conquer_Online_Server.Game.ConquerStructures
{
    public class Inventory
    {
        Dictionary<uint, ConquerItem> inventory;
        ConquerItem[] objects;
        Client.GameClient Owner;
        public Inventory(Client.GameClient client)
        {
            Owner = client;
            inventory = new Dictionary<uint, ConquerItem>(40);
            objects = new ConquerItem[0];
        }
        public bool Add(uint id, byte plus, byte times, bool bound = false)
        {
            try
            {
                Database.ConquerItemInformation infos = new Database.ConquerItemInformation(id, plus);
                while (times > 0)
                {
                    if (Count <= 39)
                    {
                        ConquerItem item = new Network.GamePackets.ConquerItem(true);
                        item.ID = id;
                        item.Plus = plus;
                        item.Bound = false;
                        item.Durability = item.MaximDurability = infos.BaseInformation.Durability;
                        item.StackSize = 1;
                        item.MaxStackSize = infos.BaseInformation.StackSize;
                        Add(item, Enums.ItemUse.CreateAndAdd);
                    }
                    else
                    {
                        return false;
                    }
                    times--;
                }
            }
            catch (Exception )
            {
                //Program.SaveException(e);
            }
            return true;
        }
        public bool AddBound(uint id, byte plus, byte times)
        {
            return Add(id, plus, times, true);
        }
        public bool Add(uint id, Game.Enums.ItemEffect effect)
        {
            try
            {
                ConquerItem item = new Network.GamePackets.ConquerItem(true);
                item.ID = id;
                item.Effect = effect;
                Database.ConquerItemInformation infos = new Database.ConquerItemInformation(id, 0);
                item.Durability = item.MaximDurability = infos.BaseInformation.Durability;
                if (Count <= 39)
                {
                    Add(item, Enums.ItemUse.CreateAndAdd);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception )
            {
                //Program.SaveException(e);
            }
            return true;
        }
        public bool Add(ConquerItem item, Enums.ItemUse use)
        {
            try
            {
                if (!Database.ConquerItemInformation.BaseInformations.ContainsKey(item.ID))
                    return true;
                if (Count == 40)
                {
                    Owner.Send(Constants.FullInventory);
                    return false;
                }
                if (!inventory.ContainsKey(item.UID))
                {
                    item.Position = 0;
                    ConquerItem _ExistingItem;
                    Database.ConquerItemInformation iteminfo = new Database.ConquerItemInformation(item.ID, 0);
                    if (Owner.Inventory.Contains(iteminfo.BaseInformation.ID, iteminfo.BaseInformation.StackSize, out _ExistingItem) && Owner.SpiltStack && use != Enums.ItemUse.None)
                    {

                        if (_ExistingItem.StackSize == 0)
                            _ExistingItem.StackSize = 1;
                        ushort _StackCount = iteminfo.BaseInformation.StackSize;
                        _StackCount -= (ushort)_ExistingItem.StackSize;
                        if (_StackCount >= 1)
                            _StackCount += 1;
                        _ExistingItem.StackSize += 1;
                        Database.ConquerItemTable.UpdateStack(_ExistingItem);
                        _ExistingItem.Mode = Game.Enums.ItemMode.Update;
                        _ExistingItem.Send(Owner);
                        _ExistingItem.Mode = Game.Enums.ItemMode.Default;
                        switch (use)
                        {

                            case Enums.ItemUse.Add:
                                Database.ConquerItemTable.DeleteItem(item.UID);
                                break;
                            case Enums.ItemUse.Move:
                                Database.ConquerItemTable.DeleteItem(item.UID);
                                break;
                        }
                        return true;
                    }
                    else
                    {
                        switch (use)
                        {
                            case Enums.ItemUse.CreateAndAdd:
                                item.UID = Conquer_Online_Server.Network.GamePackets.ConquerItem.ItemUID.Next;
                                Database.ConquerItemTable.AddItem(ref item, Owner);
                                item.MobDropped = false;
                                break;
                            case Enums.ItemUse.Add:
                                Database.ConquerItemTable.UpdateLocation(item, Owner);
                                break;
                            case Enums.ItemUse.Move:
                                item.Position = 0;
                                item.StatsLoaded = false;
                                Database.ConquerItemTable.UpdateLocation(item, Owner);
                                break;
                        }
                        if (use != Enums.ItemUse.None)
                            Database.ItemLog.LogItem(item.UID, Owner.Entity.UID, Conquer_Online_Server.Database.ItemLog.ItemLogAction.Add);
                        inventory.Add(item.UID, item);
                        objects = inventory.Values.ToArray();
                        item.Mode = Enums.ItemMode.Default;
                        if (use != Enums.ItemUse.None)
                            item.Send(Owner);
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                Program.SaveException(e);
            }
            return false;
        }
        public void Update()
        {
            objects = inventory.Values.ToArray();
        }
        public bool Remove(ConquerItem item, Enums.ItemUse use)
        {
            try
            {
                if (inventory.ContainsKey(item.UID))
                {
                    if (Owner.Entity.UseItem && item.StackSize > 1)
                    {
                        Remove(item.ID, 1);
                        Owner.Entity.UseItem = false;
                        return true;
                    }
                    else
                    {
                        switch (use)
                        {
                            case Enums.ItemUse.Remove: Database.ConquerItemTable.RemoveItem(item.UID); break;
                            case Enums.ItemUse.Move: Database.ConquerItemTable.UpdateLocation(item, Owner); break;
                        }
                        Database.ItemLog.LogItem(item.UID, Owner.Entity.UID, Conquer_Online_Server.Database.ItemLog.ItemLogAction.Remove);

                        inventory.Remove(item.UID);
                        objects = inventory.Values.ToArray();
                        Network.GamePackets.ItemUsage iu = new Network.GamePackets.ItemUsage(true);
                        iu.UID = item.UID;
                        iu.ID = Network.GamePackets.ItemUsage.RemoveInventory;
                        Owner.Send(iu);
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                Program.SaveException(e);
            }
            return false;
        }
        public bool Remove(ConquerItem item, Enums.ItemUse use, bool equipment)
        {
            try
            {
                if (inventory.ContainsKey(item.UID))
                {
                    inventory.Remove(item.UID);
                    objects = inventory.Values.ToArray();
                    Network.GamePackets.ItemUsage iu = new Network.GamePackets.ItemUsage(true);
                    iu.UID = item.UID;
                    iu.ID = Network.GamePackets.ItemUsage.RemoveInventory;
                    Owner.Send(iu);
                    return true;
                }
            }
            catch (Exception )
            {
                //Program.SaveException(e);
            }
            return false;
        }
        public bool Remove(uint UID, Enums.ItemUse use, bool sendRemove)
        {
            try
            {
                if (inventory.ContainsKey(UID))
                {
                    switch (use)
                    {
                        case Enums.ItemUse.Remove: Database.ConquerItemTable.RemoveItem(UID); break;
                        case Enums.ItemUse.Move: Database.ConquerItemTable.UpdateLocation(inventory[UID], Owner); break;
                    }
                    Database.ItemLog.LogItem(UID, Owner.Entity.UID, Conquer_Online_Server.Database.ItemLog.ItemLogAction.Remove);
                    inventory.Remove(UID);
                    objects = inventory.Values.ToArray();
                    if (sendRemove)
                    {
                        Network.GamePackets.ItemUsage iu = new Network.GamePackets.ItemUsage(true);
                        iu.UID = UID;
                        iu.ID = Network.GamePackets.ItemUsage.RemoveInventory;
                        Owner.Send(iu);
                    }
                    return true;
                }
            }
            catch (Exception )
            {
                //Program.SaveException(e);
            }
            return false;
        }
        public bool Remove(string name)
        {
            try
            {
                string loweredName = name.ToLower();
                foreach (var item in inventory.Values)
                {
                    if (Database.ConquerItemInformation.BaseInformations[item.ID].LoweredName == loweredName)
                    {
                        Database.ItemLog.LogItem(item.UID, Owner.Entity.UID, Conquer_Online_Server.Database.ItemLog.ItemLogAction.Remove);
                        Remove(item, Enums.ItemUse.Remove);
                        Network.GamePackets.ItemUsage iu = new Network.GamePackets.ItemUsage(true);
                        iu.UID = item.UID;
                        iu.ID = Network.GamePackets.ItemUsage.RemoveInventory;
                        Owner.Send(iu);
                        return true;
                    }
                }
            }
            catch (Exception )
            {
                //Program.SaveException(e);
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
        public byte Count { get { return (byte)Objects.Length; } }

        public bool TryGetItem(uint UID, out ConquerItem item)
        {
            return inventory.TryGetValue(UID, out item);
        }

        public bool ContainsUID(uint UID)
        {
            return inventory.ContainsKey(UID);
        }
        public bool Contains(uint ID, ushort maxstackamount, out ConquerItem Item)
        {
            Item = null;
            if (ID == 0)
                return false;
            foreach (ConquerItem item in Objects)
            {
                if (item.ID == ID && item.StackSize < maxstackamount)
                {
                    Item = item;
                    return true;
                }
            }
            return false;
        }
        public bool Contains(uint ID, ushort times)
        {
            if (ID == 0)
                return true;
            ushort has = 0;
            foreach (ConquerItem item in Objects)
                if (item.ID == ID)
                {
                    if (item.StackSize == 0)
                        has++;
                    else
                        has += (byte)item.StackSize;
                }
            return has >= times;
        }
        public ConquerItem GetItemByID(uint ID)
        {
            foreach (ConquerItem item in Objects)
                if (item.ID == ID)
                    return item;
            return null;
        }
        public bool Remove(uint ID, byte times)
        {
            if (ID == 0)
                return true;
            List<ConquerItem> items = new List<ConquerItem>();
            byte has = 0;
            foreach (ConquerItem item in Objects)
                if (item.ID == ID)
                {
                    if (item.StackSize > 1)
                    {
                        if (item.StackSize >= times)
                        {
                            item.StackSize -= times;
                            if (item.StackSize != 0)
                            {
                                Database.ConquerItemTable.UpdateStack(item);
                                item.Mode = Enums.ItemMode.Update;
                                item.Send(Owner);
                                item.Mode = Enums.ItemMode.Default;
                                return true;
                            }
                            else
                            {
                                Database.ConquerItemTable.DeleteItem(item.UID);
                                inventory.Remove(item.UID);
                                objects = inventory.Values.ToArray();
                                Network.GamePackets.ItemUsage iu = new Network.GamePackets.ItemUsage(true);
                                iu.UID = item.UID;
                                iu.ID = Network.GamePackets.ItemUsage.RemoveInventory;
                                Owner.Send(iu);
                                return true;
                            }
                        }
                        else
                        {
                            has += (byte)item.StackSize; items.Add(item); if (has >= times) break;
                        }
                    }
                    else
                    {
                        has++; items.Add(item); if (has >= times) break;
                    }
                }
            if (has >= times)
                foreach (ConquerItem item in items)
                    Remove(item, Enums.ItemUse.Remove);
            return has >= times;
        }


        public bool TryGetValue(uint UID, out ConquerItem Info)
        {
            Info = null;
            lock (inventory)
            {
                if (inventory.ContainsKey(UID))
                { return inventory.TryGetValue(UID, out Info); }
                else
                    return false;
            }
        }
        public bool AddSoul(uint id, uint soulitem, uint purfylevel, uint timeofpurfy
       , byte plus, byte times, bool purfystabliz = false, bool bound = false)
        {
            try
            {
                Database.ConquerItemInformation infos = new Database.ConquerItemInformation(id, plus);
                while (times > 0)
                {
                    if (Count <= 39)
                    {
                        ConquerItem item = new Network.GamePackets.ConquerItem(true);
                        #region Stacksize
                        if (infos.BaseInformation.StackSize > 1)
                        {
                            //item.StackSize = (byte)times;                                
                            ushort _StackCount = infos.BaseInformation.StackSize;
                            if (times <= infos.BaseInformation.StackSize)
                                _StackCount = (ushort)times;
                            item.StackSize = (ushort)_StackCount;
                            Database.ConquerItemTable.UpdateStack(item);
                            times -= (byte)_StackCount;
                        }
                        else
                        {
                            item = new ConquerItem(true);
                            item.StackSize = 1;
                            times--;
                        }
                        #endregion Stacksize
                        item.ID = id;
                        item.Plus = plus;
                        item.Bound = false;
                        item.Durability = item.MaximDurability = infos.BaseInformation.Durability;
                        item.StackSize = 1;
                        item.MaxStackSize = infos.BaseInformation.StackSize;
                        item.SocketOne = Game.Enums.Gem.SuperDragonGem;
                        item.SocketTwo = Game.Enums.Gem.SuperDragonGem;
                        item.Enchant = 255;
                        item.Bless = 7;
                        Add(item, Enums.ItemUse.CreateAndAdd);
                        if (purfystabliz == false)
                        {
                            #region purfy without stablize
                            ItemAdding.Purification_ purify = new ItemAdding.Purification_();
                            purify.AddedOn = DateTime.Now;
                            purify.Available = true;
                            purify.ItemUID = item.UID;
                            purify.PurificationLevel = purfylevel;
                            purify.PurificationDuration = timeofpurfy * 24 * 60 * 60;
                            purify.PurificationItemID = soulitem;
                            Database.ItemAddingTable.AddPurification(purify);
                            item.Purification = purify;
                            item.Mode = Conquer_Online_Server.Game.Enums.ItemMode.Update;
                            item.Send(Owner);
                            ItemAdding effect = new ItemAdding(true);
                            effect.Type = ItemAdding.PurificationEffect;
                            effect.Append2(purify);
                            Owner.Send(effect);
                            #endregion
                        }
                        else
                        {
                            #region purfy with stabliz
                            ItemAdding.Purification_ purify = new ItemAdding.Purification_();
                            purify.AddedOn = DateTime.Now;
                            purify.Available = true;
                            purify.ItemUID = item.UID;
                            purify.PurificationLevel = purfylevel;
                            purify.PurificationDuration = timeofpurfy * 24 * 60 * 60;
                            purify.PurificationItemID = soulitem;
                            Database.ItemAddingTable.AddPurification(purify);
                            item.Purification = purify;
                            item.Mode = Conquer_Online_Server.Game.Enums.ItemMode.Update;
                            item.Send(Owner);
                            ItemAdding effect = new ItemAdding(true);
                            effect.Type = ItemAdding.PurificationEffect;
                            effect.Append2(purify);
                            Owner.Send(effect);
                            var Backup = item.Purification;
                            Backup.PurificationDuration = 0;
                            item.Purification = Backup;
                            item.Send(Owner);
                            effect.Type = ItemAdding.StabilizationEffect;
                            effect.Append2(Backup);
                            Owner.Send(effect);
                            Database.ItemAddingTable.Stabilize(item.UID, Backup.PurificationItemID);
                            #endregion
                        }
                    }
                    else
                    {
                        return false;
                    }

                }
                times--;
            }
            catch (Exception e)
            {
                Program.SaveException(e);
            }
            return true;
        }  
        public bool GetStackContainer(uint ID, ushort maxStack, int amount, out ConquerItem Item)
        {
            Item = null;
            if (ID == 0) return false;
            foreach (ConquerItem item in Objects)
            {
                if (item.ID == ID)
                {
                    if (item.StackSize + amount <= maxStack)
                    {
                        Item = item;
                        return true;
                    }
                }
            }
            return false;
        }

        internal void StfDragon(int p, int p_2, int p_3)
        {
            throw new NotImplementedException();
        }
    }
}