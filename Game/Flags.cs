using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer_Online_Server.Database;
using Conquer_Online_Server.Network.GamePackets;

namespace Conquer_Online_Server.Game
{
    class Flags
    {
        public static string TopArcher = "";
        public static string TopWarrior = "";
        public static string TopNinja = "";
        public static string TopMonk = "";
        public static string TopTrojan = "";
        public static string TopWaterTaoist = "";
        public static string TopFireTaoist = "";
        public static string TopPirate = "";
        public static string TopConquer = "";
        public static string TopChampion = "";
        public static string TopMaster = "";
        public static string TopLastMan = "";
        public static string TopClassPk = "";
        public static string TopMember = "";
        public static string TopDeadWorld = "";
        public static string TopGentle = "";
        public static string TopCrazy = "";
        public static string TopButcher = "";
        public static string TopHorse = "";
        public static string ToponeMan = "";
        public static string TopGuildLeader = "";
        public static string TopDeputyLeader = "";
        public static string TopDeputyLeader2 = "";
        public static string TopDeputyLeader3 = "";
        public static string TopDeputyLeader4 = "";
        public static string TopDeputyLeader5 = "";
        public static string TopDeputyLeader6 = "";
        public static string TopDeputyLeader7 = "";
        public static string TopDeputyLeader8 = "";
        public static string Top2Archer = "";
        public static string MonthlyPKChampion = "";
        public static string WeeklyPKChampion = "";

        public static string TopSpouse = "";

        public static void LoadFlags()
        {

            using (var cmd = new MySqlCommand(MySqlCommandType.SELECT))
            {
                cmd.Select("flags");
                using (MySqlReader r = new MySqlReader(cmd))

                    while (r.Read())
                    {
                        TopSpouse = r.ReadString("TopSpouse");
                        TopArcher = r.ReadString("TopArcher");
                        TopWarrior = r.ReadString("TopWarrior");
                        TopNinja = r.ReadString("TopNinja");
                        TopWaterTaoist = r.ReadString("TopWaterTaoist");
                        TopFireTaoist = r.ReadString("TopFireTaoist");
                        TopTrojan = r.ReadString("TopTrojan");
                        TopGuildLeader = r.ReadString("TopGuildLeader");
                        TopDeputyLeader = r.ReadString("TopDeputyLeader");
                        WeeklyPKChampion = r.ReadString("WeeklyPKChampion");
                        MonthlyPKChampion = r.ReadString("MonthlyPKChampion");
                        TopMonk = r.ReadString("TopMonk");
                        Top2Archer = r.ReadString("Top2Archer");
                        TopDeputyLeader2 = r.ReadString("TopDeputyLeader2");
                        TopDeputyLeader3 = r.ReadString("TopDeputyLeader3");
                        TopDeputyLeader4 = r.ReadString("TopDeputyLeader4");
                        TopDeputyLeader5 = r.ReadString("TopDeputyLeader5");
                        TopDeputyLeader6 = r.ReadString("TopDeputyLeader6");
                        TopDeputyLeader7 = r.ReadString("TopDeputyLeader7");
                        TopDeputyLeader8 = r.ReadString("TopDeputyLeader8");

                        TopPirate = r.ReadString("TopPirate");
                        TopChampion = r.ReadString("TopChampion");
                        TopLastMan = r.ReadString("TopLastMan");
                        TopMaster = r.ReadString("TopMaster");
                        TopButcher = r.ReadString("TopButcher");
                        TopCrazy = r.ReadString("TopCrazy");
                        TopGentle = r.ReadString("TopGentle");
                        TopConquer = r.ReadString("TopConquer");
                        TopMember = r.ReadString("TopMember");
                        TopClassPk = r.ReadString("TopClassPk");
                        TopDeadWorld = r.ReadString("TopDeadWorld");
                        TopHorse = r.ReadString("TopHorse");
                        ToponeMan = r.ReadString("1stMan");

                    }
                //r.Close();
                //r.Dispose();
            }
        }
        public static void AddTopTrojan(Client.GameClient client)
        {

            new MySqlCommand(MySqlCommandType.UPDATE).Update("flags").Set("TopTrojan", client.Entity.Name).Execute();
            TopTrojan = client.Entity.Name;
            return;
        }
        public static void AddTopWarrior(Client.GameClient client)
        {
            new MySqlCommand(MySqlCommandType.UPDATE).Update("flags").Set("TopWarrior", client.Entity.Name).Execute();
            TopWarrior = client.Entity.Name;
            return;
        }
        public static void AddTopLastMan(Client.GameClient client)
        {
            new MySqlCommand(MySqlCommandType.UPDATE).Update("flags").Set("TopLastMan", client.Entity.Name).Execute();
            TopWarrior = client.Entity.Name;
            return;
        }
        public static void AddTopChampion(Client.GameClient client)
        {
            new MySqlCommand(MySqlCommandType.UPDATE).Update("flags").Set("TopChampion", client.Entity.Name).Execute();
            TopChampion = client.Entity.Name;
            return;
        }
        public static void Add2Archer(Client.GameClient client)
        {
            new MySqlCommand(MySqlCommandType.UPDATE).Update("flags").Set("Top2Archer", client.Entity.Name).Execute();
            Top2Archer = client.Entity.Name;
        }
        public static void AddTopArcher(Client.GameClient client)
        {
            new MySqlCommand(MySqlCommandType.UPDATE).Update("flags").Set("TopArcher", client.Entity.Name).Execute();
            TopArcher = client.Entity.Name;
            return;
        }
        public static void AddTopNinja(Client.GameClient client)
        {
            new MySqlCommand(MySqlCommandType.UPDATE).Update("flags").Set("TopNinja", client.Entity.Name).Execute();
            TopNinja = client.Entity.Name;

            return;
        }
        public static void AddTopMonk(Client.GameClient client)
        {
            new MySqlCommand(MySqlCommandType.UPDATE).Update("flags").Set("TopMonk", client.Entity.Name).Execute();
            TopMonk = client.Entity.Name;
            return;
        }
        public static void AddTopWater(Client.GameClient client)
        {
            new MySqlCommand(MySqlCommandType.UPDATE).Update("flags").Set("TopWaterTaoist", client.Entity.Name).Execute();
            TopWaterTaoist = client.Entity.Name;

            return;
        }
        public static void AddTopFire(Client.GameClient client)
        {
            new MySqlCommand(MySqlCommandType.UPDATE).Update("flags").Set("TopFireTaoist", client.Entity.Name).Execute();
            TopFireTaoist = client.Entity.Name;
            return;
        }
        public static void AddTopPirate(Client.GameClient client)
        {
            new MySqlCommand(MySqlCommandType.UPDATE).Update("flags").Set("TopPirate", client.Entity.Name).Execute();
            TopPirate = client.Entity.Name;
            return;
        }
        public static void AddTopMaster(Client.GameClient client)
        {
            new MySqlCommand(MySqlCommandType.UPDATE).Update("flags").Set("TopMaster", client.Entity.Name).Execute();
            TopMaster = client.Entity.Name;
            return;
        }
        public static void AddTopMember(Client.GameClient client)
        {
            new MySqlCommand(MySqlCommandType.UPDATE).Update("flags").Set("TopMember", client.Entity.Name).Execute();
            TopMember = client.Entity.Name;
            return;
        }
        public static void AddTopGentle(Client.GameClient client)
        {
            new MySqlCommand(MySqlCommandType.UPDATE).Update("flags").Set("TopGentle", client.Entity.Name).Execute();
            TopMember = client.Entity.Name;
            return;
        }
        public static void AddTopCrazy(Client.GameClient client)
        {
            new MySqlCommand(MySqlCommandType.UPDATE).Update("flags").Set("TopCrazy", client.Entity.Name).Execute();
            TopMember = client.Entity.Name;
            return;
        }
        public static void AddTopButcher(Client.GameClient client)
        {
            new MySqlCommand(MySqlCommandType.UPDATE).Update("flags").Set("TopButcher", client.Entity.Name).Execute();
            TopMember = client.Entity.Name;
            return;
        }
        public static void AddTopDead(Client.GameClient client)
        {
            new MySqlCommand(MySqlCommandType.UPDATE).Update("flags").Set("TopDeadWorld", client.Entity.Name).Execute();
            TopDeadWorld = client.Entity.Name;
            return;
        }
        public static void AddTopHorse(Client.GameClient client)
        {
            new MySqlCommand(MySqlCommandType.UPDATE).Update("flags").Set("TopHorse", client.Entity.Name).Execute();
            TopHorse = client.Entity.Name;
            return;
        }
        public static void AddToponeMan(Client.GameClient client)
        {
            new MySqlCommand(MySqlCommandType.UPDATE).Update("flags").Set("1stMan", client.Entity.Name).Execute();
            ToponeMan = client.Entity.Name;
            return;
        }
        public static void AddTopClassPk(Client.GameClient client)
        {
            new MySqlCommand(MySqlCommandType.UPDATE).Update("flags").Set("TopClassPk", client.Entity.Name).Execute();
            TopClassPk = client.Entity.Name;
            return;
        }
        public static void AddTopConquer(Client.GameClient client)
        {
            new MySqlCommand(MySqlCommandType.UPDATE).Update("flags").Set("TopConquer", client.Entity.Name).Execute();
            TopConquer = client.Entity.Name;
            return;
        }
        public static void AddGuildLeader(Client.GameClient client)
        {
            new MySqlCommand(MySqlCommandType.UPDATE).Update("flags").Set("TopGuildLeader", client.Entity.Name).Execute();
            TopGuildLeader = client.Entity.Name;
            return;
        }
        public static void AddGuildDeaputy(Client.GameClient client)
        {
            new MySqlCommand(MySqlCommandType.UPDATE).Update("flags").Set("TopDeputyLeader", client.Entity.Name).Execute();
            TopDeputyLeader = client.Entity.Name;
            return;
        }
        public static void AddWeekly(Client.GameClient client)
        {
            new MySqlCommand(MySqlCommandType.UPDATE).Update("flags").Set("WeeklyPKChampion", client.Entity.Name).Execute();
            WeeklyPKChampion = client.Entity.Name;
            return;
        }
        public static void AddMonthly(Client.GameClient client)
        {
            new MySqlCommand(MySqlCommandType.UPDATE).Update("flags").Set("MonthlyPKChampion", client.Entity.Name).Execute();
            MonthlyPKChampion = client.Entity.Name;
            return;
        }
        public static void AddSpouse(Client.GameClient client)
        {
            new MySqlCommand(MySqlCommandType.UPDATE).Update("flags").Set("TopSpouse", client.Entity.Name).Execute();
            TopSpouse = client.Entity.Name;
            return;
        }
        public static void AddGuildDeaputy2(Client.GameClient client)
        {
            new MySqlCommand(MySqlCommandType.UPDATE).Update("flags").Set("TopDeputyLeader2", client.Entity.Name).Execute();
            TopDeputyLeader2 = client.Entity.Name;
            return;
        }
        public static void AddGuildDeaputy3(Client.GameClient client)
        {
            new MySqlCommand(MySqlCommandType.UPDATE).Update("flags").Set("TopDeputyLeader3", client.Entity.Name).Execute();
            TopDeputyLeader3 = client.Entity.Name;
            return;
        }
        public static void AddGuildDeaputy4(Client.GameClient client)
        {
            new MySqlCommand(MySqlCommandType.UPDATE).Update("flags").Set("TopDeputyLeader4", client.Entity.Name).Execute();
            TopDeputyLeader4 = client.Entity.Name;
            return;
        }
        public static void AddGuildDeaputy6(Client.GameClient client)
        {
            new MySqlCommand(MySqlCommandType.UPDATE).Update("flags").Set("TopDeputyLeader6", client.Entity.Name).Execute();
            TopDeputyLeader6 = client.Entity.Name;
            return;
        }
        public static void AddGuildDeaputy7(Client.GameClient client)
        {
            new MySqlCommand(MySqlCommandType.UPDATE).Update("flags").Set("TopDeputyLeader7", client.Entity.Name).Execute();
            TopDeputyLeader7 = client.Entity.Name;
            return;
        }
        public static void AddGuildDeaputy8(Client.GameClient client)
        {
            new MySqlCommand(MySqlCommandType.UPDATE).Update("flags").Set("TopDeputyLeader8", client.Entity.Name).Execute();
            TopDeputyLeader8 = client.Entity.Name;
            return;
        }
        public static void AddGuildDeaputy5(Client.GameClient client)
        {
            new MySqlCommand(MySqlCommandType.UPDATE).Update("flags").Set("TopDeputyLeader5", client.Entity.Name).Execute();
            TopDeputyLeader5 = client.Entity.Name;
            return;
        }

    }
}
