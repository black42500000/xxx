using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer_Online_Server.Database;

namespace Conquer_Online_Server.Game
{
    class NameChanger//Coded By Mido
    {
        public static void MidoUpdateName(Client.GameClient client)
        {

            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("entities");
            Conquer_Online_Server.Database.MySqlReader r = new Conquer_Online_Server.Database.MySqlReader(cmd);
            String name = "";
            while (r.Read())
            {
                //newname = r.ReadString("namechange");//debug make
                name = r.ReadString("name");
                if (name != "")
                {
                    MySqlCommand cmdupdate = null;//lol i see the problem hold on ,,, hold on what? :$ try now
                    cmdupdate = new MySqlCommand(MySqlCommandType.UPDATE);
                    cmdupdate.Update("apprentice").Set("MentorName", client.Entity.NewName).Where("MentorName", name).Execute();

                    cmdupdate = new MySqlCommand(MySqlCommandType.UPDATE);
                    cmdupdate.Update("apprentice").Set("ApprenticeName", client.Entity.NewName).Where("ApprenticeName", name).Execute();

                    cmdupdate = new MySqlCommand(MySqlCommandType.UPDATE);
                    cmdupdate.Update("arena").Set("EntityName", client.Entity.NewName).Where("EntityName", name).Execute();

                    cmdupdate = new MySqlCommand(MySqlCommandType.UPDATE);
                    cmdupdate.Update("claimitems").Set("OwnerName", client.Entity.NewName).Where("OwnerName", name).Execute();

                    cmdupdate = new MySqlCommand(MySqlCommandType.UPDATE);
                    cmdupdate.Update("claimitems").Set("GainerName", client.Entity.NewName).Where("GainerName", name).Execute();

                    cmdupdate = new MySqlCommand(MySqlCommandType.UPDATE);
                    cmdupdate.Update("detaineditems").Set("OwnerName", client.Entity.NewName).Where("OwnerName", name).Execute();

                    cmdupdate = new MySqlCommand(MySqlCommandType.UPDATE);
                    cmdupdate.Update("detaineditems").Set("GainerName", client.Entity.NewName).Where("GainerName", name).Execute();

                    cmdupdate = new MySqlCommand(MySqlCommandType.UPDATE);
                    cmdupdate.Update("enemy").Set("EnemyName", client.Entity.NewName).Where("EnemyName", name).Execute();

                    cmdupdate = new MySqlCommand(MySqlCommandType.UPDATE);
                    cmdupdate.Update("friends").Set("FriendName", client.Entity.NewName).Where("FriendName", name).Execute();

                    cmdupdate = new MySqlCommand(MySqlCommandType.UPDATE);
                    cmdupdate.Update("guilds").Set("Name", client.Entity.NewName).Where("Name", name).Execute();

                    cmdupdate = new MySqlCommand(MySqlCommandType.UPDATE);
                    cmdupdate.Update("guilds").Set("LeaderName", client.Entity.NewName).Where("LeaderName", name).Execute();

                    cmdupdate = new MySqlCommand(MySqlCommandType.UPDATE);
                    cmdupdate.Update("nobility").Set("EntityName", client.Entity.NewName).Where("EntityName", name).Execute();

                    cmdupdate = new MySqlCommand(MySqlCommandType.UPDATE);
                    cmdupdate.Update("partners").Set("PartnerName", client.Entity.NewName).Where("PartnerName", name).Execute();
                    UpdateStaff(client);
                    return;
                }
            }
            r.Close();
            r.Dispose();
        }
        public static void UpdateStaff(Client.GameClient Mido)
        {
            Game.ConquerStructures.Nobility.Board.Clear();
            Database.NobilityTable.Load();
            //////////
            Mido.ClaimableItem.Clear();
            Conquer_Online_Server.Database.ClaimItemTable.LoadClaimableItems(Mido);
            Mido.DeatinedItem.Clear();
            Conquer_Online_Server.Database.DetainedItemTable.LoadDetainedItems(Mido);

            Mido.Partners.Clear();
            Mido.Enemy.Clear();
            Mido.Friends.Clear();
            Mido.Apprentices.Clear();

            Conquer_Online_Server.Database.KnownPersons.LoadEnemy(Mido);
            Conquer_Online_Server.Database.KnownPersons.LoaderFriends(Mido);
            Conquer_Online_Server.Database.KnownPersons.LoadMentor(Mido);
            Conquer_Online_Server.Database.KnownPersons.LoadPartner(Mido);

            foreach (Game.ConquerStructures.Society.TradePartner clients in Mido.Partners.Values)
            {
                if (clients.IsOnline)
                {
                    clients.Client.Partners.Clear();
                    Conquer_Online_Server.Database.KnownPersons.LoadPartner(clients.Client);
                }
            }
            foreach (Game.ConquerStructures.Society.Enemy clients in Mido.Enemy.Values)
            {
                if (clients.IsOnline)
                {
                    clients.Client.Enemy.Clear();
                    Conquer_Online_Server.Database.KnownPersons.LoadEnemy(clients.Client);
                }
            }
            foreach (Game.ConquerStructures.Society.Friend clients in Mido.Friends.Values)
            {
                if (clients.IsOnline)
                {
                    clients.Client.Friends.Clear();
                    Conquer_Online_Server.Database.KnownPersons.LoaderFriends(clients.Client);
                }
            }
            foreach (Game.ConquerStructures.Society.Apprentice clients in Mido.Apprentices.Values)
            {
                if (clients.IsOnline)
                {
                    clients.Client.Apprentices.Clear();
                    Conquer_Online_Server.Database.KnownPersons.LoaderFriends(clients.Client);
                }
            }


            //("Warning! : "+Mido.Entity.Name+" changed his name");
            return;
        }
    }
}  