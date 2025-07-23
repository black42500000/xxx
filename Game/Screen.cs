using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using Conquer_Online_Server.Interfaces;
using Conquer_Online_Server.Client;

using Conquer_Online_Server.Network.GamePackets;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Generic;

namespace Conquer_Online_Server.Game
{
    public class Screen
    {
        private static TimerRule<GameClient> MonsterBuffers, Guards, AliveMonsters, Items;
        public static void CreateTimerFactories()
        {
            MonsterBuffers = new TimerRule<GameClient>(monsterBuffersCallback, 500);
            Guards = new TimerRule<GameClient>(guardsCallback, 700);
            AliveMonsters = new TimerRule<GameClient>(aliveMonstersCallback, 500);
            Items = new TimerRule<GameClient>(itemsCallback, 1000);
        }
        private static void monsterBuffersCallback(GameClient client, int time)
        {
            if (!client.Socket.Alive)
            {
                client.Screen.DisposeTimers();
                return;
            }
            if (client.Entity == null)
                return;
            if (client.Map == null)
                return;
            if (client.Map.FreezeMonsters)
                return;

            #region Stamina
            if (client.Entity.StaminaStamp.Next(300, time: time))
            {
                if (client.Vigor < client.Entity.ExtraVigor)
                {
                    client.Vigor += (ushort)(3 + (client.Entity.Action == Game.Enums.ConquerAction.Sit ? 2 : 0));

                    {
                        Vigor vigor = new Vigor(true);
                        vigor.Amount = client.Vigor;
                        vigor.Send(client);
                    }
                }
                if (!client.Entity.ContainsFlag(Update.Flags.Ride) && !client.Entity.ContainsFlag(Update.Flags.Fly) || client.Equipment.TryGetItem(18) != null)
                {
                    int limit = 0;
                    if (client.Entity.HeavenBlessing > 0)
                        limit = 50;
                    if (client.Entity.Stamina != 100 + limit)
                    {
                        if (client.Entity.Action == Enums.ConquerAction.Sit)
                        {
                            if (client.Entity.Stamina <= 93 + limit)
                            {
                                client.Entity.Stamina += 7;
                            }
                            else
                            {
                                if (client.Entity.Stamina != 100 + limit)
                                    client.Entity.Stamina = (byte)(100 + limit);
                            }
                        }
                        else
                        {
                            if (client.Entity.Stamina <= 97 + limit)
                            {
                                client.Entity.Stamina += 3;
                            }
                            else
                            {
                                if (client.Entity.Stamina != 100 + limit)
                                    client.Entity.Stamina = (byte)(100 + limit);
                            }
                        }
                    }
                    client.Entity.StaminaStamp = new Time32(time);
                }
            }
            #endregion
            if (client.Map.ID == 8880 || client.Map.ID == 8881)
            {
                SafeDictionary<uint, PokerTable> ToRem = new SafeDictionary<uint, PokerTable>(40);
                foreach (PokerTable T in PokerTables.Values)
                {
                    if (client.Map.ID == T.Map)
                        if (Kernel.GetDistance(T.X, T.Y, client.Entity.X, client.Entity.Y) > Constants.nScreenDistance)
                            ToRem.Add(T.Id, T);
                }
                foreach (PokerTable T in ToRem.Values)
                    if (PokerTables.ContainsKey(T.Id))
                        PokerTables.Remove(T.Id);
            }
            foreach (IMapObject obj in client.Screen.Objects)
            {
                if (obj != null)
                {
                    if (obj.MapObjType == MapObjectType.Monster)
                    {
                        Entity monster = obj as Entity;
                        if (monster == null) continue;

                        if (monster.ContainsFlag(Network.GamePackets.Update.Flags.Stigma))
                        {
                            if (monster.StigmaStamp.AddSeconds(monster.StigmaTime).Next(time: time) || monster.Dead)
                            {
                                monster.StigmaTime = 0;
                                monster.StigmaIncrease = 0;
                                monster.RemoveFlag(Update.Flags.Stigma);
                            }
                        }
                        if (monster.ContainsFlag(Update.Flags.Dodge))
                        {
                            if (monster.DodgeStamp.AddSeconds(monster.DodgeTime).Next(time: time) || monster.Dead)
                            {
                                monster.DodgeTime = 0;
                                monster.DodgeIncrease = 0;
                                monster.RemoveFlag(Network.GamePackets.Update.Flags.Dodge);
                            }
                        }
                        if (monster.ContainsFlag(Update.Flags.Invisibility))
                        {
                            if (monster.InvisibilityStamp.AddSeconds(monster.InvisibilityTime).Next(time: time) || monster.Dead)
                            {
                                monster.RemoveFlag(Update.Flags.Invisibility);
                            }
                        }
                        if (monster.ContainsFlag(Update.Flags.StarOfAccuracy))
                        {
                            if (monster.StarOfAccuracyTime != 0)
                            {
                                if (monster.StarOfAccuracyStamp.AddSeconds(monster.StarOfAccuracyTime).Next(time: time) || monster.Dead)
                                {
                                    monster.RemoveFlag(Update.Flags.StarOfAccuracy);
                                }
                            }
                            else
                            {
                                if (monster.AccuracyStamp.AddSeconds(monster.AccuracyTime).Next(time: time) || monster.Dead)
                                {
                                    monster.RemoveFlag(Update.Flags.StarOfAccuracy);
                                }
                            }
                        }
                        if (monster.ContainsFlag(Update.Flags.MagicShield))
                        {
                            if (monster.MagicShieldTime != 0)
                            {
                                if (monster.MagicShieldStamp.AddSeconds(monster.MagicShieldTime).Next(time: time) || monster.Dead)
                                {
                                    monster.MagicShieldIncrease = 0;
                                    monster.MagicShieldTime = 0;
                                    monster.RemoveFlag(Update.Flags.MagicShield);
                                }
                            }
                            else
                            {
                                if (monster.ShieldStamp.AddSeconds(monster.ShieldTime).Next(time: time) || monster.Dead)
                                {
                                    monster.ShieldIncrease = 0;
                                    monster.ShieldTime = 0;
                                    monster.RemoveFlag(Update.Flags.MagicShield);
                                }
                            }
                        }
                        if (monster.Dead || monster.Killed)
                        {
                            if (!monster.ContainsFlag(Update.Flags.Ghost) || monster.Killed)
                            {
                                monster.Killed = false;
                                monster.MonsterInfo.InSight = 0;
                                monster.AddFlag(Network.GamePackets.Update.Flags.Ghost);
                                monster.AddFlag(Network.GamePackets.Update.Flags.Dead);
                                monster.AddFlag(Network.GamePackets.Update.Flags.FadeAway);
                                Network.GamePackets.Attack attack = new Network.GamePackets.Attack(true);
                                attack.Attacker = monster.Killer.UID;
                                attack.Attacked = monster.UID;
                                attack.AttackType = Network.GamePackets.Attack.Kill;
                                attack.X = monster.X;
                                attack.Y = monster.Y;
                                client.Map.Floor[monster.X, monster.Y, MapObjectType.Monster, monster] = true;
                                attack.KOCount = ++monster.Killer.KOCount;
                                if (monster.Killer.EntityFlag == EntityFlag.Player)
                                {
                                    monster.MonsterInfo.ExcludeFromSend = monster.Killer.UID;
                                    monster.Killer.Owner.Send(attack);
                                }
                                monster.MonsterInfo.SendScreen(attack);
                                monster.MonsterInfo.ExcludeFromSend = 0;
                            }
                            if (monster.DeathStamp.AddSeconds(4).Next(time: time))
                            {
                                Data data = new Data(true);
                                data.UID = monster.UID;
                                data.ID = Network.GamePackets.Data.RemoveEntity;
                                monster.MonsterInfo.SendScreen(data);
                            }
                        }
                    }
                }
            }
        }
        private static void guardsCallback(GameClient client, int time)
        {
            if (!client.Socket.Alive)
            {
                client.Screen.DisposeTimers();
                return;
            }
            if (client.Entity == null)
                return;
            if (client.Map == null)
                return;
            if (client.Map.FreezeMonsters)
                return;

            Time32 Now = new Time32(time);
            foreach (IMapObject obj in client.Screen.Objects)
            {
                if (obj != null)
                {
                    if (obj.MapObjType == MapObjectType.Monster)
                    {
                        Entity monster = obj as Entity;
                        if (monster.Companion) continue;
                        if (monster.Dead || monster.Killed) continue;

                        if (monster.MonsterInfo.Guard)
                        {
                            if (Now >= monster.MonsterInfo.LastMove.AddMilliseconds(monster.MonsterInfo.MinimumSpeed))
                            {
                                if (monster.MonsterInfo.InSight == 0)
                                {
                                    if (monster.X != monster.MonsterInfo.BoundX || monster.Y != monster.MonsterInfo.BoundY)
                                    {
                                        monster.X = monster.MonsterInfo.BoundX;
                                        monster.Y = monster.MonsterInfo.BoundY;
                                        TwoMovements jump = new TwoMovements();
                                        jump.X = monster.MonsterInfo.BoundX;
                                        jump.Y = monster.MonsterInfo.BoundY;
                                        jump.EntityCount = 1;
                                        jump.FirstEntity = monster.UID;
                                        jump.MovementType = TwoMovements.Jump;
                                        client.SendScreen(jump, true);
                                    }
                                    //if (monster.MonsterInfo.Type != 2)
                                    // client.Send(new Network.GamePackets.Message("Welcome To MrBahaa.", client.Entity.Name, monster.MonsterInfo.Name, System.Drawing.Color.White, Message.Talk));                                   
                                                        
                                    if (client.Entity.ContainsFlag(Update.Flags.FlashingName))
                                        monster.MonsterInfo.InSight = client.Entity.UID;
                                }
                                else
                                {
                                    if (client.Entity.ContainsFlag(Update.Flags.FlashingName))
                                    {
                                        if (monster.MonsterInfo.InSight == client.Entity.UID)
                                        {
                                            if (!client.Entity.Dead)
                                            {
                                                if (Now >= monster.MonsterInfo.LastMove.AddMilliseconds(monster.MonsterInfo.AttackSpeed))
                                                {
                                                    short distance = Kernel.GetDistance(monster.X, monster.Y, client.Entity.X, client.Entity.Y);

                                                    if (distance <= monster.MonsterInfo.AttackRange)
                                                    {
                                                        monster.MonsterInfo.LastMove = Time32.Now;
                                                        new Game.Attacking.Handle(null, monster, client.Entity);
                                                    }
                                                    else
                                                    {
                                                        if (distance <= monster.MonsterInfo.ViewRange)
                                                        {
                                                            TwoMovements jump = new TwoMovements();
                                                            jump.X = client.Entity.X;
                                                            jump.Y = client.Entity.Y;
                                                            monster.X = client.Entity.X;
                                                            monster.Y = client.Entity.Y;
                                                            jump.EntityCount = 1;
                                                            jump.FirstEntity = monster.UID;
                                                            jump.MovementType = TwoMovements.Jump;
                                                            client.SendScreen(jump, true);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (monster.MonsterInfo.InSight == client.Entity.UID)
                                        {
                                            monster.MonsterInfo.InSight = 0;
                                        }
                                    }
                                }

                                foreach (IMapObject obj2 in client.Screen.Objects)
                                {
                                    if (obj2 == null) continue;
                                    if (obj2.MapObjType == MapObjectType.Monster)
                                    {
                                        Entity monster2 = obj2 as Entity;

                                        if (monster2 == null) continue;
                                        if (monster2.Dead) continue;

                                        if (Now >= monster.MonsterInfo.LastMove.AddMilliseconds(monster.MonsterInfo.AttackSpeed))
                                        {
                                            if (!monster2.MonsterInfo.Guard && (!monster2.Companion || monster2.Owner.Entity.ContainsFlag(Update.Flags.FlashingName)))
                                            {
                                                short distance = Kernel.GetDistance(monster.X, monster.Y, monster2.X, monster2.Y);

                                                if (distance <= monster.MonsterInfo.AttackRange)
                                                {
                                                    monster.MonsterInfo.LastMove = Time32.Now;
                                                    new Game.Attacking.Handle(null, monster, monster2);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            #region Rev Guard
                            if (monster.Name == "RevGuard")
                            {
                                if (monster.MonsterInfo.InRev == 0)
                                {
                                    if (monster.X != monster.MonsterInfo.BoundX || monster.Y != monster.MonsterInfo.BoundY)
                                    {
                                        monster.X = monster.MonsterInfo.BoundX;
                                        monster.Y = monster.MonsterInfo.BoundY;
                                        Network.GamePackets.TwoMovements jump = new Conquer_Online_Server.Network.GamePackets.TwoMovements();
                                        jump.X = monster.MonsterInfo.BoundX;
                                        jump.Y = monster.MonsterInfo.BoundY;
                                        jump.EntityCount = 1;
                                        jump.FirstEntity = monster.UID;
                                        jump.MovementType = Network.GamePackets.TwoMovements.Jump;
                                        client.SendScreen(jump, true);
                                    }
                                    if (client.Entity.Dead)
                                        monster.MonsterInfo.InRev = client.Entity.UID;
                                }
                                else
                                {
                                    if (client.Entity.Dead)
                                    {
                                        if (monster.MonsterInfo.InRev == client.Entity.UID)
                                        {
                                            if (client.Entity.Dead)
                                            {
                                                if (Time32.Now >= monster.MonsterInfo.LastMove.AddMilliseconds(monster.MonsterInfo.AttackSpeed))
                                                {
                                                    short distance = Kernel.GetDistance(monster.X, monster.Y, client.Entity.X, client.Entity.Y);

                                                    if (distance <= monster.MonsterInfo.AttackRange)
                                                    {
                                                        monster.MonsterInfo.LastMove = Time32.Now;
                                                        new Game.Attacking.Handle(null, monster, client.Entity);
                                                    }
                                                    else
                                                    {
                                                        if (distance <= monster.MonsterInfo.ViewRange)
                                                        {
                                                            /*Network.GamePackets.TwoMovements jump = new PhoenixPrvixyProject.Network.GamePackets.TwoMovements();
                                                            jump.X = Owner.Entity.X;
                                                            jump.Y = Owner.Entity.Y;
                                                            monster.X = Owner.Entity.X;
                                                            monster.Y = Owner.Entity.Y;
                                                            jump.EntityCount = 1;
                                                            jump.FirstEntity = monster.UID;
                                                            jump.MovementType = Network.GamePackets.TwoMovements.Jump;
                                                            Owner.SendScreen(jump, true);*/
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                monster.MonsterInfo.InRev = 0;
                                            }

                                        }
                                        else
                                        {
                                            monster.MonsterInfo.InRev = 0;
                                        }
                                    }
                                    else
                                    {
                                        if (monster.MonsterInfo.InRev == client.Entity.UID)
                                        {
                                            monster.MonsterInfo.InRev = 0;
                                        }
                                    }
                                }

                            }
                            #endregion
                            #region Stig Guard
                            if (monster.Name == "StigGuard")
                            {
                                if (monster.MonsterInfo.InStig == 0)
                                {
                                    if (monster.X != monster.MonsterInfo.BoundX || monster.Y != monster.MonsterInfo.BoundY)
                                    {
                                        monster.X = monster.MonsterInfo.BoundX;
                                        monster.Y = monster.MonsterInfo.BoundY;
                                        Network.GamePackets.TwoMovements jump = new Conquer_Online_Server.Network.GamePackets.TwoMovements();
                                        jump.X = monster.MonsterInfo.BoundX;
                                        jump.Y = monster.MonsterInfo.BoundY;
                                        jump.EntityCount = 1;
                                        jump.FirstEntity = monster.UID;
                                        jump.MovementType = Network.GamePackets.TwoMovements.Jump;
                                        client.SendScreen(jump, true);
                                    }
                                    if (!client.Entity.ContainsFlag(Network.GamePackets.Update.Flags.Stigma))
                                        monster.MonsterInfo.InStig = client.Entity.UID;
                                }
                                else
                                {
                                    if (!client.Entity.ContainsFlag(Network.GamePackets.Update.Flags.Stigma))
                                    {
                                        if (monster.MonsterInfo.InStig == client.Entity.UID)
                                        {
                                            if (!client.Entity.Dead)
                                            {
                                                if (Time32.Now >= monster.MonsterInfo.LastMove.AddMilliseconds(monster.MonsterInfo.AttackSpeed))
                                                {
                                                    short distance = Kernel.GetDistance(monster.X, monster.Y, client.Entity.X, client.Entity.Y);

                                                    if (distance <= monster.MonsterInfo.AttackRange)
                                                    {
                                                        monster.MonsterInfo.LastMove = Time32.Now;
                                                        new Game.Attacking.Handle(null, monster, client.Entity);
                                                    }
                                                    else
                                                    {
                                                        if (distance <= monster.MonsterInfo.ViewRange)
                                                        {
                                                            /*Network.GamePackets.TwoMovements jump = new PhoenixPrvixyProject.Network.GamePackets.TwoMovements();
                                                            jump.X = Owner.Entity.X;
                                                            jump.Y = Owner.Entity.Y;
                                                            monster.X = Owner.Entity.X;
                                                            monster.Y = Owner.Entity.Y;
                                                            jump.EntityCount = 1;
                                                            jump.FirstEntity = monster.UID;
                                                            jump.MovementType = Network.GamePackets.TwoMovements.Jump;
                                                            Owner.SendScreen(jump, true);*/
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                monster.MonsterInfo.InStig = 0;
                                            }
                                        }
                                        else
                                        {
                                            monster.MonsterInfo.InStig = 0;
                                        }
                                    }
                                    else
                                    {
                                        if (monster.MonsterInfo.InStig == client.Entity.UID)
                                        {
                                            monster.MonsterInfo.InStig = 0;
                                        }
                                    }
                                }
                               /* for (int i = 0; i < _Object.Count; i++)
                                {
                                    if (i >= Objects.Count)
                                        break;
                                    IMapObject obj2 = _Objects[i];*/
                                {
                                    foreach (IMapObject obj2 in client.Screen.Objects)
                                    if (obj2 == null)
                                        continue;
                                    foreach (IMapObject obj2 in client.Screen.Objects)
                                    if (obj2.MapObjType == MapObjectType.Monster)
                                    {
                                        Entity monster2 = obj2 as Entity;//null;// Owner.Map.Entities[obj2.UID];
                                        if (monster2 == null)
                                            continue;
                                        if (monster2.Dead)
                                            continue;
                                        if (Time32.Now >= monster.MonsterInfo.LastMove.AddMilliseconds(monster.MonsterInfo.AttackSpeed))
                                        {
                                            if (!monster2.Name.Contains("Guard") && !monster2.Companion)
                                            {
                                                short distance = Kernel.GetDistance(monster.X, monster.Y, monster2.X, monster2.Y);

                                                if (distance <= monster.MonsterInfo.AttackRange)
                                                {
                                                    monster.MonsterInfo.LastMove = Time32.Now;
                                                    new Game.Attacking.Handle(null, monster, monster2);
                                                }
                                            }
                                        }
                                    }
                                }

                            }
                            #endregion                          
                        }
                    }
                }
            }
        }
        private static void aliveMonstersCallback(GameClient client, int time)
        {
            if (!client.Socket.Alive)
            {
                client.Screen.DisposeTimers();
                return;
            }
            if (client.Entity == null)
                return;
            if (client.Map == null)
                return;
            if (client.Map.FreezeMonsters)
                return;

            Time32 Now = new Time32(time);
            foreach (IMapObject obj in client.Screen.Objects)
            {
                if (obj != null)
                {
                    if (obj.MapObjType == MapObjectType.Monster)
                    {
                        Entity monster = obj as Entity;
                        if (monster == null) continue;
                        if (monster.MonsterInfo.Guard || monster.Companion || monster.Dead) continue;
                        if (monster.MonsterInfo.Reviver)
                        {
                            if (client.Entity.Dead && Now > client.Entity.DeathStamp.AddSeconds(5))
                            {
                                client.Entity.BringToLife();
                                SpellUse use = new SpellUse(true);
                                use.Attacker = monster.UID;
                                use.X = client.Entity.X;
                                use.Y = client.Entity.Y;
                                use.SpellID = 1100;
                                use.AddTarget(client.Entity.UID, 0, null);
                                Kernel.SendWorldMessage(use, Program.GamePool, monster.MapID);
                            }
                            return;
                        }
                        short distance = Kernel.GetDistance(monster.X, monster.Y, client.Entity.X, client.Entity.Y);
                        if (distance > Constants.pScreenDistance)
                        {
                            client.Screen.Remove(obj);
                            continue;
                        }
                        if (monster.MonsterInfo.InSight != 0 && monster.MonsterInfo.InSight != client.Entity.UID)
                        {
                            if (monster.MonsterInfo.InSight > 1000000)
                            {
                                GameClient cl;
                                if (Kernel.GamePool.TryGetValue(monster.MonsterInfo.InSight, out cl))
                                {
                                    short dst = Kernel.GetDistance(monster.X, monster.Y, cl.Entity.X, cl.Entity.Y);
                                    if (dst > Constants.pScreenDistance)
                                        monster.MonsterInfo.InSight = 0;
                                }
                                else
                                    monster.MonsterInfo.InSight = 0;
                            }
                            else
                            {
                                Entity companion = client.Map.Companions[monster.MonsterInfo.InSight];
                                if (companion != null)
                                {
                                    short dst = Kernel.GetDistance(monster.X, monster.Y, companion.X, companion.Y);
                                    if (dst > Constants.pScreenDistance)
                                        monster.MonsterInfo.InSight = 0;
                                }
                                else
                                    monster.MonsterInfo.InSight = 0;
                            }
                        }
                        if (Now >= monster.MonsterInfo.LastMove.AddMilliseconds(monster.MonsterInfo.MinimumSpeed))
                        {
                            if (distance <= Constants.pScreenDistance)
                            {
                                #region Companions
                                if (client.Companion != null)
                                {
                                    if (client.Companion.Companion && !client.Companion.Dead)
                                    {
                                        short distance2 = Kernel.GetDistance(monster.X, monster.Y, client.Companion.X, client.Companion.Y);
                                        if (distance > distance2 || client.Entity.ContainsFlag(Update.Flags.Invisibility) || client.Entity.ContainsFlag(Update.Flags.Fly))
                                        {
                                            if (monster.MonsterInfo.InSight == 0)
                                            {
                                                monster.MonsterInfo.InSight = client.Companion.UID;
                                            }
                                            else
                                            {
                                                if (monster.MonsterInfo.InSight == client.Companion.UID)
                                                {
                                                    if (distance2 > Constants.pScreenDistance)
                                                    {
                                                        monster.MonsterInfo.InSight = 0;
                                                    }
                                                    else
                                                    {
                                                        if (distance2 <= monster.MonsterInfo.AttackRange)
                                                        {
                                                            if (Now >= monster.MonsterInfo.LastMove.AddMilliseconds(monster.MonsterInfo.AttackSpeed))
                                                            {
                                                                monster.MonsterInfo.LastMove = Time32.Now;
                                                                new Game.Attacking.Handle(null, monster, client.Companion);
                                                                continue;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (distance2 > monster.MonsterInfo.ViewRange / 2)
                                                            {
                                                                if (distance2 < Constants.pScreenDistance)
                                                                {
                                                                    if (Now >= monster.MonsterInfo.LastMove.AddMilliseconds(monster.MonsterInfo.RunSpeed))
                                                                    {
                                                                        monster.MonsterInfo.LastMove = Time32.Now;

                                                                        Enums.ConquerAngle facing = Kernel.GetAngle(monster.X, monster.Y, client.Companion.X, client.Companion.Y);
                                                                        if (!monster.Move(facing))
                                                                        {
                                                                            facing = (Enums.ConquerAngle)Kernel.Random.Next(7);
                                                                            if (monster.Move(facing))
                                                                            {
                                                                                monster.Facing = facing;
                                                                                GroundMovement move = new GroundMovement(true);
                                                                                move.Direction = facing;
                                                                                move.UID = monster.UID;
                                                                                move.GroundMovementType = GroundMovement.Run;
                                                                                monster.MonsterInfo.SendScreen(move);
                                                                                continue;
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            monster.Facing = facing;
                                                                            GroundMovement move = new GroundMovement(true);
                                                                            move.Direction = facing;
                                                                            move.UID = monster.UID;
                                                                            move.GroundMovementType = GroundMovement.Run;
                                                                            monster.MonsterInfo.SendScreen(move);
                                                                            continue;
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    monster.MonsterInfo.InSight = 0;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (Now >= monster.MonsterInfo.LastMove.AddMilliseconds(monster.MonsterInfo.MoveSpeed))
                                                                {
                                                                    monster.MonsterInfo.LastMove = Time32.Now;
                                                                    Enums.ConquerAngle facing = Kernel.GetAngle(monster.X, monster.Y, client.Companion.X, client.Companion.Y);
                                                                    if (!monster.Move(facing))
                                                                    {
                                                                        facing = (Enums.ConquerAngle)Kernel.Random.Next(7);
                                                                        if (monster.Move(facing))
                                                                        {
                                                                            monster.Facing = facing;
                                                                            GroundMovement move = new GroundMovement(true);
                                                                            move.Direction = facing;
                                                                            move.UID = monster.UID;
                                                                            monster.MonsterInfo.SendScreen(move);
                                                                            continue;
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        monster.Facing = facing;
                                                                        GroundMovement move = new GroundMovement(true);
                                                                        move.Direction = facing;
                                                                        move.UID = monster.UID;
                                                                        monster.MonsterInfo.SendScreen(move);
                                                                        continue;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                #endregion
                                #region Player
                                if (monster.MonsterInfo.InSight == 0)
                                {
                                    if (distance <= monster.MonsterInfo.ViewRange)
                                    {
                                        if (!client.Entity.ContainsFlag(Update.Flags.Invisibility))
                                        {
                                            if (monster.MonsterInfo.SpellID != 0 || !client.Entity.ContainsFlag(Update.Flags.Fly))
                                            {
                                                monster.MonsterInfo.InSight = client.Entity.UID;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (monster.MonsterInfo.InSight == client.Entity.UID)
                                    {
                                        if (monster.MonsterInfo.SpellID == 0 && client.Entity.ContainsFlag(Update.Flags.Fly))
                                        {
                                            monster.MonsterInfo.InSight = 0;
                                            return;
                                        }

                                        if (client.Entity.Dead)
                                        {
                                            monster.MonsterInfo.InSight = 0;
                                            return;
                                        }
                                        if (distance > Constants.pScreenDistance)
                                        {
                                            monster.MonsterInfo.InSight = 0;
                                        }
                                        else
                                        {
                                            if (distance <= monster.MonsterInfo.AttackRange)
                                            {
                                                if (Now >= monster.MonsterInfo.LastMove.AddMilliseconds(monster.MonsterInfo.AttackSpeed))
                                                {
                                                    monster.MonsterInfo.LastMove = Time32.Now;
                                                    new Game.Attacking.Handle(null, monster, client.Entity);
                                                }
                                            }
                                            else
                                            {
                                                if (distance > monster.MonsterInfo.ViewRange / 2)
                                                {
                                                    if (distance < Constants.pScreenDistance)
                                                    {
                                                        if (Now >= monster.MonsterInfo.LastMove.AddMilliseconds(monster.MonsterInfo.RunSpeed))
                                                        {
                                                            monster.MonsterInfo.LastMove = Time32.Now;

                                                            Enums.ConquerAngle facing = Kernel.GetAngle(monster.X, monster.Y, client.Entity.X, client.Entity.Y);
                                                            if (!monster.Move(facing))
                                                            {
                                                                facing = (Enums.ConquerAngle)Kernel.Random.Next(7);
                                                                if (monster.Move(facing))
                                                                {
                                                                    monster.Facing = facing;
                                                                    GroundMovement move = new GroundMovement(true);
                                                                    move.Direction = facing;
                                                                    move.UID = monster.UID;
                                                                    move.GroundMovementType = Network.GamePackets.GroundMovement.Run;
                                                                    monster.MonsterInfo.SendScreen(move);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                monster.Facing = facing;
                                                                GroundMovement move = new GroundMovement(true);
                                                                move.Direction = facing;
                                                                move.UID = monster.UID;
                                                                move.GroundMovementType = Network.GamePackets.GroundMovement.Run;
                                                                monster.MonsterInfo.SendScreen(move);
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        monster.MonsterInfo.InSight = 0;
                                                    }
                                                }
                                                else
                                                {
                                                    if (Now >= monster.MonsterInfo.LastMove.AddMilliseconds(monster.MonsterInfo.MoveSpeed))
                                                    {
                                                        monster.MonsterInfo.LastMove = Time32.Now;
                                                        Enums.ConquerAngle facing = Kernel.GetAngle(monster.X, monster.Y, client.Entity.X, client.Entity.Y);
                                                        if (!monster.Move(facing))
                                                        {
                                                            facing = (Enums.ConquerAngle)Kernel.Random.Next(7);
                                                            if (monster.Move(facing))
                                                            {
                                                                monster.Facing = facing;
                                                                GroundMovement move = new GroundMovement(true);
                                                                move.Direction = facing;
                                                                move.UID = monster.UID;
                                                                monster.MonsterInfo.SendScreen(move);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            monster.Facing = facing;
                                                            GroundMovement move = new GroundMovement(true);
                                                            move.Direction = facing;
                                                            move.UID = monster.UID;
                                                            monster.MonsterInfo.SendScreen(move);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                #endregion
                            }
                        }
                    }
                    else if (obj.MapObjType == MapObjectType.Item)
                    {
                        FloorItem item = obj as FloorItem;
                        if (item == null) continue;

                        if (item.Type == FloorItem.Effect)
                        {
                            if (item.ItemID == FloorItem.DaggerStorm || item.ItemID == FloorItem.FuryofEgg || item.ItemID == FloorItem.Twilight || item.ItemID == FloorItem.ShacklingIce)  
                            {
                                if (item.Owner == client)
                                {
                                    if (Time32.Now > item.UseTime.AddSeconds(1))
                                    {
                                        item.UseTime = Time32.Now;
                                        var spell = Database.SpellTable.GetSpell(11600, client);

                                        var attack = new Attack(true);
                                        attack.Attacker = item.Owner.Entity.UID;
                                        attack.AttackType = Attack.Melee;

                                        foreach (var obj1 in client.Screen.Objects)
                                        {
                                            if (Kernel.GetDistance(obj1.X, obj1.Y, obj.X, obj.Y) <= 3)
                                            {
                                                if (obj1.MapObjType == MapObjectType.Monster || obj1.MapObjType == MapObjectType.Player)
                                                {
                                                    var attacked = obj1 as Entity;
                                                    if (Attacking.Handle.CanAttack(client.Entity, attacked, spell, false))
                                                    {
                                                        uint damage = Game.Attacking.Calculate.Melee(client.Entity, attacked, spell, ref attack);

                                                        attack.Damage = damage;
                                                        attack.Attacked = attacked.UID;
                                                        attack.X = attacked.X;
                                                        attack.Y = attacked.Y;

                                                        Attacking.Handle.ReceiveAttack(client.Entity, attacked, attack, damage, spell);
                                                    }
                                                }
                                                else if (obj1.MapObjType == MapObjectType.SobNpc)
                                                {
                                                    var attacked = obj1 as SobNpcSpawn;
                                                    if (Attacking.Handle.CanAttack(client.Entity, attacked, spell))
                                                    {
                                                        uint damage = Game.Attacking.Calculate.Melee(client.Entity, attacked, ref attack);
                                                        damage = (uint)(damage * spell.PowerPercent);

                                                        attack.Damage = damage;
                                                        attack.Attacked = attacked.UID;
                                                        attack.X = attacked.X;
                                                        attack.Y = attacked.Y;

                                                        Attacking.Handle.ReceiveAttack(client.Entity, attacked, attack, damage, spell);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        private static void itemsCallback(GameClient client, int time)
        {
            if (!client.Socket.Alive)
            {
                client.Screen.DisposeTimers();
                return;
            }
            if (client.Entity == null)
                return;
            if (client.Map == null)
                return;
            if (client.Map.FreezeMonsters)
                return;

            Time32 Now = new Time32(time);
            foreach (IMapObject obj in client.Screen.Objects)
            {
                if (obj != null)
                {
                    if (obj.MapObjType == MapObjectType.Item)
                    {
                        FloorItem item = obj as FloorItem;
                        if (item == null) continue;

                        if (item.Type == FloorItem.Effect)
                        {
                            if (item.ItemID == FloorItem.DaggerStorm)
                            {
                                if (item.OnFloor.AddSeconds(4).Next(time: time))
                                {
                                    item.Type = Network.GamePackets.FloorItem.RemoveEffect;
                                    //client.SendScreen(item, true);
                                    client.Map.RemoveFloorItem(item);
                                    client.RemoveScreenSpawn(item, true);
                                }
                            }
                        }
                        else
                        {
                            if (item.OnFloor.AddSeconds(Constants.FloorItemSeconds).Next(time: time))
                            {
                                item.Type = Network.GamePackets.FloorItem.Remove;
                                foreach (Interfaces.IMapObject _obj in client.Screen.Objects)
                                    if (_obj != null)
                                        if (_obj.MapObjType == MapObjectType.Player)
                                            (_obj as Entity).Owner.Send(item);
                                client.Map.RemoveFloorItem(item);
                                client.Screen.Remove(item);
                            }
                        }
                    }
                }
            }
        }

        private IDisposable[] TimerSubscriptions;
        private object DisposalSyncRoot;
        public List<Interfaces.IMapObject> _Objects;
        private Interfaces.IMapObject[] _objects;
        private static ConcurrentDictionary<uint, PokerTable> PokerTables;
        public Interfaces.IMapObject[] Objects { get { return _objects; } }

        private ConcurrentDictionary<uint, Interfaces.IMapObject> _objectDictionary;

        public Client.GameClient Owner;

        public Screen(Client.GameClient client)
        {
            Owner = client;
            _objects = new Interfaces.IMapObject[0];
            PokerTables = new ConcurrentDictionary<uint, PokerTable>();
            _objectDictionary = new ConcurrentDictionary<uint, IMapObject>();

            TimerSubscriptions = new IDisposable[] 
            {
                MonsterBuffers.Add(client),
                Guards.Add(client),
                AliveMonsters.Add(client),
                Items.Add(client)
            };
            DisposalSyncRoot = new object();
        }
        ~Screen()
        {
            DisposeTimers();
            Clear();
            _objects = null;
            _objectDictionary = null;
            Owner = null;
        }

        public void DisposeTimers()
        {
            lock (DisposalSyncRoot)
            {
                if (TimerSubscriptions == null) return;
                for (int i = 0; i < TimerSubscriptions.Length; i++)
                {
                    if (TimerSubscriptions[i] != null)
                    {
                        TimerSubscriptions[i].Dispose();
                        TimerSubscriptions[i] = null;
                    }
                }
            }
        }

        private void updateBase()
        {
            _objects = _objectDictionary.Values.ToArray();
        }

        public bool Add(Interfaces.IMapObject _object)
        {
            if (_object == null) return false;

            if (_objectDictionary.ContainsKey(_object.UID))
                if (_objectDictionary[_object.UID] == null) // should never happen
                    _objectDictionary.Remove(_object.UID);

            if (!_objectDictionary.ContainsKey(_object.UID))
            {
                if (Kernel.GetDistance(_object.X, _object.Y, Owner.Entity.X, Owner.Entity.Y) <= Constants.pScreenDistance)
                {
                    _objectDictionary[_object.UID] = _object;
                    updateBase();
                    return true;
                }
            }
            return false;
        }
        public bool Remove(Interfaces.IMapObject _object)
        {
            if (_object == null) return false;

            if (_objectDictionary.Remove(_object.UID))
            {
                updateBase();
                if (_object.MapObjType == MapObjectType.Item)
                {
                    FloorItem item = _object as FloorItem;
                    if (item.Type >= FloorItem.Effect)
                    {
                        item.Type = FloorItem.RemoveEffect;
                        Owner.Send(item);
                    }
                    else
                    {
                        item.Type = FloorItem.Remove;
                        Owner.Send(item);
                        item.Type = FloorItem.Drop;
                    }
                }
                else if (_object.MapObjType == MapObjectType.Player)
                {
                    Owner.Send(new Data(true)
                    {
                        UID = _object.UID,
                        ID = Network.GamePackets.Data.RemoveEntity
                    });
                }
                else if (_object.MapObjType == MapObjectType.StaticEntity)
                {
                    Owner.Send(new Data(true)
                    {
                        UID = _object.UID,
                        ID = Network.GamePackets.Data.RemoveEntity
                    });
                }
                return true;
            }
            return false;
        }

        public bool TryGetValue(uint uid, out Entity entity)
        {
            entity = null;
            Interfaces.IMapObject imo = null;
            if (_objectDictionary.TryGetValue(uid, out imo))
            {
                if (imo == null)
                {
                    _objectDictionary.Remove(uid);
                    updateBase();
                    return false;
                }
                if (imo is Entity)
                {
                    entity = imo as Entity;
                    return true;
                }
            }
            return false;
        }
        public bool GetRaceObject(Func<IMapObject, bool> predicate, out StaticEntity entity)
        {
            entity = null;
            foreach (var obj in Objects)
                if (obj is StaticEntity)
                    if (predicate(obj))
                        entity = obj as StaticEntity;
            return entity != null;
        }
        public bool TryGetSob(uint uid, out SobNpcSpawn sob)
        {
            sob = null;
            Interfaces.IMapObject imo = null;
            if (_objectDictionary.TryGetValue(uid, out imo))
            {
                if (imo == null)
                {
                    _objectDictionary.Remove(uid);
                    updateBase();
                    return false;
                }
                if (imo is SobNpcSpawn)
                {
                    sob = imo as SobNpcSpawn;
                    return true;
                }
            }
            return false;
        }
        public bool TryGetFloorItem(uint uid, out FloorItem item)
        {
            item = null;
            Interfaces.IMapObject imo = null;
            if (_objectDictionary.TryGetValue(uid, out imo))
            {
                if (imo == null)
                {
                    _objectDictionary.Remove(uid);
                    updateBase();
                    return false;
                }
                if (imo is FloorItem)
                {
                    item = imo as FloorItem;
                    return true;
                }
            }
            return false;
        }


        public IEnumerable<T> Select<T>(MapObjectType type) where T : class
        {
            foreach (var obj in Objects)
                if (obj != null)
                    if (obj.MapObjType == type)
                        yield return obj as T;
        }
        public IEnumerable<T> Where<T>(Func<IMapObject, bool> predicate) where T : class
        {
            foreach (var obj in Objects)
                if (obj != null)
                    if (predicate(obj))
                        yield return obj as T;
        }
        public IEnumerable<T> SelectWhere<T>(MapObjectType type, Func<T, bool> predicate) where T : class
        {
            foreach (var obj in Objects)
                if (obj != null)
                    if (obj.MapObjType == type)
                        if (predicate(obj as T))
                            yield return obj as T;
        }


        public bool Contains(Interfaces.IMapObject _object)
        {
            if (_object == null) return false;
            return _objectDictionary.ContainsKey(_object.UID);
        }
        public bool Contains(uint uid)
        {
            return _objectDictionary.ContainsKey(uid);
        }

        public void CleanUp(Interfaces.IPacket spawnWith)
        {
            bool remove;
            try
            {
                foreach (IMapObject Base in Objects)
                {
                    if (Base == null) continue;
                    remove = false;
                    if (Base.MapObjType == MapObjectType.Monster)
                    {
                        if ((Base as Entity).Dead)
                        {
                            if (Time32.Now > (Base as Entity).DeathStamp.AddSeconds(8))
                                remove = true;
                            else
                                remove = false;
                        }
                        if (Kernel.GetDistance(Owner.Entity.X, Owner.Entity.Y, Base.X, Base.Y) >= Constants.remScreenDistance)
                            remove = true;
                        if (remove)
                        {
                            if ((Base as Entity).MonsterInfo.InSight == Owner.Entity.UID)
                                (Base as Entity).MonsterInfo.InSight = 0;
                        }
                    }
                    else if (Base.MapObjType == MapObjectType.Player)
                    {
                        if (remove = (Kernel.GetDistance(Owner.Entity.X, Owner.Entity.Y, Base.X, Base.Y) >= Constants.pScreenDistance))
                        {
                            GameClient pPlayer = Base.Owner as GameClient;
                            pPlayer.Screen.Remove(Owner.Entity);
                        }
                    }
                    else if (Base.MapObjType == MapObjectType.Item)
                    {
                        remove = (Kernel.GetDistance(Owner.Entity.X, Owner.Entity.Y, Base.X, Base.Y) >= 22);

                    }
                    else
                    {
                        remove = (Kernel.GetDistance(Owner.Entity.X, Owner.Entity.Y, Base.X, Base.Y) >= Constants.remScreenDistance);
                    }
                    if (Base.MapID != Owner.Map.ID)
                        remove = true;
                    if (remove)
                    {
                        Remove(Base);
                    }
                }
            }
            catch (Exception e) { Program.SaveException(e); }
        }
        public void FullWipe()
        {
            foreach (IMapObject Base in Objects)
            {
                if (Base == null) continue;

                Data data = new Data(true);
                data.UID = Base.UID;
                data.ID = Network.GamePackets.Data.RemoveEntity;
                Owner.Send(data);

                if (Base.MapObjType == Game.MapObjectType.Player)
                {
                    GameClient pPlayer = Base.Owner as GameClient;
                    pPlayer.Screen.Remove(Owner.Entity);
                }
            }
            Clear();
        }
        public void Clear()
        {
            _objectDictionary.Clear();
            _objects = new IMapObject[0];
        }
        public void Reload(Interfaces.IPacket spawnWith)
        {
            CleanUp(spawnWith);
            try
            {
                foreach (GameClient pClient in Kernel.GamePool.Values)
                {
                    if (pClient == null) return;
                    if (pClient.Entity == null) return;
                    if (Owner == null) return;
                    if (Owner.Entity == null) return;
                    if (pClient.Entity.UID != Owner.Entity.UID)
                    {
                        if (pClient.Map.ID == Owner.Map.ID)
                        {
                            short dist = Kernel.GetDistance(pClient.Entity.X, pClient.Entity.Y, Owner.Entity.X, Owner.Entity.Y);
                            if (dist <= Constants.pScreenDistance && !Contains(pClient.Entity))
                            {
                                if (pClient.Guild != null)
                                    pClient.Guild.SendName(Owner);
                                if (Owner.Guild != null)
                                    Owner.Guild.SendName(pClient);
                                if (pClient.Entity.InteractionInProgress && pClient.Entity.InteractionWith != Owner.Entity.UID && pClient.Entity.InteractionSet)
                                {
                                    if (pClient.Entity.Body == 1003 || pClient.Entity.Body == 1004)
                                    {
                                        if (pClient.Entity.InteractionX == pClient.Entity.X && pClient.Entity.Y == pClient.Entity.InteractionY)
                                        {
                                            Network.GamePackets.Attack atak = new Conquer_Online_Server.Network.GamePackets.Attack(true);
                                            atak.Attacker = pClient.Entity.UID;
                                            atak.Attacked = pClient.Entity.InteractionWith;
                                            atak.X = pClient.Entity.X;
                                            atak.Y = pClient.Entity.Y;
                                            atak.AttackType = 49;
                                            atak.Damage = pClient.Entity.InteractionType;
                                            Owner.Send(atak);
                                        }
                                    }
                                    else
                                    {
                                        if (Conquer_Online_Server.Kernel.GamePool.ContainsKey(pClient.Entity.InteractionWith))
                                        {
                                            Client.GameClient Cs = Conquer_Online_Server.Kernel.GamePool[pClient.Entity.InteractionWith] as Client.GameClient;
                                            if (Cs.Entity.X == pClient.Entity.InteractionX && pClient.Entity.Y == pClient.Entity.InteractionY)
                                            {
                                                Network.GamePackets.Attack atak = new Conquer_Online_Server.Network.GamePackets.Attack(true);
                                                atak.Attacker = pClient.Entity.UID;
                                                atak.Attacked = pClient.Entity.InteractionWith;
                                                atak.X = pClient.Entity.X;
                                                atak.Y = pClient.Entity.Y;
                                                atak.AttackType = 49;
                                                atak.Damage = pClient.Entity.InteractionType;
                                                Owner.Send(atak);
                                            }
                                        }
                                    }
                                }
                                if (pClient.Map.BaseID == 700)
                                {
                                    if (Owner.InQualifier())
                                    {
                                        if (pClient.InQualifier())
                                        {
                                            Owner.Entity.SendSpawn(pClient);
                                            pClient.Entity.SendSpawn(Owner);
                                            if (pClient.Guild != null)
                                                Owner.Entity.SendSpawn(pClient, false);
                                            if (Owner.Guild != null)
                                                pClient.Entity.SendSpawn(Owner, false);
                                            if (spawnWith != null)
                                                pClient.Send(spawnWith);
                                        }
                                        else
                                        {
                                            Owner.Entity.SendSpawn(pClient);

                                            if (pClient.Guild != null)
                                                Owner.Entity.SendSpawn(pClient, false);
                                            Add(pClient.Entity);
                                            if (spawnWith != null)
                                                pClient.Send(spawnWith);
                                        }
                                    }
                                    else
                                    {
                                        if (pClient.InQualifier())
                                        {
                                            pClient.Entity.SendSpawn(Owner);
                                            if (Owner.Guild != null)
                                                pClient.Entity.SendSpawn(Owner, false);
                                            pClient.Screen.Add(Owner.Entity);
                                            if (spawnWith != null)
                                                Owner.Send(spawnWith);
                                        }
                                        else
                                        {
                                            Owner.Entity.SendSpawn(pClient);
                                            pClient.Entity.SendSpawn(Owner);

                                            if (pClient.Guild != null)
                                                Owner.Entity.SendSpawn(pClient, false);
                                            if (Owner.Guild != null)
                                                pClient.Entity.SendSpawn(Owner, false);

                                            if (spawnWith != null)
                                                pClient.Send(spawnWith);
                                        }
                                    }
                                }
                                else
                                {
                                    Owner.Entity.SendSpawn(pClient);
                                    pClient.Entity.SendSpawn(Owner);

                                    if (pClient.Guild != null)
                                        Owner.Entity.SendSpawn(pClient, false);
                                    if (Owner.Guild != null)
                                        pClient.Entity.SendSpawn(Owner, false);

                                    if (spawnWith != null)
                                        pClient.Send(spawnWith);
                                }
                            }
                        }
                    }
                }
                var Map = Owner.Map;
                #region Npcs
                foreach (Interfaces.INpc npc in Map.Npcs.Values)
                {
                    if (npc == null) continue;
                    if (Kernel.GetDistance(npc.X, npc.Y, Owner.Entity.X, Owner.Entity.Y) > 16) continue;
                    if (Contains(npc.UID)) continue;
                    npc.SendSpawn(Owner, false);
                }
                #endregion
                #region Items + map effects
                foreach (var item in Map.FloorItems.Values)
                {
                    if (item == null) continue;
                    if (Kernel.GetDistance(item.X, item.Y, Owner.Entity.X, Owner.Entity.Y) > 16) continue;
                    if (Contains(item.UID)) continue;
                    if (item.Type == FloorItem.Effect)
                    {
                        if (item.ItemID == FloorItem.DaggerStorm)
                        {
                            if (item.OnFloor.AddSeconds(4).Next(time: Time32.Now.AllMilliseconds()))
                            {
                                item.Type = Network.GamePackets.FloorItem.RemoveEffect;
                                foreach (Interfaces.IMapObject _obj in Objects)
                                    if (_obj != null)
                                        if (_obj.MapObjType == MapObjectType.Player)
                                            (_obj as Entity).Owner.Send(item);
                                Map.RemoveFloorItem(item);
                            }
                            else
                                item.SendSpawn(Owner, false);
                        }
                        else
                            item.SendSpawn(Owner, false);
                    }
                    else
                    {
                        if ((Time32.Now > item.OnFloor.AddSeconds(Constants.FloorItemSeconds)) || item.PickedUpAlready)
                        {
                            item.Type = Network.GamePackets.FloorItem.Remove;
                            Map.RemoveFloorItem(item);
                        }
                    }
                    item.SendSpawn(Owner);
                }
                #endregion
                foreach (Game.Entity monster in Map.Entities.Values)
                {
                    if (monster == null) continue;
                    if (Kernel.GetDistance(monster.X, monster.Y, Owner.Entity.X, Owner.Entity.Y) <= 16 && !Contains(monster.UID))
                    {
                        if (!monster.Dead)
                        {
                            monster.SendSpawn(Owner, false);
                            if (monster.MaxHitpoints > 65535)
                            {
                                Update upd = new Update(true) { UID = monster.UID };
                                upd.Append(Update.MaxHitpoints, monster.MaxHitpoints);
                                upd.Append(Update.Hitpoints, monster.Hitpoints);
                                Owner.Send(upd);
                            }
                        }
                    }
                }
                #region RaceItems
                if (Owner.Map.StaticEntities.Count != 0)
                {
                    foreach (var item in Owner.Map.StaticEntities.Values)
                    {
                        if (item == null) continue;
                        if (!item.Viable) continue;
                        if (Kernel.GetDistance(item.X, item.Y, Owner.Entity.X, Owner.Entity.Y) > 16) continue;
                        if (Contains(item.UID)) continue;
                        item.SendSpawn(Owner);
                    }
                }
                #endregion
                foreach (Game.Entity monster in Map.Companions.Values)
                {
                    if (monster == null) continue;
                    if (Kernel.GetDistance(monster.X, monster.Y, Owner.Entity.X, Owner.Entity.Y) <= 18 && !Contains(monster.UID))
                    {
                        if (!monster.Dead)
                        {
                            monster.SendSpawn(Owner);
                        }
                    }
                }
                if (Owner.Map.ID == 8880 || Owner.Map.ID == 8881)
                {
                    foreach (Game.PokerTable T in Kernel.PokerTables.Values)
                    {
                        if (T.Map == Owner.Map.ID)
                        {
                            if (Kernel.GetDistance(T.X, T.Y, Owner.Entity.X, Owner.Entity.Y) <= Constants.nScreenDistance && !PokerTables.ContainsKey(T.Id))
                            {
                                Owner.Send(Game.PokerPackets.PokerTable(T));
                                PokerTables.Add(T.Id, T);
                            }
                        }
                    }
                }
            }
            catch (Exception e) { Program.SaveException(e); }
        }
    }
}
