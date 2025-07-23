using System;
using System.Collections.Generic;
using Conquer_Online_Server.Network.GamePackets;
using Conquer_Online_Server.Network;
using System.IO;
using System.Text;
using System.Linq;
using Conquer_Online_Server.Game.Features;
using Conquer_Online_Server.Database;

namespace Conquer_Online_Server.Game.ConquerStructures.Society
{
    public enum ArsenalType
    {
        Headgear,
        Armor,
        Weapon,
        Ring,
        Boots,
        Necklace,
        Fan,
        Tower
    }
    public enum GuildRank
    {
        Agent = 590,
        Aide = 0x25a,
        ArsenalAgent = 0x254,
        ArsFollower = 0x1f0,
        ASupervisor = 0x358,
        CPAgent = 0x255,
        CPFollower = 0x1f1,
        CPSupervisor = 0x359,
        DeputyLeader = 990,
        DeputySteward = 650,
        DLeaderAide = 0x263,
        DLeaderSpouse = 620,
        Follower = 490,
        GSupervisor = 0x356,
        GuideAgent = 0x252,
        GuideFollower = 0x1ee,
        GuildLeader = 0x3e8,
        HDeputyLeader = 980,
        HonoraryManager = 880,
        HonorarySteward = 680,
        HonorarySuperv = 840,
        LeaderSpouse = 920,
        LilyAgent = 0x24f,
        LilyFollower = 0x1eb,
        LilySupervisor = 0x353,
        LSpouseAide = 610,
        Manager = 890,
        ManagerAide = 510,
        ManagerSpouse = 520,
        Member = 200,
        None = 0,
        OrchidAgent = 0x256,
        OrchidFollower = 0x1f2,
        OSupervisor = 0x35a,
        PKAgent = 0x251,
        PKFollower = 0x1ed,
        PKSupervisor = 0x355,
        RoseAgent = 0x250,
        RoseFollower = 0x1ec,
        RoseSupervisor = 0x354,
        SeniorMember = 210,
        SilverAgent = 0x253,
        SilverFollower = 0x1ef,
        SSupervisor = 0x357,
        Steward = 690,
        StewardSpouse = 420,
        Supervisor = 850,
        SupervisorAide = 0x1ff,
        SupervSpouse = 0x209,
        TSupervisor = 0x35b,
        TulipAgent = 0x257,
        TulipFollower = 0x1f3
    }
    public class Guild : Writer
    {
        public class ClassRequirements
        {
            public const uint
            Trojan = 1,
            Warrior = 2,
            Taoist = 4,
            Archer = 8,
            Ninja = 16,
            Monk = 32,
            Pirate = 64;
        }

        //member: max=5, leader: max=15
        public static float[] SharedBattlepowerPercentage = new float[] { 0, 0, .33f, .4f, .5f, .6f, .7f, .8f, .85f, 1f, 1f };
        public uint CpsWarSign;
        public uint CpsWarWinnar;
        public uint CpsWarAmount;
        public Arsenal[] Arsenals;
        public bool ArsenalBPChanged = true;
        public int UnlockedArsenals
        {
            get
            {
                int unlocked = 0;
                for (int i = 0; i < 8; i++)
                    if (Arsenals[i].Unlocked)
                        unlocked++;
                return unlocked;
            }
        }
        public uint GetCurrentArsenalCost()
        {
            int val = UnlockedArsenals;
            if (val >= 0 && val <= 1)
                return 5000000;
            else if (val >= 2 && val <= 4)
                return 10000000;
            else if (val >= 5 && val <= 6)
                return 15000000;
            else
                return 20000000;
        }
        int arsenal_bp;
        public override int GetHashCode()
        {
            return (int)ID;
        }
        public int ArsenalTotalBattlepower
        {
            get { return arsenal_bp; }
            set
            {
                arsenal_bp = value;
                foreach (var member in Members.Values)
                {
                    if (member.IsOnline)
                    {
                        member.Client.Entity.GuildBattlePower = GetSharedBattlepower(member.Rank);
                    }
                }
            }
        }
        public int GetMaxSharedBattlepower(bool force = false)
        {
            if (ArsenalBPChanged || force)
            {
                int a_bp = 0;
                var arsenals = Arsenals.OrderByDescending(p => p.TotalSharedBattlePower);
                int a = 0;
                foreach (var arsenal in arsenals)
                {
                    if (a == 5) break;
                    a_bp += (int)(arsenal.TotalSharedBattlePower);
                    a++;
                }
                ArsenalTotalBattlepower = a_bp;
                ArsenalBPChanged = false;
            }
            return arsenal_bp;
        }
        public uint GetSharedBattlepower(int rank)
        {
            return (uint)(arsenal_bp * SharedBattlepowerPercentage[rank / 100]);
        }
        public uint GetSharedBattlepower(Enums.GuildMemberRank rank)
        {
            return GetSharedBattlepower((int)rank);
        }
        public void SaveArsenal()
        {
            Database.GuildArsenalTable.Save(this);
        }
        public static Counter GuildCounter;

        public static void GuildProfile(byte[] Packet, Client.GameClient client)
        {
            Network.GamePackets.GuildProfilePacket p = new GuildProfilePacket();
            p.Deserialize(Packet);
            p.Silver = 0;
            p.Pk = client.Entity.PKPoints;
            p.Cps = 0;
            p.Guide = 0;
            if (client.Entity.ARSDON == 0)
            {
                MySqlCommand mySqlCommand = new MySqlCommand(MySqlCommandType.SELECT);
                mySqlCommand.Select("guild_arsenalsdonation").Where("guild_uid", (long)((ulong)client.Guild.ID)).And("d_uid", (long)((ulong)client.Entity.UID));
                MySqlReader mySqlReader = new MySqlReader(mySqlCommand);
                while (mySqlReader.Read())
                {
                    ulong num = (ulong)mySqlReader.ReadUInt32("item_donation");
                    client.Entity.ARSDON += num;
                }
                p.Arsenal = (uint)client.Entity.ARSDON;
            }
            else
            {
                p.Arsenal = (uint)client.Entity.ARSDON;
            }
            p.Arsenal = 0;
            p.Rose = 0;
            p.Lily = 0;
            p.Orchid = 0;
            p.Tulip = 0;
            p.HistorySilvers = 0;
            p.HistoryCps = 0;
            p.HistoryGuide = 0;
            p.HistoryPk = 0;
            client.Send(Packet);
        }

        public class Member : Interfaces.IKnownPerson
        {
            public Member(uint GuildID)
            {
                this.GuildID = GuildID;
            }

            public uint ID
            {
                get;
                set;
            }
            public string Name
            {
                get;
                set;
            }
            public bool IsOnline
            {
                get
                {
                    return Kernel.GamePool.ContainsKey(ID);
                }
            }
            public Client.GameClient Client
            {
                get
                {
                    if (!IsOnline) return null;
                    return Kernel.GamePool[ID];
                }
            }
            public ulong SilverDonation
            {
                get;
                set;
            }
            public ulong ConquerPointDonation
            {
                get;
                set;
            }
            public uint GuildID
            {
                get;
                set;
            }
            public Guild Guild
            {
                get
                {
                    return Kernel.Guilds[GuildID];
                }
            }
            public Enums.GuildMemberRank Rank
            {
                get;
                set;
            }
            public byte Level
            {
                get;
                set;
            }
            public NobilityRank NobilityRank
            {
                get;
                set;
            }
            public byte Gender
            {
                get;
                set;
            }
            public ulong TotalDoantion
            {
                get
                {
                    ulong result;
                    if (this.IsOnline)
                    {
                        result = this.SilverDonation + this.ConquerPointDonation + this.Client.Entity.ARSDON + (ulong)this.Client.Entity.Flowers.Tulips + (ulong)this.Client.Entity.Flowers.Lilies + (ulong)this.Client.Entity.Flowers.Orchads + (ulong)this.Client.Entity.Flowers.RedRoses;
                    }
                    else
                    {
                        ulong num = 0;
                        MySqlCommand mySqlCommand = new MySqlCommand(MySqlCommandType.SELECT);
                        mySqlCommand.Select("guild_arsenalsdonation").Where("guild_uid", (long)((ulong)this.GuildID)).And("d_uid", (long)((ulong)this.ID));
                        MySqlReader mySqlReader = new MySqlReader(mySqlCommand);
                        while (mySqlReader.Read())
                        {
                            ulong num2 = (ulong)mySqlReader.ReadUInt32("item_donation");
                            num += num2;
                        }
                        ulong num3 = 0;
                        ulong num4 = 0;
                        ulong num5 = 0;
                        ulong num6 = 0;
                        MySqlCommand mySqlCommand2 = new MySqlCommand(MySqlCommandType.SELECT);
                        mySqlCommand2.Select("flowers").Where("id", (long)((ulong)this.ID));
                        MySqlReader mySqlReader2 = new MySqlReader(mySqlCommand2);
                        while (mySqlReader2.Read())
                        {
                            num3 = (ulong)mySqlReader2.ReadUInt32("redroses");
                            num4 = (ulong)mySqlReader2.ReadUInt32("lilies");
                            num5 = (ulong)mySqlReader2.ReadUInt32("orchads");
                            num6 = (ulong)mySqlReader2.ReadUInt32("tulips");
                        }
                        result = this.SilverDonation + this.ConquerPointDonation + num + num3 + num4 + num5 + num6;
                    }
                    return result;
                }
            }
        }

        private byte[] Buffer;

        public uint WarScore;

        public bool PoleKeeper
        {
            get
            {
                return GuildWar.Pole.Name == Name;
            }
        }
        public bool PoleKeeper2
        {
            get
            {
                return EliteGuildWar.Poles.Name == Name;
            }
        }
        public bool PoleKeeper3
        {
            get
            {
                return PoleTwin.Poles.Name == Name;
            }
        }
       
        public Guild(string leadername)
        {
            Buffer = new byte[92 + 8];
            LeaderName = leadername;
            Writer.WriteUInt16(92, 0, Buffer);
            Writer.WriteUInt16(1106, 2, Buffer);
            Buffer[48] = 0x2;
            Buffer[49] = 0x1;
            Buffer[75] = 0x1;
            Buffer[87] = 0x20;
            LevelRequirement = 1;
            Members = new SafeDictionary<uint, Member>(1000);
            Ally = new SafeDictionary<uint, Guild>(1000);
            Enemy = new SafeDictionary<uint, Guild>(1000);

            Arsenals = new Arsenal[8];
            for (byte i = 0; i < 8; i++)
            {
                Arsenals[i] = new Arsenal(this)
                {
                    Position = (byte)(i + 1)
                };
            }
        }
        public List<Member> ListMember = new List<Member>();
        public uint ID
        {
            get { return BitConverter.ToUInt32(Buffer, 4); }
            set { Writer.WriteUInt32(value, 4, Buffer); }
        }

        public ulong SilverFund
        {
            get { return BitConverter.ToUInt64(Buffer, 12); }
            set { Writer.WriteUInt64(value, 12, Buffer); }
        }

        public uint ConquerPointFund
        {
            get { return BitConverter.ToUInt32(Buffer, 20); }
            set { Writer.WriteUInt32(value, 20, Buffer); }
        }
        public uint HDeputyLeaderCount;
        public uint HonoraryManagerCount;
        public uint HonorarySupervCount;
        public uint HonoraryStewardCount;
        public uint AideCount;
        public uint DLeaderAideCount;
        public uint ManagerAideCount;
        public uint MemberCount
        {
            get { return BitConverter.ToUInt32(Buffer, 24); }
            set { Writer.WriteUInt32(value, 24, Buffer); }
        }
        public uint adLevelRequirement;
        public uint adRebornRequirement;
        public uint adClassRequirement;
        public uint AutoRecruit;
        public ulong ad_rate;
        public uint MaleFemale;
        public uint LevelRequirement
        {
            get { return BitConverter.ToUInt32(Buffer, 48); }
            set { Writer.WriteUInt32(value, 48, Buffer); }
        }

        public uint RebornRequirement
        {
            get { return BitConverter.ToUInt32(Buffer, 52); }
            set { Writer.WriteUInt32(value, 52, Buffer); }
        }

        public uint ClassRequirement
        {
            get { return BitConverter.ToUInt32(Buffer, 56); }
            set { Writer.WriteUInt32(value, 56, Buffer); }
        }

        public bool AllowTrojans
        {
            get
            {
                return ((ClassRequirement & ClassRequirements.Trojan) != ClassRequirements.Trojan);
            }
            set
            {
                ClassRequirement ^= ClassRequirements.Trojan;
            }
        }
        public bool AllowWarriors
        {
            get
            {
                return ((ClassRequirement & ClassRequirements.Warrior) != ClassRequirements.Warrior);
            }
            set
            {
                ClassRequirement ^= ClassRequirements.Warrior;
            }
        }
        public bool AllowTaoists
        {
            get
            {
                return ((ClassRequirement & ClassRequirements.Taoist) != ClassRequirements.Taoist);
            }
            set
            {
                ClassRequirement ^= ClassRequirements.Taoist;
            }
        }
        public bool AllowArchers
        {
            get
            {
                return ((ClassRequirement & ClassRequirements.Archer) != ClassRequirements.Archer);
            }
            set
            {
                ClassRequirement ^= ClassRequirements.Archer;
            }
        }
        public bool AllowNinjas
        {
            get
            {
                return ((ClassRequirement & ClassRequirements.Ninja) != ClassRequirements.Ninja);
            }
            set
            {
                ClassRequirement ^= ClassRequirements.Ninja;
            }
        }
        public bool AllowMonks
        {
            get
            {
                return ((ClassRequirement & ClassRequirements.Monk) != ClassRequirements.Monk);
            }
            set
            {
                ClassRequirement ^= ClassRequirements.Monk;
            }
        }
        public bool AllowPirates
        {
            get
            {
                return ((ClassRequirement & ClassRequirements.Pirate) != ClassRequirements.Pirate);
            }
            set
            {
                ClassRequirement ^= ClassRequirements.Pirate;
            }
        }


        public uint DeputyLeaderCount;
        /*public uint HDeputyLeaderCount;
        public uint HonoraryManagerCount;
        public uint HonorarySupervCount;
        public uint HonoraryStewardCount;
        public uint AideCount;*/

        private byte _Level = 1;
        public byte Level
        {
            get { return _Level; }
            set
            {
                _Level = CalculateLevel();
                Buffer[60] = _Level;
            }
        }
        public byte CalculateLevel()
        {
            byte Lvl = 1;
            for (int i = 0; i < 8; i++)
                if (Arsenals[i].Donation >= 5000000)
                    Lvl++;
            return Lvl;
        }

        public string Name;
        public string adv;
        public SafeDictionary<uint, Member> Members;
        public SafeDictionary<uint, Guild> Ally, Enemy;
        public uint Wins;
        public uint Losts;
        public uint cp_donaion = 0;
        public uint money_donation = 0;
        public uint honor_donation = 0;
        public uint pkp_donation = 0;
        public uint rose_donation = 0;
        public uint tuil_donation = 0;
        public uint orchid_donation = 0;
        public uint PIScore;
        public uint EWarScore;
        public uint lilies_donation = 0;

        public string Bulletin;

        public Member Leader;
        private string leaderName;
  
        public uint PTScore;
        public uint CTFPoints;
        public uint CTFReward = 0;
        public uint CTFFlagScore;
        private ulong adEnd;
        public DateTime adend
        {
            get { return DateTime.FromBinary((long)adEnd); }
            set { adEnd = (ulong)value.Ticks; }
        }

        public string LeaderName
        {
            get
            {
                return leaderName;
            }
            set
            {
                leaderName = value;
                Writer.WriteString(value, 32, Buffer);
            }
        }
        public static Boolean CheckNameExist(String Name)
        {
            foreach (Guild guilds in Kernel.Guilds.Values)
            {
                if (guilds.Name == Name)
                    return true;
            }
            return false;
        }
        public bool Create(string name)
        {
            if (name.Length < 16)
            {
                Name = name;
                SilverFund = 500000;
                Members.Add(Leader.ID, Leader);
                try
                {
                    Database.GuildTable.Create(this);
                }
                catch { return false; }
                Kernel.Guilds.Add(ID, this);
                Message message = null;
                message = new Message("Congratulations, " + leaderName + " has created guild " + name + " Succesfully!", System.Drawing.Color.White, Message.World);
                foreach (Client.GameClient client in Program.GamePool)
                {
                    client.Send(message);
                }
                return true;
            }
            return false;
        }

        public void AddMember(Client.GameClient client)
        {
            if (client.AsMember == null && client.Guild == null)
            {
                client.AsMember = new Member(ID)
                {
                    ID = client.Entity.UID,
                    Level = client.Entity.Level,
                    Name = client.Entity.Name,
                    Rank = Conquer_Online_Server.Game.Enums.GuildMemberRank.Member
                };
                if (Nobility.Board.ContainsKey(client.Entity.UID))
                {
                    client.AsMember.Gender = Nobility.Board[client.Entity.UID].Gender;
                    client.AsMember.NobilityRank = Nobility.Board[client.Entity.UID].Rank;
                }
                MemberCount++;
                client.Guild = this;
                client.Entity.GuildID = (ushort)client.Guild.ID;
                client.Entity.GuildRank = (ushort)client.AsMember.Rank;
                uint pos = (ushort)client.AsMember.Rank;
                client.Entity.GuildBattlePower = GetSharedBattlepower(client.AsMember.Rank);
                for (int i = 0; i < client.ArsenalDonations.Length; i++)
                    client.ArsenalDonations[i] = 0;
                Database.EntityTable.UpdateGuildID(client);
                Database.EntityTable.UpdateGuildRank(client);
                Members.Add(client.Entity.UID, client.AsMember);
                SendGuild(client);
                client.Screen.FullWipe();
                client.Screen.Reload(null);
                SendGuildMessage(new Message("GuildLeader/DeputyLeader has Agreed To Join " + client.AsMember.Name + " in the guild", System.Drawing.Color.Black, Message.Guild));
            }
        }

        public void SendMembers(Client.GameClient client, ushort page)
        {
            MemoryStream strm = new MemoryStream();
            BinaryWriter wtr = new BinaryWriter(strm);
            wtr.Write((ushort)0);
            wtr.Write((ushort)2102);
            wtr.Write((uint)0);
            wtr.Write((uint)page);
            int left = (int)MemberCount - page;
            if (left > 12) left = 12;
            if (left < 0) left = 0;
            wtr.Write((uint)left);
            int count = 0;
            int maxmem = page + 12;
            int minmem = page;
            List<Member> online = new List<Member>(250);
            List<Member> offline = new List<Member>(250);
            foreach (Member member in Members.Values)
            {
                if (member.IsOnline)
                    online.Add(member);
                else
                    offline.Add(member);
            }
            var unite = online.Union<Member>(offline);
            byte[] name = null;
            foreach (Member member in unite)
            {
                if (count >= minmem && count < maxmem)
                {
                    name = System.Text.Encoding.Default.GetBytes(member.Name);
                    for (int j = 0; j < 16; j++)
                    {
                        if (name.Length > j) wtr.Write(name[j]);
                        else wtr.Write((byte)0);
                    }
                    wtr.Write((uint)member.NobilityRank);
                    wtr.Write((uint)member.Gender);
                    wtr.Write((uint)member.Level);
                    wtr.Write((uint)member.Rank);
                    wtr.Write((uint)0); // EXP
                    wtr.Write((uint)member.SilverDonation + 1);
                    wtr.Write((uint)(member.IsOnline ? 1 : 0));
                    wtr.Write((uint)0); // unknown
                    if (member.IsOnline) wtr.Write((uint)member.Client.Entity.Class); // class
                    else wtr.Write((uint)0); // class
                    wtr.Write((ulong)0); // Offline time (in minutes)
                }
                count++;
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

        public void SendGuildMessage(Interfaces.IPacket message)
        {
            foreach (Member member in Members.Values)
            {
                if (member.IsOnline)
                {
                    member.Client.Send(message);
                }
            }
        }
        public Member GetMemberByName(string membername)
        {
            foreach (Member member in Members.Values)
            {
                if (member.Name == membername)
                {
                    return member;
                }
            }
            return null;
        }
        public void ExpelMember(string membername, bool ownquit)
        {
            Member member = GetMemberByName(membername);
            if (member != null)
            {
                if (member.IsOnline)
                    PacketHandler.UninscribeAllItems(member.Client);
                else
                    foreach (var arsenal in Arsenals)
                        arsenal.RemoveInscribedItemsBy(member.ID);

                if (ownquit)
                    SendGuildMessage(new Message(member.Name + " has quit our guild.", System.Drawing.Color.Red, Message.Guild));
                else
                    SendGuildMessage(new Message(member.Name + " has been Kicked from our guild.", System.Drawing.Color.Red, Message.Guild));
                uint uid = member.ID;
                if (member.Rank == Enums.GuildMemberRank.DeputyLeader)
                    DeputyLeaderCount--;
                if (member.IsOnline)
                {
                    GuildCommand command = new GuildCommand(true);
                    command.Type = GuildCommand.Disband;
                    command.dwParam = ID;
                    member.Client.Send(command);
                    member.Client.AsMember = null;
                    member.Client.Guild = null;
                    member.Client.Entity.GuildID = (ushort)0;
                    member.Client.Entity.GuildRank = (ushort)0;
                    member.Client.Screen.FullWipe();
                    member.Client.Screen.Reload(null);
                    member.Client.Entity.GuildBattlePower = 0;
                }
                else
                {
                    member.GuildID = 0;
                    Database.EntityTable.UpdateData(member.ID, "GuildID", 0);
                }
                MemberCount--;
                Members.Remove(uid);
            }
        }

        public void Disband()
        {
            var members = Members.Values.ToArray();
            foreach (Member member in members)
            {
                uint uid = member.ID;
                if (member.IsOnline)
                {
                    PacketHandler.UninscribeAllItems(member.Client);
                    member.Client.Entity.GuildBattlePower = 0;
                    GuildCommand command = new GuildCommand(true);
                    command.Type = GuildCommand.Disband;
                    command.dwParam = ID;
                    member.Client.Entity.GuildID = 0;
                    member.Client.Entity.GuildRank = 0;
                    member.Client.Send(command);
                    member.Client.Screen.FullWipe();
                    member.Client.Screen.Reload(null);
                    member.Client.AsMember = null;
                    member.Client.Guild = null;
                    Message message = null;
                    message = new Message("guild " + Name + " has been Disbanded!", System.Drawing.Color.White, Message.World);
                    foreach (Client.GameClient client in Program.GamePool)
                    {
                        client.Send(message);
                    }
                }
                else
                {
                    foreach (var arsenal in Arsenals)
                        arsenal.RemoveInscribedItemsBy(member.ID);
                    member.GuildID = 0;
                    Database.EntityTable.UpdateData(member.ID, "GuildID", 0);
                }
                MemberCount--;
                Members.Remove(uid);
            }
            var ally_ = Ally.Values.ToArray();
            foreach (Guild ally in ally_)
            {
                RemoveAlly(ally.Name);
                ally.RemoveAlly(Name);
            }
            Database.GuildTable.Disband(this);
            Kernel.GamePool.Remove(ID);
        }

        public void AddAlly(string name)
        {
            foreach (Guild guild in Kernel.Guilds.Values)
            {
                if (guild.Name == name)
                {
                    if (Enemy.ContainsKey(guild.ID))
                        RemoveEnemy(guild.Name);
                    if (!Ally.ContainsKey(guild.ID))
                    {
                        Database.GuildTable.AddAlly(this, guild.ID);
                        Ally.Add(guild.ID, guild);
                        _String stringPacket = new _String(true);
                        stringPacket.UID = guild.ID;
                        stringPacket.Type = _String.GuildAllies;
                        stringPacket.Texts.Add(guild.Name + " " + guild.LeaderName + " " + guild.Level + " " + guild.MemberCount);
                        SendGuildMessage(stringPacket);
                    }
                    return;
                }
            }
        }

        public void RemoveAlly(string name)
        {
            foreach (Guild guild in Ally.Values)
            {
                if (guild.Name == name)
                {
                    GuildCommand cmd = new GuildCommand(true);
                    cmd.Type = GuildCommand.Neutral1;
                    cmd.dwParam = guild.ID;
                    SendGuildMessage(cmd);
                    Database.GuildTable.RemoveAlly(this, guild.ID);
                    Ally.Remove(guild.ID);
                    return;
                }
            }
        }

        public void AddEnemy(string name)
        {
            foreach (Guild guild in Kernel.Guilds.Values)
            {
                if (guild.Name == name)
                {
                    if (Ally.ContainsKey(guild.ID))
                    {
                        RemoveAlly(guild.Name);
                        guild.RemoveAlly(Name);
                    }
                    Enemy.Add(guild.ID, guild);
                    _String stringPacket = new _String(true);
                    stringPacket.UID = guild.ID;
                    stringPacket.Type = _String.GuildEnemies;
                    stringPacket.Texts.Add(guild.Name + " " + guild.LeaderName + " " + guild.Level + " " + guild.MemberCount);
                    SendGuildMessage(stringPacket);
                    SendGuildMessage(stringPacket);
                    Database.GuildTable.AddEnemy(this, guild.ID);
                    return;
                }
            }
        }

        public void RemoveEnemy(string name)
        {
            foreach (Guild guild in Enemy.Values)
            {
                if (guild.Name == name)
                {
                    GuildCommand cmd = new GuildCommand(true);
                    cmd.Type = GuildCommand.Neutral2;
                    cmd.dwParam = guild.ID;
                    SendGuildMessage(cmd);
                    SendGuildMessage(cmd);
                    Database.GuildTable.RemoveEnemy(this, guild.ID);
                    Enemy.Remove(guild.ID);
                    return;
                }
            }
        }


        public void SendName(Client.GameClient client)
        {
            _String stringPacket = new _String(true);
            stringPacket.UID = ID;
            stringPacket.Type = _String.GuildName;
            stringPacket.Texts.Add(Name + " " + LeaderName + " 0 " + MemberCount);
            client.Send(stringPacket);
        }

        public void SendGuild(Client.GameClient client)
        {
            if (Members.ContainsKey(client.Entity.UID))
            {
                if (Bulletin == null)
                    Bulletin = "This is a new guild!";
                client.Send(new Message(Bulletin, System.Drawing.Color.White, Message.GuildAnnouncement));
                Writer.WriteUInt32(uint.Parse(DateTime.Now.ToString("yyyymmdd")), 67, Buffer);
                Writer.WriteUInt32((uint)client.AsMember.SilverDonation, 8, Buffer);
                Writer.WriteUInt32((ushort)client.AsMember.Rank, 28, Buffer);
                client.Send(Buffer);
            }
        }

        public void SendAllyAndEnemy(Client.GameClient client)
        {
            foreach (Guild guild in Enemy.Values)
            {
                _String stringPacket = new _String(true);
                stringPacket.UID = guild.ID;
                stringPacket.Type = _String.GuildEnemies;
                stringPacket.Texts.Add(guild.Name + " " + guild.LeaderName + " 0 " + guild.MemberCount);
                client.Send(stringPacket);
                client.Send(stringPacket);
            }
            foreach (Guild guild in Ally.Values)
            {
                _String stringPacket = new _String(true);
                stringPacket.UID = guild.ID;
                stringPacket.Type = _String.GuildAllies;
                stringPacket.Texts.Add(guild.Name + " " + guild.LeaderName + " 0 " + guild.MemberCount);
                client.Send(stringPacket);
                client.Send(stringPacket);
            }
        }
        public static bool ValidName(string Name)
        {
            if (Name.Length < 4 && Name.Length > 15) return false;
            else if (Name.IndexOfAny(new char[20] { ' ', '#', '%', '^', '&', '*', '(', ')', ';', ':', '\'', '\"', '/', '\\', ',', '.', '{', '}', '[', ']' }) > 0) return false;
            else return true;
        }
    }
}
