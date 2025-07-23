using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Database
{
    class rates
    {
        public static uint  ClanWar, SkillSoul, SuperSkillSoul, LordTiger, swordmaster, GuildWar, PartyDrop, Weather, VotePrize, Steed, TeratoDragon, ThrillingSpook, SnowBanshe, SteedRace, MonthlyPk, ChangeName, EliteGw, DailyPk, LastMan, TopSpouse, Riencration, king, prince, maxcps, autocps, classpk, weeklypk;
        public static string VoteUrl = "";
        public static string serversite = "";
        public static string PopUpURL = "";
        public static string servername = "";
        public static string cryptkey = "";
        public static string coder = "";
        public static string cpsmethod = "";
        public static string GM = "";



        public static void LoadRates()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("rates");
            using (MySqlReader r = new MySqlReader(cmd))
                while (r.Read())
                {
                    PartyDrop = r.ReadUInt32("PartyDrop");
                    LordTiger = r.ReadUInt32("LordTiger");
                    VoteUrl = r.ReadString("VoteUrl");
                    VotePrize = r.ReadUInt32("VotePrize");
                   Weather = r.ReadUInt32("Weather");
                    Steed = r.ReadUInt32("Steed");
                    TeratoDragon = r.ReadUInt32("TeratoDragon");
                    ThrillingSpook = r.ReadUInt32("ThrillingSpook");
                    SnowBanshe = r.ReadUInt32("SnowBanshe");
                    swordmaster = r.ReadUInt32("SowrdMaster");
                    GuildWar = r.ReadUInt32("GuildWar");
                    SteedRace = r.ReadUInt32("SteedRace");
                    ChangeName = r.ReadUInt32("ChangeName");
                    MonthlyPk = r.ReadUInt32("MonthlyPk");
                    EliteGw = r.ReadUInt32("EliteGw");
                    GM = r.ReadString("GM");
                    TopSpouse = r.ReadUInt32("TopSpouse");
                    DailyPk = r.ReadUInt32("DailyPk");
                    LastMan = r.ReadUInt32("LastMan");
                    Riencration = r.ReadUInt32("Riencration");
                    king = r.ReadUInt32("kings");
                    ClanWar = r.ReadUInt32("ClanWar");
                    prince = r.ReadUInt32("prince");
                    maxcps = r.ReadUInt32("MaxCps");
                    autocps = r.ReadUInt32("autoCps"); 
                    classpk = r.ReadUInt32("ClassPk");
                    weeklypk = r.ReadUInt32("WeeklyPk");
                    SkillSoul = r.ReadUInt32("SkillSoul");
                    SuperSkillSoul = r.ReadUInt32("SuperSkillSoul");
                    serversite = r.ReadString("ServerWebsite");
                    servername = r.ReadString("ServerName");
                    coder = r.ReadString("Coder");
                    PopUpURL = r.ReadString("LoginSite");
                    Constants.ServerName = r.ReadString("ServerName");
                }
            Program.WriteLine("Rates Loaded.");
            //r.Close();
            //r.Dispose();
        }
    }
}
