using System;
using System.IO;
using System.Drawing;
using System.Threading;
using System.Collections.Generic;
using Conquer_Online_Server.Interfaces;
using System.Text;
using System.Linq;
using Conquer_Online_Server.Network.GamePackets;
using System.Threading.Generic;
using Conquer_Online_Server.Client;
using System.Collections.Concurrent;

namespace Conquer_Online_Server.Game
{
    public class Map
    {
        public static Counter DynamicIDs = new Conquer_Online_Server.Counter(11000) { Finish = 60000 };
        public Counter CloneCounter = new Counter(0);  

        public static Enums.ConquerAngle[] Angles = new Enums.ConquerAngle[] {
            Enums.ConquerAngle.SouthWest,
            Enums.ConquerAngle.West,
            Enums.ConquerAngle.NorthWest,
            Enums.ConquerAngle.North,
            Enums.ConquerAngle.NorthEast,
            Enums.ConquerAngle.East,
            Enums.ConquerAngle.SouthEast,
            Enums.ConquerAngle.South };
        public static Floor ArenaBaseFloor = null;
        public Counter EntityUIDCounter = new Conquer_Online_Server.Counter(400000);
        public List<Zoning.Zone> Zones = new List<Zoning.Zone>();
        public SafeDictionary<uint, Entity> BodyGuards;
        public SafeDictionary<uint, Entity> AIs;
        public ushort ID;
        public ushort Level;
        public ushort BaseID;
        public bool WasPKFree = false;
        public Floor Floor;
        private string Path;
        public bool IsDynamic()
        {
            return BaseID != ID;
        }
        public SafeDictionary<uint, Entity> Entities;
        public SafeDictionary<uint, Entity> Companions;
        public Dictionary<uint, INpc> Npcs;
        public ConcurrentDictionary<uint, FloorItem> FloorItems;
        public void AddPole(INpc npc)
        {
            Npcs[npc.UID] = npc;
            Floor[npc.X, npc.Y, MapObjectType.InvalidCast, npc] = false;
        }
        public void RemovePole(INpc npc)
        {
            Npcs.Remove(npc.UID);
            Floor[npc.X, npc.Y, MapObjectType.InvalidCast, null] = true;
        }
        public void AddNpc(INpc npc)
        {
            if (Npcs.ContainsKey(npc.UID) == false)
            {
                Npcs.Add(npc.UID, npc);
                #region Setting the near coords invalid to avoid unpickable items.
                Floor[npc.X, npc.Y, MapObjectType.InvalidCast, npc] = false;
                if (npc.Mesh / 10 != 108 && (byte)npc.Type < 10)
                {
                    ushort X = npc.X, Y = npc.Y;
                    foreach (Enums.ConquerAngle angle in Angles)
                    {
                        ushort xX = X, yY = Y;
                        UpdateCoordonatesForAngle(ref xX, ref yY, angle);
                        Floor[xX, yY, MapObjectType.InvalidCast, null] = false;
                    }
                }
                #endregion
            }
        }
        public void AddEntity(Entity entity)
        {
            if (entity.UID < 800000)
            {
                if (Entities.ContainsKey(entity.UID) == false)
                {
                    Entities.Add(entity.UID, entity);
                    Floor[entity.X, entity.Y, MapObjectType.Monster, entity] = false;
                }
            }
            else
            {
                if (Companions.ContainsKey(entity.UID) == false)
                {
                    Companions.Add(entity.UID, entity);
                    Floor[entity.X, entity.Y, MapObjectType.Monster, entity] = false;
                }
            }
        }
        public void AddEntity2(Entity entity)
        {
            if (!this.Entities.ContainsKey(entity.UID))
            {
                this.Entities.Add(entity.UID, entity);
                this.Floor[entity.X, entity.Y, MapObjectType.Monster, entity] = false;
            }
        }
        public void RemoveAI(Entity entity)
        {
            if (this.AIs.ContainsKey(entity.UID))
            {
                this.AIs.Remove(entity.UID);
                this.Floor[entity.X, entity.Y, MapObjectType.Player, entity] = true;
            }
        }
        public void RemoveEntity(Entity entity)
        {
            if (Entities.ContainsKey(entity.UID) == true)
            {
                Entities.Remove(entity.UID);
                Floor[entity.X, entity.Y, MapObjectType.Monster, entity] = true;
            }
            if (Companions.ContainsKey(entity.UID) == true)
            {
                Companions.Remove(entity.UID);
                Floor[entity.X, entity.Y, MapObjectType.Monster, entity] = true;
            }
        }
        public void AddAI(Entity entity)
        {
            if (!this.AIs.ContainsKey(entity.UID))
            {
                this.AIs.Add(entity.UID, entity);
                this.Floor[entity.X, entity.Y, MapObjectType.Player, entity] = false;
            }
            else
            {
                this.RemoveEntity(entity);
                this.BodyGuards.Add(entity.UID, entity); 
                this.AIs.Add(entity.UID, entity);
                this.Floor[entity.X, entity.Y, MapObjectType.Player, entity] = false;
            }
        }
        /*public void AddBodyGuard(Entity entity)
        {
            if (!this.BodyGuards.ContainsKey(entity.UID))
            {
                this.BodyGuards.Add(entity.UID, entity);
                this.Floor[entity.X, entity.Y, MapObjectType.Monster, entity] = false;
            }
            else
            {
                this.RemoveEntity(entity);
                this.BodyGuards.Add(entity.UID, entity);
                this.Floor[entity.X, entity.Y, MapObjectType.Monster, entity] = false;
            }
        }*/
        public void AddCompanion(Entity entity)
        {
            if (!this.Companions.ContainsKey(entity.UID))
            {
                this.Companions.Add(entity.UID, entity);
                this.Floor[entity.X, entity.Y, MapObjectType.Monster, entity] = false;
            }
            else
            {
                this.RemoveEntity(entity);
                this.Companions.Add(entity.UID, entity);
                this.Floor[entity.X, entity.Y, MapObjectType.Monster, entity] = false;
            }
        }
        public void AddFloorItem(Network.GamePackets.FloorItem floorItem)
        {
            FloorItems.Add(floorItem.UID, floorItem);
            Floor[floorItem.X, floorItem.Y, MapObjectType.Item, floorItem] = false;
        }
        public void RemoveFloorItem(Network.GamePackets.FloorItem floorItem)
        {            
            FloorItems.Remove(floorItem.UID);
            Floor[floorItem.X, floorItem.Y, MapObjectType.Item, floorItem] = true;
        }
        public void AddBodyGuard(Entity entity)
        {
            if (!this.BodyGuards.ContainsKey(entity.UID))
            {
                this.BodyGuards.Add(entity.UID, entity);
                this.Floor[entity.X, entity.Y, MapObjectType.Monster, entity] = false;
            }
            else
            {
                this.RemoveEntity(entity);
                this.BodyGuards.Add(entity.UID, entity);
                this.Floor[entity.X, entity.Y, MapObjectType.Monster, entity] = false;
            }
        } 
        public bool SelectCoordonates(ref ushort X, ref ushort Y)
        {
            if (Floor[X, Y, MapObjectType.Item, null])
            {
                bool can = true;
                if (Zones.Count != 0)
                {
                    foreach (Zoning.Zone z in Zones)
                    {
                        if (z.IsPartOfRectangle(new Point() { X = X, Y = Y }))
                        {
                            can = false;
                            break;
                        }
                    }
                }
                if (can)
                    return true;
            }

            foreach (Enums.ConquerAngle angle in Angles)
            {
                ushort xX = X, yY = Y;
                UpdateCoordonatesForAngle(ref xX, ref yY, angle);
                if (Floor[xX, yY, MapObjectType.Item, null])
                {
                    if (Zones.Count != 0)
                    {
                        bool can = true;
                        foreach (Zoning.Zone z in Zones)
                        {
                            if (z.IsPartOfRectangle(new Point() { X = xX, Y = yY })) { can = false; break; }
                        }
                        if (!can)
                            continue;
                    }
                    X = xX;
                    Y = yY;
                    return true;
                }
            }
            return false;
        }
        public static void UpdateCoordonatesForAngle(ref ushort X, ref ushort Y, Enums.ConquerAngle angle)
        {
            sbyte xi = 0, yi = 0;
            switch (angle)
            {
                case Enums.ConquerAngle.North: xi = -1; yi = -1; break;
                case Enums.ConquerAngle.South: xi = 1; yi = 1; break;
                case Enums.ConquerAngle.East: xi = 1; yi = -1; break;
                case Enums.ConquerAngle.West: xi = -1; yi = 1; break;
                case Enums.ConquerAngle.NorthWest: xi = -1; break;
                case Enums.ConquerAngle.SouthWest: yi = 1; break;
                case Enums.ConquerAngle.NorthEast: yi = -1; break;
                case Enums.ConquerAngle.SouthEast: xi = 1; break;
            }
            X = (ushort)(X + xi);
            Y = (ushort)(Y + yi);
        }

        public static void Pushback(ref ushort x, ref ushort y, Enums.ConquerAngle angle, int paces)
        {
            sbyte xi = 0, yi = 0;
            for (int i = 0; i < paces; i++)
            {
                switch (angle)
                {
                    case Enums.ConquerAngle.North: xi = -1; yi = -1; break;
                    case Enums.ConquerAngle.South: xi = 1; yi = 1; break;
                    case Enums.ConquerAngle.East: xi = 1; yi = -1; break;
                    case Enums.ConquerAngle.West: xi = -1; yi = 1; break;
                    case Enums.ConquerAngle.NorthWest: xi = -1; break;
                    case Enums.ConquerAngle.SouthWest: yi = 1; break;
                    case Enums.ConquerAngle.NorthEast: yi = -1; break;
                    case Enums.ConquerAngle.SouthEast: xi = 1; break;
                }
                x = (ushort)(x + xi);
                y = (ushort)(y + yi);
            }
        }
        #region Scenes
        private SceneFile[] Scenes;
        private static string NTString(string value)
        {
            value = value.Remove(value.IndexOf("\0"));
            return value;
        }
        private SceneFile CreateSceneFile(BinaryReader Reader)
        {
            SceneFile file = new SceneFile();
            file.SceneFileName = NTString(Encoding.Default.GetString(Reader.ReadBytes(260)));
            file.Location = new Point(Reader.ReadInt32(), Reader.ReadInt32());
            using (BinaryReader reader = new BinaryReader(new FileStream(Constants.DataHolderPath + file.SceneFileName, FileMode.Open)))
            {
                ScenePart[] partArray = new ScenePart[reader.ReadInt32()];
                for (int i = 0; i < partArray.Length; i++)
                {
                    reader.BaseStream.Seek(0x14cL, SeekOrigin.Current);
                    partArray[i].Size = new Size(reader.ReadInt32(), reader.ReadInt32());
                    reader.BaseStream.Seek(4L, SeekOrigin.Current);
                    partArray[i].StartPosition = new Point(reader.ReadInt32(), reader.ReadInt32());
                    reader.BaseStream.Seek(4L, SeekOrigin.Current);
                    partArray[i].NoAccess = new bool[partArray[i].Size.Width, partArray[i].Size.Height];
                    for (int j = 0; j < partArray[i].Size.Height; j++)
                    {
                        for (int k = 0; k < partArray[i].Size.Width; k++)
                        {
                            partArray[i].NoAccess[k, j] = reader.ReadInt32() == 0;
                            reader.BaseStream.Seek(8L, SeekOrigin.Current);
                        }
                    }
                }
                file.Parts = partArray;
            }
            return file;
        }
        public struct SceneFile
        {
            public string SceneFileName
            {
                get;
                set;
            }
            public Point Location
            {
                get;
                set;
            }
            public ScenePart[] Parts
            {
                get;
                set;
            }
        }
        public struct ScenePart
        {
            public string Animation;
            public string PartFile;
            public Point Offset;
            public int aniInterval;
            public System.Drawing.Size Size;
            public int Thickness;
            public Point StartPosition;
            public bool[,] NoAccess;
        }
        #endregion

        public Map(ushort id, string path)
        {
            if (!Kernel.Maps.ContainsKey(id))
                Kernel.Maps.Add(id, this);
            Npcs = new Dictionary<uint, INpc>();
            Entities = new SafeDictionary<uint, Entity>();
            FloorItems = new ConcurrentDictionary<uint, FloorItem>();
            Floor = new Floor(0, 0, id);
            Companions = new SafeDictionary<uint, Entity>();
            ID = id;
            BaseID = id;
            if (path == "") path = Database.DMaps.MapPaths[id];
            Path = path;
            #region Loading floor.
            if (File.Exists(Constants.DMapsPath + "\\maps\\" + id.ToString() + ".map"))
            {
                byte[] buff = File.ReadAllBytes(Constants.DMapsPath + "\\maps\\" + id.ToString() + ".map");
                MemoryStream FS = new MemoryStream(buff);
                BinaryReader BR = new BinaryReader(FS);
                int Width = BR.ReadInt32();
                int Height = BR.ReadInt32();
                Floor = new Game.Floor(Width, Height, ID);
                if (id == 700)
                    ArenaBaseFloor = new Game.Floor(Width, Height, ID);
                for (ushort y = 0; y < Height; y = (ushort)(y + 1))
                {
                    for (ushort x = 0; x < Width; x = (ushort)(x + 1))
                    {
                        bool walkable = !(BR.ReadByte() == 1 ? true : false);
                        Floor[x, y, MapObjectType.InvalidCast, null] = walkable;
                        if (id == 700)
                            ArenaBaseFloor[x, y, MapObjectType.InvalidCast, null] = walkable;
                    }
                }

                BR.Close();
                FS.Close();
            }
            else
            {
                if (File.Exists(Constants.DMapsPath + Path))
                {
                    byte[] buff = File.ReadAllBytes(Constants.DMapsPath + Path);
                    MemoryStream FS = new MemoryStream(buff);
                    BinaryReader BR = new BinaryReader(FS);
                    BR.ReadBytes(268);
                    int Width = BR.ReadInt32();
                    int Height = BR.ReadInt32();
                    Floor = new Game.Floor(Width, Height, ID);
                    if (id == 700)
                        ArenaBaseFloor = new Game.Floor(Width, Height, ID);
                    for (ushort y = 0; y < Height; y = (ushort)(y + 1))
                    {
                        for (ushort x = 0; x < Width; x = (ushort)(x + 1))
                        {
                            bool walkable = !Convert.ToBoolean(BR.ReadUInt16());
                            Floor[x, y, MapObjectType.InvalidCast, null] = walkable;
                            if (id == 700)
                                ArenaBaseFloor[x, y, MapObjectType.InvalidCast, null] = walkable;
                            BR.BaseStream.Seek(4L, SeekOrigin.Current);
                        }
                        BR.BaseStream.Seek(4L, SeekOrigin.Current);
                    }
                    uint amount = BR.ReadUInt32();
                    BR.BaseStream.Seek(amount * 12, SeekOrigin.Current);
                    LoadMapObjects(BR);
                    MergeSceneToTextureArea();

                    BR.Close();
                    FS.Close();
                    SaveMap();
                }
            }
            #endregion
            LoadNpcs();
            LoadZones();
            LoadMonsters();
            LoadPortals();
        }
        public Map(ushort id, ushort baseid, string path)
        {
            if (!Kernel.Maps.ContainsKey(id))
                Kernel.Maps.Add(id, this);
            Npcs = new Dictionary<uint, INpc>();
            Entities = new SafeDictionary<uint, Entity>();
            Companions = new SafeDictionary<uint, Entity>();
            FloorItems = new ConcurrentDictionary<uint, FloorItem>();
            ID = id;
            BaseID = baseid;
            Path = path;
            if (String.IsNullOrEmpty(path))
                Path = path = Database.DMaps.MapPaths[baseid];
            Floor = new Floor(0, 0, id);

            #region Loading floor.
            if (id != baseid && baseid == 700 && ArenaBaseFloor != null)
            {
                Floor = new Game.Floor(ArenaBaseFloor.Bounds.Width, ArenaBaseFloor.Bounds.Height, ID);
                for (ushort y = 0; y < ArenaBaseFloor.Bounds.Height; y = (ushort)(y + 1))
                {
                    for (ushort x = 0; x < ArenaBaseFloor.Bounds.Width; x = (ushort)(x + 1))
                    {
                        Floor[x, y, MapObjectType.InvalidCast, null] = !ArenaBaseFloor[x, y, MapObjectType.InvalidCast, null];
                    }
                }
            }
            else
            {
                if (File.Exists(Constants.DMapsPath + "\\maps\\" + baseid.ToString() + ".map"))
                {
                    byte[] buff = File.ReadAllBytes(Constants.DMapsPath + "\\maps\\" + baseid.ToString() + ".map");
                    MemoryStream FS = new MemoryStream(buff);
                    BinaryReader BR = new BinaryReader(FS);
                    int Width = BR.ReadInt32();
                    int Height = BR.ReadInt32();

                    Floor = new Game.Floor(Width, Height, ID);

                    for (ushort y = 0; y < Height; y = (ushort)(y + 1))
                    {
                        for (ushort x = 0; x < Width; x = (ushort)(x + 1))
                        {
                            Floor[x, y, MapObjectType.InvalidCast, null] = !(BR.ReadByte() == 1 ? true : false);
                        }
                    }
                    BR.Close();
                    FS.Close();
                }
                else
                {
                    if (File.Exists(Constants.DMapsPath + Path))
                    {
                        FileStream FS = new FileStream(Constants.DMapsPath + Path, FileMode.Open);
                        BinaryReader BR = new BinaryReader(FS);
                        BR.ReadBytes(268);
                        int Width = BR.ReadInt32();
                        int Height = BR.ReadInt32();

                        Floor = new Game.Floor(Width, Height, ID);

                        for (ushort y = 0; y < Height; y = (ushort)(y + 1))
                        {
                            for (ushort x = 0; x < Width; x = (ushort)(x + 1))
                            {
                                Floor[x, y, MapObjectType.InvalidCast, null] = !Convert.ToBoolean(BR.ReadUInt16());

                                BR.BaseStream.Seek(4L, SeekOrigin.Current);
                            }
                            BR.BaseStream.Seek(4L, SeekOrigin.Current);
                        }
                        uint amount = BR.ReadUInt32();
                        BR.BaseStream.Seek(amount * 12, SeekOrigin.Current);

                        int num = BR.ReadInt32();
                        List<SceneFile> list = new List<SceneFile>();
                        for (int i = 0; i < num; i++)
                        {
                            switch (BR.ReadInt32())
                            {
                                case 10:
                                    BR.BaseStream.Seek(0x48L, SeekOrigin.Current);
                                    break;

                                case 15:
                                    BR.BaseStream.Seek(0x114L, SeekOrigin.Current);
                                    break;

                                case 1:
                                    list.Add(this.CreateSceneFile(BR));
                                    break;

                                case 4:
                                    BR.BaseStream.Seek(0x1a0L, SeekOrigin.Current);
                                    break;
                            }
                        }
                        Scenes = list.ToArray();

                        for (int i = 0; i < Scenes.Length; i++)
                        {
                            foreach (ScenePart part in Scenes[i].Parts)
                            {
                                for (int j = 0; j < part.Size.Width; j++)
                                {
                                    for (int k = 0; k < part.Size.Height; k++)
                                    {
                                        Point point = new Point();
                                        point.X = ((Scenes[i].Location.X + part.StartPosition.X) + j) - part.Size.Width;
                                        point.Y = ((Scenes[i].Location.Y + part.StartPosition.Y) + k) - part.Size.Height;
                                        Floor[(ushort)point.X, (ushort)point.Y, MapObjectType.InvalidCast, null] = part.NoAccess[j, k];
                                    }
                                }
                            }
                        }

                        BR.Close();
                        FS.Close();
                        SaveMap();
                    }
                }
            }
            #endregion
            LoadNpcs();
            LoadZones();
            LoadMonsters();
            LoadPortals();
        }

        private void MergeSceneToTextureArea()
        {
            for (int i = 0; i < Scenes.Length; i++)
            {
                if (Scenes[i].Parts == null) return;
                foreach (ScenePart part in Scenes[i].Parts)
                {
                    for (int j = 0; j < part.Size.Width; j++)
                    {
                        for (int k = 0; k < part.Size.Height; k++)
                        {
                            Point point = new Point
                            {
                                X = ((Scenes[i].Location.X + part.StartPosition.X) - j),
                                Y = ((Scenes[i].Location.Y + part.StartPosition.Y) - k)
                            };
                            Floor[(ushort)point.X, (ushort)point.Y, MapObjectType.InvalidCast] = part.NoAccess[j, k];
                        }
                    }
                }
            }
        }
        private void LoadMapObjects(BinaryReader Reader)
        {
            int num = Reader.ReadInt32();
            List<SceneFile> list = new List<SceneFile>();
            for (int i = 0; i < num; i++)
            {
                int id = Reader.ReadInt32();
                id = (byte)id;
                switch (id)
                {
                    case 10:
                        Reader.BaseStream.Seek(0x48L, SeekOrigin.Current);
                        break;

                    case 15:
                        Reader.BaseStream.Seek(0x114L, SeekOrigin.Current);
                        break;

                    case 1:
                        list.Add(this.CreateSceneFile(Reader));
                        break;

                    case 4:
                        Reader.BaseStream.Seek(0x1a0L, SeekOrigin.Current);
                        break;
                }
            }
            Scenes = list.ToArray();
        }
        private void LoadPortals()
        {
            IniFile file = new Conquer_Online_Server.IniFile(Constants.PortalsPath);
            ushort portalCount = file.ReadUInt16(BaseID.ToString(), "Count");

            for (int i = 0; i < portalCount; i++)
            {
                string _PortalEnter = file.ReadString(BaseID.ToString(), "PortalEnter" + i.ToString());
                string _PortalExit = file.ReadString(BaseID.ToString(), "PortalExit" + i.ToString());
                string[] PortalEnter = _PortalEnter.Split(' ');
                string[] PortalExit = _PortalExit.Split(' ');
                Game.Portal portal = new Conquer_Online_Server.Game.Portal();
                portal.CurrentMapID = Convert.ToUInt16(PortalEnter[0]);
                portal.CurrentX = Convert.ToUInt16(PortalEnter[1]);
                portal.CurrentY = Convert.ToUInt16(PortalEnter[2]);
                if (PortalExit.Length == 3)
                {
                    portal.DestinationMapID = Convert.ToUInt16(PortalExit[0]);
                    portal.DestinationX = Convert.ToUInt16(PortalExit[1]);
                    portal.DestinationY = Convert.ToUInt16(PortalExit[2]);
                }
                Portals.Add(portal);
            }
        }
        public List<Game.Portal> Portals = new List<Game.Portal>();
        private IDisposable Timer;

        public static sbyte[] XDir = new sbyte[] 
        { 
            -1, -2, -2, -1, 1, 2, 2, 1,
             0, -2, -2, -2, 0, 2, 2, 2, 
            -1, -2, -2, -1, 1, 2, 2, 1,
             0, -1, -1, -1, 0, 1, 1, 1,
        };
        public static sbyte[] YDir = new sbyte[] 
        {
            2,  1, -1, -2, -2, -1, 1, 2,
            2,  2,  0, -2, -2, -2, 0, 2, 
            2,  1, -1, -2, -2, -1, 1, 2, 
            1,  1,  0, -1, -1, -1, 0, 1
        };
        public SafeConcurrentDictionary<uint, StaticEntity> StaticEntities = new SafeConcurrentDictionary<uint, StaticEntity>();
        public void AddStaticEntity(StaticEntity item)
        {
            Floor[item.X, item.Y, MapObjectType.StaticEntity, null] = false;
            StaticEntities[item.UID] = item;
        }
        public void RemoveStaticItem(StaticEntity item)
        {
            Floor[item.X, item.Y, MapObjectType.StaticEntity, null] = true;
            StaticEntities.Remove(item.UID);
        }
        private void SaveMap()
        {
            if (!File.Exists(Constants.DMapsPath + "\\maps\\" + BaseID.ToString() + ".map"))
            {
                FileStream stream = new FileStream(Constants.DMapsPath + "\\maps\\" + BaseID.ToString() + ".map", FileMode.Create);
                BinaryWriter writer = new BinaryWriter(stream);
                writer.Write((uint)Floor.Bounds.Width);
                writer.Write((uint)Floor.Bounds.Height);
                for (int y = 0; y < Floor.Bounds.Height; y++)
                {
                    for (int x = 0; x < Floor.Bounds.Width; x++)
                    {
                        writer.Write((byte)(Floor[x, y, MapObjectType.InvalidCast, null] == true ? 1 : 0));
                    }
                }
                writer.Close();
                stream.Close();
            }
        }
        private void LoadZones()
        {
            using (var command = new Database.MySqlCommand(Database.MySqlCommandType.SELECT))
            {
                command.Select("notavailablepaths").Where("mapid", ID);

                using (var reader = new Database.MySqlReader(command))
                {
                    while (reader.Read())
                    {
                        Zoning.Zone zone = new Zoning.Zone(
                            new Point() { X = reader.ReadInt32("Point1_X"), Y = reader.ReadInt32("Point1_Y") },
                            new Point() { X = reader.ReadInt32("Point2_X"), Y = reader.ReadInt32("Point2_Y") },
                            new Point() { X = reader.ReadInt32("Point3_X"), Y = reader.ReadInt32("Point3_Y") },
                            new Point() { X = reader.ReadInt32("Point4_X"), Y = reader.ReadInt32("Point4_Y") }
                            );
                        Zones.Add(zone);
                    }
                }
            }
        }
        private void LoadNpcs()
        {
            using (var command = new Database.MySqlCommand(Database.MySqlCommandType.SELECT))
            {
                command.Select("npcs").Where("mapid", ID);
                using (var reader = new Database.MySqlReader(command))
                {
                    while (reader.Read())
                    {
                        INpc npc = new Network.GamePackets.NpcSpawn();
                        npc.UID = reader.ReadUInt32("id");
                        npc.Mesh = reader.ReadUInt16("lookface");
                        npc.Type = (Enums.NpcType)reader.ReadByte("type");
                        npc.X = reader.ReadUInt16("cellx"); ;
                        npc.Y = reader.ReadUInt16("celly");
                        npc.MapID = ID;
                        //Network.GamePackets.PokerTable npc2 = new Network.GamePackets.PokerTable();
                        AddNpc(npc);
                    }
                }
            }
            using (var command = new Database.MySqlCommand(Database.MySqlCommandType.SELECT))
            {
                command.Select("sobnpcs").Where("mapid", ID);
                using (var reader = new Database.MySqlReader(command))
                {
                    while (reader.Read())
                    {
                        Network.GamePackets.SobNpcSpawn npc = new Network.GamePackets.SobNpcSpawn();
                        npc.UID = reader.ReadUInt32("id");
                        npc.Mesh = reader.ReadUInt16("lookface");
                        if (ID == 1039)
                            npc.Mesh = (ushort)(npc.Mesh - npc.Mesh % 10 + 7);
                        npc.Type = (Enums.NpcType)reader.ReadByte("type");
                        npc.X = reader.ReadUInt16("cellx"); ;
                        npc.Name = reader.ReadString("name");  
                        npc.Y = reader.ReadUInt16("celly");
                        npc.MapID = reader.ReadUInt16("mapid");
                        npc.Sort = reader.ReadUInt16("sort");
                        npc.ShowName = true;
                        npc.Name = reader.ReadString("name");
                        npc.Hitpoints = reader.ReadUInt32("life");
                        npc.MaxHitpoints = reader.ReadUInt32("maxlife");
                        npc._isprize = reader.ReadBoolean("prize");
                        AddNpc(npc);
                    }
                }
            }
        }
        public bool FreezeMonsters = false;
        public void LoadMonsters()
        {
            this.BodyGuards = new SafeDictionary<uint, Entity>(0x3e8);
            Companions = new SafeDictionary<uint, Entity>();
            using (var command = new Database.MySqlCommand(Database.MySqlCommandType.SELECT))
            {
                command.Select("monsterspawns").Where("mapid", ID);
                using (var reader = new Database.MySqlReader(command))
                {
                    int mycount = 0;
                    try
                    {
                        while (reader.Read())
                        {
                            uint monsterID = reader.ReadUInt32("npctype");
                            ushort CircleDiameter = reader.ReadUInt16("maxnpc");
                            ushort X = reader.ReadUInt16("bound_x");
                            ushort Y = reader.ReadUInt16("bound_y");
                            ushort XPlus = reader.ReadUInt16("bound_cx");
                            ushort YPlus = reader.ReadUInt16("bound_cy");
                            ushort Amount = reader.ReadUInt16("max_per_gen");
                            int respawn = reader.ReadInt32("rest_secs");
                            if (Database.MonsterInformation.MonsterInformations.ContainsKey(monsterID))
                            {
                                Database.MonsterInformation mt = Database.MonsterInformation.MonsterInformations[monsterID];
                                mt.RespawnTime = respawn + 5;
                                mt.BoundX = X;
                                mt.BoundY = Y;
                                mt.BoundCX = XPlus;
                                mt.BoundCY = YPlus;

                                bool more = true;
                                for (int count = 0; count < Amount; count++)
                                {
                                    if (!more)
                                        break;
                                    Entity entity = new Entity(EntityFlag.Monster, false);
                                    entity.MapObjType = MapObjectType.Monster;
                                    entity.MonsterInfo = mt.Copy();
                                    entity.MonsterInfo.Owner = entity;
                                    entity.Name = mt.Name;
                                    entity.MinAttack = mt.MinAttack;
                                    entity.MaxAttack = entity.MagicAttack = mt.MaxAttack;
                                    entity.Hitpoints = entity.MaxHitpoints = mt.Hitpoints;
                                    entity.Defence = mt.Defence;
                                    entity.Body = mt.Mesh;
                                    entity.Level = mt.Level;
                                    entity.UID = EntityUIDCounter.Next;
                                    entity.MapID = ID;
                                    entity.SendUpdates = true;
                                    // Network.Writer.WriteUInt32(112259, 44 + 4, entity.SpawnPacket);// head.
                                    // New Syle Guard Stop Now
                                    #region Guard
                                    if (mt.Type == 5)
                                    {//                                        
                                        entity.UID += 0xc3500 + 0xc3500;
                                        entity.AddFlag(Update.Flags.TopTrojan);
                                        //_String str = new _String(true);
                                        //str.Texts.Add("break_start");
                                        //str.Texts.Add("trojanpk_second");
                                        //Effect.break_start(client);
                                        //entity.Owner.SendScreen(str, true);
                                        // Network.Writer.WriteUInt32(200427, 72 + 4, entity.SpawnPacket);//steed
                                        // Network.Writer.WriteUInt32(112259, 44 + 4, entity.SpawnPacket);// head.
                                        //Network.Writer.WriteUInt16((byte)Enums.Color.Black, 139 + 4, entity.SpawnPacket);//  head color.
                                        // Network.Writer.WriteUInt32(135259, 52 + 4, entity.SpawnPacket);// Armor.
                                        //Network.Writer.WriteUInt32(822053, 200 + 4, entity.SpawnPacket);//ArmorSoul
                                        //Network.Writer.WriteUInt16((byte)Enums.Color.Black, 139 + 4, entity.SpawnPacket);// Armor color.
                                        Network.Writer.WriteUInt32(1364459535, 227 + 4, entity.SpawnPacket);//flags
                                        // Network.Writer.WriteString(mt.gui_type, 241, entity.SpawnPacket);//
                                        //Network.Writer.WriteUInt32(3000, 100 + 4, entity.SpawnPacket);//act
                                        Network.Writer.WriteUInt32(192565, 48 + 4, entity.SpawnPacket);//Garment
                                        Network.Writer.WriteUInt32(614439, 60 + 4, entity.SpawnPacket);// right wep.
                                        Network.Writer.WriteUInt32(614439, 56 + 4, entity.SpawnPacket);//left wep.
                                        // Network.Writer.WriteUInt32(360149, 68 + 4, entity.SpawnPacket);//RightWeaponAccessor
                                        // Network.Writer.WriteUInt32(360149, 64 + 4, entity.SpawnPacket);//LeftWeaponAccessoriy
                                        Network.Writer.WriteUInt64(200492, 76 + 4, entity.SpawnPacket);//MountArmor
                                        Network.Writer.WriteUInt32(535, 98 + 4, entity.SpawnPacket);//Hair
                                    }
                                    #endregion
                                    entity.X = (ushort)(X + Kernel.Random.Next(0, XPlus));
                                    entity.Y = (ushort)(Y + Kernel.Random.Next(0, YPlus));
                                    for (int count2 = 0; count2 < 50; count2++)
                                    {
                                        if (!Floor[entity.X, entity.Y, MapObjectType.Monster, null])
                                        {
                                            entity.X = (ushort)(X + Kernel.Random.Next(0, XPlus));
                                            entity.Y = (ushort)(Y + Kernel.Random.Next(0, YPlus));
                                            if (count2 == 50)
                                                more = false;
                                        }
                                        else
                                            break;
                                    }
                                    if (more)
                                    {
                                        if (Floor[entity.X, entity.Y, MapObjectType.Monster, null])
                                        {
                                            mycount++;
                                            if (mt.Type == 5)
                                            {
                                                this.AddEntity2(entity);
                                            }
                                            else
                                            {
                                                AddEntity(entity);

                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception e) { Program.SaveException(e); }
                    if (mycount != 0)
                        Timer = MonsterTimers.Add(this);
                }
            }
        }

        public Tuple<ushort, ushort> RandomCoordinates()
        {
            int times = 10000;
            int x = Kernel.Random.Next(Floor.Bounds.Width), y = Kernel.Random.Next(Floor.Bounds.Height);
            while (times-- > 0)
            {
                if (!Floor[x, y, MapObjectType.Player, null])
                {
                    x = Kernel.Random.Next(Floor.Bounds.Width);
                    y = Kernel.Random.Next(Floor.Bounds.Height);
                }
                else break;
            }
            return new Tuple<ushort, ushort>((ushort)x, (ushort)y);
        }
        public Tuple<ushort, ushort> RandomCoordinates(int _x, int _y, int radius)
        {
            int times = 10000;
            int x = _x + Kernel.Random.Sign() * Kernel.Random.Next(radius),
                y = _y + Kernel.Random.Sign() * Kernel.Random.Next(radius);
            while (times-- > 0)
            {
                if (!Floor[x, y, MapObjectType.Player, null])
                {
                    x = _x + Kernel.Random.Sign() * Kernel.Random.Next(radius);
                    y = _y + Kernel.Random.Sign() * Kernel.Random.Next(radius);
                }
                else break;
            }
            return new Tuple<ushort, ushort>((ushort)x, (ushort)y);
        }

        private static TimerRule<Map> MonsterTimers;
        public static void CreateTimerFactories()
        {
            MonsterTimers = new TimerRule<Map>(_timerCallBack, 500);
        }


        public Time32 LastReload = Time32.Now;
        private static void _timerCallBack(Map map, int time)
        {
            /* foreach (Entity monster in map.Companions.Values)
             {
                 if (!monster.Owner.Socket.Alive)
                 {
                     map.RemoveEntity(monster);
                     break;
                 }
             }*/
            Time32 Now = new Time32(time);
            foreach (Entity monster in map.Entities.Values)
            {
                if (monster.Dead)
                {
                    if (Now > monster.DeathStamp.AddSeconds(monster.MonsterInfo.RespawnTime))
                    {
                        monster.X = (ushort)(monster.MonsterInfo.BoundX + Kernel.Random.Next(0, monster.MonsterInfo.BoundCX));
                        monster.Y = (ushort)(monster.MonsterInfo.BoundY + Kernel.Random.Next(0, monster.MonsterInfo.BoundCY));
                        for (int count = 0; count < monster.MonsterInfo.BoundCX * monster.MonsterInfo.BoundCY; count++)
                        {
                            if (!map.Floor[monster.X, monster.Y, MapObjectType.Monster, null])
                            {
                                monster.X = (ushort)(monster.MonsterInfo.BoundX + Kernel.Random.Next(0, monster.MonsterInfo.BoundCX));
                                monster.Y = (ushort)(monster.MonsterInfo.BoundY + Kernel.Random.Next(0, monster.MonsterInfo.BoundCY));
                            }
                            else
                                break;
                        }
                        if (map.Floor[monster.X, monster.Y, MapObjectType.Monster, null] || monster.X == monster.MonsterInfo.BoundX && monster.Y == monster.MonsterInfo.BoundY)
                        {
                            monster.Hitpoints = monster.MonsterInfo.Hitpoints;
                            monster.RemoveFlag(monster.StatusFlag);
                            Network.GamePackets._String stringPacket = new Conquer_Online_Server.Network.GamePackets._String(true);
                            stringPacket.UID = monster.UID;
                            stringPacket.Type = Network.GamePackets._String.Effect;
                            stringPacket.Texts.Add("MBStandard");
                            monster.StatusFlag = 0;
                            foreach (var client in Program.GamePool)
                    if (client.Entity.MapID == 6000 || client.Entity.MapID == 6001 || client.Entity.MapID == 6002 || client.Entity.MapID == 6003 || client.Entity.MapID == 6004)
                                    return;
                            {
                                if (monster.Body == 979 && monster.MapID == 3220)
                                {
                                    Network.GamePackets.FloorItem floorItem = new Network.GamePackets.FloorItem(true);
                                    floorItem.UID = 7;
                                    floorItem.Type = 0xd;
                                    foreach (var client in Program.GamePool)
                                        client.Send(floorItem);
                                    Conquer_Online_Server.Kernel.SendWorldMessage(new Conquer_Online_Server.Network.GamePackets.Message("Oh,shit SnowDemon came back and he want the Bahaa go kill him fast", System.Drawing.Color.White, Conquer_Online_Server.Network.GamePackets.Message.Center), Program.GamePool);
                                    foreach (var client in Program.GamePool)
                                        client.MessageBox("SnowDemon Has Realsed! Would you like to Kill Him?",
                                       (p) => { p.Entity.Teleport(3220, 45, 45); }, null, 60);
                                }
                                if (monster.Body == 950 && monster.MapID == 1015)
                                {
                                    Conquer_Online_Server.Kernel.SendWorldMessage(new Conquer_Online_Server.Network.GamePackets.Message("TeratoDragon has apeared in BirdIsnalnd,  Who will Defeat it !", System.Drawing.Color.White, Conquer_Online_Server.Network.GamePackets.Message.Center), Program.GamePool);
                                }
                                if (monster.Body == 950 && monster.MapID == 7007)
                                {
                                    Conquer_Online_Server.Kernel.SendWorldMessage(new Conquer_Online_Server.Network.GamePackets.Message("TeratoDragon has apeared in BirdIsnalnd,  Who will Defeat it !", System.Drawing.Color.White, Conquer_Online_Server.Network.GamePackets.Message.Center), Program.GamePool);
                                }
                                if (monster.Body == 951 && monster.MapID == 1015)
                                {
                                    Network.GamePackets.FloorItem floorItem = new Network.GamePackets.FloorItem(true);
                                    floorItem.UID = 7;
                                    floorItem.Type = 0xd;
                                    foreach (var client in Program.GamePool)
                                        client.Send(floorItem);
                                    Conquer_Online_Server.Kernel.SendWorldMessage(new Conquer_Online_Server.Network.GamePackets.Message("Oh,shit SnowBanchee came back and he want the MrBahaa go kill him fast", System.Drawing.Color.White, Conquer_Online_Server.Network.GamePackets.Message.Center), Program.GamePool);
                                    foreach (var client in Program.GamePool)
                                        client.MessageBox("SnowBanchee Has Realsed! Would you like to Kill Him?",
                                             (p) => { p.Entity.Teleport(1015, 796, 575); }, null, 60);
                                }
                                if (monster.Body == 952 && monster.MapID == 4025)
                                {
                                    Network.GamePackets.FloorItem floorItem = new Network.GamePackets.FloorItem(true);
                                    floorItem.UID = 7;
                                    floorItem.Type = 0xd;
                                    foreach (var client in Program.GamePool)
                                        client.Send(floorItem);
                                    Conquer_Online_Server.Kernel.SendWorldMessage(new Conquer_Online_Server.Network.GamePackets.Message("Oh,shit ThrillingSpook came back and he want the MrBahaa go kill him fast", System.Drawing.Color.White, Conquer_Online_Server.Network.GamePackets.Message.Center), Program.GamePool);
                                    foreach (var client in Program.GamePool)
                                        client.MessageBox("ThrilingSpook Has Realsed! Would you like to Kill Him?",
                                             (p) => { p.Entity.Teleport(4025, 150, 150); }, null, 60);
                                }
                                if (monster.Body == 386 && monster.MapID == 1249)
                                {
                                    Network.GamePackets.FloorItem floorItem = new Network.GamePackets.FloorItem(true);
                                    floorItem.UID = 7;
                                    floorItem.Type = 0xd;
                                    foreach (var client in Program.GamePool)
                                        client.Send(floorItem);
                                    Conquer_Online_Server.Kernel.SendWorldMessage(new Conquer_Online_Server.Network.GamePackets.Message("Oh,shit LordTiger came back and he want the MrBahaa go kill him fast", System.Drawing.Color.White, Conquer_Online_Server.Network.GamePackets.Message.Center), Program.GamePool);
                                    foreach (var client in Program.GamePool)
                                        client.MessageBox("LordTiger Has Realsed! Would you like to Kill Him?",
                                             (p) => { p.Entity.Teleport(1249, 50, 50); }, null, 60);
                                }
                                foreach (Client.GameClient client in Program.GamePool)
                                {
                                    if (client.Map.ID == map.ID)
                                    {
                                        if (Kernel.GetDistance(client.Entity.X, client.Entity.Y, monster.X, monster.Y) < Constants.nScreenDistance)
                                        {
                                            monster.CauseOfDeathIsMagic = false;
                                            monster.SendSpawn(client, false);
                                            client.Send(stringPacket);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (monster.ToxicFogLeft > 0)
                    {
                        if (monster.MonsterInfo.Boss)
                        {
                            monster.ToxicFogLeft = 0;
                            continue;
                        }
                        if (Now > monster.ToxicFogStamp.AddSeconds(2))
                        {
                            monster.ToxicFogLeft--;
                            monster.ToxicFogStamp = Now;
                            if (monster.Hitpoints > 1)
                            {
                                uint damage = Game.Attacking.Calculate.Percent(monster, monster.ToxicFogPercent);
                                monster.Hitpoints -= damage;
                                Network.GamePackets.SpellUse suse = new Conquer_Online_Server.Network.GamePackets.SpellUse(true);
                                suse.Attacker = monster.UID;
                                suse.SpellID = 10010;
                                suse.AddTarget(monster.UID, damage, null);
                                monster.MonsterInfo.SendScreen(suse);
                            }
                        }
                    }
                }
            }
        }
        public void SpawnMonsterNearToHero(Database.MonsterInformation mt, Client.GameClient client)
        {
            if (mt == null) return;
            mt.RespawnTime = 5;
            Conquer_Online_Server.Game.Entity entity = new Conquer_Online_Server.Game.Entity(EntityFlag.Monster, false);
            entity.MapObjType = Conquer_Online_Server.Game.MapObjectType.Monster;
            entity.MonsterInfo = mt.Copy();
            entity.MonsterInfo.Owner = entity;
            entity.Name = mt.Name;
            entity.MinAttack = mt.MinAttack;
            entity.MaxAttack = entity.MagicAttack = mt.MaxAttack;
            entity.Hitpoints = entity.MaxHitpoints = mt.Hitpoints;
            entity.Body = mt.Mesh;
            entity.Level = mt.Level;
            entity.UID = EntityUIDCounter.Next;
            entity.MapID = ID;
            entity.SendUpdates = true;
            entity.X = (ushort)(client.Entity.X + Kernel.Random.Next(5));
            entity.Y = (ushort)(client.Entity.Y + Kernel.Random.Next(5));
            AddEntity(entity);
            entity.SendSpawn(client);
        }
        public void Spawnthis(Database.MonsterInformation mt, Client.GameClient client, ushort ID, ushort x, ushort y)
        {
            if (mt == null) return;
            mt.RespawnTime = 5;
            Entity entity = new Entity(EntityFlag.Monster, false);
            entity.MapObjType = MapObjectType.Monster;
            entity.MonsterInfo = mt.Copy();
            entity.MonsterInfo.Owner = entity;
            entity.Name = mt.Name;
            entity.MinAttack = mt.MinAttack;
            entity.MaxAttack = entity.MagicAttack = mt.MaxAttack;
            entity.Hitpoints = entity.MaxHitpoints = mt.Hitpoints;
            entity.Body = mt.Mesh;
            entity.Level = mt.Level;
            entity.UID = EntityUIDCounter.Next;
            entity.MapID = ID;
            entity.X = x;
            entity.Y = y;
            entity.SendUpdates = true;
            AddEntity(entity);
            entity.SendSpawn(client);
        }
        public Map MakeDynamicMap()
        {
            ushort id = (ushort)DynamicIDs.Next;
            Map myDynamic = new Map(id, this.ID, this.Path);
            return myDynamic;
        }
        bool disposed = false;
        public void Dispose()
        {
            if (!disposed)
                Kernel.Maps.Remove(ID);

            disposed = true;
        }

        public void RemoveNpc(INpc npc)
        {
            if (Npcs.ContainsKey(npc.UID))
            {
                Npcs.Remove(npc.UID);
                #region Setting the near coords invalid to avoid unpickable items.
                Floor[npc.X, npc.Y, MapObjectType.InvalidCast, null] = true;
                if (npc.Mesh / 10 != 108 && (byte)npc.Type < 10)
                {
                    ushort X = npc.X, Y = npc.Y;
                    foreach (Enums.ConquerAngle angle in Angles)
                    {
                        ushort xX = X, yY = Y;
                        UpdateCoordonatesForAngle(ref xX, ref yY, angle);
                        Floor[xX, yY, MapObjectType.InvalidCast, null] = true;
                    }
                }
                #endregion
            }
        }
    }
    public class Floor
    {
        [Flags]
        public enum DMapPointFlag : byte
        {
            Invalid = 1,
            Monster = 2,
            Item = 4,
            RaceItem = 8
        }
        public class Size
        {
            public int Width, Height;
            public Size(int width, int height)
            {
                Width = width;
                Height = height;
            }
            public Size()
            {
                Width = 0;
                Height = 0;
            }
        }
        public Size Bounds;
        public DMapPointFlag[,] Locations;
        public uint FloorMapID;
        public Floor(int width, int height, uint mapID)
        {
            FloorMapID = mapID;
            Bounds = new Size(width, height);
            Locations = new DMapPointFlag[width, height];
        }
        public unsafe bool this[int x, int y, MapObjectType type, object obj = null]
        {
            get
            {
                if (y >= Bounds.Height || x >= Bounds.Width || x < 0 || y < 0)
                    return false;

                DMapPointFlag filltype = Locations[x, y];

                if (type == MapObjectType.InvalidCast) return (filltype & DMapPointFlag.Invalid) == DMapPointFlag.Invalid;
                if ((filltype & DMapPointFlag.Invalid) == DMapPointFlag.Invalid) return false;
                if (type == MapObjectType.Player) return true;
                else if (type == MapObjectType.Monster)
                    return (filltype & DMapPointFlag.Monster) != DMapPointFlag.Monster;
                else if (type == MapObjectType.Item)
                    return (filltype & DMapPointFlag.Item) != DMapPointFlag.Item;
                else if (type == MapObjectType.StaticEntity)
                    return (filltype & DMapPointFlag.RaceItem) != DMapPointFlag.RaceItem;
                return false;
            }
            set
            {
                if (y >= Bounds.Height || x >= Bounds.Width || x < 0 || y < 0)
                    return;
                DMapPointFlag filltype = Locations[x, y];

                if (value)
                {
                    if (type == MapObjectType.InvalidCast)
                        TakeFlag(x, y, DMapPointFlag.Invalid);
                    if (type == MapObjectType.Item)
                        TakeFlag(x, y, DMapPointFlag.Item);
                    if (type == MapObjectType.Monster)
                        TakeFlag(x, y, DMapPointFlag.Monster);
                    if (type == MapObjectType.StaticEntity)
                        TakeFlag(x, y, DMapPointFlag.RaceItem);
                }
                else
                {
                    if (type == MapObjectType.InvalidCast)
                        AddFlag(x, y, DMapPointFlag.Invalid);
                    if (type == MapObjectType.Item)
                        AddFlag(x, y, DMapPointFlag.Item);
                    if (type == MapObjectType.Monster)
                        AddFlag(x, y, DMapPointFlag.Monster);
                    if (type == MapObjectType.StaticEntity)
                        AddFlag(x, y, DMapPointFlag.RaceItem);
                }
            }
        }
        public DMapPointFlag AddFlag(int x, int y, DMapPointFlag extraFlag)
        {
            Locations[x, y] |= extraFlag;
            return Locations[x, y];
        }
        public DMapPointFlag TakeFlag(int x, int y, DMapPointFlag extraFlag)
        {
            Locations[x, y] &= ~extraFlag;
            return Locations[x, y];
        }
    }
    public enum MapObjectType
    {
        SobNpc, Npc, Item, Monster, Player, Nothing, InvalidCast, StaticEntity
    }
    public class Portal
    {
        public Portal(ushort CurrentMapID, ushort CurrentX, ushort CurrentY, ushort DestinationMapID, ushort DestinationX, ushort DestinationY)
        {
            this.CurrentMapID = CurrentMapID;
            this.CurrentX = CurrentX;
            this.CurrentY = CurrentY;
            this.DestinationMapID = DestinationMapID;
            this.DestinationX = DestinationX;
            this.DestinationY = DestinationY;
        }
        public Portal()
        {

        }
        public ushort CurrentMapID
        {
            get;
            set;
        }
        public ushort CurrentX
        {
            get;
            set;
        }
        public ushort CurrentY
        {
            get;
            set;
        }
        public ushort DestinationMapID
        {
            get;
            set;
        }
        public ushort DestinationX
        {
            get;
            set;
        }
        public ushort DestinationY
        {
            get;
            set;
        }
    }
}