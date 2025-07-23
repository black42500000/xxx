namespace Conquer_Online_Server.Database
{
    using Conquer_Online_Server;
    using Conquer_Online_Server.Game;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class JiangHu
    {
        public static Dictionary<ushort, Atribut> Attributes = new Dictionary<ushort, Atribut>();
        public static Dictionary<byte, List<byte>> CultivateStatus = new Dictionary<byte, List<byte>>();
        private static byte[] FreeCourse = new byte[] { 10, 20, 0x1a, 0x1f, 0x34 };
        private static ushort[] FreeCourseInCastle = new ushort[] { 0x126, 0x271, 0x341, 0x3e8, 0x61a };
        public static bool fullload = false;
        private static ushort[] Points = new ushort[] { 100, 120, 150, 190, 240, 300 };
        private static double[] MinutesInCastle = new double[] { 31.25, 31.25, 32.0, 32.2, 30.0 };
        private static ushort[] MinutesOnTalent = new ushort[] { 0x3e8, 500, 0x180, 0x142, 0xc0 };

        public static ushort GetFreeCourse(byte Talent)
        {
            if (Talent == 0)
            {
                Talent = 1;
            }
            return FreeCourse[Math.Min(4, Talent - 1)];
        }

        public static ushort GetFreeCourseInCastle(byte Talent)
        {
            if (Talent == 0)
            {
                Talent = 1;
            }
            return FreeCourseInCastle[Math.Min(4, Talent - 1)];
        }

        public static double GetMinutesInCastle(byte Talent)
        {
            if (Talent == 0)
            {
                Talent = 1;
            }
            return MinutesInCastle[Math.Min(4, Talent - 1)];
        }

        public static ushort GetMinutesOnTalent(byte Talent)
        {
            if (Talent == 0)
            {
                Talent = 1;
            }
            return MinutesOnTalent[Math.Min(4, Talent - 1)];
        }

        public static ushort GetPower(ushort UID)
        {
            return Attributes[UID].Power;
        }

        public static ushort GetStatusPoints(byte Level)
        {
            if (Level == 0)
            {
                Level = 1;
            }
            return Points[Math.Min(5, Level - 1)];
        }

        public static void LoadJiangHu()
        {
            using (Conquer_Online_Server.Database.Read read = new Conquer_Online_Server.Database.Read(@"database\JiangHu.txt"))
            {
                if (read.Reader(true))
                {
                    int count = read.Count;
                    for (uint i = 0; i < count; i++)
                    {
                        string line = read.ReadString("");
                        if (line != null)
                        {//////////////
                            Conquer_Online_Server.Game.JiangHu hu = new Conquer_Online_Server.Game.JiangHu(0);
                            hu.Load(line);
                            Conquer_Online_Server.Game.JiangHu.JiangHuClients.TryAdd(hu.UID, hu);
                            Conquer_Online_Server.Game.JiangHu.JiangHuRanking.UpdateRank(hu);
                        }
                    }
                }
            }
            fullload = true;
        }

        public static void LoadStatus()
        {
            try
            {
                Conquer_Online_Server.Database.Read read;
                uint count;
                uint num2;
                string[] strArray;
                using (read = new Conquer_Online_Server.Database.Read(@"database\JianghuAttributes.txt"))
                {
                    if (read.Reader(true))
                    {
                        count = (uint)read.Count;
                        for (num2 = 0; num2 < count; num2++)
                        {
                            strArray = read.ReadString("").Split(new char[] { ' ' });
                            Atribut atribut = new Atribut
                            {
                                Type = byte.Parse(strArray[1]),
                                Level = byte.Parse(strArray[2]),
                                Power = ushort.Parse(strArray[3])
                            };
                            if (((atribut.Type == 10) || (atribut.Type == 11)) || (atribut.Type == 12))
                            {
                                atribut.Power = (ushort)(atribut.Power * 10);
                            }
                            ushort key = ValueToRoll(atribut.Type, atribut.Level);
                            Attributes.Add(key, atribut);
                        }
                    }
                }
                using (read = new Conquer_Online_Server.Database.Read(@"database\JingHuCultivateStatus.txt"))
                {
                    if (read.Reader(true))
                    {
                        count = (uint)read.Count;
                        for (num2 = 0; num2 < count; num2++)
                        {
                            strArray = read.ReadString("").Split(new char[] { ' ' });
                            byte num4 = byte.Parse(strArray[0]);
                            byte num5 = byte.Parse(strArray[1]);
                            List<byte> list = new List<byte>();
                            for (byte i = 0; i < num5; i = (byte)(i + 1))
                            {
                                list.Add(byte.Parse(strArray[2 + i]));
                            }
                            CultivateStatus.Add(num4, list);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Conquer_Online_Server.Console.WriteLine(exception.ToString());
            }
        }

        public static void SaveJiangHu()
        {
            if (fullload)
            {
                using (Conquer_Online_Server.Database.Write write = new Conquer_Online_Server.Database.Write(@"database\JiangHu.txt"))
                {
                    Conquer_Online_Server.Game.AttJiangHu[] huArray = Conquer_Online_Server.Game.JiangHu.JiangHuClients.Values.ToArray<Conquer_Online_Server.Game.AttJiangHu>();
                    string[] data = new string[Conquer_Online_Server.Game.JiangHu.JiangHuClients.Count];
                    for (uint i = 0; i < Conquer_Online_Server.Game.JiangHu.JiangHuClients.Count; i++)
                    {
                        data[i] = huArray[i].ToString();
                    }
                    write.Add(data, data.Length).Execute(Conquer_Online_Server.Database.Mode.Open);
                }
            }
        }

        public static ushort ValueToRoll(byte typ, byte level)
        {
            return (ushort)(typ + (level * 0x100));
        }

        public class Atribut
        {
            public byte Level;
            public ushort Power;
            public byte Type;
        }
    }
}

