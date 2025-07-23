using System;
using System.Collections.Generic;

namespace Conquer_Online_Server.Database
{
    using Member = Game.ConquerStructures.Society.Guild.Member;
    using Conquer_Online_Server.Game.ConquerStructures.Society;
  
    public class GuildTable
    {
        public static void Load()
        {
            Dictionary<uint, SafeDictionary<uint, Member>> dict = new Dictionary<uint, SafeDictionary<uint, Member>>();
            using (var cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("entities").Where("guildid", 0, true))
            using (var reader = new MySqlReader(cmd))
            {
                while (reader.Read())
                {
                    Member member = new Member(reader.ReadUInt16("guildid"));
                    member.ID = reader.ReadUInt32("uid");
                    member.Name = reader.ReadString("name");
                    member.Level = reader.ReadByte("level");

                    if (Game.ConquerStructures.Nobility.Board.ContainsKey(member.ID))
                    {
                        member.NobilityRank = Game.ConquerStructures.Nobility.Board[member.ID].Rank;
                        member.Gender = Game.ConquerStructures.Nobility.Board[member.ID].Gender;
                    }

                    member.Rank = (Game.Enums.GuildMemberRank)reader.ReadUInt16("guildrank");
                    member.SilverDonation = reader.ReadUInt64("GuildSilverDonation");
                    member.ConquerPointDonation = reader.ReadUInt64("GuildConquerPointDonation");
                    if (!dict.ContainsKey(member.GuildID)) dict.Add(member.GuildID, new SafeDictionary<uint, Member>());
                    dict[member.GuildID].Add(member.ID, member);
                }
            }
            using (var cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("guilds"))
            using (var reader = new MySqlReader(cmd))
            {
                while (reader.Read())
                {
                    Guild guild = new Guild(reader.ReadString("LeaderName"));
                    guild.ID = reader.ReadUInt32("Id");
                    guild.Name = reader.ReadString("Name");
                    guild.Wins = reader.ReadUInt32("Wins");
                    guild.Losts = reader.ReadUInt32("Losts");
                    guild.Bulletin = reader.ReadString("Bulletin");
                    guild.SilverFund = reader.ReadUInt64("SilverFund");
                    guild.CTFPoints = reader.ReadUInt32("CTFPoints");
                    guild.CTFReward = reader.ReadUInt32("CTFReward");
                    guild.ConquerPointFund = reader.ReadUInt32("ConquerPointFund");
                    guild.LevelRequirement = reader.ReadUInt32("LevelRequirement");
                    guild.RebornRequirement = reader.ReadUInt32("RebornRequirement");
                    guild.ClassRequirement = reader.ReadUInt32("ClassRequirement");
                    if (dict.ContainsKey(guild.ID))
                    {
                        guild.Members = dict[guild.ID];
                        guild.MemberCount = (uint)guild.Members.Count;
                    }
                    else
                        guild.Members = new SafeDictionary<uint, Member>();
                    Kernel.Guilds.Add(guild.ID, guild);
                    foreach (var member in guild.Members.Values)
                    {
                        if (member.Rank == Game.Enums.GuildMemberRank.GuildLeader)
                            guild.Leader = member;
                        else if (member.Rank == Game.Enums.GuildMemberRank.DeputyLeader)
                            guild.DeputyLeaderCount++;
                    }
                    GuildArsenalTable.Load(guild);
                    guild.Level = 1;
                }
            }

            LoadAllyEnemy();
            Console.WriteLine("Guild information loaded.");
        }
        public static void LoadAllyEnemy()
        {
            using (var cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("guildenemy"))
            using (var reader = new MySqlReader(cmd))
            {
                while (reader.Read())
                {
                    ushort guildID = reader.ReadUInt16("guildid");
                    ushort enemyID = reader.ReadUInt16("enemyid");
                    if (Kernel.Guilds.ContainsKey(guildID))
                        if (Kernel.Guilds.ContainsKey(enemyID))
                            Kernel.Guilds[guildID].Enemy.Add(enemyID, Kernel.Guilds[enemyID]);
                }
            }
            using (var cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("guildally"))
            using (var reader = new MySqlReader(cmd))
            {
                while (reader.Read())
                {
                    ushort guildID = reader.ReadUInt16("guildid");
                    ushort allyID = reader.ReadUInt16("allyid");
                    if (Kernel.Guilds.ContainsKey(guildID))
                        if (Kernel.Guilds.ContainsKey(allyID))
                            Kernel.Guilds[guildID].Ally.Add(allyID, Kernel.Guilds[allyID]);
                }
            }
        }

        public static void UpdateBulletin(Guild guild, string bulletin)
        {
            using (var cmd = new MySqlCommand(MySqlCommandType.UPDATE).Update("guilds").Set("Bulletin", bulletin).Where("ID", guild.ID))
                cmd.Execute();
        }
        public static void SaveFunds(Guild guild)
        {
            using (var cmd = new MySqlCommand(MySqlCommandType.UPDATE).Update("guilds")
                .Set("ConquerPointFund", guild.ConquerPointFund)
                .Set("SilverFund", guild.SilverFund)
                .Where("ID", guild.ID))
                cmd.Execute();
        }
        public static void SaveCTFPoins(Guild guild)
        {
            using (var cmd = new MySqlCommand(MySqlCommandType.UPDATE).Update("guilds")
                .Set("CTFPoints", guild.CTFPoints)
                .Where("ID", guild.ID))
                cmd.Execute();
        }
        public static void SaveCTFReward(Guild guild)
        {
            using (var cmd = new MySqlCommand(MySqlCommandType.UPDATE).Update("guilds")
                .Set("CTFReward", guild.CTFReward)
                .Where("ID", guild.ID))
                cmd.Execute();
        }
        public static void Disband(Guild guild)
        {
            using (var cmd = new MySqlCommand(MySqlCommandType.UPDATE).Update("entities")
                .Set("guildid", 0)
                .Where("guildid", guild.ID))
                cmd.Execute();
            using (var cmd = new MySqlCommand(MySqlCommandType.DELETE).Delete("guilds", "id", guild.ID))
                cmd.Execute();
        }
        public static void Create(Guild guild)
        {
            while (true)
            {
                using (var cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("guilds").Where("id", guild.ID))
                using (var reader = cmd.CreateReader())
                {
                    if (reader.Read())
                        guild.ID = Guild.GuildCounter.Next;
                    else
                        break;
                }
            }
            using (var cmd = new MySqlCommand(MySqlCommandType.INSERT).Insert("guilds")
                .Insert("ID", guild.ID).Insert("name", guild.Name).Insert("Bulletin", "")
                .Insert("SilverFund", 500000).Insert("LeaderName", guild.LeaderName))
                cmd.Execute();
        }
        public static void AddEnemy(Guild guild, uint enemy)
        {
            using (var cmd = new MySqlCommand(MySqlCommandType.INSERT).Insert("guildenemy")
                .Insert("guildID", guild.ID).Insert("enemyID", enemy))
                cmd.Execute();
        }
        public static void AddAlly(Guild guild, uint ally)
        {
            using (var cmd = new MySqlCommand(MySqlCommandType.INSERT).Insert("guildally")
                .Insert("guildID", guild.ID).Insert("allyID", ally))
                cmd.Execute();
        }
        public static void RemoveEnemy(Game.ConquerStructures.Society.Guild guild, uint enemy)
        {
            using(var command = new MySqlCommand(MySqlCommandType.DELETE))
                command.Delete("guildenemy", "GuildID", guild.ID).And("EnemyID", enemy)
                    .Execute();
        }
        public static void RemoveAlly(Game.ConquerStructures.Society.Guild guild, uint ally)
        {
            using(var command = new MySqlCommand(MySqlCommandType.DELETE))
                command.Delete("guildally", "GuildID", guild.ID).And("AllyID", ally)
                    .Execute();
            using(var command = new MySqlCommand(MySqlCommandType.DELETE))
                command.Delete("guildally", "GuildID", ally).And("AllyID", guild.ID)
                    .Execute();
        }
        public static void UpdateGuildWarStats(Guild guild)
        {
            using (var cmd = new MySqlCommand(MySqlCommandType.UPDATE).Update("guilds")
                .Set("wins", guild.Wins).Set("losts", guild.Losts)
                .Where("id", guild.ID))
                cmd.Execute();
        }

        internal static void SaveRequirements(Guild guild)
        {
            using(var command = new MySqlCommand(MySqlCommandType.UPDATE))
                command.Update("guilds").Set("LevelRequirement", guild.LevelRequirement)
                    .Set("RebornRequirement", guild.RebornRequirement).Set("ClassRequirement", guild.ClassRequirement)
                    .Where("ID", guild.ID).Execute();
        }
    }
}
