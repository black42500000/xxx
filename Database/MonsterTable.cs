using System;
using System.Collections.Generic;
using System.IO;
using Conquer_Online_Server.Network.GamePackets;


namespace Conquer_Online_Server.Database
{
    public class DROP_SOULS
    {
        public static Dictionary<uint, Items_drop> Souls = new Dictionary<uint, Items_drop>();
        public static uint Count_souls = 0;

        public class Items_drop
        {
            public uint item_id;
            public string Item_denumire;
            public uint item_rand;
        }

        public static void LoadDrops()
        {
            /*Load Souls Drop p1 / p2 /p3 */
            string[] aFMobs = File.ReadAllLines("database\\sopuls.txt");
            for (int i = 0; i < aFMobs.Length; i++)
            {
                if (aFMobs[i][0] != '*')
                {
                    string[] Info = aFMobs[i].Split(' ');
                    Items_drop Item = new Items_drop();
                    Item.Item_denumire = "SOULS";
                    Item.item_id = uint.Parse(Info[0]);
                    Count_souls += 1;

                    Souls.Add(Count_souls, Item);
                }
            }
            Console.WriteLine("Souls loading " + Souls.Count);
        }
    }
    public class MonsterInformation
    {
        private struct SpecialItemDrop
        {
            public int ItemID, Rate, Discriminant, Map;
        }
        private static List<SpecialItemDrop> SpecialItemDropList = new List<SpecialItemDrop>();
        public Game.Entity Owner;

        public uint ExcludeFromSend = 0;
        private bool LabirinthDrop = false;
        public bool Guard, Reviver;
        public uint ID;
        public ushort Mesh;
        public byte Level;
        public uint Type;
        public string Name;
        public uint Hitpoints;
        public ushort Defence;
        public ushort ViewRange;
        public ushort AttackRange;
        public int RespawnTime;
        public uint MinAttack, MaxAttack;
        public byte AttackType;
        public ushort SpellID;
        public uint InSight;
        public uint InRev;
        public uint InStig;
        public bool ISLava = false;
        public bool Boss;
        public Time32 LastMove;
        public int MoveSpeed;
        public int RunSpeed;
        public int OwnItemID, OwnItemRate;
        public int HPPotionID, MPPotionID;
        public int AttackSpeed;
        public int MinimumSpeed
        {
            get
            {
                int min = 10000000;
                if (min > MoveSpeed)
                    min = MoveSpeed;
                if (min > RunSpeed)
                    min = RunSpeed;
                if (min > AttackSpeed)
                    min = AttackSpeed;
                return min;
            }
        }
        public uint ExtraExperience;
        public uint MinMoneyDropAmount;
        public uint MaxMoneyDropAmount;

        public ushort BoundX, BoundY;
        public ushort BoundCX, BoundCY;

        public static SafeDictionary<byte, List<uint>> ItemDropCache = new SafeDictionary<byte, List<uint>>(3000);
        public static SafeDictionary<byte, List<uint>> SoulItemCache = new SafeDictionary<byte, List<uint>>(3000);

        public void SendScreen(byte[] buffer)
        {
            foreach (Client.GameClient client in Program.GamePool)
            {
                if (client != null)
                {
                    if (client.Entity != null)
                    {
                        if (client.Entity.UID != ExcludeFromSend)
                        {
                            if (Kernel.GetDistance(client.Entity.X, client.Entity.Y, Owner.X, Owner.Y) > 18)
                            {
                                continue;
                            }
                            client.Send(buffer);
                        }
                    }
                }
            }
        }
        public void SendScreen(Interfaces.IPacket buffer)
        {
            SendScreen(buffer.ToArray());
        }
        public void SendScreenSpawn(Interfaces.IMapObject _object)
        {
            foreach (Client.GameClient client in Program.GamePool)
            {
                if (client != null)
                {
                    if (client.Entity != null)
                    {
                        if (client.Entity.UID != ExcludeFromSend)
                        {
                            if (client.Map.ID == Owner.MapID)
                            {
                                if (Kernel.GetDistance(client.Entity.X, client.Entity.Y, Owner.X, Owner.Y) > 25)
                                {
                                    continue;
                                }
                                _object.SendSpawn(client, false);
                            }
                        }
                    }
                }
            }
        }
        public static bool ItemsInInventory = false;
        public void Drop(Game.Entity killer)
        {
            #region xmas event 
            if (Name == "EvilMonkMisery")
            {
                {
                    killer.Owner.Inventory.Add(3000423, 0, 1);
                    return;
                }
            }
            if (Name == "FlameDevastator")
            {
                {
                    killer.Owner.Inventory.Add(711643, 0, 1);
                    return;
                }
            }
            if (Name == "GhostReaver")
            {
                {
                    killer.Owner.Inventory.Add(711708, 0, 1);
                    return;
                }
            }
            if (Name == "FuriousDevastato")
            {
                {

                    killer.Owner.Inventory.Add(722061, 0, 1);
                    return;
                }
            }
            if (Name == "AwakeDevastator")
            {
                {

                    killer.Owner.Inventory.Add(711117, 0, 1);
                    return;
                }
            }
            #endregion
            if (Name.Contains("[Guard"))
                return;
            #region Pheasant and Birdman

            if (killer.EntityFlag == Game.EntityFlag.Player)

                if (Kernel.Rate(100))
                {
                    killer.ConquerPoints += rates.maxcps;
                    killer.Owner.Send(new Network.GamePackets.Message("#56 Congratulations #56 You Got " + rates.maxcps + " Cps #67 GoodLuck By Mr.Bahaa #67", System.Drawing.Color.Red, 2005));
                }

            #endregion
       
            #region Dragon King
            if (Owner.Name == "Dragon King" && Program.World.PureLand)
            {

                Kernel.SendWorldMessage(new Message(killer.Owner.Entity.Name + " has killed the Dragon King from the Pure Land and gained a big prize!", Message.Center));
                killer.Owner.Entity.ConquerPoints += 100;
                killer.Owner.Entity.ConquerPoints += 100;

            }
            #endregion
            #region HalfOffToken
            if (killer.EntityFlag == Game.EntityFlag.Player)
            {
                DateTime halfOffDateTime = killer.Owner["halfoffdate"];
                if (killer.Owner["halfoff"])
                    if (DateTime.Now.DayOfYear != halfOffDateTime.DayOfYear)
                        killer.Owner["halfoff"] = false;
                if (!killer.Owner["halfoff"])
                {
                    if (Kernel.Rate(1, 10000))
                    {
                        killer.Owner["halfoff"] = true;
                        killer.Owner["halfoffdate"] = DateTime.Now;

                        uint ID = 480339;

                        var infos = Database.ConquerItemInformation.BaseInformations[ID];
                        if (infos == null) return;
                        ushort X = Owner.X, Y = Owner.Y;
                        Game.Map Map = Kernel.Maps[Owner.MapID];
                        if (Map.SelectCoordonates(ref X, ref Y))
                        {
                            FloorItem floorItem = new FloorItem(true);
                            floorItem.Item = new Network.GamePackets.ConquerItem(true);
                            floorItem.Item.Color = (Game.Enums.Color)Kernel.Random.Next(4, 8);
                            floorItem.Item.ID = ID;
                            floorItem.Item.MaximDurability = infos.Durability;

                            floorItem.Item.Durability = (ushort)(Kernel.Random.Next(infos.Durability / 10));

                            if (!Network.PacketHandler.IsEquipment(ID) && infos.ConquerPointsWorth == 0)
                            {
                                floorItem.Item.StackSize = 1;
                                floorItem.Item.MaxStackSize = infos.StackSize;
                            }
                            floorItem.Item.MobDropped = true;
                            floorItem.ValueType = FloorItem.FloorValueType.Item;
                            floorItem.ItemID = ID;
                            floorItem.MapID = Owner.MapID;
                            floorItem.MapObjType = Game.MapObjectType.Item;
                            floorItem.X = X;
                            floorItem.Y = Y;
                            floorItem.Owner = killer.Owner;
                            floorItem.Type = FloorItem.Drop;
                            floorItem.OnFloor = Time32.Now;
                            floorItem.ItemColor = floorItem.Item.Color;
                            floorItem.UID = FloorItem.FloorUID.Next;
                            while (Map.Npcs.ContainsKey(floorItem.UID))
                                floorItem.UID = FloorItem.FloorUID.Next;
                            Map.AddFloorItem(floorItem);
                            SendScreenSpawn(floorItem);
                        }
                    }
                }
            }
            #endregion
            if (ItemsInInventory)
            {
                #region [Organizer-Stars 1]
                if (Name == "Apparition")
                {
                    if (Kernel.Rate(1, 5))
                    {
                        if (killer.Name.Contains("[Guard1]"))
                        {
                            return;
                        }
                        killer.Owner.Inventory.Add(710222, 0, 1);
                        killer.Owner.Send(new Network.GamePackets.Message("Congratulation You Have Got The Item From Here Go to Another Map", System.Drawing.Color.White, 255));
                    }
                }
                #endregion
                #region [Organizer-Stars 2]
                if (Name == "Bandit")
                {
                    if (Kernel.Rate(1, 5))
                    {
                        if (killer.Name.Contains("[Guard1]"))
                        {
                            return;
                        }
                        killer.Owner.Inventory.Add(710852, 0, 1);
                        killer.Owner.Send(new Network.GamePackets.Message("Congratulation You Have Got The Item From Here Go to Another Map", System.Drawing.Color.Yellow, 255));
                    }
                }
                #endregion
                #region [Organizer-Stars 3]
                if (Name == "HillMonster")
                {
                    if (Kernel.Rate(1, 5))
                    {
                        if (killer.Name.Contains("[Guard1]"))
                        {
                            return;
                        }
                        killer.Owner.Inventory.Add(711138, 0, 1);
                        killer.Owner.Send(new Network.GamePackets.Message("Congratulation You Have Got The Item From Here Go to Another Map", System.Drawing.Color.Yellow, 255));
                    }
                }
                #endregion
                #region [Organizer-Stars 4]
                if (Name == "Robin")
                {
                    if (Kernel.Rate(1, 5))
                    {
                        if (killer.Name.Contains("[Guard1]"))
                        {
                            return;
                        }
                        killer.Owner.Inventory.Add(711252, 0, 1);
                        killer.Owner.Send(new Network.GamePackets.Message("Congratulation You Have Got The Item From Here Go to Another Map", System.Drawing.Color.Yellow, 255));
                    }
                }
                #endregion
                #region [OrganizerStars 5]
                if (Name == "Ratling")
                {
                    if (Kernel.Rate(1, 5))
                    {
                        if (killer.Name.Contains("[Guard1]"))
                        {
                            return;
                        }
                        killer.Owner.Inventory.Add(711253, 0, 1);
                        killer.Owner.Send(new Network.GamePackets.Message("Congratulation You Have Got The Item From Here Go to Another Map", System.Drawing.Color.Yellow, 255));
                    }
                }
                #endregion
                #region [Organizer-Stars 6]
                if (Name == "BladeGhost")
                {
                    if (Kernel.Rate(1, 5))
                    {
                        if (killer.Name.Contains("[Guard1]"))
                        {
                            return;
                        }
                        killer.Owner.Inventory.Add(722682, 0, 1);
                        killer.Owner.Send(new Network.GamePackets.Message("Congratulation You Have Got The Item From Here Go to Another Map", System.Drawing.Color.Yellow, 255));
                    }
                }
                #endregion
                #region [Organizer-Stars 7]
                if (Name == "Birdman")
                {
                    /*if (Kernel.Rate(1, 5))
                    {
                        if (killer.Name.Contains("[Guard1]"))
                        {
                            return;
                        }
                        killer.Owner.Inventory.Add(722894, 0, 1);
                        killer.Owner.Send(new Network.GamePackets.Message("Congratulation You Have Got The Item From Here Go to Another Map", System.Drawing.Color.Yellow, 255));
                    }*/
                }
                #endregion
                #region [Organizer-Stars 8]
                if (Name == "HawKing")
                {
                    if (Kernel.Rate(1, 5))
                    {
                        if (killer.Name.Contains("[Guard1]"))
                        {
                            return;
                        }
                        killer.Owner.Inventory.Add(711021, 0, 1);
                        killer.Owner.Send(new Network.GamePackets.Message("Congratulation You Have Got The Item From Here Go to Another Map", System.Drawing.Color.Yellow, 255));
                    }
                }
                #endregion
                #region [Organizer-Stars 9]
                if (Name == "Macaque")
                {
                    if (Kernel.Rate(1, 5))
                    {
                        if (killer.Name.Contains("[Guard1]"))
                        {
                            return;
                        }
                        killer.Owner.Inventory.Add(722895, 0, 1);
                        killer.Owner.Send(new Network.GamePackets.Message("Congratulation You Have Got The Item From Here Go to Another Map", System.Drawing.Color.Yellow, 255));
                    }
                }
                #endregion
            }
            #region DemonBoxs
            if (killer.EntityFlag == Game.EntityFlag.Player || killer.Companion)
            {
                uint cps = 0;
                if (Name == "Demon")
                {
                    if (Kernel.Rate(30, 1000)) cps += 15000;
                    else if (Kernel.Rate(40, 1000)) cps += 11000;
                    else if (Kernel.Rate(70, 1000)) cps += 9800;
                    else if (Kernel.Rate(80, 1000)) cps += 9500;
                    else cps += 9000;

                    killer.Owner.Entity.ConquerPoints += cps;
                    Kernel.SendWorldMessage(new Message(killer.Owner.Entity.Name + " Has got " + cps.ToString() + " CPs From killing the " + Name + " Monster!", System.Drawing.Color.Yellow, 2000));
                }
                if (Name == "AncientDemon")
                {
                    if (Kernel.Rate(30, 1000)) cps += 70000;
                    else if (Kernel.Rate(40, 1000)) cps += 60000;
                    else if (Kernel.Rate(70, 1000)) cps += 49800;
                    else if (Kernel.Rate(80, 1000)) cps += 49500;
                    else cps += 49000;

                    killer.Owner.Entity.ConquerPoints += cps;
                    Kernel.SendWorldMessage(new Message(killer.Owner.Entity.Name + " Has got " + cps.ToString() + " CPs From killing the " + Name + " Monster!", System.Drawing.Color.Yellow, 2000));
                }
                if (Name == "FloodDemon")
                {
                    if (Kernel.Rate(30, 1000)) cps += 170000;
                    else if (Kernel.Rate(40, 1000)) cps += 150000;
                    else if (Kernel.Rate(70, 1000)) cps += 99500;
                    else if (Kernel.Rate(80, 1000)) cps += 99100;
                    else cps += 99000;

                    killer.Owner.Entity.ConquerPoints += cps;
                    Kernel.SendWorldMessage(new Message(killer.Owner.Entity.Name + " Has got " + cps.ToString() + " CPs From killing the " + Name + " Monster!", System.Drawing.Color.Yellow, 2000));
                }
                if (Name == "HeavenDemon")
                {
                    if (Kernel.Rate(30, 1000)) cps += 580000;
                    else if (Kernel.Rate(40, 1000)) cps += 550000;
                    else if (Kernel.Rate(70, 1000)) cps += 499000;
                    else if (Kernel.Rate(80, 1000)) cps += 450000;
                    else cps += 459000;

                    killer.Owner.Entity.ConquerPoints += cps;
                    Kernel.SendWorldMessage(new Message(killer.Owner.Entity.Name + " Has got " + cps.ToString() + " CPs From killing the " + Name + " Monster!", System.Drawing.Color.Yellow, 2000));
                }
                if (Name == "ChaosDemon")
                {
                    if (Kernel.Rate(30, 1000)) cps += 1300000;
                    else if (Kernel.Rate(40, 1000)) cps += 1200000;
                    else if (Kernel.Rate(70, 1000)) cps += 980000;
                    else if (Kernel.Rate(80, 1000)) cps += 950000;
                    else cps += 900000;

                    killer.Owner.Entity.ConquerPoints += cps;
                    Kernel.SendWorldMessage(new Message(killer.Owner.Entity.Name + " Has got " + cps.ToString() + " CPs From killing the " + Name + " Monster!", System.Drawing.Color.Yellow, 2000));
                }
                if (Name == "ScaredDemon")
                {
                    if (Kernel.Rate(30, 1000)) cps += 2500000;
                    else if (Kernel.Rate(40, 1000)) cps += 2100000;
                    else if (Kernel.Rate(70, 1000)) cps += 1980000;
                    else if (Kernel.Rate(80, 1000)) cps += 1910000;
                    else cps += 1900000;

                    killer.Owner.Entity.ConquerPoints += cps;
                    Kernel.SendWorldMessage(new Message(killer.Owner.Entity.Name + " Has got " + cps.ToString() + " CPs From killing the " + Name + " Monster!", System.Drawing.Color.Yellow, 2000));
                }
            }
            #endregion
            if (Name == "Naga")
            {
                {
                    killer.DisKO += 1;
                    killer.Owner.Send(new Message("Congratulations! You have got 1 Kill you have Now " + killer.DisKO + " DisKo Points", System.Drawing.Color.Azure, Message.TopLeft));
                    return;
                }
            }
            if (Name == "LavaBeast")
            {
                {
                    killer.SubClasses.StudyPoints += 20;
                    Conquer_Online_Server.Kernel.SendWorldMessage(new Conquer_Online_Server.Network.GamePackets.Message("Congratulations! " + killer.Name + " Has Killed LavaBeast and got 20 StudyPoints!", System.Drawing.Color.White, 2005), Program.GamePool);
                    return;
                }
            }
            if (Name == "Temptress")
            {
                {
                    killer.DisKO += 1;
                    killer.Owner.Send(new Message("Congratulations! You have got 1 Kill you have Now " + killer.DisKO + " DisKo Points", System.Drawing.Color.Azure, Message.TopLeft));
                    return;
                }
            }
            if (Name == "Centicore")
            {
                {
                    killer.DisKO += 1;
                    killer.Owner.Send(new Message("Congratulations! You have got 1 Kill you have Now " + killer.DisKO + " DisKo Points", System.Drawing.Color.Azure, Message.TopLeft));
                    return;
                }
            }
            if (Name == "HellTroll")
            {
                {
                    killer.DisKO += 3;
                    killer.Owner.Send(new Message("Congratulations! You have got 3 Kill you have Now " + killer.DisKO + " DisKo Points", System.Drawing.Color.Azure, Message.TopLeft));
                    return;
                }
            }
            /*#region BigBosses
                        if (Name == "SnowBanshee")
                        {
                            {
                                killer.ConquerPoints += 500000;
                                Kernel.Spawn2 = false;
                                killer.Owner.Send(new Message("Congratulations! " + killer.Name + " has defeated Boss SnowBanshe In Big Bosses", System.Drawing.Color.Azure, Message.Monster));
                                Conquer_Online_Server.Kernel.SendWorldMessage(new Conquer_Online_Server.Network.GamePackets.Message("Congratulations! " + killer.Name + " Has Killed SnowBanshe Monster In BigBosses For This He got " + 500000 + " !", System.Drawing.Color.Black, 2011), Program.GamePool);
                            }
                        }
                        if (Name == "ThrillingSpook")
                        {
                            {
                                killer.ConquerPoints += 500000;
                                Kernel.Spawn3 = false;
                                killer.Owner.Send(new Message("Congratulations! You have got 1 Kill you have Now " + killer.Name + " has defeated Boss ThrilingSpook", System.Drawing.Color.Azure, Message.Monster));
                                Conquer_Online_Server.Kernel.SendWorldMessage(new Conquer_Online_Server.Network.GamePackets.Message("Congratulations! " + killer.Name + " Has Killed ThrilingSpook In BigBosses Monster  For This He got " + 500000 + " !", System.Drawing.Color.Black, 2011), Program.GamePool);
                            }
                        }
                        if (Name == "TeratoDragon")
                        {
                            {
                                killer.ConquerPoints += 500000;
                                Kernel.Spawn4 = false;
                                killer.Owner.Send(new Message("Congratulations! You have got 1 Kill you have Now " + killer.Name + " has defeated Boss TeratDragon", System.Drawing.Color.Azure, Message.Monster));
                                Conquer_Online_Server.Kernel.SendWorldMessage(new Conquer_Online_Server.Network.GamePackets.Message("Congratulations! " + killer.Name + " Has Killed TeratDragon In BigBosses Monster  For This He got " + 500000 + " !", System.Drawing.Color.Black, 2011), Program.GamePool);
                            }
                        }
                        if (Name == "SwordMaster")
                        {
                            {
                                killer.ConquerPoints += 500000;
                                Kernel.Spawn5 = false;
                                killer.Owner.Send(new Message("Congratulations! You have got 1 Kill you have Now " + killer.Name + " has defeated Boss SwordMastr", System.Drawing.Color.Azure, Message.Monster));
                                Conquer_Online_Server.Kernel.SendWorldMessage(new Conquer_Online_Server.Network.GamePackets.Message("Congratulations! " + killer.Name + " Has Killed SwordMastr In BigBosses Monster  For This He got " + 500000 + " !", System.Drawing.Color.Black, 2011), Program.GamePool);
                            }
                        }
                        #endregion            */
            #region Snow Banchee
            if (Name == "SnowBanshee")
            {
                uint Uid = 0;
                byte type = 30;
                for (int i = 0; i < 1; i++)
                {
                    type = (byte)Kernel.Random.Next(1, 50);
                    switch (type)
                    {
                        case 1:
                            Uid = 800320;
                            break;

                        case 2:
                            Uid = 822054;
                            break;

                        case 3:
                            Uid = 800110;
                            break;

                        case 4:
                            Uid = 820056;
                            break;

                        case 5:
                            Uid = 822056;
                            break;

                        case 6:
                            Uid = 822057;
                            break;

                        case 7:
                            Uid = 822053;
                            break;

                        case 8:
                            Uid = 800019;
                            break;

                        case 9:
                            Uid = 800050;
                            break;

                        case 10:
                            Uid = 800015;
                            break;

                        case 11:
                            Uid = 800090;
                            break;

                        case 12:
                            Uid = 800513;
                            break;

                        case 13:
                            Uid = 800017;
                            break;

                        case 14:
                            Uid = 800071;
                            break;

                        case 15:
                            Uid = 800016;
                            break;

                        case 16:
                            Uid = 823051;
                            break;

                        case 17:
                            Uid = 800130;
                            break;

                        case 18:
                            Uid = 800140;
                            break;

                        case 19:
                            Uid = 800141;
                            break;

                        case 20:
                            Uid = 800200;
                            break;

                        case 21:
                            Uid = 800310;
                            break;

                        case 22:
                            Uid = 800014;
                            break;

                        case 23:
                            Uid = 800214;
                            break;

                        case 24:
                            Uid = 800230;
                            break;

                        case 25:
                            Uid = 800414;
                            break;

                        case 26:
                            Uid = 822052;
                            break;

                        case 27:
                            Uid = 800420;
                            break;

                        case 28:
                            Uid = 800401;
                            break;

                        case 29:
                            Uid = 800512;
                            break;

                        case 30:
                            Uid = 823043;
                            break;

                        case 31:
                            Uid = 800514;
                            break;

                        case 32:
                            Uid = 800520;
                            break;

                        case 33:
                            Uid = 800521;
                            break;

                        case 34:
                            Uid = 800613;
                            break;

                        case 35:
                            Uid = 800614;
                            break;

                        case 36:
                            Uid = 800615;
                            break;

                        case 37:
                            Uid = 824001;
                            break;

                        case 38:
                            Uid = 800617;
                            break;

                        case 39:
                            Uid = 800720;
                            break;

                        case 40:
                            Uid = 800721;
                            break;

                        case 41:
                            Uid = 800070;
                            break;

                        case 42:
                            Uid = 800723;
                            break;

                        case 43:
                            Uid = 800724;
                            break;

                        case 44:
                            Uid = 800018;
                            break;

                        case 45:
                            Uid = 820001;
                            break;

                        case 46:
                            Uid = 820052;
                            break;

                        case 47:
                            Uid = 820053;
                            break;

                        case 48:
                            Uid = 820054;
                            break;

                        case 49:
                            Uid = 820055;
                            break;

                        case 50:
                            Uid = 800722;
                            break;
                    }

                    if (Uid != 0)
                    {
                        ushort X = Owner.X, Y = Owner.Y;
                        Game.Map Map = Kernel.Maps[Owner.MapID];
                        //if (Map.SelectCoordonates(ref X, ref Y))
                        {
                            //Kernel.Spawn = false;
                            killer.Owner.Inventory.Add(Uid, 0, 1);
                            Kernel.SendWorldMessage(new Network.GamePackets.Message("Congratulations! " + killer.Name + " Has Defeated " + Name + " and dropped! " + Database.ConquerItemInformation.BaseInformations[Uid].Name + " and " + rates.SnowBanshe + " CPS!", System.Drawing.Color.White, Message.Center), Program.GamePool);
                            killer.ConquerPoints += rates.SnowBanshe;
                            Network.GamePackets._String str = new _String(true);
                            str.UID = killer.UID;
                            str.TextsCount = 1;
                            str.Type = _String.Effect;
                            str.Texts.Add("cortege");
                            killer.Owner.SendScreen(str, true);
                            // return;
                        }
                    }
                }
            }
            #endregion
            #region NemesisTyrant
            if (Name == "NemesisTyrant")
            {
                killer.ConquerPoints += 1000000;
                byte times = (byte)Kernel.Random.Next(1, 3);
                byte ref_times = (byte)Kernel.Random.Next(1, 6);
                for (byte i = 0; i < times; i++)
                {
                    uint Uid = 0;
                    byte type = (byte)Kernel.Random.Next(1, 75);
                    switch (type)
                    {
                        #region IDs
                        case 1:
                            Uid = 800020;
                            break;

                        case 2:
                            Uid = 800111;
                            break;

                        case 3:
                            Uid = 800142;
                            break;

                        case 4:
                            Uid = 800215;
                            break;

                        case 5:
                            Uid = 800255;
                            break;

                        case 6:
                            Uid = 800422;
                            break;

                        case 7:
                            Uid = 800522;
                            break;

                        case 8:
                            Uid = 800618;
                            break;

                        case 9:
                            Uid = 800725;
                            break;

                        case 10:
                            Uid = 800810;
                            break;

                        case 11:
                            Uid = 800811;
                            break;

                        case 12:
                            Uid = 800917;
                            break;

                        case 13:
                            Uid = 820073;
                            break;

                        case 14:
                            Uid = 820074;
                            break;

                        case 15:
                            Uid = 820075;
                            break;

                        case 16:
                            Uid = 820076;
                            break;

                        case 17:
                            Uid = 821033;
                            break;

                        case 18:
                            Uid = 821034;
                            break;

                        case 19:
                            Uid = 822071;
                            break;

                        case 20:
                            Uid = 822072;
                            break;

                        case 21:
                            Uid = 823058;
                            break;

                        case 22:
                            Uid = 823059;
                            break;

                        case 23:
                            Uid = 823060;
                            break;

                        case 24:
                            Uid = 823061;
                            break;

                        case 25:
                            Uid = 823062;
                            break;

                        case 26:
                            Uid = 824019;
                            break;

                        case 27:
                            Uid = 824020;
                            break;

                        case 28:
                            Uid = 800401;
                            break;

                        case 29:
                            Uid = 800512;
                            break;

                        case 30:
                            Uid = 823043;
                            break;

                        case 31:
                            Uid = 800514;
                            break;

                        case 32:
                            Uid = 800520;
                            break;

                        case 33:
                            Uid = 800521;
                            break;

                        case 34:
                            Uid = 800613;
                            break;

                        case 35:
                            Uid = 800614;
                            break;

                        case 36:
                            Uid = 800615;
                            break;

                        case 37:
                            Uid = 824001;
                            break;

                        case 38:
                            Uid = 800617;
                            break;

                        case 39:
                            Uid = 800720;
                            break;

                        case 40:
                            Uid = 800721;
                            break;

                        case 41:
                            Uid = 800070;
                            break;

                        case 42:
                            Uid = 800723;
                            break;

                        case 43:
                            Uid = 800724;
                            break;

                        case 44:
                            Uid = 800018;
                            break;

                        case 45:
                            Uid = 820001;
                            break;

                        case 46:
                            Uid = 820052;
                            break;

                        case 47:
                            Uid = 820053;
                            break;

                        case 48:
                            Uid = 820054;
                            break;

                        case 49:
                            Uid = 820055;
                            break;

                        case 50:
                            Uid = 800722;
                            break;
                        case 51:
                            Uid = 821028;
                            break;

                        case 52:
                            Uid = 824015;
                            break;

                        case 53:
                            Uid = 824016;
                            break;
                        #endregion

                    }

                    if (Uid != 0)
                    {
                        killer.Owner.Inventory.Add(Uid, 0, 1);
                        Kernel.SendWorldMessage(new Message("Congratulations! " + killer.Name + " Has Defeated " + Name + " and get! Some items DragonSoul, with 1,00,000 Cps.", System.Drawing.Color.White, Message.World), Program.GamePool);
                        Kernel.SendWorldMessage(new Message("Congratulations! " + killer.Name + " Has Defeated " + Name + " and get! Some items DragonSoul, with 1,00,000 Cps.", System.Drawing.Color.White, Message.Center), Program.GamePool);
                        return;
                    }
                }
            }
            #endregion  
            
            #region LordTiger
            if (Name == "LordTiger")
            {
                killer.Teleport(1002, 300, 280);
                killer.ConquerPoints += rates.TeratoDragon;
                Network.GamePackets._String str = new _String(true);
                str.UID = killer.UID;
                str.TextsCount = 1;
                str.Type = _String.Effect;
                str.Texts.Add("cortege");
                killer.Owner.SendScreen(str, true);
                Network.PacketHandler.WorldMessage(killer.Name + "  has Killed LordTiger in LordTiger Quest and win " + rates.TeratoDragon + " cps");

            }


            #endregion TigerLord Quest
            #region SnowDemon
            if (Name == "SnowDemon")
            {
                uint Uid = 0;
                byte type = 30;
                for (int i = 0; i < 1; i++)
                {
                    type = (byte)Kernel.Random.Next(1, 30);
                    switch (type)
                    {
                        case 1:
                            Uid = 800320;
                            break;

                        case 2:
                            Uid = 822054;
                            break;

                        case 3:
                            Uid = 800110;
                            break;

                        case 4:
                            Uid = 820056;
                            break;

                        case 5:
                            Uid = 822056;
                            break;

                        case 6:
                            Uid = 822057;
                            break;

                        case 7:
                            Uid = 822053;
                            break;

                        case 8:
                            Uid = 800019;
                            break;

                        case 9:
                            Uid = 800050;
                            break;

                        case 10:
                            Uid = 800015;
                            break;

                        case 11:
                            Uid = 800090;
                            break;

                        case 12:
                            Uid = 800513;
                            break;

                        case 13:
                            Uid = 800017;
                            break;

                        case 14:
                            Uid = 800071;
                            break;

                        case 15:
                            Uid = 800016;
                            break;

                        case 16:
                            Uid = 823051;
                            break;

                        case 17:
                            Uid = 800130;
                            break;

                        case 18:
                            Uid = 800140;
                            break;

                        case 19:
                            Uid = 800141;
                            break;

                        case 20:
                            Uid = 800200;
                            break;

                        case 21:
                            Uid = 800310;
                            break;

                        case 22:
                            Uid = 800014;
                            break;

                        case 23:
                            Uid = 800214;
                            break;

                        case 24:
                            Uid = 800230;
                            break;

                        case 25:
                            Uid = 800414;
                            break;

                        case 26:
                            Uid = 822052;
                            break;

                        case 27:
                            Uid = 800420;
                            break;

                        case 28:
                            Uid = 800401;
                            break;

                        case 29:
                            Uid = 800512;
                            break;

                        case 30:
                            Uid = 823043;
                            break;

                        case 31:
                            Uid = 800514;
                            break;

                        case 32:
                            Uid = 800520;
                            break;

                        case 33:
                            Uid = 800521;
                            break;

                        case 34:
                            Uid = 800613;
                            break;

                        case 35:
                            Uid = 800614;
                            break;

                        case 36:
                            Uid = 800615;
                            break;

                        case 37:
                            Uid = 824001;
                            break;

                        case 38:
                            Uid = 800617;
                            break;

                        case 39:
                            Uid = 800720;
                            break;

                        case 40:
                            Uid = 800721;
                            break;

                        case 41:
                            Uid = 800070;
                            break;

                        case 42:
                            Uid = 800723;
                            break;

                        case 43:
                            Uid = 800724;
                            break;

                        case 44:
                            Uid = 800018;
                            break;

                        case 45:
                            Uid = 820001;
                            break;

                        case 46:
                            Uid = 820052;
                            break;

                        case 47:
                            Uid = 820053;
                            break;

                        case 48:
                            Uid = 820054;
                            break;

                        case 49:
                            Uid = 820055;
                            break;

                        case 50:
                            Uid = 800722;
                            break;
                    }

                    if (Uid != 0)
                    {
                        ushort X = Owner.X, Y = Owner.Y;
                        Game.Map Map = Kernel.Maps[Owner.MapID];
                        if (Map.SelectCoordonates(ref X, ref Y))
                        {
                            killer.Owner.Inventory.Add(Uid, 0, 1);
                            Kernel.Spawn4 = false;
                            Kernel.SendWorldMessage(new Network.GamePackets.Message("Congratulations! " + killer.Name + " Has Defeated " + Name + " and dropped! " + Database.ConquerItemInformation.BaseInformations[Uid].Name + " and " + rates.TeratoDragon + " CPS!", System.Drawing.Color.White, Message.Center), Program.GamePool);
                            killer.ConquerPoints += rates.TeratoDragon;
                            Network.GamePackets._String str = new _String(true);
                            str.UID = killer.UID;
                            str.TextsCount = 1;
                            str.Type = _String.Effect;
                            str.Texts.Add("cortege");
                            killer.Owner.SendScreen(str, true);
                            // return;
                        }
                    }
                }
            }
            #endregion
            #region ThirillingSpook
            if (Name == "ThrillingSpook")
            {
                uint Uid = 0;
                byte type = 30;
                for (int i = 0; i < 1; i++)
                {
                    type = (byte)Kernel.Random.Next(1, 30);
                    switch (type)
                    {
                        case 1:
                            Uid = 800320;
                            break;

                        case 2:
                            Uid = 822054;
                            break;

                        case 3:
                            Uid = 800110;
                            break;

                        case 4:
                            Uid = 820056;
                            break;

                        case 5:
                            Uid = 822056;
                            break;

                        case 6:
                            Uid = 822057;
                            break;

                        case 7:
                            Uid = 822053;
                            break;

                        case 8:
                            Uid = 800019;
                            break;

                        case 9:
                            Uid = 800050;
                            break;

                        case 10:
                            Uid = 800015;
                            break;

                        case 11:
                            Uid = 800090;
                            break;

                        case 12:
                            Uid = 800513;
                            break;

                        case 13:
                            Uid = 800017;
                            break;

                        case 14:
                            Uid = 800071;
                            break;

                        case 15:
                            Uid = 800016;
                            break;

                        case 16:
                            Uid = 823051;
                            break;

                        case 17:
                            Uid = 800130;
                            break;

                        case 18:
                            Uid = 800140;
                            break;

                        case 19:
                            Uid = 800141;
                            break;

                        case 20:
                            Uid = 800200;
                            break;

                        case 21:
                            Uid = 800310;
                            break;

                        case 22:
                            Uid = 800014;
                            break;

                        case 23:
                            Uid = 800214;
                            break;

                        case 24:
                            Uid = 800230;
                            break;

                        case 25:
                            Uid = 800414;
                            break;

                        case 26:
                            Uid = 822052;
                            break;

                        case 27:
                            Uid = 800420;
                            break;

                        case 28:
                            Uid = 800401;
                            break;

                        case 29:
                            Uid = 800512;
                            break;

                        case 30:
                            Uid = 823043;
                            break;

                        case 31:
                            Uid = 800514;
                            break;

                        case 32:
                            Uid = 800520;
                            break;

                        case 33:
                            Uid = 800521;
                            break;

                        case 34:
                            Uid = 800613;
                            break;

                        case 35:
                            Uid = 800614;
                            break;

                        case 36:
                            Uid = 800615;
                            break;

                        case 37:
                            Uid = 824001;
                            break;

                        case 38:
                            Uid = 800617;
                            break;

                        case 39:
                            Uid = 800720;
                            break;

                        case 40:
                            Uid = 800721;
                            break;

                        case 41:
                            Uid = 800070;
                            break;

                        case 42:
                            Uid = 800723;
                            break;

                        case 43:
                            Uid = 800724;
                            break;

                        case 44:
                            Uid = 800018;
                            break;

                        case 45:
                            Uid = 820001;
                            break;

                        case 46:
                            Uid = 820052;
                            break;

                        case 47:
                            Uid = 820053;
                            break;

                        case 48:
                            Uid = 820054;
                            break;

                        case 49:
                            Uid = 820055;
                            break;

                        case 50:
                            Uid = 800722;
                            break;
                    }

                    if (Uid != 0)
                    {
                        ushort X = Owner.X, Y = Owner.Y;
                        Game.Map Map = Kernel.Maps[Owner.MapID];
                        if (Map.SelectCoordonates(ref X, ref Y))
                        {
                            Kernel.Spawn3 = false;
                            killer.Owner.Inventory.Add(Uid, 0, 1);
                            Kernel.SendWorldMessage(new Network.GamePackets.Message("Congratulations! " + killer.Name + " Has Defeated " + Name + " and dropped! " + Database.ConquerItemInformation.BaseInformations[Uid].Name + " and " + rates.ThrillingSpook + " CPS!", System.Drawing.Color.White, Message.Center), Program.GamePool);
                            killer.ConquerPoints += rates.ThrillingSpook;
                            Network.GamePackets._String str = new _String(true);
                            str.UID = killer.UID;
                            str.TextsCount = 1;
                            str.Type = _String.Effect;
                            str.Texts.Add("cortege");
                            killer.Owner.SendScreen(str, true);
                            // return;
                        }
                    }
                }
            }
            #endregion
            #region Ghosts of sbarta By mr ahmed MrCapo

            if (Name == "Centar")
            {
                {
                    uint Uid = 0;
                    byte type = (byte)ServerBase.Kernel.Random.Next(1, 2);
                    switch (type)
                    {
                        case 1: Uid = 5130517; break;
                        case 2: Uid = 5130517; break;

                    }
                    killer.Owner.Inventory.Add(Uid, 0, 1);
                    //killer.ConquerPoints += 10000;
                    killer.Owner.Send(new Message("Congratulations! " + killer.Name + " Killd is Center monsetr!!Coded By mr Conquer_Online_Server", System.Drawing.Color.Azure, Message.Monster));
                    // return;
                }
            }

            if (Name == "Hades")
            {
                {
                    uint Uid = 0;
                    byte type = (byte)ServerBase.Kernel.Random.Next(1, 2);
                    switch (type)
                    {
                        case 1: Uid = 5130516; break;
                        case 2: Uid = 5130516; break;

                    }
                    killer.Owner.Inventory.Add(Uid, 0, 1);
                    //killer.ConquerPoints += 10000;
                    killer.Owner.Send(new Message("Congratulations! " + killer.Name + " Killd is Hades monsetr!!Coded By mr Conquer_Online_Server", System.Drawing.Color.Azure, Message.Monster));
                    // return;
                }
            }
            if (Name == "Cyclops")
            {
                {
                    uint Uid = 0;
                    byte type = (byte)ServerBase.Kernel.Random.Next(1, 2);
                    switch (type)
                    {
                        case 1: Uid = 5130515; break;
                        case 2: Uid = 5130515; break;

                    }
                    killer.Owner.Inventory.Add(Uid, 0, 1);
                    //killer.ConquerPoints += 10000;
                    killer.Owner.Send(new Message("Congratulations! " + killer.Name + " Killd is Cyclops monsetr!!Coded By mr Conquer_Online_Server", System.Drawing.Color.Azure, Message.Monster));
                    // return;
                }
            }
            #region coin gold:silver:copper monsetrs
            #region coin gold:silver:copper monsetrs
            if (Name == "Currency thief")
            {
                byte times = (byte)Kernel.Random.Next(1, 3);
                byte ref_times = (byte)Kernel.Random.Next(1, 6);
                for (byte i = 0; i < times; i++)
                {
                    uint Uid = 0;
                    byte type = (byte)Kernel.Random.Next(1, 50);
                    switch (type)
                    {
                        case 1:
                            Uid = 711609;
                            break;






                    }

                    if (Uid != 0)
                    {
                        ushort X = Owner.X, Y = Owner.Y;
                        Game.Map Map = Kernel.Maps[Owner.MapID];
                        if (Map.SelectCoordonates(ref X, ref Y))
                        {
                            killer.Owner.Inventory.Add(Uid, 0, 1);
                            killer.Owner.Send(new Message("Congratulation! " + killer.Name + " has Killed and you have a #35 Gold Coin .", System.Drawing.Color.Azure, Message.Monster));
                        }
                    }
                }
            }
            #endregion

            #region coin gold:silver:copper monsetrs
            if (Name == "Currency thief1")
            {
                byte times = (byte)Kernel.Random.Next(1, 3);
                byte ref_times = (byte)Kernel.Random.Next(1, 6);
                for (byte i = 0; i < times; i++)
                {
                    uint Uid = 0;
                    byte type = (byte)Kernel.Random.Next(1, 50);
                    switch (type)
                    {
                        case 1:
                            Uid = 711610;
                            break;






                    }

                    if (Uid != 0)
                    {
                        ushort X = Owner.X, Y = Owner.Y;
                        Game.Map Map = Kernel.Maps[Owner.MapID];
                        if (Map.SelectCoordonates(ref X, ref Y))
                        {
                            killer.Owner.Inventory.Add(Uid, 0, 1);
                            killer.Owner.Send(new Message("Congratulation! " + killer.Name + " has Killed and you have a #52 Silver Coin .", System.Drawing.Color.Azure, Message.Monster));
                        }
                    }
                }
            }
            #endregion

            #region coin gold:silver:copper monsetrs
            if (Name == "Currency thief2")
            {
                byte times = (byte)Kernel.Random.Next(1, 3);
                byte ref_times = (byte)Kernel.Random.Next(1, 6);
                for (byte i = 0; i < times; i++)
                {
                    uint Uid = 0;
                    byte type = (byte)Kernel.Random.Next(1, 50);
                    switch (type)
                    {
                        case 1:
                            Uid = 711611;
                            break;






                    }

                    if (Uid != 0)
                    {
                        ushort X = Owner.X, Y = Owner.Y;
                        Game.Map Map = Kernel.Maps[Owner.MapID];
                        if (Map.SelectCoordonates(ref X, ref Y))
                        {
                            killer.Owner.Inventory.Add(Uid, 0, 1);
                            killer.Owner.Send(new Message("Congratulation! " + killer.Name + " has Killed and you have a #52 Copper Coin .", System.Drawing.Color.Azure, Message.Monster));
                        }
                    }
                }
            }
            #endregion

            #endregion  
            if (Name == "Zeus")
            {
                {
                    uint Uid = 0;
                    byte type = (byte)ServerBase.Kernel.Random.Next(1, 2);
                    switch (type)
                    {
                        case 1: Uid = 5130514; break;
                        case 2: Uid = 5130514; break;

                    }
                    killer.Owner.Inventory.Add(Uid, 0, 1);
                    //killer.ConquerPoints += 10000;
                    killer.Owner.Send(new Message("Congratulations! " + killer.Name + " Killd is Zeus monsetr!!Coded By mr Conquer_Online_Server", System.Drawing.Color.Azure, Message.Monster));
                    // return;
                }
            }

            if (Name == "KingOfDevils")
            {
                {
                    uint Uid = 0;
                    byte type = (byte)ServerBase.Kernel.Random.Next(1, 2);
                    switch (type)
                    {
                        case 1: Uid = 188285; break;
                        case 2: Uid = 188285; break;

                    }
                    killer.Owner.Inventory.Add(Uid, 0, 1);
                    killer.ConquerPoints += 20000;
                    killer.Owner.Send(new Message("Congratulations! " + killer.Name + " Killd is KingOfDevils monsetr!!Coded By mr Conquer_Online_Server", System.Drawing.Color.Azure, Message.Monster));
                    // return;
                }
            }

            #endregion by MrCapo
            #region Ghosts of sparta

            #region Ghosts of sparta part 1
            if (ServerBase.Kernel.Rate(30))
            {
                if (Name == "Lord")
                {
                    uint Uid = 0;
                    byte type = (byte)ServerBase.Kernel.Random.Next(1, 4);
                    switch (type)
                    {
                        case 1: Uid = 20141; break;
                        case 2: Uid = 20142; break;
                        case 3: Uid = 20143; break;
                        case 4: Uid = 20144; break;
                        case 5: Uid = 20145; break;
                        case 6: Uid = 20146; break;
                    }

                    if (Uid != 0)
                    {
                        ushort X = Owner.X, Y = Owner.Y;
                        Game.Map Map = Kernel.Maps[Owner.MapID];
                        if (Map.SelectCoordonates(ref X, ref Y))
                        {
                            Network.GamePackets.FloorItem floorItem = new Network.GamePackets.FloorItem(true);
                            floorItem.Item = new Network.GamePackets.ConquerItem(true);
                            floorItem.Item.Color = (Conquer_Online_Server.Game.Enums.Color)ServerBase.Kernel.Random.Next(4, 8);
                            floorItem.Item.ID = Uid;
                            floorItem.Item.MaximDurability = floorItem.Item.Durability = 65535;
                            floorItem.Item.UID = Network.GamePackets.ConquerItem.ItemUID.Next;
                            floorItem.ValueType = Network.GamePackets.FloorItem.FloorValueType.Item;
                            floorItem.ItemID = Uid;
                            floorItem.MapID = Owner.MapID;
                            floorItem.MapObjType = Game.MapObjectType.Item;
                            floorItem.X = X;
                            floorItem.Y = Y;
                            floorItem.Type = Network.GamePackets.FloorItem.Drop;
                            floorItem.OnFloor = Time32.Now;
                            floorItem.ItemColor = floorItem.Item.Color;
                            floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                            while (Map.Npcs.ContainsKey(floorItem.UID))
                                floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                            Map.AddFloorItem(floorItem);
                            SendScreenSpawn(floorItem);
                        }
                    }
                }
            }
            #endregion
            #region theGhost

            if (Owner.MapID == 9575 && ServerBase.Kernel.PercentSuccess(200))
            {
                uint Uid2 = 0;
                byte type2 = (byte)ServerBase.Kernel.Random.Next(1, 3);
                switch (type2)
                {
                    case 1: Uid2 = 50; break;
                    case 2: Uid2 = 10; break;
                }
                if (killer.DoubleExperienceTime > 0)
                {
                    killer.ConquerPoints += Uid2;
                }
                else
                {
                    uint Uid = 0;
                    byte type = (byte)ServerBase.Kernel.Random.Next(1, 30);
                    switch (type)
                    {
                        case 1: Uid = 20141; break;
                        case 2: Uid = 20142; break;
                        case 3: Uid = 20143; break;
                        case 4: Uid = 20144; break;
                        case 5: Uid = 20145; break;
                        case 6: Uid = 20146; break;
                        case 7: Uid = 1088000; break;
                        case 8: Uid = 1088000; break;
                        case 9: Uid = 1088000; break;
                        case 10: Uid = 1088000; break;
                        case 11: Uid = 1088000; break;
                        case 12: Uid = 3000252; break;
                        case 13: Uid = 1088000; break;
                        case 14: Uid = 1088000; break;
                        case 15: Uid = 1088000; break;
                        case 16: Uid = 1088000; break;
                        case 17: Uid = 1088000; break;
                        case 18: Uid = 3000253; break;
                        case 19: Uid = 1088000; break;
                        case 20: Uid = 1088000; break;
                        case 21: Uid = 1088000; break;
                        case 22: Uid = 1088000; break;
                        case 23: Uid = 1088000; break;
                        case 24: Uid = 3000254; break;
                        case 25: Uid = 1088000; break;
                        case 26: Uid = 3000254; break;
                        case 27: Uid = 1088000; break;
                        case 28: Uid = 3000256; break;
                        case 29: Uid = 1088000; break;
                        case 30: Uid = 3000257; break;
                    }
                    if (Uid != 0)
                    {
                        ushort X = Owner.X, Y = Owner.Y;
                        Game.Map Map = Kernel.Maps[Owner.MapID];
                        if (Map.SelectCoordonates(ref X, ref Y))
                        {
                            Network.GamePackets.FloorItem floorItem = new Network.GamePackets.FloorItem(true);
                            floorItem.Item = new Network.GamePackets.ConquerItem(true);
                            floorItem.Item.Color = (Conquer_Online_Server.Game.Enums.Color)ServerBase.Kernel.Random.Next(4, 8);
                            floorItem.Item.ID = (uint)Uid;
                            floorItem.Item.MaximDurability = 65355;
                            floorItem.Item.MobDropped = true;
                            floorItem.ValueType = Network.GamePackets.FloorItem.FloorValueType.Item;
                            floorItem.ItemID = (uint)Uid;
                            floorItem.MapID = Owner.MapID;
                            floorItem.MapObjType = Game.MapObjectType.Item;
                            floorItem.X = X;
                            floorItem.Y = Y;
                            floorItem.Type = Network.GamePackets.FloorItem.Drop;
                            floorItem.OnFloor = Time32.Now;
                            floorItem.ItemColor = floorItem.Item.Color;
                            floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                            while (Map.Npcs.ContainsKey(floorItem.UID))
                                floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                            Map.AddFloorItem(floorItem);
                            SendScreenSpawn(floorItem);
                        }
                    }
                }
            }

            #endregion Hope

            #endregion
            #region Box Satan life
            #region [N~monster]

            if (ServerBase.Kernel.Rate(30))
            {
                if (Name == "[N~monster]")
                {
                    uint Uid = 0;
                    byte type = (byte)ServerBase.Kernel.Random.Next(1, 12);
                    switch (type)
                    {
                        case 1: Uid = 114303; break;
                        case 2: Uid = 114303; break;
                        case 3: Uid = 117303; break;
                        case 4: Uid = 117303; break;
                        case 5: Uid = 118303; break;
                        case 6: Uid = 118303; break;
                        case 7: Uid = 121263; break;
                        case 8: Uid = 121263; break;
                        case 9: Uid = 123303; break;
                        case 10: Uid = 123303; break;
                        case 11: Uid = 20301; break;
                        case 12: Uid = 20301; break;
                    }

                    if (Uid != 0)
                    {
                        ushort X = Owner.X, Y = Owner.Y;
                        Game.Map Map = Kernel.Maps[Owner.MapID];
                        if (Map.SelectCoordonates(ref X, ref Y))
                        {
                            Network.GamePackets.FloorItem floorItem = new Network.GamePackets.FloorItem(true);
                            floorItem.Item = new Network.GamePackets.ConquerItem(true);
                            floorItem.Item.Color = (Conquer_Online_Server.Game.Enums.Color)ServerBase.Kernel.Random.Next(4, 8);
                            floorItem.Item.ID = Uid;
                            floorItem.Item.MaximDurability = floorItem.Item.Durability = 65535;
                            floorItem.Item.UID = Network.GamePackets.ConquerItem.ItemUID.Next;
                            floorItem.ValueType = Network.GamePackets.FloorItem.FloorValueType.Item;
                            floorItem.ItemID = Uid;
                            floorItem.MapID = Owner.MapID;
                            floorItem.MapObjType = Game.MapObjectType.Item;
                            floorItem.X = X;
                            floorItem.Y = Y;
                            floorItem.Type = Network.GamePackets.FloorItem.Drop;
                            floorItem.OnFloor = Time32.Now;
                            floorItem.ItemColor = floorItem.Item.Color;
                            floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                            while (Map.Npcs.ContainsKey(floorItem.UID))
                                floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                            Map.AddFloorItem(floorItem);
                            SendScreenSpawn(floorItem);
                        }
                    }
                }
            }

            #endregion [N~monster]
            #region [R~monster]

            if (ServerBase.Kernel.Rate(30))
            {
                if (Name == "[R~monster]")
                {
                    uint Uid = 0;
                    byte type = (byte)ServerBase.Kernel.Random.Next(1, 12);
                    switch (type)
                    {
                        case 1: Uid = 114303; break;
                        case 2: Uid = 114303; break;
                        case 3: Uid = 117303; break;
                        case 4: Uid = 117303; break;
                        case 5: Uid = 118303; break;
                        case 6: Uid = 118303; break;
                        case 7: Uid = 121263; break;
                        case 8: Uid = 121263; break;
                        case 9: Uid = 123303; break;
                        case 10: Uid = 123303; break;
                        case 11: Uid = 20302; break;
                        case 12: Uid = 20302; break;
                    }

                    if (Uid != 0)
                    {
                        ushort X = Owner.X, Y = Owner.Y;
                        Game.Map Map = Kernel.Maps[Owner.MapID];
                        if (Map.SelectCoordonates(ref X, ref Y))
                        {
                            Network.GamePackets.FloorItem floorItem = new Network.GamePackets.FloorItem(true);
                            floorItem.Item = new Network.GamePackets.ConquerItem(true);
                            floorItem.Item.Color = (Conquer_Online_Server.Game.Enums.Color)ServerBase.Kernel.Random.Next(4, 8);
                            floorItem.Item.ID = Uid;
                            floorItem.Item.MaximDurability = floorItem.Item.Durability = 65535;
                            floorItem.Item.UID = Network.GamePackets.ConquerItem.ItemUID.Next;
                            floorItem.ValueType = Network.GamePackets.FloorItem.FloorValueType.Item;
                            floorItem.ItemID = Uid;
                            floorItem.MapID = Owner.MapID;
                            floorItem.MapObjType = Game.MapObjectType.Item;
                            floorItem.X = X;
                            floorItem.Y = Y;
                            floorItem.Type = Network.GamePackets.FloorItem.Drop;
                            floorItem.OnFloor = Time32.Now;
                            floorItem.ItemColor = floorItem.Item.Color;
                            floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                            while (Map.Npcs.ContainsKey(floorItem.UID))
                                floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                            Map.AddFloorItem(floorItem);
                            SendScreenSpawn(floorItem);
                        }
                    }
                }
            }

            #endregion [R~monster]
            #region [U~monster]

            if (ServerBase.Kernel.Rate(30))
            {
                if (Name == "[U~monster]")
                {
                    uint Uid = 0;
                    byte type = (byte)ServerBase.Kernel.Random.Next(1, 12);
                    switch (type)
                    {
                        case 1: Uid = 114303; break;
                        case 2: Uid = 114303; break;
                        case 3: Uid = 117303; break;
                        case 4: Uid = 117303; break;
                        case 5: Uid = 118303; break;
                        case 6: Uid = 118303; break;
                        case 7: Uid = 121263; break;
                        case 8: Uid = 121263; break;
                        case 9: Uid = 123303; break;
                        case 10: Uid = 123303; break;
                        case 11: Uid = 20303; break;
                        case 12: Uid = 20303; break;
                    }

                    if (Uid != 0)
                    {
                        ushort X = Owner.X, Y = Owner.Y;
                        Game.Map Map = Kernel.Maps[Owner.MapID];
                        if (Map.SelectCoordonates(ref X, ref Y))
                        {
                            Network.GamePackets.FloorItem floorItem = new Network.GamePackets.FloorItem(true);
                            floorItem.Item = new Network.GamePackets.ConquerItem(true);
                            floorItem.Item.Color = (Conquer_Online_Server.Game.Enums.Color)ServerBase.Kernel.Random.Next(4, 8);
                            floorItem.Item.ID = Uid;
                            floorItem.Item.MaximDurability = floorItem.Item.Durability = 65535;
                            floorItem.Item.UID = Network.GamePackets.ConquerItem.ItemUID.Next;
                            floorItem.ValueType = Network.GamePackets.FloorItem.FloorValueType.Item;
                            floorItem.ItemID = Uid;
                            floorItem.MapID = Owner.MapID;
                            floorItem.MapObjType = Game.MapObjectType.Item;
                            floorItem.X = X;
                            floorItem.Y = Y;
                            floorItem.Type = Network.GamePackets.FloorItem.Drop;
                            floorItem.OnFloor = Time32.Now;
                            floorItem.ItemColor = floorItem.Item.Color;
                            floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                            while (Map.Npcs.ContainsKey(floorItem.UID))
                                floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                            Map.AddFloorItem(floorItem);
                            SendScreenSpawn(floorItem);
                        }
                    }
                }
            }

            #endregion [U~monster]
            #region [E~monster]

            if (ServerBase.Kernel.Rate(30))
            {
                if (Name == "[E~monster]")
                {
                    uint Uid = 0;
                    byte type = (byte)ServerBase.Kernel.Random.Next(1, 12);
                    switch (type)
                    {
                        case 1: Uid = 114303; break;
                        case 2: Uid = 114303; break;
                        case 3: Uid = 117303; break;
                        case 4: Uid = 117303; break;
                        case 5: Uid = 118303; break;
                        case 6: Uid = 118303; break;
                        case 7: Uid = 121263; break;
                        case 8: Uid = 121263; break;
                        case 9: Uid = 123303; break;
                        case 10: Uid = 123303; break;
                        case 11: Uid = 20304; break;
                        case 12: Uid = 20304; break;
                    }

                    if (Uid != 0)
                    {
                        ushort X = Owner.X, Y = Owner.Y;
                        Game.Map Map = Kernel.Maps[Owner.MapID];
                        if (Map.SelectCoordonates(ref X, ref Y))
                        {
                            Network.GamePackets.FloorItem floorItem = new Network.GamePackets.FloorItem(true);
                            floorItem.Item = new Network.GamePackets.ConquerItem(true);
                            floorItem.Item.Color = (Conquer_Online_Server.Game.Enums.Color)ServerBase.Kernel.Random.Next(4, 8);
                            floorItem.Item.ID = Uid;
                            floorItem.Item.MaximDurability = floorItem.Item.Durability = 65535;
                            floorItem.Item.UID = Network.GamePackets.ConquerItem.ItemUID.Next;
                            floorItem.ValueType = Network.GamePackets.FloorItem.FloorValueType.Item;
                            floorItem.ItemID = Uid;
                            floorItem.MapID = Owner.MapID;
                            floorItem.MapObjType = Game.MapObjectType.Item;
                            floorItem.X = X;
                            floorItem.Y = Y;
                            floorItem.Type = Network.GamePackets.FloorItem.Drop;
                            floorItem.OnFloor = Time32.Now;
                            floorItem.ItemColor = floorItem.Item.Color;
                            floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                            while (Map.Npcs.ContainsKey(floorItem.UID))
                                floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                            Map.AddFloorItem(floorItem);
                            SendScreenSpawn(floorItem);
                        }
                    }
                }
            }

            #endregion [E~monster]
            #region [S~monster]

            if (ServerBase.Kernel.Rate(30))
            {
                if (Name == "[S~monster]")
                {
                    uint Uid = 0;
                    byte type = (byte)ServerBase.Kernel.Random.Next(1, 12);
                    switch (type)
                    {
                        case 1: Uid = 20305; break;
                        case 2: Uid = 114303; break;
                        case 3: Uid = 20305; break;
                        case 4: Uid = 117303; break;
                        case 5: Uid = 20305; break;
                        case 6: Uid = 118303; break;
                        case 7: Uid = 20305; break;
                        case 8: Uid = 121263; break;
                        case 9: Uid = 20305; break;
                        case 10: Uid = 20305; break;
                        case 11: Uid = 20305; break;
                        case 12: Uid = 20305; break;
                    }

                    if (Uid != 0)
                    {
                        ushort X = Owner.X, Y = Owner.Y;
                        Game.Map Map = Kernel.Maps[Owner.MapID];
                        if (Map.SelectCoordonates(ref X, ref Y))
                        {
                            Network.GamePackets.FloorItem floorItem = new Network.GamePackets.FloorItem(true);
                            floorItem.Item = new Network.GamePackets.ConquerItem(true);
                            floorItem.Item.Color = (Conquer_Online_Server.Game.Enums.Color)ServerBase.Kernel.Random.Next(4, 8);
                            floorItem.Item.ID = Uid;
                            floorItem.Item.MaximDurability = floorItem.Item.Durability = 65535;
                            floorItem.Item.UID = Network.GamePackets.ConquerItem.ItemUID.Next;
                            floorItem.ValueType = Network.GamePackets.FloorItem.FloorValueType.Item;
                            floorItem.ItemID = Uid;
                            floorItem.MapID = Owner.MapID;
                            floorItem.MapObjType = Game.MapObjectType.Item;
                            floorItem.X = X;
                            floorItem.Y = Y;
                            floorItem.Type = Network.GamePackets.FloorItem.Drop;
                            floorItem.OnFloor = Time32.Now;
                            floorItem.ItemColor = floorItem.Item.Color;
                            floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                            while (Map.Npcs.ContainsKey(floorItem.UID))
                                floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                            Map.AddFloorItem(floorItem);
                            SendScreenSpawn(floorItem);
                        }
                    }
                }
            }

            #endregion [S~monster]
            #region [Satan]
            if (Name == "Satan")
            {
                {
                    uint Uid = 0;
                    byte type = (byte)ServerBase.Kernel.Random.Next(1, 20);
                    switch (type)
                    {
                        case 1: Uid = 800915; break;
                        case 2: Uid = 800915; break;
                        case 3: Uid = 800916; break;
                        case 4: Uid = 823053; break;
                        case 5: Uid = 823053; break;
                        case 6: Uid = 823054; break;
                        case 7: Uid = 823054; break;
                        case 8: Uid = 823055; break;
                        case 9: Uid = 823055; break;
                        case 10: Uid = 823056; break;
                        case 11: Uid = 823056; break;
                        case 12: Uid = 823057; break;
                        case 13: Uid = 823057; break;
                        case 14: Uid = 821027; break;
                        case 15: Uid = 821027; break;
                        case 16: Uid = 821028; break;
                        case 17: Uid = 821028; break;
                        case 18: Uid = 821032; break;
                        case 19: Uid = 821029; break;
                        case 20: Uid = 821031; break;


                    }
                    killer.Owner.Inventory.Add(Uid, 0, 1);
                    killer.ConquerPoints += 10000;
                    killer.Owner.Send(new Message("Congratulations! " + killer.Name + " Killd is Satan.!!Coded By mr Conquer_Online_Server", System.Drawing.Color.Azure, Message.Monster));
                    // return;
                }
            }
            #endregion [Satan]
            #endregion Box Satan life

            int Rand = Conquer_Online_Server.Kernel.Random.Next(1, 1200);
            if (Rand >= 529 && Rand <= 540)
            {
                uint randsouls = (uint)Conquer_Online_Server.Kernel.Random.Next(1, (int)Database.DROP_SOULS.Count_souls);
                uint ItemID = Database.DROP_SOULS.Souls[randsouls].item_id;
                var infos = Database.ConquerItemInformation.BaseInformations[ItemID];
                if (infos == null) return;
                ushort X = Owner.X, Y = Owner.Y;
                Game.Map Map = Kernel.Maps[Owner.MapID];
                if (Map.SelectCoordonates(ref X, ref Y))
                {
                    Network.GamePackets.FloorItem floorItem = new Network.GamePackets.FloorItem(true);
                    floorItem.Item = new Network.GamePackets.ConquerItem(true);
                    floorItem.Item.Color = (Conquer_Online_Server.Game.Enums.Color)Kernel.Random.Next(4, 8);
                    floorItem.Item.ID = ItemID;
                    floorItem.Item.MaximDurability = infos.Durability;

                    floorItem.Item.Durability = (ushort)(Kernel.Random.Next(infos.Durability / 10));

                    if (!Network.PacketHandler.IsEquipment(ItemID) && infos.ConquerPointsWorth == 0)
                    {
                        floorItem.Item.StackSize = 1;
                        floorItem.Item.MaxStackSize = infos.StackSize;
                    }
                    floorItem.Item.MobDropped = true;
                    floorItem.ValueType = Network.GamePackets.FloorItem.FloorValueType.Item;
                    floorItem.ItemID = ItemID;
                    floorItem.MapID = Owner.MapID;
                    floorItem.MapObjType = Game.MapObjectType.Item;
                    floorItem.X = X;
                    floorItem.Y = Y;
                    floorItem.Owner = killer.Owner;
                    floorItem.Type = Network.GamePackets.FloorItem.Drop;
                    floorItem.OnFloor = Time32.Now;
                    floorItem.ItemColor = floorItem.Item.Color;
                    floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                    while (Map.Npcs.ContainsKey(floorItem.UID))
                        floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                    Map.AddFloorItem(floorItem);
                    SendScreenSpawn(floorItem);
                }
            }


            byte morepercent = 0;
            byte lessrate = 0;
            if (killer.VIPLevel > 0)
                morepercent = (byte)(killer.VIPLevel * 5);
            if (killer.Level <= 10 && killer.MapID == 1002)
                morepercent += 100;
            if (killer.VIPLevel != 6 && killer.Class >= 40 && killer.Class <= 45)
                lessrate = 3;
            if (killer.VIPLevel != 6 && killer.Level >= 132 && killer.ContainsFlag(Network.GamePackets.Update.Flags.ShurikenVortex))
                lessrate = 3;

            if (Kernel.Rate(Constants.MoneyDropRate - lessrate + morepercent))
            {

                uint amount = (uint)Kernel.Random.Next((int)MinMoneyDropAmount, (int)MaxMoneyDropAmount);
                amount *= Constants.MoneyDropMultiple;

                if (amount > 1000000)
                    amount = 6000000;

                if (amount == 0)
                    return;
                if (killer.VIPLevel > 0)
                {
                    int percent = 10;
                    percent += killer.VIPLevel * 5 - 5;
                    amount += (uint)(amount * percent / 100);
                }
                if (killer.VIPLevel > 4)
                {
                    killer.Money += amount;
                    goto next;
                }
                uint ItemID = Network.PacketHandler.MoneyItemID(amount);
                ushort X = Owner.X, Y = Owner.Y;
                Game.Map Map = Kernel.Maps[Owner.MapID];
                if (Map.SelectCoordonates(ref X, ref Y))
                {
                    Network.GamePackets.FloorItem floorItem = new Network.GamePackets.FloorItem(true);
                    floorItem.ValueType = Network.GamePackets.FloorItem.FloorValueType.Money;
                    floorItem.Value = amount;
                    floorItem.ItemID = ItemID;
                    floorItem.MapID = Owner.MapID;
                    floorItem.MapObjType = Game.MapObjectType.Item;
                    floorItem.X = X;
                    floorItem.Y = Y;
                    floorItem.Type = Network.GamePackets.FloorItem.Drop;
                    floorItem.OnFloor = Time32.Now;
                    floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                    while (Map.Npcs.ContainsKey(floorItem.UID))
                        floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                    Map.AddFloorItem(floorItem);
                    SendScreenSpawn(floorItem);
                }
            }
        next:

            /*if (Kernel.Rate(Constants.ConquerPointsDropRate - lessrate))
            {
                uint amount = (uint)Level * 6;//serverdrop
                if (amount <50) amount = 150;
                if (amount > 200) amount = 150;
                if (killer != null && killer.Owner != null)
                {
                    if (killer.Owner.Map.BaseID == 1354)
                    {
                        amount = 30;
                    }
                }
                if (killer != null && killer.Owner != null)
                {
                    //killer.Owner.Send(Constants.PickupConquerPoints(amount));
                    //killer.ConquerPoints += (uint)amount;
                   // killer.Owner.Send(new Network.GamePackets.Message("You received " + amount + " ConquerPoints! for Kill " + Name + "", System.Drawing.Color.Red, Network.GamePackets.Message.TopLeft).ToArray());
                    return;
                }
                //  }

                #region CPBag

                //uint ItemID = 729911;
                //ushort X = Owner.X, Y = Owner.Y;
                //Game.Map Map = Kernel.Maps[Owner.MapID];
                //if (Map.SelectCoordonates(ref X, ref Y))
                //{
                //    Network.GamePackets.FloorItem floorItem = new Network.GamePackets.FloorItem(true);
                //    floorItem.ValueType = Network.GamePackets.FloorItem.FloorValueType.ConquerPoints;
                //    floorItem.Value = amount;
                //    floorItem.ItemID = ItemID;
                //    floorItem.MapID = Owner.MapID;
                //    floorItem.MapObjType = Game.MapObjectType.Item;
                //    floorItem.X = X;
                //    floorItem.Y = Y;
                //    floorItem.Type = Network.GamePackets.FloorItem.Drop;
                //    floorItem.OnFloor = Time32.Now;
                //    floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                //    while (Map.Npcs.ContainsKey(floorItem.UID))
                //        floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                //    Map.AddFloorItem(floorItem);
                //    SendScreenSpawn(floorItem);
                //}
                #endregion
            }*/
            if (Kernel.Rate(OwnItemRate + morepercent) && OwnItemID != 0)
            {
                if (killer.VIPLevel > 4)
                {
                    if (killer.Owner.Inventory.Count <= 39)
                    {
                        killer.Owner.Inventory.Add((uint)OwnItemID, 0, 1);
                        return;
                    }
                }
                var infos = Database.ConquerItemInformation.BaseInformations[(uint)OwnItemID];
                ushort X = Owner.X, Y = Owner.Y;
                Game.Map Map = Kernel.Maps[Owner.MapID];
                if (Map.SelectCoordonates(ref X, ref Y))
                {
                    Network.GamePackets.FloorItem floorItem = new Network.GamePackets.FloorItem(true);
                    floorItem.Item = new Network.GamePackets.ConquerItem(true);
                    floorItem.Item.Color = (Conquer_Online_Server.Game.Enums.Color)Kernel.Random.Next(4, 8);
                    floorItem.Item.ID = (uint)OwnItemID;
                    floorItem.Item.MaximDurability = infos.Durability;
                    if (!Network.PacketHandler.IsEquipment(OwnItemID) && infos.ConquerPointsWorth == 0)
                    {
                        floorItem.Item.StackSize = 1;
                        floorItem.Item.MaxStackSize = infos.StackSize;
                    }
                    floorItem.Item.MobDropped = true;
                    floorItem.ValueType = Network.GamePackets.FloorItem.FloorValueType.Item;
                    floorItem.ItemID = (uint)OwnItemID;
                    floorItem.MapID = Owner.MapID;
                    floorItem.MapObjType = Game.MapObjectType.Item;
                    floorItem.X = X;
                    floorItem.Y = Y;
                    floorItem.Owner = killer.Owner;
                    floorItem.Type = Network.GamePackets.FloorItem.Drop;
                    floorItem.OnFloor = Time32.Now;
                    floorItem.ItemColor = floorItem.Item.Color;
                    floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                    while (Map.Npcs.ContainsKey(floorItem.UID))
                        floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                    Map.AddFloorItem(floorItem);
                    SendScreenSpawn(floorItem);
                }
            }
            else if (Kernel.Rate(Constants.ItemDropRate + morepercent))
            {
                int quality = 3;
                for (int count = 0; count < 5; count++)
                {
                    int rate = int.Parse(Constants.ItemDropQualityRates[count]);
                    if (Kernel.Rate(rate, 1000))
                    {
                        quality = count + 5;
                        break;
                    }
                }
                int times = 50;
                byte lvl = Owner.Level;
                if (LabirinthDrop)
                    lvl = 20;
                List<uint> itemdroplist = ItemDropCache[lvl];
                if (Boss)
                    itemdroplist = SoulItemCache[lvl];
                if (itemdroplist != null)
                {
                retry:
                    times--;
                    int generateItemId = Kernel.Random.Next(itemdroplist.Count);
                    uint id = itemdroplist[generateItemId];
                    uint idb = id;
                    if (!Boss)
                    {
                        if (Database.ConquerItemInformation.BaseInformations[id].Level > 121 && times > 0)
                            goto retry;
                        id = (id / 10) * 10 + (uint)quality;
                    }
                    if (!Database.ConquerItemInformation.BaseInformations.ContainsKey(id))
                        id = idb;

                    if (killer.VIPLevel < 0)
                    {
                        if (killer.Owner.Inventory.Count <= 39)
                        {
                            if (id % 10 > 7)
                            {
                                killer.Owner.Inventory.Add(id, 0, 1);
                                return;
                            }
                        }
                    }
                    var infos = Database.ConquerItemInformation.BaseInformations[id];
                    ushort X = Owner.X, Y = Owner.Y;
                    Game.Map Map = Kernel.Maps[Owner.MapID];
                    if (Map.SelectCoordonates(ref X, ref Y))
                    {
                        Network.GamePackets.FloorItem floorItem = new Network.GamePackets.FloorItem(true);
                        floorItem.Item = new Network.GamePackets.ConquerItem(true);
                        floorItem.Item.Color = (Conquer_Online_Server.Game.Enums.Color)Kernel.Random.Next(4, 8);
                        floorItem.Item.ID = id;
                        floorItem.Item.MaximDurability = infos.Durability;
                        if (quality >= 6)
                            floorItem.Item.Durability = (ushort)(infos.Durability - Kernel.Random.Next(500));
                        else
                            floorItem.Item.Durability = (ushort)(Kernel.Random.Next(infos.Durability / 10));
                        if (!Network.PacketHandler.IsEquipment(id) && infos.ConquerPointsWorth == 0)
                        {
                            floorItem.Item.StackSize = 1;
                            floorItem.Item.MaxStackSize = infos.StackSize;
                        }
                        floorItem.Item.MobDropped = true;
                        floorItem.ValueType = Network.GamePackets.FloorItem.FloorValueType.Item;
                        floorItem.ItemID = id;
                        floorItem.MapID = Owner.MapID;
                        floorItem.MapObjType = Game.MapObjectType.Item;
                        floorItem.X = X;
                        floorItem.Y = Y;
                        floorItem.Type = Network.GamePackets.FloorItem.Drop;
                        floorItem.OnFloor = Time32.Now;
                        floorItem.Owner = killer.Owner;
                        floorItem.ItemColor = floorItem.Item.Color;
                        floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                        while (Map.Npcs.ContainsKey(floorItem.UID))
                            floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                        Map.AddFloorItem(floorItem);
                        SendScreenSpawn(floorItem);
                    }
                }
            }
            else if (Kernel.Rate(20 + morepercent))
            {
                if (HPPotionID == 0)
                    return;
                var infos = Database.ConquerItemInformation.BaseInformations[(uint)HPPotionID];
                ushort X = Owner.X, Y = Owner.Y;
                Game.Map Map = Kernel.Maps[Owner.MapID];
                if (Map.SelectCoordonates(ref X, ref Y))
                {
                    Network.GamePackets.FloorItem floorItem = new Network.GamePackets.FloorItem(true);
                    floorItem.Item = new Network.GamePackets.ConquerItem(true);
                    floorItem.Item.Color = (Conquer_Online_Server.Game.Enums.Color)Kernel.Random.Next(4, 8);
                    floorItem.Item.ID = (uint)HPPotionID;
                    floorItem.Item.MobDropped = true;
                    floorItem.Item.MaximDurability = infos.Durability;
                    floorItem.ValueType = Network.GamePackets.FloorItem.FloorValueType.Item;
                    if (!Network.PacketHandler.IsEquipment(HPPotionID))
                    {
                        floorItem.Item.StackSize = 1;
                        floorItem.Item.MaxStackSize = infos.StackSize;
                    }
                    floorItem.ItemID = (uint)HPPotionID;
                    floorItem.MapID = Owner.MapID;
                    floorItem.MapObjType = Game.MapObjectType.Item;
                    floorItem.X = X;
                    floorItem.Y = Y;
                    floorItem.Type = Network.GamePackets.FloorItem.Drop;
                    floorItem.OnFloor = Time32.Now;
                    floorItem.Owner = killer.Owner;
                    floorItem.ItemColor = floorItem.Item.Color;
                    floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                    while (Map.Npcs.ContainsKey(floorItem.UID))
                        floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                    Map.AddFloorItem(floorItem);
                    SendScreenSpawn(floorItem);
                }
            }
            else if (Kernel.Rate(20 + morepercent))
            {
                if (MPPotionID == 0)
                    return;
                var infos = Database.ConquerItemInformation.BaseInformations[(uint)MPPotionID];
                ushort X = Owner.X, Y = Owner.Y;
                Game.Map Map = Kernel.Maps[Owner.MapID];
                if (Map.SelectCoordonates(ref X, ref Y))
                {
                    Network.GamePackets.FloorItem floorItem = new Network.GamePackets.FloorItem(true);
                    floorItem.Item = new Network.GamePackets.ConquerItem(true);
                    floorItem.Item.Color = (Conquer_Online_Server.Game.Enums.Color)Kernel.Random.Next(4, 8);
                    floorItem.Item.ID = (uint)MPPotionID;
                    floorItem.Item.MaximDurability = infos.Durability;
                    floorItem.Item.MobDropped = true;
                    floorItem.ValueType = Network.GamePackets.FloorItem.FloorValueType.Item;
                    if (!Network.PacketHandler.IsEquipment(MPPotionID))
                    {
                        floorItem.Item.StackSize = 1;
                        floorItem.Item.MaxStackSize = infos.StackSize;
                    }
                    floorItem.ItemID = (uint)MPPotionID;
                    floorItem.MapID = Owner.MapID;
                    floorItem.MapObjType = Game.MapObjectType.Item;
                    floorItem.X = X;
                    floorItem.Y = Y;
                    floorItem.Type = Network.GamePackets.FloorItem.Drop;
                    floorItem.OnFloor = Time32.Now;
                    floorItem.Owner = killer.Owner;
                    floorItem.ItemColor = floorItem.Item.Color;
                    floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                    while (Map.Npcs.ContainsKey(floorItem.UID))
                        floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                    Map.AddFloorItem(floorItem);
                    SendScreenSpawn(floorItem);
                }
            }
            else
            {
                foreach (SpecialItemDrop sitem in SpecialItemDropList)
                {
                    if (sitem.Map != 0 && Owner.MapID != sitem.Map)
                        continue;
                    if (Kernel.Rate(sitem.Rate + morepercent, sitem.Discriminant))
                    {
                        if (killer.VIPLevel < 0)
                        {
                            if (killer.Owner.Inventory.Count <= 39)
                            {
                                killer.Owner.Inventory.Add((uint)sitem.ItemID, 0, 1);
                                return;
                            }
                        }
                        var infos = Database.ConquerItemInformation.BaseInformations[(uint)sitem.ItemID];
                        ushort X = Owner.X, Y = Owner.Y;
                        Game.Map Map = Kernel.Maps[Owner.MapID];
                        if (Map.SelectCoordonates(ref X, ref Y))
                        {
                            Network.GamePackets.FloorItem floorItem = new Network.GamePackets.FloorItem(true);
                            floorItem.Item = new Network.GamePackets.ConquerItem(true);
                            floorItem.Item.Color = (Conquer_Online_Server.Game.Enums.Color)Kernel.Random.Next(4, 8);
                            floorItem.Item.ID = (uint)sitem.ItemID;
                            floorItem.Item.MaximDurability = infos.Durability;
                            floorItem.Item.MobDropped = true;
                            if (!Network.PacketHandler.IsEquipment(sitem.ItemID) && infos.ConquerPointsWorth == 0)
                            {
                                floorItem.Item.StackSize = 1;
                                floorItem.Item.MaxStackSize = infos.StackSize;
                            }
                            floorItem.ValueType = Network.GamePackets.FloorItem.FloorValueType.Item;
                            floorItem.ItemID = (uint)sitem.ItemID;
                            floorItem.MapID = Owner.MapID;
                            floorItem.MapObjType = Game.MapObjectType.Item;
                            floorItem.X = X;
                            floorItem.Y = Y;
                            floorItem.Type = Network.GamePackets.FloorItem.Drop;
                            floorItem.OnFloor = Time32.Now;
                            floorItem.ItemColor = floorItem.Item.Color;
                            floorItem.Owner = killer.Owner;
                            floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                            while (Map.Npcs.ContainsKey(floorItem.UID))
                                floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                            Map.AddFloorItem(floorItem);
                            SendScreenSpawn(floorItem);
                            break;
                        }
                    }
                }
            }
        }

        public const int ReviverID = 9879;

        public static SafeDictionary<uint, MonsterInformation> MonsterInformations = new SafeDictionary<uint, MonsterInformation>(8000);

        public static void Load()
        {
            using (var cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("specialdrops"))
            using (var rdr = cmd.CreateReader())
            {
                while (rdr.Read())
                {
                    SpecialItemDrop sitem = new SpecialItemDrop();
                    sitem.ItemID = rdr.ReadInt32("itemid");
                    sitem.Rate = rdr.ReadInt32("rate");
                    sitem.Discriminant = rdr.ReadInt32("discriminant");
                    sitem.Map = rdr.ReadInt32("map");
                    SpecialItemDropList.Add(sitem);
                }
            }
            using (var command = new MySqlCommand(MySqlCommandType.SELECT))
            {
                command.Select("monsterinfos");
                using (var reader = command.CreateReader())
                {
                    while (reader.Read())
                    {
                        MonsterInformation mf = new MonsterInformation();
                        mf.ID = reader.ReadUInt32("id");
                        mf.Name = reader.ReadString("name");
                        mf.Type = reader.ReadUInt32("type");
                        mf.Mesh = reader.ReadUInt16("lookface");
                        mf.Level = reader.ReadByte("level");
                        mf.Hitpoints = reader.ReadUInt32("life");
                        IniFile IniFile = new IniFile(Constants.MonstersPath);
                        if (IniFile.ReadString(mf.Name, "MaxLife") != "")
                        {
                            if (uint.Parse(IniFile.ReadString(mf.Name, "MaxLife")) != 0)
                            {
                                mf.Hitpoints = uint.Parse(IniFile.ReadString(mf.Name, "MaxLife"));
                                byte boss = byte.Parse(IniFile.ReadString(mf.Name, "Boss"));
                                if (boss == 0)
                                    mf.Boss = false;
                                else mf.Boss = true;
                            }
                        }
                        mf.Guard = mf.Name.Contains("Guard");
                        mf.Reviver = mf.ID == ReviverID;
                        mf.ViewRange = reader.ReadUInt16("view_range");
                        mf.AttackRange = reader.ReadUInt16("attack_range");
                        mf.Defence = reader.ReadUInt16("defence");
                        mf.AttackType = reader.ReadByte("attack_user");
                        mf.MinAttack = reader.ReadUInt32("attack_min");
                        mf.MaxAttack = reader.ReadUInt32("attack_max");
                        mf.SpellID = reader.ReadUInt16("magic_type");
                        mf.MoveSpeed = reader.ReadInt32("move_speed");
                        mf.RunSpeed = reader.ReadInt32("run_speed");
                        mf.OwnItemID = reader.ReadInt32("ownitem");
                        mf.HPPotionID = reader.ReadInt32("drop_hp");
                        mf.MPPotionID = reader.ReadInt32("drop_mp");
                        mf.OwnItemRate = reader.ReadInt32("ownitemrate");
                        mf.AttackSpeed = reader.ReadInt32("attack_speed");
                        mf.ExtraExperience = reader.ReadUInt32("extra_exp");
                       /* uint MoneyDropAmount = reader.ReadUInt16("level");
                        if (MoneyDropAmount != 0)
                        {
                            mf.MaxMoneyDropAmount = MoneyDropAmount * 25;
                            if (mf.MaxMoneyDropAmount != 0)
                                mf.MinMoneyDropAmount = 1;
                        }*/
                        if (mf.MoveSpeed <= 500)
                            mf.MoveSpeed += 500;
                        if (mf.AttackSpeed <= 500)
                            mf.AttackSpeed += 500;
                        MonsterInformations.Add(mf.ID, mf);
                        byte lvl = mf.Level;
                        if (mf.Name == "Slinger" ||
                            mf.Name == "GoldGhost" ||
                            mf.Name == "AgileRat" ||
                            mf.Name == "Bladeling" ||
                            mf.Name == "BlueBird" ||
                            mf.Name == "BlueFiend" ||
                            mf.Name == "MinotaurL120")
                        {
                            mf.LabirinthDrop = true;
                            lvl = 20;
                        }
                        if (!ItemDropCache.ContainsKey(lvl))
                        {
                            List<uint> itemdroplist = new List<uint>();
                            foreach (ConquerItemBaseInformation itemInfo in ConquerItemInformation.BaseInformations.Values)
                            {

                                if (itemInfo.ID >= 800000 && itemInfo.ID <= 824014)
                                    continue;
                                ushort position = Network.PacketHandler.ItemPosition(itemInfo.ID);
                                if (Network.PacketHandler.IsArrow(itemInfo.ID) || itemInfo.Level == 0 || itemInfo.Level > 121)
                                    continue;
                                if (position < 9 && position != 7)
                                {
                                    if (itemInfo.Level == 100)
                                        if (itemInfo.Name.Contains("Dress"))
                                            continue;
                                    if (itemInfo.Level > 121)
                                        continue;
                                    int diff = (int)lvl - (int)itemInfo.Level;
                                    if (!(diff > 10 || diff < -10))
                                    {
                                        itemdroplist.Add(itemInfo.ID);
                                    }
                                }
                                if (position == 10 || position == 11 && lvl >= 70)
                                    itemdroplist.Add(itemInfo.ID);
                            }
                            ItemDropCache.Add(lvl, itemdroplist);
                        }
                        if (mf.Boss)
                        {
                            List<uint> itemdroplist = new List<uint>();
                            foreach (ConquerItemBaseInformation itemInfo in ConquerItemInformation.BaseInformations.Values)
                            {
                                if (itemInfo.ID >= 800000 && itemInfo.ID <= 824014)
                                {
                                    if (itemInfo.PurificationLevel <= 3)
                                    {
                                        int diff = (int)mf.Level - (int)itemInfo.Level;
                                        if (!(diff > 10 || diff < -10))
                                        {
                                            itemdroplist.Add(itemInfo.ID);
                                        }
                                    }
                                }
                            }
                            SoulItemCache.Add(lvl, itemdroplist);
                        }
                    }
                }
            }
            Console.WriteLine("Monster information loaded.");
            Console.WriteLine("Monster drops generated.");
        }

        public MonsterInformation Copy()
        {
            MonsterInformation mf = new MonsterInformation();
            mf.ID = this.ID;
            mf.Name = this.Name;
            mf.Mesh = this.Mesh;
            mf.Level = this.Level;
            mf.Hitpoints = this.Hitpoints;
            mf.ViewRange = this.ViewRange;
            mf.AttackRange = this.AttackRange;
            mf.AttackType = this.AttackType;
            mf.MinAttack = this.MinAttack;
            mf.MaxAttack = this.MaxAttack;
            mf.SpellID = this.SpellID;
            mf.MoveSpeed = this.MoveSpeed;
            mf.RunSpeed = this.RunSpeed;
            mf.AttackSpeed = this.AttackSpeed;
            mf.BoundX = this.BoundX;
            mf.BoundY = this.BoundY;
            mf.BoundCX = this.BoundCX;
            mf.BoundCY = this.BoundCY;
            mf.RespawnTime = this.RespawnTime;
            mf.ExtraExperience = this.ExtraExperience;
            mf.MaxMoneyDropAmount = this.MaxMoneyDropAmount;
            mf.MinMoneyDropAmount = this.MinMoneyDropAmount;
            mf.OwnItemID = this.OwnItemID;
            mf.HPPotionID = this.HPPotionID;
            mf.MPPotionID = this.MPPotionID;
            mf.OwnItemRate = this.OwnItemRate;
            mf.LabirinthDrop = this.LabirinthDrop;
            mf.Boss = this.Boss;
            mf.Guard = this.Guard;
            mf.Defence = this.Defence;
            mf.Reviver = this.Reviver;
            return mf;
        }
    }
}
