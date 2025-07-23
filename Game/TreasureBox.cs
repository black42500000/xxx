using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using Conquer_Online_Server.Client;
using Conquer_Online_Server.Interfaces;
using Conquer_Online_Server.Network.GamePackets;


namespace Conquer_Online_Server.Game
{
    public class TreasureBox
    {
        static Map Map = Kernel.Maps[10002];
        const int MAX_BOXES = 10, ITEMS = 1000, CPSMoney = 1001, LEVEL = 1002, DEATH = 1003, STUN = 1004, KICK = 1005;
        static int CurrentBoxes = 0;
        static Conquer_Online_Server.Interfaces.INpc npc;
        static ushort tempX, tempY = 0;
        static uint BaseId = 101002;
        static List<Point> VaildOnes = new List<Point>();
        public static bool OnGoing;

        public static void Generate()
        {
            try
            {
                if (CurrentBoxes < MAX_BOXES)
                {
                    if (Map == null)
                    {
                        Map = Kernel.Maps[10002];
                        return;
                    }
                    tempX = (ushort)Kernel.Random.Next(0, Map.Floor.Bounds.Width);
                    tempY = (ushort)Kernel.Random.Next(0, Map.Floor.Bounds.Height);
                    if (Map.Floor[tempX, tempY, MapObjectType.Item, null])
                    {
                        npc = new Network.GamePackets.NpcSpawn();
                        npc.UID = BaseId++;
                        npc.Mesh = (ushort)Kernel.RandFromGivingNums(9307, 9277, 9267, 9287, 9257);
                        npc.Type = Enums.NpcType.Talker;
                        npc.MapID = Map.ID;
                        npc.X = tempX;
                        npc.Y = tempY;

                        Map.AddNpc(npc);
                        CurrentBoxes++;
                        Kernel.SendWorldMessage(new Message("A new treasure box appeared!", Color.Red, 2012));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }

        public static void Reward(GameClient client)
        {
            client.Entity.CurrentTreasureBoxes++;
            var orders = Kernel.GamePool.Values.OrderByDescending(x => x.Entity.CurrentTreasureBoxes).ToArray();
            for (int i = 0; i < orders.Length + 1; i++)
            {
                if (i == 11) break;
                Message msg;

                if (i == 0)
                {
                    msg = new Message("", System.Drawing.Color.Red, Message.FirstRightCorner);
                }
                else
                {
                    if (orders[i - 1].Entity.CurrentTreasureBoxes == 0) continue;
                    msg = new Message("No " + i.ToString() + "- " + orders[i - 1].Entity.Name + " Opened " + orders[i - 1].Entity.CurrentTreasureBoxes.ToString() + " Boxes!", System.Drawing.Color.Red, Message.ContinueRightCorner);
                }

                Kernel.SendWorldMessage(msg, Program.GamePool, (ushort)10002);
            }

            int prize = 0;

            if (client.Entity.Level <= 135)
                prize = Kernel.RandFromGivingNums(ITEMS, CPSMoney, LEVEL, DEATH, KICK);
            else prize = Kernel.RandFromGivingNums(ITEMS, CPSMoney, DEATH, KICK);
            //#warning TREASURE BOX PRIZE
            switch (prize)
            {
                case LEVEL:
                    {
                        client.Entity.Level += 1;
                        Kernel.SendWorldMessage(new Message(client.Entity.Name + " Leveled-up while opening the TreasureBox!", Color.White, Message.Talk));
                        break;
                    }
                case CPSMoney:
                    {
                        uint amount = (uint)Kernel.RandFromGivingNums(10000, 15000, 20000, 90000, 25000, 30000, 35000, 40000);
                        if (amount >= 100000)
                        {
                            client.Entity.Money += amount;
                            Kernel.SendWorldMessage(new Message(client.Entity.Name + " got " + amount.ToString() + " Silvers while opening the TreasureBox!", Color.White, Message.Talk));
                        }
                        else
                        {
                            client.Entity.ConquerPoints += amount;
                            Kernel.SendWorldMessage(new Message(client.Entity.Name + " got " + amount.ToString() + " CPs while opening the TreasureBox!", Color.White, Message.Talk));
                        }
                        break;
                    }
                case ITEMS:
                    {
                        Database.ConquerItemBaseInformation temp;
                        uint itemid = (uint)Kernel.RandFromGivingNums(721020, 700013, 700003, 700033, 183325, 720770, 183495, 184305, 184315, 184375, 187305, 187315, 181395, 184405, 184365);
                        client.Inventory.Add(itemid, 0, 1);
                        Database.ConquerItemInformation.BaseInformations.TryGetValue(itemid, out temp);
                        Kernel.SendWorldMessage(new Message(client.Entity.Name + " got " + temp.Name + " while opening the TreasureBox!", Color.White, Message.Talk));
                        break;
                    }
                case DEATH:
                    {
                        client.Entity.Die(0);
                        Kernel.SendWorldMessage(new Message(client.Entity.Name + " found DEATH! while opening the TreasureBox!", Color.White, Message.Talk));
                        break;
                    }
                case KICK:
                    {
                        client.Entity.Teleport(1002, 300, 280);
                        Kernel.SendWorldMessage(new Message(client.Entity.Name + " got KICKED-OUT! while opening the TreasureBox!", Color.White, Message.Talk));
                        break;
                    }
            }

            CurrentBoxes--;
            client.Entity.ConquerPoints += 20;
            client.Entity.TreasuerPoints += 3;
            Kernel.SendWorldMessage(new Message(client.Entity.Name + " got 20 ConquerPoints & 3 TreasuerPoints while opening the TreasureBox!", Color.White, Message.Talk));

        }
    }
}
