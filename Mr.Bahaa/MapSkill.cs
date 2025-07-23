//Project by BaussHacker aka. L33TS
using System;
using System.Collections.Generic;
using Conquer_Online_Server.Game;
using Conquer_Online_Server.Network.GamePackets;
using Conquer_Online_Server.Client;
using Conquer_Online_Server.Interfaces;
using Conquer_Online_Server;

namespace ProjectX_V3_Game.Data.Skills
{
    /// <summary>
    /// Description of MapSkill.
    /// </summary>
    [Serializable()]
    public class MapSkill
    {
        public MapSkill()
        {

        }

        public string SafeAreaEffect = "dispel";
        public string DestructionEffect = "bombarrow";

        public int DamageEffect = 0;
        public int PercentTageEffect = -1;

        public Map map;

        public GameClient client;

        private List<System.Drawing.Point> DestructionAreas;
        private List<System.Drawing.Point> DestructionEffectAreas;
        private List<System.Drawing.Point> SafeSpots;

        public Entity Killer;

        public bool Shake = true; // if true, the screen will shake
        public bool Dark = true; // if true darkness will happen on the screen
        public bool Zoom = true; // if true the screen will zoom

        public ushort Range;

        public int EffectRatio = 2;

        public void ExecuteStart(GameClient c, ushort StartX, ushort StartY)
        {
            client = c;
            if (Range < 10)
                return;

            if (DestructionAreas != null)
                DestructionAreas.Clear();
            else
                DestructionAreas = new List<System.Drawing.Point>();


            if (SafeSpots != null)
                SafeSpots.Clear();
            else
                SafeSpots = new List<System.Drawing.Point>();

            if (DestructionEffectAreas != null)
                DestructionEffectAreas.Clear();
            else
                DestructionEffectAreas = new List<System.Drawing.Point>();



            for (int i = 0; i < (Range / 4); i++)
            {
                ushort X = (ushort)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next((int)StartX, (int)(StartX + Range));
                ushort Y = (ushort)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next((int)StartY, (int)(StartY + Range));
                System.Drawing.Point p = new System.Drawing.Point((int)X, (int)Y);
                if (SafeSpots.Contains(p))
                    i--;
                else
                    SafeSpots.Add(p);
            }

            for (int i = 0; i < Range / 4; i++)
            {
                ushort X = (ushort)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next((int)(StartX - Range), (int)StartX);
                ushort Y = (ushort)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next((int)(StartY - Range), (int)StartY);
                System.Drawing.Point p = new System.Drawing.Point((int)X, (int)Y);
                if (SafeSpots.Contains(p))
                    i--;
                else
                    SafeSpots.Add(p);
            }

            for (int i = 0; i < Range / EffectRatio; i++)
            {
                ushort X = (ushort)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next((int)StartX, (int)(StartX + Range));
                ushort Y = (ushort)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next((int)StartY, (int)(StartY + Range));
                System.Drawing.Point p = new System.Drawing.Point((int)X, (int)Y);
                if (DestructionEffectAreas.Contains(p))
                    i--;
                else
                    DestructionEffectAreas.Add(p);
            }
            for (int i = 0; i < Range / EffectRatio; i++)
            {
                ushort X = (ushort)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next((int)(StartX - Range), (int)StartX);
                ushort Y = (ushort)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next((int)(StartY - Range), (int)StartY);
                System.Drawing.Point p = new System.Drawing.Point((int)X, (int)Y);
                if (DestructionEffectAreas.Contains(p))
                    i--;
                else
                    DestructionEffectAreas.Add(p);
            }


            for (ushort x = (ushort)(StartX - Range); x < (StartX + Range); x++)
            {
                for (ushort y = (ushort)(StartY - Range); y < (StartY + Range); y++)
                {
                    System.Drawing.Point p = new System.Drawing.Point((int)x, (int)y);
                    if (!SafeSpots.Contains(p))
                        DestructionAreas.Add(p);
                }
            }

            for (int i = 0; i < Range; i++)
            {
                ushort X = (ushort)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next((int)(StartX - Range), (int)(StartX + Range));
                ushort Y = (ushort)ProjectX_V3_Lib.ThreadSafe.RandomGenerator.Generator.Next((int)(StartY - Range), (int)(StartY + Range));
                System.Drawing.Point p = new System.Drawing.Point((int)X, (int)Y);
                if (!SafeSpots.Contains(p))
                    DestructionAreas.Add(p);
            }

            for (int i = 0; i < 5; i++)
            {
                ProjectX_V3_Lib.Threading.DelayedTask.StartDelayedTask(() =>
                {
                    foreach (System.Drawing.Point p in SafeSpots)
                    {
                        _String str = new _String(true);
                        str.TextsCount = 1;
                        str.PositionX = (ushort)p.X;
                        str.PositionY = (ushort)p.Y;
                        str.Type = _String.MapEffect;
                        str.Texts.Add("dispel");
                        foreach (IMapObject MapObject in client.Screen.Objects)
                        {
                            if ((MapObject != null) && (MapObject.MapObjType == MapObjectType.Player))
                            {
                                if (Kernel.GetDistance(MapObject.X, MapObject.Y, (ushort)p.X, (ushort)p.Y) <= 40)
                                {
                                    MapObject.Owner.Send(str);
                                }
                            }
                        }
                        client.Send(str);


                    }
                }, 1000 * i, 0);
            }
            ProjectX_V3_Lib.Threading.DelayedTask.StartDelayedTask(ExecuteDestruction, 10000, 0);
        }

        private void ExecuteDestruction()
        {
            foreach (System.Drawing.Point p in DestructionEffectAreas)
            {
                _String str = new _String(true);
                str.TextsCount = 1;
                str.PositionX = (ushort)p.X;
                str.PositionY = (ushort)p.Y;
                str.Type = _String.MapEffect;
                str.Texts.Add("bombarrow");
                foreach (IMapObject MapObject in client.Screen.Objects)
                {
                    if ((MapObject != null) && (MapObject.MapObjType == MapObjectType.Player))
                    {
                        if (Kernel.GetDistance(MapObject.X, MapObject.Y, (ushort)p.X, (ushort)p.Y) <= 40)
                        {
                            MapObject.Owner.Send(str);

                        }
                    }
                }
                client.Send(str);
            }

            foreach (System.Drawing.Point p in DestructionAreas)
            {
                var map = client.Map;
                if (!map.Floor[(ushort)p.X, (ushort)p.X, MapObjectType.Item, null]) return;
                FloorItem floorItem = new FloorItem(true);
                floorItem.ItemID = 46;
                floorItem.MapID = client.Entity.MapID;
                floorItem.Type = FloorItem.Effect;
                floorItem.X = (ushort)p.X;
                floorItem.Y = (ushort)p.Y;
                floorItem.OnFloor = Time32.Now;
                floorItem.Owner = client;
                while (map.Npcs.ContainsKey(floorItem.UID))
                    floorItem.UID = FloorItem.FloorUID.Next;
                map.AddFloorItem(floorItem);

                foreach (IMapObject MapObject in client.Screen.Objects)
                {
                    if ((MapObject != null) && (MapObject.MapObjType == MapObjectType.Player))
                    {
                        if (Kernel.GetDistance(MapObject.X, MapObject.Y, (ushort)p.X, (ushort)p.Y) <= 40)
                        {
                            MapObject.Owner.SendScreenSpawn(floorItem, true);

                        }
                    }
                }
                client.SendScreenSpawn(floorItem, true);
            }
            #region Shake, Dark, Zoom
            List<uint> UsedUIDs = new List<uint>();
            if (Shake || Dark || Zoom)
            {
                FloorItem floorItem = new FloorItem(true);
                floorItem.Shake = Shake;
                floorItem.Darkness = Dark;
                floorItem.Zoom = Zoom;
                floorItem.AppendFlags();
                floorItem.Type = 0xd;
                foreach (System.Drawing.Point p in DestructionAreas)
                {
                    foreach (IMapObject MapObject in client.Screen.Objects)
                    {
                        if ((MapObject != null) && (MapObject.MapObjType == MapObjectType.Player))
                        {
                            if (Kernel.GetDistance(MapObject.X, MapObject.Y, (ushort)p.X, (ushort)p.Y) <= 40 && !UsedUIDs.Contains(MapObject.UID))
                            {
                                floorItem.X = MapObject.X;
                                floorItem.Y = MapObject.Y;
                                MapObject.Owner.Send(floorItem);

                                UsedUIDs.Add(MapObject.UID);
                            }
                        }
                    }
                    client.Send(floorItem);
                }


            }
            #endregion

            foreach (System.Drawing.Point p in DestructionAreas)
            {
                foreach (IMapObject MapObject in client.Screen.Objects)
                {
                    if ((MapObject != null) && (MapObject.MapObjType == MapObjectType.Player))
                    {
                        System.Drawing.Point p2 = new System.Drawing.Point((int)MapObject.X, (int)MapObject.Y);
                        if (!SafeSpots.Contains(p2) && p == p2)
                        {
                            Entity target = (MapObject as Entity);

                            int damage = DamageEffect;
                            if (PercentTageEffect != -1 && target.Hitpoints > PercentTageEffect)
                            {
                                damage = (((int)target.Hitpoints / 100) * PercentTageEffect);
                            }

                            if (damage > 0)
                            {
                                target.Hitpoints -= (uint)damage;
                                if (target.Hitpoints <= 0)
                                {
                                    var interact = new Attack(true);
                                    interact.Effect1 = Attack.AttackEffects1.None;
                                    interact.AttackType = Attack.Magic;
                                    interact.Damage = (uint)damage;
                                    interact.Attacker = Killer.UID;
                                    interact.Attacked = target.UID;
                                    interact.Decoded = true;

                                    new Conquer_Online_Server.Game.Attacking.Handle(interact, Killer, target);

                                }
                            }
                        }
                    }
                    if ((client != null) && (client.Entity.MapObjType == MapObjectType.Player))
                    {
                        System.Drawing.Point p2 = new System.Drawing.Point((int)client.Entity.X, (int)client.Entity.Y);
                        if (!SafeSpots.Contains(p2) && p == p2)
                        {
                            // Entity target = (MapObject as Entity);

                            int damage = DamageEffect;
                            if (PercentTageEffect != -1 && client.Entity.Hitpoints > PercentTageEffect)
                            {
                                damage = (((int)client.Entity.Hitpoints / 100) * PercentTageEffect);
                            }

                            if (damage > 0)
                            {
                                client.Entity.Hitpoints -= (uint)damage;
                                if (client.Entity.Hitpoints <= 0)
                                {
                                    var interact = new Attack(true);
                                    interact.Effect1 = Attack.AttackEffects1.None;
                                    interact.AttackType = Attack.Magic;
                                    interact.Damage = (uint)damage;
                                    interact.Attacker = Killer.UID;
                                    interact.Attacked = client.Entity.UID;
                                    interact.Decoded = true;

                                    new Conquer_Online_Server.Game.Attacking.Handle(interact, Killer, client.Entity);

                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
