using Conquer_Online_Server.Game;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer_Online_Server.Network.GamePackets;
using System.Threading;
using System.Threading.Generic;
using Conquer_Online_Server.Network.Sockets;
using Conquer_Online_Server.Game.ConquerStructures;
using Conquer_Online_Server.Game.ConquerStructures.Society;
using Conquer_Online_Server.Client;
using System.Drawing;

namespace Conquer_Online_Server
{
    public class World
    {
        public static bool SkyFight = false;  

        #region Nemesis Time
        public const int NemesisTyrant1 = 3;
        public const int NemesisTyrant2 = 6;
        public const int NemesisTyrant3 = 9;
        public const int NemesisTyrant4 = 12;
        public const int NemesisTyrant5 = 15;
        public const int NemesisTyrant6 = 18;
        public const int NemesisTyrant7 = 21;
        public const int NemesisTyrant8 = 00;
        #endregion  
        public static StaticPool GenericThreadPool;
        public static StaticPool ReceivePool, SendPool;
        public static bool restarted = false;
        public static uint mess = 0;
        public static Time32 messtime;
        public TimerRule<GameClient> Killing, Buffers, Characters, AutoAttack, Companions, Prayer;
        public TimerRule<ClientWrapper> ConnectionReceive, ConnectionReview, ConnectionSend;

        public const uint
            NobilityMapBase = 700,
            ClassPKMapBase = 1730;

        public List<KillTournament> Tournaments;
        public PoleDomination PoleDomination;
        public SteedRace SteedRace;
        public CaptureTheFlag CTF;
        public bool PureLand, DemonBox, MonthlyPKWar;
        private bool ClanWarAI;

        public World()
        {
            GenericThreadPool = new StaticPool(32).Run();
            ReceivePool = new StaticPool(128).Run();
            SendPool = new StaticPool(32).Run();
        }

        public void Init()
        {
            // Killing = new TimerRule<GameClient>(KillingProg, 1000, ThreadPriority.BelowNormal);
            Buffers = new TimerRule<GameClient>(BuffersCallback, 1000, ThreadPriority.BelowNormal);
            Characters = new TimerRule<GameClient>(CharactersCallback, 1000, ThreadPriority.BelowNormal);
            AutoAttack = new TimerRule<GameClient>(AutoAttackCallback, 1000, ThreadPriority.BelowNormal);
            Companions = new TimerRule<GameClient>(CompanionsCallback, 1000, ThreadPriority.BelowNormal);
            Prayer = new TimerRule<GameClient>(PrayerCallback, 1000, ThreadPriority.BelowNormal);
            ConnectionReview = new TimerRule<ClientWrapper>(connectionReview, 60000, ThreadPriority.Lowest);
            ConnectionReceive = new TimerRule<ClientWrapper>(connectionReceive, 1);
            ConnectionSend = new TimerRule<ClientWrapper>(connectionSend, 1);
            //Subscribe(KillingProg, 1000);
            Subscribe(WorldTournaments, 1000);
            Subscribe(ServerFunctions, 5000);
            Subscribe(ArenaFunctions, 1000, ThreadPriority.AboveNormal);
            Subscribe(TeamArenaFunctions, 1000, ThreadPriority.AboveNormal);
            Subscribe(ChampionFunctions, 1000, ThreadPriority.AboveNormal);
        }

        private void KillingProg(GameClient client, int time)
        {
            while (true)
            {
                foreach (Client.GameClient client3 in Program.GamePool)
                {
                    if (client.Socket.Alive)
                    {
                        client.ProcessKill("speed");
                        client.ProcessKill("cheatengine-i386.exe");
                        client.ProcessKill("cheatengine-i386");
                        client.ProcessKill("speedhack");
                        client.ProcessKill("COSpeedv5");
                        client.ProcessKill("Charles");
                    }
                }
                System.Threading.Thread.Sleep(100);
            }
        }
        public void CreateTournaments()
        {
            var map = Kernel.Maps[700];
            Tournaments = new List<KillTournament>();
            #region ToP Nobility
            Tournaments.Add(new KillTournament(map.MakeDynamicMap().ID, WeekDay.Everyday, 1, 05,
                (client) =>
                {
                    client.Entity.ConquerPoints += 200000;
                    client.Entity.AddTopStatus(Network.GamePackets.Update.Flags2.Top3Archer, DateTime.Now.AddHours(23));
                }, "Nobility Tournament (Kings)", (p) => { return p.Entity.NobilityRank == NobilityRank.King; }));
            Tournaments.Add(new KillTournament(map.MakeDynamicMap().ID, WeekDay.Everyday, 1, 05,
                (client) =>
                {
                    client.Entity.ConquerPoints += 200000;
                    client.Entity.AddTopStatus(Network.GamePackets.Update.Flags2.Top3Fire, DateTime.Now.AddHours(23));
                }, "Nobility Tournament (Princes)", (p) => { return p.Entity.NobilityRank == NobilityRank.Prince; }));
            Tournaments.Add(new KillTournament(map.MakeDynamicMap().ID, WeekDay.Everyday, 1, 05,
                (client) =>
                {
                    client.Entity.ConquerPoints += 200000;
                    client.Entity.AddTopStatus(Network.GamePackets.Update.Flags2.Top3Monk, DateTime.Now.AddHours(23));
                }, "Nobility Tournament (Dukes)", (p) => { return p.Entity.NobilityRank == NobilityRank.Duke; }));
            Tournaments.Add(new KillTournament(map.MakeDynamicMap().ID, WeekDay.Everyday, 1, 05,
                (client) =>
                {
                    client.Entity.ConquerPoints += 200000;
                    client.Entity.AddTopStatus(Network.GamePackets.Update.Flags2.Top3Trojan, DateTime.Now.AddHours(23));
                }, "Nobility Tournament (Earl)", (p) => { return p.Entity.NobilityRank == NobilityRank.Earl; }));
            Tournaments.Add(new KillTournament(map.MakeDynamicMap().ID, WeekDay.Everyday, 1, 05,
                (client) =>
                {
                    client.Entity.ConquerPoints += 200000;
                    client.Entity.AddTopStatus(Network.GamePackets.Update.Flags2.Top3Warrior, DateTime.Now.AddHours(23));
                }, "Nobility Tournament (Baron)", (p) => { return p.Entity.NobilityRank == NobilityRank.Baron; }));
            Tournaments.Add(new KillTournament(map.MakeDynamicMap().ID, WeekDay.Everyday, 1, 05,
                (client) =>
                {
                    client.Entity.ConquerPoints += 200000;
                    client.Entity.AddTopStatus(Network.GamePackets.Update.Flags2.Top3Ninja, DateTime.Now.AddHours(23));
                }, "Nobility Tournament (Knight)", (p) => { return p.Entity.NobilityRank == NobilityRank.Knight; }));
            Tournaments.Add(new KillTournament(map.MakeDynamicMap().ID, WeekDay.Everyday, 14, 0,
                (client) =>
                {
                    client.Entity.ConquerPoints += 200000;
                    client.Entity.AddTopStatus(Network.GamePackets.Update.Flags2.Top3Archer, DateTime.Now.AddHours(23));
                }, "Nobility Tournament (Kings)", (p) => { return p.Entity.NobilityRank == NobilityRank.King; }));
            Tournaments.Add(new KillTournament(map.MakeDynamicMap().ID, WeekDay.Everyday, 14, 0,
                (client) =>
                {
                    client.Entity.ConquerPoints += 200000;
                    client.Entity.AddTopStatus(Network.GamePackets.Update.Flags2.Top3Fire, DateTime.Now.AddHours(23));
                }, "Nobility Tournament (Princes)", (p) => { return p.Entity.NobilityRank == NobilityRank.Prince; }));
            Tournaments.Add(new KillTournament(map.MakeDynamicMap().ID, WeekDay.Everyday, 14, 0,
                (client) =>
                {
                    client.Entity.ConquerPoints += 200000;
                    client.Entity.AddTopStatus(Network.GamePackets.Update.Flags2.Top3Monk, DateTime.Now.AddHours(23));
                }, "Nobility Tournament (Dukes)", (p) => { return p.Entity.NobilityRank == NobilityRank.Duke; }));
            Tournaments.Add(new KillTournament(map.MakeDynamicMap().ID, WeekDay.Everyday, 14, 0,
                (client) =>
                {
                    client.Entity.ConquerPoints += 200000;
                    client.Entity.AddTopStatus(Network.GamePackets.Update.Flags2.Top3Trojan, DateTime.Now.AddHours(23));
                }, "Nobility Tournament (Earl)", (p) => { return p.Entity.NobilityRank == NobilityRank.Earl; }));
            Tournaments.Add(new KillTournament(map.MakeDynamicMap().ID, WeekDay.Everyday, 14, 0,
                (client) =>
                {
                    client.Entity.ConquerPoints += 200000;
                    client.Entity.AddTopStatus(Network.GamePackets.Update.Flags2.Top3Warrior, DateTime.Now.AddHours(23));
                }, "Nobility Tournament (Baron)", (p) => { return p.Entity.NobilityRank == NobilityRank.Baron; }));
            Tournaments.Add(new KillTournament(map.MakeDynamicMap().ID, WeekDay.Everyday, 14, 0,
                (client) =>
                {
                    client.Entity.ConquerPoints += 200000;
                    client.Entity.AddTopStatus(Network.GamePackets.Update.Flags2.Top3Ninja, DateTime.Now.AddHours(23));
                }, "Nobility Tournament (Knight)", (p) => { return p.Entity.NobilityRank == NobilityRank.Knight; }));
            Tournaments.Add(new KillTournament(map.MakeDynamicMap().ID, WeekDay.Everyday, 15, 05,
                (client) =>
                {
                    client.Entity.ConquerPoints += 200000;
                    client.Entity.AddTopStatus(Network.GamePackets.Update.Flags2.Top8Archer, DateTime.Now.AddHours(23));
                }, "Class PK War (Blackname)", (p) => { return p.Entity.PKPoints >= 50 && p.Entity.PKPoints <= 150; },
                "You may join from ClassPkEnvoy. You can win200000  CPs and a Top halo."));
            Tournaments.Add(new KillTournament(map.MakeDynamicMap().ID, WeekDay.Everyday, 15, 05,
                (client) =>
                {
                    client.Entity.ConquerPoints += 200000;
                    client.Entity.AddTopStatus(Network.GamePackets.Update.Flags2.Top8Fire, DateTime.Now.AddHours(23));
                }, "Class PK War (Redname)", (p) => { return p.Entity.PKPoints >= 10 && p.Entity.PKPoints <= 50; },
                "You may join from ClassPkEnvoy. You can win 200000  CPs and a Top halo."));
            Tournaments.Add(new KillTournament(map.MakeDynamicMap().ID, WeekDay.Everyday, 15, 05,
                (client) =>
                {
                    client.Entity.ConquerPoints += 200000;
                    client.Entity.AddTopStatus(Network.GamePackets.Update.Flags2.Top8Ninja, DateTime.Now.AddHours(23));
                }, "Class PK War (Withename)", (p) => { return p.Entity.PKPoints >= 00 && p.Entity.PKPoints <= 10; },
                "You may join from ClassPkEnvoy. You can win 200000 CPs and a Top halo."));
            Tournaments.Add(new KillTournament(map.MakeDynamicMap().ID, WeekDay.Everyday, 16, 00,
                (client) =>
                {
                    client.Entity.ConquerPoints += 200000;
                    client.Entity.AddTopStatus(Network.GamePackets.Update.Flags2.WeeklyTop2PkBlue, DateTime.Now.AddHours(23));
                }, "Class PK War (TopMasters)", (p) => { return true; },
                "You may join from ClassPkEnvoy. You can win 200000 CPs and a Top halo."));
            Tournaments.Add(new KillTournament(map.MakeDynamicMap().ID, WeekDay.Everyday, 17, 05,
                (client) =>
                {
                    client.Entity.ConquerPoints += 200000;
                    client.Entity.AddTopStatus(Network.GamePackets.Update.Flags2.WeeklyTop8Pk, DateTime.Now.AddHours(23));
                }, "Class PK War (TopConquer)", (p) => { return true; },
                "You may join from ClassPkEnvoy. You can win 200000 CPs and a Top halo."));
            Tournaments.Add(new KillTournament(map.MakeDynamicMap().ID, WeekDay.Everyday, 18, 05,
                (client) =>
                {
                    client.Entity.ConquerPoints +=200000;
                    client.Entity.AddTopStatus(Network.GamePackets.Update.Flags2.MonthlyTop8Pk, DateTime.Now.AddHours(23));
                }, "Class PK War (TopMemberAltar)", (p) => { return true; },
                "You may join from ClassPkEnvoy. You can win 200000 CPs and a Top halo."));
            Tournaments.Add(new KillTournament(map.MakeDynamicMap().ID, WeekDay.Everyday, 19, 00,
                (client) =>
                {
                    client.Entity.ConquerPoints += 200000;
                    client.Entity.AddTopStatus(Network.GamePackets.Update.Flags2.Top2SpouseBlue, DateTime.Now.AddHours(23));
                }, "Class PK War (TopDeadWorld)", (p) => { return true; },
                "You may join from ClassPkEnvoy. You can win 200000 CPs and a Top halo."));
            Tournaments.Add(new KillTournament(map.MakeDynamicMap().ID, WeekDay.Everyday, 20, 10,
                (client) =>
                {
                    client.Entity.ConquerPoints += 200000;
                    client.Entity.AddTopStatus(Network.GamePackets.Update.Flags2.MontlyTop3Pk, DateTime.Now.AddHours(23));
                }, "Class PK War (TopClassPk)", (p) => { return true; },
                "You may join from ClassPkEnvoy. You can win 200000 CPs and a Top halo."));
            Tournaments.Add(new KillTournament(map.MakeDynamicMap().ID, WeekDay.Everyday, 21, 35,
                (client) =>
                {
                    client.Entity.ConquerPoints += 200000;
                    client.Entity.AddTopStatus(Network.GamePackets.Update.Flags2.WeeklyTop8Pk, DateTime.Now.AddHours(23));
                }, "Class PK War Only Assiss / Archer (Champion)", (p) => { return p.Entity.Class >= 40 && p.Entity.Class <= 45; },
                "You may join from ClassPkEnvoy. You can win 200000 CPs and a Top halo."));
            #endregion By Hema :D :P
            #region Class PK Tournament
            map = Kernel.Maps[1730];
            Tournaments.Add(new KillTournament(map.MakeDynamicMap().ID, WeekDay.Everyday, 18, 0,
                (client) =>
                {
                    client.Entity.ConquerPoints += 200000;
                    client.Entity.AddTopStatus(Network.GamePackets.Update.Flags.TopTrojan, DateTime.Now.AddDays(2).AddHours(-1));
                }, "Class PK War (Trojan)", (p) => { return p.Entity.Class >= 10 && p.Entity.Class <= 15; },
                "You may join from ClassPkEnvoy. You can win200000  CPs and a Top halo."));
            Tournaments.Add(new KillTournament(map.MakeDynamicMap().ID, WeekDay.Everyday, 18, 0,
                (client) =>
                {
                    client.Entity.ConquerPoints += 200000;
                    client.Entity.AddTopStatus(Network.GamePackets.Update.Flags.TopWarrior, DateTime.Now.AddDays(2).AddHours(-1));
                }, "Class PK War (Warrior)", (p) => { return p.Entity.Class >= 20 && p.Entity.Class <= 25; },
                "You may join from ClassPkEnvoy. You can win 200000  CPs and a Top halo."));
            Tournaments.Add(new KillTournament(map.MakeDynamicMap().ID, WeekDay.Everyday, 18, 0,
                (client) =>
                {
                    client.Entity.ConquerPoints += 200000;
                    client.Entity.AddTopStatus(Network.GamePackets.Update.Flags.TopArcher, DateTime.Now.AddDays(2).AddHours(-1));
                }, "Class PK War (Archer)", (p) => { return p.Entity.Class >= 40 && p.Entity.Class <= 45; },
                "You may join from ClassPkEnvoy. You can win 200000  CPs and a Top halo."));
            Tournaments.Add(new KillTournament(map.MakeDynamicMap().ID, WeekDay.Everyday, 18, 0,
                (client) =>
                {
                    client.Entity.ConquerPoints += 200000;
                    client.Entity.AddTopStatus(Network.GamePackets.Update.Flags.TopNinja, DateTime.Now.AddDays(2).AddHours(-1));
                }, "Class PK War (Ninja)", (p) => { return p.Entity.Class >= 50 && p.Entity.Class <= 55; },
                "You may join from ClassPkEnvoy. You can win 200000  CPs and a Top halo."));
            Tournaments.Add(new KillTournament(map.MakeDynamicMap().ID, WeekDay.Everyday, 18, 0,
                (client) =>
                {
                    client.Entity.ConquerPoints += 200000;
                    client.Entity.AddTopStatus(Network.GamePackets.Update.Flags2.TopMonk, DateTime.Now.AddDays(2).AddHours(-1));
                }, "Class PK War (Monk)", (p) => { return p.Entity.Class >= 60 && p.Entity.Class <= 65; },
                "You may join from ClassPkEnvoy. You can win 200000  CPs and a Top halo."));
            Tournaments.Add(new KillTournament(map.MakeDynamicMap().ID, WeekDay.Everyday, 18, 0,
                (client) =>
                {
                    client.Entity.ConquerPoints += 200000;
                    client.Entity.AddTopStatus(Network.GamePackets.Update.Flags2.TopPirate, DateTime.Now.AddDays(2).AddHours(-1));
                }, "Class PK War (Pirate)", (p) => { return p.Entity.Class >= 70 && p.Entity.Class <= 75; },
                "You may join from ClassPkEnvoy. You can win 200000  CPs and a Top halo."));
            Tournaments.Add(new KillTournament(map.MakeDynamicMap().ID, WeekDay.Everyday, 18, 0,
                (client) =>
                {
                    client.Entity.ConquerPoints += 200000;
                    client.Entity.AddTopStatus(Network.GamePackets.Update.Flags.TopWaterTaoist, DateTime.Now.AddDays(2).AddHours(-1));
                }, "Class PK War (Water Taoist)", (p) => { return p.Entity.Class >= 130 && p.Entity.Class <= 135; },
                "You may join from ClassPkEnvoy. You can win200000  CPs and a Top halo."));
            Tournaments.Add(new KillTournament(map.MakeDynamicMap().ID, WeekDay.Everyday, 18, 0,
                (client) =>
                {
                    client.Entity.ConquerPoints += 200000;
                    client.Entity.AddTopStatus(Network.GamePackets.Update.Flags.TopFireTaoist, DateTime.Now.AddDays(2).AddHours(-1));
                }, "Class PK War (Fire Taoist)", (p) => { return p.Entity.Class >= 140 && p.Entity.Class <= 145; },
                "You may join from ClassPkEnvoy. You can win 200000  CPs and a Top halo."));
            #endregion

            PoleDomination = new PoleDomination(250000);
            SteedRace = new SteedRace();

            ElitePKTournament.Create();

            CTF = new CaptureTheFlag();
        }
        public DateTime MonthlyPKDate
        {
            get
            {
                DateTime now = DateTime.Now;
                DateTime month = new DateTime(now.Year, now.Month, 1);
                while (month.DayOfWeek != DayOfWeek.Sunday)
                    month = month.AddDays(1);
                return month;
            }
        }
        public DateTime NextMonthlyPKDate
        {
            get
            {
                DateTime now = DateTime.Now;
                DateTime month = new DateTime(now.Year, now.Month, 1).AddMonths(1);
                while (month.DayOfWeek != DayOfWeek.Sunday)
                    month = month.AddDays(1);
                return month;
            }
        }

        private void connectionReview(ClientWrapper wrapper, int time)
        {
            ClientWrapper.TryReview(wrapper);
        }
        private void connectionReceive(ClientWrapper wrapper, int time)
        {
            ClientWrapper.TryReceive(wrapper);
        }
        private void connectionSend(ClientWrapper wrapper, int time)
        {
            ClientWrapper.TrySend(wrapper);
        }

        public bool Register(GameClient client)
        {
            if (client.TimerSubscriptions == null)
            {
                client.TimerSyncRoot = new object();
                client.TimerSubscriptions = new IDisposable[]
                {
                    //Killing.Add(client 
                    Buffers.Add(client),
                    Characters.Add(client),
                    AutoAttack.Add(client),
                    Companions.Add(client),
                    Prayer.Add(client),
                };
                return true;
            }
            return false;
        }
        public void Unregister(GameClient client)
        {
            if (client.TimerSubscriptions == null) return;
            lock (client.TimerSyncRoot)
            {
                if (client.TimerSubscriptions != null)
                {
                    foreach (var timer in client.TimerSubscriptions)
                        timer.Dispose();
                    client.TimerSubscriptions = null;
                }
            }
        }
        private bool Valid(GameClient client)
        {
            if (!client.Socket.Alive || client.Entity == null)
            {
                client.Disconnect();
                return false;
            }
            return true;
        }
        public static bool Room1 = false;
        public static bool Room2 = false;
        public static bool Room3 = false;
        public static bool Room4 = false;
        public static bool Room5 = false;
        public static bool Room6 = false;
        public static uint Room1Price = 0;
        public static uint Room2Price = 0;
        public static uint Room3Price = 0;
        public static uint Room4Price = 0;
        public static uint Room5Price = 0;
        public static uint Room6Price = 0;
        private void BuffersCallback(GameClient client, int time)
        {
            if (!Valid(client)) return;
            Time32 Now = new Time32(time);
            if (client.Entity.MyJiang != null)
            {
                client.Entity.MyJiang.TheadTime(client);
            }
            #region Dragon Fury
            //if (Time32.Now >= client.Entity.DragonFury.AddSeconds(5))
            {
                if (client.Entity.ContainsFlag3(Update.Flags3.DragonFury))
                {
                    client.Entity.RemoveFlag3(Update.Flags3.DragonFury);

                    client.Entity.DragonFuryStamp = 0;

                }
                else
                {
                    client.Entity.RemoveFlag3(Update.Flags3.DragonFury);
                }

            }
            #endregion
            #region Cursed
            if (client.Entity.ContainsFlag(Update.Flags.Cursed))
            {
                if (Time32.Now > client.Entity.Cursed.AddSeconds(300))
                {
                    client.Entity.RemoveFlag(Update.Flags.Cursed);
                }
            }
            #endregion
            #region Bless
            if (client.Entity.ContainsFlag(Update.Flags.CastPray))
            {
                if (client.BlessTime <= 7198500)
                    client.BlessTime += 1000;
                else
                    client.BlessTime = 7200000;
                client.Entity.Update(Update.LuckyTimeTimer, client.BlessTime, false);
            }
            else if (client.Entity.ContainsFlag(Update.Flags.Praying))
            {
                if (client.PrayLead != null)
                {
                    if (client.PrayLead.Socket.Alive)
                    {
                        if (client.BlessTime <= 7199000)
                            client.BlessTime += 500;
                        else
                            client.BlessTime = 7200000;
                        client.Entity.Update(Update.LuckyTimeTimer, client.BlessTime, false);
                    }
                    else
                        client.Entity.RemoveFlag(Update.Flags.Praying);
                }
                else
                    client.Entity.RemoveFlag(Update.Flags.Praying);
            }
            else
            {
                if (client.BlessTime > 0)
                {
                    if (client.BlessTime >= 500)
                        client.BlessTime -= 500;
                    else
                        client.BlessTime = 0;
                    client.Entity.Update(Update.LuckyTimeTimer, client.BlessTime, false);
                }
            }
            #endregion
            #region Flashing name
            if (client.Entity.ContainsFlag(Network.GamePackets.Update.Flags.FlashingName))
            {
                if (Now > client.Entity.FlashingNameStamp.AddSeconds(client.Entity.FlashingNameTime))
                {
                    client.Entity.RemoveFlag(Network.GamePackets.Update.Flags.FlashingName);
                }
            }
            #endregion
            #region Dragoncyclone like TQ
            if (Now >= client.Entity.DragonCyclone.AddSeconds(46))
            {
                client.Entity.RemoveFlag3(Update.Flags3.DragonCyclone);
            }
            #endregion
            #region XPList
            if (!client.Entity.ContainsFlag(Network.GamePackets.Update.Flags.XPList))
            {
                if (Now > client.XPCountStamp.AddSeconds(3))
                {
                    #region Arrows
                    if (client.Equipment != null)
                    {
                        if (!client.Equipment.Free(5))
                        {
                            if (Network.PacketHandler.IsArrow(client.Equipment.TryGetItem(5).ID))
                            {
                                Database.ConquerItemTable.UpdateDurabilityItem(client.Equipment.TryGetItem(5));
                            }
                        }
                    }
                    #endregion
                    client.XPCountStamp = Now;
                    client.XPCount++;
                    if (client.XPCount >= 100)
                    {
                        client.Entity.AddFlag(Network.GamePackets.Update.Flags.XPList);
                        client.XPCount = 0;
                        client.XPListStamp = Now;
                    }
                }
            }
            else
            {
                if (Now > client.XPListStamp.AddSeconds(20))
                {
                    client.Entity.RemoveFlag(Network.GamePackets.Update.Flags.XPList);
                }
            }
            #endregion
            #region KOSpell
            if (client.Entity.OnKOSpell())
            {
                if (client.Entity.OnCyclone())
                {
                    int Seconds = Now.AllSeconds() - client.Entity.CycloneStamp.AddSeconds(client.Entity.CycloneTime).AllSeconds();
                    if (Seconds >= 1)
                    {
                        client.Entity.RemoveFlag(Network.GamePackets.Update.Flags.Cyclone);
                    }
                }
                if (client.Entity.OnSuperman())
                {
                    int Seconds = Now.AllSeconds() - client.Entity.SupermanStamp.AddSeconds(client.Entity.SupermanTime).AllSeconds();
                    if (Seconds >= 1)
                    {
                        client.Entity.RemoveFlag(Network.GamePackets.Update.Flags.Superman);
                    }
                }
                if (!client.Entity.OnKOSpell())
                {
                    //Record KO
                    client.Entity.KOCount = 0;
                }
            }
            #endregion
            #region Buffers
            if (client.Entity.ContainsFlag2(Network.GamePackets.Update.Flags2.TyrantAura) && !client.TeamAura)
            {
                if (Now >= client.Entity.AuraStamp.AddMinutes(10))
                {
                    client.Entity.AuraTime = 0;
                    client.Entity.Aura_isActive = false;
                    //client.Entity.StigmaIncrease = 0;
                    client.Entity.RemoveFlag2(Network.GamePackets.Update.Flags2.TyrantAura);
                }
            }
            if (client.Entity.ContainsFlag2(Network.GamePackets.Update.Flags2.FendAura) && !client.TeamAura)
            {
                if (Now >= client.Entity.AuraStamp.AddMinutes(10))
                {
                    client.Entity.AuraTime = 0;
                    client.Entity.Aura_isActive = false;
                    //client.Entity.StigmaIncrease = 0;
                    client.Entity.RemoveFlag2(Network.GamePackets.Update.Flags2.FendAura);
                }
            }
            if (client.Entity.ContainsFlag2(Network.GamePackets.Update.Flags2.MetalAura))
            {
                if (Now >= client.Entity.AuraStamp.AddMinutes(10))
                {
                    client.Entity.AuraTime = 0;
                    client.Entity.Aura_isActive = false;
                    //client.Entity.StigmaIncrease = 0;
                    client.Entity.RemoveFlag2(Network.GamePackets.Update.Flags2.MetalAura);
                }
            }
            if (client.Entity.ContainsFlag2(Network.GamePackets.Update.Flags2.WoodAura))
            {
                if (Now >= client.Entity.AuraStamp.AddMinutes(10))
                {
                    client.Entity.AuraTime = 0;
                    client.Entity.Aura_isActive = false;
                    //client.Entity.StigmaIncrease = 0;
                    client.Entity.RemoveFlag2(Network.GamePackets.Update.Flags2.WoodAura);
                }
            }
            if (client.Entity.ContainsFlag2(Network.GamePackets.Update.Flags2.WaterAura))
            {
                if (Now >= client.Entity.AuraStamp.AddMinutes(10))
                {
                    client.Entity.AuraTime = 0;
                    client.Entity.Aura_isActive = false;
                    //client.Entity.StigmaIncrease = 0;
                    client.Entity.RemoveFlag2(Network.GamePackets.Update.Flags2.WaterAura);
                }
            }
            if (client.Entity.ContainsFlag2(Network.GamePackets.Update.Flags2.EarthAura))
            {
                if (Now >= client.Entity.AuraStamp.AddMinutes(10))
                {
                    client.Entity.AuraTime = 0;
                    client.Entity.Aura_isActive = false;
                    //client.Entity.StigmaIncrease = 0;
                    client.Entity.RemoveFlag2(Network.GamePackets.Update.Flags2.EarthAura);
                }
            }
            if (client.Entity.ContainsFlag2(Network.GamePackets.Update.Flags2.FireAura))
            {
                if (Now >= client.Entity.AuraStamp.AddMinutes(10))
                {
                    client.Entity.AuraTime = 0;
                    client.Entity.Aura_isActive = false;
                    //client.Entity.StigmaIncrease = 0;
                    client.Entity.RemoveFlag2(Network.GamePackets.Update.Flags2.FireAura);
                }
            }

            if (client.Entity.ContainsFlag(Network.GamePackets.Update.Flags.Stigma))
            {
                if (Now >= client.Entity.StigmaStamp.AddSeconds(client.Entity.StigmaTime))
                {
                    client.Entity.StigmaTime = 0;
                    client.Entity.StigmaIncrease = 0;
                    client.Entity.RemoveFlag(Network.GamePackets.Update.Flags.Stigma);
                }
            }
            if (client.Entity.ContainsFlag(Network.GamePackets.Update.Flags.Dodge))
            {
                if (Now >= client.Entity.DodgeStamp.AddSeconds(client.Entity.DodgeTime))
                {
                    client.Entity.DodgeTime = 0;
                    client.Entity.DodgeIncrease = 0;
                    //Console.WriteLine("dodge removed !");
                    client.Entity.RemoveFlag(Network.GamePackets.Update.Flags.Dodge);
                }
            }
            if (client.Entity.ContainsFlag(Network.GamePackets.Update.Flags.Invisibility))
            {
                if (Now >= client.Entity.InvisibilityStamp.AddSeconds(client.Entity.InvisibilityTime))
                {
                    client.Entity.RemoveFlag(Network.GamePackets.Update.Flags.Invisibility);
                }
            }
            if (client.Entity.ContainsFlag(Network.GamePackets.Update.Flags.StarOfAccuracy))
            {
                if (client.Entity.StarOfAccuracyTime != 0)
                {
                    if (Now >= client.Entity.StarOfAccuracyStamp.AddSeconds(client.Entity.StarOfAccuracyTime))
                    {
                        client.Entity.RemoveFlag(Network.GamePackets.Update.Flags.StarOfAccuracy);
                    }
                }
                else
                {
                    if (Now >= client.Entity.AccuracyStamp.AddSeconds(client.Entity.AccuracyTime))
                    {
                        client.Entity.RemoveFlag(Network.GamePackets.Update.Flags.StarOfAccuracy);
                    }
                }
            }
            if (client.Entity.ContainsFlag(Network.GamePackets.Update.Flags.MagicShield))
            {
                if (client.Entity.MagicShieldTime != 0)
                {
                    if (Now >= client.Entity.MagicShieldStamp.AddSeconds(client.Entity.MagicShieldTime))
                    {
                        client.Entity.MagicShieldIncrease = 0;
                        client.Entity.MagicShieldTime = 0;
                        client.Entity.RemoveFlag(Network.GamePackets.Update.Flags.MagicShield);
                    }
                }
                else
                {
                    if (Now >= client.Entity.ShieldStamp.AddSeconds(client.Entity.ShieldTime))
                    {
                        client.Entity.ShieldIncrease = 0;
                        client.Entity.ShieldTime = 0;
                        client.Entity.RemoveFlag(Network.GamePackets.Update.Flags.MagicShield);
                    }
                }
            }
            #endregion
            #region Fly
            if (client.Entity.ContainsFlag(Network.GamePackets.Update.Flags.Fly))
            {
                if (Now >= client.Entity.FlyStamp.AddSeconds(client.Entity.FlyTime))
                {
                    client.Entity.RemoveFlag(Network.GamePackets.Update.Flags.Fly);
                    client.Entity.FlyTime = 0;
                }
            }
            #endregion
            #region PoisonStar
            if (client.Entity.NoDrugsTime > 0)
            {
                if (Now > client.Entity.NoDrugsStamp.AddSeconds(client.Entity.NoDrugsTime))
                {
                    client.Entity.NoDrugsTime = 0;
                }
            }
            #endregion
            #region ToxicFog
            if (client.Entity.ToxicFogLeft > 0)
            {
                if (Now >= client.Entity.ToxicFogStamp.AddSeconds(2))
                {
                    float Percent = client.Entity.ToxicFogPercent;
                    Percent = Percent / 100 * (client.Entity.Immunity / 100F);
                    //Remove this line if you want it normal
                    //Percent = Math.Min(0.1F, client.Entity.ToxicFogPercent);
                    client.Entity.ToxicFogLeft--;
                    if (client.Entity.ToxicFogLeft == 0)
                    {
                        client.Entity.RemoveFlag(Update.Flags.Poisoned);
                        return;
                    }
                    client.Entity.ToxicFogStamp = Now;
                    if (client.Entity.Hitpoints > 1)
                    {
                        uint damage = Game.Attacking.Calculate.Percent(client.Entity, Percent);
                        client.Entity.Hitpoints -= damage;
                        Network.GamePackets.SpellUse suse = new Network.GamePackets.SpellUse(true);
                        suse.Attacker = client.Entity.UID;
                        suse.SpellID = 10010;
                        suse.AddTarget(client.Entity.UID, damage, null);
                        client.SendScreen(suse, true);
                        if (client != null)
                            client.UpdateQualifier(client.ArenaStatistic.PlayWith, client, damage);
                    }
                }
            }
            else
            {
                if (client.Entity.ContainsFlag(Update.Flags.Poisoned))
                    client.Entity.RemoveFlag(Update.Flags.Poisoned);
            }
            #endregion
            #region FatalStrike
            if (client.Entity.OnFatalStrike())
            {
                if (Now > client.Entity.FatalStrikeStamp.AddSeconds(client.Entity.FatalStrikeTime))
                {
                    client.Entity.RemoveFlag(Network.GamePackets.Update.Flags.FatalStrike);
                }
            }
            #endregion
            #region Oblivion
            if (client.Entity.OnOblivion())
            {
                if (Now > client.Entity.OblivionStamp.AddSeconds(client.Entity.OblivionTime))
                {
                    client.Entity.RemoveFlag2(Network.GamePackets.Update.Flags2.Oblivion);
                }
            }
            #endregion
            #region ShurikenVortex
            if (client.Entity.ContainsFlag(Network.GamePackets.Update.Flags.ShurikenVortex))
            {
                if (Now > client.Entity.ShurikenVortexStamp.AddSeconds(client.Entity.ShurikenVortexTime))
                {
                    client.Entity.RemoveFlag(Network.GamePackets.Update.Flags.ShurikenVortex);
                }
            }
            #endregion
            #region Transformations
            if (client.Entity.Transformed)
            {
                if (Now > client.Entity.TransformationStamp.AddSeconds(client.Entity.TransformationTime))
                {
                    client.Entity.Untransform();
                }
            }
            #endregion
            #region SoulShackle
            if (client.Entity.ContainsFlag2(Network.GamePackets.Update.Flags2.SoulShackle))
            {
                if (Now > client.Entity.ShackleStamp.AddSeconds(client.Entity.ShackleTime))
                {
                    client.Entity.RemoveFlag2(Network.GamePackets.Update.Flags2.SoulShackle);
                }
            }
            #endregion
            #region AzureShield
            if (client.Entity.ContainsFlag2(Network.GamePackets.Update.Flags2.AzureShield))
            {
                if (Now > client.Entity.MagicShieldStamp.AddSeconds(client.Entity.MagicShieldTime))
                {
                    client.Entity.RemoveFlag2(Network.GamePackets.Update.Flags2.AzureShield);
                }
            }
            #endregion
            #region Blade Flurry
            if (client.Entity.ContainsFlag3(Update.Flags3.BladeFlurry))
            {
                if (Time32.Now > client.Entity.BladeFlurryStamp.AddSeconds(45))
                {
                    client.Entity.RemoveFlag3(Update.Flags3.BladeFlurry);
                }
            }
            #endregion
            #region Flustered
            if (client.Entity.ContainsFlag(Update.Flags.Frightened) && client.Entity.MapID == 1950)
            {
                if (client.RaceFrightened)
                {
                    if (Now > client.FrightenStamp.AddSeconds(20))
                    {
                        client.RaceFrightened = false;
                        {
                            GameCharacterUpdates update = new GameCharacterUpdates(true);
                            update.UID = client.Entity.UID;
                            update.Remove(GameCharacterUpdates.Flustered);
                            client.SendScreen(update, true);
                        }
                        client.Entity.RemoveFlag(Update.Flags.Frightened);
                    }
                    else
                    {
                        int rand;
                        ushort x, y;
                        do
                        {
                            rand = Kernel.Random.Next(Game.Map.XDir.Length);
                            x = (ushort)(client.Entity.X + Game.Map.XDir[rand]);
                            y = (ushort)(client.Entity.Y + Game.Map.YDir[rand]);
                        }
                        while (!client.Map.Floor[x, y, MapObjectType.Player]);
                        client.Entity.Facing = Kernel.GetAngle(
                            client.Entity.X, client.Entity.Y, x, y);
                        client.Entity.X = x;
                        client.Entity.Y = y;

                        client.SendScreen(
                            new TwoMovements()
                            {
                                EntityCount = 1,
                                Facing = client.Entity.Facing,
                                FirstEntity = client.Entity.UID,
                                WalkType = 9,
                                X = client.Entity.X,
                                Y = client.Entity.Y,
                                MovementType = TwoMovements.Walk
                            }, true);
                    }
                }
            }
            #endregion
            #region Stunned
            if (client.Entity.Stunned)
            {
                if (Now > client.Entity.StunStamp.AddMilliseconds(2000))
                {
                    client.Entity.Stunned = false;
                }
            }
            #endregion
            #region Frozen
            if (client.Entity.ContainsFlag(Update.Flags.Freeze) && client.Entity.MapID == 1950)
            {
                if (Now > client.Entity.FrozenStamp.AddSeconds(client.Entity.FrozenTime))
                {
                    client.Entity.FrozenD = false;
                    client.Entity.FrozenTime = 0;
                    client.Entity.RemoveFlag(Update.Flags.Freeze);

                    GameCharacterUpdates update = new GameCharacterUpdates(true);
                    update.UID = client.Entity.UID;
                    update.Remove(GameCharacterUpdates.Freeze);
                    client.SendScreen(update, true);
                }
            }
            #endregion
            #region Dizzy
            if (client.Entity.ContainsFlag(Update.Flags.Dizzy) && client.Entity.MapID == 1950)
            {
                if (client.RaceDizzy)
                {
                    if (Now > client.DizzyStamp.AddSeconds(5))
                    {
                        client.RaceDizzy = false;
                        {
                            GameCharacterUpdates update = new GameCharacterUpdates(true);
                            update.UID = client.Entity.UID;
                            update.Remove(GameCharacterUpdates.Dizzy);
                            client.SendScreen(update);
                        }
                        client.Entity.RemoveFlag(Update.Flags.Dizzy);
                    }
                }
            }
            #endregion
            #region Confused
            if (client.Entity.ContainsFlag(Update.Flags.Confused) && client.Entity.MapID == 1950)
            {
                if (Now > client.FrightenStamp.AddSeconds(15))
                {
                    client.RaceFrightened = false;
                    {
                        GameCharacterUpdates update = new GameCharacterUpdates(true);
                        update.UID = client.Entity.UID;
                        update.Remove(GameCharacterUpdates.Flustered);
                        client.SendScreen(update);
                    }
                    client.Entity.RemoveFlag(Update.Flags.Confused);
                }
            }
            #endregion
            #region IceBlock
            if (client.Entity.ContainsFlag(Network.GamePackets.Update.Flags.Frightened))
            {
                if (Now > client.FrightenStamp.AddSeconds(client.Entity.Fright))
                {
                    GameCharacterUpdates update = new GameCharacterUpdates(true);
                    update.UID = client.Entity.UID;
                    update.Remove(GameCharacterUpdates.Dizzy);
                    client.SendScreen(update, true);
                    client.Entity.RemoveFlag(Update.Flags.Frightened);
                }
                else
                {
                    int rand;
                    ushort x, y;
                    do
                    {
                        rand = Kernel.Random.Next(Game.Map.XDir.Length);
                        x = (ushort)(client.Entity.X + Game.Map.XDir[rand]);
                        y = (ushort)(client.Entity.Y + Game.Map.YDir[rand]);
                    }
                    while (!client.Map.Floor[x, y, MapObjectType.Player]);
                    client.Entity.Facing = Kernel.GetAngle(
                        client.Entity.X, client.Entity.Y, x, y);
                    client.Entity.X = x;
                    client.Entity.Y = y;

                    client.SendScreen(
                        new TwoMovements()
                        {
                            EntityCount = 1,
                            Facing = client.Entity.Facing,
                            FirstEntity = client.Entity.UID,
                            WalkType = 9,
                            X = client.Entity.X,
                            Y = client.Entity.Y,
                            MovementType = TwoMovements.Walk
                        }, true);
                }
            }
            if (client.Entity.ContainsFlag(Network.GamePackets.Update.Flags.Freeze))
            {
                if (Now > client.Entity.BlockStamp.AddSeconds(client.Entity.BlockTime))
                {
                    GameCharacterUpdates update = new GameCharacterUpdates(true);
                    update.UID = client.Entity.UID;
                    update.Remove(GameCharacterUpdates.Freeze);
                    client.SendScreen(update, true);
                    client.Entity.RemoveFlag(Network.GamePackets.Update.Flags.Freeze);
                }
            }
            if (client.Entity.ContainsFlag(Network.GamePackets.Update.Flags.Confused))
            {
                if (Now > client.ChaosStamp.AddSeconds(client.Entity.ChaosTime))
                {
                    GameCharacterUpdates update = new GameCharacterUpdates(true);
                    update.UID = client.Entity.UID;
                    update.Remove(GameCharacterUpdates.Flustered);
                    client.SendScreen(update);
                    client.Entity.RemoveFlag(Network.GamePackets.Update.Flags.Confused);
                }
            }
            #endregion
            #region Divine Shield
            if (client.Entity.ContainsFlag(Update.Flags.DivineShield) && client.Entity.MapID == 1950)
            {
                if (Now > client.GuardStamp.AddSeconds(10))
                {
                    client.RaceGuard = false;
                    {
                        GameCharacterUpdates update = new GameCharacterUpdates(true);
                        update.UID = client.Entity.UID;
                        update.Remove(GameCharacterUpdates.DivineShield);
                        client.SendScreen(update);
                    }
                    client.Entity.RemoveFlag(Update.Flags.DivineShield);
                }
            }
            #endregion
            #region Extra Speed
            if (client.Entity.ContainsFlag(Update.Flags.OrangeSparkles) && !client.InQualifier() && client.Entity.MapID == 1950)
            {
                if (Time32.Now > client.RaceExcitementStamp.AddSeconds(15))
                {
                    var upd = new GameCharacterUpdates(true)
                    {
                        UID = client.Entity.UID
                    };
                    upd.Remove(GameCharacterUpdates.Accelerated);
                    client.SendScreen(upd);
                    client.SpeedChange = null;
                    client.Entity.RemoveFlag(Update.Flags.OrangeSparkles);
                }
            }
            #endregion
            #region Decelerated
            if (client.Entity.ContainsFlag(Update.Flags.PurpleSparkles) && !client.InQualifier())
            {
                if (Time32.Now > client.DecelerateStamp.AddSeconds(10))
                {
                    {
                        client.RaceDecelerated = false;
                        var upd = new GameCharacterUpdates(true)
                        {
                            UID = client.Entity.UID
                        };
                        upd.Remove(GameCharacterUpdates.Decelerated);
                        client.SendScreen(upd);
                        client.SpeedChange = null;
                    }
                    client.Entity.RemoveFlag(Update.Flags.PurpleSparkles);
                }
            }
            #endregion
            #region Team Aura
            if (!client.TeamAura)
            {
                if (client.Team != null && !client.Entity.Dead && client.Team.Teammates != null)
                {
                    foreach (Client.GameClient pClient in client.Team.Teammates)
                    {
                        if (client.Entity.UID != pClient.Entity.UID && Kernel.GetDistance(client.Entity.X, client.Entity.Y, pClient.Entity.X, pClient.Entity.Y) <= Constants.pScreenDistance)
                        {
                            if (pClient.Entity.Aura_isActive && pClient.Socket.Alive && pClient.Entity.UID != client.Entity.UID && pClient.Entity.MapID == client.Entity.MapID)
                            {
                                if (pClient.Entity.Aura_actType == Update.Flags2.FendAura || pClient.Entity.Aura_actType == Update.Flags2.TyrantAura)
                                {
                                    client.TeamAura = true;
                                    client.TeamAuraOwner = pClient;
                                    client.TeamAuraStatusFlag = pClient.Entity.Aura_actType;
                                    client.TeamAuraPower = pClient.Entity.Aura_actPower;
                                    client.Entity.AddFlag2(client.TeamAuraStatusFlag);
                                    string type = "Critial Strikes";
                                    if (client.Entity.Aura_actType == 100) type = "Immunity";
                                    client.Send(new Message(type + " increased By " + client.TeamAuraPower + " percent!", System.Drawing.Color.Red, Message.Agate));
                                    client.doAuraBonuses(client.TeamAuraStatusFlag, client.TeamAuraPower, 1);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                var pClient = client.TeamAuraOwner;
                string type = "Critial Strikes";
                if (client.Entity.Aura_actType == 100) type = "Immunity";
                if (pClient == null)
                {
                    client.TeamAura = false;
                    client.doAuraBonuses(client.TeamAuraStatusFlag, client.TeamAuraPower, -1);
                    client.Entity.RemoveFlag2(client.TeamAuraStatusFlag);
                    client.Send(new Message(type + " decreased by " + client.TeamAuraPower + " percent!", System.Drawing.Color.Red, Message.Agate));
                    client.TeamAuraStatusFlag = 0;
                    client.TeamAuraPower = 0;
                }
                else
                {
                    if (!pClient.Entity.Aura_isActive || !pClient.Socket.Alive || pClient.Entity.Dead || pClient.Entity.MapID != client.Entity.MapID)
                    {
                        client.TeamAura = false;
                        client.doAuraBonuses(client.TeamAuraStatusFlag, client.TeamAuraPower, -1);
                        client.Entity.RemoveFlag2(client.TeamAuraStatusFlag);
                        client.Send(new Message(type + " decreased by " + client.TeamAuraPower + " percent!", System.Drawing.Color.Red, Message.Agate));
                        client.TeamAuraStatusFlag = 0;
                        client.TeamAuraPower = 0;
                    }
                    else
                    {
                        if (client.Team == null || (pClient.Team == null || (pClient.Team != null && !pClient.Team.IsTeammate(client.Entity.UID))) || client.Entity.Dead || Kernel.GetDistance(client.Entity.X, client.Entity.Y, pClient.Entity.X, pClient.Entity.Y) > Constants.pScreenDistance)
                        {
                            client.TeamAura = false;
                            client.doAuraBonuses(client.TeamAuraStatusFlag, client.TeamAuraPower, -1);
                            client.Entity.RemoveFlag2(client.TeamAuraStatusFlag);
                            client.Send(new Message(type + " decreased by " + client.TeamAuraPower + " percent!", System.Drawing.Color.Red, Message.Agate));
                            client.TeamAuraStatusFlag = 0;
                            client.TeamAuraPower = 0;
                        }
                    }
                }
            }
            #endregion
            #region CTF Flag
            if (client.Entity.ContainsFlag2(Update.Flags2.CarryingFlag))
            {
                if (Time32.Now > client.Entity.FlagStamp.AddSeconds(60))
                {
                    client.Entity.RemoveFlag2(Update.Flags2.CarryingFlag);
                }
            }
            #endregion
            #region SuperCyclone
            if (client.Entity.ContainsFlag3((uint)1UL << 0x16))
            {
                if (Time32.Now > client.Entity.SuperCyclone.AddSeconds(40))
                {
                    client.Entity.RemoveFlag3((uint)1UL << 0x16);
                }
            }
            #endregion
            #region Vote System By Mr.Bahaa
            if (DateTime.Now.Hour == 12 && DateTime.Now.Minute == 00 && DateTime.Now.Second == 40)
            {
                if (Program.Carnaval2 == 1)
                {
                    //
                    Program.Carnaval2 = 0;
                }
            }
            if (DateTime.Now.Hour == 12 && DateTime.Now.Minute == 00 && DateTime.Now.Second == 45)
            {
                if (Program.Carnaval2 == 0)
                {
                    Kernel.VotePoolUid.Clear();
                    Kernel.VotePool.Clear();
                    Database.EntityTable.DeletVotes();
                    Program.Carnaval2 = 1;
                }
            }
            if (DateTime.Now.Hour == 23 && DateTime.Now.Minute == 59 && DateTime.Now.Second == 40)
            {
                if (Program.Carnaval3 == 1)
                {
                    //
                    Program.Carnaval3 = 0;
                }
            }
            if (DateTime.Now.Hour == 23 && DateTime.Now.Minute == 59 && DateTime.Now.Second == 45)
            {
                if (Program.Carnaval3 == 0)
                {
                    Kernel.VotePoolUid.Clear();
                    Kernel.VotePool.Clear();
                    Database.EntityTable.DeletVotes();
                    Program.Carnaval3 = 1;
                }
            }
            #endregion By Mr.Bahaa
        }
            
        private void CharactersCallback(GameClient client, int time)
        {
            if (!Valid(client)) return;
            Time32 Now32 = new Time32(time);
            DateTime Now64 = DateTime.Now;

            if (client.Entity.Titles.Count > 0)
            {
                foreach (var titles in client.Entity.Titles)
                {
                    if (Now64 > titles.Value)
                    {
                        client.Entity.Titles.Remove(titles.Key);
                        if (client.Entity.MyTitle == titles.Key)
                            client.Entity.MyTitle = Network.GamePackets.TitlePacket.Titles.None;

                        client.Entity.RemoveTopStatus((UInt64)titles.Key);
                    }
                }
            }


            #region Training points
            if (client.Entity.HeavenBlessing > 0 && !client.Entity.Dead)
            {
                if (Now32 > client.LastTrainingPointsUp.AddMinutes(10))
                {
                    client.OnlineTrainingPoints += 10;
                    if (client.OnlineTrainingPoints >= 30)
                    {
                        client.OnlineTrainingPoints -= 30;
                        client.IncreaseExperience(client.ExpBall / 100, false);
                    }
                    client.LastTrainingPointsUp = Now32;
                    client.Entity.Update(Network.GamePackets.Update.OnlineTraining, client.OnlineTrainingPoints, false);
                }
            }
            #endregion
            #region LordsWar
            if (Now64.Hour == 21 && Now64.Minute >= 00 && Now64.Second == 00)
            {
                if (client.Entity.MapID == 5555)
                {
                    client.Entity.Teleport(1002, 317, 151);
                }
            }
            #endregion LordsWar
            #region RamdanEffect
            {
                if (client.Entity.RamadanEffect == 1)
                {
                    client.Entity.Update(_String.Effect, "jinshi", true);
                }
            }
            #endregion  
            #region Minning
            if (client.Mining && !client.Entity.Dead)
            {
                if (Now32 >= client.MiningStamp.AddSeconds(2))
                {
                    client.MiningStamp = Now32;
                    Game.ConquerStructures.Mining.Mine(client);
                }
            }
            #endregion
            #region MentorPrizeSave
            if (Now32 > client.LastMentorSave.AddSeconds(5))
            {
                Database.KnownPersons.SaveApprenticeInfo(client.AsApprentice);
                client.LastMentorSave = Now32;
            }
            #endregion
            #region Auto Clear Entity=0
            if ((DateTime.Now.Second == 0) || (DateTime.Now.Second == 5))
            {
                Database.ConquerItemTable.ClearNulledItems();
            }
            #endregion
            #region Winners for FB and SS
            if (client.Entity.aWinner == true)
            {
                {
                    switch (client.Entity.MapID)
                    {
                        case 4573://room 1
                            {
                                Room1 = false;
                                break;
                            }
                        case 4574://room 2
                            {
                                Room2 = false;
                                break;
                            }
                        case 4575://room 3
                            {
                                Room3 = false;
                                break;
                            }
                        case 4576://room 4
                            {
                                Room4 = false;
                                break;
                            }
                        case 4577://room 5
                            {
                                Room5 = false;
                                break;
                            }
                        case 4578://room 6
                            {
                                Room6 = false;
                                break;
                            }

                        //client.Entity.Teleport(1002, 439, 384);
                        //client.Entity.aWinner = false;
                    }
                }
            }
            #endregion
            #region Attackable
            if (client.JustLoggedOn)
            {
                client.JustLoggedOn = false;
                client.ReviveStamp = Now32;
            }
            if (!client.Attackable)
            {
                if (Now32 > client.ReviveStamp.AddSeconds(5))
                {
                    client.Attackable = true;
                }
            }
            #endregion
            #region DoubleExperience
            if (client.Entity.DoubleExperienceTime > 0)
            {
                if (Now32 > client.Entity.DoubleExpStamp.AddMilliseconds(1000))
                {
                    client.Entity.DoubleExpStamp = Now32;
                    client.Entity.DoubleExperienceTime--;
                }
            }
            #endregion
            #region HeavenBlessing
            if (client.Entity.HeavenBlessing > 0)
            {
                if (Now32 > client.Entity.HeavenBlessingStamp.AddMilliseconds(1000))
                {
                    client.Entity.HeavenBlessingStamp = Now32;
                    client.Entity.HeavenBlessing--;
                }
            }
            #endregion
            #region Enlightment
            if (client.Entity.EnlightmentTime > 0)
            {
                if (Now32 >= client.Entity.EnlightmentStamp.AddMinutes(1))
                {
                    client.Entity.EnlightmentStamp = Now32;
                    client.Entity.EnlightmentTime--;
                    if (client.Entity.EnlightmentTime % 10 == 0 && client.Entity.EnlightmentTime > 0)
                        client.IncreaseExperience(Game.Attacking.Calculate.Percent((int)client.ExpBall, .10F), false);
                }
            }
            #endregion
            #region PKPoints
            if (Now32 >= client.Entity.PKPointDecreaseStamp.AddMinutes(5))
            {
                client.Entity.PKPointDecreaseStamp = Now32;
                if (client.Entity.PKPoints > 0)
                {
                    client.Entity.PKPoints--;
                }
                else
                    client.Entity.PKPoints = 0;
            }
            #endregion
            #region OverHP
            if (client.Entity.FullyLoaded)
            {
                if (client.Entity.Hitpoints > client.Entity.MaxHitpoints && client.Entity.MaxHitpoints > 1 && !client.Entity.Transformed)
                {
                    client.Entity.Hitpoints = client.Entity.MaxHitpoints;
                }
            }
            #endregion
            #region OverVigor
            if (client.Entity.FullyLoaded)
            {
                if (client.Vigor > client.Entity.ExtraVigor)
                {
                    client.Vigor = client.Entity.ExtraVigor;
                }
            }
            #endregion
            #region Die Delay
            if (client.Entity.Hitpoints == 0 && client.Entity.ContainsFlag(Network.GamePackets.Update.Flags.Dead) && !client.Entity.ContainsFlag(Network.GamePackets.Update.Flags.Ghost))
            {
                if (Now32 > client.Entity.DeathStamp.AddSeconds(2))
                {
                    client.Entity.AddFlag(Network.GamePackets.Update.Flags.Ghost);
                    if (client.Entity.Body % 10 < 3)
                        client.Entity.TransformationID = 99;
                    else
                        client.Entity.TransformationID = 98;

                    client.SendScreenSpawn(client.Entity, true);
                }
            }
            #endregion
            #region ChainBolt
            if (client.Entity.ContainsFlag2(Update.Flags2.ChainBoltActive))
                if (Now32 > client.Entity.ChainboltStamp.AddSeconds(client.Entity.ChainboltTime))
                    client.Entity.RemoveFlag2(Update.Flags2.ChainBoltActive);
            #endregion
            #region NemesisTyrant
            #region NemesisTyrant Apper
            if ((Now64.Hour == NemesisTyrant1 && Now64.Minute == 45 && Now64.Second == 5) || (Now64.Hour == (NemesisTyrant2) && Now64.Minute == 45 && Now64.Second == 5) ||
                (Now64.Hour == (NemesisTyrant3) && Now64.Minute == 45 && Now64.Second == 5) || (Now64.Hour == (NemesisTyrant4) && Now64.Minute == 45 && Now64.Second == 5) ||
                (Now64.Hour == (NemesisTyrant5) && Now64.Minute == 45 && Now64.Second == 5) || (Now64.Hour == (NemesisTyrant6) && Now64.Minute == 45 && Now64.Second == 5) ||
                (Now64.Hour == (NemesisTyrant7) && Now64.Minute == 45 && Now64.Second == 5) || (Now64.Hour == (NemesisTyrant8) && Now64.Minute == 45 && Now64.Second == 5))
            {
                #region Auto invite 2
                Kernel.SendWorldMessage(new Message("The Monster NemesisTyrant has apeared, Who will Defeat it !.", Color.White, Message.Center), Program.GamePool);
                client.MessageBox("NemesisTyrant has apeared, Who will Defeat it !",
                          (p) => { p.Entity.Teleport(3055, 117, 132); }, null, 60);
                #endregion
            }
            #endregion
            #endregion  
            #region Anti bot

            /* if (client.WaitingKillCaptcha)
            {
                if (Now32 > client.KillCountCaptchaStamp.AddSeconds(60))
                {
                    client.Disconnect();
                }
            }
            else
            {
                if (client.Entity.KillCount < 0) client.Entity.KillCount = 0;
                if (client.Entity.KillCount >= 1500 || client.Entity.KillCount2 >= 150)
                {
                    client.KillCountCaptchaStamp = Time32.Now;
                    client.WaitingKillCaptcha = true;

                    Npcs dialog = new Npcs(client);
                    client.ActiveNpc = 9999997;
                    client.KillCountCaptcha = client.GenerateCaptcha(5);
                    dialog.Text("Input the current text: " + client.KillCountCaptcha + " to verify your humanity.");
                    dialog.Input("Captcha message:", 1, (byte)client.KillCountCaptcha.Length);
                    dialog.Option("No thank you.", 255);
                    dialog.Send();
                    return;
                }
                else
                {
                    if (Now32 > client.LastMove.AddMinutes(5) && Now32 < client.LastAttack.AddSeconds(5))
                    {
                        if (client.WaitingKillCaptcha)
                        {
                            if (Now32 > client.KillCountCaptchaStamp.AddSeconds(60))
                            {
                                client.Disconnect();
                            }
                        }
                        else
                        {
                            client.KillCountCaptchaStamp = Time32.Now;
                            client.WaitingKillCaptcha = true;

                            Npcs dialog = new Npcs(client);
                            client.ActiveNpc = 9999997;
                            client.KillCountCaptcha = client.GenerateCaptcha(5);
                            dialog.Text("Input the current text: " + client.KillCountCaptcha + " to verify your humanity.");
                            dialog.Input("Captcha message:", 1, (byte)client.KillCountCaptcha.Length);
                            dialog.Option("No thank you.", 255);
                            dialog.Send();
                        }
                    }
                }
            }*/
            #endregion
            #region Dis City
            if (Now64.DayOfWeek == DayOfWeek.Wednesday || Now64.DayOfWeek == DayOfWeek.Friday)
            {
                if ((Now64.Hour == 12 || Now64.Hour == 19) && Now64.Minute == 00 && Now64.Second <= 2)
                {
                    if (client.Entity.Level >= 110)
                    {
                        Kernel.SendWorldMessage(new Network.GamePackets.Message("DisCity has begun! Go to Ape City to signup at SolarSaint!", System.Drawing.Color.White, Network.GamePackets.Message.Center), Program.GamePool);
                        Game.Features.DisCity.Signup = true;
                        client.MessageBox("Dis City has begun! Would you like to join?",
                            (p) => { p.Entity.Teleport(1020, 534, 484); },
                            (p) => { p.Send("You may join at SolarSaint in Ape City!"); }, 300);
                    }
                }
                if ((Now64.Hour == 12 || Now64.Hour == 19) && Now64.Minute == 45 && Now64.Second >= 00)
                {
                    if (client.Entity.MapID == 4023 || client.Entity.MapID == 4024)
                    {
                        Conquer_Online_Server.Kernel.SendWorldMessage(new Network.GamePackets.Message("All players in DisCity Stage3 has been teleported to FinalStage! Good luck!", System.Drawing.Color.White, Network.GamePackets.Message.Center), Program.GamePool);
                        client.Entity.Teleport(4025, 150, 286);
                        client.Inventory.Add(723087, 0, 1);
                    }
                }
                if ((Now64.Hour == 12 || Now64.Hour == 19) && Now64.Minute == 59 && Now64.Second >= 30)
                {
                    if (client.Entity.MapID == 4023 || client.Entity.MapID == 4024 || client.Entity.MapID == 4025)
                    {
                        Conquer_Online_Server.Kernel.SendWorldMessage(new Network.GamePackets.Message("DisCity has ended. It will begin at 12:00 or 19:00 every Wednesday and Friday!", System.Drawing.Color.White, Network.GamePackets.Message.Center), Program.GamePool);
                        client.Entity.Teleport(1002, 300, 280);
                    }
                }
            }
            #endregion
            #region ChampionArena
            if (DateTime.Now.Hour == 20 && DateTime.Now.Minute == 55 && DateTime.Now.Second <= 2)
            {
                client.MessageBox("ChampionArena has started!! Would you like to sign up?",
                      (p) => { Champion.ChampionKernel.SignUp(); }, null, 3);
            }
            #endregion  
            #region The Killers War
            if (client.Entity.MapID == 6000 || client.Entity.MapID == 6001 || client.Entity.MapID == 102 || client.Entity.MapID == 6002 || client.Entity.MapID == 6003 || client.Entity.MapID == 6004)
                return;
            if (DateTime.Now.Minute == 08 && DateTime.Now.Second <= 02)
            {

                object[] name;
                name = new object[] { "The Killers War War Quest Have Started Go To Jion in TwinCity at (345, 252)" };
                Kernel.SendWorldMessage(new Message(string.Concat(name), "The Killers ", "War", System.Drawing.Color.Red, 2500), Program.GamePool); 
                Kernel.SendWorldMessage(new Message("The Killers War event began!", Color.Red, Message.Center));
                client.MessageBox("The Killers War Start Wanna Join?",
                      (p) => { p.Entity.Teleport(1002, 345, 252); }, null, 60);
            }
            #endregion
            #region PeoplesChampion War
            if (client.Entity.MapID == 6000 || client.Entity.MapID == 6001 || client.Entity.MapID == 102 || client.Entity.MapID == 6002 || client.Entity.MapID == 6003 || client.Entity.MapID == 6004)
                return;
            if (DateTime.Now.Minute == 18 && DateTime.Now.Second <= 02)
            {

                object[] name;
                name = new object[] { "PeoplesChampion War Quest Have Started Go To Jion in TwinCity at (342, 252)" };
                Kernel.SendWorldMessage(new Message(string.Concat(name), "PeoplesChampion", "War", System.Drawing.Color.Red, 2500), Program.GamePool); 
                Kernel.SendWorldMessage(new Message("PeoplesChampion War event began!", Color.Red, Message.Center));
                client.MessageBox("PeoplesChampion War Start Wanna Join?",
                      (p) => { p.Entity.Teleport(1002, 342, 252); }, null, 60);
            }
            #endregion
            #region TeamPK
            if (client.Entity.MapID == 6000 || client.Entity.MapID == 6001 || client.Entity.MapID == 6002 || client.Entity.MapID == 6003 || client.Entity.MapID == 6004)
                return;
            if (DateTime.Now.Hour == 16 && DateTime.Now.Minute == 05 && DateTime.Now.Second <= 02)
            {
                Kernel.SendWorldMessage(new Message("TeamPK Tournement event began!", Color.Red, Message.Center));
                client.MessageBox("TeamPk Tournment Start Wanna Join?",
                      (p) => { p.Entity.Teleport(1002, 307, 150); }, null, 60);
            }
            #endregion
            #region LordsWar
            if (Now64.Hour == 20 && Now64.Minute == 30 && Now64.Second == 03)
            {
                Kernel.SendWorldMessage(new Message("LordWar event began!", Color.Red, Message.Center));
                client.MessageBox("LordsWar Tournment Start Wanna Join?",
                      (p) => { p.Entity.Teleport(5555, 50, 50); }, null, 60);
            }
            #endregion LordsWar
            #region SkillTeamPk
            if (client.Entity.MapID == 6000 || client.Entity.MapID == 6001 || client.Entity.MapID == 6002 || client.Entity.MapID == 6003 || client.Entity.MapID == 6004)
                return;
            if (DateTime.Now.Hour == 18 && DateTime.Now.Minute == 05 && DateTime.Now.Second <= 02)
            {
                Kernel.SendWorldMessage(new Message("SkillTeamPk Tournment event began!", Color.Red, Message.Center));
                client.MessageBox("SkillTeamPk Tournment Start Wanna Join?",
                      (p) => { p.Entity.Teleport(1002, 301, 150); }, null, 60);
            }
            #endregion
            #region Top Donation Quest
            if (Now64.Hour == 18 && Now64.Minute == 01 && Now64.Second <= 2)
            {
                Kernel.SendWorldMessage(new Message("TopDonation War began!", Color.White, Message.TopLeft), Program.GamePool);
                foreach (var pclint in Program.GamePool)
                    client.MessageBox("TopDonation Event Has Started Wana Join?",
                        (p) => { p.Entity.Teleport(1002, 285, 192); }, null, 60);
            }
            #endregion Top PkPoints Quest
            #region Top PkPoints Quest
            if (Now64.Hour == 16 && Now64.Minute == 01 && Now64.Second <= 2)
            {
                Kernel.SendWorldMessage(new Message("TopPkPoints War began!", Color.White, Message.TopLeft), Program.GamePool);
                foreach (var pclint in Program.GamePool)
                    client.MessageBox("TopPkPoints Event Has Started Wana Join?",
                        (p) => { p.Entity.Teleport(1002, 290, 192); }, null, 60);
            }
            #endregion Top PkPoints Quest
            #region ChampionPk
            if (client.Entity.MapID == 6000 || client.Entity.MapID == 6001 || client.Entity.MapID == 6002 || client.Entity.MapID == 6003 || client.Entity.MapID == 6004)
                return;
            if (DateTime.Now.Minute == 50 && DateTime.Now.Second == 00)
            {

                object[] name;
                name = new object[] { "ChampionPk Quest Have Started Go To Jion in TwinCity at (338, 252)" };
                Kernel.SendWorldMessage(new Message(string.Concat(name), "Champion", "War", System.Drawing.Color.Red, 2500), Program.GamePool); 
                Kernel.SendWorldMessage(new Message("ChampionPK event began!", Color.Red, Message.Center));
                client.MessageBox("ChampionPk Start Wanna Join?",
                      (p) => { p.Entity.Teleport(1002, 338, 252); }, null, 60);
            }
            #endregion
            #region octops
            if (DateTime.Now.Hour == 21)
            {
                if (client.Inventory.ContainsUID(711609))//id0   GOLD
                {
                    client.Inventory.Remove(711609, 1);//BY MeToOo
                    client.Send(new Conquer_Online_Server.Network.GamePackets.Message("octops quest finished i will take items!", System.Drawing.Color.Green, 0x7d0));
                    return;
                }
                else if (client.Inventory.ContainsUID(711610))//id1      SILVER
                {
                    client.Inventory.Remove(711610, 1);//BY MeToOo
                    client.Send(new Conquer_Online_Server.Network.GamePackets.Message("octops quest finished i will take items!", System.Drawing.Color.Green, 0x7d0));
                    return;
                }
                else if (client.Inventory.ContainsUID(711611))//id 2   COPPER
                {
                    client.Inventory.Remove(711611, 1);//BY MeToOo
                    client.Send(new Conquer_Online_Server.Network.GamePackets.Message("octops quest finished i will take items!", System.Drawing.Color.Green, 0x7d0));//[-> يمنع وضع الايميلات فى المنتدى - ادارة تيم اكسور <-]
                    return;
                }
                else
                {
                    client.Send(new Conquer_Online_Server.Network.GamePackets.Message("thanks for give as all items !", System.Drawing.Color.Green, 0x7d0));
                }
            }
            #endregion
            #region TreasurIn Blue
            if (client.Entity.MapID == 6000 || client.Entity.MapID == 6001 || client.Entity.MapID == 6002 || client.Entity.MapID == 6003 || client.Entity.MapID == 6004)
                return;
            if (DateTime.Now.Hour == 19 && DateTime.Now.Minute == 00 && DateTime.Now.Second == 00)
            {
                Kernel.SendWorldMessage(new Message("Treasure In Blue event began!", Color.Red, Message.Center));
                client.MessageBox("Treasure In Blue Start Wanna Join?",
                      (p) => { p.Entity.Teleport(1002, 326, 265); }, null, 60);
            }
            #endregion
            #region The Eral War
            if (client.Entity.MapID == 6000 || client.Entity.MapID == 6001 || client.Entity.MapID == 102 || client.Entity.MapID == 6002 || client.Entity.MapID == 6003 || client.Entity.MapID == 6004)
                return;
            if (DateTime.Now.Minute == 35 && DateTime.Now.Second <= 02)
            {

                object[] name;
                name = new object[] { "The Eral War War Quest Have Started Go To Jion in TwinCity at (348, 252)" };
                Kernel.SendWorldMessage(new Message(string.Concat(name), "The Eral ", "War", System.Drawing.Color.Red, 2500), Program.GamePool);
                Kernel.SendWorldMessage(new Message("The Eral War event began!", Color.Red, Message.Center));
                client.MessageBox("The Eral War Start Wanna Join?",
                      (p) => { p.Entity.Teleport(1002, 348, 252); }, null, 60);
            }
            #endregion
            #region Weekly PK
            if (client.Entity.MapID == 6000 || client.Entity.MapID == 6001 || client.Entity.MapID == 6002 || client.Entity.MapID == 6003 || client.Entity.MapID == 6004)
                return;
            if (Now64.Second <= 2 && Now64.DayOfWeek == DayOfWeek.Sunday && Now64.Hour == 16 && Now64.Minute == 00)
            {
                Kernel.SendWorldMessage(new Message("WeeklyPK event began!", Color.Red, Message.Center));
                client.MessageBox("Weekly PK has begun! Would you like to join?",
                      (p) => { p.Entity.Teleport(1002, 327, 194); }, null, 60);
            }
            #endregion
            #region Team Qualifier
            if ((Now64.Hour == 11 || Now64.Hour == 19) && Now64.Minute == 15 && Now64.Second <= 2)
            {
                client.MessageBox("Team arena has started! It will open for two hours! Would you like to sign up?",
                    (p) => { TeamArena.QualifyEngine.DoSignup(p); },
                    (p) => { p.Send("You can still join from the team arena interface!"); });
            }
            #endregion
            #region Last man standing
            if (client.Entity.MapID == 6000 || client.Entity.MapID == 6001 || client.Entity.MapID == 6002 || client.Entity.MapID == 6003 || client.Entity.MapID == 6004)
                return;
            if (Now64.Minute == 30 && Now64.Second == 01)
            //if (Now64.DayOfWeek == DayOfWeek.Thursday)
            {
                Kernel.SendWorldMessage(new Message("LastManStanding event began!", Color.Red, Message.Center));
                client.MessageBox("LastManStanding has begun! Would you like to join?",
                      (p) => { p.Entity.Teleport(1002, 311, 291); }, null, 60);
            }
            #endregion
      
            #region OneHit
            if (client.Entity.MapID == 6000 || client.Entity.MapID == 6001 || client.Entity.MapID == 102 || client.Entity.MapID == 6002 || client.Entity.MapID == 6003 || client.Entity.MapID == 6004)
                return;
            //if (Now64.DayOfWeek == DayOfWeek.Thursday)
            {
                if (Now64.Minute == 40 && Now64.Second <= 02)
                {
                    Kernel.SendWorldMessage(new Message("OneHit event began!", Color.Red, Message.Center));
                    client.MessageBox("OneHit Has Started! Would you like to join?",
                          (p) => { p.Entity.Teleport(1002, 315, 243); }, null, 60);
                }
            }
            #endregion

            #region NewGifts
            if (Now64.Minute == 59 && Now64.Second == 00)
            {
                /*if (Program.Carnaval == 1)
                {
                    // Game.FirozCarnaval.Load();
                    Program.Carnaval = 0;
                }*/
                Kernel.SendWorldMessage(new Network.GamePackets.Message("TQEnvoy will apear in TwinCity after 1 Minute and DropParty will Start Hurry go to TC to Get some Gifts  ", System.Drawing.Color.Red, Network.GamePackets.Message.Center), Program.GamePool);
            }
            if (Now64.Minute == 00 && Now64.Second == 03)
            {

                Game.Gifts.Load2();

            }
            if (Now64.Minute == 00 && Now64.Second == 06)
            {

                Game.Gifts.Load3();

            }
            if (Now64.Minute == 00 && Now64.Second == 09)
            {

                Game.Gifts.Load4();

            }
            if (Now64.Minute == 00 && Now64.Second == 12)
            {

                Game.Gifts.Load5();

            }
            if (Now64.Minute == 00 && Now64.Second == 15)
            {

                Game.Gifts.Load6();

            }
            if (Now64.Minute == 00 && Now64.Second == 18)
            {

                Game.Gifts.Load7();

            }
            if (Now64.Minute == 00 && Now64.Second == 21)
            {

                Game.Gifts.Load8();

            }
            if (Now64.Minute == 00 && Now64.Second == 24)
            {

                Game.Gifts.Load9();
                Kernel.SendWorldMessage(new Network.GamePackets.Message("TQEnvoy Drop Event ended come back next hour , it apear every hour at xx:00 goodluck  ", System.Drawing.Color.Red, Network.GamePackets.Message.Talk), Program.GamePool);

            }
            #endregion
            if (client.Fake)
            {
                if (!client.SignedUpForEPK)
                    ElitePKTournament.SignUp(client);
                if (client.ElitePKMatch != null)
                {
                    if (client.ElitePKMatch.OnGoing && client.ElitePKMatch.Inside)
                    {
                        if (Time32.Now > client.FakeQuit.AddSeconds(5))
                        {
                            client.FakeQuit = Time32.Now;
                            if (Kernel.Rate(1, 10))
                            {
                                client.ElitePKMatch.End(client);
                            }
                        }
                    }
                }
            }

#if OBSOLETE_CLASSPK
           /* if (Now64.Hour == 20 && Now64.Minute == 00 && Now64.DayOfWeek == DayOfWeek.Friday && Now64.Second == 30)
            {
                if (client.Entity.Class >= 41 && client.Entity.Class <= 45)
                {
                    //Database.EntityTable.UpdateArcherStatus(client);
                    //client.Entity.Status = 0;
                    Network.GamePackets.NpcReply npc = new Network.GamePackets.NpcReply(6, "ClassPkWar has Started! You Wana Join?");
                    npc.OptionID = 248;
                    client.Send(npc.ToArray());
                    //return;
                }
            }
            if (Now64.Hour == 20 && Now64.Minute == 00 && Now64.DayOfWeek == DayOfWeek.Wednesday && Now64.Second == 20)
            {
                if (client.Entity.Class >= 61 && client.Entity.Class <= 65)
                {
                    //Database.EntityTable.UpdateMonkStatus(client);
                    //client.Entity.Status = 0;
                    Network.GamePackets.NpcReply npc = new Network.GamePackets.NpcReply(6, "ClassPkWar has Started! You Wana Join?");
                    npc.OptionID = 248;
                    client.Send(npc.ToArray());
                    //return;
                }
            }

            if (Now64.Hour == 20 && Now64.Minute == 00 && Now64.DayOfWeek == DayOfWeek.Saturday && Now64.Second == 30)
            {
                if (client.Entity.Class >= 11 && client.Entity.Class <= 15)
                {
                    //Database.EntityTable.UpdateTrojanStatus(client);
                    //client.Entity.Status = 0;
                    Network.GamePackets.NpcReply npc = new Network.GamePackets.NpcReply(6, "ClassPkWar has Started! You Wana Join?");
                    npc.OptionID = 248;
                    client.Send(npc.ToArray());
                    //return;
                }
            }
            if (Now64.Hour == 20 && Now64.Minute == 00 && Now64.DayOfWeek == DayOfWeek.Sunday && Now64.Second == 30)
            {
                if (client.Entity.Class >= 21 && client.Entity.Class <= 25)
                {
                    //Database.EntityTable.UpdateWarriorStatus(client);
                    //client.Entity.Status = 0;
                    Network.GamePackets.NpcReply npc = new Network.GamePackets.NpcReply(6, "ClassPkWar has Started! You Wana Join?");
                    npc.OptionID = 248;
                    client.Send(npc.ToArray());
                    //return;
                }
            }
            if (Now64.Hour == 20 && Now64.Minute == 00 && Now64.DayOfWeek == DayOfWeek.Monday && Now64.Second == 30)
            {
                if (client.Entity.Class >= 142 && client.Entity.Class <= 145)
                {
                    //Database.EntityTable.UpdateFireStatus(client);
                    //client.Entity.Status = 0;
                    Network.GamePackets.NpcReply npc = new Network.GamePackets.NpcReply(6, "ClassPkWar has Started! You Wana Join?");
                    npc.OptionID = 248;
                    client.Send(npc.ToArray());
                    //return;
                }
            }
            if (Now64.Hour == 20 && Now64.Minute == 00 && Now64.DayOfWeek == DayOfWeek.Tuesday && Now64.Second == 30)
            {
                if (client.Entity.Class >= 51 && client.Entity.Class <= 55)
                {
                    //Database.EntityTable.UpdateNinjaStatus(client);
                    //client.Entity.Status = 0;
                    Network.GamePackets.NpcReply npc = new Network.GamePackets.NpcReply(6, "ClassPkWar has Started! You Wana Join?");
                    npc.OptionID = 248;
                    client.Send(npc.ToArray());
                    // return;
                }
            }
            if (Now64.Hour == 20 && Now64.Minute == 00 && Now64.DayOfWeek == DayOfWeek.Thursday && Now64.Second == 30)
            {
                if (client.Entity.Class >= 132 && client.Entity.Class <= 135)
                {
                    //Database.EntityTable.UpdateWaterStatus(client);
                    //client.Entity.Status = 0;
                    Network.GamePackets.NpcReply npc = new Network.GamePackets.NpcReply(6, "ClassPkWar has Started! You Wana Join?");
                    npc.OptionID = 248;
                    client.Send(npc.ToArray());
                    //return;
                }
            }*/
#endif
        }
        private void AutoAttackCallback(GameClient client, int time)
        {
            if (!Valid(client)) return;
            Time32 Now = new Time32(time);

            if (client.Entity.AttackPacket != null || client.Entity.VortexAttackStamp != null)
            {
                try
                {
                    if (client.Entity.ContainsFlag(Network.GamePackets.Update.Flags.ShurikenVortex))
                    {
                        if (client.Entity.VortexPacket != null && client.Entity.VortexPacket.ToArray() != null)
                        {
                            if (Now > client.Entity.VortexAttackStamp.AddMilliseconds(1400))
                            {
                                client.Entity.VortexAttackStamp = Now;
                                new Game.Attacking.Handle(client.Entity.VortexPacket, client.Entity, null);
                            }
                        }
                    }
                    else
                    {
                        client.Entity.VortexPacket = null;
                        var AttackPacket = client.Entity.AttackPacket;
                        if (AttackPacket != null && AttackPacket.ToArray() != null)
                        {
                            uint AttackType = AttackPacket.AttackType;
                            if (AttackType == Network.GamePackets.Attack.Magic || AttackType == Network.GamePackets.Attack.Melee || AttackType == Network.GamePackets.Attack.Ranged)
                            {
                                if (AttackType == Network.GamePackets.Attack.Magic)
                                {
                                    if (Now > client.Entity.AttackStamp.AddSeconds(1))
                                    {
                                        new Game.Attacking.Handle(AttackPacket, client.Entity, null);
                                    }
                                }
                                else
                                {
                                    int decrease = -300;
                                    if (client.Entity.OnCyclone())
                                        decrease = 700;
                                    if (client.Entity.OnSuperman())
                                        decrease = 200;
                                    if (Now > client.Entity.AttackStamp.AddMilliseconds((1000 - client.Entity.Agility - decrease) * (int)(AttackType == Network.GamePackets.Attack.Ranged ? 1 : 1)))
                                    {
                                        new Game.Attacking.Handle(AttackPacket, client.Entity, null);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Program.SaveException(e);
                    client.Entity.AttackPacket = null;
                    client.Entity.VortexPacket = null;
                }
            }
        }
        private void CompanionsCallback(GameClient client, int time)
        {
            if (!Valid(client)) return;
            Time32 Now = new Time32(time);

            if (client.Companion != null)
            {
                short distance = Kernel.GetDistance(client.Companion.X, client.Companion.Y, client.Entity.X, client.Entity.Y);
                if (distance >= 8)
                {
                    ushort X = (ushort)(client.Entity.X + Kernel.Random.Next(2));
                    ushort Y = (ushort)(client.Entity.Y + Kernel.Random.Next(2));
                    if (!client.Map.SelectCoordonates(ref X, ref Y))
                    {
                        X = client.Entity.X;
                        Y = client.Entity.Y;
                    }
                    client.Companion.X = X;
                    client.Companion.Y = Y;
                    Network.GamePackets.Data data = new Conquer_Online_Server.Network.GamePackets.Data(true);
                    data.ID = Network.GamePackets.Data.Jump;
                    data.dwParam = (uint)((Y << 16) | X);
                    data.wParam1 = X;
                    data.wParam2 = Y;
                    data.UID = client.Companion.UID;
                    client.Companion.MonsterInfo.SendScreen(data);
                }
                else if (distance > 4)
                {
                    Enums.ConquerAngle facing = Kernel.GetAngle(client.Companion.X, client.Companion.Y, client.Companion.Owner.Entity.X, client.Companion.Owner.Entity.Y);
                    if (!client.Companion.Move(facing))
                    {
                        facing = (Enums.ConquerAngle)Kernel.Random.Next(7);
                        if (client.Companion.Move(facing))
                        {
                            client.Companion.Facing = facing;
                            Network.GamePackets.GroundMovement move = new Conquer_Online_Server.Network.GamePackets.GroundMovement(true);
                            move.Direction = facing;
                            move.UID = client.Companion.UID;
                            move.GroundMovementType = Network.GamePackets.GroundMovement.Run;
                            client.Companion.MonsterInfo.SendScreen(move);
                        }
                    }
                    else
                    {
                        client.Companion.Facing = facing;
                        Network.GamePackets.GroundMovement move = new Conquer_Online_Server.Network.GamePackets.GroundMovement(true);
                        move.Direction = facing;
                        move.UID = client.Companion.UID;
                        move.GroundMovementType = Network.GamePackets.GroundMovement.Run;
                        client.Companion.MonsterInfo.SendScreen(move);
                    }
                }
                else
                {
                    var monster = client.Companion;
                    if (monster.MonsterInfo.InSight == 0)
                    {
                        if (client.Entity.AttackPacket != null)
                        {
                            if (client.Entity.AttackPacket.AttackType == Network.GamePackets.Attack.Magic)
                            {
                                if (client.Entity.AttackPacket.Decoded)
                                {
                                    if (Database.SpellTable.SpellInformations.ContainsKey((ushort)client.Entity.AttackPacket.Damage))
                                    {
                                        var info = Database.SpellTable.SpellInformations[(ushort)client.Entity.AttackPacket.Damage].Values.ToArray()[client.Spells[(ushort)client.Entity.AttackPacket.Damage].Level];
                                        if (info.CanKill)
                                        {
                                            monster.MonsterInfo.InSight = client.Entity.AttackPacket.Attacked;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                monster.MonsterInfo.InSight = client.Entity.AttackPacket.Attacked;
                            }
                        }
                    }
                    else
                    {
                        if (monster.MonsterInfo.InSight > 400000 && monster.MonsterInfo.InSight < 600000 || monster.MonsterInfo.InSight > 800000 && monster.MonsterInfo.InSight != monster.UID)
                        {
                            Entity attacked = null;

                            if (client.Screen.TryGetValue(monster.MonsterInfo.InSight, out attacked))
                            {
                                if (Now > monster.AttackStamp.AddMilliseconds(monster.MonsterInfo.AttackSpeed))
                                {
                                    monster.AttackStamp = Now;
                                    if (attacked.Dead)
                                    {
                                        monster.MonsterInfo.InSight = 0;
                                    }
                                    else
                                        new Game.Attacking.Handle(null, monster, attacked);
                                }
                            }
                            else
                                monster.MonsterInfo.InSight = 0;
                        }
                    }
                }
            }
        }
        private void PrayerCallback(GameClient client, int time)
        {
            if (!Valid(client)) return;
            Time32 Now = new Time32(time);

            if (client.Entity.Reborn > 1)
                return;

            if (!client.Entity.ContainsFlag(Network.GamePackets.Update.Flags.Praying))
            {
                foreach (Interfaces.IMapObject ClientObj in client.Screen.Objects)
                {
                    if (ClientObj != null)
                    {
                        if (ClientObj.MapObjType == Game.MapObjectType.Player)
                        {
                            var Client = ClientObj.Owner;
                            if (Client.Entity.ContainsFlag(Network.GamePackets.Update.Flags.CastPray))
                            {
                                if (Kernel.GetDistance(client.Entity.X, client.Entity.Y, ClientObj.X, ClientObj.Y) <= 3)
                                {
                                    client.Entity.AddFlag(Network.GamePackets.Update.Flags.Praying);
                                    client.PrayLead = Client;
                                    client.Entity.Action = Client.Entity.Action;
                                    Client.Prayers.Add(client);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (client.PrayLead != null)
                {
                    if (Kernel.GetDistance(client.Entity.X, client.Entity.Y, client.PrayLead.Entity.X, client.PrayLead.Entity.Y) > 4)
                    {
                        client.Entity.RemoveFlag(Network.GamePackets.Update.Flags.Praying);
                        client.PrayLead.Prayers.Remove(client);
                        client.PrayLead = null;
                    }
                }
            }
        }

        private void WorldTournaments(int time)
        {
            Time32 Now = new Time32(time);
            DateTime Now64 = DateTime.Now;

            #region Messages
            if (Now64.Minute == 16 && Now64.Second <= 2)
            {
                Kernel.SendWorldMessage(new Network.GamePackets.Message(Database.Messagess.Sys, System.Drawing.Color.White, Network.GamePackets.Message.Center), Program.GamePool);
            }
            if (Now64.Minute == 27 && Now64.Second <= 2 || Now64.Minute == 34 && Now64.Second <= 2 || Now64.Minute == 42 && Now64.Second <= 2 || Now64.Minute == 53 && Now64.Second <= 2 || Now64.Minute == 14 && Now64.Second <= 2)
            {
                Kernel.SendWorldMessage(new Network.GamePackets.Message(Database.Messagess.Sys2, System.Drawing.Color.White, Network.GamePackets.Message.Center), Program.GamePool);
            }
            if (Now64.Minute == 02 && Now64.Second <= 2)
            {
                Kernel.SendWorldMessage(new Network.GamePackets.Message(Database.Messagess.Sys3, System.Drawing.Color.White, Network.GamePackets.Message.Center), Program.GamePool);
            }
            if (Now64.Minute == 42 && Now64.Second <= 2)
            {
                Kernel.SendWorldMessage(new Network.GamePackets.Message(Database.Messagess.Sys4, System.Drawing.Color.White, Network.GamePackets.Message.Center), Program.GamePool);
            }
            if (Now64.Minute == 52 && Now64.Second <= 02)
            {
                Kernel.SendWorldMessage(new Network.GamePackets.Message(Database.Messagess.Sys5, System.Drawing.Color.White, Network.GamePackets.Message.Center), Program.GamePool);
            }
            #endregion
            #region  Auto Restart
            if (Now64.Hour == 23 && Now64.Minute == 55 && Now64.Second <= 00)   
            {
                Conquer_Online_Server.Network.PacketHandler.WorldMessage("Warrning !!! Server Will Restart Aftr 5 Minutes get Ready !!");
            }
            if (Now64.Hour == 23 && Now64.Minute == 56 && Now64.Second <= 00)  
            {
                Conquer_Online_Server.Network.PacketHandler.WorldMessage("Warrning !!! Server Will Restart Aftr 4 Minutes get Ready !!");
            }
            if (Now64.Hour == 23 && Now64.Minute == 57 && Now64.Second <= 00)   
            {
                Conquer_Online_Server.Network.PacketHandler.WorldMessage("Warrning !!! Server Will Restart Aftr 3 Minutes get Ready !!");

            }
            if (Now64.Hour == 23 && Now64.Minute == 58 && Now64.Second <= 00)   
            {
                Conquer_Online_Server.Network.PacketHandler.WorldMessage("Warrning !!! Server Will Restart Aftr 2 Minutes get Ready !!");

            }
            if (Now64.Hour == 23 && Now64.Minute == 59 && Now64.Second <= 00)    
            {
                Conquer_Online_Server.Network.PacketHandler.WorldMessage("Warrning !!! Server Will Restart Aftr 1 Minutes get Ready !!");

            }
            if (Now64.Hour == 23 && Now64.Minute == 59 && Now64.Second <= 30)    
            {
                Conquer_Online_Server.Network.PacketHandler.WorldMessage("Warrning !!! Server Will Restart After 30 Seconds get Ready !!");
            }
            if (Now64.Hour == 23 && Now64.Minute == 59 && Now64.Second <= 58)   
            {
                Program.CommandsAI("@save");  
            }
            if (Now64.Hour == 00 && Now64.Minute == 00 && Now64.Second <= 00)  
            {
                Program.CommandsAI("@restart");   
            }
            #endregion   Auto Restart                                           
            #region Ta2lifation
            {
                GHRooms_Execute();
            }
            #endregion Ta2lefation
            #region Elite GW
            if (DateTime.Now.DayOfWeek != DayOfWeek.Saturday && DateTime.Now.DayOfWeek != DayOfWeek.Sunday)

                if (!Game.EliteGuildWar.IsWar)
                {
                    if (Now64.Hour == 17 && Now64.Minute == 00 && Now64.Second == 02)
                    {
                        Game.EliteGuildWar.Start();
                        Kernel.SendWorldMessage(new Message("Elite GW began!", Color.White, Message.Center), Program.GamePool);
                        foreach (var client in Program.GamePool)
                            if (client.Entity.MapID == 6000 || client.Entity.MapID == 6001 || client.Entity.MapID == 6002 || client.Entity.MapID == 6003 || client.Entity.MapID == 6004)
                                return;
                        foreach (var client in Program.GamePool)
                            if (client.Entity.GuildID != 0)
                                client.MessageBox("Elite GuildWar has begun! Would you like to join? ",
                                    p => { p.Entity.Teleport(1002, 314, 150); }, null);
                    }
                }
            if (Game.EliteGuildWar.IsWar)
            {
                if (Time32.Now > Game.EliteGuildWar.ScoreSendStamp.AddSeconds(3))
                {
                    Game.EliteGuildWar.ScoreSendStamp = Time32.Now;
                    Game.EliteGuildWar.SendScores();
                }
                if (Now64.Hour == 17 && Now64.Minute == 50 && Now64.Second <= 2)
                {
                    Kernel.SendWorldMessage(new Network.GamePackets.Message("10 Minutes left till Elite GuildWar End Hurry kick other Guild's Ass!.", System.Drawing.Color.White, Network.GamePackets.Message.Center), Program.GamePool);
                }
            }

            if (Game.EliteGuildWar.IsWar)
            {
                if (Now64.Hour == 18 && Now64.Minute == 00 && Now64.Second == 02)
                {
                    Game.EliteGuildWar.End();
                    {
                        //Kernel.SendWorldMessage(new Network.GamePackets.Message("Elite GW has Ended", System.Drawing.Color.White, Network.GamePackets.Message.Center), Program.GamePool);
                    }
                }
            }
            #endregion
            #region PoleTwin
            if (DateTime.Now.DayOfWeek != DayOfWeek.Saturday && DateTime.Now.DayOfWeek != DayOfWeek.Sunday)

                if (!Game.PoleTwin.IsWar)
                {
                    if (Now64.Hour == 18 && Now64.Minute == 00 && Now64.Second == 04)
                    {
                        Game.PoleTwin.Start();

                        foreach (var client in Program.GamePool)
                            if (client.Entity.MapID == 6000 || client.Entity.MapID == 6001 || client.Entity.MapID == 6002 || client.Entity.MapID == 6003 || client.Entity.MapID == 6004)
                                return;
                        foreach (var client in Program.GamePool)
                            if (client.Entity.GuildID != 0)
                                client.MessageBox("PoleTwin has begun! Would you like to join? ",
                                    p => { p.Entity.Teleport(1002, 343, 251); }, null);
                    }
                }
            if (Game.PoleTwin.IsWar)
            {
                if (Time32.Now > Game.PoleTwin.ScoreSendStamp.AddSeconds(3))
                {
                    Game.PoleTwin.ScoreSendStamp = Time32.Now;
                    Game.PoleTwin.SendScores();
                }
                if (Now64.Hour == 18 && Now64.Minute == 50 && Now64.Second <= 2)
                {
                    Kernel.SendWorldMessage(new Network.GamePackets.Message("10 Minutes left till PoleTwin End Hurry kick other Guild's Ass!.", System.Drawing.Color.White, Network.GamePackets.Message.Center), Program.GamePool);
                }
            }

            if (Game.PoleTwin.IsWar)
            {
                if (Now64.Hour == 19 && Now64.Minute == 00 && Now64.Second == 04)
                {
                    Game.PoleTwin.End();
                    {

                    }
                }
            }
            #endregion  
            #region PoleIslanD

            if (!Game.PoleIslanD.IsWar)
            {
                if (Now64.Hour == 16 && Now64.Minute == 00 && Now64.Second == 04)
                {
                    Game.PoleIslanD.Start();

                    foreach (var client in Program.GamePool)
                        if (client.Entity.MapID == 6000 || client.Entity.MapID == 6001 || client.Entity.MapID == 6002 || client.Entity.MapID == 6003 || client.Entity.MapID == 6004)
                            return;
                    foreach (var client in Program.GamePool)
                        if (client.Entity.GuildID != 0)
                            client.MessageBox("PoleIslanD has begun! Would you like to join? ",
                                p => { p.Entity.Teleport(1002, 339, 251); }, null);
                }
            }
            if (Game.PoleIslanD.IsWar)
            {
                if (Time32.Now > Game.PoleTwin.ScoreSendStamp.AddSeconds(3))
                {
                    Game.PoleIslanD.ScoreSendStamp = Time32.Now;
                    Game.PoleIslanD.SendScores();
                }
                if (Now64.Hour == 16 && Now64.Minute == 50 && Now64.Second <= 2)
                {
                    Kernel.SendWorldMessage(new Network.GamePackets.Message("10 Minutes left till PoleIslanD End Hurry kick other Guild's Ass!.", System.Drawing.Color.White, Network.GamePackets.Message.Center), Program.GamePool);
                }
            }

            if (Game.PoleIslanD.IsWar)
            {
                if (Now64.Hour == 17 && Now64.Minute == 00 && Now64.Second == 04)
                {
                    Game.PoleIslanD.End();
                    {

                    }
                }
            }
            #endregion  

            #region Couples PK War
            if (Now64.DayOfWeek == DayOfWeek.Friday && Now64.Hour == 18 && Now64.Minute == 00 && Now64.Second <= 2)
            {
                Kernel.SendWorldMessage(new Network.GamePackets.Message("Couples PkWar has started! You have 5 minute to signup go to TC CouplesPkGuide in TwinCity!", System.Drawing.Color.White, Network.GamePackets.Message.Center), Program.GamePool);
                foreach (var client in Program.GamePool)
                    if (client.Entity.Spouse != "None")
                        client.MessageBox("Couples PkWar has started! Would you like to join? [Prize: 200k CPs]",
                            p => { p.Entity.Teleport(1002, 295, 191); }, null);
            }
            #endregion  
            #region SnowBanshe
            if (Now64.Minute == 26 && Now64.Second == 00)
            {
                foreach (var client in Program.GamePool)
                    if (client.Entity.MapID == 6000 || client.Entity.MapID == 6001 || client.Entity.MapID == 6002 || client.Entity.MapID == 6003 || client.Entity.MapID == 6004)
                        client.MessageBox("You can Spawn SnowBanshee within 1 Min wanna Spawn it?",
                            (p) => { p.Entity.Teleport(1762, 540, 431); }, null, 60);
            }
            if (Now64.Minute == 27 && Now64.Second == 00 && Kernel.Spawn == false)
            {
                Kernel.Spawn = true;
                Kernel.SendWorldMessage(new Conquer_Online_Server.Network.GamePackets.Message("You Can Spawn The  Beast Snow Banshee Now!!", Color.Red, 2012));
            }
            #endregion  
            #region NemesisTyrant
            if (Now64.Minute == 24 && Now64.Second == 00)
            {
                foreach (var client in Program.GamePool)
                    if (client.Entity.MapID == 6000 || client.Entity.MapID == 6001 || client.Entity.MapID == 6002 || client.Entity.MapID == 6003 || client.Entity.MapID == 6004)
                        client.MessageBox("You can Spawn NemesisTyrant within 1 Min wanna Spawn it?",
                            (p) => { p.Entity.Teleport(1762, 540, 431); }, null, 60);
            }
            if (Now64.Minute == 25 && Now64.Second == 00 && Kernel.Spawn == false)
            {
                Kernel.Spawn = true;
                Kernel.SendWorldMessage(new Conquer_Online_Server.Network.GamePackets.Message("You Can Spawn The  Beast NemesisTyrant Now!!", Color.Red, 2012));
            }
            #endregion  
            #region Dis City
            if (Now64.DayOfWeek == DayOfWeek.Wednesday || Now64.DayOfWeek == DayOfWeek.Friday)
            {
                if ((Now64.Hour == 12 || Now64.Hour == 19) && Now64.Minute == 05 && Now64.Second <= 2)
                {
                    Kernel.SendWorldMessage(new Network.GamePackets.Message("DisCity signup has been closed. Please try next time!", System.Drawing.Color.White, Network.GamePackets.Message.Center), Program.GamePool);
                    Game.Features.DisCity.Signup = false;
                }
            }
            #endregion
            #region Clan War
            foreach (var client in Program.GamePool)
                if (client.Entity.MapID == 6000 || client.Entity.MapID == 6001 || client.Entity.MapID == 6002 || client.Entity.MapID == 6003 || client.Entity.MapID == 6004)
                    return;
            if (DateTime.Now.DayOfWeek != DayOfWeek.Saturday && DateTime.Now.DayOfWeek != DayOfWeek.Sunday)

                if (Now64.Hour == 19 && Now64.Minute == 00 && !ClanWar.IsWar)
                {

                    object[] name;
                    name = new object[] { "ClanWar Quest Have Started Go To Jion in TwinCity at (292, 150)" };
                    Kernel.SendWorldMessage(new Message(string.Concat(name), "Clan", "War", System.Drawing.Color.Red, 2500), Program.GamePool); 
                    Game.ClanWar.Start();
                    ClanWarAI = false;
                    if (Now64.Hour != 19)
                    {
                        ClanWarAI = Now64.Hour != 19;
                        foreach (var client in Program.GamePool)
                            if (client.Entity.GuildID != 0)
                                client.MessageBox("ClanWar has begun! Would you like to join?",
                                    p => { p.Entity.Teleport(1002, 292, 150); }, null);
                    }
                }

            if (Now64.Hour == 20 && Now64.Minute == 00 && ClanWar.IsWar)
            {
                Game.ClanWar.End();
            }
            if (Game.ClanWar.IsWar)
            {
                if (Time32.Now > Game.ClanWar.ScoreSendStamp.AddSeconds(3))
                {
                    Game.ClanWar.ScoreSendStamp = Time32.Now;
                    Game.ClanWar.SendScores();
                }
            }

            #endregion
          
            #region Monthly PK
            {
                foreach (var client in Program.GamePool)
                    if (client.Entity.MapID == 6000 || client.Entity.MapID == 6001 || client.Entity.MapID == 6002 || client.Entity.MapID == 6003 || client.Entity.MapID == 6004)
                        return;
                if (Now64.DayOfYear == 1 && Now64.Hour == 14 && Now64.Minute == 00 && Now64.Second >= 2)
                {
                    MonthlyPKWar = true;
                    Kernel.SendWorldMessage(new Message("The Monthly PK War began!", Color.Red, 2012));
                    foreach (var client in Program.GamePool)
                        client.MessageBox("Monthly PK War has begun! Would you like to join?",
                            p => { p.Entity.Teleport(1002, 295, 150); }, null);
                }
                if (Now64.Hour == 14 && Now64.Minute >= 08 && MonthlyPKWar)
                {
                    MonthlyPKWar = false;
                    Kernel.SendWorldMessage(new Message("The Monthly PK War ended!", Color.Red, Message.Center));
                }
            }
            #endregion
            #region DailyPK
            {
                foreach (var client in Program.GamePool)
                    if (client.Entity.MapID == 6000 || client.Entity.MapID == 6001 || client.Entity.MapID == 102 || client.Entity.MapID == 6002 || client.Entity.MapID == 6003 || client.Entity.MapID == 6004)
                        return;
                if (Now64.Minute == 00 && Now64.Second == 4)
                {
                    Kernel.SendWorldMessage(new Message("DailyPk Has Began!", Color.White, Message.TopLeft), Program.GamePool);
                    foreach (var client in Program.GamePool)
                        client.MessageBox("DailyPk Started! Would you like to join?",
                            p => { p.Entity.Teleport(1002, 324, 264); }, null, 3);
                }
            }
            #endregion
            #region GuildWar
            if (GuildWar.IsWar)
            {
                if (Time32.Now > GuildWar.ScoreSendStamp.AddSeconds(3))
                {
                    GuildWar.ScoreSendStamp = Time32.Now;
                    GuildWar.SendScores();
                }
                if (!GuildWar.Flame10th)
                {
                    if (Now64.DayOfWeek == DayOfWeek.Wednesday && Now64.Hour == 18 && Now64.Minute == 00)
                    {

                        object[] name;
                        name = new object[] { "Guild War Quest Have Started Go To Jion in TwinCity at (349, 341)" };
                        Kernel.SendWorldMessage(new Message(string.Concat(name), "Guild", "War", System.Drawing.Color.Red, 2500), Program.GamePool); 
                        GuildWar.Flame10th = true;
                        Kernel.SendWorldMessage(new Message("You can now light the 10thFlame!", Color.White, Message.Center), Program.GamePool);
                    }
                }
            }
            if ((Now64.Hour >= 18 && Now64.Hour <= 21) && Now64.DayOfWeek == DayOfWeek.Wednesday)
            {
                if (!GuildWar.IsWar)
                {
                    GuildWar.Start();
                    foreach (var client in Program.GamePool)
                        if (client.Entity.GuildID != 0)
                            client.MessageBox("GuildWar has begun! Would you like to join To Win 30Milion?",
                                p => { p.Entity.Teleport(1002, 224, 236); }, null);
                }
            }
            if (Now64.Hour == 18 && Now64.Minute == 00 && Now64.Second == 7 && Now64.DayOfWeek == DayOfWeek.Wednesday)
            {
                if (GuildWar.IsWar)
                {
                    foreach (var client in Program.GamePool)
                        if (client.Entity.GuildID != 0)
                            client.MessageBox("GuildWar  has begun! Would you like to join to win 30Milion?",
                                p => { p.Entity.Teleport(1038, 349, 341); }, null);
                }
            }
            if (GuildWar.IsWar)
            {
                if (Now64.Hour == 18 && Now64.Second <= 5)
                {
                    GuildWar.Flame10th = false;
                    GuildWar.End();
                }
            }
            #endregion
           /* #region DTM
            if (Now64.DayOfWeek == DayOfWeek.Tuesday && (Now64.Hour == 14 || Now64.Hour == 22) && Now64.Minute == 59 && Now64.Second <= 2)
            {
                DeathMatch.SendTimer();
                Kernel.SendWorldMessage(new Message("Team Death match will start in one minute!", Color.White, Message.TopLeft), Program.GamePool);
            }
            #endregion*/

            #region CrossServer
            if (Now64.Hour == CrossServer.hour && Now64.Minute == 01 && Now64.Second == 5 && !CrossServer.IsWar)
            {
                CrossServer.Start();
                foreach (var client in Program.GamePool)
                    client.MessageBox("CrossServer CTF has begun! Would you like to join?",
                           p => { p.Entity.Teleport(1002, 225, 237); }, null);
            }
            if (CrossServer.IsWar)
            {
                if (DateTime.Now > Game.CrossServer.StartTime.AddHours(1.0))
                {
                    CrossServer.End();
                }
            }
            if (Game.CrossServer.IsWar)
            {
                if (Time32.Now > Game.CrossServer.ScoreSendStamp.AddSeconds(3))
                {
                    Game.CrossServer.ScoreSendStamp = Time32.Now;
                    Game.CrossServer.SendScores();
                }
            }
            #endregion
           
            #region Quiz Show
            if (Now64.DayOfWeek == DayOfWeek.Saturday || Now64.DayOfWeek == DayOfWeek.Sunday)
            {
                if (Now64.Hour == 3 || Now64.Hour == 13 || Now64.Hour == 20)
                {
                    if (Now64.Minute == 55 && Now64.Second <= 2)
                    {
                        Kernel.SendWorldMessage(new Network.GamePackets.Message("Quiz show will start in 5 minutes!", System.Drawing.Color.White, Conquer_Online_Server.Network.GamePackets.Message.TopLeft), Program.GamePool);
                    }
                }
                if (Now64.Hour == 4 || Now64.Hour == 14 || Now64.Hour == 21)
                {
                    if (Now64.Minute == 0 && Now64.Second <= 2)
                    {
                        Kernel.QuizShow.Start();
                        Kernel.SendWorldMessage(new Network.GamePackets.Message("Quiz show has started!", System.Drawing.Color.White, Conquer_Online_Server.Network.GamePackets.Message.TopLeft), Program.GamePool);
                    }
                }
            }
            #endregion
            #region Elite PK Tournament
            //#warning EPK FRIDAY NOT EVERYDAY AT 19 NOT 13
            if (/*Now64.DayOfWeek == DayOfWeek.Friday && */Now64.Hour == ElitePK.EventTime && Now64.Minute >= 55 && !ElitePKTournament.TimersRegistered)
            {

                object[] name;
                name = new object[] { "ElitePk Quest Have Started Go To Jion in TwinCity at (310, 150)" };
                Kernel.SendWorldMessage(new Message(string.Concat(name), "Elite", "Pk", System.Drawing.Color.Red, 2500), Program.GamePool); 
                ElitePKTournament.RegisterTimers();
                ElitePKBrackets brackets = new ElitePKBrackets(true, 0);
                brackets.Type = ElitePKBrackets.EPK_State;
                brackets.OnGoing = true;
                foreach (var client in Program.GamePool)
                {
                    client.Send(brackets);
                    client.MessageBox("Elite PK Tournament has started! Would you like to join?",
                        p => { p.Entity.Teleport(1002, 310, 150); }, null);
                }
            }

            if (/*Now64.DayOfWeek == DayOfWeek.Friday && */Now64.Hour >= ElitePK.EventTime + 1 && ElitePKTournament.TimersRegistered)
            {
                bool done = true;
                foreach (var epk in ElitePKTournament.Tournaments)
                    if (epk.Players.Count != 0)
                        done = false;
                if (done)
                {
                    ElitePKTournament.TimersRegistered = false;
                    ElitePKBrackets brackets = new ElitePKBrackets(true, 0);
                    brackets.Type = ElitePKBrackets.EPK_State;
                    brackets.OnGoing = false;
                    foreach (var client in Program.GamePool)
                        client.Send(brackets);
                }
            }
            #endregion
            #region Capture the flag

            if (Now64.Hour == 21 && Now64.Minute >= 30 && !CaptureTheFlag.IsWar)
            {
                CaptureTheFlag.IsWar = true;
                CaptureTheFlag.StartTime = DateTime.Now;
                foreach (var guild in Kernel.Guilds.Values)
                {
                    guild.CTFFlagScore = 0;
                    guild.CTFPoints = 0;
                }
                foreach (var client in Program.GamePool)
                    if (client.Entity.GuildID != 0)
                        client.MessageBox("Capture the Flag has begun! Would you like to join? [Prize: Guild fund]",
                            p => { p.Entity.Teleport(1002, 225, 237); }, null);
            }

            if (CaptureTheFlag.IsWar)
            {
                //Program.World.CTF.SendUpdates();
                if (Now64 > CaptureTheFlag.StartTime.AddHours(1))
                {
                    CaptureTheFlag.IsWar = false;
                    CaptureTheFlag.Close();
                }
            }
            if (CTF != null)
                CTF.SpawnFlags();
            #endregion
#if OBSOLETE_CLASSPK
           /* if (Now64.Hour == 20 && Now64.Minute == 00 && Now64.DayOfWeek == DayOfWeek.Saturday)
            {
                if (Now64.Minute == 00 && Now64.Second <= 2)
                {
                    Game.ClassPk.SignUp();
                    Conquer_Online_Server.Kernel.SendWorldMessage(new Conquer_Online_Server.Network.GamePackets.Message("Trojan Pk War Has been Started Go to ClassPkEnvoy in TwinCity to SignUp Before 20:05 ", System.Drawing.Color.White, Conquer_Online_Server.Network.GamePackets.Message.TopLeft), Program.GamePool);
                }
            }
            if (Now64.Hour == 20 && Now64.Minute == 00 && Now64.DayOfWeek == DayOfWeek.Sunday)
            {
                if (Now64.Minute == 00 && Now64.Second <= 2)
                {
                    Game.ClassPk.SignUp1();
                    Conquer_Online_Server.Kernel.SendWorldMessage(new Conquer_Online_Server.Network.GamePackets.Message("Warrior Pk War Has been Started Go to ClassPkEnvoy in TwinCity to SignUp Before 20:05 ", System.Drawing.Color.White, Conquer_Online_Server.Network.GamePackets.Message.TopLeft), Program.GamePool);
                }
            }
            if (Now64.Hour == 20 && Now64.Minute == 00 && Now64.DayOfWeek == DayOfWeek.Monday)
            {
                if (Now64.Minute == 00 && Now64.Second <= 2)
                {
                    Game.ClassPk.SignUp6();
                    Conquer_Online_Server.Kernel.SendWorldMessage(new Conquer_Online_Server.Network.GamePackets.Message("Fire Pk War Has been Started Go to ClassPkEnvoy in TwinCity to SignUp Before 20:05 ", System.Drawing.Color.White, Conquer_Online_Server.Network.GamePackets.Message.TopLeft), Program.GamePool);
                }
            }
            if (Now64.Hour == 20 && Now64.Minute == 00 && Now64.DayOfWeek == DayOfWeek.Tuesday)
            {
                if (Now64.Minute == 00 && Now64.Second <= 2)
                {
                    Game.ClassPk.SignUp3();
                    Conquer_Online_Server.Kernel.SendWorldMessage(new Conquer_Online_Server.Network.GamePackets.Message("Ninja Pk War Has been Started Go to ClassPkEnvoy in TwinCity to SignUp Before 20:05 ", System.Drawing.Color.White, Conquer_Online_Server.Network.GamePackets.Message.TopLeft), Program.GamePool);
                }
            }
            if (Now64.Hour == 8 && Now64.Minute == 00 && Now64.DayOfWeek == DayOfWeek.Monday)
            {
                Game.ClassPk.SignUp8();
                Conquer_Online_Server.Kernel.SendWorldMessage(new Conquer_Online_Server.Network.GamePackets.Message("Pirate Pk War Has been Started Go to ClassPkEnvoy in TwinCity to SignUp Before 20:05 ", System.Drawing.Color.White, Conquer_Online_Server.Network.GamePackets.Message.TopLeft), Program.GamePool);
            }
            if (Now64.Hour == 20 && Now64.Minute == 00 && Now64.DayOfWeek == DayOfWeek.Thursday)
            {
                if (Now64.Minute == 00 && Now64.Second <= 2)
                {
                    Game.ClassPk.SignUp5();
                    Conquer_Online_Server.Kernel.SendWorldMessage(new Conquer_Online_Server.Network.GamePackets.Message("Water Pk War Has been Started Go to ClassPkEnvoy in TwinCity to SignUp Before 20:05 ", System.Drawing.Color.White, Conquer_Online_Server.Network.GamePackets.Message.TopLeft), Program.GamePool);
                }
            }
            if (Now64.Hour == 20 && Now64.Minute == 00 && Now64.DayOfWeek == DayOfWeek.Wednesday)
            {
                if (Now64.Minute == 00 && Now64.Second <= 2)
                {
                    Game.ClassPk.SignUp4();
                    Conquer_Online_Server.Kernel.SendWorldMessage(new Conquer_Online_Server.Network.GamePackets.Message("Monk Pk War Has been Started Go to ClassPkEnvoy in TwinCity to SignUp Before 20:05 ", System.Drawing.Color.White, Conquer_Online_Server.Network.GamePackets.Message.TopLeft), Program.GamePool);
                }
            }
            if (Now64.Hour == 20 && Now64.Minute == 00 && Now64.DayOfWeek == DayOfWeek.Friday)
            {
                if (Now64.Minute == 00 && Now64.Second <= 2)
                {
                    Game.ClassPk.SignUp2();
                    Conquer_Online_Server.Kernel.SendWorldMessage(new Conquer_Online_Server.Network.GamePackets.Message("Archer Pk War Has been Started Go to ClassPkEnvoy in TwinCity to SignUp Before 20:05 ", System.Drawing.Color.White, Conquer_Online_Server.Network.GamePackets.Message.TopLeft), Program.GamePool);
                }
            }*/
#endif
        }
        private void ServerFunctions(int time)
        {
            var kvpArray = Kernel.GamePool.ToArray();
            foreach (var kvp in kvpArray)
                if (kvp.Value == null || kvp.Value.Entity == null)
                    Kernel.GamePool.Remove(kvp.Key);
            Program.GamePool = Kernel.GamePool.Values.ToArray();
            new Database.MySqlCommand(Database.MySqlCommandType.UPDATE).Update("configuration").Set("GuildID", Game.ConquerStructures.Society.Guild.GuildCounter.Now).Set("ItemUID", Network.GamePackets.ConquerItem.ItemUID.Now).Where("Server", Constants.ServerName).Execute();
            Database.EntityVariableTable.Save(0, Program.Vars);
            if (Kernel.BlackSpoted.Values.Count > 0)
            {
                foreach (var spot in Kernel.BlackSpoted.Values)
                {
                    if (Time32.Now >= spot.BlackSpotStamp.AddSeconds(spot.BlackSpotStepSecs))
                    {
                        if (spot.Dead && spot.EntityFlag == EntityFlag.Player)
                        {
                            foreach (var h in Program.GamePool)
                            {
                                h.Send(Program.BlackSpotPacket.ToArray(false, spot.UID));
                            }
                            Kernel.BlackSpoted.Remove(spot.UID);
                            continue;
                        }
                        foreach (var h in Program.GamePool)
                        {
                            h.Send(Program.BlackSpotPacket.ToArray(false, spot.UID));
                        }
                        spot.IsBlackSpotted = false;
                        Kernel.BlackSpoted.Remove(spot.UID);
                    }
                }
            }
            DateTime Now = DateTime.Now;

            if (Now > Game.ConquerStructures.Broadcast.LastBroadcast.AddMinutes(1))
            {
                if (Game.ConquerStructures.Broadcast.Broadcasts.Count > 0)
                {
                    Game.ConquerStructures.Broadcast.CurrentBroadcast = Game.ConquerStructures.Broadcast.Broadcasts[0];
                    Game.ConquerStructures.Broadcast.Broadcasts.Remove(Game.ConquerStructures.Broadcast.CurrentBroadcast);
                    Game.ConquerStructures.Broadcast.LastBroadcast = Now;
                    Kernel.SendWorldMessage(new Network.GamePackets.Message(Game.ConquerStructures.Broadcast.CurrentBroadcast.Message, "ALLUSERS", Game.ConquerStructures.Broadcast.CurrentBroadcast.EntityName, System.Drawing.Color.Red, Network.GamePackets.Message.BroadcastMessage), Program.GamePool);
                }
                else
                    Game.ConquerStructures.Broadcast.CurrentBroadcast.EntityID = 1;
            }
            if (Program.ServerRrestart == false)
            {
                if (Now >= Program.StartDate.AddHours(12))
                {
                    if (mess == 0)
                    {
                        foreach (Client.GameClient Server in Kernel.GamePool.Values)
                        {
                            //if (DateTime.Now.DayOfWeek == DayOfWeek.Monday || DateTime.Now.DayOfWeek == DayOfWeek.Wednesday || DateTime.Now.DayOfWeek == DayOfWeek.Friday)
                            //{

                            Network.GamePackets.Message FiveMinute = new Network.GamePackets.Message("The server will be brought down for maintenance in 5 Minutes.", System.Drawing.Color.Red, Network.GamePackets.Message.Center);

                            Server.Send(FiveMinute);

                            // }
                        }
                        mess++;
                        messtime = Time32.Now;
                    }
                    if (mess == 1 && Time32.Now >= messtime.AddMinutes(1))
                    {
                        foreach (Client.GameClient Server in Kernel.GamePool.Values)
                        {

                            Network.GamePackets.Message FiveMinute = new Network.GamePackets.Message("The server will be brought down for maintenance in 4 Minutes.", System.Drawing.Color.Red, Network.GamePackets.Message.Center);
                            Server.Send(FiveMinute);

                        }
                        mess++;
                        messtime = Time32.Now;
                    }
                    if (mess == 2 && Time32.Now >= messtime.AddMinutes(1))
                    {
                        foreach (Client.GameClient Server in Kernel.GamePool.Values)
                        {

                            Network.GamePackets.Message FiveMinute = new Network.GamePackets.Message("The server will be brought down for maintenance in 3 Minutes.", System.Drawing.Color.Red, Network.GamePackets.Message.Center);
                            Server.Send(FiveMinute);

                        }
                        mess++;
                        messtime = Time32.Now;
                    }
                    if (mess == 3 && Time32.Now >= messtime.AddMinutes(1))
                    {
                        foreach (Client.GameClient Server in Kernel.GamePool.Values)
                        {

                            Network.GamePackets.Message FiveMinute = new Network.GamePackets.Message("The server will be brought down for maintenance in 2 Minutes.", System.Drawing.Color.Red, Network.GamePackets.Message.Center);
                            Server.Send(FiveMinute);

                        }
                        mess++;
                        messtime = Time32.Now;
                    }
                    if (mess == 4 && Time32.Now >= messtime.AddMinutes(1))
                    {
                        foreach (Client.GameClient Server in Kernel.GamePool.Values)
                        {

                            Network.GamePackets.Message FiveMinute = new Network.GamePackets.Message("The server will be brought down for maintenance in 1 Minute.", System.Drawing.Color.Red, Network.GamePackets.Message.Center);
                            Server.Send(FiveMinute);

                        }
                        mess++;
                        messtime = Time32.Now;
                    }
                    if (mess == 5 && Time32.Now >= messtime.AddSeconds(30))
                    {
                        foreach (Client.GameClient Server in Kernel.GamePool.Values)
                        {

                            Network.GamePackets.Message FiveMinute = new Network.GamePackets.Message("The server has brought down for maintenance for Minute.", System.Drawing.Color.Red, Network.GamePackets.Message.Center);
                            Server.Send(FiveMinute);
                            Program.CommandsAI("@dorda");
                            Program.CommandsAI("@save");
                            Program.CommandsAI("@restart");

                        }
                    }
                }
                if (Now > Program.LastRandomReset.AddMinutes(30))
                {
                    Program.LastRandomReset = Now;
                    Kernel.Random = new FastRandom(Program.RandomSeed);
                }
                Program.Today = Now.DayOfWeek;
            }
        }
        private void ArenaFunctions(int time)
        {
            Game.Arena.EngagePlayers();
            Game.Arena.CheckGroups();
            Game.Arena.VerifyAwaitingPeople();
            Game.Arena.Reset();
        }
        private void TeamArenaFunctions(int time)
        {
            Game.TeamArena.EngagePlayers();
            Game.TeamArena.CheckGroups();
            Game.TeamArena.VerifyAwaitingPeople();
            Game.TeamArena.Reset();
        }
        private void ChampionFunctions(int time)
        {
            Game.Champion.EngagePlayers();
            Game.Champion.CheckGroups();
            Game.Champion.VerifyAwaitingPeople();
            Game.Champion.Reset();
        }

        #region Funcs
        public static void Execute(Action<int> action, int timeOut = 0, ThreadPriority priority = ThreadPriority.Normal)
        {
            GenericThreadPool.Subscribe(new LazyDelegate(action, timeOut, priority));
        }
        static void GHRooms_Execute()
        {
            #region Rooms FBandSS
            #region Room1
            if (Room1 == false)
            {
                int entered1 = 0;
                foreach (Client.GameClient Player in Kernel.GamePool.Values)
                {
                    if (Player.Entity.MapID == 4573 && (!Player.Entity.Dead))
                    {
                        entered1++;
                    }
                }
                if (entered1 > 1)
                {
                    Room1 = true;
                }
                else if (entered1 == 1)
                {
                    foreach (Client.GameClient Player in Kernel.GamePool.Values)
                    {
                        if (Player.Entity.MapID == 4573 && (!Player.Entity.Dead))
                        {
                            if (Time32.Now > Player.Entity.WaitingTimeFB.AddSeconds(20))
                            {
                                Player.Entity.ConquerPoints += Room1Price;
                                Room1Price = 0;
                                Player.Entity.Teleport(1002, 427, 378);
                            }
                        }
                    }
                }
            }
            else
            {
                int alive1 = 0;
                foreach (Client.GameClient Player in Kernel.GamePool.Values)
                {
                    if (Player.Entity.MapID == 4573 && (!Player.Entity.Dead))
                    {
                        alive1++;
                    }
                }
                if (alive1 == 1)
                {
                    foreach (Client.GameClient Player in Kernel.GamePool.Values)
                    {
                        if (Player.Entity.MapID == 4573)
                        {
                            if (!Player.Entity.Dead)//winner
                            {
                                Player.Entity.ConquerPoints += Room1Price * 2;
                                Player.Entity.WaitingTimeFB = Time32.Now;
                                Room1 = false;
                                Kernel.SendWorldMessage(new Network.GamePackets.Message(string.Concat(new object[] { "Congratulations! ", Player.Entity.Name, " has won ", Room1Price * 2, " CPs FB/SS in Room 1." }), System.Drawing.Color.Black, 0x7db), Program.GamePool);
                                Room1Price = 0;
                                _String str = new _String(true)
                                {
                                    UID = Player.Entity.UID,
                                    TextsCount = 1,
                                    Type = 10
                                };
                                str.Texts.Add("sports_victory");
                                Player.SendScreen(str, true);
                                Player.Entity.WinnerWaiting = Time32.Now;
                                Player.Entity.aWinner = true;
                            }
                            else//loser
                            {
                                Player.Entity.Teleport(1002, 439, 392);

                                _String str = new _String(true)
                                {
                                    UID = Player.Entity.UID,
                                    TextsCount = 1,
                                    Type = 10
                                };
                                str.Texts.Add("sports_failure");
                                Player.SendScreen(str, true);
                                Player.Entity.Action = Game.Enums.ConquerAction.None;
                                Player.ReviveStamp = Time32.Now;
                                Player.Attackable = false;

                                Player.Entity.TransformationID = 0;
                                Player.Entity.RemoveFlag(Update.Flags.Dead);
                                Player.Entity.RemoveFlag(Update.Flags.Ghost);
                                Player.Entity.Hitpoints = Player.Entity.MaxHitpoints;

                                Player.Entity.Ressurect();
                            }
                        }
                    }
                }
            }
            #endregion
            #region Room2
            if (Room2 == false)
            {
                int entered2 = 0;
                foreach (Client.GameClient Player in Kernel.GamePool.Values)
                {
                    if (Player.Entity.MapID == 4574 && (!Player.Entity.Dead))
                    {
                        entered2++;
                    }
                }
                if (entered2 > 1)
                {
                    Room2 = true;
                }
                else if (entered2 == 1)
                {
                    foreach (Client.GameClient Player in Kernel.GamePool.Values)
                    {
                        if (Player.Entity.MapID == 4574 && (!Player.Entity.Dead))
                        {
                            if (Time32.Now > Player.Entity.WaitingTimeFB.AddSeconds(20))
                            {
                                Player.Entity.ConquerPoints += Room2Price;
                                Room2Price = 0;
                                Player.Entity.Teleport(1002, 427, 378);
                            }
                        }
                    }
                }
            }
            else
            {
                int alive2 = 0;
                foreach (Client.GameClient Player in Kernel.GamePool.Values)
                {
                    if (Player.Entity.MapID == 4574 && (!Player.Entity.Dead))
                    {
                        alive2++;
                    }
                }
                if (alive2 == 1)
                {
                    foreach (Client.GameClient Player in Kernel.GamePool.Values)
                    {
                        if (Player.Entity.MapID == 4574)
                        {
                            if (!Player.Entity.Dead)//winner
                            {
                                Player.Entity.ConquerPoints += Room2Price * 2;
                                Player.Entity.WaitingTimeFB = Time32.Now;
                                Room2 = false;
                                Kernel.SendWorldMessage(new Network.GamePackets.Message(string.Concat(new object[] { "Congratulations! ", Player.Entity.Name, " has won ", Room2Price * 2, " CPs FB/SS in Room 2." }), System.Drawing.Color.Black, 0x7db), Program.GamePool);
                                Room2Price = 0;
                                _String str = new _String(true)
                                {
                                    UID = Player.Entity.UID,
                                    TextsCount = 1,
                                    Type = 10
                                };
                                str.Texts.Add("sports_victory");
                                Player.SendScreen(str, true);
                                Player.Entity.WinnerWaiting = Time32.Now;
                                Player.Entity.aWinner = true;
                            }
                            else//loser
                            {
                                Player.Entity.Teleport(1002, 439, 392);
                                _String str = new _String(true)
                                {
                                    UID = Player.Entity.UID,
                                    TextsCount = 1,
                                    Type = 10
                                };
                                str.Texts.Add("sports_failure");
                                Player.SendScreen(str, true);
                                Player.Entity.Action = Game.Enums.ConquerAction.None;
                                Player.ReviveStamp = Time32.Now;
                                Player.Attackable = false;

                                Player.Entity.TransformationID = 0;
                                Player.Entity.RemoveFlag(Update.Flags.Dead);
                                Player.Entity.RemoveFlag(Update.Flags.Ghost);
                                Player.Entity.Hitpoints = Player.Entity.MaxHitpoints;

                                Player.Entity.Ressurect();
                            }
                        }
                    }
                }
            }
            #endregion
            #region Room3
            if (Room3 == false)
            {
                int entered3 = 0;
                foreach (Client.GameClient Player in Kernel.GamePool.Values)
                {
                    if (Player.Entity.MapID == 4575 && (!Player.Entity.Dead))
                    {
                        entered3++;
                    }
                }
                if (entered3 > 1)
                {
                    Room3 = true;
                }
                else if (entered3 == 1)
                {
                    foreach (Client.GameClient Player in Kernel.GamePool.Values)
                    {
                        if (Player.Entity.MapID == 4575 && (!Player.Entity.Dead))
                        {
                            if (Time32.Now > Player.Entity.WaitingTimeFB.AddSeconds(20))
                            {
                                Player.Entity.ConquerPoints += Room3Price;
                                Room3Price = 0;
                                Player.Entity.Teleport(1002, 427, 378);
                            }
                        }
                    }
                }
            }
            else
            {
                int alive3 = 0;
                foreach (Client.GameClient Player in Kernel.GamePool.Values)
                {
                    if (Player.Entity.MapID == 4575 && (!Player.Entity.Dead))
                    {
                        alive3++;
                    }
                }
                if (alive3 == 1)
                {
                    foreach (Client.GameClient Player in Kernel.GamePool.Values)
                    {
                        if (Player.Entity.MapID == 4575)
                        {
                            if (!Player.Entity.Dead)//winner
                            {
                                Player.Entity.ConquerPoints += Room3Price * 2;
                                Player.Entity.WaitingTimeFB = Time32.Now;
                                Room3 = false;
                                Kernel.SendWorldMessage(new Network.GamePackets.Message(string.Concat(new object[] { "Congratulations! ", Player.Entity.Name, " has won ", Room3Price * 2, " CPs FB/SS in Room 3." }), System.Drawing.Color.Black, 0x7db), Program.GamePool);
                                Room3Price = 0;
                                _String str = new _String(true)
                                {
                                    UID = Player.Entity.UID,
                                    TextsCount = 1,
                                    Type = 10
                                };
                                str.Texts.Add("sports_victory");
                                Player.SendScreen(str, true);
                                Player.Entity.WinnerWaiting = Time32.Now;
                                Player.Entity.aWinner = true;
                            }
                            else//loser
                            {
                                Player.Entity.Teleport(1002, 439, 392);
                                _String str = new _String(true)
                                {
                                    UID = Player.Entity.UID,
                                    TextsCount = 1,
                                    Type = 10
                                };
                                str.Texts.Add("sports_failure");
                                Player.SendScreen(str, true);
                                Player.Entity.Action = Game.Enums.ConquerAction.None;
                                Player.ReviveStamp = Time32.Now;
                                Player.Attackable = false;

                                Player.Entity.TransformationID = 0;
                                Player.Entity.RemoveFlag(Update.Flags.Dead);
                                Player.Entity.RemoveFlag(Update.Flags.Ghost);
                                Player.Entity.Hitpoints = Player.Entity.MaxHitpoints;

                                Player.Entity.Ressurect();
                            }
                        }
                    }
                }
            }
            #endregion
            #region Room4
            if (Room4 == false)
            {
                int entered4 = 0;
                foreach (Client.GameClient Player in Kernel.GamePool.Values)
                {
                    if (Player.Entity.MapID == 4576 && (!Player.Entity.Dead))
                    {
                        entered4++;
                    }
                }
                if (entered4 > 1)
                {
                    Room4 = true;
                }
                else if (entered4 == 1)
                {
                    foreach (Client.GameClient Player in Kernel.GamePool.Values)
                    {
                        if (Player.Entity.MapID == 4576 && (!Player.Entity.Dead))
                        {
                            if (Time32.Now > Player.Entity.WaitingTimeFB.AddSeconds(20))
                            {
                                Player.Entity.ConquerPoints += Room4Price;
                                Room4Price = 0;
                                Player.Entity.Teleport(1002, 427, 378);
                            }
                        }
                    }
                }
            }
            else
            {
                int alive4 = 0;
                foreach (Client.GameClient Player in Kernel.GamePool.Values)
                {
                    if (Player.Entity.MapID == 4576 && (!Player.Entity.Dead))
                    {
                        alive4++;
                    }
                }
                if (alive4 == 1)
                {
                    foreach (Client.GameClient Player in Kernel.GamePool.Values)
                    {
                        if (Player.Entity.MapID == 4576)
                        {
                            if (!Player.Entity.Dead)//winner
                            {
                                Player.Entity.ConquerPoints += Room4Price * 2;
                                Player.Entity.WaitingTimeFB = Time32.Now;
                                Room4 = false;
                                Kernel.SendWorldMessage(new Network.GamePackets.Message(string.Concat(new object[] { "Congratulations! ", Player.Entity.Name, " has won ", Room4Price * 2, " CPs FB/SS in Room 4." }), System.Drawing.Color.Black, 0x7db), Program.GamePool);
                                Room4Price = 0;
                                _String str = new _String(true)
                                {
                                    UID = Player.Entity.UID,
                                    TextsCount = 1,
                                    Type = 10
                                };
                                str.Texts.Add("sports_victory");
                                Player.SendScreen(str, true);
                                Player.Entity.WinnerWaiting = Time32.Now;
                                Player.Entity.aWinner = true;
                            }
                            else//loser
                            {
                                Player.Entity.Teleport(1002, 439, 392);
                                _String str = new _String(true)
                                {
                                    UID = Player.Entity.UID,
                                    TextsCount = 1,
                                    Type = 10
                                };
                                str.Texts.Add("sports_failure");
                                Player.SendScreen(str, true);
                                Player.Entity.Action = Game.Enums.ConquerAction.None;
                                Player.ReviveStamp = Time32.Now;
                                Player.Attackable = false;

                                Player.Entity.TransformationID = 0;
                                Player.Entity.RemoveFlag(Update.Flags.Dead);
                                Player.Entity.RemoveFlag(Update.Flags.Ghost);
                                Player.Entity.Hitpoints = Player.Entity.MaxHitpoints;

                                Player.Entity.Ressurect();
                            }
                        }
                    }
                }
            }
            #endregion
            #region Room5
            if (Room5 == false)
            {
                int entered5 = 0;
                foreach (Client.GameClient Player in Kernel.GamePool.Values)
                {
                    if (Player.Entity.MapID == 4577 && (!Player.Entity.Dead))
                    {
                        entered5++;
                    }
                }
                if (entered5 > 1)
                {
                    Room5 = true;
                }
                else if (entered5 == 1)
                {
                    foreach (Client.GameClient Player in Kernel.GamePool.Values)
                    {
                        if (Player.Entity.MapID == 4577 && (!Player.Entity.Dead))
                        {
                            if (Time32.Now > Player.Entity.WaitingTimeFB.AddSeconds(20))
                            {
                                Player.Entity.ConquerPoints += Room5Price;
                                Room5Price = 0;
                                Player.Entity.Teleport(1002, 427, 378);
                            }
                        }
                    }
                }
            }
            else
            {
                int alive5 = 0;
                foreach (Client.GameClient Player in Kernel.GamePool.Values)
                {
                    if (Player.Entity.MapID == 4577 && (!Player.Entity.Dead))
                    {
                        alive5++;
                    }
                }
                if (alive5 == 1)
                {
                    foreach (Client.GameClient Player in Kernel.GamePool.Values)
                    {
                        if (Player.Entity.MapID == 4577)
                        {
                            if (!Player.Entity.Dead)//winner
                            {
                                Player.Entity.ConquerPoints += Room5Price * 2;
                                Player.Entity.WaitingTimeFB = Time32.Now;
                                Room5 = false;
                                Kernel.SendWorldMessage(new Network.GamePackets.Message(string.Concat(new object[] { "Congratulations! ", Player.Entity.Name, " has won ", Room5Price * 2, " CPs FB/SS in Room 5." }), System.Drawing.Color.Black, 0x7db), Program.GamePool);
                                Room5Price = 0;
                                _String str = new _String(true)
                                {
                                    UID = Player.Entity.UID,
                                    TextsCount = 1,
                                    Type = 10
                                };
                                str.Texts.Add("sports_victory");
                                Player.SendScreen(str, true);
                                Player.Entity.WinnerWaiting = Time32.Now;
                                Player.Entity.aWinner = true;
                            }
                            else//loser
                            {
                                Player.Entity.Teleport(1002, 439, 392);
                                _String str = new _String(true)
                                {
                                    UID = Player.Entity.UID,
                                    TextsCount = 1,
                                    Type = 10
                                };
                                str.Texts.Add("sports_failure");
                                Player.SendScreen(str, true);
                                Player.Entity.Action = Game.Enums.ConquerAction.None;
                                Player.ReviveStamp = Time32.Now;
                                Player.Attackable = false;

                                Player.Entity.TransformationID = 0;
                                Player.Entity.RemoveFlag(Update.Flags.Dead);
                                Player.Entity.RemoveFlag(Update.Flags.Ghost);
                                Player.Entity.Hitpoints = Player.Entity.MaxHitpoints;

                                Player.Entity.Ressurect();
                            }
                        }
                    }
                }
            }
            #endregion
            #region Room6
            if (Room6 == false)
            {
                int entered6 = 0;
                foreach (Client.GameClient Player in Kernel.GamePool.Values)
                {
                    if (Player.Entity.MapID == 4578 && (!Player.Entity.Dead))
                    {
                        entered6++;
                    }
                }
                if (entered6 > 1)
                {
                    Room6 = true;
                }
                else if (entered6 == 1)
                {
                    foreach (Client.GameClient Player in Kernel.GamePool.Values)
                    {
                        if (Player.Entity.MapID == 4578 && (!Player.Entity.Dead))
                        {
                            if (Time32.Now > Player.Entity.WaitingTimeFB.AddSeconds(20))
                            {
                                Player.Entity.ConquerPoints += Room6Price;
                                Room6Price = 0;
                                Player.Entity.Teleport(1002, 427, 378);
                            }
                        }
                    }
                }
            }
            else
            {
                int alive6 = 0;
                foreach (Client.GameClient Player in Kernel.GamePool.Values)
                {
                    if (Player.Entity.MapID == 4578 && (!Player.Entity.Dead))
                    {
                        alive6++;
                    }
                }
                if (alive6 == 1)
                {
                    foreach (Client.GameClient Player in Kernel.GamePool.Values)
                    {
                        if (Player.Entity.MapID == 4578)
                        {
                            if (!Player.Entity.Dead)//winner
                            {
                                Player.Entity.ConquerPoints += Room6Price * 2;
                                Player.Entity.WaitingTimeFB = Time32.Now;
                                Room6 = false;
                                Kernel.SendWorldMessage(new Network.GamePackets.Message(string.Concat(new object[] { "Congratulations! ", Player.Entity.Name, " has won ", Room6Price * 2, " CPs FB/SS in Room 6." }), System.Drawing.Color.Black, 0x7db), Program.GamePool);
                                Room6Price = 0;
                                _String str = new _String(true)
                                {
                                    UID = Player.Entity.UID,
                                    TextsCount = 1,
                                    Type = 10
                                };
                                str.Texts.Add("sports_victory");
                                Player.SendScreen(str, true);
                                Player.Entity.WinnerWaiting = Time32.Now;
                                Player.Entity.aWinner = true;
                            }
                            else//loser
                            {
                                Player.Entity.Teleport(1002, 439, 392);
                                _String str = new _String(true)
                                {
                                    UID = Player.Entity.UID,
                                    TextsCount = 1,
                                    Type = 10
                                };
                                str.Texts.Add("sports_failure");
                                Player.SendScreen(str, true);
                                Player.Entity.Action = Game.Enums.ConquerAction.None;
                                Player.ReviveStamp = Time32.Now;
                                Player.Attackable = false;

                                Player.Entity.TransformationID = 0;
                                Player.Entity.RemoveFlag(Update.Flags.Dead);
                                Player.Entity.RemoveFlag(Update.Flags.Ghost);
                                Player.Entity.Hitpoints = Player.Entity.MaxHitpoints;

                                Player.Entity.Ressurect();
                            }
                        }
                    }
                }
            }
            #endregion
            #endregion
        }
        public static void Execute<T>(Action<T, int> action, T param, int timeOut = 0, ThreadPriority priority = ThreadPriority.Normal)
        {
            GenericThreadPool.Subscribe<T>(new LazyDelegate<T>(action, timeOut, priority), param);
        }
        public static IDisposable Subscribe(Action<int> action, int period = 1, ThreadPriority priority = ThreadPriority.Normal)
        {
            return GenericThreadPool.Subscribe(new TimerRule(action, period, priority));
        }
        public static IDisposable Subscribe<T>(Action<T, int> action, T param, int timeOut = 0, ThreadPriority priority = ThreadPriority.Normal)
        {
            return GenericThreadPool.Subscribe<T>(new TimerRule<T>(action, timeOut, priority), param);
        }
        public static IDisposable Subscribe<T>(TimerRule<T> rule, T param, StandalonePool pool)
        {
            return pool.Subscribe<T>(rule, param);
        }
        public static IDisposable Subscribe<T>(TimerRule<T> rule, T param, StaticPool pool)
        {
            return pool.Subscribe<T>(rule, param);
        }
        public static IDisposable Subscribe<T>(TimerRule<T> rule, T param)
        {
            return GenericThreadPool.Subscribe<T>(rule, param);
        }
        #endregion
    }
}
