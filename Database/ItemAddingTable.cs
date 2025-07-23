using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Conquer_Online_Server.Network.GamePackets;
namespace Conquer_Online_Server.Database
{
    public class ItemAddingTable
    {
        public static void GetAddingsForItem(ConquerItem item)
        {
            using (var cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("itemadding").Where("UID", item.UID))
            using (var reader = cmd.CreateReader())
            {
                while (reader.Read())
                {
                    if (reader.ReadInt32("Addingtype") == 0)
                    {
                        ItemAdding.Purification_ purification = new ItemAdding.Purification_();
                        purification.ItemUID = item.UID;
                        purification.Available = true;
                        purification.PurificationItemID = reader.ReadUInt32("Addingid");
                        purification.PurificationDuration = reader.ReadUInt32("Duration");
                        purification.PurificationLevel = reader.ReadUInt32("Addinglevel");
                        purification.AddedOn = DateTime.FromBinary(reader.ReadInt64("Addedon"));
                        if (purification.PurificationDuration != 0)
                        {
                            TimeSpan span1 = new TimeSpan(purification.AddedOn.AddSeconds(purification.PurificationDuration).Ticks);
                            TimeSpan span2 = new TimeSpan(DateTime.Now.Ticks);
                            int secondsleft = (int)(span1.TotalSeconds - span2.TotalSeconds);
                            if (secondsleft <= 0)
                            {
                                purification.Available = false;
                                RemoveAdding(item.UID, purification.PurificationItemID);
                                continue;
                            }
                        }
                        item.Purification = purification;
                    }
                    else
                    {
                        ItemAdding.Refinery_ extraeffect = new ItemAdding.Refinery_();
                        extraeffect.ItemUID = item.UID;
                        extraeffect.Available = true;
                        extraeffect.EffectID = reader.ReadUInt32("Addingid");
                        extraeffect.EffectLevel = reader.ReadUInt32("Addinglevel");
                        extraeffect.EffectPercent = reader.ReadUInt32("Addingpercent");
                        extraeffect.EffectDuration = reader.ReadUInt32("Duration");
                        extraeffect.AddedOn = DateTime.FromBinary(reader.ReadInt64("Addedon"));
                        if (extraeffect.EffectDuration != 0)
                        {
                            TimeSpan span1 = new TimeSpan(extraeffect.AddedOn.AddSeconds(extraeffect.EffectDuration).Ticks);
                            TimeSpan span2 = new TimeSpan(DateTime.Now.Ticks);
                            int secondsleft = (int)(span1.TotalSeconds - span2.TotalSeconds);
                            if (secondsleft <= 0)
                            {
                                extraeffect.Available = false;
                                RemoveAdding(item.UID, extraeffect.EffectID);
                                continue;
                            }
                        }
                        item.ExtraEffect = extraeffect;
                    }
                }
            }
        }
        public static void RemoveAdding(uint UID, uint addingid)
        {
            using (var cmd = new MySqlCommand(MySqlCommandType.DELETE))
                cmd.Delete("itemadding", "UID", UID).And("addingid", addingid)
                    .Execute();
        }
        public static void AddPurification(ItemAdding.Purification_ item)
        {
            using (var cmd = new MySqlCommand(MySqlCommandType.INSERT))
                cmd.Insert("itemadding").Insert("UID", item.ItemUID)
                    .Insert("addingtype", 0).Insert("addingid", item.PurificationItemID)
                    .Insert("addinglevel", item.PurificationLevel).Insert("addingpercent", 0)
                    .Insert("duration", item.PurificationDuration).Insert("addedon", item.AddedOn.Ticks)
                    .Execute();
        }
        public static void Stabilize(uint UID, uint addingid)
        {
            using (var cmd = new MySqlCommand(MySqlCommandType.UPDATE).Update("itemadding"))
                cmd.Set("duration", 0).Where("UID", UID).And("addingid", addingid)
                    .Execute();
        }
        public static void StabilizeRefinery(uint UID, uint addingid)
        {
            using (var cmd = new MySqlCommand(MySqlCommandType.UPDATE).Update("itemadding"))
                cmd.Set("duration", 0).Where("UID", UID).And("addingid", addingid)
                    .Execute();
        }
        public static void AddExtraEffect(ItemAdding.Refinery_ effect)
        {
            using (var cmd = new MySqlCommand(MySqlCommandType.INSERT))
                cmd.Insert("itemadding").Insert("UID", effect.ItemUID)
                    .Insert("addingtype", 1).Insert("addingid", effect.EffectID)
                    .Insert("addinglevel", effect.EffectLevel).Insert("addingpercent", effect.EffectPercent)
                    .Insert("duration", effect.EffectDuration).Insert("addedon", effect.AddedOn.Ticks)
                    .Execute();

        }
    }
}