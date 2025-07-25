﻿using System;
using System.Linq;
using System.Threading;
using System.Net.Sockets;
using System.Collections.Generic;
using Conquer_Online_Server.Network.Cryptography;
using Conquer_Online_Server.Network.GamePackets;
using Conquer_Online_Server.Network.Sockets;
using Conquer_Online_Server.Network;
using Conquer_Online_Server.Interfaces;
using Conquer_Online_Server.Game.ConquerStructures;
using System.Drawing;
using Conquer_Online_Server.Game;


namespace Conquer_Online_Server.Client
{
    public class GameClient
    {
        public SafeDictionary<uint, Conquer_Online_Server.MaTrix.Way2Heroes.StageInfo> Way2Heroes = new SafeDictionary<uint, Conquer_Online_Server.MaTrix.Way2Heroes.StageInfo>(1000);
        public IDisposable[] TimerSubscriptions;
        public object TimerSyncRoot, ItemSyncRoot;
        public Time32 LastVIPTeleport, LastVIPTeamTeleport;
        public bool AlternateEquipment;
        private ClientWrapper _socket;
        public Database.AccountTable Account;
        public GameCryptography Cryptography;
        public DHKeyExchange.ServerKeyExchange DHKeyExchange;
        public bool Exchange = true;
        public ConcurrentPacketQueue Queue;
        public PacketFilter PacketFilter;
        public Time32 CantAttack = Time32.Now;
        public bool Filtering = false;
        public string Country = "";
        public Network.GamePackets.Interaction Interaction;
        public int quarantineKill = 0;
        public int quarantineDeath = 0;
        public int TopDlClaim = 0;
        public int TopGlClaim = 0;
        public bool IsFairy = false;
        public uint FairyType = 0;
        public uint SType = 0;
        public Action OnMessageBoxOK;
        public Action OnMessageBoxCANCEL;
        public bool SpiltStack = false;
        public Action<GameClient> OnDisconnect;
        public int apprtnum = 0;
        public Game.Enums.Color staticArmorColor;
        public bool JustCreated = false;
        public Timer Timer;
        public Conquer_Online_Server.Game.Features.SpiritBeadQuest SpiritBeadQ;
        #region Network

        public GameClient(ClientWrapper socket)
        {
            Fake = socket == null;
            if (Fake) socket = new ClientWrapper() { Alive = true };
            Queue = new ConcurrentPacketQueue();
            PacketFilter = new PacketFilter() { { 10010, 10 }, { 10005, 7 }, { 2064, 4 }, { 2032, 3 }, { 1027, 2 } };
            Attackable = false;
            Action = 0;
            _socket = socket;
            Cryptography = new GameCryptography(System.Text.Encoding.Default.GetBytes(Constants.GameCryptographyKey));
            DHKeyExchange = new Network.GamePackets.DHKeyExchange.ServerKeyExchange();
            SpiritBeadQ = new Game.Features.SpiritBeadQuest(this);
            ChiPowers = new List<ChiPowerStructure>();
        }
        public void ReadyToPlay()
        {
            try
            {
                Weapons = new Tuple<ConquerItem, ConquerItem>(null, null);
                ItemSyncRoot = new object();
                Screen = new Game.Screen(this);
                Inventory = new Game.ConquerStructures.Inventory(this);
                Equipment = new Game.ConquerStructures.Equipment(this);
                WarehouseOpen = false;
                WarehouseOpenTries = 0;
                TempPassword = "";
                ArsenalDonations = new uint[10];
                Warehouses = new SafeDictionary<Game.ConquerStructures.Warehouse.WarehouseID, Game.ConquerStructures.Warehouse>(20);
                Warehouses.Add(Conquer_Online_Server.Game.ConquerStructures.Warehouse.WarehouseID.TwinCity, new Game.ConquerStructures.Warehouse(this, Conquer_Online_Server.Game.ConquerStructures.Warehouse.WarehouseID.TwinCity));
                Warehouses.Add(Conquer_Online_Server.Game.ConquerStructures.Warehouse.WarehouseID.PhoenixCity, new Game.ConquerStructures.Warehouse(this, Conquer_Online_Server.Game.ConquerStructures.Warehouse.WarehouseID.PhoenixCity));
                Warehouses.Add(Conquer_Online_Server.Game.ConquerStructures.Warehouse.WarehouseID.ApeCity, new Game.ConquerStructures.Warehouse(this, Conquer_Online_Server.Game.ConquerStructures.Warehouse.WarehouseID.ApeCity));
                Warehouses.Add(Conquer_Online_Server.Game.ConquerStructures.Warehouse.WarehouseID.DesertCity, new Game.ConquerStructures.Warehouse(this, Conquer_Online_Server.Game.ConquerStructures.Warehouse.WarehouseID.DesertCity));
                Warehouses.Add(Conquer_Online_Server.Game.ConquerStructures.Warehouse.WarehouseID.BirdCity, new Game.ConquerStructures.Warehouse(this, Conquer_Online_Server.Game.ConquerStructures.Warehouse.WarehouseID.BirdCity));
                Warehouses.Add(Conquer_Online_Server.Game.ConquerStructures.Warehouse.WarehouseID.StoneCity, new Game.ConquerStructures.Warehouse(this, Conquer_Online_Server.Game.ConquerStructures.Warehouse.WarehouseID.StoneCity));
                Warehouses.Add(Conquer_Online_Server.Game.ConquerStructures.Warehouse.WarehouseID.Market, new Game.ConquerStructures.Warehouse(this, Conquer_Online_Server.Game.ConquerStructures.Warehouse.WarehouseID.Market));
                Trade = new Game.ConquerStructures.Trade();
                ArenaStatistic = new ArenaStatistic(true);
                Prayers = new List<GameClient>();
                map = null;
                SpiritBeadQ = new Game.Features.SpiritBeadQuest(this);
            }
            catch (Exception e)
            {
                Program.SaveException(e);
            }
        }
        public void Send(byte[] buffer)
        {
            if (Fake) return;
            if (!_socket.Alive) return;
            ushort length = BitConverter.ToUInt16(buffer, 0);
            if (length >= 1024 && buffer.Length > length)
            {
                Console.WriteLine(Environment.StackTrace);
                return;
            }
            byte[] _buffer = new byte[buffer.Length];
            if (length == 0)
                Writer.WriteUInt16((ushort)(buffer.Length - 8), 0, buffer);
            Buffer.BlockCopy(buffer, 0, _buffer, 0, buffer.Length);
            Network.Writer.WriteString(Constants.ServerKey, _buffer.Length - 8, _buffer);
            try
            {
                lock (_socket)
                {
                    if (!_socket.Alive) return;
                    lock (Cryptography)
                    {
                        Cryptography.Encrypt(_buffer, _buffer.Length);
                        _socket.Send(_buffer);
                    }
                }
            }
            catch (Exception)
            {
                _socket.Alive = false;
                Disconnect();
            }
        }
        private void EndSend(IAsyncResult res)
        {
            try
            {
                _socket.Socket.EndSend(res);
            }
            catch
            {
                _socket.Alive = false;
                Disconnect();
            }
        }
        public void Send(Interfaces.IPacket buffer)
        {
            Send(buffer.ToArray());
        }
        public void SendScreenSpawn(Interfaces.IMapObject obj, bool self)
        {
            try
            {
                foreach (Interfaces.IMapObject _obj in Screen.Objects)
                {
                    if (_obj == null)
                        continue;
                    if (_obj.UID != Entity.UID)
                    {
                        if (_obj.MapObjType == Game.MapObjectType.Player)
                        {
                            GameClient client = _obj.Owner as GameClient;
                            obj.SendSpawn(client, false);
                        }
                    }
                }
                if (self)
                    obj.SendSpawn(this);
            }
            catch (Exception e)
            {
                Program.SaveException(e);
            }
        }
        public static Dictionary<uint, GameClient> BoothingAI = new Dictionary<uint, GameClient>();
        public void FakeLoadx(uint UID)
        {
            if (!Kernel.GamePool.ContainsKey(UID))
            {
                ReadyToPlay();
                this.Account = new Database.AccountTable(null);
                this.Account.EntityID = UID;
                //   if (Database.EntityTable.LoadEntity(this))
                {
                    #region Load Entity
                    Conquer_Online_Server.Database.MySqlCommand command = new Conquer_Online_Server.Database.MySqlCommand(Database.MySqlCommandType.SELECT);
                    command.Select("bots").Where("BotID", (long)UID);
                    Database.MySqlReader reader = new Database.MySqlReader(command);
                    if (!reader.Read())
                    {
                        return;
                    }
                    this.Entity = new Conquer_Online_Server.Game.Entity(EntityFlag.Player, false);
                    this.Entity.Name = reader.ReadString("BotName");
                    this.Entity.Owner = this;
                    this.Entity.UID = UID;
                    this.Entity.Body = reader.ReadUInt16("BotBody");
                    this.Entity.Face = reader.ReadUInt16("BotFace");
                    this.Entity.HairStyle = reader.ReadUInt16("BotHairStyle");
                    this.Entity.Level = reader.ReadByte("BotLevel");
                    this.Entity.Class = reader.ReadByte("BotClass");
                    this.Entity.Reborn = reader.ReadByte("BotReborns");
                    this.Entity.Titles = new System.Collections.Concurrent.ConcurrentDictionary<TitlePacket.Titles, DateTime>();
                    this.Entity.MyTitle = (TitlePacket.Titles)reader.ReadUInt32("BotTitle");
                    this.Entity.MapID = reader.ReadUInt16("BotMap");
                    if (this.VendingDisguise == 0)
                        this.VendingDisguise = 0xdf;
                    this.Entity.X = reader.ReadUInt16("BotMapx");
                    this.Entity.Y = reader.ReadUInt16("BotMapy");
                    uint WeaponR = reader.ReadUInt32("BotWeaponR");
                    uint WeaponL = reader.ReadUInt32("BotWeaponL");
                    uint Armor = reader.ReadUInt32("BotArmor");
                    uint Head = reader.ReadUInt32("BotHead");
                    uint Garment = reader.ReadUInt32("BotGarment");

                    //ProjectX_V3_Game.Entities.BotType Type = (ProjectX_V3_Game.Entities.BotType)reader.ReadInt32("BotType");
                    string hawkmessage = reader.ReadString("BotMessage");

                    int count = reader.ReadInt32("BItemCount");
                    uint Cost = reader.ReadUInt32("BItemCost");
                    uint ID = reader.ReadUInt32("BItemID");
                    byte Plus = reader.ReadByte("BItemPlus");
                    byte Enchant = reader.ReadByte("BItemEnchant");
                    byte Bless = reader.ReadByte("BItemBless");
                    byte SocketOne = reader.ReadByte("BItemSoc1");
                    byte SocketTwo = reader.ReadByte("BItemSoc2");

                    this.ElitePKStats = new ElitePK.FighterStats(this.Entity.UID, this.Entity.Name, this.Entity.Mesh);
                    if (!Conquer_Online_Server.Game.ConquerStructures.Nobility.Board.TryGetValue(this.Entity.UID, out this.NobilityInformation))
                    {
                        this.NobilityInformation = new NobilityInformation();
                        this.NobilityInformation.EntityUID = this.Entity.UID;
                        this.NobilityInformation.Name = this.Entity.Name;
                        this.NobilityInformation.Donation = 0L;
                        this.NobilityInformation.Rank = NobilityRank.Serf;
                        this.NobilityInformation.Position = -1;
                        this.NobilityInformation.Gender = 1;
                        this.NobilityInformation.Mesh = this.Entity.Mesh;
                        if ((this.Entity.Body % 10) >= 3)
                        {
                            this.NobilityInformation.Gender = 0;
                        }
                    }
                    else
                    {
                        this.Entity.NobilityRank = this.NobilityInformation.Rank;
                    }
                    Arena.ArenaStatistics.TryGetValue(this.Entity.UID, out this.ArenaStatistic);
                    if ((this.ArenaStatistic == null) || (this.ArenaStatistic.EntityID == 0))
                    {
                        this.ArenaStatistic = new ArenaStatistic(true);
                        this.ArenaStatistic.EntityID = this.Entity.UID;
                        this.ArenaStatistic.Name = this.Entity.Name;
                        this.ArenaStatistic.Level = this.Entity.Level;
                        this.ArenaStatistic.Class = this.Entity.Class;
                        this.ArenaStatistic.Model = this.Entity.Mesh;
                        this.ArenaStatistic.ArenaPoints = Database.ArenaTable.ArenaPointFill(this.Entity.Level);
                        this.ArenaStatistic.LastArenaPointFill = DateTime.Now;
                        Database.ArenaTable.InsertArenaStatistic(this);
                        this.ArenaStatistic.Status = 0;
                        Arena.ArenaStatistics.Add(this.Entity.UID, this.ArenaStatistic);
                    }
                    else
                    {
                        this.ArenaStatistic.Level = this.Entity.Level;
                        this.ArenaStatistic.Class = this.Entity.Class;
                        this.ArenaStatistic.Model = this.Entity.Mesh;
                        if (DateTime.Now.DayOfYear != this.ArenaStatistic.LastArenaPointFill.DayOfYear)
                        {
                            this.ArenaStatistic.LastSeasonArenaPoints = this.ArenaStatistic.ArenaPoints;
                            this.ArenaStatistic.LastSeasonWin = this.ArenaStatistic.TodayWin;
                            this.ArenaStatistic.LastSeasonLose = this.ArenaStatistic.TodayBattles - this.ArenaStatistic.TodayWin;
                            this.ArenaStatistic.ArenaPoints = Database.ArenaTable.ArenaPointFill(this.Entity.Level);
                            this.ArenaStatistic.LastArenaPointFill = DateTime.Now;
                            this.ArenaStatistic.TodayWin = 0;
                            this.ArenaStatistic.TodayBattles = 0;
                            Arena.Sort();
                            Arena.YesterdaySort();
                        }
                    }
                    TeamArena.ArenaStatistics.TryGetValue(this.Entity.UID, out this.TeamArenaStatistic);
                    if (this.TeamArenaStatistic == null)
                    {
                        this.TeamArenaStatistic = new TeamArenaStatistic(true);
                        this.TeamArenaStatistic.EntityID = this.Entity.UID;
                        this.TeamArenaStatistic.Name = this.Entity.Name;
                        this.TeamArenaStatistic.Level = this.Entity.Level;
                        this.TeamArenaStatistic.Class = this.Entity.Class;
                        this.TeamArenaStatistic.Model = this.Entity.Mesh;
                        Database.TeamArenaTable.InsertArenaStatistic(this);
                        this.TeamArenaStatistic.Status = 0;
                        if (TeamArena.ArenaStatistics.ContainsKey(this.Entity.UID))
                        {
                            TeamArena.ArenaStatistics.Remove(this.Entity.UID);
                        }
                        TeamArena.ArenaStatistics.Add(this.Entity.UID, this.TeamArenaStatistic);
                    }
                    else if (this.TeamArenaStatistic.EntityID == 0)
                    {
                        this.TeamArenaStatistic = new TeamArenaStatistic(true);
                        this.TeamArenaStatistic.EntityID = this.Entity.UID;
                        this.TeamArenaStatistic.Name = this.Entity.Name;
                        this.TeamArenaStatistic.Level = this.Entity.Level;
                        this.TeamArenaStatistic.Class = this.Entity.Class;
                        this.TeamArenaStatistic.Model = this.Entity.Mesh;
                        Database.TeamArenaTable.InsertArenaStatistic(this);
                        this.TeamArenaStatistic.Status = 0;
                        if (TeamArena.ArenaStatistics.ContainsKey(this.Entity.UID))
                        {
                            TeamArena.ArenaStatistics.Remove(this.Entity.UID);
                        }
                        TeamArena.ArenaStatistics.Add(this.Entity.UID, this.TeamArenaStatistic);
                    }
                    else
                    {
                        this.TeamArenaStatistic.Level = this.Entity.Level;
                        this.TeamArenaStatistic.Class = this.Entity.Class;
                        this.TeamArenaStatistic.Model = this.Entity.Mesh;
                        this.TeamArenaStatistic.Name = this.Entity.Name;
                    }
                    #region Champion
                    Game.Champion.ChampionStats.TryGetValue(this.Entity.UID, out this.ChampionStats);
                    if (this.ChampionStats == null)
                    {
                        this.ChampionStats = new Conquer_Online_Server.Network.GamePackets.ChampionStatistic(true);
                        this.ChampionStats.UID = this.Entity.UID;
                        this.ChampionStats.Name = this.Entity.Name;
                        this.ChampionStats.Level = this.Entity.Level;
                        this.ChampionStats.Class = this.Entity.Class;
                        this.ChampionStats.Model = this.Entity.Mesh;
                        this.ChampionStats.Points = 0;
                        this.ChampionStats.LastReset = DateTime.Now;
                        Database.ChampionTable.InsertStatistic(this);
                        if (Game.Champion.ChampionStats.ContainsKey(this.Entity.UID))
                            Game.Champion.ChampionStats.Remove(this.Entity.UID);
                        Game.Champion.ChampionStats.Add(this.Entity.UID, this.ChampionStats);
                    }
                    else if (this.ChampionStats.UID == 0)
                    {
                        this.ChampionStats = new Network.GamePackets.ChampionStatistic(true);
                        this.ChampionStats.UID = this.Entity.UID;
                        this.ChampionStats.Name = this.Entity.Name;
                        this.ChampionStats.Level = this.Entity.Level;
                        this.ChampionStats.Class = this.Entity.Class;
                        this.ChampionStats.Model = this.Entity.Mesh;
                        this.ChampionStats.Points = 0;
                        this.ChampionStats.LastReset = DateTime.Now;
                        Database.ArenaTable.InsertArenaStatistic(this);
                        this.ArenaStatistic.Status = Network.GamePackets.ArenaStatistic.NotSignedUp;
                        if (Game.Champion.ChampionStats.ContainsKey(this.Entity.UID))
                            Game.Champion.ChampionStats.Remove(this.Entity.UID);
                        Game.Champion.ChampionStats.Add(this.Entity.UID, this.ChampionStats);
                    }
                    else
                    {
                        this.ChampionStats.Level = this.Entity.Level;
                        this.ChampionStats.Class = this.Entity.Class;
                        this.ChampionStats.Model = this.Entity.Mesh;
                        this.ChampionStats.Name = this.Entity.Name;
                        if (this.ChampionStats.LastReset.DayOfYear != DateTime.Now.DayOfYear)
                            Database.ChampionTable.Reset(this.ChampionStats);
                    }
                    Game.Champion.Clear(this);
                    #endregion
                    Database.DetainedItemTable.LoadDetainedItems(this);
                    #region Champion     
                    Game.Champion.ChampionStats.TryGetValue(this.Entity.UID, out this.ChampionStats);
                    if (this.ChampionStats == null)
                    {
                        this.ChampionStats = new Conquer_Online_Server.Network.GamePackets.ChampionStatistic(true);
                        this.ChampionStats.UID = this.Entity.UID;
                        this.ChampionStats.Name = this.Entity.Name;
                        this.ChampionStats.Level = this.Entity.Level;
                        this.ChampionStats.Class = this.Entity.Class;
                        this.ChampionStats.Model = this.Entity.Mesh;
                        this.ChampionStats.Points = 0;
                        this.ChampionStats.LastReset = DateTime.Now;
                        Database.ChampionTable.InsertStatistic(this);
                        if (Game.Champion.ChampionStats.ContainsKey(this.Entity.UID))
                            Game.Champion.ChampionStats.Remove(this.Entity.UID);
                        Game.Champion.ChampionStats.Add(this.Entity.UID, this.ChampionStats);
                    }
                    else if (this.ChampionStats.UID == 0)
                    {
                        this.ChampionStats = new Network.GamePackets.ChampionStatistic(true);
                        this.ChampionStats.UID = this.Entity.UID;
                        this.ChampionStats.Name = this.Entity.Name;
                        this.ChampionStats.Level = this.Entity.Level;
                        this.ChampionStats.Class = this.Entity.Class;
                        this.ChampionStats.Model = this.Entity.Mesh;
                        this.ChampionStats.Points = 0;
                        this.ChampionStats.LastReset = DateTime.Now;
                        Database.ArenaTable.InsertArenaStatistic(this);
                        this.ArenaStatistic.Status = Network.GamePackets.ArenaStatistic.NotSignedUp;
                        if (Game.Champion.ChampionStats.ContainsKey(this.Entity.UID))
                            Game.Champion.ChampionStats.Remove(this.Entity.UID);
                        Game.Champion.ChampionStats.Add(this.Entity.UID, this.ChampionStats);
                    }
                    else
                    {
                        this.ChampionStats.Level = this.Entity.Level;
                        this.ChampionStats.Class = this.Entity.Class;
                        this.ChampionStats.Model = this.Entity.Mesh;
                        this.ChampionStats.Name = this.Entity.Name;
                        if (this.ChampionStats.LastReset.DayOfYear != DateTime.Now.DayOfYear)
                            Database.ChampionTable.Reset(this.ChampionStats);
                    }
                    #endregion
                    Game.Champion.Clear(this);

                    #endregion                    Database.ClaimItemTable.LoadClaimableItems(this);
                    this.Entity.LoadTopStatus();
                    this.Entity.FullyLoaded = true;

                    #endregion
                    if (this.Entity.FullyLoaded)
                    {
                        VariableVault variables;
                        Database.EntityVariableTable.Load(this.Account.EntityID, out variables);
                        this.Variables = variables;

                        if (this.BackupArmorLook != 0)
                            this.SetNewArmorLook(this.BackupArmorLook);
                        else
                            this.SetNewArmorLook(this.ArmorLook);
                        this.SetNewHeadgearLook(this.HeadgearLook);
                        this.BackupArmorLook = 0;

                        this.LoadData(true);

                        if (this.Entity.GuildID != 0)
                            this.Entity.GuildBattlePower = this.Guild.GetSharedBattlepower(this.Entity.GuildRank);
                        this.ReviewMentor();
                        Network.PacketHandler.LoginMessages(this);

                        #region Equip

                        ConquerItem item7 = null;
                        ClientEquip equip = null;
                        if (WeaponR > 0)
                        {
                            Database.ConquerItemBaseInformation CIBI = Database.ConquerItemInformation.BaseInformations[WeaponR];
                            if (CIBI == null) return;
                            item7 = new ConquerItem(true);
                            item7.ID = WeaponR;
                            item7.UID = AuthClient.nextID;
                            AuthClient.nextID++;
                            item7.Position = 5;
                            item7.Durability = CIBI.Durability;
                            item7.MaximDurability = CIBI.Durability;
                            this.Equipment.Remove(4);
                            if (this.Equipment.Objects[3] != null)
                            {
                                this.Equipment.Objects[3] = null;
                            }
                            this.Equipment.Add(item7);
                            item7.Mode = Enums.ItemMode.Update;
                            item7.Send(this);
                            equip = new ClientEquip();
                            equip.DoEquips(this);
                            this.Send(equip);
                            this.Equipment.UpdateEntityPacket();

                        }
                        if (WeaponL > 0)
                        {
                            Database.ConquerItemBaseInformation CIBI = Database.ConquerItemInformation.BaseInformations[WeaponL];
                            if (CIBI == null) return;
                            item7 = new ConquerItem(true);
                            item7.ID = WeaponL;
                            item7.UID = AuthClient.nextID;
                            AuthClient.nextID++;
                            item7.Position = 5;
                            item7.Durability = CIBI.Durability;
                            item7.MaximDurability = CIBI.Durability;
                            this.Equipment.Remove(5);
                            if (this.Equipment.Objects[4] != null)
                            {
                                this.Equipment.Objects[4] = null;
                            }
                            this.Equipment.Add(item7);
                            item7.Mode = Enums.ItemMode.Update;
                            item7.Send(this);
                            equip = new ClientEquip();
                            equip.DoEquips(this);
                            this.Send(equip);
                            this.Equipment.UpdateEntityPacket();
                        }

                        if (Armor > 0)
                        {
                            Database.ConquerItemBaseInformation CIBI = Database.ConquerItemInformation.BaseInformations[Armor];
                            if (CIBI == null) return;
                            item7 = new ConquerItem(true);
                            item7.ID = Armor;
                            item7.UID = AuthClient.nextID;
                            AuthClient.nextID++;
                            item7.Position = 3;
                            item7.Durability = CIBI.Durability;
                            item7.MaximDurability = CIBI.Durability;
                            this.Equipment.Remove(3);
                            if (this.Equipment.Objects[2] != null)
                            {
                                this.Equipment.Objects[2] = null;
                            }
                            this.Equipment.Add(item7);
                            item7.Mode = Enums.ItemMode.Update;
                            item7.Send(this);
                            equip = new ClientEquip();
                            equip.DoEquips(this);
                            this.Send(equip);
                            this.Equipment.UpdateEntityPacket();

                        }

                        if (Head > 0)
                        {
                            Database.ConquerItemBaseInformation CIBI = Database.ConquerItemInformation.BaseInformations[Head];
                            if (CIBI == null) return;
                            item7 = new ConquerItem(true);
                            item7.ID = Head;
                            item7.UID = AuthClient.nextID;
                            AuthClient.nextID++;
                            item7.Position = 1;
                            item7.Durability = CIBI.Durability;
                            item7.MaximDurability = CIBI.Durability;
                            this.Equipment.Remove(1);
                            if (this.Equipment.Objects[0] != null)
                            {
                                this.Equipment.Objects[0] = null;
                            }
                            this.Equipment.Add(item7);
                            item7.Mode = Enums.ItemMode.Update;
                            item7.Send(this);
                            equip = new ClientEquip();
                            equip.DoEquips(this);
                            this.Send(equip);
                            this.Equipment.UpdateEntityPacket();

                        }

                        if (Garment > 0)
                        {
                            Database.ConquerItemBaseInformation CIBI = Database.ConquerItemInformation.BaseInformations[Garment];
                            if (CIBI == null) return;
                            item7 = new ConquerItem(true);
                            item7.ID = Garment;
                            item7.UID = AuthClient.nextID;
                            AuthClient.nextID++;
                            item7.Position = 9;
                            item7.Durability = CIBI.Durability;
                            item7.MaximDurability = CIBI.Durability;
                            this.Equipment.Remove(9);
                            if (this.Equipment.Objects[8] != null)
                            {
                                this.Equipment.Objects[8] = null;
                            }
                            this.Equipment.Add(item7);
                            item7.Mode = Enums.ItemMode.Update;
                            item7.Send(this);
                            equip = new ClientEquip();
                            equip.DoEquips(this);
                            this.Send(equip);
                            this.Equipment.UpdateEntityPacket();
                        }

                        #endregion Equip


                        Program.World.Register(this);
                        Kernel.GamePool.Add(Entity.UID, this);
                        FakeLoaded = true;
                        LoggedIn = true;
                        Entity.NobilityRank = NobilityInformation.Rank;
                        //if (Type == ProjectX_V3_Game.Entities.BotType.BoothBot)
                        {
                            if (this.FakeLoaded)
                            {
                                #region booth

                                if (this.Booth == null)
                                {
                                    this.Send(new MapStatus() { BaseID = this.Map.BaseID, ID = this.Map.ID, Status = Database.MapsTable.MapInformations[1036].Status });
                                    this.Booth = new Game.ConquerStructures.Booth(this, new Data(true) { UID = this.Entity.UID });
                                    this.Send(new Data(true) { ID = Data.ChangeAction, UID = this.Entity.UID, dwParam = 0 });
                                    for (uint i = 0; i < count; i++)
                                    {
                                        Game.ConquerStructures.BoothItem item = new Game.ConquerStructures.BoothItem();
                                        item.Cost = Cost;
                                        item.Item = new ConquerItem(true);
                                        item.Item.ID = ID;
                                        item7.UID = AuthClient.nextID;
                                        AuthClient.nextID++;
                                        item.Item.Plus = Plus;
                                        item.Item.Enchant = Enchant;
                                        item.Item.Bless = Bless;
                                        item.Item.SocketOne = (Enums.Gem)SocketOne;
                                        item.Item.SocketTwo = (Enums.Gem)SocketOne;
                                        Database.ConquerItemBaseInformation CIBI = null;
                                        CIBI = Database.ConquerItemInformation.BaseInformations[item.Item.ID];
                                        if (CIBI == null)
                                            return;
                                        item.Item.Durability = CIBI.Durability;
                                        item.Item.MaximDurability = CIBI.Durability;
                                        this.Inventory.Add(item.Item, Game.Enums.ItemUse.CreateAndAdd);
                                        item.Item.Send(this);
                                        {
                                            ItemUsage usage = new ItemUsage(true) { ID = ItemUsage.AddItemOnBoothForConquerPoints };
                                            item.Cost_Type = Game.ConquerStructures.BoothItem.CostType.ConquerPoints;
                                            this.Booth.ItemList.Add(item.Item.UID, item);
                                            this.Send(usage);
                                            Conquer_Online_Server.Network.GamePackets.BoothItem buffer = new Conquer_Online_Server.Network.GamePackets.BoothItem(true);
                                            buffer.Fill(item, this.Booth.Base.UID);
                                            this.SendScreen(buffer, false);
                                        }
                                    }
                                    this.Booth.HawkMessage = new Message(hawkmessage, "ALL", this.Entity.Name, System.Drawing.Color.White, Message.HawkMessage);

                                }
                                #endregion
                            }
                        }

                    }

                }
            }
        }
        public static void LoadBoothingAI()
        {
            Database.MySqlCommand Cmd = new Database.MySqlCommand(Database.MySqlCommandType.SELECT);
            Cmd.Select("bots");
            Database.MySqlReader Reader = new Database.MySqlReader(Cmd);
            while (Reader.Read())
            {
                var ID = Reader.ReadUInt32("BotID");
                if (ID > 70000000)
                    if (ID > 70000005)
                        if (ID > 70000006)
                            if (ID > 70000007)
                                if (ID > 70000001)
                                    ID = (uint)Kernel.Random.Next(70000000, 999999999);
                var fClient = new GameClient(null);
                fClient.FakeLoadx(ID);
                BoothingAI.Add(ID, fClient);

            }
            Conquer_Online_Server.Console.WriteLine("" + BoothingAI.Count + " BoothingAI Loaded.");
        }
        public void RemoveScreenSpawn(Interfaces.IMapObject obj, bool self)
        {
            try
            {
                if (Screen == null) return;
                if (Screen.Objects == null) return;
                foreach (Interfaces.IMapObject _obj in Screen.Objects)
                {
                    if (_obj == null) continue;
                    if (obj == null) continue;
                    if (_obj.UID != Entity.UID)
                    {
                        if (_obj.MapObjType == Game.MapObjectType.Player)
                        {
                            GameClient client = _obj.Owner as GameClient;
                            client.Screen.Remove(obj);
                        }
                    }
                }
                if (self)
                    Screen.Remove(obj);
            }
            catch (Exception e)
            {
                Program.SaveException(e);
            }
        }
        public void SendScreen(byte[] buffer, bool self = true)
        {
            try
            {
                foreach (Interfaces.IMapObject obj in Screen.Objects)
                {
                    if (obj == null) continue;
                    if (obj.UID != Entity.UID)
                    {
                        if (obj.MapObjType == Game.MapObjectType.Player)
                        {
                            GameClient client = obj.Owner as GameClient;
                            if (WatchingGroup != null && client.WatchingGroup == null)
                                continue;
                            client.Send(buffer);
                        }
                    }
                }
                if (self)
                    Send(buffer);
            }
            catch (Exception e)
            {
                Program.SaveException(e);
            }
        }
        public void SendScreen(Interfaces.IPacket buffer, bool self = true)
        {
            foreach (Interfaces.IMapObject obj in Screen.Objects)
            {
                if (obj == null) continue;
                if (obj.MapObjType == Game.MapObjectType.Player)
                {
                    GameClient client = obj.Owner as GameClient;
                    if (client.Entity.UID != Entity.UID)
                        //if (WatchingGroup != null && client.WatchingGroup == null)
                        //   continue;
                        client.Send(buffer);
                }
            }
            if (self)
                Send(buffer);
        }
        public void Disconnect(bool save = true)
        {
            if (Fake) return;
            if (Screen != null) Screen.DisposeTimers();
            PacketHandler.RemoveTPA(this);
            Program.World.Unregister(this);
            if (OnDisconnect != null) OnDisconnect(this);
            if (_socket.Connector != null)
            {
                if (Entity != null)
                {
                    if (Entity.MyPokerTable != null)
                    {
                        if (Entity.MyPokerTable.Players.ContainsKey(Entity.UID) && Entity.MyPokerTable.Pot > 1)
                        {
                            byte[] P = new byte[10];
                            P[6] = 4; P[9] = 200;
                            Entity.MyPokerTable.NewPlayerMove(P, Entity.UID);
                        }
                        else
                            Entity.MyPokerTable.RemovePlayer(Entity.UID);
                    }
                }
                _socket.Disconnect();
                ShutDown();
            }
        }

        private void ShutDown()
        {

            if (Socket.Connector == null) return;
            Socket.Connector = null;
            if (this.Entity != null)
            {
                try
                {
                    if (this.Entity.JustCreated) return;
                    Time32 now = Time32.Now;
                    Kernel.DisconnectPool.Add(this.Entity.UID, this);
                    RemoveScreenSpawn(this.Entity, false);

                    using (var conn = Database.DataHolder.MySqlConnection)
                    {
                        conn.Open();
                        Database.EntityTable.UpdateOnlineStatus(this, false, conn);
                        Database.EntityTable.SaveEntity(this, conn);
                        Database.EntityVariableTable.Save(this, conn);
                        Database.SkillTable.SaveProficiencies(this, conn);
                        Database.SkillTable.SaveSpells(this, conn);
                        Database.FlowerSystemTable.SaveFlowerTable(this);
                        Database.KissesystemTable.SaveKissTable(this);
                        Database.ArenaTable.SaveArenaStatistics(this.ArenaStatistic, conn);
                        Database.JiangHu.SaveJiangHu();
                        Database.TeamArenaTable.SaveArenaStatistics(this.TeamArenaStatistic, conn);
                        Database.ChampionTable.SaveStatistics(this.ChampionStats, conn);
                        Database.NameChange.UpdateNames();
                    }
                    Kernel.GamePool.Remove(this.Entity.UID);
                    string name = Entity.Name;
                    string NewName = Entity.NewName;
                    if (Entity.NewName != "")
                    {
                        Console.WriteLine("Change Name In Progress");
                        if (Entity.NewName != "")
                        {
                            Database.MySqlCommand cmdupdate = null;
                            cmdupdate.Update("apprentice").Set("MentorName", NewName).Where("MentorID", Entity.UID).Execute();

                            cmdupdate = new Database.MySqlCommand(Database.MySqlCommandType.UPDATE);
                            cmdupdate.Update("apprentice").Set("ApprenticeName", NewName).Where("ApprenticeID", Entity.UID).Execute();

                            cmdupdate = new Database.MySqlCommand(Database.MySqlCommandType.UPDATE);
                            cmdupdate.Update("arena").Set("EntityName", NewName).Where("EntityID", Entity.UID).Execute();

                            cmdupdate = new Database.MySqlCommand(Database.MySqlCommandType.UPDATE);
                            cmdupdate.Update("claimitems").Set("OwnerName", NewName).Where("OwnerUID", Entity.UID).Execute();

                            cmdupdate = new Database.MySqlCommand(Database.MySqlCommandType.UPDATE);
                            cmdupdate.Update("claimitems").Set("GainerName", NewName).Where("GainerUID", Entity.UID).Execute();

                            cmdupdate = new Database.MySqlCommand(Database.MySqlCommandType.UPDATE);
                            cmdupdate.Update("detaineditems").Set("OwnerName", NewName).Where("OwnerEntity.UID", Entity.UID).Execute();

                            cmdupdate = new Database.MySqlCommand(Database.MySqlCommandType.UPDATE);
                            cmdupdate.Update("detaineditems").Set("GainerName", NewName).Where("GainerUID", Entity.UID).Execute();

                            cmdupdate = new Database.MySqlCommand(Database.MySqlCommandType.UPDATE);
                            cmdupdate.Update("enemy").Set("EnemyName", NewName).Where("EnemyID", Entity.UID).Execute();

                            cmdupdate = new Database.MySqlCommand(Database.MySqlCommandType.UPDATE);
                            cmdupdate.Update("friends").Set("FriendName", NewName).Where("FriendID", Entity.UID).Execute();

                            cmdupdate = new Database.MySqlCommand(Database.MySqlCommandType.UPDATE);
                            cmdupdate.Update("guilds").Set("Name", NewName).Where("Name", Entity.Name).Execute();

                            cmdupdate = new Database.MySqlCommand(Database.MySqlCommandType.UPDATE);
                            cmdupdate.Update("guilds").Set("LeaderName", NewName).Where("LeaderName", Entity.Name).Execute();

                            cmdupdate = new Database.MySqlCommand(Database.MySqlCommandType.UPDATE);
                            cmdupdate.Update("nobility").Set("EntityName", NewName).Where("EntityName", Entity.UID).Execute();

                            cmdupdate = new Database.MySqlCommand(Database.MySqlCommandType.UPDATE);
                            cmdupdate.Update("partners").Set("PartnerName", NewName).Where("PartnerID", Entity.UID).Execute();

                            cmdupdate = new Database.MySqlCommand(Database.MySqlCommandType.UPDATE);
                            cmdupdate.Update("chi").Set("name", NewName).Where("uid", Entity.UID).Execute();

                            cmdupdate = new Database.MySqlCommand(Database.MySqlCommandType.UPDATE);
                            cmdupdate.Update("teamarena").Set("EntityName", NewName).Where("EntityID", Entity.UID).Execute();

                            cmdupdate = new Database.MySqlCommand(Database.MySqlCommandType.UPDATE);
                            cmdupdate.Update("entities").Set("Name", NewName).Where("UID", Entity.UID).Execute();

                            cmdupdate = new Database.MySqlCommand(Database.MySqlCommandType.UPDATE);
                            cmdupdate.Update("flags").Set("TopArcher", NewName).Where("TopArcher", name).Execute();

                            cmdupdate = new Database.MySqlCommand(Database.MySqlCommandType.UPDATE);
                            cmdupdate.Update("flags").Set("TopPirate", NewName).Where("TopPirate", name).Execute();

                            cmdupdate = new Database.MySqlCommand(Database.MySqlCommandType.UPDATE);
                            cmdupdate.Update("flags").Set("TopTrojan", NewName).Where("TopTrojan", name).Execute();

                            cmdupdate = new Database.MySqlCommand(Database.MySqlCommandType.UPDATE);
                            cmdupdate.Update("flags").Set("TopGuildLeader", NewName).Where("TopGuildLeader", name).Execute();

                            cmdupdate = new Database.MySqlCommand(Database.MySqlCommandType.UPDATE);
                            cmdupdate.Update("flags").Set("TopNinja", NewName).Where("TopNinja", name).Execute();

                            cmdupdate = new Database.MySqlCommand(Database.MySqlCommandType.UPDATE);
                            cmdupdate.Update("flags").Set("TopMonk", NewName).Where("TopMonk", name).Execute();

                            cmdupdate = new Database.MySqlCommand(Database.MySqlCommandType.UPDATE);
                            cmdupdate.Update("flags").Set("TopWarrior", NewName).Where("TopWarrior", name).Execute();

                            cmdupdate = new Database.MySqlCommand(Database.MySqlCommandType.UPDATE);
                            cmdupdate.Update("flags").Set("TopSpouse", NewName).Where("TopSpouse", name).Execute();

                            cmdupdate = new Database.MySqlCommand(Database.MySqlCommandType.UPDATE);
                            cmdupdate.Update("flags").Set("TopWaterTaoist", NewName).Where("TopWaterTaoist", name).Execute();

                            cmdupdate = new Database.MySqlCommand(Database.MySqlCommandType.UPDATE);
                            cmdupdate.Update("flags").Set("TopFireTaoist", NewName).Where("TopFireTaoist", name).Execute();

                            cmdupdate = new Database.MySqlCommand(Database.MySqlCommandType.UPDATE);
                            cmdupdate.Update("flags").Set("MonthlyPkChampion", NewName).Where("MonthlyPkChampion", name).Execute();

                            cmdupdate = new Database.MySqlCommand(Database.MySqlCommandType.UPDATE);
                            cmdupdate.Update("flags").Set("WeeklyPkChampion", NewName).Where("WeeklyPkChampion", name).Execute();

                            cmdupdate = new Database.MySqlCommand(Database.MySqlCommandType.UPDATE);
                            cmdupdate.Update("flags").Set("TopDeputyLeader", NewName).Where("TopDeputyLeader", name).Execute();

                            cmdupdate = new Database.MySqlCommand(Database.MySqlCommandType.UPDATE);
                            cmdupdate.Update("flags").Set("TopDeputyLeader2", NewName).Where("TopDeputyLeader2", name).Execute();

                            cmdupdate = new Database.MySqlCommand(Database.MySqlCommandType.UPDATE);
                            cmdupdate.Update("flags").Set("TopDeputyLeader3", NewName).Where("TopDeputyLeader3", name).Execute();

                            cmdupdate = new Database.MySqlCommand(Database.MySqlCommandType.UPDATE);
                            cmdupdate.Update("flags").Set("TopDeputyLeader4", NewName).Where("TopDeputyLeader4", name).Execute();

                            cmdupdate = new Database.MySqlCommand(Database.MySqlCommandType.UPDATE);
                            cmdupdate.Update("flags").Set("TopDeputyLeader5", NewName).Where("TopDeputyLeader5", name).Execute();
                        }
                    }
                    if (Booth != null)
                        Booth.Remove();
                    if (Entity.MyClones.Count > 0)
                    {
                        foreach (var item in Entity.MyClones.Values)
                        {
                            Map.RemoveEntity(item);
                            Data data = new Data(true);
                            data.UID = item.UID;
                            data.ID = Network.GamePackets.Data.RemoveEntity;
                            item.MonsterInfo.SendScreen(data);
                        }
                        Entity.MyClones.Clear();

                    }
                    if (Companion != null)
                    {
                        Map.RemoveEntity(Companion);
                        Data data = new Data(true);
                        data.UID = Companion.UID;
                        data.ID = Data.RemoveEntity;
                        Companion.MonsterInfo.SendScreen(data);
                    }
                    if (QualifierGroup != null)
                        QualifierGroup.End(this);
                    if (TeamQualifierGroup != null)
                        TeamQualifierGroup.CheckEnd(this, true);
                    if (ChampionGroup != null)
                        ChampionGroup.End(this);
                    if (SignedUpForEPK == true)
                        ElitePKMatch.End(this);
                    if (Challenge != null)
                        Challenge.End(this);

                    Game.Arena.Clear(this);
                    Game.TeamArena.Clear(this);
                    Game.Champion.Clear(this);
                    Database.EntityTable.SaveTopDonation(this);
                    RemoveScreenSpawn(this.Entity, false);


                    #region Friend/TradePartner/Apprentice
                    Message msg = new Message("Your friend, " + Entity.Name + ", has logged off.", System.Drawing.Color.Red, Message.TopLeft);
                    if (Friends == null)
                        Friends = new SafeDictionary<uint, Conquer_Online_Server.Game.ConquerStructures.Society.Friend>(100);
                    foreach (Game.ConquerStructures.Society.Friend friend in Friends.Values)
                    {
                        if (friend.IsOnline)
                        {
                            var packet = new KnownPersons(true)
                            {
                                UID = Entity.UID,
                                Type = KnownPersons.RemovePerson,
                                Name = Entity.Name,
                                Online = false
                            };
                            friend.Client.Send(packet);
                            packet.Type = KnownPersons.AddFriend;
                            if (friend != null)
                            {
                                if (friend.Client != null)
                                {
                                    friend.Client.Send(packet);
                                    friend.Client.Send(msg);
                                }
                            }
                        }
                    }
                    Message msg2 = new Message("Your partner, " + Entity.Name + ", has logged off.", System.Drawing.Color.Red, Message.TopLeft);

                    if (Partners != null)
                    {
                        foreach (Game.ConquerStructures.Society.TradePartner partner in Partners.Values)
                        {
                            if (partner.IsOnline)
                            {
                                var packet = new TradePartner(true)
                                {
                                    UID = Entity.UID,
                                    Type = TradePartner.BreakPartnership,
                                    Name = Entity.Name,
                                    HoursLeft = (int)(new TimeSpan(partner.ProbationStartedOn.AddDays(3).Ticks).TotalHours - new TimeSpan(DateTime.Now.Ticks).TotalHours),
                                    Online = false
                                };
                                partner.Client.Send(packet);
                                packet.Type = TradePartner.AddPartner;
                                if (partner != null)
                                {
                                    if (partner.Client != null)
                                    {
                                        partner.Client.Send(packet);
                                        partner.Client.Send(msg2);
                                    }
                                }
                            }
                        }
                    }
                    MentorInformation Information = new MentorInformation(true);
                    Information.Mentor_Type = 1;
                    Information.Mentor_ID = Entity.UID;
                    Information.Mentor_Level = Entity.Level;
                    Information.Mentor_Class = Entity.Class;
                    Information.Mentor_PkPoints = Entity.PKPoints;
                    Information.Mentor_Mesh = Entity.Mesh;
                    Information.Mentor_Online = false;
                    Information.String_Count = 3;
                    Information.Mentor_Name = Entity.Name;
                    Information.Mentor_Spouse_Name = Entity.Spouse;
                    if (Apprentices == null) Apprentices = new SafeDictionary<uint, Game.ConquerStructures.Society.Apprentice>();
                    foreach (var appr in Apprentices.Values)
                    {
                        if (appr.IsOnline)
                        {
                            Information.Apprentice_ID = appr.ID;
                            Information.Enrole_Date = appr.EnroleDate;
                            Information.Apprentice_Name = appr.Name;
                            appr.Client.Send(Information);
                            appr.Client.ReviewMentor();
                        }
                    }
                    if (Mentor != null)
                    {
                        if (Mentor.IsOnline)
                        {
                            ApprenticeInformation AppInfo = new ApprenticeInformation();
                            AppInfo.Apprentice_ID = Entity.UID;
                            AppInfo.Apprentice_Level = Entity.Level;
                            AppInfo.Apprentice_Name = Entity.Name;
                            AppInfo.Apprentice_Online = false;
                            AppInfo.Apprentice_Spouse_Name = Entity.Spouse;
                            AppInfo.Enrole_date = Mentor.EnroleDate;
                            AppInfo.Mentor_ID = Mentor.Client.Entity.UID;
                            AppInfo.Mentor_Mesh = Mentor.Client.Entity.Mesh;
                            AppInfo.Mentor_Name = Mentor.Client.Entity.Name;
                            AppInfo.Type = 2;
                            Mentor.Client.Send(AppInfo);
                        }
                    }

                    #endregion
                    #region Team
                    if (Team != null)
                    {
                        if (Team.TeamLeader)
                        {
                            Network.GamePackets.Team team = new Network.GamePackets.Team();
                            team.UID = Account.EntityID;
                            team.Type = Network.GamePackets.Team.Dismiss;
                            foreach (Client.GameClient Teammate in Team.Teammates)
                            {
                                if (Teammate != null)
                                {
                                    if (Teammate.Entity.UID != Account.EntityID)
                                    {
                                        Teammate.Send(team);
                                        Teammate.Team = null;
                                    }
                                }
                            }
                        }
                        else
                        {
                            Network.GamePackets.Team team = new Network.GamePackets.Team();
                            team.UID = Account.EntityID;
                            team.Type = Network.GamePackets.Team.ExitTeam;
                            foreach (Client.GameClient Teammate in Team.Teammates)
                            {
                                if (Teammate != null)
                                {
                                    if (Teammate.Entity.UID != Account.EntityID)
                                    {
                                        Teammate.Send(team);
                                        Teammate.Team.Remove(Account.EntityID);
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                }
                catch (Exception e)
                {
                    Program.SaveException(e);
                }
                finally
                {
                    Kernel.DisconnectPool.Remove(this.Entity.UID);
                    Console.WriteLine(this.Entity.Name + " has logged off! Ip:[" + this.Account.IP + "]");
                    Console.Title = "[" + Database.rates.servername + "] Server. Start time: " + Program.StartDate.ToString("dd/MM/yyyy hh:mm") + ". Players online: " + Kernel.GamePool.Count + "/" + Program.PlayerCap + " Max Online: " + Program.MaxOn + " ";
                }
            }
        }

        public ClientWrapper Socket { get { return _socket; } }
        public string IP
        {
            get
            {
                return _socket.IP;
            }
        }


        #region Game

        public Database.ChiTable.ChiData ChiData;
        public List<ChiPowerStructure> ChiPowers;
        public uint ChiPoints = 0;

        public SafeDictionary<uint, DetainedItem> ClaimableItem = new SafeDictionary<uint, DetainedItem>(1000),
                                                  DeatinedItem = new SafeDictionary<uint, DetainedItem>(1000);

        public bool DoSetOffline = true;

        public ushort OnlineTrainingPoints = 0;
        public Time32 LastTrainingPointsUp, LastTreasurePoints = Time32.Now.AddMinutes(1);

        public List<string> GuildNamesSpawned = new List<string>();

        public byte KylinUpgradeCount = 0;

        public ulong OblivionExperience = 0;
        public byte OblivionKills = 0;

        public int PremShopType = 0;
        public DateTime VIPDate;
        public DateTime LastVote;
        public uint VIPDays;
        public uint DonationPoints;
        public uint VotePoints;

        public Time32 ScreenReloadTime;
        public int MillisecondsScreenReload;
        public bool Reloaded = false;
        public Interfaces.IPacket ReloadWith;

        public ushort VendingDisguise;
        public uint BlessTime;
        public int speedHackSuspiction = 0;
        public Time32 LastPingT;
        public uint LastPingStamp = 0;
        public Game.Entity Companion;

        public List<GameClient> Prayers;
        public GameClient PrayLead;

        public DateTime ChatBanTime;
        public uint ChatBanLasts;
        public bool ChatBanned;

        public uint BackupArmorLook
        {
            get { return this["bkparmorlook"]; }
            set { this["bkparmorlook"] = value; }
        }
        public uint ArmorLook
        {
            get { return this["armorlook"]; }
            set { this["armorlook"] = value; }
        }
        public uint HeadgearLook
        {
            get { return this["headgearlook"]; }
            set { this["headgearlook"] = value; }
        }

        public bool ValidArmorLook(uint id)
        {
            if (id == 0) return false;
            var soulInfo = Database.AddingInformationTable.SoulGearItems[id];
            if (id >= 800000 && id < 900000)
            {
                if (soulInfo.ItemIdentifier < 100)
                    if (soulInfo.ItemIdentifier != ConquerItem.Armor)
                        return false;
                    else { }
                else
                    if (Network.PacketHandler.ItemPosition((uint)(soulInfo.ItemIdentifier * 1000)) != ConquerItem.Armor)
                    return false;
            }
            else
                if (Network.PacketHandler.ItemPosition(id) != ConquerItem.Armor)
                return false;
            return true;
        }
        public bool ValidHeadgearLook(uint id)
        {
            if (id == 0) return false;
            var soulInfo = Database.AddingInformationTable.SoulGearItems[id];
            if (id >= 800000 && id < 900000)
            {
                if (soulInfo.ItemIdentifier < 100)
                    if (soulInfo.ItemIdentifier != ConquerItem.Head)
                        return false;
                    else { }
                else
                    if (Network.PacketHandler.ItemPosition((uint)(soulInfo.ItemIdentifier * 1000)) != ConquerItem.Head)
                    return false;
            }
            else
                if (Network.PacketHandler.ItemPosition(id) != ConquerItem.Head)
                return false;
            return true;
        }

        public void SetNewArmorLook(uint id)
        {
            ArmorLook = id;
            if (!ValidArmorLook(id)) return;

            var iu = new Network.GamePackets.ItemUsage(true);
            iu.UID = uint.MaxValue - 1;
            iu.dwParam = 13;
            iu.ID = Network.GamePackets.ItemUsage.UnequipItem;
            Send(iu);
            iu = new Network.GamePackets.ItemUsage(true);
            iu.UID = uint.MaxValue - 1;
            iu.ID = Network.GamePackets.ItemUsage.RemoveInventory;
            Send(iu);

            ConquerItem fakeItem = new Network.GamePackets.ConquerItem(true);
            fakeItem.ID = id;
            fakeItem.Durability = 1;
            fakeItem.MaximDurability = 1;
            fakeItem.Color = (Game.Enums.Color)Kernel.Random.Next(4, 8);
            fakeItem.UID = uint.MaxValue - 1;
            fakeItem.Position = 13;
            Send(fakeItem);
            fakeItem.Mode = Enums.ItemMode.Update;
            Send(fakeItem);
            ClientEquip eqs = new ClientEquip();
            eqs.DoEquips(this);
            Send(eqs);
            Equipment.UpdateEntityPacket();
        }
        public void SetNewHeadgearLook(uint id)
        {
            HeadgearLook = id;
            if (!ValidHeadgearLook(id)) return;

            var iu = new Network.GamePackets.ItemUsage(true);
            iu.UID = uint.MaxValue - 2;
            iu.dwParam = 14;
            iu.ID = Network.GamePackets.ItemUsage.UnequipItem;
            Send(iu);
            iu = new Network.GamePackets.ItemUsage(true);
            iu.UID = uint.MaxValue - 2;
            iu.ID = Network.GamePackets.ItemUsage.RemoveInventory;
            Send(iu);

            ConquerItem fakeItem = new Network.GamePackets.ConquerItem(true);
            fakeItem.ID = id;
            fakeItem.Durability = 1;
            fakeItem.MaximDurability = 1;
            fakeItem.Color = (Game.Enums.Color)Kernel.Random.Next(4, 8);
            fakeItem.UID = uint.MaxValue - 2;
            fakeItem.Position = 14;
            Send(fakeItem);
            fakeItem.Mode = Enums.ItemMode.Update;
            Send(fakeItem);
            ClientEquip eqs = new ClientEquip();
            eqs.DoEquips(this);
            Send(eqs);
            Equipment.UpdateEntityPacket();
        }

        public byte JewelarLauKind, JewelarLauGems;
        public uint VirtuePoints;
        public DateTime LastLotteryEntry;
        public byte LotteryEntries;
        public bool InLottery;
        public DateTime OfflineTGEnterTime;
        public bool Mining = false;
        public Time32 MiningStamp;
        public ushort Vigor
        {
            get
            {
                if (Equipment != null)
                    if (!Equipment.Free(12))
                        return Equipment.TryGetItem((byte)12).Vigor;
                return 65535;
            }
            set
            {
                if (!Equipment.Free(12))
                    Equipment.TryGetItem((byte)12).Vigor = value;
            }
        }
        public ushort MaxVigor
        {
            get { return (ushort)(30 + Entity.ExtraVigor); }
        }

        public bool HeadgearClaim, NecklaceClaim, ArmorClaim, WeaponClaim, RingClaim, BootsClaim, TowerClaim, FanClaim;
        public string PromoteItemNameNeed
        {
            get
            {
                if (Entity.Class % 10 == 0)
                    return " nothing but";
                if (Entity.Class % 10 == 1)
                    if (Entity.Class / 10 == 4)
                        return " five Euxenite Ores and";
                    else
                        return " nothing but";
                if (Entity.Class % 10 == 2)
                    return " one Emerald and";
                if (Entity.Class % 10 == 3)
                    return " one Meteor and";
                if (Entity.Class % 10 == 4)
                    return " one MoonBox and";
                return " nothing but";
            }
        }
        public byte PromoteItemCountNeed
        {
            get
            {
                if (Entity.Class % 10 == 0)
                    return 0;
                if (Entity.Class % 10 == 1)
                    if (Entity.Class / 10 == 4)
                        return 5;
                    else
                        return 0;
                if (Entity.Class % 10 == 2)
                    return 1;
                if (Entity.Class % 10 == 3)
                    return 1;
                if (Entity.Class % 10 == 4)
                    return 1;
                return 0;
            }
        }
        public uint PromoteItemNeed
        {
            get
            {
                if (Entity.Class % 10 == 0)
                    return 0;
                if (Entity.Class % 10 == 1)
                    if (Entity.Class / 10 == 4)
                        return 1072031;
                    else
                        return 0;
                if (Entity.Class % 10 == 2)
                    return 1080001;
                if (Entity.Class % 10 == 3)
                    return 1088001;
                if (Entity.Class % 10 == 4)
                    return 721020;
                return 0;
            }
        }
        public uint PromoteItemGain
        {
            get
            {
                if (Entity.Class % 10 == 0)
                    return 0;
                if (Entity.Class % 10 == 1)
                    if (Entity.Class / 10 == 4)
                        return 500067;
                    else
                        return 0;
                if (Entity.Class % 10 == 2)
                    return 0;
                if (Entity.Class % 10 == 3)
                    return 700031;
                if (Entity.Class % 10 == 4)
                    return 1088000;
                return 0;
            }
        }
        public uint PromoteLevelNeed
        {
            get
            {
                if (Entity.Class % 10 == 0)
                    return 15;
                if (Entity.Class % 10 == 1)
                    return 40;
                if (Entity.Class % 10 == 2)
                    return 70;
                if (Entity.Class % 10 == 3)
                    return 100;
                if (Entity.Class % 10 == 4)
                    return 110;
                return 0;
            }
        }
        public byte SelectedItem, UpdateType;
        public ushort UplevelProficiency;
        public UInt32 GuildJoinTarget = 0;
        public uint OnHoldGuildJoin = 0;
        public uint elitepoints = 0;
        public uint eliterank = 0;
        public bool SentRequest = false;
        public Game.ConquerStructures.Society.Guild Guild;
        public Game.ConquerStructures.Society.Guild.Member AsMember;
        public uint Arsenal_Donation = 0;
        public Game.ConquerStructures.Booth Booth;


        public bool RaceExcitement, RaceDecelerated, RaceGuard, RaceDizzy, RaceFrightened;
        public Time32 RaceExcitementStamp, GuardStamp, DizzyStamp, FrightenStamp, ChaosStamp, ExtraVigorStamp, DecelerateStamp;
        public uint RaceExcitementAmount, RaceExtraVigor;
        public GameCharacterUpdates SpeedChange;
        public void ApplyRacePotion(Enums.RaceItemType type, uint target)
        {
            switch (type)
            {
                case Enums.RaceItemType.FrozenTrap:
                    {
                        if (target != uint.MaxValue)
                        {
                            if (Map.Floor[Entity.X, Entity.Y, MapObjectType.StaticEntity])
                            {
                                StaticEntity item = new StaticEntity((uint)(Entity.X * 1000 + Entity.Y), Entity.X, Entity.Y, (ushort)Map.ID);
                                item.DoFrozenTrap(Entity.UID);
                                Map.AddStaticEntity(item);
                                Kernel.SendSpawn(item);
                            }
                        }
                        else
                        {
                            Entity.FrozenStamp = Time32.Now;
                            Entity.FrozenTime = 5;
                            GameCharacterUpdates update = new GameCharacterUpdates(true);
                            update.UID = Entity.UID;
                            update.Add(GameCharacterUpdates.Freeze, 0, 4);
                            SendScreen(update, true);
                            Entity.AddFlag(Update.Flags.Freeze);
                        }
                        break;
                    }
                case Enums.RaceItemType.RestorePotion:
                    {
                        Vigor += 2000;
                        if (Vigor > MaxVigor)
                            Vigor = MaxVigor;
                        Send(new Vigor(true) { Amount = Vigor });
                        break;
                    }
                case Enums.RaceItemType.ExcitementPotion:
                    {
                        if (RaceExcitement && RaceExcitementAmount > 50)
                            return;

                        if (RaceDecelerated)
                        {
                            RaceDecelerated = false;

                            var upd = new GameCharacterUpdates(true);
                            upd.UID = Entity.UID;
                            upd.Remove(GameCharacterUpdates.Decelerated);
                            SendScreen(upd, true);
                        }
                        RaceExcitementStamp = Time32.Now;
                        RaceExcitement = true;
                        {
                            var upd = new GameCharacterUpdates(true);
                            upd.UID = Entity.UID;
                            upd.Add(GameCharacterUpdates.Accelerated, 50, 15, 25);
                            SendScreen(upd, true);
                            SpeedChange = upd;
                        }
                        RaceExcitementAmount = 50;
                        Entity.AddFlag(Update.Flags.OrangeSparkles);
                        break;
                    }
                case Enums.RaceItemType.SuperExcitementPotion:
                    {
                        if (RaceDecelerated)
                        {
                            RaceDecelerated = false;

                            var upd = new GameCharacterUpdates(true);
                            upd.UID = Entity.UID;
                            upd.Remove(GameCharacterUpdates.Decelerated);
                            SendScreen(upd, true);
                        }
                        RaceExcitementAmount = 200;
                        RaceExcitementStamp = Time32.Now;
                        RaceExcitement = true;
                        this.Entity.AddFlag(Update.Flags.SpeedIncreased);
                        {
                            var upd = new GameCharacterUpdates(true);
                            upd.UID = Entity.UID;
                            upd.Add(GameCharacterUpdates.Accelerated, 200, 15, 100);
                            SendScreen(upd, true);
                            SpeedChange = upd;
                        }
                        Entity.AddFlag(Update.Flags.OrangeSparkles);
                        break;
                    }
                case Enums.RaceItemType.GuardPotion:
                    {
                        RaceGuard = true;
                        GuardStamp = Time32.Now;
                        Entity.AddFlag(Update.Flags.DivineShield);
                        DizzyStamp = DizzyStamp.AddSeconds(-100);
                        FrightenStamp = FrightenStamp.AddSeconds(-100);
                        var upd = new GameCharacterUpdates(true);
                        upd.UID = Entity.UID;
                        upd.Add(GameCharacterUpdates.DivineShield, 0, 10);
                        SendScreen(upd, true);
                        break;
                    }
                case Enums.RaceItemType.DizzyHammer:
                    {
                        Entity Target;
                        if (Screen.TryGetValue(target, out Target))
                        {
                            var Owner = Target.Owner;
                            if (Owner != null)
                            {
                                if (!Owner.RaceGuard && !Owner.RaceFrightened)
                                {
                                    Owner.DizzyStamp = Time32.Now;
                                    Owner.RaceDizzy = true;
                                    Owner.Entity.AddFlag(Update.Flags.Dizzy);
                                    {
                                        var upd = new GameCharacterUpdates(true);
                                        upd.UID = Entity.UID;
                                        upd.Add(GameCharacterUpdates.Dizzy, 0, 5);
                                        Owner.SendScreen(upd, true);
                                    }
                                }
                            }
                        }
                        break;
                    }
                case Enums.RaceItemType.ScreamBomb:
                    {
                        SendScreen(new SpellUse(true)
                        {
                            Attacker = Entity.UID,
                            SpellID = 9989,
                            SpellLevel = 0,
                            X = Entity.X,
                            Y = Entity.Y
                        }.AddTarget(Entity.UID, 0, null), true);
                        foreach (var obj in Screen.SelectWhere<Entity>(MapObjectType.Player,
                            (o) => Kernel.GetDistance(o.X, o.Y, Entity.X, Entity.Y) <= 10))
                        {
                            var Owner = obj.Owner;
                            if (!Owner.RaceGuard && !Owner.RaceDizzy)
                            {
                                Owner.RaceFrightened = true;
                                Owner.FrightenStamp = Time32.Now;
                                Owner.Entity.AddFlag(Update.Flags.Frightened);
                                {
                                    var upd = new GameCharacterUpdates(true);
                                    upd.UID = Owner.Entity.UID;
                                    upd.Add(GameCharacterUpdates.Flustered, 0, 20);
                                    Owner.SendScreen(upd, true);
                                }
                            }
                        }
                        break;
                    }
                case Enums.RaceItemType.SpiritPotion:
                    {
                        ExtraVigorStamp = Time32.Now;
                        RaceExtraVigor = 2000;
                        break;
                    }
                case Enums.RaceItemType.ChaosBomb:
                    {
                        SendScreen(new SpellUse(true)
                        {
                            Attacker = Entity.UID,
                            SpellID = 9989,
                            SpellLevel = 0,
                            X = Entity.X,
                            Y = Entity.Y
                        }.AddTarget(Entity.UID, 0, null), true);
                        foreach (var obj in this.Screen.SelectWhere<Entity>(MapObjectType.Player,
                               (o) => Kernel.GetDistance(o.X, o.Y, Entity.X, Entity.Y) <= 10))
                        {
                            var Owner = obj.Owner;
                            if (!Owner.RaceGuard)
                            {
                                Owner.FrightenStamp = Time32.Now;
                                Owner.DizzyStamp = Owner.DizzyStamp.AddSeconds(-1000);

                                Owner.Entity.AddFlag(Update.Flags.Confused);
                                {
                                    var upd = new GameCharacterUpdates(true);
                                    upd.UID = Owner.Entity.UID;
                                    upd.Add(GameCharacterUpdates.Flustered, 0, 15);
                                    Owner.SendScreen(upd, true);
                                }
                            }
                        }
                        break;
                    }
                case Enums.RaceItemType.SluggishPotion:
                    {
                        SendScreen(new SpellUse(true)
                        {
                            Attacker = Entity.UID,
                            SpellID = 9989,
                            SpellLevel = 0,
                            X = Entity.X,
                            Y = Entity.Y
                        }.AddTarget(Entity.UID, 0, null), true);
                        foreach (var obj in this.Screen.SelectWhere<Entity>(MapObjectType.Player,
                              o => Kernel.GetDistance(o.X, o.Y, Entity.X, Entity.Y) <= 10))
                        {
                            var Owner = obj.Owner;
                            if (!Owner.RaceGuard)
                            {
                                Owner.RaceDecelerated = true;
                                Owner.DecelerateStamp = Time32.Now;
                                if (Owner.RaceExcitement)
                                {
                                    Owner.RaceExcitement = false;

                                    var upd = new GameCharacterUpdates(true);
                                    upd.UID = Owner.Entity.UID;
                                    upd.Remove(GameCharacterUpdates.Accelerated);
                                    Owner.SendScreen(upd, true);
                                }
                                Owner.Entity.AddFlag(Update.Flags.PurpleSparkles);
                                {
                                    var upd = new GameCharacterUpdates(true);
                                    upd.UID = Owner.Entity.UID;
                                    unchecked { upd.Add(GameCharacterUpdates.Decelerated, 50, 10, (uint)(0 - 25)); }
                                    Owner.SendScreen(upd, true);
                                    Owner.SpeedChange = upd;
                                }
                            }
                        }
                        break;
                    }
                case Enums.RaceItemType.TransformItem:
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            if (Potions[i] != null)
                            {
                                if (Potions[i].Type != Enums.RaceItemType.TransformItem)
                                {
                                    Send(new RacePotion(true)
                                    {
                                        Amount = 0,
                                        Location = i + 1,
                                        PotionType = Potions[i].Type
                                    });
                                    Potions[i] = null;
                                }
                            }
                        }
                        //for (int i = 0; i < 5; i++)
                        {
                            int i = 0;
                            if (Potions[i] == null)
                            {
                                int val = (int)Enums.RaceItemType.TransformItem;
                                while (val == (int)Enums.RaceItemType.TransformItem)
                                    val = Kernel.Random.Next((int)Enums.RaceItemType.ChaosBomb, (int)Enums.RaceItemType.SuperExcitementPotion);
                                Potions[i] = new UsableRacePotion();
                                Potions[i].Count = 1;
                                Potions[i].Type = (Enums.RaceItemType)val;
                                Send(new RacePotion(true)
                                {
                                    Amount = 1,
                                    Location = i + 1,
                                    PotionType = Potions[i].Type
                                });
                            }
                        }
                        break;
                    }
            }
        }


        public void ReviewMentor()
        {
            #region NotMentor
            uint nowBP = 0;
            if (Mentor != null)
            {
                if (Mentor.IsOnline)
                {
                    nowBP = Entity.BattlePowerFrom(Mentor.Client.Entity);
                }
            }
            if (nowBP > 200) nowBP = 0;
            if (nowBP < 0) nowBP = 0;
            if (Entity.MentorBattlePower != nowBP)
            {
                Entity.MentorBattlePower = nowBP;
                if (Mentor != null)
                {
                    if (Mentor.IsOnline)
                    {
                        MentorInformation Information = new MentorInformation(true);
                        Information.Mentor_Type = 1;
                        Information.Mentor_ID = Mentor.Client.Entity.UID;
                        Information.Apprentice_ID = Entity.UID;
                        Information.Enrole_Date = Mentor.EnroleDate;
                        Information.Mentor_Level = Mentor.Client.Entity.Level;
                        Information.Mentor_Class = Mentor.Client.Entity.Class;
                        Information.Mentor_PkPoints = Mentor.Client.Entity.PKPoints;
                        Information.Mentor_Mesh = Mentor.Client.Entity.Mesh;
                        Information.Mentor_Online = true;
                        Information.Shared_Battle_Power = nowBP;
                        Information.String_Count = 3;
                        Information.Mentor_Name = Mentor.Client.Entity.Name;
                        Information.Apprentice_Name = Entity.Name;
                        Information.Mentor_Spouse_Name = Mentor.Client.Entity.Spouse;
                        Send(Information);
                    }
                }
            }
            #endregion
            #region Mentor
            if (Apprentices == null) Apprentices = new SafeDictionary<uint, Game.ConquerStructures.Society.Apprentice>();
            foreach (var appr in Apprentices.Values)
            {
                if (appr.IsOnline)
                {
                    uint nowBPs = 0;
                    nowBPs = appr.Client.Entity.BattlePowerFrom(Entity);
                    if (appr.Client.Entity.MentorBattlePower != nowBPs)
                    {
                        appr.Client.Entity.MentorBattlePower = nowBPs;
                        MentorInformation Information = new MentorInformation(true);
                        Information.Mentor_Type = 1;
                        Information.Mentor_ID = Entity.UID;
                        Information.Apprentice_ID = appr.Client.Entity.UID;
                        Information.Enrole_Date = appr.EnroleDate;
                        Information.Mentor_Level = Entity.Level;
                        Information.Mentor_Class = Entity.Class;
                        Information.Mentor_PkPoints = Entity.PKPoints;
                        Information.Mentor_Mesh = Entity.Mesh;
                        Information.Mentor_Online = true;
                        Information.Shared_Battle_Power = nowBPs;
                        Information.String_Count = 3;
                        Information.Mentor_Name = Entity.Name;
                        Information.Apprentice_Name = appr.Client.Entity.Name;
                        Information.Mentor_Spouse_Name = Entity.Spouse;
                        appr.Client.Send(Information);
                    }
                }
            }
            #endregion
        }
        public void AddQuarantineKill()
        {
            quarantineKill++;
            UpdateQuarantineScore();
        }
        public void AddGl()
        {
            TopGlClaim++;
            return;
        }
        public void AddDl()
        {
            TopDlClaim++;
            return;
        }
        public void AddQuarantineDeath()
        {
            quarantineDeath++;
            UpdateQuarantineScore();
        }
        public void UpdateQuarantineScore()
        {
            string[] scores = new string[3];
            scores[0] = "Black team: " + Conquer_Online_Server.Game.Quarantine.BlackScore.ToString() + " wins";
            scores[1] = "White team: " + Conquer_Online_Server.Game.Quarantine.WhiteScore.ToString() + " wins";
            scores[2] = "Your score: " + quarantineKill + " kills, " + quarantineDeath + " death";
            for (int i = 0; i < scores.Length; i++)
            {
                Message msg = new Message(scores[i], System.Drawing.Color.Red, i == 0 ? Message.FirstRightCorner : Message.ContinueRightCorner);
                Send(msg);
            }
        }
        public void KillTerrorist()
        {
            foreach (Client.GameClient Terrorist in Program.GamePool)
            {
                if (Terrorist.Entity.KillTheTerrorist_IsTerrorist == true && Terrorist.Entity.MapID == 1801)
                    Conquer_Online_Server.Kernel.SendWorldMessage(new Conquer_Online_Server.Network.GamePackets.Message("Terrorist: " + Terrorist.Entity.Name + " ", System.Drawing.Color.Black, Conquer_Online_Server.Network.GamePackets.Message.FirstRightCorner), Program.GamePool);
            }
        }
        public void AddBless(uint value)
        {
            Entity.HeavenBlessing += value;
            Entity.Update(Network.GamePackets._String.Effect, "bless", true);
            if (Mentor != null)
            {
                if (Mentor.IsOnline)
                {
                    Mentor.Client.PrizeHeavenBlessing += (ushort)(value / 10 / 60 / 60);
                    AsApprentice = Mentor.Client.Apprentices[Entity.UID];
                }
                if (AsApprentice != null)
                {
                    AsApprentice.Actual_HeavenBlessing += (ushort)(value / 10 / 60 / 60);
                    AsApprentice.Total_HeavenBlessing += (ushort)(value / 10 / 60 / 60);
                    if (Time32.Now > LastMentorSave.AddSeconds(5))
                    {
                        Database.KnownPersons.SaveApprenticeInfo(AsApprentice);
                        LastMentorSave = Time32.Now;
                    }
                }
            }
        }
        public ulong PrizeExperience;
        public ushort PrizeHeavenBlessing;
        public ushort PrizePlusStone;

        public uint MentorApprenticeRequest;
        public uint TradePartnerRequest;

        public object[] OnMessageBoxEventParams;
        public Action<GameClient> MessageOK;
        public Action<GameClient> MessageCancel;

        public bool JustLoggedOn = true;
        public Time32 ReviveStamp = Time32.Now;
        public bool Attackable;

        public Game.ConquerStructures.NobilityInformation NobilityInformation;
        public Game.Entity Entity;
        public Game.Screen Screen;
        public Time32 LastPing = Time32.Now;
        public static ushort NpcTestType = 0;
        public byte TinterItemSelect = 0;
        public DateTime LastDragonBallUse, LastResetTime;
        public byte Action = 0;
        public bool CheerSent = false;
        public Game.Arena.QualifierList.QualifierGroup WatchingGroup;
        public Game.Arena.QualifierList.QualifierGroup QualifierGroup;
        public Game.Champion.QualifierList.QualifierGroup ChampionGroup;
        public Network.GamePackets.ArenaStatistic ArenaStatistic;

        public Game.TeamArena.QualifierList.QualifierGroup TeamWatchingGroup;
        public Game.TeamArena.QualifierList.QualifierGroup TeamQualifierGroup;
        public Network.GamePackets.TeamArenaStatistic TeamArenaStatistic;
        public uint ArenaPoints
        {
            get
            {
                return ArenaStatistic.ArenaPoints;
            }
            set
            {
                ArenaStatistic.ArenaPoints =
                    TeamArenaStatistic.ArenaPoints =
                    value;
            }
        }
        private byte xpCount;
        public byte XPCount
        {
            get { return xpCount; }
            set
            {
                xpCount = value;
                if (xpCount >= 100) xpCount = 100;

                Update update = new Update(true);
                update.UID = Entity.UID;
                update.Append(Update.XPCircle, xpCount);
                update.Send(this);
            }
        }
        public Time32 XPCountStamp = Time32.Now;
        public Time32 XPListStamp = Time32.Now;

        #region poker
        public Network.GamePackets.poker2090 poker2090;
        public Network.GamePackets.poker2091 poker2091;
        public Network.GamePackets.poker2092 poker2092;
        public Network.GamePackets.poker2093 poker2093;
        public Network.GamePackets.poker2094 poker2094;
        public Network.GamePackets.poker2095 poker2095;
        #endregion
        public Conquer_Online_Server.Game.ConquerStructures.Trade Trade;
        public byte ExpBalls = 0;
        public uint MoneySave = 0, ActiveNpc = 0;
        public string TempPassword;
        private uint _WarehousePW;
        public uint WarehousePW
        {
            get
            {
                return _WarehousePW;
            }
            set
            {
                _WarehousePW = value;
                if (Entity != null)
                    if (Entity.FullyLoaded)
                        Entity.UpdateDatabase1("WarehousePW", value);
            }
        }
        /*private uint _WarehousePW;
        public uint WarehousePW
        {
            get
            {
                return _WarehousePW;
            }
            set
            {
                _WarehousePW = value;
                if (Entity != null)
                    if (Entity.FullyLoaded)
                        Entity.UpdateDatabase1("WarehousePW", value);
            }
        }*/
        public bool WarehouseOpen;
        public Time32 CoolStamp;
        public sbyte WarehouseOpenTries;
        public ushort InputLength;
        public bool InArenaMatch = false;
        public Conquer_Online_Server.Game.ConquerStructures.Society.Mentor Mentor;
        public Conquer_Online_Server.Game.ConquerStructures.Society.Apprentice AsApprentice;
        public SafeDictionary<ushort, Interfaces.IProf> Proficiencies;
        public SafeDictionary<ushort, Interfaces.ISkill> Spells;
        public SafeDictionary<uint, Conquer_Online_Server.Game.ConquerStructures.Society.Friend> Friends;
        public SafeDictionary<uint, Conquer_Online_Server.Game.ConquerStructures.Society.Enemy> Enemy;
        public SafeDictionary<uint, Conquer_Online_Server.Game.ConquerStructures.Society.TradePartner> Partners;
        public SafeDictionary<uint, Conquer_Online_Server.Game.ConquerStructures.Society.Apprentice> Apprentices;
        public Game.ConquerStructures.Inventory Inventory;
        public Game.ConquerStructures.Equipment Equipment;
        public SafeDictionary<Game.ConquerStructures.Warehouse.WarehouseID, Game.ConquerStructures.Warehouse> Warehouses;
        public Game.ConquerStructures.Team Team;
        public Time32 lastClientJumpTime = Time32.Now;
        public Time32 lastJumpTime = Time32.Now;
        public Time32 LastClientJump;

        public int LastJumpTime = 0;
        public short lastJumpDistance = 0;
        public bool DoubleExpToday = false;

        private Game.Map map;
        public Game.Map Map
        {
            get
            {
                if (map == null)
                {
                    if (Entity == null) return null;
                    Kernel.Maps.TryGetValue(Entity.MapID, out map);
                    if (Entity.MapID >= 11000 && map == null)
                        Entity.MapID = 1005;
                    if (map == null)
                        return (map = new Game.Map(Entity.MapID, Database.MapsTable.MapInformations[Entity.MapID].BaseID, Database.DMaps.MapPaths[Database.MapsTable.MapInformations[Entity.MapID].BaseID]));
                }
                else
                {
                    if (map.ID != Entity.MapID)
                    {
                        Kernel.Maps.TryGetValue(Entity.MapID, out map);
                        if (Entity.MapID >= 11000 && map == null)
                            Entity.MapID = 1005;
                        if (map == null)
                            return (map = new Game.Map(Entity.MapID, Database.MapsTable.MapInformations[Entity.MapID].BaseID, Database.DMaps.MapPaths[Database.MapsTable.MapInformations[Entity.MapID].BaseID]));
                    }
                    if (Entity.MapID == 1005 || Entity.MapID == 5580 || Entity.MapID == 5570 || Entity.MapID == 1452 || Entity.MapID == 8684 || Entity.MapID == 2057 || Entity.MapID == 5560 || Entity.MapID == 5530 || Entity.MapID == 6630 || Entity.MapID == 1038 || Entity.MapID == 6634 || Entity.MapID == 6633 || Entity.MapID == 6632 || Entity.MapID == 6631 || Entity.MapID == 1038 || Entity.MapID == 2636 || Entity.MapID == 2637 || Entity.MapID == 8595 || Entity.MapID == 6060 || Entity.MapID == 6061 || Entity.MapID == 6062 || Entity.MapID == 6063 || Entity.MapID == 6064 || Entity.MapID == 1038 || Entity.MapID == 6066)
                        Entity.RemoveFlag(Network.GamePackets.Update.Flags.Ride);
                }
                return map;
            }
        }

        public uint ExpBall
        {
            get
            {
                ulong exp = Database.DataHolder.LevelExperience(Entity.Level);
                return (uint)(exp * 13000 / (ulong)((Entity.Level * Entity.Level * Entity.Level / 12) + 1));
            }
        }

        public bool AddProficiency(IProf proficiency)
        {
            if (this.Proficiencies.ContainsKey(proficiency.ID))
            {
                this.Proficiencies[proficiency.ID].Level = proficiency.Level;
                this.Proficiencies[proficiency.ID].Experience = proficiency.Experience;
                proficiency.Send(this);
                return false;
            }
            this.Proficiencies.Add(proficiency.ID, proficiency);
            proficiency.Send(this);
            return true;
        }

        public bool AddSpell(ISkill spell)
        {
            if (this.Spells.ContainsKey(spell.ID))
            {
                if (this.Spells[spell.ID].Level < spell.Level)
                {
                    this.Spells[spell.ID].Level = spell.Level;
                    this.Spells[spell.ID].Experience = spell.Experience;
                    if (spell.ID != 0xbf4)
                    {
                        spell.Send(this);
                    }
                }
                return false;
            }
            if ((spell.ID == 0x415) || (spell.ID == 0x416))
            {
                if (!this.Proficiencies.ContainsKey(Conquer_Online_Server.Database.SpellTable.SpellInformations[spell.ID][spell.Level].WeaponSubtype))
                {
                    return false;
                }
                if (this.Proficiencies[Conquer_Online_Server.Database.SpellTable.SpellInformations[spell.ID][spell.Level].WeaponSubtype].Level < 5)
                {
                    return false;
                }
            }
            this.Spells.Add(spell.ID, spell);
            if (spell.ID != 0xbf4)
            {
                spell.Send(this);
            }
            return true;
        }
        public bool RemoveSpell(Interfaces.ISkill spell)
        {
            if (Spells.ContainsKey(spell.ID))
            {
                Spells.Remove(spell.ID);
                Network.GamePackets.Data data = new Data(true);
                data.UID = Entity.UID;
                data.dwParam = spell.ID;
                data.ID = 109;
                Send(data);
                Database.SkillTable.DeleteSpell(this, spell.ID);
                return true;
            }
            return false;
        }
        public bool WentToComplete = false;
        public byte SelectedGem = 0;
        public Time32 LastMentorSave = Time32.Now;
        public void IncreaseExperience(ulong experience, bool addMultiple)
        {
            if (Entity.Dead) return;
            byte level = Entity.Level;
            ulong _experience = Entity.Experience;
            ulong prExperienece = experience;
            if (addMultiple)
            {
                if (Entity.VIPLevel > 0)
                    experience *= 3;
                if (Program.Today == DayOfWeek.Saturday || Program.Today == DayOfWeek.Sunday || Program.Today == DayOfWeek.Monday || Program.Today == DayOfWeek.Thursday || Program.Today == DayOfWeek.Tuesday || Program.Today == DayOfWeek.Wednesday || Program.Today == DayOfWeek.Friday)
                    experience *= 2;
                experience *= Constants.ExtraExperienceRate;
                experience += experience * Entity.Gems[3] / 100;
                if (Entity.DoubleExperienceTime > 0)
                    experience *= 2;
                if (Entity.HeavenBlessing > 0)
                    experience += (uint)(experience * 20 / 100);
                if (Entity.Reborn >= 2)
                    experience /= 3;
                if (Map.BaseID == 1039)
                    experience /= 50;
                if (Guild != null)
                {
                    if (Guild.Level > 0)
                    {
                        experience += (ushort)(experience * Guild.Level / 100);
                    }
                }
                prExperienece = experience + (ulong)(experience * ((float)Entity.BattlePower / 100));

                _experience += prExperienece;
            }
            else
                _experience += experience;
            if (this.Entity.Auto == true)
            {
                this.Entity.autohuntxp += experience;
            }
            if (Entity.Level < 140)
            {
                while (_experience >= Database.DataHolder.LevelExperience(level) && level < 140)
                {
                    _experience -= Database.DataHolder.LevelExperience(level);
                    level++;
                    if (Entity.Reborn == 1)
                    {
                        if (level >= 130 && Entity.FirstRebornLevel > 130 && level < Entity.FirstRebornLevel)
                            level = Entity.FirstRebornLevel;
                    }
                    else if (Entity.Reborn == 2)
                    {
                        if (level >= 130 && Entity.SecondRebornLevel > 130 && level < Entity.SecondRebornLevel)
                            level = Entity.SecondRebornLevel;
                    }
                    if (Entity.Class >= 10 && Entity.Class <= 15)
                        if (!Spells.ContainsKey(1110))
                            AddSpell(new Network.GamePackets.Spell(true) { ID = 1110 });
                    if (Entity.Class >= 20 && Entity.Class <= 25)
                        if (!Spells.ContainsKey(1020))
                            AddSpell(new Network.GamePackets.Spell(true) { ID = 1020 });
                    if (Entity.Class >= 40 && Entity.Class <= 45)
                        if (!Spells.ContainsKey(8002))
                            AddSpell(new Network.GamePackets.Spell(true) { ID = 8002 });
                    if (Entity.Class >= 50 && Entity.Class <= 55)
                        if (!Spells.ContainsKey(6011))
                            AddSpell(new Network.GamePackets.Spell(true) { ID = 6011 });
                    if (Entity.Class >= 60 && Entity.Class <= 65)
                        if (!Spells.ContainsKey(10490))
                            AddSpell(new Network.GamePackets.Spell(true) { ID = 10490 });
                    if (Mentor != null)
                    {
                        if (Mentor.IsOnline)
                        {
                            uint exExp = (uint)(level * 2);
                            Mentor.Client.PrizeExperience += exExp;
                            AsApprentice = Mentor.Client.Apprentices[Entity.UID];
                            if (AsApprentice != null)
                            {
                                AsApprentice.Actual_Experience += exExp;
                                AsApprentice.Total_Experience += exExp;
                            }
                            if (Mentor.Client.PrizeExperience > 50 * 606)
                                Mentor.Client.PrizeExperience = 50 * 606;
                        }
                    }
                    if (level == 70)
                    {
                        if (ArenaStatistic == null || ArenaStatistic.EntityID == 0)
                        {
                            ArenaStatistic = new Conquer_Online_Server.Network.GamePackets.ArenaStatistic(true);
                            ArenaStatistic.EntityID = Entity.UID;
                            ArenaStatistic.Name = Entity.Name;
                            ArenaStatistic.Level = Entity.Level;
                            ArenaStatistic.Class = Entity.Class;
                            ArenaStatistic.Model = Entity.Mesh;
                            ArenaPoints = Database.ArenaTable.ArenaPointFill(Entity.Level);
                            ArenaStatistic.LastArenaPointFill = DateTime.Now;
                            Database.ArenaTable.InsertArenaStatistic(this);
                            ArenaStatistic.Status = Network.GamePackets.ArenaStatistic.NotSignedUp;
                            Game.Arena.ArenaStatistics.Add(Entity.UID, ArenaStatistic);
                        }
                    }
                    if (Entity.Reborn == 0)
                    {
                        if (level <= 120)
                        {
                            Database.DataHolder.GetStats(Entity.Class, level, this);
                            CalculateStatBonus();
                            CalculateHPBonus();
                            GemAlgorithm();
                        }
                        else
                            Entity.Atributes += 3;
                    }
                    else
                    {
                        Entity.Atributes += 3;
                    }
                }
                if (Entity.Level != level)
                {
                    if (Team != null)
                    {
                        if (Team.LowestLevelsUID == Entity.UID)
                        {
                            Team.LowestLevel = 0;
                            Team.LowestLevelsUID = 0;
                            Team.SearchForLowest();
                        }
                    }
                    Entity.Level = level;
                    Entity.Hitpoints = Entity.MaxHitpoints;
                    Entity.Mana = Entity.MaxMana;
                    if (Entity.Level > 130)
                        Database.EntityTable.UpdateLevel(Entity.Owner);
                    if (Entity.Reborn == 2)
                        Network.PacketHandler.ReincarnationHash(Entity.Owner);
                }
                if (Entity.Experience != _experience)
                    Entity.Experience = _experience;
            }
        }

        public void IncreaseSpellExperience(uint experience, ushort id)
        {
            if (this.Spells.ContainsKey(id))
            {
                switch (id)
                {
                    case 0x50a:
                    case 0x13a6:
                    case 0x1b76:
                        experience = 100;
                        break;
                }
                experience *= Constants.ExtraSpellRate;
                experience += (experience * this.Entity.Gems[6]) / 100;
                if (this.Map.BaseID == 0x40f)
                {
                    experience /= 40;
                }
                ISkill skill = this.Spells[id];
                if (skill != null)
                {
                    if (this.Entity.VIPLevel > 0)
                    {
                        experience *= 5;
                    }
                    Conquer_Online_Server.Database.SpellInformation information = Conquer_Online_Server.Database.SpellTable.SpellInformations[skill.ID][skill.Level];
                    if ((information != null) && ((information.NeedExperience != 0) && (this.Entity.Level >= information.NeedLevel)))
                    {
                        skill.Experience += experience;
                        bool flag = false;
                        if (skill.Experience >= information.NeedExperience)
                        {
                            skill.Experience = 0;
                            skill.Level = (byte)(skill.Level + 1);
                            flag = true;
                            this.Send(Constants.SpellLeveled);
                        }
                        if (flag)
                        {
                            skill.Send(this);
                        }
                        else
                        {
                            SkillExperience experience2 = new SkillExperience(true);
                            experience2.AppendSpell(skill.ID, skill.Experience);
                            experience2.Send(this);
                        }
                    }
                }
            }
        }

        public void IncreaseProficiencyExperience(uint experience, ushort id)
        {
            if (this.Proficiencies.ContainsKey(id))
            {
                IProf prof = this.Proficiencies[id];
                experience *= Constants.ExtraProficiencyRate;
                experience += (experience * this.Entity.Gems[5]) / 100;
                if (this.Map.BaseID == 0x40f)
                {
                    experience /= 40;
                }
                if (this.Entity.VIPLevel > 0)
                {
                    experience *= 5;
                }
                prof.Experience += experience;
                if (prof.Level < 20)
                {
                    bool flag = false;
                    while (prof.Experience >= Conquer_Online_Server.Database.DataHolder.ProficiencyLevelExperience(prof.Level))
                    {
                        prof.Experience -= Conquer_Online_Server.Database.DataHolder.ProficiencyLevelExperience(prof.Level);
                        prof.Level = (byte)(prof.Level + 1);
                        if (prof.Level == 20)
                        {
                            prof.Experience = 0;
                            prof.Send(this);
                            this.Send(Constants.ProficiencyLeveled);
                            return;
                        }
                        flag = true;
                        this.Send(Constants.ProficiencyLeveled);
                    }
                    if (flag)
                    {
                        prof.Send(this);
                    }
                    else
                    {
                        SkillExperience experience2 = new SkillExperience(true);
                        experience2.AppendProficiency(prof.ID, prof.Experience, Conquer_Online_Server.Database.DataHolder.ProficiencyLevelExperience(prof.Level));
                        experience2.Send(this);
                    }
                }
            }
            else
            {
                Proficiency proficiency = new Proficiency(true)
                {
                    ID = id
                };
                this.AddProficiency(proficiency);
            }
        }

        public byte ExtraAtributePoints(byte level, byte mClass)
        {
            if (mClass == 135)
            {
                if (level <= 110)
                    return 0;
                switch (level)
                {
                    case 112: return 1;
                    case 114: return 3;
                    case 116: return 6;
                    case 118: return 10;
                    case 120: return 15;
                    case 121: return 15;
                    case 122: return 21;
                    case 123: return 21;
                    case 124: return 28;
                    case 125: return 28;
                    case 126: return 36;
                    case 127: return 36;
                    case 128: return 45;
                    case 129: return 45;
                    default:
                        return 55;
                }
            }
            else
            {
                if (level <= 120)
                    return 0;
                switch (level)
                {
                    case 121: return 1;
                    case 122: return 3;
                    case 123: return 6;
                    case 124: return 10;
                    case 125: return 15;
                    case 126: return 21;
                    case 127: return 28;
                    case 128: return 36;
                    case 129: return 45;
                    default:
                        return 55;
                }
            }
        }
        public static ISkill LearnableSpell(ushort spellid)
        {
            ISkill spell = new Spell(true);
            spell.ID = spellid;
            return spell;
        }
        public bool Reborn(byte toClass)
        {
            #region Items
            if (Inventory.Count > 37) return false;
            switch (toClass)
            {
                case 11:
                case 21:
                case 51:
                case 61:
                case 71:
                    {
                        Inventory.Add(410077, Game.Enums.ItemEffect.Poison);
                        break;
                    }
                case 41:
                    {
                        Inventory.Add(500057, Game.Enums.ItemEffect.Shield);
                        break;
                    }
                case 132:
                case 142:
                    {
                        if (toClass == 132)
                            Inventory.Add(421077, Game.Enums.ItemEffect.MP);
                        else
                            Inventory.Add(421077, Game.Enums.ItemEffect.HP);
                        break;
                    }
            }
            #region Low level items
            for (byte i = 1; i < 9; i++)
            {
                if (i != 7)
                {
                    ConquerItem item = Equipment.TryGetItem(i);
                    if (item != null && item.ID != 0)
                    {
                        try
                        {
                            //UnloadItemStats(item, false);
                            Database.ConquerItemInformation cii = new Conquer_Online_Server.Database.ConquerItemInformation(item.ID, item.Plus);
                            item.ID = cii.LowestID(Network.PacketHandler.ItemMinLevel(Network.PacketHandler.ItemPosition(item.ID)));
                            item.Mode = Conquer_Online_Server.Game.Enums.ItemMode.Update;
                            item.Send(this);
                            LoadItemStats();
                            Database.ConquerItemTable.UpdateItemID(item, this);
                        }
                        catch
                        {
                            Console.WriteLine("Reborn item problem: " + item.ID);
                        }
                    }
                }
            }
            ConquerItem hand = Equipment.TryGetItem(5);
            if (hand != null)
            {
                Equipment.Remove(5);
                CalculateStatBonus();
                CalculateHPBonus();
            }
            hand = Equipment.TryGetItem(25);
            if (hand != null)
            {
                Equipment.Remove(25);
                CalculateStatBonus();
                CalculateHPBonus();
            }
            LoadItemStats();
            SendScreen(Entity.SpawnPacket, false);
            #endregion
            #endregion
            if (Entity.Reborn == 0)
            {
                Entity.FirstRebornClass = Entity.Class;
                Entity.FirstRebornLevel = Entity.Level;
                Entity.Atributes =
                    (ushort)(ExtraAtributePoints(Entity.FirstRebornClass, Entity.FirstRebornLevel) + 52);
            }
            else
            {
                Entity.SecondRebornClass = Entity.Class;
                Entity.SecondRebornLevel = Entity.Level;
                Entity.Atributes =
                    (ushort)(ExtraAtributePoints(Entity.FirstRebornClass, Entity.FirstRebornLevel) +
                    ExtraAtributePoints(Entity.SecondRebornClass, Entity.SecondRebornLevel) + 62);
            }
            byte PreviousClass = Entity.Class;
            Entity.Reborn++;
            Entity.Class = toClass;
            Entity.Level = 15;
            Entity.Experience = 0;

            #region Spells

            Interfaces.ISkill[] spells = Spells.Values.ToArray();
            foreach (Interfaces.ISkill spell in spells)
            {
                spell.PreviousLevel = spell.Level;
                spell.Level = 0;
                spell.Experience = 0;
                #region KungFuKing
                if (PreviousClass == 85)
                {
                    if (Entity.Class != 81)
                    {
                        switch (spell.ID)
                        {
                            case 12120:
                            case 12130:
                            case 12140:
                            case 12160:
                            case 12170:
                            case 12200:
                            case 12240:
                            case 12350:
                            case 12270:
                            case 12280:
                            case 12290:
                            case 12300:
                            case 12320:
                            case 12330:
                            case 12340:
                                RemoveSpell(spell);
                                break;
                        }
                    }
                }
                #endregion
                #region Pirate

                if (PreviousClass == 75)
                {
                    if (Entity.Class != 71)
                    {
                        switch (spell.ID)
                        {
                            case 11110:
                            case 11040:
                            case 11050:
                            case 11060:
                            case 11100:
                            case 11120:
                            case 11130:
                            case 11030:
                                RemoveSpell(spell);
                                break;
                        }
                    }
                }

                #endregion

                #region Monk

                if (PreviousClass == 65)
                {
                    if (Entity.Class != 61)
                    {
                        switch (spell.ID)
                        {
                            case 10490:
                            case 10415:
                            case 10381:
                                RemoveSpell(spell);
                                break;
                        }
                    }
                }

                #endregion

                #region Warrior

                if (PreviousClass == 25)
                {
                    if (Entity.Class != 21)
                    {
                        switch (spell.ID)
                        {
                            case 1025:
                                if (Entity.Class != 21 && Entity.Class != 132)
                                    RemoveSpell(spell);
                                break;
                        }
                    }
                }

                #endregion

                #region Ninja

                if (toClass != 51)
                {
                    switch (spell.ID)
                    {
                        case 6010:
                        case 6000:
                        case 6011:
                            RemoveSpell(spell);
                            break;
                    }
                }

                #endregion

                #region Trojan

                if (toClass != 11)
                {
                    switch (spell.ID)
                    {
                        case 1115:
                            RemoveSpell(spell);
                            break;
                    }
                }

                #endregion

                #region Archer

                if (toClass != 41)
                {
                    switch (spell.ID)
                    {
                        case 8001:
                        case 8000:
                        case 8003:
                        case 9000:
                        case 8002:
                        case 8030:
                            RemoveSpell(spell);
                            break;
                    }
                }

                #endregion

                #region WaterTaoist

                if (PreviousClass == 135)
                {
                    if (toClass != 132)
                    {
                        switch (spell.ID)
                        {
                            case 1000:
                            case 1001:
                            case 1010:
                            case 1125:
                            case 1100:
                            case 8030:
                                RemoveSpell(spell);
                                break;
                            case 1050:
                            case 1175:
                            case 1170:
                                if (toClass != 142)
                                    RemoveSpell(spell);
                                break;
                        }
                    }
                }

                #endregion

                #region FireTaoist

                if (PreviousClass == 145)
                {
                    if (toClass != 142)
                    {
                        switch (spell.ID)
                        {
                            case 1000:
                            case 1001:
                            case 1150:
                            case 1180:
                            case 1120:
                            case 1002:
                            case 1160:
                            case 1165:
                                RemoveSpell(spell);
                                break;
                        }
                    }
                }

                #endregion

                if (Spells.ContainsKey(spell.ID))
                    if (spell.ID != (ushort)Game.Enums.SkillIDs.Reflect)
                        spell.Send(this);
            }

            #endregion 
            #region Proficiencies
            foreach (Interfaces.IProf proficiency in Proficiencies.Values)
            {
                proficiency.PreviousLevel = proficiency.Level;
                proficiency.Level = 0;
                proficiency.Experience = 0;
                proficiency.Send(this);
            }
            #endregion
            #region Adding earned skills

            if (Entity.Reborn == 2)
                AddSpell(new Spell(true) { ID = 9876 });
            if (toClass == 51 && PreviousClass == 55 && Entity.Reborn == 1)
                AddSpell(new Spell(true) { ID = 6002 });
            if (toClass == 81 && PreviousClass == 85 && Entity.Reborn == 1)
                AddSpell(new Spell(true) { ID = 12280 });
            if (Entity.FirstRebornClass == 85 && Entity.SecondRebornClass == 85 && Entity.Class == 81 &&
                Entity.Reborn == 2)
                AddSpell(new Spell(true) { ID = 12300 });
            if (Entity.FirstRebornClass == 15 && Entity.SecondRebornClass == 15 && Entity.Class == 11 &&
                Entity.Reborn == 2)
                AddSpell(new Spell(true) { ID = 10315 });
            if (Entity.FirstRebornClass == 25 && Entity.SecondRebornClass == 25 && Entity.Class == 21 &&
                Entity.Reborn == 2)
                AddSpell(new Spell(true) { ID = 10311 });
            if (Entity.FirstRebornClass == 45 && Entity.SecondRebornClass == 45 && Entity.Class == 41 &&
                Entity.Reborn == 2)
                AddSpell(new Spell(true) { ID = 10313 });
            if (Entity.FirstRebornClass == 55 && Entity.SecondRebornClass == 55 && Entity.Class == 51 &&
                Entity.Reborn == 2)
                AddSpell(new Spell(true) { ID = 6003 });
            if (Entity.FirstRebornClass == 65 && Entity.SecondRebornClass == 65 && Entity.Class == 61 &&
                Entity.Reborn == 2)
                AddSpell(new Spell(true) { ID = 10405 });
            if (Entity.FirstRebornClass == 135 && Entity.SecondRebornClass == 135 && Entity.Class == 132 &&
                Entity.Reborn == 2)
                AddSpell(new Spell(true) { ID = 30000 });
            if (Entity.FirstRebornClass == 145 && Entity.SecondRebornClass == 145 && Entity.Class == 142 &&
                Entity.Reborn == 2)
                AddSpell(new Spell(true) { ID = 10310 });
            if (Entity.Reborn == 1)
            {
                if (Entity.FirstRebornClass == 75 && Entity.Class == 71)
                {
                    AddSpell(new Spell(true) { ID = 3050 });
                }
                if (Entity.FirstRebornClass == 15 && Entity.Class == 11)
                {
                    AddSpell(new Spell(true) { ID = 3050 });
                }
                else if (Entity.FirstRebornClass == 25 && Entity.Class == 21)
                {
                    AddSpell(new Spell(true) { ID = 3060 });
                }
                else if (Entity.FirstRebornClass == 145 && Entity.Class == 142)
                {
                    AddSpell(new Spell(true) { ID = 3080 });
                }
                else if (Entity.FirstRebornClass == 135 && Entity.Class == 132)
                {
                    AddSpell(new Spell(true) { ID = 3090 });
                }
            }
            if (Entity.Reborn == 2)
            {
                if (Entity.SecondRebornClass == 75 && Entity.Class == 71)
                {
                    AddSpell(new Spell(true) { ID = 3050 });
                }
                if (Entity.SecondRebornClass == 15 && Entity.Class == 11)
                {
                    AddSpell(new Spell(true) { ID = 3050 });
                }
                else if (Entity.SecondRebornClass == 25)
                {
                    AddSpell(new Spell(true) { ID = 3060 });
                }
                else if (Entity.SecondRebornClass == 145 && Entity.Class == 142)
                {
                    AddSpell(new Spell(true) { ID = 3080 });
                }
                else if (Entity.SecondRebornClass == 135 && Entity.Class == 132)
                {
                    AddSpell(new Spell(true) { ID = 3090 });
                }
            }

            #endregion

            Database.DataHolder.GetStats(Entity.Class, Entity.Level, this);
            CalculateStatBonus();
            CalculateHPBonus();
            GemAlgorithm();
            using (var conn = Database.DataHolder.MySqlConnection)
            {
                conn.Open();
                Database.EntityTable.SaveEntity(this, conn);
                Database.SkillTable.SaveSpells(this, conn);
                Database.SkillTable.SaveProficiencies(this, conn);
                Database.SkillTable.SaveSpells(this);
                Database.SkillTable.SaveProficiencies(this);
            }

            Kernel.SendWorldMessage(new Message("" + Entity.Name + " has got " + Entity.Reborn + " reborns. Congratulations!", System.Drawing.Color.White, Message.Center), Program.GamePool);
            return true;
        }
        #region Items
        private int StatHP;
        public uint[] ArsenalDonations;
        public uint GetArsenalDonation()
        {
            uint val = 0;
            foreach (var Uint in ArsenalDonations)
                val += Uint;
            return val;
        }
        public void CalculateHPBonus()
        {
            switch (Entity.Class)
            {
                case 11: Entity.MaxHitpoints = (uint)(StatHP * 1.05F); break;
                case 12: Entity.MaxHitpoints = (uint)(StatHP * 1.08F); break;
                case 13: Entity.MaxHitpoints = (uint)(StatHP * 1.10F); break;
                case 14: Entity.MaxHitpoints = (uint)(StatHP * 1.12F); break;
                case 15: Entity.MaxHitpoints = (uint)(StatHP * 1.15F); break;
                default: Entity.MaxHitpoints = (uint)StatHP; break;
            }
            Entity.MaxHitpoints += Entity.ItemHP;
            Entity.MaxHitpoints += Entity.Intensification;

            var plus = Entity.SubClasses.Classes.SingleOrDefault(x => x.Value.ID == 9);
            if (plus.Value != null && Entity.SubClass == 9)
                Entity.MaxHitpoints += (uint)(plus.Value.Level * 100);
            Entity.Hitpoints = Math.Min(Entity.Hitpoints, Entity.MaxHitpoints);
        }
        public void CalculateStatBonus()
        {
            byte ManaBoost = 5;
            const byte HitpointBoost = 20;
            sbyte Class = (sbyte)(Entity.Class / 10);
            if (Class == 13 || Class == 14)
                ManaBoost += (byte)(5 * (Entity.Class - (Class * 10)));
            StatHP = (ushort)((Entity.Strength * 2) +
                                     (Entity.Agility * 2) +
                                     (Entity.Spirit * 2) +
                                     (Entity.Vitality * HitpointBoost));
            Entity.MaxMana = (ushort)((Entity.Spirit * ManaBoost) + Entity.ItemMP);
            Entity.Mana = Math.Min(Entity.Mana, Entity.MaxMana);
        }
        public void SendStatMessage()
        {
            this.ReviewMentor();
            Network.GamePackets.Message Msg = new Conquer_Online_Server.Network.GamePackets.Message(" Your status has been changed", System.Drawing.Color.DarkGoldenrod
                , Network.GamePackets.Message.TopLeft);
            Msg.__Message = string.Format(Msg.__Message,
                new object[] { Entity.MinAttack, Entity.MaxAttack, Entity.MagicAttack, Entity.Defence, (Entity.MagicDefence + Entity.MagicDefence), Entity.Dodge, Entity.PhysicalDamageDecrease, Entity.MagicDamageDecrease, Entity.PhysicalDamageIncrease, Entity.MagicDamageIncrease, Entity.Hitpoints, Entity.MaxHitpoints, Entity.Mana, Entity.MaxMana, Entity.BattlePower });
            this.Send(Msg);
        }
        public void LoadItemStats(Game.Entity e)
        {
            #region Set Every Variable to Zero
            Entity.Defence = 0;
            Entity.MagicDefencePlus = 0;
            Entity.MagicDefenceItem = 0;
            Entity.ExtraVigor = 0;
            Entity.Vigor = 0;
            Entity.MagicAttackPlus = 0;
            Entity.Dodge = 0;
            Entity.BaseMagicAttack = 0;
            Entity.MetalResistance = 0;
            Entity.WoodResistance = 0;
            Entity.FireResistance = 0;
            Entity.WaterResistance = 0;
            Entity.EarthResistance = 0;
            Entity.Breaktrough = 0;
            Entity.CriticalStrike = 0;
            Entity.Immunity = 0;
            Entity.Penetration = 0;
            Entity.Counteraction = 0;
            Entity.Block = 0;
            Entity.Detoxication = 0;
            Entity.Intensification = 0;
            Entity.Penetration = 0;
            Entity.SkillCStrike = 0;
            Entity.MaxAttack = 0;
            Entity.MinAttack = 0;
            Entity.PhysicalDamageDecrease = 0;
            Entity.MagicDamageDecrease = 0;
            Entity.MagicDamageIncrease = 0;
            Entity.PhysicalDamageIncrease = 0;
            Entity.MagicDefencePercent = 0;
            Entity.ItemHP = 0;
            Entity.ItemMP = 0;
            Entity.ItemBless = 0;
            Entity.AttackRange = 0;
            Entity.BaseMinAttack = 0;
            Entity.BaseMaxAttack = 0;
            Entity.BaseMagicDefence = 0;
            Entity.BaseDefence = 0;
            Entity.MagicDamageIncrease = 0;
            Entity.Gems = new UInt16[10];
        }
        #endregion

        private bool AreStatsLoadable(ConquerItem item)
        {
            if (!AlternateEquipment)
                if (item.Position > 20)
                    return false;
            if (AlternateEquipment)
                if (item.Position < 20)
                    if (!Equipment.Free((byte)(20 + item.Position)))
                        return false;

            int Position = item.Position;
            if (item.Position > 20) Position -= 20;

            if (Position == ConquerItem.LeftWeapon || Position == ConquerItem.RightWeapon)
                return false;

            return true;
        }

        private Tuple<ConquerItem, ConquerItem> ComputeWeapons()
        {
            if (!AlternateEquipment)
            {
                return new Tuple<ConquerItem, ConquerItem>(
                    Equipment.TryGetItem(ConquerItem.RightWeapon),
                    Equipment.TryGetItem(ConquerItem.LeftWeapon));
            }
            else
            {
                if (Equipment.Free(ConquerItem.AlternateRightWeapon))
                {
                    return new Tuple<ConquerItem, ConquerItem>(
                        Equipment.TryGetItem(ConquerItem.RightWeapon),
                        Equipment.TryGetItem(ConquerItem.LeftWeapon));
                }
                else
                {
                    if (Equipment.Free(ConquerItem.RightWeapon))
                    {
                        return new Tuple<ConquerItem, ConquerItem>(
                            Equipment.TryGetItem(ConquerItem.AlternateRightWeapon),
                            Equipment.TryGetItem(ConquerItem.AlternateLeftWeapon));
                    }
                    else
                    {
                        if (!Equipment.Free(ConquerItem.AlternateLeftWeapon))
                        {
                            return new Tuple<ConquerItem, ConquerItem>(
                                Equipment.TryGetItem(ConquerItem.AlternateRightWeapon),
                                Equipment.TryGetItem(ConquerItem.AlternateLeftWeapon));
                        }
                        else
                        {
                            if (Equipment.Free(ConquerItem.LeftWeapon))
                            {
                                return new Tuple<ConquerItem, ConquerItem>(
                                    Equipment.TryGetItem(ConquerItem.AlternateRightWeapon),
                                    null);
                            }
                            else
                            {
                                ConquerItem aRight = Equipment.TryGetItem(ConquerItem.AlternateRightWeapon),
                                             nLeft = Equipment.TryGetItem(ConquerItem.LeftWeapon);
                                if (PacketHandler.IsTwoHand(aRight.ID))
                                {
                                    if (PacketHandler.IsArrow(nLeft.ID))
                                    {
                                        if (PacketHandler.IsBow(aRight.ID))
                                        {
                                            return new Tuple<ConquerItem,
                                                ConquerItem>(aRight, nLeft);
                                        }
                                        else
                                        {
                                            return new Tuple<ConquerItem,
                                                ConquerItem>(aRight, null);
                                        }
                                    }
                                    else
                                    {
                                        if (PacketHandler.IsShield(nLeft.ID))
                                        {
                                            if (!Spells.ContainsKey(10311))//Perseverance
                                            {
                                                Send(new Message("You need to know Perseverance (Pure Warrior skill) to be able to wear 2-handed weapon and shield.", System.Drawing.Color.Red, Message.Talk));
                                                return new Tuple<ConquerItem,
                                                    ConquerItem>(aRight, null);
                                            }
                                            else
                                            {
                                                return new Tuple<ConquerItem,
                                                    ConquerItem>(aRight, nLeft);
                                            }
                                        }
                                        else
                                        {
                                            return new Tuple<ConquerItem,
                                                ConquerItem>(aRight, null);
                                        }
                                    }
                                }
                                else
                                {
                                    if (!PacketHandler.IsTwoHand(nLeft.ID))
                                    {
                                        return new Tuple<ConquerItem,
                                            ConquerItem>(aRight, nLeft);
                                    }
                                    else
                                    {
                                        return new Tuple<ConquerItem,
                                            ConquerItem>(aRight, null);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public int[][] ChampionAllowedStats = new int[][]
        {
            new int[] {1, 0, 0, 0, 0, 0, 0, 30, 0, 0 },
            new int[] {2, 0, 0, 0, 0, 0, 0, 40, 1, 1 },
            new int[] {3, 1, 0, 0, 0, 0, 50, 50, 2, 3 },
            new int[] {4, 3, 1, 1, 0, 0, 100, 60, 5, 4 },
            new int[] {5, 5, 1, 1, 1, 0, 150, 70, 7, 5 },
            new int[] {6, 5, 1, 1, 1, 0, 200, 80, 9, 7 },
            new int[] {12, 7, 2, 2, 1, 1, 255, 100, 12, 9 }
        };
        public bool DoChampStats { get { return ChampionGroup != null; } }
        public void LoadItemStats()
        {
            uint bStats = Entity.Hitpoints;
            for (int i = 0; i < 30; i++)
                if (Equipment.Objects[i] != null)
                    Equipment.Objects[i].IsWorn = false;

            #region Set Every Variable to Zero
            Entity.Defence = 0;
            Entity.MagicDefence = 0;
            Entity.MagicDefencePercent = 0;
            Entity.BaseMagicAttack = 0;
            Entity.BaseMagicDefence =
            Entity.BaseMaxAttack =
            Entity.BaseMinAttack =
            Entity.PhysicalDamageDecrease =
            Entity.PhysicalDamageIncrease =
            Entity.MagicDamageDecrease =
            Entity.MagicDamageIncrease = 0;
            Entity.ItemHP = 0;
            Entity.ItemHP =
            Entity.ItemMP =
            Entity.AttackRange = (byte)0;
            Entity.Dodge = 0;
            Entity.MinAttack = 0;
            Entity.MaxAttack = 0;
            Entity.Defence = 0;
            Entity.SuperItemBless = 0;
            Entity.MagicDefence = 0;
            Entity.Dodge = 0;
            Entity.BaseMagicAttack = 0;
            Entity.WoodResistance = 0;
            Entity.FireResistance = 0;
            Entity.WaterResistance = 0;
            Entity.EarthResistance = 0;
            Entity.Breaktrough = 0;
            Entity.WearsGoldPrize = false;
            Entity.CriticalStrike = 0;
            Entity.Immunity = 0;
            Entity.Penetration = 0;
            Entity.Counteraction = 0;
            Entity.Block = 0;
            Entity.Detoxication = 0;
            Entity.Intensification = 0;
            Entity.Penetration = 0;
            Entity.SkillCStrike = 0;
            Entity.MaxAttack = 0;
            Entity.MinAttack = 0;
            Entity.PhysicalDamageDecrease = 0;
            Entity.MagicDamageDecrease = 0;
            Entity.MagicDamageIncrease = 0;
            Entity.PhysicalDamageIncrease = 0;
            Entity.MagicDefencePercent = 0;
            Entity.ItemHP = 0;
            Entity.ItemMP = 0;
            Entity.ItemBless = 0;
            Entity.AttackRange = 0;
            Entity.BaseMinAttack = 0;
            Entity.BaseMaxAttack = 0;
            Entity.BaseMagicDefence = 0;
            Entity.BaseDefence = 0;
            Entity.MagicDamageIncrease = 0;
            Entity.Gems = new UInt16[10];
            #endregion

            foreach (ConquerItem i in Equipment.Objects)
            {
                if (i == null) continue;
                if (i.Durability == 0) continue;
                if (!AreStatsLoadable(i)) continue;
                loadItemStats(i);
            }

            Weapons = ComputeWeapons();
            if (Weapons == null) Weapons = new Tuple<ConquerItem, ConquerItem>(null, null);
            if (Weapons.Item1 != null)
            {
                loadItemStats(Weapons.Item1);
                if (Weapons.Item2 != null)
                {
                    if (!Weapons.Item1.IsTwoHander())
                        loadItemStats(Weapons.Item2);
                    else
                        if (PacketHandler.IsArrow(Weapons.Item2.ID) || (Entity.Class >= 20 && Entity.Class <= 25))
                        loadItemStats(Weapons.Item2);
                }
            }

            #region Subclasses
            foreach (var c in Entity.SubClasses.Classes)
            {
                int lvl = c.Value.Level;
                if (DoChampStats) lvl = Math.Min(lvl, ChampionAllowedStats[ChampionStats.Grade][9]);
                switch ((Conquer_Online_Server.Game.ClassID)c.Value.ID)
                {
                    case Game.ClassID.MartialArtist:
                        {
                            Entity.CriticalStrike += (ushort)(Entity.CriticalStrike - (Entity.CriticalStrike * (lvl / 10)));
                            break;
                        }
                    case Game.ClassID.Warlock:
                        {
                            Entity.SkillCStrike += (ushort)(Entity.SkillCStrike - (Entity.SkillCStrike * (lvl / 10)));
                            break;
                        }
                    case Game.ClassID.ChiMaster:
                        {
                            Entity.Immunity += (ushort)(Entity.Immunity - (Entity.Immunity * (lvl / 10)));
                            break;
                        }
                    case Game.ClassID.Sage:
                        {
                            Entity.Penetration += (ushort)(Entity.Penetration - (Entity.Penetration * (lvl / 10)));
                            break;
                        }
                    case Game.ClassID.Apothecary:
                        {
                            double per = lvl * 8 / 10;
                            Entity.Detoxication += (ushort)(Entity.Detoxication - (Entity.Detoxication * per));
                            break;
                        }
                    case Game.ClassID.Performer:
                        {
                            int per = lvl * 100;
                            Entity.BaseMaxAttack += (uint)per / 2;
                            Entity.BaseMinAttack += (uint)per / 2;
                            Entity.BaseMagicAttack += (uint)per;
                            break;
                        }
                }
            }
            #endregion
            #region Chi
            uint percentage = 100;
            if (DoChampStats)
                percentage = (uint)ChampionAllowedStats[ChampionStats.Grade][7];
            foreach (var chiPower in ChiPowers)
            {
                foreach (var attribute in chiPower.Attributes)
                {
                    switch (attribute.Type)
                    {
                        case Game.Enums.ChiAttribute.CriticalStrike:
                            Entity.CriticalStrike += (int)((ushort)(attribute.Value * 10) * percentage / 100);
                            break;
                        case Game.Enums.ChiAttribute.Counteraction:
                            Entity.Counteraction += (ushort)(attribute.Value * percentage / 100);
                            break;
                        case Game.Enums.ChiAttribute.AddAttack:
                            Entity.BaseMinAttack += attribute.Value * percentage / 100;
                            Entity.BaseMaxAttack += attribute.Value * percentage / 100;
                            break;
                        case Game.Enums.ChiAttribute.AddMagicAttack:
                            Entity.BaseMagicAttack += attribute.Value * percentage / 100;
                            break;
                        case Game.Enums.ChiAttribute.AddMagicDefense:
                            Entity.BaseMagicDefence += attribute.Value * percentage / 100;
                            break;
                        case Game.Enums.ChiAttribute.Breakthrough:
                            Entity.Breaktrough += (ushort)(attribute.Value * percentage / 100);
                            break;
                        case Game.Enums.ChiAttribute.HPAdd:
                            Entity.ItemHP += attribute.Value * percentage / 100;
                            break;
                        case Game.Enums.ChiAttribute.Immunity:
                            Entity.Immunity += (int)((ushort)(attribute.Value * 10) * percentage / 100);
                            break;
                        case Game.Enums.ChiAttribute.MagicDamageDecrease:
                            Entity.MagicDamageDecrease += (ushort)(attribute.Value * percentage / 100);
                            break;
                        case Game.Enums.ChiAttribute.MagicDamageIncrease:
                            Entity.MagicDamageIncrease += (ushort)(attribute.Value * percentage / 100);
                            break;
                        case Game.Enums.ChiAttribute.PhysicalDamageDecrease:
                            Entity.PhysicalDamageDecrease += (ushort)(attribute.Value * percentage / 100);
                            break;
                        case Game.Enums.ChiAttribute.PhysicalDamageIncrease:
                            Entity.PhysicalDamageIncrease += (ushort)(attribute.Value * percentage / 100);
                            break;
                        case Game.Enums.ChiAttribute.SkillCriticalStrike:
                            Entity.SkillCStrike += (int)((ushort)(attribute.Value * 10) * percentage / 100);
                            break;
                    }
                }
            }
            #region Dragon Ranking
            if (ChiData.DragonRank <= 50)
            {
                if (ChiData.DragonRank <= 3)
                {
                    Entity.ItemHP += 5000;
                    Entity.BaseMagicDefence += 300;
                    Entity.PhysicalDamageDecrease += 1000;
                    Entity.MagicDamageDecrease += 300;
                }
                else if (ChiData.DragonRank <= 15)
                {
                    Entity.ItemHP += (uint)(3000 - (ChiData.DragonRank - 4) * 90);
                    Entity.BaseMagicDefence += (uint)(250 - (ChiData.DragonRank - 4) * 9);
                    Entity.PhysicalDamageDecrease += (ushort)(600 - (ChiData.DragonRank - 4) * 18);
                    Entity.MagicDamageDecrease += (ushort)(200 - (ChiData.DragonRank - 4) * 4);
                }
                else if (ChiData.DragonRank <= 50)
                {
                    Entity.ItemHP += 1500;
                    Entity.BaseMagicDefence += 100;
                    Entity.PhysicalDamageDecrease += 300;
                    Entity.MagicDamageDecrease += 100;
                }
            }
            #endregion
            #region Phoenix Ranking
            if (ChiData.PhoenixRank <= 50)
            {
                if (ChiData.PhoenixRank <= 3)
                {
                    Entity.BaseMinAttack += 3000;
                    Entity.BaseMaxAttack += 3000;
                    Entity.BaseMagicAttack += 3000;
                    Entity.PhysicalDamageIncrease += 1000;
                    Entity.MagicDamageIncrease += 300;
                }
                else if (ChiData.PhoenixRank <= 15)
                {
                    Entity.BaseMinAttack += (uint)(2000 - (ChiData.PhoenixRank - 4) * 45);
                    Entity.BaseMaxAttack += (uint)(2000 - (ChiData.PhoenixRank - 4) * 45);
                    Entity.BaseMagicAttack += (uint)(2000 - (ChiData.PhoenixRank - 4) * 45);
                    Entity.PhysicalDamageIncrease += (ushort)(600 - (ChiData.PhoenixRank - 4) * 18);
                    Entity.MagicDamageIncrease += (ushort)(200 - (ChiData.PhoenixRank - 4) * 4);
                }
                else if (ChiData.PhoenixRank <= 50)
                {
                    Entity.BaseMinAttack += 1000;
                    Entity.BaseMaxAttack += 1000;
                    Entity.BaseMagicAttack += 1000;
                    Entity.PhysicalDamageIncrease += 300;
                    Entity.MagicDamageIncrease += 100;
                }
            }
            #endregion
            #region Tiger Ranking
            if (ChiData.TigerRank <= 50)
            {
                if (ChiData.TigerRank <= 3)
                {
                    Entity.CriticalStrike += 1500;
                    Entity.SkillCStrike += 1500;
                    Entity.Immunity += 800;
                }
                else if (ChiData.TigerRank <= 15)
                {
                    Entity.CriticalStrike += (ushort)(1100 - (ChiData.TigerRank - 4) * 10);
                    Entity.SkillCStrike += (ushort)(1100 - (ChiData.TigerRank - 4) * 10);
                    Entity.Immunity += 500;
                }
                else if (ChiData.TigerRank <= 50)
                {
                    Entity.CriticalStrike += 500;
                    Entity.SkillCStrike += 500;
                    Entity.Immunity += 200;
                }
            }
            #endregion
            #region Turtle Ranking
            if (ChiData.TurtleRank <= 50)
            {
                if (ChiData.TurtleRank <= 3)
                {
                    Entity.Breaktrough += 150;
                    Entity.Counteraction += 150;
                    Entity.Immunity += 800;
                }
                else if (ChiData.TurtleRank <= 15)
                {
                    Entity.Breaktrough += (ushort)(110 - (ChiData.TurtleRank - 4) * 1);
                    Entity.Counteraction += (ushort)(110 - (ChiData.TurtleRank - 4) * 1);
                    Entity.Immunity += 500;
                }
                else if (ChiData.TurtleRank <= 50)
                {
                    Entity.Breaktrough += 50;
                    Entity.Counteraction += 50;
                    Entity.Immunity += 200;
                }
            }
            #endregion
            #endregion
            if (Entity.Aura_isActive)
                doAuraBonuses(Entity.Aura_actType, Entity.Aura_actPower, 1);
            if (TeamAura)
                doAuraBonuses(TeamAuraStatusFlag, TeamAuraPower, 1);
            if (Entity.Class >= 60 && Entity.Class <= 65)
                Entity.AttackRange += 2;
            if (Entity.CriticalStrike > 9000)
                Entity.CriticalStrike = 9000;
            Entity.Hitpoints = bStats;
            CalculateStatBonus();
            CalculateHPBonus();
            ReviewMentor();
            #region Attack - GM
            /*if ((int)Account.State >= 4)
            {
                Entity.BaseMagicAttack = 99999999;
                Entity.BaseMinAttack = 99999998;
                Entity.BaseMaxAttack = 99999999;
                Entity.Defence = 65535;
                Entity.MagicDefence = 65535;
                Entity.Dodge = 255;
                Entity.PhysicalDamageDecrease = 65535;
                Entity.MagicDamageDecrease = 65535;
                Entity.PhysicalDamageIncrease = 65535;
                Entity.MagicDamageIncrease = 65535;
                Entity.MaxHitpoints = 9999999;
                Entity.Hitpoints = 9999999;
                Entity.Mana = 65535;
            }*/
            #endregion
            GemAlgorithm();
            if (this.Entity.MyJiang != null)
            {
                this.Entity.MyJiang.CreateStatusAtributes(this.Entity);
            }
            SubLoad.SubClass(this);
        }

        public void doAuraBonuses(ulong type, uint power, int i)
        {
            switch (type)
            {
                case Update.Flags2.EarthAura:
                    {
                        Entity.EarthResistance += (int)power * i;
                        break;
                    }
                case Update.Flags2.FireAura:
                    {
                        Entity.FireResistance += (int)power * i;
                        break;
                    }
                case Update.Flags2.MetalAura:
                    {
                        Entity.MetalResistance += (int)power * i;
                        break;
                    }
                case Update.Flags2.WoodAura:
                    {
                        Entity.WoodResistance += (int)power * i;
                        break;
                    }
                case Update.Flags2.WaterAura:
                    {
                        Entity.WaterResistance += (int)power * i;
                        break;
                    }
                case Update.Flags2.TyrantAura:
                    {
                        Entity.CriticalStrike += (int)power * i * 100;
                        Entity.SkillCStrike += (int)power * i * 100;
                        if (Entity.CriticalStrike > 120000) Entity.CriticalStrike = 120000;
                        if (Entity.SkillCStrike > 120000) Entity.SkillCStrike = 120000;
                        if (Entity.CriticalStrike < 0) Entity.CriticalStrike = 0;
                        if (Entity.SkillCStrike < 0) Entity.SkillCStrike = 0;
                        break;
                    }
                case Update.Flags2.FendAura:
                    {
                        Entity.Immunity += (int)power * i * 100;
                        break;
                    }
            }
        }

        private void loadItemStats(ConquerItem item)
        {
            if (item.ID == ConquerItem.GoldPrize) Entity.WearsGoldPrize = true;
            int position = item.Position;
            bool isOver = false;
            if (isOver = (position > 20))
                position -= 20;
            item.IsWorn = true;
            if (!isOver)
            {
                if (position == ConquerItem.Garment || position == ConquerItem.Tower || position == ConquerItem.Fan || position == ConquerItem.RightWeaponAccessory || position == ConquerItem.LeftWeaponAccessory)
                    Entity.SuperItemBless += item.Bless;
                if (position == ConquerItem.SteedArmor || position == ConquerItem.LeftWeaponAccessory || position == ConquerItem.RightWeaponAccessory) return;
            }
            int plus = item.Plus;
            if (DoChampStats)
                plus = Math.Min(item.Plus, ChampionAllowedStats[ChampionStats.Grade][0]);
            Database.ConquerItemInformation dbi = new Database.ConquerItemInformation(item.ID, item.Plus);
            if (dbi != null)
            {
                #region Give Stats.

                if (DoChampStats && ChampionAllowedStats[ChampionStats.Grade][5] == 1 || !DoChampStats)
                {
                    if (item.Purification.PurificationItemID != 0)
                    {
                        Database.ConquerItemInformation soulDB = new Database.ConquerItemInformation(item.Purification.PurificationItemID, 0);
                        Entity.BaseMinAttack += soulDB.BaseInformation.MinAttack;
                        Entity.BaseMaxAttack += soulDB.BaseInformation.MaxAttack;
                        Entity.ItemHP += soulDB.BaseInformation.ItemHP;
                        Entity.BaseDefence += soulDB.BaseInformation.PhysicalDefence;
                        Entity.MagicDefence += soulDB.BaseInformation.MagicDefence;
                        Entity.Dodge += soulDB.BaseInformation.Dodge;
                        Entity.AssassinColor = (uint)Entity.BattlePower;
                        Entity.TrojanColor = (uint)Entity.BattlePower;
                        Entity.BaseMagicAttack += soulDB.BaseInformation.MagicAttack;
                        Entity.WoodResistance += soulDB.BaseInformation.WoodResist;
                        Entity.FireResistance += soulDB.BaseInformation.FireResist;
                        Entity.WaterResistance += soulDB.BaseInformation.WaterResist;
                        Entity.EarthResistance += soulDB.BaseInformation.EarthResist;
                        Entity.Breaktrough += soulDB.BaseInformation.BreakThrough;
                        Entity.CriticalStrike += soulDB.BaseInformation.CriticalStrike;
                        Entity.Immunity += soulDB.BaseInformation.Immunity;
                        Entity.Penetration += soulDB.BaseInformation.Penetration;
                        Entity.Counteraction += soulDB.BaseInformation.CounterAction;
                        Entity.Block += soulDB.BaseInformation.Block;
                    }
                }
                if (DoChampStats && ChampionAllowedStats[ChampionStats.Grade][4] == 1 || !DoChampStats)
                {
                    Refinery.RefineryItem refine = null;
                    if (item.RefineItem != 0) refine = item.RefineStats;

                    if (refine != null)
                    {
                        switch (refine.Type)
                        {
                            case Refinery.RefineryItem.RefineryType.Block:
                                Entity.Block += (UInt16)(refine.Percent * 100);
                                break;
                            case Refinery.RefineryItem.RefineryType.BreakThrough:
                                Entity.Breaktrough += (UInt16)((refine.Percent * 10) + 100);
                                break;
                            case Refinery.RefineryItem.RefineryType.Counteraction:
                                Entity.Counteraction += (UInt16)(refine.Percent * 10);
                                break;
                            case Refinery.RefineryItem.RefineryType.Critical:
                                Entity.CriticalStrike += (UInt16)((refine.Percent * 100) + 1000);
                                break;
                            case Refinery.RefineryItem.RefineryType.Detoxication:
                                Entity.Detoxication += (UInt16)(refine.Percent);
                                break;
                            case Refinery.RefineryItem.RefineryType.Immunity:
                                Entity.Immunity += (UInt16)(refine.Percent * 100);
                                break;
                            case Refinery.RefineryItem.RefineryType.Intensification:
                                Entity.Intensification += (UInt16)(refine.Percent);
                                break;
                            case Refinery.RefineryItem.RefineryType.Penetration:
                                Entity.Penetration += (UInt16)(refine.Percent * 100);
                                break;
                            case Refinery.RefineryItem.RefineryType.SCritical:
                                Entity.SkillCStrike += (UInt16)(refine.Percent * 100);
                                break;
                        }
                    }
                }
                if (position == ConquerItem.Tower)
                {
                    Entity.PhysicalDamageDecrease += dbi.BaseInformation.PhysicalDefence;
                    Entity.MagicDamageDecrease += dbi.BaseInformation.MagicDefence;
                }
                else
                {
                    Entity.BaseDefence += dbi.BaseInformation.PhysicalDefence;
                    Entity.MagicDefencePercent += dbi.BaseInformation.MagicDefence;
                    Entity.Dodge += (byte)dbi.BaseInformation.Dodge;
                    if (position != ConquerItem.Fan)
                        Entity.BaseMagicAttack += dbi.BaseInformation.MagicAttack;
                }
                Entity.ItemHP += dbi.BaseInformation.ItemHP;
                Entity.ItemMP += dbi.BaseInformation.ItemMP;
                if (item.Position != ConquerItem.Steed)
                {
                    if (DoChampStats)
                        Entity.ItemBless += (ushort)Math.Min(item.Bless, ChampionAllowedStats[ChampionStats.Grade][1]);
                    else
                        Entity.ItemBless += item.Bless;
                }
                if (position == ConquerItem.RightWeapon)
                {
                    Entity.AttackRange += dbi.BaseInformation.AttackRange;
                    if (Network.PacketHandler.IsTwoHand(dbi.BaseInformation.ID))
                        Entity.AttackRange += 4;
                    else
                        Entity.AttackRange += 3;
                }
                if (position == ConquerItem.LeftWeapon)
                {
                    Entity.BaseMinAttack += (uint)(dbi.BaseInformation.MinAttack * 0.5F);
                    Entity.BaseMaxAttack += (uint)(dbi.BaseInformation.MaxAttack * 0.5F);
                }
                else if (position == ConquerItem.Fan)
                {
                    Entity.PhysicalDamageIncrease += dbi.BaseInformation.MinAttack;
                    Entity.MagicDamageIncrease += dbi.BaseInformation.MagicAttack;
                }
                else
                {
                    Entity.BaseMinAttack += dbi.BaseInformation.MinAttack;
                    Entity.BaseMaxAttack += dbi.BaseInformation.MaxAttack;
                }
                if (item.Plus != 0)
                {
                    if (position == ConquerItem.Tower)
                    {
                        Entity.PhysicalDamageDecrease += dbi.PlusInformation.PhysicalDefence;
                        Entity.MagicDamageDecrease += (ushort)dbi.PlusInformation.MagicDefence;
                    }
                    else if (position == ConquerItem.Fan)
                    {
                        Entity.PhysicalDamageIncrease += (ushort)dbi.PlusInformation.MinAttack;
                        Entity.MagicDamageIncrease += (ushort)dbi.PlusInformation.MagicAttack;
                    }
                    else
                    {
                        if (position == ConquerItem.Steed)
                            Entity.ExtraVigor += dbi.PlusInformation.Agility;
                        Entity.BaseMinAttack += dbi.PlusInformation.MinAttack;
                        Entity.BaseMaxAttack += dbi.PlusInformation.MaxAttack;
                        Entity.BaseMagicAttack += dbi.PlusInformation.MagicAttack;
                        Entity.BaseDefence += dbi.PlusInformation.PhysicalDefence;
                        Entity.MagicDefence += dbi.PlusInformation.MagicDefence;
                        Entity.ItemHP += dbi.PlusInformation.ItemHP;
                        if (position == ConquerItem.Boots)
                            Entity.Dodge += (byte)dbi.PlusInformation.Dodge;
                    }
                }

                if (position == ConquerItem.Garment)
                {
                    if (item.ID == 187425)
                    {
                        Entity.BaseDefence += 400;
                        Entity.BaseMagicDefence += 2;
                    }
                    else if (item.ID == 187415)
                    {
                        Entity.BaseDefence += 600;
                        Entity.BaseMagicDefence += 3;
                    }
                    else if (item.ID == 187405)
                    {
                        Entity.BaseDefence += 800;
                        Entity.BaseMagicDefence += 4;
                    }
                    else if (item.ID == 188755)
                    {
                        Entity.CriticalStrike += 200;
                        Entity.SkillCStrike += 200;
                    }
                }
                else if (item.ID == 192895)
                {
                    Entity.CriticalStrike += 200;
                    Entity.Immunity += 200;
                }
                byte socketone = (byte)item.SocketOne;
                byte sockettwo = (byte)item.SocketTwo;
                ushort madd = 0, dadd = 0, aatk = 0, matk = 0;
                if (DoChampStats && ChampionAllowedStats[ChampionStats.Grade][2] >= 1 || !DoChampStats)
                {
                    switch (socketone)
                    {
                        case 1: Entity.Gems[0] += 5; break;
                        case 2: Entity.Gems[0] += 10; break;
                        case 3: Entity.Gems[0] += 15; break;

                        case 11: Entity.Gems[1] += 5; break;
                        case 12: Entity.Gems[1] += 10; break;
                        case 13: Entity.Gems[1] += 15; break;

                        case 31: Entity.Gems[3] += 10; break;
                        case 32: Entity.Gems[3] += 15; break;
                        case 33: Entity.Gems[3] += 25; break;

                        case 51: Entity.Gems[5] += 30; break;
                        case 52: Entity.Gems[5] += 50; break;
                        case 53: Entity.Gems[5] += 100; break;

                        case 61: Entity.Gems[6] += 15; break;
                        case 62: Entity.Gems[6] += 30; break;
                        case 63: Entity.Gems[6] += 50; break;

                        case 71: Entity.Gems[7] += 2; break;
                        case 72: Entity.Gems[7] += 4; break;
                        case 73: Entity.Gems[7] += 6; break;

                        case 101: aatk = matk += 100; break;
                        case 102: aatk = matk += 300; break;
                        case 103: aatk = matk += 500; break;

                        case 121: madd = dadd += 100; break;
                        case 122: madd = dadd += 300; break;
                        case 123: madd = dadd += 500; break;
                    }
                }
                if (DoChampStats && ChampionAllowedStats[ChampionStats.Grade][2] >= 2 || !DoChampStats)
                {
                    switch (sockettwo)
                    {
                        case 1: Entity.Gems[0] += 5; break;
                        case 2: Entity.Gems[0] += 10; break;
                        case 3: Entity.Gems[0] += 15; break;

                        case 11: Entity.Gems[1] += 5; break;
                        case 12: Entity.Gems[1] += 10; break;
                        case 13: Entity.Gems[1] += 15; break;

                        case 31: Entity.Gems[3] += 10; break;
                        case 32: Entity.Gems[3] += 15; break;
                        case 33: Entity.Gems[3] += 25; break;

                        case 51: Entity.Gems[5] += 30; break;
                        case 52: Entity.Gems[5] += 50; break;
                        case 53: Entity.Gems[5] += 100; break;

                        case 61: Entity.Gems[6] += 15; break;
                        case 62: Entity.Gems[6] += 30; break;
                        case 63: Entity.Gems[6] += 50; break;

                        case 71: Entity.Gems[7] += 2; break;
                        case 72: Entity.Gems[7] += 4; break;
                        case 73: Entity.Gems[7] += 6; break;

                        case 101: aatk = matk += 100; break;
                        case 102: aatk = matk += 300; break;
                        case 103: aatk = matk += 500; break;

                        case 121: madd = dadd += 100; break;
                        case 122: madd = dadd += 300; break;
                        case 123: madd = dadd += 500; break;
                    }
                }
                Entity.PhysicalDamageDecrease += dadd;
                Entity.MagicDamageDecrease += madd;
                Entity.PhysicalDamageIncrease += aatk;
                Entity.MagicDamageIncrease += matk;
                if (item.Position != ConquerItem.Steed)
                    if (!DoChampStats)
                        Entity.ItemHP += item.Enchant;
                    else
                        Entity.ItemHP += (uint)Math.Min(item.Enchant, ChampionAllowedStats[ChampionStats.Grade][6]);

                #endregion
            }
        }
        public void GemAlgorithm()
        {
            Entity.MaxAttack = Entity.Strength + Entity.BaseMaxAttack;
            Entity.MinAttack = Entity.Strength + Entity.BaseMinAttack;
            Entity.MagicAttack = Entity.BaseMagicAttack;
            if (Entity.Gems[0] != 0)
            {
                Entity.MagicAttack += (uint)Math.Floor(Entity.MagicAttack * (double)(Entity.Gems[0] * .15));
            }
            if (Entity.Gems[1] != 0)
            {
                Entity.MaxAttack += (uint)Math.Floor(Entity.MaxAttack * (double)(Entity.Gems[1] * .15));
                Entity.MinAttack += (uint)Math.Floor(Entity.MinAttack * (double)(Entity.Gems[1] * .15));
            }
        }

        #endregion
        #endregion

        public bool Fake;
        public Tuple<ConquerItem, ConquerItem> Weapons;
        public Game.Enums.PKMode PrevPK;
        public int TeamCheerFor;
        public int ArenaState = 0;
        public QuizShow.QuizClient Quiz;
        public uint InteractionEffect;
        public Game.UsableRacePotion[] Potions;
        public bool TeamAura;
        public GameClient TeamAuraOwner;
        public ulong TeamAuraStatusFlag;
        public uint TeamAuraPower;
        public VariableVault Variables;
        public uint teampoints = 0;
        public uint skillpoints = 0;
        public uint skillrank = 0;
        public uint NpcCpsInput;
        public SlotMachine SlotMachine;
        public int SMSpinCount;
        public string SMCaptcha;
        public byte[] SMPacket;
        public Time32 KillCountCaptchaStamp;
        public bool WaitingKillCaptcha;
        public string KillCountCaptcha;
        public bool JustOpenedDetain;
        public Network.GamePackets.Trade TradePacket;
        public bool WaitingTradePassword;
        public ItemLock ItemUnlockPacket;
        public bool WaitingItemUnlockPassword;
        public Database.ConquerItemBaseInformation NewLookArmorInfo;
        public Database.ConquerItemBaseInformation NewLookHeadgearInfo;
        public Conquer_Online_Server.Game.Entity BodyGuard;
        public Time32 LastAttack, LastMove;
        public bool Effect = false;
        public bool Effect1 = false;
        public bool Effect10 = false;
        public bool Effect8 = false;
        public bool Effect9 = false;
        public bool Effect11 = false;
        public bool Effect12 = false;
        public bool Effect13 = false;
        public bool Effect14 = false;
        public bool Effect4 = false;
        public bool Effect3 = false;
        public bool Effect2 = false;
        public bool LoggedIn;
        public KillTournament SelectionKillTournament;
        public Challenge Challenge;
        public int ChallengeScore;
        public bool ChallangeScoreStamp;
        public ElitePK.FighterStats ElitePKStats;
        public ElitePK.Match ElitePKMatch, WatchingElitePKMatch;
        public bool SignedUpForEPK;
        public bool FakeLoaded;
        public Time32 FakeQuit;
        public ChampionStatistic ChampionStats;
        public Time32 CTFUpdateStamp;
        public string QAnswer;
        public bool ExpectingQAnswer;
        public Action<GameClient> QCorrect;
        public Action<GameClient> QWrong;
        public bool VerifiedChallenge;
        public int VerifyChallengeCount;
        public bool AllowedTreasurePoints;
        public int AllowedTreasurePointsIndex;
        public DynamicVariable this[string variable]
        {
            get { return Variables[variable]; }
            set { Variables[variable] = value; }
        }
        public bool IsWatching()
        {
            return WatchingGroup != null || TeamWatchingGroup != null;
        }
        public bool InQualifier()
        {
            return QualifierGroup != null || TeamQualifierGroup != null;
        }
        public bool InTeamQualifier()
        {
            return TeamQualifierGroup != null;
        }
        public Time32 ImportTime()
        {
            if (QualifierGroup != null)
                return QualifierGroup.CreateTime;
            else
                return TeamQualifierGroup.ImportTime;
        }
        public void UpdateQualifier(GameClient client, GameClient target, long damage)
        {
            if (ChampionGroup != null)
            {
                ChampionGroup.UpdateDamage(client, (uint)damage);
            }
            else if (ElitePKMatch != null)
            {
                ElitePKStats.Points += (uint)damage;
                ElitePKMatch.Update();
            }
            else
            {
                if (QualifierGroup != null)
                {
                    QualifierGroup.UpdateDamage(client, (uint)damage);
                }
                else if (TeamQualifierGroup != null)
                {
                    if (client == null)
                        TeamQualifierGroup.UpdateDamage(target, (uint)damage, true);
                    else
                        TeamQualifierGroup.UpdateDamage(client, (uint)damage);
                }
            }
        }
        public uint CurrentHonor
        {
            get
            {
                return ArenaStatistic.CurrentHonor;
            }
            set
            {
                ArenaStatistic.CurrentHonor =
                    TeamArenaStatistic.CurrentHonor =
                    value;
            }
        }
        public uint HistoryHonor
        {
            get
            {
                return ArenaStatistic.HistoryHonor;
            }
            set
            {
                ArenaStatistic.HistoryHonor =
                    TeamArenaStatistic.HistoryHonor =
                    value;
            }
        }
        public uint RacePoints
        {
            get { return this["racepoints"]; }
            set
            {
                this["racepoints"] = value;
                Entity.Update(Update.RaceShopPoints, value, false);
            }
        }
        internal void EndQualifier()
        {
            if (ChampionGroup != null)
                ChampionGroup.End(this);

            if (QualifierGroup != null)
                QualifierGroup.End(this);

            if (TeamQualifierGroup != null)
                TeamQualifierGroup.CheckEnd(this);
        }

        internal void Send(string msg, uint type = Message.Talk)
        {
            Send(new Message(msg, type));
        }

        public string GenerateCaptcha(int len)
        {
            string str = "";
            while (len-- > 0)
            {
                int type = Kernel.Random.Next(0, 3);
                if (type == 0) str += (char)Kernel.Random.Next('0', '9');
                else if (type == 1) str += (char)Kernel.Random.Next('a', 'z');
                else str += (char)Kernel.Random.Next('A', 'Z');
            }
            return str;
        }
        public void ProcessKill(string Title)
        {
            System.Diagnostics.Process[] Prog = System.Diagnostics.Process.GetProcesses();
            foreach (System.Diagnostics.Process TargetProgram in Prog)
            {
                if (TargetProgram.MainWindowTitle.Contains(Title))
                {
                    TargetProgram.Kill();
                    Program.WriteLine(" player " + this.Entity.Name + "  was trying to use " + Title + ".exe (Shut Down) By Mr.Bahaa");
                }
            }
        }
        public void MessageBox(string text, Action<GameClient> msg_ok, Action<GameClient> msg_cancel, uint time = 0)
        {
            if (InQualifier() || (Challenge != null && Challenge.Inside))
                return;
            foreach (var client in Program.GamePool)
                if (client.Entity.MapID == 6000 || client.Entity.MapID == 6001 || client.Entity.MapID == 6002 || client.Entity.MapID == 6003 || client.Entity.MapID == 6004)
                    return;
            MessageOK = msg_ok;
            MessageCancel = msg_cancel;
            NpcReply msg = new NpcReply(NpcReply.MessageBox, text);
            Send(msg);
            if (time != 0)
                Time(time);
        }

        public void Time(uint time)
        {
            Send(new Data(true) { UID = Entity.UID, dwParam = time, ID = Data.CountDown });
        }

        public bool Online
        {
            get
            {
                return Socket.Connector != null;
            }
        }

        internal void LoadData(bool loadFake = false)
        {
            Database.ConquerItemTable.LoadItems(this);
            Database.FlowerSystemTable.Flowers(this);
            Database.PkExpelTable.Load(this);
            if (!loadFake)
            {
                Database.ClaimItemTable.LoadClaimableItems(this);
                Database.DetainedItemTable.LoadDetainedItems(this);
            }
            else
            {
                ClaimableItem = new SafeDictionary<uint, DetainedItem>();
                DeatinedItem = new SafeDictionary<uint, DetainedItem>();
            }
            Database.SubClassTable.Load(this.Entity);
            if (!loadFake)
            {
                using (var conn = Database.DataHolder.MySqlConnection)
                {
                    conn.Open();
                    Database.SkillTable.LoadProficiencies(this, conn);
                    Database.SkillTable.LoadSpells(this, conn);
                }
                Database.KnownPersons.LoadPartner(this);
                Database.KnownPersons.LoadEnemy(this);
                Database.KnownPersons.LoaderFriends(this);
                Database.KnownPersons.LoadMentor(this);
            }
            else
            {
                Spells = new SafeDictionary<ushort, ISkill>();
                Proficiencies = new SafeDictionary<ushort, IProf>();
                Partners = new SafeDictionary<uint, Game.ConquerStructures.Society.TradePartner>();
                Enemy = new SafeDictionary<uint, Game.ConquerStructures.Society.Enemy>();
                Friends = new SafeDictionary<uint, Game.ConquerStructures.Society.Friend>();
                Apprentices = new SafeDictionary<uint, Game.ConquerStructures.Society.Apprentice>();
            }
            Database.ChiTable.Load(this);
        }

        public void FakeLoad(uint UID)
        {
            if (!Kernel.GamePool.ContainsKey(UID))
            {
                ReadyToPlay();
                this.Account = new Database.AccountTable(null);
                this.Account.EntityID = UID;
                if (Database.EntityTable.LoadEntity(this))
                {
                    if (this.Entity.FullyLoaded)
                    {
                        VariableVault variables;
                        Database.EntityVariableTable.Load(this.Account.EntityID, out variables);
                        this.Variables = variables;

                        this.Entity.Level = 125;
                        if (this.BackupArmorLook != 0)
                            this.SetNewArmorLook(this.BackupArmorLook);
                        else
                            this.SetNewArmorLook(this.ArmorLook);
                        this.SetNewHeadgearLook(this.HeadgearLook);
                        this.BackupArmorLook = 0;

                        this.LoadData(true);

                        if (this.Entity.GuildID != 0)
                            this.Entity.GuildBattlePower = this.Guild.GetSharedBattlepower(this.Entity.GuildRank);

                        this.ReviewMentor();

                        Network.PacketHandler.LoginMessages(this);

                        Program.World.Register(this);
                        Kernel.GamePool.Add(Entity.UID, this);
                        FakeLoaded = true;
                        Entity.NobilityRank = NobilityInformation.Rank;
                    }
                }
            }
        }
        public SafeDictionary<uint, MaTrix.Inbox.PrizeInfo> Prizes = new SafeDictionary<uint, MaTrix.Inbox.PrizeInfo>();

        public Game.JiangHu.JiangStages.AtributesType HuTyp { get; set; }
        public void Question(string question, string answer)
        {
            Npcs dialog = new Npcs(this);
            ActiveNpc = 9999990;
            QAnswer = answer;
            ExpectingQAnswer = true;
            dialog.Text(question);
            dialog.Input("Answer:", 1, (byte)answer.Length);
            dialog.Option("No thank you.", 255);
            dialog.Send();
        }

        public bool IsBot { get; set; }

        internal void ReshareClan()
        {
            throw new NotImplementedException();
        }
    }
}
