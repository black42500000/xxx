using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Conquer_Online_Server.Network;
using Conquer_Online_Server.Network.GamePackets;
using System.Collections;
using Conquer_Online_Server.Interfaces;

using Conquer_Online_Server.Database;
using System.Collections.Concurrent;
using Conquer_Online_Server.Client;

namespace Conquer_Online_Server.Game
{
    public class Entity : Writer, Interfaces.IBaseEntity, Interfaces.IMapObject
    {
        public List<Game.ConquerStructures.ChiRetreatStructure> RetreatChiPowers = new List<Game.ConquerStructures.ChiRetreatStructure>();
        public SafeDictionary<uint, Entity> MyClones = new SafeDictionary<uint, Entity>();
        public bool IsClone { get; set; }
        public void AddClone(string Name, ushort cloneid)
        {
            var Entity = new Entity(EntityFlag.Monster, true);
            Entity.Owner = Owner;
            Entity.MonsterInfo = new MonsterInformation();
            MonsterInformation.MonsterInformations.TryGetValue(9003, out Entity.MonsterInfo);
            Entity.MonsterInfo.Owner = Entity;
            Entity.IsClone = true;
            Entity.ClanName = NaIme;
            Entity.Name = Name;
            Entity.GuildID = GuildID;
            Entity.GuildRank = GuildRank;
            Entity.NobilityRank = NobilityRank;
            Entity.HairStyle = HairStyle;
            Entity.HairColor = HairColor;
            Entity.AddFlag(Network.GamePackets.Update.Flags.Invisibility);
            Entity.InvisibilityTime = 60 * 4;
            Entity.InvisibilityStamp = Time32.Now;
            //Entity.TrojanBP = TrojanBP;

            #region Equip same as me
            foreach (ConquerItem item in Owner.Equipment.Objects)
            {

                if (item == null) continue;
                if (Owner.Equipment.Free(item.Position)) continue;
                if (!item.IsWorn) continue;
                switch ((ushort)item.Position)
                {
                    case Network.GamePackets.ConquerItem.AlternateHead:
                    case Network.GamePackets.ConquerItem.Head:
                        if (Owner.HeadgearLook != 0)
                        {
                            Network.Writer.WriteUInt32(0, Game.ConquerStructures.Equipment.HeadSoul, Entity.SpawnPacket);
                            Network.Writer.WriteUInt32(Owner.HeadgearLook, Game.ConquerStructures.Equipment.Head, Entity.SpawnPacket);
                        }
                        else
                        {
                            if (item.Purification.Available)
                                Network.Writer.WriteUInt32(item.Purification.PurificationItemID, Game.ConquerStructures.Equipment.HeadSoul, Entity.SpawnPacket);
                            else Network.Writer.WriteUInt32(0, Game.ConquerStructures.Equipment.HeadSoul, Entity.SpawnPacket);
                            Network.Writer.WriteUInt32(item.ID, Game.ConquerStructures.Equipment.Head, Entity.SpawnPacket);
                        }
                        Network.Writer.WriteUInt16((byte)item.Color, Game.ConquerStructures.Equipment.HeadColor, Entity.SpawnPacket);
                        break;
                    case Network.GamePackets.ConquerItem.AlternateGarment:
                    case Network.GamePackets.ConquerItem.Garment:
                        Network.Writer.WriteUInt32(item.ID, Game.ConquerStructures.Equipment.Garment, Entity.SpawnPacket);
                        break;
                    case Network.GamePackets.ConquerItem.AlternateArmor:
                    case Network.GamePackets.ConquerItem.Armor:
                        if (Owner.ArmorLook != 0)
                        {
                            Network.Writer.WriteUInt32(0, Game.ConquerStructures.Equipment.ArmorSoul, Entity.SpawnPacket);
                            Network.Writer.WriteUInt32(Owner.ArmorLook, Game.ConquerStructures.Equipment.Armor, Entity.SpawnPacket);
                        }
                        else
                        {
                            if (item.Purification.Available)
                                Network.Writer.WriteUInt32(item.Purification.PurificationItemID, Game.ConquerStructures.Equipment.ArmorSoul, Entity.SpawnPacket);
                            else Network.Writer.WriteUInt32(0, Game.ConquerStructures.Equipment.ArmorSoul, Entity.SpawnPacket);
                            Network.Writer.WriteUInt32(item.ID, Game.ConquerStructures.Equipment.Armor, Entity.SpawnPacket);
                        }
                        Network.Writer.WriteUInt16((byte)item.Color, Game.ConquerStructures.Equipment.ArmorColor, Entity.SpawnPacket);
                        break;
                    case Network.GamePackets.ConquerItem.AlternateRightWeapon:
                    case Network.GamePackets.ConquerItem.RightWeapon:
                        if (item.Purification.Available)
                            Network.Writer.WriteUInt32(item.Purification.PurificationItemID, Game.ConquerStructures.Equipment.RightWeaponSoul, Entity.SpawnPacket);
                        else
                            Network.Writer.WriteUInt32(0, Game.ConquerStructures.Equipment.RightWeaponSoul, Entity.SpawnPacket);
                        Network.Writer.WriteUInt32(item.ID, Game.ConquerStructures.Equipment.RightWeapon, Entity.SpawnPacket);
                        if (Network.PacketHandler.IsTwoHand(item.ID))
                        {
                            Network.Writer.WriteUInt32(0, Game.ConquerStructures.Equipment.LeftWeaponSoul, Entity.SpawnPacket);
                            Network.Writer.WriteUInt32(0, Game.ConquerStructures.Equipment.LeftWeapon, Entity.SpawnPacket);
                            Network.Writer.WriteUInt16(0, Game.ConquerStructures.Equipment.LeftWeaponColor, Entity.SpawnPacket);
                        }
                        break;
                    case Network.GamePackets.ConquerItem.RightWeaponAccessory:
                        Network.Writer.WriteUInt32(item.ID, Game.ConquerStructures.Equipment.RightWeaponAccessory, Entity.SpawnPacket);
                        break;
                    case Network.GamePackets.ConquerItem.AlternateLeftWeapon:
                    case Network.GamePackets.ConquerItem.LeftWeapon:
                        if (item.Purification.Available)
                            Network.Writer.WriteUInt32(item.Purification.PurificationItemID, Game.ConquerStructures.Equipment.LeftWeaponSoul, Entity.SpawnPacket);
                        else
                            Network.Writer.WriteUInt32(0, Game.ConquerStructures.Equipment.LeftWeaponSoul, Entity.SpawnPacket);

                        Network.Writer.WriteUInt32(item.ID, Game.ConquerStructures.Equipment.LeftWeapon, Entity.SpawnPacket);
                        Network.Writer.WriteUInt16((byte)item.Color, Game.ConquerStructures.Equipment.LeftWeaponColor, Entity.SpawnPacket);
                        break;
                    case Network.GamePackets.ConquerItem.LeftWeaponAccessory:
                        Network.Writer.WriteUInt32(item.ID, Game.ConquerStructures.Equipment.LeftWeaponAccessory, Entity.SpawnPacket);
                        break;
                    case Network.GamePackets.ConquerItem.Steed:
                        Network.Writer.WriteUInt32(item.ID, Game.ConquerStructures.Equipment.Steed, Entity.SpawnPacket);
                        Network.Writer.WriteUInt16((byte)item.Plus, Game.ConquerStructures.Equipment.SteedPlus, Entity.SpawnPacket);
                        Network.Writer.WriteUInt32(item.SocketProgress, Game.ConquerStructures.Equipment.SteedColor, Entity.SpawnPacket);
                        break;
                    case Network.GamePackets.ConquerItem.SteedArmor:
                        Network.Writer.WriteUInt32(item.ID, Game.ConquerStructures.Equipment.MountArmor, Entity.SpawnPacket);
                        break;
                }
            }
            #endregion Equip same as me


            Entity.MinAttack = MinAttack;
            Entity.MaxAttack = Entity.MagicAttack = Math.Max(MinAttack, MaxAttack);
            Entity.Hitpoints = Entity.MaxHitpoints = Hitpoints;
            Entity.Body = Body;

            Entity.UID = 703400 + Owner.Map.CloneCounter.Next;
            //Entity.UID = identity;
            Entity.CUID = Owner.Entity.UID;
            Entity.SpawnPacket[Entity._CUID - 3] = 2;
            Writer.WriteUInt16(cloneid, Entity._CUID - 2, Entity.SpawnPacket);
            Entity.MapID = Owner.Map.ID;
            Entity.SendUpdates = true;
            Entity.X = Owner.Entity.X;
            Entity.Y = Owner.Entity.Y;
            if (!Owner.Map.Companions.ContainsKey(Entity.UID))
                Owner.Map.Companions.Add(Entity.UID, Entity);

            MyClones.Add(Entity.UID, Entity);

            Owner.SendScreenSpawn(Entity, true);
        }
        public uint CUID
        {
            get
            {
                if (SpawnPacket != null)
                    return BitConverter.ToUInt32(SpawnPacket, _CUID);
                else
                    return _uid;
            }
            set
            {
                _uid = value;
                WriteUInt32(value, _CUID, SpawnPacket);
            }
        }  
        public Statement.KungFuGClasses KunFuClasses = new Statement.KungFuGClasses();
        #region JaingHu
        private byte talent;
        public byte TalentStaus
        {
            get
            {
                SpawnPacket[241] = talent;
                return talent;
            }
            set
            {
                if (EntityFlag == EntityFlag.Player)
                {
                    talent = value;
                    SpawnPacket[241] = value;
                    SpawnPacket[242] = 1;
                }

            }
        }
        public uint KHP = 0;
        public uint KMP = 0;
        public uint KHPs = 0;
        public uint KMPs = 0;
        public uint KPAttack = 0;
        public uint KMAttack = 0;
        public uint KPDefence = 0;
        public uint KMDefence = 0;
        public uint KFinalPAttack = 0;
        public uint KFinalMAttack = 0;
        public uint KFinalPDamage = 0;
        public uint KFinalMDamage = 0;
        public uint KPStrike = 0;
        public uint KMStrike = 0;
        public uint KImmunty = 0;
        public uint KBreak = 0;
        public uint KAntiBreak = 0;
        #endregion
        #region VIP NAme
        public void InsertXorNameToDB(string oldName, string newName)
        {
            new Database.MySqlCommand(Database.MySqlCommandType.UPDATE).Update("entities").Set(oldName, newName).Where("UID", UID).Execute();
        }
        #endregion
        public uint Prof;
        public Time32 SuperCyclone;
        #region RebornSpells
        public bool RebornSpell(ushort skillid)
        {
            if (skillid == 9876 || skillid == 6002 || skillid == 10315 || skillid == 10311 || skillid == 10313 ||
                skillid == 6003 || skillid == 10405 || skillid == 30000 || skillid == 10310 || skillid == 3050 ||
                skillid == 3060 || skillid == 3080 || skillid == 3090)
                return true;
            else
                return false;
        }
        #endregion
        public bool BodyGuard;
        #region Top By Mr Conquer_Online_Server
        public ushort _TopPirate2 = 0;
        public ushort TopPirate2
        {
            get { return _TopPirate2; }
            set
            {
                _TopPirate2 = value;
                if (value >= 1)
                {
                    AddFlag2(Network.GamePackets.Update.Flags2.TopPirate2);
                }
            }
        }
        public ushort _Top2Warrior = 0;
        public ushort Top2Warrior
        {
            get { return _Top2Warrior; }
            set
            {
                _Top2Warrior = value;
                if (value >= 1)
                {
                    AddFlag2(Network.GamePackets.Update.Flags2.Top2Warrior);
                }
            }
        }
        public ushort _Topmonk3 = 0;
        public ushort TopMonk2
        {
            get { return _Topmonk3; }
            set
            {
                _Topmonk3 = value;
                if (value >= 1)
                {
                    AddFlag2(Network.GamePackets.Update.Flags2.Top2Monk);
                }
            }
        }
        public ushort _TopTrojan2 = 0;
        public ushort TopTrojan2
        {
            get { return _TopTrojan2; }
            set
            {
                _TopTrojan2 = value;
                if (value >= 1)
                {
                    AddFlag2(Network.GamePackets.Update.Flags2.Top2Trojan);
                }
            }
        }
        public ushort _TopWarrior2 = 0;
        public ushort TopWarrior2
        {
            get { return _TopWarrior2; }
            set
            {
                _TopWarrior2 = value;
                if (value >= 1)
                {
                    AddFlag2(Network.GamePackets.Update.Flags2.Top2Warrior);
                }
            }
        }
        public ushort _TopNinja2 = 0;
        public ushort TopNinja2
        {
            get { return _TopNinja2; }
            set
            {
                _TopNinja2 = value;
                if (value >= 1)
                {
                    AddFlag2(Network.GamePackets.Update.Flags2.Top2Ninja);
                }
            }
        }
        public ushort _TopWaterTaoist2 = 0;
        public ushort TopWaterTaoist2
        {
            get { return _TopWaterTaoist2; }
            set
            {
                _TopWaterTaoist2 = value;
                if (value >= 1)
                {
                    AddFlag2(Network.GamePackets.Update.Flags2.Top2Water);
                }
            }
        }
        public ushort _TopArcher2 = 0;
        public ushort TopArcher2
        {
            get { return _TopArcher2; }
            set
            {
                _TopArcher2 = value;
                if (value >= 1)
                {
                    AddFlag2(Network.GamePackets.Update.Flags2.Top2Archer);
                }
            }
        }
        public ushort _TopFireTaoist2 = 0;
        public ushort TopFireTaoist2
        {
            get { return _TopFireTaoist2; }
            set
            {
                _TopFireTaoist2 = value;
                if (value >= 1)
                {
                    AddFlag2(Network.GamePackets.Update.Flags2.Top2Fire);
                }
            }
        }
        #endregion
        public Dictionary<uint, MrBahaa.PkExpeliate> PkExplorerValues = new Dictionary<uint, MrBahaa.PkExpeliate>();
        public Conquer_Online_Server.Game.JiangHu MyJiang;
        #region Offsets
        public const int
                    TimeStamp = 4,
                    _Mesh = TimeStamp + 4,//8
                    _UID = _Mesh + 4,//12
                    _GuildID = _UID + 4,//16
                    _GuildRank = _GuildID + 4, //20
                    _StatusFlag = _GuildRank + 6, //26
                    _StatusFlag2 = _StatusFlag + 8,//34
                    _StatusFlag3 = _StatusFlag2 + 8,//42
                    _AppearanceType = _StatusFlag3 + 4, //46
                    _Hitpoints = 94,//94
                    _MonsterLevel = 94 + 6,//96
                    _X = _MonsterLevel + 2,// 98,
                    _Y = _X + 2, //100
                    Uint32_FlowerRank = 133,//133
                    _HairStyle = _Y + 2, //102
                    _Facing = _HairStyle + 2,// 104
                    _Action = _Facing + 1, //105
                    _Reborn = _Action + 7, //112,
                    _Level = _Reborn + 1, //113
                    _WindowSpawn = _Level + 2, //115
                    _Away = _WindowSpawn + 1,// 116
                    _ExtraBattlepower = _Away + 1,// 117
                    _FlowerIcon = _ExtraBattlepower + 12, //129
                    _NobilityRank = _FlowerIcon + 8, //137
                    _QuizPoints = _NobilityRank + 10, //147
                    _ClanUID = _QuizPoints + 26,// 173
                    _ClanRank = _ClanUID + 4, //177
                    _Title = _ClanRank + 8, //185
                    _EntitySize = _Title + 2, //187
                    _ShowArenaGlow = _EntitySize + 9,// 196
                    _Boss = _ShowArenaGlow + 3, //199,
                    _RaceItem = _Boss + 2, //201,
                    _ActiveSubclass = _RaceItem + 15, //216,
                    _FirstRebornClass = _ActiveSubclass + 9, //225,
                    _SecondRebornClass = _FirstRebornClass + 2,// 227,
                    _Class = _SecondRebornClass + 2, // 229, 
                    _CountryCode = _Class + 2,// 231,
                    _AssassinAsBattlePower = _CountryCode + 6, // 237
                    _JingHu_Talen = _AssassinAsBattlePower + 4,//241
                    _JiangHuActive = _JingHu_Talen + 1,//242
                    _ShadowID = _JiangHuActive + 7,//249
                    _Names = _ShadowID + 4;//253
        // _Names = _AssassinAsBattlePower + 5;//238;

        #endregion
        public bool DragonWarrior()
        {
            if (EntityFlag == Game.EntityFlag.Player)
            {
                var weapons = Owner.Weapons;
                if (weapons.Item1 != null)
                    if (weapons.Item1.ID / 1000 == 617)
                        return true;
                    else if (weapons.Item2 != null)
                        if (weapons.Item2.ID / 1000 == 617)
                            return true;
            }
            return false;
        }
        public uint NinjaColor
        {
            get
            {
                return BitConverter.ToUInt32(this.SpawnPacket, 237);
            }
            set
            {
                WriteUInt32(value, 237, this.SpawnPacket);
            }
        } 
        public byte RamadanEffect;
        #region SkillSoul
        public bool OpenskillSoul;
        public bool SuperSkillSoul;
        public bool SkillSoul(ushort skillid)
        {
            if (skillid == 11005 || skillid == 1001 || skillid == 1165 || skillid == 6001 || skillid == 6000 || skillid == 10309 ||
                skillid == 11650 || skillid == 1002 || skillid == 10415 || skillid == 11110 || skillid == 11670 ||
                skillid == 11600 || skillid == 10381 || skillid == 1046 || skillid == 11190 || skillid == 1045 ||
                skillid == 11000 || skillid == 5010 || skillid == 1420 || skillid == 11070 || skillid == 11040 ||
                skillid == 11130 || skillid == 11120 || skillid == 11160 || skillid == 11170 || skillid == 11180 ||
                skillid == 11200 || skillid == 11230 || skillid == 11590 || skillid == 11620 || skillid == 11660 ||
                skillid == 10310 || skillid == 10311 || skillid == 10313 || skillid == 10315 || skillid == 10395 ||
                skillid == 10400 || skillid == 10405 || skillid == 10410 || skillid == 10420 || skillid == 10421 ||
                skillid == 10422 || skillid == 10424 || skillid == 10425 || skillid == 10435 || skillid == 10430 ||
                skillid == 10470 || skillid == 10490 || skillid == 11000 || skillid == 11140 || skillid == 30000 ||
                skillid == 11610 || skillid == 11060 || skillid == 11050 || skillid == 10390 ||
                skillid == 11100 || skillid == 11030 || skillid == 1115)
                return true;
            else
                return false;
        }
        #endregion
        #region Variables
        public Time32 DragonCyclone;
        public Time32 DragonFlowStamp;
        public Time32 DragonFlowStamp2;
        public Conquer_Online_Server.Game.Features.Flowers.Flowers Flowers;
        public Conquer_Online_Server.Game.Features.Kisses.Kisses Kisses = new Conquer_Online_Server.Game.Features.Kisses.Kisses();
        public int KillCount = 0, KillCount2 = 0, KC2 = 0;
        public uint LastXLocation, LastYLocation;
        public bool InSteedRace, Invisable, IsBlackSpotted, IsEagleEyeShooted = false;
        public Database.MonsterInformation MonsterInfo;
        public Time32 DeathStamp, VortexAttackStamp, AttackStamp, StaminaStamp, FlashingNameStamp, CycloneStamp, SupermanStamp, TwoFlod, FatigueStamp, CannonBarrageStamp,
                      StigmaStamp, InvisibilityStamp, StarOfAccuracyStamp, MagicShieldStamp, DodgeStamp, EnlightmentStamp, BlackSpotStamp, BlackbeardsRageStamp, DefensiveStanceStamp,
                      AccuracyStamp, ShieldStamp, FlyStamp, NoDrugsStamp, ToxicFogStamp, FatalStrikeStamp, DoubleExpStamp, BladeTempest, MagicDefenderStamp,
                      ShurikenVortexStamp, IntensifyStamp, TransformationStamp, CounterKillStamp, PKPointDecreaseStamp, LastPopUPCheck,
                      HeavenBlessingStamp, OblivionStamp, AuraStamp, ShackleStamp, AzureStamp, StunStamp, WhilrwindKick, GuildRequest, Confuse, LastTeamLeaderLocationSent = Time32.Now,
                      BladeFlurryStamp;
        public bool IsDropped = false;
        public bool IsWatching = false;
        public bool HasMagicDefender = false;
        public bool IsDefensiveStance = false;
        public bool MagicDefenderOwner = false;
        public bool KillTheTerrorist_IsTerrorist = false;
        public bool Tournament_Signed = false;
        public DateTime MerchantDate;
        public int Merchant { get; set; }
        public int Sentagain { get; set; }
        public DateTime MerchantDay;
        public bool SpawnProtection = false;
        public bool TeamDeathMatch_Signed = false;
        public bool TeamDeathMatch_RedCaptain = false;
        public bool TeamDeathMatch_BlackCaptain = false;
        public bool TeamDeathMatch_WhiteCaptain = false;
        public bool TeamDeathMatch_BlueCaptain = false;
        public bool TeamDeathMatch_RedTeam = false;
        public bool TeamDeathMatch_BlackTeam = false;
        public bool TeamDeathMatch_WhiteTeam = false;
        public int TeamDeathMatchTeamKey
        {
            get
            {
                if (TeamDeathMatch_BlackTeam) return 0;
                else if (TeamDeathMatch_BlueTeam) return 1;
                else if (TeamDeathMatch_RedTeam) return 2;
                return 3;
            }
        }

        public Time32 BlockStamp;
        public int BlockTime;
        public int Fright;
        public Achievement MyAchievement;
        public uint InteractionType = 0;
        public uint InteractionWith = 0;
        public bool InteractionInProgress = false;
        public ushort InteractionX = 0;
        public ushort InteractionY = 0;
        public bool InteractionSet = false;
        public int CurrentTreasureBoxes = 0;
        public uint Points = 0;
        //public uint UID = 0;
        //public ushort Avatar = 0;
        //public ushort Mesh = 0;
        //public string Name = "";
        public ushort Postion = 0;
        public ConcurrentDictionary<TitlePacket.Titles, DateTime> Titles;
        public ConcurrentDictionary<int, DateTime> Halos;
        public bool IsWarTop(ulong Title)
        {
            return Title >= 11 && Title <= 19;
        }
        public void AddTopStatus(UInt64 Title, DateTime EndsOn, Boolean Db = true)
        {
            Title = TopStatusToInt(Title);
            Boolean HasFlag = false;
            if (IsWarTop(Title))
            {
                HasFlag = Titles.ContainsKey((TitlePacket.Titles)Title);
                Titles.TryAdd((TitlePacket.Titles)Title, EndsOn);
            }
            else
            {
                int T = (int)Title;
                HasFlag = Halos.ContainsKey(T);
                Halos[T] = EndsOn;
                if (Title == 7 || Title == 8
                    || Title == 23 || Title == 24
                    || Title == 25 || Title == 26
                    || Title == 27 || Title == 28
                    || Title == 29 || Title == 30
                    || Title == 31 || Title == 32
                    || Title == 33 || Title == 34
                    || Title == 35 || Title == 36
                    || Title == 37 || Title == 22) AddFlag2(IntToTopStatus(Title));
                else AddFlag(IntToTopStatus(Title));
            }
            if (Db)
            {
                if (HasFlag)
                {
                    MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
                    cmd.Update("status").Set("time", Kernel.ToDateTimeInt(EndsOn))
                        .Where("status", Title).And("entityid", (UInt32)UID);
                    cmd.Execute();
                }
                else
                {
                    MySqlCommand cmd = new MySqlCommand(MySqlCommandType.INSERT);
                    cmd.Insert("status").Insert("entityid", (UInt32)UID).Insert("status", Title).Insert("time", Kernel.ToDateTimeInt(EndsOn));
                    cmd.Execute();
                }
            }
        }
        public void RemoveTopStatus(UInt64 Title)
        {
            ulong baseFlag = TopStatusToInt(Title);
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.DELETE);
            cmd.Delete("status", "entityid", UID).And("status", baseFlag).Execute();
            if (baseFlag == 7 || baseFlag == 8
                || baseFlag == 23 || baseFlag == 24
                || baseFlag == 25 || baseFlag == 26
                || baseFlag == 27 || baseFlag == 28
                || baseFlag == 29 || baseFlag == 30
                || baseFlag == 31 || baseFlag == 32
                || baseFlag == 33 || baseFlag == 34
                || baseFlag == 35 || baseFlag == 36
                || baseFlag == 37 || baseFlag == 22) RemoveFlag2(Title);
            else RemoveFlag(Title);
        }  
        public void LoadTopStatus()
        {
            using (MySqlCommand Command = new MySqlCommand(MySqlCommandType.SELECT))
            {
                Command.Select("status").Where("entityid", UID).Execute();
                using (MySqlReader Reader = new MySqlReader(Command))
                {
                    while (Reader.Read())
                    {
                        UInt64 Title = Reader.ReadUInt64("status");
                        DateTime Time = Kernel.FromDateTimeInt(Reader.ReadUInt64("time"));
                        if (DateTime.Now > Time)
                            RemoveTopStatus(Title);
                        else
                        {
                            //if (!ContainsFlag(IntToTopStatus(Title)))
                            AddTopStatus(Title, Time, false);
                            //else DeleteStatus(Title);
                        }
                    }
                }
            }
        }
        public UInt64 TopStatusToInt(UInt64 top)
        {
            switch (top)
            {
                case Network.GamePackets.Update.Flags.TopWaterTaoist: return 1;
                case Network.GamePackets.Update.Flags.TopWarrior: return 2;
                case Network.GamePackets.Update.Flags.TopTrojan: return 3;
                case Network.GamePackets.Update.Flags.TopArcher: return 4;
                case Network.GamePackets.Update.Flags.TopNinja: return 5;
                case Network.GamePackets.Update.Flags.TopFireTaoist: return 6;
                case Network.GamePackets.Update.Flags2.TopMonk: return 7;
                case Network.GamePackets.Update.Flags.TopSpouse: return 8;
                case Network.GamePackets.Update.Flags.TopGuildLeader: return 9;
                case Network.GamePackets.Update.Flags.TopDeputyLeader: return 10;
                case Network.GamePackets.Update.Flags.MonthlyPKChampion: return 20;
                case Network.GamePackets.Update.Flags.WeeklyPKChampion: return 21;
                case Network.GamePackets.Update.Flags2.TopPirate: return 22;
                case Network.GamePackets.Update.Flags2.Top3Archer: return 23;
                case Network.GamePackets.Update.Flags2.WeeklyTop2PkBlue: return 24;
                case Network.GamePackets.Update.Flags2.Top3Fire: return 25;
                case Network.GamePackets.Update.Flags2.Top3Monk: return 26;
                case Network.GamePackets.Update.Flags2.Top3Trojan: return 27;
                case Network.GamePackets.Update.Flags2.Top3Warrior: return 28;
                case Network.GamePackets.Update.Flags2.Top3Ninja: return 29;
                case Network.GamePackets.Update.Flags2.Top2Archer: return 30;
                case Network.GamePackets.Update.Flags2.Top8Ninja: return 31;
                case Network.GamePackets.Update.Flags2.Top8Fire: return 32;
                case Network.GamePackets.Update.Flags2.Top8Archer: return 33;
                case Network.GamePackets.Update.Flags2.WeeklyTop8Pk: return 34;
                case Network.GamePackets.Update.Flags2.MonthlyTop8Pk: return 35;
                case Network.GamePackets.Update.Flags2.Top2SpouseBlue: return 36;
                case Network.GamePackets.Update.Flags2.MontlyTop3Pk: return 37;
            }
            return top;
        }
        public UInt64 IntToTopStatus(UInt64 top)
        {
            switch (top)
            {
                case 1: return Network.GamePackets.Update.Flags.TopWaterTaoist;
                case 2: return Network.GamePackets.Update.Flags.TopWarrior;
                case 3: return Network.GamePackets.Update.Flags.TopTrojan;
                case 4: return Network.GamePackets.Update.Flags.TopArcher;
                case 5: return Network.GamePackets.Update.Flags.TopNinja;
                case 6: return Network.GamePackets.Update.Flags.TopFireTaoist;
                case 7: return Network.GamePackets.Update.Flags2.TopMonk;
                case 8: return Network.GamePackets.Update.Flags.TopSpouse;
                case 9: return Network.GamePackets.Update.Flags.TopGuildLeader;
                case 10: return Network.GamePackets.Update.Flags.TopDeputyLeader;
                case 20: return Network.GamePackets.Update.Flags.MonthlyPKChampion;
                case 21: return Network.GamePackets.Update.Flags.WeeklyPKChampion;
                case 22: return Network.GamePackets.Update.Flags2.TopPirate;
                case 23: return Network.GamePackets.Update.Flags2.Top3Archer;
                case 24: return Network.GamePackets.Update.Flags2.WeeklyTop2PkBlue;
                case 25: return Network.GamePackets.Update.Flags2.Top3Fire;
                case 26: return Network.GamePackets.Update.Flags2.Top3Monk;
                case 27: return Network.GamePackets.Update.Flags2.Top3Trojan;
                case 28: return Network.GamePackets.Update.Flags2.Top3Ninja;
                case 29: return Network.GamePackets.Update.Flags2.Top3Warrior;
                case 30: return Network.GamePackets.Update.Flags2.Top2Archer;
                case 31: return Network.GamePackets.Update.Flags2.Top8Ninja;
                case 32: return Network.GamePackets.Update.Flags2.Top8Fire;
                case 33: return Network.GamePackets.Update.Flags2.Top8Archer;
                case 34: return Network.GamePackets.Update.Flags2.WeeklyTop8Pk;
                case 35: return Network.GamePackets.Update.Flags2.MonthlyTop8Pk;
                case 36: return Network.GamePackets.Update.Flags2.Top2SpouseBlue;
                case 37: return Network.GamePackets.Update.Flags2.MontlyTop3Pk;
            }
            return top;
        }  
        /*public static void MrBahaa(Client.GameClient client)
        {
            TitlePacket tpacket = new TitlePacket(false);   
            if (client.Entity.Titles.Keys.Contains(tpacket.Title) || tpacket.Title == TitlePacket.Titles.None)
            {
                client.Entity.MyTitle = client.Entity.Prof;
                client.Send(tpacket);
                //client.Entity.Teleport(client.Entity.MapID, client.Entity.X, client.Entity.Y);
            }
        }*/
        public UInt32 ActivePOPUP;
        //TitlePacket tpacket = new TitlePacket(false); 
        private uint _namechanges;
        public uint namechanges
        {
            get
            {

                return _namechanges;
            }
            set
            {
                _namechanges = value;
            }
        }  
        public TitlePacket.Titles MyTitle
        {
            get { return (TitlePacket.Titles)SpawnPacket[_Title]; }
            set
            {
                SpawnPacket[_Title] = (Byte)value;
                if (FullyLoaded2)
                {
                    MySqlCommand cmd = new MySqlCommand(MySqlCommandType.UPDATE);
                    cmd.Update("entities").Set("My_Title", (Byte)value).Where("uid", UID).Execute();
                    // ClientStats.Entity.MyTitle = value;
                    //ClientStats.Send(tpacket);
                }
            }
        }
        public Updating.Offset1 UpdateOffset1 = Updating.Offset1.None;
        public Updating.Offset2 UpdateOffset2 = Updating.Offset2.None;
        public Updating.Offset3 UpdateOffset3 = Updating.Offset3.None;
        public Updating.Offset4 UpdateOffset4 = Updating.Offset4.None;
        public Updating.Offset5 UpdateOffset5 = Updating.Offset5.None;
        public Updating.Offset6 UpdateOffset6 = Updating.Offset6.None;
        public Updating.Offset7 UpdateOffset7 = Updating.Offset7.None;
        public int DisKO = 0;
        public static byte ScreenDistance = 0;
        public bool DisQuest = false;

        public static bool dis = false;
        public static bool dis2 = false;
        public static byte DisMax1 = 0;
        public static byte DisMax2 = 0;
        public static byte DisMax3 = 0;
        public Client.GameClient ClientStats;
        public StatusStatics Statistics;
        public double DragonGems;
        public double PhoenixGems;
        public int BlackSpotStepSecs;
        public ushort Detoxication;
        public int Immunity;
        public int FatigueSecs;
        public ushort Breaktrough;
        public int CriticalStrike;
        public int SkillCStrike;
        public ushort Intensification;
        public ushort Block;
        public ushort FinalMagicDmgPlus;
        public ushort FinalMagicDmgReduct;
        public ushort FinalDmgPlus;
        public ushort FinalDmgReduct;
        public ushort Penetration;
        public ushort Counteraction;
        public int MetalResistance;
        public int WoodResistance;
        public int WaterResistance;
        public int FireResistance;
        public int EarthResistance;
        public bool TeamDeathMatch_BlueTeam = false;
        public int TeamDeathMatch_Hits = 0;
        public byte _SubClass, _SubClassLevel;
        public Game.Subclasses SubClasses = new Game.Subclasses();
        public bool Stunned = false, Confused = false;
        public bool Companion;
        public bool CauseOfDeathIsMagic = false;
        public bool OnIntensify;
        private DateTime mLastLogin;
        uint f_flower;

        public uint ActualMyTypeFlower
        {
            get
            {
                return f_flower;
            }
            set
            {
                f_flower = value;
                WriteUInt32(value, 129 + 4, this.SpawnPacket);
            }
        }
        private uint flower_R;
        public uint AddFlower
        {
            get { return flower_R; }
            set
            {
                flower_R = value;
            }
        }
        public short KOSpellTime
        {
            get
            {
                if (KOSpell == 1110)
                {
                    if (ContainsFlag(Network.GamePackets.Update.Flags.Cyclone))
                    {
                        return CycloneTime;
                    }
                }
                else if (KOSpell == 1025)
                {
                    if (ContainsFlag(Network.GamePackets.Update.Flags.Superman))
                    {
                        return SupermanTime;
                    }
                }
                return 0;
            }
            set
            {
                if (KOSpell == 1110)
                {
                    if (ContainsFlag(Network.GamePackets.Update.Flags.Cyclone))
                    {
                        int Seconds = CycloneStamp.AddSeconds(value).AllSeconds() - Time32.Now.AllSeconds();
                        if (Seconds >= 20)
                        {
                            CycloneTime = 20;
                            CycloneStamp = Time32.Now;
                        }
                        else
                        {
                            CycloneTime = (short)Seconds;
                            CycloneStamp = Time32.Now;
                        }
                    }
                }
                if (KOSpell == 1025)
                {
                    if (ContainsFlag(Network.GamePackets.Update.Flags.Superman))
                    {
                        int Seconds = SupermanStamp.AddSeconds(value).AllSeconds() - Time32.Now.AllSeconds();
                        if (Seconds >= 20)
                        {
                            SupermanTime = 20;
                            SupermanStamp = Time32.Now;
                        }
                        else
                        {
                            SupermanTime = (short)Seconds;
                            SupermanStamp = Time32.Now;
                        }
                    }
                }
            }
        }
        public short CycloneTime = 0, SupermanTime = 0, NoDrugsTime = 0, FatalStrikeTime = 0, ShurikenVortexTime = 0, OblivionTime = 0, AuraTime = 0, ShackleTime = 0, ChaosTime = 0, AzureTime;
        public ushort KOSpell = 0;
        public int AzureDamage = 0;
        private ushort _enlightenPoints;
        private byte _receivedEnlighenPoints;
        private ushort _enlightmenttime;
        public float ToxicFogPercent, StigmaIncrease, MagicShieldIncrease, DodgeIncrease, ShieldIncrease;
        public byte ToxicFogLeft, FlashingNameTime, FlyTime, StigmaTime, InvisibilityTime, StarOfAccuracyTime, MagicShieldTime, DodgeTime, AccuracyTime, ShieldTime, MagicDefenderSecs;
        public ushort KOCount = 0;
        public bool CounterKillSwitch = false;
        public Network.GamePackets.Attack AttackPacket;
        public Network.GamePackets.Attack VortexPacket;
        public byte[] SpawnPacket;
        private string _Name, _Spouse;
        private ushort _MDefence, _MDefencePercent;
        public ushort BaseDefence;
        private Client.GameClient _Owner;
        public uint ItemHP = 0;
        public ushort ItemMP = 0, ItemBless = 0, PhysicalDamageDecrease = 0, PhysicalDamageIncrease = 0, MagicDamageDecrease = 0, MagicDamageIncrease = 0, AttackRange = 1, Vigor = 0, ExtraVigor = 0;
        public ushort[] Gems = new ushort[10];
        private uint _MinAttack, _MaxAttack, _MagicAttack;
        public uint BaseMinAttack, BaseMaxAttack, BaseMagicAttack, BaseMagicDefence;
        private uint _TransMinAttack, _TransMaxAttack, _TransDodge, _TransPhysicalDefence, _TransMagicDefence;
        public bool Killed = false;
        public bool Transformed
        {
            get
            {
                return TransformationID != 98 && TransformationID != 99 && TransformationID != 0;
            }
        }
        public uint TransformationAttackRange = 0;
        public int TransformationTime = 0;
        public uint TransformationMaxHP = 0;
        public string KillerName;
        public uint KillerCps = 0;
        public DateTime KillerTime;
        public uint cpsea = 0;
        public uint inkillmode = 0;
        public ulong ARSDON = 0;
        private byte _Dodge;
        private Enums.PKMode _PKMode;
        private EntityFlag _EntityFlag;
        private MapObjectType _MapObjectType;
        public Enums.Mode Mode;
        private ulong _experience, _NobalityDonation;
        private uint _heavenblessing, _money, _uid, _hitpoints, _maxhitpoints, _quizpoints;
        private uint _conquerpoints, _Xmas, _BoundCps, _status, _status2, _status3, _status4, _Quest, _TreasuerPoints;
        private ushort _doubleexp, _body, _transformationid, _face, _strength, _agility, _spirit, _vitality, _atributes, _mana, _maxmana, _hairstyle, _mapid, _previousmapid, _x, _y, _pkpoints;
        private byte _stamina, _class, _reborn, _level;
        byte cls, secls, seclss;
        public byte FirstRebornClass
        {
            get
            {
                return cls;
            }
            set
            {
                cls = value;
                SpawnPacket[_FirstRebornClass] = value;
            }
        }
        public bool Hercule()
        {
            if (EntityFlag == Game.EntityFlag.Player)
            {
                var weapons = Owner.Weapons;
                if (weapons.Item1 != null && weapons.Item2 != null)
                    if (weapons.Item1.ID / 1000 == 480 && weapons.Item2.ID / 1000 == 480 || weapons.Item1.ID / 1000 == 410 && weapons.Item2.ID / 1000 == 410 || weapons.Item1.ID / 1000 == 420 && weapons.Item2.ID / 1000 == 420 || weapons.Item1.ID / 1000 == 614 && weapons.Item2.ID / 1000 == 614)
                        return true;
            }
            return false;
        }
        public bool EpicTrojan()
        {
            if (EntityFlag == Game.EntityFlag.Player)
            {
                var weapons = Owner.Weapons;
                if (weapons.Item1 != null)
                    if (weapons.Item1.ID / 1000 == 614)
                        return true;
                    else if (weapons.Item2 != null)
                        if (weapons.Item2.ID / 1000 == 614)
                            return true;
            }
            return false;
        }
        public bool EpicNinja()
        {
            if (EntityFlag == Game.EntityFlag.Player)
            {
                var weapons = Owner.Weapons;
                if (weapons.Item1 != null)
                    if (weapons.Item1.ID / 1000 == 616)
                        return true;
                    else if (weapons.Item2 != null)
                        if (weapons.Item2.ID / 1000 == 616)
                            return true;
            }
            return false;
        }
        public byte SecondRebornClass
        {
            get
            {
                return secls;
            }
            set
            {
                secls = value;
                SpawnPacket[_SecondRebornClass] = value;
            }
        }
        public ulong NobalityDonation
        {
            get
            {
                return _NobalityDonation;
            }
            set
            {
                _NobalityDonation = value;
            }
        }
        public byte ThirdRebornClass
        {
            get
            {
                return seclss;
            }
            set
            {
                seclss = value;
                SpawnPacket[_Class] = value;
            }
        }
        public byte FirstRebornLevel, SecondRebornLevel;
        public bool FullyLoaded = false, SendUpdates = false, HandleTiming = false, FullyLoaded2 = false;
        private Network.GamePackets.Update update;
        public Time32 WaitingTimeFB;
        public Time32 WinnerWaiting;
        public bool aWinner = false;
        #endregion
        public bool UseItem = false;
        public Action<Entity> OnDeath;
        #region TopDonation

        public int _TopKing = 0
            , _TopPrince = 0
            , _TopDucke = 0
            , _TopKinght = 0
            , _TopEarl = 0
            , _TopBaron = 0;
        public int _TopBlackname = 0
            , _TopRedname = 0
            , _TopWithename = 0;

        public int TopBlackname
        {
            get { return _TopBlackname; }
            set
            {
                _TopBlackname = value;
                if (value >= 1)
                {
                    AddFlag2(Network.GamePackets.Update.Flags2.Top8Archer);
                    Conquer_Online_Server.Database.EntityTable.SaveTopDonation(this.Owner);
                }
            }
        }
        public int TopRedname//Top8Fire
        {
            get { return _TopRedname; }
            set
            {
                _TopRedname = value;
                if (value >= 1)
                {
                    AddFlag2(Network.GamePackets.Update.Flags2.Top8Fire);
                    Conquer_Online_Server.Database.EntityTable.SaveTopDonation(this.Owner);
                }
            }
        }
        public int TopWithename//Top3Monk
        {
            get { return _TopWithename; }
            set
            {
                _TopWithename = value;
                if (value >= 1)
                {
                    AddFlag2(Network.GamePackets.Update.Flags2.Top8Ninja);
                    Conquer_Online_Server.Database.EntityTable.SaveTopDonation(this.Owner);
                }
            }
        }
        public int TopKing //Top3Archer
        {
            get { return _TopKing; }
            set
            {
                _TopKing = value;
                if (value >= 1)
                {
                    AddFlag2(Network.GamePackets.Update.Flags2.Top3Archer);
                    Conquer_Online_Server.Database.EntityTable.SaveTopDonation(this.Owner);
                }
            }
        }

        public int TopPrince//Top3Fire
        {
            get { return _TopPrince; }
            set
            {
                _TopPrince = value;
                if (value >= 1)
                {
                    AddFlag2(Network.GamePackets.Update.Flags2.Top3Fire);
                    Conquer_Online_Server.Database.EntityTable.SaveTopDonation(this.Owner);
                }
            }
        }

        public int TopDucke//Top3Monk
        {
            get { return _TopDucke; }
            set
            {
                _TopDucke = value;
                if (value >= 1)
                {
                    AddFlag2(Network.GamePackets.Update.Flags2.Top3Monk);
                    Conquer_Online_Server.Database.EntityTable.SaveTopDonation(this.Owner);
                }
            }
        }

        public int TopKinght //Top3Ninja
        {
            get { return _TopKinght; }
            set
            {
                _TopKinght = value;
                if (value >= 1)
                {
                    AddFlag2(Network.GamePackets.Update.Flags2.Top3Ninja);
                    Conquer_Online_Server.Database.EntityTable.SaveTopDonation(this.Owner);
                }
            }
        }

        public int TopEarl//Top3Trojan
        {
            get { return _TopEarl; }
            set
            {
                _TopEarl = value;
                if (value >= 1)
                {
                    AddFlag2(Network.GamePackets.Update.Flags2.Top3Trojan);
                    Conquer_Online_Server.Database.EntityTable.SaveTopDonation(this.Owner);
                }
            }
        }

        public int TopBaron//Top3Warrior
        {
            get { return _TopBaron; }
            set
            {
                _TopBaron = value;
                if (value >= 1)
                {
                    AddFlag2(Network.GamePackets.Update.Flags2.Top3Warrior);
                    Conquer_Online_Server.Database.EntityTable.SaveTopDonation(this.Owner);
                }
            }
        }

        #endregion TopDonation
        #region Acessors
        #region Fan/Tower Acessor
        public int getFan(bool Magic)
        {
            if (Owner.Equipment.Free(10))
                return 0;

            ushort magic = 0;
            ushort physical = 0;
            ushort gemVal = 0;

            #region Get
            ConquerItem Item = this.Owner.Equipment.TryGetItem(10);

            if (Item != null)
            {
                if (Item.ID > 0)
                {
                    switch (Item.ID % 10)
                    {
                        case 3:
                        case 4:
                        case 5: physical += 300; magic += 150; break;
                        case 6: physical += 500; magic += 200; break;
                        case 7: physical += 700; magic += 300; break;
                        case 8: physical += 900; magic += 450; break;
                        case 9: physical += 1200; magic += 750; break;
                    }

                    switch (Item.Plus)
                    {
                        case 0: break;
                        case 1: physical += 200; magic += 100; break;
                        case 2: physical += 400; magic += 200; break;
                        case 3: physical += 600; magic += 300; break;
                        case 4: physical += 800; magic += 400; break;
                        case 5: physical += 1000; magic += 500; break;
                        case 6: physical += 1200; magic += 600; break;
                        case 7: physical += 1300; magic += 700; break;
                        case 8: physical += 1400; magic += 800; break;
                        case 9: physical += 1500; magic += 900; break;
                        case 10: physical += 1600; magic += 950; break;
                        case 11: physical += 1700; magic += 1000; break;
                        case 12: physical += 1800; magic += 1050; break;
                    }
                    switch (Item.SocketOne)
                    {
                        case Enums.Gem.NormalThunderGem: gemVal += 100; break;
                        case Enums.Gem.RefinedThunderGem: gemVal += 300; break;
                        case Enums.Gem.SuperThunderGem: gemVal += 500; break;
                    }
                    switch (Item.SocketTwo)
                    {
                        case Enums.Gem.NormalThunderGem: gemVal += 100; break;
                        case Enums.Gem.RefinedThunderGem: gemVal += 300; break;
                        case Enums.Gem.SuperThunderGem: gemVal += 500; break;
                    }
                }
            }
            #endregion

            magic += gemVal;
            physical += gemVal;

            if (Magic)
                return (int)magic;
            else
                return (int)physical;
        }

        public int getTower(bool Magic)
        {
            if (Owner.Equipment.Free(11))
                return 0;

            ushort magic = 0;
            ushort physical = 0;
            ushort gemVal = 0;

            #region Get
            ConquerItem Item = this.Owner.Equipment.TryGetItem(11);

            if (Item != null)
            {
                if (Item.ID > 0)
                {
                    switch (Item.ID % 10)
                    {
                        case 3:
                        case 4:
                        case 5: physical += 250; magic += 100; break;
                        case 6: physical += 400; magic += 150; break;
                        case 7: physical += 550; magic += 200; break;
                        case 8: physical += 700; magic += 300; break;
                        case 9: physical += 1100; magic += 600; break;
                    }

                    switch (Item.Plus)
                    {
                        case 0: break;
                        case 1: physical += 150; magic += 50; break;
                        case 2: physical += 350; magic += 150; break;
                        case 3: physical += 550; magic += 250; break;
                        case 4: physical += 750; magic += 350; break;
                        case 5: physical += 950; magic += 450; break;
                        case 6: physical += 1100; magic += 550; break;
                        case 7: physical += 1200; magic += 625; break;
                        case 8: physical += 1300; magic += 700; break;
                        case 9: physical += 1400; magic += 750; break;
                        case 10: physical += 1500; magic += 800; break;
                        case 11: physical += 1600; magic += 850; break;
                        case 12: physical += 1700; magic += 900; break;
                    }
                    switch (Item.SocketOne)
                    {
                        case Enums.Gem.NormalGloryGem: gemVal += 100; break;
                        case Enums.Gem.RefinedGloryGem: gemVal += 300; break;
                        case Enums.Gem.SuperGloryGem: gemVal += 500; break;
                    }
                    switch (Item.SocketTwo)
                    {
                        case Enums.Gem.NormalGloryGem: gemVal += 100; break;
                        case Enums.Gem.RefinedGloryGem: gemVal += 300; break;
                        case Enums.Gem.SuperGloryGem: gemVal += 500; break;
                    }
                }
            }
            #endregion

            magic += gemVal;
            physical += gemVal;

            if (Magic)
                return (int)magic;
            else
                return (int)physical;
        }
        #endregion
        public static void KilleStart(Client.GameClient client, Npcs dialog, string PlayerName, uint UID)
        {
            object[] playerName;
            if (Kernel.GamePool.ContainsKey(UID) && Kernel.GamePool[UID] != null)
            {
                Client.GameClient item = Kernel.GamePool[UID];
                if (item != null)
                {
                    if (item.Entity.Name != client.Entity.KillerName)
                    {
                        dialog.Text(string.Concat("Sorry, this player may be offline or not found 1. ", PlayerName));
                        dialog.Option("Sorry.", 255);
                        dialog.Send();
                    }
                    else
                    {
                        if (item.Entity.inkillmode != 0)
                        {
                            playerName = new object[] { "Sorry, ", PlayerName, " Some One offer him In fight " };
                            dialog.Text(string.Concat(playerName));
                            dialog.Option("OK, Sorry.", 255);
                            dialog.Send();
                        }
                        else
                        {
                            item.Entity.inkillmode = 1;
                            item.Entity.KillerCps = client.Entity.KillerCps;
                            item.Entity.KillerTime = DateTime.Now;
                            client.Entity.KillerCps = 0;
                            EntityTable.SaveEntity(client);
                            EntityTable.SaveEntity(item);
                            playerName = new object[1];
                            object[] name = new object[] { client.Entity.Name, " Offer ", item.Entity.KillerCps, "Cps For Who Kill  Player :", PlayerName, " He Is Wanted Now" };
                            playerName[0] = string.Concat(name);
                            Kernel.SendWorldMessage(new Message(string.Concat(playerName), System.Drawing.Color.Red, 2011), Program.GamePool);
                        }
                    }
                }
                else
                {
                    dialog.Text(string.Concat("Sorry, this player may be offline or not found 2. ", PlayerName));
                    dialog.Option("Sorry.", 255);
                    dialog.Send();
                }
            }
        }
        public Double GemBonus(Byte type)
        {
            Double bonus = 0;
            foreach (ConquerItem i in Owner.Equipment.Objects)
                if (i != null)
                    if (i.IsWorn)
                        bonus += i.GemBonus(type);
            if (Class >= 130 && Class <= 135)
                if (type == ItemSocket.Tortoise)
                    bonus = Math.Min(0.12, bonus);
            return bonus;
        }
        public int BattlePower
        {
            get
            {
                return BattlePowerCalc(this);
            }
        }
        public int NMBattlePower
        {
            get
            {
                return (int)(BattlePowerCalc(this) - MentorBattlePower);
            }
        }
        public uint BattlePowerFrom(Entity mentor)
        {

            return (uint)(((mentor.BattlePower - mentor.ExtraBattlePower) - (BattlePower - ExtraBattlePower)) / 3.3F);
        }
        public DateTime LastLogin
        {
            get { return this.mLastLogin; }
            set { this.mLastLogin = value; }
        }
        public bool WearsGoldPrize = false;
        public string LoweredName;
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
                LoweredName = value.ToLower();
                if (EntityFlag == Game.EntityFlag.Player)
                {
                    if (ClanName != "")
                    {
                        SpawnPacket = new byte[8 + _Names + _Name.Length + ClanName.Length + 5];
                        WriteUInt16((ushort)(SpawnPacket.Length - 8), 0, SpawnPacket);
                        WriteUInt16(10014, 2, SpawnPacket);
                        WriteUInt32((uint)Time32.timeGetTime().GetHashCode(), 4, SpawnPacket);
                        WriteStringList(new List<string>() { _Name, "", "", "", "", ClanName }, _Names, SpawnPacket);
                    }
                    else
                    {
                        SpawnPacket = new byte[8 + _Names + _Name.Length + 20];
                        WriteUInt16((ushort)(SpawnPacket.Length - 8), 0, SpawnPacket);
                        WriteUInt16(10014, 2, SpawnPacket);
                        WriteUInt32((uint)Time32.timeGetTime().GetHashCode(), 4, SpawnPacket);
                        WriteStringList(new List<string>() { _Name, "", "", "", "", "" }, _Names, SpawnPacket);
                    }
                }
                else
                {
                    if (ClanName != "")
                    {
                        SpawnPacket = new byte[8 + _Names + _Name.Length + ClanName.Length + 6];
                        WriteUInt16((ushort)(SpawnPacket.Length - 8), 0, SpawnPacket);
                        WriteUInt16(10014, 2, SpawnPacket);
                        WriteUInt32((uint)Time32.timeGetTime().GetHashCode(), 4, SpawnPacket);
                        WriteStringList(new List<string>() { _Name, "", "", ClanName }, _Names, SpawnPacket);
                    }
                    else
                    {
                        SpawnPacket = new byte[8 + _Names + _Name.Length + 20];
                        WriteUInt16((ushort)(SpawnPacket.Length - 8), 0, SpawnPacket);
                        WriteUInt16(10014, 2, SpawnPacket);
                        WriteUInt32((uint)Time32.timeGetTime().GetHashCode(), 4, SpawnPacket);
                        WriteStringList(new List<string>() { _Name, "", "", "" }, _Names, SpawnPacket);
                    }

                }
            }
        }  
        public string NameMaker;
        public string Spouse
        {
            get
            {
                return _Spouse;
            }
            set
            {
                if (EntityFlag == EntityFlag.Player)
                {
                    Update(Network.GamePackets._String.Spouse, value, false);
                }
                _Spouse = value;
            }
        }
        public uint Money
        {
            get
            {
                return _money;
            }
            set
            {
                _money = value;
                if (EntityFlag == EntityFlag.Player)
                    Update(Network.GamePackets.Update.Money, value, false);


            }
        }
        private byte _vipLevel;
        public byte VIPLevel
        {
            get
            {
                return _vipLevel;
            }
            set
            {
                if (EntityFlag == EntityFlag.Player)
                {
                    Update(Network.GamePackets.Update.VIPLevel, value, false);
                }
                _vipLevel = value;
            }
        }
        public byte reinc;
        public byte ReincarnationLev
        {
            get
            {
                return reinc;
            }
            set
            {
                reinc = value;
            }
        }
        public uint BoundCps
        {
            get
            {
                return _BoundCps;
            }
            set
            {
                if (value <= 0 || value > 999999999)
                {
                    value = 0;
                }

                _BoundCps = value;
                if (EntityFlag == EntityFlag.Player)
                {
                    Update(Network.GamePackets.Update.BoundConquerPoints, (uint)value, false);
                }
            }
        }
        public uint ConquerPoints
        {
            get
            {
                return _conquerpoints;
            }
            set
            {
                if (value <= 0)
                    value = 0;

                _conquerpoints = value;
                Database.EntityTable.UpdateCps(this.Owner);
                if (EntityFlag == EntityFlag.Player)
                {
                    //if (FullyLoaded)
                    //UpdateDatabase("ConquerPoints", value);
                    Update(Network.GamePackets.Update.ConquerPoints, (uint)value, false);
                }
            }
        }
        private uint _SecondaryPass;
        public uint SecondaryPass
        {
            get
            {
                return _SecondaryPass;
            }
            set
            {
                _SecondaryPass = value;
                if (this != null)
                {
                    if (this.FullyLoaded)
                    {
                        this.UpdateDatabase("secondarypassword", value);
                    }
                }
            }
        }
        public uint TreasuerPoints
        {
            get
            {
                return _TreasuerPoints;
            }
            set
            {
                if (value <= 0)
                    value = 0;

                _TreasuerPoints = value;
                Database.EntityTable.UpdateTreasuerPoints(this.Owner);
            }
        }
        public ushort Body
        {
            get
            {
                return _body;
            }
            set
            {
                WriteUInt32((uint)(TransformationID * 10000000 + Face * 10000 + value), _Mesh, SpawnPacket);
                _body = value;
                if (EntityFlag == EntityFlag.Player)
                {
                    if (Owner != null)
                    {
                        Owner.ArenaStatistic.Model = (uint)(Face * 10000 + value);
                        Update(Network.GamePackets.Update.Mesh, Mesh, true);
                    }
                }
            }
        }
        public ushort DoubleExperienceTime
        {
            get
            {
                return _doubleexp;
            }
            set
            {
                ushort oldVal = DoubleExperienceTime;
                _doubleexp = value;
                if (FullyLoaded)
                    if (oldVal <= _doubleexp)
                        if (EntityFlag == EntityFlag.Player)
                        {
                            if (Owner != null)
                            {
                                Update(Network.GamePackets.Update.DoubleExpTimer, DoubleExperienceTime, 200, false);
                            }
                        }
            }
        }

        public uint HeavenBlessing
        {
            get
            {
                return _heavenblessing;
            }
            set
            {
                uint oldVal = HeavenBlessing;
                _heavenblessing = value;
                if (FullyLoaded)
                    if (value > 0)
                        if (!ContainsFlag(Network.GamePackets.Update.Flags.HeavenBlessing) || oldVal <= _heavenblessing)
                        {
                            AddFlag(Network.GamePackets.Update.Flags.HeavenBlessing);
                            Update(Network.GamePackets.Update.HeavensBlessing, HeavenBlessing, false);
                            Update(Network.GamePackets._String.Effect, "bless", true);
                        }
            }
        }
        public uint PokerTable = 0;
        public PokerTable MyPokerTable
        {
            get
            {
                if (Conquer_Online_Server.Kernel.PokerTables.ContainsKey(PokerTable))
                    return Conquer_Online_Server.Kernel.PokerTables[PokerTable];
                else return null;
            }
            set
            {
                PokerTable = value.Id;
            }
        }
        public byte Stamina
        {
            get
            {
                return _stamina;
            }
            set
            {
                _stamina = value;
                if (EntityFlag == EntityFlag.Player)
                    Update(Network.GamePackets.Update.Stamina, value, false);
            }
        }
        public ushort TransformationID
        {
            get
            {
                return _transformationid;
            }
            set
            {
                _transformationid = value;
                WriteUInt32((uint)(value * 10000000 + Face * 10000 + Body), 4 + 4, SpawnPacket);
                if (EntityFlag == EntityFlag.Player)
                    Update(Network.GamePackets.Update.Mesh, Mesh, true);
            }
        }
        public ushort Face
        {
            get
            {
                return _face;
            }
            set
            {
                WriteUInt32((uint)(TransformationID * 10000000 + value * 10000 + Body), 4 + 4, SpawnPacket);
                _face = value;
                if (EntityFlag == EntityFlag.Player)
                {
                    if (Owner != null)
                    {
                        Owner.ArenaStatistic.Model = (uint)(value * 10000 + Body);
                        Update(Network.GamePackets.Update.Mesh, Mesh, true);
                    }
                }
            }
        }
        public uint Mesh
        {
            get
            {
                return BitConverter.ToUInt32(SpawnPacket, _Mesh);
            }
        }
        public byte Class
        {
            get
            {
                return _class;
            }
            set
            {
                if (EntityFlag == EntityFlag.Player)
                {
                    if (Owner != null)
                    {
                        if (Owner.ArenaStatistic != null)
                            Owner.ArenaStatistic.Class = value;
                        Update(Network.GamePackets.Update.Class, value, false);
                    }
                }
                _class = value;
                SpawnPacket[_Class] = value;
                //SpawnPacket[209] = value;
                //SpawnPacket[214] = value;
                //SpawnPacket[217] = value;
                //SpawnPacket[218] = value;
            }
        }
        public byte Reborn
        {
            get
            {
                SpawnPacket[_Reborn] = _reborn;
                return _reborn;
            }
            set
            {
                if (EntityFlag == EntityFlag.Player)
                {
                    Update(Network.GamePackets.Update.Reborn, value, true);
                }
                _reborn = value;
                SpawnPacket[_Reborn] = value;
            }
        }
        public byte Level
        {
            get
            {
                if (EntityFlag == EntityFlag.Player)
                {
                    SpawnPacket[_Level] = _level;
                    return _level;
                }
                else
                {
                    SpawnPacket[_MonsterLevel] = _level;
                    return _level;
                }
            }
            set
            {
                if (EntityFlag == EntityFlag.Player)
                {
                    Update(Network.GamePackets.Update.Level, value, true);
                    Data update = new Data(true);
                    update.UID = UID;
                    update.ID = Data.Leveled;
                    if (Owner != null)
                    {
                        (Owner as Client.GameClient).SendScreen(update, true);
                        Owner.ArenaStatistic.Level = value;
                        Owner.ArenaStatistic.ArenaPoints = 1000;
                    }
                    if (Owner != null)
                    {
                        if (Owner.AsMember != null)
                        {
                            Owner.AsMember.Level = value;
                        }
                    }
                    SpawnPacket[_Level] = value;
                    //if (FullyLoaded)
                    UpdateDatabase("Level", value);
                }
                else
                {
                    SpawnPacket[_MonsterLevel] = value;
                }
                _level = value;

            }
        }

        private uint mentorBP;
        public uint MentorBattlePower
        {
            get
            {
                return mentorBP;
            }
            set
            {
                if ((int)value < 0)
                    value = 0;
                if (Owner.Mentor != null)
                {
                    if (Owner.Mentor.IsOnline)
                    {
                        ExtraBattlePower -= mentorBP;
                        mentorBP = value;
                        ExtraBattlePower += value;
                        int val = Owner.Mentor.Client.Entity.BattlePower;
                        Update(Network.GamePackets.Update.MentorBattlePower, (uint)Math.Min(val, value), (uint)val);
                    }
                    else
                    {
                        ExtraBattlePower -= mentorBP;
                        mentorBP = 0;
                        Update(Network.GamePackets.Update.MentorBattlePower, (uint)0, (uint)0);
                    }
                }
                else
                {
                    ExtraBattlePower -= mentorBP;
                    mentorBP = 0;
                    Update(Network.GamePackets.Update.MentorBattlePower, (uint)0, (uint)0);
                }
            }
        }

        public uint ExtraBattlePower
        {
            get
            {
                return BitConverter.ToUInt32(SpawnPacket, _ExtraBattlepower);
            }
            set
            {
                if (value > 200) value = 0;
                WriteUInt32(value, _ExtraBattlepower, SpawnPacket);
            }
        }
        public bool awayTeleported = false;
        public byte Away
        {
            get
            {
                return SpawnPacket[112 + 4];
            }
            set
            {
                SpawnPacket[112 + 4] = value;
            }
        }
        public byte Boss
        {
            get
            {
                return SpawnPacket[_Boss];
            }
            set
            {
                SpawnPacket[_Boss] = 1;
            }
        }
        public uint UID
        {
            get
            {
                if (SpawnPacket != null)
                    return BitConverter.ToUInt32(SpawnPacket, _UID);
                else
                    return _uid;
            }
            set
            {
                _uid = value;
                WriteUInt32(value, _UID, SpawnPacket);
            }
        }

        public ushort GuildID
        {
            get
            {
                return BitConverter.ToUInt16(SpawnPacket, _GuildID);
            }
            set
            {
                WriteUInt32(value, _GuildID, SpawnPacket);
            }
        }

        public ushort GuildRank
        {
            get
            {
                return BitConverter.ToUInt16(SpawnPacket, _GuildRank);
            }
            set
            {
                WriteUInt16(value, _GuildRank, SpawnPacket);
            }
        }
        public ushort Strength
        {
            get
            {
                return _strength;
            }
            set
            {
                if (EntityFlag == EntityFlag.Player)
                {
                    Update(Network.GamePackets.Update.Strength, value, false);
                }
                _strength = value;
            }
        }
        public byte TitleActivated
        {
            get { return this.SpawnPacket[181 + 4]; }
            set { this.SpawnPacket[181 + 4] = value; }
        }
        public ushort Agility
        {
            get
            {
                if (OnCyclone())
                    return (ushort)(_agility);
                return _agility;
            }
            set
            {
                if (EntityFlag == EntityFlag.Player)
                    Update(Network.GamePackets.Update.Agility, value, false);
                _agility = value;
            }
        }
        public ushort Spirit
        {
            get
            {
                return _spirit;
            }
            set
            {
                if (EntityFlag == EntityFlag.Player)
                    Update(Network.GamePackets.Update.Spirit, value, false);
                _spirit = value;
            }
        }
        public ushort Vitality
        {
            get
            {
                return _vitality;
            }
            set
            {
                if (EntityFlag == EntityFlag.Player)
                    Update(Network.GamePackets.Update.Vitality, value, false);
                _vitality = value;
            }
        }
        public ushort Atributes
        {
            get
            {
                return _atributes;
            }
            set
            {
                if (EntityFlag == EntityFlag.Player)
                    Update(Network.GamePackets.Update.Atributes, value, false);
                _atributes = value;
            }
        }
        public uint Hitpoints
        {
            get
            {
                return _hitpoints;
            }
            set
            {
                if (EntityFlag == EntityFlag.Player)
                    Update(Network.GamePackets.Update.Hitpoints, value, false);
                _hitpoints = value;

                if (Boss > 0)
                {
                    uint key = (uint)(MaxHitpoints / 10000);
                    if (key != 0)
                        WriteUInt16((ushort)(value / key), _Hitpoints, SpawnPacket);
                    else
                        WriteUInt16((ushort)(value * MaxHitpoints / 1000 / 1.09), _Hitpoints, SpawnPacket);
                }
                else
                    WriteUInt16((ushort)value, _Hitpoints, SpawnPacket);
            }
        }
        public ushort Mana
        {
            get
            {
                return _mana;
            }
            set
            {
                if (EntityFlag == EntityFlag.Player)
                    Update(Network.GamePackets.Update.Mana, value, false);
                _mana = value;
            }
        }
        public ushort MaxMana
        {
            get
            {
                return _maxmana;
            }
            set
            {
                if (EntityFlag == EntityFlag.Player)
                    Update(Network.GamePackets.Update.MaxMana, value, false);
                _maxmana = value;
            }
        }
        public ushort HairStyle
        {
            get
            {
                return _hairstyle;
            }
            set
            {
                if (EntityFlag == EntityFlag.Player)
                {
                    Update(Network.GamePackets.Update.HairStyle, value, true);
                }
                _hairstyle = value;
                WriteUInt16(value, _HairStyle, SpawnPacket);
            }
        }
        /*  public Byte SubClass//jose (re copy with urs)
          {
              get { return _SubClass; }
              set
              {
                  _SubClass = value;
                  SpawnPacket[_ActiveSubclass] =
                      EntityFlag != Game.EntityFlag.Monster ? _SubClass : (Byte)0;
              }
          }

          public byte SubClassLevel
          {
              get { return _SubClassLevel; }
              set
              {
                  _SubClassLevel = value;
              }
          }*/
        public byte SubClass
        {
            get
            {
                return this._SubClass;
            }
            set
            {
                this._SubClass = value;
                this.SpawnPacket[_ActiveSubclass] = (this.EntityFlag != Conquer_Online_Server.Game.EntityFlag.Monster) ? this._SubClass : ((byte)0);
                if (EntityFlag == Game.EntityFlag.Player)
                {
                    if (FullyLoaded)
                    {
                        UpdateDatabase("SubClass", _SubClass);
                    }
                }
            }
        }
        public byte SubClassesActive
        {
            get { return SpawnPacket[218 + 4]; }
            set { SpawnPacket[218 + 4] = value; }
        }
        public byte SubClassLevel
        {
            get { return _SubClassLevel; }
            set
            {
                this._SubClassLevel = value;
                switch (EntityFlag)
                {
                    case EntityFlag.Player:
                        if (FullyLoaded)
                        { UpdateDatabase("SubClassLevel", value); }
                        break;
                }
            }
        }
        public ConquerStructures.NobilityRank NobilityRank
        {
            get
            {
                return (Conquer_Online_Server.Game.ConquerStructures.NobilityRank)SpawnPacket[_NobilityRank];
            }
            set
            {
                SpawnPacket[_NobilityRank] = (byte)value;
                if (Owner != null)
                {
                    if (Owner.AsMember != null)
                    {
                        Owner.AsMember.NobilityRank = value;
                    }
                }
            }
        }

        public byte HairColor
        {
            get
            {
                return (byte)(HairStyle / 100);
            }
            set
            {
                HairStyle = (ushort)((value * 100) + (HairStyle % 100));
            }
        }
        public ushort MapID
        {
            get
            {
                return _mapid;
            }
            set
            {
                _mapid = value;
            }
        }
        public uint Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
            }
        }
        public uint Status2
        {
            get
            {
                return _status2;
            }
            set
            {
                _status2 = value;
            }
        }

        public uint Status3
        {
            get
            {
                return _status3;
            }
            set
            {
                _status3 = value;
            }
        }

        public uint Status4
        {
            get
            {
                return _status4;
            }
            set
            {
                _status4 = value;
            }
        }
        public uint Quest
        {
            get
            {
                return _Quest;
            }
            set
            {
                _Quest = value;
            }
        }
        public ushort PreviousMapID
        {
            get
            {
                return _previousmapid;
            }
            set
            {
                _previousmapid = value;
            }
        }
        public ushort X
        {
            get
            {
                return _x;
            }
            set
            {
                _x = value;
                WriteUInt16(value, 98 + 4, this.SpawnPacket);
            }
        }
        public ushort Y
        {
            get
            {
                return _y;
            }
            set
            {
                _y = value;
                WriteUInt16(value, 100 + 4, this.SpawnPacket);
            }
        }
        public ushort PX
        {
            get;
            set;
        }
        public ushort PY
        {
            get;
            set;
        }
        public bool Dead
        {
            get
            {
                return Hitpoints < 1;
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        public ushort Defence
        {
            get
            {
                if (Time32.Now < ShieldStamp.AddSeconds(ShieldTime) && ContainsFlag(Network.GamePackets.Update.Flags.MagicShield))
                    if (ShieldIncrease > 0)
                        return (ushort)Math.Min(65535, (int)(BaseDefence * ShieldIncrease));
                if (SuperItemBless > 0)
                    return (ushort)(BaseDefence + (float)BaseDefence / 100 * SuperItemBless);
                return BaseDefence;
            }
            set { BaseDefence = value; }
        }
        public ushort TransformationDefence
        {
            get
            {
                if (ContainsFlag(Network.GamePackets.Update.Flags.MagicShield))
                {
                    if (ShieldTime > 0)
                        return (ushort)(_TransPhysicalDefence * ShieldIncrease);
                    else
                        return (ushort)(_TransPhysicalDefence * MagicShieldIncrease);
                }
                return (ushort)_TransPhysicalDefence;
            }
            set { _TransPhysicalDefence = value; }
        }
        public ushort MagicDefencePercent
        {
            get { return _MDefencePercent; }
            set { _MDefencePercent = value; }
        }
        public ushort TransformationMagicDefence
        {
            get { return (ushort)_TransMagicDefence; }
            set { _TransMagicDefence = value; }
        }
        public ushort MagicDefence
        {
            get { return _MDefence; }
            set { _MDefence = value; }
        }
        public Client.GameClient Owner
        {
            get { return _Owner; }
            set { _Owner = value; }
        }
        public uint TransformationMinAttack
        {
            get
            {
                if (ContainsFlag(Network.GamePackets.Update.Flags.Stigma))
                    return (uint)(_TransMinAttack * StigmaIncrease);
                return _TransMinAttack;
            }
            set { _TransMinAttack = value; }
        }
        public uint TransformationMaxAttack
        {
            get
            {
                if (ContainsFlag(Network.GamePackets.Update.Flags.Stigma))
                    return (uint)(_TransMaxAttack * StigmaIncrease);
                return _TransMaxAttack;
            }
            set { _TransMaxAttack = value; }
        }
        public uint MinAttack
        {
            get
            {
                return _MinAttack;
            }
            set { _MinAttack = value; }
        }
        public uint MaxAttack
        {
            get
            {
                return _MaxAttack;
            }
            set { _MaxAttack = value; }
        }
        public uint MaxHitpoints
        {
            get
            {
                return _maxhitpoints;
            }
            set
            {
                if (EntityFlag == EntityFlag.Player)
                    if (TransformationID != 0 && TransformationID != 98)
                        Update(Network.GamePackets.Update.MaxHitpoints, value, true);
                _maxhitpoints = value;
            }
        }
        public uint MagicAttack
        {
            get
            {
                return _MagicAttack;
            }
            set { _MagicAttack = value; }
        }
        public byte Dodge
        {
            get
            {
                if (ContainsFlag(Network.GamePackets.Update.Flags.Dodge))
                {
                    Console.WriteLine("Calc Dodge =" + (_Dodge * DodgeIncrease).ToString());
                    return (byte)(_Dodge * DodgeIncrease);
                }
                return _Dodge;
            }
            set { _Dodge = value; }
        }
        public byte TransformationDodge
        {
            get
            {
                if (ContainsFlag(Network.GamePackets.Update.Flags.Dodge))
                    return (byte)(_TransDodge * DodgeIncrease);
                return (byte)_TransDodge;
            }
            set { _TransDodge = value; }
        }
        public MapObjectType MapObjType
        {
            get { return _MapObjectType; }
            set { _MapObjectType = value; }
        }

        public EntityFlag EntityFlag
        {
            get { return _EntityFlag; }
            set { _EntityFlag = value; }
        }
        public ulong Experience
        {
            get
            {
                return _experience;
            }
            set
            {
                if (EntityFlag == EntityFlag.Player)
                    Update(Network.GamePackets.Update.Experience, value, false);
                _experience = value;
            }
        }

        public ushort EnlightenPoints
        {
            get
            {
                return _enlightenPoints;
            }
            set
            {
                _enlightenPoints = value;
            }
        }

        public byte ReceivedEnlightenPoints
        {
            get
            {
                return _receivedEnlighenPoints;
            }
            set
            {
                _receivedEnlighenPoints = value;
            }
        }

        public ushort EnlightmentTime
        {
            get
            {
                return _enlightmenttime;
            }
            set
            {
                _enlightmenttime = value;
            }
        }

        public ushort PKPoints
        {
            get
            {
                return _pkpoints;
            }
            set
            {
                _pkpoints = value;
                if (EntityFlag == EntityFlag.Player)
                {
                    Update(Network.GamePackets.Update.PKPoints, value, false);
                    if (PKPoints > 99)
                    {
                        RemoveFlag(Network.GamePackets.Update.Flags.RedName);
                        AddFlag(Network.GamePackets.Update.Flags.BlackName);
                    }
                    else if (PKPoints > 29)
                    {
                        AddFlag(Network.GamePackets.Update.Flags.RedName);
                        RemoveFlag(Network.GamePackets.Update.Flags.BlackName);
                    }
                    else if (PKPoints < 30)
                    {
                        RemoveFlag(Network.GamePackets.Update.Flags.RedName);
                        RemoveFlag(Network.GamePackets.Update.Flags.BlackName);
                    }
                }
            }
        }
        public uint QuizPoints
        {
            get
            {
                return _quizpoints;
            }
            set
            {
                if (EntityFlag == EntityFlag.Player)
                {
                    Update(Network.GamePackets.Update.QuizPoints, value, true);
                }
                _quizpoints = value;
                if (value >= 0 && value < 901)
                {
                    WriteUInt32(value, 142 + 4, SpawnPacket);
                }
                if (value >= 901 && value < 9001)//Master
                {
                    WriteUInt32(value + 500000, 142 + 4, SpawnPacket);
                }
                if (value >= 9001 && value < 27001)//Scholar
                {
                    WriteUInt32(value + 5000000, 142 + 4, SpawnPacket);
                }
                if (value >= 27001)//MrBahaa
                {
                    WriteUInt32(value + 50000000, 142 + 4, SpawnPacket);
                }
                // WriteUInt32(value, 140, SpawnPacket);
            }
        }
        public UInt32 ClanId
        {
            get { return BitConverter.ToUInt32(SpawnPacket, _ClanUID); }
            set { WriteUInt32((UInt32)value, _ClanUID, SpawnPacket); }
        }
        public Clan Myclan;
        public Clan.Ranks ClanRank
        {
            get { return (Clan.Ranks)SpawnPacket[_ClanRank]; }
            set { SpawnPacket[_ClanRank] = (Byte)value; }
        }
        public Clan GetClan
        {
            get
            {
                Clan cl;
                Kernel.Clans.TryGetValue(ClanId, out cl);
                return cl;
            }
        }
        string clan = "";
        public string ClanName
        {
            get { return clan; }
            set
            {

                clan = value;
                if (clan.Length > 15)
                    clan = clan.Substring(0, 15);
                WriteStringList(new List<string>() { _Name, "", "", clan }, _Names, SpawnPacket);
            }
        } 
        private UInt32 mClanJoinTarget;
        public UInt32 ClanJoinTarget
        {
            get { return this.mClanJoinTarget; }
            set { this.mClanJoinTarget = value; }
        }
        public Enums.PKMode PKMode
        {
            get { return _PKMode; }
            set { _PKMode = value; }
        }
        public ushort Action
        {
            get { return BitConverter.ToUInt16(SpawnPacket, _Action); }
            set
            {
                WriteUInt16(value, _Action, SpawnPacket);
            }
        }
        public Enums.ConquerAngle Facing
        {
            get { return (Enums.ConquerAngle)SpawnPacket[_Facing]; }
            set
            {
                SpawnPacket[_Facing] = (byte)value;
            }
        }
        public ulong StatusFlag
        {
            get
            {
                return BitConverter.ToUInt64(SpawnPacket, _StatusFlag);
            }
            set
            {
                ulong OldV = StatusFlag;
                if (value != OldV)
                {
                    WriteUInt64(value, _StatusFlag, SpawnPacket);
                    //Update(Network.GamePackets.Update.StatusFlag, value, !ContainsFlag(Network.GamePackets.Update.Flags.XPList));
                    UpdateEffects(true);
                }
            }
        }
        private ulong _Stateff2 = 0;
        public ulong StatusFlag2
        {
            get { return _Stateff2; }
            set
            {
                ulong OldV = StatusFlag2;
                if (value != OldV)
                {
                    _Stateff2 = value;
                    WriteUInt64(value, _StatusFlag2, SpawnPacket);

                    UpdateEffects(true);
                    // Update2(Network.GamePackets.Update.StatusFlag, value, true);// !ContainsFlag(Network.GamePackets.Update.Flags.XPList));//you need to update the SECOND value of stateff
                }
            }
        }
        uint _Stateff3 = 0;
        public uint StatusFlag3
        {
            get { return _Stateff3; }
            set
            {
                uint OldV = StatusFlag3;
                if (value != OldV)
                {
                    _Stateff3 = value;
                    WriteUInt32(value, _StatusFlag3, SpawnPacket);

                    UpdateEffects(true);
                    // Update2(Network.GamePackets.Update.StatusFlag, value, true);// !ContainsFlag(Network.GamePackets.Update.Flags.XPList));//you need to update the SECOND value of stateff
                }
            }
        }
        public void Save(String row, String value)
        {
            MySqlCommand Command = new MySqlCommand(MySqlCommandType.UPDATE);
            Command.Update("entities")
                .Set(row, value)
                .Where("uid", UID)
                .Execute();
        }
        public void Save(String row, UInt16 value)
        {
            MySqlCommand Command = new MySqlCommand(MySqlCommandType.UPDATE);
            Command.Update("entities")
                .Set(row, value)
                .Where("uid", UID)
                .Execute();
        }
        public void Save(String row, Boolean value)
        {
            MySqlCommand Command = new MySqlCommand(MySqlCommandType.UPDATE);
            Command.Update("entities")
                .Set(row, value)
                .Where("uid", UID)
                .Execute();
        }
        public void Save(String row, UInt32 value)
        {
            MySqlCommand Command = new MySqlCommand(MySqlCommandType.UPDATE);
            Command.Update("entities")
                .Set(row, value)
                .Where("uid", UID)
                .Execute();
        }
        #endregion
        #region Send Screen Acessor
        public void SendScreen(Interfaces.IPacket Data)
        {
            Client.GameClient[] Chars = new Client.GameClient[Kernel.GamePool.Count];
            Kernel.GamePool.Values.CopyTo(Chars, 0);
            foreach (Client.GameClient C in Chars)
                if (C != null)
                    if (C.Entity != null)
                        if (Game.Calculations.PointDistance(X, Y, C.Entity.X, C.Entity.Y) <= 20)
                            C.Send(Data);
            Chars = null;

        }
        #endregion
        public void DieString()
        {
            _String str = new _String(true);
            str.UID = this.UID;
            str.Type = _String.Effect;
            str.Texts.Add("ghost");
            str.Texts.Add("1ghost");
            str.TextsCount = 1;
            if (EntityFlag == Game.EntityFlag.Player)
            {
                this.SendScreen(str);
            }
        }
        #region Functions
        private uint _MagicDefenceItem = 0;
        public uint MagicDefenceItem
        {
            get
            {
                return _MagicDefenceItem;
            }
            set { _MagicDefenceItem = value; }
        }
        private uint _MagicAttackPlus = 0;
        public uint MagicAttackPlus
        {
            get
            {
                return _MagicAttackPlus;
            }
            set { _MagicAttackPlus = value; }
        }
        public uint MagicDefencePlus
        {
            get
            {
                return MagicDefencePlus;
            }
            set { MagicDefencePlus = value; }
        }
        public UInt16 BattlePowerCalc(Entity e)
        {
            UInt16 BP = (ushort)(e.Level + ExtraBattlePower);

            if (e == null) return 0;
            if (e.Owner == null) return 0;
            var weapons = e.Owner.Weapons;
            foreach (ConquerItem i in e.Owner.Equipment.Objects)
            {
                if (i == null) continue;
                int pos = i.Position; if (pos > 20) pos -= 20;
                if (pos != ConquerItem.Bottle &&
                    pos != ConquerItem.Garment && pos != ConquerItem.RightWeaponAccessory && pos != ConquerItem.LeftWeaponAccessory && pos != ConquerItem.SteedArmor)
                {
                    if (!i.IsWorn) continue;
                    if (pos == ConquerItem.RightWeapon || pos == ConquerItem.LeftWeapon)
                        continue;
                    BP += ItemBatlePower(i);
                }
            }
            if (weapons.Item1 != null)
            {
                var i = weapons.Item1;
                Byte Multiplier = 1;
                if (i.IsTwoHander())
                    Multiplier = weapons.Item2 == null ? (Byte)2 : (Byte)1;
                BP += (ushort)(ItemBatlePower(i) * Multiplier);
            }
            if (weapons.Item2 != null)
                BP += ItemBatlePower(weapons.Item2);
            if (EntityFlag == Game.EntityFlag.Player)
            {
                if (Owner.DoChampStats)
                    BP += (Byte)(Math.Min((byte)e.NobilityRank, Owner.ChampionAllowedStats[Owner.ChampionStats.Grade][8]));
                else
                    BP += (Byte)e.NobilityRank;
            }
            BP += (Byte)(e.Reborn * 5);

            return BP;
        }
        public string NewName = "";
        private uint _EditeName;
        public uint EditeName
        {
            get
            {

                return _EditeName;
            }
            set
            {
                _EditeName = value;
            }
        }
        public uint AssassinColor
        {
            get
            {
                return BitConverter.ToUInt32(this.SpawnPacket, 233 + 4);
            }
            set
            {
                WriteUInt32(value, 233 + 4, this.SpawnPacket);
            }
        }
        private ushort ItemBatlePower(ConquerItem i)
        {
            Byte Multiplier = 1;
            Byte quality = (Byte)(i.ID % 10);
            int BP = 0;
            if (quality >= 6)
            {
                BP += (Byte)((quality - 5) * Multiplier);
            }
            if (i.SocketOne != 0)
            {
                BP += (Byte)(1 * Multiplier);
                if ((Byte)i.SocketOne % 10 == 3)
                    BP += (Byte)(1 * Multiplier);
                if (i.SocketTwo != 0)
                {
                    BP += (Byte)(1 * Multiplier);
                    if ((Byte)i.SocketTwo % 10 == 3)
                        BP += (Byte)(1 * Multiplier);
                }
            }
            BP += (Byte)(i.Plus * Multiplier);
            return (ushort)BP;
        }
        public Entity(EntityFlag Flag, bool companion)
        {
            Companion = companion;
            this.BodyGuard = companion;
            this.EntityFlag = Flag;
            Mode = Enums.Mode.None;
            update = new Conquer_Online_Server.Network.GamePackets.Update(true);
            update.UID = UID;
            switch (Flag)
            {
                case EntityFlag.Player:
                    MapObjType = Game.MapObjectType.Player;
                    Halos = new ConcurrentDictionary<int, DateTime>();
                    break;
                case EntityFlag.Monster: MapObjType = Game.MapObjectType.Monster; break;
            }
            SpawnPacket = new byte[0];
        }

        public void Ressurect()
        {
            if (EntityFlag == EntityFlag.Player)
                Owner.Send(new MapStatus() { BaseID = Owner.Map.BaseID, ID = Owner.Map.ID, Status = Database.MapsTable.MapInformations[Owner.Map.ID].Status, Weather = Database.MapsTable.MapInformations[Owner.Map.ID].Weather });
        }
        public void BringToLife()
        {
            Hitpoints = MaxHitpoints;
            TransformationID = 0;
            Stamina = 100;
            FlashingNameTime = 0;
            FlashingNameStamp = Time32.Now;
            RemoveFlag(Network.GamePackets.Update.Flags.FlashingName);
            RemoveFlag(Network.GamePackets.Update.Flags.Dead | Network.GamePackets.Update.Flags.Ghost);
            if (EntityFlag == EntityFlag.Player)
                Owner.Send(new MapStatus() { BaseID = Owner.Map.BaseID, ID = Owner.Map.ID, Status = Database.MapsTable.MapInformations[Owner.Map.ID].Status });
            if (EntityFlag == Game.EntityFlag.Player)
            {
                Owner.ReviveStamp = Time32.Now;
                Owner.Attackable = false;
            }
        }
        public void DropRandomStuff(Entity KillerName)
        {
            if (Money > 100)
            {
                int amount = (int)(Money / 2);
                amount = Kernel.Random.Next(amount);
                if (Kernel.Rate(40))
                {
                    uint ItemID = Network.PacketHandler.MoneyItemID((uint)amount);
                    ushort x = X, y = Y;
                    Game.Map Map = Kernel.Maps[MapID];
                    if (Map.SelectCoordonates(ref x, ref y))
                    {
                        Money -= (uint)amount;
                        Network.GamePackets.FloorItem floorItem = new Network.GamePackets.FloorItem(true);
                        floorItem.ValueType = Network.GamePackets.FloorItem.FloorValueType.Money;
                        floorItem.Value = (uint)amount;
                        floorItem.ItemID = ItemID;
                        floorItem.MapID = MapID;
                        floorItem.MapObjType = Game.MapObjectType.Item;
                        floorItem.X = x;
                        floorItem.Y = y;
                        floorItem.Type = Network.GamePackets.FloorItem.Drop;
                        floorItem.OnFloor = Time32.Now;
                        floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                        while (Map.Npcs.ContainsKey(floorItem.UID))
                            floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                        Map.AddFloorItem(floorItem);
                        Owner.SendScreenSpawn(floorItem, true);
                    }
                }
            }
            if (Owner.Inventory.Count > 0)
            {
                var array = Owner.Inventory.Objects.ToArray();
                uint count = (uint)(array.Length / 4);
                byte startfrom = (byte)Kernel.Random.Next((int)count);
                for (int c = 0; c < count; c++)
                {
                    int index = c + startfrom;
                    if (array[index] != null)
                    {
                        if (PKPoints > 99)
                        {
                            if (array[index].Lock == 0)
                            {
                                if (array[index].UnlockEnd > DateTime.Now.AddDays(1))
                                {
                                    if (!array[index].Bound && !array[index].Inscribed && array[index].ID != 723753)
                                    {
                                        if (!array[index].Suspicious && array[index].Lock != 1 && array[index].ID != 723755 && array[index].ID != 723767 && array[index].ID != 723772)
                                        {
                                            if (Kernel.Rate(140) && array[index].ID != 723774 && array[index].ID != 723776)
                                            {
                                                var Item = array[index];
                                                if (Item.ID >= 729960 && Item.ID <= 729970) return;
                                                Item.Lock = 0;
                                                var infos = Database.ConquerItemInformation.BaseInformations[(uint)Item.ID];
                                                ushort x = X, y = Y;
                                                Game.Map Map = Kernel.Maps[MapID];
                                                if (Map.SelectCoordonates(ref x, ref y))
                                                {
                                                    Network.GamePackets.FloorItem floorItem = new Network.GamePackets.FloorItem(true);
                                                    Owner.Inventory.Remove(Item, Enums.ItemUse.Remove);
                                                    floorItem.Item = Item;
                                                    floorItem.ValueType = Network.GamePackets.FloorItem.FloorValueType.Item;
                                                    floorItem.ItemID = (uint)Item.ID;
                                                    floorItem.MapID = MapID;
                                                    floorItem.MapObjType = Game.MapObjectType.Item;
                                                    floorItem.X = x;
                                                    floorItem.Y = y;
                                                    floorItem.Type = Network.GamePackets.FloorItem.Drop;
                                                    floorItem.OnFloor = Time32.Now;
                                                    floorItem.ItemColor = floorItem.Item.Color;
                                                    floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                                                    while (Map.Npcs.ContainsKey(floorItem.UID))
                                                        floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                                                    Map.AddFloorItem(floorItem);
                                                    Owner.SendScreenSpawn(floorItem, true);
                                                }
                                            }
                                        }

                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (PKPoints >= 30 && Killer != null && Killer.Owner != null)
            {
                foreach (var Item in Owner.Equipment.Objects)
                {
                    if (Item != null)
                    {
                        //5 = LeftHand, 9 = Garment, 12 = Horse
                        if (Item.Position == 9 || Item.Position == 12)
                            return;
                        if (Item.Position == 5)
                            if (Item.ID.ToString().StartsWith("105"))
                                return;
                        if (Kernel.Rate(35 + (int)(PKPoints > 30 ? 75 : 0)))
                        {
                            ushort x = X, y = Y;
                            Game.Map Map = Kernel.Maps[MapID];
                            if (Map.SelectCoordonates(ref x, ref y))
                            {
                                Owner.Equipment.RemoveToGround(Item.Position);
                                var infos = Database.ConquerItemInformation.BaseInformations[(uint)Item.ID];

                                Network.GamePackets.FloorItem floorItem = new Network.GamePackets.FloorItem(true);
                                floorItem.Item = Item;
                                floorItem.ValueType = Network.GamePackets.FloorItem.FloorValueType.Item;
                                floorItem.ItemID = (uint)Item.ID;
                                floorItem.MapID = MapID;
                                floorItem.MapObjType = Game.MapObjectType.Item;
                                floorItem.X = x;
                                floorItem.Y = y;
                                floorItem.Type = Network.GamePackets.FloorItem.DropDetain;
                                floorItem.OnFloor = Time32.Now;
                                floorItem.ItemColor = floorItem.Item.Color;
                                floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                                while (Map.Npcs.ContainsKey(floorItem.UID))
                                    floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                                Owner.SendScreenSpawn(floorItem, true);

                                Database.DetainedItemTable.DetainItem(Item, Owner, Killer.Owner);
                                Owner.Equipment.UpdateEntityPacket();
                                ClientEquip eq = new ClientEquip(Owner);
                                eq.DoEquips(Owner);
                                Owner.Send(eq);
                                break;
                            }
                        }
                    }
                }
            }
            if (PKPoints > 99)
            {
                if (KillerName.EntityFlag == EntityFlag.Player)
                {
                    Kernel.SendWorldMessage(new Network.GamePackets.Message(Name + " has been captured by " + KillerName.Name + " and sent in jail! The world is now safer!", System.Drawing.Color.Red, Message.Talk), Program.GamePool);
                    Teleport(6000, 50, 50);
                }
                else
                {
                    Kernel.SendWorldMessage(new Network.GamePackets.Message(Name + " has been captured and sent in jail! The world is now safer!", System.Drawing.Color.Red, Message.Talk), Program.GamePool);
                    Teleport(6000, 50, 50);
                }
                //Teleport(6000, 30, 76);
            }
        }
        public static double GetAngle(ushort x, ushort y, ushort x2, ushort y2)
        {
            double xf1 = x, xf2 = x2, yf1 = y, yf2 = y2;
            double result = 90 - Math.Atan((xf1 - xf2) / (yf1 - yf2)) * 180 / Math.PI;
            if (xf1 - xf2 < 0 && yf1 - yf2 < 0)
                result += 180;
            else if (xf1 - xf2 == 0 && yf1 - yf2 < 0)
                result += 180;
            else if (yf1 - yf2 < 0 && xf1 - xf2 > 0)
                result -= 180;
            return result;
        }
        public class Vector { public ushort X, Y; }
        public static Vector GetBorderCoords(ushort old_x, ushort old_y, ushort Target_x, ushort Target_y)
        {
            double Θ = GetAngle(old_x, old_y, Target_x, Target_y);
            double w, h;
            Vector v = new Vector();
            byte quadrant = 1;
            if (Θ < 0)
                Θ += 360;
            else if (Θ == 360)
                Θ = 0;
            while (Θ >= 90)
            {
                Θ -= 90;
                quadrant++;
            }
            double screendistance = ScreenDistance;
            if (quadrant == 1)
            {
                screendistance = ScreenDistance / (Math.Cos(Θ * Math.PI / 180));
                if (screendistance > 25)
                    screendistance = ScreenDistance / (Math.Sin(Θ * Math.PI / 180));
                else if (Θ != 0)
                    v.Y++;
                h = screendistance * (Math.Sin(Θ * Math.PI / 180));
                w = screendistance * (Math.Cos(Θ * Math.PI / 180));
                v.X += (ushort)(Target_x + Math.Round(w));
                if (Θ == 90)
                    v.Y += (ushort)(Target_y - Math.Round(h));
                else
                    v.Y += (ushort)(Target_y + Math.Round(h));
            }
            else if (quadrant == 2)
            {
                screendistance = ScreenDistance / (Math.Cos(Θ * Math.PI / 180));
                if (screendistance > 25)
                {
                    screendistance = ScreenDistance / (Math.Sin(Θ * Math.PI / 180));
                    v.Y++;
                }
                w = screendistance * (Math.Sin(Θ * Math.PI / 180));
                h = screendistance * (Math.Cos(Θ * Math.PI / 180));
                v.X += (ushort)(Target_x - w);
                v.Y += (ushort)(Target_y + h);
            }
            else if (quadrant == 3)
            {
                screendistance = ScreenDistance / (Math.Cos(Θ * Math.PI / 180));
                if (screendistance > 25)
                    screendistance = ScreenDistance / (Math.Sin(Θ * Math.PI / 180));
                h = screendistance * (Math.Sin(Θ * Math.PI / 180));
                w = screendistance * (Math.Cos(Θ * Math.PI / 180));
                v.X += (ushort)(Target_x - w);
                v.Y += (ushort)(Target_y - h);
            }
            else if (quadrant == 4)
            {
                screendistance = ScreenDistance / (Math.Cos(Θ * Math.PI / 180));
                if (screendistance > 25)
                    screendistance = ScreenDistance / (Math.Sin(Θ * Math.PI / 180));
                else if (Θ > 0)
                    v.X++;
                w = screendistance * (Math.Sin(Θ * Math.PI / 180));
                h = screendistance * (Math.Cos(Θ * Math.PI / 180));
                v.X += (ushort)(Target_x + w);
                v.Y += (ushort)(Target_y - h);
            }
            return v;
        }

        public void Die(UInt32 killer)//replace this one too for die delay
        {
            RemoveFlag2(Network.GamePackets.Update.Flags2.CarryingFlag);
            if (EntityFlag == EntityFlag.Player)
            {
                Owner.XPCount = 0;
                if (Owner.Booth != null)
                {
                    Owner.Booth.Remove();
                    Owner.Booth = null;
                }
            }
            Killed = true;
            Hitpoints = 0;
            DeathStamp = Time32.Now;
            ToxicFogLeft = 0;
            if (Companion)
            {
                AddFlag(Network.GamePackets.Update.Flags.Ghost | Network.GamePackets.Update.Flags.Dead | Network.GamePackets.Update.Flags.FadeAway);
                Network.GamePackets.Attack attack = new Network.GamePackets.Attack(true);
                attack.Attacked = UID;
                attack.AttackType = Network.GamePackets.Attack.Kill;
                attack.X = X;
                attack.Y = Y;
                MonsterInfo.SendScreen(attack);
                Owner.Map.RemoveEntity(this);
                if (Owner.Entity.MyClones.ContainsKey(UID))
                {
                    Owner.Entity.MyClones.Remove(UID);
                    return;
                }
                Owner.Companion = null;
            }  
            if (this.BodyGuard)
            {
                this.AddFlag(0xc20L);
                Network.GamePackets.Attack attack = new Network.GamePackets.Attack(true)
                {
                    Attacked = this.UID,
                    AttackType = 14,
                    X = this.X,
                    Y = this.Y
                };
                this.MonsterInfo.SendScreen(attack);
                this.Owner.Map.RemoveEntity(this);
                this.Owner.BodyGuard = null;
            }
            if (EntityFlag == EntityFlag.Player)
            {
                if (Constants.PKFreeMaps.Contains(MapID))
                    goto Over;

                //DropRandomStuff(Killer);

            Over:
                AddFlag(Network.GamePackets.Update.Flags.Dead);
                RemoveFlag(Network.GamePackets.Update.Flags.Fly);
                RemoveFlag(Network.GamePackets.Update.Flags.Ride);
                RemoveFlag(Network.GamePackets.Update.Flags.Cyclone);
                RemoveFlag((uint)1UL << 0x16);
                RemoveFlag(Network.GamePackets.Update.Flags.Superman);
                RemoveFlag(Network.GamePackets.Update.Flags.FatalStrike);
                RemoveFlag(Network.GamePackets.Update.Flags.FlashingName);
                RemoveFlag(Network.GamePackets.Update.Flags.ShurikenVortex);
                RemoveFlag2(Network.GamePackets.Update.Flags2.Oblivion);

                Network.GamePackets.Attack attack = new Attack(true);
                attack.AttackType = Network.GamePackets.Attack.Kill;
                attack.X = X;
                attack.Y = Y;
                attack.Attacked = UID;
                attack.Attacker = killer;
                attack.Damage = 0;
                Owner.SendScreen(attack, true);

                //  if (Body % 10 < 3)
                //     TransformationID = 99;
                // else
                //    TransformationID = 98;

                Owner.Send(new MapStatus() { BaseID = Owner.Map.BaseID, ID = Owner.Map.ID, Status = Database.MapsTable.MapInformations[Owner.Map.ID].Status, Weather = Database.MapsTable.MapInformations[Owner.Map.ID].Weather });

                Owner.EndQualifier();
            }
            else
            {
                Kernel.Maps[MapID].Floor[X, Y, MapObjType, this] = true;
            }
            if (EntityFlag == EntityFlag.Player)
                if (OnDeath != null) OnDeath(this);
        }
        public Entity Killer;
        public static string GetMapName(uint MapID)
        {
            string mapName = "Unknown";
            switch (MapID)
            {
                case 0x259:
                    mapName = "OfflineTG";
                    break;

                case 700:
                    mapName = "LotteryMap";
                    break;

                case 0x3e8:
                    mapName = "Desert";
                    break;

                case 0x3e9:
                    mapName = "MysticCastle";
                    break;

                case 0x3ea:
                    mapName = "CentralPlain";
                    break;

                case 0x3eb:
                    mapName = "MineCave";
                    break;

                case 0x3ec:
                    mapName = "JobCenter";
                    break;

                case 0x3ed:
                    mapName = "Arena";
                    break;

                case 0x3ee:
                    mapName = "Stable";
                    break;

                case 0x3ef:
                    mapName = "Blachsmith";
                    break;

                case 0x3f0:
                    mapName = "Grocery";
                    break;

                case 0x3f1:
                    mapName = "ArmorStore";
                    break;

                case 0x3f2:
                    mapName = "BirthVillage";
                    break;

                case 0x3f3:
                    mapName = "Forest";
                    break;

                case 0x3f4:
                    mapName = "Dreamland";
                    break;

                case 0x3f5:
                    mapName = "TigerCave";
                    break;

                case 0x3f6:
                    mapName = "DragonPool";
                    break;

                case 0x3f7:
                    mapName = "Island";
                    break;

                case 0x3f8:
                    mapName = "KylinCave";
                    break;

                case 0x3fa:
                    mapName = "Arena";
                    break;

                case 0x3fc:
                    mapName = "Canyon";
                    break;

                case 0x3fd:
                    mapName = "CopperMine";
                    break;

                case 0x401:
                    mapName = "IronMine";
                    break;

                case 0x402:
                    mapName = "CopperMine";
                    break;

                case 0x403:
                    mapName = "SilverMine";
                    break;

                case 0x404:
                    mapName = "GoldMine";
                    break;

                case 0x40c:
                    mapName = "Market";
                    break;

                case 0x40e:
                    mapName = "GuildArea";
                    break;

                case 0x40f:
                    mapName = "TrainingGround";
                    break;

                case 0x410:
                    mapName = "SkyCityPass";
                    break;

                case 0x411:
                    mapName = "PrizeClaimingMa";
                    break;

                case 0x412:
                    mapName = "PassPortal";
                    break;

                case 0x413:
                    mapName = "Peace";
                    break;

                case 0x414:
                    mapName = "Chaos";
                    break;

                case 0x415:
                    mapName = "Deserted";
                    break;

                case 0x416:
                    mapName = "Prosperous";
                    break;

                case 0x417:
                    mapName = "Disturbed";
                    break;

                case 0x418:
                    mapName = "Calmed";
                    break;

                case 0x419:
                    mapName = "Death";
                    break;

                case 0x41a:
                    mapName = "Life";
                    break;

                case 0x41b:
                    mapName = "MysticIsland";
                    break;

                case 0x41c:
                    mapName = "TestIsland";
                    break;

                case 0x424:
                    mapName = "Maze1";
                    break;

                case 0x425:
                    mapName = "Maze2";
                    break;

                case 0x426:
                    mapName = "Maze3";
                    break;

                case 0x427:
                    mapName = "AdventureIsland";
                    break;

                case 0x42e:
                    mapName = "SnakeDen";
                    break;

                case 0x430:
                    mapName = "CityArena4";
                    break;

                case 0x431:
                    mapName = "Arena1";
                    break;

                case 0x432:
                    mapName = "Arena2";
                    break;

                case 0x433:
                    mapName = "NewCanyon";
                    break;

                case 0x434:
                    mapName = "NewForest";
                    break;

                case 0x435:
                    mapName = "NewDesert";
                    break;

                case 0x436:
                    mapName = "NewIsland";
                    break;

                case 0x438:
                    mapName = "Arena2";
                    break;

                case 0x439:
                    mapName = "Arena3";
                    break;

                case 0x442:
                    mapName = "Arena1";
                    break;

                case 0x443:
                    mapName = "Arena2";
                    break;

                case 0x444:
                    mapName = "Arena1";
                    break;

                case 0x445:
                    mapName = "Arena2";
                    break;

                case 0x446:
                    mapName = "Arena1";
                    break;

                case 0x447:
                    mapName = "Arena2";
                    break;

                case 0x44c:
                    mapName = "MoonPlatform";
                    break;

                case 0x44d:
                    mapName = "MoonPlatform";
                    break;

                case 0x44e:
                    mapName = "MoonPlatform";
                    break;

                case 0x44f:
                    mapName = "MoonPlatform";
                    break;

                case 0x450:
                    mapName = "MoonPlatform";
                    break;

                case 0x451:
                    mapName = "MoonPlatform";
                    break;

                case 0x452:
                    mapName = "MoonPlatform";
                    break;

                case 0x453:
                    mapName = "MoonPlatform";
                    break;

                case 0x454:
                    mapName = "MoonPlatform";
                    break;

                case 0x455:
                    mapName = "MoonPlatform";
                    break;

                case 0x4b1:
                    mapName = "GlobeQuest1";
                    break;

                case 0x4b2:
                    mapName = "GlobeQuest2";
                    break;

                case 0x4b4:
                    mapName = "GlobeQuest4";
                    break;

                case 0x4b5:
                    mapName = "GlobeQuest5";
                    break;

                case 0x4b7:
                    mapName = "GlobeQuest7";
                    break;

                case 0x4b8:
                    mapName = "GlobeQuest8";
                    break;

                case 0x4ba:
                    mapName = "GlobeQuest10";
                    break;

                case 0x4bb:
                    mapName = "GlobeQuest11";
                    break;

                case 0x4bc:
                    mapName = "GlobeIsland";
                    break;

                case 0x4bd:
                    mapName = "GlobeDesert";
                    break;

                case 0x4be:
                    mapName = "GlobeCanyon";
                    break;

                case 0x4bf:
                    mapName = "GlobeForest";
                    break;

                case 0x4c0:
                    mapName = "GlobePlain";
                    break;

                case 0x4c1:
                    mapName = "JointCanyon";
                    break;

                case 0x4c2:
                    mapName = "IronMine1";
                    break;

                case 0x4c3:
                    mapName = "GlobeExit";
                    break;

                case 0x514:
                    mapName = "MysticCave";
                    break;

                case 0x547:
                    mapName = "Labyrinth";
                    break;

                case 0x548:
                    mapName = "Labyrinth";
                    break;

                case 0x549:
                    mapName = "Labyrinth";
                    break;

                case 0x54a:
                    mapName = "Labyrinth";
                    break;

                case 0x5ab:
                    mapName = "MeteorArena";
                    break;

                case 0x5dc:
                    mapName = "ClassPKArena1";
                    break;

                case 0x5dd:
                    mapName = "ClassPKArena2";
                    break;

                case 0x5de:
                    mapName = "ClassPKArena3";
                    break;

                case 0x5e1:
                    mapName = "CityArena1";
                    break;

                case 0x5e2:
                    mapName = "CityArena2";
                    break;

                case 0x5e4:
                    mapName = "CityArena4";
                    break;

                case 0x5e7:
                    mapName = "FurnitureStore";
                    break;

                case 0x5eb:
                    mapName = "CityArena1";
                    break;

                case 0x5ec:
                    mapName = "CityArena2";
                    break;

                case 0x5ee:
                    mapName = "CityArena4";
                    break;

                case 0x5f5:
                    mapName = "CityArena1";
                    break;

                case 0x5f6:
                    mapName = "CityArena2";
                    break;

                case 0x5f8:
                    mapName = "CityArena4";
                    break;

                case 0x60e:
                    mapName = "HalloweenCity1";
                    break;

                case 0x60f:
                    mapName = "HalloweenCity1";
                    break;

                case 0x6a4:
                    mapName = "EvilAbyss";
                    break;

                case 0x6e3:
                    mapName = "Dreamland";
                    break;

                case 0x6e4:
                    mapName = "Dreamland";
                    break;

                case 0x6e5:
                    mapName = "Hall";
                    break;

                case 0x6e8:
                    mapName = "KunLun";
                    break;

                case 0x6e9:
                    mapName = "Garden";
                    break;

                case 0x6ea:
                    mapName = "ArenaStage1";
                    break;

                case 0x6eb:
                    mapName = "ArenaStage2";
                    break;

                case 0x6ec:
                    mapName = "ArenaStage3";
                    break;

                case 0x6ed:
                    mapName = "ArenaStage4";
                    break;

                case 0x6ee:
                    mapName = "ArenaStage5";
                    break;

                case 0x6ef:
                    mapName = "ArenaStage6";
                    break;

                case 0x6f1:
                    mapName = "ArenaStage7";
                    break;

                case 0x6f2:
                    mapName = "DangerCave";
                    break;

                case 0x6f3:
                    mapName = "GhostCity";
                    break;

                case 0x6f4:
                    mapName = "DarkCity";
                    break;

                case 0x6f6:
                    mapName = "TreasureHouse";
                    break;

                case 0x6f7:
                    mapName = "TreasureHouse1";
                    break;

                case 0x6f8:
                    mapName = "Hut";
                    break;

                case 0x6f9:
                    mapName = "Dungeon1F";
                    break;

                case 0x6fa:
                    mapName = "Dungeon2F";
                    break;

                case 0x6fb:
                    mapName = "Dungeon3F";
                    break;

                case 0x6ff:
                    mapName = "RoseGarden";
                    break;

                case 0x700:
                    mapName = "SwanLake";
                    break;

                case 0x702:
                    mapName = "ViperCave";
                    break;

                case 0x709:
                    mapName = "Crypt";
                    break;

                case 0x70e:
                    mapName = "OrchidGarden";
                    break;

                case 0x70f:
                    mapName = "LockerRoomA";
                    break;

                case 0x710:
                    mapName = "MalePKArena";
                    break;

                case 0x711:
                    mapName = "LockerRoomB";
                    break;

                case 0x712:
                    mapName = "FemalePKArena";
                    break;

                case 0x714:
                    mapName = "ExtremePKArena";
                    break;

                case 0x71a:
                    mapName = "BanditChamer";
                    break;

                case 0x72d:
                    mapName = "ClassPKArena4";
                    break;

                case 0x72e:
                    mapName = "ClassPKArena5";
                    break;

                case 0x72f:
                    mapName = "ClassPKArena6";
                    break;

                case 0x742:
                    mapName = "PokerRoom";
                    break;

                case 0x744:
                    mapName = "VIPPokerRoom";
                    break;

                case 0x747:
                    mapName = "TwinCityArena";
                    break;

                case 0x748:
                    mapName = "WindPlainArena";
                    break;

                case 0x74c:
                    mapName = "PhoenixCastleArena";
                    break;

                case 0x74d:
                    mapName = "MapleForestArena";
                    break;

                case 0x751:
                    mapName = "ApeCityArena";
                    break;

                case 0x752:
                    mapName = "LoveCanyonArena";
                    break;

                case 0x756:
                    mapName = "BirdIslandArena";
                    break;

                case 0x757:
                    mapName = "BirdIslandArena";
                    break;

                case 0x75b:
                    mapName = "DesertCityArena";
                    break;

                case 0x75c:
                    mapName = "DesertArena";
                    break;

                case 0x760:
                    mapName = "LotteryHouse";
                    break;

                case 0x761:
                    mapName = "CouplesPKGround";
                    break;

                case 0x786:
                    mapName = "FrozenGrotto1";
                    break;

                case 0x787:
                    mapName = "FrozenGrotto2";
                    break;

                case 0x788:
                    mapName = "ClassPKArena10";
                    break;

                case 0x79a:
                    mapName = "ClassPKArena7";
                    break;

                case 0x79b:
                    mapName = "ClassPKArena8";
                    break;

                case 0x79c:
                    mapName = "ClassPKArena9";
                    break;

                case 0x79e:
                    mapName = "HorseRacing";
                    break;

                case 0x79f:
                    mapName = "RockMonsterDen";
                    break;

                case 0x7a9:
                    mapName = "Mausoleum";
                    break;

                case 0x7aa:
                    mapName = "Mausoleum";
                    break;

                case 0x7ab:
                    mapName = "Mausoleum";
                    break;

                case 0x7ac:
                    mapName = "Mausoleum";
                    break;

                case 0x7ad:
                    mapName = "Mausoleum";
                    break;

                case 0x7ae:
                    mapName = "Mausoleum";
                    break;

                case 0x7af:
                    mapName = "Mausoleum";
                    break;

                case 0x7b0:
                    mapName = "Mausoleum";
                    break;

                case 0x7b1:
                    mapName = "Mausoleum";
                    break;

                case 0x7b3:
                    mapName = "Mausoleum";
                    break;

                case 0x7b4:
                    mapName = "Mausoleum";
                    break;

                case 0x7b5:
                    mapName = "Mausoleum";
                    break;

                case 0x7b6:
                    mapName = "Mausoleum";
                    break;

                case 0x7b7:
                    mapName = "Mausoleum";
                    break;

                case 0x7b8:
                    mapName = "Mausoleum";
                    break;

                case 0x7b9:
                    mapName = "Mausoleum";
                    break;

                case 0x7ba:
                    mapName = "Mausoleum";
                    break;

                case 0x7bb:
                    mapName = "Mausoleum";
                    break;

                case 0x7bd:
                    mapName = "Mausoleum";
                    break;

                case 0x7be:
                    mapName = "Mausoleum";
                    break;

                case 0x7bf:
                    mapName = "Mausoleum";
                    break;

                case 0x7c0:
                    mapName = "Mausoleum";
                    break;

                case 0x7c1:
                    mapName = "Mausoleum";
                    break;

                case 0x7c2:
                    mapName = "Mausoleum";
                    break;

                case 0x7c3:
                    mapName = "Mausoleum";
                    break;

                case 0x7c4:
                    mapName = "Mausoleum";
                    break;

                case 0x7c5:
                    mapName = "Mausoleum";
                    break;

                case 0x7cf:
                    mapName = "FrozenGrotto3";
                    break;

                case 0x7d0:
                    mapName = "IronMine2";
                    break;

                case 0x7d1:
                    mapName = "IronMine2F";
                    break;

                case 0x7d2:
                    mapName = "IronMine3F";
                    break;

                case 0x7d3:
                    mapName = "IronMine3F";
                    break;

                case 0x7d4:
                    mapName = "IronMine3F";
                    break;

                case 0x7d5:
                    mapName = "IronMine3F";
                    break;

                case 0x7d6:
                    mapName = "IronMine4F";
                    break;

                case 0x7d7:
                    mapName = "IronMine4F";
                    break;

                case 0x7d8:
                    mapName = "IronMine4F";
                    break;

                case 0x7d9:
                    mapName = "IronMine4F";
                    break;

                case 0x7da:
                    mapName = "IronMine4F";
                    break;

                case 0x7db:
                    mapName = "IronMine4F";
                    break;

                case 0x7dc:
                    mapName = "IronMine4F";
                    break;

                case 0x7dd:
                    mapName = "IronMine4F";
                    break;

                case 0x7e4:
                    mapName = "CopperMine2F";
                    break;

                case 0x7e5:
                    mapName = "CopperMine2F";
                    break;

                case 0x7e6:
                    mapName = "CopperMine3F";
                    break;

                case 0x7e7:
                    mapName = "CopperMine3F";
                    break;

                case 0x7e8:
                    mapName = "CopperMine3F";
                    break;

                case 0x7e9:
                    mapName = "CopperMine3F";
                    break;

                case 0x7ea:
                    mapName = "CopperMine4F";
                    break;

                case 0x7eb:
                    mapName = "CopperMine4F";
                    break;

                case 0x7ec:
                    mapName = "CopperMine4F";
                    break;

                case 0x7ed:
                    mapName = "CopperMine4F";
                    break;

                case 0x7ee:
                    mapName = "CopperMine4F";
                    break;

                case 0x7ef:
                    mapName = "CopperMine4F";
                    break;

                case 0x7f0:
                    mapName = "CopperMine4F";
                    break;

                case 0x7f1:
                    mapName = "CopperMine4F";
                    break;

                case 0x7f8:
                    mapName = "SilverMine2F";
                    break;

                case 0x7f9:
                    mapName = "SilverMine2F";
                    break;

                case 0x7fa:
                    mapName = "SilverMine3F";
                    break;

                case 0x7fb:
                    mapName = "SilverMine3F";
                    break;

                case 0x7fc:
                    mapName = "SilverMine3F";
                    break;

                case 0x7fd:
                    mapName = "SilverMine3F";
                    break;

                case 0x7fe:
                    mapName = "SilverMine4F";
                    break;

                case 0x7ff:
                    mapName = "SilverMine4F";
                    break;

                case 0x800:
                    mapName = "SilverMine4F";
                    break;

                case 0x801:
                    mapName = "SilverMine4F";
                    break;

                case 0x802:
                    mapName = "SilverMine4F";
                    break;

                case 0x803:
                    mapName = "SilverMine4F";
                    break;

                case 0x804:
                    mapName = "SilverMine4F";
                    break;

                case 0x805:
                    mapName = "SilverMine4F";
                    break;

                case 0x806:
                    mapName = "FrozenGrotto4";
                    break;

                case 0x807:
                    mapName = "FrozenGrotto5";
                    break;

                case 0x808:
                    mapName = "FrozenGrotto6";
                    break;

                case 0x80c:
                    mapName = "GuildContest";
                    break;

                case 0x817:
                    mapName = "GuildPKTournamentMap";
                    break;

                case 0x818:
                    mapName = "GuildPKTournamentMap";
                    break;

                case 0x819:
                    mapName = "GuildPKTournamentMap";
                    break;

                case 0x81a:
                    mapName = "GuildPKTournamentMap";
                    break;

                case 0x81b:
                    mapName = "ElitePKWaitingArea1";
                    break;

                case 0x81c:
                    mapName = "ElitePKWaitingArea2";
                    break;

                case 0x81d:
                    mapName = "ElitePKWaitingArea3";
                    break;

                case 0x81e:
                    mapName = "ElitePKWaitingArea4";
                    break;

                case 0xfb5:
                    mapName = "HellGate";
                    break;

                case 0xfb6:
                    mapName = "HellHall";
                    break;

                case 0xfb7:
                    mapName = "LeftCloister";
                    break;

                case 0xfb8:
                    mapName = "RightCloister";
                    break;

                case 0xfb9:
                    mapName = "BattleFormation";
                    break;

                case 0x1388:
                    mapName = "NPCJail";
                    break;

                case 0x138a:
                    mapName = "دفعه";
                    break;

                case 0x1770:
                    mapName = "PKerJail";
                    break;

                case 0x1771:
                    mapName = "Jail";
                    break;

                case 0x1772:
                    mapName = "MacroJail";
                    break;

                case 0x1773:
                    mapName = "BotJail";
                    break;

                case 0x177a:
                    mapName = "AFKerJail";
                    break;

                case 0x2276:
                    mapName = "FiendLairFloor1";
                    break;

                case 0x2277:
                    mapName = "FiendLairFloor2";
                    break;

                case 0x2278:
                    mapName = "FiendLairFloor3";
                    break;

                case 0x2279:
                    mapName = "FiendLairFloor4";
                    break;

                case 0x227a:
                    mapName = "FiendLairFloor5";
                    break;

                case 0x227b:
                    mapName = "FiendLairFloor6";
                    break;

                case 0x227c:
                    mapName = "FiendLairFloor7";
                    break;

                case 0x227d:
                    mapName = "FiendLairFloor8";
                    break;

                case 0x227e:
                    mapName = "FiendLairFloor9";
                    break;

                case 0x227f:
                    mapName = "FiendLairFloor10";
                    break;

                case 0x2280:
                    mapName = "FiendLairFloor11";
                    break;

                case 0x2281:
                    mapName = "FiendLairFloor12";
                    break;

                case 0x2282:
                    mapName = "FiendLairFloor13";
                    break;

                case 0x2283:
                    mapName = "FiendLairFloor14";
                    break;

                case 0x2284:
                    mapName = "FiendLairFloor15";
                    break;

                case 0x2285:
                    mapName = "FiendLairFloor16";
                    break;

                case 0x2286:
                    mapName = "FiendLairFloor17";
                    break;

                case 0x2287:
                    mapName = "FiendLairFloor18";
                    break;

                case 0xdbba0:
                    mapName = "PlayersArena";
                    break;

                case 0xde2b0:
                    mapName = "ElitePKTournament";
                    break;

                case 0xf4240:
                    mapName = "ClanQualifier";
                    break;

                case 0xbef:
                    mapName = "GaleShallow";
                    break;

                case 0xbf0:
                    mapName = "SeaOfDeath";
                    break;

                default:
                    mapName = "OtherMap";
                    break;
            }
            return mapName;

        }

        public void Die(Entity killer)
        {//TQ  


            #region Die Guild System
            if (killer.EntityFlag == EntityFlag.Player && EntityFlag == EntityFlag.Player)
            {

                if (Owner.Guild != null && killer.Owner.Guild != null && Owner.Map.ID == 1015)
                {
                    Owner.Guild.pkp_donation += 2;// AhmedRizK  
                    Owner.Guild.pkp_donation -= 2;
                    killer.Money += 20;
                    Kernel.SendWorldMessage(new Message("The " + killer.Owner.AsMember.Rank + " " + killer.Name + " of the Guild " + killer.Owner.Guild.Name + " has killed the " + killer.Owner.AsMember.Rank + " " + Name + " of the Guild " + Owner.Guild.Name + " at BirdIsland!", System.Drawing.Color.Yellow, Network.GamePackets.Message.Guild), Program.GamePool);
                }
                if (Owner.Guild != null && killer.Owner.Guild != null && Owner.Map.ID == 1020)
                {
                    Owner.Guild.pkp_donation += 2;
                    Owner.Guild.pkp_donation -= 2;
                    killer.Money += 20;
                    Kernel.SendWorldMessage(new Message("The " + killer.Owner.AsMember.Rank + " " + killer.Name + " of the Guild " + killer.Owner.Guild.Name + " has killed the " + killer.Owner.AsMember.Rank + " " + Name + " of the Guild " + Owner.Guild.Name + " at ApeCity!", System.Drawing.Color.Yellow, Network.GamePackets.Message.Guild), Program.GamePool);
                }
                if (Owner.Guild != null && killer.Owner.Guild != null && Owner.Map.ID == 1011)
                {
                    Owner.Guild.pkp_donation += 2;
                    Owner.Guild.pkp_donation -= 2;
                    killer.Money += 20;
                    Kernel.SendWorldMessage(new Message("The " + killer.Owner.AsMember.Rank + " " + killer.Name + " of the Guild " + killer.Owner.Guild.Name + " has killed the " + killer.Owner.AsMember.Rank + " " + Name + " of the Guild " + Owner.Guild.Name + " at PhoenixCastle!", System.Drawing.Color.Yellow, Network.GamePackets.Message.Guild), Program.GamePool);
                }
                if (Owner.Guild != null && killer.Owner.Guild != null && Owner.Map.ID == 1000)
                {
                    Owner.Guild.pkp_donation += 2;
                    Owner.Guild.pkp_donation -= 2;
                    killer.Money += 20;
                    Kernel.SendWorldMessage(new Message("The " + killer.Owner.AsMember.Rank + " " + killer.Name + " of the Guild " + killer.Owner.Guild.Name + " has killed the " + killer.Owner.AsMember.Rank + " " + Name + " of the Guild " + Owner.Guild.Name + " at DesertCity!", System.Drawing.Color.Yellow, Network.GamePackets.Message.Guild), Program.GamePool);
                }
                if (Owner.Guild != null && killer.Owner.Guild != null && Owner.Map.ID == 1001)
                {
                    Owner.Guild.pkp_donation += 2;
                    Owner.Guild.pkp_donation -= 2;
                    killer.Money += 20;
                    Kernel.SendWorldMessage(new Message("The " + killer.Owner.AsMember.Rank + " " + killer.Name + " of the Guild " + killer.Owner.Guild.Name + " has killed the " + killer.Owner.AsMember.Rank + " " + Name + " of the Guild " + Owner.Guild.Name + " at MysticCastle!", System.Drawing.Color.Yellow, Network.GamePackets.Message.Guild), Program.GamePool);
                }
                if (Owner.Guild != null && killer.Owner.Guild != null && Owner.Map.ID == 1762)
                {
                    Owner.Guild.pkp_donation += 2;
                    Owner.Guild.pkp_donation -= 2;
                    killer.Money += 20;
                    Kernel.SendWorldMessage(new Message("The " + killer.Owner.AsMember.Rank + " " + killer.Name + " of the Guild " + killer.Owner.Guild.Name + " has killed the " + killer.Owner.AsMember.Rank + " " + Name + " of the Guild " + Owner.Guild.Name + " at FrozenGrotto 2!", System.Drawing.Color.Yellow, Network.GamePackets.Message.Guild), Program.GamePool);
                }
                if (Owner.Guild != null && killer.Owner.Guild != null && Owner.Map.ID == 2056)
                {
                    Owner.Guild.pkp_donation += 2;
                    Owner.Guild.pkp_donation -= 2;
                    killer.Money += 20;
                    Kernel.SendWorldMessage(new Message("The " + killer.Owner.AsMember.Rank + " " + killer.Name + " of the Guild " + killer.Owner.Guild.Name + " has killed the " + killer.Owner.AsMember.Rank + " " + Name + " of the Guild " + Owner.Guild.Name + " at FrozenGrotto 6!", System.Drawing.Color.Yellow, Network.GamePackets.Message.Guild), Program.GamePool);
                }
            }
            #endregion
            if (ContainsFlag2(Network.GamePackets.Update.Flags2.CarryingFlag))
            {
                StaticEntity entity = new StaticEntity((uint)(X * 1000 + Y), X, Y, MapID);
                entity.DoFlag();
                Owner.Map.AddStaticEntity(entity);
                RemoveFlag2(Network.GamePackets.Update.Flags2.CarryingFlag);

                if (killer.GuildID != GuildID)
                    if (killer.Owner.Guild != null)
                        killer.Owner.Guild.CTFPoints += (uint)(Level / 5);
            }
            if (killer.MapID == 7777)
                killer.Owner.elitepoints += 1;
            if (EntityFlag == EntityFlag.Player)
            {
                Owner.XPCount = 0;
                if (Owner.Booth != null)
                {
                    Owner.Booth.Remove();
                    Owner.Booth = null;
                }
            }
            killer.KillCount++;
            killer.KillCount2++;
            Killer = killer;
            Hitpoints = 0;
            DeathStamp = Time32.Now;
            //DieString();  
            ToxicFogLeft = 0;
            if (Companion)
            {
                AddFlag(Network.GamePackets.Update.Flags.Ghost | Network.GamePackets.Update.Flags.Dead | Network.GamePackets.Update.Flags.FadeAway);
                Network.GamePackets.Attack zattack = new Network.GamePackets.Attack(true);
                zattack.Attacked = UID;
                zattack.AttackType = Network.GamePackets.Attack.Kill;
                zattack.X = X;
                zattack.Y = Y;
                MonsterInfo.SendScreen(zattack);
                Owner.Map.RemoveEntity(this);
                Owner.Companion = null;

                if (killer.EntityFlag == EntityFlag.Player)
                {
                    if (Constants.PKFreeMaps.Contains(killer.MapID))
                        goto Over;
                    if (Constants.Damage1Map.Contains(killer.MapID))
                        goto Over;
                    if (killer.Owner.Map.BaseID == 700)
                        goto Over;
                    if (((killer.PKMode != Conquer_Online_Server.Game.Enums.PKMode.Jiang) && (killer.PKMode != Conquer_Online_Server.Game.Enums.PKMode.Guild) && (killer.PKMode != Conquer_Online_Server.Game.Enums.PKMode.Bahaa) && !this.ContainsFlag(1L)) && !this.ContainsFlag(0x8000L))  
                    {
                        killer.AddFlag(Network.GamePackets.Update.Flags.FlashingName);
                        killer.FlashingNameStamp = Time32.Now;
                        killer.FlashingNameTime = 60;
                        if (killer.GuildID != 0)
                        {
                            if (killer.Owner.Guild.Enemy.ContainsKey(GuildID))
                            {
                                killer.PKPoints += 3;
                            }
                            else
                            {
                                if (!killer.Owner.Enemy.ContainsKey(UID))
                                    killer.PKPoints += 10;
                                else
                                    killer.PKPoints += 5;
                            }
                        }
                        else
                        {
                            if (!killer.Owner.Enemy.ContainsKey(UID))
                                killer.PKPoints += 10;
                            else
                                killer.PKPoints += 5;
                        }
                        Network.PacketHandler.AddEnemy(this.Owner, killer.Owner);
                    }
                    if (killer.PKMode != Conquer_Online_Server.Game.Enums.PKMode.Jiang)
                    {
                        if (killer.EntityFlag == Conquer_Online_Server.Game.EntityFlag.Player)
                        {
                            this.DropRandomStuff(this.Killer);
                        }
                        else
                        {
                            this.DropRandomStuff(this.Killer);
                        }
                    }
                } // tq
            }
            RemoveFlag(Network.GamePackets.Update.Flags.FlashingName);
        Over:

            Network.GamePackets.Attack attack = new Attack(true);
            attack.Attacker = killer.UID;
            attack.Attacked = UID;
            attack.AttackType = Network.GamePackets.Attack.Kill;
            attack.X = X;
            attack.Y = Y;

            if (EntityFlag == EntityFlag.Player)
            {
                AddFlag(Network.GamePackets.Update.Flags.Dead);
                RemoveFlag(Network.GamePackets.Update.Flags.Fly);
                RemoveFlag(Network.GamePackets.Update.Flags.Ride);
                RemoveFlag(Network.GamePackets.Update.Flags.Cyclone);
                RemoveFlag(Network.GamePackets.Update.Flags.Superman);
                RemoveFlag(Network.GamePackets.Update.Flags.FatalStrike);
                RemoveFlag(Network.GamePackets.Update.Flags.FlashingName);
                RemoveFlag(Network.GamePackets.Update.Flags.ShurikenVortex);
                RemoveFlag2(Network.GamePackets.Update.Flags2.Oblivion);

                //  if (Body % 10 < 3)  
                //    TransformationID = 99;  
                //else  
                //  TransformationID = 98;  

                Owner.SendScreen(attack, true);
                Owner.Send(new MapStatus() { BaseID = Owner.Map.BaseID, ID = Owner.Map.ID, Status = Database.MapsTable.MapInformations[Owner.Map.ID].Status, Weather = Database.MapsTable.MapInformations[Owner.Map.ID].Weather });

                Owner.EndQualifier();
            }
            else
            {

                if (!Companion && !IsDropped)
                    MonsterInfo.Drop(killer);
                /*  if (!BodyGuard && !IsDropped)  
                      MonsterInfo.Drop(killer);*/
                Kernel.Maps[MapID].Floor[X, Y, MapObjType, this] = true;
                if (killer.EntityFlag == EntityFlag.Player)
                {
                    Killer.Owner.SpiritBeadQ.GainSpirits(MonsterInfo.Level);
                    killer.Owner.IncreaseExperience(MaxHitpoints, true);
                    if (killer.Owner.Team != null)
                    {
                        foreach (Client.GameClient teammate in killer.Owner.Team.Teammates)
                        {
                            if (Kernel.GetDistance(killer.X, killer.Y, teammate.Entity.X, teammate.Entity.Y) <= Constants.pScreenDistance)
                            {
                                if (killer.UID != teammate.Entity.UID)
                                {
                                    uint extraExperience = MaxHitpoints / 2;
                                    if (killer.Spouse == teammate.Entity.Name)
                                        extraExperience = MaxHitpoints * 2;
                                    byte TLevelN = teammate.Entity.Level;
                                    if (killer.Owner.Team.CanGetNoobExperience(teammate))
                                    {
                                        if (teammate.Entity.Level < 137)
                                        {
                                            extraExperience *= 2;
                                            teammate.IncreaseExperience(extraExperience, false);
                                            teammate.Send(Constants.NoobTeamExperience(extraExperience));
                                        }
                                    }
                                    else
                                    {
                                        if (teammate.Entity.Level < 137)
                                        {
                                            teammate.IncreaseExperience(extraExperience, false);
                                            teammate.Send(Constants.TeamExperience(extraExperience));
                                        }
                                    }
                                    byte TLevelNn = teammate.Entity.Level;
                                    byte newLevel = (byte)(TLevelNn - TLevelN);
                                    if (newLevel != 0)
                                    {
                                        if (TLevelN < 70)
                                        {
                                            for (int i = TLevelN; i < TLevelNn; i++)
                                            {
                                                teammate.Team.Teammates[0].VirtuePoints += (uint)(i * 3.83F);
                                                teammate.Team.SendMessage(new Message("The leader, " + teammate.Team.Teammates[0].Entity.Name + ", has gained " + (uint)(i * 7.7F) + " virtue points for power leveling the rookies.", System.Drawing.Color.Red, Message.Team));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (killer.Level < 137)
                    {
                        uint extraExp = MaxHitpoints;
                        extraExp *= Constants.ExtraExperienceRate;
                        extraExp += extraExp * killer.Gems[3] / 100;
                        extraExp += (uint)(extraExp * ((float)killer.BattlePower / 100));
                        if (killer.DoubleExperienceTime > 0)
                            extraExp *= 2;
                        if (killer.HeavenBlessing > 0)
                            extraExp += (uint)(extraExp * 20 / 100);
                        if (killer.Reborn >= 2)
                            extraExp /= 3;
                        killer.Owner.Send(Constants.ExtraExperience(extraExp));
                    }
                    killer.Owner.XPCount++;
                    if (killer.OnKOSpell())
                        killer.KOSpellTime++;
                }
            }
            if (EntityFlag == EntityFlag.Player)
                if (OnDeath != null) OnDeath(this);
        }
        public void RemoveMagicDefender()
        {
            if (MagicDefenderOwner && HasMagicDefender)
            {
                if (Owner.Team != null && HasMagicDefender && MagicDefenderOwner)
                {
                    foreach (var mate in Owner.Team.Teammates)
                    {
                        mate.Entity.HasMagicDefender = false;
                        mate.Entity.MagicDefenderSecs = 0;
                        mate.Entity.RemoveFlag3(Conquer_Online_Server.Network.GamePackets.Update.Flags3.MagicDefender);
                        mate.Entity.Update(mate.Entity.StatusFlag, mate.Entity.StatusFlag2, mate.Entity.StatusFlag3, Conquer_Online_Server.Network.GamePackets.Update.MagicDefenderIcone, 0x80, 0, 0, false);
                    }
                }
                MagicDefenderOwner = false;
            }
            RemoveFlag3(Conquer_Online_Server.Network.GamePackets.Update.Flags3.MagicDefender);
            Update(StatusFlag, StatusFlag2, StatusFlag3, Conquer_Online_Server.Network.GamePackets.Update.MagicDefenderIcone, 0x80, 0, 0, false);
            HasMagicDefender = false;
        }
        public void Update(ulong val1, ulong val2, uint val3, uint val4, uint val5, uint val6, uint val7, bool screen)
        {
            if (!SendUpdates)
                return;
            if (this.Owner == null)
                return;
            update = new Update(true);
            update.UID = UID;
            update.Append(val1, val2, val3, val4, val5, val6, val7);

            if (!screen)
                update.Send(Owner);
            else
                Owner.SendScreen(update, true);
        }
        public void Update(byte type, uint value, uint secondvalue)
        {
            Network.GamePackets.Update upd = new Update(true);
            upd.Append(type, value);
            upd.Append(type, secondvalue);
            upd.UID = UID;
            Owner.Send(upd);
        }
        public void Update(byte type, byte value, bool screen)
        {
            if (!SendUpdates)
                return;
            if (this.Owner == null)
                return;
            update = new Update(true);
            update.UID = UID;
            update.Append(type, value, (byte)UpdateOffset1, (byte)UpdateOffset2, (byte)UpdateOffset3, (byte)UpdateOffset4, (byte)UpdateOffset5, (byte)UpdateOffset6, (byte)UpdateOffset7);
            if (!screen)
                update.Send(Owner);
            else
                Owner.SendScreen(update, true);
        }
        public void Update(byte type, ushort value, bool screen)
        {
            if (!SendUpdates)
                return;
            update = new Update(true);
            update.UID = UID;
            update.Append(type, value);
            if (!screen)
                update.Send(Owner as Client.GameClient);
            else
                (Owner as Client.GameClient).SendScreen(update, true);
        }
        public void Update(byte type, uint value, bool screen)
        {
            if (!SendUpdates)
                return;
            update = new Update(true);
            update.UID = UID;
            update.Append(type, value);
            if (!screen)
                update.Send(Owner as Client.GameClient);
            else
                (Owner as Client.GameClient).SendScreen(update, true);
        }
        public void Update(byte type, ulong value, bool screen)
        {
            if (!SendUpdates)
                return;
            update = new Update(true);
            update.UID = UID;
            update.Append(type, value);
            if (EntityFlag == EntityFlag.Player)
            {
                if (!screen)
                    update.Send(Owner as Client.GameClient);
                else
                    (Owner as Client.GameClient).SendScreen(update, true);
            }
            else
            {
                MonsterInfo.SendScreen(update);
            }
        }
        public void Update(byte type, ulong value, ulong value2, bool screen)
        {
            if (!SendUpdates)
                return;
            update = new Update(true);
            update.UID = UID;
            update.PoPAppend(type, value, value2);
            if (!screen)
                update.Send(Owner as Client.GameClient);
            else
                (Owner as Client.GameClient).SendScreen(update, true);
        }
        public void UpdateEffects(bool screen)
        {
            if (!SendUpdates)
                return;
            update = new Update(true);
            update.UID = UID;
            update.AppendFull(0x19, StatusFlag, StatusFlag2, StatusFlag3);
            if (EntityFlag == EntityFlag.Player)
            {
                if (!screen)
                    update.Send(Owner as Client.GameClient);
                else
                    (Owner as Client.GameClient).SendScreen(update, true);
            }
            else
            {
                MonsterInfo.SendScreen(update);
            }
        }
        public void Update2(byte type, ulong value, bool screen)
        {
            if (!SendUpdates)
                return;
            update = new Update(true);
            update.UID = UID;
            update.Append2(type, value);
            if (EntityFlag == EntityFlag.Player)
            {
                if (!screen)
                    update.Send(Owner as Client.GameClient);
                else
                    (Owner as Client.GameClient).SendScreen(update, true);
            }
            else
            {
                MonsterInfo.SendScreen(update);
            }
        }
        public void Update(byte type, string value, bool screen)
        {
            if (!SendUpdates)
                return;
            Network.GamePackets._String update = new _String(true);
            update.UID = this.UID;
            update.Type = type;
            update.TextsCount = 1;
            update.Texts.Add(value);
            if (EntityFlag == EntityFlag.Player)
            {
                if (!screen)
                    update.Send(Owner as Client.GameClient);
                else
                    (Owner as Client.GameClient).SendScreen(update, true);
            }
            else
            {
                MonsterInfo.SendScreen(update);
            }
        }
        private void UpdateDatabase(string column, byte value)
        {
            new Database.MySqlCommand(Conquer_Online_Server.Database.MySqlCommandType.UPDATE).Update("entities").Set(column, value).Where("UID", UID).Execute();
        }
        private void UpdateDatabase(string column, long value)
        {
            new Database.MySqlCommand(Conquer_Online_Server.Database.MySqlCommandType.UPDATE).Update("entities").Set(column, value).Where("UID", UID).Execute();
        }
        private void UpdateDatabase(string column, ulong value)
        {
            new Database.MySqlCommand(Conquer_Online_Server.Database.MySqlCommandType.UPDATE).Update("entities").Set(column, value).Where("UID", UID).Execute();
        }
        //public void UpdateDatabas(string column, byte value)
        //{
        //    if (EntityFlag == Game.EntityFlag.Player)
        //        if (FullyLoaded)
        //            new Database.MySqlCommand(Conquer_Online_Server.Database.MySqlCommandType.UPDATE).Update("entities").Set(column, value).Where("UID", UID).Execute();
        //    Database.KillConnections.Kill();
        //}  
        private void UpdateDatabase(string column, bool value)
        {
            new Database.MySqlCommand(Conquer_Online_Server.Database.MySqlCommandType.UPDATE).Update("entities").Set(column, value).Where("UID", UID).Execute();
        }
        /*private void UpdateDatabase(string column, string value)
        {
            new Database.MySqlCommand(Conquer_Online_Server.Database.MySqlCommandType.UPDATE).Update("entities").Set(column, value).Where("UID", UID).Execute();
        }*/

        public static sbyte[] XDir = new sbyte[] { 0, -1, -1, -1, 0, 1, 1, 1 };
        public static sbyte[] YDir = new sbyte[] { 1, 1, 0, -1, -1, -1, 0, 1 };
        public static sbyte[] XDir2 = new sbyte[] { 0, -2, -2, -2, 0, 2, 2, 2, -1, -2, -2, -1, 1, 2, 2, 1, -1, -2, -2, -1, 1, 2, 2, 1 };
        public static sbyte[] YDir2 = new sbyte[] { 2, 2, 0, -2, -2, -2, 0, 2, 2, 1, -1, -2, -2, -1, 1, 2, 2, 1, -1, -2, -2, -1, 1, 2 };

        public bool Move(Enums.ConquerAngle Direction, int teleport = 1)
        {
            ushort _X = X, _Y = Y;
            Facing = Direction;
            int dir = ((int)Direction) % XDir.Length;
            sbyte xi = XDir[dir], yi = YDir[dir];
            _X = (ushort)(X + xi);
            _Y = (ushort)(Y + yi);
            Game.Map Map = null;
            if (Kernel.Maps.TryGetValue(MapID, out Map))
            {
                var objType = MapObjType;
                if (Map.Floor[_X, _Y, objType])
                {
                    if (objType == MapObjectType.Monster)
                    {
                        Map.Floor[_X, _Y, MapObjType] = false;
                        Map.Floor[X, Y, MapObjType] = true;
                    }
                    X = _X;
                    Y = _Y;
                    return true;
                }
                else
                {
                    if (Mode == Enums.Mode.None)
                        if (EntityFlag != EntityFlag.Monster)
                            if (teleport == 1)
                                Teleport(MapID, X, Y);
                    return false;
                }
            }
            else
            {
                if (EntityFlag != EntityFlag.Monster)
                    Teleport(MapID, X, Y);
                else
                    return false;
            }
            return true;
        }
        public bool Move(Enums.ConquerAngle Direction, bool slide)
        {
            ushort _X = X, _Y = Y;
            if (!slide)
                return Move((Enums.ConquerAngle)((byte)Direction % 8));

            int dir = ((int)Direction) % XDir2.Length;
            Facing = Direction;
            sbyte xi = XDir2[dir], yi = YDir2[dir];
            _X = (ushort)(X + xi);
            _Y = (ushort)(Y + yi);

            Game.Map Map = null;

            if (Kernel.Maps.TryGetValue(MapID, out Map))
            {
                if (Map.Floor[_X, _Y, MapObjType])
                {
                    if (MapObjType == MapObjectType.Monster)
                    {
                        Map.Floor[_X, _Y, MapObjType] = false;
                        Map.Floor[X, Y, MapObjType] = true;
                    }
                    X = _X;
                    Y = _Y;
                    return true;
                }
                else
                {
                    if (Mode == Enums.Mode.None)
                    {
                        if (EntityFlag != EntityFlag.Monster)
                            Teleport(MapID, X, Y);
                        else
                            return false;
                    }
                }
            }
            else
            {
                if (EntityFlag != EntityFlag.Monster)
                    Teleport(MapID, X, Y);
                else
                    return false;
            }
            return true;
        }
        public void SendSpawn(Client.GameClient client)
        {
            SendSpawn(client, true);
        }
        public void SendSpawn(Client.GameClient client, bool checkScreen = true)
        {
            if (!this.Invisable)
            {
                if (client.Screen.Add(this) || !checkScreen)
                {
                    if (EntityFlag == Game.EntityFlag.Player)
                    {
                        if (client.InQualifier() && Owner.IsWatching())
                            return;
                        if (Owner.WatchingElitePKMatch != null)
                            return;
                        if (Invisable) return;
                        client.Send(SpawnPacket);
                        if (this.Owner.IsFairy)
                        {
                            FairySpawn FS = new FairySpawn(true);
                            FS.SType = this.Owner.SType;
                            FS.FairyType = this.Owner.FairyType;
                            FS.UID = this.UID;
                            client.Send(FS);
                        }
                    }
                    else
                        client.Send(SpawnPacket);
                    if (EntityFlag == EntityFlag.Player)
                    {
                        if (Owner.Booth != null)
                        {
                            client.Send(Owner.Booth);
                            if (Owner.Booth.HawkMessage != null)
                                client.Send(Owner.Booth.HawkMessage);
                        }
                    }
                }
            }
        }
        public void SendSpawnx(GameClient client, bool checkScreen = true)
        {
            if (client.Screen.Add(this) || !checkScreen)
            {
                client.Send(this.SpawnPacket);
                if (EntityFlag == EntityFlag.Player)
                {
                    if (Owner.WatchingElitePKMatch != null)
                        return;
                    if (this.Owner.IsFairy)
                    {
                        FairySpawn FS = new FairySpawn(true);
                        FS.SType = this.Owner.SType;
                        FS.FairyType = this.Owner.FairyType;
                        FS.UID = this.UID;
                        client.Send(FS);
                    }
                }
                if ((this.EntityFlag == Conquer_Online_Server.Game.EntityFlag.Player) && (this.Owner.Booth != null))
                {
                    client.Send((byte[])this.Owner.Booth);
                    if (this.Owner.Booth.HawkMessage != null)
                    {
                        client.Send(this.Owner.Booth.HawkMessage);
                    }
                }
            }
        }

        public void AddFlag(ulong flag)
        {
            //if (!ContainsFlag(Network.GamePackets.Update.Flags.Dead) && !ContainsFlag(Network.GamePackets.Update.Flags.Ghost))
            StatusFlag |= flag;
        }

        public bool ContainsFlag(ulong flag)
        {
            ulong aux = StatusFlag;
            aux &= ~flag;
            return !(aux == StatusFlag);
        }
        public void RemoveFlag(ulong flag)
        {
            if (ContainsFlag(flag))
            {
                StatusFlag &= ~flag;
            }
        }
        public void AddFlag2(ulong flag)
        {
            if (flag == Network.GamePackets.Update.Flags2.SoulShackle) { StatusFlag2 |= flag; return; }
            if (!ContainsFlag(Network.GamePackets.Update.Flags.Dead) && !ContainsFlag(Network.GamePackets.Update.Flags.Ghost))
                StatusFlag2 |= flag;
        }
        public bool ContainsFlag2(ulong flag)
        {
            ulong aux = StatusFlag2;
            aux &= ~flag;
            return !(aux == StatusFlag2);
        }
        public void RemoveFlag2(ulong flag)
        {
            if (ContainsFlag2(flag))
            {
                StatusFlag2 &= ~flag;
            }
        }
        public void AddFlag3(uint flag)
        {
            StatusFlag3 |= flag;
        }
        public bool ContainsFlag3(uint flag)
        {
            uint aux = StatusFlag3;
            aux &= ~flag;
            return !(aux == StatusFlag3);
        }
        public void RemoveFlag3(uint flag)
        {
            if (ContainsFlag3(flag))
            {
                StatusFlag3 &= ~flag;
            }
        }
        public void Shift(ushort X, ushort Y, uint mapID, Interfaces.IPacket shift = null)
        {
            if (_mapid != mapID) return;

            if (EntityFlag == EntityFlag.Player)
            {
                if (!Database.MapsTable.MapInformations.ContainsKey(MapID))
                    return;
            }
            this.X = X;
            this.Y = Y;
            if (shift == null)
            {
                shift = new Network.GamePackets.Data(true)
                {
                    UID = UID,
                    ID = Network.GamePackets.Data.FlashStep,
                    dwParam = MapID,
                    wParam1 = X,
                    wParam2 = Y
                };
            }
            if (EntityFlag == EntityFlag.Player)
            {
                Owner.SendScreen(shift, true);
                Owner.Screen.Reload(shift);
            }
        }
        public void Shift(ushort X, ushort Y)
        {
            if (EntityFlag == EntityFlag.Player)
            {
                if (!Database.MapsTable.MapInformations.ContainsKey(MapID))
                    return;
                this.X = X;
                this.Y = Y;
                Network.GamePackets.Data Data = new Network.GamePackets.Data(true);
                Data.UID = UID;
                Data.ID = Network.GamePackets.Data.FlashStep;
                Data.dwParam = MapID;
                Data.wParam1 = X;
                Data.wParam2 = Y;
                Owner.SendScreen(Data, true);
                Owner.Screen.Reload(null);
            }
        }
        public AppearanceType Appearance
        {
            get { return (AppearanceType)BitConverter.ToUInt16(SpawnPacket, _AppearanceType); }
            set { WriteUInt16((ushort)value, _AppearanceType, SpawnPacket); }
        }
        public bool fMove(Enums.ConquerAngle Direction, ref ushort _X, ref ushort _Y)
        {
            Facing = Direction;
            sbyte xi = 0, yi = 0;
            switch (Direction)
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
            _X = (ushort)(_X + xi);
            _Y = (ushort)(_Y + yi);
            if (EntityFlag == Game.EntityFlag.Player)
            {
                if (Owner.Map.Floor[_X, _Y, MapObjType, null])
                    return true;
                else
                    return false;
            }
            else
            {
                Game.Map Map = null;
                if (Kernel.Maps.TryGetValue(MapID, out Map))
                {
                    if (Map.Floor[_X, _Y, MapObjType, null])
                        return true;
                    else
                        return false;
                }
                return true;
            }
        }
        public void Teleport(ushort X, ushort Y)
        {
            if (EntityFlag == EntityFlag.Player)
            {
                if (!Database.MapsTable.MapInformations.ContainsKey(MapID) && !Owner.InQualifier())
                {
                    MapID = 1002;
                    X = 300;
                    Y = 280;
                }
                this.X = X;
                this.Y = Y;
                Network.GamePackets.Data Data = new Network.GamePackets.Data(true);
                Data.UID = UID;
                Data.ID = Network.GamePackets.Data.Teleport;
                Data.dwParam = Database.MapsTable.MapInformations[MapID].BaseID;
                Data.wParam1 = X;
                Data.wParam2 = Y;
                Owner.Send(Data);
                Owner.Screen.FullWipe();
                Owner.Screen.Reload(null);
                Owner.Send(new MapStatus() { BaseID = Owner.Map.BaseID, ID = Owner.Map.ID, Status = Database.MapsTable.MapInformations[Owner.Map.ID].Status, Weather = Database.MapsTable.MapInformations[Owner.Map.ID].Weather });
            }
        }
        public void SetLocation(ushort MapID, ushort X, ushort Y)
        {
            if (EntityFlag == EntityFlag.Player)
            {
                this.X = X;
                this.Y = Y;
                this.MapID = MapID;
            }
        }
        public void TeleportHouse(ushort MapID, ushort X, ushort Y)
        {
            if (EntityFlag == EntityFlag.Player)
            {
                if (!Database.MapsTable.MapInformations.ContainsKey(MapID) && Owner.QualifierGroup == null)
                {
                    MapID = 1002;
                    X = 300;
                    Y = 280;
                }
                if (EntityFlag == EntityFlag.Player)
                {
                    if (Owner.InQualifier())
                        if (MapID != 700 && MapID < 11000)
                            Owner.EndQualifier();
                    if (Owner.Companion != null)
                    {
                        Owner.Map.RemoveEntity(Owner.Companion);
                        Data data = new Data(true);
                        data.UID = Owner.Companion.UID;
                        data.ID = Network.GamePackets.Data.RemoveEntity;
                        Owner.Companion.MonsterInfo.SendScreen(data);
                        Owner.Companion = null;
                    }
                }
                if (Owner.Entity.MyClones.Count > 0)
                {
                    foreach (var item in Owner.Entity.MyClones.Values)
                    {
                        Owner.Map.RemoveEntity(item);
                        Data data = new Data(true);
                        data.UID = item.UID;
                        data.ID = Network.GamePackets.Data.RemoveEntity;
                        item.MonsterInfo.SendScreen(data);
                    }
                    Owner.Entity.MyClones.Clear();

                }  
                if ((this.EntityFlag == Conquer_Online_Server.Game.EntityFlag.Player) && (this.Owner.BodyGuard != null))
                {
                    this.Owner.Map.RemoveEntity(this.Owner.BodyGuard);
                    Data buffer = new Data(true)
                    {
                        UID = this.Owner.BodyGuard.UID,
                        ID = 0x87
                    };
                    this.Owner.BodyGuard.MonsterInfo.SendScreen(buffer);
                    this.Owner.BodyGuard = null;
                }
                if (MapID == this.MapID)
                {
                    Teleport(X, Y);
                    return;
                }
                this.X = X;
                this.Y = Y;
                PX = 0;
                PY = 0;
                this.PreviousMapID = this.MapID;
                this.MapID = MapID;
                Network.GamePackets.Data Data = new Network.GamePackets.Data(true);
                Data.UID = UID;
                Data.ID = Network.GamePackets.Data.Teleport;
                Data.dwParam = Database.MapsTable.MapInformations[MapID].BaseID;
                Data.wParam1 = X;
                Data.wParam2 = Y;
                Owner.Send(Data);

                Owner.Screen.FullWipe();
                Owner.Screen.Reload(null);
                Owner.Send(new MapStatus() { BaseID = Owner.Map.BaseID, ID = Owner.Map.ID, Status = Database.MapsTable.MapInformations[Owner.Map.ID].Status, Weather = Database.MapsTable.MapInformations[Owner.Map.ID].Weather });
                if (!Owner.Equipment.Free(12))
                    if (Owner.Map.ID == 1036 && Owner.Equipment.TryGetItem((byte)12).Plus < 6)
                        RemoveFlag(Network.GamePackets.Update.Flags.Ride);
            }
        }
        public void Teleport(ushort MapID, ushort X, ushort Y)
        {
            if (EntityFlag == EntityFlag.Player)
            {
                ushort baseID = 0;
                if (!Kernel.Maps.ContainsKey(MapID))
                {
                    if (!Database.MapsTable.MapInformations.ContainsKey(MapID) && Owner.QualifierGroup == null)
                    {
                        baseID = MapID = 1002;
                        X = 300;
                        Y = 280;
                    }
                    else
                    {
                        baseID = Database.MapsTable.MapInformations[MapID].BaseID;
                    }
                }
                else
                {
                    baseID = Kernel.Maps[MapID].BaseID;
                }

                if (EntityFlag == EntityFlag.Player)
                {
                    if (Owner.InQualifier())
                        if (MapID != 700 && MapID < 11000)
                            Owner.EndQualifier();
                    if (Owner.Companion != null)
                    {
                        Owner.Map.RemoveEntity(Owner.Companion);
                        Data data = new Data(true);
                        data.UID = Owner.Companion.UID;
                        data.ID = Network.GamePackets.Data.RemoveEntity;
                        Owner.Companion.MonsterInfo.SendScreen(data);
                        Owner.Companion = null;
                    }
                }
                if (MapID == this.MapID)
                {
                    Teleport(X, Y);
                    return;
                }
                this.X = X;
                this.Y = Y;
                this.PreviousMapID = this.MapID;
                this.MapID = MapID;
                Network.GamePackets.Data Data = new Network.GamePackets.Data(true);
                Data.UID = UID;
                Data.ID = Network.GamePackets.Data.Teleport;
                Data.dwParam = baseID;
                Data.wParam1 = X;
                Data.wParam2 = Y;
                Owner.Send(Data);
                Owner.Send(new MapStatus() { BaseID = Owner.Map.BaseID, ID = Owner.Map.ID, Status = Database.MapsTable.MapInformations[Owner.Map.ID].Status, Weather = Database.MapsTable.MapInformations[Owner.Map.ID].Weather });
                Owner.Entity.Action = Conquer_Online_Server.Game.Enums.ConquerAction.None;
                Owner.ReviveStamp = Time32.Now;
                Owner.Attackable = false;
                if (!Owner.Equipment.Free(12))
                    if (Owner.Map.ID == 1036 && Owner.Equipment.TryGetItem((byte)12).Plus < 6)
                        RemoveFlag(Network.GamePackets.Update.Flags.Ride);
                Owner.Screen.Reload(null);
            }
            Owner.Screen.Reload(null);  
        }
        public ushort PrevX, PrevY;
        public void Teleport(ushort BaseID, ushort DynamicID, ushort X, ushort Y)
        {
            if (EntityFlag == EntityFlag.Player)
            {
                if (!Database.DMaps.MapPaths.ContainsKey(BaseID))
                    return;

                if (Owner.InQualifier())
                    if (BaseID != 700 && BaseID < 11000)
                        Owner.EndQualifier();
                if (!Kernel.Maps.ContainsKey(DynamicID)) new Map(DynamicID, BaseID, Database.DMaps.MapPaths[BaseID]);
                this.PrevX = this.X;
                this.PrevY = this.Y;
                this.X = X;
                this.Y = Y;
                this.PreviousMapID = this.MapID;
                this.MapID = DynamicID;
                Network.GamePackets.Data Data = new Network.GamePackets.Data(true);
                Data.UID = UID;
                Data.ID = Network.GamePackets.Data.Teleport;
                Data.dwParam = BaseID;
                Data.wParam1 = X;
                Data.wParam2 = Y;
                Owner.Send(Data);
                Owner.Entity.Action = Conquer_Online_Server.Game.Enums.ConquerAction.None;
                Owner.ReviveStamp = Time32.Now;
                Owner.Attackable = false;
                if (Owner.ChampionGroup == null)
                    Owner.Send(new MapStatus() { BaseID = Owner.Map.BaseID, ID = Owner.Map.ID, Status = Database.MapsTable.MapInformations[Owner.Map.BaseID].Status, Weather = Database.MapsTable.MapInformations[Owner.Map.BaseID].Weather });
                if (!Owner.Equipment.Free(12))
                    if (Owner.Map.ID == 1036 && Owner.Equipment.TryGetItem((byte)12).Plus < 6)
                        RemoveFlag(Network.GamePackets.Update.Flags.Ride);
            }
        }

        public bool OnKOSpell()
        {
            return OnCyclone() || OnSuperman() || OnSuperCyclone();
        }
        public bool OnOblivion()
        {
            return ContainsFlag2(Network.GamePackets.Update.Flags2.Oblivion);
        }
        public bool OnCyclone()
        {
            return ContainsFlag(Network.GamePackets.Update.Flags.Cyclone);
        }
        public bool OnSuperCyclone()
        {
            return ContainsFlag3(Network.GamePackets.Update.Flags3.SuperCyclone);
        }
        public bool OnSuperman()
        {
            return ContainsFlag(Network.GamePackets.Update.Flags.Superman);
        }
        public bool OnFatalStrike()
        {
            return ContainsFlag(Network.GamePackets.Update.Flags.FatalStrike);
        }

        public String ToHex(byte[] buf)
        {
            var builder = new StringBuilder();
            foreach (var b in buf)
                builder.Append(b.ToString("X2") + " ");
            return builder.ToString();
        }
        public void Untransform()
        {
            if (MapID == 1036 && TransformationTime == 3600)
                return;
            this.TransformationID = 0;

            double maxHP = TransformationMaxHP;
            double HP = Hitpoints;
            double point = HP / maxHP;

            Hitpoints = (uint)(MaxHitpoints * point);
            Update(Network.GamePackets.Update.MaxHitpoints, MaxHitpoints, false);
        }
        public byte[] WindowSpawn()
        {
            byte[] buffer = new byte[SpawnPacket.Length];
            SpawnPacket.CopyTo(buffer, 0);
            buffer[_WindowSpawn] = 1;
            return buffer;
        }
        #endregion




        public bool JiangActive
        {
            get
            {
                return (this.SpawnPacket[_JiangHuActive] == 1);
            }
            set
            {
                Conquer_Online_Server.Network.Writer.WriteByte(value ? ((byte)1) : ((byte)0), (ushort)_JiangHuActive, this.SpawnPacket);
            }
        }
        public byte JiangTalent
        {
            get
            {
                return this.SpawnPacket[_JingHu_Talen];
            }
            set
            {
                Conquer_Online_Server.Network.Writer.WriteByte(value, (ushort)_JingHu_Talen, this.SpawnPacket);
            }
        }
        public void SetVisible()
        {
            SpawnPacket[_WindowSpawn] = 0;
        }
        public ushort CountryID
        {

            get
            {
                return BitConverter.ToUInt16(SpawnPacket, _CountryCode);
            }
            set
            {
                WriteUInt16((ushort)value, _CountryCode, SpawnPacket);
            }
        }
        public uint Xmas
        {
            get
            {
                return _Xmas;
            }
            set
            {
                _Xmas = value;
            }
        }
        public void SendSysMesage(string mesaj)
        {
            byte[] buffer = new Conquer_Online_Server.Network.GamePackets.Message(mesaj, System.Drawing.Color.Yellow, 0x7dc).ToArray();
            this.Owner.Send(buffer);
        }
        private uint guildBP = 0;
        public bool Assassin()
        {
            if (EntityFlag == Game.EntityFlag.Player)
            {
                var weapons = Owner.Weapons;
                if (weapons.Item1 != null)
                    if (weapons.Item1.ID / 1000 == 613)
                        return true;
                    else if (weapons.Item2 != null)
                        if (weapons.Item2.ID / 1000 == 613)
                            return true;
            }
            return false;
        }
        private Time32 spiritFocusStamp;
        private bool spiritFocus;
        public bool SpiritFocus
        {
            get
            {
                if (!spiritFocus) return false;
                if (Time32.Now > spiritFocusStamp.AddSeconds(5))
                    return true;
                return false;
            }
            set { spiritFocus = value; if (value) spiritFocusStamp = Time32.Now; }
        }
        public int SuperItemBless;
        public float SpiritFocusPercent;
        public Time32 EagleEyeStamp;
        public Time32 Cursed;
        public Time32 MortalWoundStamp;
        public bool Aura_isActive;
        public ulong Aura_actType;
        public uint Aura_actPower;
        public Time32 SpellStamp;
        public Time32 LastExpSave;
        public bool JustCreated = false;
        public Time32 ChainboltStamp;
        public int ChainboltTime;
        public Time32 FrozenStamp;
        public int FrozenTime;
        public object FlusterTime;
        public bool FrozenD;
        public AppearanceType AppearanceBkp;
        public Enums.PKMode PrevPKMode;
        public Time32 FlagStamp;
        private uint assassinBP;
        public bool Auto = false;
        private ulong _autohuntxp;
        public ulong autohuntxp
        {
            get
            {

                return _autohuntxp;
            }
            set
            {
                _autohuntxp = value;
            }
        }
        public uint GuildBattlePower
        {
            get
            {
                return guildBP;
            }
            set
            {
                ExtraBattlePower -= guildBP;
                guildBP = value;
                Update(Network.GamePackets.Update.GuildBattlepower, guildBP, false);
                ExtraBattlePower += guildBP;
            }
        }

        internal void PreviousTeleport()
        {
            if (Constants.PKFreeMaps2.Contains(MapID))
            {
                Teleport(1002, 300, 280);
            }
            else
                Teleport(PreviousMapID, PrevX, PrevY);
            BringToLife();
        }
        public bool IsThisLeftGate(int X, int Y)
        {
            if (Game.GuildWar.RightGate == null)
                return false;
            if (MapID == 1038)
            {
                if ((X == 223 || X == 222) && (Y >= 175 && Y <= 185))
                {
                    if (Game.GuildWar.RightGate.Mesh / 10 == 27)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public bool IsThisRightGate(int X, int Y)
        {
            if (Game.GuildWar.LeftGate == null)
                return false;
            if (MapID == 1038)
            {
                if ((Y == 210 || Y == 209) && (X >= 154 && X <= 166))
                {
                    if (Game.GuildWar.LeftGate.Mesh / 10 == 24)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public bool ThroughGate(int X, int Y)
        {
            return IsThisLeftGate(X, Y) || IsThisRightGate(X, Y);
        }
        public void UpdateDatabase1(string column, string value)
        {
            new Database.MySqlCommand(Database.MySqlCommandType.UPDATE).Update("entities").Set(column, value).Where("UID", UID).Execute();
        }
        public void UpdateDatabase1(string column, uint value)
        {
            new Database.MySqlCommand(Database.MySqlCommandType.UPDATE).Update("entities").Set(column, value).Where("UID", UID).Execute();
        }
        private void UpdateDatabase(string column, string value)
        {
            new Database.MySqlCommand(Database.MySqlCommandType.UPDATE).Update("entities").Set(column, value).Where("UID", UID).Execute();
        }
        public uint TrojanColor
        {
            get
            {
                return BitConverter.ToUInt32(this.SpawnPacket, 237);
            }
            set
            {
                WriteUInt32(value, 237, this.SpawnPacket);
            }
        }
        public uint AssassinBP
        {
            get { return assassinBP; }
            set
            {
                assassinBP = value;
                Writer.WriteUInt32(value, _AssassinAsBattlePower, SpawnPacket);
            }
        }

        public string NaIme { get; set; }

        public object PlayerFlag { get; set; }

        public static int _CUID { get; set; }

        public byte DragonFlowVAlue { get; set; }

        public Time32 ISonDragonStamp { get; set; }

        public int DragonFuryStamp { get; set; }
    }
}