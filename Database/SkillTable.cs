#region OldSkillTable
namespace Conquer_Online_Server.Database
{
    using Conquer_Online_Server.Client;
    using Conquer_Online_Server.Interfaces;
    using Conquer_Online_Server.Network.GamePackets;
    using MySql.Data.MySqlClient;
    using System;
    using System.Collections.Generic;

    public class SkillTable
    {
        public static void removeAllSkills(Client.GameClient client)
        {
            using (var conn = DataHolder.MySqlConnection)
            {
                using (var cmd = new MySql.Data.MySqlClient.MySqlCommand("Delete from  skills where EntityID =" + client.Entity.UID.ToString(), conn))
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public static void removeAllProfs(Client.GameClient client)
        {
            using (var conn = DataHolder.MySqlConnection)
            {
                using (var cmd = new MySql.Data.MySqlClient.MySqlCommand("Delete from  profs where EntityID =" + client.Entity.UID.ToString(), conn))
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void DeleteSpell(GameClient client, ushort ID)
        {
            Conquer_Online_Server.Database.MySqlCommand command = new Conquer_Online_Server.Database.MySqlCommand(MySqlCommandType.DELETE);
            command.Delete("skills", "ID", (long)ID).And("EntityID", (long)client.Entity.UID).Execute();
        }

        public static void LoadProficiencies(GameClient client)
        {
            if (client.Fake) return;
            if (client.Entity != null)
            {
                client.Proficiencies = new SafeDictionary<ushort, IProf>(100);
                MySqlReader reader = new MySqlReader(new Conquer_Online_Server.Database.MySqlCommand(MySqlCommandType.SELECT).Select("profs").Where("EntityID", (long)client.Entity.UID));
                while (reader.Read())
                {
                    IProf prof = new Proficiency(true)
                    {
                        ID = reader.ReadUInt16("ID"),
                        Level = reader.ReadByte("Level"),
                        PreviousLevel = reader.ReadByte("PreviousLevel"),
                        Experience = reader.ReadUInt32("Experience"),
                        Available = true
                    };
                    if (!client.Proficiencies.ContainsKey(prof.ID))
                    {
                        client.Proficiencies.Add(prof.ID, prof);
                    }
                }
            }
        }

        public static void LoadProficiencies(GameClient client, MySqlConnection conn)
        {
            if (client.Fake) return;
            if (client.Entity != null)
            {
                client.Proficiencies = new SafeDictionary<ushort, IProf>(100);
                MySqlReader reader = new MySqlReader(new Conquer_Online_Server.Database.MySqlCommand(MySqlCommandType.SELECT).Select("profs").Where("EntityID", (long)client.Entity.UID));
                while (reader.Read())
                {
                    IProf prof = new Proficiency(true)
                    {
                        ID = reader.ReadUInt16("ID"),
                        Level = reader.ReadByte("Level"),
                        PreviousLevel = reader.ReadByte("PreviousLevel"),
                        Experience = reader.ReadUInt32("Experience"),

                        Available = true
                    };
                    if (!client.Proficiencies.ContainsKey(prof.ID))
                    {
                        client.Proficiencies.Add(prof.ID, prof);
                    }
                }
                //  Reader.Close();
                ////  Reader.Dispose();
            }
        }

        public static void LoadSpells(GameClient client)
        {
            if (client.Fake) return;
            if (client.Entity != null)
            {
                client.Spells = new SafeDictionary<ushort, ISkill>(100);
                MySqlReader reader = new MySqlReader(new Conquer_Online_Server.Database.MySqlCommand(MySqlCommandType.SELECT).Select("skills").Where("EntityID", (long)client.Entity.UID));
                while (reader.Read())
                {
                    ISkill skill = new Spell(true)
                    {
                        ID = reader.ReadUInt16("ID"),
                        Level = reader.ReadByte("Level"),
                        PreviousLevel = reader.ReadByte("PreviousLevel"),
                        Experience = reader.ReadUInt32("Experience"),
                        LevelHu = reader.ReadByte("LevelHu"),
                        LevelHu2 = reader.ReadByte("LevelHu"),
                        Available = true
                    };
                    if (!client.Spells.ContainsKey(skill.ID))
                    {
                        client.Spells.Add(skill.ID, skill);
                    }
                }
                //  Reader.Close();
                ////  Reader.Dispose();
            }
        }

        public static void LoadSpells(GameClient client, MySqlConnection conn)
        {
            if (client.Fake) return;
            if (client.Entity != null)
            {
                client.Spells = new SafeDictionary<ushort, ISkill>(100);
                MySqlReader reader = new MySqlReader(new Conquer_Online_Server.Database.MySqlCommand(MySqlCommandType.SELECT).Select("skills").Where("EntityID", (long)client.Entity.UID));
                while (reader.Read())
                {
                    ISkill skill = new Spell(true);
                    {
                        skill.ID = reader.ReadUInt16("ID");
                        skill.Level = reader.ReadByte("Level");
                        skill.PreviousLevel = reader.ReadByte("PreviousLevel");
                        skill.Experience = reader.ReadUInt32("Experience");
                        skill.PreviousLevel = reader.ReadByte("PreviousLevel");
                        skill.LevelHu = reader.ReadByte("LevelHu");
                        skill.LevelHu2 = reader.ReadByte("LevelHu");
                        skill.Available = true;
                    };
                    if (!client.Spells.ContainsKey(skill.ID))
                    {
                        client.Spells.Add(skill.ID, skill);
                    }
                }
                //  Reader.Close();
                ////  Reader.Dispose();
            }
        }

        public static void SaveProficiencies(GameClient client)
        {
            if (client.Fake) return;
            if (((client.Entity != null) && (client.Proficiencies != null)) && (client.Proficiencies.Count != 0))
            {
                foreach (IProf prof in client.Proficiencies.Values)
                {
                    Conquer_Online_Server.Database.MySqlCommand command;
                    if (prof.Available)
                    {
                        if (prof.PreviousLevel != prof.Level)
                        {
                            command = new Conquer_Online_Server.Database.MySqlCommand(MySqlCommandType.UPDATE);
                            command.Update("profs").Set("Level", (long)prof.Level).Set("PreviousLevel", (long)prof.Level).Set("PreviousLevel", (long)prof.PreviousLevel).Set("Experience", (long)prof.Experience).Where("EntityID", (long)client.Entity.UID).And("ID", (long)prof.ID).Execute();
                        }
                    }
                    else
                    {
                        prof.Available = true;
                        command = new Conquer_Online_Server.Database.MySqlCommand(MySqlCommandType.INSERT);
                        command.Insert("profs").Insert("Level", (long)prof.Level).Insert("PreviousLevel", (long)prof.Level).Insert("Experience", (long)prof.Experience).Insert("EntityID", (long)client.Entity.UID).Insert("ID", (long)prof.ID).Execute();
                    }
                }
            }
        }

        public static void SaveProficiencies(GameClient client, MySqlConnection conn)
        {
            if (client.Fake) return;
            if (((client.Entity != null) && (client.Proficiencies != null)) && (client.Proficiencies.Count != 0))
            {
                foreach (IProf prof in client.Proficiencies.Values)
                {
                    Conquer_Online_Server.Database.MySqlCommand command;
                    if (prof.Available)
                    {
                        if (prof.PreviousLevel != prof.Level)
                        {
                            command = new Conquer_Online_Server.Database.MySqlCommand(MySqlCommandType.UPDATE);
                            command.Update("profs").Set("Level", (long)prof.Level).Set("PreviousLevel", (long)prof.Level).Set("PreviousLevel", (long)prof.PreviousLevel).Set("Experience", (long)prof.Experience).Where("EntityID", (long)client.Entity.UID).And("ID", (long)prof.ID).Execute();
                        }
                    }
                    else
                    {
                        prof.Available = true;
                        command = new Conquer_Online_Server.Database.MySqlCommand(MySqlCommandType.INSERT);
                        command.Insert("profs").Insert("Level", (long)prof.Level).Insert("PreviousLevel", (long)prof.Level).Insert("Experience", (long)prof.Experience).Insert("EntityID", (long)client.Entity.UID).Insert("ID", (long)prof.ID).Execute();
                    }
                }
            }
        }

        public static void SaveSpells(GameClient client)
        {
            if (client.Fake) return;
            if (((client.Entity != null) && (client.Spells != null)) && (client.Spells.Count != 0))
            {
                foreach (ISkill skill in client.Spells.Values)
                {
                    Conquer_Online_Server.Database.MySqlCommand command;
                    if (skill.Available)
                    {
                        if (skill.PreviousLevel != skill.Level)
                        {
                            command = new Conquer_Online_Server.Database.MySqlCommand(MySqlCommandType.UPDATE);
                            command.Update("skills").Set("LevelHu", (long)skill.LevelHu).Set("Level", (long)skill.Level).Set("PreviousLevel", (long)skill.Level).Set("PreviousLevel", (long)skill.PreviousLevel).Set("Experience", (long)skill.Experience).Where("EntityID", (long)client.Entity.UID).And("ID", (long)skill.ID).Execute();
                        }
                    }
                    else
                    {
                        skill.Available = true;
                        command = new Conquer_Online_Server.Database.MySqlCommand(MySqlCommandType.INSERT);
                        command.Insert("skills").Insert("LevelHu", (long)skill.LevelHu).Insert("Level", (long)skill.Level).Insert("PreviousLevel", (long)skill.Level).Insert("Experience", (long)skill.Experience).Insert("EntityID", (long)client.Entity.UID).Insert("ID", (long)skill.ID).Execute();
                    }
                }
            }
        }

        public static void SaveSpells(GameClient client, MySqlConnection conn)
        {
            if (client.Fake) return;
            if (((client.Entity != null) && (client.Spells != null)) && (client.Spells.Count != 0))
            {
                foreach (ISkill skill in client.Spells.Values)
                {
                    Conquer_Online_Server.Database.MySqlCommand command;
                    if (skill.Available)
                    {
                        if (skill.PreviousLevel != skill.Level)
                        {
                            command = new Conquer_Online_Server.Database.MySqlCommand(MySqlCommandType.UPDATE);
                            command.Update("skills").Set("LevelHu", (long)skill.LevelHu).Set("Level", (long)skill.Level).Set("PreviousLevel", (long)skill.Level).Set("PreviousLevel", (long)skill.PreviousLevel).Set("Experience", (long)skill.Experience).Where("EntityID", (long)client.Entity.UID).And("ID", (long)skill.ID).Execute();
                        }
                    }
                    else
                    {
                        skill.Available = true;
                        command = new Conquer_Online_Server.Database.MySqlCommand(MySqlCommandType.INSERT);
                        command.Insert("skills").Insert("LevelHu", (long)skill.LevelHu).Insert("Level", (long)skill.Level).Insert("PreviousLevel", (long)skill.Level).Insert("Experience", (long)skill.Experience).Insert("EntityID", (long)client.Entity.UID).Insert("ID", (long)skill.ID).Execute();
                    }
                }
            }
        }
    }
}
#endregion