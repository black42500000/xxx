using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer_Online_Server.Client;
using Conquer_Online_Server.Network.GamePackets;
using System.Drawing;
using System.Collections.Concurrent;

namespace Conquer_Online_Server.Game
{
    public class KillTournament
    {
        private ushort mapID;
        private Map map;
        private WeekDay day;
        private int hour, minute;
        private Func<GameClient, bool> selectFunc;
        private IDisposable Subscriber;
        private bool onGoing, announcement, canAttack;
        private string name;
        private ConcurrentDictionary<uint, GameClient> players;
        private Action<GameClient> prize;
        private Time32 cantAttack;
        private string message;
        public bool IsOn { get { return onGoing; } }

        public KillTournament(ushort mapID, WeekDay day, int hour, int minute, Action<GameClient> prize, string name, Func<GameClient, bool> selectFunc, string message = "")
        {
            this.mapID = mapID;
            Constants.PKFreeMaps.Add(this.mapID);
            this.day = day;
            this.hour = hour;
            this.minute = minute;
            this.prize = prize;
            this.name = name;
            this.message = message;
            this.selectFunc = selectFunc;
            this.Subscriber = World.Subscribe(work, 1000);
            this.onGoing = this.announcement = this.canAttack = false;
            this.players = new ConcurrentDictionary<uint, GameClient>();
        }

        private void work(int time)
        {
            if (map == null)
            {
                if (!Kernel.Maps.ContainsKey(mapID))
                    return;
                map = Kernel.Maps[mapID];
            }
            DateTime time64 = DateTime.Now;
            DateTime nextTournament = setDate(time64);
            if (day.Contains(time64.DayOfWeek))
            {
                if (!onGoing)
                {
                    if (!announcement)
                    {
                        if (time64 >= nextTournament.Subtract(TimeSpan.FromMinutes(5)) && time64 <= nextTournament)
                        {
                            announcement = true;
                            Kernel.SendWorldMessage(new Message(name + " begins in 5 minutes.", Color.Red, Message.Center));
                        }
                    }
                    else
                    {
                        if (time64 >= nextTournament)
                        {
                            onGoing = true;
                            canAttack = false;
                            cantAttack = Time32.Now.AddMinutes(1);
                            foreach (var client in Program.GamePool)
                            {
                                if (selectFunc(client))
                                {
                                    client.SelectionKillTournament = this;
                                    client.MessageOK = (pClient) =>
                                    {
                                        if (pClient.SelectionKillTournament.onGoing)
                                            pClient.SelectionKillTournament.joinClient(pClient);
                                    };
                                    if (message != "") client.Send(message);
                                    client.Send(new NpcReply(NpcReply.MessageBox, name + " began. Would you like to join?"));
                                    client.Send(new Data(true) { UID = client.Entity.UID, dwParam = 60, ID = Data.CountDown });
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (!canAttack)
                    {
                        if (time64 >= nextTournament.AddMinutes(1))
                        {
                            canAttack = true;
                            foreach (var client in players.Values)
                            {
                                client.Send(new Message("You can now attack!", Color.Red, Message.Talk));
                                client.Send(new Message("The tournament ends at " + nextTournament.AddMinutes(10).Minute + "!", Color.Red, Message.Talk));
                            }
                        }
                    }
                    else
                    {
                        if (time64 >= nextTournament.AddMinutes(10))
                            checkLives();
                    }
                }
            }
        }

        private DateTime setDate(DateTime time64)
        {
            return new DateTime(time64.Year, time64.Month, time64.Day, hour, minute, 0);
        }

        public static KillTournament Select(GameClient client, uint mapID)
        {
            DateTime now = DateTime.Now;
            foreach (var tournament in Program.World.Tournaments)
                if (tournament.map.BaseID == mapID)
                    if (tournament.onGoing)
                        if (now <= tournament.setDate(now).AddMinutes(10))
                            if (tournament.selectFunc(client))
                                return tournament;
            return null;
        }

        public bool OnGoing()
        {
            return onGoing;
        }

        public void Join(GameClient client)
        {
            joinClient(client);
        }

        private void joinClient(GameClient client)
        {
            var coordinates = map.RandomCoordinates();
            client.Entity.Teleport(map.BaseID, map.ID, coordinates.Item1, coordinates.Item2);
            client.CantAttack = cantAttack;
            players.Add(client.Entity.UID, client);
            client.Entity.OnDeath = (pEntity) =>
            {
                pEntity.Teleport(1002, 400, 400);
                players.Remove(pEntity.UID);
                checkLives();
            };
            client.OnDisconnect = (pClient) =>
            {
                pClient.Entity.Teleport(1002, 400, 400);
                players.Remove(pClient.Entity.UID);
                checkLives();
            };
        }

        private void checkLives()
        {
            if (players.Values.Count((o) => { return o.Entity.MapID == map.ID; }) <= 1)
            {
                var player = players.Values.FirstOrDefault((o) => { return o.Entity.MapID == map.ID; });
                if (player != null)
                {
                    player.Entity.Teleport(1002, 400, 400);
                    prize(player);
                    Kernel.SendWorldMessage(new Message(player.Entity.Name + " won the " + name + "!", Color.Red, Message.Talk));
                    Kernel.SendWorldMessage(new Message(player.Entity.Name + " won the " + name + "!", Color.Red, Message.Center));
                }
                players.Clear();
                announcement = false;
                onGoing = false;
            }
        }
    }
}