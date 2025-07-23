using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer_Online_Server.Network.GamePackets;
namespace Conquer_Online_Server.Game.ConquerStructures
{
    public class Daily
    {
        public static bool PowerArenas = false;
        public static ushort Map = 8877;
        public static bool signup = false;
        public static int howmanyinmap = 0;
        public static int howmanyinmap2 = 0;
        public static int howmanyinmap3 = 0;
        public static int howmanyinmap10 = 0;
        public static int howmanyinmap11 = 0;
        public static int howmanyinmap12 = 0;
        public static int howmanyinmap13 = 0;
        public static int howmanyinmap14 = 0;
        public static int howmanyinmap15 = 0;
        public static int howmanyinmap16 = 0;
        public static int howmanyinmap17 = 0;
        public static int howmanyinmap18 = 0;
        public static int TopDlClaim = 0;
        public static int TopGlClaim = 0;
        public static void AddDl()
        {
            TopDlClaim++;
            //return;
        }
        public static void AddGl()
        {
            TopGlClaim++;
            //return;
        }
        public static void Map3()
        {
            howmanyinmap = 0;
            foreach (Client.GameClient client in Program.GamePool)
            {
                if (client.Entity.MapID == 5699 && client.Entity.Hitpoints >= 1)
                {
                    howmanyinmap += 1;
                    Conquer_Online_Server.Kernel.SendWorldMessage(new Conquer_Online_Server.Network.GamePackets.Message("Players Alive in Map3 Now: " + howmanyinmap + " ", System.Drawing.Color.Black, Conquer_Online_Server.Network.GamePackets.Message.FirstRightCorner), Program.GamePool);
                }

            }
        }
        public static void Map2()
        {
            howmanyinmap = 0;
            foreach (Client.GameClient client in Program.GamePool)
            {
                if (client.Entity.MapID == 5698 && client.Entity.Hitpoints >= 1)
                {
                    howmanyinmap += 1;
                    Conquer_Online_Server.Kernel.SendWorldMessage(new Conquer_Online_Server.Network.GamePackets.Message("Players Alive in Map2 Now: " + howmanyinmap + " ", System.Drawing.Color.Black, Conquer_Online_Server.Network.GamePackets.Message.FirstRightCorner), Program.GamePool);
                }

            }
        }
        public static void Map1()
        {
            howmanyinmap = 0;
            foreach (Client.GameClient client in Program.GamePool)
            {
                if (client.Entity.MapID == 5697 && client.Entity.Hitpoints >= 1)
                {
                    howmanyinmap += 1;
                    Conquer_Online_Server.Kernel.SendWorldMessage(new Conquer_Online_Server.Network.GamePackets.Message("Players Alive in Map1 Now: " + howmanyinmap + " ", System.Drawing.Color.Black, Conquer_Online_Server.Network.GamePackets.Message.FirstRightCorner), Program.GamePool);
                }

            }
        }
        public static void CheackAlive()
        {
            howmanyinmap = 0;
            foreach (Client.GameClient client in Program.GamePool)
            {
                if (client.Entity.MapID == 8877 && client.Entity.Hitpoints >= 1)
                {
                    howmanyinmap += 1;
                    Conquer_Online_Server.Kernel.SendWorldMessage(new Conquer_Online_Server.Network.GamePackets.Message("Players Alive in DailyPK Now: " + howmanyinmap + " ", System.Drawing.Color.Black, Conquer_Online_Server.Network.GamePackets.Message.FirstRightCorner), Program.GamePool);
                }

            }
        }
        public static void CheackAlive2()
        {
            howmanyinmap2 = 0;
            foreach (Client.GameClient client in Program.GamePool)
            {
                if (client.Entity.MapID == 3333 && client.Entity.Hitpoints >= 1)
                {
                    howmanyinmap2 += 1;
                    Conquer_Online_Server.Kernel.SendWorldMessage(new Conquer_Online_Server.Network.GamePackets.Message("Players Alive in LastManStanding: " + howmanyinmap2 + " ", System.Drawing.Color.Black, Conquer_Online_Server.Network.GamePackets.Message.FirstRightCorner), Program.GamePool);
                }

            }
        }
        public static void CheackAlive3()
        {
            howmanyinmap2 = 0;
            foreach (Client.GameClient client in Program.GamePool)
            {
                if (client.Entity.MapID == 1601 && client.Entity.Hitpoints >= 1)
                {
                    howmanyinmap2 += 1;
                    Conquer_Online_Server.Kernel.SendWorldMessage(new Conquer_Online_Server.Network.GamePackets.Message("Players Alive in ConquerPK: " + howmanyinmap2 + " ", System.Drawing.Color.Black, Conquer_Online_Server.Network.GamePackets.Message.FirstRightCorner), Program.GamePool);
                }

            }
        }
        public static void CheackAlive20()
        {
            howmanyinmap10 = 0;
            foreach (Client.GameClient client in Kernel.GamePool.Values)
            {
                if (client.Entity.MapID == 1701 && client.Entity.Hitpoints >= 1)
                {
                    howmanyinmap10 += 1;
                    Kernel.SendWorldMessage(new Network.GamePackets.Message("Players Alive in ChampionPk: " + howmanyinmap10 + " ", System.Drawing.Color.Black, Network.GamePackets.Message.FirstRightCorner), Program.GamePool);
                }

            }
        }
        public static void CheackSpouse()
        {
            howmanyinmap3 = 0;
            foreach (Client.GameClient client in Program.GamePool)
            {
                if (client.Entity.MapID == 1090 && client.Entity.Hitpoints >= 1)
                {
                    if (client.Entity.Body == 1003 || client.Entity.Body == 1004)
                    {
                        howmanyinmap3 += 1;
                        Conquer_Online_Server.Kernel.SendWorldMessage(new Conquer_Online_Server.Network.GamePackets.Message("Teams Alive in CouplesPk: " + howmanyinmap3 + " ", System.Drawing.Color.Black, Conquer_Online_Server.Network.GamePackets.Message.FirstRightCorner), Program.GamePool);

                    }
                }
            }
        }
        public static void CheackSpouse2()
        {
            howmanyinmap16 = 0;
            foreach (Client.GameClient client in Program.GamePool)
            {
                if (client.Entity.MapID == 1820 && client.Entity.Hitpoints >= 1)
                {
                    foreach (Client.GameClient Teammate in client.Team.Teammates)
                    {
                        if (Teammate.Team.TeamLeader)
                        {
                            howmanyinmap16 += 1;
                            Conquer_Online_Server.Kernel.SendWorldMessage(new Conquer_Online_Server.Network.GamePackets.Message("Teams Alive in TeamWar: " + howmanyinmap16 + " ", System.Drawing.Color.Black, Conquer_Online_Server.Network.GamePackets.Message.FirstRightCorner), Program.GamePool);

                        }
                    }
                }
            }
        }
        public static void SignUp()
        {
            foreach (Client.GameClient client in Program.GamePool)
                if (DateTime.Now.Minute == 00 && signup == false && client.Entity.Class >= 10 && client.Entity.Class <= 15)
                {
                    signup = true;
                    PowerArenas = true;
                    client.Entity.Status = 0;
                    client.Entity.RemoveFlag(Conquer_Online_Server.Network.GamePackets.Update.Flags.TopTrojan);
                }
        }


        public static void End()
        {
            if (DateTime.Now.Minute == 30)
            {
                //signup = false;
                //PowerArenas = false;
                foreach (Client.GameClient client in Program.GamePool)
                {
                    if (DateTime.Now.Minute == 30)
                    {
                        client.Entity.ConquerPoints += 150;
                        Conquer_Online_Server.Kernel.SendWorldMessage(new Conquer_Online_Server.Network.GamePackets.Message("Daily Has Ended Come Next Hour ", System.Drawing.Color.Red, Conquer_Online_Server.Network.GamePackets.Message.TopLeft), Program.GamePool);
                    }
                    if (client.Entity.MapID == 8877)
                    {
                        client.Entity.Teleport(1002, 400, 400);
                    }
                    client.Entity.RemoveFlag(Update.Flags.Flashy);
                }
            }
        }
    }
}
