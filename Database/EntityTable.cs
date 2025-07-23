using System;
using System.IO;
using System.Text;

using System.Collections.Generic;
using Conquer_Online_Server.Network.GamePackets;
using System.Collections.Concurrent;
using Conquer_Online_Server.Game;
namespace Conquer_Online_Server.Database
{
    public static class EntityTable
    {
        public static void LoadPlayersVots()
        {

            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("VoteIp");
            Database.MySqlReader d = new Database.MySqlReader(cmd);
            while (d.Read())
            {
                MrBahaa.PlayersVot Vot = new MrBahaa.PlayersVot();
                Vot.Uid = d.ReadUInt32("ID");
                Vot.AdressIp = d.ReadString("IP");
                if (!Kernel.VotePool.ContainsKey(Vot.AdressIp))
                    Kernel.VotePool.Add(Vot.AdressIp, Vot);
                if (!Kernel.VotePoolUid.ContainsKey(Vot.Uid))
                    Kernel.VotePoolUid.Add(Vot.Uid, Vot);

            }
            d.Close();
            d.Dispose();
        }
        public static void DeletVotes(MrBahaa.PlayersVot PlayerVot)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.DELETE).Delete("VoteIp", "ID", PlayerVot.Uid).And("IP", PlayerVot.AdressIp);
            cmd.Execute();
        }
        public static void DeletVotes()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.DELETE).Delete("VoteIp", "delete", "1");
            cmd.Execute();
        }
        public static void SavePlayersVot(MrBahaa.PlayersVot PlayerVot)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.INSERT);
            cmd.Insert("VoteIp").Insert("ID", PlayerVot.Uid).Insert("IP", PlayerVot.AdressIp).Execute();

        }
        public static void SaveTopDonation(Client.GameClient client)
        {

            MySqlCommand cmds = new MySqlCommand(MySqlCommandType.DELETE);
            cmds.Delete("topdonation", "EntityID", client.Entity.UID).Execute();

            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.INSERT);
            //Program.WriteLine("New insert to TopDonation  [" + client.Entity.Name + "]");
            cmd.Insert("TopDonation")
           .Insert("EntityID", client.Entity.UID)
           .Insert("Name", client.Entity.Name)
           .Insert("TopKing", client.Entity.TopKing)
           .Insert("TopPrince", client.Entity.TopPrince)
           .Insert("TopRedname", client.Entity.TopRedname)
           .Insert("TopWithename", client.Entity.TopWithename)
           .Insert("TopBlackname", client.Entity.TopBlackname)
           .Insert("TopDucke", client.Entity.TopDucke)
           ;
            cmd.Execute();
        }
        public static bool LoadTopDonation(Client.GameClient client)
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT);
            cmd.Select("topdonation").Where("Name", client.Entity.Name).And("EntityID", client.Entity.UID);
            MySqlReader r = new MySqlReader(cmd);
            if (r.Read())
            {
                //client.Entity.Name = r.ReadString("Name");
                client.Entity.TopKing = r.ReadUInt16("TopKing");
                client.Entity.TopRedname = r.ReadUInt16("TopRedname");
                client.Entity.TopWithename = r.ReadUInt16("TopWithename");
                client.Entity.TopBlackname = r.ReadUInt16("TopBlackname");
                client.Entity.TopPrince = r.ReadUInt16("TopPrince");
                client.Entity.TopDucke = r.ReadUInt16("TopDucke");
                client.Entity.TopKinght = r.ReadUInt16("TopKinght");
                client.Entity.TopEarl = r.ReadUInt16("TopEarl");
                client.Entity.TopBaron = r.ReadUInt16("TopBaron");
            }
            else
            {
                SaveTopDonation(client);
                return false;
                //
            }
            return true;
        }
        public static void DeleteTopDonation(Client.GameClient C)
        {
            MySqlCommand cmdd = new MySqlCommand(MySqlCommandType.UPDATE);
            int ress = cmdd.Update("topdonation")
                //.Set("UID", 0)
                .Set("TopKing", 0)
                .Set("TopPrince", 0)
                .Set("TopDucke", 0)
                .Set("TopKinght", 0)
                .Set("TopEarl", 0)
                .Set("TopBaron", 0)
                .Set("TopBlackname", 0)
                .Set("TopRedname", 0)
                .Set("TopWithename", 0)
                .Execute();
            //programe.WriteLine50("DeleteTopDonation halo :)");
        }
        /*public static bool LoadAchievement(Client.GameClient client)
        {
            Database.MySqlCommand command = new Database.MySqlCommand(MySqlCommandType.SELECT);
            command.Select("achievement").Where("UID", (long)client.Account.EntityID);
            MySqlReader reader = new MySqlReader(command);
            if (reader.Read())
            {
                client.Entity.MyAchievement = new Game.Achievement(client.Entity);
                client.Entity.MyAchievement.Load(reader.ReadString("Achievement"));
            }
            else
            {
                Database.MySqlCommand cmd = new Database.MySqlCommand(MySqlCommandType.INSERT);
                Conquer_Online_Server.Console.WriteLine("New insert to Achievement Table  [" + client.Entity.Name + "]");
                cmd.Insert("Achievement").Insert("UID", (long)client.Entity.UID).Insert("Owner", client.Account.Username).Insert("Name", client.Entity.Name);
                cmd.Execute();
                client.Entity.MyAchievement = new Game.Achievement(client.Entity);
            }
            reader.Close();
            reader.Dispose();
            return true;
        }*/
        public static bool LoadEntity(Client.GameClient client)
        {
            using (var cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("entities").Where("UID", client.Account.EntityID))
            using (var reader = new MySqlReader(cmd))
            {
                if (reader.Read())
                {
                    Conquer_Online_Server.Game.AttJiangHu hu;
                    client.WarehousePW = reader.ReadUInt32("WarehousePW");
                    client.Entity = new Game.Entity(Game.EntityFlag.Player, false);
                    if (client.IsBot == false)
                    {
                        client.Entity.Name = reader.ReadString("Name");
                    }
                    else
                    {
                        client.Entity.Name = reader.ReadString("Name") + "[Bot]";
                    }
                    client.HeadgearClaim = reader.ReadBoolean("HeadgearClaim");
                    client.Entity.Spouse = reader.ReadString("Spouse");
                    client.Entity.RamadanEffect = reader.ReadByte("Ramdan");
                    client.Entity.Owner = client;
                    client.Entity.OpenskillSoul = reader.ReadBoolean("OpenskillSoul");
                    client.Entity.SuperSkillSoul = reader.ReadBoolean("SuperSkillSoul");
                    client.Entity.AddFlower = reader.ReadUInt32("Flower");
                    client.MoneySave = reader.ReadUInt32("MoneySave");
                    client.Entity.SecondaryPass = reader.ReadUInt32("WarehousePW");
                    client.Entity.Experience = reader.ReadUInt64("Experience");
                    client.Entity.Money = reader.ReadUInt32("Money");
                    client.Entity.ConquerPoints = reader.ReadUInt32("ConquerPoints");
                    client.Entity.BoundCps = reader.ReadUInt32("BoundCps");
                    client.Entity.TreasuerPoints = reader.ReadUInt32("TreasuerPoints");
                    client.Entity.UID = reader.ReadUInt32("Uid");
                    client.Entity.MyAchievement = new Game.Achievement(client.Entity);
                    client.Entity.MyAchievement.Load(reader.ReadString("Achievement"));
                    client.Entity.Hitpoints = reader.ReadUInt32("Hitpoints");
                    client.Entity.KillerCps = reader.method_6("KillerCps");
                    client.Entity.KillerTime = DateTime.FromBinary(reader.method_7("KillerTime"));
                    client.Entity.inkillmode = reader.method_6("inkillmode");
                    //client.Entity.updatelist = reader.ReadUInt32("updatelist");
                    client.Entity.Quest = reader.ReadUInt32("Quest");
                    client.Entity.QuizPoints = reader.ReadUInt32("QuizPoints");
                    client.Entity.Body = reader.ReadUInt16("Body");
                    client.Entity.Face = reader.ReadUInt16("Face");
                    client.Entity.Strength = reader.ReadUInt16("Strength");
                    client.Entity.Titles = new ConcurrentDictionary<TitlePacket.Titles, DateTime>();
                    client.Entity.MyTitle = (TitlePacket.Titles)reader.ReadUInt32("My_Title");
                    client.Entity.Agility = reader.ReadUInt16("Agility");
                    client.Entity.Spirit = reader.ReadUInt16("Spirit");
                    client.Entity.Vitality = reader.ReadUInt16("Vitality");
                    client.Entity.Atributes = reader.ReadUInt16("Atributes");
                    client.ElitePKStats = new ElitePK.FighterStats(client.Entity.UID, client.Entity.Name, client.Entity.Mesh);
                    //client.Entity.SubClass = (byte)reader.ReadUInt32("SubClass;
                    //client.Entity.SubClassLevel = (byte)reader.ReadUInt32("SubClassLevel;
                    client.Entity.SubClass = reader.ReadByte("SubClass");
                    client.Entity.SubClassLevel = reader.ReadByte("SubClassLevel");
                    client.Entity.SubClasses.Active = client.Entity.SubClass;
                    client.Entity.SubClassesActive = client.Entity.SubClass;
                    client.Entity.SubClasses.StudyPoints = reader.ReadUInt16("StudyPoints");
                    client.VirtuePoints = (uint)reader.ReadUInt32("VirtuePoints");
                    client.Entity.Mana = reader.ReadUInt16("Mana");
                    client.Entity.HairStyle = reader.ReadUInt16("HairStyle");
                    client.Entity.MapID = reader.ReadUInt16("MapID");
                    client.VendingDisguise = reader.ReadUInt16("VendingDisguise");
                    client.SpiritBeadQ.CanAccept = !Convert.ToBoolean(reader.ReadUInt32("CanAcceptSpiritBead"));
                    client.SpiritBeadQ.Bead = (uint)reader.ReadUInt32("SpiritQuestBead");
                    client.SpiritBeadQ.CollectedSpirits = (uint)reader.ReadUInt32("CollectedSpirits");
                    client.Entity.CountryID = reader.ReadUInt16("CountryID");
                    if (client.VendingDisguise == 0)
                        client.VendingDisguise = 223;
                    //  client.Entity.Pos = reader.ReadUInt16("CTFP");
                    client.Entity.X = reader.ReadUInt16("X");
                    client.Entity.Y = reader.ReadUInt16("Y");
                    if (Constants.PKFreeMaps2.Contains(client.Entity.MapID))
                    {
                        client.Entity.MapID = 1002;
                        client.Entity.X = 300;
                        client.Entity.Y = 280;
                    }
                    if (client.Map.BaseID == 1844 || client.Entity.MapID == 1950
                        || client.Entity.MapID == 3333 || client.Entity.MapID == 7777
                        || client.Entity.MapID == 1090 || client.Entity.MapID == 4021
                        || client.Entity.MapID == 4022 || client.Entity.MapID == 4023
                        || client.Entity.MapID == 4024 || client.Entity.MapID == 4025
                        || client.Entity.MapID == 1508 || client.Entity.MapID == 1518
                        || client.Entity.MapID == 7001 || client.Entity.MapID == 1801
                        || client.Entity.MapID == 2065 || client.Entity.MapID == 8883
                        || client.Entity.MapID == 7778 || client.Entity.MapID == 7779
                        || client.Entity.MapID == 1543 || client.Entity.MapID == 1544
                        || client.Entity.MapID == 1545 || client.Entity.MapID == 1546
                        || client.Entity.MapID == 1547 || client.Entity.MapID == 1548
                        || client.Entity.MapID == 9292 || client.Entity.MapID == 1548
                        || client.Entity.MapID == 1602 || client.Entity.MapID == 1601
                        || client.Entity.MapID == 2578 || client.Entity.MapID == 2578
                        || client.Entity.MapID == 5699 || client.Entity.MapID == 5698
                        || client.Entity.MapID == 5697 || client.Entity.MapID == 2578
                        || client.Entity.MapID == 3081 || client.Entity.MapID == 5000
                        || client.Entity.MapID == 2057 || client.Entity.MapID == 7878
                        || client.Entity.MapID == 1701 || client.Entity.MapID == 1811
                        || client.Entity.MapID == 1601 || client.Entity.MapID == 9393
                        || client.Entity.MapID == 9391 || client.Entity.MapID == 9392)
                    {
                        client.Entity.MapID = 1002;
                        client.Entity.X = 300;
                        client.Entity.Y = 280;
                    }
                    if (client.Map.BaseID == 1005 && client.Entity.MapID != 1005)
                    {
                        client.Entity.MapID = 1002;
                        client.Entity.X = 300;
                        client.Entity.Y = 280;
                    }
                    if (client.Map.BaseID == 7878 && client.Entity.MapID != 7878)
                    {
                        client.Entity.MapID = 1002;
                        client.Entity.X = 300;
                        client.Entity.Y = 280;//TQ
                    }
                    if (client.Map.BaseID == 7779 && client.Entity.MapID != 7779)
                    {
                        client.Entity.MapID = 1002;
                        client.Entity.X = 300;
                        client.Entity.Y = 280;//TQ
                    }
                    if (client.Map.BaseID == 7778 && client.Entity.MapID != 7778)
                    {
                        client.Entity.MapID = 1002;
                        client.Entity.X = 300;
                        client.Entity.Y = 280;//TQ
                    }
                    if (client.Map.BaseID == 2057 && client.Entity.MapID != 2057)
                    {
                        client.Entity.MapID = 1002;
                        client.Entity.X = 300;
                        client.Entity.Y = 280;
                    }
                    if (client.Map.BaseID == 1543)
                    {
                        client.Entity.MapID = 1002;
                        client.Entity.X = 300;
                        client.Entity.Y = 280;
                    }
                    if (client.Map.BaseID == 1544)
                    {
                        client.Entity.MapID = 1002;
                        client.Entity.X = 300;
                        client.Entity.Y = 280;
                    }
                    if (client.Map.BaseID == 1545)
                    {
                        client.Entity.MapID = 1002;
                        client.Entity.X = 300;
                        client.Entity.Y = 280;
                    }
                    if (client.Map.BaseID == 1546)
                    {
                        client.Entity.MapID = 1002;
                        client.Entity.X = 300;
                        client.Entity.Y = 280;
                    }
                    if (client.Map.BaseID == 1547)
                    {
                        client.Entity.MapID = 1002;
                        client.Entity.X = 300;
                        client.Entity.Y = 280;
                    }
                    if (client.Map.BaseID == 1548)
                    {
                        client.Entity.MapID = 1002;
                        client.Entity.X = 300;
                        client.Entity.Y = 280;
                    }
                    if (client.Entity.MapID == 5555)
                    {
                        client.Entity.MapID = 1002;
                        client.Entity.X = 300;
                        client.Entity.Y = 280;
                    }
                    if (client.Entity.MapID == 0)
                    {
                        client.Entity.MapID = 1002;
                        client.Entity.X = 300;
                        client.Entity.Y = 280;
                        if (client.Entity.Level == 0)
                        {
                            client.Entity.Level = 1;
                            client.Entity.Money += 10000;
                        }
                    }
                    if (Game.Flags.WeeklyPKChampion == client.Entity.Name)
                    {
                        client.Entity.AddFlag(Network.GamePackets.Update.Flags.WeeklyPKChampion);
                    }
                    if (Game.Flags.MonthlyPKChampion == client.Entity.Name)
                    {
                        client.Entity.AddFlag(Network.GamePackets.Update.Flags.MonthlyPKChampion);
                    }
                    if (Game.Flags.TopTrojan == client.Entity.Name)
                    {
                        client.Entity.AddFlag(Network.GamePackets.Update.Flags.TopTrojan);
                    }
                    if (Game.Flags.TopWarrior == client.Entity.Name)
                    {
                        client.Entity.AddFlag(Network.GamePackets.Update.Flags.TopWarrior);
                    }
                    if (Game.Flags.TopArcher == client.Entity.Name)
                    {
                        client.Entity.AddFlag(Network.GamePackets.Update.Flags.TopArcher);
                    }
                    if (Game.Flags.TopNinja == client.Entity.Name)
                    {
                        client.Entity.AddFlag(Network.GamePackets.Update.Flags.TopNinja);
                    }
                    if (Game.Flags.TopMonk == client.Entity.Name)
                    {
                        client.Entity.AddFlag(Network.GamePackets.Update.Flags.TopMonk);
                    }
                    if (Game.Flags.TopWaterTaoist == client.Entity.Name)
                    {
                        client.Entity.AddFlag(Network.GamePackets.Update.Flags.TopWaterTaoist);
                    }
                    if (Game.Flags.TopFireTaoist == client.Entity.Name)
                    {
                        client.Entity.AddFlag(Network.GamePackets.Update.Flags.TopFireTaoist);
                    }
                    if (Game.Flags.TopPirate == client.Entity.Name)
                    {
                        client.Entity.AddFlag2(Network.GamePackets.Update.Flags2.TopPirate);
                    }
                    if (Game.Flags.TopMaster == client.Entity.Name)
                    {
                        client.Entity.AddFlag2(Network.GamePackets.Update.Flags2.WeeklyTop2PkBlue);
                    }
                    if (Game.Flags.TopChampion == client.Entity.Name)
                    {
                        client.Entity.AddFlag2(Network.GamePackets.Update.Flags2.Top2Water);
                    }
                    if (Game.Flags.TopConquer == client.Entity.Name)
                    {
                        client.Entity.AddFlag2(Network.GamePackets.Update.Flags2.WeeklyTop8Pk);
                    }
                    if (Game.Flags.TopDeadWorld == client.Entity.Name)
                    {
                        client.Entity.AddFlag2(Network.GamePackets.Update.Flags2.Top2SpouseBlue);
                    }
                    if (Game.Flags.TopHorse == client.Entity.Name)
                    {
                        client.Entity.AddFlag2(Network.GamePackets.Update.Flags2.Top2Water);
                    }
                    if (Game.Flags.TopMember == client.Entity.Name)
                    {
                        client.Entity.AddFlag2(Network.GamePackets.Update.Flags2.MonthlyTop8Pk);
                    }
                    if (Game.Flags.TopClassPk == client.Entity.Name)
                    {
                        client.Entity.AddFlag2(Network.GamePackets.Update.Flags2.MontlyTop3Pk);
                    }
                    if (Game.Flags.ToponeMan == client.Entity.Name)
                    {
                        client.Entity.AddFlag2(Network.GamePackets.Update.Flags2.Top2Trojan);
                    }
                    if (Game.Flags.TopGuildLeader == client.Entity.Name)
                    {
                        client.Entity.AddFlag(Network.GamePackets.Update.Flags.TopGuildLeader);
                    }
                    if (Game.Flags.TopDeputyLeader == client.Entity.Name)
                    {
                        client.Entity.AddFlag(Network.GamePackets.Update.Flags.TopDeputyLeader);
                    }
                    if (Game.Flags.TopSpouse == client.Entity.Name)
                    {
                        client.Entity.AddFlag(Network.GamePackets.Update.Flags.TopSpouse);
                    }
                    if (Game.Flags.Top2Archer == client.Entity.Name)
                    {
                        client.Entity.AddFlag2(Network.GamePackets.Update.Flags2.Top2Archer);
                    }
                    if (Game.Flags.TopButcher == client.Entity.Name)
                    {
                        client.Entity.AddFlag2(Conquer_Online_Server.Network.GamePackets.Update.Flags2.Top8Warrior);
                    }
                    if (Game.Flags.TopCrazy == client.Entity.Name)
                    {
                        client.Entity.AddFlag2(Conquer_Online_Server.Network.GamePackets.Update.Flags2.Top3Warrior);
                    }
                    if (Game.Flags.TopGentle == client.Entity.Name)
                    {
                        client.Entity.AddFlag2(Conquer_Online_Server.Network.GamePackets.Update.Flags2.Top3Water);
                    }
                    if (Game.Flags.TopSpouse == client.Entity.Spouse)
                    {
                        client.Entity.AddFlag(Network.GamePackets.Update.Flags.TopSpouse);
                    }
                    if (Game.Flags.TopDeputyLeader2 == client.Entity.Name)
                    {
                        client.Entity.AddFlag(Network.GamePackets.Update.Flags.TopDeputyLeader);
                    }
                    if (Game.Flags.TopDeputyLeader3 == client.Entity.Name)
                    {
                        client.Entity.AddFlag(Network.GamePackets.Update.Flags.TopDeputyLeader);
                    }
                    if (Game.Flags.TopDeputyLeader4 == client.Entity.Name)
                    {
                        client.Entity.AddFlag(Network.GamePackets.Update.Flags.TopDeputyLeader);
                    }
                    if (Game.Flags.TopDeputyLeader5 == client.Entity.Name)
                    {
                        client.Entity.AddFlag(Network.GamePackets.Update.Flags.TopDeputyLeader);
                    }
                    client.BlessTime = reader.ReadUInt32("BlessTime");
                    //client.HeadgearClaim = reader.ReadUInt32("HeadgearClaim;
                    client.NecklaceClaim = reader.ReadBoolean("NecklaceClaim");
                    client.ArmorClaim = reader.ReadBoolean("ArmorClaim");
                    client.WeaponClaim = reader.ReadBoolean("WeaponClaim");
                    client.RingClaim = reader.ReadBoolean("RingClaim");
                    client.BootsClaim = reader.ReadBoolean("BootsClaim");
                    client.FanClaim = reader.ReadBoolean("FanClaim");
                    client.TowerClaim = reader.ReadBoolean("TowerClaim");
                    client.HeadgearClaim = reader.ReadBoolean("HeadgearClaim");
                    client.InLottery = reader.ReadBoolean("InLottery");
                    client.LotteryEntries = reader.ReadByte("LotteryEntries");
                    client.LastLotteryEntry = DateTime.FromBinary(reader.ReadInt64("LastLotteryEntry"));
                    if (client.Entity.MapID >= 7008)
                    {
                        client.Entity.MapID = 1002;
                        client.Entity.X = 300;
                        client.Entity.Y = 280;
                    }
                    client.Entity.PreviousMapID = reader.ReadUInt16("PreviousMapID");
                    client.Entity.PKPoints = reader.ReadUInt16("PKPoints");
                    client.Entity.Class = reader.ReadByte("Class");
                    client.Entity.Reborn = reader.ReadByte("Reborn");
                    client.Entity.Level = reader.ReadByte("Level");
                    client.Entity.FirstRebornClass = reader.ReadByte("FirstRebornClass");
                    client.Entity.SecondRebornClass = reader.ReadByte("SecondRebornClass");
                    client.Entity.FirstRebornLevel = reader.ReadByte("FirstRebornLevel");
                    client.Entity.SecondRebornLevel = reader.ReadByte("SecondRebornLevel");
                    client.LastDragonBallUse = DateTime.FromBinary(reader.ReadInt64("LastDragonBallUse"));
                    client.LastResetTime = DateTime.FromBinary(reader.ReadInt64("LastResetTime"));
                    client.Entity.EnlightenPoints = reader.ReadUInt16("EnlightenPoints");
                    client.Entity.EnlightmentTime = reader.ReadUInt16("EnlightmentWait");
                    if (client.Entity.EnlightmentTime > 0)
                    {
                        if (client.Entity.EnlightmentTime % 20 > 0)
                        {
                            client.Entity.EnlightmentTime -= (ushort)(client.Entity.EnlightmentTime % 20);
                            client.Entity.EnlightmentTime += 20;
                        }
                    }
                    client.Entity.ReceivedEnlightenPoints = reader.ReadByte("EnlightsReceived");
                    client.Entity.DoubleExperienceTime = reader.ReadUInt16("DoubleExpTime");
                    client.DoubleExpToday = reader.ReadBoolean("DoubleExpToday");
                    client.Entity.HeavenBlessing = reader.ReadUInt32("HeavenBlessingTime");
                    client.Entity.VIPLevel = reader.ReadByte("VIPLevel");
                    client.Entity.PrevX = reader.ReadUInt16("PreviousX");
                    client.Entity.PrevY = reader.ReadUInt16("PreviousY");
                    client.ExpBalls = reader.ReadByte("ExpBalls");

                    client.Entity.ClanId = reader.ReadUInt32("ClanId");
                    client.Entity.ClanRank = (Clan.Ranks)reader.ReadUInt32("ClanRank");

                    UInt64 lastLoginInt = reader.ReadUInt32("LastLogin");
                    if (lastLoginInt != 0)
                        client.Entity.LastLogin = Kernel.FromDateTimeInt(lastLoginInt);
                    else
                        client.Entity.LastLogin = DateTime.Now;

                    if (client.Entity.MapID == 601)
                        client.OfflineTGEnterTime = DateTime.FromBinary(reader.ReadInt64("OfflineTGEnterTime"));
                    Game.ConquerStructures.Nobility.Sort(client.Entity.UID);

                    if (Kernel.Guilds.ContainsKey(reader.ReadUInt32("GuildID")))
                    {
                        client.Guild = Kernel.Guilds[reader.ReadUInt32("GuildID")];
                        if (client.Guild.Members.ContainsKey(client.Entity.UID))
                        {
                            client.AsMember = client.Guild.Members[client.Entity.UID];
                            if (client.AsMember.GuildID == 0)
                            {
                                client.AsMember = null;
                                client.Guild = null;
                            }
                            else
                            {
                                client.Entity.GuildID = (ushort)client.Guild.ID;
                                client.Entity.GuildRank = (ushort)client.AsMember.Rank;
                            }
                        }
                        else
                            client.Guild = null;
                    }
                    if (!Game.ConquerStructures.Nobility.Board.TryGetValue(client.Entity.UID, out client.NobilityInformation))
                    {
                        client.NobilityInformation = new Conquer_Online_Server.Game.ConquerStructures.NobilityInformation();
                        client.NobilityInformation.EntityUID = client.Entity.UID;
                        client.NobilityInformation.Name = client.Entity.Name;
                        client.NobilityInformation.Donation = 0;
                        client.NobilityInformation.Rank = Conquer_Online_Server.Game.ConquerStructures.NobilityRank.Serf;
                        client.NobilityInformation.Position = -1;
                        client.NobilityInformation.Gender = 1;
                        client.NobilityInformation.Mesh = client.Entity.Mesh;
                        if (client.Entity.Body % 10 >= 3)
                            client.NobilityInformation.Gender = 0;
                    }
                    client.Entity.NobilityRank = client.NobilityInformation.Rank;

                    if (DateTime.Now.DayOfYear != client.LastResetTime.DayOfYear)
                    {
                        client.ChiPoints += 500;
                        if (client.ChiPoints > 4000) client.ChiPoints = 4000;
                        if (client.Entity.Level >= 90)
                        {
                            client.Entity.EnlightenPoints = 100;
                            if (client.Entity.NobilityRank == Conquer_Online_Server.Game.ConquerStructures.NobilityRank.Knight ||
                                client.Entity.NobilityRank == Conquer_Online_Server.Game.ConquerStructures.NobilityRank.Baron)
                                client.Entity.EnlightenPoints += 100;
                            else if (client.Entity.NobilityRank == Conquer_Online_Server.Game.ConquerStructures.NobilityRank.Earl ||
                                client.Entity.NobilityRank == Conquer_Online_Server.Game.ConquerStructures.NobilityRank.Duke)
                                client.Entity.EnlightenPoints += 200;
                            else if (client.Entity.NobilityRank == Conquer_Online_Server.Game.ConquerStructures.NobilityRank.Prince)
                                client.Entity.EnlightenPoints += 300;
                            else if (client.Entity.NobilityRank == Conquer_Online_Server.Game.ConquerStructures.NobilityRank.King)
                                client.Entity.EnlightenPoints += 400;
                            if (client.Entity.VIPLevel != 0)
                            {
                                if (client.Entity.VIPLevel <= 3)
                                    client.Entity.EnlightenPoints += 100;
                                else if (client.Entity.VIPLevel <= 5)
                                    client.Entity.EnlightenPoints += 200;
                                else if (client.Entity.VIPLevel == 6)
                                    client.Entity.EnlightenPoints += 300;
                            }
                        }
                        client.Entity.ReceivedEnlightenPoints = 0;
                        client.Entity.RamadanEffect = 0;
                        client.DoubleExpToday = false;
                        client.ExpBalls = 0;
                        client.LotteryEntries = 0;
                        client.SpiritBeadQ.Reset(false, true);
                        client.Entity.Quest = 0;
                        client.LastResetTime = DateTime.Now;
                        // client.Send(new FlowerPacket(client.Entity.Flowers));
                        ResetExpball(client);
                        ResetLottery(client);
                        ResetQuest(client);
                    }
                    #region Team Arena
                    Game.TeamArena.ArenaStatistics.TryGetValue(client.Entity.UID, out client.TeamArenaStatistic);
                    if (client.TeamArenaStatistic == null)
                    {
                        client.TeamArenaStatistic = new Conquer_Online_Server.Network.GamePackets.TeamArenaStatistic(true);
                        client.TeamArenaStatistic.EntityID = client.Entity.UID;
                        client.TeamArenaStatistic.Name = client.Entity.Name;
                        client.TeamArenaStatistic.Level = client.Entity.Level;
                        client.TeamArenaStatistic.Class = client.Entity.Class;
                        client.TeamArenaStatistic.Model = client.Entity.Mesh;
                        TeamArenaTable.InsertArenaStatistic(client);
                        client.TeamArenaStatistic.Status = Network.GamePackets.TeamArenaStatistic.NotSignedUp;
                        if (Game.TeamArena.ArenaStatistics.ContainsKey(client.Entity.UID))
                            Game.TeamArena.ArenaStatistics.Remove(client.Entity.UID);
                        Game.TeamArena.ArenaStatistics.Add(client.Entity.UID, client.TeamArenaStatistic);
                    }
                    else if (client.TeamArenaStatistic.EntityID == 0)
                    {
                        client.TeamArenaStatistic = new Conquer_Online_Server.Network.GamePackets.TeamArenaStatistic(true);
                        client.TeamArenaStatistic.EntityID = client.Entity.UID;
                        client.TeamArenaStatistic.Name = client.Entity.Name;
                        client.TeamArenaStatistic.Level = client.Entity.Level;
                        client.TeamArenaStatistic.Class = client.Entity.Class;
                        client.TeamArenaStatistic.Model = client.Entity.Mesh;
                        TeamArenaTable.InsertArenaStatistic(client);
                        client.TeamArenaStatistic.Status = Network.GamePackets.TeamArenaStatistic.NotSignedUp;
                        if (Game.TeamArena.ArenaStatistics.ContainsKey(client.Entity.UID))
                            Game.TeamArena.ArenaStatistics.Remove(client.Entity.UID);
                        Game.TeamArena.ArenaStatistics.Add(client.Entity.UID, client.TeamArenaStatistic);
                    }
                    else
                    {
                        client.TeamArenaStatistic.Level = client.Entity.Level;
                        client.TeamArenaStatistic.Class = client.Entity.Class;
                        client.TeamArenaStatistic.Model = client.Entity.Mesh;
                        client.TeamArenaStatistic.Name = client.Entity.Name;
                    }
                    Game.TeamArena.Clear(client);
                    #endregion
                    #region Arena
                    Game.Arena.ArenaStatistics.TryGetValue(client.Entity.UID, out client.ArenaStatistic);
                    if (client.ArenaStatistic == null)
                    {
                        client.ArenaStatistic = new Conquer_Online_Server.Network.GamePackets.ArenaStatistic(true);
                        client.ArenaStatistic.EntityID = client.Entity.UID;
                        client.ArenaStatistic.Name = client.Entity.Name;
                        client.ArenaStatistic.Level = client.Entity.Level;
                        client.ArenaStatistic.Class = client.Entity.Class;
                        client.ArenaStatistic.Model = client.Entity.Mesh;
                        client.ArenaPoints = ArenaTable.ArenaPointFill(client.Entity.Level);
                        client.ArenaStatistic.LastArenaPointFill = DateTime.Now;
                        ArenaTable.InsertArenaStatistic(client);
                        client.ArenaStatistic.Status = Network.GamePackets.ArenaStatistic.NotSignedUp;
                        if (Game.Arena.ArenaStatistics.ContainsKey(client.Entity.UID))
                            Game.Arena.ArenaStatistics.Remove(client.Entity.UID);
                        Game.Arena.ArenaStatistics.Add(client.Entity.UID, client.ArenaStatistic);
                    }
                    else if (client.ArenaStatistic.EntityID == 0)
                    {
                        client.ArenaStatistic = new Conquer_Online_Server.Network.GamePackets.ArenaStatistic(true);
                        client.ArenaStatistic.EntityID = client.Entity.UID;
                        client.ArenaStatistic.Name = client.Entity.Name;
                        client.ArenaStatistic.Level = client.Entity.Level;
                        client.ArenaStatistic.Class = client.Entity.Class;
                        client.ArenaStatistic.Model = client.Entity.Mesh;
                        client.ArenaPoints = ArenaTable.ArenaPointFill(client.Entity.Level);
                        client.ArenaStatistic.LastArenaPointFill = DateTime.Now;
                        ArenaTable.InsertArenaStatistic(client);
                        client.ArenaStatistic.Status = Network.GamePackets.ArenaStatistic.NotSignedUp;
                        if (Game.Arena.ArenaStatistics.ContainsKey(client.Entity.UID))
                            Game.Arena.ArenaStatistics.Remove(client.Entity.UID);
                        Game.Arena.ArenaStatistics.Add(client.Entity.UID, client.ArenaStatistic);
                    }
                    else
                    {
                        client.ArenaStatistic.Level = client.Entity.Level;
                        client.ArenaStatistic.Class = client.Entity.Class;
                        client.ArenaStatistic.Model = client.Entity.Mesh;
                        client.ArenaStatistic.Name = client.Entity.Name;
                    }
                    client.ArenaPoints = client.ArenaStatistic.ArenaPoints;
                    client.CurrentHonor = client.ArenaStatistic.CurrentHonor;
                    client.HistoryHonor = client.ArenaStatistic.HistoryHonor;
                    Game.Arena.Clear(client);
                    #endregion
                    #region Champion
                    Game.Champion.ChampionStats.TryGetValue(client.Entity.UID, out client.ChampionStats);
                    if (client.ChampionStats == null)
                    {
                        client.ChampionStats = new Conquer_Online_Server.Network.GamePackets.ChampionStatistic(true);
                        client.ChampionStats.UID = client.Entity.UID;
                        client.ChampionStats.Name = client.Entity.Name;
                        client.ChampionStats.Level = client.Entity.Level;
                        client.ChampionStats.Class = client.Entity.Class;
                        client.ChampionStats.Model = client.Entity.Mesh;
                        client.ChampionStats.Points = 0;
                        client.ChampionStats.LastReset = DateTime.Now;
                        ChampionTable.InsertStatistic(client);
                        if (Game.Champion.ChampionStats.ContainsKey(client.Entity.UID))
                            Game.Champion.ChampionStats.Remove(client.Entity.UID);
                        Game.Champion.ChampionStats.Add(client.Entity.UID, client.ChampionStats);
                    }
                    else if (client.ChampionStats.UID == 0)
                    {
                        client.ChampionStats = new Network.GamePackets.ChampionStatistic(true);
                        client.ChampionStats.UID = client.Entity.UID;
                        client.ChampionStats.Name = client.Entity.Name;
                        client.ChampionStats.Level = client.Entity.Level;
                        client.ChampionStats.Class = client.Entity.Class;
                        client.ChampionStats.Model = client.Entity.Mesh;
                        client.ChampionStats.Points = 0;
                        client.ChampionStats.LastReset = DateTime.Now;
                        ArenaTable.InsertArenaStatistic(client);
                        client.ArenaStatistic.Status = Network.GamePackets.ArenaStatistic.NotSignedUp;
                        if (Game.Champion.ChampionStats.ContainsKey(client.Entity.UID))
                            Game.Champion.ChampionStats.Remove(client.Entity.UID);
                        Game.Champion.ChampionStats.Add(client.Entity.UID, client.ChampionStats);
                    }
                    else
                    {
                        client.ChampionStats.Level = client.Entity.Level;
                        client.ChampionStats.Class = client.Entity.Class;
                        client.ChampionStats.Model = client.Entity.Mesh;
                        client.ChampionStats.Name = client.Entity.Name;
                        if (client.ChampionStats.LastReset.DayOfYear != DateTime.Now.DayOfYear)
                            ChampionTable.Reset(client.ChampionStats);
                    }
                    Game.Champion.Clear(client);
                    #endregion
                    if (Conquer_Online_Server.Game.JiangHu.JiangHuClients.TryGetValue(client.Entity.UID, out hu))
                    {
                        client.Entity.MyJiang = hu as Conquer_Online_Server.Game.JiangHu;
                        client.Entity.MyJiang.TimerStamp = DateTime.Now;
                    }
                    if (client.Entity.MyJiang != null)
                    {
                        client.Entity.MyJiang.OnJiangHu = false;
                        client.Entity.MyJiang.SendStatusMode(client);
                    }
                    //EntityTable.LoadTopDonation(client);
                    //  LoadAchievement(client);
                    Network.MerchantTable.LoadMerchant(client);
                    client.Entity.FullyLoaded = true;
                    return true;
                }
                else
                    return false;
            }
        }
        public static void UpdateData(Client.GameClient client, string column, object value)
        {
            UpdateData(client.Entity.UID, column, value);
        }
        public static void UpdateData(uint UID, string column, object value)
        {
            if (value is Boolean)
            {
                using (var cmd = new MySqlCommand(MySqlCommandType.UPDATE))
                    cmd.Update("entities").Set(column, (Boolean)value).Where("UID", UID)
                        .Execute();
            }
            else
            {
                using (var cmd = new MySqlCommand(MySqlCommandType.UPDATE))
                    cmd.Update("entities").Set(column, value.ToString()).Where("UID", UID)
                        .Execute();
            }
        }
        public static void UpdateGuildRank(uint UID, Conquer_Online_Server.Game.Enums.GuildMemberRank rank)
        {
            UpdateData(UID, "GuildRank", (int)rank);
        }
        public static void UpdateOnlineStatus(Client.GameClient client, bool online, MySql.Data.MySqlClient.MySqlConnection conn)
        {
            if (online || (!online && client.DoSetOffline))
            {
                UpdateData(client, "Online", online);
            }
        }
        public static void UpdateOnlineStatus(Client.GameClient client, bool online)
        {
            if (online || (!online && client.DoSetOffline))
            {
                UpdateData(client, "Online", online);
            }
        }
        public static void UpdateBCps(Client.GameClient client)
        {
            Conquer_Online_Server.Database.MySqlCommand command = new Conquer_Online_Server.Database.MySqlCommand(MySqlCommandType.UPDATE);
            command.Update("entities").Set("BoundCps", (long)client.Entity.BoundCps).Where("UID", (long)client.Entity.UID).Execute();
        }
        public static void UpdateCps(Client.GameClient client)
        {
            UpdateData(client, "ConquerPoints", client.Entity.ConquerPoints);
        }
        public static void UpdateMoney(Client.GameClient client)
        {
            UpdateData(client, "Money", client.Entity.Money);
        }
        public static void UpdateLevel(Client.GameClient client)
        {
            UpdateData(client, "Level", client.Entity.Level);
        }
        public static void UpdateGuildID(Client.GameClient client)
        {
            UpdateData(client, "guildid", client.Entity.GuildID);
        }
        public static void UpdateClanID(Client.GameClient client)
        {
            UpdateData(client, "ClanId", client.Entity.ClanId);
        }
        public static void RemoveClan(Client.GameClient client)
        {
            using (var cmd = new MySqlCommand(MySqlCommandType.UPDATE))
                cmd.Update("entities").Set("ClanId", 0).Set("ClanDonation", 0).Set("ClanRank", 0)
                    .Where("ClanId", client.Entity.ClanId).Execute();
        }
        public static void RemoveClanMember(string name)
        {
            using (var cmd = new MySqlCommand(MySqlCommandType.UPDATE))
                cmd.Update("entities").Set("ClanId", 0).Set("ClanDonation", 0).Set("ClanRank", 0).Where("Name", name).Execute();
        }

        public static void UpdateClanRank(Client.GameClient client)
        {
            UpdateData(client, "ClanRank", (uint)client.Entity.ClanRank);
        }
        public static void UpdateClanRank(uint UID, uint rank)
        {
            UpdateData(UID, "ClanRank", rank);
        }

        public static void UpdateClanDonation(Client.GameClient client)
        {
            UpdateData(client, "clandonation", (uint)client.Entity.ClanRank);
        }
        public static void UpdateGuildRank(Client.GameClient client)
        {
            UpdateData(client, "GuildRank", client.Entity.GuildRank);
        }
        public static void UpdateSkillExp(Client.GameClient client, uint spellid, uint exp)
        {
            using (var cmd = new MySqlCommand(MySqlCommandType.UPDATE))
                cmd.Update("skills").Set("Experience", exp).Where("EntityID", client.Entity.UID).And("ID", spellid).Execute();
        }
        public static void ResetLottery(Client.GameClient client)
        {
            UpdateData(client, "LotteryEntries", 0);
        }
        public static void ResetQuest(Client.GameClient client)
        {
            UpdateData(client, "Quest", 0);
        }
        public static void ResetExpball(Client.GameClient client)
        {
            UpdateData(client, "ExpBalls", 0);
        }
        public static bool SaveEntity(Client.GameClient client, MySql.Data.MySqlClient.MySqlConnection conn)
        {
            try
            {
                Conquer_Online_Server.Database.MySqlCommand command = new Conquer_Online_Server.Database.MySqlCommand(MySqlCommandType.UPDATE);
                command.Update("entities")
                    .Set("WarehousePW", client.WarehousePW)
                    .Set("Spouse", client.Entity.Spouse)

                    .Set("Money", client.Entity.Money)
                        .Set("Ramdan", client.Entity.RamadanEffect)
                    .Set("Experience", client.Entity.Experience)
                    .Set("ConquerPoints", client.Entity.ConquerPoints)
                    .Set("BoundCps", client.Entity.BoundCps)
                    .Set("Body", client.Entity.Body)
                    .Set("Face", client.Entity.Face)
                    .Set("Class", client.Entity.Class)

                    .Set("Reborn", client.Entity.Reborn)
                    .Set("Level", client.Entity.Level)

                    .Set("Status", client.Entity.Status)
                    .Set("Status2", client.Entity.Status2)
                    .Set("Status3", client.Entity.Status3)
                    .Set("Status4", client.Entity.Status4)
                    .Set("My_Title", (byte)client.Entity.MyTitle)
                    .Set("RacePoints", client.RacePoints)
                    .Set("StudyPoints", client.Entity.SubClasses.StudyPoints)
                    .Set("HairStyle", client.Entity.HairStyle)
                    .Set("EnlightsReceived", client.Entity.ReceivedEnlightenPoints)
                    .Set("PKPoints", client.Entity.PKPoints)
                    .Set("QuizPoints", client.Entity.QuizPoints)
                    .Set("ExpBalls", client.ExpBalls)
                    .Set("MoneySave", client.MoneySave)
                    .Set("Hitpoints", client.Entity.Hitpoints)
                    .Set("LastDragonBallUse", client.LastDragonBallUse.Ticks)
                    .Set("Strength", client.Entity.Strength)
                    .Set("Agility", client.Entity.Agility)
                    .Set("Spirit", client.Entity.Spirit)
                    .Set("Quest", client.Entity.Quest)
                    .Set("Vitality", client.Entity.Vitality)
                    .Set("PreviousX", client.Entity.PrevX)
                    .Set("PreviousY", client.Entity.PrevY)
                    .Set("Atributes", client.Entity.Atributes)
                    .Set("Mana", client.Entity.Mana)

                    .Set("VIPLevel", client.Entity.VIPLevel)
                    .Set("MapID", client.Entity.MapID)
                    .Set("X", client.Entity.X)
                    .Set("Y", client.Entity.Y)
                    .Set("VirtuePoints", client.VirtuePoints)
                    .Set("PreviousMapID", client.Entity.PreviousMapID)
                    .Set("EnlightenPoints", client.Entity.EnlightenPoints)
                    .Set("LastResetTime", client.LastResetTime.Ticks)
                    .Set("DoubleExpTime", client.Entity.DoubleExperienceTime)
                    .Set("Achievement", client.Entity.MyAchievement.ToString())
                    .Set("DoubleExpToday", client.DoubleExpToday)
                    .Set("HeavenBlessingTime", client.Entity.HeavenBlessing)
                    .Set("InLottery", client.InLottery)
                    .Set("LotteryEntries", client.LotteryEntries)
                    .Set("LastLotteryEntry", client.LastLotteryEntry.Ticks)
                    .Set("HeadgearClaim", client.HeadgearClaim)
                    .Set("NecklaceClaim", client.NecklaceClaim)
                    .Set("ArmorClaim", client.ArmorClaim)
                    .Set("WeaponClaim", client.WeaponClaim)
                    .Set("RingClaim", client.RingClaim)
                    .Set("Flower", client.Entity.AddFlower)
                    .Set("BootsClaim", client.BootsClaim)
                    .Set("TowerClaim", client.TowerClaim)
                    .Set("FanClaim", client.FanClaim)
                    .Set("ChatBanTime", client.ChatBanTime.Ticks)
                    .Set("ChatBanLasts", client.ChatBanLasts)
                    .Set("ChatBanned", client.ChatBanned)
                    .Set("BlessTime", client.BlessTime)
                    .Set("FirstRebornClass", client.Entity.FirstRebornClass)
                    .Set("SecondRebornClass", client.Entity.SecondRebornClass)
                    .Set("FirstRebornLevel", client.Entity.FirstRebornLevel)
                    .Set("SecondRebornLevel", client.Entity.SecondRebornLevel)
                    .Set("EnlightmentWait", client.Entity.EnlightmentTime)
                    .Set("LastLogin", client.Entity.LastLogin.Ticks)
                    .Set("CountryID", (ushort)client.Entity.CountryID)
                    .Set("ClanId", (uint)client.Entity.ClanId)
                    .Set("ClanRank", (uint)client.Entity.ClanRank);
                if (client.Entity.Reborn == 1)
                {
                    command.Set("FirstRebornClass", (long)client.Entity.FirstRebornClass);
                }
                if (client.Entity.Reborn == 2)
                {
                    command.Set("SecondRebornClass", (long)client.Entity.SecondRebornClass);
                }
                if (client.Entity.MapID == 601)
                {
                    command.Set("OfflineTGEnterTime", client.OfflineTGEnterTime.Ticks);
                }
                else
                {
                    command.Set("OfflineTGEnterTime", "0");
                }
                client.Entity.LastLogin = DateTime.Now;
                if (client.AsMember != null)
                {
                    command.Set("GuildID", (long)client.AsMember.GuildID).Set("GuildRank", (long)((ulong)client.AsMember.Rank)).Set("GuildSilverDonation", client.AsMember.SilverDonation).Set("GuildConquerPointDonation", client.AsMember.ConquerPointDonation);
                }
                else
                {
                    command.Set("GuildID", (long)0).Set("GuildRank", (long)0).Set("GuildSilverDonation", (long)0).Set("GuildConquerPointDonation", (long)0);
                }
                command.Where("UID", (long)client.Entity.UID).Execute();
            }
            catch (Exception exception)
            {
                Program.SaveException(exception);
            }
            return true;
        }
        public static bool SaveEntity(Client.GameClient c)
        {
            using (var conn = DataHolder.MySqlConnection)
            {
                conn.Open();
                return SaveEntity(c, conn);
            }
        }
        static bool InvalidCharacters(string Name)
        {
            foreach (char c in Name)
            {
                if (Kernel.InvalidCharacters.Contains(c) || (byte)c < 48)
                {
                    return true;
                }
            }
            return false;
        }
        public static void UpdateNames()
        {
            Dictionary<String, NameChangeC> UPDATE = new Dictionary<string, NameChangeC>();
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("nobility");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            // String newname = "", name = "";
            int UID;
            ulong Donation;
            while (r.Read())
            {
                //newname = r.ReadString("namechange");
                //name = r.ReadString("name");
                UID = (int)r.ReadInt64("EntityUID");
                Donation = (ulong)r.ReadInt64("Donation");
                if (Donation != 0)
                {
                    MySqlCommand cmdupdate = null;
                    cmdupdate = new MySqlCommand(MySqlCommandType.UPDATE);
                    cmdupdate.Update("entities").Set("Donation", Donation).Where("UID", UID).Execute();
                    Console.WriteLine("Donation Set.");
                }
            }
            r.Close();
            r.Dispose();
        }
        /*   public static bool CreateEntity(Network.GamePackets.EnitityCreate eC, Client.GameClient client, ref string message)
           {
               if (eC.Name.Length > 16)
                   eC.Name = eC.Name.Substring(0, 16);
               if (eC.Name == "")
                   return false;

               if (InvalidCharacters(eC.Name))
               {
                   message = "Invalid characters inside the name.";
                   return false;
               }
               using (var rdr = new MySqlReader(new MySqlCommand(MySqlCommandType.SELECT).Select("entities").Where("name", eC.Name)))
               {
                   if (rdr.Read())
                   {
                       message = "The chosen name is already in use.";
                       return false;
                   }
               }
               client.Entity = new Game.Entity(Game.EntityFlag.Player, false);
               client.Entity.Name = eC.Name;
               DataHolder.GetStats(eC.Class, 1, client);
               client.CalculateStatBonus();
               client.CalculateHPBonus();
               client.Entity.Hitpoints = client.Entity.MaxHitpoints;
               client.Entity.Mana = (ushort)(client.Entity.Spirit * 5);
               client.Entity.Class = eC.Class;
               client.Entity.Body = eC.Body;
               if (eC.Body == 1003 || eC.Body == 1004)
                   client.Entity.Face = (ushort)Kernel.Random.Next(1, 50);
               else
                   client.Entity.Face = (ushort)Kernel.Random.Next(201, 250);
               byte Color = (byte)Kernel.Random.Next(4, 8);
               client.Entity.HairStyle = (ushort)(Color * 100 + 10 + (byte)Kernel.Random.Next(4, 9));
               client.Entity.UID = Program.EntityUID.Next;
               client.Entity.JustCreated = true;

               while (true)
               {
                   using (var cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("entities").Where("uid", client.Entity.UID))
                   using (var reader = cmd.CreateReader())
                   {
                       if (reader.Read())
                           client.Entity.UID = Program.EntityUID.Next;
                       else
                           break;
                   }
               }
               while (true)
               {
                   try
                   {
                       using (var cmd = new MySqlCommand(MySqlCommandType.INSERT))
                           cmd.Insert("entities").Insert("Name", eC.Name).Insert("Owner", client.Account.Username).Insert("Class", eC.Class).Insert("UID", client.Entity.UID)
                               .Insert("Hitpoints", client.Entity.Hitpoints).Insert("Mana", client.Entity.Mana).Insert("Body", client.Entity.Body)
                               .Insert("Face", client.Entity.Face).Insert("HairStyle", client.Entity.HairStyle).Insert("Strength", client.Entity.Strength)
                               .Insert("WarehousePW", "").Insert("Agility", client.Entity.Agility).Insert("Vitality", client.Entity.Vitality).Insert("Spirit", client.Entity.Spirit)
                               .Execute();

                       message = "ANSWER_OK";
                       break;
                   }
                   catch
                   {
                       client.Entity.UID = Program.EntityUID.Next;
                   }
               }
            
               using (var cmd = new MySqlCommand(MySqlCommandType.UPDATE).Update("configuration").Set("EntityID", client.Entity.UID).Where("Server", Constants.ServerName))
                   cmd.Execute();
               client.Account.EntityID = client.Entity.UID;
               client.Account.Save();
               return true;
           }
           */
        public static bool CreateEntity(Network.GamePackets.EnitityCreate eC, Client.GameClient client, ref string message)
        {
            if (eC.Name.Length > 16)
                eC.Name = eC.Name.Substring(0, 16);
            if (eC.Name == "")
                return false;

            if (InvalidCharacters(eC.Name))
            {
                message = "Invalid characters inside the name.";
                return false;
            }
            using (var rdr = new MySqlReader(new MySqlCommand(MySqlCommandType.SELECT).Select("entities").Where("name", eC.Name)))
            {
                if (rdr.Read())
                {
                    message = "The chosen name is already in use.";
                    return false;
                }
            }
            client.Entity = new Game.Entity(Game.EntityFlag.Player, false);
            client.Entity.Name = eC.Name;
            switch (eC.Class)
            {
                case 0:
                case 1: eC.Class = 100; break;
                case 2:
                case 3: eC.Class = 10; break;
                case 4:
                case 5: eC.Class = 40; break;
                case 6:
                case 7: eC.Class = 20; break;
                case 8:
                case 9: eC.Class = 50; break;
                case 10:
                case 11: eC.Class = 60; break;
                case 12:
                case 13: eC.Class = 70; break;
                case 14:
                case 15: eC.Class = 80; break;
            } 
            DataHolder.GetStats(eC.Class, 1, client);
            client.CalculateStatBonus();
            client.CalculateHPBonus();
            client.Entity.Hitpoints = client.Entity.MaxHitpoints;
            client.Entity.Mana = (ushort)(client.Entity.Spirit * 5);
            client.Entity.Class = eC.Class;
            client.Entity.Body = eC.Body;
            if (eC.Body == 1003 || eC.Body == 1004)
                client.Entity.Face = (ushort)Kernel.Random.Next(1, 50);
            else
                client.Entity.Face = (ushort)Kernel.Random.Next(201, 250);
            byte Color = (byte)Kernel.Random.Next(4, 8);
            client.Entity.HairStyle = (ushort)(Color * 100 + 10 + (byte)Kernel.Random.Next(4, 9));
            client.Entity.UID = Program.EntityUID.Next;
            client.Entity.JustCreated = true;

            while (true)
            {
                using (var cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("entities").Where("uid", client.Entity.UID))
                using (var reader = cmd.CreateReader())
                {
                    if (reader.Read())
                        client.Entity.UID = Program.EntityUID.Next;
                    else
                        break;
                }
            }
            while (true)
            {
                try
                {
                    using (var cmd = new MySqlCommand(MySqlCommandType.INSERT))
                        cmd.Insert("entities").Insert("Name", eC.Name).Insert("Owner", client.Account.Username).Insert("Class", eC.Class).Insert("UID", client.Entity.UID)
                            .Insert("Hitpoints", client.Entity.Hitpoints).Insert("Mana", client.Entity.Mana).Insert("Body", client.Entity.Body)
                            .Insert("Face", client.Entity.Face).Insert("HairStyle", client.Entity.HairStyle).Insert("Strength", client.Entity.Strength)
                            .Insert("WarehousePW", "").Insert("Agility", client.Entity.Agility).Insert("Vitality", client.Entity.Vitality).Insert("Spirit", client.Entity.Spirit)
                            .Execute();
                    Database.MySqlCommand com = new Database.MySqlCommand(MySqlCommandType.INSERT);
                    com.Insert("achievement").Insert("UID", (long)client.Entity.UID).Insert("Owner", client.Account.Username).Insert("Name", client.Entity.Name);
                    message = "ANSWER_OK";
                    break;
                }
                catch
                {
                    client.Entity.UID = Program.EntityUID.Next;
                }
            }

            using (var cmd = new MySqlCommand(MySqlCommandType.UPDATE).Update("configuration").Set("EntityID", client.Entity.UID).Where("Server", Constants.ServerName))
                cmd.Execute();
            client.Account.EntityID = client.Entity.UID;
            client.Account.Save();
            return true;
        }
        public static void KillerTime(Client.GameClient client)
        {
            Conquer_Online_Server.Database.MySqlCommand mySqlCommand = new Conquer_Online_Server.Database.MySqlCommand(MySqlCommandType.UPDATE);

            mySqlCommand.Update("entities").Set("KillerTime", (long)0).Where("UID", (long)((ulong)client.Entity.UID)).Execute();
        }

        public static void UpdateTreasuerPoints(Client.GameClient GameClient)
        {
            UpdateData(GameClient, "TreasuerPoints", GameClient.Entity.TreasuerPoints);
        }
    }
}
