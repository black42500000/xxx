﻿using System;
using Conquer_Online_Server.Network.GamePackets;

using System.Collections.Generic;
using Conquer_Online_Server.Interfaces;
using Conquer_Online_Server.Game.ConquerStructures;

namespace Conquer_Online_Server.Game.Attacking
{
    public class Calculate
    {
        static float
            goldPrizePsyAttackCoefficient = .2f,
            goldPrizePsyDefenceCoefficient = .2f,
            goldPrizeMagAttackCoefficient = .2f,
            goldPrizeMagDefenceCoefficient = .2f;

        static float reduceValue = 0.6f;
        public static uint Melee(Entity attacker, Entity attacked, ref Attack Packet)
        {
            if (attacker.EntityFlag == EntityFlag.Player)
            {
                GemEffect.Effect(attacker);
            }
            if (attacked.EntityFlag == EntityFlag.Player)
            {
                BlessEffect.Effect(attacked);
            }
            if (attacker.MapID == DeathMatch.MAPID) return 1;
            long Damage = 0;
            Boolean CritImmune = false;


            if (attacker.EntityFlag == EntityFlag.Monster)
                if (attacked.EntityFlag == EntityFlag.Player)
                    if (Kernel.Rate(Math.Min(60, attacked.Dodge + 30)))
                        return 0;

            Durability(attacker, attacked, null);
            if (attacked.Name.Contains("[GM]") || attacked.Name.Contains("[PM]"))
                return 1;  

            if (attacked.ContainsFlag(Network.GamePackets.Update.Flags.ShurikenVortex))
                return 1;
            if (!attacker.Transformed)
                Damage = Kernel.Random.Next((int)Math.Min((long)attacker.MinAttack, (long)attacker.MaxAttack), (int)Math.Max((long)attacker.MinAttack, (long)attacker.MaxAttack) + 1);
            else
                Damage = Kernel.Random.Next((int)attacker.TransformationMinAttack, (int)attacker.TransformationMaxAttack + 1);
            if (attacker.WearsGoldPrize) Damage += (long)(Damage * goldPrizePsyAttackCoefficient);
            if (attacked.WearsGoldPrize) Damage -= (long)(Damage * goldPrizePsyDefenceCoefficient);
            if (attacker.EntityFlag == EntityFlag.Player)
            {
                //if (attacker.BattlePower < attacked.BattlePower)
                canBT = true;
            }
            else canBT = false;
            /*if (canBT)
                        {
                            if (canBT)
                            {
                                if (Kernel.Rate((float)attacked.Counteraction / 100f))
                                    canBT = false;
                            }
                            if (canBT)
                            {
                                if (Kernel.Rate((float)attacker.Breaktrough / 100f))
                                {
                                    Damage = (Int32)attacker.MaxAttack + 3000;
                                    Packet.Effect1 |= Attack.AttackEffects1.Penetration;
                                }
                            }
                        }*/

            if (attacker.ContainsFlag(Network.GamePackets.Update.Flags.Stigma))
                if (!attacker.Transformed && Damage > 1)
                    Damage = (long)(Damage * 1.30);

            if (attacked.EntityFlag == EntityFlag.Monster)
            {
                if (attacked.MapID < 1351 || attacked.MapID > 1354)
                    Damage = (long)(Damage * (1 + (GetLevelBonus(attacker.Level, attacked.Level) * 0.08)));
            }
            if (attacker.EntityFlag == EntityFlag.Player)
            {
                if (attacked.EntityFlag == EntityFlag.Monster)
                {
                    if (attacked.MapID < 1351 || attacked.MapID > 1354)
                        if (!attacker.Owner.Equipment.Free(4) && !attacker.Owner.Equipment.Free(5))
                            Damage = (long)(Damage * 1.5);
                }
                if (attacked.EntityFlag == EntityFlag.Monster)
                    if (attacked.MapID < 1351 || attacked.MapID > 1354)
                        Damage = (long)(Damage * AttackMultiplier(attacker, attacked));

                if (attacker.OnSuperman())
                    if (attacked.EntityFlag == EntityFlag.Monster)
                        if (!attacked.MonsterInfo.Boss)
                            Damage *= 10;
                if (attacked.Name == "GoldenOctopus")
                {
                    return (uint)Damage / 9000;
                }
                if (attacker.OnFatalStrike())
                    if (attacked.EntityFlag == EntityFlag.Monster)
                        if (!attacked.MonsterInfo.Boss)
                            Damage *= 5;
                        else Damage += (long)(Damage * .1);
            }
            if (!attacked.Transformed)
            {
                if (attacked.ContainsFlag(Network.GamePackets.Update.Flags.MagicShield))
                {
                    if (attacked.ShieldTime > 0)
                        Damage -= attacked.Defence;
                    else
                        Damage -= (ushort)(attacked.Defence * attacked.MagicShieldIncrease);
                }
                else
                {
                    Damage -= attacked.Defence;
                }
            }
            else
                Damage -= attacked.TransformationDefence;

            if (attacked.IsDefensiveStance)
            {
                if (attacked.FatigueSecs > 120)
                {
                    Damage -= attacked.Defence + (long)(attacked.Defence * 0.40);
                }
                else
                {
                    Damage -= attacked.Defence + (long)(attacked.Defence * 0.20);
                }
            }


            if (Kernel.Rate(5))
            {
                if (attacker.EntityFlag == EntityFlag.Player)
                {
                    if (attacker.Owner.BlessTime > 0)
                    {
                        Damage *= 2;
                        _String str = new _String(true);
                        str.UID = attacker.UID;
                        str.TextsCount = 1;
                        str.Type = _String.Effect;
                        str.Texts.Add("LuckyGuy");
                        attacker.Owner.SendScreen(str, true);
                    }
                }
            }

            if (Kernel.Rate(5))
            {
                if (attacked.EntityFlag == EntityFlag.Player)
                {
                    if (attacked.Owner.BlessTime > 0)
                    {
                        Damage = 1;
                        _String str = new _String(true);
                        str.UID = attacker.UID;
                        str.TextsCount = 1;
                        str.Type = _String.Effect;
                        str.Texts.Add("LuckyGuy");
                        attacked.Owner.SendScreen(str, true);
                    }
                }
            }

            Damage = RemoveExcessDamage(Damage, attacker, attacked);

            if (attacker.Assassin())
            {
                Damage = Damage * 15 / 100;
                if (attacked.EntityFlag == EntityFlag.Player)
                    if (attacked.Class >= 100)
                        Damage = Damage * 30 / 100;
            }


            Damage += attacker.PhysicalDamageIncrease;
            Damage -= attacked.PhysicalDamageDecrease;
            if (attacker.EntityFlag == EntityFlag.Player)
            {
                if (!CritImmune)
                {
                    if (Kernel.Rate((float)attacker.CriticalStrike / 100f))
                    {
                        Packet.Effect1 |= Attack.AttackEffects1.CriticalStrike;
                        Damage = (Int32)Math.Floor((float)Damage * 1.1);
                    }
                }
            }
            if (attacked.EntityFlag == EntityFlag.Player)
            {
                if (Kernel.Rate((float)attacked.Block / 100f))
                {
                    Packet.Effect1 |= Attack.AttackEffects1.Block;
                    Damage = (Int32)Math.Floor((float)Damage / 2);
                }
            }
            try
            {
                if (attacked.EntityFlag == EntityFlag.Player && (attacker.BattlePower < attacked.BattlePower))
                {
                    if (attacked.Owner.Challenge == null)
                    {
                        int sub = attacked.BattlePower - attacker.BattlePower;
                        //sub *= 2000;//##Samak BattlePower
                        if (sub > 41)
                        {
                            Damage = 1;
                        }
                        else
                        {//Samak
                            Damage = (Int32)Math.Floor((float)Damage * (.50 - (sub * 0.01)));
                        }
                    }
                }
                if (attacked.EntityFlag == EntityFlag.Player && (attacker.BattlePower > attacked.BattlePower))
                {

                    Damage = (Int32)Math.Floor((float)Damage * (1.20));
                }
            }
            catch (Exception) { }

            //Ahmed Samak handles azureshield
            if (attacked.ContainsFlag2(Network.GamePackets.Update.Flags2.AzureShield))
            {

                if (attacked.AzureDamage >= Damage)
                {
                    // System.Console.WriteLine("^^^^Damage is " + Damage.ToString() + " Azure is : " + attacked.AzureDamage.ToString());
                    attacked.AzureDamage -= (int)Damage;
                    int sec = 60 - (Time32.Now - attacked.MagicShieldStamp).AllSeconds();
                    attacked.Owner.Send(Constants.Shield(attacked.AzureDamage, sec));
                    // System.Console.WriteLine("^^^^Damage is " + Damage.ToString() + " Azure is : " + attacked.AzureDamage.ToString());
                    return 0;
                }
                else
                {
                    //System.Console.WriteLine("XXXXDamage is " + Damage.ToString() + " Azure is : " + attacked.AzureDamage.ToString());
                    Damage -= attacked.AzureDamage;
                    attacked.AzureDamage = 0;
                    attacked.RemoveFlag2(Update.Flags2.AzureShield);
                    //System.Console.WriteLine("XXXXDamage is " + Damage.ToString() + " Azure is : " + attacked.AzureDamage.ToString());
                    return (uint)(Damage * reduceValue);

                }

            }

            ///////////////////////////////

            if (attacked.EntityFlag == EntityFlag.Monster)
            {
                if (Damage >= 700 * attacked.MaxHitpoints)
                    Damage = (long)(700 * attacked.MaxHitpoints);
            }
            /*if (attacker.NobilityRank == Conquer_Online_Server.Game.ConquerStructures.NobilityRank.King)
            {
                Damage = Damage + 7000;
            }
            if (attacker.NobilityRank == Conquer_Online_Server.Game.ConquerStructures.NobilityRank.Prince)
            {
                Damage = Damage + 1500;
            }
            if (attacker.NobilityRank == Conquer_Online_Server.Game.ConquerStructures.NobilityRank.Duke)
            {
                Damage = Damage + 500;
            }*/
            Damage += Damage;
            Damage += Damage / 4;
            if (Damage <= 0)
                Damage = 1;

            //if (Damage > 65000)
            //Damage = 65000;
            // chec
            if (attacker.Class > 140)
                Damage = 1;

            AutoRespone(attacker, attacked, ref Damage);
            if (Constants.Damage1Map.Contains(attacker.MapID))
                Damage = 1;
            if (attacked.Class >= 140 && attacked.Class <= 145)
                Damage = (long)(Damage - (Damage * 0.10));
            if (attacker.MapID == 2065)
                if (attacker.EntityFlag == EntityFlag.Monster)
                    Damage *= 10;
                else
                    Damage /= 10;
            return (uint)(Damage * reduceValue);
        }

        public static uint Melee(Entity attacker, Entity attacked, Database.SpellInformation spell, ref Attack Packet)
        {
            if (attacker.EntityFlag == EntityFlag.Player)
            {
                GemEffect.Effect(attacker);
            }
            if (attacker.MapID == DeathMatch.MAPID) return 1;
            long Damage = 0;
            if (Constants.FBandSSEvent.Contains(attacker.MapID))
            {
                Damage = (int)attacked.Hitpoints + 1;
                return (uint)Damage;
            }
            Boolean CritImmune = false;
            Boolean canBT = false;

            if (attacker.EntityFlag == EntityFlag.Monster)
                if (attacked.EntityFlag == EntityFlag.Player)
                    if (Kernel.Rate(Math.Min(60, attacked.Dodge + 30)))
                        return 0;

            Durability(attacker, attacked, null);
            if (attacked.Name.Contains("[GM]") || attacked.Name.Contains("[PM]"))
                return 1;  
            if (attacked.ContainsFlag(Network.GamePackets.Update.Flags.ShurikenVortex))
                return 1;
            if (!attacker.Transformed)
                Damage = Kernel.Random.Next((int)Math.Min((long)attacker.MinAttack, (long)attacker.MaxAttack), (int)Math.Max((long)attacker.MinAttack, (long)attacker.MaxAttack) + 1);
            else
                Damage = Kernel.Random.Next((int)attacker.TransformationMinAttack, (int)attacker.TransformationMaxAttack + 1);
            if (attacker.WearsGoldPrize) Damage += (long)(Damage * goldPrizePsyAttackCoefficient);
            if (attacked.WearsGoldPrize) Damage -= (long)(Damage * goldPrizePsyDefenceCoefficient);
            if (attacker.EntityFlag == EntityFlag.Player)
            {
                //if (attacker.BattlePower < attacked.BattlePower)
                canBT = true;
            }
            else canBT = false;
            /*if (canBT)
            {
                if (canBT)
                {
                    if (Kernel.Rate((float)attacked.Counteraction / 100f))
                        canBT = false;
                }
                if (canBT)
                {
                    if (Kernel.Rate((float)attacker.Breaktrough / 100f))
                    {
                        Damage = (Int32)attacker.MaxAttack + 3000;
                        Packet.Effect1 |= Attack.AttackEffects1.Penetration;
                    }
                }
            }*/
            if (attacker.ContainsFlag(Network.GamePackets.Update.Flags.Stigma))
                if (!attacker.Transformed && Damage > 1)
                    Damage = (long)(Damage * 1.30);

            if (attacked.EntityFlag == EntityFlag.Monster)
            {
                if (attacked.MapID < 1351 || attacked.MapID > 1354)
                    Damage = (long)(Damage * (1 + (GetLevelBonus(attacker.Level, attacked.Level) * 0.08)));
            }
            if (attacker.EntityFlag == EntityFlag.Player)
            {
                if (attacked.EntityFlag == EntityFlag.Monster)
                {
                    if (attacked.MapID < 1351 || attacked.MapID > 1354)
                        if (!attacker.Owner.Equipment.Free(4) && !attacker.Owner.Equipment.Free(5))
                            Damage = (long)(Damage * 1.5);
                }
                if (attacked.EntityFlag == EntityFlag.Monster)
                    if (attacked.MapID < 1351 || attacked.MapID > 1354)
                        Damage = (long)(Damage * AttackMultiplier(attacker, attacked));
                if (attacker.OnSuperman())
                    if (attacked.EntityFlag == EntityFlag.Monster)
                        if (!attacked.MonsterInfo.Boss)
                            Damage *= 10;

                if (attacker.OnFatalStrike())
                    if (attacked.EntityFlag == EntityFlag.Monster)
                        if (!attacked.MonsterInfo.Boss)
                            Damage *= 5;
                        else Damage += (long)(Damage * .1);
            }
            if (!attacked.Transformed)
            {
                if (attacked.ContainsFlag(Network.GamePackets.Update.Flags.MagicShield))
                {
                    if (attacked.ShieldTime > 0)
                        Damage -= attacked.Defence;
                    else
                        Damage -= (ushort)(attacked.Defence * attacked.MagicShieldIncrease);
                }
                else
                {
                    Damage -= attacked.Defence;
                }
            }
            else
                Damage -= attacked.TransformationDefence;

            if (attacked.IsDefensiveStance)
            {
                if (attacked.FatigueSecs > 120)
                {
                    Damage -= attacked.Defence + (long)(attacked.Defence * 0.40);
                }
                else
                {
                    Damage -= attacked.Defence + (long)(attacked.Defence * 0.20);
                }
            }


            if (Kernel.Rate(5))
            {
                if (attacker.EntityFlag == EntityFlag.Player)
                {
                    if (attacker.Owner.BlessTime > 0)
                    {
                        Damage *= 2;
                        _String str = new _String(true);
                        str.UID = attacker.UID;
                        str.TextsCount = 1;
                        str.Type = _String.Effect;
                        str.Texts.Add("LuckyGuy");
                        attacker.Owner.SendScreen(str, true);
                    }
                }
            }

            if (Kernel.Rate(5))
            {
                if (attacked.EntityFlag == EntityFlag.Player)
                {
                    if (attacked.Owner.BlessTime > 0)
                    {
                        Damage = 1;
                        _String str = new _String(true);
                        str.UID = attacker.UID;
                        str.TextsCount = 1;
                        str.Type = _String.Effect;
                        str.Texts.Add("LuckyGuy");
                        attacked.Owner.SendScreen(str, true);
                    }
                }
            }

            if (spell.ID == 6000 && attacked.EntityFlag == EntityFlag.Monster)
            {
                if (spell.PowerPercent != 0)
                    Damage = (long)(Damage * spell.PowerPercent);
            }
            else if (spell.ID != 6000)
            {
                if (spell.PowerPercent != 0)
                    Damage = (long)(Damage * spell.PowerPercent);
            }
            #region Attack
            if (attacked.EntityFlag == EntityFlag.Player)
            {
                if (canBT)
                {
                    if (Kernel.Rate((float)attacker.Breaktrough / 10f))
                    {
                        Packet.Effect1 |= Attack.AttackEffects1.Penetration;
                        Damage = (Int32)Math.Floor((float)Damage * 1.6);
                    }
                }

            }
            #endregion
            Damage = RemoveExcessDamage(Damage, attacker, attacked);
            Damage += attacker.PhysicalDamageIncrease;
            Damage -= attacked.PhysicalDamageDecrease;
            if (attacker.EntityFlag == EntityFlag.Player)
            {
                if (!CritImmune)
                {
                    if (Kernel.Rate((float)attacker.CriticalStrike / 100f))
                    {
                        Packet.Effect1 |= Attack.AttackEffects1.CriticalStrike;
                        Damage = (Int32)Math.Floor((float)Damage * 1.1);
                    }
                }
            }
            if (attacked.EntityFlag == EntityFlag.Player)
            {
                if (Kernel.Rate((float)attacked.Block / 100f))
                {
                    Packet.Effect1 |= Attack.AttackEffects1.Block;
                    Damage = (Int32)Math.Floor((float)Damage / 2);
                }
            }
            try
            {
                if (attacked.EntityFlag == EntityFlag.Player && (attacker.BattlePower < attacked.BattlePower))
                {
                    if (attacked.Owner.Challenge == null)
                    {
                        int sub = attacked.BattlePower - attacker.BattlePower;
                        //sub *= 2000;//##Samak BattlePower
                        if (sub > 41)
                        {
                            Damage = 1;
                        }
                        else
                        {//Samak
                            Damage = (Int32)Math.Floor((float)Damage * (.50 - (sub * 0.01)));
                        }
                    }
                }
                if (attacked.EntityFlag == EntityFlag.Player && (attacker.BattlePower > attacked.BattlePower))
                {

                    Damage = (Int32)Math.Floor((float)Damage * (1.20));
                }
            }
            catch (Exception) { }

            //Ahmed Samak handles azureshield
            if (attacked.ContainsFlag2(Network.GamePackets.Update.Flags2.AzureShield))
            {

                if (attacked.AzureDamage >= Damage)
                {
                    // System.Console.WriteLine("^^^^Damage is " + Damage.ToString() + " Azure is : " + attacked.AzureDamage.ToString());
                    attacked.AzureDamage -= (int)Damage;
                    int sec = 60 - (Time32.Now - attacked.MagicShieldStamp).AllSeconds();
                    attacked.Owner.Send(Constants.Shield(attacked.AzureDamage, sec));
                    // System.Console.WriteLine("^^^^Damage is " + Damage.ToString() + " Azure is : " + attacked.AzureDamage.ToString());
                    return 0;
                }
                else
                {
                    //System.Console.WriteLine("XXXXDamage is " + Damage.ToString() + " Azure is : " + attacked.AzureDamage.ToString());
                    Damage -= attacked.AzureDamage;
                    attacked.AzureDamage = 0;
                    attacked.RemoveFlag2(Update.Flags2.AzureShield);
                    //System.Console.WriteLine("XXXXDamage is " + Damage.ToString() + " Azure is : " + attacked.AzureDamage.ToString());
                    return (uint)(Damage * reduceValue);

                }

            }

            ///////////////////////////////

            if (attacked.EntityFlag == EntityFlag.Monster)
            {
                if (Damage >= 700 * attacked.MaxHitpoints)
                    Damage = (long)(700 * attacked.MaxHitpoints);
            }
            /*if (attacker.NobilityRank == Conquer_Online_Server.Game.ConquerStructures.NobilityRank.King)
            {
                Damage = Damage + 7000;
            }
            if (attacker.NobilityRank == Conquer_Online_Server.Game.ConquerStructures.NobilityRank.Prince)
            {
                Damage = Damage + 1500;
            }
            if (attacker.NobilityRank == Conquer_Online_Server.Game.ConquerStructures.NobilityRank.Duke)
            {
                Damage = Damage + 500;
            }*/
            Damage += Damage;
            Damage += Damage / 4;
            if (Damage <= 0)
                Damage = 1;

            //if (Damage > 65000)
            //Damage = 65000;
            // chec
            if (attacker.Class > 140)
                Damage = 1;

            AutoRespone(attacker, attacked, ref Damage);
            if (Constants.Damage1Map.Contains(attacker.MapID))
                Damage = 1;
            if (attacked.Class >= 140 && attacked.Class <= 145)
                Damage = (long)(Damage - (Damage * 0.10));
            return (uint)(Damage * reduceValue);
        }
        public static uint Melee(Entity attacker, SobNpcSpawn attacked, ref Attack Packet)
        {
            if (attacker.EntityFlag == EntityFlag.Player)
            {
                GemEffect.Effect(attacker);
            }
            if (attacker.MapID == DeathMatch.MAPID) return 1;
            long Damage = 0;
            Boolean CritImmune = false;

            Durability(attacker, null, null);
            if (attacked.Name.Contains("[GM]") || attacked.Name.Contains("[PM]"))
                return 1;  
            if (!attacker.Transformed)
                Damage = Kernel.Random.Next((int)Math.Min((long)attacker.MinAttack, (long)attacker.MaxAttack), (int)Math.Max((long)attacker.MinAttack, (long)attacker.MaxAttack) + 1);
            else
                Damage = Kernel.Random.Next((int)attacker.TransformationMinAttack, (int)attacker.TransformationMaxAttack + 1);
            if (attacker.WearsGoldPrize) Damage += (long)(Damage * goldPrizePsyAttackCoefficient);
            if (attacker.ContainsFlag(Network.GamePackets.Update.Flags.Stigma))
                if (!attacker.Transformed)
                    Damage = (long)(Damage * 1.30);
            Damage += attacker.PhysicalDamageIncrease;

            if (Kernel.Rate(5))
            {
                if (attacker.EntityFlag == EntityFlag.Player)
                {
                    if (attacker.Owner.BlessTime > 0)
                    {
                        Damage *= 2;
                        _String str = new _String(true);
                        str.UID = attacker.UID;
                        str.TextsCount = 1;
                        str.Type = _String.Effect;
                        str.Texts.Add("LuckyGuy");
                        attacker.Owner.SendScreen(str, true);
                    }
                }
            }
            if (attacker.EntityFlag == EntityFlag.Player)
            {
                if (!CritImmune)
                {
                    if (Kernel.Rate((float)attacker.CriticalStrike / 100f))
                    {
                        Packet.Effect1 |= Attack.AttackEffects1.CriticalStrike;
                        Damage = (Int32)Math.Floor((float)Damage * 1.2);
                    }
                }
            }
            Damage = Damage * 2;
            if (Damage <= 0)
                Damage = 1;
            if (Constants.Damage1Map.Contains(attacker.MapID))
                Damage = 1;

            return (uint)(Damage * reduceValue);
        }
        public static bool Miss(int Percent)
        {
            if (Percent >= 100)
                return false;

            return Kernel.Rate(Percent, 100);
        }
        public static uint Magic(Entity Attacker, Entity Attacked, Database.SpellInformation SInfo, ref Attack Packet)
        {
            if (Attacked.Name.Contains("[GM]") || Attacked.Name.Contains("[PM]"))
                return 1;  
            if (Attacker.MapID == DeathMatch.MAPID) return 1;
            switch (Attacked.EntityFlag)
            {
                case EntityFlag.Player:
                    {
                        if (Attacker.EntityFlag == EntityFlag.Player)
                        {
                            GemEffect.Effect(Attacker);
                        }
                        if (Attacked.EntityFlag == EntityFlag.Player)
                        {
                            BlessEffect.Effect(Attacked);
                        }
                        long Damage = 0;
                        Int32 Defence = 0;
                        bool CritImmune = false;
                        if (Kernel.Rate((float)Attacked.Immunity / 100f))
                            CritImmune = true;
                        Int32 Fan = 0, Tower = 0;
                        if (Attacker.EntityFlag == EntityFlag.Player)
                            Fan = Attacker.getFan(true);
                        if (Attacked.EntityFlag == EntityFlag.Player)
                            Tower = Attacked.getTower(true);

                        if (Attacked.EntityFlag == EntityFlag.Player)
                            if (Attacked.ContainsFlag(Network.GamePackets.Update.Flags.ShurikenVortex))
                                return 1;

                        if (Miss(SInfo.Percent))
                            return 0;

                        Defence = (Int32)Attacked.MagicDefence;
                        if (Attacker.Penetration > 0)
                            Defence -= (ushort)((float)Defence / 100 * (Attacker.Penetration / 100));
                        Damage = (Int32)Attacker.BaseMagicAttack;
                        Damage += (Int32)SInfo.Power;

                        //
                        if (Attacker.MonsterInfo != null)
                            if (Attacker.MonsterInfo.Name == "[Guard1]")
                                Damage = 64000 * 5;
                        if (Attacker.WearsGoldPrize) Damage += (long)(Damage * goldPrizeMagAttackCoefficient);
                        if (Attacked.WearsGoldPrize) Damage -= (long)(Damage * goldPrizeMagDefenceCoefficient);
                        //if (Attacked.Hitpoints <= Damage)
                        //{
                        //   Attacked.Die(Attacker);
                        // }

                        if (Attacked.EntityFlag == EntityFlag.Player)
                        {
                            if (Attacked.Reborn == 1)
                                Damage = (Int32)(Damage * 0.69);
                            if (Attacked.Reborn == 2)
                                Damage = (Int32)(Damage * 0.49);
                        }
                        Damage += Fan;
                        if (Attacker.EntityFlag == EntityFlag.Player)
                            Damage += Attacker.MagicDamageIncrease;

                        if (Attacker.EntityFlag == EntityFlag.Player)
                        {
                            if (Damage > 0)
                            {
                                Damage += (Int32)(Math.Floor((Double)Damage * Attacker.GemBonus(ItemSocket.Phoenix)));
                            }
                        }
                        //UInt32 bpdmage = 0, bpdefence = 0;
                        //if (Attacked.EntityFlag == EntityFlag.Player)
                        //bpdefence = (UInt32)(Attacked.BattlePower * 3.5);
                        //Damage -= (long)bpdefence;
                        //Damage -= Attacked.MagicDamageDecrease;
                        Damage -= Defence;
                        Damage -= Tower;
                        //if (Damage <= 1) Damage = 1;

                        //return (UInt32)Damage;

                        //Ahmed Samak : fix the KounterKill issue the orignal block as shown below
                        //if (Damage <= 1) Damage = 1;
                        //return (UInt32)Damage;
                        //the new block offers us the behavior of the melee response.

                        if (!CritImmune)
                        {
                            if (Kernel.Rate((float)Attacker.SkillCStrike / 100f))
                            {
                                Packet.Effect1 |= Attack.AttackEffects1.CriticalStrike;
                                Damage = (Int32)Math.Floor((float)Damage * 1.50);
                            }
                        }
                        if (Attacked.EntityFlag == EntityFlag.Player)
                        {
                            if (Kernel.Rate((float)Attacked.Block / 100f))
                            {
                                Packet.Effect1 |= Attack.AttackEffects1.Block;
                                Damage = (Int32)Math.Floor((float)Damage / 2);
                            }
                        }
                        if (Attacked.EntityFlag == EntityFlag.Player)
                        {
                            if (Damage > 0)
                            {
                                Damage -= (Int32)(Math.Floor((float)Damage * Attacked.GemBonus(ItemSocket.Tortoise)));
                            }
                        }
                        if (Damage <= 0)
                            Damage = 1;

                        Damage = Damage / 2;

                        AutoRespone(Attacker, Attacked, ref Damage);
                        if (Constants.Damage1Map.Contains(Attacker.MapID))
                            Damage = 1;
                        //Ahmed Samak handles azureshield
                        //System.Console.WriteLine(Damage.ToString() + "       7");
                        if (Attacked.ContainsFlag2(Network.GamePackets.Update.Flags2.AzureShield))
                        {

                            if (Attacked.AzureDamage >= Damage)
                            {
                                // System.Console.WriteLine("^^^^Damage is " + Damage.ToString() + " Azure is : " + Attacked.AzureDamage.ToString());
                                Attacked.AzureDamage -= (int)Damage;
                                int sec = 60 - (Time32.Now - Attacked.MagicShieldStamp).AllSeconds();
                                Attacked.Owner.Send(Constants.Shield(Attacked.AzureDamage, sec));
                                //System.Console.WriteLine("^^^^Damage is " + Damage.ToString() + " Azure is : " + Attacked.AzureDamage.ToString());
                                return 0;
                            }
                            else
                            {
                                //System.Console.WriteLine("XXXXDamage is " + Damage.ToString() + " Azure is : " + Attacked.AzureDamage.ToString());
                                Damage -= Attacked.AzureDamage;
                                Attacked.AzureDamage = 0;
                                Attacked.RemoveFlag2(Update.Flags2.AzureShield);
                                // System.Console.WriteLine("XXXXDamage is " + Damage.ToString() + " Azure is : " + Attacked.AzureDamage.ToString());
                                //System.Console.WriteLine(Damage.ToString() + "       8");
                                return (uint)Damage;

                            }

                        }
                        if (Constants.Damage1Map.Contains(Attacker.MapID))
                            Damage = 1;
                        if (Attacked.HasMagicDefender && (Attacker.BattlePower - Attacked.BattlePower) <= 3)
                        {
                            Damage = 0;
                            Packet.Effect1 = Attack.AttackEffects1.Immu;
                        }
                        else
                            Attacked.RemoveMagicDefender();
                        return (uint)Damage;
                    }
                case EntityFlag.Monster:
                    {
                        long Damage = 0;
                        Int32 Defence = 0;

                        Int32 Fan = 0;
                        if (Attacker.EntityFlag == EntityFlag.Player)
                            Fan = Attacker.getFan(true);

                        if (Miss(SInfo.Percent))
                            return 0;

                        Defence = Attacked.MonsterInfo.Level * 2;

                        Damage = (Int32)Attacker.BaseMagicAttack;
                        Damage += (Int32)SInfo.Power;
                        if (Attacker.WearsGoldPrize) Damage += (long)(Damage * goldPrizeMagAttackCoefficient);

                        if (Attacker.MonsterInfo != null)
                            if (Attacker.MonsterInfo.Name == "[Guard1]")
                                Damage = 64000 * 5;
                        if (Attacked.Hitpoints <= Damage)
                        {
                            Attacked.Die(Attacker);
                            Attacked.IsDropped = true;
                        }
                        Damage += Fan;
                        if (Attacker.EntityFlag == EntityFlag.Player)
                            Damage += Attacker.MagicDamageIncrease;


                        if (Attacker.EntityFlag == EntityFlag.Player)
                        {
                            if (Damage > 0)
                            {
                                Damage += (Int32)(Math.Floor((Double)Damage * Attacker.GemBonus(ItemSocket.Phoenix)));
                            }
                        }
                        //UInt32 bpdamage = 0;
                        //if (Attacker.EntityFlag == EntityFlag.Player)
                        //bpdamage = (UInt32)(Attacker.BattlePower * 3.5);

                        Damage *= (long)(2.5);

                        //Damage += (long)bpdamage;
                        Damage -= Defence;
                        /*if (!CritImmune)
                        {
                            if (Kernel.Rate((float)Attacker.SkillCStrike / 100f))
                            {
                                Packet.Effect1 |= Attack.AttackEffects1.CriticalStrike;
                                Damage = (Int32)Math.Floor((float)Damage * 2.0);
                            }
                        }*/


                        if (Damage <= 1) Damage = 1;
                        if (Constants.Damage1Map.Contains(Attacker.MapID))
                            Damage = 1;

                        return (UInt32)Damage;
                    }
            }
            return 0;
        }
        public static uint Magic(Entity attacker, Entity attacked, ushort spellID, byte spellLevel, ref Attack Packet)
        {
            if (attacker.MapID == DeathMatch.MAPID) return 1;
            if (Database.SpellTable.SpellInformations.ContainsKey(spellID))
            {
                Database.SpellInformation spell = Database.SpellTable.SpellInformations[spellID][spellLevel];
                return Magic(attacker, attacked, spell, ref Packet);
            }
            return 0;
        }
        public static uint Magic(Entity attacker, SobNpcSpawn attacked, Database.SpellInformation spell, ref Attack Packet)
        {
            if (attacked.Name.Contains("[GM]") || attacked.Name.Contains("[PM]"))
                return 1; 
            if (attacker.MapID == DeathMatch.MAPID) return 1;
            if (spell != null)
                if (!Kernel.Rate(spell.Percent))
                    return 0;
            if (spell != null)
                Durability(attacker, null, spell);
            if (attacker.Transformed)
                return 0;

            long Damage = 0;
            Damage = (long)attacker.MagicAttack;
            if (spell != null)
                Damage += spell.Power;
            if (attacker.WearsGoldPrize) Damage += (long)(Damage * goldPrizeMagAttackCoefficient);
            if (Kernel.Rate(5))
            {
                if (attacker.EntityFlag == EntityFlag.Player)
                {
                    if (attacker.Owner.BlessTime > 0)
                    {
                        Damage *= 2;
                        _String str = new _String(true);
                        str.UID = attacker.UID;
                        str.TextsCount = 1;
                        str.Type = _String.Effect;
                        str.Texts.Add("LuckyGuy");
                        attacker.Owner.SendScreen(str, true);
                    }
                }
            }

            if (attacker.EntityFlag == EntityFlag.Player)
            {
                if (Damage > 0)
                {
                    Damage += (Int32)(Math.Floor((Double)Damage * attacker.GemBonus(ItemSocket.Phoenix)));
                }
            }
            if (attacker.Class > 100)
                Damage -= Damage * 7 / 10;
            Damage += attacker.MagicDamageIncrease;
            /*if (Kernel.Rate((float)attacker.SkillCStrike / 100f))
            {
                Packet.Effect1 |= Attack.AttackEffects1.CriticalStrike;
                Damage = (Int32)Math.Floor((float)Damage * 2.0);
            }*/
            if (Damage <= 0)
                Damage = 1;
            if (Constants.Damage1Map.Contains(attacker.MapID))
                Damage = 1;
            if (attacker.MonsterInfo != null)
                if (attacker.MonsterInfo.Name == "[Guard1]")
                    Damage = (long)attacked.MaxHitpoints + 1;
            if (Constants.Damage1Map.Contains(attacker.MapID))
                Damage = 1;

            return (uint)Damage;
        }

        public static uint Ranged(Entity attacker, Entity attacked, ref Attack Packet)
        {
            if (attacker.EntityFlag == EntityFlag.Player)
            {
                GemEffect.Effect(attacker);
            }
            if (attacked.EntityFlag == EntityFlag.Player)
            {
                BlessEffect.Effect(attacked);
            }
            if (attacker.MapID == DeathMatch.MAPID) return 1;
            long Damage = 0;
            Durability(attacker, attacked, null);
            if (attacked.Name.Contains("[GM]") || attacked.Name.Contains("[PM]"))
                return 1; 
            if (attacked.ContainsFlag(Network.GamePackets.Update.Flags.ShurikenVortex))
                return 1;
            if (attacker.Transformed)
                return 0;

            Damage = Kernel.Random.Next((int)Math.Min(attacker.MinAttack, attacker.MaxAttack), (int)Math.Max(attacker.MinAttack, attacker.MaxAttack) + 1);
            if (attacker.WearsGoldPrize) Damage += (long)(Damage * goldPrizePsyAttackCoefficient);
            if (attacked.WearsGoldPrize) Damage -= (long)(Damage * goldPrizePsyDefenceCoefficient);


            if (attacker.EntityFlag == EntityFlag.Player)
            {
                //if (attacker.BattlePower > attacked.BattlePower)
            }
            /*if (canBT)
            {
                if (canBT)
                {
                    if (Kernel.Rate((float)attacked.Counteraction / 10f))
                        canBT = false;
                }
                if (canBT)
                {
                    if (Kernel.Rate((float)attacker.Breaktrough / 10f))
                    {
                        Damage = (Int32)attacker.MaxAttack + 5000;
                        Packet.Effect1 |= Attack.AttackEffects1.Penetration;
                    }
                }
            }*/
            if (attacker.OnSuperman())
            {
                if (attacked.EntityFlag == EntityFlag.Monster)
                {
                    if (!attacked.MonsterInfo.Boss)
                        Damage *= 10;

                }

            }

            if (attacker.OnFatalStrike())
                if (attacked.EntityFlag == EntityFlag.Monster)
                    Damage *= 5;

            if (!attacked.Transformed)
                Damage -= attacked.Defence;
            else
                Damage -= attacked.TransformationDefence;

            Damage -= Damage * attacked.ItemBless / 100;

            byte dodge = attacked.Dodge;
            if (dodge > 100)
                dodge = 99;
            if (!attacked.Transformed)
                Damage -= Damage * dodge / 100;
            else
                Damage -= Damage * attacked.TransformationDodge / 100;

            if (attacker.OnIntensify && Time32.Now >= attacker.IntensifyStamp.AddSeconds(4))
            {
                Damage *= 2;
                attacker.OnIntensify = false;
            }

            if (attacker.ContainsFlag(Network.GamePackets.Update.Flags.Stigma))
                if (!attacker.Transformed)
                    Damage = (long)(Damage * 1.30);

            if (attacked.EntityFlag == EntityFlag.Monster)
            {
                if (attacked.MapID < 1351 || attacked.MapID > 1354)
                    Damage = (long)(Damage * (1 + (GetLevelBonus(attacker.Level, attacked.Level) * 0.08)));

                if (attacked.MapID < 1351 || attacked.MapID > 1354)
                    Damage = (long)(Damage * AttackMultiplier(attacker, attacked));
            }
            if (Kernel.Rate(5))
            {
                if (attacker.EntityFlag == EntityFlag.Player)
                {
                    if (attacker.Owner.BlessTime > 0)
                    {
                        Damage *= 2;
                        _String str = new _String(true);
                        str.UID = attacker.UID;
                        str.TextsCount = 1;
                        str.Type = _String.Effect;
                        str.Texts.Add("LuckyGuy");
                        attacker.Owner.SendScreen(str, true);
                    }
                }
            }

            if (Kernel.Rate(5))
            {
                if (attacked.EntityFlag == EntityFlag.Player)
                {
                    if (attacked.Owner.BlessTime > 0)
                    {
                        Damage = 1;
                        _String str = new _String(true);
                        str.UID = attacker.UID;
                        str.TextsCount = 1;
                        str.Type = _String.Effect;
                        str.Texts.Add("LuckyGuy");
                        attacked.Owner.SendScreen(str, true);
                    }
                }
            }
            if (attacked.EntityFlag == EntityFlag.Monster)
            {
                if (Damage >= 700 * attacked.MaxHitpoints)
                    Damage = (long)(700 * attacked.MaxHitpoints);
            }

            Damage += attacker.PhysicalDamageIncrease;
            Damage -= attacked.PhysicalDamageDecrease;
            /*if (attacker.EntityFlag == EntityFlag.Player)
            {
                if (!CritImmune)
                {
                    if (Kernel.Rate((float)attacker.CriticalStrike / 100f))
                    {
                        Packet.Effect1 |= Attack.AttackEffects1.CriticalStrike;
                        Damage = (Int32)Math.Floor((float)Damage * 1.5);
                    }
                }
            }*/
            if (attacked.EntityFlag == EntityFlag.Player)
            {
                if (Kernel.Rate((float)attacked.Block / 100f))
                {
                    Packet.Effect1 |= Attack.AttackEffects1.Block;
                    Damage = (Int32)Math.Floor((float)Damage / 2);
                }
            }
            try
            {
                if (attacked.EntityFlag == EntityFlag.Player && (attacker.BattlePower < attacked.BattlePower))
                {
                    if (attacked.Owner.Challenge == null)
                    {
                        int sub = attacked.BattlePower - attacker.BattlePower;
                        if (sub > 41)
                        {
                            Damage = 1;
                        }
                        else
                        {//Samak
                            Damage = (Int32)Math.Floor((float)Damage * (.50 - (sub * 0.01)));
                        }
                    }
                }
                if (attacked.EntityFlag == EntityFlag.Player && (attacker.BattlePower > attacked.BattlePower))
                {

                    Damage = (Int32)Math.Floor((float)Damage * (1.20));
                }
            }
            catch (Exception) { }
            Damage += Damage;
            Damage += Damage / 4;

            if (Damage <= 0)
                Damage = 1;
            if (attacker.MonsterInfo != null)
                if (attacker.MonsterInfo.Name == "[Guard1]")
                    Damage = (long)attacked.Hitpoints + 1;
            //if (Damage > 65000)
            //Damage = 65000;
            AutoRespone(attacker, attacked, ref Damage);
            if (Constants.Damage1Map.Contains(attacker.MapID))
                Damage = 1;


            // System.Console.WriteLine("Damage is :" + Damage.ToString() + " The Dodge is " + attacked.Dodge.ToString() +"         16");
            //Ahmed Samak handles azureshield
            if (attacked.ContainsFlag2(Network.GamePackets.Update.Flags2.AzureShield))
            {

                if (attacked.AzureDamage >= Damage)
                {
                    ////System.Console.WriteLine("^^^^Damage is " + Damage.ToString() + " Azure is : " + attacked.AzureDamage.ToString());
                    attacked.AzureDamage -= (int)Damage;
                    int sec = 60 - (Time32.Now - attacked.MagicShieldStamp).AllSeconds();
                    attacked.Owner.Send(Constants.Shield(attacked.AzureDamage, sec));
                    //System.Console.WriteLine("^^^^Damage is " + Damage.ToString() + " Azure is : " + attacked.AzureDamage.ToString());
                    return 0;
                }
                else
                {
                    //System.Console.WriteLine("XXXXDamage is " + Damage.ToString() + " Azure is : " + attacked.AzureDamage.ToString());
                    Damage -= attacked.AzureDamage;
                    attacked.AzureDamage = 0;
                    attacked.RemoveFlag2(Update.Flags2.AzureShield);
                    //System.Console.WriteLine("XXXXDamage is " + Damage.ToString() + " Azure is : " + attacked.AzureDamage.ToString());
                    return (uint)(Damage * reduceValue);

                }

            }
            if (Constants.Damage1Map.Contains(attacker.MapID))
                Damage = 1;
            return (uint)(Damage * reduceValue);

        }

        public static uint Ranged(Entity attacker, Entity attacked, Database.SpellInformation spell, ref Attack Packet)
        {
            if (attacker.EntityFlag == EntityFlag.Player)
            {
                GemEffect.Effect(attacker);
            }
            if (attacked.EntityFlag == EntityFlag.Player)
            {
                BlessEffect.Effect(attacked);
            }
            if (attacker.MapID == DeathMatch.MAPID) return 1;
            long Damage = 0;
             
            if (attacked.Name.Contains("[GM]") || attacked.Name.Contains("[PM]"))
                return 1; 
            if (attacked.ContainsFlag(Network.GamePackets.Update.Flags.ShurikenVortex))
                return 1;
            if (attacker.Transformed)
                return 0;

            Damage = Kernel.Random.Next((int)Math.Min(attacker.MinAttack, attacker.MaxAttack), (int)Math.Max(attacker.MinAttack, attacker.MaxAttack) + 1);
            if (attacker.WearsGoldPrize) Damage += (long)(Damage * goldPrizePsyAttackCoefficient);
            if (attacked.WearsGoldPrize) Damage -= (long)(Damage * goldPrizePsyDefenceCoefficient);

            if (attacker.EntityFlag == EntityFlag.Player)
            {
                if (attacker.BattlePower > attacked.BattlePower)
                    canBT = false;
            }
            else canBT = false;
            if (canBT)
            {
                if (canBT)
                {
                    if (Kernel.Rate((float)attacked.Counteraction / 10f) && spell.ID != 1115)
                        canBT = false;
                }
                //if (canBT)
                //{
                //    if (Kernel.Rate((float)attacker.Breaktrough / 10f))
                //    {
                //        Damage = (Int32)attacker.MaxAttack + 5000;
                //        Packet.Effect1 |= Attack.AttackEffects1.Penetration;
                //    }
                //}
            }
            if (attacker.OnSuperman())
            {
                if (attacked.EntityFlag == EntityFlag.Monster)
                {
                    if (!attacked.MonsterInfo.Boss)
                        Damage *= 10;

                }

            }

            if (attacker.OnFatalStrike())
                if (attacked.EntityFlag == EntityFlag.Monster)
                    Damage *= 5;

            if (!attacked.Transformed)
                Damage -= attacked.Defence;
            else
                Damage -= attacked.TransformationDefence;

            Damage -= Damage * attacked.ItemBless / 100;

            byte dodge = attacked.Dodge;
            if (dodge > 100)
                dodge = 99;
            if (!attacked.Transformed)
                Damage -= Damage * dodge / 100;
            else
                Damage -= Damage * attacked.TransformationDodge / 100;

            if (attacker.OnIntensify && Time32.Now >= attacker.IntensifyStamp.AddSeconds(4))
            {
                Damage *= 2;
                attacker.OnIntensify = false;
            }

            if (attacker.ContainsFlag(Network.GamePackets.Update.Flags.Stigma))
                if (!attacker.Transformed)
                    Damage = (long)(Damage * 1.30);

            if (attacked.EntityFlag == EntityFlag.Monster)
            {
                if (attacked.MapID < 1351 || attacked.MapID > 1354)
                    Damage = (long)(Damage * (1 + (GetLevelBonus(attacker.Level, attacked.Level) * 0.08)));

                if (attacked.MapID < 1351 || attacked.MapID > 1354)
                    Damage = (long)(Damage * AttackMultiplier(attacker, attacked));
            }
            if (Kernel.Rate(5))
            {
                if (attacker.EntityFlag == EntityFlag.Player)
                {
                    if (attacker.Owner.BlessTime > 0)
                    {
                        Damage *= 2;
                        _String str = new _String(true);
                        str.UID = attacker.UID;
                        str.TextsCount = 1;
                        str.Type = _String.Effect;
                        str.Texts.Add("LuckyGuy");
                        attacker.Owner.SendScreen(str, true);
                    }
                }
            }

            if (Kernel.Rate(5))
            {
                if (attacked.EntityFlag == EntityFlag.Player)
                {
                    if (attacked.Owner.BlessTime > 0)
                    {
                        Damage = 1;
                        _String str = new _String(true);
                        str.UID = attacker.UID;
                        str.TextsCount = 1;
                        str.Type = _String.Effect;
                        str.Texts.Add("LuckyGuy");
                        attacked.Owner.SendScreen(str, true);
                    }
                }
            }
            if (attacked.EntityFlag == EntityFlag.Monster)
            {
                if (Damage >= 700 * attacked.MaxHitpoints)
                    Damage = (long)(700 * attacked.MaxHitpoints);
            }

            if (spell.PowerPercent != 0)
                Damage = (long)(Damage * spell.PowerPercent);

            Damage += attacker.PhysicalDamageIncrease;
            Damage -= attacked.PhysicalDamageDecrease;
            /*if (attacker.EntityFlag == EntityFlag.Player)
            {
                if (!CritImmune)
                {
                    if (Kernel.Rate((float)attacker.CriticalStrike / 100f) && spell.ID != 1115)
                    {
                        Packet.Effect1 |= Attack.AttackEffects1.CriticalStrike;
                        Damage = (Int32)Math.Floor((float)Damage * 1.5);
                    }
                }
            }*/

            if (attacked.EntityFlag == EntityFlag.Player)
            {
                if (Kernel.Rate((float)attacked.Block / 100f))
                {
                    Packet.Effect1 |= Attack.AttackEffects1.Block;
                    Damage = (Int32)Math.Floor((float)Damage / 2);
                }
            }
            try
            {
                if (attacked.EntityFlag == EntityFlag.Player && (attacker.BattlePower < attacked.BattlePower))
                {
                    if (attacked.Owner.Challenge == null)
                    {
                        int sub = attacked.BattlePower - attacker.BattlePower;
                        if (sub > 41)
                        {
                            Damage = 1;
                        }
                        else
                        {//Samak
                            Damage = (Int32)Math.Floor((float)Damage * (.50 - (sub * 0.01)));
                        }
                    }
                }
                if (attacked.EntityFlag == EntityFlag.Player && (attacker.BattlePower > attacked.BattlePower))
                {

                    Damage = (Int32)Math.Floor((float)Damage * (1.20));
                }
            }
            catch (Exception) { }
            Damage += Damage;
            Damage += Damage / 4;

            if (Damage <= 0)
                Damage = 1;
            if (attacker.MonsterInfo != null)
                if (attacker.MonsterInfo.Name == "[Guard1]")
                    Damage = (long)attacked.Hitpoints + 1;
            //if (Damage > 65000)
            //Damage = 65000;
            AutoRespone(attacker, attacked, ref Damage);
            if (Constants.Damage1Map.Contains(attacker.MapID))
                Damage = 1;

            //System.Console.WriteLine("Damage is :" + Damage.ToString() + " The Dodge is " + attacked.Dodge.ToString() + "         16");


            //Ahmed Samak handles azureshield
            if (attacked.ContainsFlag2(Network.GamePackets.Update.Flags2.AzureShield))
            {

                if (attacked.AzureDamage >= Damage)
                {
                    //System.Console.WriteLine("^^^^Damage is " + Damage.ToString() + " Azure is : " + attacked.AzureDamage.ToString());
                    attacked.AzureDamage -= (int)Damage;
                    int sec = 60 - (Time32.Now - attacked.MagicShieldStamp).AllSeconds();
                    attacked.Owner.Send(Constants.Shield(attacked.AzureDamage, sec));
                    //System.Console.WriteLine("^^^^Damage is " + Damage.ToString() + " Azure is : " + attacked.AzureDamage.ToString());
                    return 0;
                }
                else
                {
                    //System.Console.WriteLine("XXXXDamage is " + Damage.ToString() + " Azure is : " + attacked.AzureDamage.ToString());
                    Damage -= attacked.AzureDamage;
                    attacked.AzureDamage = 0;
                    attacked.RemoveFlag2(Update.Flags2.AzureShield);
                    //System.Console.WriteLine("XXXXDamage is " + Damage.ToString() + " Azure is : " + attacked.AzureDamage.ToString());
                    return (uint)(Damage * reduceValue);

                }

            }
            if (Constants.Damage1Map.Contains(attacker.MapID))
                Damage = 1;
            return (uint)(Damage * reduceValue);
        }
        public static uint Ranged(Entity attacker, SobNpcSpawn attacked, ref Attack Packet)
        {
            if (attacker.MapID == DeathMatch.MAPID) return 1;
            long Damage = 0;

            Durability(attacker, null, null);
            if (attacker.Transformed)
                return 0;

            Damage = Kernel.Random.Next((int)Math.Min(attacker.MinAttack, attacker.MaxAttack), (int)Math.Max(attacker.MinAttack, attacker.MaxAttack) + 1);
            if (attacker.WearsGoldPrize) Damage += (long)(Damage * goldPrizePsyAttackCoefficient);

            /* if (canBT)
             {
                 if (Kernel.Rate((float)attacker.Breaktrough / 10f))
                 {
                     Damage = (Int32)attacker.MaxAttack + 5000;
                     Packet.Effect1 |= Attack.AttackEffects1.Penetration;
                 }
             }*/
            if (attacker.OnIntensify && Time32.Now >= attacker.IntensifyStamp.AddSeconds(4))
            {
                Damage *= 2;

                attacker.OnIntensify = false;
            }

            if (Kernel.Rate(5))
            {
                if (attacker.EntityFlag == EntityFlag.Player)
                {
                    if (attacker.Owner.BlessTime > 0)
                    {
                        Damage *= 2;
                        _String str = new _String(true);
                        str.UID = attacker.UID;
                        str.TextsCount = 1;
                        str.Type = _String.Effect;
                        str.Texts.Add("LuckyGuy");
                        attacker.Owner.SendScreen(str, true);
                    }
                }
            }
            if (attacker.ContainsFlag(Network.GamePackets.Update.Flags.Stigma))
                if (!attacker.Transformed)
                    Damage = (long)(Damage * 1.30);

            Damage += attacker.PhysicalDamageIncrease;
            /*if (attacker.EntityFlag == EntityFlag.Player)
            {
                if (!CritImmune)
                {
                    if (Kernel.Rate((float)attacker.CriticalStrike / 100f))
                    {
                        Packet.Effect1 |= Attack.AttackEffects1.CriticalStrike;
                        Damage = (Int32)Math.Floor((float)Damage * 1.5);
                    }
                }
            }*/
            Damage += Damage;
            Damage += Damage / 4;

            if (Damage <= 0)
                Damage = 1;
            //if (Damage > 65000)
            //Damage = 65000;
            if (attacker.MonsterInfo != null)
                if (attacker.MonsterInfo.Name == "[Guard1]")
                    Damage = (long)attacked.Hitpoints + 1;
            if (Constants.Damage1Map.Contains(attacker.MapID))
                Damage = 1;
            return (uint)(Damage * reduceValue);
        }

        public static long RemoveExcessDamage(long CurrentDamage, Entity Attacker, Entity Opponent)
        {
            if (Opponent.EntityFlag != EntityFlag.Player)
                return CurrentDamage;
            if (Opponent.Reborn == 1)
                CurrentDamage = (long)Math.Round((double)(CurrentDamage * 0.7));
            else if (Opponent.Reborn == 2)
                CurrentDamage = (long)Math.Round((double)(CurrentDamage * 0.5));
            CurrentDamage = (long)Math.Round((double)(CurrentDamage * (Math.Max(0.1, 1.00 - (Opponent.ItemBless * 0.01)))));
//CurrentDamage -= CurrentDamage * Math.Min(Opponent.Gems[7], (ushort)15) / 25;

            return CurrentDamage;
        }

        public static uint Percent(Entity attacked, float percent)
        {
            return (uint)(attacked.Hitpoints * percent);
        }

        public static uint Percent(SobNpcSpawn attacked, float percent)
        {
            return (uint)(attacked.Hitpoints * percent);
        }
        public static uint Percent(uint target, float percent)
        {
            return Convert.ToUInt32(target * percent);
        }
        public static uint Percent(int target, float percent)
        {
            return (uint)(target * percent);
        }

        private static void Durability(Entity attacker, Entity attacked, Database.SpellInformation spell)
        {
#if DURABILITY
            if (spell != null)
                if (!spell.CanKill)
                    return;
            if (attacker.EntityFlag == EntityFlag.Player)
                if (attacker.Owner.Map.ID == 1039)
                    return;
            #region Attack
            if (attacker != null)
                if (attacker.EntityFlag == EntityFlag.Player)
                {
                    for (byte i = 4; i <= 6; i++)
                    {
                        if (!attacker.Owner.Equipment.Free(i))
                        {
                            var item = attacker.Owner.Equipment.TryGetItem(i);
                            if (i == 5)
                            {
                                if (Network.PacketHandler.IsArrow(item.ID))
                                {
                                    continue;
                                }
                            }
                            if (Kernel.Rate(20, 100))
                            {
                                if (item.Durability != 0)
                                {
                                    item.Durability--;
                                    if (item.Durability == 0)
                                        //attacker.Owner.UnloadItemStats(item, true);
                                        Database.ConquerItemTable.UpdateDurabilityItem(item);
                                    item.Mode = Enums.ItemMode.Update;
                                    item.Send(attacker.Owner);
                                    item.Mode = Enums.ItemMode.Default;
                                }
                            }
                        }
                        if (i == 6)
                            break;
                    }
                    if (!attacker.Owner.Equipment.Free(10))
                    {
                        var item = attacker.Owner.Equipment.TryGetItem(10);
                        if (Kernel.Rate(20, 100))
                        {
                            if (item.Durability != 0)
                            {
                                item.Durability--;
                                if (item.Durability == 0)
                                    //attacker.Owner.UnloadItemStats(item, true);
                                    Database.ConquerItemTable.UpdateDurabilityItem(item);
                                item.Mode = Enums.ItemMode.Update;
                                item.Send(attacker.Owner);
                                item.Mode = Enums.ItemMode.Default;
                            }
                        }
                    }
                }
            #endregion
            #region Defence
            if (attacked != null)
                if (attacked.EntityFlag == EntityFlag.Player)
                {
                    for (byte i = 1; i <= 8; i++)
                    {
                        if (i == 4 || i == 6 || i == 7)
                            continue;
                        if (!attacked.Owner.Equipment.Free(i))
                        {
                            var item = attacked.Owner.Equipment.TryGetItem(i);
                            if (i == 5)
                            {
                                if (Network.PacketHandler.ItemPosition(item.ID) != 5 && Network.PacketHandler.IsArrow(item.ID))
                                {
                                    continue;
                                }
                            }
                            if (Kernel.Rate(30, 100))
                            {
                                if (item.Durability != 0)
                                {
                                    item.Durability--;
                                    if (item.Durability == 0)
                                        //attacked.Owner.UnloadItemStats(item, true);
                                        Database.ConquerItemTable.UpdateDurabilityItem(item);

                                    item.Mode = Enums.ItemMode.Update;
                                    item.Send(attacked.Owner);
                                    item.Mode = Enums.ItemMode.Default;
                                }
                            }
                        }
                        if (i == 8)
                            break;
                    }
                    if (!attacked.Owner.Equipment.Free(11) && Kernel.Rate(30, 100))
                    {
                        var item = attacked.Owner.Equipment.TryGetItem(11);
                        if (Kernel.Rate(30, 100))
                        {
                            if (item.Durability != 0)
                            {
                                item.Durability--;
                                if (item.Durability == 0)
                                    //attacked.Owner.UnloadItemStats(item, true);
                                    Database.ConquerItemTable.UpdateDurabilityItem(item);
                                item.Mode = Enums.ItemMode.Update;
                                item.Send(attacked.Owner);
                                item.Mode = Enums.ItemMode.Default;
                            }
                        }
                    }
                }

            #endregion
#endif
        }

        private static void AutoRespone(Entity attacker, Entity attacked, ref long Damage)
        {
            try
            {
                if (attacker.EntityFlag == EntityFlag.Player)
                {
                    if (attacker.Owner.Spells.ContainsKey(11120))
                    {
                        var s = attacker.Owner.Spells[11120];
                        var spell = Database.SpellTable.SpellInformations[s.ID][s.Level];
                        if (spell != null)
                        {
                            if (Conquer_Online_Server.Kernel.Rate(spell.Percent))
                            {
                                var ent = attacked as Entity;
                                if (!ent.IsBlackSpotted)
                                {
                                    ent.IsBlackSpotted = true;
                                    ent.BlackSpotStamp = Time32.Now;
                                    ent.BlackSpotStepSecs = spell.Duration;
                                    Kernel.BlackSpoted.TryAdd(ent.UID, ent);
                                    BlackSpotPacket bsp = new BlackSpotPacket();
                                    foreach (var h in Program.GamePool)
                                    {
                                        h.Send(bsp.ToArray(true, ent.UID));
                                    }
                                }
                            }
                        }
                    }
                }
                if (attacked.EntityFlag == EntityFlag.Player)
                {
                    if (attacked.Owner.Spells.ContainsKey(11130) && attacked.Owner.Entity.IsEagleEyeShooted)
                    {
                        var s = attacked.Owner.Spells[11130];
                        var spell = Database.SpellTable.SpellInformations[s.ID][s.Level];
                        if (spell != null)
                        {
                            if (Kernel.Rate(spell.Percent))
                            {
                                attacked.Owner.Entity.IsEagleEyeShooted = false;
                                SpellUse ssuse = new SpellUse(true);
                                ssuse.Attacker = attacked.UID;
                                ssuse.SpellID = spell.ID;
                                ssuse.SpellLevel = spell.Level;
                                ssuse.AddTarget(attacked.Owner.Entity.UID, new SpellUse.DamageClass().Damage = 11030, null);
                                if (attacked.EntityFlag == EntityFlag.Player)
                                {
                                    attacked.Owner.SendScreen(ssuse, true);
                                }
                            }
                        }
                    }
                    if (attacked.CounterKillSwitch && Kernel.Rate(30) && !attacker.ContainsFlag(Update.Flags.Fly) && Time32.Now > attacked.CounterKillStamp.AddSeconds(15))
                    {
                        attacked.CounterKillStamp = Time32.Now;
                        Network.GamePackets.Attack attack = new Conquer_Online_Server.Network.GamePackets.Attack(true);
                        attack.Effect1 = Attack.AttackEffects1.None;
                        uint damage = Melee(attacked, attacker, ref attack);
                        //Database.SpellInformation information = Database.SpellTable.SpellInformations[6003][attacked.Owner.Spells[6003].Level];
                        damage = damage / 3;
                        attack.Attacked = attacker.UID;
                        attack.Attacker = attacked.UID;
                        attack.AttackType = Network.GamePackets.Attack.Scapegoat;
                        attack.Damage = 0;
                        attack.ResponseDamage = damage;
                        attack.X = attacked.X;
                        attack.Y = attacked.Y;

                        if (attacker.Hitpoints <= damage)
                        {
                            if (attacker.EntityFlag == EntityFlag.Player)
                            {
                                attacked.Owner.UpdateQualifier(attacked.Owner, attacker.Owner, attacker.Hitpoints);

                                attacker.Owner.SendScreen(attack, true);
                                attacked.AttackPacket = null;
                            }
                            else
                            {
                                attacker.MonsterInfo.SendScreen(attack);
                            }
                            attacker.Die(attacked);
                        }
                        else
                        {
                            attacker.Hitpoints -= damage;
                            if (attacker.EntityFlag == EntityFlag.Player)
                            {
                                attacked.Owner.UpdateQualifier(attacked.Owner, attacker.Owner, damage);

                                attacker.Owner.SendScreen(attack, true);
                            }
                            else
                            {
                                attacker.MonsterInfo.SendScreen(attack);
                            }
                        }
                        Damage = 0;
                    }
                    else if (attacked.Owner.Spells.ContainsKey(3060) && Kernel.Rate(15))
                    {
                        uint damage = (uint)(Damage / 10);
                        if (damage <= 0)
                            damage = 1;
                        if (damage > 10000) damage = 10000;
                        Network.GamePackets.Attack attack = new Conquer_Online_Server.Network.GamePackets.Attack(true);
                        attack.Attacked = attacker.UID;
                        attack.Attacker = attacked.UID;
                        attack.AttackType = Network.GamePackets.Attack.Reflect;
                        attack.Damage = damage;
                        attack.ResponseDamage = damage;
                        attack.X = attacked.X;
                        attack.Y = attacked.Y;

                        if (attacker.Hitpoints <= damage)
                        {
                            if (attacker.EntityFlag == EntityFlag.Player)
                            {
                                attacked.Owner.UpdateQualifier(attacked.Owner, attacker.Owner, attacker.Hitpoints);

                                attacker.Owner.SendScreen(attack, true);
                                attacked.AttackPacket = null;
                            }
                            else
                            {
                                attacker.MonsterInfo.SendScreen(attack);
                            }
                            attacker.Die(attacked);
                        }
                        else
                        {
                            attacker.Hitpoints -= damage;
                            if (attacker.EntityFlag == EntityFlag.Player)
                            {
                                attacked.Owner.UpdateQualifier(attacked.Owner, attacker.Owner, damage);
                                attacker.Owner.SendScreen(attack, true);
                            }
                            else
                            {
                                attacker.MonsterInfo.SendScreen(attack);
                            }
                        }
                        Damage = 0;
                    }
                }
            }
            catch (Exception e) { Program.SaveException(e); }
        }
        public static int GetLevelBonus(int l1, int l2)
        {
            int num = l1 - l2;
            int bonus = 0;
            if (num >= 3)
            {
                num -= 3;
                bonus = 1 + (num / 5);
            }
            return bonus;
        }
        private static double AttackMultiplier(Entity attacker, Entity attacked)
        {
            if (attacked.Level > attacker.Level)
                return 1;
            return ((double)(attacker.Level - attacked.Level)) / 10 + 1;
        }
        public static ulong CalculateExpBonus(ushort Level, ushort MonsterLevel, ulong Experience)
        {
            int leveldiff = (2 + Level - MonsterLevel);
            if (leveldiff < -5)
                Experience = (ulong)(Experience * 1.3);
            else if (leveldiff < -1)
                Experience = (ulong)(Experience * 1.2);
            else if (leveldiff == 4)
                Experience = (ulong)(Experience * 0.8);
            else if (leveldiff == 5)
                Experience = (ulong)(Experience * 0.3);
            else if (leveldiff > 5)
                Experience = (ulong)(Experience * 0.1);
            return Experience;
        }

        public static bool canBT { get; set; }
    }
}
