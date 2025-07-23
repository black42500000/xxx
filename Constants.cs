using System;
using Conquer_Online_Server.Network.GamePackets;

namespace Conquer_Online_Server
{
    public class Constants
    {
        public static readonly Message FullInventory = new Message("There is not enough room in your inventory!", System.Drawing.Color.Red, Message.TopLeft),
            OneFlowerADay = new Message("You may only send 1 flower a day", System.Drawing.Color.Red, Message.TopLeft),            
            OneKissADay = new Message("You may only send 1 Kiss a day", System.Drawing.Color.Red, Message.TopLeft),
            TradeRequest = new Message("Trade request sent.", System.Drawing.Color.Red, Message.TopLeft),
             CursedBidden = new Message("You cant telepor while you are cursed", System.Drawing.Color.Red, Message.TopLeft),
            th30FlowerADay = new Message("You may only send  free flower a day", System.Drawing.Color.Red, Message.TopLeft),
            TradeInventoryFull = new Message("There is not enough room in your partner inventory.", System.Drawing.Color.Red, Message.TopLeft),
            TradeInProgress = new Message("An trade is already in progress. Try again later.", System.Drawing.Color.Red, Message.TopLeft),
            FloorItemNotAvailable = new Message("You need to wait until you will be able to pick this item up!", System.Drawing.Color.Red, Message.TopLeft),
            JailItemUnusable = new Message("You can't use this item in here!", System.Drawing.Color.Red, Message.TopLeft),
            PKForbidden = new Message("PK Forbidden in this map.", System.Drawing.Color.Red, Message.TopLeft),
            ExpBallsUsed = new Message("You can use only ten exp balls a day. Try tomorrow.", System.Drawing.Color.Red, Message.TopLeft),
            SpellLeveled = new Message("Congratulation, you have just leveled your spell.", System.Drawing.Color.Red, Message.TopLeft),
            ProficiencyLeveled = new Message("Congratulation, you have just leveled your proficiency.", System.Drawing.Color.Red, Message.TopLeft),
            ArrowsReloaded = new Message("Arrows Reloaded.", System.Drawing.Color.Red, Message.TopLeft),
            Warrent = new Message("The [Guard1]s are looking for you!", System.Drawing.Color.Red, Message.TopLeft),
            VIPExpired = new Message("Your VIP has expired. Please reactivate your VIP if you wish to keep VIP services.", System.Drawing.Color.Red, Message.World),
            VIPLifetime = new Message("Your VIP service is unlimited.", System.Drawing.Color.Red, Message.World),
            WrongAccessory = new Message("You cannot wear this accessory and this item at the same time.", System.Drawing.Color.Red, Message.World),
            NoAccessory = new Message("You cannot wear an accessory without a support item.", System.Drawing.Color.Red, Message.World),
            vipteleport = new Message(" you can't teleport in this map.", System.Drawing.Color.Red, Message.World);

        public static Message VIPRemaining(string days, string hours)
        {
            return new Message("You have " + days + " day(s) and " + hours + " hour(s) of VIP service remaining.", System.Drawing.Color.Red, Message.World);
        }
        public static Message NoArrows(string name)
        {
            return new Message("Can't reload arrows, you are out of " + name + "s!", System.Drawing.Color.Red, Message.TopLeft);
        }
        public static Message Stigma(float percent, int time)
        {
            return new Message("Stigma activated. Your attack will be increased with " + percent + " for " + time + ".", System.Drawing.Color.Red, Message.TopLeft);
        }
        public static Message Accuracy(int time)
        {
            return new Message("Accuracy activated. Your agility will be increased a bit for " + time + ".", System.Drawing.Color.Red, Message.TopLeft);
        }
        public static Message Invisibility(int time)
        {
            return new Message("Invisibility activated. You will be invisible for monsters as long as you don't attack for " + time + ".", System.Drawing.Color.Red, Message.TopLeft);
        }
        public static Message Shield(float percent, int time)
        {
            return new Message("Shield activated. Your defence will be increased with " + percent + " for " + time + ".", System.Drawing.Color.Red, Message.TopLeft);
        }
        public static Message Shackled(int time)
        {
            return new Message("You have been shackled and can not move for " + time + " Seconds.", System.Drawing.Color.Red, Message.TopLeft);
        }
        public static Message Dodge(float percent, int time)
        {
            return new Message("Dodge activated. Your dodge will be increased with " + percent + " for " + time + ".", System.Drawing.Color.Red, Message.TopLeft);
        }
        public static Message NoDrugs(int time)
        {
            return new Message("Poison star activated. You will not be able to use drugs for " + time + " seconds.", System.Drawing.Color.Red, Message.TopLeft);
        }
        public static Message ExtraExperience(uint experience)
        {
            return new Message("You have gained extra " + experience + " experience for killing the monster.", System.Drawing.Color.Red, Message.TopLeft);
        }
        public static Message TeamExperience(uint experience)
        {
            return new Message("One of your teammates killed a monster so you gained " + experience + " experience.", System.Drawing.Color.Red, Message.TopLeft);
        }
        public static Message NoobTeamExperience(uint experience)
        {
            return new Message("One of your teammates killed a monster and because you have a noob inside your team, you gained " + experience + " experience.", System.Drawing.Color.Red, Message.TopLeft);
        }
        public static Message PickupGold(uint amount)
        {
            return new Message("You have picked up " + amount + " gold.", System.Drawing.Color.Red, Message.TopLeft);
        }
        public static Message AzureShield(float percent, int time)
        {
            return new Message("AzureShield activated. Your defence will be increased with " + percent + " for " + time + " Seconds.", System.Drawing.Color.Red, Message.TopLeft);
        }
        public static Message PickupConquerPoints(uint amount)
        {
            return new Message("You have picked up " + amount + " conquer points.", System.Drawing.Color.Red, Message.TopLeft);
        }
        public static Message PickupItem(string name)
        {
            return new Message("You have picked up a/an " + name + " item.", System.Drawing.Color.Red, Message.TopLeft);
        }
        public static Message DummyLevelTooHigh()
        {
            return new Message("You can't attack this dummy because your level is not high enough.", System.Drawing.Color.Red, Message.TopLeft);
        }
        public static Message BoothItemSell(string buyername, string itemname, bool conquerpoints, uint cost)
        {
            return new Message("Congratulations. You just have just sold " + itemname + " to " + buyername + " for " + cost + (conquerpoints ? " ConquerPoints." : " Gold."), System.Drawing.Color.Red, Message.TopLeft);
        }
        public static Message Enchant(int origEnch, int newEnch)
        {
            if (newEnch <= origEnch)
                return new Message("You were unlucky. You didn't gain any more enchantment in your item. Your generated enchant is " + newEnch + ".", System.Drawing.Color.Red, Message.TopLeft);
            else
                return new Message("You were lucky. You gained more enchantment in your item. Your generated enchant is " + newEnch + ".", System.Drawing.Color.Red, Message.TopLeft);
        }
        public static Message VoteSpan(Client.GameClient client)
        {
            if (DateTime.Now <= client.LastVote.AddHours(12))
            {
                TimeSpan agospan = client.LastVote.Subtract(DateTime.Now);
                TimeSpan tillspan = DateTime.Now.Subtract(client.LastVote);
                string message = "You last voted ";
                if (agospan.Hours >= 0)
                    message += agospan.Hours.ToString() + " hours, ";
                if (agospan.Minutes >= 0)
                    message += agospan.Minutes.ToString() + " minutes, and ";
                message += agospan.Seconds.ToString() + " ago. Please wait ";
                if (tillspan.Hours >= 0)
                    message += tillspan.Hours.ToString() + " hours, ";
                if (tillspan.Minutes >= 0)
                    message += agospan.Minutes.ToString() + " minutes, and ";
                message += tillspan.Seconds.ToString() + " ago. To vote again!";
                return new Message(message, System.Drawing.Color.Red, Message.TopLeft);
            }
            return new Message("You haven't voted in the past 12 hours. Vote now to gain an extra point!", System.Drawing.Color.Red, Message.TopLeft);
        }
        public const string DataHolderPath = "database\\",
        NpcFilePath = "database\\Npcs.txt",
        DMapsPath = "database\\",
        ShopsPath = "database\\Shops.dat",
        EShopsPath = "database\\EShops.ini",
        EShopsV2Path = "database\\shops\\emoneyshopV2.ini",
        HonorShopPath = "database\\shops\\HonorShop.ini",
        RaceShopPath = "database\\shops\\RacePointShop.ini",
        ChampionShopPath = "database\\shops\\MrBahaaLeagueShop.ini",
        PortalsPath = "database\\Portals.ini",
        RevivePoints = "database\\RevivePoints.ini",
        MonstersPath = "database\\Monsters.txt",
        ItemBaseInfosPath = "database\\Items.txt",
        ItemPlusInfosPath = "database\\ItemsPlus.ini",
        SoulGearInformation = "database\\soulgear.txt",
        UnhandledExceptionsPath = "database\\exceptions\\",
        ServerKey = "TQServer",
        WelcomeMessages = "database\\WelcomeMessages.txt",
        QuizShow = "database\\QuizShow.txt",
        GameCryptographyKey = "C238xs65pjy7HU9Q";
        public static string ServerName = "MrBahaa";
        public const int MaxBroadcasts = 50;
        public static uint ExtraExperienceRate, ExtraSpellRate, ExtraProficiencyRate, MoneyDropRate, MoneyDropMultiple, ConquerPointsDropRate, ConquerPointsDropMultiple, ItemDropRate;
        public static string[] ItemDropQualityRates;
        public static string WebAccExt, ServerWebsite, WebVoteExt, WebDonateExt, ServerGMPass;
        public const sbyte pScreenDistance = 19;
        public const sbyte nScreenDistance = 19;
        public const sbyte remScreenDistance = 19;
        public const ushort DisconnectTimeOutSeconds = 10,
            FloorItemSeconds = 20,
            FloorItemAvailableAfter = 15;

        public const ushort SocketOneProgress = 100,
            SocketTwoProgress = 300;

        public static readonly System.Collections.Generic.List<ushort> PKForbiddenMaps = new System.Collections.Generic.List<ushort>()
        {
            1036,
            1002,
            700,
            1039,
            1004,
            1006,
            8880,
            8881,
            1950,
            8800,
            8801,
            8802,
            8803,
            601
        };
        public static readonly System.Collections.Generic.List<ushort> PKJiangHu = new System.Collections.Generic.List<ushort>()
        {
            1011,
            1015,
            1020,
            1000
        };
        public static readonly System.Collections.Generic.List<ulong> VipNo = new System.Collections.Generic.List<ulong>() 
        { 
            1, 
            2, 
            3, 
            2060, 
            7009, 
            7779, 
            1950, 
            1005,  
            3031, 
            7005, 
            7006, 
            7008, 
            6000, 
            6004, 
            6001, 
            6002,  
            6003, 
            1844, 
            7001, 
            4500, 
            4501, 
            4502, 
            4503, 
            4504, 
            4505, 
            4506, 
            1801, 
            1508, 
            1518, 
            7777, 
            8877, 
            5522,5523,5524,5560,5526,5527,5525,5521, 
            3333, 
            //1090, 
            1225, 
            700 
        };
        public static readonly System.Collections.Generic.List<ushort> PKFreeMaps = new System.Collections.Generic.List<ushort>()
        {
            5555,
            1249,
            3055,  
            3216,
            3214,
            7779,
            7778,
            1707,
            8596,
            8597,
            8598,
            8599,
            1820,
            3320,
            3321,
            9992,
            9993,
            9994,
            9995,
            9996,
            9997,
            9999,
            3322,
            1811,
            9393,
            9392,
            9391,
            1701,
            1601,
            2065,
            1038,
            1005, 
            6000,
            6004,
            6001,
            6002, 
            6003,
            1844,
            7001,
            2071,
            1801,
            1508,
            1518,
            7777,
            8877,
            3333,
            1090,
            700,
            1509
        };
        public static readonly System.Collections.Generic.List<ulong> FBandSSEvent = new System.Collections.Generic.List<ulong>() 
        { 
            4573, 
            4574, 
            4575, 
            4576, 
            4577, 
            4578 
        };  
        public static readonly System.Collections.Generic.List<string> NoFog = new System.Collections.Generic.List<string>() 
        { 
           // "Pheasant", 
            "CriemnalMrBahaa",
            "9TailsMonster",
            "YellowTiger",
            "BlackScythe",
            "DeathMonster",
            "ThePunisher",
            "Cyclops", 
            "Hades", 
            "Centar", 
            "GoldenOctopus", 
            "SwordMaster",  
            "MrBahaa",              
            "ThrillingSpook ", 
            "TeratoDragon", 
            "SnowBanshee",
            "LavaBeast",
            "NemesisTyrant"
 
        };
        public static readonly System.Collections.Generic.List<ulong> PKFreeMaps2 = new System.Collections.Generic.List<ulong>()
        {
            5555,
            1249,
            3216,
            3214,
            7779,
            7778,
            1707,
            8596,
            8595,
            8597,
            8598,
            8599,
            1820,
            1701,
            1601,
            1811,
            3320,
            3321,
            9992,
            9993,
            9994,
            9995,
            9996,
            9997,
            9999,
            3322,
             9393,
            9392,
            9391,
            7009,
            2060,
            1509,
            7779,
            3031,
            1844,
            7001,
            2071,
            1801,
            7005,
            7006,
            7008,
            1508,
            1518,
            7777,
            8877,
            5522,
            5523,
            5524,
            5560,
            5526,
            5527,
            5525,
            5521,
            4500,
            4501,
            4502,
            4503,
            4504,
            4505,
            4506,
            3333,
           // 1090,
            1225,
            5550,
            5560,
            5580,
            5570,
            5550,
            5540,
            5530,
            3071
        };
        public static readonly System.Collections.Generic.List<int> SoulList = new System.Collections.Generic.List<int>()
        {
            
            80032000
        };
        public static readonly System.Collections.Generic.List<int> MaxItems = new System.Collections.Generic.List<int>()
        {
            410439,
            420439, 
            480439,
            610439,
            601439, 
            421439,
            823052,
            824001,
            823043,
            822052,
            800014,
            800017,
            800513,
            822053,
            820056,
            800110,
            800320
        };
        public static readonly System.Collections.Generic.List<int> AvaibleSpells = new System.Collections.Generic.List<int>()
        {
            1380,
            1385, 
            1390,
            1395,
            1400, 
            1405,
            1410,
            1046,
            1045,
            5030,
            7001,
            7002,
            7003,
            7020,
            1415,
1418,
1416,
1417,
1419,
            1360
        
        };
        public static readonly System.Collections.Generic.List<ushort> Damage1Map = new System.Collections.Generic.List<ushort>()
        {
            1844,
            1801
        };
    }
}
