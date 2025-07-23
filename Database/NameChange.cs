namespace Conquer_Online_Server.Database
{
    using Conquer_Online_Server;
    using System;
    using System.Collections.Generic;

    public class NameChange
    {
        public static void UpdateNames()
        {
            MySqlCommand command3;
            Dictionary<string, NameChangeC> dictionary = new Dictionary<string, NameChangeC>();
            MySqlReader reader = new MySqlReader(new MySqlCommand(MySqlCommandType.SELECT).Select("entities"));
            string str = "";
            string str2 = "";
            while (reader.Read())
            {
                str = reader.ReadString("namechange");
                str2 = reader.ReadString("name");
                int num = (int)reader.ReadInt64("UID");
                if (str != "")
                {
                    MySqlCommand command2 = null;
                    command2 = new MySqlCommand(MySqlCommandType.UPDATE);
                    command2.Update("apprentice").Set("MentorName", str).Where("MentorID", (long)num).Execute();
                    command2 = new MySqlCommand(MySqlCommandType.UPDATE);
                    command2.Update("apprentice").Set("ApprenticeName", str).Where("ApprenticeID", (long)num).Execute();
                    command2 = new MySqlCommand(MySqlCommandType.UPDATE);
                    command2.Update("elitepk").Set("Name", str).Where("UID", (long)num).Execute();
                    command2 = new MySqlCommand(MySqlCommandType.UPDATE);
                    command2.Update("flags").Set("TopArcher", str).Where("TopArcher", str2).Execute();
                    command2 = new MySqlCommand(MySqlCommandType.UPDATE);
                    command2.Update("flags").Set("TopTrojan", str).Where("TopTrojan", str2).Execute();
                    command2 = new MySqlCommand(MySqlCommandType.UPDATE);
                    command2.Update("flags").Set("TopGuildLeader", str).Where("TopGuildLeader", str2).Execute();
                    command2 = new MySqlCommand(MySqlCommandType.UPDATE);
                    command2.Update("flags").Set("TopNinja", str).Where("TopNinja", str2).Execute();
                    command2 = new MySqlCommand(MySqlCommandType.UPDATE);
                    command2.Update("flags").Set("TopMonk", str).Where("TopMonk", str2).Execute();
                    command2 = new MySqlCommand(MySqlCommandType.UPDATE);
                    command2.Update("flags").Set("TopWarrior", str).Where("TopWarrior", str2).Execute();
                    command2 = new MySqlCommand(MySqlCommandType.UPDATE);
                    command2.Update("flags").Set("TopSpouse", str).Where("TopSpouse", str2).Execute();
                    command2 = new MySqlCommand(MySqlCommandType.UPDATE);
                    command2.Update("flags").Set("TopWaterTaoist", str).Where("TopWaterTaoist", str2).Execute();
                    command2 = new MySqlCommand(MySqlCommandType.UPDATE);
                    command2.Update("flags").Set("TopFireTaoist", str).Where("TopFireTaoist", str2).Execute();
                    command2 = new MySqlCommand(MySqlCommandType.UPDATE);
                    command2.Update("flags").Set("MonthlyPkChampion", str).Where("MonthlyPkChampion", str2).Execute();
                    command2 = new MySqlCommand(MySqlCommandType.UPDATE);
                    command2.Update("flags").Set("WeeklyPkChampion", str).Where("WeeklyPkChampion", str2).Execute();
                    command2 = new MySqlCommand(MySqlCommandType.UPDATE);
                    command2.Update("flags").Set("TopDeputyLeader", str).Where("TopDeputyLeader", str2).Execute();
                    command2 = new MySqlCommand(MySqlCommandType.UPDATE);
                    command2.Update("flags").Set("TopDeputyLeader2", str).Where("TopDeputyLeader2", str2).Execute();
                    command2 = new MySqlCommand(MySqlCommandType.UPDATE);
                    command2.Update("flags").Set("TopDeputyLeader3", str).Where("TopDeputyLeader3", str2).Execute();
                    command2 = new MySqlCommand(MySqlCommandType.UPDATE);
                    command2.Update("flags").Set("TopDeputyLeader4", str).Where("TopDeputyLeader4", str2).Execute();
                    command2 = new MySqlCommand(MySqlCommandType.UPDATE);
                    command2.Update("flags").Set("TopDeputyLeader5", str).Where("TopDeputyLeader5", str2).Execute();
                    command2 = new MySqlCommand(MySqlCommandType.UPDATE);
                    command2.Update("arena").Set("EntityName", str).Where("EntityID", (long)num).Execute();
                    command2 = new MySqlCommand(MySqlCommandType.UPDATE);
                    command2.Update("teamarena").Set("EntityName", str).Where("EntityID", (long)num).Execute();
                    command2 = new MySqlCommand(MySqlCommandType.UPDATE);
                    command2.Update("claimitems").Set("OwnerName", str).Where("OwnerUID", (long)num).Execute();
                    command2 = new MySqlCommand(MySqlCommandType.UPDATE);
                    command2.Update("claimitems").Set("GainerName", str).Where("GainerUID", (long)num).Execute();
                    command2 = new MySqlCommand(MySqlCommandType.UPDATE);
                    command2.Update("detaineditems").Set("OwnerName", str).Where("OwnerUID", (long)num).Execute();
                    command2 = new MySqlCommand(MySqlCommandType.UPDATE);
                    command2.Update("detaineditems").Set("GainerName", str).Where("GainerUID", (long)num).Execute();
                    command2 = new MySqlCommand(MySqlCommandType.UPDATE);
                    command2.Update("enemy").Set("EnemyName", str).Where("EnemyID", (long)num).Execute();
                    command2 = new MySqlCommand(MySqlCommandType.UPDATE);
                    command2.Update("friends").Set("FriendName", str).Where("FriendID", (long)num).Execute();
                    command2 = new MySqlCommand(MySqlCommandType.UPDATE);
                    command2.Update("guilds").Set("Name", str).Where("Name", str2).Execute();
                    command2 = new MySqlCommand(MySqlCommandType.UPDATE);
                    command2.Update("guilds").Set("LeaderName", str).Where("LeaderName", str2).Execute();
                    command2 = new MySqlCommand(MySqlCommandType.UPDATE);
                    command2.Update("clans").Set("Name", str).Where("Name", str2).Execute();
                    command2 = new MySqlCommand(MySqlCommandType.UPDATE);
                    command2.Update("clans").Set("LeaderName", str).Where("LeaderName", str2).Execute();
                    command2 = new MySqlCommand(MySqlCommandType.UPDATE);
                    command2.Update("nobility").Set("EntityName", str).Where("EntityUID", (long)num).Execute();
                    command2 = new MySqlCommand(MySqlCommandType.UPDATE);
                    command2.Update("partners").Set("PartnerName", str).Where("PartnerID", (long)num).Execute();
                    if (!dictionary.ContainsKey(str2))
                    {
                        NameChangeC ec = new NameChangeC
                        {
                            NewName = str,
                            OldName = str2
                        };
                        dictionary.Add(str2, ec);
                    }
                }
            }
            //  Reader.Close();
            ////  Reader.Dispose();
            if (dictionary.Count > 0)
            {
                // Program.ForegroundColor = ConsoleColor.DarkGreen;
                Conquer_Online_Server.Console.WriteLine(" [NAME CHANGES]");
            }
            foreach (NameChangeC ec2 in dictionary.Values)
            {
                command3 = new MySqlCommand(MySqlCommandType.UPDATE);
                command3.Update("entities").Set("Name", ec2.NewName).Set("namechange", "").Where("Name", ec2.OldName).Execute();
                Conquer_Online_Server.Console.WriteLine(" -[" + ec2.OldName + "] : -[" + ec2.NewName + "]");
                // Program.ForegroundColor = ConsoleColor.White;
            }
            foreach (NameChangeC ec2 in dictionary.Values)
            {
                command3 = new MySqlCommand(MySqlCommandType.UPDATE);
                command3.Update("entities").Set("Spouse", ec2.NewName).Where("Spouse", ec2.OldName).Execute();
            }
            dictionary.Clear();
        }
    }
}

