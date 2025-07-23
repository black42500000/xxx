using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer_Online_Server.Client;
using Conquer_Online_Server.Game;

namespace Conquer_Online_Server.Database
{
    public class PkExpelTable
    {

        public static void Load(GameClient client)
        {
            try
            {
                MySqlCommand command = new MySqlCommand(MySqlCommandType.SELECT);
                command.Select("pk_explorer").Where("uid", (long)client.Account.EntityID);
                MySqlReader reader = new MySqlReader(command);
                while (reader.Read())
                {
                    MrBahaa.PkExpeliate expeliate = new MrBahaa.PkExpeliate
                    {
                        killedUID = reader.ReadUInt32("killed_uid"),
                        Name = reader.ReadString("killed_name"),
                        KilledAt = reader.ReadString("killed_map"),
                        LostExp = reader.ReadUInt32("lost_exp"),
                        Times = reader.ReadUInt32("times"),
                        Potency = reader.ReadUInt32("battle_power"),
                        Level = reader.ReadByte("level")
                    };
                    client.Entity.PkExplorerValues.Add(expeliate.killedUID, expeliate);
                }

            }
            catch (Exception exception)
            {
                Program.SaveException(exception);
            }
        }
        public static void PkExploitAdd(Client.GameClient client, MrBahaa.PkExpeliate pk)
        {
            MySqlCommand cmds = new MySqlCommand(MySqlCommandType.SELECT);
            cmds.Select("pk_explorer");
            MySqlReader rdr = new MySqlReader(cmds);
            {
                MySqlCommand cmd = new MySqlCommand(MySqlCommandType.INSERT);
                cmd.Insert("pk_explorer").Insert("uid", client.Account.EntityID)
                     .Insert("killed_uid", pk.killedUID)
                     .Insert("killed_name", pk.Name)
                     .Insert("killed_map", pk.KilledAt)
                     .Insert("lost_exp", pk.LostExp)
                     .Insert("times", pk.Times)
                     .Insert("battle_power", pk.Potency).Insert("level", pk.Level);
                cmd.Execute();

                client.Entity.PkExplorerValues.Add(pk.killedUID, pk);
            }

        }
        public static void Update(Client.GameClient client, MrBahaa.PkExpeliate pk)
        {

            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
            cmd.Update("pk_explorer")
                 .Set("killed_name", pk.Name)
                 .Set("killed_map", pk.KilledAt)
                 .Set("lost_exp", pk.LostExp)
                 .Set("times", pk.Times)
                 .Set("battle_power", pk.Potency)
                 .Set("level", pk.Level)
                 .Where("uid", client.Account.EntityID).And("killed_uid", pk.killedUID);
            cmd.Execute();
            client.Entity.PkExplorerValues.Remove(pk.killedUID);
            client.Entity.PkExplorerValues.Add(pk.killedUID, pk);

        }
    }
}