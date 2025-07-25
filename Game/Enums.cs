﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Game
{
    public enum AppearanceType
    {
        Default = 0,
        Garment = 1,
        Artifact = 2,
        Equipment = 3,

        First = Default,
        Last = Equipment + 1
    }
    public enum SlotMachineSubType : byte
    {
        StartSpin = 0,
        StopSpin = 1,
        ClientFinishSpin = 2
    }

    public enum SlotMachineItems : byte
    {
        Stancher = 0,
        Meteor = 1,
        Sword = 2,
        TwoSwords = 3,
        SwordAndShield = 4,
        ExpBall = 5,
        DragonBall = 6
    }
    public static class Enums
    {
        public enum ItemPositionName : ushort
        {
            Inventory = 0,
            Headgear = 1,
            Necklace = 2,
            Armor = 3,
            Right_weapon = 4,
            Left_weapon = 5,
            Ring = 6,
            Bottle = 7,
            Boots = 8,
            Garment = 9,
            Fan = 10,
            Tower = 11,
            Mount = 12,
            Right_weapon_accessory = 15,
            Left_weapon_accessory = 16,
            Mount_armor = 17,
            Riding_crop = 18,
            Alternate_Headgear = 21,
            Alternate_Necklace = 22,
            Alternate_Armor = 23,
            Alternate_Right_weapon = 24,
            Alternate_Left_weapon = 25,
            Alternate_Ring = 26,
            Alternate_Bottle = 27,
            Alternate_Boots = 28,
            Alternate_Garment = 29,
            Remove = 255
        }
        public enum ChiAttribute
        {
            None = 0,
            CriticalStrike = 1,
            SkillCriticalStrike = 2,
            Immunity = 3,
            Breakthrough = 4,
            Counteraction = 5,
            HPAdd = 6,
            AddAttack = 7,
            AddMagicAttack = 8,
            AddMagicDefense = 9,
            PhysicalDamageIncrease = 10,
            MagicDamageIncrease = 11,
            PhysicalDamageDecrease = 12,
            MagicDamageDecrease = 13
        }
        public static int ChiMinValues(ChiAttribute attribute)
        {
            switch (attribute)
            {
                case ChiAttribute.CriticalStrike: return 10;
                case ChiAttribute.SkillCriticalStrike: return 10;
                case ChiAttribute.Immunity: return 10;
                case ChiAttribute.Breakthrough: return 10;
                case ChiAttribute.Counteraction: return 10;
                case ChiAttribute.HPAdd: return 1000;
                case ChiAttribute.AddAttack: return 500;
                case ChiAttribute.AddMagicAttack: return 500;
                case ChiAttribute.AddMagicDefense: return 50;
                case ChiAttribute.PhysicalDamageIncrease: return 50;
                case ChiAttribute.PhysicalDamageDecrease: return 50;
                case ChiAttribute.MagicDamageIncrease: return 50;
                case ChiAttribute.MagicDamageDecrease: return 50;
                default: return 10;
            }
        }
        public static int ChiMaxValues(ChiAttribute attribute)
        {
            switch (attribute)
            {
                case ChiAttribute.CriticalStrike: return 200;
                case ChiAttribute.SkillCriticalStrike: return 200;
                case ChiAttribute.Immunity: return 200;
                case ChiAttribute.Breakthrough: return 200;
                case ChiAttribute.Counteraction: return 200;
                case ChiAttribute.HPAdd: return 3500;
                case ChiAttribute.AddAttack: return 2000;
                case ChiAttribute.AddMagicAttack: return 2500;
                case ChiAttribute.AddMagicDefense: return 250;
                case ChiAttribute.PhysicalDamageIncrease: return 500;
                case ChiAttribute.PhysicalDamageDecrease: return 500;
                case ChiAttribute.MagicDamageIncrease: return 300;
                case ChiAttribute.MagicDamageDecrease: return 300;
                default: return 100;
            }
        }

        public enum ChiPowerType
        {
            None = 0,
            Dragon = 1,
            Phoenix,
            Tiger,
            Turtle
        }


        public enum CountryID : ushort
        {
            Default = 0,
            Australia = 1,
            Brazil = 2,
            Canada = 3,
            Conquer = 4,
            Egypt = 5,
            France = 6,
            Macedonia = 7,
            Germany = 8,
            SaudiArabia = 9,
            Malaysia = 10,
            Mexico = 11,
            Nederlands = 12,
            Philippines = 13,
            Poland = 14,
            Portugal = 15,
            Romania = 16,
            Singapore = 17,
            Sweden = 18,
            Turkey = 19,
            UK = 20,
            USA = 21,
            Vietnam = 22
        }
        public enum GuildMemberRank : ushort
        {
            GuildLeader = 1000,
            DeputyLeader = 990,
            HDeputyLeader = 980,
            LeaderSpouse = 920,
            Manager = 890,
            HonoraryManager = 880,
            TSupervisor = 859,
            OSupervisor = 858,
            CPSupervisor = 857,
            ASupervisor = 856,
            SSupervisor = 855,
            GSupervisor = 854,
            PKSupervisor = 853,
            RoseSupervisor = 852,
            LilySupervisor = 851,
            Supervisor = 850,
            HonorarySuperv = 840,
            Steward = 690,
            HonorarySteward = 680,
            DeputySteward = 650,
            DLeaderSpouse = 620,
            DLeaderAide = 611,
            LSpouseAide = 610,
            Aide = 602,
            TulipAgent = 599,
            OrchidAgent = 598,
            CPAgent = 597,
            ArsenalAgent = 596,
            SilverAgent = 595,
            GuideAgent = 594,
            PKAgent = 593,
            RoseAgent = 592,
            LilyAgent = 591,
            Agent = 590,
            SupervSpouse = 521,
            ManagerSpouse = 520,
            SupervisorAide = 511,
            ManagerAide = 510,
            TulipFollower = 499,
            OrchidFollower = 498,
            CPFollower = 497,
            ArsFollower = 496,
            SilverFollower = 495,
            GuideFollower = 494,
            PKFollower = 493,
            RoseFollower = 492,
            LilyFollower = 491,
            Follower = 490,
            StewardSpouse = 420,
            SeniorMember = 210,
            Member = 200
        }
        public enum PKMode : byte
        {
            PK = 0,
            Peace = 1,
            Team = 2,
            Capture = 3,
            Jiang = 7,
            Guild = 6,
            Bahaa = 4,
        }  
        public class ConquerAction
        {
            public const ushort
            None = 0x00,
            Cool = 0xE6,
            Kneel = 0xD2,
            Sad = 0xAA,
            Happy = 0x96,
            Angry = 0xA0,
            Lie = 0x0E,
            Dance = 0x01,
            Wave = 0xBE,
            Bow = 0xC8,
            Sit = 0xFA,
            Jump = 0x64;
        }

        public enum ConquerAngle : byte
        {
            SouthWest = 0,
            West = 1,
            NorthWest = 2,
            North = 3,
            NorthEast = 4,
            East = 5,
            SouthEast = 6,
            South = 7
        }
        public static ConquerAngle OppositeAngle(ConquerAngle angle)
        {
            byte myAngle = (byte)angle;
            if (myAngle > 3)
                return (ConquerAngle)(myAngle - 4);
            else
                return (ConquerAngle)(myAngle + 4);
        }
        public enum NpcType : byte
        {
            Stun = 0,
            Shop = 1,
            Talker = 2,
            Beautician = 5,
            Upgrader = 6,
            Socketer = 7,
            Booth = 14,
            Gambling = 19,
            Stake = 21,
            Scarecrow = 22,
            Furniture = 25,
            Poker = 33,
            Poker2 = 34,
            Poker3 = 35,
            Poker4 = 36,
            Poker5 = 37,
            Poker6 = 38,
            ClanInfo = 31
        }
        public enum Mode : byte
        {
            None, Recording
        }

        public enum ItemMode : byte
        {
            Default = 1,
            Trade = 2,
            Update = 3,
            View = 4,
            ChatItem = 9,
        }
        public enum ItemEffect : byte
        {
            None = 0,
            Poison = 0xC8,
            HP = 0xC9,
            MP = 0xCA,
            Shield = 0xCB,
            Horse = 0x64
        }
        public enum Color : byte
        {
            Black = 2,
            Orange = 3,
            LightBlue = 4,
            Red = 5,
            Blue = 6,
            Yellow = 7,
            Purple = 8,
            White = 9
        }
        public enum StatusFlag : int
        {
            None = 0,
            Poison = 1,
            XpSkill = 4,
            Ghost = 5,
            TeamLeader = 6,
            StarOfAccurracy = 7,
            MagicShield = 8,
            Stigma = 9,
            Dead = 10,
            FadeAway = 11,
            FlashingName = 12,
            RedName = 14,
            BlackName = 15,
            SetFree = 16,
            Superman = 18,
            Blessing = 20,
            Cyclone = 23,
            Dodge = 26,
            Fly = 27,
            CastPray = 30,
            Praying = 31,
            Cursed = 32,
            TopGuildLeader = 34,
            TopDeputyLeader = 35,
            MontlyPkChampion = 36,
            WeeklyPkChampion = 37,
            TopWarrior = 38,
            TopTrojan = 39,
            TopArcher = 40,
            TopWaterTaoist = 41,
            TopFireTaoist = 42,
            TopNinja = 43,
            ShurikenVortex = 46,
            FatalStrike = 47,
            Rebirth3 = 48,
            Ride = 50,
            TopSpouse = 51,
            Unk1 = 52,
            PoisonStar = 53,
            Unk2 = 55,
            LionShields = 57,
            IcePrision = 59,
            ChainBolt = 91,
            AzureShield = 92,
            TyrantAura = 97,
            FiendAura = 99,
            MetalAura = 101,
            WoodAura = 103,
            WaterAura = 105,
            FireAura = 107,
            EarthAura = 109,
            soulshackle = 110,
            Oblivion = 111,
            TopMonk = 113
        }
        public enum ItemUse
        {
            None, Add, CreateAndAdd, Move, Remove, Delete
        }

        public enum ItemQuality : byte
        {
            Fixed = 0,
            Normal = 2,
            NormalV1 = 3,
            NormalV2 = 4,
            NormalV3 = 5,
            Refined = 6,
            Unique = 7,
            Elite = 8,
            Super = 9,
            Other = 1
        }

        public enum Gem : byte
        {
            NormalPhoenixGem = 1,
            RefinedPhoenixGem = 2,
            SuperPhoenixGem = 3,

            NormalDragonGem = 11,
            RefinedDragonGem = 12,
            SuperDragonGem = 13,

            NormalFuryGem = 21,
            RefinedFuryGem = 22,
            SuperFuryGem = 23,

            NormalRainbowGem = 31,
            RefinedRainbowGem = 32,
            SuperRainbowGem = 33,

            NormalKylinGem = 41,
            RefinedKylinGem = 42,
            SuperKylinGem = 43,

            NormalVioletGem = 51,
            RefinedVioletGem = 52,
            SuperVioletGem = 53,

            NormalMoonGem = 61,
            RefinedMoonGem = 62,
            SuperMoonGem = 63,

            NormalTortoiseGem = 71,
            RefinedTortoiseGem = 72,
            SuperTortoiseGem = 73,

            NormalThunderGem = 101,
            RefinedThunderGem = 102,
            SuperThunderGem = 103,

            NormalGloryGem = 121,
            RefinedGloryGem = 122,
            SuperGloryGem = 123,

            NoSocket = 0,
            EmptySocket = 255
        }

        public enum SkillIDs : ushort
        {
            Bless = 9876,
            Riding = 7001,
            Spook = 7002,
            WarCry = 7003,
            FlashStep = 4550,
            TwofoldBlades = 6000,
            ToxicFog = 6001,
            PoisonStar = 6002,
            CounterKill = 6003,
            ArcherBane = 6004,
            TwilightDance = 12070,
            SuperTwofoldBlade = 12080,
            FatalSpin = 12110,
            SpeedKick = 12120,
            ViolentKick = 12130,
            StormKick = 12140,
            CrackingSwip = 12160,
            SplittingSwipe = 12170,
            DragonSwing = 12200,
            DragonPunch = 12240,
            DragonSlash = 12350,
            DragonFlow = 12270,
            DragonRoar = 12280,
            DragonCyclone = 12290,
            AirKick = 12320,
            AirSweep = 12330,
            AirRaid = 12340,
            DragonFury = 12300,
            ShurikenVortex = 6010,
            FatalStrike = 6011,
            Accuracy = 1015,
            Cyclone = 1110,
            Hercules = 1115,
            SpiritHealing = 1190,
            Shield = 1020,
            SuperMan = 1025,
            Roar = 1040,
            Dash = 1051,
            FlyingMoon = 1320,
            Scatter = 8001,
            RapidFire = 8000,
            AdvancedFly = 8003,
            Intensify = 9000,
            XPFly = 8002,
            ArrowRain = 8030,
            Thunder = 1000,
            Cure = 1005,
            Fire = 1001,
            Meditation = 1195,
            FireRing = 1150,
            FireMeteor = 1180,
            FireCircle = 1120,
            Tornado = 1002,
            Bomb = 1160,
            FireOfHell = 1165,
            Lightning = 1010,
            Volcano = 1125,
            SpeedLightning = 5001,
            HealingRain = 1055,
            StarOfAccuracy = 1085,
            MagicShield = 1090,
            Stigma = 1095,
            Invisibility = 1075,
            Pray = 1100,
            AdvancedCure = 1175,
            Nectar = 1170,
            XPRevive = 1050,
            FastBlade = 1045,
            ScentSword = 1046,
            Phoenix = 5030,
            WideStrike = 1250,
            Boreas = 5050,
            Snow = 5010,
            StrandedMonster = 5020,
            SpeedGun = 1260,
            Penetration = 1290,
            Boom = 5040,
            Halt = 1300,
            Seizer = 7000,
            EarthQuake = 7010,
            Rage = 7020,
            Celestial = 7030,
            Roamer = 7040,
            Robot = 1270,
            SuperCyclone = 11970,
            FatalCross = 11980,
            MortalStrike = 11990,
            BreathFocus = 11960,
            WaterElf = 1280,
            DivineHare = 1350,
            NightDevil = 1360,
            Reflect = 3060,
            CruelShade = 3050,
            Dodge = 3080,
            Pervade = 3090,
            ViperFang = 11005,
            DragonTail = 11000,
            SummonGuard = 4000,
            FireEvil = 4060,
            BloodyBat = 4050,
            Skeleton = 4070,
            SummonBat = 4010,
            SummonBatBoss = 4020,
            Dance2 = 1380,
            DragonBreath = 7014,
            DragonBreath2 = 7017,
            DragonBreath3 = 7015,
            DragonBreath4 = 7011,
            DragonBreath5 = 7012,
            Dance3 = 1385,
            Dance4 = 1390,
            Dance5 = 1395,
            Dance6 = 1400,
            Dance7 = 1405,
            Dance8 = 1410,
            Restore = 1105,
            DragonWhirl = 10315,
            HeavenBlade = 10310,
            StarArrow = 10313,
            AzureShield = 30000,
            RadiantPalm = 10381,
            Oblivion = 10390,
            TyrantAura = 10395,
            Serenity = 10400,
            soulshackle = 10405,
            DeflectionAura = 10410,
            Perseverance = 10311,
            WhirlWindKick = 10415,
            Tranquility = 10425,
            Compassion = 10430,
            AuraMetal = 10420,
            AuraWood = 10421,
            AuraWater = 10422,
            AuraFire = 10423,
            AuraEarth = 10424,
            TripleAttack = 10490,
            IronShirt = 5100,
            DeathBlow = 10484,
            BladeTempest = 11110, //FatleStrike Same
            ScurvyBomb = 11040, // same bomb
            CannonBarrage = 11050, //same volcano
            BlackbeardRage = 11060,
            GaleBomb = 11070,
            KrakensRevenge = 11100,
            BlackSpot = 11120,
            AdrenalineRush = 11130,
            PiEagleEye = 11030
        }
        public enum RaceItemType : ushort
        {
            Null = 8329,
            ChaosBomb = 8330,
            SpiritPotion = 8331,
            ExcitementPotion = 8332,
            FrozenTrap = 8333,
            ScreamBomb = 8334,
            SluggishPotion = 8335,
            GuardPotion = 8336,
            DizzyHammer = 8337,
            TransformItem = 8338,
            RestorePotion = 8339,
            SuperExcitementPotion = 8340,
        }
        public enum Maps : uint
        {
            MarketRace = 1950, // Start: 88, 149 End: 420, 431
            IceRace = 2064, //Start: 175,250 End: 200,170
            DungeonRace = 2063, //Start: 450, 520 End: 690,450
            LavaRace = 2062, // Start: 150,350 End:320,160
            IslandRace = 2061, // Start: 60,400 End: 870,830

            Desert = 1000,
            AncientMaze = 1001,
            TwinCity = 1002,
            Mine = 1003,
            Promotion = 1004,
            Arena = 1005,
            Stables = 1006,
            BirthVillage = 1010,
            PhoenixCastle = 1011,
            RebirthMap = 1012,
            HalkingCave = 1013,
            BanditCave = 1014,
            BirdIsland = 1015,
            TombBatCave = 1016,
            AdvanceZone = 1017,
            ArmorColour = 1008,
            ApeMoutain = 1020,
            DesertCity = 1000,
            Moonspring = 1000,
            WaterFallCave = 1011,
            MapleForest = 1011,
            LoveCanyon = 1020,
            ReedsIsland = 1015,
            DreamLand = 1012,
            WonderLand = 1013,
            DragonPool = 1014,
            KylinCave = 1016,
            Market = 1036,
            star01 = 1100,
            star02 = 1101,
            star03 = 1102,
            star04 = 1103,
            star05 = 1104,
            star10 = 1105,
            star06 = 1106,
            star07 = 1107,
            star08 = 1108,
            star09 = 1109,
            smith = 1007,
            grocery2 = 1009,
            parena = 1018,
            larena = 1019,
            mine = 1021,
            brave = 1022,
            mineone = 1025,
            minetwo = 1026,
            minethree = 1027,
            minefour = 1028,
            mineone2 = 1029,
            minetwo2 = 1030,
            minethree2 = 1031,
            minefour2 = 1032,
            newbie2 = 1035,
            mineone3 = 5000,
            prison = 6000,
            factionblack = 1037,
            faction = 1038,
            playground = 1039,
            skycut = 1040,
            skymaze = 1041,
            prison2 = 6001,
            lineuppass = 1042,
            lineup = 1043,
            lineup2 = 1044,
            lineup3 = 1045,
            lineup4 = 1046,
            lineup5 = 1047,
            lineup6 = 1048,
            lineup7 = 1049,
            lineup8 = 1050,
            riskisland = 1051,
            skymaze1 = 1060,
            skymaze2 = 1061,
            skymaze3 = 1062,
            star = 1064,
            boa = 1070,
            parena1 = 1080,
            parena2 = 1081,
            newcanyon = 1075,
            newwoods = 1076,
            newdesert = 1077,
            newisland = 1078,
            mysisland = 1079,
            riskisland1 = 1063,
            idlandmap = 1082,
            parenam = 1090,
            parenas = 1091,
            house01 = 1098,
            house03 = 1099,
            sanctuary = 1601,
            task01 = 1201,
            task02 = 1202,
            task04 = 1204,
            task05 = 1205,
            task07 = 1207,
            task08 = 1208,
            task10 = 1210,
            task11 = 1211,
            islandsnail = 1212,
            desertsnail = 1213,
            canyonfairy = 1214,
            woodsfairy = 1215,
            newplainfairy = 1216,
            minea = 1500,
            mineb = 1501,
            minec = 1502,
            mined = 1503,
            stask01 = 1351,
            stask02 = 1352,
            stask03 = 1353,
            stask04 = 1354,
            slpk = 1505,
            hhpk = 1506,
            blpk = 1507,
            ympk = 1508,
            mfpk = 1509,
            faction01 = 1550,
            faction012 = 1551,
            grocery3 = 1510,
            forum1 = 1511,
            tiger1 = 1512,
            jokul01 = 1615,
            tiemfiles = 1616,
            tiemfiles1 = 1617,
            Dgate = 2021,
            Dsquare = 2022,
            Dcloister = 2023,
            Dsigil = 2024,
            cordiform = 1645,
            faction2 = 1560,
            faction3 = 1561,
            forum2 = 1707,
            Gulf = 1700
        }

        public enum ArenaIDs : uint
        {
            ShowPlayerRankList = 0xA,
            QualifierList = 0x6
        }

        public enum ArenaSignUpStatusIDs : uint
        {
            NotSignedUp = 0,
            WaitingForOpponent = 1,
            WaitingInactive = 2,
        }
        public enum Update : uint
        {
            HP = 0,
            MaxHitpoints = 1,
            Mana = 2,
            MaxMana = 3,
            Money = 4,
            Experience = 5,
            PkPt = 6,
            Job = 7,
            Stam = 8,
            WarehouseMoney = 9,
            Stats = 10,
            Mesh = 11,
            Level = 12,
            Spirit = 13,
            Vitality = 14,
            Strength = 15,
            Agility = 16,
            HeavensBlessing = 17,
            DoubleExpTimer = 18,
            CursedTimer = 20,
            Reborn = 22,
            StatusEffect = 25,
            Hair = 26,
            XPPct = 27,
            LuckyTimeTimer = 28,
            CP = 29,
            OnlineTraining = 31,
            ExtraBattlePower = 36,
            Merchant = 38,
            VIPLevel = 39,
            QuizPoints = 40,
            EnlightPoints = 41,
            BonusBP = 44,
            BoundCp = 45
        }
        public enum ArenaSignUpIDs : uint
        {
            ShowQuitButton = 0,
            SignUp = 6,
            StartCountDown = 2,
            Dialog = 0xA,
            DoTheStuff = 8
        }
    }
}
