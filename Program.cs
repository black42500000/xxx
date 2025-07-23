using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Conquer_Online_Server.Network;
using Conquer_Online_Server.Database;
using Conquer_Online_Server.Network.Sockets;
using Conquer_Online_Server.Network.AuthPackets;
using Conquer_Online_Server.Game.ConquerStructures.Society;
using Conquer_Online_Server.Game;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using Conquer_Online_Server.Interfaces;
using System.Text;
using Conquer_Online_Server.Network.GamePackets;

namespace Conquer_Online_Server
{
    class Program
    {
        public static void WriteError(Exception ex)
        {
            Console.WriteLine(ex.ToString() + " (Report this Shit to Mr.Bahaa)");
        }
       
        public static void WriteLine(Exception Exc)
        {
            try
            {
                Console.WriteLine(Exc.ToString());

            }
            catch { }
        }
        
       
        public static void WriteLine(string Line)
        {
            try
            {
                Console.WriteLine(Line);
            }
            catch { }
        }

        public static void WriteLine()
        {
            try
            {
                Console.WriteLine("");
            }
            catch { }

        }
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        public static int PlayerCap = 4000;
        public static ServerSocket AuthServer;
        public static bool CpuUsageTimer = true;
        public static MemoryCompressor MCompressor = new MemoryCompressor();
        public static int CpuUse = 0;
        public static ServerSocket GameServer;
        public static Counter EntityUID;
        public static string DBName = "";
        public static string DBUser = "";
        public static string DBPass = "";
        public static string GameIP;
        public static long MaxOn = 0;
        public static long SkillPkRank = 0;
        public static long TeamRank = 0;
        public static Encoding Encoding = ASCIIEncoding.Default;
        public static DayOfWeek Today;
        public static long WeatherType = 0;
        public static ushort GamePort;
        public static uint ScreenColor = 0;
        public static uint DragonSwing = 0;
        public static long Carnaval = 0;
        public static long Carnaval2 = 0;
        public static long Carnaval3 = 0;
        public static bool ServerRrestart = false;
        public static ushort AuthPort;
        public static DateTime StartDate;        
        public static DateTime RestartDate = DateTime.Now.AddHours(24);
        public static World World;
        public static Client.GameClient[] GamePool = new Client.GameClient[0];
        public static VariableVault Vars;
        public static Client.GameClient[] Values = null;
        public static int RandomSeed = 0;
        static void Main(string[] args)
        {
            Time32 Start = Time32.Now;
            RandomSeed = Convert.ToInt32(DateTime.Now.Ticks.ToString().Remove(DateTime.Now.Ticks.ToString().Length / 2));
            Kernel.Random = new FastRandom(RandomSeed);
            StartDate = DateTime.Now;
            Console.Title = "Mr Bahaa V17 Source Please Wait....";
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            IntPtr hWnd = FindWindow(null, Console.Title);
            Program.WriteLine("|-------------------------------------------------|");
            Program.WriteLine("|--------Welcome To Mr Bahaa V17 Source-----------|");
            Program.WriteLine("|--------Source In Update Of 6005-----------------|");
            Program.WriteLine("|--------Yahoo:    has.bahaa@yahoo.com--------|");
            Program.WriteLine("|--Facebook Page.. FB/AnaElBahaaYala--|");
            Program.WriteLine("|-------------------------------------------------|");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Load server configuration!");
            string ConfigFileName = "configuration.ini";
            IniFile IniFile = new IniFile(ConfigFileName);
            GameIP = IniFile.ReadString("configuration", "IP");
            string SourceOwner = IniFile.ReadString("configuration", "SourceOwner");
            string Forum = IniFile.ReadString("configuration", "Forum");
            GamePort = IniFile.ReadUInt16("configuration", "GamePort");
            AuthPort = IniFile.ReadUInt16("configuration", "AuthPort");
            string ServerName = IniFile.ReadString("configuration", "ServerName");
            Database.DataHolder.CreateConnection(IniFile.ReadString("MySql", "Username"), IniFile.ReadString("MySql", "Password"), IniFile.ReadString("MySql", "Database"), IniFile.ReadString("MySql", "Host"));
            Conquer_Online_Server.Database.JiangHu.LoadJiangHu();
            Conquer_Online_Server.Database.JiangHu.LoadStatus();
            EntityUID = new Counter(0);
            using (MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT))
            {
                cmd.Select("configuration").Where("Server", ServerName);
                using (MySqlReader r = new MySqlReader(cmd))
                {
                    if (r.Read())
                    {
                        EntityUID = new Counter(r.ReadUInt32("EntityID"));
                        Game.ConquerStructures.Society.Guild.GuildCounter = new Conquer_Online_Server.Counter(r.ReadUInt32("GuildID"));
                        Network.GamePackets.ConquerItem.ItemUID = new Conquer_Online_Server.Counter(r.ReadUInt32("ItemUID"));
                        Constants.ExtraExperienceRate = r.ReadUInt32("ExperienceRate");
                        Constants.ExtraSpellRate = r.ReadUInt32("ProficiencyExperienceRate");
                        Constants.ExtraProficiencyRate = r.ReadUInt32("SpellExperienceRate");
                        Constants.MoneyDropRate = r.ReadUInt32("MoneyDropRate");
                        Constants.MoneyDropMultiple = r.ReadUInt32("MoneyDropMultiple");
                        Constants.ConquerPointsDropRate = r.ReadUInt32("ConquerPointsDropRate");
                        Constants.ConquerPointsDropMultiple = r.ReadUInt32("ConquerPointsDropMultiple");
                        Constants.ItemDropRate = r.ReadUInt32("ItemDropRate");
                        Constants.ItemDropQualityRates = r.ReadString("ItemDropQualityString").Split('~');
                        Constants.WebAccExt = r.ReadString("AccountWebExt");
                        Constants.WebVoteExt = r.ReadString("VoteWebExt");
                        Constants.WebDonateExt = r.ReadString("DonateWebExt");
                        Constants.ServerWebsite = r.ReadString("ServerWebsite");
                        PlayerCap = r.ReadInt32("PlayerCap");
                        Database.EntityVariableTable.Load(0, out Vars);
                        Console.WriteLine("Initializing database.");
                        World = new World();
                        World.Init();
                        Database.ConquerItemTable.ClearNulledItems();
                        Database.NameChange.UpdateNames();
                        Database.ConquerItemInformation.Load();
                        Database.DataHolder.ReadStats();
                        Database.MonsterInformation.Load();
                        Database.IPBan.Load();
                        Database.SpellTable.Load();
                        Database.ShopFile.Load();
                        Database.HonorShop.Load();
                        Database.RacePointShop.Load();
                        Database.MapsTable.Load();
                        Database.NobilityTable.Load();
                        Database.EntityTable.LoadPlayersVots();
                        Database.ArenaTable.Load();
                        Database.TeamArenaTable.Load();
                        Database.ChampionTable.Load();
                        Database.GuildTable.Load();
                        Database.LotteryTable.Load();
                        Database.DROP_SOULS.LoadDrops();
                        Database.ChiTable.LoadAllChi();
                        Database.PoketTables.LoadTables();
                        Kernel.QuizShow = new Game.ConquerStructures.QuizShow();
                        Refinery.Load();
                        Map.CreateTimerFactories();
                        Database.DMaps.Load();
                        Game.Screen.CreateTimerFactories();
                        rates.LoadRates();
                        Flags.LoadFlags();
                        HelpDesk.MrBahaaLoading();
                        Messagess.Load();
                        GamePool = new Client.GameClient[0];
                        new Game.Map(1002, Database.DMaps.MapPaths[1002]);
                        new Game.Map(1038, Database.DMaps.MapPaths[1038]);
                        new Game.Map(2071, Database.DMaps.MapPaths[2071]);
                        Game.GuildWar.Initiate();
                        new Game.Map(CrossServer.mapid, Database.DMaps.MapPaths[CrossServer.mapid]);
                        Game.CrossServer.Initiate();
                        CrossServer.Load();
                        new Game.Map(1509, Database.DMaps.MapPaths[1509]);
                        new Game.Map(10002, 2021, Database.DMaps.MapPaths[2021]);
                        new Game.Map(8883, 1004, Database.DMaps.MapPaths[1004]);
                        Constants.PKFreeMaps.Add(8883);
                        Constants.PKFreeMaps.Add(CrossServer.mapid);
                        Game.ClanWar.Initiate();
                        Console.WriteLine("Guild war initializated.");
                        Game.EliteGuildWar.EliteGwint();
                        Console.WriteLine("Elite Guild war initializated.");
                        Console.WriteLine("Loading Game Clans.");
                        Clan.LoadClans();
                        Game.Tournaments.SkillTournament.LoadTop8();
                        Console.WriteLine("SkillPk Winner Loaded.");
                        Game.Tournaments.TeamTournament.LoadTop8();
                        Console.WriteLine("TeamPk Winner Loaded.");
                        Console.WriteLine("House Table Loaded.");
                        Database.ReincarnationTable.Load();
                        Console.WriteLine("Reincarnat Table Loaded.");
                        Console.WriteLine("Flower Table Loaded.");
                        new Map(2072, DMaps.MapPaths[1002]);
                        Game.PoleTwin.PoleTwinIni();
                        Console.WriteLine("PoleTwin initializated By Bahaa.");
                        Network.Cryptography.AuthCryptography.PrepareAuthCryptography();
                        World.CreateTournaments();
                        new MySqlCommand(MySqlCommandType.UPDATE).Update("entities").Set("Online", 0).Execute();
                        Console.WriteLine("Initializing sockets.");
                        AuthServer = new ServerSocket();
                        AuthServer.OnClientConnect += AuthServer_OnClientConnect;
                        AuthServer.OnClientReceive += AuthServer_OnClientReceive;
                        AuthServer.OnClientDisconnect += AuthServer_OnClientDisconnect;
                        AuthServer.Enable(AuthPort, "0.0.0.0");
                        GameServer = new ServerSocket();
                        Console.Title = "[" + ServerName + "] Server. Start time: " + Program.StartDate.ToString("dd/MM/yyyy hh:mm") + ". Players online: " + Kernel.GamePool.Count + "/" + Program.PlayerCap + " Max Online: " + Program.MaxOn + " ";
                        GameServer.OnClientConnect += GameServer_OnClientConnect;
                        GameServer.OnClientReceive += GameServer_OnClientReceive;
                        GameServer.OnClientDisconnect += GameServer_OnClientDisconnect;
                        GameServer.Enable(GamePort, "0.0.0.0");
                        Console.WriteLine("Auth server online.");
                        Console.WriteLine("Game server online.");
                        Program.WriteLine("|-------------------------------------------------|");
                        Program.WriteLine("|--------------Source Is Full Working Now---------|");
                        Program.WriteLine("|----------------GoodLuck From Mr Bahaa------------|");
                        Program.WriteLine("|-------------------------------------------------|");
                        Console.WriteLine("Server loaded in " + (Time32.Now - Start) + " milliseconds.");
                        Program.MCompressor.Optimize();
                        Program.ServerRrestart = false;
                        Kernel.Online = true;
                        GC.Collect();
                        WorkConsole();
                    }
                }
            }
        }



        #region Exceptions & Logs
        public static void AddVendorLog(String vendor, string buying, string moneyamount, ConquerItem Item)
        {
            String folderN = DateTime.Now.Year + "-" + DateTime.Now.Month,
                Path = "gmlogs\\VendorLogs\\",
                NewPath = System.IO.Path.Combine(Path, folderN);
            if (!File.Exists(NewPath + folderN))
            {
                System.IO.Directory.CreateDirectory(System.IO.Path.Combine(Path, folderN));
            }
            if (!File.Exists(NewPath + "\\" + DateTime.Now.Day + ".txt"))
            {
                using (System.IO.FileStream fs = System.IO.File.Create(NewPath + "\\" + DateTime.Now.Day + ".txt"))
                {
                    fs.Close();
                }
            }

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(NewPath + "\\" + DateTime.Now.Day + ".txt", true))
            {
                file.WriteLine("------------------------------------------------------------------------------------");
                file.WriteLine("{0} HAS BOUGHT AN ITEM : {2} FROM {1} SHOP - for {3}", vendor, buying, Item.ToLog(), moneyamount);
                file.WriteLine("------------------------------------------------------------------------------------");
            }
        }
        public static void AddGMCommand(string gm, string commandStr)
        {
            String folderN = DateTime.Now.Year + "-" + DateTime.Now.Month,
                Path = "gmlogs\\GMCommandsLog\\",
                NewPath = System.IO.Path.Combine(Path, folderN);
            if (!File.Exists(NewPath + folderN))
            {
                System.IO.Directory.CreateDirectory(System.IO.Path.Combine(Path, folderN));
            }
            if (!File.Exists(NewPath + "\\" + DateTime.Now.Day + ".txt"))
            {
                using (System.IO.FileStream fs = System.IO.File.Create(NewPath + "\\" + DateTime.Now.Day + ".txt"))
                {
                    fs.Close();
                }
            }

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(NewPath + "\\" + DateTime.Now.Day + ".txt", true))
            {
                file.WriteLine(gm + commandStr);
            }
        }
        public static void AddDropLog(String Name, ConquerItem Item)
        {
            String folderN = DateTime.Now.Year + "-" + DateTime.Now.Month,
                Path = "gmlogs\\droplogs\\",
                NewPath = System.IO.Path.Combine(Path, folderN);
            if (!File.Exists(NewPath + folderN))
            {
                System.IO.Directory.CreateDirectory(System.IO.Path.Combine(Path, folderN));
            }
            string path = NewPath + "\\" + DateTime.Now.Day + ".txt";
            if (!File.Exists(path)) File.AppendAllText(path, "");

            string text = "------------------------------------------------------------------------------------"
                + Environment.NewLine + string.Format("Player {0} HAS DROPPED AN ITEM : {1} -", Name, Item.ToLog())
                + Environment.NewLine + "------------------------------------------------------------------------------------";
            File.AppendAllText(path, text);
        }
        public static void AddTradeLog(Conquer_Online_Server.Game.ConquerStructures.Trade first, String firstN, Conquer_Online_Server.Game.ConquerStructures.Trade second, String secondN)
        {
            String folderN = DateTime.Now.Year + "-" + DateTime.Now.Month,
                Path = "gmlogs\\tradelogs\\",
                NewPath = System.IO.Path.Combine(Path, folderN);
            if (!File.Exists(NewPath + folderN))
            {
                System.IO.Directory.CreateDirectory(System.IO.Path.Combine(Path, folderN));
            }
            if (!File.Exists(NewPath + "\\" + DateTime.Now.Day + ".txt"))
            {
                using (System.IO.FileStream fs = System.IO.File.Create(NewPath + "\\" + DateTime.Now.Day + ".txt"))
                {
                    fs.Close();
                }
            }

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(NewPath + "\\" + DateTime.Now.Day + ".txt", true))
            {
                file.WriteLine("************************************************************************************");
                file.WriteLine("First Person TradeLog ( {0} ) -", firstN);
                file.WriteLine("Gold Traded: " + first.Money);
                file.WriteLine("Conquer Points Traded: " + first.ConquerPoints);

                for (int i = 0; i < first.Items.Count; i++)
                {
                    file.WriteLine("------------------------------------------------------------------------------------");
                    file.WriteLine("Item : " + first.Items[i].ToLog());
                    file.WriteLine("------------------------------------------------------------------------------------");
                }
                file.WriteLine("Second Person TradeLog ( {0} ) -", secondN);
                file.WriteLine("Gold Traded: " + second.Money);
                file.WriteLine("Conquer Points Traded: " + second.ConquerPoints);

                for (int i = 0; i < second.Items.Count; i++)
                {
                    file.WriteLine("------------------------------------------------------------------------------------");
                    file.WriteLine("Item : " + second.Items[i].ToLog());
                    file.WriteLine("------------------------------------------------------------------------------------");
                }
                file.WriteLine("************************************************************************************");
            }
        }
        public static void AddWarLog(string War, string CPs, string name)
        {
            String folderN = DateTime.Now.Year + "-" + DateTime.Now.Month,
                    Path = "gmlogs\\Warlogs\\",
                    NewPath = System.IO.Path.Combine(Path, folderN);
            if (!File.Exists(NewPath + folderN))
            {
                System.IO.Directory.CreateDirectory(System.IO.Path.Combine(Path, folderN));
            }
            if (!File.Exists(NewPath + "\\" + DateTime.Now.Day + ".txt"))
            {
                using (System.IO.FileStream fs = System.IO.File.Create(NewPath + "\\" + DateTime.Now.Day + ".txt"))
                {
                    fs.Close();
                }
            }

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(NewPath + "\\" + DateTime.Now.Day + ".txt", true))
            {
                file.WriteLine(name + " got " + CPs + " CPs from the [" + War + "] as prize at " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second);
            }
        }
        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            SaveException(e.Exception);
        }

        public static void SaveException(Exception e)
        {
            if (e.TargetSite.Name == "ThrowInvalidOperationException")
                return;
            if (e.Message.Contains("String reference not set"))
                return;

            //Console.WriteLine(e);

            var dt = DateTime.Now;
            string date = dt.Month + "-" + dt.Day + "//";

            if (!Directory.Exists(Application.StartupPath + Constants.UnhandledExceptionsPath))
                Directory.CreateDirectory(Application.StartupPath + "\\" + Constants.UnhandledExceptionsPath);
            if (!Directory.Exists(Application.StartupPath + "\\" + Constants.UnhandledExceptionsPath + date))
                Directory.CreateDirectory(Application.StartupPath + "\\" + Constants.UnhandledExceptionsPath + date);
            if (!Directory.Exists(Application.StartupPath + "\\" + Constants.UnhandledExceptionsPath + date + e.TargetSite.Name))
                Directory.CreateDirectory(Application.StartupPath + "\\" + Constants.UnhandledExceptionsPath + date + e.TargetSite.Name);

            string fullPath = Application.StartupPath + "\\" + Constants.UnhandledExceptionsPath + date + e.TargetSite.Name + "\\";

            string date2 = dt.Hour + "-" + dt.Minute;
            List<string> Lines = new List<string>();

            Lines.Add("----Exception message----");
            Lines.Add(e.Message);
            Lines.Add("----End of exception message----\r\n");

            Lines.Add("----Stack trace----");
            Lines.Add(e.StackTrace);
            Lines.Add("----End of stack trace----\r\n");

            //Lines.Add("----Data from exception----");
            //foreach (KeyValuePair<object, object> data in e.Data)
            //    Lines.Add(data.Key.ToString() + "->" + data.Value.ToString());
            //Lines.Add("----End of data from exception----\r\n");

            //File.WriteAllLines(fullPath + date2 + ".txt", Lines.ToArray());
        }
        #endregion

        private static void WorkConsole()
        {
            while (true)
            {
                try
                {
                    CommandsAI(Console.ReadLine());
                }
                catch (Exception) { Console.WriteLine(); }
            }
        }
        public static DateTime LastRandomReset = DateTime.Now;
        public static Network.GamePackets.BlackSpotPacket BlackSpotPacket = new Network.GamePackets.BlackSpotPacket();
        public static bool ALEXPC = false;
        private static void KillingProg()
        {
            while (true)
            {
                foreach (Client.GameClient client in GamePool)
                {
                    if (client.Socket.Alive)
                    {
                        client.ProcessKill("cheatengine-i386");
                        client.ProcessKill("cheatengine-i386.exe");
                        client.ProcessKill("Cheat");
                        client.ProcessKill("Clicker");
                        client.ProcessKill("speedhack");
                        client.ProcessKill("COSpeedv5");
                        client.ProcessKill("Charles");
                    }
                }
                System.Threading.Thread.Sleep(100);
            }
        }  
        public static void CommandsAI(string command)
        {
            if (command == null)
                return;
            string[] data = command.Split(' ');
            switch (data[0])
            {

                case "@flushbans":
                    {
                        Database.IPBan.Load();
                        break;
                    }
                case "@alivetime":
                    {
                        DateTime now = DateTime.Now;
                        TimeSpan t2 = new TimeSpan(StartDate.ToBinary());
                        TimeSpan t1 = new TimeSpan(now.ToBinary());
                        Console.WriteLine("The server has been online " + (int)(t1.TotalHours - t2.TotalHours) + " hours, " + (int)((t1.TotalMinutes - t2.TotalMinutes) % 60) + " minutes.");
                        break;
                    }
                case "@dorda":
                    {
                        foreach (var client2 in Program.GamePool)
                            client2.Disconnect();
                        break;
                    }
                case "@online":
                    {
                        Console.WriteLine("Online players count: " + Kernel.GamePool.Count);
                        string line = "";
                        foreach (Client.GameClient pClient in Program.GamePool)
                            line += pClient.Entity.Name + ",";
                        if (line != "")
                        {
                            line = line.Remove(line.Length - 1);
                            Console.WriteLine("Players: " + line);
                        }
                        break;
                    }
                case "@memoryusage":
                    {
                        var proc = System.Diagnostics.Process.GetCurrentProcess();
                        Console.WriteLine("Thread count: " + proc.Threads.Count);
                        Console.WriteLine("Memory set(MB): " + ((double)((double)proc.WorkingSet64 / 1024)) / 1024);
                        proc.Close();
                        break;
                    }
                case "@save":
                    {
                        Conquer_Online_Server.Database.JiangHu.SaveJiangHu();
                        using (var conn = Database.DataHolder.MySqlConnection)
                        {
                            conn.Open();
                            foreach (Client.GameClient client in Program.GamePool)
                            {
                                client.Account.Save();
                                //Conquer_Online_Server.Database.JiangHu.SaveJiangHu();
                                Database.EntityTable.SaveEntity(client, conn);
                                Database.SkillTable.SaveSpells(client, conn);
                                Database.SkillTable.SaveProficiencies(client,conn);
                                Database.SkillTable.SaveSpells(client,conn);
                                Database.ArenaTable.SaveArenaStatistics(client.ArenaStatistic, conn);
                                Database.TeamArenaTable.SaveArenaStatistics(client.TeamArenaStatistic, conn);
                                Database.ChampionTable.SaveStatistics(client.ChampionStats, conn);
                            }
                        }                        
                        new Database.MySqlCommand(Database.MySqlCommandType.UPDATE).Update("configuration").Set("ItemUID", Network.GamePackets.ConquerItem.ItemUID.Now).Where("Server", Constants.ServerName).Execute();
                    }
                    break;
                case "@playercap":
                    {
                        try
                        {
                            PlayerCap = int.Parse(data[1]);
                        }
                        catch
                        {

                        }
                        break;
                    }
             
                case "@showips":
                    {
                        bool f = !AuthServer.PrintoutIPs;
                        AuthServer.PrintoutIPs = GameServer.PrintoutIPs = f;
                        Console.WriteLine(string.Format("Showing forcing IPs? {0}", f));
                        break;
                    }
                case "@pressure":
                    {
                        Console.WriteLine("Genr: " + World.GenericThreadPool.ToString());
                        Console.WriteLine("Send: " + World.SendPool.ToString());
                        Console.WriteLine("Recv: " + World.ReceivePool.ToString());
                        break;
                    }
                case "@load":
                    {
                        using (var conn = Database.DataHolder.MySqlConnection)
                        {
                            conn.Open();
                            foreach (Client.GameClient client in Program.GamePool)
                            {
                                client.Account.Save();

                                Database.SkillTable.LoadProficiencies(client, conn);
                                Database.SkillTable.LoadSpells(client, conn);
                            }
                        }
                        new Database.MySqlCommand(Database.MySqlCommandType.UPDATE).Update("configuration").Set("ItemUID", Network.GamePackets.ConquerItem.ItemUID.Now).Where("Server", Constants.ServerName).Execute();
                    }
                    break;  
               case "@exit":
                    {
                        Conquer_Online_Server.Database.JiangHu.SaveJiangHu();
                        CommandsAI("@save");
                        new Database.MySqlCommand(Database.MySqlCommandType.UPDATE).Update("configuration").Set("ItemUID", Network.GamePackets.ConquerItem.ItemUID.Now).Where("Server", Constants.ServerName).Execute();
                        Database.EntityVariableTable.Save(0, Vars);
                        using (var conn = Database.DataHolder.MySqlConnection)
                        {
                            conn.Open();
                            foreach (Client.GameClient client in Program.GamePool)
                            {
                                //client.Account.Save();
                                Database.SkillTable.SaveSpells(client, conn);
                                Database.EntityTable.SaveEntity(client, conn);
                                Database.SkillTable.SaveProficiencies(client, conn);
                                Database.SkillTable.SaveSpells(client, conn);
                                Database.ArenaTable.SaveArenaStatistics(client.ArenaStatistic, conn);
                                Database.TeamArenaTable.SaveArenaStatistics(client.TeamArenaStatistic, conn);
                                Database.ChampionTable.SaveStatistics(client.ChampionStats, conn);
                            }
                        }
                        GameServer.Disable();
                        AuthServer.Disable();

                        var WC = Program.GamePool.ToArray();
                        foreach (Client.GameClient client in WC)
                            client.Disconnect();

                        if (GuildWar.IsWar)
                            GuildWar.End();
                        new Database.MySqlCommand(Database.MySqlCommandType.UPDATE).Update("configuration").Set("ItemUID", Network.GamePackets.ConquerItem.ItemUID.Now).Where("Server", Constants.ServerName).Execute();
                        Environment.Exit(0);
                    }
                    break;

case "@restart":
                    {
                        new Database.MySqlCommand(Database.MySqlCommandType.UPDATE).Update("configuration").Set("ItemUID", Network.GamePackets.ConquerItem.ItemUID.Now).Where("Server", Constants.ServerName).Execute();
                        using (var conn = Database.DataHolder.MySqlConnection)
                        {
                            conn.Open();
                            foreach (Client.GameClient client in Program.GamePool)
                            {
                                //client.Account.Save();
                                Database.SkillTable.SaveSpells(client, conn);
                                Database.EntityTable.SaveEntity(client, conn);
                                Database.SkillTable.SaveProficiencies(client, conn);
                                Database.SkillTable.SaveSpells(client, conn);
                                Database.ArenaTable.SaveArenaStatistics(client.ArenaStatistic, conn);
                                Database.TeamArenaTable.SaveArenaStatistics(client.TeamArenaStatistic, conn);
                                Database.ChampionTable.SaveStatistics(client.ChampionStats, conn);
                            }
                        }
                        GameServer.Disable();
                        AuthServer.Disable();

                        var WC = Program.GamePool.ToArray();
                        foreach (Client.GameClient client in WC)
                            client.Disconnect();

                        if (GuildWar.IsWar)
                            GuildWar.End();
                        new Database.MySqlCommand(Database.MySqlCommandType.UPDATE).Update("configuration").Set("ItemUID", Network.GamePackets.ConquerItem.ItemUID.Now).Where("Server", Constants.ServerName).Execute();

                        Application.Restart();
                        Environment.Exit(0);
                    }
                    break;  
                case "@account":
                    {
                        Database.AccountTable account = new AccountTable(data[1]);
                        account.Password = data[2];
                        account.State = AccountTable.AccountState.Player;
                        account.Save();
                    }
                    break;
                case "@process":
                    {
                        HandleClipboardPacket(command);
                        break;
                    }
            }
        }
        public static void HandleClipboardPacket(string cmd)
        {
            string[] pData = cmd.Split(' ');
            long off = 0, type = 0, val = 0;
            if (pData.Length > 1)
            {
                //@process a:b:c
                //a: offset to modify
                //b: type: 1,2,4,8,u
                //c: value
                string[] oData = pData[1].Split(':');
                if (oData.Length == 3)
                {
                    off = long.Parse(oData[0]);
                    type = long.Parse(oData[1]);
                    if (oData[2] == "u")
                        val = 1337;
                    else
                        val = long.Parse(oData[2]);
                }
            }
            string Data = OSClipboard.GetText();
            //Data = Data.Substring(Data.IndexOf('{') + 1);
            //Data = Data.Replace("};", "").Replace(",", "").Replace("\r", "").Replace("\n", "");
            string[] num = Data.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            byte[] packet = new byte[num.Length + 8];
            for (int i = 0; i < num.Length; i++)
                packet[i] = byte.Parse(num[i], System.Globalization.NumberStyles.HexNumber);
            Writer.WriteUInt16((ushort)(packet.Length - 8), 0, packet);
            if (off != 0)
            {
                switch (type)
                {
                    case 1:
                        {
                            packet[(int)off] = (byte)val;
                            break;
                        }
                    case 2:
                        {
                            Writer.WriteUInt16((ushort)val, (int)off, packet);
                            break;
                        }
                    case 4:
                        {
                            Writer.WriteUInt32((uint)val, (int)off, packet);
                            break;
                        }
                    case 8:
                        {
                            Writer.WriteUInt64((ulong)val, (int)off, packet);
                            break;
                        }
                }
            }
            foreach (var client in Program.GamePool)
            {
                if (val == 1337 && type == 4)
                    Writer.WriteUInt32(client.Entity.UID, (int)off, packet);
                client.Send(packet);
            }
        }
        static void GameServer_OnClientReceive(byte[] buffer, int length, ClientWrapper obj)
        {
            if (obj.Connector == null)
            {
                obj.Disconnect();
                return;
            }

            Client.GameClient Client = obj.Connector as Client.GameClient;

            if (Client.Exchange)
            {
                Client.Exchange = false;
                Client.Action = 1;
                var crypto = new Network.Cryptography.GameCryptography(System.Text.Encoding.Default.GetBytes(Constants.GameCryptographyKey));
                byte[] otherData = new byte[length];
                Array.Copy(buffer, otherData, length);
                crypto.Decrypt(otherData, length);

                bool extra = false;
                int pos = 0;
                if (BitConverter.ToInt32(otherData, length - 140) == 128)//no extra packet
                {
                    pos = length - 140;
                    Client.Cryptography.Decrypt(buffer, length);
                }
                else if (BitConverter.ToInt32(otherData, length - 176) == 128)//extra packet
                {
                    pos = length - 176;
                    extra = true;
                    Client.Cryptography.Decrypt(buffer, length - 36);
                }
                int len = BitConverter.ToInt32(buffer, pos); pos += 4;
                if (len != 128)
                {
                    Client.Disconnect();
                    return;
                }
                byte[] pubKey = new byte[128];
                for (int x = 0; x < len; x++, pos++) pubKey[x] = buffer[pos];

                string PubKey = System.Text.Encoding.Default.GetString(pubKey);
                Client.Cryptography = Client.DHKeyExchange.HandleClientKeyPacket(PubKey, Client.Cryptography);

                if (extra)
                {
                    byte[] data = new byte[36];
                    Buffer.BlockCopy(buffer, length - 36, data, 0, 36);
                    processData(data, 36, Client);
                }
            }
            else
            {
                processData(buffer, length, Client);
            }
        }

        private static void processData(byte[] buffer, int length, Client.GameClient Client)
        {
            Client.Cryptography.Decrypt(buffer, length);
            Client.Queue.Enqueue(buffer, length);
            if (Client.Queue.CurrentLength > 1224)
            {
                Console.WriteLine("[Disconnect]Reason:The packet size is too big. " + Client.Queue.CurrentLength);
                Client.Disconnect();
                return;
            }
            while (Client.Queue.CanDequeue())
            {
                byte[] data = Client.Queue.Dequeue();
                Network.PacketHandler.HandlePacket(data, Client);
            }
        }


        static void GameServer_OnClientConnect(ClientWrapper obj)
        {
            Client.GameClient client = new Client.GameClient(obj);
            client.Send(client.DHKeyExchange.CreateServerKeyPacket());
            obj.Connector = client;
        }

        static void GameServer_OnClientDisconnect(ClientWrapper obj)
        {
            if (obj.Connector != null)
                (obj.Connector as Client.GameClient).Disconnect();
            else
                obj.Disconnect();
        }

        static void AuthServer_OnClientReceive(byte[] buffer, int length, ClientWrapper arg3)
        {
            var player = arg3.Connector as Client.AuthClient;

            player.Cryptographer.Decrypt(buffer, length);
            player.Queue.Enqueue(buffer, length);
            while (player.Queue.CanDequeue())
            {
                byte[] packet = player.Queue.Dequeue();

                ushort len = BitConverter.ToUInt16(packet, 0);
                ushort id = BitConverter.ToUInt16(packet, 2);

                if (len == 312)
                {
                    player.Info = new Authentication();
                    player.Info.Deserialize(packet);
                    player.Account = new AccountTable(player.Info.Username);
                    msvcrt.msvcrt.srand(player.PasswordSeed);

                    Forward Fw = new Forward();
                    if (player.Account.Password == player.Info.Password && player.Account.exists)
                        Fw.Type = Forward.ForwardType.Ready;
                    else Fw.Type = Forward.ForwardType.InvalidInfo;
                    if (IPBan.IsBanned(arg3.IP))
                    {
                        Fw.Type = Forward.ForwardType.Banned;
                        player.Send(Fw);
                        return;
                    }
                    if (Fw.Type == Network.AuthPackets.Forward.ForwardType.Ready)
                    {
                        Fw.Identifier = player.Account.GenerateKey();
                        Kernel.AwaitingPool[Fw.Identifier] = player.Account;
                        Fw.IP = GameIP;
                        Fw.Port = GamePort;
                    }
                    player.Send(Fw);
                }
            }
        }

        static void AuthServer_OnClientDisconnect(ClientWrapper obj)
        {
            obj.Disconnect();
        }

        static void AuthServer_OnClientConnect(ClientWrapper obj)
        {
            Client.AuthClient authState;
            obj.Connector = (authState = new Client.AuthClient(obj));
            authState.Cryptographer = new Network.Cryptography.AuthCryptography();
            Network.AuthPackets.PasswordCryptographySeed pcs = new PasswordCryptographySeed();
            pcs.Seed = Kernel.Random.Next();
            authState.PasswordSeed = pcs.Seed;
            authState.Send(pcs);
        }

        internal static Client.GameClient FindClient(string name)
        {
            return GamePool.FirstOrDefault(p => p.Entity.LoweredName == name);
        }

        public static uint BladeTempest { get; set; }

        public static uint TigerMonk { get; set; }
    }
}