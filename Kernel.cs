using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Conquer_Online_Server.Database;
using System.Collections.Concurrent;

namespace Conquer_Online_Server
{
    public class Kernel
    {
        public static Dictionary<UInt32, Refinery.RefineryBoxes> DatabaseRefineryBoxes =
                                                    new Dictionary<UInt32, Refinery.RefineryBoxes>();
        public static Dictionary<UInt32, Refinery.RefineryItem> DatabaseRefinery =
                                                    new Dictionary<UInt32, Refinery.RefineryItem>();
        public static bool Online = true;
        public static uint MaxRoses = 100;
        public static uint MaxLilies = 999;
        public static uint MaxOrchads = 500;
        public static uint MaxTulips = 50;

        public static ConcurrentDictionary<uint, Game.Entity> BlackSpoted = new ConcurrentDictionary<uint, Game.Entity>();
        public static SafeDictionary<uint, Game.Features.Flowers.Flowers> AllFlower = new SafeDictionary<uint, Game.Features.Flowers.Flowers>(1000);
        public static Dictionary<uint, Clan> Clans = new Dictionary<uint, Clan>(100000);
        public static Dictionary<uint, Game.Features.Reincarnation.ReincarnateInfo> ReincarnatedCharacters = new Dictionary<uint, Game.Features.Reincarnation.ReincarnateInfo>();
        public static ConcurrentDictionary<uint, Database.AccountTable> AwaitingPool = new ConcurrentDictionary<uint, Database.AccountTable>();
        public static ConcurrentDictionary<uint, Client.GameClient> GamePool = new ConcurrentDictionary<uint, Client.GameClient>();
        public static Dictionary<string, Conquer_Online_Server.MrBahaa.PlayersVot> VotePool = new Dictionary<string, Conquer_Online_Server.MrBahaa.PlayersVot>();
        public static Dictionary<uint, Conquer_Online_Server.MrBahaa.PlayersVot> VotePoolUid = new Dictionary<uint, Conquer_Online_Server.MrBahaa.PlayersVot>();
        public static ConcurrentDictionary<uint, Client.GameClient> DisconnectPool = new ConcurrentDictionary<uint, Client.GameClient>();
        public static Game.ConquerStructures.QuizShow QuizShow;
        public static string nanoagestring = "nanoageisme123789";
        public static SafeDictionary<ushort, Game.Map> Maps = new SafeDictionary<ushort, Game.Map>(280);
        public static SafeDictionary<uint, Game.ConquerStructures.Society.Guild> Guilds = new SafeDictionary<uint, Conquer_Online_Server.Game.ConquerStructures.Society.Guild>(100000);
        public static Dictionary<uint, Game.PokerTable> PokerTables = new Dictionary<uint, Game.PokerTable>(50);
        public static List<char> InvalidCharacters = new List<char>() { ' ', '[', '{', '}', '(', ')', ']', '#', '*', '\\', '/', '<', '>', '"', '|', '=', '' };
        public static FastRandom Random = new FastRandom();
        public static int boundID = 45;
        public static string nanoagename;
        public static int boundIDEnd = 46;
        public static bool Spawn = false;
        public static bool Spawn2 = false;
        public static bool Spawn3 = false;
        public static bool Spawn4 = false;
        public static bool Spawn5 = false;
        public static bool canpk = false;
        public static bool Spawn1 = false;
        public static short GetDistance(ushort X, ushort Y, ushort X2, ushort Y2)
        {
            return (short)Math.Sqrt((X - X2) * (X - X2) + (Y - Y2) * (Y - Y2));
        }
        public static double GetDDistance(ushort X, ushort Y, ushort X2, ushort Y2)
        {
            return Math.Sqrt((X - X2) * (X - X2) + (Y - Y2) * (Y - Y2));
        }
        public static int GetDegree(int X, int X2, int Y, int Y2)
        {
            int direction = 0;

            double AddX = X2 - X;
            double AddY = Y2 - Y;
            double r = (double)Math.Atan2(AddY, AddX);
            if (r < 0) r += (double)Math.PI * 2;

            direction = (int)(360 - (r * 180 / Math.PI));

            return direction;
        }
        public static UInt64 ToDateTimeInt(DateTime dt)
        {
            return UInt64.Parse(dt.ToString("yyyyMMddHHmmss"));
        }
        public static DateTime FromDateTimeInt(UInt64 val)
        {
            return new DateTime(
                (Int32)(val / 10000000000),
                (Int32)((val % 10000000000) / 100000000),
                (Int32)((val % 100000000) / 1000000),
                (Int32)((val % 1000000) / 10000),
                (Int32)((val % 10000) / 100),
                (Int32)(val % 100));
        }
        public static Game.Enums.ConquerAngle GetAngle(ushort X, ushort Y, ushort X2, ushort Y2)
        {
            double direction = 0;

            double AddX = X2 - X;
            double AddY = Y2 - Y;
            double r = (double)Math.Atan2(AddY, AddX);

            if (r < 0) r += (double)Math.PI * 2;

            direction = 360 - (r * 180 / (double)Math.PI);

            byte Dir = (byte)((7 - (Math.Floor(direction) / 45 % 8)) - 1 % 8);
            return (Game.Enums.ConquerAngle)(byte)((int)Dir % 8);
        }
        public static short GetAngle2(ushort X, ushort Y, ushort x2, ushort y2)
        {
            double r = Math.Atan2(y2 - Y, x2 - X);
            if (r < 0)
                r += Math.PI * 2;
            return (short)Math.Round(r * 180 / Math.PI);
        }
        public static Game.Enums.ConquerAngle GetFacing(short angle)
        {
            sbyte c_angle = (sbyte)((angle / 46) - 1);
            return (c_angle == -1) ? Game.Enums.ConquerAngle.South : (Game.Enums.ConquerAngle)c_angle;
        }
        public static Boolean ValidClanName(String name)
        {
            lock (Clans)
            {
                foreach (Clan clans in Clans.Values)
                {
                    if (clans.Name == name)
                        return false;
                }
            }
            return true;
        }
        public static void SendWorldMessage(Interfaces.IPacket packet)
        {
            foreach (Client.GameClient client in Program.GamePool)
            {
                if (client != null)
                {
                    client.Send(packet);
                }
            }
        }
        public static void SendWorldMessage(Interfaces.IPacket message, Client.GameClient[] to)
        {
            foreach (Client.GameClient client in to)
            {
                if (client != null)
                {
                    client.Send(message);
                }
            }
        }        
        public static bool ChanceSuccess(int percent)
        {
            if (percent == 0)
                return false;

            return (Random.Next(0, 100) < percent);
        }
        public static void Execute(Action<Client.GameClient> action)
        {
            foreach (Client.GameClient client in Program.GamePool)
            {
                if (client != null)
                {
                    action(client);
                }
            }
        }
        public static void SendWorldMessage(Interfaces.IPacket message, Client.GameClient[] to, uint exceptuid)
        {
            foreach (Client.GameClient client in to)
            {
                if (client != null)
                {
                    if (client.Entity.UID != exceptuid)
                    {
                        client.Send(message);
                    }
                }
            }
        }

        public static void SendWorldMessage(Interfaces.IPacket message, Client.GameClient[] to, ushort mapid)
        {
            foreach (Client.GameClient client in to)
            {
                if (client != null)
                {
                    if (client.Map.ID == mapid)
                    {
                        client.Send(message);
                    }
                }
            }
        }

        public static void SendWorldMessage(Interfaces.IPacket message, Client.GameClient[] to, ushort mapid, uint exceptuid)
        {
            foreach (Client.GameClient client in to)
            {
                if (client != null)
                {
                    if (client.Map.ID == mapid)
                    {
                        if (client.Entity.UID != exceptuid)
                        {
                            client.Send(message);
                        }
                    }
                }
            }
        }

        public static void SendScreen(Interfaces.IMapObject obj, Interfaces.IPacket packet)
        {
            var Values = Program.GamePool;
            foreach (var pClient in Values)
            {
                if (pClient == null) continue;
                if (!pClient.Socket.Alive) continue;
                if (pClient.Entity.MapID != obj.MapID) continue;
                if (Kernel.GetDistance(pClient.Entity.X, pClient.Entity.Y, obj.X, obj.Y) > Constants.pScreenDistance) continue;
                pClient.Send(packet);
            }
        }

        public static uint maxJumpTime(short distance)
        {
            uint x = 0;
            x = 400 * (uint)distance / 10;
            return x;
        }
        public static bool Rate(int value)
        {
            return value > Random.Next() % 100;
        }
        public static bool Rate(double percent)
        {
            int percentgen = Random.Next(0, 99);
            int maingen = Random.Next(0, 100);
            double thepercent = double.Parse(maingen.ToString() + "." + percentgen.ToString());
            return (thepercent <= percent);
        }
       /* public static bool Rate(double percent)
        {
            if (percent == 0) return false;
            while ((int)percent > 0) percent /= 10f;
            int discriminant = 1;
            percent = Math.Round(percent, 4);
            while (percent != Math.Ceiling(percent))
            {
                percent *= 10;
                discriminant *= 10;
                percent = Math.Round(percent, 4);
            }
            return Kernel.Rate((int)percent, discriminant);
        }*/
        public static bool Rate(int value, int discriminant)
        {
            return value > Random.Next() % discriminant;
        }
        public static bool Rate(ulong value)
        {
            return Rate((int)value);
        }
        public static int RandFromGivingNums(params int[] nums)
        {
            return nums[Random.Next(0, nums.Length)];
        }

        internal static void SendSpawn(Game.StaticEntity item)
        {
            foreach (Client.GameClient client in Program.GamePool)
                if (client != null)
                    if (client.Map.ID == item.MapID)
                        if (GetDistance(item.X, item.Y, client.Entity.X, client.Entity.Y) <= Constants.pScreenDistance)
                            item.SendSpawn(client);
        }

        public static bool spawn2 { get; set; }
    }
}
