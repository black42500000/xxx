using System;
using Conquer_Online_Server.Game;
using Conquer_Online_Server.Network.GamePackets;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Conquer_Online_Server.Statement;

namespace Conquer_Online_Server.Statement
{
    public enum KungFuGrade : byte
    {
        None = 0,
        Dragon = 1,
        Phoenix = 2,
        Tiger = 3,
        Turtle = 4
    }
    public class KungFu
    {
        public uint UID;
        public uint TalentPoint;
        public uint FreeTimes;
        public byte TalentStatus;
        public string Name;

        public byte Grade;
    }
    public class KungFuRanks
    {
        public uint UID;
        public uint Intter;
        public string Name;
        public byte Level;
        public string KungName;
        public byte Grade;
    }
    public class KungFuGrades
    {
        public uint UID;
        public byte Stage;
        public byte Kun1;
        public byte Str1;
        public byte Kun2;
        public byte Str2;
        public byte Kun3;
        public byte Str3;
        public byte Kun4;
        public byte Str4;
        public byte Kun5;
        public byte Str5;
        public byte Kun6;
        public byte Str6;
        public byte Kun7;
        public byte Str7;
        public byte Kun8;
        public byte Str8;
        public byte Kun9;
        public byte Str9;
        public uint Inner;
        public byte TotalOpened;

    }
    public class KungFuGClasses
    {
        public Dictionary<uint, KungFu> MainGui;
        public Dictionary<uint, KungFuGrades> KungFuStages;

        public KungFuGClasses()
        {
            MainGui = new Dictionary<uint, KungFu>();
            KungFuStages = new Dictionary<uint, KungFuGrades>();

            // TalentPoint = 0;

        }
    }
}
namespace Conquer_Online_Server.Database
{

    public class SubClassTable
    {
        public static ConcurrentDictionary<uint, KungFuRanks> KunFuRanks = new ConcurrentDictionary<uint, KungFuRanks>();
        public static KungFuRanks[] Rankss;
        public static void LoadKungFuRanks()
        {
            Statement.KungFuGClasses KunFuClasses = new Statement.KungFuGClasses();
            Database.MySqlCommand cmd = new Database.MySqlCommand(Database.MySqlCommandType.SELECT);
            uint ch = 0;
            cmd.Select("kungfumain").Order("Strangsss DESC");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            while (r.Read())
            {

                #region Read
                KungFuRanks koko = new KungFuRanks();
                koko.UID = r.ReadUInt32("UID");
                koko.Intter = r.ReadUInt32("Strangsss");
                var rdr = new Conquer_Online_Server.Database.MySqlReader(new
     Conquer_Online_Server.Database.MySqlCommand(
     Conquer_Online_Server.Database.MySqlCommandType.SELECT)
     .Select("entities").Where("UID", koko.UID));
                if (rdr.Read())
                {
                    koko.Name = rdr.ReadString("name");
                    koko.Level = rdr.ReadByte("Level");
                }
                rdr.Close();
                var rdrs = new Conquer_Online_Server.Database.MySqlReader(new
     Conquer_Online_Server.Database.MySqlCommand(
     Conquer_Online_Server.Database.MySqlCommandType.SELECT)
     .Select("kungfumain").Where("UID", koko.UID));
                if (rdrs.Read())
                {
                    koko.KungName = rdrs.ReadString("name");
                }
                rdrs.Close();
                ch++;


                #endregion
                if (!KunFuRanks.ContainsKey(koko.UID))
                {
                    KunFuRanks.Add(koko.UID, koko);
                }
            }
            r.Close();
        }
        public static void LoadKungFu(Game.Entity Entity)
        {
            MySqlCommand Command = new MySqlCommand(MySqlCommandType.SELECT);
            Command.Select("KungFuMain").Where("UID", Entity.UID);
            MySqlReader Reader = new MySqlReader(Command);
            while (Reader.Read())
            {
                Statement.KungFu Chi = new Statement.KungFu();

                Chi.UID = Reader.ReadUInt32("UID");
                Chi.Name = Reader.ReadString("Name");
                Chi.Grade = Reader.ReadByte("Grade");
                Chi.TalentStatus = Reader.ReadByte("TalentStatus");
                Chi.TalentPoint = Reader.ReadUInt32("TalentPoint");
                Chi.FreeTimes = Reader.ReadUInt32("FreeTimes");

                if (!Entity.KunFuClasses.MainGui.ContainsKey(Chi.UID))
                {
                    Entity.KunFuClasses.MainGui.Add(Chi.UID, Chi);
                }
                else
                {
                    continue;
                }
            }
            Reader.Close();
        }
        public static void InsertKungFu(Entity Entity, string Name, byte Grade, byte TalentStatus, uint TalentPoint, uint FreeTimes)
        {

            MySqlCommand Command = new MySqlCommand(MySqlCommandType.INSERT);
            Command.Insert("KungFuMain")
                .Insert("Grade", Grade)
                .Insert("UID", Entity.UID)
                .Insert("Name", Name)
                .Insert("TalentStatus", TalentStatus)
                .Insert("TalentPoint", TalentPoint)
                .Insert("FreeTimes", FreeTimes)
                .Insert("Strangsss", 0)
                .Execute();
            // Entity.SubClasses.Classes.Add(id, Sub);
        }
        public static void UpdateKungFu(Entity Entity, string Name, byte Grade, byte TalentStatus, uint TalentPoint, uint FreeTimes, uint Inner)
        {


            MySqlCommand Command = new MySqlCommand(MySqlCommandType.UPDATE);
            Command.Update("KungFuMain")
                .Set("Grade", Grade)
                .Set("TalentPoint", TalentPoint)
                .Set("Name", Name)
                .Set("TalentStatus", TalentStatus)
                .Set("FreeTimes", FreeTimes)
                 .Set("Strangsss", Inner)

                .Where("UID", Entity.UID)

                .Execute();

        }
        public static void LoadKungFuAttrib(Game.Entity Entity)
        {
            MySqlCommand Command = new MySqlCommand(MySqlCommandType.SELECT);
            Command.Select("KungFu").Where("UID", Entity.UID);
            MySqlReader Reader = new MySqlReader(Command);
            while (Reader.Read())
            {
                Statement.KungFuGrades Chi = new Statement.KungFuGrades();

                Chi.UID = Reader.ReadUInt32("UID");
                Chi.Stage = Reader.ReadByte("Stage");
                Chi.TotalOpened = Reader.ReadByte("TotalOpened");
                Chi.Inner = Reader.ReadUInt32("Strangsss");
                Chi.Kun1 = Reader.ReadByte("Kun1");
                Chi.Str1 = Reader.ReadByte("Str1");
                Chi.Kun2 = Reader.ReadByte("Kun2");
                Chi.Str2 = Reader.ReadByte("Str2");
                Chi.Kun3 = Reader.ReadByte("Kun3");
                Chi.Str3 = Reader.ReadByte("Str3");
                Chi.Kun4 = Reader.ReadByte("Kun4");
                Chi.Str4 = Reader.ReadByte("Str4");
                Chi.Kun5 = Reader.ReadByte("Kun5");
                Chi.Str5 = Reader.ReadByte("Str5");
                Chi.Kun6 = Reader.ReadByte("Kun6");
                Chi.Str6 = Reader.ReadByte("Str6");
                Chi.Kun7 = Reader.ReadByte("Kun7");
                Chi.Str7 = Reader.ReadByte("Str7");
                Chi.Kun8 = Reader.ReadByte("Kun8");
                Chi.Str8 = Reader.ReadByte("Str8");
                Chi.Kun9 = Reader.ReadByte("Kun9");
                Chi.Str9 = Reader.ReadByte("Str9");

                if (!Entity.KunFuClasses.KungFuStages.ContainsKey(Chi.Stage))
                {
                    Entity.KunFuClasses.KungFuStages.Add(Chi.Stage, Chi);
                }
                else
                {
                    continue;
                }
            }
            Reader.Close();
        }
        public static void InsertAttrbiuteKungFu(uint UID, byte Stage, byte TotalOpened, uint Inner, byte Type, byte Str)
        {

            MySqlCommand Command = new MySqlCommand(MySqlCommandType.INSERT);
            Command.Insert("KungFu")

                .Insert("UID", UID)
                .Insert("TotalOpened", TotalOpened)
                .Insert("Stage", Stage)
                .Insert("Strangsss", Inner)
                .Insert("Kun1", Type)
                .Insert("Str1", Str);

            Command.Execute();
        }
        public static void UpdateAttrbiuteKungFu(Entity Entity, byte Stage, byte TotalOpened, uint Inner, byte Star, byte Type, byte Str)
        {
            switch (Star)
            {
                #region Star1
                case 1:
                    {
                        MySqlCommand Command = new MySqlCommand(MySqlCommandType.UPDATE);
                        Command.Update("KungFu")
                            .Set("Stage", Stage)
                            .Set("TotalOpened", TotalOpened)
                            .Set("Strangsss", Inner)
                            .Set("Kun1", Type)
                            .Set("Str1", Str)
                             .Where("UID", Entity.UID).And("Stage", Stage)
                            .Execute();
                        break;
                    }
                #endregion
                #region Star2
                case 2:
                    {
                        MySqlCommand Command = new MySqlCommand(MySqlCommandType.UPDATE);
                        Command.Update("KungFu")
                            .Set("Stage", Stage)
                            .Set("TotalOpened", TotalOpened)
                            .Set("Strangsss", Inner)
                            .Set("Kun2", Type)
                            .Set("Str2", Str)
                             .Where("UID", Entity.UID).And("Stage", Stage)
                            .Execute();
                        break;
                    }
                #endregion
                #region Star3
                case 3:
                    {
                        MySqlCommand Command = new MySqlCommand(MySqlCommandType.UPDATE);
                        Command.Update("KungFu")
                            .Set("Stage", Stage)
                            .Set("TotalOpened", TotalOpened)
                            .Set("Strangsss", Inner)
                            .Set("Kun3", Type)
                            .Set("Str3", Str)
                             .Where("UID", Entity.UID).And("Stage", Stage)
                            .Execute();
                        break;
                    }
                #endregion
                #region Star4
                case 4:
                    {
                        MySqlCommand Command = new MySqlCommand(MySqlCommandType.UPDATE);
                        Command.Update("KungFu")
                            .Set("Stage", Stage)
                            .Set("TotalOpened", TotalOpened)
                            .Set("Strangsss", Inner)
                            .Set("Kun4", Type)
                            .Set("Str4", Str)
                             .Where("UID", Entity.UID).And("Stage", Stage)
                            .Execute();
                        break;
                    }
                #endregion
                #region Star5
                case 5:
                    {
                        MySqlCommand Command = new MySqlCommand(MySqlCommandType.UPDATE);
                        Command.Update("KungFu")
                            .Set("Stage", Stage)
                            .Set("TotalOpened", TotalOpened)
                            .Set("Strangsss", Inner)
                            .Set("Kun5", Type)
                            .Set("Str5", Str)
                             .Where("UID", Entity.UID).And("Stage", Stage)
                            .Execute();
                        break;
                    }
                #endregion
                #region Star6
                case 6:
                    {
                        MySqlCommand Command = new MySqlCommand(MySqlCommandType.UPDATE);
                        Command.Update("KungFu")
                            .Set("Stage", Stage)
                            .Set("TotalOpened", TotalOpened)
                            .Set("Strangsss", Inner)
                            .Set("Kun6", Type)
                            .Set("Str6", Str)
                             .Where("UID", Entity.UID).And("Stage", Stage)
                            .Execute();
                        break;
                    }
                #endregion
                #region Star7
                case 7:
                    {
                        MySqlCommand Command = new MySqlCommand(MySqlCommandType.UPDATE);
                        Command.Update("KungFu")
                            .Set("Stage", Stage)
                            .Set("TotalOpened", TotalOpened)
                            .Set("Strangsss", Inner)
                            .Set("Kun7", Type)
                            .Set("Str7", Str)
                             .Where("UID", Entity.UID).And("Stage", Stage)
                            .Execute();
                        break;
                    }
                #endregion
                #region Star8
                case 8:
                    {
                        MySqlCommand Command = new MySqlCommand(MySqlCommandType.UPDATE);
                        Command.Update("KungFu")
                            .Set("Stage", Stage)
                            .Set("TotalOpened", TotalOpened)
                            .Set("Strangsss", Inner)
                            .Set("Kun8", Type)
                            .Set("Str8", Str)
                             .Where("UID", Entity.UID).And("Stage", Stage)
                            .Execute();
                        break;
                    }
                #endregion
                #region Star9
                case 9:
                    {
                        MySqlCommand Command = new MySqlCommand(MySqlCommandType.UPDATE);
                        Command.Update("KungFu")
                            .Set("Stage", Stage)
                            .Set("TotalOpened", TotalOpened)
                            .Set("Strangsss", Inner)
                            .Set("Kun9", Type)
                            .Set("Str9", Str)
                             .Where("UID", Entity.UID).And("Stage", Stage)
                            .Execute();
                        break;
                    }
                #endregion
            }

        }

        public static void Load(Entity Entity)
        {
            using (var cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("subclasses").Where("id", Entity.UID))
            using (var reader = cmd.CreateReader())
            {
                while (reader.Read())
                {
                    Game.SubClass Sub = new Game.SubClass();
                    Sub.ID = reader.ReadByte("Uid");
                    Sub.Level = reader.ReadByte("Level");
                    Sub.Phase = reader.ReadByte("Phase");
                    Entity.SubClasses.Classes.Add(Sub.ID, Sub);

                    Game_SubClass packet = new Game_SubClass();
                    packet.ClassId = (Game_SubClass.ID)Sub.ID;
                    packet.Phase = Sub.Phase;
                    packet.Type = Game_SubClass.Types.Learn;
                    Entity.Owner.Send(packet);
                    packet.Type = Game_SubClass.Types.MartialPromoted;
                    Entity.Owner.Send(packet);
                }
            }
        }

        public static bool Contains(Entity Entity, byte id)
        {
            bool Return = false;
            using (var Command = new MySqlCommand(MySqlCommandType.SELECT))
            {
                Command.Select("subclasses").Where("id", Entity.UID).And("uid", id);
                using (var Reader = Command.CreateReader())
                    if (Reader.Read())
                        if (Reader.ReadByte("uid") == id)
                            Return = true;
            }
            return Return;
        }

        public static void Insert(Entity Entity, byte id)
        {
            using (var Command = new MySqlCommand(MySqlCommandType.INSERT))
                Command.Insert("subclasses")
                    .Insert("uid", id)
                    .Insert("id", Entity.UID)
                    .Execute();
        }
        public static void Update(Game.Entity Entity, Game.SubClass SubClass)
        {
            MySqlCommand Command = new MySqlCommand(MySqlCommandType.UPDATE);
            Command.Update("subclasses")
                .Set("phase", SubClass.Phase)
                .Set("level", SubClass.Level)
                .Where("id", Entity.UID)
                .And("uid", SubClass.ID)
                .Execute();
        }

        public static void Update(Client.GameClient client)
        {
            EntityTable.UpdateData(client, "StudyPoints", client.Entity.SubClasses.StudyPoints);
        }
    }
}
