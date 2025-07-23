using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Conquer_Online_Server.Network.GamePackets;
using Conquer_Online_Server.Interfaces;

namespace Conquer_Online_Server.Game
{
    class Gifts
    {
        public static ushort X = 300, Y = 289;
        public static ushort X1 = 300, Y1 = 280;
        public static ushort X2 = 300, Y2 = 265;
        public static ushort X3 = 310, Y3 = 266;
        public static ushort X4 = 320, Y4 = 267;
        public static ushort X5 = 321, Y5 = 278;
        public static ushort X6 = 321, Y6 = 288;
        public static ushort X7 = 311, Y7 = 288;
        public static void Load()
        {
            if (Kernel.Maps.ContainsKey(1002))
            {
                uint ItemID = 720159;
                #region CPBag

                INpc npc = new Network.GamePackets.NpcSpawn();
                npc.UID = 1305;
                npc.Mesh = 13050;
                npc.Type = Enums.NpcType.Talker;
                npc.X = (ushort)(X - 1);
                npc.Y = (ushort)(Y - 1);
                npc.MapID = 1002;
                //Kernel.Maps[1002].AddNpc(npc);
                // Program.kimozTime16 = Time32.Now;




                Game.Map Map = Kernel.Maps[1002];
                if (Map.SelectCoordonates(ref X, ref Y))
                {
                    Network.GamePackets.FloorItem floorItem = new Network.GamePackets.FloorItem(true);
                    floorItem.ValueType = Network.GamePackets.FloorItem.FloorValueType.ConquerPoints;
                    floorItem.Value = Database.rates.PartyDrop;
                    floorItem.ItemID = ItemID;
                    floorItem.MapID = 1002;
                    floorItem.MapObjType = Game.MapObjectType.Item;
                    floorItem.X = X;
                    floorItem.Y = Y;
                    floorItem.Type = Network.GamePackets.FloorItem.Drop;
                    floorItem.OnFloor = Time32.Now;
                    floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                    while (Map.Npcs.ContainsKey(floorItem.UID))
                    {
                        floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                    }
                    Map.AddFloorItem(floorItem);
                    foreach (Client.GameClient C in Kernel.GamePool.Values)
                    {
                        if (C.Entity.MapID == 1002)
                        {
                            C.SendScreenSpawn(floorItem, true);
                            npc.SendSpawn(C);

                            //C.Entity.Update(PhoenixPrvixyProject.Network.GamePackets._String.Effect, "wsmhcxq_att", true);
                        }
                    }
                }

                // Network.GamePackets.NpcInitial2.DeleteNPC2(1305);
                #endregion
                //Load2();
            }
        }
        public static void Load2()
        {
            if (Kernel.Maps.ContainsKey(1002))
            {
                #region CPBag

                uint ItemID = 720159;
                ushort X = X1, Y = Y1;
                INpc npc = new Network.GamePackets.NpcSpawn();
                npc.UID = 1305;
                npc.Mesh = 13050;
                npc.Type = Enums.NpcType.Talker;
                npc.X = (ushort)(X - 1);
                npc.Y = (ushort)(Y - 1);
                npc.MapID = 1002;
                // Kernel.Maps[1002].AddNpc(npc);

                //  Program.kimozTime16 = Time32.Now;


                Game.Map Map = Kernel.Maps[1002];
                if (Map.SelectCoordonates(ref X, ref Y))
                {
                    Network.GamePackets.FloorItem floorItem = new Network.GamePackets.FloorItem(true);
                    floorItem.ValueType = Network.GamePackets.FloorItem.FloorValueType.ConquerPoints;
                    floorItem.Value = Database.rates.PartyDrop;
                    floorItem.ItemID = ItemID;
                    floorItem.MapID = 1002;
                    floorItem.MapObjType = Game.MapObjectType.Item;
                    floorItem.X = X;
                    floorItem.Y = Y;
                    floorItem.Type = Network.GamePackets.FloorItem.Drop;
                    floorItem.OnFloor = Time32.Now;
                    floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                    while (Map.Npcs.ContainsKey(floorItem.UID))
                        floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                    Map.AddFloorItem(floorItem);
                    foreach (Client.GameClient C in Kernel.GamePool.Values)
                    {
                        if (C.Entity.MapID == 1002)
                        {
                            C.SendScreenSpawn(floorItem, true);
                            npc.SendSpawn(C);

                            // C.Entity.Update(PhoenixPrvixyProject.Network.GamePackets._String.Effect, "wsmhcxq_att", true);
                        }
                    }

                    // Network.GamePackets.NpcInitial2.DeleteNPC2(1305);
                #endregion

                }
            }
        }
        public static void Load3()
        {
            if (Kernel.Maps.ContainsKey(1002))
            {
                #region CPBag

                uint ItemID = 720159;
                ushort X = X2, Y = Y2;
                INpc npc = new Network.GamePackets.NpcSpawn();
                npc.UID = 1305;
                npc.Mesh = 13050;
                npc.Type = Enums.NpcType.Talker;
                npc.X = (ushort)(X - 1);
                npc.Y = (ushort)(Y - 1);
                npc.MapID = 1002;
                // Kernel.Maps[1002].AddNpc(npc);

                Game.Map Map = Kernel.Maps[1002];
                if (Map.SelectCoordonates(ref X, ref Y))
                {
                    Network.GamePackets.FloorItem floorItem = new Network.GamePackets.FloorItem(true);
                    floorItem.ValueType = Network.GamePackets.FloorItem.FloorValueType.ConquerPoints;
                    floorItem.Value = Database.rates.PartyDrop;
                    floorItem.ItemID = ItemID;
                    floorItem.MapID = 1002;
                    floorItem.MapObjType = Game.MapObjectType.Item;
                    floorItem.X = X;
                    floorItem.Y = Y;
                    floorItem.Type = Network.GamePackets.FloorItem.Drop;
                    floorItem.OnFloor = Time32.Now;
                    floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                    while (Map.Npcs.ContainsKey(floorItem.UID))
                        floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                    Map.AddFloorItem(floorItem);
                    foreach (Client.GameClient C in Kernel.GamePool.Values)
                    {
                        if (C.Entity.MapID == 1002)
                        {
                            C.SendScreenSpawn(floorItem, true);
                            npc.SendSpawn(C);
                            // C.Entity.Update(PhoenixPrvixyProject.Network.GamePackets._String.Effect, "wsmhcxq_att", true);
                        }
                    }
                    // Network.GamePackets.NpcInitial2.DeleteNPC2(1305);
                #endregion
                    //Load4();
                }
            }
        }
        public static void Load4()
        {
            if (Kernel.Maps.ContainsKey(1002))
            {
                #region CPBag

                uint ItemID = 720159;
                ushort X = X3, Y = Y3;
                INpc npc = new Network.GamePackets.NpcSpawn();
                npc.UID = 1305;
                npc.Mesh = 13050;
                npc.Type = Enums.NpcType.Talker;
                npc.X = (ushort)(X - 1);
                npc.Y = (ushort)(Y - 1);
                npc.MapID = 1002;
                //Kernel.Maps[1002].AddNpc(npc);

                Game.Map Map = Kernel.Maps[1002];
                if (Map.SelectCoordonates(ref X, ref Y))
                {
                    Network.GamePackets.FloorItem floorItem = new Network.GamePackets.FloorItem(true);
                    floorItem.ValueType = Network.GamePackets.FloorItem.FloorValueType.ConquerPoints;
                    floorItem.Value = Database.rates.PartyDrop;
                    floorItem.ItemID = ItemID;
                    floorItem.MapID = 1002;
                    floorItem.MapObjType = Game.MapObjectType.Item;
                    floorItem.X = X;
                    floorItem.Y = Y;
                    floorItem.Type = Network.GamePackets.FloorItem.Drop;
                    floorItem.OnFloor = Time32.Now;
                    floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                    while (Map.Npcs.ContainsKey(floorItem.UID))
                        floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                    Map.AddFloorItem(floorItem);
                    foreach (Client.GameClient C in Kernel.GamePool.Values)
                    {
                        if (C.Entity.MapID == 1002)
                        {
                            C.SendScreenSpawn(floorItem, true);
                            npc.SendSpawn(C);
                            //  C.Entity.Update(PhoenixPrvixyProject.Network.GamePackets._String.Effect, "wsmhcxq_att", true);
                        }
                    }
                    // Network.GamePackets.NpcInitial2.DeleteNPC2(1305);
                #endregion
                    Load5();
                }
            }
        }
        public static void Load5()
        {
            if (Kernel.Maps.ContainsKey(1002))
            {
                #region CPBag

                uint ItemID = 720159;
                ushort X = X4, Y = Y4;
                INpc npc = new Network.GamePackets.NpcSpawn();
                npc.UID = 1305;
                npc.Mesh = 13050;
                npc.Type = Enums.NpcType.Talker;
                npc.X = (ushort)(X - 1);
                npc.Y = (ushort)(Y - 1);
                npc.MapID = 1002;
                //Kernel.Maps[1002].AddNpc(npc);*

                Game.Map Map = Kernel.Maps[1002];
                if (Map.SelectCoordonates(ref X, ref Y))
                {
                    Network.GamePackets.FloorItem floorItem = new Network.GamePackets.FloorItem(true);
                    floorItem.ValueType = Network.GamePackets.FloorItem.FloorValueType.ConquerPoints;
                    floorItem.Value = Database.rates.PartyDrop;
                    floorItem.ItemID = ItemID;
                    floorItem.MapID = 1002;
                    floorItem.MapObjType = Game.MapObjectType.Item;
                    floorItem.X = X;
                    floorItem.Y = Y;
                    floorItem.Type = Network.GamePackets.FloorItem.Drop;
                    floorItem.OnFloor = Time32.Now;
                    floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                    while (Map.Npcs.ContainsKey(floorItem.UID))
                        floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                    Map.AddFloorItem(floorItem);
                    foreach (Client.GameClient C in Kernel.GamePool.Values)
                    {
                        if (C.Entity.MapID == 1002)
                        {
                            C.SendScreenSpawn(floorItem, true);
                            npc.SendSpawn(C);
                            // C.Entity.Update(PhoenixPrvixyProject.Network.GamePackets._String.Effect, "wsmhcxq_att", true);
                        }
                    }
                    // Network.GamePackets.NpcInitial2.DeleteNPC2(1305);
                #endregion
                    // Load6();
                }
            }
        }
        public static void Load6()
        {
            if (Kernel.Maps.ContainsKey(1002))
            {
                #region CPBag

                uint ItemID = 720159;
                ushort X = X5, Y = Y5;
                INpc npc = new Network.GamePackets.NpcSpawn();
                npc.UID = 1305;
                npc.Mesh = 13050;
                npc.Type = Enums.NpcType.Talker;
                npc.X = (ushort)(X - 1);
                npc.Y = (ushort)(Y - 1);
                npc.MapID = 1002;
                //Kernel.Maps[1002].AddNpc(npc);

                Game.Map Map = Kernel.Maps[1002];
                if (Map.SelectCoordonates(ref X, ref Y))
                {
                    Network.GamePackets.FloorItem floorItem = new Network.GamePackets.FloorItem(true);
                    floorItem.ValueType = Network.GamePackets.FloorItem.FloorValueType.ConquerPoints;
                    floorItem.Value = Database.rates.PartyDrop;
                    floorItem.ItemID = ItemID;
                    floorItem.MapID = 1002;
                    floorItem.MapObjType = Game.MapObjectType.Item;
                    floorItem.X = X;
                    floorItem.Y = Y;
                    floorItem.Type = Network.GamePackets.FloorItem.Drop;
                    floorItem.OnFloor = Time32.Now;
                    floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                    while (Map.Npcs.ContainsKey(floorItem.UID))
                        floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                    Map.AddFloorItem(floorItem);
                    foreach (Client.GameClient C in Kernel.GamePool.Values)
                    {
                        if (C.Entity.MapID == 1002)
                        {
                            C.SendScreenSpawn(floorItem, true);
                            npc.SendSpawn(C);
                            //  C.Entity.Update(PhoenixPrvixyProject.Network.GamePackets._String.Effect, "wsmhcxq_att", true);
                        }
                    }
                    //Network.GamePackets.NpcInitial2.DeleteNPC2(1305);
                #endregion
                    //Load7();
                }
            }
        }
        public static void Load7()
        {
            if (Kernel.Maps.ContainsKey(1002))
            {
                #region CPBag

                uint ItemID = 720159;
                ushort X = X6, Y = Y6;
                INpc npc = new Network.GamePackets.NpcSpawn();
                npc.UID = 1305;
                npc.Mesh = 13050;
                npc.Type = Enums.NpcType.Talker;
                npc.X = (ushort)(X - 1);
                npc.Y = (ushort)(Y - 1);
                npc.MapID = 1002;
                //  Kernel.Maps[1002].AddNpc(npc);

                Game.Map Map = Kernel.Maps[1002];
                if (Map.SelectCoordonates(ref X, ref Y))
                {
                    Network.GamePackets.FloorItem floorItem = new Network.GamePackets.FloorItem(true);
                    floorItem.ValueType = Network.GamePackets.FloorItem.FloorValueType.ConquerPoints;
                    floorItem.Value = Database.rates.PartyDrop;
                    floorItem.ItemID = ItemID;
                    floorItem.MapID = 1002;
                    floorItem.MapObjType = Game.MapObjectType.Item;
                    floorItem.X = X;
                    floorItem.Y = Y;
                    floorItem.Type = Network.GamePackets.FloorItem.Drop;
                    floorItem.OnFloor = Time32.Now;
                    floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                    while (Map.Npcs.ContainsKey(floorItem.UID))
                        floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                    Map.AddFloorItem(floorItem);
                    foreach (Client.GameClient C in Kernel.GamePool.Values)
                    {
                        if (C.Entity.MapID == 1002)
                        {
                            C.SendScreenSpawn(floorItem, true);
                            npc.SendSpawn(C);
                            // C.Entity.Update(PhoenixPrvixyProject.Network.GamePackets._String.Effect, "wsmhcxq_att", true);
                        }
                    }

                    // Network.GamePackets.NpcInitial2.DeleteNPC2(1305);

                }
                #endregion
            }
            // Load8();
        }//443 377
        public static void Load8()
        {
            if (Kernel.Maps.ContainsKey(1002))
            {
                #region CPBag

                uint ItemID = 720159;
                ushort X = X7, Y = Y7;
                INpc npc = new Network.GamePackets.NpcSpawn();
                npc.UID = 1305;
                npc.Mesh = 13050;
                npc.Type = Enums.NpcType.Talker;
                npc.X = (ushort)(X - 1);
                npc.Y = (ushort)(Y - 1);
                npc.MapID = 1002;
                //Kernel.Maps[1002].AddNpc(npc);

                Game.Map Map = Kernel.Maps[1002];
                if (Map.SelectCoordonates(ref X, ref Y))
                {
                    Network.GamePackets.FloorItem floorItem = new Network.GamePackets.FloorItem(true);
                    floorItem.ValueType = Network.GamePackets.FloorItem.FloorValueType.ConquerPoints;
                    floorItem.Value = Database.rates.PartyDrop;
                    floorItem.ItemID = ItemID;
                    floorItem.MapID = 1002;
                    floorItem.MapObjType = Game.MapObjectType.Item;
                    floorItem.X = X;
                    floorItem.Y = Y;
                    floorItem.Type = Network.GamePackets.FloorItem.Drop;
                    floorItem.OnFloor = Time32.Now;
                    floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                    while (Map.Npcs.ContainsKey(floorItem.UID))
                        floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                    Map.AddFloorItem(floorItem);
                    foreach (Client.GameClient C in Kernel.GamePool.Values)
                    {
                        if (C.Entity.MapID == 1002)
                        {
                            C.SendScreenSpawn(floorItem, true);
                            npc.SendSpawn(C);
                        }
                    }


                #endregion
                    // Load9();
                }
            }
        }
        public static void Load9()
        {
            if (Kernel.Maps.ContainsKey(1002))
            {
                ushort X = 900, Y = 900;
                INpc npc = new Network.GamePackets.NpcSpawn();
                npc.UID = 1305;
                npc.Mesh = 13050;
                npc.Type = Enums.NpcType.Talker;
                npc.X = (ushort)(X - 1);
                npc.Y = (ushort)(Y - 1);
                npc.MapID = 1002;
                foreach (Client.GameClient C in Kernel.GamePool.Values)
                {
                    if (C.Entity.MapID == 1002)
                    {

                        npc.SendSpawn(C);
                        // C.Entity.Update(PhoenixPrvixyProject.Network.GamePackets._String.Effect, "wsmhcxq_att", true);
                    }
                }
                // Kernel.Maps[1002].AddNpc(npc);

            }


        }


    }

}

