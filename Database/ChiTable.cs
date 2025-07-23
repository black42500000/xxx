﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Conquer_Online_Server.Game.ConquerStructures;
using Conquer_Online_Server.Game;
using System.Collections.Concurrent;

namespace Conquer_Online_Server.Database
{
    public class ChiTable
    {
        public class ChiData
        {
            public uint UID;
            public string Name;
            public List<ChiPowerStructure> Powers;

            public int DragonRank, PhoenixRank, TigerRank, TurtleRank;

            public int DragonPoints
            {
                get
                {
                    if (Powers.Count > 0)
                        return Powers[0].Points;
                    return 0;
                }
            }
            public int PhoenixPoints
            {
                get
                {
                    if (Powers.Count > 1)
                        return Powers[1].Points;
                    return 0;
                }
            }
            public int TigerPoints
            {
                get
                {
                    if (Powers.Count > 2)
                        return Powers[2].Points;
                    return 0;
                }
            }
         
            public int TurtlePoints
            {
                get
                {
                    if (Powers.Count > 3)
                        return Powers[3].Points;
                    return 0;
                }
            }

            internal uint SelectRank(Enums.ChiPowerType chiPowerType)
            {
                switch (chiPowerType)
                {
                    case Enums.ChiPowerType.Dragon:
                        return (uint)DragonRank;
                    case Enums.ChiPowerType.Phoenix:
                        return (uint)PhoenixRank;
                    case Enums.ChiPowerType.Tiger:
                        return (uint)TigerRank;
                    case Enums.ChiPowerType.Turtle:
                        return (uint)TurtleRank;
                }
                return 0;
            }

            internal uint SelectPoints(Enums.ChiPowerType chiPowerType)
            {
                switch (chiPowerType)
                {
                    case Enums.ChiPowerType.Dragon:
                        return (uint)DragonPoints;
                    case Enums.ChiPowerType.Phoenix:
                        return (uint)PhoenixPoints;
                    case Enums.ChiPowerType.Tiger:
                        return (uint)TigerPoints;
                    case Enums.ChiPowerType.Turtle:
                        return (uint)TurtlePoints;
                }
                return 0;
            }
        }
        public static ConcurrentDictionary<uint, ChiData> AllData = new ConcurrentDictionary<uint, ChiData>();
        public static ChiData[] Dragon, Phoenix, Tiger, Turtle;

        public static void LoadAllChi()
        {
            using (var cmd = new MySqlCommand(MySqlCommandType.SELECT))
            {
                cmd.Select("chi");
                using (MySqlReader rdr = new MySqlReader(cmd))
                {
                    while (rdr.Read())
                    {
                        ChiData chiData = new ChiData();
                        chiData.UID = rdr.ReadUInt32("uid");
                        chiData.Name = rdr.ReadString("name");
                        chiData.Powers = new List<ChiPowerStructure>();
                        byte[] data = rdr.ReadBlob("chipowers");
                        if (data.Length > 0)
                        {
                            using (var stream = new MemoryStream(data))
                            using (var reader = new BinaryReader(stream))
                            {
                                int count = reader.ReadByte();
                                for (int i = 0; i < count; i++)
                                    chiData.Powers.Add(new ChiPowerStructure().Deserialize(reader));
                            }
                        }
                        AllData[chiData.UID] = chiData;
                    }
                }
            }
        }
        public static void Load(Client.GameClient client)
        {
            using (var cmd = new MySqlCommand(MySqlCommandType.SELECT))
            {
                cmd.Select("chi").Where("uid", client.Entity.UID);
                using (MySqlReader rdr = new MySqlReader(cmd))
                {
                    if (rdr.Read())
                    {
                        client.ChiPoints = rdr.ReadUInt32("points");
                        byte[] data = rdr.ReadBlob("chipowers");
                        if (data.Length > 0)
                        {
                            using (var stream = new MemoryStream(data))
                            using (var reader = new BinaryReader(stream))
                            {
                                int count = reader.ReadByte();
                                for (int i = 0; i < count; i++)
                                {
                                    var power = new ChiPowerStructure().Deserialize(reader);
                                    if (power.Power == (Enums.ChiPowerType)(i + 1))
                                        client.ChiPowers.Add(power);
                                }
                            }
                        }
                    }
                    else
                    {
                        using (var command = new MySqlCommand(MySqlCommandType.INSERT))
                        {
                            command.Insert("chi").Insert("uid", client.Entity.UID).Insert("name", client.Entity.Name);
                            command.Execute();
                        }
                    }
                }
                client.ChiData = (AllData[client.Entity.UID] = new ChiData() { UID = client.Entity.UID, Name = client.Entity.Name, Powers = client.ChiPowers });
                Sort();
            }
        }
        public static void Save(Client.GameClient client)
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write((byte)client.ChiPowers.Count);
            foreach (var chiPower in client.ChiPowers)
                chiPower.Serialize(writer);
            string SQL = "UPDATE `chi` SET chipowers=@ChiPowers, points=@Points where UID = " + client.Entity.UID + " ;";
            byte[] rawData = stream.ToArray();
            using (var conn = DataHolder.MySqlConnection)
            {
                conn.Open();
                using (var cmd = new MySql.Data.MySqlClient.MySqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = SQL;
                    cmd.Parameters.AddWithValue("@ChiPowers", rawData);
                    cmd.Parameters.AddWithValue("@Points", client.ChiPoints);
                    cmd.ExecuteNonQuery();
                }
            }
        }


        public static object SyncRoot = new object();
        public static void Sort(Enums.ChiPowerType type = Enums.ChiPowerType.None)
        {
            lock (SyncRoot)
            {
                if (type == Enums.ChiPowerType.Dragon || type == Enums.ChiPowerType.None)
                {
                    Dragon = AllData.Values.OrderByDescending((c) => c.DragonPoints).ToArray();
                    SetRank(Dragon, (a, b) => { a.DragonRank = b; });
                }
                if (type == Enums.ChiPowerType.Phoenix || type == Enums.ChiPowerType.None)
                {
                    Phoenix = AllData.Values.OrderByDescending((c) => c.PhoenixPoints).ToArray();
                    SetRank(Phoenix, (a, b) => { a.PhoenixRank = b; });
                }
                if (type == Enums.ChiPowerType.Tiger || type == Enums.ChiPowerType.None)
                {
                    Tiger = AllData.Values.OrderByDescending((c) => c.TigerPoints).ToArray();
                    SetRank(Tiger, (a, b) => { a.TigerRank = b; });
                }
                if (type == Enums.ChiPowerType.Turtle || type == Enums.ChiPowerType.None)
                {
                    Turtle = AllData.Values.OrderByDescending((c) => c.TurtlePoints).ToArray();
                    SetRank(Turtle, (a, b) => { a.TurtleRank = b; });
                }
            }
        }
        public static void SetRank(ChiData[] array, Action<ChiData, int> modify)
        {
            for (int i = 0; i < array.Length; i++)
                modify(array[i], i + 1);
        }
    }
}
