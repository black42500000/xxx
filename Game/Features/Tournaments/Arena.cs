﻿/*
 * The following code belongs to Ultimation and impulse.
 * Ultimation is credited for most of this piece of code.
 * Thanks to him, we have arena implented.
 * */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Conquer_Online_Server.Client;
using Conquer_Online_Server.Network.GamePackets;
using System.Threading;
using System.Globalization;
using System.Collections.Concurrent;
using Conquer_Online_Server.Game;
using System.Drawing;

namespace Conquer_Online_Server.Game
{
    public class Arena
    {
        public static ConcurrentDictionary<uint, GameClient> PlayerList = new ConcurrentDictionary<uint, GameClient>();
        public static ConcurrentDictionary<uint, GameClient> WaitingPlayerList = new ConcurrentDictionary<uint, GameClient>();

        public static ConcurrentDictionary<uint, ArenaStatistic> ArenaStatistics = new ConcurrentDictionary<uint, ArenaStatistic>();

        public static SafeDictionary<uint, ArenaStatistic> YesterdayArenaStatistics = new SafeDictionary<uint, ArenaStatistic>();

        public static List<ArenaStatistic> ArenaStatisticsList, YesterdayArenaStatisticsList;

        public static int InArenaCount = 0, HistoryArenaCount = 0;

        public static void Sort()
        {
            var where = ArenaStatistics.Values.Where((a) => a.TodayWin != 0 || a.TodayBattles != 0);
            List<ArenaStatistic> stats = where.OrderByDescending((p) => p.ArenaPoints).ToList();
            InArenaCount = stats.Count;
            for (uint i = 0; i < stats.Count; i++)
                stats[(int)i].Rank = i + 1;
            ArenaStatisticsList = stats;
        }
        public static void YesterdaySort()
        {
            var where = ArenaStatistics.Values.Where((a) => a.LastSeasonWin != 0 || a.LastSeasonLose != 0);
            List<ArenaStatistic> stats = where.OrderByDescending((p) => p.LastSeasonArenaPoints).ToList();
            HistoryArenaCount = stats.Count;
            for (uint i = 0; i < stats.Count; i++)
                stats[(int)i].LastSeasonRank = i + 1;
            YesterdayArenaStatisticsList = stats;
        }

        private static void SaveArenaStats()
        {
            var array = ArenaStatistics.Values.ToArray();
            foreach (ArenaStatistic stats in array)
                if (stats != null)
                    Database.ArenaTable.SaveArenaStatistics(stats);
        }

        public static class QualifierList
        {
            public static ConcurrentDictionary<uint, QualifierGroup> Groups = new ConcurrentDictionary<uint, QualifierGroup>();
            public static Counter GroupCounter = new Conquer_Online_Server.Counter();

            public static byte[] BuildPacket(ushort page)
            {
                MemoryStream strm = new MemoryStream();
                BinaryWriter wtr = new BinaryWriter(strm);
                wtr.Write((ushort)0);
                wtr.Write((ushort)2206);
                wtr.Write((uint)page);
                wtr.Write((uint)Game.Enums.ArenaIDs.QualifierList);
                wtr.Write((uint)Groups.Count);
                wtr.Write((uint)PlayerList.Count);
                wtr.Write((uint)0);
                page--;
                wtr.Write((uint)(Groups.Count - page));
                QualifierGroup[] GroupsList = Groups.Values.ToArray();
                for (int count = page; count < page + 6; count++)
                {
                    if (count >= Groups.Count)
                        break;

                    QualifierGroup entry = GroupsList[count];

                    wtr.Write((uint)entry.Player1.ArenaStatistic.EntityID);
                    wtr.Write((uint)entry.Player1.ArenaStatistic.Model);
                    byte[] array = Encoding.Default.GetBytes(entry.Player1.ArenaStatistic.Name);
                    for (int i = 0; i < 16; i++)
                    {
                        if (i < entry.Player1.ArenaStatistic.Name.Length)
                        {
                            wtr.Write(array[i]);
                        }
                        else
                            wtr.Write((byte)0);
                    }

                    wtr.Write((uint)entry.Player1.ArenaStatistic.Level);
                    wtr.Write((uint)entry.Player1.ArenaStatistic.Class);
                    wtr.Write((uint)entry.Player1.ArenaStatistic.Rank);
                    wtr.Write((uint)entry.Player1.ArenaPoints);
                    wtr.Write((uint)entry.Player1.ArenaStatistic.TodayWin);
                    wtr.Write((uint)(entry.Player1.ArenaStatistic.TodayBattles - entry.Player1.ArenaStatistic.TodayWin));
                    wtr.Write((uint)0);
                    wtr.Write((uint)entry.Player1.CurrentHonor);
                    wtr.Write((uint)entry.Player1.HistoryHonor);
                    wtr.Write((uint)entry.Player2.ArenaStatistic.EntityID);
                    wtr.Write((uint)entry.Player2.ArenaStatistic.Model);

                    byte[] array2 = Encoding.Default.GetBytes(entry.Player2.ArenaStatistic.Name);
                    for (int i = 0; i < 16; i++)
                    {
                        if (i < entry.Player2.ArenaStatistic.Name.Length)
                        {
                            wtr.Write(array2[i]);
                        }
                        else
                            wtr.Write((byte)0);
                    }

                    wtr.Write((uint)entry.Player2.ArenaStatistic.Level);
                    wtr.Write((uint)entry.Player2.ArenaStatistic.Class);
                    wtr.Write((uint)entry.Player2.ArenaStatistic.Rank);
                    wtr.Write((uint)entry.Player2.ArenaPoints);
                    wtr.Write((uint)entry.Player2.ArenaStatistic.TodayWin);
                    wtr.Write((uint)0);
                    wtr.Write((uint)(entry.Player1.ArenaStatistic.TodayBattles - entry.Player1.ArenaStatistic.TodayWin));
                    wtr.Write((uint)entry.Player2.CurrentHonor);
                    wtr.Write((uint)entry.Player2.HistoryHonor);
                }
                GroupsList = null;
                int packetlength = (int)strm.Length;
                strm.Position = 0;
                wtr.Write((ushort)packetlength);
                strm.Position = strm.Length;
                wtr.Write(Encoding.Default.GetBytes("TQServer"));
                strm.Position = 0;
                byte[] buf = new byte[strm.Length];
                strm.Read(buf, 0, buf.Length);
                wtr.Close();
                strm.Close();
                return buf;
            }

            public class QualifierGroup
            {
                #region Watchers
                public byte[] BuildWatcherList(List<Client.GameClient> list, ushort id)
                {
                    MemoryStream strm = new MemoryStream();
                    BinaryWriter wtr = new BinaryWriter(strm);
                    wtr.Write((ushort)38);
                    wtr.Write((ushort)2211);
                    wtr.Write((ushort)id);
                    wtr.Write((ulong)0);
                    wtr.Write((uint)list.Count);
                    wtr.Write((uint)Player1Cheers);
                    wtr.Write((uint)Player2Cheers);
                    foreach (Client.GameClient client in list)
                    {
                        wtr.Write((uint)client.Entity.Mesh);
                        for (int i = 0; i < 16; i++)
                        {
                            if (i < client.ArenaStatistic.Name.Length)
                            {
                                wtr.Write((byte)client.ArenaStatistic.Name[i]);
                            }
                            else
                                wtr.Write((byte)0);
                        }
                        wtr.Write((uint)client.Entity.UID);
                        wtr.Write((uint)client.ArenaStatistic.Level);
                        wtr.Write((uint)client.ArenaStatistic.Class);
                        wtr.Write((uint)client.ArenaStatistic.Rank);
                    }
                    int packetlength = (int)strm.Length;
                    strm.Position = 0;
                    wtr.Write((ushort)packetlength);
                    strm.Position = strm.Length;
                    wtr.Write(Encoding.Default.GetBytes("TQServer"));
                    strm.Position = 0;
                    byte[] buf = new byte[strm.Length];
                    strm.Read(buf, 0, buf.Length);
                    wtr.Close();
                    strm.Close();
                    return buf;
                }
                public void BeginWatching(Client.GameClient client)
                {
                    if (!client.Entity.Dead && client.WatchingGroup == null && client.QualifierGroup == null)
                    {
                        client.Send(BuildWatcherList(new List<GameClient>(), 0));
                        client.Send(this.match.BuildPacket());

                        Watchers.Add(client);
                        byte[] packet = BuildWatcherList(Watchers, 2);
                        foreach (Client.GameClient client2 in Watchers)
                            client2.Send(packet);
                        Player1.Send(packet);
                        Player2.Send(packet);
                        client.WatchingGroup = this;
                        client.Entity.Teleport(1005, dynamicMap.ID, (ushort)Kernel.Random.Next(35, 70), (ushort)Kernel.Random.Next(35, 70));
                    }
                }
                public List<Client.GameClient> Watchers = new List<GameClient>();
                #endregion

                public Time32 CreateTime;
                public Time32 DoneStamp;

                public uint Player1Damage, Player2Damage;
                public uint Player1Cheers, Player2Cheers;

                public List<uint> Cheerers = new List<uint>();

                public bool Done;

                private Game.Enums.PKMode P1Mode, P2Mode;

                public uint ID;

                public GroupMatch match = new GroupMatch();

                public Client.GameClient Winner, Loser;
                public Client.GameClient Player1, Player2;

                private Map dynamicMap;
                public Time32 ImportTime;
                public QualifierGroup(Client.GameClient player1, Client.GameClient player2)
                {
                    Player1 = player1;
                    Player2 = player2;
                    CreateTime = Time32.Now;
                    Player1Damage = 0;
                    Player2Damage = 0;
                    Done = false;
                    ID = GroupCounter.Next;
                    match.Group = this;
                    Done = false;
                    Groups.Add(ID, this);
                }

                public GameClient OppositeClient(GameClient client)
                {
                    if (client == Player1)
                        return Player2;
                    else
                        return Player1;
                }

                public void Import()
                {
                    Player1.QualifierGroup = this;
                    Player2.QualifierGroup = this;

                    if (!Kernel.Maps.ContainsKey(700))
                        new Map(700, Database.DMaps.MapPaths[700]);
                    Map origMap = Kernel.Maps[700];
                    dynamicMap = origMap.MakeDynamicMap();
                    Player1.Entity.Teleport(origMap.ID, dynamicMap.ID, (ushort)Kernel.Random.Next(35, 70), (ushort)Kernel.Random.Next(35, 70));
                    Player2.Entity.Teleport(origMap.ID, dynamicMap.ID, (ushort)Kernel.Random.Next(35, 70), (ushort)Kernel.Random.Next(35, 70));
                    ImportTime = Time32.Now;
                    if (Player1.Map.ID == Player2.Map.ID)
                    {
                        ArenaSignup sign = new ArenaSignup();
                        sign.DialogID = ArenaSignup.MainIDs.StartTheFight;
                        sign.Stats = Player1.ArenaStatistic;
                        Player2.Send(sign.BuildPacket());
                        sign.Stats = Player2.ArenaStatistic;
                        Player1.Send(sign.BuildPacket());
                        sign.DialogID = ArenaSignup.MainIDs.Match;
                        sign.OptionID = ArenaSignup.DialogButton.MatchOn;
                        Player1.Send(sign.BuildPacket());
                        Player2.Send(sign.BuildPacket());
                        Player1.Send(match.BuildPacket());
                        Player2.Send(match.BuildPacket());
                        Player1.Entity.BringToLife();
                        Player2.Entity.BringToLife();
                        Player1.Entity.RemoveFlag(Network.GamePackets.Update.Flags.Ride);
                        Player2.Entity.RemoveFlag(Network.GamePackets.Update.Flags.Ride);
                        P1Mode = Player1.Entity.PKMode;
                        Player1.Entity.PKMode = Conquer_Online_Server.Game.Enums.PKMode.PK;
                        Player1.Send(new Data(true) { UID = Player1.Entity.UID, ID = Data.ChangePKMode, dwParam = (uint)Player1.Entity.PKMode });
                        P2Mode = Player2.Entity.PKMode;
                        Player2.Entity.PKMode = Conquer_Online_Server.Game.Enums.PKMode.PK;
                        Player2.Send(new Data(true) { UID = Player2.Entity.UID, ID = Data.ChangePKMode, dwParam = (uint)Player2.Entity.PKMode });
                    }
                    else
                        End();
                }

                public void Export()
                {
                    Groups.Remove(ID);

                    if (dynamicMap != null)
                        dynamicMap.Dispose();

                    var arr = Watchers.ToArray();
                    foreach (Client.GameClient client in arr)
                        QualifyEngine.DoLeave(client);
                    arr = null;

                    Player1.Entity.PreviousTeleport();  
                    if (Player1.Map.BaseID == 700 && Player1.Entity.MapID != 700)
                        Player1.Entity.Teleport(1002, 400, 400);
                    Player2.Entity.PreviousTeleport();
                    if (Player2.Map.BaseID == 700 && Player2.Entity.MapID != 700)
                        Player2.Entity.Teleport(1002, 400, 400);
                    Loser.Entity.Ressurect();
                    Winner.Entity.Ressurect();

                    Player1.Entity.PKMode = P1Mode;
                    Player1.Send(new Data(true) { UID = Player1.Entity.UID, ID = Data.ChangePKMode, dwParam = (uint)Player1.Entity.PKMode });
                    Player2.Entity.PKMode = P2Mode;
                    Player2.Send(new Data(true) { UID = Player2.Entity.UID, ID = Data.ChangePKMode, dwParam = (uint)Player2.Entity.PKMode });
                    Player1.QualifierGroup = null;
                    Player2.QualifierGroup = null;

                }

                public void End()
                {
                    if (Done) return;
                    if (Player1Damage > Player2Damage)
                    {
                        Winner = Player1;
                        Loser = Player2;
                    }
                    else
                    {
                        Winner = Player2;
                        Loser = Player1;
                    }

                    var sign = new ArenaSignup();
                    sign.Stats = Loser.ArenaStatistic;
                    sign.DialogID = ArenaSignup.MainIDs.Dialog;
                    sign.OptionID = ArenaSignup.DialogButton.Lose;
                    Loser.Send(sign.BuildPacket());
                    sign.OptionID = ArenaSignup.DialogButton.Win;
                    sign.Stats = Winner.ArenaStatistic;
                    Winner.Send(sign.BuildPacket());

                    Done = true;
                    DoneStamp = Time32.Now;
                }

                public void End(GameClient loser)
                {
                    if (Done) return;
                    if (Player1.Account.EntityID == loser.Account.EntityID)
                    {
                        Winner = Player2;
                        Loser = Player1;
                    }
                    else
                    {
                        Winner = Player1;
                        Loser = Player2;
                    }

                    var sign = new ArenaSignup();
                    sign.Stats = Loser.ArenaStatistic;
                    sign.DialogID = ArenaSignup.MainIDs.Dialog;
                    sign.OptionID = ArenaSignup.DialogButton.Lose;
                    Loser.Send(sign.BuildPacket());
                    sign.OptionID = ArenaSignup.DialogButton.Win;
                    sign.Stats = Winner.ArenaStatistic;
                    Winner.Send(sign.BuildPacket());

                    Done = true;
                    DoneStamp = Time32.Now;
                }

                public void UpdateDamage(GameClient client, uint damage)
                {
                    if (client != null && Player1 != null)
                    {
                        if (client.Account.EntityID == Player1.Account.EntityID)
                        {
                            Player1Damage += damage;
                        }
                        else
                        {
                            Player2Damage += damage;
                        }
                        Player1.Send(match.BuildPacket());
                        Player2.Send(match.BuildPacket());
                    }
                }
            }
        }

        public class QualifyEngine
        {
            public static void RequestGroupList(Client.GameClient client, ushort page)
            {
                client.Send(QualifierList.BuildPacket(page));
            }

            public static void DoQuit(Client.GameClient client)
            {
                PlayerList.Remove(client.Account.EntityID);
                RequestGroupList(client, 1);

                if (client.QualifierGroup != null)
                {
                    client.QualifierGroup.End(client);
                    client.QualifierGroup.CreateTime = Time32.Now.AddSeconds(-100);
                }
                else
                {
                    ArenaSignup sign = new ArenaSignup();
                    sign.DialogID = ArenaSignup.MainIDs.OpponentGaveUp;
                    if (client.ArenaStatistic.PlayWith != null)
                    {
                        Client.GameClient other = client.ArenaStatistic.PlayWith;
                        other.Send(sign.BuildPacket());
                        Win(other, client);
                    }
                    Clear(client);
                }
            }

            public static void DoSignup(Client.GameClient client)
            {
                if (client.ArenaStatistic.Status != ArenaStatistic.NotSignedUp || client.TeamArenaStatistic.Status != TeamArenaStatistic.NotSignedUp)
                {
                    client.Send(new Message("You already joined a qualifier arena! Quit the other one and sign up again.", Color.Beige, Message.Agate));
                    return;
                }
                if (client.InQualifier())
                {
                    return;
                }
                if (client.ArenaStatistic.ArenaPoints == 0)
                    return;
                if (client.Entity.MapID == 601) return;
                if (client.Map.BaseID >= 6000 && client.Map.BaseID <= 6003) return;

                if (WaitingPlayerList.ContainsKey(client.Account.EntityID))
                {
                    if (client.QualifierGroup == null)
                        WaitingPlayerList.Remove(client.Account.EntityID);
                    else
                        return;
                }
                PlayerList.Add(client.Account.EntityID, client);
                client.ArenaStatistic.Status = Network.GamePackets.ArenaStatistic.WaitingForOpponent;
                client.ArenaState = FindMatch;
                client.Send(client.ArenaStatistic);
                RequestGroupList(client, 1);
            }
            public static void DoGiveUp(Client.GameClient client)
            {
                if (client.ArenaState == WaitForBox)
                {
                    client.ArenaStatistic.AcceptBox = false;
                    client.ArenaState = WaitForOther;
                }
                else
                {
                    client.ArenaStatistic.Status = Network.GamePackets.ArenaStatistic.WaitingInactive;
                    client.Send(client.ArenaStatistic);
                    RequestGroupList(client, 1);
                    ArenaSignup sign = new ArenaSignup();
                    sign.DialogID = ArenaSignup.MainIDs.OpponentGaveUp;

                    if (client.ArenaStatistic.PlayWith != null)
                    {
                        client.ArenaStatistic.PlayWith.Send(sign.BuildPacket());

                        client.ArenaStatistic.PlayWith.ArenaStatistic.Status = Network.GamePackets.ArenaStatistic.NotSignedUp;
                        client.ArenaStatistic.Status = Network.GamePackets.ArenaStatistic.NotSignedUp;
                        client.ArenaStatistic.PlayWith.Send(client.ArenaStatistic.PlayWith.ArenaStatistic);
                        client.Send(client.ArenaStatistic);

                        if (WaitingPlayerList.ContainsKey(client.Account.EntityID))
                        {
                            WaitingPlayerList.Remove(client.Account.EntityID);
                            WaitingPlayerList.Remove(client.ArenaStatistic.PlayWith.Account.EntityID);
                        }
                        if (client.QualifierGroup != null)
                        {
                            if (!client.QualifierGroup.Done)
                            {
                                client.QualifierGroup.End(client);
                            }
                            else
                            {
                                Win(client.ArenaStatistic.PlayWith, client);
                            }
                        }
                        else
                        {
                            Win(client.ArenaStatistic.PlayWith, client);
                        }
                    }
                }
            }

            public static void DoAccept(Client.GameClient client)
            {
                if (client.ArenaState == WaitForBox)
                {
                    client.ArenaStatistic.AcceptBox = true;
                    client.ArenaState = WaitForOther;
                }
            }

            public static void DoLeave(Client.GameClient client)
            {
                if (client.WatchingGroup != null)
                {
                    client.WatchingGroup.Watchers.Remove(client);
                    client.Send(client.WatchingGroup.BuildWatcherList(client.WatchingGroup.Watchers, 3));
                    byte[] packet = client.WatchingGroup.BuildWatcherList(client.WatchingGroup.Watchers, 2);
                    foreach (Client.GameClient client2 in client.WatchingGroup.Watchers)
                        client2.Send(packet);
                    byte[] p2 = client.WatchingGroup.BuildWatcherList(new List<Client.GameClient>(), 3);
                    client.Send(p2);
                    client.WatchingGroup.Player1.Send(packet);
                    client.WatchingGroup.Player2.Send(packet);
                    client.WatchingGroup = null;
                    client.Entity.PreviousTeleport();
                }
            }

            public static void DoCheer(Client.GameClient client, uint uid)
            {
                if (client.WatchingGroup != null && !client.WatchingGroup.Cheerers.Contains(client.Entity.UID))
                {
                    client.WatchingGroup.Cheerers.Add(client.Entity.UID);
                    if (client.WatchingGroup.Player1.Entity.UID == uid)
                        client.WatchingGroup.Player1Cheers++;
                    else
                        client.WatchingGroup.Player2Cheers++;
                    byte[] packet = client.WatchingGroup.BuildWatcherList(client.WatchingGroup.Watchers, 2);
                    foreach (Client.GameClient client2 in client.WatchingGroup.Watchers)
                        client2.Send(packet);
                    client.WatchingGroup.Player1.Send(packet);
                    client.WatchingGroup.Player2.Send(packet);
                }
            }
        }

        public class Statistics
        {
            public static void ShowWiners(Client.GameClient client)
            {
                MemoryStream strm = new MemoryStream();
                BinaryWriter wtr = new BinaryWriter(strm);
                int MyCount = 0;
                wtr.Write((ushort)0);
                wtr.Write((ushort)2208);
                wtr.Write((uint)Game.Enums.ArenaIDs.ShowPlayerRankList);
                foreach (ArenaStatistic entry in YesterdayArenaStatisticsList)
                {
                    MyCount++;
                    wtr.Write((uint)entry.EntityID);
                    byte[] array = Encoding.Default.GetBytes(entry.Name);
                    for (int i = 0; i < 16; i++)
                    {
                        if (i < entry.Name.Length)
                            wtr.Write(array[i]);
                        else
                            wtr.Write((byte)0);
                    }
                    wtr.Write((uint)entry.Model);
                    wtr.Write((uint)entry.Level);
                    wtr.Write((uint)entry.Class);
                    wtr.Write((uint)entry.LastSeasonRank);
                    wtr.Write((uint)entry.LastSeasonRank);
                    wtr.Write((uint)entry.LastSeasonArenaPoints);
                    wtr.Write((uint)entry.LastSeasonWin);
                    wtr.Write((uint)entry.LastSeasonLose);
                    if (MyCount == 11)
                        break;
                }
                int packetlength = (int)strm.Length;
                strm.Position = 0;
                wtr.Write((ushort)packetlength);
                strm.Position = strm.Length;
                wtr.Write(Encoding.Default.GetBytes("TQServer"));
                strm.Position = 0;
                byte[] buf = new byte[strm.Length];
                strm.Read(buf, 0, buf.Length);
                wtr.Close();
                strm.Close();
                client.Send(buf);
            }

            public static void ShowRankingPage(ushort thisSeason, int pageIndex, Client.GameClient client)
            {
                ArenaList list = new ArenaList(pageIndex);
                list.ID = Game.Enums.ArenaIDs.ShowPlayerRankList;
                list.PageNumber = (ushort)pageIndex;
                list.Subtype = thisSeason;
                if (list.Subtype == 0)
                {
                    var Array = ArenaStatisticsList;
                    if (Array.Count > (((pageIndex) * 10) - 10))
                    {
                        list.Players.Clear();
                        for (int i = ((pageIndex) * 10 - 10); i < ((pageIndex) * 10 - 10) + 10; i++)
                        {
                            if (i < Array.Count)
                            {
                                if (Array[i].Rank > 0)
                                {
                                    list.Players.Add(Array[i]);
                                }
                            }
                        }
                    }
                }
                else
                {
                    var Array = YesterdayArenaStatisticsList;
                    if (Array.Count > (((pageIndex) * 10) - 10))
                    {
                        list.Players.Clear();
                        for (int i = ((pageIndex) * 10 - 10); i < ((pageIndex) * 10 - 10) + 10; i++)
                        {
                            if (i < Array.Count)
                            {
                                if (Array[i].LastSeasonRank > 0)
                                {
                                    list.Players.Add(Array[i]);
                                }
                            }
                        }
                    }
                }
                client.Send(list.BuildPacket());
            }
        }

        public class ArenaList
        {
            public ushort Size;
            public ushort Type;
            public ushort Subtype;
            public ushort PageNumber;
            public Game.Enums.ArenaIDs ID;
            public List<ArenaStatistic> Players = new List<ArenaStatistic>();
            public ArenaList(int PageIndex, ushort type = 2207)
            {
                Type = type;
                PageNumber = (ushort)PageIndex;
            }
            public ArenaList(byte[] Packet)
            {
                BinaryReader rdr = new BinaryReader(new MemoryStream(Packet));
                Size = rdr.ReadUInt16();
                Type = rdr.ReadUInt16();
                Subtype = rdr.ReadUInt16();
                PageNumber = rdr.ReadUInt16();
            }
            public ArenaList()
            {
                Type = 2207;
            }
            public byte[] BuildPacket()
            {
                ushort mRank = (ushort)(PageNumber * 10 - 10);
                MemoryStream strm = new MemoryStream();
                BinaryWriter wtr = new BinaryWriter(strm);

                wtr.Write((ushort)0);
                wtr.Write((ushort)Type);
                wtr.Write((ushort)Subtype);
                wtr.Write((ushort)PageNumber);
                if (Subtype == 0)
                    wtr.Write((uint)InArenaCount);
                else
                    wtr.Write((uint)HistoryArenaCount);
                wtr.Write((uint)ID);
                foreach (ArenaStatistic entry in Players)
                {
                    if (Subtype == 0)
                        wtr.Write((ushort)entry.Rank);
                    else
                        wtr.Write((ushort)entry.LastSeasonRank);

                    for (int i = 0; i < 16; i++)
                    {
                        if (i < entry.Name.Length)
                            wtr.Write((byte)entry.Name[i]);
                        else
                            wtr.Write((byte)0);
                    }
                    if (Subtype == 1)
                        wtr.Write((ushort)6004);
                    else
                        wtr.Write((ushort)0);
                    if (Subtype == 1)
                        wtr.Write((uint)entry.CurrentHonor);
                    else
                        wtr.Write((uint)entry.ArenaPoints);

                    wtr.Write((uint)entry.Class);
                    wtr.Write((uint)entry.Level);

                    if (Subtype == 1)
                        wtr.Write((uint)33942209);
                    else
                        wtr.Write((uint)1);
                }
                int packetlength = (int)strm.Length;
                strm.Position = 0;
                wtr.Write((ushort)packetlength);
                strm.Position = strm.Length;
                wtr.Write(Encoding.Default.GetBytes("TQServer"));
                strm.Position = 0;
                byte[] buf = new byte[strm.Length];
                strm.Read(buf, 0, buf.Length);
                wtr.Close();
                strm.Close();
                return buf;
            }
        }

        public class ArenaSignup
        {
            public abstract class MainIDs
            {
                public const uint ArenaIconOn = 0,
                                    ArenaIconOff = 1,
                                    StartCountDown = 2,
                                    OpponentGaveUp = 4,
                                    Match = 6,
                                    YouAreKicked = 7,
                                    StartTheFight = 8,
                                    Dialog = 9,
                                    Dialog2 = 10;
            }
            public abstract class DialogButton
            {
                public const uint Lose = 3,
                                    Win = 1,
                                    MatchOff = 3,
                                    MatchOn = 5;
            }


            public ushort Type = 2205;
            public uint DialogID;
            public uint OptionID;
            public ArenaStatistic Stats;
            public byte[] BuildPacket()
            {
                MemoryStream strm = new MemoryStream();
                BinaryWriter wtr = new BinaryWriter(strm);
                if (Stats == null)
                {
                    Stats = new ArenaStatistic(true);
                    Stats.Name = "";
                }
                wtr.Write((ushort)0);
                wtr.Write((ushort)Type);
                wtr.Write((uint)DialogID);
                wtr.Write((uint)OptionID);
                wtr.Write((uint)Stats.EntityID);
                byte[] array = Encoding.Default.GetBytes(Stats.Name);
                for (int i = 0; i < 20; i++)
                {
                    if (i < Stats.Name.Length)
                    {
                        wtr.Write(array[i]);
                    }
                    else
                        wtr.Write((byte)0);
                }
                wtr.Write((uint)Stats.Class);
                wtr.Write((uint)Stats.Rank);
                wtr.Write((uint)Stats.ArenaPoints);
                wtr.Write((uint)Stats.Level);
                int packetlength = (int)strm.Length;
                strm.Position = 0;
                wtr.Write((ushort)packetlength);
                strm.Position = strm.Length;
                wtr.Write(Encoding.Default.GetBytes("TQServer"));
                strm.Position = 0;
                byte[] buf = new byte[strm.Length];
                strm.Read(buf, 0, buf.Length);
                wtr.Close();
                strm.Close();
                return buf;
            }
        }

        public class GroupMatch
        {
            public ushort Type = 2210;
            public QualifierList.QualifierGroup Group;
            public byte[] BuildPacket()
            {
                MemoryStream strm = new MemoryStream();
                BinaryWriter wtr = new BinaryWriter(strm);

                wtr.Write((ushort)56);
                wtr.Write((ushort)Type);
                wtr.Write((uint)Group.Player1.ArenaStatistic.EntityID);
                byte[] array = Encoding.Default.GetBytes(Group.Player1.ArenaStatistic.Name);
                for (int i = 0; i < 16; i++)
                {
                    if (i < Group.Player1.ArenaStatistic.Name.Length)
                    {
                        wtr.Write(array[i]);
                    }
                    else
                        wtr.Write((byte)0);
                }
                wtr.Write((uint)Group.Player1Damage);
                wtr.Write((uint)Group.Player2.ArenaStatistic.EntityID);
                byte[] array2 = Encoding.Default.GetBytes(Group.Player2.ArenaStatistic.Name);
                for (int i = 0; i < 16; i++)
                {
                    if (i < Group.Player2.ArenaStatistic.Name.Length)
                    {
                        wtr.Write(array2[i]);
                    }
                    else
                        wtr.Write((byte)0);
                }
                wtr.Write((uint)Group.Player2Damage);
                wtr.Write((uint)1);
                wtr.Write(Encoding.Default.GetBytes("TQServer"));
                strm.Position = 0;
                byte[] buf = new byte[strm.Length];
                strm.Read(buf, 0, buf.Length);
                wtr.Close();
                strm.Close();
                return buf;
            }
        }

        public static void Win(Client.GameClient winner, Client.GameClient loser)
        {
            if (winner.ArenaStatistic.PlayWith != null && loser.ArenaStatistic.PlayWith != null)
            {
                int diff = Kernel.Random.Next(30, 50);

                winner.ArenaStatistic.PlayWith = null;
                loser.ArenaStatistic.PlayWith = null;

                WaitingPlayerList.Remove(loser.Account.EntityID);
                WaitingPlayerList.Remove(winner.Account.EntityID);

                winner.ArenaStatistic.Status = Network.GamePackets.ArenaStatistic.NotSignedUp;
                winner.Send(winner.ArenaStatistic);
                winner.ArenaState = FindMatch;
                winner.QualifierGroup = null;
                loser.ArenaStatistic.Status = Network.GamePackets.ArenaStatistic.NotSignedUp;
                loser.Send(winner.ArenaStatistic);
                loser.ArenaState = FindMatch;
                loser.QualifierGroup = null;

                winner.ArenaPoints += (uint)diff;
                loser.ArenaPoints -= (uint)diff;
                if (loser.ArenaPoints > 80000)
                    loser.ArenaPoints = 0;

                winner.ArenaStatistic.TodayWin++;
                winner.ArenaStatistic.TotalWin++;
                loser.ArenaStatistic.TodayBattles++;
                loser.ArenaStatistic.TotalLose++;
                if (winner.ArenaStatistic.TodayWin == 9)
                {
                    winner.Inventory.Add(723912, 0, 1);
                    winner.ArenaStatistic.HistoryHonor += 5000;
                    winner.ArenaStatistic.CurrentHonor += 5000;
                    winner.Send(new Network.GamePackets.Message("You won 9 times in the Qualifier today and received an EXP Ball and 500 Honor Points", System.Drawing.Color.Red, Message.System));
                }
                if (winner.ArenaStatistic.TodayBattles == 20)
                {
                    winner.Inventory.Add(723912, 0, 1);
                    winner.ArenaStatistic.HistoryHonor += 5000;
                    winner.ArenaStatistic.CurrentHonor += 5000;
                    winner.Send(new Network.GamePackets.Message("You have enlisted in the Qualifier for 20 times today and received an EXP Ball and 500 Honor Points", System.Drawing.Color.Red, Message.System));
                }
                if (loser.ArenaStatistic.TodayBattles == 20)
                {
                    loser.Inventory.Add(723912, 0, 1);
                    winner.ArenaStatistic.CurrentHonor += 5000;
                    winner.Send(new Network.GamePackets.Message("You have enlisted in the Qualifier for 20 times today and received an EXP Ball and 500 Honor Points", System.Drawing.Color.Red, Message.System));
                }
                Sort();

                winner.Send(winner.ArenaStatistic);
                loser.Send(loser.ArenaStatistic);

                StringBuilder builder = new StringBuilder();
                if (winner.Entity.GuildID != 0)
                {
                    builder.Append("(");
                    builder.Append(winner.Guild.Name);
                    builder.Append(") ");
                }
                builder.Append(winner.Entity.Name);
                builder.Append(" has defeated ");

                if (loser.Entity.GuildID != 0)
                {
                    builder.Append("(");
                    builder.Append(loser.Guild.Name);
                    builder.Append(") ");
                }
                builder.Append(loser.Entity.Name);
                if (winner.ArenaStatistic.Rank > 0)
                {
                    builder.Append(" in the Qualifier, and is currently ranked No. "); builder.Append(winner.ArenaStatistic.Rank);
                }
                else { builder.Append(" in the Qualifier"); }

                builder.Append(".");

                Kernel.SendWorldMessage(new Message(builder.ToString(), System.Drawing.Color.Red, Message.ArenaQualifier), Program.GamePool);

                winner.QualifierGroup = null;
                loser.QualifierGroup = null;

                ArenaSignup sign = new ArenaSignup();
                sign.Stats = loser.ArenaStatistic;
                sign.DialogID = ArenaSignup.MainIDs.Dialog2;
                loser.Send(sign.BuildPacket());
                sign.Stats = winner.ArenaStatistic;
                sign.OptionID = ArenaSignup.DialogButton.Win;
                winner.Send(sign.BuildPacket());

                Clear(loser);
                loser.Send(loser.ArenaStatistic);

                Clear(winner);
                winner.Send(winner.ArenaStatistic);
                _String str = new _String(true);
                str.UID = winner.Entity.UID;
                str.TextsCount = 1;
                str.Type = _String.Effect;
                str.Texts.Add("sports_victory");
                winner.SendScreen(str, true);

                _String strs = new _String(true);
                strs.UID = loser.Entity.UID;
                strs.TextsCount = 1;
                strs.Type = _String.Effect;
                strs.Texts.Add("sports_failure");
                loser.SendScreen(strs, true);
            }
        }

        private static DateTime YesterdaySorted = DateTime.Now;

        public static void Clear(Client.GameClient client)
        {
            var arena_stat = client.ArenaStatistic;
            arena_stat.AcceptBox = false;
            arena_stat.AcceptBoxShow = Time32.Now;
            arena_stat.PlayWith = null;
            arena_stat.Status = ArenaStatistic.NotSignedUp;
            client.Send(arena_stat);
            client.ArenaState = Arena.None;
            client.QualifierGroup = null;
            client.TeamQualifierGroup = null;
            Arena.WaitingPlayerList.Remove(arena_stat.EntityID);
            Arena.PlayerList.Remove(arena_stat.EntityID);
        }

        public const int
            None = 0,
            FindMatch = 1,
            WaitForBox = 2,
            WaitForOther = 3,
            Fight = 4;

        public static void EngagePlayers()
        {
            if (PlayerList.Count < 2)
                return;
            var Players = new GameClient[PlayerList.Count];
            PlayerList.Values.CopyTo(Players, 0);
            int i, j;
            int iPlus, jPlus;
            int iEnd, jEnd;
            iPlus = Kernel.Random.Next(2);
            jPlus = Kernel.Random.Next(2);
            if (iPlus == 0) { i = 0; iPlus = 1; iEnd = Players.Length; } else { i = Players.Length - 1; iPlus = -1; iEnd = -1; }
            if (jPlus == 0) { j = 0; jPlus = 1; jEnd = Players.Length; } else { j = Players.Length - 1; jPlus = -1; jEnd = -1; }
            Time32 Now = Time32.Now;
            for (; i != iEnd; i += iPlus)
            {
                var Challanger = Players[i];

                if (CanJoin(Challanger, Now) && Challanger.ArenaState == FindMatch)
                {
                    for (; j != jEnd; j += jPlus)
                    {
                        var Challanged = Players[j];
                        if (CanJoin(Challanged, Now) && Challanged.ArenaState == FindMatch)
                        {
                            if (Challanger.Entity.UID != Challanged.Entity.UID)
                            {
                                Challanger.ArenaState = Challanged.ArenaState = WaitForBox;
                                Challanged.ArenaStatistic.AcceptBoxShow = Challanger.ArenaStatistic.AcceptBoxShow = Time32.Now;
                                Challanged.ArenaStatistic.Status = Network.GamePackets.ArenaStatistic.WaitingInactive;
                                Challanger.ArenaStatistic.Status = Network.GamePackets.ArenaStatistic.WaitingInactive;
                                Challanged.Send(Challanged.ArenaStatistic);
                                Challanger.Send(Challanger.ArenaStatistic);
                                Challanged.ArenaStatistic.PlayWith = Challanger;
                                Challanger.ArenaStatistic.PlayWith = Challanged;
                                ArenaSignup sign = new ArenaSignup();
                                sign.DialogID = ArenaSignup.MainIDs.StartCountDown;
                                sign.Stats = Challanged.ArenaStatistic;
                                Challanged.Send(sign.BuildPacket());
                                sign.Stats = Challanger.ArenaStatistic;
                                Challanger.Send(sign.BuildPacket());
                                PlayerList.Remove(Challanged.Account.EntityID);
                                PlayerList.Remove(Challanger.Account.EntityID);
                                WaitingPlayerList.Add(Challanged.Account.EntityID, Challanged);
                                WaitingPlayerList.Add(Challanger.Account.EntityID, Challanger);
                                break;
                            }
                        }
                    }
                }
            }
        }
        public static void VerifyAwaitingPeople()
        {
            foreach (var Challanger in WaitingPlayerList.Values)
            {
                if (Challanger != null)
                {
                    if (Challanger.ArenaStatistic == null)
                    {
                        WaitingPlayerList.Remove(Challanger.Entity.UID);
                        continue;
                    }
                    if (Challanger.ArenaStatistic.PlayWith != null)
                    {
                        Client.GameClient Challanged = Challanger.ArenaStatistic.PlayWith;
                        if (Challanger.ArenaState == WaitForBox || Challanged.ArenaState == WaitForBox)
                        {
                            if (Time32.Now > Challanger.ArenaStatistic.AcceptBoxShow.AddSeconds(60))
                            {
                                if (Challanger.ArenaState == WaitForBox)
                                {
                                    Win(Challanged, Challanger);
                                }
                                else
                                {
                                    Win(Challanger, Challanged);
                                }
                                return;
                            }
                        }
                        if (Challanger.ArenaState == WaitForOther && !Challanger.ArenaStatistic.AcceptBox)
                        {
                            Win(Challanged, Challanger);
                        }
                        else if (Challanged.ArenaState == WaitForOther && !Challanged.ArenaStatistic.AcceptBox)
                        {
                            Win(Challanger, Challanged);
                        }
                        else if (Challanger.ArenaState == WaitForOther && Challanged.ArenaState == WaitForOther)
                        {
                            if (!Challanger.ArenaStatistic.AcceptBox || !Challanged.ArenaStatistic.AcceptBox)
                            {
                                if (!Challanger.ArenaStatistic.AcceptBox)
                                {
                                    Win(Challanged, Challanger);
                                }
                                else
                                {
                                    Win(Challanger, Challanged);
                                }
                            }
                            else
                            {
                                if (Challanged.Entity.MapID == 1081 || Challanged.Entity.ContainsFlag2(Update.Flags2.SoulShackle))
                                {
                                    Arena.QualifyEngine.DoQuit(Challanged);
                                    continue;
                                }
                                if (Challanger.Entity.MapID == 1081 || Challanger.Entity.ContainsFlag2(Update.Flags2.SoulShackle))
                                {
                                    Arena.QualifyEngine.DoQuit(Challanger);
                                    continue;
                                }
                                Challanger.ArenaState = Challanged.ArenaState = Fight;
                                QualifierList.QualifierGroup group = new QualifierList.QualifierGroup(Challanger, Challanged);
                                group.Import();
                            }
                        }
                    }
                }
            }
        }
        public static void CheckGroups()
        {
            if (QualifierList.Groups.Count > 0)
            {
                foreach (var group in QualifierList.Groups.Values)
                {
                    if (Time32.Now > group.CreateTime.AddSeconds(5))
                    {
                        if (!group.Done)
                        {
                            if (Time32.Now > group.CreateTime.AddMinutes(3))
                            {
                                group.End();
                            }
                        }
                        else
                        {
                            if (Time32.Now > group.DoneStamp.AddSeconds(1))
                            {
                                group.DoneStamp = Time32.Now.AddDays(1);
                                group.Export();
                                Win(group.Winner, group.Loser);
                            }
                        }
                    }
                }
            }
        }

        private static bool DayPassed = false;
        private static DateTime StartDateTime = DateTime.Now;
        public static void Reset()
        {
            if (!DayPassed)
            {
                if (DateTime.Now.DayOfYear != StartDateTime.DayOfYear)
                {
                    DayPassed = true;
                    var array = ArenaStatistics.Values.ToArray();
                    foreach (ArenaStatistic stat in array)
                    {
                        try
                        {
                            if (Kernel.GamePool.ContainsKey(stat.EntityID))
                            {
                                Client.GameClient client = Kernel.GamePool[stat.EntityID];
                                Database.ArenaTable.Reset(client, stat);
                                client.Send(stat);
                            }
                            else
                                Database.ArenaTable.Reset(null, stat);
                        }
                        catch { }
                    }
                    try { SaveArenaStats(); }
                    catch { }
                    YesterdaySort();
                }
            }
            else
            {
                if (DateTime.Now.DayOfYear == StartDateTime.DayOfYear)
                {
                    DayPassed = false;
                    StartDateTime = DateTime.Now;
                }
            }
        }

        private static bool CanJoin(Client.GameClient client, Time32 now)
        {
            if (client != null)
            {
                if (client.Entity.MapID == 1081) return false;
                if (client.ArenaStatistic.PlayWith == null)
                {
                    if (client.QualifierGroup == null)
                    {
                        if (client.Map.BaseID == 1038) return false;
                        if (client.Map.BaseID == 700) return false;
                        if (client.Entity.MapID >= 1090 && client.Entity.MapID <= 1094) return false;
                        if (client.Entity.MapID >= 1505 && client.Entity.MapID <= 1509) return false;
                        if (client.Entity.ContainsFlag2(Update.Flags2.SoulShackle)) return false;
                        if (client.Map.ID == 1002 || client.Map.ID == 1005 || !Constants.PKFreeMaps.Contains(client.Map.ID))
                            if (client.ArenaStatistic.Status == Network.GamePackets.ArenaStatistic.WaitingForOpponent)
                                return true;
                    }
                }
            }
            return false;
        }
    }
}
