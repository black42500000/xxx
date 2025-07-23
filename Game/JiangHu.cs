namespace Conquer_Online_Server.Game
{
    using Conquer_Online_Server;
    using Conquer_Online_Server.Client;
    using Conquer_Online_Server.Database;
    using Conquer_Online_Server.Network.GamePackets;
    using Conquer_Online_Server.ServerBase;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Net.Mail;
    using System.Net;

    public class JiangHu : Conquer_Online_Server.Game.AttJiangHu
    {
        public static ConcurrentDictionary<uint, Conquer_Online_Server.Game.AttJiangHu> JiangHuClients = new ConcurrentDictionary<uint, Conquer_Online_Server.Game.AttJiangHu>();
        public System.Random Random = new System.Random();
        public DateTime RemoveJiangMod;
        public object sync = new object();
        public DateTime TimerStamp;

        public JiangHu(uint m_UID)
        {
            this.Talent = 3;
            this.Stage = 1;
            this.Star = 1;
            this.FreeCourse = 0x2710;
            this.FreeTimeTodey = 10;
            this.MyOldStar = null;
            this.UID = m_UID;
            DateTime time = new DateTime();
            this.CountDownEnd = time;
            this.StartCountDwon = new DateTime();
            this.TimerStamp = new DateTime();
            this.RemoveJiangMod = new DateTime();
            this.Stagers = new JiangStages[9];
            for (byte i = 0; i < 9; i = (byte)(i + 1))
            {
                this.Stagers[i] = new JiangStages();
            }
            if (!(JiangHuClients.ContainsKey(this.UID) || (this.UID == 0)))
            {
                JiangHuClients.TryAdd(this.UID, this);
            }
            this.OnJiangHu = true;
            this.TimerStamp = DateTime.Now;
        }

        public static bool AllowNameCaracters(string Name)
        {
            if (Name.Contains<char>('['))
            {
                return false;
            }
            if (Name.Contains<char>(']'))
            {
                return false;
            }
            if (Name.Contains("GM"))
            {
                return false;
            }
            if (Name.Contains("PM"))
            {
                return false;
            }
            if ((Name.Contains("!") || Name.Contains("@")) || Name.Contains("#"))
            {
                return false;
            }
            if ((Name.Contains("$") || Name.Contains("%")) || Name.Contains("^"))
            {
                return false;
            }
            if (((Name.Contains("&") || Name.Contains("*")) || Name.Contains("/")) || Name.Contains("|"))
            {
                return false;
            }
            return true;
        }

        public void CreateStatusAtributes(Conquer_Online_Server.Game.Entity client)
        {
            this.Inner_Strength = 0;
            foreach (JiangStages stages in this.Stagers)
            {
                if (stages.Activate)
                {
                    foreach (Conquer_Online_Server.Game.JiangHu.JiangStages.Star star in stages.Stars)
                    {
                        if (star.Activate && (star.UID != 0))
                        {
                            if (!client.Dead)
                            {
                                Conquer_Online_Server.Database.JiangHu.Atribut atribut = Conquer_Online_Server.Database.JiangHu.Attributes[star.UID];
                                this.Inner_Strength = (ushort)(this.Inner_Strength + Conquer_Online_Server.Database.JiangHu.GetStatusPoints(star.Level));
                                this.IncreaseStatus(client, (JiangStages.AtributesType)atribut.Type, atribut.Power);
                                client.Hitpoints += client.MaxHitpoints;
                            }
                        }
                    }
                }
            }
        }

        public void CreateTime()
        {
            this.StartCountDwon = DateTime.Now;
            this.CountDownEnd = DateTime.Now.AddMinutes((double)Conquer_Online_Server.Database.JiangHu.GetMinutesOnTalent(this.Talent));
        }

        public void DecreaseStatus(Conquer_Online_Server.Game.Entity client, JiangStages.AtributesType status, ushort Power)
        {
            switch (status)
            {
                case JiangStages.AtributesType.MaxLife:
                    client.MaxHitpoints -= Power;
                    break;

                case JiangStages.AtributesType.PAttack:
                    client.MaxAttack -= Power;
                    client.MinAttack -= Power;
                    break;

                case JiangStages.AtributesType.MAttack:
                    client.MagicAttack -= Power;
                    break;

                case JiangStages.AtributesType.FinalAttack:
                    client.PhysicalDamageIncrease = (ushort)(client.PhysicalDamageIncrease - Power);
                    break;

                case JiangStages.AtributesType.FinalMagicAttack:
                    client.MagicDamageIncrease = (ushort)(client.MagicDamageIncrease - Power);
                    break;

                case JiangStages.AtributesType.FinalDefense:
                    client.PhysicalDamageDecrease = (ushort)(client.PhysicalDamageDecrease - Power);
                    break;

                case JiangStages.AtributesType.FinalMagicDefense:
                    client.MagicDamageDecrease = (ushort)(client.MagicDamageDecrease - Power);
                    break;

                case JiangStages.AtributesType.CriticalStrike:
                    client.CriticalStrike -= Power;
                    break;

                case JiangStages.AtributesType.SkillCriticalStrike:
                    client.SkillCStrike -= Power;
                    break;

                case JiangStages.AtributesType.Immunity:
                    client.Immunity -= Power;
                    break;

                case JiangStages.AtributesType.Breakthrough:
                    client.Breaktrough = (ushort)(client.Breaktrough - Power);
                    break;

                case JiangStages.AtributesType.Counteraction:
                    client.Counteraction = (ushort)(client.Counteraction - Power);
                    break;

                case JiangStages.AtributesType.MaxMana:
                    client.MaxMana = (ushort)(client.MaxMana - Power);
                    break;
            }
        }

        public bool GetChange(uint value)
        {
            return ((Conquer_Online_Server.ServerBase.Kernel.Random.Next() % 100) < value);
        }

        public void GetKill(Conquer_Online_Server.Client.GameClient attacker, Conquer_Online_Server.Game.JiangHu attacked)
        {
            if ((attacked != null) && this.GetChange(0x23))
            {
                this.Talent = (byte)Math.Min(5, this.Talent + 1);
                attacker.Entity.JiangTalent = this.Talent;
                this.SendInfo(attacker, 5, new string[] { attacker.Entity.UID.ToString(), this.Talent.ToString() });
            }
        }

        public void GetReward(Conquer_Online_Server.Client.GameClient client)
        {
            do
            {
                this.FreeCourse++;
            }
            while ((this.FreeCourse % 0x3e8) != 0);
            this.SendInfo(client, 13, new string[] { this.FreeCourse.ToString(), this.Time.ToString() });
            this.CreateTime();
        }

        public void GetRoll(Conquer_Online_Server.Client.GameClient client, byte mStar, byte mStage, bool Restore = false)
        {
            try
            {

                JiangStages stages = this.Stagers[mStage - 1];
                if (stages.Activate)
                {
                    Conquer_Online_Server.Game.JiangHu.JiangStages.Star star = stages.Stars[mStar - 1];
                    if (star.UID != 0)
                    {
                        this.DecreaseStatus(client.Entity, star.Typ, Conquer_Online_Server.Database.JiangHu.GetPower(star.UID));
                        this.Inner_Strength = (ushort)(this.Inner_Strength - Conquer_Online_Server.Database.JiangHu.GetStatusPoints(star.Level));                
                    }
                    if (!Restore)
                    {
                        OldStar star2 = new OldStar
                        {
                            Stage = mStage,
                            PositionStar = mStar,
                            Star = star
                        };
                        this.MyOldStar = star2;
                    }
                    star.Level = this.GetStatusLevel();
                    star.Typ = (JiangStages.AtributesType)this.Random.Next(1, 0x10);
                    do
                    {
                        star.Typ = (JiangStages.AtributesType)this.Random.Next(1, 0x10);
                    }
                    while (Conquer_Online_Server.Database.JiangHu.CultivateStatus[star.Level].Contains((byte)star.Typ));
                    if (!Restore)
                    {
                        star.UID = this.ValueToRoll(star.Typ, star.Level);
                        if (!star.Activate)
                        {
                            this.Star = (byte)(this.Star + 1);
                            star.Activate = true;
                        }
                        client.Send(new Conquer_Online_Server.Network.GamePackets.JiangHuUpdate { Atribute = star.UID, FreeCourse = this.FreeCourse, Stage = mStage, Star = mStar, FreeTimeTodeyUsed = (byte)this.FreeTimeTodeyUsed, RoundBuyPoints = this.RoundBuyPoints }.ToArray());
                        this.IncreaseStatus(client.Entity, star.Typ, Conquer_Online_Server.Database.JiangHu.GetPower(star.UID));
                        this.Inner_Strength = (ushort)(this.Inner_Strength + Conquer_Online_Server.Database.JiangHu.GetStatusPoints(star.Level));
                    }
                    else
                    {
                        star = this.MyOldStar.Star;
                        this.IncreaseStatus(client.Entity, star.Typ, Conquer_Online_Server.Database.JiangHu.GetPower(star.UID));
                        this.Inner_Strength = (ushort)(this.Inner_Strength + Conquer_Online_Server.Database.JiangHu.GetStatusPoints(star.Level));
                    }
                    if ((mStage < 9) && !((mStar != 9) || this.Stagers[mStage].Activate))
                    {
                        this.Stage = (byte)(this.Stage + 1);
                        this.Stagers[mStage].Activate = true;
                        this.SendInfo(client, 12, new string[] { this.Stage.ToString() });
                    }
                    JiangHuRanking.UpdateRank(this);
                }

            }
            catch (Exception exception)
            {
                Conquer_Online_Server.Console.WriteLine(exception.ToString());
            }
        }

        public void Cheat(Conquer_Online_Server.Client.GameClient client, byte mStar, byte mStage, byte type, byte power, bool Restore = false)
        {
            JiangStages stages = this.Stagers[mStage - 1];
            if (stages.Activate)
            {
                Conquer_Online_Server.Game.JiangHu.JiangStages.Star star = stages.Stars[mStar - 1];
                if (star.UID != 0)
                {
                    this.DecreaseStatus(client.Entity, (JiangStages.AtributesType)type, power);
                    this.Inner_Strength = (ushort)(this.Inner_Strength - power);
                }
                if (!Restore)
                {
                    OldStar star2 = new OldStar
                    {
                        Stage = mStage,
                        PositionStar = mStar,
                        Star = star
                    };
                    this.MyOldStar = star2;
                }
                star.Level = power;
                star.Typ = (JiangStages.AtributesType)type;
                do
                {
                    star.Typ = (JiangStages.AtributesType)type;
                }
                while (Conquer_Online_Server.Database.JiangHu.CultivateStatus[power].Contains(type));
                if (!Restore)
                {
                    star.UID = this.ValueToRoll((JiangStages.AtributesType)type, power);
                    if (!star.Activate)
                    {
                        this.Star = (byte)(this.Star + 1);
                        star.Activate = true;
                    }
                    client.Send(new Conquer_Online_Server.Network.GamePackets.JiangHuUpdate { Atribute = star.UID, FreeCourse = this.FreeCourse, Stage = mStage, Star = mStar, FreeTimeTodeyUsed = (byte)this.FreeTimeTodeyUsed, RoundBuyPoints = this.RoundBuyPoints }.ToArray());
                    this.IncreaseStatus(client.Entity, star.Typ, Conquer_Online_Server.Database.JiangHu.GetPower(star.UID));
                    this.Inner_Strength = (ushort)(this.Inner_Strength + power);
                }
                else
                {//
                    star = this.MyOldStar.Star;
                    this.IncreaseStatus(client.Entity, star.Typ, power);
                    this.Inner_Strength = (ushort)(this.Inner_Strength + power);
                }
                if ((mStage < 9) || this.Stagers[mStage].Activate)
                {
                    this.Stage = (byte)(this.Stage + 1);
                    this.Stagers[mStage].Activate = true;
                    this.SendInfo(client, 12, new string[] { this.Stage.ToString() });
                }
                JiangHuRanking.UpdateRank(this);
            }

        }

        public byte GetStatusLevel()
        {
            byte num = (byte)this.Random.Next(1, this.Random.Next(1, 15));
            if (num > 6)
            {
                num = 6;
            }
            return num;
        }

        public byte GetValueLevel(ushort val)
        {
            return (byte)((val - ((ushort)(val % 0x100))) / 0x100);
        }

        public JiangStages.AtributesType GetValueType(uint val)
        {
            return (JiangStages.AtributesType)(val % 0x100);
        }

        public void IncreaseStatus(Conquer_Online_Server.Game.Entity client, JiangStages.AtributesType status, ushort Power)
        {
            switch (status)
            {
                case JiangStages.AtributesType.MaxLife:
                    client.MaxHitpoints += Power;
                    break;

                case JiangStages.AtributesType.PAttack:
                    client.MaxAttack += Power;
                    client.MinAttack += Power;
                    break;

                case JiangStages.AtributesType.MAttack:
                    client.MagicAttack += Power;
                    break;

                case JiangStages.AtributesType.FinalAttack:
                    client.PhysicalDamageIncrease = (ushort)(client.PhysicalDamageIncrease + Power);
                    break;

                case JiangStages.AtributesType.FinalMagicAttack:
                    client.MagicDamageIncrease = (ushort)(client.MagicDamageIncrease + Power);
                    break;

                case JiangStages.AtributesType.FinalDefense:
                    client.PhysicalDamageDecrease = (ushort)(client.PhysicalDamageDecrease + Power);
                    break;

                case JiangStages.AtributesType.FinalMagicDefense:
                    client.MagicDamageDecrease = (ushort)(client.MagicDamageDecrease + Power);
                    break;

                case JiangStages.AtributesType.CriticalStrike:
                    client.CriticalStrike += Power;
                    break;

                case JiangStages.AtributesType.SkillCriticalStrike:
                    client.SkillCStrike += Power;
                    break;

                case JiangStages.AtributesType.Immunity:
                    client.Immunity += Power;
                    break;

                case JiangStages.AtributesType.Breakthrough:
                    client.Breaktrough = (ushort)(client.Breaktrough + Power);
                    break;

                case JiangStages.AtributesType.Counteraction:
                    client.Counteraction = (ushort)(client.Counteraction + Power);
                    break;

                case JiangStages.AtributesType.MaxMana:
                    client.MaxMana = (ushort)(client.MaxMana + Power);
                    break;
            }
        }

        public bool InTwinCastle(Conquer_Online_Server.Game.Entity location)
        {
            ushort x = location.X;
            ushort y = location.Y;
            if (location.MapID != 0x3ea)
            {
                return false;
            }
            return (((((x <= 0x1e5) && (x >= 0x177)) && (y <= 0x195)) && (y >= 210)) || (((((x >= 0x1e7) && (x <= 0x1f3)) && (y <= 370)) && (y >= 0x14c)) || (((((x >= 0x197) && (x <= 0x1ce)) && (y >= 0x194)) && (y <= 0x1a8)) || ((((x >= 0x15d) && (x <= 0x177)) && (y >= 0x138)) && (y <= 0x17d)))));
        }

        public void Load(string Line)
        {
            try
            {
                if (((Line != null) && (Line != "")) && Line.Contains<char>('#'))
                {
                    string[] strArray = Line.Split(new char[] { '#' });
                    this.UID = uint.Parse(strArray[0]);
                    this.OwnName = strArray[1];
                    this.JiangName = strArray[2];
                    this.Level = byte.Parse(strArray[3]);
                    this.Talent = byte.Parse(strArray[4]);
                    this.Stage = byte.Parse(strArray[5]);
                    this.Star = byte.Parse(strArray[6]);
                    this.FreeTimeTodey = byte.Parse(strArray[7]);
                    this.OnJiangHu = byte.Parse(strArray[8]) == 1;
                    this.FreeCourse = uint.Parse(strArray[9]);
                    this.StartCountDwon = DateTime.Now;
                    this.TimerStamp = DateTime.Now;
                    this.CountDownEnd = DateTime.Now.AddSeconds((double)uint.Parse(strArray[10]));
                    this.RoundBuyPoints = ushort.Parse(strArray[11]);
                    ushort index = 12;
                    foreach (JiangStages stages in this.Stagers)
                    {
                        stages.Activate = byte.Parse(strArray[index]) == 1;
                        index = (ushort)(index + 1);
                        foreach (Conquer_Online_Server.Game.JiangHu.JiangStages.Star star in stages.Stars)
                        {
                            star.Activate = byte.Parse(strArray[index]) == 1;
                            index = (ushort)(index + 1);
                            star.UID = ushort.Parse(strArray[index]);
                            index = (ushort)(index + 1);
                            if (star.Activate)
                            {
                                star.Typ = this.GetValueType(star.UID);
                                star.Level = this.GetValueLevel(star.UID);
                                this.Inner_Strength = (ushort)(this.Inner_Strength + Conquer_Online_Server.Database.JiangHu.GetStatusPoints(star.Level));
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Conquer_Online_Server.Console.WriteLine(exception.ToString());
            }
        }

        public void OnloginClient(Conquer_Online_Server.Client.GameClient client)
        {
            this.SendStatus(client, client);
            this.SendStatusMode(client);
            this.TimerStamp = DateTime.Now;
        }

        public void ResetDay(Conquer_Online_Server.Client.GameClient client)
        {
            this.RoundBuyPoints = 0;
            this.FreeTimeTodeyUsed = 0;
            this.SendStatus(client, client);
        }

        public bool SameCourseStage(uint first, uint last)
        {
            if (first > 0x186a0)
            {
                return (first.ToString()[2] == last.ToString()[2]);
            }
            return (first.ToString()[1] == last.ToString()[1]);
        }

        public void SendInfo(Conquer_Online_Server.Client.GameClient client, byte mode = 1, params string[] data)
        {
            lock (this.sync)
            {
                try
                {
                    Conquer_Online_Server.Network.GamePackets.JiangHu hu = new Conquer_Online_Server.Network.GamePackets.JiangHu
                    {
                        Mode = mode,
                        Texts = new List<string>(data)
                    };
                    hu.CreateArray();
                    hu.Send(client);
                }
                catch (Exception exception)
                {
                    Conquer_Online_Server.Console.WriteLine(exception.ToString());
                }
            }
        }

        public void SendStatus(Conquer_Online_Server.Client.GameClient client, Conquer_Online_Server.Client.GameClient Attacked)
        {
            try
            {
                IEnumerable<JiangStages> source = from p in this.Stagers
                                                  where p.Activate
                                                  select p;
                Conquer_Online_Server.Network.GamePackets.JiangHuStatus status = new Conquer_Online_Server.Network.GamePackets.JiangHuStatus((byte)source.Count<JiangStages>())
                {
                    Name = this.JiangName,
                    FreeTimeTodey = 10,
                    Talent = this.Talent,
                    Stage = this.Stage,
                    RoundBuyPoints = this.RoundBuyPoints,
                    FreeTimeTodeyUsed = (byte)this.FreeTimeTodeyUsed,
                    StudyPoints = Attacked.Entity.SubClasses.StudyPoints
                };
                if (client.Entity.UID != Attacked.Entity.UID)
                {
                    status.Timer = 0xd1d401;
                }
                else
                {
                    status.Timer = 0xec8600;
                }
                status.Apprend(source.ToArray<JiangStages>());
                client.Send(status.ToArray());
            }
            catch (Exception exception)
            {
                Conquer_Online_Server.Console.WriteLine(exception.ToString());
            }
        }

        public void SendStatusMode(Conquer_Online_Server.Client.GameClient client)
        {
            client.Entity.JiangTalent = this.Talent;
            client.Entity.JiangActive = this.OnJiangHu;
            this.SendInfo(client, 7, new string[] { client.Entity.UID.ToString(), this.Talent.ToString(), this.OnJiangHu ? "1" : "2" });
            if (this.OnJiangHu)
            {
                this.RemoveJiangMod = DateTime.Now;
            }
        }

        public void TheadTime(Conquer_Online_Server.Client.GameClient client)
        {
            try
            {
                if (((client != null) && (client.Entity != null)) && (client.Entity.FullyLoaded && (DateTime.Now > this.TimerStamp.AddMinutes(1.0))))
                {
                    if (((client.Entity.PKMode != Conquer_Online_Server.Game.Enums.PKMode.Jiang) && this.OnJiangHu) && (DateTime.Now >= this.RemoveJiangMod.AddMinutes(1.0)))
                    {
                        this.OnJiangHu = false;
                        this.SendStatusMode(client);
                        client.SendScreen(client.Entity.SpawnPacket, false);
                    }
                    if (client.Entity.PKMode == Conquer_Online_Server.Game.Enums.PKMode.Jiang)
                    {
                        this.OnJiangHu = true;
                        this.RemoveJiangMod = DateTime.Now;
                    }
                    if ((this.FreeCourse < 0x989680) && (this.FreeTimeTodeyUsed < 10))
                    {
                        if (this.InTwinCastle(client.Entity))
                        {
                            this.StartCountDwon = this.StartCountDwon.AddMinutes(Conquer_Online_Server.Database.JiangHu.GetMinutesInCastle(this.Talent));
                            this.FreeCourse += Conquer_Online_Server.Database.JiangHu.GetFreeCourseInCastle(this.Talent);
                        }
                        else
                        {
                            this.FreeCourse += Conquer_Online_Server.Database.JiangHu.GetFreeCourse(this.Talent);
                            this.StartCountDwon = this.StartCountDwon.AddMinutes(1.0);
                        }
                        if (this.StartCountDwon > this.CountDownEnd)
                        {
                            this.GetReward(client);
                        }
                        this.SendInfo(client, 13, new string[] { this.FreeCourse.ToString(), this.Time.ToString() });
                        if ((this.FreeCourse % 0x2710) == 0)
                        {
                            this.GetReward(client);
                        }
                    }
                    else
                    {
                        this.FreeCourse = 0x989680;
                    }
                    this.TimerStamp = DateTime.Now;
                }
            }
            catch (Exception exception)
            {
                Conquer_Online_Server.Console.WriteLine(exception.ToString());
            }
        }

        public override string ToString()
        {
            if (this.StartCountDwon.Ticks > this.CountDownEnd.Ticks)
            {
                this.CreateTime();
            }
            uint num = (uint)((this.CountDownEnd.Ticks - this.StartCountDwon.Ticks) / 0x989680L);
            StringBuilder builder = new StringBuilder();
            builder.Append(string.Concat(new object[] { 
                this.UID, "#", this.OwnName, "#", this.JiangName, "#", this.Level, "#", this.Talent, "#", this.Stage, "#", this.Star, "#", this.FreeTimeTodeyUsed, "#", 
                this.OnJiangHu ? ((byte) 0) : ((byte) 1), "#", this.FreeCourse, "#", num, "#", this.RoundBuyPoints, "#"
             }));
            foreach (JiangStages stages in this.Stagers)
            {
                builder.Append(stages.ToString());
            }
            return builder.ToString();
        }

        public void UpdateStundyPoints(Conquer_Online_Server.Client.GameClient client, ushort amount)
        {
            client.Entity.SubClasses.StudyPoints = (ushort)(client.Entity.SubClasses.StudyPoints + amount);
            client.Send(new Conquer_Online_Server.Network.GamePackets.SubClassShowFull(true) { ID = 8, Study = client.Entity.SubClasses.StudyPoints, StudyReceive = amount }.ToArray());
            Conquer_Online_Server.Network.GamePackets._String str = new Conquer_Online_Server.Network.GamePackets._String(true)
            {
                Type = 10,
                UID = client.Entity.UID
            };
            str.Texts.Add("zf2-e300");
            client.SendScreen(str.ToArray(), true);
        }

        public ushort ValueToRoll(JiangStages.AtributesType status, byte level)
        {
            return (ushort)(((ushort)status) + (level * 0x100));
        }

        public DateTime CountDownEnd { get; set; }

        public string JiangName { get; set; }

        public uint FreeCourse { get; set; }

        public uint FreeTimeTodey { get; set; }

        public uint FreeTimeTodeyUsed { get; set; }

        public ushort Inner_Strength { get; set; }

        public byte Level { get; set; }

        public OldStar MyOldStar { get; set; }

        public bool OnJiangHu { get; set; }

        public string OwnName { get; set; }

        public byte Rank { get; set; }

        public ushort RoundBuyPoints { get; set; }

        public byte Stage { get; set; }

        public JiangStages[] Stagers { get; set; }

        public byte Star { get; set; }

        public DateTime StartCountDwon { get; set; }

        public byte Talent { get; set; }

        public uint Time
        {
            get
            {
                DateTime time = new DateTime(0x7b2, 1, 1);
                TimeSpan span = (TimeSpan)(this.CountDownEnd - time.ToLocalTime());
                return (uint)span.TotalSeconds;
            }
        }

        public uint UID { get; set; }

        public static class JiangHuRanking
        {
            private static object SyncRoot = new object();
            private static ConcurrentDictionary<uint, Conquer_Online_Server.Game.AttJiangHu> TopRank = new ConcurrentDictionary<uint, Conquer_Online_Server.Game.AttJiangHu>();
            public static Conquer_Online_Server.Game.AttJiangHu[] TopRank100 = null;

            private static void CalculateRanks()
            {
                foreach (Conquer_Online_Server.Game.AttJiangHu hu in TopRank.Values)
                {
                    hu.Rank = 0;
                }
                IOrderedEnumerable<Conquer_Online_Server.Game.AttJiangHu> enumerable = from jiang in TopRank.Values.ToArray<Conquer_Online_Server.Game.AttJiangHu>()
                                                                                orderby jiang.Inner_Strength descending
                                                                                select jiang;
                List<Conquer_Online_Server.Game.AttJiangHu> list = new List<Conquer_Online_Server.Game.AttJiangHu>();
                byte num = 1;
                foreach (Conquer_Online_Server.Game.AttJiangHu hu in enumerable)
                {
                    if (num == 0x65)
                    {
                        break;
                    }
                    hu.Rank = num;
                    list.Add(hu);
                    num = (byte)(num + 1);
                }
                TopRank100 = list.ToArray();
                TopRank = new ConcurrentDictionary<uint, Conquer_Online_Server.Game.AttJiangHu>();
                foreach (Conquer_Online_Server.Game.AttJiangHu hu in list)
                {
                    TopRank.TryAdd(hu.UID, hu);
                }
                list = null;
            }

            public static void UpdateRank(Conquer_Online_Server.Game.AttJiangHu jiang)
            {
                lock (SyncRoot)
                {
                    if (!TopRank.ContainsKey(jiang.UID))
                    {
                        TopRank.TryAdd(jiang.UID, jiang);
                    }
                    CalculateRanks();
                }
            }
        }

        public class JiangStages
        {
            public bool Activate = false;
            public Star[] Stars = new Star[9];

            public JiangStages()
            {
                for (byte i = 0; i < 9; i = (byte)(i + 1))
                {
                    this.Stars[i] = new Star();
                }
            }

            public bool ContainAtribut(AtributesType typ)
            {
                foreach (Star star in this.Stars)
                {
                    if (star.Typ == typ)
                    {
                        return true;
                    }
                }
                return false;
            }

            public override string ToString()
            {
                StringBuilder builder = new StringBuilder();
                builder.Append((this.Activate ? ((byte)1) : ((byte)0)) + "#");
                foreach (Star star in this.Stars)
                {
                    builder.Append(star.ToString());
                }
                return builder.ToString();
            }

            public enum AtributesType
            {
                None,
                MaxLife,
                PAttack,
                MAttack,
                PDefense,
                Mdefense,
                FinalAttack,
                FinalMagicAttack,
                FinalDefense,
                FinalMagicDefense,
                CriticalStrike,
                SkillCriticalStrike,
                Immunity,
                Breakthrough,
                Counteraction,
                MaxMana
            }

            public class Star
            {
                public bool Activate = false;
                public byte Level;
                public Conquer_Online_Server.Game.JiangHu.JiangStages.AtributesType Typ;
                public ushort UID;

                public override string ToString()
                {
                    StringBuilder builder = new StringBuilder();
                    builder.Append(string.Concat(new object[] { this.Activate ? ((byte)1) : ((byte)0), "#", this.UID, "#" }));
                    return builder.ToString();
                }
            }
        }

        public class OldStar
        {
            public byte PositionStar;
            public byte Stage;
            public Conquer_Online_Server.Game.JiangHu.JiangStages.Star Star;
        }
    }
}

