﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer_Online_Server.Network.GamePackets;
using Conquer_Online_Server.Game.ConquerStructures;
using NpcDialogs;

using Conquer_Online_Server.Network;
using Conquer_Online_Server.Interfaces;
using System.Drawing;
using Conquer_Online_Server.Database;

namespace Conquer_Online_Server.Game.Attacking
{
    public class Handle
    {
        private Attack attack;
        private Entity attacker, attacked;
        private byte num;
        public Handle(Attack attack, Entity attacker, Entity attacked)
        {
            this.attack = attack;
            this.attacker = attacker;
            this.attacked = attacked;
            this.Execute();
        }
        #region Interations
        public class InteractionRequest
        {
            public InteractionRequest(Network.GamePackets.Attack attack, Game.Entity a_client)
            {
                Client.GameClient client = a_client.Owner;

                client.Entity.InteractionInProgress = false;
                client.Entity.InteractionWith = attack.Attacked;
                client.Entity.InteractionType = 0;
                client.InteractionEffect = attack.ResponseDamage;

                if (Kernel.GamePool.ContainsKey(attack.Attacked))
                {
                    Client.GameClient clienttarget = Kernel.GamePool[attack.Attacked];
                    clienttarget.Entity.InteractionInProgress = false;
                    clienttarget.Entity.InteractionWith = client.Entity.UID;
                    clienttarget.Entity.InteractionType = 0;
                    clienttarget.InteractionEffect = attack.ResponseDamage;
                    attack.Attacker = client.Entity.UID;
                    attack.X = clienttarget.Entity.X;
                    attack.Y = clienttarget.Entity.Y;
                    attack.AttackType = 46;
                    clienttarget.Send(attack);
                    clienttarget.Send(attack);
                }
            }
        }
        public class InteractionEffect
        {
            public InteractionEffect(Network.GamePackets.Attack attack, Game.Entity a_client)
            {
                Client.GameClient client = a_client.Owner;

                if (Kernel.GamePool.ContainsKey(client.Entity.InteractionWith))
                {
                    Client.GameClient clienttarget = Kernel.GamePool[client.Entity.InteractionWith];

                    if (clienttarget.Entity.X == client.Entity.X && clienttarget.Entity.Y == client.Entity.Y)
                    {
                        attack.Damage = client.Entity.InteractionType;
                        attack.ResponseDamage = client.InteractionEffect;
                        clienttarget.Entity.InteractionSet = true;
                        client.Entity.InteractionSet = true;
                        attack.Attacker = clienttarget.Entity.UID;
                        attack.Attacked = client.Entity.UID;
                        attack.AttackType = 47;
                        attack.X = clienttarget.Entity.X;
                        attack.Y = clienttarget.Entity.Y;

                        clienttarget.Send(attack);
                        attack.AttackType = 49;

                        attack.Attacker = client.Entity.UID;
                        attack.Attacked = clienttarget.Entity.UID;
                        client.SendScreen(attack, true);

                        attack.Attacker = clienttarget.Entity.UID;
                        attack.Attacked = client.Entity.UID;
                        client.SendScreen(attack, true);
                    }
                }
            }
        }
        public class InteractionAccept
        {
            public InteractionAccept(Network.GamePackets.Attack attack, Game.Entity a_client)
            {

                Client.GameClient client = a_client.Owner;
                if (client.Entity.ContainsFlag(Network.GamePackets.Update.Flags.Ride))
                    client.Entity.RemoveFlag(Network.GamePackets.Update.Flags.Ride);
                if (client.Entity.InteractionWith != attack.Attacked)
                    return;
                attack.ResponseDamage = client.InteractionEffect;
                client.Entity.InteractionSet = false;
                if (Kernel.GamePool.ContainsKey(attack.Attacked))
                {
                    Client.GameClient clienttarget = Kernel.GamePool[attack.Attacked];
                    if (clienttarget.Entity.ContainsFlag(Network.GamePackets.Update.Flags.Ride))
                        clienttarget.Entity.RemoveFlag(Network.GamePackets.Update.Flags.Ride);
                    clienttarget.Entity.InteractionSet = false;
                    if (clienttarget.Entity.InteractionWith != client.Entity.UID)
                        return;
                    if (clienttarget.Entity.Body == 1003 || clienttarget.Entity.Body == 1004)
                    {
                        attack.Attacker = client.Entity.UID;
                        attack.X = client.Entity.X;
                        attack.Y = client.Entity.Y;
                        clienttarget.Send(attack);
                        clienttarget.Entity.InteractionInProgress = true;
                        client.Entity.InteractionInProgress = true;
                        clienttarget.Entity.InteractionType = attack.Damage;
                        clienttarget.Entity.InteractionX = client.Entity.X;
                        clienttarget.Entity.InteractionY = client.Entity.Y;
                        client.Entity.InteractionType = attack.Damage;
                        client.Entity.InteractionX = client.Entity.X;
                        client.Entity.InteractionY = client.Entity.Y;
                        if (clienttarget.Entity.X == client.Entity.X && clienttarget.Entity.Y == client.Entity.Y)
                        {
                            attack.Damage = client.Entity.InteractionType;
                            clienttarget.Entity.InteractionSet = true;
                            client.Entity.InteractionSet = true;
                            attack.Attacker = clienttarget.Entity.UID;
                            attack.Attacked = client.Entity.UID;
                            attack.AttackType = 47;
                            attack.X = clienttarget.Entity.X;
                            attack.Y = clienttarget.Entity.Y;
                            attack.ResponseDamage = clienttarget.InteractionEffect;
                            clienttarget.Send(attack);
                            attack.AttackType = 49;
                            attack.Attacker = client.Entity.UID;
                            attack.Attacked = clienttarget.Entity.UID;
                            client.SendScreen(attack, true);
                            attack.Attacker = clienttarget.Entity.UID;
                            attack.Attacked = client.Entity.UID;
                            client.SendScreen(attack, true);
                        }
                    }
                    else
                    {
                        attack.AttackType = 47;
                        attack.Attacker = client.Entity.UID;
                        attack.X = client.Entity.X;
                        attack.Y = client.Entity.Y;
                        clienttarget.Send(attack);
                        clienttarget.Entity.InteractionInProgress = true;
                        client.Entity.InteractionInProgress = true;
                        clienttarget.Entity.InteractionType = attack.Damage;
                        clienttarget.Entity.InteractionX = clienttarget.Entity.X;
                        clienttarget.Entity.InteractionY = clienttarget.Entity.Y;
                        client.Entity.InteractionType = attack.Damage;
                        client.Entity.InteractionX = clienttarget.Entity.X;
                        client.Entity.InteractionY = clienttarget.Entity.Y;
                        if (clienttarget.Entity.X == client.Entity.X && clienttarget.Entity.Y == client.Entity.Y)
                        {
                            clienttarget.Entity.InteractionSet = true;
                            client.Entity.InteractionSet = true;
                            attack.Attacker = clienttarget.Entity.UID;
                            attack.Attacked = client.Entity.UID;
                            attack.X = clienttarget.Entity.X;
                            attack.Y = clienttarget.Entity.Y;
                            attack.ResponseDamage = clienttarget.InteractionEffect;
                            clienttarget.Send(attack);
                            attack.AttackType = 49;
                            client.SendScreen(attack, true);
                            attack.Attacker = client.Entity.UID;
                            attack.Attacked = clienttarget.Entity.UID;
                            client.SendScreen(attack, true);
                        }
                    }
                }
            }
        }
        public class InteractionStopEffect
        {
            public InteractionStopEffect(Network.GamePackets.Attack attack, Game.Entity a_client)
            {
                Client.GameClient client = a_client.Owner;

                if (Kernel.GamePool.ContainsKey(attack.Attacked))
                {
                    Client.GameClient clienttarget = Kernel.GamePool[attack.Attacked];
                    attack.Attacker = client.Entity.UID;
                    attack.Attacked = clienttarget.Entity.UID;
                    attack.Damage = client.Entity.InteractionType;
                    attack.X = client.Entity.X;
                    attack.Y = client.Entity.Y;
                    attack.AttackType = 50;
                    client.SendScreen(attack, true);
                    attack.Attacker = clienttarget.Entity.UID; ;
                    attack.Attacked = client.Entity.UID;
                    clienttarget.SendScreen(attack, true);
                    client.Entity.Teleport(client.Entity.MapID, client.Entity.X, client.Entity.Y);
                    clienttarget.Entity.Teleport(clienttarget.Entity.MapID, clienttarget.Entity.X, clienttarget.Entity.Y);
                    client.Entity.InteractionType = 0;
                    client.Entity.InteractionWith = 0;
                    client.Entity.InteractionInProgress = false;
                    clienttarget.Entity.InteractionType = 0;
                    clienttarget.Entity.InteractionWith = 0;
                    clienttarget.Entity.InteractionInProgress = false;
                }
            }
        }
        public class InteractionRefuse
        {
            public InteractionRefuse(Network.GamePackets.Attack attack, Game.Entity a_client)
            {
                Client.GameClient client = a_client.Owner;

                client.Entity.InteractionType = 0;
                client.Entity.InteractionWith = 0;
                client.Entity.InteractionInProgress = false;

                if (Kernel.GamePool.ContainsKey(attack.Attacked))
                {
                    Client.GameClient clienttarget = Kernel.GamePool[attack.Attacked];
                    clienttarget.Entity.InteractionType = 0;
                    clienttarget.Entity.InteractionWith = 0;
                    clienttarget.Entity.InteractionInProgress = false;
                }
            }
        }
        #endregion

        public void SendTwilightEffect(SpellUse suse, ushort X, ushort Y)
        {
            byte[] buf = new byte[50];//Coded By Professor :D
            Writer.WriteInt32(42, 0, buf);
            Writer.WriteUInt32(10010, 2, buf);
            Writer.WriteUInt32(attacker.UID, 8, buf);
            Writer.WriteUInt16(suse.X, 12, buf);
            Writer.WriteUInt16(suse.Y, 14, buf);
            //Writer.WriteUInt32(51160, 20, buf);
            Writer.WriteUInt32(1736, 22, buf);
            Writer.WriteUInt32(434, 24, buf);
            //Writer.WriteUInt32((uint)client.Entity.Facing, 26, buf);
            Writer.WriteUInt32(attacker.X, 28, buf);
            Writer.WriteUInt32(attacker.Y, 30, buf);
            attacker.Owner.Send(buf);
            FloorItem floor = new FloorItem(true);
            floor.ItemID = 40;
            floor.ItemColor = (Enums.Color)2;
            floor.Type = FloorItem.Effect;
            floor.MapID = attacker.MapID;
            floor.X = X;
            floor.Y = Y;
            while (attacker.Owner.Map.Npcs.ContainsKey(floor.UID)) floor.UID = Network.GamePackets.FloorItem.FloorUID.Next;
            attacker.Owner.Map.AddFloorItem(floor);
            attacker.Owner.SendScreenSpawn(floor, true);

        }
        public Entity findClosestTarget(Entity attacker, ushort X, ushort Y, IEnumerable<Interfaces.IMapObject> Array)
        {
            Entity closest = attacker;
            int dPrev = 10000, dist = 0;
            foreach (var _obj in Array)
            {
                if (_obj == null) continue;
                if (_obj.MapObjType != MapObjectType.Player && _obj.MapObjType != MapObjectType.Monster) continue;
                dist = Kernel.GetDistance(X, Y, _obj.X, _obj.Y);
                if (dist < dPrev)
                {
                    dPrev = dist;
                    closest = (Entity)_obj;
                }
            }
            return closest;
        }

        private void Execute()
        {
            #region interactions
            if (attack != null)
            {
                switch (attack.AttackType)
                {
                    case (uint)Network.GamePackets.Attack.InteractionRequest:
                        new InteractionRequest(attack, attacker);
                        return;
                    case (uint)Network.GamePackets.Attack.InteractionEffect:
                        new InteractionEffect(attack, attacker);
                        return;
                    case (uint)Network.GamePackets.Attack.InteractionAccept:
                        new InteractionAccept(attack, attacker);
                        return;
                    case (uint)Network.GamePackets.Attack.InteractionRefuse:
                        new InteractionRefuse(attack, attacker);
                        return;
                    case (uint)Network.GamePackets.Attack.InteractionStopEffect:
                        new InteractionStopEffect(attack, attacker);
                        return;
                }
            }
            #endregion
            #region Monster -> Player \ Monster
            if (attack == null)
            {
                if (attacker.EntityFlag != EntityFlag.Monster)
                    return;
                if (attacker.Companion)
                {
                    if (Constants.PKForbiddenMaps.Contains(attacker.MapID))
                        return;
                }
                if (attacked.EntityFlag == EntityFlag.Player)
                {
                    if (!attacked.Owner.Attackable)
                        return;
                    if (attacked.Dead)
                        return;
                    #region Guard Shakle && Snow 
                    if (attacker.Name == "Guard1")// Guard Name :D 
                    {

                        uint rand = (uint)Conquer_Online_Server.ServerBase.Kernel.Random.Next(1, 4);
                        switch (rand)
                        {
                            case 1:
                                attacker.MonsterInfo.SpellID = 30012;// Freeze 
                                break;
                            case 2:
                                attacker.MonsterInfo.SpellID = 30011;// Shakle 
                                break;

                        }
                        if (Kernel.Rate(10))
                        {
                            attacker.MonsterInfo.SpellID = 30011;
                        }
                        if (Kernel.Rate(7))
                        {
                            attacker.MonsterInfo.SpellID = 30012;
                        }
                        #region Shakle TeamXor
                        if (attacker.MonsterInfo.SpellID == 30011)
                        {
                            SpellUse suse = new SpellUse(true);
                            attack = new Attack(true);
                            attack.Effect1 = Attack.AttackEffects1.None;
                            uint damage = 0;
                            damage += (uint)Kernel.Random.Next(2700, 5000);
                            suse.Effect1 = attack.Effect1;
                            if (attacked.Hitpoints <= damage)
                            {
                                attacked.Die(attacker);
                            }
                            else
                            {
                                attacked.Hitpoints -= damage;
                                attacked.Owner.Entity.BlockStamp = Time32.Now;
                                attacked.Owner.Entity.BlockTime = 25;
                                GameCharacterUpdates update = new GameCharacterUpdates(true);
                                update.UID = attacked.UID;
                                update.Add(GameCharacterUpdates.SoulShacle, 0, 10);
                                attacked.Owner.SendScreen(update, true);
                                attacked.AddFlag(Update.Flags2.SoulShackle);
                            }
                            if (attacker.Companion)
                                attacker.Owner.IncreaseExperience(Math.Min(damage, attacked.Hitpoints), true);

                            suse.Attacker = attacker.UID;
                            suse.SpellID = attacker.MonsterInfo.SpellID;
                            suse.X = attacked.X;
                            suse.Y = attacked.Y;
                            suse.AddTarget(attacked.UID, damage, attack);
                            foreach (var obj in attacked.Owner.Screen.Objects)
                            {
                                if (Calculations.InBox(obj.X, obj.Y, attacker.X, attacker.Y, 10))
                                {
                                    if (obj.MapObjType == MapObjectType.Player)
                                    {
                                        if (obj.Owner.Entity.ContainsFlag(Update.Flags.Dead))
                                            continue;
                                        attacked = obj as Entity;
                                        if (attacked.Hitpoints <= damage)
                                        {
                                            attacked.Die(attacker);
                                        }
                                        else
                                        {
                                            attacked.Hitpoints -= damage;
                                            attacked.Owner.Entity.ShackleStamp = Time32.Now;
                                            attacked.Owner.Entity.ShackleTime = 25;// Shakle Time ( 25 Sec Now) 
                                            GameCharacterUpdates update = new GameCharacterUpdates(true);
                                            update.UID = attacked.UID;
                                            update.Add(GameCharacterUpdates.SoulShacle, 0, 25);
                                            attacked.Owner.SendScreen(update, true);
                                            if (attacked.EntityFlag == EntityFlag.Player)
                                            {
                                                attacked.Owner.Send(Constants.Shackled(attacked.ShackleTime));

                                                attacked.AddFlag2(Update.Flags2.SoulShackle);

                                            }

                                        }
                                        suse.AddTarget(attacked.UID, damage, attack);
                                    }
                                }
                            }
                            attacked.Owner.SendScreen(suse, true);
                        }
                        #endregion


                        #region Freeze TeamXorr
                        if (attacker.MonsterInfo.SpellID == 30012)
                        {
                            SpellUse suse = new SpellUse(true);
                            attack = new Attack(true);
                            attack.Effect1 = Attack.AttackEffects1.None;
                            uint damage = 0;
                            damage += (uint)Kernel.Random.Next(2700, 5000);
                            suse.Effect1 = attack.Effect1;
                            if (attacked.Hitpoints <= damage)
                            {
                                attacked.Die(attacker);
                            }
                            else
                            {
                                attacked.Hitpoints -= damage;
                                attacked.Owner.Entity.BlockStamp = Time32.Now;
                                attacked.Owner.Entity.BlockTime = 25;
                                GameCharacterUpdates update = new GameCharacterUpdates(true);
                                update.UID = attacked.UID;
                                update.Add(GameCharacterUpdates.Freeze, 0, 10);
                                attacked.Owner.SendScreen(update, true);
                                attacked.AddFlag(Update.Flags.Freeze);
                            }
                            if (attacker.Companion)
                                attacker.Owner.IncreaseExperience(Math.Min(damage, attacked.Hitpoints), true);

                            suse.Attacker = attacker.UID;
                            suse.SpellID = attacker.MonsterInfo.SpellID;
                            suse.X = attacked.X;
                            suse.Y = attacked.Y;
                            suse.AddTarget(attacked.UID, damage, attack);
                            foreach (var obj in attacked.Owner.Screen.Objects)
                            {
                                if (Calculations.InBox(obj.X, obj.Y, attacker.X, attacker.Y, 10))
                                {
                                    if (obj.MapObjType == MapObjectType.Player)
                                    {
                                        if (obj.Owner.Entity.ContainsFlag(Update.Flags.Dead))
                                            continue;
                                        attacked = obj as Entity;
                                        if (attacked.Hitpoints <= damage)
                                        {
                                            attacked.Die(attacker);
                                        }
                                        else
                                        {
                                            attacked.Hitpoints -= damage;
                                            attacked.Owner.Entity.BlockStamp = Time32.Now;
                                            attacked.Owner.Entity.BlockTime = 25;
                                            GameCharacterUpdates update = new GameCharacterUpdates(true);
                                            update.UID = attacked.UID;
                                            update.Add(GameCharacterUpdates.Freeze, 0, 10);
                                            attacked.Owner.SendScreen(update, true);
                                            attacked.AddFlag(Update.Flags.Freeze);
                                        }

                                        suse.AddTarget(attacked.UID, damage, attack);
                                    }
                                }
                            }
                            attacked.Owner.SendScreen(suse, true);
                        }
                        #endregion

                    }

                    #endregion
                    #region SnowBanhe
                    if (attacker.Name == "SnowBanshee")
                    {

                        uint rand = (uint)Conquer_Online_Server.Kernel.Random.Next(1, 4);
                        switch (rand)
                        {
                            case 1:
                                attacker.MonsterInfo.SpellID = 10001;
                                break;
                            case 2:
                                attacker.MonsterInfo.SpellID = 30010;
                                break;
                            case 3:
                                attacker.MonsterInfo.SpellID = 10001;
                                break;
                            case 4:
                                attacker.MonsterInfo.SpellID = 30010;
                                break;
                        }
                        if (Kernel.Rate(5))
                        {
                            attacker.MonsterInfo.SpellID = 30011;
                        }
                        if (Kernel.Rate(5))
                        {
                            attacker.MonsterInfo.SpellID = 30012;
                        }

                        #region IceThrom AngerCop
                        if (attacker.MonsterInfo.SpellID == 30010 || attacker.MonsterInfo.SpellID == 10001)
                        {
                            uint damage = 0;
                            damage += (uint)Kernel.Random.Next(1500, 3000);
                            if (attacked.Hitpoints <= damage)
                            {
                                attacked.Die(attacker);
                            }
                            else
                            {
                                attacked.Hitpoints -= damage;
                            }
                            if (attacker.Companion)
                                attacker.Owner.IncreaseExperience(Math.Min(damage, attacked.Hitpoints), true);
                            SpellUse suse = new SpellUse(true);
                            suse.Attacker = attacker.UID;
                            suse.SpellID = attacker.MonsterInfo.SpellID;
                            suse.X = attacked.X;
                            suse.Y = attacked.Y;
                            suse.AddTarget(attacked.UID, damage, attack);
                            attacked.Owner.SendScreen(suse, true);
                        }
                        #endregion
                        #region Chill
                        if (attacker.MonsterInfo.SpellID == 30011)
                        {
                            SpellUse suse = new SpellUse(true);
                            attack = new Attack(true);
                            attack.Effect1 = Attack.AttackEffects1.None;
                            uint damage = 0;
                            damage += (uint)Kernel.Random.Next(5000, 8000);
                            suse.Effect1 = attack.Effect1;
                            if (attacked.Hitpoints <= damage)
                            {
                                attacked.Die(attacker);
                            }
                            else
                            {
                                attacked.Hitpoints -= damage;
                                attacked.Owner.ChaosStamp = Time32.Now;
                                attacked.Owner.Entity.ChaosTime = 10;
                                var upd = new GameCharacterUpdates(true);
                                upd.UID = attacked.UID;
                                upd.Add(GameCharacterUpdates.Flustered, 0, 10);
                                attacked.Owner.SendScreen(upd, true);
                                attacked.Owner.Entity.AddFlag(Update.Flags.Confused);

                            }
                            if (attacker.Companion)
                                attacker.Owner.IncreaseExperience(Math.Min(damage, attacked.Hitpoints), true);

                            suse.Attacker = attacker.UID;
                            suse.SpellID = attacker.MonsterInfo.SpellID;
                            suse.X = attacked.X;
                            suse.Y = attacked.Y;
                            suse.AddTarget(attacked.UID, damage, attack);
                            foreach (var obj in attacked.Owner.Screen.Objects)
                            {
                                if (Calculations.InBox(obj.X, obj.Y, attacker.X, attacker.Y, 14))
                                {
                                    if (obj.MapObjType == MapObjectType.Player)
                                    {
                                        if (obj.Owner.Entity.ContainsFlag(Update.Flags.Dead))
                                            continue;
                                        attacked = obj as Entity;
                                        if (attacked.Hitpoints <= damage)
                                        {
                                            attacked.Die(attacker);
                                        }
                                        else
                                        {
                                            attacked.Hitpoints -= damage;
                                            attacked.Owner.ChaosStamp = Time32.Now;
                                            attacked.Owner.Entity.ChaosTime = 10;
                                            var upd = new GameCharacterUpdates(true);
                                            upd.UID = attacked.UID;
                                            upd.Add(GameCharacterUpdates.Flustered, 0, 10);
                                            attacked.Owner.SendScreen(upd, true);
                                            attacked.Owner.Entity.AddFlag(Update.Flags.Confused);
                                        }

                                        suse.AddTarget(attacked.UID, damage, attack);
                                    }
                                }
                            }
                            attacked.Owner.SendScreen(suse, true);
                        }
                        #endregion
                        #region AngerCrop
                        if (attacker.MonsterInfo.SpellID == 30012)
                        {
                            SpellUse suse = new SpellUse(true);
                            attack = new Attack(true);
                            attack.Effect1 = Attack.AttackEffects1.None;
                            uint damage = 0;
                            damage += (uint)Kernel.Random.Next(2700, 5000);
                            suse.Effect1 = attack.Effect1;
                            if (attacked.Hitpoints <= damage)
                            {
                                attacked.Die(attacker);
                            }
                            else
                            {
                                attacked.Hitpoints -= damage;
                                attacked.Owner.Entity.BlockStamp = Time32.Now;
                                attacked.Owner.Entity.BlockTime = 10;
                                GameCharacterUpdates update = new GameCharacterUpdates(true);
                                update.UID = attacked.UID;
                                update.Add(GameCharacterUpdates.Freeze, 0, 10);
                                attacked.Owner.SendScreen(update, true);
                                attacked.AddFlag(Update.Flags.Freeze);
                            }
                            if (attacker.Companion)
                                attacker.Owner.IncreaseExperience(Math.Min(damage, attacked.Hitpoints), true);

                            suse.Attacker = attacker.UID;
                            suse.SpellID = attacker.MonsterInfo.SpellID;
                            suse.X = attacked.X;
                            suse.Y = attacked.Y;
                            suse.AddTarget(attacked.UID, damage, attack);
                            foreach (var obj in attacked.Owner.Screen.Objects)
                            {
                                if (Calculations.InBox(obj.X, obj.Y, attacker.X, attacker.Y, 10))
                                {
                                    if (obj.MapObjType == MapObjectType.Player)
                                    {
                                        if (obj.Owner.Entity.ContainsFlag(Update.Flags.Dead))
                                            continue;
                                        attacked = obj as Entity;
                                        if (attacked.Hitpoints <= damage)
                                        {
                                            attacked.Die(attacker);
                                        }
                                        else
                                        {
                                            attacked.Hitpoints -= damage;
                                            attacked.Owner.Entity.BlockStamp = Time32.Now;
                                            attacked.Owner.Entity.BlockTime = 10;
                                            GameCharacterUpdates update = new GameCharacterUpdates(true);
                                            update.UID = attacked.UID;
                                            update.Add(GameCharacterUpdates.Freeze, 0, 10);
                                            attacked.Owner.SendScreen(update, true);
                                            attacked.AddFlag(Update.Flags.Freeze);
                                        }

                                        suse.AddTarget(attacked.UID, damage, attack);
                                    }
                                }
                            }
                            attacked.Owner.SendScreen(suse, true);
                        }
                        #endregion

                    }

                    #endregion
                    #region NemesisTyrant
                    if (attacker.Name == "NemesisTyrant")
                    {

                        uint rand = (uint)Conquer_Online_Server.Kernel.Random.Next(1, 4);
                        switch (rand)
                        {
                            case 1:
                                attacker.MonsterInfo.SpellID = 10001;
                                break;
                            case 2:
                                attacker.MonsterInfo.SpellID = 30010;
                                break;
                            case 3:
                                attacker.MonsterInfo.SpellID = 10001;
                                break;
                            case 4:
                                attacker.MonsterInfo.SpellID = 30010;
                                break;
                        }
                        if (Kernel.Rate(5))
                        {
                            attacker.MonsterInfo.SpellID = 30011;
                        }
                        if (Kernel.Rate(5))
                        {
                            attacker.MonsterInfo.SpellID = 30012;
                        }

                        #region IceThrom AngerCop
                        if (attacker.MonsterInfo.SpellID == 30010 || attacker.MonsterInfo.SpellID == 10001)
                        {
                            uint damage = 0;
                            damage += (uint)Kernel.Random.Next(1500, 3000);
                            if (attacked.Hitpoints <= damage)
                            {
                                attacked.Die(attacker);
                            }
                            else
                            {
                                attacked.Hitpoints -= damage;
                            }
                            if (attacker.Companion)
                                attacker.Owner.IncreaseExperience(Math.Min(damage, attacked.Hitpoints), true);
                            SpellUse suse = new SpellUse(true);
                            suse.Attacker = attacker.UID;
                            suse.SpellID = attacker.MonsterInfo.SpellID;
                            suse.X = attacked.X;
                            suse.Y = attacked.Y;
                            suse.AddTarget(attacked.UID, damage, attack);
                            attacked.Owner.SendScreen(suse, true);
                        }
                        #endregion
                        #region Chill
                        if (attacker.MonsterInfo.SpellID == 30011)
                        {
                            SpellUse suse = new SpellUse(true);
                            attack = new Attack(true);
                            attack.Effect1 = Attack.AttackEffects1.None;
                            uint damage = 0;
                            damage += (uint)Kernel.Random.Next(2700, 5000);
                            suse.Effect1 = attack.Effect1;
                            if (attacked.Hitpoints <= damage)
                            {
                                attacked.Die(attacker);
                            }
                            else
                            {
                                attacked.Hitpoints -= damage;
                                attacked.Owner.ChaosStamp = Time32.Now;
                                attacked.Owner.Entity.ChaosTime = 5;
                                var upd = new GameCharacterUpdates(true);
                                upd.UID = attacked.UID;
                                upd.Add(GameCharacterUpdates.Flustered, 0, 5);
                                attacked.Owner.SendScreen(upd, true);
                                attacked.Owner.Entity.AddFlag(Update.Flags.Confused);

                            }
                            if (attacker.Companion)
                                attacker.Owner.IncreaseExperience(Math.Min(damage, attacked.Hitpoints), true);

                            suse.Attacker = attacker.UID;
                            suse.SpellID = attacker.MonsterInfo.SpellID;
                            suse.X = attacked.X;
                            suse.Y = attacked.Y;
                            suse.AddTarget(attacked.UID, damage, attack);
                            foreach (var obj in attacked.Owner.Screen.Objects)
                            {
                                if (Calculations.InBox(obj.X, obj.Y, attacker.X, attacker.Y, 14))
                                {
                                    if (obj.MapObjType == MapObjectType.Player)
                                    {
                                        if (obj.Owner.Entity.ContainsFlag(Update.Flags.Dead))
                                            continue;
                                        attacked = obj as Entity;
                                        if (attacked.Hitpoints <= damage)
                                        {
                                            attacked.Die(attacker);
                                        }
                                        else
                                        {
                                            attacked.Hitpoints -= damage;
                                            attacked.Owner.ChaosStamp = Time32.Now;
                                            attacked.Owner.Entity.ChaosTime = 10;
                                            var upd = new GameCharacterUpdates(true);
                                            upd.UID = attacked.UID;
                                            upd.Add(GameCharacterUpdates.Flustered, 0, 10);
                                            attacked.Owner.SendScreen(upd, true);
                                            attacked.Owner.Entity.AddFlag(Update.Flags.Confused);
                                        }

                                        suse.AddTarget(attacked.UID, damage, attack);
                                    }
                                }
                            }
                            attacked.Owner.SendScreen(suse, true);
                        }
                        #endregion
                        #region AngerCrop
                        if (attacker.MonsterInfo.SpellID == 30012)
                        {
                            SpellUse suse = new SpellUse(true);
                            attack = new Attack(true);
                            attack.Effect1 = Attack.AttackEffects1.None;
                            uint damage = 0;
                            damage += (uint)Kernel.Random.Next(2700, 5000);
                            suse.Effect1 = attack.Effect1;
                            if (attacked.Hitpoints <= damage)
                            {
                                attacked.Die(attacker);
                            }
                            else
                            {
                                attacked.Hitpoints -= damage;
                                attacked.Owner.Entity.BlockStamp = Time32.Now;
                                attacked.Owner.Entity.BlockTime = 5;
                                GameCharacterUpdates update = new GameCharacterUpdates(true);
                                update.UID = attacked.UID;
                                update.Add(GameCharacterUpdates.Freeze, 0, 5);
                                attacked.Owner.SendScreen(update, true);
                                attacked.AddFlag(Update.Flags.Freeze);
                            }
                            if (attacker.Companion)
                                attacker.Owner.IncreaseExperience(Math.Min(damage, attacked.Hitpoints), true);

                            suse.Attacker = attacker.UID;
                            suse.SpellID = attacker.MonsterInfo.SpellID;
                            suse.X = attacked.X;
                            suse.Y = attacked.Y;
                            suse.AddTarget(attacked.UID, damage, attack);
                            foreach (var obj in attacked.Owner.Screen.Objects)
                            {
                                if (Calculations.InBox(obj.X, obj.Y, attacker.X, attacker.Y, 10))
                                {
                                    if (obj.MapObjType == MapObjectType.Player)
                                    {
                                        if (obj.Owner.Entity.ContainsFlag(Update.Flags.Dead))
                                            continue;
                                        attacked = obj as Entity;
                                        if (attacked.Hitpoints <= damage)
                                        {
                                            attacked.Die(attacker);
                                        }
                                        else
                                        {
                                            attacked.Hitpoints -= damage;
                                            attacked.Owner.Entity.BlockStamp = Time32.Now;
                                            attacked.Owner.Entity.BlockTime = 5;
                                            GameCharacterUpdates update = new GameCharacterUpdates(true);
                                            update.UID = attacked.UID;
                                            update.Add(GameCharacterUpdates.Freeze, 0, 5);
                                            attacked.Owner.SendScreen(update, true);
                                            attacked.AddFlag(Update.Flags.Freeze);
                                        }

                                        suse.AddTarget(attacked.UID, damage, attack);
                                    }
                                }
                            }
                            attacked.Owner.SendScreen(suse, true);
                        }
                        #endregion

                    }

                    #endregion  
                    #region TreatoDragon
                    if (attacker.Name == "TeratoDragon" || attacker.Name == "SnowDemon" || attacker.Name == "EvilMonkMisery" || attacker.Name == "FlameDevastator" || attacker.Name == "FuriousDevastato" || attacker.Name == "AwakeDevastator" || attacker.Name == "FuriousDevil")  
                    {
                        uint rand = (uint)Conquer_Online_Server.Kernel.Random.Next(1, 4);
                        switch (rand)
                        {
                            case 1:
                                attacker.MonsterInfo.SpellID = 7014;
                                break;
                            case 2:
                                attacker.MonsterInfo.SpellID = 7017;
                                break;
                            case 3:
                                attacker.MonsterInfo.SpellID = 7014;
                                break;
                            case 4:
                                attacker.MonsterInfo.SpellID = 7017;
                                break;
                        }
                        if (Kernel.Rate(5))
                        {
                            attacker.MonsterInfo.SpellID = 7013;
                        }
                        if (Kernel.Rate(5))
                        {
                            attacker.MonsterInfo.SpellID = 7015;
                        }
                        #region TD Area
                        if (attacker.MonsterInfo.SpellID == 7014 || attacker.MonsterInfo.SpellID == 7017)
                        {
                            SpellUse suse = new SpellUse(true);
                            attack = new Attack(true);
                            attack.Effect1 = Attack.AttackEffects1.None;
                            uint damage = 0;
                            damage += (uint)Kernel.Random.Next(1500, 3000);
                            suse.Effect1 = attack.Effect1;
                            if (attacked.Hitpoints <= damage)
                            {
                                attacked.Die(attacker);
                            }
                            else
                            {
                                attacked.Hitpoints -= damage;
                            }
                            if (attacker.Companion)
                                attacker.Owner.IncreaseExperience(Math.Min(damage, attacked.Hitpoints), true);

                            suse.Attacker = attacker.UID;
                            suse.SpellID = attacker.MonsterInfo.SpellID;
                            suse.X = attacked.X;
                            suse.Y = attacked.Y;
                            suse.AddTarget(attacked.UID, damage, attack);
                            foreach (var obj in attacked.Owner.Screen.Objects)
                            {
                                if (Calculations.InBox(obj.X, obj.Y, attacker.X, attacker.Y, 14))
                                {
                                    if (obj.MapObjType == MapObjectType.Player)
                                    {
                                        if (obj.Owner.Entity.ContainsFlag(Update.Flags.Dead))
                                            continue;
                                        attacked = obj as Entity;
                                        if (attacked.Hitpoints <= damage)
                                        {
                                            attacked.Die(attacker);
                                        }
                                        else
                                        {
                                            attacked.Hitpoints -= damage;
                                        }

                                        suse.AddTarget(attacked.UID, damage, attack);
                                    }
                                }
                            }
                            attacked.Owner.SendScreen(suse, true);
                        }
                    }
                        #endregion
                    #region 2nd skill
                    if (attacker.MonsterInfo.SpellID == 7013)
                    {
                        SpellUse suse = new SpellUse(true);
                        attack = new Attack(true);
                        attack.Effect1 = Attack.AttackEffects1.None;
                        uint damage = 0;
                        damage += (uint)Kernel.Random.Next(2700, 5000);
                        suse.Effect1 = attack.Effect1;
                        if (attacked.Hitpoints <= damage)
                        {
                            attacked.Die(attacker);
                        }
                        else
                        {
                            attacked.Hitpoints -= damage;
                            attacked.Owner.FrightenStamp = Time32.Now;
                            attacked.Owner.Entity.Fright = 15;
                            var upd = new GameCharacterUpdates(true);
                            upd.UID = attacked.UID;
                            upd.Add(GameCharacterUpdates.Dizzy, 0, 15);
                            attacked.Owner.SendScreen(upd, true);
                            attacked.Owner.Entity.AddFlag(Update.Flags.Frightened);
                        }
                        if (attacker.Companion)
                            attacker.Owner.IncreaseExperience(Math.Min(damage, attacked.Hitpoints), true);
                        if (attacker.BodyGuard)
                            attacker.Owner.IncreaseExperience(Math.Min(damage, attacked.Hitpoints), true);
                        suse.Attacker = attacker.UID;
                        suse.SpellID = attacker.MonsterInfo.SpellID;
                        suse.X = attacked.X;
                        suse.Y = attacked.Y;
                        suse.AddTarget(attacked.UID, damage, attack);
                        foreach (var obj in attacked.Owner.Screen.Objects)
                        {
                            if (Calculations.InBox(obj.X, obj.Y, attacker.X, attacker.Y, 14))
                            {
                                if (obj.MapObjType == MapObjectType.Player)
                                {
                                    if (obj.Owner.Entity.ContainsFlag(Update.Flags.Dead))
                                        continue;
                                    attacked = obj as Entity;
                                    if (attacked.Hitpoints <= damage)
                                    {
                                        attacked.Die(attacker);
                                    }
                                    else
                                    {
                                        attacked.Hitpoints -= damage;
                                        attacked.Owner.FrightenStamp = Time32.Now;
                                        attacked.Owner.Entity.Fright = 15;
                                        var upd = new GameCharacterUpdates(true);
                                        upd.UID = attacked.UID;
                                        upd.Add(GameCharacterUpdates.Dizzy, 0, 15);
                                        attacked.Owner.SendScreen(upd, true);
                                        attacked.Owner.Entity.AddFlag(Update.Flags.Frightened);
                                    }

                                    suse.AddTarget(attacked.UID, damage, attack);
                                }
                            }
                        }
                        attacked.Owner.SendScreen(suse, true);
                    }
                    #endregion
                    #region Chill
                    if (attacker.MonsterInfo.SpellID == 7015)
                    {
                        SpellUse suse = new SpellUse(true);
                        attack = new Attack(true);
                        attack.Effect1 = Attack.AttackEffects1.None;
                        uint damage = 0;
                        damage += (uint)Kernel.Random.Next(2700, 5000);
                        suse.Effect1 = attack.Effect1;
                        if (attacked.Hitpoints <= damage)
                        {
                            attacked.Die(attacker);
                        }
                        else
                        {
                            attacked.Hitpoints -= damage;
                            attacked.Owner.ChaosStamp = Time32.Now;
                            attacked.Owner.Entity.ChaosTime = 15;
                            var upd = new GameCharacterUpdates(true);
                            upd.UID = attacked.UID;
                            upd.Add(GameCharacterUpdates.Flustered, 0, 15);
                            attacked.Owner.SendScreen(upd, true);
                            attacked.Owner.Entity.AddFlag(Update.Flags.Confused);

                        }
                        if (attacker.Companion)
                            attacker.Owner.IncreaseExperience(Math.Min(damage, attacked.Hitpoints), true);
                        if (attacker.BodyGuard)
                            attacker.Owner.IncreaseExperience(Math.Min(damage, attacked.Hitpoints), true);
                        suse.Attacker = attacker.UID;
                        suse.SpellID = attacker.MonsterInfo.SpellID;
                        suse.X = attacked.X;
                        suse.Y = attacked.Y;
                        suse.AddTarget(attacked.UID, damage, attack);
                        foreach (var obj in attacked.Owner.Screen.Objects)
                        {
                            if (Calculations.InBox(obj.X, obj.Y, attacker.X, attacker.Y, 14))
                            {
                                if (obj.MapObjType == MapObjectType.Player)
                                {
                                    if (obj.Owner.Entity.ContainsFlag(Update.Flags.Dead))
                                        continue;
                                    attacked = obj as Entity;
                                    if (attacked.Hitpoints <= damage)
                                    {
                                        attacked.Die(attacker);
                                    }
                                    else
                                    {
                                        attacked.Hitpoints -= damage;
                                        attacked.Owner.ChaosStamp = Time32.Now;
                                        attacked.Owner.Entity.ChaosTime = 15;
                                        var upd = new GameCharacterUpdates(true);
                                        upd.UID = attacked.UID;
                                        upd.Add(GameCharacterUpdates.Flustered, 0, 15);
                                        attacked.Owner.SendScreen(upd, true);
                                        attacked.Owner.Entity.AddFlag(Update.Flags.Confused);
                                    }

                                    suse.AddTarget(attacked.UID, damage, attack);
                                }
                            }
                        }
                        attacked.Owner.SendScreen(suse, true);
                    }
                    #endregion
                    #endregion
                    
                    if (attacker.MonsterInfo.SpellID == 0)
                    {
                        attack = new Attack(true);
                        attack.Effect1 = Attack.AttackEffects1.None;
                        uint damage = Calculate.Melee(attacker, attacked, ref attack);
                        attack.Attacker = attacker.UID;
                        attack.Attacked = attacked.UID;
                        attack.AttackType = Attack.Melee;
                        attack.Damage = damage;
                        attack.X = attacked.X;
                        attack.Y = attacked.Y;

                        if (attacked.Hitpoints <= damage)
                        {
                            attacked.Owner.SendScreen(attack, true);
                            attacked.Die(attacker.UID);
                        }
                        else
                        {
                            attacked.Hitpoints -= damage;
                            attacked.Owner.SendScreen(attack, true);
                        }
                    }
                    else
                    {
                        SpellUse suse = new SpellUse(true);
                        attack = new Attack(true);
                        attack.Effect1 = Attack.AttackEffects1.None;
                        uint damage = Calculate.Magic(attacker, attacked, attacker.MonsterInfo.SpellID, 0, ref attack);
                        if (attacker.BodyGuard)
                            attacker.Owner.IncreaseExperience(Math.Min(damage, attacked.Hitpoints), true);
                        suse.Effect1 = attack.Effect1;

                        if (attacked.Hitpoints <= damage)
                        {
                            attacked.Die(attacker.UID);
                        }
                        else
                        {
                            attacked.Hitpoints -= damage;
                        }
                        if (attacker.Companion)
                            attacker.Owner.IncreaseExperience(Math.Min(damage, attacked.Hitpoints), true);

                        suse.Attacker = attacker.UID;
                        suse.SpellID = attacker.MonsterInfo.SpellID;
                        suse.X = attacked.X;
                        suse.Y = attacked.Y;
                        suse.AddTarget(attacked.UID, damage, attack);
                        attacked.Owner.SendScreen(suse, true);
                    }
                }
                else
                {
                    if (attacker.MonsterInfo.SpellID == 0)
                    {
                        attack = new Attack(true);
                        attack.Effect1 = Attack.AttackEffects1.None;
                        uint damage = Calculate.Melee(attacker, attacked, ref attack);
                        attack.Attacker = attacker.UID;
                        attack.Attacked = attacked.UID;
                        attack.AttackType = Attack.Melee;
                        attack.Damage = damage;
                        attack.X = attacked.X;
                        attack.Y = attacked.Y;
                        attacked.MonsterInfo.SendScreen(attack);
                        if (attacker.Companion || attacker.BodyGuard)
                            if (damage > attacked.Hitpoints)
                                attacker.Owner.IncreaseExperience(Math.Min(damage, attacked.Hitpoints), true);
                            else
                                attacker.Owner.IncreaseExperience(damage, true);
                        if (attacked.Hitpoints <= damage)
                        {
                            attacked.Die(attacker);
                            attack = new Attack(true);
                            attack.Attacker = attacker.UID;
                            attack.Attacked = attacked.UID;
                            attack.AttackType = Network.GamePackets.Attack.Kill;
                            attack.X = attacked.X;
                            attack.Y = attacked.Y;
                            attacked.MonsterInfo.SendScreen(attack);
                        }
                        else
                        {
                            attacked.Hitpoints -= damage;
                        }
                    }
                    else
                    {
                        SpellUse suse = new SpellUse(true);
                        attack = new Attack(true);
                        attack.Effect1 = Attack.AttackEffects1.None;
                        uint damage = Calculate.Magic(attacker, attacked, attacker.MonsterInfo.SpellID, 0, ref attack);
                        if (attacker.BodyGuard)
                            attacker.Owner.IncreaseExperience(Math.Min(damage, attacked.Hitpoints), true);
                        suse.Effect1 = attack.Effect1;

                        suse.Attacker = attacker.UID;
                        suse.SpellID = attacker.MonsterInfo.SpellID;
                        suse.X = attacked.X;
                        suse.Y = attacked.Y;
                        suse.AddTarget(attacked.UID, damage, attack);
                        attacked.MonsterInfo.SendScreen(suse);
                        if (attacker.Companion || attacker.BodyGuard)
                            if (damage > attacked.Hitpoints)
                                attacker.Owner.IncreaseExperience(Math.Min(damage, attacked.Hitpoints), true);
                            else
                                attacker.Owner.IncreaseExperience(damage, true);
                        if (attacked.Hitpoints <= damage)
                        {
                            attacked.Die(attacker);
                            attack = new Attack(true);
                            attack.Attacker = attacker.UID;
                            attack.Attacked = attacked.UID;
                            attack.AttackType = Network.GamePackets.Attack.Kill;
                            attack.X = attacked.X;
                            attack.Y = attacked.Y;
                            attacked.MonsterInfo.SendScreen(attack);
                        }
                        else
                        {
                            attacked.Hitpoints -= damage;
                        }
                    }
                }
            }
            #endregion
            #region Player -> Player \ Monster \ Sob Npc
            else
            {
                #region Merchant
                if (attack.AttackType == Attack.MerchantAccept || attack.AttackType == Attack.MerchantRefuse)
                {

                    attacker.AttackPacket = null;
                    return;
                }
                #endregion
                #region Marriage
                if (attack.AttackType == Attack.MarriageAccept || attack.AttackType == Attack.MarriageRequest)
                {
                    if (attack.AttackType == Attack.MarriageRequest)
                    {
                        Client.GameClient Spouse = null;
                        uint takeout = attack.Attacked;
                        if (takeout == attacker.UID)
                            takeout = attack.Attacker;
                        if (Kernel.GamePool.TryGetValue(takeout, out Spouse))
                        {
                            if (attacker.Spouse != "None" || Spouse.Entity.Spouse != "None")
                            {
                                attacker.Owner.Send(new Message("You cannot marry someone that is already married with someone else!", System.Drawing.Color.Black, Message.TopLeft));
                            }
                            else
                            {
                                uint id1 = attacker.Mesh % 10, id2 = Spouse.Entity.Mesh % 10;

                                if (id1 <= 2 && id2 >= 3 || id1 >= 2 && id2 <= 3)
                                {

                                    attack.X = Spouse.Entity.X;
                                    attack.Y = Spouse.Entity.Y;

                                    Spouse.Send(attack);
                                }
                                else
                                {
                                    attacker.Owner.Send(new Message("You cannot marry someone of your gender!", System.Drawing.Color.Black, Message.TopLeft));
                                }
                            }
                        }
                    }
                    else
                    {
                        Client.GameClient Spouse = null;
                        if (Kernel.GamePool.TryGetValue(attack.Attacked, out Spouse))
                        {
                            if (attacker.Spouse != "None" || Spouse.Entity.Spouse != "None")
                            {
                                attacker.Owner.Send(new Message("You cannot marry someone that is already married with someone else!", System.Drawing.Color.Black, Message.TopLeft));
                            }
                            else
                            {
                                if (attacker.Mesh % 10 <= 2 && Spouse.Entity.Mesh % 10 >= 3 || attacker.Mesh % 10 >= 3 && Spouse.Entity.Mesh % 10 <= 2)
                                {
                                    Spouse.Entity.Spouse = attacker.Name;
                                    attacker.Spouse = Spouse.Entity.Name;
                                    Message message = null;
                                    if (Spouse.Entity.Mesh % 10 >= 3)
                                        message = new Message("Joy and happiness! " + Spouse.Entity.Name + " and " + attacker.Name + " have joined together in the holy marriage. We wish them a stone house.", System.Drawing.Color.BurlyWood, Message.Center);
                                    else
                                        message = new Message("Joy and happiness! " + attacker.Name + " and " + attacker.Spouse + " have joined together in the holy marriage. We wish them a stone house.", System.Drawing.Color.BurlyWood, Message.Center);

                                    foreach (Client.GameClient client in Program.GamePool)
                                    {
                                        client.Send(message);
                                    }

                                    Spouse.Entity.Update(_String.Effect, "firework-2love", true);
                                    attacker.Update(_String.Effect, "firework-2love", true);
                                }
                                else
                                {
                                    attacker.Owner.Send(new Message("You cannot marry someone of your gender!", System.Drawing.Color.Black, Message.TopLeft));
                                }
                            }
                        }
                    }
                }
                #endregion
                #region Attacking
                else
                {
                    attacker.Owner.Attackable = true;
                    Entity attacked = null;

                    SobNpcSpawn attackedsob = null;

                    #region Checks
                    if (attack.Attacker != attacker.UID)
                        return;
                    if (attacker.EntityFlag != EntityFlag.Player)
                        return;
                    attacker.RemoveFlag(Update.Flags.Invisibility);

                    bool pass = false;
                    if (attack.AttackType == Attack.Melee)
                    {
                        if (attacker.OnFatalStrike())
                        {
                            if (attack.Attacked < 600000)
                            {
                                pass = true;
                            }
                        }
                    }
                    ushort decrease = 0;
                    if (attacker.OnCyclone())
                        decrease = 1;//Ahmed Samak :  to fix the cyclone in scatter issue .. the value replaced from 700 to 1;

                    if (attacker.OnSuperman())
                        decrease = 300;
                    if (!pass && attack.AttackType != Attack.Magic)
                    {
                        int milliSeconds = 1000 - attacker.Agility - decrease;
                        if (milliSeconds < 0 || milliSeconds > 5000)
                            milliSeconds = 0;
                        if (Time32.Now < attacker.AttackStamp.AddMilliseconds(milliSeconds))
                            return;

                        attacker.AttackStamp = Time32.Now;
                    }
                    if (attacker.Dead)
                    {
                        if (attacker.AttackPacket != null)
                            attacker.AttackPacket = null;
                        return;
                    }
                    if (attacker.Owner.InQualifier())
                    {
                        if (Time32.Now < attacker.Owner.ImportTime().AddSeconds(8))
                        {
                            return;
                        }
                    }
                    bool doWep1Spell = false, doWep2Spell = false;
                restart:

                    #region Extract attack information
                    ushort SpellID = 0, X = 0, Y = 0;
                    uint Target = 0;
                    if (attack.AttackType == Attack.Magic)
                    {
                        if (!attack.Decoded)
                        {
                            #region GetSkillID
                            SpellID = Convert.ToUInt16(((long)attack.ToArray()[24 + 4] & 0xFF) | (((long)attack.ToArray()[25 + 4] & 0xFF) << 8));
                            SpellID ^= (ushort)0x915d;
                            SpellID ^= (ushort)attacker.UID;
                            SpellID = (ushort)(SpellID << 0x3 | SpellID >> 0xd);
                            SpellID -= 0xeb42;
                            #endregion
                            #region GetCoords
                            X = (ushort)((attack.ToArray()[16 + 4] & 0xFF) | ((attack.ToArray()[17 + 4] & 0xFF) << 8));
                            X = (ushort)(X ^ (uint)(attacker.UID & 0xffff) ^ 0x2ed6);
                            X = (ushort)(((X << 1) | ((X & 0x8000) >> 15)) & 0xffff);
                            X = (ushort)((X | 0xffff0000) - 0xffff22ee);

                            Y = (ushort)((attack.ToArray()[18 + 4] & 0xFF) | ((attack.ToArray()[19 + 4] & 0xFF) << 8));
                            Y = (ushort)(Y ^ (uint)(attacker.UID & 0xffff) ^ 0xb99b);
                            Y = (ushort)(((Y << 5) | ((Y & 0xF800) >> 11)) & 0xffff);
                            Y = (ushort)((Y | 0xffff0000) - 0xffff8922);
                            #endregion
                            #region GetTarget
                            Target = ((uint)attack.ToArray()[12 + 4] & 0xFF) | (((uint)attack.ToArray()[13 + 4] & 0xFF) << 8) | (((uint)attack.ToArray()[14 + 4] & 0xFF) << 16) | (((uint)attack.ToArray()[15 + 4] & 0xFF) << 24);
                            Target = ((((Target & 0xffffe000) >> 13) | ((Target & 0x1fff) << 19)) ^ 0x5F2D2463 ^ attacker.UID) - 0x746F4AE6;
                            #endregion

                            attack.X = X;
                            attack.Y = Y;
                            attack.Damage = SpellID;
                            attack.Attacked = Target;
                            attack.Decoded = true;
                        }
                        else
                        {
                            X = attack.X;
                            Y = attack.Y;
                            SpellID = (ushort)attack.Damage;
                            Target = attack.Attacked;
                        }
                    }
                    #endregion

                    if (!pass && attack.AttackType == Attack.Magic)
                    {
                        if (!(doWep1Spell || doWep2Spell))
                        {
                            if (SpellID == 1045 || SpellID == 1046 || SpellID == 11005 || SpellID == 11000 || SpellID == 1100) // FB and SS
                            {
                                //do checks
                            }
                            else
                            {
                                int milliSeconds = 1000 - attacker.Agility - decrease;
                                if (milliSeconds < 0 || milliSeconds > 5000)
                                    milliSeconds = 0;
                                if (Time32.Now < attacker.AttackStamp.AddMilliseconds(milliSeconds))
                                    return;
                            }

                            attacker.AttackStamp = Time32.Now;
                        }
                    }
                    #endregion

                    if (attacker.MapID == SteedRace.MAPID)
                    {
                        if (attacker.ContainsFlag(Update.Flags.Ride))
                        {
                            attacker.Owner.Send(new NpcReply(NpcReply.MessageBox, "Do you want to quit the steed race?"));
                            attacker.Owner.MessageOK = (pClient) =>
                            {
                                pClient.Entity.Teleport(1002, 400, 400);
                                pClient.Entity.RemoveFlag(Update.Flags.Ride);
                            };
                            attacker.Owner.MessageCancel = null;
                        }
                        return;
                    }

                    if (attacker.ContainsFlag(Update.Flags.Ride) && attacker.Owner.Equipment.TryGetItem(18) == null)
                    {
                        if (attack.AttackType != Attack.Magic)
                            attacker.RemoveFlag(Update.Flags.Ride);
                        else
                            if (!(SpellID == 7003 || SpellID == 7002))
                                attacker.RemoveFlag(Update.Flags.Ride);
                    }
                    if (attacker.ContainsFlag(Update.Flags.CastPray))
                        attacker.RemoveFlag(Update.Flags.CastPray);
                    if (attacker.ContainsFlag(Update.Flags.Praying))
                        attacker.RemoveFlag(Update.Flags.Praying);

                    #region Dash
                    if (SpellID == 1051)
                    {
                        if (attacker.MapID == DeathMatch.MAPID)
                        {
                            attacker.Owner.Send(new Message("You have to use manual linear skills(FastBlade/ScentSword)", System.Drawing.Color.Red, Message.Talk));
                            return;
                        }
                        if (Kernel.GetDistance(attack.X, attack.Y, attacker.X, attacker.Y) > 4)
                        {
                            attacker.Owner.Disconnect();
                            return;
                        }
                        attacker.X = attack.X; attacker.Y = attack.Y;
                        ushort x = attacker.X, y = attacker.Y;
                        Game.Map.UpdateCoordonatesForAngle(ref x, ref y, (Enums.ConquerAngle)Target);
                        foreach (Interfaces.IMapObject obj in attacker.Owner.Screen.Objects)
                        {
                            if (obj == null)
                                continue;
                            if (obj.X == x && obj.Y == y && (obj.MapObjType == MapObjectType.Monster || obj.MapObjType == MapObjectType.Player))
                            {
                                Entity entity = obj as Entity;
                                if (!entity.Dead)
                                {
                                    Target = obj.UID;
                                    break;
                                }
                            }
                        }
                    }
                    #endregion
                    #region CounterKill
                    if (attack.AttackType == Attack.CounterKillSwitch)
                    {
                        if (attacker.MapID == DeathMatch.MAPID)
                        {
                            attacker.Owner.Send(new Message("You have to use manual linear skills(FastBlade/ScentSword)", System.Drawing.Color.Red, Message.Talk));
                            return;
                        }
                        if (attacked != null)
                            if (attacked.ContainsFlag(Update.Flags.Fly)) { attacker.AttackPacket = null; return; }
                        if (attacker != null)
                            if (attacker.ContainsFlag(Update.Flags.Fly)) { attacker.AttackPacket = null; return; }
                        if (attacker.Owner.Spells.ContainsKey(6003))
                        {
                            if (!attacker.CounterKillSwitch)
                            {
                                if (Time32.Now >= attacker.CounterKillStamp.AddSeconds(30))
                                {
                                    attacker.CounterKillStamp = Time32.Now;
                                    attacker.CounterKillSwitch = true;
                                    Attack m_attack = new Attack(true);
                                    m_attack.Attacked = attacker.UID;
                                    m_attack.Attacker = attacker.UID;
                                    m_attack.AttackType = Attack.CounterKillSwitch;
                                    m_attack.Damage = 1;
                                    m_attack.X = attacker.X;
                                    m_attack.Y = attacker.Y;
                                    m_attack.Send(attacker.Owner);
                                }
                            }
                            else
                            {
                                attacker.CounterKillSwitch = false;
                                Attack m_attack = new Attack(true);
                                m_attack.Attacked = attacker.UID;
                                m_attack.Attacker = attacker.UID;
                                m_attack.AttackType = Attack.CounterKillSwitch;
                                m_attack.Damage = 0;
                                m_attack.X = attacker.X;
                                m_attack.Y = attacker.Y;
                                m_attack.Send(attacker.Owner);
                            }

                            attacker.Owner.IncreaseSpellExperience(100, 6003);
                            attacker.AttackPacket = null;
                        }
                    }
                    #endregion
                    #region Melee
                    else if (attack.AttackType == Attack.Melee)
                    {
                        if (attacker.MapID == DeathMatch.MAPID)
                        {
                            attacker.Owner.Send(new Message("You have to use manual linear skills(FastBlade/ScentSword)", System.Drawing.Color.Red, Message.Talk));
                            return;
                        }
                        if (attacker.Owner.Screen.TryGetValue(attack.Attacked, out attacked))
                        {
                            CheckForExtraWeaponPowers(attacker.Owner, attacked);
                            if (!CanAttack(attacker, attacked, null, attack.AttackType == Attack.Melee)) return;
                            pass = false;
                            if (attacker.OnFatalStrike())
                            {
                                if (attacked.EntityFlag == EntityFlag.Monster)
                                {
                                    pass = true;
                                }
                            }
                            ushort range = attacker.AttackRange;
                            if (attacker.Transformed)
                                range = (ushort)attacker.TransformationAttackRange;
                            if (Kernel.GetDistance(attacker.X, attacker.Y, attacked.X, attacked.Y) <= range || pass)
                            {
                                attack.Effect1 = Attack.AttackEffects1.None;
                                #region DragonPunch
                                if (attacker.DragonWarrior())
                                {
                                    if (attacker.Owner.Spells.ContainsKey(12240))
                                    {
                                        var spell = Database.SpellTable.GetSpell(12240, attacker.Owner);
                                        if (spell != null)
                                        {
                                            spell.CanKill = true;
                                            if (Kernel.Rate(spell.Percent))
                                            {
                                                SpellUse suse = new SpellUse(true);
                                                suse.Attacker = attacker.UID;
                                                suse.SpellID = spell.ID;
                                                suse.SpellLevel = spell.Level;
                                                suse.X = attacker.X;
                                                suse.Y = attacker.Y;
                                                IMapObject lastAttacked = attacker;
                                                if (Handle.CanAttack(attacker, attacked, spell, false))
                                                {
                                                    lastAttacked = attacked;
                                                    uint damages = Game.Attacking.Calculate.Melee(attacker, attacked, spell, ref attack);
                                                    suse.Effect1 = attack.Effect1;
                                                    Handle.ReceiveAttack(attacker, attacked, attack, damages, spell);
                                                    suse.AddTarget(attacked.UID, damages, attack);
                                                }
                                                foreach (Interfaces.IMapObject _obj in attacker.Owner.Screen.Objects)
                                                {
                                                    if (_obj == null) continue;
                                                    if (_obj.MapObjType == MapObjectType.Monster || _obj.MapObjType == MapObjectType.Player)
                                                    {
                                                        if (_obj.UID == attacked.UID) continue;
                                                        var attacked1 = _obj as Entity;
                                                        if (Kernel.GetDistance(lastAttacked.X, lastAttacked.Y, attacked1.X, attacked1.Y) <= 5)
                                                        {
                                                            if (Handle.CanAttack(attacker, attacked1, spell, false))
                                                            {
                                                                lastAttacked = attacked1;
                                                                uint damages = Game.Attacking.Calculate.Melee(attacker, attacked1, spell, ref attack);
                                                                suse.Effect1 = attack.Effect1;
                                                                if (damages == 0) break;
                                                                Handle.ReceiveAttack(attacker, attacked1, attack, damages, spell);
                                                                suse.AddTarget(attacked1.UID, damages, attack);
                                                            }
                                                        }
                                                    }
                                                    else if (_obj.MapObjType == MapObjectType.SobNpc)
                                                    {
                                                        attackedsob = _obj as SobNpcSpawn;
                                                        if (Kernel.GetDistance(lastAttacked.X, lastAttacked.Y, attackedsob.X, attackedsob.Y) <= 5)
                                                        {
                                                            if (Handle.CanAttack(attacker, attackedsob, spell))
                                                            {
                                                                lastAttacked = attackedsob;
                                                                uint damages = Game.Attacking.Calculate.Melee(attacker, attackedsob, ref attack);
                                                                suse.Effect1 = attack.Effect1;
                                                                if (damages == 0) break;
                                                                Handle.ReceiveAttack(attacker, attackedsob, attack, damages, spell);
                                                                suse.AddTarget(attackedsob.UID, damages, attack);
                                                            }
                                                        }
                                                    }
                                                }
                                                attacker.Owner.SendScreen(suse, true);
                                                return;
                                            }
                                        }

                                    }

                                }
                                #endregion
                                uint damage = Game.Attacking.Calculate.Melee(attacker, attacked, ref attack);
                                attack.Damage = damage;
                                if (attacker.OnFatalStrike())
                                {
                                    if (attacked.EntityFlag == EntityFlag.Monster)
                                    {
                                        var weaps = attacker.Owner.Weapons;
                                        bool can = false;
                                        if (weaps.Item1 != null)
                                            if (weaps.Item1.ID / 1000 == 601)
                                                can = true;
                                        if (weaps.Item2 != null)
                                            if (weaps.Item2.ID / 1000 == 601)
                                                can = true;
                                        if (!can)
                                            return;
                                        ushort x = attacked.X;
                                        ushort y = attacked.Y;
                                        Map.UpdateCoordonatesForAngle(ref x, ref y, Kernel.GetAngle(attacked.X, attacked.Y, attacker.X, attacker.Y));
                                        attacker.Shift(x, y);
                                        attack.X = x;
                                        attack.Y = y;
                                        //double dmg = (double)damage * 3.5;
                                        //damage = (uint)dmg;
                                        damage = damage / 2;
                                        attack.Damage = damage;

                                        attack.AttackType = Attack.FatalStrike;
                                    }
                                }
                                //over:

                                var weapons = attacker.Owner.Weapons;
                                if (weapons.Item1 != null)
                                {
                                    ConquerItem rightweapon = weapons.Item1;
                                    ushort wep1subyte = (ushort)(rightweapon.ID / 1000), wep2subyte = 0;
                                    bool wep1bs = false, wep2bs = false;
                                    if (wep1subyte == 421)
                                    {
                                        wep1bs = true;
                                        wep1subyte--;
                                    }
                                    ushort wep1spellid = 0, wep2spellid = 0;
                                    if (Database.SpellTable.WeaponSpells.ContainsKey(wep1subyte))
                                        wep1spellid = Database.SpellTable.WeaponSpells[wep1subyte];
                                    Database.SpellInformation wep1spell = null, wep2spell = null;
                                    if (attacker.Owner.Spells.ContainsKey(wep1spellid) && Database.SpellTable.SpellInformations.ContainsKey(wep1spellid))
                                    {
                                        wep1spell = Database.SpellTable.SpellInformations[wep1spellid][attacker.Owner.Spells[wep1spellid].Level];
                                        doWep1Spell = Kernel.Rate(wep1spell.Percent);
                                        if (attacked.EntityFlag == EntityFlag.Player && wep1spellid == 10490)
                                            doWep1Spell = Kernel.Rate(5);
                                    }
                                    if (!doWep1Spell)
                                    {
                                        if (weapons.Item2 != null)
                                        {
                                            ConquerItem leftweapon = weapons.Item2;
                                            wep2subyte = (ushort)(leftweapon.ID / 1000);
                                            if (wep2subyte == 421)
                                            {
                                                wep2bs = true;
                                                wep2subyte--;
                                            }
                                            if (Database.SpellTable.WeaponSpells.ContainsKey(wep2subyte))
                                                wep2spellid = Database.SpellTable.WeaponSpells[wep2subyte];
                                            if (attacker.Owner.Spells.ContainsKey(wep2spellid) && Database.SpellTable.SpellInformations.ContainsKey(wep2spellid))
                                            {
                                                wep2spell = Database.SpellTable.SpellInformations[wep2spellid][attacker.Owner.Spells[wep2spellid].Level];
                                                doWep2Spell = Kernel.Rate(wep2spell.Percent);
                                                if (attacked.EntityFlag == EntityFlag.Player && wep2spellid == 10490)
                                                    doWep2Spell = Kernel.Rate(5);
                                            }
                                        }
                                    }
                                    #region 11960
                                    if (attacker.EpicTrojan())
                                    {
                                        if (attacker.Owner.Spells.ContainsKey(11960))
                                        {
                                            var spell = Database.SpellTable.GetSpell(11960, attacker.Owner);
                                            if (spell != null)
                                            {
                                                spell.CanKill = true;
                                                if (Kernel.Rate(32))
                                                {
                                                    SpellUse suse = new SpellUse(true);
                                                    suse.Attacker = attacker.UID;
                                                    suse.SpellID = spell.ID;
                                                    suse.SpellLevel = spell.Level;
                                                    suse.X = attacker.X;
                                                    suse.Y = attacker.Y;
                                                    IMapObject lastAttacked = attacker;
                                                    foreach (Interfaces.IMapObject _obj in attacker.Owner.Screen.Objects)
                                                    {
                                                        if (_obj == null) continue;
                                                        if (_obj.MapObjType == MapObjectType.Monster || _obj.MapObjType == MapObjectType.Player)
                                                        {
                                                            if (_obj.UID == attacked.UID) continue;
                                                            var attacked1 = _obj as Entity;
                                                            if (Kernel.GetDistance(attacker.X, attacker.Y, attacked1.X, attacked1.Y) <= 5)
                                                            {
                                                                if (Handle.CanAttack(attacker, attacked1, spell, false))
                                                                {
                                                                    lastAttacked = attacked1;
                                                                    uint damages = Game.Attacking.Calculate.Melee(attacker, attacked1, spell, ref attack);
                                                                    suse.Effect1 = attack.Effect1;
                                                                    if (damages == 0) break;
                                                                    Handle.ReceiveAttack(attacker, attacked1, attack, damages, spell);
                                                                    suse.AddTarget(attacked1.UID, damages, attack);
                                                                    byte num = 0;
                                                                    switch (spell.Level)
                                                                    {
                                                                        case 0: num = 5; break;
                                                                        case 1: num = 6; break;
                                                                        case 2: num = 7; break;
                                                                        case 3: num = 8; break;
                                                                        case 4: num = 9; break;
                                                                        case 5: num = 10; break;
                                                                        case 6: num = 11; break;
                                                                        case 7: num = 12; break;
                                                                        case 8: num = 14; break;
                                                                        case 9: num = 20; break;

                                                                    }
                                                                    if (attacker.Stamina <= 150)
                                                                    {
                                                                        attacker.Stamina += num;
                                                                        attacker.Owner.Send(new Message("the~caster~has~a~100%~chance~to~recover~Stamina~by~" + num + ".", System.Drawing.Color.Red, Message.TopLeft));
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        else if (_obj.MapObjType == MapObjectType.SobNpc)
                                                        {
                                                            attackedsob = _obj as SobNpcSpawn;
                                                            if (Kernel.GetDistance(lastAttacked.X, lastAttacked.Y, attackedsob.X, attackedsob.Y) <= 5)
                                                            {
                                                                if (Handle.CanAttack(attacker, attackedsob, spell))
                                                                {
                                                                    lastAttacked = attackedsob;
                                                                    uint damages = Game.Attacking.Calculate.Melee(attacker, attackedsob, ref attack);
                                                                    suse.Effect1 = attack.Effect1;
                                                                    if (damages == 0) break;
                                                                    Handle.ReceiveAttack(attacker, attackedsob, attack, damages, spell);
                                                                    suse.AddTarget(attackedsob.UID, damages, attack);
                                                                }
                                                            }
                                                        }
                                                    }
                                                    attacker.Owner.SendScreen(suse, true);
                                                    return;
                                                }
                                            }
                                        }
                                    }
                                    #endregion
                                    #region 11990
                                    if (attacker.EpicTrojan())
                                    {
                                        if (attacker.Owner.Spells.ContainsKey(11990))
                                        {
                                            var spell = Database.SpellTable.GetSpell(11990, attacker.Owner);
                                            if (spell != null)
                                            {
                                                spell.CanKill = true;
                                                if (Kernel.Rate(32))
                                                {
                                                    SpellUse suse = new SpellUse(true);
                                                    suse.Attacker = attacker.UID;
                                                    suse.SpellID = spell.ID;
                                                    suse.SpellLevel = spell.Level;
                                                    suse.X = attacker.X;
                                                    suse.Y = attacker.Y;
                                                    IMapObject lastAttacked = attacker;
                                                    if (Handle.CanAttack(attacker, attacked, spell, false))
                                                    {
                                                        lastAttacked = attacked;
                                                        uint damages = Game.Attacking.Calculate.Melee(attacker, attacked, spell, ref attack);
                                                        suse.Effect1 = attack.Effect1;
                                                        Handle.ReceiveAttack(attacker, attacked, attack, damages, spell);
                                                        suse.AddTarget(attacked.UID, damages, attack);
                                                    }
                                                    foreach (Interfaces.IMapObject _obj in attacker.Owner.Screen.Objects)
                                                    {
                                                        if (_obj == null) continue;
                                                        if (_obj.MapObjType == MapObjectType.Monster || _obj.MapObjType == MapObjectType.Player)
                                                        {
                                                            if (_obj.UID == attacked.UID) continue;
                                                            var attacked1 = _obj as Entity;
                                                            if (Kernel.GetDistance(lastAttacked.X, lastAttacked.Y, attacked1.X, attacked1.Y) <= 5)
                                                            {
                                                                if (Handle.CanAttack(attacker, attacked1, spell, false))
                                                                {
                                                                    lastAttacked = attacked1;
                                                                    uint damages = Game.Attacking.Calculate.Melee(attacker, attacked1, spell, ref attack);
                                                                    suse.Effect1 = attack.Effect1;
                                                                    if (damages == 0) break;
                                                                    Handle.ReceiveAttack(attacker, attacked1, attack, damages, spell);
                                                                    suse.AddTarget(attacked1.UID, damages, attack);
                                                                }
                                                            }
                                                        }
                                                        else if (_obj.MapObjType == MapObjectType.SobNpc)
                                                        {
                                                            attackedsob = _obj as SobNpcSpawn;
                                                            if (Kernel.GetDistance(lastAttacked.X, lastAttacked.Y, attackedsob.X, attackedsob.Y) <= 5)
                                                            {
                                                                if (Handle.CanAttack(attacker, attackedsob, spell))
                                                                {
                                                                    lastAttacked = attackedsob;
                                                                    uint damages = Game.Attacking.Calculate.Melee(attacker, attackedsob, ref attack);
                                                                    suse.Effect1 = attack.Effect1;
                                                                    if (damages == 0) break;
                                                                    Handle.ReceiveAttack(attacker, attackedsob, attack, damages, spell);
                                                                    suse.AddTarget(attackedsob.UID, damages, attack);
                                                                }
                                                            }
                                                        }
                                                    }
                                                    attacker.Owner.SendScreen(suse, true);
                                                    return;
                                                }
                                            }
                                        }
                                    }
                                    #endregion
                                    #region FatalSpin
                                    if (attacker.EpicNinja())
                                    {
                                        if (attacker.Owner.Spells.ContainsKey(12110))
                                        {
                                            var spell = Database.SpellTable.GetSpell(12110, attacker.Owner);
                                            if (spell != null)
                                            {
                                                spell.CanKill = true;
                                                if (Kernel.Rate(spell.Percent))
                                                {
                                                    SpellUse suse = new SpellUse(true);
                                                    suse.Attacker = attacker.UID;
                                                    suse.SpellID = spell.ID;
                                                    suse.SpellLevel = spell.Level;
                                                    suse.X = attacker.X;
                                                    suse.Y = attacker.Y;
                                                    IMapObject lastAttacked = attacker;
                                                    uint p = 0;
                                                    if (Handle.CanAttack(attacker, attacked, spell, false))
                                                    {
                                                        lastAttacked = attacked;
                                                        uint damages = Game.Attacking.Calculate.Melee(attacker, attacked, spell, ref attack);
                                                        suse.Effect1 = attack.Effect1;
                                                        Handle.ReceiveAttack(attacker, attacked, attack, damages, spell);
                                                        suse.AddTarget(attacked.UID, damages, attack);
                                                    }
                                                    foreach (Interfaces.IMapObject _obj in attacker.Owner.Screen.Objects)
                                                    {
                                                        if (_obj == null) continue;
                                                        if (_obj.MapObjType == MapObjectType.Monster || _obj.MapObjType == MapObjectType.Player)
                                                        {
                                                            if (_obj.UID == attacked.UID) continue;
                                                            var attacked1 = _obj as Entity;
                                                            if (Kernel.GetDistance(lastAttacked.X, lastAttacked.Y, attacked1.X, attacked1.Y) <= 5)
                                                            {
                                                                if (Handle.CanAttack(attacker, attacked1, spell, false))
                                                                {
                                                                    lastAttacked = attacked1;
                                                                    uint damages = Game.Attacking.Calculate.Melee(attacker, attacked1, spell, ref attack);
                                                                    damage = (uint)(damage * 15);
                                                                    suse.Effect1 = attack.Effect1;
                                                                    if (damages == 0) break;
                                                                    Handle.ReceiveAttack(attacker, attacked1, attack, damages, spell);
                                                                    suse.AddTarget(attacked1.UID, damages, attack);
                                                                }
                                                            }
                                                        }
                                                        else if (_obj.MapObjType == MapObjectType.SobNpc)
                                                        {
                                                            attackedsob = _obj as SobNpcSpawn;
                                                            if (Kernel.GetDistance(lastAttacked.X, lastAttacked.Y, attackedsob.X, attackedsob.Y) <= 5)
                                                            {
                                                                if (Handle.CanAttack(attacker, attackedsob, spell))
                                                                {
                                                                    lastAttacked = attackedsob;
                                                                    uint damages = Game.Attacking.Calculate.Melee(attacker, attackedsob, ref attack);
                                                                    damage = (uint)(damage * 15);
                                                                    suse.Effect1 = attack.Effect1;
                                                                    if (damages == 0) break;
                                                                    Handle.ReceiveAttack(attacker, attackedsob, attack, damages, spell);
                                                                    suse.AddTarget(attackedsob.UID, damages, attack);
                                                                }
                                                            }
                                                        }
                                                    }
                                                    attacker.Owner.SendScreen(suse, true);
                                                    return;
                                                }
                                            }

                                        }

                                    }
                                    #endregion
                                    if (!attacker.Transformed)
                                    {
                                        if (doWep1Spell)
                                        {
                                            attack.AttackType = Attack.Magic;
                                            attack.Decoded = true;
                                            attack.X = attacked.X;
                                            attack.Y = attacked.Y;
                                            attack.Attacked = attacked.UID;
                                            attack.Damage = wep1spell.ID;
                                            goto restart;
                                        }
                                        if (doWep2Spell)
                                        {
                                            attack.AttackType = Attack.Magic;
                                            attack.Decoded = true;
                                            attack.X = attacked.X;
                                            attack.Y = attacked.Y;
                                            attack.Attacked = attacked.UID;
                                            attack.Damage = wep2spell.ID;
                                            goto restart;
                                        }
                                        if (wep1bs)
                                            wep1subyte++;
                                        if (attacker.EntityFlag == EntityFlag.Player && attacked.EntityFlag != EntityFlag.Player)
                                            if (damage > attacked.Hitpoints)
                                            {
                                                attacker.Owner.IncreaseProficiencyExperience(Math.Min(damage, attacked.Hitpoints), wep1subyte);
                                                if (wep2subyte != 0)
                                                {
                                                    if (wep2bs)
                                                        wep2subyte++;
                                                    attacker.Owner.IncreaseProficiencyExperience(Math.Min(damage, attacked.Hitpoints), wep2subyte);
                                                }
                                            }
                                            else
                                            {
                                                attacker.Owner.IncreaseProficiencyExperience(damage, wep1subyte);
                                                if (wep2subyte != 0)
                                                {
                                                    if (wep2bs)
                                                        wep2subyte++;
                                                    attacker.Owner.IncreaseProficiencyExperience(damage, wep2subyte);
                                                }
                                            }
                                    }
                                }
                                else
                                {
                                    if (!attacker.Transformed)
                                    {
                                        if (attacker.EntityFlag == EntityFlag.Player && attacked.EntityFlag != EntityFlag.Player)
                                            if (damage > attacked.Hitpoints)
                                            {
                                                attacker.Owner.IncreaseProficiencyExperience(Math.Min(damage, attacked.Hitpoints), 0);
                                            }
                                            else
                                            {
                                                attacker.Owner.IncreaseProficiencyExperience(damage, 0);
                                            }
                                    }
                                }
                                ReceiveAttack(attacker, attacked, attack, damage, null);
                                attack.AttackType = Attack.Melee;
                            }
                            else
                            {
                                attacker.AttackPacket = null;
                            }
                        }
                        else if (attacker.Owner.Screen.TryGetSob(attack.Attacked, out attackedsob))
                        {
                            CheckForExtraWeaponPowers(attacker.Owner, null);
                            if (CanAttack(attacker, attackedsob, null))
                            {
                                ushort range = attacker.AttackRange;
                                if (attacker.Transformed)
                                    range = (ushort)attacker.TransformationAttackRange;
                                if (Kernel.GetDistance(attacker.X, attacker.Y, attackedsob.X, attackedsob.Y) <= range)
                                {
                                    attack.Effect1 = Attack.AttackEffects1.None;
                                    uint damage = Game.Attacking.Calculate.Melee(attacker, attackedsob, ref attack);

                                    var weapons = attacker.Owner.Weapons;
                                    if (weapons.Item1 != null)
                                    {
                                        ConquerItem rightweapon = weapons.Item1;
                                        ushort wep1subyte = (ushort)(rightweapon.ID / 1000), wep2subyte = 0;
                                        bool wep1bs = false, wep2bs = false;
                                        if (wep1subyte == 421)
                                        {
                                            wep1bs = true;
                                            wep1subyte--;
                                        }
                                        ushort wep1spellid = 0, wep2spellid = 0;
                                        if (Database.SpellTable.WeaponSpells.ContainsKey(wep1subyte))
                                            wep1spellid = Database.SpellTable.WeaponSpells[wep1subyte];
                                        Database.SpellInformation wep1spell = null, wep2spell = null;
                                        if (attacker.Owner.Spells.ContainsKey(wep1spellid) && Database.SpellTable.SpellInformations.ContainsKey(wep1spellid))
                                        {
                                            wep1spell = Database.SpellTable.SpellInformations[wep1spellid][attacker.Owner.Spells[wep1spellid].Level];
                                            doWep1Spell = Kernel.Rate(wep1spell.Percent);
                                        }
                                        if (!doWep1Spell)
                                        {
                                            if (weapons.Item2 != null)
                                            {
                                                ConquerItem leftweapon = weapons.Item2;
                                                wep2subyte = (ushort)(leftweapon.ID / 1000);
                                                if (wep2subyte == 421)
                                                {
                                                    wep2bs = true;
                                                    wep2subyte--;
                                                }
                                                if (Database.SpellTable.WeaponSpells.ContainsKey(wep2subyte))
                                                    wep2spellid = Database.SpellTable.WeaponSpells[wep2subyte];
                                                if (attacker.Owner.Spells.ContainsKey(wep2spellid) && Database.SpellTable.SpellInformations.ContainsKey(wep2spellid))
                                                {
                                                    wep2spell = Database.SpellTable.SpellInformations[wep2spellid][attacker.Owner.Spells[wep2spellid].Level];
                                                    doWep2Spell = Kernel.Rate(wep2spell.Percent);
                                                }
                                            }
                                        }

                                        if (!attacker.Transformed)
                                        {
                                            if (doWep1Spell)
                                            {
                                                attack.AttackType = Attack.Magic;
                                                attack.Decoded = true;
                                                attack.X = attackedsob.X;
                                                attack.Y = attackedsob.Y;
                                                attack.Attacked = attackedsob.UID;
                                                attack.Damage = wep1spell.ID;
                                                goto restart;
                                            }
                                            if (doWep2Spell)
                                            {
                                                attack.AttackType = Attack.Magic;
                                                attack.Decoded = true;
                                                attack.X = attackedsob.X;
                                                attack.Y = attackedsob.Y;
                                                attack.Attacked = attackedsob.UID;
                                                attack.Damage = wep2spell.ID;
                                                goto restart;
                                            }
                                            if (attacker.MapID == 1039)
                                            {
                                                if (wep1bs)
                                                    wep1subyte++;
                                                if (attacker.EntityFlag == EntityFlag.Player)
                                                    if (damage > attackedsob.Hitpoints)
                                                    {
                                                        attacker.Owner.IncreaseProficiencyExperience(Math.Min(damage, attackedsob.Hitpoints), wep1subyte);
                                                        if (wep2subyte != 0)
                                                        {
                                                            if (wep2bs)
                                                                wep2subyte++;
                                                            attacker.Owner.IncreaseProficiencyExperience(Math.Min(damage, attackedsob.Hitpoints), wep2subyte);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        attacker.Owner.IncreaseProficiencyExperience(damage, wep1subyte);
                                                        if (wep2subyte != 0)
                                                        {
                                                            if (wep2bs)
                                                                wep2subyte++;
                                                            attacker.Owner.IncreaseProficiencyExperience(damage, wep2subyte);
                                                        }
                                                    }
                                            }
                                        }
                                    }
                                    attack.Damage = damage;
                                    ReceiveAttack(attacker, attackedsob, attack, damage, null);
                                }
                                else
                                {
                                    attacker.AttackPacket = null;
                                }
                            }
                        }
                        else
                        {
                            attacker.AttackPacket = null;
                        }
                    }
                    #endregion
                    #region Ranged
                    else if (attack.AttackType == Attack.Ranged)
                    {
                        if (attacker.MapID == DeathMatch.MAPID)
                        {
                            attacker.Owner.Send(new Message("You have to use manual linear skills(FastBlade/ScentSword)", System.Drawing.Color.Red, Message.Talk));
                            return;
                        }
                        if (attacker.Owner.Screen.TryGetValue(attack.Attacked, out attacked))
                        {
                            CheckForExtraWeaponPowers(attacker.Owner, attacked);
                            if (!CanAttack(attacker, attacked, null, false))
                                return;
                            var weapons = attacker.Owner.Weapons;
                            if (weapons.Item1 == null) return;
                            if (weapons.Item1.ID / 1000 != 500 && weapons.Item1.ID / 1000 != 613)
                                return;

                            if (weapons.Item1.ID / 1000 == 500)
                                if (weapons.Item2 != null)
                                    if (!PacketHandler.IsArrow(weapons.Item2.ID))
                                        return;

                            #region Kinetic Spark
                            if (attacker.ContainsFlag3(Update.Flags3.KineticSpark))
                            {
                                var spell = Database.SpellTable.GetSpell(11590, attacker.Owner);
                                if (spell != null)
                                {
                                    spell.CanKill = true;
                                    if (Kernel.Rate(spell.Percent))
                                    {
                                        SpellUse suse = new SpellUse(true);
                                        suse.Attacker = attacker.UID;
                                        suse.SpellID = spell.ID;
                                        suse.SpellLevel = spell.Level;
                                        suse.X = attacker.X;
                                        suse.Y = attacker.Y;
                                        IMapObject lastAttacked = attacker;
                                        uint p = 0;
                                        if (Handle.CanAttack(attacker, attacked, spell, false))
                                        {
                                            lastAttacked = attacked;
                                            uint damage = Game.Attacking.Calculate.Melee(attacker, attacked, spell, ref attack);
                                            suse.Effect1 = attack.Effect1;
                                            if (attacker.SpiritFocus)
                                            {
                                                damage = (uint)(damage * attacker.SpiritFocusPercent);
                                                attacker.SpiritFocus = false;
                                            }
                                            damage = damage - damage * (p += 20) / 100;
                                            Handle.ReceiveAttack(attacker, attacked, attack, damage, spell);
                                            suse.AddTarget(attacked.UID, damage, attack);
                                        }
                                        foreach (Interfaces.IMapObject _obj in attacker.Owner.Screen.Objects)
                                        {
                                            if (_obj == null) continue;
                                            if (_obj.MapObjType == MapObjectType.Monster || _obj.MapObjType == MapObjectType.Player)
                                            {
                                                if (_obj.UID == attacked.UID) continue;
                                                var attacked1 = _obj as Entity;
                                                if (Kernel.GetDistance(lastAttacked.X, lastAttacked.Y, attacked1.X, attacked1.Y) <= 5)
                                                {
                                                    if (Handle.CanAttack(attacker, attacked1, spell, false))
                                                    {
                                                        lastAttacked = attacked1;
                                                        uint damage = Game.Attacking.Calculate.Melee(attacker, attacked1, spell, ref attack);
                                                        suse.Effect1 = attack.Effect1;
                                                        if (attacker.SpiritFocus)
                                                        {
                                                            damage = (uint)(damage * attacker.SpiritFocusPercent);
                                                            attacker.SpiritFocus = false;
                                                        }
                                                        damage = damage - damage * (p += 20) / 100;
                                                        if (damage == 0) break;
                                                        Handle.ReceiveAttack(attacker, attacked1, attack, damage, spell);
                                                        suse.AddTarget(attacked1.UID, damage, attack);
                                                    }
                                                }
                                            }
                                            else if (_obj.MapObjType == MapObjectType.SobNpc)
                                            {
                                                attackedsob = _obj as SobNpcSpawn;
                                                if (Kernel.GetDistance(lastAttacked.X, lastAttacked.Y, attackedsob.X, attackedsob.Y) <= 5)
                                                {
                                                    if (Handle.CanAttack(attacker, attackedsob, spell))
                                                    {
                                                        lastAttacked = attackedsob;
                                                        uint damage = Game.Attacking.Calculate.Melee(attacker, attackedsob, ref attack);
                                                        suse.Effect1 = attack.Effect1;
                                                        if (attacker.SpiritFocus)
                                                        {
                                                            damage = (uint)(damage * attacker.SpiritFocusPercent);
                                                            attacker.SpiritFocus = false;
                                                        }
                                                        damage = damage - damage * (p += 20) / 100;
                                                        if (damage == 0) break;
                                                        Handle.ReceiveAttack(attacker, attackedsob, attack, damage, spell);
                                                        suse.AddTarget(attackedsob.UID, damage, attack);
                                                    }
                                                }
                                            }
                                        }
                                        attacker.Owner.SendScreen(suse, true);
                                        return;
                                    }
                                }
                            }
                            #endregion

                            if (Kernel.GetDistance(attacker.X, attacker.Y, attacked.X, attacked.Y) <= Constants.pScreenDistance)
                            {
                                attack.Effect1 = Attack.AttackEffects1.None;
                                uint damage = 0;

                                if (!attacker.Assassin())
                                    damage = Game.Attacking.Calculate.Ranged(attacker, attacked, ref attack);
                                else
                                    damage = Game.Attacking.Calculate.Melee(attacker, attacked, ref attack);

                                attack.Damage = damage;
                                if (attacker.EntityFlag == EntityFlag.Player && attacked.EntityFlag != EntityFlag.Player)
                                    if (damage > attacked.Hitpoints)
                                    {
                                        attacker.Owner.IncreaseProficiencyExperience(Math.Min(damage, attacked.Hitpoints), (ushort)(weapons.Item1.ID / 1000));
                                    }
                                    else
                                    {
                                        attacker.Owner.IncreaseProficiencyExperience(damage, (ushort)(weapons.Item1.ID / 1000));
                                    }
                                ReceiveAttack(attacker, attacked, attack, damage, null);
                            }
                        }
                        else if (attacker.Owner.Screen.TryGetSob(attack.Attacked, out attackedsob))
                        {
                            if (CanAttack(attacker, attackedsob, null))
                            {
                                if (attacker.Owner.Equipment.TryGetItem(ConquerItem.LeftWeapon) == null)
                                    return;

                                var weapons = attacker.Owner.Weapons;
                                if (weapons.Item1 == null) return;
                                if (weapons.Item1.ID / 1000 != 500 && weapons.Item1.ID / 1000 != 613)
                                    return;

                                if (attacker.MapID != 1039)
                                    if (weapons.Item1.ID / 1000 == 500)
                                        if (weapons.Item2 != null)
                                            if (!PacketHandler.IsArrow(weapons.Item2.ID))
                                                return;

                                #region Kinetic Spark
                                if (attacker.ContainsFlag3(Update.Flags3.KineticSpark))
                                {
                                    var spell = Database.SpellTable.GetSpell(11590, attacker.Owner);
                                    if (spell != null)
                                    {
                                        spell.CanKill = true;
                                        if (Kernel.Rate(spell.Percent))
                                        {
                                            SpellUse suse = new SpellUse(true);
                                            suse.Attacker = attacker.UID;
                                            suse.SpellID = spell.ID;
                                            suse.SpellLevel = spell.Level;
                                            suse.X = attacker.X;
                                            suse.Y = attacker.Y;

                                            IMapObject lastAttacked = attacker;
                                            uint p = 0;
                                            if (Handle.CanAttack(attacker, attackedsob, spell))
                                            {
                                                lastAttacked = attackedsob;
                                                uint damage = Game.Attacking.Calculate.Melee(attacker, attackedsob, ref attack);
                                                suse.Effect1 = attack.Effect1;
                                                if (attacker.SpiritFocus)
                                                {
                                                    damage = (uint)(damage * attacker.SpiritFocusPercent);
                                                    attacker.SpiritFocus = false;
                                                }
                                                damage = damage - damage * (p += 20) / 100;
                                                Handle.ReceiveAttack(attacker, attackedsob, attack, damage, spell);
                                                suse.AddTarget(attackedsob.UID, damage, attack);
                                            }
                                            foreach (Interfaces.IMapObject _obj in attacker.Owner.Screen.Objects)
                                            {
                                                if (_obj == null) continue;
                                                if (_obj.MapObjType == MapObjectType.Monster || _obj.MapObjType == MapObjectType.Player)
                                                {
                                                    var attacked1 = _obj as Entity;
                                                    if (Kernel.GetDistance(lastAttacked.X, lastAttacked.Y, attacked1.X, attacked1.Y) <= 5)
                                                    {
                                                        if (Handle.CanAttack(attacker, attacked1, spell, false))
                                                        {
                                                            lastAttacked = attacked1;
                                                            uint damage = Game.Attacking.Calculate.Melee(attacker, attacked1, spell, ref attack);
                                                            suse.Effect1 = attack.Effect1;
                                                            if (attacker.SpiritFocus)
                                                            {
                                                                damage = (uint)(damage * attacker.SpiritFocusPercent);
                                                                attacker.SpiritFocus = false;
                                                            }
                                                            damage = damage - damage * (p += 20) / 100;
                                                            if (damage == 0) break;
                                                            Handle.ReceiveAttack(attacker, attacked1, attack, damage, spell);
                                                            suse.AddTarget(attacked1.UID, damage, attack);
                                                        }
                                                    }
                                                }
                                                else if (_obj.MapObjType == MapObjectType.SobNpc)
                                                {
                                                    if (_obj.UID == Target) continue;
                                                    var attackedsob1 = _obj as SobNpcSpawn;
                                                    if (Kernel.GetDistance(lastAttacked.X, lastAttacked.Y, attackedsob1.X, attackedsob1.Y) <= 5)
                                                    {
                                                        if (Handle.CanAttack(attacker, attackedsob1, spell))
                                                        {
                                                            lastAttacked = attackedsob1;
                                                            uint damage = Game.Attacking.Calculate.Melee(attacker, attackedsob1, ref attack);
                                                            suse.Effect1 = attack.Effect1;
                                                            if (attacker.SpiritFocus)
                                                            {
                                                                damage = (uint)(damage * attacker.SpiritFocusPercent);
                                                                attacker.SpiritFocus = false;
                                                            }
                                                            damage = damage - damage * (p += 20) / 100;
                                                            if (damage == 0) break;
                                                            Handle.ReceiveAttack(attacker, attackedsob1, attack, damage, spell);
                                                            suse.AddTarget(attackedsob1.UID, damage, attack);
                                                        }
                                                    }
                                                }
                                            }
                                            attacker.Owner.SendScreen(suse, true);
                                            return;
                                        }
                                    }
                                }
                                #endregion

                                if (Kernel.GetDistance(attacker.X, attacker.Y, attackedsob.X, attackedsob.Y) <= Constants.pScreenDistance)
                                {
                                    attack.Effect1 = Attack.AttackEffects1.None;
                                    uint damage = 0;
                                    if (!attacker.Assassin())
                                        damage = Game.Attacking.Calculate.Ranged(attacker, attackedsob, ref attack);
                                    else
                                        damage = Game.Attacking.Calculate.Melee(attacker, attackedsob, ref attack);
                                    attack.Damage = damage;
                                    ReceiveAttack(attacker, attackedsob, attack, damage, null);

                                    if (damage > attackedsob.Hitpoints)
                                    {
                                        attacker.Owner.IncreaseProficiencyExperience(Math.Min(damage, attackedsob.Hitpoints), (ushort)(weapons.Item1.ID / 1000));
                                    }
                                    else
                                    {
                                        attacker.Owner.IncreaseProficiencyExperience(damage, (ushort)(weapons.Item1.ID / 1000));
                                    }
                                }
                            }
                        }
                        else
                        {
                            attacker.AttackPacket = null;
                        }
                    }
                    #endregion
                    #region Magic
                    else if (attack.AttackType == Attack.Magic)
                    {
                        CheckForExtraWeaponPowers(attacker.Owner, attacked);
                        uint Experience = 100;
                        bool shuriken = false;
                        ushort spellID = SpellID;
                        if (SpellID >= 3090 && SpellID <= 3306)
                            spellID = 3090;
                        if (spellID == 6012)
                            shuriken = true;

                        if (attacker.MapID == DeathMatch.MAPID)
                        {
                            if (SpellID != 1045 && SpellID != 1046)
                            {
                                attacker.Owner.Send(new Message("You have to use Melee OR Manual linear skills(FastBlade/ScentSword)", System.Drawing.Color.Red, Message.Talk));
                                return;
                            }
                        }

                        if (attacker == null)
                            return;
                        if (attacker.Owner == null)
                        {
                            attacker.AttackPacket = null;
                            return;
                        }
                        if (attacker.Owner.Spells == null)
                        {
                            attacker.Owner.Spells = new SafeDictionary<ushort, Interfaces.ISkill>();
                            attacker.AttackPacket = null;
                            return;
                        }
                        if (attacker.Owner.Spells[spellID] == null && spellID != 6012)
                        {
                            attacker.AttackPacket = null;
                            return;
                        }

                        Database.SpellInformation spell = null;
                        if (shuriken)
                            spell = Database.SpellTable.SpellInformations[6010][0];
                        else
                        {
                            byte choselevel = 0;
                            if (spellID == SpellID)
                                choselevel = attacker.Owner.Spells[spellID].Level;
                            if (Database.SpellTable.SpellInformations[SpellID] != null && !Database.SpellTable.SpellInformations[SpellID].ContainsKey(choselevel))
                                choselevel = (byte)(Database.SpellTable.SpellInformations[SpellID].Count - 1);

                            spell = Database.SpellTable.SpellInformations[SpellID][choselevel];
                        }
                        if (spell == null)
                        {
                            attacker.AttackPacket = null;
                            return;
                        }
                        attacked = null;
                        attackedsob = null;
                        if (attacker.Owner.Screen.TryGetValue(Target, out attacked) || attacker.Owner.Screen.TryGetSob(Target, out attackedsob) || Target == attacker.UID || spell.Sort != 1)
                        {
                            if (Target == attacker.UID)
                                attacked = attacker;
                            if (attacked != null)
                            {
                                if (attacked.Dead && spell.Sort != Database.SpellSort.Revive && spell.ID != 10405 && spell.ID != 10425)
                                {
                                    attacker.AttackPacket = null;
                                    return;
                                }
                            }
                            if (Target >= 400000 && Target <= 600000 || Target >= 800000)
                            {
                                if (attacked == null && attackedsob == null)
                                    return;
                            }
                            else if (Target != 0 && attackedsob == null)
                                return;
                            if (attacked != null)
                            {
                                if (attacked.EntityFlag == EntityFlag.Monster)
                                {
                                    if (spell.CanKill)
                                    {
                                        if (attacked.MonsterInfo.InSight == 0)
                                        {
                                            attacked.MonsterInfo.InSight = attacker.UID;
                                        }
                                    }
                                }
                            }
                            if (!attacker.Owner.Spells.ContainsKey(spellID))
                            {
                                if (spellID != 6012)
                                    return;
                            }
                            var weapons = attacker.Owner.Weapons;
                            if (spell != null)
                            {
                                if (spell.OnlyWithThisWeaponSubtype != 0)
                                {
                                    uint firstwepsubtype, secondwepsubtype;
                                    if (weapons.Item1 != null)
                                    {
                                        firstwepsubtype = weapons.Item1.ID / 1000;
                                        if (weapons.Item2 != null)
                                        {
                                            secondwepsubtype = weapons.Item2.ID / 1000;
                                            if (firstwepsubtype != spell.OnlyWithThisWeaponSubtype)
                                            {
                                                if (secondwepsubtype != spell.OnlyWithThisWeaponSubtype)
                                                {
                                                    attacker.AttackPacket = null;
                                                    return;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (firstwepsubtype != spell.OnlyWithThisWeaponSubtype)
                                            {
                                                attacker.AttackPacket = null;
                                                return;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        attacker.AttackPacket = null;
                                        return;
                                    }
                                }
                            }
                            switch (spellID)
                            {
                                #region EagleEye
                                case 11030:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            if (spell.ID == 10310)
                                            {
                                                if (Time32.Now > attacker.EagleEyeStamp.AddSeconds(20))
                                                    return;
                                                attacker.EagleEyeStamp = Time32.Now;
                                            }
                                            if (attacked != null)
                                            {
                                                if (Kernel.GetDistance(attacker.X, attacker.Y, attacked.X, attacked.Y) <= spell.Distance)
                                                {
                                                    SpellUse suse = new SpellUse(true);
                                                    suse.Attacker = attacker.UID;
                                                    suse.SpellID = spell.ID;
                                                    suse.SpellLevel = spell.Level;
                                                    suse.X = X;
                                                    suse.Y = Y;

                                                    if (CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                                    {
                                                        PrepareSpell(spell, attacker.Owner);

                                                        attack.Effect1 = Attack.AttackEffects1.None;
                                                        uint damage = Game.Attacking.Calculate.Magic(attacker, attacked, spell, ref attack) *2;
                                                        if (spell.ID == 11030)
                                                            damage = Game.Attacking.Calculate.Melee(attacker, attacked, spell, ref attack) *2;
                                                        suse.Effect1 = attack.Effect1;

                                                        ReceiveAttack(attacker, attacked, attack, damage, spell);

                                                        suse.AddTarget(attacked.UID, damage, attack);
                                                        attacker.Owner.Entity.IsEagleEyeShooted = true;

                                                        if (attacked.EntityFlag == EntityFlag.Player)
                                                            attacked.Owner.SendScreen(suse, true);
                                                        else
                                                            attacked.MonsterInfo.SendScreen(suse);

                                                        var attackd = attacked as Entity;
                                                        if (Kernel.BlackSpoted.ContainsKey(attackd.UID) && spell.ID == 11030)
                                                        {
                                                            attacker.Owner.Entity.IsEagleEyeShooted = false;
                                                            if (attacker.Owner.Spells.ContainsKey(11130))
                                                            {
                                                                var s = attacker.Owner.Spells[11130];
                                                                var sspell = Database.SpellTable.SpellInformations[s.ID][s.Level];
                                                                if (spell != null)
                                                                {
                                                                    attacker.EagleEyeStamp = Time32.Now.AddSeconds(-100);
                                                                    attacker.Owner.Entity.IsEagleEyeShooted = false;
                                                                    SpellUse ssuse = new SpellUse(true);
                                                                    ssuse.Attacker = attacker.UID;
                                                                    ssuse.SpellID = sspell.ID;
                                                                    ssuse.SpellLevel = sspell.Level;
                                                                    ssuse.AddTarget(attacker.Owner.Entity.UID, new SpellUse.DamageClass().Damage = 11030, attack);
                                                                    if (attacker.EntityFlag == EntityFlag.Player)
                                                                    {
                                                                        attacker.Owner.SendScreen(ssuse, true);
                                                                    }

                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    attacker.AttackPacket = null;
                                                }
                                            }
                                            else
                                            {
                                                if (Kernel.GetDistance(attacker.X, attacker.Y, attackedsob.X, attackedsob.Y) <= spell.Distance)
                                                {
                                                    SpellUse suse = new SpellUse(true);
                                                    suse.Attacker = attacker.UID;
                                                    suse.SpellID = spell.ID;
                                                    suse.SpellLevel = spell.Level;
                                                    suse.X = X;
                                                    suse.Y = Y;

                                                    if (CanAttack(attacker, attackedsob, spell))
                                                    {
                                                        PrepareSpell(spell, attacker.Owner);
                                                        attack.Effect1 = Attack.AttackEffects1.None;
                                                        uint damage = Game.Attacking.Calculate.Magic(attacker, attackedsob, spell, ref attack);
                                                        suse.Effect1 = attack.Effect1;

                                                        ReceiveAttack(attacker, attackedsob, attack, damage, spell);

                                                        suse.AddTarget(attackedsob.UID, damage, attack);

                                                        attacker.Owner.SendScreen(suse, true);
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            attacker.AttackPacket = null;
                                        }
                                        break;
                                    }
                                #endregion
                                #region Single magic damage spells
                                
                                case 1000:
                                case 1001:

                                case 1002:
                                case 1150:
                                case 1160:
                                case 1180:

                                case 1320:
                                    //case 11040:
                                    //case 10381:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            if (spell.ID == 10310)
                                            {
                                                if (Time32.Now > attacker.EagleEyeStamp.AddSeconds(20))
                                                    return;
                                                attacker.EagleEyeStamp = Time32.Now;
                                            }
                                            if (attacked != null)
                                            {
                                                if (Kernel.GetDistance(attacker.X, attacker.Y, attacked.X, attacked.Y) <= spell.Distance)
                                                {
                                                    SpellUse suse = new SpellUse(true);
                                                    suse.Attacker = attacker.UID;
                                                    suse.SpellID = spell.ID;
                                                    suse.SpellLevel = spell.Level;
                                                    suse.X = X;
                                                    suse.Y = Y;

                                                    if (CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                                    {
                                                        PrepareSpell(spell, attacker.Owner);

                                                        attack.Effect1 = Attack.AttackEffects1.None;
                                                        uint damage = Game.Attacking.Calculate.Magic(attacker, attacked, spell, ref attack)/4 ;
                                                        if (spell.ID == 11030)
                                                            damage = Game.Attacking.Calculate.Melee(attacker, attacked, spell, ref attack) /4;
                                                        suse.Effect1 = attack.Effect1;

                                                        ReceiveAttack(attacker, attacked, attack, damage, spell);

                                                        suse.AddTarget(attacked.UID, damage, attack);
                                                        attacker.Owner.Entity.IsEagleEyeShooted = true;

                                                        if (attacked.EntityFlag == EntityFlag.Player)
                                                            attacked.Owner.SendScreen(suse, true);
                                                        else
                                                            attacked.MonsterInfo.SendScreen(suse);

                                                        var attackd = attacked as Entity;
                                                        if (Kernel.BlackSpoted.ContainsKey(attackd.UID) && spell.ID == 11030)
                                                        {
                                                            attacker.Owner.Entity.IsEagleEyeShooted = false;
                                                            if (attacker.Owner.Spells.ContainsKey(11130))
                                                            {
                                                                var s = attacker.Owner.Spells[11130];
                                                                var sspell = Database.SpellTable.SpellInformations[s.ID][s.Level];
                                                                if (spell != null)
                                                                {
                                                                    attacker.EagleEyeStamp = Time32.Now.AddSeconds(-100);
                                                                    attacker.Owner.Entity.IsEagleEyeShooted = false;
                                                                    SpellUse ssuse = new SpellUse(true);
                                                                    ssuse.Attacker = attacker.UID;
                                                                    ssuse.SpellID = sspell.ID;
                                                                    ssuse.SpellLevel = sspell.Level;
                                                                    ssuse.AddTarget(attacker.Owner.Entity.UID, new SpellUse.DamageClass().Damage = 11030, attack);
                                                                    if (attacker.EntityFlag == EntityFlag.Player)
                                                                    {
                                                                        attacker.Owner.SendScreen(ssuse, true);
                                                                    }

                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    attacker.AttackPacket = null;
                                                }
                                            }
                                            else
                                            {
                                                if (Kernel.GetDistance(attacker.X, attacker.Y, attackedsob.X, attackedsob.Y) <= spell.Distance)
                                                {
                                                    SpellUse suse = new SpellUse(true);
                                                    suse.Attacker = attacker.UID;
                                                    suse.SpellID = spell.ID;
                                                    suse.SpellLevel = spell.Level;
                                                    suse.X = X;
                                                    suse.Y = Y;

                                                    if (CanAttack(attacker, attackedsob, spell))
                                                    {
                                                        PrepareSpell(spell, attacker.Owner);
                                                        attack.Effect1 = Attack.AttackEffects1.None;
                                                        uint damage = Game.Attacking.Calculate.Magic(attacker, attackedsob, spell, ref attack);
                                                        suse.Effect1 = attack.Effect1;

                                                        ReceiveAttack(attacker, attackedsob, attack, damage, spell);

                                                        suse.AddTarget(attackedsob.UID, damage, attack);

                                                        attacker.Owner.SendScreen(suse, true);
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            attacker.AttackPacket = null;
                                        }
                                        break;
                                    }
                                #endregion
                                #region Single heal/meditation spells
                                case 1190:
                                case 1195:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            PrepareSpell(spell, attacker.Owner);

                                            SpellUse suse = new SpellUse(true);
                                            suse.Attacker = attacker.UID;
                                            suse.SpellID = spell.ID;
                                            suse.SpellLevel = spell.Level;
                                            suse.X = X;
                                            suse.Y = Y;

                                            uint damage = spell.Power;
                                            if (spell.ID == 1190)
                                            {
                                                Experience = damage = Math.Min(damage, attacker.MaxHitpoints - attacker.Hitpoints);
                                                attacker.Hitpoints += damage;
                                            }
                                            else
                                            {
                                                Experience = damage = Math.Min(damage, (uint)(attacker.MaxMana - attacker.Mana));
                                                attacker.Mana += (ushort)damage;
                                            }

                                            suse.AddTarget(attacker.UID, spell.Power, attack);

                                            attacker.Owner.SendScreen(suse, true);
                                        }
                                        break;
                                    }
                                #endregion
                                #region Multi heal spells
                                case 1005:
                                case 1055:
                                case 1170:
                                case 1175:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            SpellUse suse = new SpellUse(true);
                                            suse.Attacker = attacker.UID;
                                            suse.SpellID = spell.ID;
                                            suse.SpellLevel = spell.Level;
                                            suse.X = X;
                                            suse.Y = Y;
                                            if (attackedsob != null)
                                            {
                                                if (attacker.MapID == 1038)
                                                    break;
                                                if (Kernel.GetDistance(attacker.X, attacker.Y, attackedsob.X, attackedsob.Y) <= spell.Distance)
                                                {
                                                    PrepareSpell(spell, attacker.Owner);

                                                    uint damage = spell.Power;
                                                    damage = Math.Min(damage, attackedsob.MaxHitpoints - attackedsob.Hitpoints);
                                                    attackedsob.Hitpoints += damage;
                                                    Experience += damage;
                                                    suse.AddTarget(attackedsob.UID, damage, attack);

                                                    attacker.Owner.SendScreen(suse, true);
                                                }
                                            }
                                            else
                                            {
                                                if (spell.Multi)
                                                {
                                                    if (attacker.Owner.Team != null)
                                                    {
                                                        PrepareSpell(spell, attacker.Owner);
                                                        foreach (Client.GameClient teammate in attacker.Owner.Team.Teammates)
                                                        {
                                                            if (Kernel.GetDistance(attacker.X, attacker.Y, teammate.Entity.X, teammate.Entity.Y) <= spell.Distance)
                                                            {
                                                                uint damage = spell.Power;
                                                                damage = Math.Min(damage, teammate.Entity.MaxHitpoints - teammate.Entity.Hitpoints);
                                                                teammate.Entity.Hitpoints += damage;
                                                                Experience += damage;
                                                                suse.AddTarget(teammate.Entity.UID, damage, attack);

                                                                if (spell.NextSpellID != 0)
                                                                {
                                                                    attack.Damage = spell.NextSpellID;
                                                                    attacker.AttackPacket = attack;
                                                                }
                                                                else
                                                                {
                                                                    attacker.AttackPacket = null;
                                                                }
                                                            }
                                                        }
                                                        if (attacked.EntityFlag == EntityFlag.Player)
                                                            attacked.Owner.SendScreen(suse, true);
                                                        else
                                                            attacked.MonsterInfo.SendScreen(suse);
                                                    }
                                                    else
                                                    {
                                                        if (Kernel.GetDistance(attacker.X, attacker.Y, attacked.X, attacked.Y) <= spell.Distance)
                                                        {
                                                            PrepareSpell(spell, attacker.Owner);

                                                            uint damage = spell.Power;
                                                            damage = Math.Min(damage, attacked.MaxHitpoints - attacked.Hitpoints);
                                                            attacked.Hitpoints += damage;
                                                            Experience += damage;
                                                            suse.AddTarget(attacked.UID, damage, attack);

                                                            if (spell.NextSpellID != 0)
                                                            {
                                                                attack.Damage = spell.NextSpellID;
                                                                attacker.AttackPacket = attack;
                                                            }
                                                            else
                                                            {
                                                                attacker.AttackPacket = null;
                                                            }
                                                            if (attacked.EntityFlag == EntityFlag.Player)
                                                                attacked.Owner.SendScreen(suse, true);
                                                            else
                                                                attacked.MonsterInfo.SendScreen(suse);
                                                        }
                                                        else
                                                        {
                                                            attacker.AttackPacket = null;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (Kernel.GetDistance(attacker.X, attacker.Y, attacked.X, attacked.Y) <= spell.Distance)
                                                    {
                                                        PrepareSpell(spell, attacker.Owner);

                                                        uint damage = spell.Power;
                                                        damage = Math.Min(damage, attacked.MaxHitpoints - attacked.Hitpoints);
                                                        attacked.Hitpoints += damage;
                                                        Experience += damage;
                                                        suse.AddTarget(attacked.UID, damage, attack);

                                                        if (spell.NextSpellID != 0)
                                                        {
                                                            attack.Damage = spell.NextSpellID;
                                                            attacker.AttackPacket = attack;
                                                        }
                                                        else
                                                        {
                                                            attacker.AttackPacket = null;
                                                        }
                                                        if (attacked.EntityFlag == EntityFlag.Player)
                                                            attacked.Owner.SendScreen(suse, true);
                                                        else
                                                            attacked.MonsterInfo.SendScreen(suse);
                                                    }
                                                    else
                                                    {
                                                        attacker.AttackPacket = null;
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            attacker.AttackPacket = null;
                                        }
                                        break;
                                    }
                                #endregion
                                #region Revive
                                case 1050:
                                case 1100:
                                    {
                                        if (attackedsob != null)
                                            return;
                                        if (Kernel.GetDistance(attacker.X, attacker.Y, attacked.X, attacked.Y) <= spell.Distance)
                                        {
                                            if (CanUseSpell(spell, attacker.Owner))
                                            {
                                                PrepareSpell(spell, attacker.Owner);

                                                SpellUse suse = new SpellUse(true);
                                                suse.Attacker = attacker.UID;
                                                suse.SpellID = spell.ID;
                                                suse.SpellLevel = spell.Level;
                                                suse.X = X;
                                                suse.Y = Y;

                                                suse.AddTarget(attacked.UID, 0, attack);

                                                attacked.Owner.Entity.Action = Conquer_Online_Server.Game.Enums.ConquerAction.None;
                                                attacked.Owner.ReviveStamp = Time32.Now;
                                                attacked.Owner.Attackable = false;

                                                attacked.Owner.Entity.TransformationID = 0;
                                                attacked.Owner.Entity.RemoveFlag(Update.Flags.Dead);
                                                attacked.Owner.Entity.RemoveFlag(Update.Flags.Ghost);
                                                attacked.Owner.Entity.Hitpoints = attacked.Owner.Entity.MaxHitpoints;

                                                attacked.Ressurect();

                                                attacked.Owner.SendScreen(suse, true);
                                            }
                                        }
                                        break;
                                    }
                                #endregion
                                #region Linear spells
                                case 1260:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            PrepareSpell(spell, attacker.Owner);
                                            Game.Attacking.InLineAlgorithm ila = new Conquer_Online_Server.Game.Attacking.InLineAlgorithm(attacker.X,
                                        X, attacker.Y, Y, (byte)spell.Range, InLineAlgorithm.Algorithm.DDA);
                                            SpellUse suse = new SpellUse(true);
                                            suse.Attacker = attacker.UID;
                                            suse.SpellID = SpellID;
                                            suse.SpellLevel = attacker.Owner.Spells[SpellID].Level;
                                            suse.X = X;
                                            suse.Y = Y;
                                            foreach (Interfaces.IMapObject _obj in attacker.Owner.Screen.Objects)
                                            {
                                                if (_obj == null)
                                                    continue;
                                                if (_obj.MapObjType == MapObjectType.Monster || _obj.MapObjType == MapObjectType.Player)
                                                {
                                                    attacked = _obj as Entity;
                                                    if (ila.InLine(attacked.X, attacked.Y))
                                                    {
                                                        if (!CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                                            continue;

                                                        attack.Effect1 = Attack.AttackEffects1.None;
                                                        uint damage = Game.Attacking.Calculate.Melee(attacker, attacked, spell, ref attack);
                                                        suse.Effect1 = attack.Effect1;
                                                        double dmg = (double)damage * 0.7;
                                                        damage = (uint)dmg;
                                                        attack.Damage = damage;

                                                        ReceiveAttack(attacker, attacked, attack, damage, spell);

                                                        suse.AddTarget(attacked.UID, damage, attack);
                                                    }
                                                }
                                                else if (_obj.MapObjType == MapObjectType.SobNpc)
                                                {
                                                    attackedsob = _obj as SobNpcSpawn;

                                                    if (ila.InLine(attackedsob.X, attackedsob.Y))
                                                    {
                                                        if (!CanAttack(attacker, attackedsob, spell))
                                                            continue;

                                                        attack.Effect1 = Attack.AttackEffects1.None;
                                                        uint damage = Game.Attacking.Calculate.Melee(attacker, attackedsob, ref attack);
                                                        damage = (uint)(damage * spell.PowerPercent);
                                                        attack.Damage = damage;

                                                        ReceiveAttack(attacker, attackedsob, attack, damage, spell);

                                                        suse.AddTarget(attackedsob.UID, damage, attack);
                                                    }
                                                }
                                            }
                                            attacker.Owner.SendScreen(suse, true);
                                        }
                                        break;
                                    }
                                case 1045:
                                case 1046:
                                case 11000:
                                case 11005:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            PrepareSpell(spell, attacker.Owner);
                                            Game.Attacking.InLineAlgorithm ila = new Conquer_Online_Server.Game.Attacking.InLineAlgorithm(attacker.X,
                                        X, attacker.Y, Y, (byte)spell.Range, InLineAlgorithm.Algorithm.DDA);
                                            SpellUse suse = new SpellUse(true);
                                            suse.Attacker = attacker.UID;
                                            suse.SpellID = SpellID;
                                            suse.SpellLevel = attacker.Owner.Spells[SpellID].Level;
                                            //suse.SpellLevelHu = attacker.Owner.Spells[spell.ID].LevelHu;
                                            suse.X = X;
                                            suse.Y = Y;
                                            foreach (Interfaces.IMapObject _obj in attacker.Owner.Screen.Objects)
                                            {
                                                if (_obj == null)
                                                    continue;
                                                if (_obj.MapObjType == MapObjectType.Monster || _obj.MapObjType == MapObjectType.Player)
                                                {
                                                    attacked = _obj as Entity;
                                                    if (ila.InLine(attacked.X, attacked.Y))
                                                    {
                                                        if (!CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                                            continue;

                                                        attack.Effect1 = Attack.AttackEffects1.None;
                                                        uint damage = Game.Attacking.Calculate.Melee(attacker, attacked, spell, ref attack) * 3;
                                                        suse.Effect1 = attack.Effect1;

                                                        attack.Damage = damage;

                                                        ReceiveAttack(attacker, attacked, attack, damage, spell);

                                                        suse.AddTarget(attacked.UID, damage, attack);
                                                    }
                                                }
                                                else if (_obj.MapObjType == MapObjectType.SobNpc)
                                                {
                                                    attackedsob = _obj as SobNpcSpawn;

                                                    if (ila.InLine(attackedsob.X, attackedsob.Y))
                                                    {
                                                        if (!CanAttack(attacker, attackedsob, spell))
                                                            continue;

                                                        attack.Effect1 = Attack.AttackEffects1.None;
                                                        uint damage = Game.Attacking.Calculate.Melee(attacker, attackedsob, ref attack);
                                                        damage = (uint)(damage * spell.PowerPercent);
                                                        attack.Damage = damage;

                                                        ReceiveAttack(attacker, attackedsob, attack, damage, spell);

                                                        suse.AddTarget(attackedsob.UID, damage, attack);
                                                    }
                                                }
                                            }
                                            attacker.Owner.SendScreen(suse, true);
                                        }
                                        break;
                                    }
                                #endregion
                                #region XPSpells inofensive
                                case 1015:
                                case 1020:
                                case 1025:
                                case 1110:
                                case 6011:
                                case 10390:
                                    {
                                        //case 11060: {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            PrepareSpell(spell, attacker.Owner);

                                            SpellUse suse = new SpellUse(true);
                                            suse.Attacker = attacker.UID;
                                            suse.SpellID = spell.ID;
                                            suse.SpellLevel = spell.Level;
                                            suse.X = X;
                                            suse.Y = Y;

                                            suse.AddTarget(attacked.UID, 0, attack);

                                            if (spell.ID == 6011)
                                            {
                                                attacked.FatalStrikeStamp = Time32.Now;
                                                attacked.FatalStrikeTime = 60;
                                                attacked.AddFlag(Update.Flags.FatalStrike);
                                                attacker.RemoveFlag(Update.Flags.Ride);
                                            }
                                            else
                                            {
                                                if (spell.ID == 1110 || spell.ID == 1025 || spell.ID == 10390)
                                                {
                                                    if (!attacked.OnKOSpell())
                                                        attacked.KOCount = 0;

                                                    attacked.KOSpell = spell.ID;
                                                    if (spell.ID == 1110)
                                                    {
                                                        attacked.CycloneStamp = Time32.Now;
                                                        attacked.CycloneTime = 20;
                                                        attacked.AddFlag(Update.Flags.Cyclone);
                                                    }
                                                    else if (spell.ID == 10390)
                                                    {
                                                        attacked.OblivionStamp = Time32.Now;
                                                        attacked.OblivionTime = 20;
                                                        attacked.AddFlag2(Update.Flags2.Oblivion);
                                                    }
                                                    else
                                                    {
                                                        attacked.SupermanStamp = Time32.Now;
                                                        attacked.SupermanTime = 20;
                                                        attacked.AddFlag(Update.Flags.Superman);
                                                    }
                                                }
                                                else
                                                {
                                                    if (spell.ID == 1020)
                                                    {
                                                        attacked.ShieldTime = 0;
                                                        attacked.ShieldStamp = Time32.Now;
                                                        attacked.MagicShieldStamp = Time32.Now;
                                                        attacked.MagicShieldTime = 0;

                                                        attacked.AddFlag(Update.Flags.MagicShield);
                                                        attacked.ShieldStamp = Time32.Now;
                                                        attacked.ShieldIncrease = spell.PowerPercent;
                                                        attacked.ShieldTime = 60;
                                                    }
                                                    else
                                                    {
                                                        attacked.AccuracyStamp = Time32.Now;
                                                        attacked.StarOfAccuracyStamp = Time32.Now;
                                                        attacked.StarOfAccuracyTime = 0;
                                                        attacked.AccuracyTime = 0;

                                                        attacked.AddFlag(Update.Flags.StarOfAccuracy);
                                                        attacked.AccuracyStamp = Time32.Now;
                                                        attacked.AccuracyTime = (byte)spell.Duration;
                                                    }
                                                }
                                            }
                                            attacked.Owner.SendScreen(suse, true);
                                        }
                                        break;
                                    }
                                #endregion
                                #region FireOfHell
                                case 1165:
                                case 9999://Boison Awen For Monk
                                case 9998://Fera Boil For Monk
                                case 9997://Dark Shadow FOr Monk
                                case 9996://Swift Blze
                                case 9995://Fire Peneration
                                case 9994://Dire Scorch
                                case 9993://Fire Impolation
                                case 9992://Flying Flme
                                case 9991://SandBite
                                case 9990:
                                case 9988://Fear Ror
                                case 9986://PurbleShadow
                                case 9985://BlueShadow
                                case 9984://FrealAttack
                                case 9983://FireShower
                                case 9982://WhiteShottel
                                case 9981://ScarletFog
                                case 9980://WhiteLoucst
                                case 9979://BlueLocust
                                case 9978://RedLoucst
                                case 9977://InferoStting
                                case 9976://DeatCoup
                                case 9975://HeavenShotter
                                case 9974://DestinyThunnder
                                case 9973://NineFlams
                                case 9971://DestinyShater
                                case 9972://DestinyFire
                                case 9970://DespotKill
                                case 9987://Fantastic Bite
                                case 9969:
                                case 9989://Icy Curse
                                case 10003://Land Sweep For Ninja

                                case 30013://IcyBolt For Ninja 
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            PrepareSpell(spell, attacker.Owner);

                                            SpellUse suse = new SpellUse(true);
                                            suse.Attacker = attacker.UID;
                                            suse.SpellID = spell.ID;
                                            suse.SpellLevel = spell.Level;
                                            //suse.SpellLevelHu = attacker.Owner.Spells[spell.ID].LevelHu;
                                            suse.X = X;
                                            suse.Y = Y;
                                            Sector sector = new Sector(attacker.X, attacker.Y, X, Y);
                                            sector.Arrange(spell.Sector, spell.Distance);
                                            foreach (Interfaces.IMapObject _obj in attacker.Owner.Screen.Objects)
                                            {
                                                if (_obj == null)
                                                    continue;
                                                if (_obj.MapObjType == MapObjectType.Monster || _obj.MapObjType == MapObjectType.Player)
                                                {
                                                    attacked = _obj as Entity;

                                                    if (sector.Inside(attacked.X, attacked.Y))
                                                    {
                                                        if (CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                                        {
                                                            attack.Effect1 = Attack.AttackEffects1.None;
                                                            uint damage = Game.Attacking.Calculate.Magic(attacker, attacked, spell, ref attack);
                                                            suse.Effect1 = attack.Effect1;

                                                            ReceiveAttack(attacker, attacked, attack, damage, spell);

                                                            suse.AddTarget(attacked.UID, damage, attack);
                                                        }
                                                    }
                                                }
                                                else if (_obj.MapObjType == MapObjectType.SobNpc)
                                                {
                                                    attackedsob = _obj as SobNpcSpawn;

                                                    if (sector.Inside(attackedsob.X, attackedsob.Y))
                                                    {
                                                        if (CanAttack(attacker, attackedsob, spell))
                                                        {
                                                            attack.Effect1 = Attack.AttackEffects1.None;
                                                            uint damage = Game.Attacking.Calculate.Magic(attacker, attackedsob, spell, ref attack);
                                                            suse.Effect1 = attack.Effect1;
                                                            ReceiveAttack(attacker, attackedsob, attack, damage, spell);

                                                            suse.AddTarget(attackedsob.UID, damage, attack);
                                                        }
                                                    }
                                                }
                                            }
                                            attacker.Owner.SendScreen(suse, true);
                                        }
                                        break;
                                    }
                                #endregion
                                #region Circle spells
                                case 1010:
                                case 1115:
                                case 1120:
                                case 1125:
                                case 3090:
                                case 5001:
                                case 8030:
                                case 10315:
                              //  case 7014: //Dragon Breath For Fire Toist 
                               // case 10001://Anger  Chop For Water Toist
                               // case 7013://Flame Shower For Fire toist
                               // case 10002://PnicRoar Fow Warior

                               // case 10004://Unholymplosion For Worior
                               // case 10005://AcidicBreath For Ninja
                               // case 30010://IcyShot For Water
                               // case 30011://IcyThorn For Water Toist
                               // case 30012://Chill For Water Toist 

                              //  case 30014:
                              //  case 30015://Icy Bolt For Water
                              //  case 7017:// Mistry Flame For Fire Toist
                                //case 7015://FearRoar For Fire Toist
                               // case 7011://TailSweep For Worior

                               // case 7012://Destroyer
                               // case 7016://Dragon Blaze For No Effect




                               // case 10512://New Swords For Worior
                               // case 10504://SwordsBeam For Worior
                                //case 10505://VindictiveStrike For Worior 
                               // case 10508://Fatal Crash For Fire
                               // case 10509://Icy Touch For Ninja

                               // case 10506://Blade Vortex For Ninja

                               // case 10363://Split For Ninja
                               // case 10360://Earth Tremble For Ninja
                                    {
                                        if (spell.ID == 10315)
                                        {
                                            if (attacker.Owner.Weapons.Item1 == null) return;
                                            if (attacker.Owner.Weapons.Item1.IsTwoHander()) return;
                                        }
                                        if (spell.ID == 1115)
                                        {
                                            if (!attacker.Hercule())
                                                return;
                                        }
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            PrepareSpell(spell, attacker.Owner);

                                            SpellUse suse = new SpellUse(true);
                                            suse.Attacker = attacker.UID;
                                            suse.SpellID = spell.ID;
                                            //suse.SpellLevelHu = attacker.Owner.Spells[spell.ID].LevelHu;
                                            suse.SpellLevel = spell.Level;
                                            suse.X = X;
                                            suse.Y = Y;
                                            UInt16 ox, oy;
                                            ox = attacker.X;
                                            oy = attacker.Y;
                                            if (spellID == 10315)
                                            {
                                                Attack npacket = new Attack(true);
                                                npacket.Attacker = attacker.UID;
                                                npacket.AttackType = 53;
                                                npacket.X = X;
                                                npacket.Y = Y;
                                                Writer.WriteUInt16(spell.ID, 24 + 4, npacket.ToArray());
                                                Writer.WriteByte(spell.Level, 26 + 4, npacket.ToArray());
                                                attacker.Owner.SendScreen(npacket, true);
                                                attacker.X = X;
                                                attacker.Y = Y;
                                                attacker.SendSpawn(attacker.Owner);
                                                attacker.Owner.Screen.Reload(npacket);
                                            }

                                            List<IMapObject> objects = new List<IMapObject>();
                                            if (attacker.Owner.Screen.Objects.Count() > 0)
                                                objects = GetObjects(ox, oy, attacker.Owner);
                                            if (objects != null)
                                            {
                                                if (Kernel.GetDistance(attacker.X, attacker.Y, X, Y) <= spell.Range)
                                                {
                                                    if (spellID == 10315)
                                                    {
                                                        foreach (IMapObject objs in objects.ToArray())
                                                        {
                                                            if (objs == null)
                                                                continue;

                                                            if (objs.MapObjType == MapObjectType.Monster || objs.MapObjType == MapObjectType.Player)
                                                            {
                                                                attacked = objs as Entity;
                                                                if (Kernel.GetDistance(attacker.X, attacker.Y, attacked.X, attacked.Y) <= spell.Range)
                                                                {
                                                                    if (CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                                                    {
                                                                        attack.Effect1 = Attack.AttackEffects1.None;
                                                                        uint damage = Game.Attacking.Calculate.Melee(attacker, attacked, ref attack) / 3;
                                                                        damage = damage - (uint)(damage * .30/3);
                                                                        suse.Effect1 = attack.Effect1;
                                                                        if (spell.Power > 0)
                                                                        {
                                                                            attack.Effect1 = Attack.AttackEffects1.None;
                                                                            damage = Game.Attacking.Calculate.Magic(attacker, attacked, spell, ref attack) / 3;
                                                                            suse.Effect1 = attack.Effect1;
                                                                        }
                                                                        if (spell.ID == 8030)
                                                                        {
                                                                            attack.Effect1 = Attack.AttackEffects1.None;
                                                                            damage = Game.Attacking.Calculate.Ranged(attacker, attacked, ref attack) / 3;
                                                                        }

                                                                        ReceiveAttack(attacker, attacked, attack, damage, spell);

                                                                        suse.AddTarget(attacked.UID, damage, attack);
                                                                    }
                                                                }
                                                            }
                                                            else if (objs.MapObjType == MapObjectType.SobNpc)
                                                            {
                                                                attackedsob = objs as SobNpcSpawn;
                                                                if (Kernel.GetDistance(attacker.X, attacker.Y, attackedsob.X, attackedsob.Y) <= spell.Range)
                                                                {
                                                                    if (CanAttack(attacker, attackedsob, spell))
                                                                    {
                                                                        attack.Effect1 = Attack.AttackEffects1.None;
                                                                        uint damage = Game.Attacking.Calculate.Melee(attacker, attackedsob, ref attack);
                                                                        if (spell.Power > 0)
                                                                        {
                                                                            damage = Game.Attacking.Calculate.Magic(attacker, attackedsob, spell, ref attack);
                                                                        }
                                                                        attack.Effect1 = Attack.AttackEffects1.None;
                                                                        if (spell.ID == 8030)
                                                                            damage = Game.Attacking.Calculate.Ranged(attacker, attackedsob, ref attack);
                                                                        suse.Effect1 = attack.Effect1;
                                                                        ReceiveAttack(attacker, attackedsob, attack, damage, spell);

                                                                        suse.AddTarget(attackedsob.UID, damage, attack);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        foreach (Interfaces.IMapObject _obj in attacker.Owner.Screen.Objects)
                                                        {
                                                            if (_obj == null)
                                                                continue;
                                                            if (_obj.MapObjType == MapObjectType.Monster || _obj.MapObjType == MapObjectType.Player)
                                                            {
                                                                attacked = _obj as Entity;
                                                                if (Kernel.GetDistance(attacker.X, attacker.Y, attacked.X, attacked.Y) <= spell.Range)
                                                                {
                                                                    if (CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                                                    {
                                                                        attack.Effect1 = Attack.AttackEffects1.None;
                                                                        uint damage = Game.Attacking.Calculate.Melee(attacker, attacked, ref attack) / 2;
                                                                        damage = damage - (uint)(damage * .30/2);
                                                                        suse.Effect1 = attack.Effect1;
                                                                        if (spell.Power > 0)
                                                                        {
                                                                            attack.Effect1 = Attack.AttackEffects1.None;
                                                                            damage = Game.Attacking.Calculate.Magic(attacker, attacked, spell, ref attack) / 2;
                                                                            suse.Effect1 = attack.Effect1;
                                                                        }
                                                                        attack.Effect1 = Attack.AttackEffects1.None;
                                                                        if (spell.ID == 8030)
                                                                            damage = Game.Attacking.Calculate.Ranged(attacker, attacked, ref attack) / 2;
                                                                        if (spell.ID == 1115)
                                                                            damage = Game.Attacking.Calculate.Melee(attacker, attacked, spell, ref attack) / 2;

                                                                        damage = damage * 2;
                                                                        ReceiveAttack(attacker, attacked, attack, damage, spell);

                                                                        suse.AddTarget(attacked.UID, damage, attack);
                                                                    }
                                                                }
                                                            }
                                                            else if (_obj.MapObjType == MapObjectType.SobNpc)
                                                            {
                                                                attackedsob = _obj as SobNpcSpawn;
                                                                if (Kernel.GetDistance(attacker.X, attacker.Y, attackedsob.X, attackedsob.Y) <= spell.Range)
                                                                {
                                                                    if (CanAttack(attacker, attackedsob, spell))
                                                                    {
                                                                        attack.Effect1 = Attack.AttackEffects1.None;
                                                                        uint damage = Game.Attacking.Calculate.Melee(attacker, attackedsob, ref attack);
                                                                        if (spell.Power > 0)
                                                                            damage = Game.Attacking.Calculate.Magic(attacker, attackedsob, spell, ref attack);
                                                                        attack.Effect1 = Attack.AttackEffects1.None;
                                                                        if (spell.ID == 8030)
                                                                            damage = Game.Attacking.Calculate.Ranged(attacker, attackedsob, ref attack);

                                                                        suse.Effect1 = attack.Effect1;
                                                                        ReceiveAttack(attacker, attackedsob, attack, damage, spell);

                                                                        suse.AddTarget(attackedsob.UID, damage, attack);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    attacker.AttackPacket = null;
                                                }
                                            }
                                            attacker.Owner.SendScreen(suse, true);
                                            Conquer_Online_Server.Game.Calculations.IsBreaking(attacker.Owner, ox, oy);
                                        }
                                        break;
                                    }
                                #endregion
                                #region Buffers
                                case 1075:
                                case 1085:
                                case 1090:
                                case 1095:
                                case 3080:
                                case 10405://this is not what I edited yesterday...
                                case 30000:
                                    {
                                        if (attackedsob != null)
                                        {
                                            if (CanUseSpell(spell, attacker.Owner))
                                            {
                                                PrepareSpell(spell, attacker.Owner);

                                                SpellUse suse = new SpellUse(true);
                                                suse.Attacker = attacker.UID;
                                                suse.SpellID = spell.ID;
                                                suse.SpellLevel = spell.Level;
                                                suse.X = X;
                                                suse.Y = Y;

                                                suse.AddTarget(attackedsob.UID, 0, null);

                                                attacker.Owner.SendScreen(suse, true);
                                            }
                                        }
                                        else
                                        {
                                            if (Kernel.GetDistance(attacker.X, attacker.Y, attacked.X, attacked.Y) <= spell.Distance)
                                            {
                                                if (CanUseSpell(spell, attacker.Owner))
                                                {
                                                    PrepareSpell(spell, attacker.Owner);

                                                    SpellUse suse = new SpellUse(true);
                                                    suse.Attacker = attacker.UID;
                                                    suse.SpellID = spell.ID;
                                                    suse.SpellLevel = spell.Level;
                                                    suse.X = X;
                                                    suse.Y = Y;

                                                    suse.AddTarget(attacked.UID, 0, null);

                                                    if (spell.ID == 1075 || spell.ID == 1085)
                                                    {
                                                        if (spell.ID == 1075)
                                                        {
                                                            attacked.AddFlag(Update.Flags.Invisibility);
                                                            attacked.InvisibilityStamp = Time32.Now;
                                                            attacked.InvisibilityTime = (byte)spell.Duration;
                                                            if (attacked.EntityFlag == EntityFlag.Player)
                                                                attacked.Owner.Send(Constants.Invisibility(spell.Duration));
                                                        }
                                                        else
                                                        {
                                                            attacked.AccuracyStamp = Time32.Now;
                                                            attacked.StarOfAccuracyStamp = Time32.Now;
                                                            attacked.StarOfAccuracyTime = 0;
                                                            attacked.AccuracyTime = 0;

                                                            attacked.AddFlag(Update.Flags.StarOfAccuracy);
                                                            attacked.StarOfAccuracyStamp = Time32.Now;
                                                            attacked.StarOfAccuracyTime = (byte)spell.Duration;
                                                            if (attacked.EntityFlag == EntityFlag.Player)
                                                                attacked.Owner.Send(Constants.Accuracy(spell.Duration));
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (spell.ID == 1090)
                                                        {
                                                            attacked.ShieldTime = 0;
                                                            attacked.ShieldStamp = Time32.Now;
                                                            attacked.MagicShieldStamp = Time32.Now;
                                                            attacked.MagicShieldTime = 0;

                                                            attacked.AddFlag(Update.Flags.MagicShield);
                                                            attacked.MagicShieldStamp = Time32.Now;
                                                            attacked.MagicShieldIncrease = spell.PowerPercent;
                                                            attacked.MagicShieldTime = (byte)spell.Duration;
                                                            if (attacked.EntityFlag == EntityFlag.Player)
                                                                attacked.Owner.Send(Constants.Shield(spell.PowerPercent, spell.Duration));
                                                        }
                                                        else if (spell.ID == 1095)
                                                        {
                                                            attacked.AddFlag(Update.Flags.Stigma);
                                                            attacked.StigmaStamp = Time32.Now;
                                                            attacked.StigmaIncrease = spell.PowerPercent;
                                                            attacked.StigmaTime = (byte)spell.Duration;
                                                            if (attacked.EntityFlag == EntityFlag.Player)
                                                                attacked.Owner.Send(Constants.Stigma(spell.PowerPercent, spell.Duration));
                                                        }
                                                        /* else if (spell.ID == 30000)
                                                         {
                                                             //Samak 
                                                             if (attacked.ContainsFlag2(Update.Flags2.AzureShield))
                                                             {
                                                                 return;
                                                             }
                                                             //attack.AttackType = Attack.kimo2;
                                                             attacked.ShieldTime = 0;
                                                             attacked.ShieldStamp = Time32.Now;
                                                             attacked.MagicShieldStamp = Time32.Now;
                                                             attacked.MagicShieldTime = 0;
                                                             //Console.WriteLine("The Dodge is :" + attacked.Dodge.ToString());
                                                             attacked.AddFlag2(Update.Flags2.AzureShield);
                                                             attacked.MagicShieldStamp = Time32.Now;
                                                             attacked.AzureDamage = 12000;
                                                             //Console.WriteLine("AzureShiled granted " + 12000 + "  The Dodge is :" + attacked.Dodge.ToString());
                                                             attacked.MagicShieldIncrease = spell.PowerPercent;
                                                             switch (spell.Level)
                                                             {
                                                                 case 0: attacked.MagicShieldTime = 30; break;
                                                                 case 1: attacked.MagicShieldTime = 35; break;
                                                                 case 2: attacked.MagicShieldTime = 40; break;
                                                                 case 3: attacked.MagicShieldTime = 50; break;
                                                                 case 4: attacked.MagicShieldTime = 60; break;
                                                             }
                                                             if (attacked.EntityFlag == EntityFlag.Player)
                                                                 attacked.Owner.Send(Constants.Shield(12000, attacked.MagicShieldTime));
                                                         }*/
                                                        else if (spell.ID == 30000)
                                                        {

                                                            if (attacked.ContainsFlag2(Update.Flags2.AzureShield))
                                                            {
                                                                return;
                                                            }
                                                            //attack.AttackType = Attack.HassaN2;
                                                            attacked.ShieldTime = 0;
                                                            attacked.ShieldStamp = Time32.Now;
                                                            attacked.MagicShieldStamp = Time32.Now;
                                                            attacked.MagicShieldTime = 0;
                                                            //Console.WriteLine("The Dodge is :" + attacked.Dodge.ToString());
                                                            attacked.AddFlag2(Update.Flags2.AzureShield);
                                                            attacked.MagicShieldStamp = Time32.Now;
                                                            attacked.AzureDamage = 12000;
                                                            //Console.WriteLine("AzureShiled granted " + 12000 + "  The Dodge is :" + attacked.Dodge.ToString());
                                                            attacked.MagicShieldIncrease = spell.PowerPercent;
                                                            attacked.MagicShieldTime = 60;
                                                            if (attacked.EntityFlag == EntityFlag.Player)
                                                            {
                                                                attacked.Owner.Send(Constants.AzureShield(12000, 30));

                                                                Conquer_Online_Server.Network.GamePackets.SyncPacket packet4 = new Conquer_Online_Server.Network.GamePackets.SyncPacket
                                                                {
                                                                    Identifier = attacked.UID,
                                                                    Count = 2,
                                                                    Type = Conquer_Online_Server.Network.GamePackets.SyncPacket.SyncType.StatusFlag,
                                                                    StatusFlag1 = (ulong)attacked.StatusFlag,
                                                                    StatusFlag2 = (ulong)attacked.StatusFlag2,
                                                                    StatusFlag3 = (uint)attacked.StatusFlag3,
                                                                    Unknown1 = 0x31,
                                                                    StatusFlagOffset = 0x5d,
                                                                    Time = (uint)60,
                                                                    Value = (uint)attacked.AzureDamage,
                                                                    Level = spell.Level
                                                                };
                                                                attacked.Owner.Send((byte[])packet4);
                                                            }
                                                        }
                                                        if (spell.ID == 10405 && attacked.Dead)
                                                        {
                                                            if ((attacked.BattlePower - attacker.BattlePower) >= 10)
                                                                return;
                                                            attacked.AddFlag(Update.Flags.Dead);//Flag them as dead... should not be needed. This is no movement
                                                            attacked.ShackleStamp = Time32.Now;//Set stamp so source can remove the flag after X seconds
                                                            attacked.ShackleTime = (short)(30 + 15 * spell.Level);//double checking here. Could be db has this wrong.
                                                            if (attacked.EntityFlag == EntityFlag.Player)
                                                            {
                                                                attacked.Owner.Send(Constants.Shackled(attacked.ShackleTime));

                                                                attacked.AddFlag2(Update.Flags2.SoulShackle);//Give them shackeld effect   

                                                                SyncPacket packet3 = new SyncPacket
                                                                {
                                                                    Identifier = attacked.UID,
                                                                    Count = 2,
                                                                    Type = Network.GamePackets.SyncPacket.SyncType.StatusFlag,
                                                                    StatusFlag1 = (ulong)attacked.StatusFlag,
                                                                    StatusFlag2 = (ulong)attacked.StatusFlag2,
                                                                    Unknown1 = 0x36,
                                                                    StatusFlagOffset = 0x6f,
                                                                    Time = (uint)spell.Duration,
                                                                    Value = 10,
                                                                    Level = spell.Level
                                                                };
                                                                attacked.Owner.Send((byte[])packet3);

                                                            }
                                                        }
                                                    }
                                                    if (attacked.EntityFlag == EntityFlag.Player)
                                                        attacked.Owner.SendScreen(suse, true);
                                                    else
                                                        attacked.MonsterInfo.SendScreen(suse);

                                                }
                                            }
                                        }
                                        break;
                                    }
                                #endregion
                                #region Percent
                                case 3050:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            if (attackedsob != null)
                                            {
                                                if (Kernel.GetDistance(attacker.X, attacker.Y, attackedsob.X, attackedsob.Y) <= spell.Distance)
                                                {
                                                    SpellUse suse = new SpellUse(true);
                                                    suse.Attacker = attacker.UID;
                                                    suse.SpellID = spell.ID;
                                                    suse.SpellLevel = spell.Level;
                                                    suse.X = X;
                                                    suse.Y = Y;

                                                    if (CanAttack(attacker, attackedsob, spell))
                                                    {
                                                        PrepareSpell(spell, attacker.Owner);
                                                        uint damage = Game.Attacking.Calculate.Percent(attackedsob, spell.PowerPercent);

                                                        attackedsob.Hitpoints -= damage;

                                                        suse.AddTarget(attackedsob.UID, damage, attack);

                                                        attacker.Owner.SendScreen(suse, true);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (Kernel.GetDistance(attacker.X, attacker.Y, attacked.X, attacked.Y) <= spell.Distance)
                                                {
                                                    SpellUse suse = new SpellUse(true);
                                                    suse.Attacker = attacker.UID;
                                                    suse.SpellID = spell.ID;
                                                    suse.SpellLevel = spell.Level;
                                                    suse.X = X;
                                                    suse.Y = Y;

                                                    if (CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                                    {
                                                        PrepareSpell(spell, attacker.Owner);
                                                        uint damage = Game.Attacking.Calculate.Percent(attacked, spell.PowerPercent);

                                                        attacker.Owner.UpdateQualifier(attacker.Owner, attacked.Owner, damage);

                                                        attacked.Hitpoints -= damage;

                                                        suse.AddTarget(attacked.UID, damage, attack);

                                                        if (attacked.EntityFlag == EntityFlag.Player)
                                                            attacked.Owner.SendScreen(suse, true);
                                                        else
                                                            attacked.MonsterInfo.SendScreen(suse);
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    }
                                #endregion
                                #region ExtraXP
                                case 1040:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            SpellUse suse = new SpellUse(true);
                                            suse.Attacker = attacker.UID;
                                            suse.SpellID = spell.ID;
                                            suse.SpellLevel = spell.Level;
                                            suse.X = X;
                                            suse.Y = Y;
                                            PrepareSpell(spell, attacker.Owner);
                                            if (attacker.Owner.Team != null)
                                            {
                                                foreach (Client.GameClient teammate in attacker.Owner.Team.Teammates)
                                                {
                                                    if (teammate.Entity.UID != attacker.UID)
                                                    {
                                                        if (Kernel.GetDistance(attacker.X, attacker.Y, teammate.Entity.X, teammate.Entity.Y) <= 18)
                                                        {
                                                            teammate.XPCount += 20;
                                                            suse.AddTarget(teammate.Entity.UID, 20, null);
                                                        }
                                                    }
                                                }
                                            }
                                            attacker.Owner.SendScreen(suse, true);
                                        }
                                        break;
                                    }
                                #endregion
                                #region WeaponSpells
                                #region Circle
                                case 5010:
                                case 7020:
                                    //case 11110:
                                    //case 10490:
                                    {

                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            PrepareSpell(spell, attacker.Owner);

                                            SpellUse suse = new SpellUse(true);
                                            suse.Attacker = attacker.UID;
                                            suse.SpellID = spell.ID;
                                            suse.SpellLevel = spell.Level;
                                            //suse.SpellLevelHu = attacker.Owner.Spells[spell.ID].LevelHu;
                                            if (suse.SpellID != 10415)
                                            {
                                                suse.X = X;
                                                suse.Y = Y;
                                            }
                                            else
                                            {
                                                suse.X = 6;
                                            }

                                            if (Kernel.GetDistance(attacker.X, attacker.Y, X, Y) <= attacker.AttackRange + 1)
                                            {
                                                foreach (Interfaces.IMapObject _obj in attacker.Owner.Screen.Objects)
                                                {
                                                    if (_obj == null)
                                                        continue;
                                                    if (_obj.MapObjType == MapObjectType.Monster || _obj.MapObjType == MapObjectType.Player)
                                                    {
                                                        attacked = _obj as Entity;
                                                        if (Kernel.GetDistance(attacker.X, attacker.Y, attacked.X, attacked.Y) <= spell.Range)
                                                        {
                                                            if (attacked.ContainsFlag(Update.Flags.Fly))
                                                                return;
                                                            if (CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                                            {
                                                                PrepareSpell(spell, attacker.Owner);

                                                                attack.Effect1 = Attack.AttackEffects1.None;
                                                                uint damage = Game.Attacking.Calculate.Melee(attacker, attacked, spell, ref attack);
                                                                suse.Effect1 = attack.Effect1;

                                                                ReceiveAttack(attacker, attacked, attack, damage, spell);

                                                                suse.AddTarget(attacked.UID, damage, attack);
                                                            }
                                                        }
                                                    }
                                                    else if (_obj.MapObjType == MapObjectType.SobNpc)
                                                    {
                                                        attackedsob = _obj as SobNpcSpawn;
                                                        if (Kernel.GetDistance(attacker.X, attacker.Y, attackedsob.X, attackedsob.Y) <= spell.Range)
                                                        {
                                                            if (CanAttack(attacker, attackedsob, spell))
                                                            {
                                                                PrepareSpell(spell, attacker.Owner);
                                                                attack.Effect1 = Attack.AttackEffects1.None;
                                                                uint damage = Game.Attacking.Calculate.Melee(attacker, attackedsob, ref attack);
                                                                damage = (uint)(damage * spell.PowerPercent);
                                                                ReceiveAttack(attacker, attackedsob, attack, damage, spell);

                                                                suse.AddTarget(attackedsob.UID, damage, attack);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                attacker.AttackPacket = null;
                                            }
                                            attacker.Owner.SendScreen(suse, true);
                                        }

                                        break;
                                    }
                                #endregion
                                #region Single target
                                case 11140:
                                case 10490:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            PrepareSpell(spell, attacker.Owner);

                                            TryTrip suse = new TryTrip(true);
                                            suse.Attacker = attacker.UID;
                                            suse.SpellID = spell.ID;
                                            suse.SpellLevel = spell.Level;

                                            if (Kernel.GetDistance(attacker.X, attacker.Y, X, Y) <= attacker.AttackRange + 1)
                                            {
                                                if (attackedsob != null)
                                                {
                                                    if (CanAttack(attacker, attackedsob, spell))
                                                    {
                                                        PrepareSpell(spell, attacker.Owner);
                                                        attack.Effect1 = Attack.AttackEffects1.None;
                                                        uint damage = Game.Attacking.Calculate.Melee(attacker, attackedsob, ref attack);
                                                        //damage = (uint)(damage * spell.PowerPercent);
                                                        ReceiveAttack(attacker, attackedsob, attack, damage, spell);
                                                        suse.Damage = damage;
                                                        suse.Attacked = attackedsob.UID;
                                                    }
                                                }
                                                else
                                                {
                                                    if (CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                                    {
                                                        PrepareSpell(spell, attacker.Owner);

                                                        attack.Effect1 = Attack.AttackEffects1.None;
                                                        uint damage = Game.Attacking.Calculate.Melee(attacker, attacked, spell, ref attack);

                                                        ReceiveAttack(attacker, attacked, attack, damage, spell);
                                                        suse.Damage = damage;
                                                        suse.Attacked = attacked.UID;
                                                    }
                                                }
                                                attacker.AttackPacket = null;
                                            }
                                            else
                                            {
                                                attacker.AttackPacket = null;
                                            }
                                            attacker.Owner.SendScreen(suse, true);
                                        }
                                        attacker.AttackPacket = null;
                                        break;
                                    }
                                case 1290:
                                case 5030:
                                case 5040:
                                case 7000:
                                case 7010:
                                case 7030:
                                case 7040:
                                    //case 10381:

                                    //case 10490:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            PrepareSpell(spell, attacker.Owner);

                                            SpellUse suse = new SpellUse(true);
                                            suse.Attacker = attacker.UID;
                                            suse.SpellID = spell.ID;
                                            suse.SpellLevel = spell.Level;
                                            //suse.SpellLevelHu = attacker.Owner.Spells[spell.ID].LevelHu;
                                            suse.X = X;
                                            suse.Y = Y;

                                            if (Kernel.GetDistance(attacker.X, attacker.Y, X, Y) <= attacker.AttackRange + 1)
                                            {
                                                if (attackedsob != null)
                                                {
                                                    if (CanAttack(attacker, attackedsob, spell))
                                                    {
                                                        PrepareSpell(spell, attacker.Owner);
                                                        attack.Effect1 = Attack.AttackEffects1.None;
                                                        uint damage = Game.Attacking.Calculate.Melee(attacker, attackedsob, ref attack);
                                                        // damage = (uint)(damage * spell.PowerPercent);
                                                        ReceiveAttack(attacker, attackedsob, attack, damage, spell);

                                                        suse.AddTarget(attackedsob.UID, damage, attack);
                                                    }
                                                }
                                                else
                                                {
                                                    if (CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                                    {
                                                        PrepareSpell(spell, attacker.Owner);

                                                        attack.Effect1 = Attack.AttackEffects1.None;
                                                        uint damage = Game.Attacking.Calculate.Melee(attacker, attacked, spell, ref attack);
                                                        suse.Effect1 = attack.Effect1;

                                                        ReceiveAttack(attacker, attacked, attack, damage, spell);

                                                        suse.AddTarget(attacked.UID, damage, attack);
                                                    }
                                                }
                                                attacker.AttackPacket = null;
                                            }
                                            else
                                            {
                                                attacker.AttackPacket = null;
                                            }
                                            attacker.Owner.SendScreen(suse, true);
                                        }
                                        attacker.AttackPacket = null;
                                        break;
                                    }
                                #endregion
                                #region Sector
                                case 1250:
                                case 5050:
                                case 5020:
                                case 1300:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            PrepareSpell(spell, attacker.Owner);

                                            SpellUse suse = new SpellUse(true);
                                            suse.Attacker = attacker.UID;
                                            suse.SpellID = spell.ID;
                                            suse.SpellLevel = spell.Level;
                                            suse.X = X;
                                            suse.Y = Y;
                                            Sector sector = new Sector(attacker.X, attacker.Y, X, Y);
                                            sector.Arrange(spell.Sector, spell.Range);
                                            if (Kernel.GetDistance(attacker.X, attacker.Y, X, Y) <= spell.Distance + 1)
                                            {
                                                foreach (Interfaces.IMapObject _obj in attacker.Owner.Screen.Objects)
                                                {
                                                    if (_obj == null)
                                                        continue;
                                                    if (_obj.MapObjType == MapObjectType.Monster || _obj.MapObjType == MapObjectType.Player)
                                                    {
                                                        attacked = _obj as Entity;

                                                        if (sector.Inside(attacked.X, attacked.Y))
                                                        {
                                                            if (CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                                            {
                                                                attack.Effect1 = Attack.AttackEffects1.None;
                                                                uint damage = Game.Attacking.Calculate.Melee(attacker, attacked, spell, ref attack);
                                                                suse.Effect1 = attack.Effect1;

                                                                ReceiveAttack(attacker, attacked, attack, damage, spell);

                                                                suse.AddTarget(attacked.UID, damage, attack);
                                                            }
                                                        }
                                                    }
                                                    else if (_obj.MapObjType == MapObjectType.SobNpc)
                                                    {
                                                        attackedsob = _obj as SobNpcSpawn;

                                                        if (sector.Inside(attackedsob.X, attackedsob.Y))
                                                        {
                                                            if (CanAttack(attacker, attackedsob, spell))
                                                            {
                                                                attack.Effect1 = Attack.AttackEffects1.None;
                                                                uint damage = Game.Attacking.Calculate.Melee(attacker, attackedsob, ref attack);
                                                                damage = (uint)(damage * spell.PowerPercent);
                                                                ReceiveAttack(attacker, attackedsob, attack, damage, spell);

                                                                suse.AddTarget(attackedsob.UID, damage, attack);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                attacker.AttackPacket = null;
                                            }
                                            attacker.Owner.SendScreen(suse, true);
                                        }
                                        break;
                                    }
                                #endregion
                                #endregion
                                #region Fly
                                case 8002:
                                case 8003:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            if (attacker.MapID == 1950)
                                                return;
                                            PrepareSpell(spell, attacker.Owner);
                                            SpellUse suse = new SpellUse(true);
                                            suse.Attacker = attacker.UID;
                                            suse.SpellID = spell.ID;
                                            suse.SpellLevel = spell.Level;
                                            suse.X = X;
                                            suse.Y = Y;
                                            attacked.FlyStamp = Time32.Now;
                                            attacked.FlyTime = (byte)spell.Duration;

                                            suse.AddTarget(attacker.UID, attacker.FlyTime, null);

                                            attacker.AddFlag(Update.Flags.Fly);
                                            attacker.RemoveFlag(Update.Flags.Ride);
                                            attacker.Owner.SendScreen(suse, true);
                                        }
                                        break;
                                    }
                                #endregion
                                #region New Skills Trojan
                                #region SuperCyclone
                                case 11970:
                                    {
                                        SpellUse suse = new SpellUse(true);
                                        suse.Attacker = attacker.UID;
                                        suse.SpellID = spell.ID;
                                        suse.SpellLevel = spell.Level;
                                        suse.X = X;
                                        suse.Y = Y;
                                        if (!attacker.ContainsFlag3((uint)1UL << 0x16))
                                            attacked.KOCount = 0;
                                        if (attacker.Owner.Entity.ContainsFlag(0x10))
                                        {
                                            attacker.Owner.Entity.RemoveFlag(0x10);
                                            attacker.Owner.Entity.AddFlag3((uint)1UL << 0x16);
                                            attacker.Owner.Entity.SuperCyclone = Time32.Now;
                                        }
                                        attacker.Owner.SendScreen(suse, true);
                                        break;
                                    }
                                #endregion
                                #region MortalStrike
                                case 11990:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            PrepareSpell(spell, attacker.Owner);
                                            SpellUse suse = new SpellUse(true);
                                            suse.Attacker = attacker.UID;
                                            suse.SpellID = spell.ID;
                                            suse.SpellLevel = spell.Level;
                                            suse.X = X;
                                            suse.Y = Y;
                                            if (Kernel.GetDistance(attacker.X, attacker.Y, X, Y) <= attacker.AttackRange + 1)
                                            {
                                                if (attackedsob != null)
                                                {
                                                    if (CanAttack(attacker, attackedsob, spell))
                                                    {
                                                        PrepareSpell(spell, attacker.Owner);
                                                        attack.Effect1 = Attack.AttackEffects1.None;
                                                        uint damage = Game.Attacking.Calculate.Melee(attacker, attackedsob, ref attack);
                                                        ReceiveAttack(attacker, attackedsob, attack, damage, spell);

                                                        suse.AddTarget(attackedsob.UID, damage, attack);
                                                    }
                                                }
                                                else
                                                {
                                                    if (CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                                    {
                                                        PrepareSpell(spell, attacker.Owner);

                                                        attack.Effect1 = Attack.AttackEffects1.None;
                                                        uint damage = Game.Attacking.Calculate.Melee(attacker, attacked, spell, ref attack);
                                                        suse.Effect1 = attack.Effect1;

                                                        ReceiveAttack(attacker, attacked, attack, damage, spell);

                                                        suse.AddTarget(attacked.UID, damage, attack);
                                                    }
                                                }
                                                attacker.AttackPacket = null;
                                            }
                                            else
                                            {
                                                attacker.AttackPacket = null;
                                            }
                                            attacker.Owner.SendScreen(suse, true);
                                        }
                                        attacker.AttackPacket = null;
                                        break;
                                    }
                                #endregion

                                #endregion
                                #region Ninja Spells
                                case 6010://Vortex
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            PrepareSpell(spell, attacker.Owner);

                                            SpellUse suse = new SpellUse(true);
                                            suse.Attacker = attacker.UID;
                                            suse.SpellID = spell.ID;
                                            suse.SpellLevel = spell.Level;
                                            suse.X = X;
                                            suse.Y = Y;

                                            attacker.AddFlag(Update.Flags.ShurikenVortex);
                                            attacker.RemoveFlag(Update.Flags.Ride);
                                            attacker.ShurikenVortexStamp = Time32.Now;
                                            attacker.ShurikenVortexTime = 20;

                                            attacker.Owner.SendScreen(suse, true);

                                            attacker.VortexPacket = new Attack(true);
                                            attacker.VortexPacket.Decoded = true;
                                            attacker.VortexPacket.Damage = 6012;
                                            attacker.VortexPacket.AttackType = Attack.Magic;
                                            attacker.VortexPacket.Attacker = attacker.UID;
                                        }
                                        break;
                                    }
                                case 6012://VortexRespone
                                    {
                                        if (!attacker.ContainsFlag(Update.Flags.ShurikenVortex))
                                        {
                                            attacker.AttackPacket = null;
                                            break;
                                        }
                                        SpellUse suse = new SpellUse(true);
                                        suse.Attacker = attacker.UID;
                                        suse.SpellID = spell.ID;
                                        suse.SpellLevel = spell.Level;
                                        suse.X = attacker.X;
                                        suse.Y = attacker.Y;
                                        foreach (Interfaces.IMapObject _obj in attacker.Owner.Screen.Objects)
                                        {
                                            if (_obj == null)
                                                continue;
                                            if (_obj.MapObjType == MapObjectType.Monster || _obj.MapObjType == MapObjectType.Player)
                                            {
                                                attacked = _obj as Entity;
                                                if (Kernel.GetDistance(attacker.X, attacker.Y, attacked.X, attacked.Y) <= spell.Range)
                                                {
                                                    if (CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                                    {
                                                        uint damage = Game.Attacking.Calculate.Melee(attacker, attacked, ref attack);
                                                        damage = (UInt32)((damage * 50) / 100);
                                                        suse.Effect1 = attack.Effect1;

                                                        ReceiveAttack(attacker, attacked, attack, damage, spell);

                                                        suse.AddTarget(attacked.UID, damage, attack);
                                                    }
                                                }
                                            }
                                            else if (_obj.MapObjType == MapObjectType.SobNpc)
                                            {
                                                attackedsob = _obj as SobNpcSpawn;
                                                if (Kernel.GetDistance(attacker.X, attacker.Y, attackedsob.X, attackedsob.Y) <= spell.Range)
                                                {
                                                    if (CanAttack(attacker, attackedsob, spell))
                                                    {
                                                        attack.Effect1 = Attack.AttackEffects1.None;
                                                        uint damage = Game.Attacking.Calculate.Melee(attacker, attackedsob, ref attack);
                                                        suse.Effect1 = attack.Effect1;
                                                        ReceiveAttack(attacker, attackedsob, attack, damage, spell);

                                                        suse.AddTarget(attackedsob.UID, damage, attack);
                                                    }
                                                }
                                            }
                                        }
                                        attacker.Owner.SendScreen(suse, true);
                                        break;
                                    }
                                case 6001:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            PrepareSpell(spell, attacker.Owner);
                                            SpellUse suse = new SpellUse(true);
                                            suse.Attacker = attacker.UID;
                                            suse.SpellID = spell.ID;
                                            suse.SpellLevel = spell.Level;
                                            suse.X = X;
                                            suse.Y = Y;
                                            if (Kernel.GetDistance(attacker.X, attacker.Y, X, Y) <= spell.Distance)
                                            {
                                                foreach (Interfaces.IMapObject _obj in attacker.Owner.Screen.Objects)
                                                {
                                                    if (_obj.MapObjType == MapObjectType.Player || _obj.MapObjType == MapObjectType.Monster)
                                                    {
                                                        attacked = _obj as Entity;
                                                        if (attacked.MapObjType == MapObjectType.Monster)
                                                            if (attacked.MonsterInfo.Boss)
                                                                continue;
                                                        if (Kernel.GetDistance(X, Y, attacked.X, attacked.Y) <= spell.Range)
                                                        {
                                                            if (CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                                            {
                                                                int potDifference = attacker.BattlePower - attacked.BattlePower;
                                                                int rate = spell.Percent + potDifference - 20;
                                                                if (Kernel.Rate(rate))
                                                                {
                                                                    attacked.ToxicFogStamp = Time32.Now;
                                                                    attacked.ToxicFogLeft = 20;
                                                                    attacked.ToxicFogPercent = spell.PowerPercent * 4;
                                                                    attacked.AddFlag(Update.Flags.Poisoned);
                                                                    suse.AddTarget(attacked.UID, 1, null);
                                                                }
                                                                else
                                                                {
                                                                    suse.AddTarget(attacked.UID, 0, null);
                                                                    suse.Targets[attacked.UID].Hit = false;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                attacker.AttackPacket = null;
                                            }
                                            attacker.Owner.SendScreen(suse, true);
                                        }
                                        break;
                                    }
                                case 6000:
                                    {
                                        if (Time32.Now >= attacker.SpellStamp.AddMilliseconds(500))
                                        {
                                            if (CanUseSpell(spell, attacker.Owner))
                                            {
                                                ushort Xx, Yx;
                                                if (attacked != null)
                                                {
                                                    Xx = attacked.X;
                                                    Yx = attacked.Y;
                                                }
                                                else
                                                {
                                                    Xx = attackedsob.X;
                                                    Yx = attackedsob.Y;
                                                }
                                                if (Kernel.GetDistance(attacker.X, attacker.Y, Xx, Yx) <= spell.Range)
                                                {
                                                    if (attackedsob == null)
                                                        if (attacked.ContainsFlag(Network.GamePackets.Update.Flags.Fly))
                                                            return;
                                                    //if (attacked.ContainsFlag(Network.GamePackets.Update.Flags.Fly))
                                                    //  return;
                                                    if (attacker.ContainsFlag(Network.GamePackets.Update.Flags.Fly))
                                                        return;
                                                    PrepareSpell(spell, attacker.Owner);

                                                    SpellUse suse = new SpellUse(true);
                                                    suse.Attacker = attacker.UID;
                                                    suse.SpellID = spell.ID;
                                                    suse.SpellLevel = spell.Level;
                                                    suse.X = X;
                                                    suse.Y = Y;

                                                    bool send = false;

                                                    if (attackedsob == null)
                                                    {
                                                        if (CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                                        {
                                                            attack.Effect1 = Attack.AttackEffects1.None;
                                                            uint damage = Game.Attacking.Calculate.Melee(attacker, attacked, spell, ref attack) * 2;
                                                            damage = (damage * 20) / 100;
                                                            ReceiveAttack(attacker, attacked, attack, damage, spell);

                                                            suse.AddTarget(attacked.UID, damage, attack);
                                                            send = true;

                                                            if (attacker.Owner.Spells.ContainsKey(11230) && !attacked.Dead)
                                                            {
                                                                var s = attacker.Owner.Spells[11230];
                                                                var spellz = Database.SpellTable.SpellInformations[s.ID][s.Level];
                                                                if (spellz != null)
                                                                {
                                                                    if (Conquer_Online_Server.Kernel.Rate(spellz.Percent))
                                                                    {
                                                                        SpellUse ssuse = new SpellUse(true);
                                                                        ssuse.Attacker = attacker.UID;
                                                                        ssuse.SpellID = spellz.ID;
                                                                        ssuse.SpellLevel = spellz.Level;
                                                                        damage = Game.Attacking.Calculate.Melee(attacker, attacked, ref attack) * 1;
                                                                        ssuse.AddTarget(attacked.UID, new SpellUse.DamageClass().Damage = damage, attack);
                                                                        ReceiveAttack(attacker, attacked, attack, damage, spellz);
                                                                        attacker.Owner.SendScreen(ssuse, true);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (CanAttack(attacker, attackedsob, spell))
                                                        {
                                                            attack.Effect1 = Attack.AttackEffects1.None;
                                                            uint damage = Game.Attacking.Calculate.Melee(attacker, attackedsob, ref attack) * 3;
                                                            damage = (uint)(damage * spell.PowerPercent);
                                                            ReceiveAttack(attacker, attackedsob, attack, damage, spell);
                                                            suse.Effect1 = attack.Effect1;

                                                            suse.AddTarget(attackedsob.UID, damage, attack);
                                                            send = true;
                                                        }
                                                    }
                                                    if (send)
                                                        attacker.Owner.SendScreen(suse, true);
                                                    attacker.SpellStamp = Time32.Now;
                                                }
                                                else
                                                {
                                                    attacker.AttackPacket = null;
                                                }
                                            }
                                        }
                                        break;
                                    }

                                case 10381:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            ushort Xx, Yx;
                                            if (attacked != null)
                                            {
                                                Xx = attacked.X;
                                                Yx = attacked.Y;
                                            }
                                            else
                                            {
                                                Xx = attackedsob.X;
                                                Yx = attackedsob.Y;
                                            }
                                            if (Kernel.GetDistance(attacker.X, attacker.Y, Xx, Yx) <= spell.Range)
                                            {
                                                if (attackedsob == null)
                                                    if (attacked.ContainsFlag(Network.GamePackets.Update.Flags.Fly))
                                                        return;
                                                //if (attacked.ContainsFlag(Network.GamePackets.Update.Flags.Fly))
                                                //  return;
                                                if (attacker.ContainsFlag(Network.GamePackets.Update.Flags.Fly))
                                                    return;
                                                PrepareSpell(spell, attacker.Owner);

                                                SpellUse suse = new SpellUse(true);
                                                suse.Attacker = attacker.UID;
                                                suse.SpellID = spell.ID;
                                                suse.SpellLevel = spell.Level;
                                                suse.X = X;
                                                suse.Y = Y;

                                                bool send = false;

                                                if (attackedsob == null)
                                                {
                                                    if (CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                                    {
                                                        attack.Effect1 = Attack.AttackEffects1.None;
                                                        uint damage = Game.Attacking.Calculate.Melee(attacker, attacked, spell, ref attack);
                                                        damage = (damage * 13) / 10;
                                                        suse.Effect1 = attack.Effect1;

                                                        ReceiveAttack(attacker, attacked, attack, damage, spell);

                                                        suse.Targets.Add(attacked.UID, damage);
                                                        send = true;
                                                    }
                                                }
                                                else
                                                {
                                                    if (CanAttack(attacker, attackedsob, spell))
                                                    {
                                                        attack.Effect1 = Attack.AttackEffects1.None;
                                                        uint damage = Game.Attacking.Calculate.Melee(attacker, attackedsob, ref attack);
                                                        damage = (uint)(damage * spell.PowerPercent);
                                                        ReceiveAttack(attacker, attackedsob, attack, damage, spell);
                                                        suse.Effect1 = attack.Effect1;

                                                        suse.Targets.Add(attackedsob.UID, damage);
                                                        send = true;
                                                    }
                                                }
                                                if (send)
                                                    attacker.Owner.SendScreen(suse, true);
                                            }
                                            else
                                            {
                                                attacker.AttackPacket = null;
                                            }
                                        }
                                        break;
                                    }
                                case 6002:
                                    {
                                        if (attackedsob != null)
                                            return;
                                        if (attacked.EntityFlag == EntityFlag.Monster)
                                            return;
                                        if (Kernel.GetDistance(attacker.X, attacker.Y, attacked.X, attacked.Y) <= spell.Distance)
                                        {
                                            if (CanUseSpell(spell, attacker.Owner))
                                            {
                                                PrepareSpell(spell, attacker.Owner);

                                                int potDifference = attacker.BattlePower - attacked.BattlePower;

                                                int rate = spell.Percent + potDifference;

                                                SpellUse suse = new SpellUse(true);
                                                suse.Attacker = attacker.UID;
                                                suse.SpellID = spell.ID;
                                                suse.SpellLevel = spell.Level;
                                                suse.X = X;
                                                suse.Y = Y;
                                                if (CanAttack(attacker, attacked, spell, false))
                                                {
                                                    suse.AddTarget(attacked.UID, 1, attack);
                                                    if (Kernel.Rate(rate))
                                                    {
                                                        attacked.NoDrugsStamp = Time32.Now;
                                                        attacked.NoDrugsTime = (short)spell.Duration;
                                                        if (attacked.EntityFlag == EntityFlag.Player)
                                                            attacked.Owner.Send(Constants.NoDrugs(spell.Duration));
                                                    }
                                                    else
                                                    {
                                                        suse.Targets[attacked.UID].Hit = false;
                                                    }

                                                    attacked.Owner.SendScreen(suse, true);
                                                }
                                            }
                                        }
                                        break;
                                    }
                                case 6004:
                                    {
                                        if (attackedsob != null)
                                            return;
                                        if (attacked.EntityFlag == EntityFlag.Monster)
                                            return;
                                        if (!attacked.ContainsFlag(Update.Flags.Fly))
                                            return;
                                        if (Kernel.GetDistance(attacker.X, attacker.Y, attacked.X, attacked.Y) <= spell.Distance)
                                        {
                                            if (CanUseSpell(spell, attacker.Owner))
                                            {
                                                PrepareSpell(spell, attacker.Owner);

                                                int potDifference = attacker.BattlePower - attacked.BattlePower;

                                                int rate = spell.Percent + potDifference;

                                                SpellUse suse = new SpellUse(true);
                                                suse.Attacker = attacker.UID;
                                                suse.SpellID = spell.ID;
                                                suse.SpellLevel = spell.Level;
                                                suse.X = X;
                                                suse.Y = Y;
                                                if (CanAttack(attacker, attacked, spell, false))
                                                {
                                                    uint dmg = Calculate.Percent(attacked, 0.1F);
                                                    suse.AddTarget(attacked.UID, dmg, attack);

                                                    if (Kernel.Rate(rate))
                                                    {
                                                        attacked.Hitpoints -= dmg;
                                                        attacked.RemoveFlag(Update.Flags.Fly);
                                                    }
                                                    else
                                                    {
                                                        suse.Targets[attacked.UID].Hit = false;
                                                    }

                                                    attacked.Owner.SendScreen(suse, true);
                                                }
                                            }
                                        }
                                        break;
                                    }
                                case 11180:
                                    {
                                        if (attacked != null)
                                        {
                                            if (attacked.ContainsFlag(Update.Flags.Fly))
                                                return;
                                            if (Kernel.GetDistance(attacker.X, attacker.Y, attacked.X, attacked.Y) <= spell.Distance)
                                            {
                                                if (CanUseSpell(spell, attacker.Owner))
                                                {
                                                    if (!Kernel.Rate(Math.Max(5, 100 - (attacked.BattlePower - attacker.BattlePower) / 5))) return;
                                                    PrepareSpell(spell, attacker.Owner);
                                                    SpellUse suse = new SpellUse(true);
                                                    suse.Attacker = attacker.UID;
                                                    suse.SpellID = spell.ID;
                                                    suse.SpellLevel = spell.Level;
                                                    suse.X = attacked.X;
                                                    suse.Y = attacked.Y;
                                                    ushort newx = attacker.X;
                                                    ushort newy = attacker.Y;
                                                    Map.Pushback(ref newx, ref newy, attacked.Facing, 5);
                                                    if (attacker.Owner.Map.Floor[newx, newy, MapObjectType.Player, attacked])
                                                    {
                                                        suse.X = attacked.X = newx;
                                                        suse.Y = attacked.Y = newy;
                                                    }
                                                    Conquer_Online_Server.Network.GamePackets.SpellUse.DamageClass tar = new SpellUse.DamageClass();
                                                    if (CanAttack(attacker, attacked, spell, false))
                                                    {
                                                        tar.Damage = Calculate.Melee(attacker, attacked, spell, ref attack);
                                                        suse.AddTarget(attacked.UID, tar, attack);
                                                        ReceiveAttack(attacker, attacked, attack, tar.Damage, spell);
                                                    }
                                                    if (attacker.EntityFlag == EntityFlag.Player)
                                                        attacker.Owner.SendScreen(suse, true);
                                                }
                                            }
                                        }
                                        break;
                                    }
                                case 11170:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {

                                            PrepareSpell(spell, attacker.Owner);

                                            SpellUse suse = new SpellUse(true);
                                            suse.Attacker = attacker.UID;
                                            suse.SpellID = spell.ID;
                                            suse.SpellLevel = spell.Level;
                                            suse.X = X;
                                            suse.Y = Y;

                                            Conquer_Online_Server.Network.GamePackets.SpellUse.DamageClass tar = new SpellUse.DamageClass();
                                            foreach (var t in attacker.Owner.Screen.Objects)
                                            {
                                                if (t == null)
                                                    continue;
                                                if (t.MapObjType == MapObjectType.Player || t.MapObjType == MapObjectType.Monster)
                                                {
                                                    var target = t as Entity;
                                                    if (Kernel.GetDistance(X, Y, target.X, target.Y) <= spell.Range)
                                                    {
                                                        if (CanAttack(attacker, target, spell, false))
                                                        {
                                                            tar.Damage = Calculate.Melee(attacker, target, spell, ref attack) * 2;
                                                            tar.Hit = true;
                                                            suse.AddTarget(target.UID, tar, attack);
                                                            ReceiveAttack(attacker, target, attack, tar.Damage, spell);
                                                        }
                                                    }
                                                }
                                            }

                                            if (attacker.EntityFlag == EntityFlag.Player)
                                                attacker.Owner.SendScreen(suse, true);
                                        }
                                        break;
                                    }
                                #endregion

                                #region New Ninja
                                #region Twilight Dance
                                case 12070:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            PrepareSpell(spell, attacker.Owner);
                                            SpellUse suse = new SpellUse(true);
                                            suse.Attacker = attacker.UID;
                                            suse.SpellID = spell.ID;
                                            suse.SpellLevel = spell.Level;
                                            ushort _X = attacker.X, _Y = attacker.Y;
                                            ushort _tX = X, _tY = Y;
                                            byte dist = (byte)spell.Distance;
                                            var Array = attacker.Owner.Screen.Objects;
                                            InLineAlgorithm algo = new InLineAlgorithm(attacker.X, X, attacker.Y, Y, dist,
                                                                               InLineAlgorithm.Algorithm.DDA);
                                            X = attacker.X;
                                            Y = attacker.Y;
                                            var count = (double)algo.lcoords.Count / 3;
                                            int i = 0;
                                            for (i = 0; i < 4; i++)
                                            {
                                                var selected = i * (int)count;
                                                selected = Math.Min(algo.lcoords.Count - 1, selected);
                                                X = (ushort)algo.lcoords[selected].X;
                                                Y = (ushort)algo.lcoords[selected].Y;

                                                suse.X = X;//
                                                suse.Y = Y;
                                                if (!attacker.Owner.Map.Floor[X, Y, MapObjectType.Player, null])
                                                    return;
                                                double disth = 1.5;
                                                if (attacker.MapID == DeathMatch.MAPID) disth = 1;
                                                foreach (Interfaces.IMapObject _obj in Array)
                                                {
                                                    bool hit = false;
                                                    for (int j = 0; j < i; j++)
                                                        if (Kernel.GetDDistance(_obj.X, _obj.Y, (ushort)algo.lcoords[j].X, (ushort)algo.lcoords[j].Y) <= disth)
                                                            hit = true;
                                                    if (hit)
                                                    {
                                                        if (_obj.MapObjType == MapObjectType.Monster)
                                                        {
                                                            attacked = _obj as Entity;
                                                            if (CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                                            {
                                                                var damage = Game.Attacking.Calculate.Magic(attacker, attacked, spell, ref attack);
                                                                switch (i)
                                                                {
                                                                    case 1:
                                                                        damage *= 92 / 100;
                                                                        break;
                                                                    case 2:
                                                                        damage *= 102 / 100;
                                                                        break;
                                                                    default:
                                                                        damage *= 120 / 100;
                                                                        break;
                                                                }
                                                                ReceiveAttack(attacker, attacked, attack, damage, spell);

                                                                suse.AddTarget(attacked.UID, damage, attack);
                                                            }
                                                        }
                                                        else if (_obj.MapObjType == MapObjectType.Player)
                                                        {
                                                            attacked = _obj as Entity;
                                                            if (CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                                            {
                                                                var damage = Game.Attacking.Calculate.Melee(attacker, attacked, spell, ref attack);

                                                                ReceiveAttack(attacker, attacked, attack, damage, spell);

                                                                suse.AddTarget(attacked.UID, damage, attack);
                                                            }
                                                        }
                                                        else if (_obj.MapObjType == MapObjectType.SobNpc)
                                                        {
                                                            attackedsob = _obj as SobNpcSpawn;
                                                            if (CanAttack(attacker, attackedsob, spell))
                                                            {
                                                                var damage = Game.Attacking.Calculate.Melee(attacker, attackedsob, ref attack);

                                                                ReceiveAttack(attacker, attackedsob, attack, damage, spell);

                                                                suse.AddTarget(attackedsob.UID, damage, attack);
                                                            }
                                                        }
                                                    }
                                                }

                                                SendTwilightEffect(suse, X, Y);
                                                attacker.Owner.SendScreen(suse, true);

                                            }
                                        }
                                        break;
                                    }
                                #endregion
                                #region SuperTwofoldBlade
                                case 12080:
                                    {
                                        if (Time32.Now >= attacker.SpellStamp.AddMilliseconds(500))
                                        {
                                            if (CanUseSpell(spell, attacker.Owner))
                                            {
                                                ushort Xx, Yx;
                                                if (attacked != null)
                                                {
                                                    Xx = attacked.X;
                                                    Yx = attacked.Y;
                                                }
                                                else
                                                {
                                                    Xx = attackedsob.X;
                                                    Yx = attackedsob.Y;
                                                }
                                                if (Kernel.GetDistance(attacker.X, attacker.Y, Xx, Yx) <= spell.Distance)
                                                {
                                                    if (attackedsob == null)
                                                        if (attacked.ContainsFlag(Network.GamePackets.Update.Flags.Fly))
                                                            return;
                                                    if (attacker.ContainsFlag(Network.GamePackets.Update.Flags.Fly))
                                                        return;
                                                    PrepareSpell(spell, attacker.Owner);

                                                    SpellUse suse = new SpellUse(true);
                                                    suse.Attacker = attacker.UID;
                                                    suse.SpellID = spell.ID;
                                                    suse.SpellLevel = spell.Level;
                                                    suse.X = X;
                                                    suse.Y = Y;
                                                    //suse.SpellLevelHu = client_Spell.LevelHu2;
                                                    bool send = false;

                                                    if (attackedsob == null)
                                                    {
                                                        if (CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                                        {
                                                            attack.Effect1 = Attack.AttackEffects1.None;
                                                            uint damage = Game.Attacking.Calculate.Melee(attacker, attacked, ref attack);

                                                            var dist = Kernel.GetDistance(attacker.X, attacker.Y, attacked.X, attacked.Y);
                                                            if (dist <= 3)
                                                            {
                                                                damage *= 120 / 100;
                                                            }
                                                            else
                                                            {
                                                                damage *= 90 / 100;
                                                            }
                                                            suse.Effect1 = attack.Effect1;
                                                            ReceiveAttack(attacker, attacked, attack, damage, spell);

                                                            suse.AddTarget(attacked.UID, damage, attack);
                                                            send = true;

                                                            if (attacker.Owner.Spells.ContainsKey(11230) && !attacked.Dead)
                                                            {
                                                                var s = attacker.Owner.Spells[11230];
                                                                var spellz = Database.SpellTable.SpellInformations[s.ID][s.Level];
                                                                if (spellz != null)
                                                                {
                                                                    if (Kernel.Rate(spellz.Percent))
                                                                    {
                                                                        SpellUse ssuse = new SpellUse(true);
                                                                        ssuse.Attacker = attacker.UID;
                                                                        ssuse.SpellID = spellz.ID;

                                                                        ssuse.SpellLevel = spellz.Level;
                                                                        damage = Game.Attacking.Calculate.Melee(attacker, attacked, ref attack) / 2;
                                                                        ssuse.AddTarget(attacked.UID, new SpellUse.DamageClass().Damage = damage, attack);
                                                                        ReceiveAttack(attacker, attacked, attack, damage, spellz);
                                                                        attacker.Owner.SendScreen(ssuse, true);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (CanAttack(attacker, attackedsob, spell))
                                                        {
                                                            attack.Effect1 = Attack.AttackEffects1.None;
                                                            uint damage = Calculate.Melee(attacker, attackedsob, ref attack);
                                                            damage = (uint)(damage * spell.PowerPercent);
                                                            ReceiveAttack(attacker, attackedsob, attack, damage, spell);
                                                            suse.Effect1 = attack.Effect1;

                                                            suse.AddTarget(attackedsob.UID, damage, attack);
                                                            send = true;
                                                        }
                                                    }
                                                    if (send)
                                                        attacker.Owner.SendScreen(suse, true);
                                                    attacker.SpellStamp = Time32.Now;
                                                }
                                                else
                                                {
                                                    attacker.AttackPacket = null;
                                                }
                                            }
                                        }
                                        break;
                                    }


                                #endregion
                                #region ShadowClone
                                case 12090:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            PrepareSpell(spell, attacker.Owner);
                                            SpellUse spellUse = new SpellUse(true);
                                            spellUse.Attacker = attacker.Owner.Entity.UID;
                                            spellUse.SpellID = spell.ID;
                                            spellUse.SpellLevel = spell.Level;
                                            spellUse.X = attacker.Owner.Entity.X;
                                            spellUse.Y = attacker.Owner.Entity.Y;
                                            attacker.Owner.SendScreen(spellUse, true);

                                            if (attacker.MyClones.Count > 0)
                                            {
                                                foreach (var item in attacker.MyClones.Values)
                                                {
                                                    Data data = new Data(true);
                                                    data.UID = item.UID;
                                                    data.ID = Network.GamePackets.Data.RemoveEntity;
                                                    item.MonsterInfo.SendScreen(data);
                                                    attacker.Owner.Map.RemoveEntity(item);
                                                    attacker.Owner.Send(data);
                                                }
                                                attacker.MyClones.Clear();
                                                return;
                                            }
                                            attacker.AddClone("" + attacker.Name + "", 3);
                                            attacker.AddClone("" + attacker.Name + "", 10003);
                                        }
                                        break;
                                    }
                                #endregion
                                #region FatalSpin
                                case 12110:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            PrepareSpell(spell, attacker.Owner);
                                            Game.Attacking.InLineAlgorithm ila = new Game.Attacking.InLineAlgorithm(attacker.X,
                                        X, attacker.Y, Y, (byte)spell.Range, InLineAlgorithm.Algorithm.DDA);
                                            SpellUse suse = new SpellUse(true);
                                            suse.Attacker = attacker.UID;
                                            suse.SpellID = SpellID;
                                            suse.SpellLevel = attacker.Owner.Spells[SpellID].Level;
                                            suse.X = X;
                                            suse.Y = Y;
                                            //suse.SpellLevelHu = client_Spell.LevelHu2;
                                            foreach (Interfaces.IMapObject _obj in attacker.Owner.Screen.Objects)
                                            {
                                                if (_obj == null)
                                                    continue;
                                                if (_obj.MapObjType == MapObjectType.Monster || _obj.MapObjType == MapObjectType.Player)
                                                {
                                                    attacked = _obj as Entity;
                                                    if (ila.InLine(attacked.X, attacked.Y))
                                                    {
                                                        if (!CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                                            continue;

                                                        attack.Effect1 = Attack.AttackEffects1.None;
                                                        uint damage = Game.Attacking.Calculate.Melee(attacker, attacked, ref attack);
                                                        suse.Effect1 = attack.Effect1;

                                                        attack.Damage = damage;

                                                        ReceiveAttack(attacker, attacked, attack, damage, spell);

                                                        suse.AddTarget(attacked.UID, damage, attack);
                                                    }
                                                }
                                                else if (_obj.MapObjType == MapObjectType.SobNpc)
                                                {
                                                    attackedsob = _obj as SobNpcSpawn;

                                                    if (ila.InLine(attackedsob.X, attackedsob.Y))
                                                    {
                                                        if (!CanAttack(attacker, attackedsob, spell))
                                                            continue;

                                                        attack.Effect1 = Attack.AttackEffects1.None;
                                                        uint damage = Game.Attacking.Calculate.Melee(attacker, attackedsob, ref attack);
                                                        // damage = (uint)(damage * spell.PowerPercent);
                                                        attack.Damage = damage;

                                                        ReceiveAttack(attacker, attackedsob, attack, damage, spell);

                                                        suse.AddTarget(attackedsob.UID, damage, attack);
                                                    }
                                                }
                                            }
                                            attacker.Owner.SendScreen(suse, true);
                                        }
                                        break;
                                    }
                                #endregion
                                #endregion
                                #region Riding
                                case 7001:
                                    {
                                        if (attacker.Owner.Map.BaseID == 700) return;
                                        if (attacker.ContainsFlag(Update.Flags.ShurikenVortex))
                                            return;
                                        if (!attacker.Owner.Equipment.Free(12))
                                        {
                                            SpellUse suse = new SpellUse(true);
                                            suse.Attacker = attacker.UID;
                                            suse.SpellID = spell.ID;
                                            suse.SpellLevel = spell.Level;
                                            suse.X = X;
                                            suse.Y = Y;

                                            if (attacker.ContainsFlag(Update.Flags.Ride))
                                            {
                                                attacker.RemoveFlag(Update.Flags.Ride);
                                            }
                                            else
                                            {
                                                if (attacker.Owner.Map.ID == 1036 && attacker.Owner.Equipment.TryGetItem((byte)12).Plus < 6)
                                                    break;
                                                if (attacker.Stamina >= 100)
                                                {
                                                    attacker.AddFlag(Update.Flags.Ride);
                                                    attacker.Stamina -= 100;
                                                    attacker.Vigor = attacker.ExtraVigor;
                                                    Network.GamePackets.Vigor vigor = new Network.GamePackets.Vigor(true);
                                                    vigor.Amount = attacker.Owner.Vigor;
                                                    vigor.Send(attacker.Owner);
                                                }
                                            }
                                            suse.AddTarget(attacker.UID, 0, attack);
                                            attacker.Owner.SendScreen(suse, true);
                                        }
                                        break;
                                    }
                                case 7002:
                                    {//Spook
                                        if (attacked.ContainsFlag(Update.Flags.Ride) && attacker.ContainsFlag(Update.Flags.Ride))
                                        {
                                            ConquerItem attackedSteed = null, attackerSteed = null;
                                            if ((attackedSteed = attacked.Owner.Equipment.TryGetItem(ConquerItem.Steed)) != null)
                                            {
                                                if ((attackerSteed = attacker.Owner.Equipment.TryGetItem(ConquerItem.Steed)) != null)
                                                {
                                                    SpellUse suse = new SpellUse(true);
                                                    suse.Attacker = attacker.UID;
                                                    suse.SpellID = spell.ID;
                                                    suse.SpellLevel = spell.Level;
                                                    suse.X = X;
                                                    suse.Y = Y;
                                                    suse.AddTarget(attacked.UID, 0, attack);
                                                    if (attackedSteed.Plus < attackerSteed.Plus)
                                                        attacked.RemoveFlag(Update.Flags.Ride);
                                                    else if (attackedSteed.Plus == attackerSteed.Plus && attackedSteed.PlusProgress <= attackerSteed.PlusProgress)
                                                        attacked.RemoveFlag(Update.Flags.Ride);
                                                    else
                                                        suse.Targets[attacked.UID].Hit = false;
                                                    attacker.Owner.SendScreen(suse, true);
                                                }
                                            }
                                        }
                                        break;
                                    }
                                case 7003:
                                    {//WarCry
                                        SpellUse suse = new SpellUse(true);
                                        suse.Attacker = attacker.UID;
                                        suse.SpellID = spell.ID;
                                        suse.SpellLevel = spell.Level;
                                        suse.X = X;
                                        suse.Y = Y;
                                        ConquerItem attackedSteed = null, attackerSteed = null;
                                        foreach (Interfaces.IMapObject _obj in attacker.Owner.Screen.Objects)
                                        {
                                            if (_obj == null)
                                                continue;
                                            if (_obj.MapObjType == MapObjectType.Player && _obj.UID != attacker.UID)
                                            {
                                                attacked = _obj as Entity;
                                                if ((attackedSteed = attacked.Owner.Equipment.TryGetItem(ConquerItem.Steed)) != null)
                                                {
                                                    if ((attackerSteed = attacker.Owner.Equipment.TryGetItem(ConquerItem.Steed)) != null)
                                                    {
                                                        if (Kernel.GetDistance(attacker.X, attacker.Y, attacked.X, attacked.Y) <= attackedSteed.Plus)
                                                        {
                                                            if (CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                                            {
                                                                suse.AddTarget(attacked.UID, 0, attack);
                                                                if (attackedSteed.Plus < attackerSteed.Plus)
                                                                    attacked.RemoveFlag(Update.Flags.Ride);
                                                                else if (attackedSteed.Plus == attackerSteed.Plus && attackedSteed.PlusProgress <= attackerSteed.PlusProgress)
                                                                    attacked.RemoveFlag(Update.Flags.Ride);
                                                                else
                                                                    suse.Targets[attacked.UID].Hit = false;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        attacker.Owner.SendScreen(suse, true);
                                        break;
                                    }
                                #endregion  
                                #region Dash
                                case 1051:
                                    {
                                        if (attacked != null)
                                        {
                                            if (!attacked.Dead)
                                            {
                                                var direction = Kernel.GetAngle(attacker.X, attacker.Y, attacked.X, attacked.Y);
                                                if (CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                                {
                                                    attack = new Attack(true);
                                                    attack.Effect1 = Attack.AttackEffects1.None;
                                                    uint damage = Calculate.Melee(attacker, attacked, ref attack);
                                                    attack.AttackType = Attack.Dash;
                                                    attack.X = attacked.X;
                                                    attack.Y = attacked.Y;
                                                    attack.Attacker = attacker.UID;
                                                    attack.Attacked = attacked.UID;
                                                    attack.Damage = damage;
                                                    attack.ToArray()[27] = (byte)direction;
                                                    attacked.Move(direction);
                                                    attacker.Move(direction);

                                                    ReceiveAttack(attacker, attacked, attack, damage, spell);

                                                    attacker.Owner.SendScreen(attack, true);
                                                }
                                            }
                                        }
                                        break;
                                    }
                                #endregion
                                #region BladeTempest
                                //case 11110: {
                                //        if(attacked != null) {
                                //            if(!attacked.Dead) {
                                //                var direction = Kernel.GetAngle(attacker.X, attacker.Y, attacked.X, attacked.Y);
                                //                if(CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee)) {
                                //                    attack = new Attack(true);
                                //                    attack.Effect1 = Attack.AttackEffects1.None;
                                //                    uint damage = Calculate.Melee(attacker, attacked, ref attack) / 2;
                                //                    attack.AttackType = Attack.FatalStrike;
                                //                    attack.X = attacked.X;
                                //                    attack.Y = attacked.Y;
                                //                    attack.Attacker = attacker.UID;
                                //                    attack.Attacked = attacked.UID;
                                //                    attack.Damage = damage;
                                //                    attack.ToArray()[27] = (byte)direction;
                                //                    attacked.Move(direction);
                                //                    attacker.Move(direction);

                                //                    ReceiveAttack(attacker, attacked, attack, damage, spell);

                                //                    attacker.Owner.SendScreen(attack, true);
                                //                }
                                //            }
                                //        }
                                //        break;
                                //    }
                                #endregion
                                #region RapidFire
                                case 8000:
                                    {
                                        if (attackedsob != null)
                                        {
                                            if (CanUseSpell(spell, attacker.Owner))
                                            {
                                                if (CanAttack(attacker, attackedsob, spell))
                                                {
                                                    PrepareSpell(spell, attacker.Owner);
                                                    SpellUse suse = new SpellUse(true);
                                                    suse.Attacker = attacker.UID;
                                                    suse.SpellID = spell.ID;
                                                    suse.SpellLevel = spell.Level;
                                                    suse.X = attackedsob.X;
                                                    suse.Y = attackedsob.Y;
                                                    attack.Effect1 = Attack.AttackEffects1.None;
                                                    uint damage = Calculate.Ranged(attacker, attackedsob, ref attack);
                                                    suse.Effect1 = attack.Effect1;
                                                    damage = (uint)(damage * spell.PowerPercent);
                                                    suse.AddTarget(attackedsob.UID, damage, attack);

                                                    ReceiveAttack(attacker, attackedsob, attack, damage, spell);

                                                    attacker.Owner.SendScreen(suse, true);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (!attacked.Dead)
                                            {
                                                if (CanUseSpell(spell, attacker.Owner))
                                                {
                                                    if (CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                                    {
                                                        PrepareSpell(spell, attacker.Owner);
                                                        SpellUse suse = new SpellUse(true);
                                                        suse.Attacker = attacker.UID;
                                                        suse.SpellID = spell.ID;
                                                        suse.SpellLevel = spell.Level;
                                                        suse.X = attacked.X;
                                                        suse.Y = attacked.Y;
                                                        attack.Effect1 = Attack.AttackEffects1.None;
                                                        uint damage = Calculate.Ranged(attacker, attacked, ref attack);
                                                        damage = (uint)(damage * spell.PowerPercent);
                                                        suse.AddTarget(attacked.UID, damage, attack);

                                                        ReceiveAttack(attacker, attacked, attack, damage, spell);

                                                        attacker.Owner.SendScreen(suse, true);
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    }
                                #endregion

                                #region Scatter
                                case 8001:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            PrepareSpell(spell, attacker.Owner);

                                            SpellUse suse = new SpellUse(true);
                                            suse.Attacker = attacker.UID;
                                            suse.SpellID = spell.ID;
                                            suse.SpellLevel = spell.Level;
                                            suse.X = X;
                                            suse.Y = Y;
                                            Sector sector = new Sector(attacker.X, attacker.Y, X, Y);
                                            sector.Arrange(spell.Sector, spell.Distance);
                                            foreach (Interfaces.IMapObject _obj in attacker.Owner.Screen.Objects)
                                            {
                                                if (_obj == null)
                                                    continue;
                                                if (_obj.MapObjType == MapObjectType.Monster || _obj.MapObjType == MapObjectType.Player)
                                                {
                                                    attacked = _obj as Entity;

                                                    if (sector.Inside(attacked.X, attacked.Y))
                                                    {
                                                        if (CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                                        {
                                                            attack.Effect1 = Attack.AttackEffects1.None;
                                                            uint damage = Game.Attacking.Calculate.Ranged(attacker, attacked, spell, ref attack);

                                                            ReceiveAttack(attacker, attacked, attack, damage, spell);

                                                            suse.AddTarget(attacked.UID, damage, attack);
                                                        }
                                                    }
                                                }
                                                else if (_obj.MapObjType == MapObjectType.SobNpc)
                                                {
                                                    attackedsob = _obj as SobNpcSpawn;

                                                    if (sector.Inside(attackedsob.X, attackedsob.Y))
                                                    {
                                                        if (CanAttack(attacker, attackedsob, spell))
                                                        {
                                                            attack.Effect1 = Attack.AttackEffects1.None;
                                                            uint damage = Game.Attacking.Calculate.Ranged(attacker, attackedsob, ref attack);
                                                            suse.Effect1 = attack.Effect1;
                                                            if (damage == 0)
                                                                damage = 1;
                                                            damage = Game.Attacking.Calculate.Percent((int)damage, spell.PowerPercent);

                                                            ReceiveAttack(attacker, attackedsob, attack, damage, spell);

                                                            suse.AddTarget(attackedsob.UID, damage, attack);
                                                        }
                                                    }
                                                }
                                            }
                                            attacker.Owner.SendScreen(suse, true);
                                        }
                                        break;
                                    }
                                #endregion
                                #region Intensify
                                case 9000:
                                    {
                                        this.attacker.IntensifyStamp = Time32.Now;
                                        this.attacker.OnIntensify = true;
                                        Attack attack3 = new Attack(true)
                                        {
                                            Attacker = this.attacker.UID,
                                            AttackType = 0x18,
                                            X = X,
                                            Y = Y
                                        };
                                        Writer.WriteUInt16(0, 4, attack3.ToArray());
                                        Writer.WriteUInt16(attack.X, 16 + 4, attack3.ToArray());
                                        Writer.WriteUInt16(attack.Y, 18 + 4, attack3.ToArray());
                                        Writer.WriteUInt16(spell.ID, 24 + 4, attack3.ToArray());
                                        Writer.WriteByte(spell.Level, 26 + 4, attack3.ToArray());
                                        this.attacker.Owner.SendScreen(attack3, true);
                                        this.attacker.X = X;
                                        this.attacker.Y = Y;
                                        this.attacker.SendSpawn(this.attacker.Owner);
                                        break;
                                    }
                                #endregion
                                #region StarArrow
                                case 10313:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            PrepareSpell(spell, attacker.Owner);

                                            SpellUse suse = new SpellUse(true);
                                            suse.Attacker = attacker.UID;
                                            suse.SpellID = spell.ID;
                                            suse.SpellLevel = spell.Level;

                                            if (attacked != null)
                                            {
                                                if (CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                                {
                                                    ushort _X = attacked.X, _Y = attacked.Y;
                                                    byte dist = 5;
                                                    var angle = Kernel.GetAngle(attacker.X, attacker.Y, attacked.X, attacked.Y);
                                                    while (dist != 0)
                                                    {
                                                        if (attacked.fMove(angle, ref _X, ref _Y))
                                                        {
                                                            X = _X;
                                                            Y = _Y;
                                                        }
                                                        else break;
                                                        dist--;
                                                    }
                                                    suse.X = attacked.X = X;
                                                    suse.Y = attacked.Y = Y;
                                                    attack.Effect1 = Attack.AttackEffects1.None;
                                                    uint damage = Game.Attacking.Calculate.Ranged(attacker, attacked, spell, ref attack);
                                                    ReceiveAttack(attacker, attacked, attack, damage, spell);

                                                    suse.AddTarget(attacked.UID, damage, attack);
                                                }
                                            }
                                            else if (attackedsob != null)
                                            {
                                                if (CanAttack(attacker, attackedsob, spell))
                                                {
                                                    suse.X = attackedsob.X;
                                                    suse.Y = attackedsob.Y;
                                                    attack.Effect1 = Attack.AttackEffects1.None;
                                                    uint damage = Game.Attacking.Calculate.Ranged(attacker, attackedsob, ref attack);
                                                    suse.Effect1 = attack.Effect1;
                                                    if (damage == 0)
                                                        damage = 1;
                                                    damage = Game.Attacking.Calculate.Percent((int)damage, spell.PowerPercent);

                                                    ReceiveAttack(attacker, attackedsob, attack, damage, spell);

                                                    suse.AddTarget(attackedsob.UID, damage, attack);
                                                }
                                            }
                                            attacker.Owner.SendScreen(suse, true);
                                        }
                                        break;
                                    }
                                #endregion
                                #region Trasnformations
                                case 1270:
                                case 10501:
                                case 1280:
                                case 1350:
                                case 10514:
                                case 1360:
                                case 3321:
                                case 10000:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            if (attacker.MapID == 1036)
                                                return;
                                            if (attacker.MapID == 1950)
                                                return;
                                            bool wasTransformated = attacker.Transformed;
                                            PrepareSpell(spell, attacker.Owner);

                                            #region Atributes
                                            switch (spell.ID)
                                            {
                                                case 3321://GM skill
                                                    {
                                                        attacker.TransformationMaxAttack = 2000000;
                                                        attacker.TransformationMinAttack = 2000000;
                                                        attacker.TransformationDefence = 65355;
                                                        attacker.TransformationMagicDefence = 65355;
                                                        attacker.TransformationDodge = 35;
                                                        attacker.TransformationTime = 65355;
                                                        attacker.TransformationID = 223;
                                                        attacker.Hitpoints = attacker.MaxHitpoints;
                                                        attacker.Mana = attacker.MaxMana;
                                                        break;
                                                    }
                                                case 1350:
                                                    switch (spell.Level)
                                                    {
                                                        case 0:
                                                            {
                                                                attacker.TransformationMaxAttack = 182;
                                                                attacker.TransformationMinAttack = 122;
                                                                attacker.TransformationDefence = 1300;
                                                                attacker.TransformationMagicDefence = 94;
                                                                attacker.TransformationDodge = 35;
                                                                attacker.TransformationTime = 39;
                                                                attacker.TransformationID = 207;
                                                                break;
                                                            }
                                                        case 1:
                                                            {
                                                                attacker.TransformationMaxAttack = 200;
                                                                attacker.TransformationMinAttack = 134;
                                                                attacker.TransformationDefence = 1400;
                                                                attacker.TransformationMagicDefence = 96;
                                                                attacker.TransformationDodge = 40;
                                                                attacker.TransformationTime = 49;
                                                                attacker.TransformationID = 207;
                                                                break;
                                                            }
                                                        case 2:
                                                            {
                                                                attacker.TransformationMaxAttack = 240;
                                                                attacker.TransformationMinAttack = 160;
                                                                attacker.TransformationDefence = 1500;
                                                                attacker.TransformationMagicDefence = 97;
                                                                attacker.TransformationDodge = 45;
                                                                attacker.TransformationTime = 59;
                                                                attacker.TransformationID = 207;
                                                                break;
                                                            }
                                                        case 3:
                                                            {
                                                                attacker.TransformationMaxAttack = 258;
                                                                attacker.TransformationMinAttack = 172;
                                                                attacker.TransformationDefence = 1600;
                                                                attacker.TransformationMagicDefence = 98;
                                                                attacker.TransformationDodge = 50;
                                                                attacker.TransformationTime = 69;
                                                                attacker.TransformationID = 267;
                                                                break;
                                                            }
                                                        case 4:
                                                            {
                                                                attacker.TransformationMaxAttack = 300;
                                                                attacker.TransformationMinAttack = 200;
                                                                attacker.TransformationDefence = 1900;
                                                                attacker.TransformationMagicDefence = 99;
                                                                attacker.TransformationDodge = 55;
                                                                attacker.TransformationTime = 79;
                                                                attacker.TransformationID = 267;
                                                                break;
                                                            }
                                                    }
                                                    break;
                                                case 1270:
                                                    switch (spell.Level)
                                                    {
                                                        case 0:
                                                            {
                                                                attacker.TransformationMaxAttack = 282;
                                                                attacker.TransformationMinAttack = 179;
                                                                attacker.TransformationDefence = 73;
                                                                attacker.TransformationMagicDefence = 34;
                                                                attacker.TransformationDodge = 9;
                                                                attacker.TransformationTime = 34;
                                                                attacker.TransformationID = 214;
                                                                break;
                                                            }
                                                        case 1:
                                                            {
                                                                attacker.TransformationMaxAttack = 395;
                                                                attacker.TransformationMinAttack = 245;
                                                                attacker.TransformationDefence = 126;
                                                                attacker.TransformationMagicDefence = 45;
                                                                attacker.TransformationDodge = 12;
                                                                attacker.TransformationTime = 39;
                                                                attacker.TransformationID = 214;
                                                                break;
                                                            }
                                                        case 2:
                                                            {
                                                                attacker.TransformationMaxAttack = 616;
                                                                attacker.TransformationMinAttack = 367;
                                                                attacker.TransformationDefence = 180;
                                                                attacker.TransformationMagicDefence = 53;
                                                                attacker.TransformationDodge = 15;
                                                                attacker.TransformationTime = 44;
                                                                attacker.TransformationID = 214;
                                                                break;
                                                            }
                                                        case 3:
                                                            {
                                                                attacker.TransformationMaxAttack = 724;
                                                                attacker.TransformationMinAttack = 429;
                                                                attacker.TransformationDefence = 247;
                                                                attacker.TransformationMagicDefence = 53;
                                                                attacker.TransformationDodge = 15;
                                                                attacker.TransformationTime = 49;
                                                                attacker.TransformationID = 214;
                                                                break;
                                                            }
                                                        case 4:
                                                            {
                                                                attacker.TransformationMaxAttack = 1231;
                                                                attacker.TransformationMinAttack = 704;
                                                                attacker.TransformationDefence = 499;
                                                                attacker.TransformationMagicDefence = 50;
                                                                attacker.TransformationDodge = 20;
                                                                attacker.TransformationTime = 54;
                                                                attacker.TransformationID = 274;
                                                                break;
                                                            }
                                                        case 5:
                                                            {
                                                                attacker.TransformationMaxAttack = 1573;
                                                                attacker.TransformationMinAttack = 941;
                                                                attacker.TransformationDefence = 601;
                                                                attacker.TransformationMagicDefence = 53;
                                                                attacker.TransformationDodge = 25;
                                                                attacker.TransformationTime = 59;
                                                                attacker.TransformationID = 274;
                                                                break;
                                                            }
                                                        case 6:
                                                            {
                                                                attacker.TransformationMaxAttack = 1991;
                                                                attacker.TransformationMinAttack = 1107;
                                                                attacker.TransformationDefence = 1029;
                                                                attacker.TransformationMagicDefence = 55;
                                                                attacker.TransformationDodge = 30;
                                                                attacker.TransformationTime = 64;
                                                                attacker.TransformationID = 274;
                                                                break;
                                                            }
                                                        case 7:
                                                            {
                                                                attacker.TransformationMaxAttack = 2226;
                                                                attacker.TransformationMinAttack = 1235;
                                                                attacker.TransformationDefence = 1029;
                                                                attacker.TransformationMagicDefence = 55;
                                                                attacker.TransformationDodge = 35;
                                                                attacker.TransformationTime = 69;
                                                                attacker.TransformationID = 274;
                                                                break;
                                                            }
                                                    }
                                                    break;
                                                case 1360:
                                                    switch (spell.Level)
                                                    {
                                                        case 0:
                                                            {
                                                                attacker.TransformationMaxAttack = 1215;
                                                                attacker.TransformationMinAttack = 610;
                                                                attacker.TransformationDefence = 100;
                                                                attacker.TransformationMagicDefence = 96;
                                                                attacker.TransformationDodge = 30;
                                                                attacker.TransformationTime = 59;
                                                                attacker.TransformationID = 217;
                                                                break;
                                                            }
                                                        case 1:
                                                            {
                                                                attacker.TransformationMaxAttack = 1310;
                                                                attacker.TransformationMinAttack = 650;
                                                                attacker.TransformationDefence = 400;
                                                                attacker.TransformationMagicDefence = 97;
                                                                attacker.TransformationDodge = 30;
                                                                attacker.TransformationTime = 79;
                                                                attacker.TransformationID = 217;
                                                                break;
                                                            }
                                                        case 2:
                                                            {
                                                                attacker.TransformationMaxAttack = 1420;
                                                                attacker.TransformationMinAttack = 710;
                                                                attacker.TransformationDefence = 650;
                                                                attacker.TransformationMagicDefence = 98;
                                                                attacker.TransformationDodge = 30;
                                                                attacker.TransformationTime = 89;
                                                                attacker.TransformationID = 217;
                                                                break;
                                                            }
                                                        case 3:
                                                            {
                                                                attacker.TransformationMaxAttack = 1555;
                                                                attacker.TransformationMinAttack = 780;
                                                                attacker.TransformationDefence = 720;
                                                                attacker.TransformationMagicDefence = 98;
                                                                attacker.TransformationDodge = 30;
                                                                attacker.TransformationTime = 99;
                                                                attacker.TransformationID = 277;
                                                                break;
                                                            }
                                                        case 4:
                                                            {
                                                                attacker.TransformationMaxAttack = 1660;
                                                                attacker.TransformationMinAttack = 840;
                                                                attacker.TransformationDefence = 1200;
                                                                attacker.TransformationMagicDefence = 99;
                                                                attacker.TransformationDodge = 30;
                                                                attacker.TransformationTime = 109;
                                                                attacker.TransformationID = 277;
                                                                break;
                                                            }
                                                    }
                                                    break;
                                                case 1280:
                                                    switch (spell.Level)
                                                    {
                                                        case 0:
                                                            {
                                                                attacker.TransformationMaxAttack = 930;
                                                                attacker.TransformationMinAttack = 656;
                                                                attacker.TransformationDefence = 290;
                                                                attacker.TransformationMagicDefence = 45;
                                                                attacker.TransformationDodge = 40;
                                                                attacker.TransformationTime = 29;
                                                                attacker.TransformationID = 213;
                                                                break;
                                                            }
                                                        case 1:
                                                            {
                                                                attacker.TransformationMaxAttack = 1062;
                                                                attacker.TransformationMinAttack = 750;
                                                                attacker.TransformationDefence = 320;
                                                                attacker.TransformationMagicDefence = 46;
                                                                attacker.TransformationDodge = 40;
                                                                attacker.TransformationTime = 34;
                                                                attacker.TransformationID = 213;
                                                                break;
                                                            }
                                                        case 2:
                                                            {
                                                                attacker.TransformationMaxAttack = 1292;
                                                                attacker.TransformationMinAttack = 910;
                                                                attacker.TransformationDefence = 510;
                                                                attacker.TransformationMagicDefence = 50;
                                                                attacker.TransformationDodge = 40;
                                                                attacker.TransformationTime = 39;
                                                                attacker.TransformationID = 213;
                                                                break;
                                                            }
                                                        case 3:
                                                            {
                                                                attacker.TransformationMaxAttack = 1428;
                                                                attacker.TransformationMinAttack = 1000;
                                                                attacker.TransformationDefence = 600;
                                                                attacker.TransformationMagicDefence = 53;
                                                                attacker.TransformationDodge = 40;
                                                                attacker.TransformationTime = 44;
                                                                attacker.TransformationID = 213;
                                                                break;
                                                            }
                                                        case 4:
                                                            {
                                                                attacker.TransformationMaxAttack = 1570;
                                                                attacker.TransformationMinAttack = 1100;
                                                                attacker.TransformationDefence = 700;
                                                                attacker.TransformationMagicDefence = 55;
                                                                attacker.TransformationDodge = 40;
                                                                attacker.TransformationTime = 49;
                                                                attacker.TransformationID = 213;
                                                                break;
                                                            }
                                                        case 5:
                                                            {
                                                                attacker.TransformationMaxAttack = 1700;
                                                                attacker.TransformationMinAttack = 1200;
                                                                attacker.TransformationDefence = 880;
                                                                attacker.TransformationMagicDefence = 57;
                                                                attacker.TransformationDodge = 40;
                                                                attacker.TransformationTime = 54;
                                                                attacker.TransformationID = 273;
                                                                break;
                                                            }
                                                        case 6:
                                                            {
                                                                attacker.TransformationMaxAttack = 1900;
                                                                attacker.TransformationMinAttack = 1300;
                                                                attacker.TransformationDefence = 1540;
                                                                attacker.TransformationMagicDefence = 59;
                                                                attacker.TransformationDodge = 40;
                                                                attacker.TransformationTime = 59;
                                                                attacker.TransformationID = 273;
                                                                break;
                                                            }
                                                        case 7:
                                                            {
                                                                attacker.TransformationMaxAttack = 2100;
                                                                attacker.TransformationMinAttack = 1500;
                                                                attacker.TransformationDefence = 1880;
                                                                attacker.TransformationMagicDefence = 61;
                                                                attacker.TransformationDodge = 40;
                                                                attacker.TransformationTime = 59;
                                                                attacker.TransformationID = 273;
                                                                break;
                                                            }
                                                        case 8:
                                                            {
                                                                attacker.TransformationMaxAttack = 2300;
                                                                attacker.TransformationMinAttack = 1600;
                                                                attacker.TransformationDefence = 1970;
                                                                attacker.TransformationMagicDefence = 63;
                                                                attacker.TransformationDodge = 40;
                                                                attacker.TransformationTime = 59;
                                                                attacker.TransformationID = 273;
                                                                break;
                                                            }
                                                    }
                                                    break;

                                            }
                                            #endregion

                                            SpellUse spellUse = new SpellUse(true);
                                            spellUse.Attacker = attacker.UID;
                                            spellUse.SpellID = spell.ID;
                                            spellUse.SpellLevel = spell.Level;
                                            spellUse.X = X;
                                            spellUse.Y = Y;
                                            spellUse.AddTarget(attacker.UID, (uint)0, attack);
                                            attacker.Owner.SendScreen(spellUse, true);
                                            attacker.TransformationStamp = Time32.Now;
                                            attacker.TransformationMaxHP = 3000;
                                            if (spell.ID == 1270)
                                                attacker.TransformationMaxHP = 50000;
                                            attacker.TransformationAttackRange = 3;
                                            if (spell.ID == 1360)
                                                attacker.TransformationAttackRange = 10;
                                            if (!wasTransformated)
                                            {
                                                double maxHP = attacker.MaxHitpoints;
                                                double HP = attacker.Hitpoints;
                                                double point = HP / maxHP;

                                                attacker.Hitpoints = (uint)(attacker.TransformationMaxHP * point);
                                            }
                                            attacker.Update(Update.MaxHitpoints, attacker.TransformationMaxHP, false);
                                        }
                                        break;
                                    }
                                #endregion
                                #region Bless
                                case 9876:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            PrepareSpell(spell, attacker.Owner);
                                            attacker.AddFlag(Update.Flags.CastPray);
                                            SpellUse spellUse = new SpellUse(true);
                                            spellUse.Attacker = attacker.UID;
                                            spellUse.SpellID = spell.ID;
                                            spellUse.SpellLevel = spell.Level;
                                            spellUse.X = X;
                                            spellUse.Y = Y;
                                            spellUse.AddTarget(attacker.UID, 0, attack);
                                            attacker.Owner.SendScreen(spellUse, true);
                                        }
                                        break;
                                    }
                                #endregion
                                #region Companions
                                case 4000:
                                case 4010:
                                case 4020:
                                case 4050:
                                case 4060:
                                case 4070:
                                case 12020:  // conquer Domyat
                                case 12030:
                                case 12031:
                                case 12040:
                                case 12041:
                                case 12050:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            if (attacker.Owner.Companion != null)
                                            {
                                                if (attacker.Owner.Companion.MonsterInfo != null)
                                                {
                                                    attacker.Owner.Map.RemoveEntity(attacker.Owner.Companion);
                                                    Data data = new Data(true);
                                                    data.UID = attacker.Owner.Companion.UID;
                                                    data.ID = Data.RemoveEntity;
                                                    attacker.Owner.Companion.MonsterInfo.SendScreen(data);
                                                    attacker.Owner.Companion = null;
                                                }
                                            }
                                            PrepareSpell(spell, attacker.Owner);
                                            SpellUse spellUse = new SpellUse(true);
                                            spellUse.Attacker = attacker.UID;
                                            spellUse.SpellID = spell.ID;
                                            spellUse.SpellLevel = spell.Level;
                                            spellUse.X = X;
                                            spellUse.Y = Y;
                                            spellUse.AddTarget(attacker.UID, 0, attack);
                                            attacker.Owner.SendScreen(spellUse, true);
                                            attacker.Owner.Companion = new Entity(EntityFlag.Monster, true);
                                            attacker.Owner.Companion.MonsterInfo = new Conquer_Online_Server.Database.MonsterInformation();
                                            Database.MonsterInformation mt = Database.MonsterInformation.MonsterInformations[spell.Power];
                                            attacker.Owner.Companion.Owner = attacker.Owner;
                                            attacker.Owner.Companion.MapObjType = MapObjectType.Monster;
                                            attacker.Owner.Companion.MonsterInfo = mt.Copy();
                                            attacker.Owner.Companion.MonsterInfo.Owner = attacker.Owner.Companion;
                                            attacker.Owner.Companion.Name = mt.Name;
                                            attacker.Owner.Companion.MinAttack = mt.MinAttack;
                                            attacker.Owner.Companion.MaxAttack = attacker.Owner.Companion.MagicAttack = mt.MaxAttack;
                                            attacker.Owner.Companion.Hitpoints = attacker.Owner.Companion.MaxHitpoints = mt.Hitpoints;
                                            attacker.Owner.Companion.Body = mt.Mesh;
                                            attacker.Owner.Companion.Level = mt.Level;
                                            attacker.Owner.Companion.UID = (uint)(attacker.UID - 200000);
                                            attacker.Owner.Companion.MapID = attacker.Owner.Map.ID;
                                            attacker.Owner.Companion.SendUpdates = true;
                                            attacker.Owner.Companion.X = attacker.X;
                                            attacker.Owner.Companion.Y = attacker.Y;
                                            attacker.Owner.Map.AddEntity(attacker.Owner.Companion);
                                            attacker.Owner.SendScreenSpawn(attacker.Owner.Companion, true);
                                        }
                                        break;
                                    }
                                #endregion

                                #region MonkSpells

                                #region Auras

                                // case 10424:
                                case 10423:
                                case 10422:
                                case 10421:
                                case 10420:
                                //Tyrant Aura
                                case 10395:
                                //Fend Aura
                                case 10410:
                                    HandleAura(attacker, spell);
                                    break;
                                #endregion

                                //Compassion
                                case 10430:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            SpellUse suse = new SpellUse(true);
                                            suse.Attacker = attacker.UID;
                                            suse.SpellID = spell.ID;
                                            suse.SpellLevel = spell.Level;
                                            suse.X = X;
                                            suse.Y = Y;

                                            if (attacker.Owner.Team != null)
                                            {
                                                PrepareSpell(spell, attacker.Owner);
                                                foreach (Client.GameClient teammate in attacker.Owner.Team.Teammates)
                                                {
                                                    if (Kernel.GetDistance(attacker.X, attacker.Y, teammate.Entity.X, teammate.Entity.Y) <= spell.Distance)
                                                    {
                                                        teammate.Entity.RemoveFlag(Update.Flags.Poisoned);

                                                        suse.AddTarget(teammate.Entity.UID, 0, attack);
                                                    }
                                                }
                                                attacker.Owner.SendScreen(suse, true);
                                            }
                                            else
                                            {
                                                PrepareSpell(spell, attacker.Owner);

                                                attacker.RemoveFlag(Update.Flags.Poisoned);

                                                suse.AddTarget(attacker.UID, 0, attack);

                                                if (attacked.EntityFlag == EntityFlag.Player)
                                                    attacked.Owner.SendScreen(suse, true);
                                                else
                                                    attacker.Owner.SendScreen(suse, true);
                                            }
                                        }
                                        break;
                                    }
                                //Serenity
                                case 10400:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            PrepareSpell(spell, attacker.Owner);

                                            if (attacker == null) return;

                                            SpellUse suse = new SpellUse(true);
                                            suse.Attacker = attacker.UID;
                                            suse.SpellID = SpellID;
                                            suse.SpellLevel = attacker.Owner.Spells[SpellID].Level;
                                            suse.X = X;
                                            suse.Y = Y;

                                            suse.AddTarget(attacker.UID, 0, attack);
                                            attacked.ToxicFogLeft = 0;
                                            attacked.NoDrugsTime = 0;
                                            attacked.RemoveFlag2(Update.Flags2.SoulShackle);
                                            Conquer_Online_Server.Network.GamePackets.SyncPacket packet3 = new Conquer_Online_Server.Network.GamePackets.SyncPacket
                                            {
                                                Identifier = attacked.UID,
                                                Count = 2,
                                                Type = Conquer_Online_Server.Network.GamePackets.SyncPacket.SyncType.StatusFlag,
                                                StatusFlag1 = (ulong)attacked.StatusFlag,
                                                StatusFlag2 = (ulong)attacked.StatusFlag2,
                                                Unknown1 = 0x36,
                                                StatusFlagOffset = 0x6f,
                                                Time = 0,
                                                Value = 0,
                                                Level = spell.Level
                                            };
                                            attacked.Owner.Send((byte[])packet3);
                                            attacker.Owner.SendScreen(suse, true);
                                        }
                                        break;
                                    }
                                //Tranquility
                                case 10425:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            PrepareSpell(spell, attacker.Owner);

                                            if (attacked == null) return;

                                            SpellUse suse = new SpellUse(true);
                                            suse.Attacker = attacker.UID;
                                            suse.SpellID = SpellID;
                                            suse.SpellLevel = attacker.Owner.Spells[SpellID].Level;
                                            suse.X = X;
                                            suse.Y = Y;


                                            suse.AddTarget(attacked.UID, 0, attack);

                                            attacked.ToxicFogLeft = 0;
                                            attacked.RemoveFlag2(Update.Flags2.SoulShackle);
                                            if (attacked.EntityFlag == EntityFlag.Player)
                                            {
                                                Conquer_Online_Server.Network.GamePackets.SyncPacket packet3 = new Conquer_Online_Server.Network.GamePackets.SyncPacket
                                                {
                                                    Identifier = attacked.UID,
                                                    Count = 2,
                                                    Type = Conquer_Online_Server.Network.GamePackets.SyncPacket.SyncType.StatusFlag,
                                                    StatusFlag1 = (ulong)attacked.StatusFlag,
                                                    StatusFlag2 = (ulong)attacked.StatusFlag2,
                                                    Unknown1 = 0x36,
                                                    StatusFlagOffset = 0x6f,
                                                    Time = 0,
                                                    Value = 0,
                                                    Level = spell.Level
                                                };
                                            }
                                            attacked.NoDrugsTime = 0;

                                            if (attacked.EntityFlag == EntityFlag.Player)
                                                attacked.Owner.SendScreen(suse, true);
                                            else
                                                attacker.Owner.SendScreen(suse, true);
                                        }
                                        break;
                                    }
                                //Radiant Palm 
                                /*case 10381:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            PrepareSpell(spell, attacker.Owner);
                                            Game.Attacking.InLineAlgorithm ila = new Conquer_Online_Server.Game.Attacking.InLineAlgorithm(attacker.X,
                                        X, attacker.Y, Y, (byte)spell.Range, InLineAlgorithm.Algorithm.DDA);
                                            SpellUse suse = new SpellUse(true);
                                            suse.Attacker = attacker.UID;
                                            suse.SpellID = SpellID;
                                            suse.SpellLevel = attacker.Owner.Spells[SpellID].Level;
                                            suse.X = X;
                                            suse.Y = Y;
                                            for (int c = 0; c < attacker.Owner.Screen.Objects.Length; c++)
                                            {
                                                //For a multi threaded application, while we go through the collection
                                                //the collection might change. We will make sure that we wont go off  
                                                //the limits with a check.
                                                if (c >= attacker.Owner.Screen.Objects.Length)
                                                    break;
                                                Interfaces.IMapObject _obj = attacker.Owner.Screen.Objects[c];
                                                if (_obj == null)
                                                    continue;
                                                if (_obj.MapObjType == MapObjectType.Monster || _obj.MapObjType == MapObjectType.Player)
                                                {
                                                    attacked = _obj as Entity;
                                                    if (ila.InLine(attacked.X, attacked.Y))
                                                    {
                                                        if (!CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                                            continue;
                                                        //Conquer_Online_Server nothing
                                                        uint damage = Game.Attacking.Calculate.Melee(attacker, attacked, spell, ref attack)-8000;
                                

                                                        attack.Damage = damage;

                                                        ReceiveAttack(attacker, attacked, attack, damage, spell);

                                                        suse.AddTarget(attacked.UID, damage, attack);
                                                    }
                                                }
                                                else if (_obj.MapObjType == MapObjectType.SobNpc)
                                                {
                                                    attackedsob = _obj as SobNpcSpawn;

                                                    if (ila.InLine(attackedsob.X, attackedsob.Y))
                                                    {
                                                        if (!CanAttack(attacker, attackedsob, spell))
                                                            continue;

                                                        uint damage = Game.Attacking.Calculate.Melee(attacker, attackedsob, ref attack);
                                                        damage = (uint)(damage * spell.PowerPercent);
                                                        attack.Damage = damage;

                                                        ReceiveAttack(attacker, attackedsob, attack, damage, spell);

                                                        suse.AddTarget(attackedsob.UID, damage, attack);
                                                    }
                                                }
                                            }

                                            attacker.Owner.SendScreen(suse, true);
                                        }
                                        attacker.AttackPacket = null;
                                        break;
                                    }*/
                                //WhirlwindKick
                                case 10415:
                                    {
                                        if (Time32.Now < attacker.SpellStamp.AddMilliseconds(900)) { attacker.AttackPacket = null; return; }
                                        if (Kernel.GetDistance(attacker.X, attacker.Y, X, Y) <= 3)
                                        {
                                            if (CanUseSpell(spell, attacker.Owner))
                                            {
                                                PrepareSpell(spell, attacker.Owner);

                                                SpellUse suse = new SpellUse(true);
                                                suse.Attacker = attacker.UID;
                                                suse.SpellID = spell.ID;
                                                suse.SpellLevel = 0;
                                                suse.X = (ushort)Kernel.Random.Next(3, 10);
                                                suse.Y = 0;

                                                if (Kernel.GetDistance(attacker.X, attacker.Y, X, Y) <= 3)
                                                {
                                                    for (int c = 0; c < attacker.Owner.Screen.Objects.Length; c++)
                                                    {
                                                        //For a multi threaded application, while we go through the collection
                                                        //the collection might change. We will make sure that we wont go off  
                                                        //the limits with a check.
                                                        if (c >= attacker.Owner.Screen.Objects.Length)
                                                            break;
                                                        Interfaces.IMapObject _obj = attacker.Owner.Screen.Objects[c];
                                                        if (_obj == null)
                                                            continue;
                                                        if (_obj.MapObjType == MapObjectType.Monster || _obj.MapObjType == MapObjectType.Player)
                                                        {
                                                            attacked = _obj as Entity;
                                                            if (Kernel.GetDistance(attacker.X, attacker.Y, attacked.X, attacked.Y) <= spell.Range)
                                                            {
                                                                if (CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Ranged))
                                                                {
                                                                    uint damage = Game.Attacking.Calculate.Melee(attacker, attacked, ref attack) * 3;
                                                                    damage = (damage * 40) / 100;
                                                                    suse.Effect1 = attack.Effect1;
                                                                    ReceiveAttack(attacker, attacked, attack, damage, spell);
                                                                    attacked.Stunned = true;
                                                                    attacked.StunStamp = Time32.Now;
                                                                    suse.AddTarget(attacked.UID, damage, attack);

                                                                }
                                                            }
                                                        }
                                                    }
                                                    attacker.AttackPacket = null;
                                                }
                                                else
                                                {
                                                    attacker.AttackPacket = null; return;
                                                }
                                                attacker.Owner.SendScreen(suse, true);
                                                attacker.SpellStamp = Time32.Now;
                                                suse.Targets = new SafeDictionary<uint, SpellUse.DamageClass>();
                                                attacker.AttackPacket = null; return;
                                            }
                                            attacker.AttackPacket = null;
                                        }
                                        attacker.AttackPacket = null; return;
                                    }
                                #endregion

                                #region PirateSpells
                                #region GaleBomb
                                case 11070:
                                    if (CanUseSpell(spell, attacker.Owner))
                                    {

                                        PrepareSpell(spell, attacker.Owner);
                                        Map map;
                                        SpellUse suse = new SpellUse(true);
                                        suse.Attacker = attacker.UID;
                                        suse.SpellID = spell.ID;
                                        suse.SpellLevel = spell.Level;
                                        suse.X = X;
                                        suse.Y = Y;
                                        Conquer_Online_Server.Network.GamePackets.SpellUse.DamageClass tar = new SpellUse.DamageClass();
                                        int num = 0;

                                        switch (spell.Level)
                                        {
                                            case 0:
                                            case 1:
                                                num = 3;
                                                break;
                                            case 2:
                                            case 3:
                                                num = 4;
                                                break;
                                            default:
                                                num = 5;
                                                break;
                                        }
                                        int i = 0;
                                        Kernel.Maps.TryGetValue(attacker.Owner.Map.BaseID, out map);
                                        foreach (var t in attacker.Owner.Screen.Objects)
                                        {
                                            if (t == null)
                                                continue;
                                            if (t.MapObjType == MapObjectType.Player || t.MapObjType == MapObjectType.Monster)
                                            {
                                                var target = t as Entity;
                                                if (Kernel.GetDistance(X, Y, target.X, target.Y) <= spell.Range)
                                                {
                                                    if (CanAttack(attacker, target, spell, false))
                                                    {
                                                        tar.Damage = Calculate.Melee(attacker, target, spell, ref attack) - 10000 * (uint)(spell.PowerPercent);
                                                        tar.Hit = true;
                                                        tar.newX = target.X;
                                                        tar.newY = target.Y;
                                                        Map.Pushback(ref tar.newX, ref tar.newY, attacker.Facing, 5);

                                                        if (map != null)
                                                        {
                                                            if (map.Floor[tar.newX, tar.newY, MapObjectType.Player, attacker])
                                                            {
                                                                target.X = tar.newX;
                                                                target.Y = tar.newY;
                                                            }
                                                            else
                                                            {
                                                                tar.newX = target.X;
                                                                tar.newY = target.Y;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (attacker.Owner.Map.Floor[tar.newX, tar.newY, MapObjectType.Player, attacker])
                                                            {
                                                                target.X = tar.newX;
                                                                target.Y = tar.newY;
                                                            }
                                                            else
                                                            {
                                                                target.X = tar.newX;
                                                                target.Y = tar.newY;
                                                            }
                                                        }

                                                        suse.AddTarget(target.UID, tar, attack);
                                                        ReceiveAttack(attacker, target, attack, tar.Damage, spell);
                                                        i++;
                                                        if (i > num) break;
                                                    }
                                                }
                                            }
                                        }

                                        if (attacker.EntityFlag == EntityFlag.Player)
                                            attacker.Owner.SendScreen(suse, true);
                                    }
                                    break;
                                #endregion
                                #region BladeTempest
                                case 11110:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {

                                            PrepareSpell(spell, attacker.Owner);
                                            SpellUse suse = new SpellUse(true);
                                            suse.Attacker = attacker.UID;
                                            suse.SpellID = spell.ID;
                                            suse.SpellLevel = spell.Level;
                                            ushort _X = attacker.X, _Y = attacker.Y;
                                            ushort _tX = X, _tY = Y;
                                            byte dist = (byte)spell.Distance;
                                            var Array = attacker.Owner.Screen.Objects;
                                            InLineAlgorithm algo = new InLineAlgorithm(attacker.X, X, attacker.Y, Y, dist,
                                                                               InLineAlgorithm.Algorithm.DDA);
                                            X = attacker.X;
                                            Y = attacker.Y;
                                            int i = 0;
                                            for (i = 0; i < algo.lcoords.Count; i++)
                                            {
                                                if (attacker.Owner.Map.Floor[algo.lcoords[i].X, algo.lcoords[i].Y, MapObjectType.Player]
                                                    && !attacker.ThroughGate(algo.lcoords[i].X, algo.lcoords[i].Y))
                                                {
                                                    X = (ushort)algo.lcoords[i].X;
                                                    Y = (ushort)algo.lcoords[i].Y;
                                                }
                                                else
                                                {
                                                    break;
                                                }
                                            }
                                            suse.X = X;
                                            suse.Y = Y;
                                            if (!attacker.Owner.Map.Floor[X, Y, MapObjectType.Player, null])
                                                return;
                                            double disth = 1.5;
                                            if (attacker.MapID == DeathMatch.MAPID) disth = 1;
                                            foreach (Interfaces.IMapObject _obj in Array)
                                            {
                                                bool hit = false;
                                                for (int j = 0; j < i; j++)
                                                    if (Kernel.GetDDistance(_obj.X, _obj.Y, (ushort)algo.lcoords[j].X, (ushort)algo.lcoords[j].Y) <= disth)
                                                        hit = true;
                                                if (hit)
                                                {
                                                    if (_obj.MapObjType == MapObjectType.Monster)
                                                    {
                                                        attacked = _obj as Entity;
                                                        if (CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                                        {
                                                            var damage = Game.Attacking.Calculate.Melee(attacker, attacked, spell, ref attack);

                                                            ReceiveAttack(attacker, attacked, attack, damage, spell);

                                                            suse.AddTarget(attacked.UID, damage, attack);
                                                        }
                                                    }
                                                    else if (_obj.MapObjType == MapObjectType.Player)
                                                    {
                                                        attacked = _obj as Entity;
                                                        if (CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                                        {
                                                            var damage = Game.Attacking.Calculate.Melee(attacker, attacked, spell, ref attack);
                                                            if (attacked.EntityFlag == EntityFlag.Player)
                                                                damage = (damage * 30) / 20;
                                                            ReceiveAttack(attacker, attacked, attack, damage, spell);

                                                            suse.AddTarget(attacked.UID, damage, attack);
                                                        }
                                                    }
                                                    else if (_obj.MapObjType == MapObjectType.SobNpc)
                                                    {
                                                        attackedsob = _obj as SobNpcSpawn;
                                                        if (CanAttack(attacker, attackedsob, spell))
                                                        {
                                                            var damage = Game.Attacking.Calculate.Melee(attacker, attackedsob, ref attack);

                                                            ReceiveAttack(attacker, attackedsob, attack, damage, spell);

                                                            suse.AddTarget(attackedsob.UID, damage, attack);
                                                        }
                                                    }
                                                }
                                            }
                                            attacker.PX = attacker.X;
                                            attacker.PY = attacker.Y;
                                            attacker.X = X;
                                            attacker.Y = Y;
                                            attacker.Owner.SendScreen(suse, true);
                                            attacker.Owner.Screen.Reload(suse);
                                        }
                                        break;
                                    }
                                #endregion
                                #region MagicDefender
                                case 11200:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            PrepareSpell(spell, attacker.Owner);
                                            SpellUse spellUse = new SpellUse(true);
                                            spellUse.Attacker = attacker.UID;
                                            spellUse.SpellID = spell.ID;
                                            spellUse.SpellLevel = spell.Level;
                                            spellUse.X = X;
                                            spellUse.Y = Y;
                                            if (attacker.Owner.Team != null)
                                            {
                                                foreach (var mate in attacker.Owner.Team.Teammates)
                                                {
                                                    if (Kernel.GetDistance(attacker.X, attacker.Y, mate.Entity.X, mate.Entity.Y) <= 4)
                                                    {
                                                        spellUse.AddTarget(mate.Entity.UID, 0, attack);
                                                        if (attacker.UID == mate.Entity.UID)
                                                            mate.Entity.MagicDefenderOwner = true;
                                                        mate.Entity.HasMagicDefender = true;
                                                        mate.Entity.MagicDefenderSecs = (byte)spell.Duration;
                                                        attacker.RemoveFlag3(Conquer_Online_Server.Network.GamePackets.Update.Flags3.MagicDefender);
                                                        mate.Entity.AddFlag3(Conquer_Online_Server.Network.GamePackets.Update.Flags3.MagicDefender);
                                                        mate.Entity.Update(mate.Entity.StatusFlag, mate.Entity.StatusFlag2, mate.Entity.StatusFlag3, Update.MagicDefenderIcone, 0x80, mate.Entity.MagicDefenderSecs, 0, false);
                                                        mate.Entity.MagicDefenderStamp = Time32.Now;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                spellUse.AddTarget(attacker.UID, 0, null);
                                                attacker.Owner.Entity.HasMagicDefender = true;
                                                attacker.Owner.Entity.MagicDefenderOwner = true;
                                                attacker.Owner.Entity.MagicDefenderSecs = (byte)spell.Duration;
                                                attacker.RemoveFlag3(Conquer_Online_Server.Network.GamePackets.Update.Flags3.MagicDefender);
                                                attacker.AddFlag3(Conquer_Online_Server.Network.GamePackets.Update.Flags3.MagicDefender);
                                                attacker.Owner.Entity.Update(attacker.Owner.Entity.StatusFlag, attacker.Owner.Entity.StatusFlag2, attacker.Owner.Entity.StatusFlag3, Update.MagicDefenderIcone, 0x80, attacker.Owner.Entity.MagicDefenderSecs, 0, false);
                                                attacker.Owner.Entity.MagicDefenderStamp = Time32.Now;
                                                if (attacker.EntityFlag == EntityFlag.Player)
                                                {

                                                    attacker.Owner.SendScreen(spellUse, true);
                                                    Conquer_Online_Server.Network.GamePackets.SyncPacket packet = new Conquer_Online_Server.Network.GamePackets.SyncPacket
                                                    {
                                                        Identifier = attacker.UID,
                                                        Count = 2,
                                                        Type = Conquer_Online_Server.Network.GamePackets.SyncPacket.SyncType.StatusFlag,
                                                        StatusFlag1 = (ulong)attacker.StatusFlag,
                                                        StatusFlag2 = (ulong)attacker.StatusFlag2,
                                                        Unknown1 = 0x31,
                                                        StatusFlagOffset = 0x80,
                                                        Time = (uint)spell.Duration,
                                                        Value = 10,
                                                        Level = spell.Level
                                                    };
                                                }
                                            }
                                            attacker.Owner.SendScreen(spellUse, true);
                                        }
                                        break;
                                    }
                                #endregion
                                #region DefensiveStance
                                case 11160:
                                    {
                                        if (Time32.Now >= attacker.DefensiveStanceStamp.AddSeconds(10))
                                        {
                                            if (CanUseSpell(spell, attacker.Owner))
                                            {
                                                PrepareSpell(spell, attacker.Owner);
                                                SpellUse spellUse = new SpellUse(true);
                                                spellUse.Attacker = attacker.UID;
                                                spellUse.SpellID = spell.ID;
                                                spellUse.SpellLevel = spell.Level;
                                                spellUse.X = X;
                                                spellUse.Y = Y;
                                                attacker.Owner.SendScreen(spellUse, true);
                                                if (attacker.IsDefensiveStance)
                                                {
                                                    attacker.RemoveFlag2(Conquer_Online_Server.Network.GamePackets.Update.Flags2.Fatigue);
                                                    attacker.IsDefensiveStance = false;
                                                }
                                                else
                                                {
                                                    attacker.FatigueSecs = spell.Duration;
                                                    attacker.FatigueStamp = Time32.Now;
                                                    attacker.AddFlag2(Conquer_Online_Server.Network.GamePackets.Update.Flags2.Fatigue);
                                                    attacker.Update(attacker.Owner.Entity.StatusFlag, attacker.Owner.Entity.StatusFlag2, attacker.Owner.Entity.StatusFlag3, Update.DefensiveStance, 0x7E, (uint)spell.Duration, (uint)(spell.Level + 1), false);
                                                    attacker.IsDefensiveStance = true;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            attacker.Owner.Send(new Message("You need to wait 10 seconds in order to cast the spell again!", Color.Red, Message.TopLeft));
                                        }
                                        break;
                                    }
                                #endregion
                                #endregion

                                #region Assassin Skills

                                #region PathOfShadow
                                case 11620:
                                    {
                                        var weps = attacker.Owner.Weapons;
                                        if ((weps.Item1 != null && weps.Item1.ID / 1000 != 613) || (weps.Item2 != null && weps.Item2.ID / 1000 != 613))
                                        {
                                            attacker.Owner.Send(new Message("You need to wear only knifes!", Color.Red, Message.Talk));
                                            return;
                                        }
                                        SpellUse spellUse = new SpellUse(true);
                                        spellUse.Attacker = attacker.UID;
                                        spellUse.SpellID = spell.ID;
                                        spellUse.SpellLevel = spell.Level;
                                        spellUse.X = X;
                                        spellUse.Y = Y;
                                        attacker.Owner.SendScreen(spellUse, true);
                                        // attacker.AssassinColor = attacker.BattlePower;
                                        if (attacker.ContainsFlag3(Update.Flags3.Assassin))
                                        {
                                            attacker.RemoveFlag3(Update.Flags3.Assassin);
                                            if (attacker.ContainsFlag3(Update.Flags3.BladeFlurry))
                                                attacker.RemoveFlag3(Update.Flags3.BladeFlurry);
                                            if (attacker.ContainsFlag3(Update.Flags3.KineticSpark))
                                                attacker.RemoveFlag3(Update.Flags3.KineticSpark);
                                        }
                                        else
                                            attacker.AddFlag3(Update.Flags3.Assassin);
                                        break;
                                    }
                                #endregion
                                #region Blade Furry
                                case 11610:
                                    {
                                        if (!attacker.ContainsFlag3(Update.Flags3.Assassin))
                                            return;
                                        SpellUse suse = new SpellUse(true);
                                        suse.Attacker = attacker.UID;
                                        suse.SpellID = spell.ID;
                                        suse.SpellLevel = spell.Level;
                                        suse.X = X;
                                        suse.Y = Y;
                                        if (attacker.ContainsFlag(Update.Flags.XPList))
                                        {
                                            attacker.AddFlag3(Update.Flags3.BladeFlurry);
                                            attacker.BladeFlurryStamp = Time32.Now;
                                            attacker.RemoveFlag(Update.Flags.XPList);
                                        }
                                        else
                                        {
                                            if (Kernel.GetDistance(attacker.X, attacker.Y, X, Y) <= spell.Range)
                                            {
                                                foreach (Interfaces.IMapObject _obj in attacker.Owner.Screen.Objects)
                                                {
                                                    if (_obj == null) continue;
                                                    if (_obj.MapObjType == MapObjectType.Monster || _obj.MapObjType == MapObjectType.Player)
                                                    {
                                                        attacked = _obj as Entity;
                                                        if (Kernel.GetDistance(attacker.X, attacker.Y, attacked.X, attacked.Y) <= spell.Range)
                                                        {
                                                            if (CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                                            {
                                                                attack.Effect1 = Attack.AttackEffects1.None;
                                                                uint damage = Game.Attacking.Calculate.Melee(attacker, attacked, spell, ref attack);
                                                                suse.Effect1 = attack.Effect1;
                                                                if (attacker.SpiritFocus)
                                                                {
                                                                    damage = (uint)(damage * attacker.SpiritFocusPercent);
                                                                    attacker.SpiritFocus = false;
                                                                }
                                                                ReceiveAttack(attacker, attacked, attack, damage / 2, spell);
                                                                suse.AddTarget(attacked.UID, damage, attack);
                                                            }
                                                        }
                                                    }
                                                    else if (_obj.MapObjType == MapObjectType.SobNpc)
                                                    {
                                                        attackedsob = _obj as SobNpcSpawn;
                                                        if (Kernel.GetDistance(attacker.X, attacker.Y, attackedsob.X, attackedsob.Y) <= spell.Range)
                                                        {
                                                            if (CanAttack(attacker, attackedsob, spell))
                                                            {
                                                                attack.Effect1 = Attack.AttackEffects1.None;
                                                                uint damage = Game.Attacking.Calculate.Melee(attacker, attackedsob, ref attack);
                                                                damage /= 100;
                                                                //damage = damage - (uint)(damage * spell.Percent);
                                                                suse.Effect1 = attack.Effect1;
                                                                if (attacker.SpiritFocus)
                                                                {
                                                                    damage = (uint)(damage * attacker.SpiritFocusPercent);
                                                                    attacker.SpiritFocus = false;
                                                                }
                                                                ReceiveAttack(attacker, attackedsob, attack, damage, spell);
                                                                suse.AddTarget(attackedsob.UID, damage, attack);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        attacker.Owner.SendScreen(suse, true);
                                        break;
                                    }
                                #endregion
                                #region Mortal Wound
                                case 11660:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            if (!attacker.ContainsFlag3(Update.Flags3.Assassin))
                                                return;
                                            if (Time32.Now > attacker.MortalWoundStamp.AddMilliseconds(900))
                                            {
                                                attacker.MortalWoundStamp = Time32.Now;
                                                PrepareSpell(spell, attacker.Owner);

                                                SpellUse suse = new SpellUse(true);
                                                suse.Attacker = attacker.UID;
                                                suse.SpellID = spell.ID;

                                                suse.SpellLevel = spell.Level;
                                                suse.X = X;
                                                suse.Y = Y;
                                                if (attackedsob == null)
                                                {

                                                    if (CanAttack(attacker, attacked, spell, false))
                                                    {
                                                        attack.Effect1 = Attack.AttackEffects1.None;
                                                        uint damage = Game.Attacking.Calculate.Melee(attacker, attacked, spell, ref attack);
                                                        damage = (damage * 20) / 100;
                                                        if (attacker.SpiritFocus)
                                                        {
                                                            damage = (uint)(damage * attacker.SpiritFocusPercent);
                                                            attacker.SpiritFocus = false;
                                                        }
                                                        if (attacked.EntityFlag == EntityFlag.Monster)
                                                        {
                                                            if (!(attacked.Name.Contains("TeratoDragon") || attacked.Name.Contains("SnowBanshee") || attacked.Name.Contains("Pheasant") || attacked.Name.Contains("Guard") || attacked.Name.Contains("ThrillingSpook")))
                                                                damage = (damage * 10);
                                                        }
                                                        else
                                                        {
                                                            if (Kernel.GetDistance(attacker.X, attacker.Y, attacked.X, attacked.Y) <= 5)
                                                                damage = (damage * 10) / 100;

                                                        }
                                                        suse.Effect1 = attack.Effect1;
                                                        ReceiveAttack(attacker, attacked, attack, damage / 2, spell);
                                                        suse.AddTarget(attacked.UID, damage, attack);
                                                        attacker.Owner.SendScreen(suse, true);
                                                    }
                                                }
                                                else
                                                {
                                                    if (CanAttack(attacker, attackedsob, spell))
                                                    {
                                                        attack.Effect1 = Attack.AttackEffects1.None;
                                                        uint damage = Game.Attacking.Calculate.Melee(attacker, attackedsob, ref attack) / 20;
                                                        damage = (uint)(damage * spell.PowerPercent);
                                                        if (attacker.SpiritFocus)
                                                        {
                                                            damage = (uint)(damage * attacker.SpiritFocusPercent);
                                                            attacker.SpiritFocus = false;
                                                        }
                                                        suse.Effect1 = attack.Effect1;

                                                        ReceiveAttack(attacker, attackedsob, attack, damage, spell);
                                                        suse.AddTarget(attackedsob.UID, damage, attack);
                                                        attacker.Owner.SendScreen(suse, true);
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    }
                                #endregion
                                #region Blistering Wave
                                case 11650:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            if (!attacker.ContainsFlag3(Update.Flags3.Assassin))
                                                return;
                                            PrepareSpell(spell, attacker.Owner);

                                            SpellUse suse = new SpellUse(true);
                                            suse.Attacker = attacker.UID;
                                            suse.SpellID = spell.ID;

                                            suse.SpellLevel = spell.Level;
                                            suse.X = X;
                                            suse.Y = Y;
                                            Sector sector = new Sector(attacker.X, attacker.Y, X, Y);
                                            sector.Arrange(spell.Sector, spell.Distance);
                                            foreach (Interfaces.IMapObject _obj in attacker.Owner.Screen.Objects)
                                            {
                                                if (_obj == null)
                                                    continue;
                                                if (_obj.MapObjType == MapObjectType.Monster || _obj.MapObjType == MapObjectType.Player)
                                                {
                                                    attacked = _obj as Entity;

                                                    if (sector.Inside(attacked.X, attacked.Y))
                                                    {
                                                        if (CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                                        {
                                                            attack.Effect1 = Attack.AttackEffects1.None;
                                                            //Attack.AttackEffects1 s = Attack.AttackEffects1.None;
                                                            uint damage = Game.Attacking.Calculate.Melee(attacker, attacked, spell, ref attack);
                                                            damage = (damage * 80) / 100;
                                                            suse.Effect1 = attack.Effect1;
                                                            if (attacker.SpiritFocus)
                                                            {
                                                                damage = (uint)(damage * attacker.SpiritFocusPercent);
                                                                attacker.SpiritFocus = false;
                                                            }

                                                            if (attacked.EntityFlag == EntityFlag.Monster)
                                                            {
                                                                if (!(attacked.Name.Contains("TeratoDragon") || attacked.Name.Contains("SnowBanshee") || attacked.Name.Contains("BabyiFranko") || attacked.Name.Contains("Guard") || attacked.Name.Contains("ThrillingSpook")))
                                                                    damage = (damage * 5);
                                                            }
                                                            else
                                                            {
                                                                if (Kernel.GetDistance(attacker.X, attacker.Y, attacked.X, attacked.Y) <= 5)
                                                                    damage = (damage * 20) / 100;
                                                                else { continue; }
                                                            }


                                                            ReceiveAttack(attacker, attacked, attack, damage, spell);
                                                            suse.AddTarget(attacked.UID, damage, attack);
                                                        }
                                                    }
                                                }
                                                else if (_obj.MapObjType == MapObjectType.SobNpc)
                                                {
                                                    attackedsob = _obj as SobNpcSpawn;

                                                    if (sector.Inside(attackedsob.X, attackedsob.Y))
                                                    {
                                                        if (CanAttack(attacker, attackedsob, spell))
                                                        {
                                                            attack.Effect1 = Attack.AttackEffects1.None;
                                                            //Attack.AttackEffects1 s = Attack.AttackEffects1.None;
                                                            uint damage = Game.Attacking.Calculate.Melee(attacker, attackedsob, ref attack);
                                                            damage = (damage * 30) / 100;
                                                            ////if (Kernel.Rate((attacker.CriticalStrike) / 100)) { uint calc = (uint)(damage * 1.5F); attack.Damage = (uint)(damage * 1.5F); damage = calc; attack.Effect1 = YuriProject.Network.GamePackets.Attack.AttackEffects1.CriticalStrike; }
                                                            damage = (uint)(damage * spell.PowerPercent);
                                                            ReceiveAttack(attacker, attackedsob, attack, damage, spell);
                                                            suse.Effect1 = attack.Effect1;
                                                            suse.Targets.Add(attackedsob.UID, damage);
                                                        }
                                                    }
                                                }
                                            }
                                            attacker.Owner.SendScreen(suse, true);
                                        }
                                        break;
                                    }
                                #endregion
                                #region SpiritFocus
                                case 11670:
                                    this.attacker.IntensifyStamp = Time32.Now;
                                    this.attacker.OnIntensify = true;
                                    Attack attack4 = new Attack(true)
                                    {
                                        Attacker = this.attacker.UID,
                                        AttackType = 0x18,
                                        X = X,
                                        Y = Y
                                    };
                                    Writer.WriteUInt16(0, 4, attack4.ToArray());
                                    Writer.WriteUInt16(attack.X, 16 + 4, attack4.ToArray());
                                    Writer.WriteUInt16(attack.Y, 18 + 4, attack4.ToArray());
                                    Writer.WriteUInt16(spell.ID, 24 + 4, attack4.ToArray());
                                    Writer.WriteByte(spell.Level, 26 + 4, attack4.ToArray());
                                    this.attacker.Owner.SendScreen(attack4, true);
                                    this.attacker.X = X;
                                    this.attacker.Y = Y;
                                    this.attacker.SendSpawn(this.attacker.Owner);
                                    break;
                                #endregion
                                #region Kinetic Spark
                                case 11590:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            if (!attacker.ContainsFlag3(Update.Flags3.Assassin))
                                                return;
                                            PrepareSpell(spell, attacker.Owner);

                                            SpellUse spellUse = new SpellUse(true);
                                            spellUse.Attacker = attacker.UID;
                                            spellUse.SpellID = spell.ID;
                                            spellUse.SpellLevel = spell.Level;
                                            spellUse.X = X;
                                            spellUse.Y = Y;
                                            attacker.Owner.SendScreen(spellUse, true);

                                            if (attacker.ContainsFlag3(Update.Flags3.KineticSpark))
                                                attacker.RemoveFlag3(Update.Flags3.KineticSpark);
                                            else
                                                attacker.AddFlag3(Update.Flags3.KineticSpark);
                                        }
                                        break;
                                    }
                                #endregion
                                #region Dagger Storm
                                case 11600:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            if (!attacker.ContainsFlag3(Update.Flags3.Assassin))
                                                return;

                                            var map = attacker.Owner.Map;
                                            if (!map.Floor[X, Y, MapObjectType.Item, null]) return;
                                            PrepareSpell(spell, attacker.Owner);

                                            SpellUse suse = new SpellUse(true);
                                            suse.Attacker = attacker.UID;
                                            suse.Attacker1 = attacker.UID;
                                            suse.SpellID = spell.ID;
                                            suse.SpellLevel = spell.Level;
                                            suse.AddTarget(attacker.UID, 0, null);
                                            attacker.Owner.SendScreen(suse, true);

                                            FloorItem floorItem = new FloorItem(true);
                                            if (attacker.Owner.Spells[spellID].LevelHu2 == 1)
                                                floorItem.ItemID = FloorItem.Effect;
                                            else if (attacker.Owner.Spells[spellID].LevelHu2 == 2)
                                                floorItem.ItemID = FloorItem.Effect;
                                            else
                                                floorItem.ItemID = FloorItem.DaggerStorm;
                                            floorItem.Type = FloorItem.Effect;
                                            floorItem.X = X;
                                            floorItem.Y = Y;
                                            floorItem.OnFloor = Time32.Now;
                                            floorItem.Owner = attacker.Owner;
                                            while (map.Npcs.ContainsKey(floorItem.UID))
                                                floorItem.UID = Network.GamePackets.FloorItem.FloorUID.Next;
                                            map.AddFloorItem(floorItem);
                                            attacker.Owner.SendScreenSpawn(floorItem, true);
                                        }
                                        break;
                                    }
                                #endregion

                                #endregion
                                #region ChainBolt
                                case 10309:
                                    {
                                        SpellUse suse = new SpellUse(true);
                                        suse.Attacker = attacker.UID;
                                        suse.SpellID = spell.ID;
                                        suse.SpellLevel = spell.Level;
                                        suse.X = attacker.X;
                                        suse.Y = attacker.Y;
                                        if (attacked != null)
                                        {
                                            if (attacker.ContainsFlag2(Update.Flags2.ChainBoltActive))
                                            {
                                                PrepareSpell(spell, attacker.Owner);
                                                suse.X = X;
                                                suse.Y = Y;
                                                int maxR = spell.Distance;
                                                if (attacked != null)
                                                {
                                                    if (Kernel.GetDistance(attacker.X, attacker.Y, attacked.X, attacked.Y) <= maxR)
                                                    {
                                                        if (CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                                        {
                                                            var damage = Game.Attacking.Calculate.Magic(attacker, attacked, spell, ref attack);
                                                            damage -= (uint)(damage * .3);
                                                            ReceiveAttack(attacker, attacked, attack, damage, spell);
                                                            suse.AddTarget(attacked.UID, damage, attack);
                                                        }
                                                    }
                                                }
                                                var Array = attacker.Owner.Screen.Objects;
                                                var closestTarget = findClosestTarget(attacked, attacked.X, attacked.Y, Array);
                                                ushort x = closestTarget.X, y = closestTarget.Y;
                                                int targets = Math.Max((int)spell.Level, 1);

                                                foreach (Interfaces.IMapObject _obj in Array)
                                                {
                                                    if (targets == 0) continue;
                                                    if (_obj.MapObjType == MapObjectType.Monster || _obj.MapObjType == MapObjectType.Player)
                                                    {
                                                        attacked = _obj as Entity;

                                                        if (Kernel.GetDistance(x, y, attacked.X, attacked.Y) <= maxR)
                                                        {
                                                            if (CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                                            {
                                                                maxR = 6;

                                                                var damage2 = Game.Attacking.Calculate.Magic(attacker, attacked, spell, ref attack);
                                                                damage2 -= (uint)(damage2 * .3);
                                                                ReceiveAttack(attacker, attacked, attack, damage2, spell);

                                                                suse.AddTarget(attacked.UID, damage2, attack);
                                                                x = attacked.X;
                                                                y = attacked.Y;
                                                                targets--;
                                                            }
                                                        }
                                                    }
                                                }
                                                if (suse.Targets.Count == 0) return;
                                                attacker.Owner.SendScreen(suse, true);
                                            }
                                            else
                                            {
                                                if (CanUseSpell(spell, attacker.Owner))
                                                {
                                                    PrepareSpell(spell, attacker.Owner);
                                                    attacker.ChainboltStamp = Time32.Now;
                                                    attacker.ChainboltTime = spell.Duration;
                                                    attacker.AddFlag2(Update.Flags2.ChainBoltActive);
                                                }
                                            }
                                        }
                                        break;
                                    }
                                #endregion
                                #region Booth skill
                                case 10424:
                                    {

                                        if (attacker.Owner.Booth == null)
                                        {
                                            Data data = new Data(true);
                                            data.ID = 111;
                                            data.UID = attacker.UID;
                                            data.TimeStamp = Time32.Now;
                                            data.dwParam = 40079;
                                            //data.boo = 1;
                                            data.wParam1 = attacker.X;
                                            data.wParam2 = attacker.Y;
                                            attacker.Owner.Send(data);
                                            //Data gData = new Data(false);
                                            attacker.Owner.Booth = new Conquer_Online_Server.Game.ConquerStructures.Booth(attacker.Owner, data);
                                            Data data4 = new Data(true)
                                            {
                                                ID = 0x51,
                                                UID = attacker.UID,
                                                dwParam = 0
                                            };
                                            attacker.Owner.Send(data4);
                                            this.attacker.Owner.Send(new Message("You have enabled the Magic Defender", System.Drawing.Color.Aqua, 0x7d0));
                                            break;
                                        }
                                        else if (attacker.Owner.Booth != null)
                                        {
                                            attacker.TransformationID = 0;
                                            attacker.Owner.Booth.Remove();
                                            attacker.Owner.Booth = null;
                                            attacker.Owner.Entity.Action = Enums.ConquerAction.Jump;
                                        }
                                        attacker.Owner.Screen.FullWipe();
                                        attacker.Owner.Screen.Reload(null);

                                        Conquer_Online_Server.Game.Weather.CurrentWeatherBase.Send(attacker.Owner);



                                    }
                                    break;

                                #endregion
                                #region Lee-Long Fixed Mr.Bahaa
                                #region DragonSwing
                                case 12200:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            PrepareSpell(spell, attacker.Owner);
                                            SpellUse suse = new SpellUse(true);
                                            suse.Attacker = attacker.UID;
                                            suse.SpellID = spell.ID;
                                            suse.SpellLevel = spell.Level;
                                            ushort _X = attacker.X, _Y = attacker.Y;
                                            ushort _tX = X, _tY = Y;
                                            byte dist = (byte)spell.Distance;
                                            var Array = attacker.Owner.Screen.Objects;
                                            InLineAlgorithm algo = new InLineAlgorithm(attacker.X, X, attacker.Y, Y, dist,
                                                                               InLineAlgorithm.Algorithm.DDA);
                                            X = attacker.X;
                                            Y = attacker.Y;
                                            int i = 0;
                                            for (i = 0; i < algo.lcoords.Count; i++)
                                            {
                                                if (attacker.Owner.Map.Floor[algo.lcoords[i].X, algo.lcoords[i].Y, MapObjectType.Player]
                                                    && !attacker.ThroughGate(algo.lcoords[i].X, algo.lcoords[i].Y))
                                                {
                                                    X = (ushort)algo.lcoords[i].X;
                                                    Y = (ushort)algo.lcoords[i].Y;
                                                }
                                                else
                                                {
                                                    break;
                                                }
                                            }
                                            suse.X = X;
                                            suse.Y = Y;
                                            if (!attacker.Owner.Map.Floor[X, Y, MapObjectType.Player, null])
                                                return;
                                            double disth = 1.5;
                                            if (attacker.MapID == 5678) disth = 1;
                                            if (attacker.MapID == 3045) disth = 1;
                                            foreach (Interfaces.IMapObject _obj in Array)
                                            {
                                                bool hit = false;
                                                for (int j = 0; j < i; j++)
                                                    if (Kernel.GetDDistance(_obj.X, _obj.Y, (ushort)algo.lcoords[j].X, (ushort)algo.lcoords[j].Y) <= disth)
                                                        hit = true;
                                                if (hit)
                                                {
                                                    if (_obj.MapObjType == MapObjectType.Monster)
                                                    {
                                                        attacked = _obj as Entity;
                                                        if (CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                                        {
                                                            var damage = Game.Attacking.Calculate.Melee(attacker, attacked, spell, ref attack);

                                                            ReceiveAttack(attacker, attacked, attack, damage, spell);

                                                            suse.AddTarget(attacked.UID, damage, attack);
                                                        }
                                                    }
                                                    else if (_obj.MapObjType == MapObjectType.Player)
                                                    {
                                                        attacked = _obj as Entity;
                                                        if (CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                                        {
                                                            uint damage = Game.Attacking.Calculate.Melee(attacker, attacked, spell, ref attack);
                                                            damage = (uint)(damage * Program.DragonSwing);
                                                            ReceiveAttack(attacker, attacked, attack, damage, spell);

                                                            suse.AddTarget(attacked.UID, damage, attack);
                                                        }
                                                    }
                                                    else if (_obj.MapObjType == MapObjectType.SobNpc)
                                                    {
                                                        attackedsob = _obj as SobNpcSpawn;
                                                        if (CanAttack(attacker, attackedsob, spell))
                                                        {
                                                            var damage = Game.Attacking.Calculate.Melee(attacker, attackedsob, ref attack);

                                                            ReceiveAttack(attacker, attackedsob, attack, damage, spell);

                                                            suse.AddTarget(attackedsob.UID, damage, attack);
                                                        }
                                                    }
                                                }
                                            }
                                            attacker.PX = attacker.X;
                                            attacker.PY = attacker.Y;
                                            attacker.X = X;
                                            attacker.Y = Y;
                                            attacker.Owner.SendScreen(suse, true);
                                            attacker.Owner.Screen.Reload(suse);
                                        }
                                        break;
                                    }
                                #endregion
                                #region DragonSlash
                                case 12350:
                                    {
                                        var weps = attacker.Owner.Weapons;
                                        if ((weps.Item1 != null && weps.Item1.ID / 1000 != 617) || (weps.Item2 != null && weps.Item2.ID / 1000 != 617))
                                        {
                                            attacker.Owner.Send(new Message("You need to wear only Two Nunchaku! [DragonWarroir-Weapone]", Color.Red, Message.Talk));
                                            return;
                                        }
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            PrepareSpell(spell, attacker.Owner);
                                            Game.Attacking.InLineAlgorithm ila = new Conquer_Online_Server.Game.Attacking.InLineAlgorithm(attacker.X,
                                        X, attacker.Y, Y, (byte)spell.Range, InLineAlgorithm.Algorithm.DDA);
                                            SpellUse suse = new SpellUse(true);
                                            suse.Attacker = attacker.UID;
                                            suse.SpellID = SpellID;

                                            suse.SpellLevel = attacker.Owner.Spells[SpellID].Level;
                                            suse.X = X;
                                            suse.Y = Y;
                                            foreach (Interfaces.IMapObject _obj in attacker.Owner.Screen.Objects)
                                            {
                                                if (_obj == null)
                                                    continue;
                                                if (_obj.MapObjType == MapObjectType.Monster || _obj.MapObjType == MapObjectType.Player)
                                                {
                                                    attacked = _obj as Entity;
                                                    if (ila.InLine(attacked.X, attacked.Y))
                                                    {
                                                        if (!CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                                            continue;

                                                        attack.Effect1 = Attack.AttackEffects1.None;
                                                        uint damage = Game.Attacking.Calculate.Melee(attacker, attacked, spell, ref attack);
                                                        damage = (uint)(damage * 3);
                                                        suse.Effect1 = attack.Effect1;

                                                        attack.Damage = damage;

                                                        ReceiveAttack(attacker, attacked, attack, damage, spell);

                                                        suse.AddTarget(attacked.UID, damage, attack);
                                                    }
                                                }
                                                else if (_obj.MapObjType == MapObjectType.SobNpc)
                                                {
                                                    attackedsob = _obj as SobNpcSpawn;

                                                    if (ila.InLine(attackedsob.X, attackedsob.Y))
                                                    {
                                                        if (!CanAttack(attacker, attackedsob, spell))
                                                            continue;

                                                        attack.Effect1 = Attack.AttackEffects1.None;
                                                        uint damage = Game.Attacking.Calculate.Melee(attacker, attackedsob, ref attack);
                                                        damage = (damage * 3) / 100;
                                                        damage = (uint)(damage * spell.PowerPercent);
                                                        attack.Damage = damage;

                                                        ReceiveAttack(attacker, attackedsob, attack, damage, spell);

                                                        suse.AddTarget(attackedsob.UID, damage, attack);
                                                    }
                                                }
                                            }
                                            attacker.Owner.SendScreen(suse, true);
                                        }
                                        break;
                                    }
                                #endregion
                                #region DragonFlow
                                case 12270:
                                    {
                                        SpellUse spellUse = new SpellUse(true);
                                        spellUse.Attacker = attacker.UID;
                                        spellUse.SpellID = spell.ID;
                                        spellUse.SpellLevel = spell.Level;
                                        spellUse.X = X;
                                        spellUse.Y = Y;

                                        if (attacker.ContainsFlag3(Update.Flags3.DragonFLow))
                                        {
                                            attacker.AssassinBP = (uint)attacker.BattlePower;
                                            attacker.RemoveFlag3(Update.Flags3.DragonFLow);
                                            attacker.DragonFlowVAlue = (byte)spell.Power;
                                            attacker.ISonDragonStamp = Time32.Now;
                                        }
                                        else
                                        {
                                            attacker.Owner.SendScreen(spellUse, true);
                                            attacker.AddFlag3(Update.Flags3.DragonFLow);
                                        }
                                        break;
                                    }

                                #endregion
                                #region StormKick
                                case 12140:
                                    {
                                        UInt16 ox, oy;
                                        ox = attacker.X;
                                        oy = attacker.Y;
                                        PrepareSpell(spell, attacker.Owner);

                                        SpellUse suse = new SpellUse(true);
                                        suse.Attacker = attacker.UID;
                                        suse.SpellID = spell.ID;
                                        suse.SpellLevel = spell.Level;
                                        //    suse.SoulLevel = client_Spell.SoulLevel; 
                                        suse.X = X;
                                        suse.Y = Y;
                                        RangeMove moveIn = new RangeMove();
                                        List<RangeMove.Coords> ranger = moveIn.MoveCoords(attacker.X, attacker.Y, X, Y);
                                        attacker.X = X;
                                        attacker.Y = Y;
                                        Attack shift = new Attack(true);
                                        shift.Attacker = attacker.UID;
                                        shift.X = X;
                                        shift.Y = Y;
                                        shift.AttackType = Attack.SkillMove;
                                        shift.dwParam = 12290;//10315; 
                                        shift.KOCount = spell.Level;
                                        attacker.Shift(X, Y, attacker.MapID, shift);
                                        attacker.Owner.SendScreen(suse, true);
                                        if (CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                        {
                                            var damage = Game.Attacking.Calculate.Melee(attacker, attacked, spell, ref attack);
                                            damage = (uint)(damage * 10);
                                            ReceiveAttack(attacker, attacked, attack, damage, spell);

                                            suse.AddTarget(attacked.UID, damage, attack);
                                        }
                                        else
                                        {
                                            if (attackedsob != null)
                                            {
                                                if (CanAttack(attacker, attackedsob, spell))
                                                {

                                                    var damage = Game.Attacking.Calculate.Melee(attacker, attacked, spell, ref attack);
                                                    damage = (uint)(damage * 10);
                                                    ReceiveAttack(attacker, attackedsob, attack, damage, spell);

                                                    suse.AddTarget(attackedsob.UID, damage, attack);
                                                }
                                            }
                                        }
                                        attacker.Owner.SendScreen(suse, true);
                                        Conquer_Online_Server.Game.Calculations.IsBreaking(attacker.Owner, ox, oy);
                                        break;
                                    }
                                #endregion
                                #region DragonRoar
                                case 12280:
                                    {

                                        SpellUse spellUse = new SpellUse(true);
                                        spellUse.Attacker = attacker.UID;
                                        spellUse.Attacker1 = attacker.UID;
                                        spellUse.SpellID = spell.ID;
                                        spellUse.SpellLevel = spell.Level;
                                        spellUse.X = X;
                                        spellUse.Y = Y;
                                        spellUse.AddTarget(attacker.UID, 1, attack);
                                        switch (spell.Level)
                                        {
                                            case 0: num = 20; break;
                                            case 1: num = 25; break;
                                            case 2: num = 30; break;
                                            case 3: num = 35; break;
                                            case 4: num = 50; break;
                                            //case 5: num = 60; break; 
                                        }
                                        if (attacker.Owner.Team != null)
                                        {
                                            foreach (Client.GameClient teammate in attacker.Owner.Team.Teammates)
                                            {
                                                if (teammate.Entity.UID != attacker.UID)
                                                {
                                                    if (Kernel.GetDistance(attacker.X, attacker.Y, teammate.Entity.X, teammate.Entity.Y) <= 4)
                                                    {
                                                        teammate.Entity.Stamina += num;
                                                        spellUse.AddTarget(teammate.Entity.UID, 20, null);
                                                    }
                                                }
                                            }
                                        }
                                        attacker.Owner.SendScreen(spellUse, true);
                                        if (attacker.ContainsFlag3((uint)Update.Flags3.DragonRoar))
                                            attacker.RemoveFlag3((uint)Update.Flags3.DragonRoar);
                                        else
                                            attacker.AddFlag3((uint)Update.Flags3.DragonRoar);
                                        break;
                                    }
                                #endregion
                                #region ViolentKick
                                case 12130:
                                    {
                                        UInt16 ox, oy;
                                        ox = attacker.X;
                                        oy = attacker.Y;
                                        PrepareSpell(spell, attacker.Owner);

                                        SpellUse suse = new SpellUse(true);
                                        suse.Attacker = attacker.UID;
                                        suse.SpellID = spell.ID;
                                        suse.SpellLevel = spell.Level;
                                        suse.X = X;
                                        suse.Y = Y;
                                        RangeMove moveIn = new RangeMove();
                                        List<RangeMove.Coords> ranger = moveIn.MoveCoords(attacker.X, attacker.Y, X, Y);
                                        attacker.X = X;
                                        attacker.Y = Y;
                                        Attack shift = new Attack(true);
                                        shift.Attacker = attacker.UID;
                                        shift.X = X;
                                        shift.Y = Y;
                                        shift.AttackType = Attack.SkillMove;
                                        shift.dwParam = 12290;//10315; 
                                        shift.KOCount = spell.Level;
                                        attacker.Shift(X, Y, attacker.MapID, shift);
                                        attacker.Owner.SendScreen(suse, true);
                                        if (CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                        {
                                            var damage = Game.Attacking.Calculate.Melee(attacker, attacked, spell, ref attack);
                                            damage = (uint)(damage * 10);
                                            ReceiveAttack(attacker, attacked, attack, damage, spell);

                                            suse.AddTarget(attacked.UID, damage, attack);
                                        }
                                        else
                                        {
                                            if (attackedsob != null)
                                            {
                                                if (CanAttack(attacker, attackedsob, spell))
                                                {

                                                    var damage = Game.Attacking.Calculate.Melee(attacker, attacked, spell, ref attack);
                                                    damage = (uint)(damage * 10);
                                                    ReceiveAttack(attacker, attackedsob, attack, damage, spell);

                                                    suse.AddTarget(attackedsob.UID, damage, attack);
                                                }
                                            }
                                        }
                                        attacker.Owner.SendScreen(suse, true);
                                        Conquer_Online_Server.Game.Calculations.IsBreaking(attacker.Owner, ox, oy);
                                        break;
                                    }
                                #endregion
                                #region SpeedKick
                                case 12120:
                                    {
                                        UInt16 ox, oy;
                                        ox = attacker.X;
                                        oy = attacker.Y;
                                        PrepareSpell(spell, attacker.Owner);
                                        SpellUse suse = new SpellUse(true);
                                        suse.Attacker = attacker.UID;
                                        suse.SpellID = spell.ID;
                                        suse.SpellLevel = spell.Level;
                                        //    suse.SoulLevel = client_Spell.SoulLevel; 
                                        suse.X = X;
                                        suse.Y = Y;
                                        RangeMove moveIn = new RangeMove();
                                        List<RangeMove.Coords> ranger = moveIn.MoveCoords(attacker.X, attacker.Y, X, Y);
                                        attacker.X = X;
                                        attacker.Y = Y;
                                        Attack shift = new Attack(true);
                                        shift.Attacker = attacker.UID;
                                        shift.X = X;
                                        shift.Y = Y;
                                        shift.AttackType = Attack.SkillMove;
                                        shift.dwParam = 12290;//10315; 
                                        shift.KOCount = spell.Level;
                                        attacker.Shift(X, Y, attacker.MapID, shift);
                                        attacker.Owner.SendScreen(suse, true);
                                        if (CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                        {
                                            var damage = Game.Attacking.Calculate.Melee(attacker, attacked, spell, ref attack);
                                            damage = (uint)(damage * 10);
                                            ReceiveAttack(attacker, attacked, attack, damage, spell);

                                            suse.AddTarget(attacked.UID, damage, attack);
                                        }
                                        else
                                        {
                                            if (attackedsob != null)
                                            {
                                                if (CanAttack(attacker, attackedsob, spell))
                                                {

                                                    var damage = Game.Attacking.Calculate.Melee(attacker, attacked, spell, ref attack);
                                                    damage = (uint)(damage * 10);
                                                    ReceiveAttack(attacker, attackedsob, attack, damage, spell);

                                                    suse.AddTarget(attackedsob.UID, damage, attack);
                                                }
                                            }
                                        }
                                        attacker.Owner.SendScreen(suse, true);
                                        Conquer_Online_Server.Game.Calculations.IsBreaking(attacker.Owner, ox, oy);
                                        break;
                                    }
                                #endregion
                                #region Dragonblood
                                case 12160:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {

                                            PrepareSpell(spell, attacker.Owner);

                                            SpellUse suse = new SpellUse(true);
                                            suse.Attacker = attacker.UID;
                                            suse.SpellID = spell.ID;
                                            suse.SpellLevel = spell.Level;
                                            suse.X = X;
                                            suse.Y = Y;

                                            Conquer_Online_Server.Network.GamePackets.SpellUse.DamageClass tar = new SpellUse.DamageClass();
                                            foreach (var t in attacker.Owner.Screen.Objects)
                                            {
                                                if (t == null)
                                                    continue;
                                                if (t.MapObjType == MapObjectType.Player || t.MapObjType == MapObjectType.Monster)
                                                {
                                                    var target = t as Entity;
                                                    if (Kernel.GetDistance(X, Y, target.X, target.Y) <= spell.Range)
                                                    {
                                                        if (CanAttack(attacker, target, spell, false))
                                                        {
                                                            tar.Damage = Calculate.Melee(attacker, target, spell, ref attack) / 30;
                                                            tar.Hit = true;
                                                            suse.AddTarget(target.UID, tar, attack);
                                                            ReceiveAttack(attacker, target, attack, tar.Damage, spell);
                                                        }
                                                    }
                                                }
                                            }

                                            if (attacker.EntityFlag == EntityFlag.Player)
                                                attacker.Owner.SendScreen(suse, true);
                                        }
                                        break;
                                    }
                                #endregion
                                #region DragonCyclone
                                case 12290:
                                    {
                                        UInt16 ox, oy;
                                        ox = attacker.X;
                                        oy = attacker.Y;
                                        if (attacker.Owner.Entity.ContainsFlag(Conquer_Online_Server.Network.GamePackets.Update.Flags.XPList))
                                        {
                                            attacker.Owner.Entity.RemoveFlag(Conquer_Online_Server.Network.GamePackets.Update.Flags.XPList);
                                            attacker.Owner.Entity.AddFlag3(Conquer_Online_Server.Network.GamePackets.Update.Flags3.DragonCyclone);
                                            attacker.Owner.Entity.DragonCyclone = Time32.Now;
                                            return;
                                        }
                                        PrepareSpell(spell, attacker.Owner);

                                        SpellUse suse = new SpellUse(true);
                                        suse.Attacker = attacker.UID;
                                        suse.SpellID = spell.ID;
                                        suse.SpellLevel = spell.Level;
                                        //    suse.SoulLevel = client_Spell.SoulLevel; 
                                        suse.X = X;
                                        suse.Y = Y;
                                        RangeMove moveIn = new RangeMove();
                                        List<RangeMove.Coords> ranger = moveIn.MoveCoords(attacker.X, attacker.Y, X, Y);
                                        attacker.X = X;
                                        attacker.Y = Y;
                                        Attack shift = new Attack(true);
                                        shift.Attacker = attacker.UID;
                                        shift.X = X;
                                        shift.Y = Y;
                                        shift.AttackType = Attack.SkillMove;
                                        shift.dwParam = 12290;//10315; 
                                        shift.KOCount = spell.Level;
                                        attacker.Shift(X, Y, attacker.MapID, shift);
                                        attacker.Owner.SendScreen(suse, true);


                                        var Array = attacker.Owner.Screen.Objects;
                                        foreach (Interfaces.IMapObject _obj in Array)
                                        {
                                            if (_obj.MapObjType == MapObjectType.Monster || _obj.MapObjType == MapObjectType.Player)
                                            {
                                                attacked = _obj as Entity;
                                                if (!moveIn.InRange(attacked.X, attacked.Y, 4, ranger))
                                                    continue;
                                                if (CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                                {
                                                    var damage = Game.Attacking.Calculate.Magic(attacker, attacked, spell, ref attack);
                                                    damage = (uint)(damage * 5);
                                                    ReceiveAttack(attacker, attacked, attack, damage, spell);

                                                    suse.AddTarget(attacked.UID, damage, attack);
                                                }
                                            }
                                            else if (_obj.MapObjType == MapObjectType.SobNpc)
                                            {
                                                attackedsob = _obj as SobNpcSpawn;

                                                if (CanAttack(attacker, attackedsob, spell))
                                                {
                                                    if (!moveIn.InRange(attackedsob.X, attackedsob.Y, 4, ranger))
                                                        continue;
                                                    var damage = Game.Attacking.Calculate.Magic(attacker, attacked, spell, ref attack);
                                                    damage = (uint)(damage * 5);
                                                    ReceiveAttack(attacker, attackedsob, attack, damage, spell);

                                                    suse.AddTarget(attackedsob.UID, damage, attack);
                                                }
                                            }
                                        }
                                        attacker.Owner.SendScreen(suse, true);
                                        Conquer_Online_Server.Game.Calculations.IsBreaking(attacker.Owner, ox, oy);
                                        break;
                                    }
                                #endregion
                                #region SplittingSwipe
                                case 12170:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {

                                            PrepareSpell(spell, attacker.Owner);

                                            SpellUse suse = new SpellUse(true);
                                            suse.Attacker = attacker.UID;
                                            suse.SpellID = spell.ID;
                                            suse.SpellLevel = spell.Level;
                                            suse.X = X;
                                            suse.Y = Y;

                                            //Conquer_Online_Server.Network.GamePackets.SpellUse.Dama  geClass tar = new SpellUse.DamageClass(); 
                                            foreach (var t in attacker.Owner.Screen.Objects)
                                            {
                                                if (t == null)
                                                    continue;
                                                if (t.MapObjType == MapObjectType.Player || t.MapObjType == MapObjectType.Monster)
                                                {
                                                    var target = t as Entity;
                                                    if (Kernel.GetDistance(X, Y, target.X, target.Y) <= spell.Range)
                                                    {
                                                        if (CanAttack(attacker, target, spell, false))
                                                        {
                                                            tar.Damage = Calculate.Melee(attacker, target, spell, ref attack) / 2; 
                                                            tar.Hit = true;
                                                            //suse.AddTarget(target.UID, tar, attack); 
                                                            ReceiveAttack(attacker, target, attack, tar.Damage, spell);
                                                        }
                                                    }
                                                }
                                            }

                                            if (attacker.PlayerFlag == PlayerFlag.Entity)
                                                attacker.Owner.SendScreen(suse, true);
                                        }
                                        break;
                                    }
                                #endregion 
                                #region DragonFury
                                case 12300:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            SpellUse suse = new SpellUse(true);
                                            suse.Attacker = attacker.UID;
                                            suse.SpellID = spell.ID;
                                            suse.SpellLevel = spell.Level;
                                            suse.X = X;
                                            suse.Y = Y;
                                            PrepareSpell(spell, attacker.Owner);
                                            {
                                                {
                                                    if (attacked.EntityFlag == EntityFlag.Entity)
                                                    {
                                                        attack.Effect1 = Attack.AttackEffects1.None;
                                                        uint damage = Game.Attacking.Calculate.Melee(attacker, attacked, spell, ref attack) / 15;
                                                        suse.Effect1 = attack.Effect1;
                                                        attack.Damage = damage;
                                                        attacked.AddFlag3(Update.Flags3.DragonFury);
                                                        ReceiveAttack(attacker, attacked, attack, damage, spell);
                                                        suse.AddTarget(attacked.UID, damage, attack);
                                                    }
                                                    else if (attacked.EntityFlag == EntityFlag.Monster)
                                                    {
                                                        attack.Effect1 = Attack.AttackEffects1.None;
                                                        uint damage = Game.Attacking.Calculate.Melee(attacker, attacked, spell, ref attack) / 5;
                                                        suse.Effect1 = attack.Effect1;
                                                        attack.Damage = damage;
                                                        attacked.AddFlag3(Update.Flags3.DragonFury);
                                                        ReceiveAttack(attacker, attacked, attack, damage, spell);
                                                        suse.AddTarget(attacked.UID, damage, attack);
                                                        attacked.RemoveFlag3(Update.Flags3.DragonFury);
                                                    }
                                                }
                                            }
                                            attacker.Owner.SendScreen(suse, true);
                                        }
                                        break;
                                    }
                                #endregion
                                #region AirKick*AirSweep*AirRaid
                                case 12320:
                                case 12330:
                                case 12340:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            PrepareSpell(spell, attacker.Owner);
                                            SpellUse suse = new SpellUse(true);
                                            suse.Attacker = attacker.UID;
                                            suse.SpellID = spell.ID;
                                            suse.SpellLevel = spell.Level;
                                            ushort _X = attacker.X, _Y = attacker.Y;
                                            ushort _tX = X, _tY = Y;
                                            byte dist = (byte)spell.Distance;
                                            var Array = attacker.Owner.Screen.Objects;
                                            InLineAlgorithm algo = new InLineAlgorithm(attacker.X, X, attacker.Y, Y, dist,
                                                                               InLineAlgorithm.Algorithm.DDA);
                                            X = attacker.X;
                                            Y = attacker.Y;
                                            int i = 0;
                                            for (i = 0; i < algo.lcoords.Count; i++)
                                            {
                                                if (attacker.Owner.Map.Floor[algo.lcoords[i].X, algo.lcoords[i].Y, MapObjectType.Player]
                                                    && !attacker.ThroughGate(algo.lcoords[i].X, algo.lcoords[i].Y))
                                                {
                                                    X = (ushort)algo.lcoords[i].X;
                                                    Y = (ushort)algo.lcoords[i].Y;
                                                }
                                                else
                                                {
                                                    break;
                                                }
                                            }
                                            suse.X = X;
                                            suse.Y = Y;
                                            if (!attacker.Owner.Map.Floor[X, Y, MapObjectType.Player, null])
                                                return;
                                            double disth = 1.5;
                                            if (attacker.MapID == 5678) disth = 1;
                                            if (attacker.MapID == 3045) disth = 1;
                                            if (attacker.MapID == DeathMatch.MAPID) disth = 1;
                                            foreach (Interfaces.IMapObject _obj in Array)
                                            {
                                                bool hit = false;
                                                for (int j = 0; j < i; j++)
                                                    if (Kernel.GetDDistance(_obj.X, _obj.Y, (ushort)algo.lcoords[j].X, (ushort)algo.lcoords[j].Y) <= disth)
                                                        hit = true;
                                                if (hit)
                                                {
                                                    if (_obj.MapObjType == MapObjectType.Monster)
                                                    {
                                                        attacked = _obj as Entity;
                                                        if (CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                                        {
                                                            var damage = Game.Attacking.Calculate.Melee(attacker, attacked, spell, ref attack);
                                                            damage = (uint)(damage * 7);
                                                            ReceiveAttack(attacker, attacked, attack, damage, spell);

                                                            suse.AddTarget(attacked.UID, damage, attack);
                                                        }
                                                    }
                                                    else if (_obj.MapObjType == MapObjectType.Player)
                                                    {
                                                        attacked = _obj as Entity;
                                                        if (CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                                        {
                                                            uint damage = Game.Attacking.Calculate.Melee(attacker, attacked, spell, ref attack);
                                                            damage = (uint)(damage * Program.BladeTempest);
                                                            ReceiveAttack(attacker, attacked, attack, damage, spell);

                                                            suse.AddTarget(attacked.UID, damage, attack);
                                                        }
                                                    }
                                                    else if (_obj.MapObjType == MapObjectType.SobNpc)
                                                    {
                                                        attackedsob = _obj as SobNpcSpawn;
                                                        if (CanAttack(attacker, attackedsob, spell))
                                                        {
                                                            var damage = Game.Attacking.Calculate.Melee(attacker, attackedsob, ref attack);
                                                            damage = (uint)(damage * 7);
                                                            ReceiveAttack(attacker, attackedsob, attack, damage, spell);

                                                            suse.AddTarget(attackedsob.UID, damage, attack);
                                                        }
                                                    }
                                                }
                                            }
                                            attacker.PX = attacker.X;
                                            attacker.PY = attacker.Y;
                                            attacker.X = X;
                                            attacker.Y = Y;
                                            attacker.Owner.SendScreen(suse, true);
                                            attacker.Owner.Screen.Reload(suse);
                                        }
                                        break;
                                    }
                                #endregion 
                                #endregion
                                /* #region ChargingVortex
                                case 11190:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            /*if (!attacker.ContainsFlag(Update.Flags.Ride))
                                            {
                                                attacker.AttackPacket = null;
                                                return;
                                            }
                                            if (attacker.Owner.AlternateEquipment)
                                            {
                                                if (attacker.Owner.Equipment.Free(ConquerItem.RightWeapon))
                                                {
                                                    if (attacker.Owner.Equipment.Free(ConquerItem.AlternateRightWeapon))
                                                        return;
                                                    if (!Network.PacketHandler.IsTwoHand(attacker.Owner.Equipment.TryGetItem(ConquerItem.AlternateRightWeapon).ID))
                                                        return;
                                                }
                                                else
                                                {
                                                    if (attacker.Owner.Equipment.Free(ConquerItem.AlternateRightWeapon))
                                                    {
                                                        if (!Network.PacketHandler.IsTwoHand(attacker.Owner.Equipment.TryGetItem(ConquerItem.RightWeapon).ID))
                                                            return;
                                                    }
                                                    else
                                                        if (!Network.PacketHandler.IsTwoHand(attacker.Owner.Equipment.TryGetItem(ConquerItem.AlternateRightWeapon).ID))
                                                            return;
                                                }
                                            }
                                            else
                                            {
                                                if (attacker.Owner.Equipment.Free(ConquerItem.RightWeapon))
                                                    return;
                                                if (!Network.PacketHandler.IsTwoHand(attacker.Owner.Equipment.TryGetItem(ConquerItem.RightWeapon).ID))
                                                    return;
                                            }
                                            if (Kernel.GetDistance(attacker.X, attacker.Y, X, Y) <= spell.Distance)
                                            {
                                                attacker.AddFlag(Update.Flags.Ride);
                                                PrepareSpell(spell, attacker.Owner);

                                                SpellUse suse = new SpellUse(true);
                                                suse.Attacker = attacker.UID;
                                                suse.SpellID = spell.ID;
                                                suse.SpellLevel = spell.Level;
                                                ushort _X = attacker.X, _Y = attacker.Y;
                                                ushort _tX = X, _tY = Y;
                                                byte dist = (byte)Math.Min(Kernel.GetDistance(attacker.X, attacker.Y, X, Y), spell.Distance);
                                                var angle = Kernel.GetAngle(_X, _Y, _tX, _tY);
                                                bool oneMove = false;
                                                while (dist != 0)
                                                {
                                                    if (attacker.fMove(angle, ref _X, ref _Y))
                                                    {
                                                        oneMove = true;
                                                        X = _X;
                                                        Y = _Y;
                                                        angle = Kernel.GetAngle(_X, _Y, _tX, _tY);
                                                    }
                                                    else break;
                                                    dist--;
                                                }
                                                if (!oneMove)
                                                {
                                                    X = suse.X = attacker.X;
                                                    Y = suse.Y = attacker.Y;
                                                }
                                                else
                                                {
                                                    suse.X = X;
                                                    suse.Y = Y;
                                                }
                                                var Array = attacker.Owner.Screen.Objects;
                                                foreach (Interfaces.IMapObject _obj in Array)
                                                {
                                                    if (_obj.MapObjType == MapObjectType.Monster || _obj.MapObjType == MapObjectType.Player)
                                                    {
                                                        attacked = _obj as Entity;
                                                        if (Kernel.GetDistance(X, Y, attacked.X, attacked.Y) > spell.Range)
                                                            continue;
                                                        if (CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                                        {
                                                            var damage = Game.Attacking.Calculate.Melee(attacker, attacked, spell, ref attack);

                                                            ReceiveAttack(attacker, attacked, attack, damage, spell);

                                                            suse.AddTarget(attacked.UID, damage, attack);
                                                        }
                                                    }
                                                    else if (_obj.MapObjType == MapObjectType.SobNpc)
                                                    {
                                                        attackedsob = _obj as SobNpcSpawn;
                                                        if (spell.ID == 10315)
                                                            if (Kernel.GetDistance(attacker.X, attacker.Y, attackedsob.X, attackedsob.Y) > spell.Range)
                                                                if (Kernel.GetDistance(X, Y, attackedsob.X, attackedsob.Y) > spell.Range)
                                                                    if (Kernel.GetDistance((ushort)((X + attacker.X) / 2), (ushort)((Y + attacker.Y) / 2), attackedsob.X, attackedsob.Y) > spell.Range)
                                                                        continue;
                                                        if (spell.ID != 10315)
                                                            if (Kernel.GetDistance(X, Y, attackedsob.X, attackedsob.Y) > spell.Range)
                                                                continue;
                                                        if (CanAttack(attacker, attackedsob, spell))
                                                        {
                                                            var damage = Game.Attacking.Calculate.Melee(attacker, attackedsob, ref attack);

                                                            ReceiveAttack(attacker, attackedsob, attack, damage, spell);

                                                            suse.AddTarget(attackedsob.UID, damage, attack);
                                                        }
                                                    }
                                                }
                                                Attack shift = new Attack(true);
                                                shift.Attacker = attacker.UID;
                                                shift.X = X;
                                                shift.Y = Y;
                                                shift.AttackType = Attack.SkillMove;
                                                shift.dwParam = 10315;
                                                attacker.Shift(X, Y, attacker.MapID, shift);
                                                attacker.RemoveFlag(Update.Flags.Ride);
                                                attacker.Owner.SendScreen(suse, true);
                                            }
                                            else
                                            {
                                                attacker.AttackPacket = null;
                                            }
                                        }
                                        break;
                                    }
                                    #endregion*/
                                #region ChargingVortex
                                case 11190:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            if (attacker.Owner.Map.Floor[X, Y, MapObjectType.InvalidCast, null])
                                                break;
                                            spell.UseStamina = 20;
                                            PrepareSpell(spell, attacker.Owner);
                                            SpellUse suse = new SpellUse(true);
                                            suse.Attacker = attacker.UID;
                                            suse.SpellID = spell.ID;
                                            suse.SpellLevel = spell.Level;
                                            //suse.SpellLevelHu = attacker.Owner.Spells[spell.ID].LevelHu;
                                            suse.X = X;
                                            suse.Y = Y;
                                            attack.X = X;
                                            attack.Y = Y;
                                            attack.Attacker = attacker.UID;
                                            attack.AttackType = 53;
                                            attack.X = X;
                                            attack.Y = Y;
                                            attacker.Owner.SendScreen(attack, true);
                                            attacker.X = X;
                                            attacker.Y = Y;
                                            if (Kernel.GetDistance(attacker.X, attacker.Y, X, Y) <= spell.Range)
                                            {
                                                /* for (int c = 0; c < attacker.Owner.Screen.Objects.Count; c++)
                                                 {
                                                     if (c >= attacker.Owner.Screen.Objects.Count)
                                                         break;
                                                     Interfaces.IMapObject _obj = attacker.Owner.Screen.Objects[c];
                                                     if (_obj == null)
                                                         continue;
                                                     if (_obj.MapObjType == MapObjectType.Monster || _obj.MapObjType == MapObjectType.Player)
                                                     {
                                                         attacked = _obj as Entity;
                                                         if (Kernel.GetDistance(attacker.X, attacker.Y, attacked.X, attacked.Y) <= spell.Range)
                                                         {
                                                             if (CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                                             {
                                                                 uint damage = Game.Attacking.Calculate.Melee(attacker, attacked, spell, ref attack);
                                                                 ReceiveAttack(attacker, attacked, attack, damage, spell);
                                                                // suse.Targets.Add(attacked.UID, damage);
                                                                 suse.AddTarget(attacked.UID, damage, attack);
                                                             }
                                                         }
                                                     }*/
                                                var Array = attacker.Owner.Screen.Objects;
                                                foreach (Interfaces.IMapObject _obj in Array)
                                                {
                                                    if (_obj.MapObjType == MapObjectType.Monster || _obj.MapObjType == MapObjectType.Player)
                                                    {
                                                        attacked = _obj as Entity;
                                                        if (Kernel.GetDistance(X, Y, attacked.X, attacked.Y) > spell.Range)
                                                            continue;
                                                        if (CanAttack(attacker, attacked, spell, attack.AttackType == Attack.Melee))
                                                        {
                                                            var damage = Game.Attacking.Calculate.Melee(attacker, attacked, spell, ref attack)*2;

                                                            ReceiveAttack(attacker, attacked, attack, damage, spell);

                                                            suse.AddTarget(attacked.UID, damage, attack);
                                                        }
                                                    }
                                                    else if (_obj.MapObjType == MapObjectType.SobNpc)
                                                    {
                                                        attackedsob = _obj as SobNpcSpawn;
                                                        if (Kernel.GetDistance(attacker.X, attacker.Y, attackedsob.X, attackedsob.Y) <= spell.Range)
                                                        {
                                                            if (CanAttack(attacker, attackedsob, spell))
                                                            {
                                                                uint damage = Game.Attacking.Calculate.Melee(attacker, attackedsob, ref attack)*2;
                                                                ReceiveAttack(attacker, attacked, attack, damage, spell);
                                                                // suse.Targets.Add(attackedsob.UID, damage);
                                                                suse.AddTarget(attackedsob.UID, damage, attack);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                attacker.AttackPacket = null;
                                            }
                                            attacker.Owner.SendScreen(suse, true);
                                            attacker.Owner.Screen.Reload(suse);
                                        }
                                        break;
                                    }
                                #endregion
                                #region Heaven Blade
                                //HeavenBlade
                                case 10310:
                                    {
                                        if (CanUseSpell(spell, attacker.Owner))
                                        {
                                            if (attacked != null)
                                            {
                                                if (Kernel.GetDistance(attacker.X, attacker.Y, attacked.X, attacked.Y) <= spell.Distance)
                                                {
                                                    SpellUse suse = new SpellUse(true);
                                                    suse.Attacker = attacker.UID;
                                                    suse.SpellID = spell.ID;
                                                    suse.SpellLevel = spell.Level;
                                                    suse.X = X;
                                                    suse.Y = Y;

                                                    if (CanAttack(attacker, attacked, spell, false))
                                                    {
                                                        PrepareSpell(spell, attacker.Owner);
                                                        var damage = Game.Attacking.Calculate.Magic(attacker, attacked, spell, ref attack);
                                                        if (Kernel.Rate(spell.Percent))
                                                        {
                                                            ReceiveAttack(attacker, attacked, attack, damage, spell);
                                                            suse.AddTarget(attacked.UID, damage, attack);
                                                        }
                                                        else
                                                        {
                                                            damage = 0;
                                                            suse.AddTarget(attacked.UID, damage, attack);
                                                        }
                                                        attacker.Owner.SendScreen(suse, true);
                                                    }
                                                }
                                                else
                                                {
                                                    attacker.AttackPacket = null;
                                                }
                                            }
                                            else
                                            {
                                                if (attackedsob != null)
                                                {
                                                    if (Kernel.GetDistance(attacker.X, attacker.Y, attackedsob.X, attackedsob.Y) <= spell.Distance)
                                                    {
                                                        SpellUse suse = new SpellUse(true);
                                                        suse.Attacker = attacker.UID;
                                                        suse.SpellID = spell.ID;
                                                        suse.SpellLevel = spell.Level;
                                                        suse.X = X;
                                                        suse.Y = Y;

                                                        if (CanAttack(attacker, attackedsob, spell))
                                                        {
                                                            PrepareSpell(spell, attacker.Owner);
                                                            var damage = Game.Attacking.Calculate.Magic(attacker, attackedsob, spell, ref attack);
                                                            if (Kernel.Rate(spell.Percent))
                                                            {
                                                                ReceiveAttack(attacker, attackedsob, attack, damage, spell);

                                                                suse.AddTarget(attackedsob.UID, damage, attack);
                                                            }
                                                            else
                                                            {
                                                                damage = 0;
                                                                suse.AddTarget(attackedsob.UID, damage, attack);
                                                            }
                                                            attacker.Owner.SendScreen(suse, true);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            attacker.AttackPacket = null;
                                        }
                                        break;
                                    }
                                #endregion

                            }
                            attacker.Owner.IncreaseSpellExperience(Experience, spellID);
                            if (attacker.MapID == 1039)
                            {
                                if (spell.ID == 7001 || spell.ID == 9876)
                                {
                                    attacker.AttackPacket = null;
                                    return;
                                }
                                if (attacker.AttackPacket != null)
                                {
                                    attack.Damage = spell.ID;
                                    attacker.AttackPacket = attack;
                                    if (Database.SpellTable.WeaponSpells.ContainsValue(spell.ID))
                                    {
                                        if (attacker.AttackPacket == null)
                                        {
                                            attack.AttackType = Attack.Melee;
                                            attacker.AttackPacket = attack;
                                        }
                                        else
                                        {
                                            attacker.AttackPacket.AttackType = Attack.Melee;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (spell.NextSpellID != 0)
                                {
                                    if (spell.NextSpellID >= 1000 && spell.NextSpellID <= 1002)
                                        if (Target >= 1000000)
                                        {
                                            attacker.AttackPacket = null;
                                            return;
                                        }
                                    attack.Damage = spell.NextSpellID;
                                    attacker.AttackPacket = attack;
                                }
                                else
                                {
                                    if (!Database.SpellTable.WeaponSpells.ContainsValue(spell.ID) || spell.ID == 9876)
                                        attacker.AttackPacket = null;
                                    else
                                    {
                                        if (attacker.AttackPacket == null)
                                        {
                                            attack.AttackType = Attack.Melee;
                                            attacker.AttackPacket = attack;
                                        }
                                        else
                                        {
                                            attacker.AttackPacket.AttackType = Attack.Melee;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            attacker.AttackPacket = null;
                        }
                    }
                #endregion
                }
            #endregion
            }
            #endregion

        }
        public static void HandleAura(Game.Entity attacker, Database.SpellInformation spell)
        {
            ulong statusFlag = 0;
            switch (spell.ID)
            {
                //case 10424: statusFlag = Update.Flags2.EarthAura; break;
                case 10423: statusFlag = Update.Flags2.FireAura; break;
                case 10422: statusFlag = Update.Flags2.WaterAura; break;
                case 10421: statusFlag = Update.Flags2.WoodAura; break;
                case 10420: statusFlag = Update.Flags2.MetalAura; break;
                case 10410: statusFlag = Update.Flags2.FendAura; break;
                case 10395: statusFlag = Update.Flags2.TyrantAura; break;
            }

            if (attacker.Dead) return;
            if (attacker.Aura_isActive)
            {
                attacker.RemoveFlag2(attacker.Aura_actType);
                attacker.Owner.doAuraBonuses(attacker.Aura_actType, attacker.Aura_actPower, -1);
                attacker.Aura_isActive = false;
                if (statusFlag == attacker.Aura_actType)
                {
                    attacker.Aura_actType = 0;
                    attacker.Aura_actPower = 0;
                    return;
                }
            }
            if (CanUseSpell(spell, attacker.Owner))
            {
                attacker.AuraStamp = Time32.Now;
                attacker.AuraTime = 20;

                PrepareSpell(spell, attacker.Owner);

                SpellUse suse = new SpellUse(true);
                suse.Attacker = attacker.UID;
                suse.SpellID = spell.ID;
                suse.SpellLevel = spell.Level;
                suse.X = attacker.X;
                suse.Y = attacker.Y;

                suse.AddTarget(attacker.UID, 0, null);
                attacker.Owner.SendScreen(suse, true);
                attacker.AddFlag2(statusFlag);
                attacker.Aura_isActive = true;
                attacker.Aura_actType = statusFlag;
                attacker.Aura_actPower = spell.Power;
                attacker.Owner.doAuraBonuses(statusFlag, spell.Power, 1);
            }
        }
        public static List<IMapObject> GetObjects(UInt16 ox, UInt16 oy, Client.GameClient c)
        {
            UInt16 x, y;
            x = c.Entity.X;
            y = c.Entity.Y;

            var list = new List<IMapObject>();
            c.Entity.X = ox;
            c.Entity.Y = oy;
            foreach (IMapObject objects in c.Screen.Objects)
            {
                if (objects != null)
                    if (objects.UID != c.Entity.UID)
                        if (!list.Contains(objects))
                            list.Add(objects);
            }
            c.Entity.X = x;
            c.Entity.Y = y;
            foreach (IMapObject objects in c.Screen.Objects)
            {
                if (objects != null)
                    if (objects.UID != c.Entity.UID)
                        if (!list.Contains(objects))
                            list.Add(objects);
            }
            if (list.Count > 0)
                return list;
            return null;
        }
        public static void ReceiveAttack(Game.Entity attacker, Game.Entity attacked, Attack attack, uint damage, Database.SpellInformation spell)
        {

            if (attacker.MapID == DeathMatch.MAPID)
            {
                DeathMatch.Points[attacker.TeamDeathMatchTeamKey]++;
                attacker.TeamDeathMatch_Hits++;
                attacked.Hitpoints = attacked.MaxHitpoints;
            }

            {
                if (damage > attacked.Hitpoints)
                {
                    attacker.Owner.IncreaseExperience(Calculate.CalculateExpBonus(attacker.Level, attacked.Level, Math.Min(damage, attacked.Hitpoints)), true);
                    if (spell != null)
                        attacker.Owner.IncreaseSpellExperience((uint)Calculate.CalculateExpBonus(attacker.Level, attacked.Level, Math.Min(damage, attacked.Hitpoints)), spell.ID);
                }
                else
                {
                    attacker.Owner.IncreaseExperience(Calculate.CalculateExpBonus(attacker.Level, attacked.Level, damage), true);
                    if (spell != null)
                        attacker.Owner.IncreaseSpellExperience((uint)Calculate.CalculateExpBonus(attacker.Level, attacked.Level, damage), spell.ID);
                }
            }
            if (attacker.EntityFlag == EntityFlag.Monster && attacked.EntityFlag == EntityFlag.Player)
            {
                if (attacked.Action == Enums.ConquerAction.Sit)
                    if (attacked.Stamina > 20)
                        attacked.Stamina -= 20;
                    else
                        attacked.Stamina = 0;
                attacked.Action = Enums.ConquerAction.None;
            }

            if (attack.AttackType == Attack.Magic)
            {
                if (attacked.Hitpoints <= damage)
                {
                    attacker.Owner.UpdateQualifier(attacker.Owner, attacked.Owner, attacked.Hitpoints);
                    attacked.CauseOfDeathIsMagic = true;
                    attacked.Die(attacker);
                    attacked.IsDropped = false;
                    if (attacker.PKMode == Enums.PKMode.Jiang)
                    {
                        if (attacked.JiangActive)
                        {
                            if (attacker.MyJiang != null && attacked.MyJiang != null)
                            {
                                attacker.MyJiang.GetKill(attacker.Owner, attacked.MyJiang);
                            }
                        }
                    }
                }
                else
                {
                    attacker.Owner.UpdateQualifier(attacker.Owner, attacked.Owner, damage);
                    attacked.Hitpoints -= damage;
                }
            }
            else
            {
                if (attacked.Hitpoints <= damage)
                {
                    if (attacked.EntityFlag == EntityFlag.Player)
                    {
                        attacker.Owner.UpdateQualifier(attacker.Owner, attacked.Owner, attacked.Hitpoints);
                        attacked.Owner.SendScreen(attack, true);
                        attacker.AttackPacket = null;
                    }
                    else
                    {
                        attacked.MonsterInfo.SendScreen(attack);
                    }
                    attacked.Die(attacker);
                    if (attacker.PKMode == Enums.PKMode.Jiang)
                    {
                        if (attacked.JiangActive)
                        {
                            if (attacker.MyJiang != null && attacked.MyJiang != null)
                            {
                                attacker.MyJiang.GetKill(attacker.Owner, attacked.MyJiang);
                            }
                        }
                    }
                }
                else
                {
                    attacked.Hitpoints -= damage;
                    if (attacked.EntityFlag == EntityFlag.Player)
                    {
                        attacker.Owner.UpdateQualifier(attacker.Owner, attacked.Owner, damage);
                        attacked.Owner.SendScreen(attack, true);
                    }
                    else
                        attacked.MonsterInfo.SendScreen(attack);
                    attacker.AttackPacket = attack;
                    attacker.AttackStamp = Time32.Now;
                }
            }
        }

        public static void ReceiveAttack(Game.Entity attacker, SobNpcSpawn attacked, Attack attack, uint damage, Database.SpellInformation spell)
        {
            if (attacker.MapID == 2075)
            {
                if (attacked.UID == 815)
                {
                    if (Game.PoleIslanD.PoleKeeper == attacker.Owner.Guild)
                        return;
                    if (attacked.Hitpoints <= damage)
                        attacked.Hitpoints = 0;

                    Game.PoleIslanD.AddScore(damage, attacker.Owner.Guild);
                }
            }  
            if (attacker.MapID == 2072)
            {
                if (attacked.UID == 818)
                {
                    if (Game.PoleTwin.PoleKeeper == attacker.Owner.Guild)
                        return;
                    if (attacked.Hitpoints <= damage)
                        attacked.Hitpoints = 0;

                    Game.PoleTwin.AddScore(damage, attacker.Owner.Guild);
                }
            }  
            /* if (attacker.MapID == 2072)
             {
                 if (attacked.UID == 818)
                 {
                     if (Game.PoleTwin.PoleKeeper == attacker.Owner.Guild)
                         return;
                     if (attacked.Hitpoints <= damage)
                         attacked.Hitpoints = 0;
                     attacker.Money += 1000;
                     Game.PoleTwin.AddScore(damage, attacker.Owner.Guild);
                 }
             }*/
            if (attacker.MapID == CrossServer.mapid)
            {
                if (attacked.UID == CrossServer.npc)
                {

                    if (attacked.Hitpoints <= damage)
                        attacked.Hitpoints = 0;
                    CrossServer.AddScore(damage, attacker.CountryID);
                }
            }
            if (attacker.EntityFlag == EntityFlag.Player)
                if (damage > attacked.Hitpoints)
                {
                    if (attacker.MapID == 1039)
                        attacker.Owner.IncreaseExperience(Math.Min(damage, attacked.Hitpoints), true);
                    if (spell != null)
                        attacker.Owner.IncreaseSpellExperience(Math.Min(damage, attacked.Hitpoints), spell.ID);
                }
                else
                {
                    if (attacker.MapID == 1039)
                        attacker.Owner.IncreaseExperience(damage, true);
                    if (spell != null)
                        attacker.Owner.IncreaseSpellExperience(damage, spell.ID);
                }
            if (attacked.UID == 123456)
            {
                if (Program.World.PoleDomination.KillerGuildID == attacker.Owner.Guild.ID)
                    return;
                Program.World.PoleDomination.AddScore(damage, attacker.Owner.Guild);
            }

            if (attacker.MapID == CaptureTheFlag.MapID)
                if (attacker.GuildID != 0 && Program.World.CTF.Bases[attacked.UID].CapturerID != attacker.GuildID)
                    Program.World.CTF.AddScore(damage, attacker.Owner.Guild, attacked);
            /* if (attacker.MapID == 2072)
             {
                 if (attacked.UID == 818)
                 {
                     if (Game.PoleTwin.PoleKeeper == attacker.Owner.Guild)
                         return;
                     if (attacked.Hitpoints <= damage)
                         attacked.Hitpoints = 0;

                     Game.PoleTwin.AddScore(damage, attacker.Owner.Guild);
                 }
             }*/
            if (attacker.MapID == 1038)
            {
                if (attacked.UID == 810)
                {
                    if (Game.GuildWar.PoleKeeper == attacker.Owner.Guild)
                        return;
                    if (attacked.Hitpoints <= damage)
                        attacked.Hitpoints = 0;
                    Game.GuildWar.AddScore(damage, attacker.Owner.Guild);
                }
            }
            if (attacker.MapID == 1509)
            {
                if (attacked.UID == 812)
                {
                    Clan clan = attacker.GetClan;
                    if (Game.ClanWar.PoleKeeper == clan)
                        return;
                    // attacker.ConquerPoints += 20;
                    //attacker.Owner.Send(new Message("You have got 20 CPS", System.Drawing.Color.Black, Message.TopLeft));
                    if (attacked.Hitpoints <= damage)
                        attacked.Hitpoints = 0;
                    Game.ClanWar.AddScore(damage, clan);
                }
            }
            if (attacker.MapID == 2071)
            {
                if (attacked.UID == 811)
                {
                    if (Game.EliteGuildWar.PoleKeeper == attacker.Owner.Guild)
                        return;
                    if (attacked.Hitpoints <= damage)
                        attacked.Hitpoints = 0;
                    Game.EliteGuildWar.AddScore(damage, attacker.Owner.Guild);
                }
            }
            if (attack.AttackType == Attack.Magic)
            {
                if (attacked.Hitpoints <= damage)
                {
                    attacked.Die(attacker);
                }
                else
                {
                    attacked.Hitpoints -= damage;
                }
            }
            else
            {
                attacker.Owner.SendScreen(attack, true);
                if (attacked.Hitpoints <= damage)
                {
                    attacked.Die(attacker);
                }
                else
                {
                    attacked.Hitpoints -= damage;
                    attacker.AttackPacket = attack;
                    attacker.AttackStamp = Time32.Now;
                }
            }
        }
        public static bool isArcherSkill(uint ID)
        {
            if (ID >= 8000 && ID <= 9875)
                return true;
            return false;
        }
        public static bool CanUseSpell(Database.SpellInformation spell, Client.GameClient client)
        {

            if (client.WatchingGroup != null)
                return false;
            if (Constants.FBandSSEvent.Contains(client.Entity.MapID))
            {
                if (spell.ID != 1045 && spell.ID != 1046)
                {
                    client.Send(new Message("You can use only ScentSowrd And FastBlade Skill", System.Drawing.Color.Red, 0x7dc));
                    return false;
                }
            }
            if (spell == null)
                return false;
            if (client.Entity.Mana < spell.UseMana)
                return false;
            if (client.Entity.Stamina < spell.UseStamina)
                return false;
            if (spell.UseArrows > 0 && isArcherSkill(spell.ID))
            {
                var weapons = client.Weapons;
                if (weapons.Item2 != null)
                    if (!client.Entity.ContainsFlag3(Update.Flags3.Assassin))
                        if (!PacketHandler.IsArrow(weapons.Item2.ID))
                            return false;

                return true;
            }
            if (spell.NeedXP == 1 && !client.Entity.ContainsFlag(Update.Flags.XPList))
                return false;
            return true;
        }
        public static void PrepareSpell(Database.SpellInformation spell, Client.GameClient client)
        {
            if (spell.NeedXP == 1)
                client.Entity.RemoveFlag(Update.Flags.XPList);
            if (client.Map.ID != 1039)
            {
                if (spell.UseMana > 0)
                    if (client.Entity.Mana >= spell.UseMana)
                        client.Entity.Mana -= spell.UseMana;
                if (spell.UseStamina > 0)
                    if (client.Entity.Stamina >= spell.UseStamina)
                        client.Entity.Stamina -= spell.UseStamina;
            }
        }
        public static bool CanAttack(Game.Entity attacker, SobNpcSpawn attacked, Database.SpellInformation spell)
        {
            if (attacker.MapID == 2075)
            {
                if (attacker.GuildID == 0 || !Game.PoleIslanD.IsWar)
                {
                    if (attacked.UID == 815)
                    {
                        return false;
                    }
                }
                if (Game.PoleIslanD.PoleKeeper != null)
                {
                    if (Game.PoleIslanD.PoleKeeper == attacker.Owner.Guild)
                    {
                        if (attacked.UID == 815)
                        {
                            return false;
                        }
                    }
                    else if (attacked.UID == 516020 || attacked.UID == 516021)
                    {
                        if (Game.PoleIslanD.PoleKeeper == attacker.Owner.Guild)
                        {
                            if (attacker.PKMode == Enums.PKMode.Team)
                                return false;
                        }
                    }
                }
            }  
            if (attacker.MapID == 2072)
            {
                if (attacker.GuildID == 0 || !Game.PoleTwin.IsWar)
                {
                    if (attacked.UID == 818)
                    {
                        return false;
                    }
                }
                if (Game.PoleTwin.PoleKeeper != null)
                {
                    if (Game.PoleTwin.PoleKeeper == attacker.Owner.Guild)
                    {
                        if (attacked.UID == 818)//Yuriii
                        {
                            return false;
                        }
                    }
                    else if (attacked.UID == 516077 || attacked.UID == 516076)
                    {
                        if (Game.PoleTwin.PoleKeeper == attacker.Owner.Guild)
                        {
                            if (attacker.PKMode == Enums.PKMode.Team)
                                return false;
                        }
                    }
                }
            }  
            /* if (attacker.MapID == 2072)
             {
                 if (attacker.GuildID == 0 || !Game.PoleTwin.IsWar)
                 {
                     if (attacked.UID == 818)
                     {
                         return false;
                     }
                 }
                 if (Game.PoleTwin.PoleKeeper != null)
                 {
                     if (Game.PoleTwin.PoleKeeper == attacker.Owner.Guild)
                     {
                         if (attacked.UID == 818)
                         {
                             return false;
                         }
                     }
                     else if (attacked.UID == 516077 || attacked.UID == 516076)
                     {
                         if (Game.PoleTwin.PoleKeeper == attacker.Owner.Guild)
                         {
                             if (attacker.PKMode == Enums.PKMode.Team)
                                 return false;
                         }
                     }
                 }
             }*/
            if (attacker.MapID == CrossServer.mapid)
            {
                if (!CrossServer.IsWar)
                {
                    if (attacked.UID == CrossServer.npc)
                    {
                        return false;
                    }
                }
                if (CrossServer.PoleKeeper != null)
                {
                    if (CrossServer.PoleKeeper.Name == attacker.Owner.Country)
                    {
                        if (attacked.UID == CrossServer.npc)
                        {
                            return false;
                        }
                    }
                }
            }
            if (attacker.MapID == CaptureTheFlag.MapID)
            {
                if (Program.World.CTF.Bases.ContainsKey(attacked.UID))
                {
                    var _base = Program.World.CTF.Bases[attacked.UID];
                    if (_base.CapturerID == attacker.GuildID)
                        return false;
                }
                return true;
            }
            if (attacked.UID == 123456)
                if (attacked.Hitpoints > 0)
                    if (attacker.GuildID != 0 && attacker.GuildID != Program.World.PoleDomination.KillerGuildID)
                        return true;
                    else return false;
                else return false;
            if (attacker.MapID == 1509)
            {
                if (attacker.ClanId == 0 || !Game.ClanWar.IsWar)
                {
                    if (attacked.UID == 812)
                    {
                        return false;
                    }
                }
                if (Game.ClanWar.PoleKeeper != null)
                {
                    if (Game.ClanWar.PoleKeeper == attacker.GetClan)
                    {
                        if (attacked.UID == 812)
                        {
                            return false;
                        }
                    }
                }
            }
            if (attacker.MapID == 1038)
            {
                if (attacker.GuildID == 0 || !Game.GuildWar.IsWar)
                {
                    if (attacked.UID == 810)
                    {
                        return false;
                    }
                }
                if (Game.GuildWar.PoleKeeper != null)
                {
                    if (Game.GuildWar.PoleKeeper == attacker.Owner.Guild)
                    {
                        if (attacked.UID == 810)
                        {
                            return false;
                        }
                    }
                    else if (attacked.UID == 516075 || attacked.UID == 516074)
                    {
                        if (Game.GuildWar.PoleKeeper == attacker.Owner.Guild)
                        {
                            if (attacker.PKMode == Enums.PKMode.Team)
                                return false;
                        }
                    }
                }
            }
            if (attacker.MapID == 2071)
            {
                if (attacker.GuildID == 0 || !Game.EliteGuildWar.IsWar)
                {
                    if (attacked.UID == 811)
                    {
                        return false;
                    }
                }
                if (Game.EliteGuildWar.PoleKeeper != null)
                {
                    if (Game.EliteGuildWar.PoleKeeper == attacker.Owner.Guild)
                    {
                        if (attacked.UID == 811)
                        {
                            return false;
                        }
                    }
                    else if (attacked.UID == 516075 || attacked.UID == 516074)
                    {
                        if (Game.EliteGuildWar.PoleKeeper == attacker.Owner.Guild)
                        {
                            if (attacker.PKMode == Enums.PKMode.Team)
                                return false;
                        }
                    }
                }
            }
            if (attacker.MapID == 1039)
            {
                bool stake = true;
                if (attacked.LoweredName.Contains("crow"))
                    stake = false;

                ushort levelbase = (ushort)(attacked.Mesh / 10);
                if (stake)
                    levelbase -= 42;
                else
                    levelbase -= 43;

                byte level = (byte)(20 + (levelbase / 3) * 5);
                if (levelbase == 108 || levelbase == 109)
                    level = 125;
                if (attacker.Level >= level)
                    return true;
                else
                {
                    attacker.AttackPacket = null;
                    attacker.Owner.Send(Constants.DummyLevelTooHigh());
                    return false;
                }
            }
            return true;
        }

        public static bool CanAttack(Game.Entity attacker, Game.Entity attacked, Database.SpellInformation spell, bool melee)
        {
            if (attacker.PKMode == Enums.PKMode.Guild)
            {
                if (attacker.Owner.Guild.Enemy.ContainsKey(attacked.GuildID))
                {
                    if (attacked.Dead)
                    {
                        return false;
                    }
                    if (attacker.UID == attacked.UID)
                        return false;
                    if (attacker.MapID == 1000 || attacker.MapID == 1015 || attacker.MapID == 1020 || attacker.MapID == 1011)
                    {
                        return true;
                    }
                }
            }
            if (attacker.PKMode == Enums.PKMode.Bahaa)
            {
                if (attacker.Owner.Enemy.ContainsKey(attacked.UID))
                {
                    if (attacked.Dead)
                    {
                        return false;
                    }
                    if (attacker.UID == attacked.UID)
                        return false;
                    if (attacker.MapID == 1000 || attacker.MapID == 1015 || attacker.MapID == 1020 || attacker.MapID == 1011)
                    {
                        return true;
                    }
                }
            }  
            if (attacker.PKMode == Enums.PKMode.Jiang)
            {
                if (attacked.JiangActive)
                {
                    if (attacked.Dead)
                    {

                        return false;
                    }
                    if (attacker.UID == attacked.UID)
                        return false;//Maxs
                    if (attacker.MapID == 1002 || attacker.MapID == 1000 || attacker.MapID == 1015 || attacker.MapID == 1020 || attacker.MapID == 1011)
                    {
                        return true;
                    }
                }

            }
            if (attacked.Dead) return false;
            if (attacker.MapID == 1820 && attacker.PKMode == Game.Enums.PKMode.PK)
            {
                if (attacker.Owner.Team != null)
                {
                    if (attacker.Owner.Team.IsTeammate(attacked.UID))
                    {
                        attacker.Owner.Send(new Message("Sorry You Can't Attack your Team In TeamWar By Mr.Bahaa ", System.Drawing.Color.Red, Message.TopLeft));
                        return false;
                    }
                }
            }
            if (attacker.MapID == CrossServer.mapid)
            {
                if (attacker.Owner.Country == attacked.Owner.Country)
                {
                    return false;
                }
            }
            if (attacked.Companion || attacker.BodyGuard)
                if (attacked.MonsterInfo.ID == MonsterInformation.ReviverID)
                    return false;
            if (attacked.Dead) return false;
            if (attacker.EntityFlag == EntityFlag.Player)
                if (attacker.Owner.WatchingElitePKMatch != null)
                    return false;
            if (attacked.EntityFlag == EntityFlag.Player)
                if (attacked.Owner.WatchingElitePKMatch != null)
                    return false;
            if (attacker.EntityFlag == EntityFlag.Player)
                if (attacked != null && attacked.EntityFlag == EntityFlag.Player)
                    if (attacker.Owner.InTeamQualifier() && attacked.Owner.InTeamQualifier())
                        return !attacker.Owner.Team.IsTeammate(attacked.UID);
            if (attacker.MapID == CaptureTheFlag.MapID)
                if (!CaptureTheFlag.Attackable(attacker) || !CaptureTheFlag.Attackable(attacked))
                    return false;
            if (attacker.MapID == DeathMatch.MAPID)
                return attacker.TeamDeathMatchTeamKey != attacked.TeamDeathMatchTeamKey;
            if (spell != null)
                if (spell.CanKill && attacker.EntityFlag == EntityFlag.Player && Constants.PKForbiddenMaps.Contains(attacker.Owner.Map.ID) && attacked.EntityFlag == EntityFlag.Player && (attacker.TalentStaus == 0))
                    return false;
            if (attacker.EntityFlag == EntityFlag.Player)
                if (attacker.Owner.WatchingElitePKMatch != null)
                    return false;
            if (attacked.EntityFlag == EntityFlag.Player)
                if (attacked.Owner.WatchingElitePKMatch != null)
                    return false;
            if (attacker.EntityFlag == EntityFlag.Player)//TQ 
                if (attacker.Owner.WatchingGroup != null)
                    return false;
            if (attacker.EntityFlag == EntityFlag.Player)
                if (attacker.Owner.TeamQualifierGroup != null)
                    return false;//TQ 
            if (attacker.EntityFlag == EntityFlag.Player)
                if (attacker.Owner.WatchingElitePKMatch == null)
                    if (attacked.EntityFlag == EntityFlag.Player)
                        if (attacked.Owner.WatchingElitePKMatch != null)
                            return false;
            if (attacker.EntityFlag == EntityFlag.Player)
                if (attacker.Owner.TeamQualifierGroup == null)
                    if (attacked.EntityFlag == EntityFlag.Player)
                        if (attacked.Owner.TeamQualifierGroup != null)
                            return false;
            if (attacker.EntityFlag == EntityFlag.Player)
                if (attacker.Owner.WatchingGroup == null)
                    if (attacked.EntityFlag == EntityFlag.Player)
                        if (attacked.Owner.WatchingGroup != null)
                            return false;
            if (attacked == null)
                return false;
            if (attacked.Dead)
            {
                attacker.AttackPacket = null;
                return false;
            }
            if (attacker.EntityFlag == EntityFlag.Player && attacked.EntityFlag == EntityFlag.Player)
                if ((attacker.Owner.InQualifier() && attacked.Owner.IsWatching()) || (attacked.Owner.InQualifier() && attacker.Owner.IsWatching()))
                    return false;
            if (attacker.EntityFlag == EntityFlag.Player)
                if (Time32.Now < attacker.Owner.CantAttack)
                    return false;
            if (attacked.EntityFlag == EntityFlag.Monster)
            {
                if (attacked.Companion/* || attacker.BodyGuard*/)
                {
                    if (Constants.PKForbiddenMaps.Contains(attacker.Owner.Map.ID))
                    {
                        if (attacked.Owner == attacker.Owner)
                            return false;
                        if (attacker.PKMode != Conquer_Online_Server.Game.Enums.PKMode.PK &&
                         attacker.PKMode != Conquer_Online_Server.Game.Enums.PKMode.Team &&
                            attacker.PKMode != Conquer_Online_Server.Game.Enums.PKMode.Jiang)
                            return false;
                        else
                        {
                            attacker.AddFlag(Network.GamePackets.Update.Flags.FlashingName);
                            attacker.FlashingNameStamp = Time32.Now;
                            attacker.FlashingNameTime = 10;
                            return true;
                        }
                    }
                }
                if (attacked.Name.Contains("Guard"))
                {
                    if (attacker.PKMode != Conquer_Online_Server.Game.Enums.PKMode.PK &&
                    attacker.PKMode != Conquer_Online_Server.Game.Enums.PKMode.Team)
                        return false;
                    else
                    {
                        attacker.AddFlag(Network.GamePackets.Update.Flags.FlashingName);
                        attacker.FlashingNameStamp = Time32.Now;
                        attacker.FlashingNameTime = 10;

                        return true;
                    }
                }
                else
                    return true;
            }
            else
            {
                if (attacked.EntityFlag == EntityFlag.Player)
                    if (!attacked.Owner.Attackable)
                        return false;
                if (attacker.EntityFlag == EntityFlag.Player)
                    if (attacker.Owner.WatchingGroup == null)
                        if (attacked.EntityFlag == EntityFlag.Player)
                            if (attacked.Owner.WatchingGroup != null)
                                return false;

                if (attacked.EntityFlag == EntityFlag.Player)
                    if (attacked.Owner.WatchingElitePKMatch != null)
                        return false;
                if (spell != null)
                {//Ahmed Conquer_Online_Server : fixing the scatter on fly issue ..  the condition below to constraint the cecking for the attacked is on ground or not on any spell except the scatter. 
                    if (spell.ID != 8001)
                    {
                        if (spell.OnlyGround)
                            if (attacked.ContainsFlag(Update.Flags.Fly))
                                return false;
                        if (melee && attacked.ContainsFlag(Update.Flags.Fly))
                            return false;
                    }
                }
                if (spell != null)
                {
                    if (spell.ID == 6010)
                    {
                        if (attacked.ContainsFlag(Update.Flags.Fly))
                            return false;
                    }
                }
                if (spell != null)
                {
                    if (spell.ID == 10381)
                    {
                        if (attacked.ContainsFlag(Update.Flags.Fly))
                            return false;
                    }
                }
                if (spell != null)
                {
                    if (spell.ID == 6000)
                    {
                        if (attacked.ContainsFlag(Update.Flags.Fly))
                            return false;
                    }
                }
                if (spell != null)
                {
                    if (spell.ID == 5030)
                    {
                        if (attacked.ContainsFlag(Update.Flags.Fly))
                            return false;
                    }
                }
                if (spell == null)
                {
                    if (attacked.ContainsFlag(Update.Flags.Fly))
                        return false;
                }
                if (Constants.PKForbiddenMaps.Contains(attacker.Owner.Map.ID))
                {
                    if (attacker.MapID == 1002 && Kernel.GetDistance(attacker.X, attacker.Y, 443, 699) < 18 && attacker.EntityFlag == EntityFlag.Player && attacked.EntityFlag == EntityFlag.Player)
                    {
                        Kernel.canpk = true;
                    }
                    if (Kernel.canpk)
                    {
                        Kernel.canpk = false;
                        attacker.AddFlag(Network.GamePackets.Update.Flags.FlashingName);
                        attacker.FlashingNameStamp = Time32.Now;
                        attacker.FlashingNameTime = 10;
                        return true;
                    }
                    if (!Kernel.canpk && attacker.PKMode == Conquer_Online_Server.Game.Enums.PKMode.PK || attacker.PKMode == Conquer_Online_Server.Game.Enums.PKMode.Team || (spell != null && spell.CanKill))
                    {
                        attacker.Owner.Send(Constants.PKForbidden);
                        attacker.AttackPacket = null;
                        return false;
                    }
                }
                if (attacker.PKMode == Conquer_Online_Server.Game.Enums.PKMode.Capture)
                {
                    if (attacked.ContainsFlag(Update.Flags.FlashingName) || attacked.PKPoints > 99)
                    {
                        return true;
                    }
                }
                if (attacker.PKMode == Conquer_Online_Server.Game.Enums.PKMode.Peace)
                {
                    return false;
                }
                if (attacker.UID == attacked.UID)
                    return false;


                if (attacker.PKMode == Conquer_Online_Server.Game.Enums.PKMode.Team)
                {
                    if (attacker.Owner.Team != null)
                    {
                        if (attacker.Owner.Team.IsTeammate(attacked.UID))
                        {
                            attacker.AttackPacket = null;
                            return false;
                        }
                    }

                    if (attacker.GuildID == attacked.GuildID && attacker.GuildID != 0)
                    {
                        attacker.AttackPacket = null;
                        return false;
                    }
                    if (attacker.ClanId == attacked.ClanId && attacker.ClanId != 0)
                    {
                        attacker.AttackPacket = null;
                        return false;
                    }
                    if (attacker.Owner.Friends.ContainsKey(attacked.UID))
                    {
                        attacker.AttackPacket = null;
                        return false;
                    }
                    if (attacker.Owner.Guild != null)
                    {
                        if (attacker.Owner.Guild.Ally.ContainsKey(attacked.GuildID))
                        {
                            attacker.AttackPacket = null;
                            return false;
                        }
                    }
                    if (attacker.ClanId != 0)
                    {
                        if (attacker.GetClan.Allies.ContainsKey(attacked.ClanId))
                        {
                            attacker.AttackPacket = null;
                            return false;
                        }
                    }
                }

                if (spell != null)
                    if (spell.OnlyGround)
                        if (attacked.ContainsFlag(Update.Flags.Fly))
                            return false;

                if (spell != null)
                    if (!spell.CanKill)
                        return true;

                if (attacker.PKMode != Conquer_Online_Server.Game.Enums.PKMode.PK &&
                    attacker.PKMode != Conquer_Online_Server.Game.Enums.PKMode.Team && attacked.PKPoints < 99)
                {
                    attacker.AttackPacket = null;
                    return false;
                }
                else
                {
                    if (!attacked.ContainsFlag(Update.Flags.FlashingName))
                    {
                        if (!attacked.ContainsFlag(Update.Flags.BlackName))
                        {
                            if (Constants.PKFreeMaps.Contains(attacker.MapID))
                                return true;
                            if (Constants.Damage1Map.Contains(attacker.MapID))
                                return true;
                            if (attacker.Owner.Map.BaseID == 700)
                                return true;
                            attacker.AddFlag(Network.GamePackets.Update.Flags.FlashingName);
                            attacker.FlashingNameStamp = Time32.Now;
                            attacker.FlashingNameTime = 10;
                        }
                    }
                }
                return true;
            }
        }



        public static bool Allyss(Entity attacker, Entity attacked)
        {
            if (attacker.Owner.Team != null)
            {
                if (attacker.Owner.Team.IsTeammate(attacked.UID))
                {
                    attacker.Owner.Send(new Message("Sorry You Can't Attack your Team in Jiang Hu Mode", System.Drawing.Color.Red, Message.TopLeft));
                    attacker.AttackPacket = null; return true;
                }


            }
            return false;
        }

        static void CheckForExtraWeaponPowers(Client.GameClient client, Entity attacked)
        {
            #region Right Hand
            var weapons = client.Weapons;
            if (weapons.Item1 != null)
            {
                if (weapons.Item1.ID != 0)
                {
                    var Item = weapons.Item1;
                    if (Item.Effect != Enums.ItemEffect.None)
                    {
                        if (Kernel.Rate(30))
                        {
                            switch (Item.Effect)
                            {
                                case Enums.ItemEffect.HP:
                                    {
                                        SpellUse spellUse = new SpellUse(true);
                                        spellUse.Attacker = 1;
                                        spellUse.SpellID = 1175;
                                        spellUse.SpellLevel = 4;
                                        spellUse.X = client.Entity.X;
                                        spellUse.Y = client.Entity.Y;
                                        spellUse.AddTarget(client.Entity.UID, 300, null);
                                        uint damage = Math.Min(300, client.Entity.MaxHitpoints - client.Entity.Hitpoints);
                                        client.Entity.Hitpoints += damage;
                                        client.SendScreen(spellUse, true);
                                        break;
                                    }
                                case Enums.ItemEffect.MP:
                                    {
                                        SpellUse spellUse = new SpellUse(true);
                                        spellUse.Attacker = 1;
                                        spellUse.SpellID = 1175;
                                        spellUse.SpellLevel = 2;
                                        spellUse.X = client.Entity.X;
                                        spellUse.Y = client.Entity.Y;
                                        spellUse.AddTarget(client.Entity.UID, 300, null);
                                        ushort damage = (ushort)Math.Min(300, client.Entity.MaxMana - client.Entity.Mana);
                                        client.Entity.Mana += damage;
                                        client.SendScreen(spellUse, true);
                                        break;
                                    }
                                case Enums.ItemEffect.Shield:
                                    {
                                        if (client.Entity.ContainsFlag(Update.Flags.MagicShield))
                                            return;
                                        SpellUse spellUse = new SpellUse(true);
                                        spellUse.Attacker = 1;
                                        spellUse.SpellID = 1020;
                                        spellUse.SpellLevel = 0;
                                        spellUse.X = client.Entity.X;
                                        spellUse.Y = client.Entity.Y;
                                        spellUse.AddTarget(client.Entity.UID, 120, null);
                                        client.Entity.ShieldTime = 0;
                                        client.Entity.ShieldStamp = Time32.Now;
                                        client.Entity.MagicShieldStamp = Time32.Now;
                                        client.Entity.MagicShieldTime = 0;

                                        client.Entity.AddFlag(Update.Flags.MagicShield);
                                        client.Entity.MagicShieldStamp = Time32.Now;
                                        client.Entity.MagicShieldIncrease = 2;
                                        client.Entity.MagicShieldTime = 120;
                                        if (client.Entity.EntityFlag == EntityFlag.Player)
                                            client.Send(Constants.Shield(2, 120));
                                        client.SendScreen(spellUse, true);
                                        break;
                                    }
                                case Enums.ItemEffect.Poison:
                                    {
                                        if (attacked != null)
                                        {
                                            if (Constants.PKForbiddenMaps.Contains(client.Entity.MapID))
                                                return;
                                            if (client.Map.BaseID == 700)
                                                return;
                                            if (attacked.UID == client.Entity.UID)
                                                return;
                                            if (attacked.ToxicFogLeft > 0)
                                                return;
                                            SpellUse spellUse = new SpellUse(true);
                                            spellUse.SpellID = 5040;
                                            spellUse.Attacker = attacked.UID;
                                            spellUse.SpellLevel = 9;
                                            spellUse.X = attacked.X;
                                            spellUse.Y = attacked.Y;
                                            spellUse.AddTarget(attacked.UID, 0, null);
                                            spellUse.Targets[attacked.UID].Hit = true;
                                            attacked.ToxicFogStamp = Time32.Now;
                                            attacked.ToxicFogLeft = 10;
                                            attacked.ToxicFogPercent = 0.05F;
                                            client.SendScreen(spellUse, true);
                                        }
                                        break;
                                    }
                            }
                        }
                    }
                }
            }
            #endregion
            #region Left Hand
            if (weapons.Item2 != null)
            {
                if (weapons.Item2.ID != 0)
                {
                    var Item = weapons.Item2;
                    if (Item.Effect != Enums.ItemEffect.None)
                    {
                        if (Kernel.Rate(30))
                        {
                            switch (Item.Effect)
                            {
                                case Enums.ItemEffect.HP:
                                    {
                                        SpellUse spellUse = new SpellUse(true);
                                        spellUse.Attacker = 1;
                                        spellUse.SpellID = 1175;
                                        spellUse.SpellLevel = 4;
                                        spellUse.X = client.Entity.X;
                                        spellUse.Y = client.Entity.Y;
                                        spellUse.AddTarget(client.Entity.UID, 300, null);
                                        uint damage = Math.Min(300, client.Entity.MaxHitpoints - client.Entity.Hitpoints);
                                        client.Entity.Hitpoints += damage;
                                        client.SendScreen(spellUse, true);
                                        break;
                                    }
                                case Enums.ItemEffect.MP:
                                    {
                                        SpellUse spellUse = new SpellUse(true);
                                        spellUse.Attacker = 1;
                                        spellUse.SpellID = 1175;
                                        spellUse.SpellLevel = 2;
                                        spellUse.X = client.Entity.X;
                                        spellUse.Y = client.Entity.Y;
                                        spellUse.AddTarget(client.Entity.UID, 300, null);
                                        ushort damage = (ushort)Math.Min(300, client.Entity.MaxMana - client.Entity.Mana);
                                        client.Entity.Mana += damage;
                                        client.SendScreen(spellUse, true);
                                        break;
                                    }
                                case Enums.ItemEffect.Shield:
                                    {
                                        if (client.Entity.ContainsFlag(Update.Flags.MagicShield))
                                            return;
                                        SpellUse spellUse = new SpellUse(true);
                                        spellUse.Attacker = 1;
                                        spellUse.SpellID = 1020;
                                        spellUse.SpellLevel = 0;
                                        spellUse.X = client.Entity.X;
                                        spellUse.Y = client.Entity.Y;
                                        spellUse.AddTarget(client.Entity.UID, 120, null);
                                        client.Entity.ShieldTime = 0;
                                        client.Entity.ShieldStamp = Time32.Now;
                                        client.Entity.MagicShieldStamp = Time32.Now;
                                        client.Entity.MagicShieldTime = 0;

                                        client.Entity.AddFlag(Update.Flags.MagicShield);
                                        client.Entity.MagicShieldStamp = Time32.Now;
                                        client.Entity.MagicShieldIncrease = 2;
                                        client.Entity.MagicShieldTime = 120;
                                        if (client.Entity.EntityFlag == EntityFlag.Player)
                                            client.Send(Constants.Shield(2, 120));
                                        client.SendScreen(spellUse, true);
                                        break;
                                    }
                                case Enums.ItemEffect.Poison:
                                    {
                                        if (attacked != null)
                                        {
                                            if (attacked.UID == client.Entity.UID)
                                                return;
                                            if (Constants.PKForbiddenMaps.Contains(client.Entity.MapID))
                                                return;
                                            if (client.Map.BaseID == 700)
                                                return;
                                            if (attacked.ToxicFogLeft > 0)
                                                return;
                                            SpellUse spellUse = new SpellUse(true);
                                            spellUse.SpellID = 5040;
                                            spellUse.Attacker = attacked.UID;
                                            spellUse.SpellLevel = 9;
                                            spellUse.X = attacked.X;
                                            spellUse.Y = attacked.Y;
                                            spellUse.AddTarget(attacked.UID, 0, null);
                                            spellUse.Targets[attacked.UID].Hit = true;
                                            attacked.ToxicFogStamp = Time32.Now;
                                            attacked.ToxicFogLeft = 10;
                                            attacked.ToxicFogPercent = 0.05F;
                                            client.SendScreen(spellUse, true);
                                        }
                                        break;
                                    }
                            }
                        }
                    }
                }
            }
            #endregion
        }
        public void CheckForSuperGems(Client.GameClient client)
        {
            for (uint i = 1; i < 12; i++)
            {
                if (i != 7)
                {
                    ConquerItem item = client.Equipment.TryGetItem(i);
                    if (item != null && item.ID != 0)
                    {
                        if (item.SocketOne != 0)
                        {
                            if (item.SocketOne == Enums.Gem.SuperPhoenixGem)
                            {
                                if (Kernel.Rate(3)) //this is where your chances when to display the phoenix gem effect
                                {
                                    _String str = new _String(true);
                                    str.UID = attacker.UID;
                                    str.TextsCount = 1;
                                    str.Type = _String.Effect;
                                    str.Texts.Add("phoenix");
                                    attacker.Owner.SendScreen(str, true);
                                }
                            }
                            if (item.SocketOne == Enums.Gem.SuperDragonGem)
                            {
                                if (Kernel.Rate(3)) //this is where your chances when to display the phoenix gem effect
                                {
                                    _String str = new _String(true);
                                    str.UID = attacker.UID;
                                    str.TextsCount = 1;
                                    str.Type = _String.Effect;
                                    str.Texts.Add("dragon");
                                    attacker.Owner.SendScreen(str, true);
                                }
                            }
                        }
                    }
                }
            }
        }

        public uint uintDamage { get; set; }
    }
}
