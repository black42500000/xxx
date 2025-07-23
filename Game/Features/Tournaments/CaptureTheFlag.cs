using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer_Online_Server.Network.GamePackets;
using Conquer_Online_Server.Client;
using System.Collections.Concurrent;
using Conquer_Online_Server.Game.ConquerStructures.Society;
using Conquer_Online_Server.Network;

namespace Conquer_Online_Server.Game
{
    public class CaptureTheFlag
    {
        public class Base
        {
            public SobNpcSpawn Flag;
            public ConcurrentDictionary<uint, uint> Scores;
            public uint CapturerID;

            public Base(SobNpcSpawn flag)
            {
                Flag = flag;
                Scores = new ConcurrentDictionary<uint, uint>();
                CapturerID = 0;
            }

            public void Capture()
            {
                if (Scores.Count == 0) Scores.Add((uint)0, (uint)0);
                uint guildId = Scores.OrderByDescending(p => p.Value).Single().Key;
                CapturerID = guildId;
                var guild = Kernel.Guilds[guildId];
                Flag.Name = guild.Name;
                Flag.Hitpoints = Flag.MaxHitpoints;
                Kernel.SendScreen(Flag, Flag);
            }
        }
        public const ushort MapID = 2057;

        private Map Map;
        public Dictionary<uint, Base> Bases;
        public static bool IsWar;
        public static DateTime StartTime;

        public CaptureTheFlag()
        {
            Bases = new Dictionary<uint, Base>();
            Map = Kernel.Maps[MapID];
            foreach (var npc in Map.Npcs.Values)
                if (npc is SobNpcSpawn)
                    Bases.Add(npc.UID, new Base((SobNpcSpawn)npc));
            SpawnFlags();
        }

        public void SpawnFlags()
        {
            int toAdd = 6 - Map.StaticEntities.Count;
            for (int i = toAdd; i > 0; i--)
            {
                var coords = Map.RandomCoordinates();
                StaticEntity entity = new StaticEntity((uint)(coords.Item1 * 1000 + coords.Item2), coords.Item1, coords.Item2, MapID);
                entity.DoFlag();
                Map.AddStaticEntity(entity);
            }
        }

        public bool SignUp(GameClient client)
        {
            if (client.Entity.GuildID == 0) return false;
            if (client.Guild == null) return false;
            var coords = Map.RandomCoordinates(482, 367, 27);
            client.Entity.Teleport(MapID, coords.Item1, coords.Item2);
            return true;
        }

        public void AroundBase(GameClient client)
        {
            if (client.Entity.MapID != MapID) return;
            if (client.Entity.GuildID == 0) return;
            if (client.Guild == null) return;

            foreach (var _base in Bases.Values)
            {
                if (Kernel.GetDistance(client.Entity.X, client.Entity.Y, _base.Flag.X, _base.Flag.Y) <= 9)
                {
                    if (_base.CapturerID == client.Entity.GuildID)
                    {
                        if (client.Entity.ContainsFlag2(Update.Flags2.CarryingFlag))
                        {
                            client.Send(generateTimer(0));
                            client.Send(generateEffect());
                            client.Entity.RemoveFlag2(Update.Flags2.CarryingFlag);
                            client.Guild.CTFPoints += (ushort)(client.Entity.Level / 2);
                            //get points
                        }
                    }
                }
            }
        }

        public static bool Attackable(Game.Entity entity)
        {
            return Kernel.GetDistance(entity.X, entity.Y, 482, 367) > 32;
        }

        public void AddScore(uint damage, Guild guild, SobNpcSpawn attacked)
        {
            if (Bases.ContainsKey(attacked.UID))
            {
                var _base = Program.World.CTF.Bases[attacked.UID];
                if (!_base.Scores.ContainsKey(guild.ID))
                    _base.Scores.Add(guild.ID, damage);
                else
                    _base.Scores[guild.ID] += damage;
            }
        }

        public static void Close()
        {
            foreach (var player in Program.GamePool)
                if (player.Entity.MapID == MapID)
                    player.Entity.Teleport(1002, 301, 278);

            var array = Kernel.Guilds.Values.Where(p => p.CTFPoints != 0).OrderByDescending(p => p.CTFPoints).ToArray();
            for (int i = 0; i < Math.Min(8, array.Length); i++)
            {
                Database.GuildTable.SaveCTFPoins(array[i]);
                Database.GuildTable.SaveCTFReward(array[i]);
                if (i == 0)
                {
                    array[i].CTFReward += 10;
                    array[i].ConquerPointFund += 3000;
                    array[i].SilverFund += 120000000;
                }
                else if (i == 1)
                {
                    array[i].CTFReward += 9;
                    array[i].ConquerPointFund += 2000;
                    array[i].SilverFund += 100000000;
                }
                else if (i == 2)
                {
                    array[i].CTFReward += 8;
                    array[i].ConquerPointFund += 1000;
                    array[i].SilverFund += 80000000;
                }
                else if (i == 3)
                {
                    array[i].CTFReward += 7;
                    array[i].ConquerPointFund += 600;
                    array[i].SilverFund += 65000000;
                }
                else if (i == 4)
                {
                    array[i].CTFReward += 6;
                    array[i].ConquerPointFund += 500;
                    array[i].SilverFund += 50000000;
                }
                else if (i == 5)
                {
                    array[i].CTFReward += 5;
                    array[i].ConquerPointFund += 400;
                    array[i].SilverFund += 40000000;
                }
                else if (i == 6)
                {
                    array[i].CTFReward += 4;
                    array[i].ConquerPointFund += 300;
                    array[i].SilverFund += 30000000;
                }
                else if (i == 7)
                {
                    array[i].CTFReward += 3;
                    array[i].ConquerPointFund += 200;
                    array[i].SilverFund += 20000000;
                }
            }
        }

        public void SendUpdates(GameClient client)
        {
            if (Time32.Now > client.CTFUpdateStamp.AddSeconds(5))
            {
                client.CTFUpdateStamp = Time32.Now;
                var buffer = generateCTFRanking();
                client.Send(buffer);
                foreach (var _base in Bases.Values)
                {
                    if (Kernel.GetDistance(client.Entity.X, client.Entity.Y, _base.Flag.X, _base.Flag.Y) <= 9)
                    {
                        buffer = generateFlagRanking(_base);
                        client.Send(buffer);
                    }
                }
            }
        }

        public byte[] generateCTFRanking()
        {
            var array = Kernel.Guilds.Values.Where(p => p.CTFPoints != 0).OrderByDescending(p => p.CTFPoints).ToArray();
            return generateList(2, array, p => p.CTFPoints);
        }
        private byte[] generateFlagRanking(Base flag)
        {
            var scores = flag.Scores.OrderByDescending(p => p.Value).ToArray();
            var array = new Guild[Math.Min(5, scores.Length)];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = Kernel.Guilds[scores[i].Key];
                array[i].CTFFlagScore = scores[i].Value;
            }
            return generateList(1, array, p => p.CTFFlagScore);
        }

        private byte[] generateList(int type, Guild[] array = null, Func<Guild, UInt32> select = null)
        {
            byte[] data = null;
            if (array == null)
                data = new byte[48];
            else
                data = new byte[48 + array.Length * 24];
            Writer.WriteInt32(data.Length - 8, 0, data);
            Writer.WriteUInt16(2224, 2, data);
            Writer.WriteInt32(type, 4, data);
            if (array != null)
            {
                Writer.WriteInt32(array.Length, 28, data);
                for (int i = 0; i < array.Length; i++)
                {
                    int offset = 32 + i * 24;
                    Writer.WriteInt32(i, offset, data); offset += 4;
                    Writer.WriteUInt32(select(array[i]), offset, data); offset += 4;
                    Writer.WriteString(array[i].Name, 0, data);
                }
            }
            return data;
        }
        public byte[] generateTimer(uint time)
        {
            return generatePacket(8, time);
        }
        public byte[] generateEffect()
        {
            return generatePacket(6, 6327607);
        }
        private byte[] generatePacket(int type, uint dwParam)
        {
            byte[] data = new byte[48];
            Writer.WriteInt32(data.Length - 8, 0, data);
            Writer.WriteUInt16(2224, 2, data);
            Writer.WriteInt32(type, 4, data);
            Writer.WriteUInt32(dwParam, 8, data);
            return data;
        }
        public void SendUpdates()
        {
            foreach (var player in Program.GamePool)
                if (player.Entity.MapID == MapID)
                    SendUpdates(player);
        }

        public void CloseList(GameClient client)
        {
            client.Send(generateList(3));
        }
    }
}
