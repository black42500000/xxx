using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer_Online_Server.Network.GamePackets;

namespace Conquer_Online_Server.Game.Features.Reincarnation
{
    public class Handle
    {
        public static void Hash(Client.GameClient client)
        {
            if (Kernel.ReincarnatedCharacters.ContainsKey(client.Entity.UID))
            {
                if (client.Entity.Level >= 110 && client.Entity.Reborn >= 2)
                {
                    ushort stats = 0;
                    uint lev1 = client.Entity.Level;
                    ReincarnateInfo info = Kernel.ReincarnatedCharacters[client.Entity.UID];
                    client.Entity.Level = info.Level;
                    client.Entity.Experience = info.Experience;
                    Kernel.ReincarnatedCharacters.Remove(info.UID);
                    Database.ReincarnationTable.RemoveReincarnated(client.Entity);
                    stats = (ushort)(((client.Entity.Level - lev1) * 3) - 3);
                    client.Entity.Atributes += stats;
                }
            }
        }
    }

    #region Reincarnation

    public class Reincarnation
    {
        private Client.GameClient _client;

        public Reincarnation(Client.GameClient client, byte new_class)
        {
            if (client.Entity.Level < 120)
                return;
            if (Kernel.ReincarnatedCharacters.ContainsKey(client.Entity.UID))
                return;
            _client = client;
            Database.ReincarnationTable.NewReincarnated(client.Entity);
            Game.Features.Reincarnation.ReincarnateInfo info = new Game.Features.Reincarnation.ReincarnateInfo();
            info.UID = client.Entity.UID;
            info.Level = client.Entity.Level;
            info.Experience = client.Entity.Experience;
            Kernel.ReincarnatedCharacters.Add(info.UID, info);
            client.Entity.FirstRebornClass = client.Entity.SecondRebornClass;
            client.Entity.SecondRebornClass = client.Entity.Class;
            client.Entity.Class = new_class;
            client.Entity.SecondRebornLevel = client.Entity.Level;
            //client.Entity.ReincarnationLev = client.Entity.Level;//kikoz
            client.Entity.Level = 15;
            client.Entity.Experience = 0;
            client.Entity.Atributes =
                (ushort)
                    (client.ExtraAtributePoints(client.Entity.FirstRebornClass, client.Entity.FirstRebornLevel) +
                     client.ExtraAtributePoints(client.Entity.SecondRebornClass, client.Entity.SecondRebornLevel) +
                     62);

            #region RemoveAllSpells

            Interfaces.ISkill[] spells = client.Spells.Values.ToArray();
            foreach (Interfaces.ISkill spell in spells)
            {
                if (!Constants.AvaibleSpells.Contains(spell.ID))
                {
                    client.RemoveSpell(spell);
                    Database.SkillTable.DeleteSpell(client, spell.ID);
                }
                //spell.Send(client);
            }
            //Skills.Clear();
            client.Spells.Clear();
            Database.SkillTable.removeAllSkills(client); //Samak
            //Profs.Clear();
            client.Proficiencies.Clear();
            Database.SkillTable.removeAllProfs(client); //Samak  

            #region Archer2
            #region Arch-Arch

            if (client.Entity.FirstRebornClass == 45 && client.Entity.SecondRebornClass == 45)
            {
                if (client.Entity.Class == 41)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 10313 }); //Star Arrow
                    client.AddSpell(new Spell(true) { ID = 5000 }); //Freezing Arrows
                }
                else
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 5000 }); //Freezing Arrows
                    client.AddSpell(new Spell(true) { ID = 5002 }); //Poisonous Arrows
                }
            }
            #endregion
            #region Arch-Fire

            else if (client.Entity.FirstRebornClass == 45 && client.Entity.SecondRebornClass == 145)
            {
                if (client.Entity.Class == 11 || client.Entity.Class == 21 || client.Entity.Class == 41 ||
                    client.Entity.Class == 71 || client.Entity.Class == 81)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1000 }); //Thunder
                    client.AddSpell(new Spell(true) { ID = 1001 }); //Fire
                    client.AddSpell(new Spell(true) { ID = 1005 }); //Cure
                    client.AddSpell(new Spell(true) { ID = 1195 }); //Meditation
                    client.AddSpell(new Spell(true) { ID = 5002 }); //Poisonous Arrows
                }
                else if (client.Entity.Class == 51 || client.Entity.Class == 61)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1000 }); //Thunder
                    client.AddSpell(new Spell(true) { ID = 1001 }); //Fire
                    client.AddSpell(new Spell(true) { ID = 1005 }); //Cure
                    client.AddSpell(new Spell(true) { ID = 1195 }); //Meditation
                    client.AddSpell(new Spell(true) { ID = 10010 }); //Poison
                }
                else if (client.Entity.Class == 132)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 5002 }); //Poisonous Arrows
                    client.AddSpell(new Spell(true) { ID = 1120 }); //FireCircle
                }
                else if (client.Entity.Class == 142)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 5002 }); //Poisonous Arrows
                    client.AddSpell(new Spell(true) { ID = 3080 }); //Dodge
                }
            }

            #endregion
            #region Arch-Tro

            if (client.Entity.FirstRebornClass == 45 && client.Entity.SecondRebornClass == 15)
            {
                if (client.Entity.Class == 41 || client.Entity.Class == 132 || client.Entity.Class == 142 ||
                    client.Entity.Class == 51 || client.Entity.Class == 61 || client.Entity.Class == 71 ||
                    client.Entity.Class == 81)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 5002 }); //Poisonous Arrows
                    client.AddSpell(new Spell(true) { ID = 1110 }); //Cyclone
                    client.AddSpell(new Spell(true) { ID = 1190 }); //SpiritHealing
                    client.AddSpell(new Spell(true) { ID = 1270 }); //Robot
                }
                else if (client.Entity.Class == 11)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 3050 }); //CruelShade
                    client.AddSpell(new Spell(true) { ID = 5002 }); //Poisonous Arrows
                }
                else if (client.Entity.Class == 21)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 5002 }); //Poisonous Arrows
                    client.AddSpell(new Spell(true) { ID = 1110 }); //Cyclone
                    client.AddSpell(new Spell(true) { ID = 1190 }); //SpiritHealing
                    client.AddSpell(new Spell(true) { ID = 1270 }); //Robot
                    client.AddSpell(new Spell(true) { ID = 5100 }); //Iron Shirt
                }
            }

            #endregion
            #region Arch-War

            if (client.Entity.FirstRebornClass == 45 && client.Entity.SecondRebornClass == 25)
            {
                if (client.Entity.Class == 142 || client.Entity.Class == 41)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 5002 }); //Poisonous Arrows
                    client.AddSpell(new Spell(true) { ID = 1020 }); //Shield
                    client.AddSpell(new Spell(true) { ID = 1040 }); //Roar
                    client.AddSpell(new Spell(true) { ID = 3060 }); //Reflect
                }
                else if (client.Entity.Class == 11 || client.Entity.Class == 51 || client.Entity.Class == 61 ||
                         client.Entity.Class == 71 || client.Entity.Class == 81)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 5002 }); //Poisonous Arrows
                    client.AddSpell(new Spell(true) { ID = 1015 }); //Accuracy
                    client.AddSpell(new Spell(true) { ID = 1040 }); //Roar
                    client.AddSpell(new Spell(true) { ID = 3060 }); //Reflect
                    client.AddSpell(new Spell(true) { ID = 1320 }); //FlyingMoon
                }
                else if (client.Entity.Class == 51)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 5002 }); //Poisonous Arrows
                    client.AddSpell(new Spell(true) { ID = 3060 }); //Reflect
                }
                else if (client.Entity.Class == 21)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 5002 }); //Poisonous Arrows
                    client.AddSpell(new Spell(true) { ID = 3060 }); //Reflect
                }
                else if (client.Entity.Class == 132)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 5002 }); //Poisonous Arrows
                    client.AddSpell(new Spell(true) { ID = 1020 }); //Shield
                    client.AddSpell(new Spell(true) { ID = 1040 }); //Roar
                    client.AddSpell(new Spell(true) { ID = 3060 }); //Reflect
                    client.AddSpell(new Spell(true) { ID = 1025 }); //SuperMan
                }
            }

            #endregion
            #region Arch-Water

            if (client.Entity.FirstRebornClass == 45 && client.Entity.SecondRebornClass == 135)
            {
                if (client.Entity.Class == 41)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1005 }); //Cure
                    client.AddSpell(new Spell(true) { ID = 1090 }); //MagicShield
                    client.AddSpell(new Spell(true) { ID = 1095 }); //Stigma
                    client.AddSpell(new Spell(true) { ID = 1195 }); //Meditation
                    client.AddSpell(new Spell(true) { ID = 5002 }); //Poisonous Arrows
                    client.AddSpell(new Spell(true) { ID = 1280 }); //WaterElf
                    client.AddSpell(new Spell(true) { ID = 1350 }); //DivineHare
                }
                else if (client.Entity.Class == 11 || client.Entity.Class == 21 || client.Entity.Class == 61)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1005 }); //Cure
                    client.AddSpell(new Spell(true) { ID = 1085 }); //StarOfAccuracy
                    client.AddSpell(new Spell(true) { ID = 1090 }); //MagicShield
                    client.AddSpell(new Spell(true) { ID = 1095 }); //Stigma
                    client.AddSpell(new Spell(true) { ID = 1195 }); //Meditation
                    client.AddSpell(new Spell(true) { ID = 5002 }); //Poisonous Arrows
                    client.AddSpell(new Spell(true) { ID = 1280 }); //WaterElf
                    client.AddSpell(new Spell(true) { ID = 1350 }); //DivineHare
                }
                else if (client.Entity.Class == 142)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1050 }); //XPRevive
                    client.AddSpell(new Spell(true) { ID = 5002 }); //Poisonous Arrows
                    client.AddSpell(new Spell(true) { ID = 1055 }); //HealingRain 
                    client.AddSpell(new Spell(true) { ID = 1175 }); //AdvancedCure
                    client.AddSpell(new Spell(true) { ID = 1280 }); //WaterElf
                    client.AddSpell(new Spell(true) { ID = 1350 }); //DivineHare
                }
                else if (client.Entity.Class == 132)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 5002 }); //Poisonous Arrows
                    client.AddSpell(new Spell(true) { ID = 1280 }); //WaterElf
                    client.AddSpell(new Spell(true) { ID = 1350 }); //DivineHare
                    client.AddSpell(new Spell(true) { ID = 3090 }); //Pervade
                }
                else if (client.Entity.Class == 51)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 10010 }); //Poison
                    client.AddSpell(new Spell(true) { ID = 1005 }); //Cure
                    client.AddSpell(new Spell(true) { ID = 1085 }); //StarOfAccuracy
                    client.AddSpell(new Spell(true) { ID = 1090 }); //MagicShield
                    client.AddSpell(new Spell(true) { ID = 1095 }); //Stigma
                    client.AddSpell(new Spell(true) { ID = 1195 }); //Meditation
                }
                else if (client.Entity.Class == 71 || client.Entity.Class == 81)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 5002 }); //Poisonous Arrows
                    client.AddSpell(new Spell(true) { ID = 1005 }); //Cure
                    client.AddSpell(new Spell(true) { ID = 1085 }); //StarOfAccuracy
                    client.AddSpell(new Spell(true) { ID = 1090 }); //MagicShield
                    client.AddSpell(new Spell(true) { ID = 1095 }); //Stigma
                    client.AddSpell(new Spell(true) { ID = 1195 }); //Meditation
                }
            }

            #endregion
            #region Arch-Nin

            if (client.Entity.FirstRebornClass == 45 && client.Entity.SecondRebornClass == 55)
            {
                if (client.Entity.Class == 41)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 6001 }); //ToxicFog
                }
                else if (client.Entity.Class == 11 || client.Entity.Class == 21 || client.Entity.Class == 132 ||
                         client.Entity.Class == 142 || client.Entity.Class == 61 || client.Entity.Class == 71 ||
                    client.Entity.Class == 81)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 5002 }); //Poisonous Arrows
                    client.AddSpell(new Spell(true) { ID = 6001 }); //ToxicFog
                }
                else if (client.Entity.Class == 51)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 5002 }); //Poisonous Arrows
                    client.AddSpell(new Spell(true) { ID = 6000 }); //TwofoldBlades
                    client.AddSpell(new Spell(true) { ID = 6001 }); //ToxicFog
                    client.AddSpell(new Spell(true) { ID = 6002 }); //PoisonStar
                    client.AddSpell(new Spell(true) { ID = 6004 }); //ArcherBane
                    client.AddSpell(new Spell(true) { ID = 6011 }); //FatalStrike
                    client.AddSpell(new Spell(true) { ID = 6010 }); //ShurikenVortex
                }
            }

            #endregion
            #region Arch-Monk

            if (client.Entity.FirstRebornClass == 145 && client.Entity.SecondRebornClass == 65)
            {
                if (client.Entity.Class == 21)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 10400 }); //Serenity
                    client.AddSpell(new Spell(true) { ID = 5002 }); //Poisonous Arrows
                    client.AddSpell(new Spell(true) { ID = 1000 }); //Thunder
                    client.AddSpell(new Spell(true) { ID = 1001 }); //Fire
                    client.AddSpell(new Spell(true) { ID = 1005 }); //Cure
                }
                else if (client.Entity.Class == 11 || client.Entity.Class == 41 || client.Entity.Class == 132 ||
                         client.Entity.Class == 142 || client.Entity.Class == 51 || client.Entity.Class == 61 ||
                         client.Entity.Class == 71 || client.Entity.Class == 81)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 5002 }); //Poisonous Arrows
                    client.AddSpell(new Spell(true) { ID = 10400 }); //Serenity
                }
            }

            #endregion
            #region Arch-Pirate

            if (client.Entity.FirstRebornClass == 145 && client.Entity.SecondRebornClass == 75)
            {
                if (client.Entity.Class == 132 || client.Entity.Class == 142 || client.Entity.Class == 41)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 11070 }); //Gale Bomb
                }
                else if (client.Entity.Class == 11 || client.Entity.Class == 21 || client.Entity.Class == 51 ||
                         client.Entity.Class == 61 || client.Entity.Class == 71 || client.Entity.Class == 81)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 11070 }); //Gale Bomb
                    client.AddSpell(new Spell(true) { ID = 5002 }); //Poisonous Arrows
                }
            }

            #endregion
            #region Arch-KungFu

            if (client.Entity.FirstRebornClass == 145 && client.Entity.SecondRebornClass == 85)
            {
                if (client.Entity.Class == 132 || client.Entity.Class == 142 || client.Entity.Class == 41)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                }
                else if (client.Entity.Class == 11 || client.Entity.Class == 21 || client.Entity.Class == 51 ||
                         client.Entity.Class == 61 || client.Entity.Class == 71 || client.Entity.Class == 81)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 5002 }); //Poisonous Arrows
                }
            }

            #endregion
            #endregion
            #region Trojan2
            #region Tro-Arch

            if (client.Entity.FirstRebornClass == 15 && client.Entity.SecondRebornClass == 45)
            {
                if (client.Entity.Class == 41)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1190 }); //SpiritHealing
                    client.AddSpell(new Spell(true) { ID = 1110 }); //Cyclone
                    client.AddSpell(new Spell(true) { ID = 1270 }); //Robot
                    client.AddSpell(new Spell(true) { ID = 5000 }); //Freezing Arrows
                }
                else if (client.Entity.Class == 71 || client.Entity.Class == 81)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1190 }); //SpiritHealing
                    client.AddSpell(new Spell(true) { ID = 1110 }); //Cyclone
                    client.AddSpell(new Spell(true) { ID = 1270 }); //Robot
                }
                else
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1190 }); //SpiritHealing
                    client.AddSpell(new Spell(true) { ID = 1110 }); //Cyclone
                    client.AddSpell(new Spell(true) { ID = 1270 }); //Robot
                    client.AddSpell(new Spell(true) { ID = 5002 }); //Poisonous Arrows
                }
            }

            #endregion
            #region Tro-Fire

            if (client.Entity.FirstRebornClass == 15 && client.Entity.SecondRebornClass == 145)
            {
                if (client.Entity.Class == 41 || client.Entity.Class == 51 || client.Entity.Class == 61 ||
                    client.Entity.Class == 71 || client.Entity.Class == 81)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1190 }); //SpiritHealing
                    client.AddSpell(new Spell(true) { ID = 1110 }); //Cyclone
                    client.AddSpell(new Spell(true) { ID = 1270 }); //Robot
                    client.AddSpell(new Spell(true) { ID = 1000 }); //Thunder
                    client.AddSpell(new Spell(true) { ID = 1001 }); //Fire
                    client.AddSpell(new Spell(true) { ID = 1005 }); //Cure
                    client.AddSpell(new Spell(true) { ID = 1195 }); //Meditation
                }
                else if (client.Entity.Class == 11 || client.Entity.Class == 21)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1270 }); //Robot
                    client.AddSpell(new Spell(true) { ID = 1000 }); //Thunder
                    client.AddSpell(new Spell(true) { ID = 1001 }); //Fire
                    client.AddSpell(new Spell(true) { ID = 1005 }); //Cure
                    client.AddSpell(new Spell(true) { ID = 1195 }); //Meditation
                }
                else if (client.Entity.Class == 142)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 3080 }); //Dodge
                    client.AddSpell(new Spell(true) { ID = 1190 }); //SpiritHealing
                    client.AddSpell(new Spell(true) { ID = 1110 }); //Cyclone
                    client.AddSpell(new Spell(true) { ID = 1270 }); //Robot
                }
                else if (client.Entity.Class == 132)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1190 }); //SpiritHealing
                    client.AddSpell(new Spell(true) { ID = 1110 }); //Cyclone
                    client.AddSpell(new Spell(true) { ID = 1270 }); //Robot
                    client.AddSpell(new Spell(true) { ID = 1120 }); //FireCircle
                }
            }

            #endregion
            #region Tro-Tro

            if (client.Entity.FirstRebornClass == 15 && client.Entity.SecondRebornClass == 15)
            {
                if (client.Entity.Class == 41 || client.Entity.Class == 51 || client.Entity.Class == 61 ||
                    client.Entity.Class == 71 || client.Entity.Class == 81)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1190 }); //SpiritHealing
                    client.AddSpell(new Spell(true) { ID = 1110 }); //Cyclone
                    client.AddSpell(new Spell(true) { ID = 1270 }); //Robot
                    client.AddSpell(new Spell(true) { ID = 3050 }); //CruelShade
                }
                else if (client.Entity.Class == 142)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1190 }); //SpiritHealing
                    client.AddSpell(new Spell(true) { ID = 1110 }); //Cyclone
                    client.AddSpell(new Spell(true) { ID = 1270 }); //Robot
                    client.AddSpell(new Spell(true) { ID = 3050 }); //CruelShade
                }
                else if (client.Entity.Class == 11)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 3050 }); //Cruel Shade
                    client.AddSpell(new Spell(true) { ID = 10315 }); //DragonWhirl
                }
                else if (client.Entity.Class == 21)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 3050 }); //Cruel Shade
                    client.AddSpell(new Spell(true) { ID = 1110 }); //Cyclone
                    client.AddSpell(new Spell(true) { ID = 1190 }); //SpiritHealing
                    client.AddSpell(new Spell(true) { ID = 1270 }); //Robot
                    client.AddSpell(new Spell(true) { ID = 5100 }); //Iron Shirt
                }
                else if (client.Entity.Class == 132)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 3050 }); //Cruel Shade
                    client.AddSpell(new Spell(true) { ID = 1270 }); //Robot
                    client.AddSpell(new Spell(true) { ID = 1190 }); //Spirit Healing
                    client.AddSpell(new Spell(true) { ID = 1110 }); //Cyclone
                }
            }

            #endregion
            #region Tro-War

            if (client.Entity.FirstRebornClass == 15 && client.Entity.SecondRebornClass == 25)
            {
                if (client.Entity.Class == 41 || client.Entity.Class == 142)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1110 }); //Cyclone
                    client.AddSpell(new Spell(true) { ID = 1190 }); //SpiritHealing
                    client.AddSpell(new Spell(true) { ID = 5100 }); //IronShirt
                    client.AddSpell(new Spell(true) { ID = 1040 }); //Roar
                    client.AddSpell(new Spell(true) { ID = 1020 }); //ShieldShield
                    client.AddSpell(new Spell(true) { ID = 1270 }); //Robot
                    client.AddSpell(new Spell(true) { ID = 3060 }); //Reflect
                }
                else if (client.Entity.Class == 11 || client.Entity.Class == 51 || client.Entity.Class == 61 ||
                         client.Entity.Class == 71 || client.Entity.Class == 81)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1110 }); //Cyclone
                    client.AddSpell(new Spell(true) { ID = 1190 }); //SpiritHealing
                    client.AddSpell(new Spell(true) { ID = 5100 }); //IronShirt
                    client.AddSpell(new Spell(true) { ID = 1270 }); //Robot
                    client.AddSpell(new Spell(true) { ID = 3060 }); //Reflect
                    client.AddSpell(new Spell(true) { ID = 1040 }); //Roar
                    client.AddSpell(new Spell(true) { ID = 1015 }); //Accuracy
                    client.AddSpell(new Spell(true) { ID = 1320 }); //FlyingMoon
                }
                else if (client.Entity.Class == 21)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1110 }); //Cyclone
                    client.AddSpell(new Spell(true) { ID = 1190 }); //SpiritHealing
                    client.AddSpell(new Spell(true) { ID = 1270 }); //Robot
                    client.AddSpell(new Spell(true) { ID = 3060 }); //Reflect
                }
                else if (client.Entity.Class == 132)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1110 }); //Cyclone
                    client.AddSpell(new Spell(true) { ID = 1190 }); //SpiritHealing
                    client.AddSpell(new Spell(true) { ID = 1270 }); //Robot
                    client.AddSpell(new Spell(true) { ID = 3060 }); //Reflect
                    client.AddSpell(new Spell(true) { ID = 5100 }); //IronShirt
                    client.AddSpell(new Spell(true) { ID = 1040 }); //Roar
                    client.AddSpell(new Spell(true) { ID = 1020 }); //ShieldShield
                    client.AddSpell(new Spell(true) { ID = 1025 }); //SuperMan
                }
            }

            #endregion
            #region Tro-Water

            if (client.Entity.FirstRebornClass == 15 && client.Entity.SecondRebornClass == 135)
            {
                if (client.Entity.Class == 41)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1110 }); //Cyclone
                    client.AddSpell(new Spell(true) { ID = 1190 }); //SpiritHealing
                    client.AddSpell(new Spell(true) { ID = 1270 }); //Robot
                    client.AddSpell(new Spell(true) { ID = 1005 }); //Cure
                    client.AddSpell(new Spell(true) { ID = 1095 }); //Stigma
                    client.AddSpell(new Spell(true) { ID = 1195 }); //Meditation
                    client.AddSpell(new Spell(true) { ID = 1090 }); //MagicShield
                    client.AddSpell(new Spell(true) { ID = 1075 }); //Invisibility
                }
                else if (client.Entity.Class == 142)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1110 }); //Cyclone
                    client.AddSpell(new Spell(true) { ID = 1190 }); //SpiritHealing
                    client.AddSpell(new Spell(true) { ID = 1270 }); //Robot
                    client.AddSpell(new Spell(true) { ID = 1050 }); //XPRevive
                    client.AddSpell(new Spell(true) { ID = 1175 }); //AdvancedCure
                    client.AddSpell(new Spell(true) { ID = 1075 }); //Invisibility
                }
                else if (client.Entity.Class == 11)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1270 }); //Robot
                    client.AddSpell(new Spell(true) { ID = 1005 }); //Cure
                    client.AddSpell(new Spell(true) { ID = 1195 }); //Meditation
                    client.AddSpell(new Spell(true) { ID = 1095 }); //Stigma
                    client.AddSpell(new Spell(true) { ID = 1090 }); //MagicShield
                    client.AddSpell(new Spell(true) { ID = 1085 }); //StarOfAccuracy
                }
                else if (client.Entity.Class == 21 || client.Entity.Class == 51 || client.Entity.Class == 71 ||
                    client.Entity.Class == 81)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1110 }); //Cyclone
                    client.AddSpell(new Spell(true) { ID = 1190 }); //SpiritHealing
                    client.AddSpell(new Spell(true) { ID = 1270 }); //Robot
                    client.AddSpell(new Spell(true) { ID = 1005 }); //Cure
                    client.AddSpell(new Spell(true) { ID = 1085 }); //StarOfAccuracy
                    client.AddSpell(new Spell(true) { ID = 1095 }); //Stigma
                    client.AddSpell(new Spell(true) { ID = 1195 }); //Meditation
                    client.AddSpell(new Spell(true) { ID = 1090 }); //MagicShield
                }
                else if (client.Entity.Class == 132)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1110 }); //Cyclone
                    client.AddSpell(new Spell(true) { ID = 1190 }); //SpiritHealing
                    client.AddSpell(new Spell(true) { ID = 1270 }); //Robot
                    client.AddSpell(new Spell(true) { ID = 3090 }); //Pervade
                }
                else if (client.Entity.Class == 61)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1110 }); //Cyclone
                    client.AddSpell(new Spell(true) { ID = 1190 }); //SpiritHealing
                    client.AddSpell(new Spell(true) { ID = 1270 }); //Robot
                    client.AddSpell(new Spell(true) { ID = 1005 }); //Cure
                    client.AddSpell(new Spell(true) { ID = 1085 }); //StarOfAccuracy
                    client.AddSpell(new Spell(true) { ID = 1095 }); //Stigma
                    client.AddSpell(new Spell(true) { ID = 1195 }); //Meditation
                    client.AddSpell(new Spell(true) { ID = 1090 }); //MagicShield
                    client.AddSpell(new Spell(true) { ID = 1350 }); //DivineHare
                    client.AddSpell(new Spell(true) { ID = 1280 }); //Water Elf
                }
            }

            #endregion
            #region Tro-Nin

            if (client.Entity.FirstRebornClass == 15 && client.Entity.SecondRebornClass == 55)
            {
                if (client.Entity.Class == 51)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1110 }); //Cyclone
                    client.AddSpell(new Spell(true) { ID = 1190 }); //SpiritHealing
                    client.AddSpell(new Spell(true) { ID = 1270 }); //Robot
                    client.AddSpell(new Spell(true) { ID = 6000 }); //TwofoldBlades
                    client.AddSpell(new Spell(true) { ID = 6001 }); //ToxicFog
                    client.AddSpell(new Spell(true) { ID = 6002 }); //PoisonStar
                    client.AddSpell(new Spell(true) { ID = 6004 }); //ArcherBane
                    client.AddSpell(new Spell(true) { ID = 6011 }); //FatalStrike
                    client.AddSpell(new Spell(true) { ID = 6010 }); //ShurikenVortex
                }
                else
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1110 }); //Cyclone
                    client.AddSpell(new Spell(true) { ID = 1190 }); //SpiritHealing
                    client.AddSpell(new Spell(true) { ID = 1270 }); //Robot
                    client.AddSpell(new Spell(true) { ID = 6001 }); //ToxicFog
                }
            }

            #endregion
            #region Tro-Monk

            if (client.Entity.FirstRebornClass == 15 && client.Entity.SecondRebornClass == 65)
            {
                client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                client.AddSpell(new Spell(true) { ID = 1110 }); //Cyclone
                client.AddSpell(new Spell(true) { ID = 1190 }); //SpiritHealing
                client.AddSpell(new Spell(true) { ID = 1270 }); //Robot
                client.AddSpell(new Spell(true) { ID = 10400 }); //Serenity
            }

            #endregion
            #region Tro-Pirate

            if (client.Entity.FirstRebornClass == 15 && client.Entity.SecondRebornClass == 75)
            {
                client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                client.AddSpell(new Spell(true) { ID = 1110 }); //Cyclone
                client.AddSpell(new Spell(true) { ID = 1190 }); //SpiritHealing
                client.AddSpell(new Spell(true) { ID = 1270 }); //Robot
                client.AddSpell(new Spell(true) { ID = 11070 }); //Gale Bomb
            }

            #endregion
            #region Tro-KungFu
            if (client.Entity.FirstRebornClass == 15 && client.Entity.SecondRebornClass == 85)
            {
                client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                client.AddSpell(new Spell(true) { ID = 1110 }); //Cyclone
                client.AddSpell(new Spell(true) { ID = 1190 }); //SpiritHealing
                client.AddSpell(new Spell(true) { ID = 1270 }); //Robot
            }
            #endregion
            #endregion
            #region Ninja2
            #region Nin-Arch

            if (client.Entity.FirstRebornClass == 55 && client.Entity.SecondRebornClass == 45)
            {
                if (client.Entity.Class == 41)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 6001 }); //Toxic Fog
                    client.AddSpell(new Spell(true) { ID = 5000 }); //Freezing Arrow
                }
                else if (client.Entity.Class == 51)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 5002 }); //Poisonous Arrows
                }
                else if (client.Entity.Class == 11 || client.Entity.Class == 21 || client.Entity.Class == 142 ||
                         client.Entity.Class == 132 || client.Entity.Class == 61 || client.Entity.Class == 71 ||
                    client.Entity.Class == 81)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 6001 }); //Toxic Fog
                    client.AddSpell(new Spell(true) { ID = 5002 }); //Poisonous Arrows
                }
            }

            #endregion
            #region Nin-Fire

            {
                if (client.Entity.FirstRebornClass == 55 && client.Entity.SecondRebornClass == 145)
                {
                    if (client.Entity.Class == 11 || client.Entity.Class == 21 || client.Entity.Class == 41 ||
                        client.Entity.Class == 51 || client.Entity.Class == 61 || client.Entity.Class == 71 ||
                        client.Entity.Class == 81)
                    {
                        client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                        client.AddSpell(new Spell(true) { ID = 6001 }); //Toxic Fog
                        client.AddSpell(new Spell(true) { ID = 1000 }); //Thunder
                        client.AddSpell(new Spell(true) { ID = 1001 }); //Fire
                        client.AddSpell(new Spell(true) { ID = 1005 }); //Cure
                        client.AddSpell(new Spell(true) { ID = 1195 }); //Meditation
                    }
                    else if (client.Entity.Class == 142)
                    {
                        client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                        client.AddSpell(new Spell(true) { ID = 6001 }); //Toxic Fog
                        client.AddSpell(new Spell(true) { ID = 3080 }); //Dodge
                        client.AddSpell(new Spell(true) { ID = 1000 }); //Thunder
                    }
                    else if (client.Entity.Class == 132)
                    {
                        client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                        client.AddSpell(new Spell(true) { ID = 1120 }); //FireCircle
                        client.AddSpell(new Spell(true) { ID = 6001 }); //Toxic Fog
                    }
                }
            }

            #endregion
            #region Nin-Tro

            if (client.Entity.FirstRebornClass == 55 && client.Entity.SecondRebornClass == 15)
            {
                if (client.Entity.Class == 41 || client.Entity.Class == 51 || client.Entity.Class == 132 ||
                    client.Entity.Class == 142 || client.Entity.Class == 61 || client.Entity.Class == 71 ||
                    client.Entity.Class == 81)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1110 }); //Cyclone
                    client.AddSpell(new Spell(true) { ID = 1190 }); //SpiritHealing
                    client.AddSpell(new Spell(true) { ID = 1270 }); //Robot
                    client.AddSpell(new Spell(true) { ID = 6001 }); //Toxic Fog
                }
                else if (client.Entity.Class == 11)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 6001 }); //Toxic Fog
                }
                else if (client.Entity.Class == 21)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1110 }); //Cyclone
                    client.AddSpell(new Spell(true) { ID = 1190 }); //SpiritHealing
                    client.AddSpell(new Spell(true) { ID = 1270 }); //Robot
                    client.AddSpell(new Spell(true) { ID = 6001 }); //Toxic Fog
                    client.AddSpell(new Spell(true) { ID = 5100 }); //IronShirt
                }
            }

            #endregion
            #region Nin-War

            if (client.Entity.FirstRebornClass == 55 && client.Entity.SecondRebornClass == 25)
            {
                if (client.Entity.Class == 41 || client.Entity.Class == 142)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 6001 }); //Toxic Fog
                    client.AddSpell(new Spell(true) { ID = 1020 }); //Shield
                    client.AddSpell(new Spell(true) { ID = 3060 }); //Reflect
                    client.AddSpell(new Spell(true) { ID = 1040 }); //Roar
                }
                else if (client.Entity.Class == 11 || client.Entity.Class == 51 || client.Entity.Class == 61 ||
                         client.Entity.Class == 71 || client.Entity.Class == 81)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 6001 }); //Toxic Fog
                    client.AddSpell(new Spell(true) { ID = 1015 }); //Accuracy
                    client.AddSpell(new Spell(true) { ID = 3060 }); //Reflect
                    client.AddSpell(new Spell(true) { ID = 1040 }); //Roar
                    client.AddSpell(new Spell(true) { ID = 1320 }); //FlyingMoon
                }
                else if (client.Entity.Class == 21)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 6001 }); //Toxic Fog
                    client.AddSpell(new Spell(true) { ID = 3060 }); //Reflect
                }
                else if (client.Entity.Class == 132)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 6001 }); //Toxic Fog
                    client.AddSpell(new Spell(true) { ID = 1020 }); //ShieldShield
                    client.AddSpell(new Spell(true) { ID = 1025 }); //SuperMan
                    client.AddSpell(new Spell(true) { ID = 3060 }); //Reflect
                    client.AddSpell(new Spell(true) { ID = 1040 }); //Roar
                }
            }

            #endregion
            #region Nin-Water

            if (client.Entity.FirstRebornClass == 55 && client.Entity.SecondRebornClass == 135)
            {
                if (client.Entity.Class == 41)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 6001 }); //Toxic Fog
                    client.AddSpell(new Spell(true) { ID = 1005 }); //Cure
                    client.AddSpell(new Spell(true) { ID = 1095 }); //Stigma
                    client.AddSpell(new Spell(true) { ID = 1195 }); //Meditation
                    client.AddSpell(new Spell(true) { ID = 1090 }); //MagicShield
                    client.AddSpell(new Spell(true) { ID = 1075 }); //Invisibility
                }
                else if (client.Entity.Class == 142)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 6001 }); //Toxic Fog
                    client.AddSpell(new Spell(true) { ID = 1050 }); //XPRevive
                    client.AddSpell(new Spell(true) { ID = 1175 }); //AdvancedCure
                    client.AddSpell(new Spell(true) { ID = 1075 }); //Invisibility
                    client.AddSpell(new Spell(true) { ID = 1055 }); //HealingRain
                }
                else if (client.Entity.Class == 11 || client.Entity.Class == 21 || client.Entity.Class == 51 ||
                         client.Entity.Class == 61 || client.Entity.Class == 71 || client.Entity.Class == 81)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 6001 }); //Toxic Fog
                    client.AddSpell(new Spell(true) { ID = 1005 }); //Cure
                    client.AddSpell(new Spell(true) { ID = 1085 }); //StarOfAccuracy
                    client.AddSpell(new Spell(true) { ID = 1090 }); //MagicShield
                    client.AddSpell(new Spell(true) { ID = 1095 }); //Stigma
                    client.AddSpell(new Spell(true) { ID = 1195 }); //Meditation
                }
                else if (client.Entity.Class == 132)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 6001 }); //Toxic Fog
                    client.AddSpell(new Spell(true) { ID = 3090 }); //Pervade
                }
            }

            #endregion
            #region Nin-Nin

            if (client.Entity.FirstRebornClass == 55 && client.Entity.SecondRebornClass == 55)
            {
                if (client.Entity.Class == 51)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 6000 }); //TwofoldBlades
                    client.AddSpell(new Spell(true) { ID = 6001 }); //ToxicFog
                    client.AddSpell(new Spell(true) { ID = 6002 }); //PoisonStar
                    client.AddSpell(new Spell(true) { ID = 6003 }); //Counter-Kill
                    client.AddSpell(new Spell(true) { ID = 6004 }); //ArcherBane
                    client.AddSpell(new Spell(true) { ID = 6010 }); //ShurikenVortex
                    client.AddSpell(new Spell(true) { ID = 6011 }); //FatalStrike
                    client.AddSpell(new Spell(true) { ID = 12070 }); //TwilightDance
                    client.AddSpell(new Spell(true) { ID = 12080 }); //SuperTwofoldBlade
                    client.AddSpell(new Spell(true) { ID = 12110 }); //FatalSpin
                }
                else
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 6001 }); //Toxic Fog
                }
            }

            #endregion
            #region Nin-Monk

            if (client.Entity.FirstRebornClass == 55 && client.Entity.SecondRebornClass == 65)
            {
                client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                client.AddSpell(new Spell(true) { ID = 10400 }); //Serenity
                client.AddSpell(new Spell(true) { ID = 6001 }); //ToxicFog
            }

            #endregion
            #region Nin-Pirate

            if (client.Entity.FirstRebornClass == 55 && client.Entity.SecondRebornClass == 75)
            {
                client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                client.AddSpell(new Spell(true) { ID = 11070 }); //Gale Bomb
                client.AddSpell(new Spell(true) { ID = 6001 }); //ToxicFog
            }

            #endregion
            #region Nin-KungFu
            if (client.Entity.FirstRebornClass == 55 && client.Entity.SecondRebornClass == 85)
            {
                client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                client.AddSpell(new Spell(true) { ID = 6001 }); //ToxicFog
            }
            #endregion
            #endregion
            #region Fire2
            #region Fire-Arch

            if (client.Entity.FirstRebornClass == 145 && client.Entity.SecondRebornClass == 45)
            {
                client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                client.AddSpell(new Spell(true) { ID = 1000 }); //Thunder
                client.AddSpell(new Spell(true) { ID = 1001 }); //Fire
                client.AddSpell(new Spell(true) { ID = 1005 }); //Cure
                client.AddSpell(new Spell(true) { ID = 1195 }); //Meditation
                client.AddSpell(new Spell(true) { ID = 5002 }); //PoisonousArrows
            }

            #endregion
            #region Fire-Fire

            if (client.Entity.FirstRebornClass == 145 && client.Entity.SecondRebornClass == 145)
            {
                if (client.Entity.Class == 41 || client.Entity.Class == 11 || client.Entity.Class == 51 ||
                    client.Entity.Class == 21 || client.Entity.Class == 71 || client.Entity.Class == 81)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 3080 }); //Dodge
                    client.AddSpell(new Spell(true) { ID = 1000 }); //Thunder
                    client.AddSpell(new Spell(true) { ID = 1001 }); //fire
                    client.AddSpell(new Spell(true) { ID = 1005 }); //cura
                    client.AddSpell(new Spell(true) { ID = 1195 }); //Meditation
                }
                else if (client.Entity.Class == 142)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 10310 }); //Heaven Blade 
                    client.AddSpell(new Spell(true) { ID = 3080 }); //Dodge
                }
                else if (client.Entity.Class == 132)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 3080 }); //
                    client.AddSpell(new Spell(true) { ID = 1120 }); //FireCircle
                }
                else if (client.Entity.Class == 61)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1350 }); //DivineHare
                    client.AddSpell(new Spell(true) { ID = 1280 }); //Water Elf
                }
            }

            #endregion
            #region Fire-Tro

            if (client.Entity.FirstRebornClass == 145 && client.Entity.SecondRebornClass == 15)
            {
                if (client.Entity.Class == 41 || client.Entity.Class == 142 || client.Entity.Class == 132 ||
                    client.Entity.Class == 51 || client.Entity.Class == 61)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1000 }); //thunder
                    client.AddSpell(new Spell(true) { ID = 1001 }); //fire
                    client.AddSpell(new Spell(true) { ID = 1005 }); //cura
                    client.AddSpell(new Spell(true) { ID = 1195 }); //Meditation
                    client.AddSpell(new Spell(true) { ID = 1270 }); //Golem
                    client.AddSpell(new Spell(true) { ID = 1110 }); //cyclona
                    client.AddSpell(new Spell(true) { ID = 1190 }); //SpiritHealing
                }
                else if (client.Entity.Class == 71)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1000 }); //thunder
                    client.AddSpell(new Spell(true) { ID = 1001 }); //fire
                    client.AddSpell(new Spell(true) { ID = 1005 }); //cura
                    client.AddSpell(new Spell(true) { ID = 1195 }); //Meditation
                    client.AddSpell(new Spell(true) { ID = 1110 }); //cyclona
                }
                else if (client.Entity.Class == 21)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1000 }); //thunder
                    client.AddSpell(new Spell(true) { ID = 1001 }); //fire
                    client.AddSpell(new Spell(true) { ID = 1005 }); //cura
                    client.AddSpell(new Spell(true) { ID = 1195 }); //Meditation
                    client.AddSpell(new Spell(true) { ID = 1270 }); //Golem
                    client.AddSpell(new Spell(true) { ID = 5100 }); //IronShirt
                    client.AddSpell(new Spell(true) { ID = 1110 }); //cyclona
                    client.AddSpell(new Spell(true) { ID = 1190 }); //SpiritHealing
                }
                else if (client.Entity.Class == 11)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1000 }); //thunder
                    client.AddSpell(new Spell(true) { ID = 1001 }); //fire
                    client.AddSpell(new Spell(true) { ID = 1005 }); //cura
                    client.AddSpell(new Spell(true) { ID = 1195 }); //Meditation
                    client.AddSpell(new Spell(true) { ID = 3050 }); //CruelShade
                }
            }

            #endregion
            #region Fire-War

            if (client.Entity.FirstRebornClass == 145 && client.Entity.SecondRebornClass == 25)
            {
                if (client.Entity.Class == 41)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1000 }); //
                    client.AddSpell(new Spell(true) { ID = 1001 }); //
                    client.AddSpell(new Spell(true) { ID = 1005 }); //
                    client.AddSpell(new Spell(true) { ID = 1195 }); //
                    client.AddSpell(new Spell(true) { ID = 1020 }); //
                    client.AddSpell(new Spell(true) { ID = 1040 }); //
                    client.AddSpell(new Spell(true) { ID = 3060 }); //
                }
                else if (client.Entity.Class == 142)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1020 }); //
                    client.AddSpell(new Spell(true) { ID = 1040 }); //
                    client.AddSpell(new Spell(true) { ID = 3060 }); //
                }
                else if (client.Entity.Class == 11 || client.Entity.Class == 51 || client.Entity.Class == 61 ||
                         client.Entity.Class == 71 || client.Entity.Class == 81)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1000 }); //
                    client.AddSpell(new Spell(true) { ID = 1001 }); //
                    client.AddSpell(new Spell(true) { ID = 1005 }); //
                    client.AddSpell(new Spell(true) { ID = 1195 }); //
                    client.AddSpell(new Spell(true) { ID = 1015 }); //
                    client.AddSpell(new Spell(true) { ID = 1040 }); //
                    client.AddSpell(new Spell(true) { ID = 1320 }); //
                    client.AddSpell(new Spell(true) { ID = 3060 }); //
                }
                else if (client.Entity.Class == 21)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1000 }); //
                    client.AddSpell(new Spell(true) { ID = 1001 }); //
                    client.AddSpell(new Spell(true) { ID = 1005 }); //
                    client.AddSpell(new Spell(true) { ID = 1195 }); //
                    client.AddSpell(new Spell(true) { ID = 3060 }); //
                }
                else if (client.Entity.Class == 132)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1000 }); //
                    client.AddSpell(new Spell(true) { ID = 1001 }); //
                    client.AddSpell(new Spell(true) { ID = 1005 }); //
                    client.AddSpell(new Spell(true) { ID = 1195 }); //
                    client.AddSpell(new Spell(true) { ID = 3060 }); //
                    client.AddSpell(new Spell(true) { ID = 1025 }); //
                    client.AddSpell(new Spell(true) { ID = 1020 }); //
                    client.AddSpell(new Spell(true) { ID = 1040 }); //
                }
            }

            #endregion
            #region Fire-Water

            if (client.Entity.FirstRebornClass == 145 && client.Entity.SecondRebornClass == 135)
            {
                if (client.Entity.Class == 41)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1005 }); //
                    client.AddSpell(new Spell(true) { ID = 1195 }); //
                    client.AddSpell(new Spell(true) { ID = 1095 }); //
                    client.AddSpell(new Spell(true) { ID = 1090 }); //
                    client.AddSpell(new Spell(true) { ID = 1075 }); //Invisibility
                }
                else if (client.Entity.Class == 142)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1050 }); //
                    client.AddSpell(new Spell(true) { ID = 1055 }); //
                    client.AddSpell(new Spell(true) { ID = 1175 }); //
                    client.AddSpell(new Spell(true) { ID = 1075 }); //Invisibility
                }
                else if (client.Entity.Class == 11 || client.Entity.Class == 21 || client.Entity.Class == 51)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1005 }); //
                    client.AddSpell(new Spell(true) { ID = 1085 }); //
                    client.AddSpell(new Spell(true) { ID = 1090 }); //
                    client.AddSpell(new Spell(true) { ID = 1095 }); //
                    client.AddSpell(new Spell(true) { ID = 1195 }); //
                }
                else if (client.Entity.Class == 132)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 3090 }); //
                    client.AddSpell(new Spell(true) { ID = 1120 }); //
                }
                else if (client.Entity.Class == 61 || client.Entity.Class == 71 || client.Entity.Class == 81)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1005 }); //
                    client.AddSpell(new Spell(true) { ID = 1085 }); //
                    client.AddSpell(new Spell(true) { ID = 1090 }); //
                    client.AddSpell(new Spell(true) { ID = 1095 }); //
                    client.AddSpell(new Spell(true) { ID = 1195 }); //
                    client.AddSpell(new Spell(true) { ID = 1350 }); //DivineHare
                    client.AddSpell(new Spell(true) { ID = 1280 }); //Water Elf
                }
            }

            #endregion
            #region Fire-Nin

            if (client.Entity.FirstRebornClass == 145 && client.Entity.SecondRebornClass == 55)
            {
                if (client.Entity.Class == 51 || client.Entity.Class == 61 || client.Entity.Class == 71 ||
                    client.Entity.Class == 81)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 3080 }); //
                    client.AddSpell(new Spell(true) { ID = 6001 }); //
                    client.AddSpell(new Spell(true) { ID = 1000 }); //
                    client.AddSpell(new Spell(true) { ID = 1001 }); //
                    client.AddSpell(new Spell(true) { ID = 1005 }); //
                    client.AddSpell(new Spell(true) { ID = 1195 }); //
                }
                else
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 6001 }); //
                    client.AddSpell(new Spell(true) { ID = 1000 }); //
                    client.AddSpell(new Spell(true) { ID = 1001 }); //
                    client.AddSpell(new Spell(true) { ID = 1005 }); //
                    client.AddSpell(new Spell(true) { ID = 1195 }); //
                }
            }

            #endregion
            #region Fire-Monk

            if (client.Entity.FirstRebornClass == 145 && client.Entity.SecondRebornClass == 65)
            {
                if (client.Entity.Class == 11 || client.Entity.Class == 21 || client.Entity.Class == 41 ||
                    client.Entity.Class == 132 || client.Entity.Class == 142)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1000 }); //Thunder
                    client.AddSpell(new Spell(true) { ID = 1000 }); //Fire
                    client.AddSpell(new Spell(true) { ID = 1005 }); //Cure
                    client.AddSpell(new Spell(true) { ID = 1195 }); //Meditation
                    client.AddSpell(new Spell(true) { ID = 10400 }); //Serenity
                }
                else if (client.Entity.Class == 51 || client.Entity.Class == 61 || client.Entity.Class == 71 ||
                    client.Entity.Class == 81)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1000 }); //Thunder
                    client.AddSpell(new Spell(true) { ID = 1000 }); //Fire
                    client.AddSpell(new Spell(true) { ID = 1005 }); //Cure
                    client.AddSpell(new Spell(true) { ID = 1195 }); //Meditation
                    client.AddSpell(new Spell(true) { ID = 10400 }); //Serenity
                    client.AddSpell(new Spell(true) { ID = 3080 }); //Dodge
                }
            }

            #endregion
            #region Fire-Pirate

            if (client.Entity.FirstRebornClass == 145 && client.Entity.SecondRebornClass == 75)
            {
                client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                client.AddSpell(new Spell(true) { ID = 1000 }); //Thunder
                client.AddSpell(new Spell(true) { ID = 1000 }); //Fire
                client.AddSpell(new Spell(true) { ID = 1005 }); //Cure
                client.AddSpell(new Spell(true) { ID = 1195 }); //Meditation
                client.AddSpell(new Spell(true) { ID = 11070 }); //Gale Bomb
            }

            #endregion
            #region Fire-KungFu
            if (client.Entity.FirstRebornClass == 145 && client.Entity.SecondRebornClass == 85)
            {
                client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                client.AddSpell(new Spell(true) { ID = 1000 }); //Thunder
                client.AddSpell(new Spell(true) { ID = 1000 }); //Fire
                client.AddSpell(new Spell(true) { ID = 1005 }); //Cure
                client.AddSpell(new Spell(true) { ID = 1195 }); //Meditation
            }
            #endregion
            #endregion
            #region War2
            #region War-Arch

            if (client.Entity.FirstRebornClass == 25 && client.Entity.SecondRebornClass == 45)
            {
                if (client.Entity.Class == 41)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 5000 }); //Freezing Arrows
                }
                else if (client.Entity.Class == 132 || client.Entity.Class == 142)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1020 });
                    client.AddSpell(new Spell(true) { ID = 1040 });
                    client.AddSpell(new Spell(true) { ID = 5002 }); //Poisonous Arrows
                }
                else if (client.Entity.Class == 11 || client.Entity.Class == 51 || client.Entity.Class == 61 ||
                         client.Entity.Class == 71 || client.Entity.Class == 81)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1040 });
                    client.AddSpell(new Spell(true) { ID = 5002 });
                }
                else if (client.Entity.Class == 21)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 5002 });
                }
            }

            #endregion
            #region War-Fire

            if (client.Entity.FirstRebornClass == 25 && client.Entity.SecondRebornClass == 145)
            {
                if (client.Entity.Class == 41)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1020 });
                    client.AddSpell(new Spell(true) { ID = 1040 });
                    client.AddSpell(new Spell(true) { ID = 1000 });
                    client.AddSpell(new Spell(true) { ID = 1001 });
                    client.AddSpell(new Spell(true) { ID = 1005 });
                    client.AddSpell(new Spell(true) { ID = 1195 });
                }
                else if (client.Entity.Class == 142)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1020 });
                    client.AddSpell(new Spell(true) { ID = 1040 });
                    client.AddSpell(new Spell(true) { ID = 3080 });
                }
                else if (client.Entity.Class == 11 || client.Entity.Class == 51 || client.Entity.Class == 61 ||
                         client.Entity.Class == 71 || client.Entity.Class == 81)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1040 });
                    client.AddSpell(new Spell(true) { ID = 1000 });
                    client.AddSpell(new Spell(true) { ID = 1001 });
                    client.AddSpell(new Spell(true) { ID = 1005 });
                    client.AddSpell(new Spell(true) { ID = 1195 });
                }
                else if (client.Entity.Class == 25)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1000 });
                    client.AddSpell(new Spell(true) { ID = 1001 });
                    client.AddSpell(new Spell(true) { ID = 1005 });
                    client.AddSpell(new Spell(true) { ID = 1195 });
                }
                else if (client.Entity.Class == 132)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1020 });
                    client.AddSpell(new Spell(true) { ID = 1040 });
                    client.AddSpell(new Spell(true) { ID = 1120 });
                }
            }

            #endregion
            #region War-Tro

            if (client.Entity.FirstRebornClass == 25 && client.Entity.SecondRebornClass == 15)
            {
                if (client.Entity.Class == 41 || client.Entity.Class == 142 || client.Entity.Class == 132 ||
                    client.Entity.Class == 51 || client.Entity.Class == 61 || client.Entity.Class == 71 ||
                    client.Entity.Class == 81)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1320 });
                    client.AddSpell(new Spell(true) { ID = 1040 });
                    client.AddSpell(new Spell(true) { ID = 1110 });
                    client.AddSpell(new Spell(true) { ID = 1190 });
                    client.AddSpell(new Spell(true) { ID = 1270 });
                }
                else if (client.Entity.Class == 11)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1320 });
                    client.AddSpell(new Spell(true) { ID = 1040 });
                    client.AddSpell(new Spell(true) { ID = 3050 });
                }
                else if (client.Entity.Class == 21)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 5100 });
                    client.AddSpell(new Spell(true) { ID = 1110 });
                    client.AddSpell(new Spell(true) { ID = 1190 });
                    client.AddSpell(new Spell(true) { ID = 1270 });
                }
            }

            #endregion
            #region War-War

            if (client.Entity.FirstRebornClass == 25 && client.Entity.SecondRebornClass == 25)
            {
                if (client.Entity.Class == 41 || client.Entity.Class == 142)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1040 });
                    client.AddSpell(new Spell(true) { ID = 1320 });
                    client.AddSpell(new Spell(true) { ID = 3060 });
                }
                else if (client.Entity.Class == 11 || client.Entity.Class == 51 || client.Entity.Class == 61 ||
                         client.Entity.Class == 71 || client.Entity.Class == 81)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1040 });
                    client.AddSpell(new Spell(true) { ID = 1020 });
                    client.AddSpell(new Spell(true) { ID = 3060 });
                    client.AddSpell(new Spell(true) { ID = 1015 });
                }
                else if (client.Entity.Class == 21)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 10311 }); //Perseverance
                    client.AddSpell(new Spell(true) { ID = 3060 });
                }
                else if (client.Entity.Class == 132)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1025 });
                    client.AddSpell(new Spell(true) { ID = 1040 });
                    client.AddSpell(new Spell(true) { ID = 1320 });
                    client.AddSpell(new Spell(true) { ID = 3060 });
                }
            }

            #endregion
            #region War-Water

            if (client.Entity.FirstRebornClass == 25 && client.Entity.SecondRebornClass == 135)
            {
                if (client.Entity.Class == 41)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1020 });
                    client.AddSpell(new Spell(true) { ID = 1040 });
                    client.AddSpell(new Spell(true) { ID = 1005 });
                    client.AddSpell(new Spell(true) { ID = 1075 }); //Invisibility
                    client.AddSpell(new Spell(true) { ID = 1090 });
                    client.AddSpell(new Spell(true) { ID = 1095 });
                    client.AddSpell(new Spell(true) { ID = 1195 });
                    client.AddSpell(new Spell(true) { ID = 1280 });
                    client.AddSpell(new Spell(true) { ID = 1350 });
                }
                if (client.Entity.Class == 142)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1020 });
                    client.AddSpell(new Spell(true) { ID = 1040 });
                    client.AddSpell(new Spell(true) { ID = 1005 });
                    client.AddSpell(new Spell(true) { ID = 1175 });
                    client.AddSpell(new Spell(true) { ID = 1050 });
                    client.AddSpell(new Spell(true) { ID = 1055 });
                    client.AddSpell(new Spell(true) { ID = 1280 });
                    client.AddSpell(new Spell(true) { ID = 1350 });
                    client.AddSpell(new Spell(true) { ID = 1075 }); //Invisibility
                }
                else if (client.Entity.Class == 11)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1005 });
                    client.AddSpell(new Spell(true) { ID = 1040 });
                    client.AddSpell(new Spell(true) { ID = 1085 });
                    client.AddSpell(new Spell(true) { ID = 1090 });
                    client.AddSpell(new Spell(true) { ID = 1095 });
                    client.AddSpell(new Spell(true) { ID = 1195 });
                    client.AddSpell(new Spell(true) { ID = 1280 });
                    client.AddSpell(new Spell(true) { ID = 1350 });
                }
                else if (client.Entity.Class == 21)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1005 });
                    client.AddSpell(new Spell(true) { ID = 1085 });
                    client.AddSpell(new Spell(true) { ID = 1090 });
                    client.AddSpell(new Spell(true) { ID = 1095 });
                    client.AddSpell(new Spell(true) { ID = 1195 });
                    client.AddSpell(new Spell(true) { ID = 1280 });
                    client.AddSpell(new Spell(true) { ID = 1350 });
                }
                else if (client.Entity.Class == 132)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1020 });
                    client.AddSpell(new Spell(true) { ID = 1025 });
                    client.AddSpell(new Spell(true) { ID = 1040 });
                    client.AddSpell(new Spell(true) { ID = 3090 });
                    client.AddSpell(new Spell(true) { ID = 1280 });
                    client.AddSpell(new Spell(true) { ID = 1350 });
                }
                else if (client.Entity.Class == 51 || client.Entity.Class == 71 || client.Entity.Class == 81)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1005 });
                    client.AddSpell(new Spell(true) { ID = 1040 });
                    client.AddSpell(new Spell(true) { ID = 1085 });
                    client.AddSpell(new Spell(true) { ID = 1090 });
                    client.AddSpell(new Spell(true) { ID = 1095 });
                    client.AddSpell(new Spell(true) { ID = 1195 });
                }
                else if (client.Entity.Class == 61)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1005 });
                    client.AddSpell(new Spell(true) { ID = 1040 });
                    client.AddSpell(new Spell(true) { ID = 1085 });
                    client.AddSpell(new Spell(true) { ID = 1090 });
                    client.AddSpell(new Spell(true) { ID = 1095 });
                    client.AddSpell(new Spell(true) { ID = 1195 });
                    client.AddSpell(new Spell(true) { ID = 1350 }); //DivineHare
                    client.AddSpell(new Spell(true) { ID = 1280 }); //Water Elf
                }
            }

            #endregion
            #region War-Nin

            if (client.Entity.FirstRebornClass == 25 && client.Entity.SecondRebornClass == 55)
            {
                if (client.Entity.Class == 51)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1320 });
                    client.AddSpell(new Spell(true) { ID = 1040 });
                }
                else
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1320 });
                    client.AddSpell(new Spell(true) { ID = 1040 });
                    client.AddSpell(new Spell(true) { ID = 6001 });
                }
            }

            #endregion
            #region War-Monk

            if (client.Entity.FirstRebornClass == 25 && client.Entity.SecondRebornClass == 65)
            {
                if (client.Entity.Class == 21 || client.Entity.Class == 41 || client.Entity.Class == 51 ||
                    client.Entity.Class == 61 || client.Entity.Class == 142 || client.Entity.Class == 71 ||
                    client.Entity.Class == 81)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1040 }); //Roar
                    client.AddSpell(new Spell(true) { ID = 1320 }); //FlyingMoon
                    client.AddSpell(new Spell(true) { ID = 10400 }); //Serenity
                }
                else if (client.Entity.Class == 11)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1040 }); //Roar
                    client.AddSpell(new Spell(true) { ID = 1320 }); //FlyingMoon
                    client.AddSpell(new Spell(true) { ID = 10400 }); //Serenity
                    client.AddSpell(new Spell(true) { ID = 1110 }); //Cyclone
                }
                else if (client.Entity.Class == 132)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1040 }); //Roar
                    client.AddSpell(new Spell(true) { ID = 1320 }); //FlyingMoon
                    client.AddSpell(new Spell(true) { ID = 10400 }); //Serenity
                    client.AddSpell(new Spell(true) { ID = 3080 }); // Dodge
                }
            }

            #endregion
            #region War-Pirate

            if (client.Entity.FirstRebornClass == 25 && client.Entity.SecondRebornClass == 75)
            {
                client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                client.AddSpell(new Spell(true) { ID = 1040 }); //Roar
                client.AddSpell(new Spell(true) { ID = 1320 }); //FlyingMoon
                client.AddSpell(new Spell(true) { ID = 11070 }); //Gale Bomb
                client.AddSpell(new Spell(true) { ID = 3060 }); // Reflec
            }

            #endregion
            #region War-KungFu
            if (client.Entity.FirstRebornClass == 25 && client.Entity.SecondRebornClass == 81)
            {
                client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                client.AddSpell(new Spell(true) { ID = 1040 }); //Roar
                client.AddSpell(new Spell(true) { ID = 1320 }); //FlyingMoon
                client.AddSpell(new Spell(true) { ID = 3060 }); // Reflec
            }
            #endregion
            #endregion
            #region Water2
            #region Water-Arch

            if (client.Entity.FirstRebornClass == 135 && client.Entity.SecondRebornClass == 45)
            {
                if (client.Entity.Class == 41)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1005 });
                    client.AddSpell(new Spell(true) { ID = 1075 }); //Invisibility
                    client.AddSpell(new Spell(true) { ID = 1090 });
                    client.AddSpell(new Spell(true) { ID = 1095 });
                    client.AddSpell(new Spell(true) { ID = 1195 });
                    client.AddSpell(new Spell(true) { ID = 5000 });
                    client.AddSpell(new Spell(true) { ID = 5002 });
                }
                else if (client.Entity.Class == 61)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1005 });
                    client.AddSpell(new Spell(true) { ID = 1075 }); //Invisibility
                    client.AddSpell(new Spell(true) { ID = 1090 });
                    client.AddSpell(new Spell(true) { ID = 1095 });
                    client.AddSpell(new Spell(true) { ID = 1195 });
                    client.AddSpell(new Spell(true) { ID = 5000 });
                    client.AddSpell(new Spell(true) { ID = 5002 });
                    client.AddSpell(new Spell(true) { ID = 1350 }); //DivineHare
                    client.AddSpell(new Spell(true) { ID = 1280 }); //Water Elf
                }
                else if (client.Entity.Class == 71 || client.Entity.Class == 81)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1005 });
                    client.AddSpell(new Spell(true) { ID = 1075 }); //Invisibility
                    client.AddSpell(new Spell(true) { ID = 1090 });
                    client.AddSpell(new Spell(true) { ID = 1095 });
                    client.AddSpell(new Spell(true) { ID = 1085 }); //Star Of Accuracy
                    client.AddSpell(new Spell(true) { ID = 5000 });
                    client.AddSpell(new Spell(true) { ID = 5002 });
                    client.AddSpell(new Spell(true) { ID = 1350 }); //DivineHare
                    client.AddSpell(new Spell(true) { ID = 1280 }); //Water Elf
                }
                else
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1005 });
                    client.AddSpell(new Spell(true) { ID = 1075 }); //Invisibility
                    client.AddSpell(new Spell(true) { ID = 1090 });
                    client.AddSpell(new Spell(true) { ID = 1095 });
                    client.AddSpell(new Spell(true) { ID = 1195 });
                    client.AddSpell(new Spell(true) { ID = 5000 });
                    client.AddSpell(new Spell(true) { ID = 5002 });
                }
            }

            #endregion
            #region Water-Fire

            if (client.Entity.FirstRebornClass == 135 && client.Entity.SecondRebornClass == 145)
            {
                if (client.Entity.Class == 11 || client.Entity.Class == 21 || client.Entity.Class == 41 ||
                    client.Entity.Class == 51 || client.Entity.Class == 71 || client.Entity.Class == 81)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1050 });
                    client.AddSpell(new Spell(true) { ID = 1175 });
                    client.AddSpell(new Spell(true) { ID = 1075 }); //Invisibility
                    client.AddSpell(new Spell(true) { ID = 1055 });
                    client.AddSpell(new Spell(true) { ID = 1000 });
                    client.AddSpell(new Spell(true) { ID = 1001 });
                    client.AddSpell(new Spell(true) { ID = 1005 });
                    client.AddSpell(new Spell(true) { ID = 1195 });
                }
                else if (client.Entity.Class == 61)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1050 });
                    client.AddSpell(new Spell(true) { ID = 1175 });
                    client.AddSpell(new Spell(true) { ID = 1075 }); //Invisibility
                    client.AddSpell(new Spell(true) { ID = 1055 });
                    client.AddSpell(new Spell(true) { ID = 1000 });
                    client.AddSpell(new Spell(true) { ID = 1001 });
                    client.AddSpell(new Spell(true) { ID = 1005 });
                    client.AddSpell(new Spell(true) { ID = 1195 });
                    client.AddSpell(new Spell(true) { ID = 1350 }); //DivineHare
                    client.AddSpell(new Spell(true) { ID = 1280 }); //Water Elf
                }
                else if (client.Entity.Class == 132)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1050 });
                    client.AddSpell(new Spell(true) { ID = 1175 });
                    client.AddSpell(new Spell(true) { ID = 1075 }); //Invisibility
                    client.AddSpell(new Spell(true) { ID = 1055 });
                    client.AddSpell(new Spell(true) { ID = 1120 });
                }
                else if (client.Entity.Class == 142)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1050 });
                    client.AddSpell(new Spell(true) { ID = 1175 });
                    client.AddSpell(new Spell(true) { ID = 1075 }); //Invisibility
                    client.AddSpell(new Spell(true) { ID = 1055 });
                    client.AddSpell(new Spell(true) { ID = 3080 });
                }
            }

            #endregion
            #region Water-Tro

            if (client.Entity.FirstRebornClass == 135 && client.Entity.SecondRebornClass == 15)
            {
                if (client.Entity.Class == 41 || client.Entity.Class == 142 || client.Entity.Class == 132 ||
                    client.Entity.Class == 51)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1005 }); //Cure
                    client.AddSpell(new Spell(true) { ID = 1085 }); //StarOfAccuracy
                    client.AddSpell(new Spell(true) { ID = 1090 }); //MagicShield
                    client.AddSpell(new Spell(true) { ID = 1095 }); //Stigma
                    client.AddSpell(new Spell(true) { ID = 1195 }); //Meditation
                    client.AddSpell(new Spell(true) { ID = 1190 }); //SpiritHealing
                    client.AddSpell(new Spell(true) { ID = 1110 }); //Cyclone
                    client.AddSpell(new Spell(true) { ID = 1270 }); //Robot
                }
                else if (client.Entity.Class == 61)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1005 }); //Cure
                    client.AddSpell(new Spell(true) { ID = 1085 }); //StarOfAccuracy
                    client.AddSpell(new Spell(true) { ID = 1090 }); //MagicShield
                    client.AddSpell(new Spell(true) { ID = 1095 }); //Stigma
                    client.AddSpell(new Spell(true) { ID = 1195 }); //Meditation
                    client.AddSpell(new Spell(true) { ID = 1190 }); //SpiritHealing
                    client.AddSpell(new Spell(true) { ID = 1110 }); //Cyclone
                    client.AddSpell(new Spell(true) { ID = 1270 }); //Robot
                    client.AddSpell(new Spell(true) { ID = 1350 }); //DivineHare
                    client.AddSpell(new Spell(true) { ID = 1280 }); //Water Elf
                }
                else if (client.Entity.Class == 71 || client.Entity.Class == 81)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1005 }); //Cure
                    client.AddSpell(new Spell(true) { ID = 1085 }); //StarOfAccuracy
                    client.AddSpell(new Spell(true) { ID = 1090 }); //MagicShield
                    client.AddSpell(new Spell(true) { ID = 1095 }); //Stigma
                    client.AddSpell(new Spell(true) { ID = 1190 }); //SpiritHealing
                    client.AddSpell(new Spell(true) { ID = 1110 }); //Cyclone
                    client.AddSpell(new Spell(true) { ID = 1270 }); //Robot
                }
                else if (client.Entity.Class == 21)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1005 }); //
                    client.AddSpell(new Spell(true) { ID = 1085 }); //
                    client.AddSpell(new Spell(true) { ID = 1090 }); //
                    client.AddSpell(new Spell(true) { ID = 1095 }); //
                    client.AddSpell(new Spell(true) { ID = 1195 }); //
                    client.AddSpell(new Spell(true) { ID = 1190 }); //
                    client.AddSpell(new Spell(true) { ID = 1110 }); //
                    client.AddSpell(new Spell(true) { ID = 1270 }); //
                    client.AddSpell(new Spell(true) { ID = 5100 }); //
                }
                else if (client.Entity.Class == 11)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1005 }); //
                    client.AddSpell(new Spell(true) { ID = 1085 }); //
                    client.AddSpell(new Spell(true) { ID = 1090 }); //
                    client.AddSpell(new Spell(true) { ID = 1095 }); //
                    client.AddSpell(new Spell(true) { ID = 1195 }); //
                    client.AddSpell(new Spell(true) { ID = 3050 }); //
                }
            }

            #endregion
            #region Water-War

            if (client.Entity.FirstRebornClass == 135 && client.Entity.SecondRebornClass == 25)
            {
                if (client.Entity.Class == 41)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1005 }); //
                    client.AddSpell(new Spell(true) { ID = 1085 }); //
                    client.AddSpell(new Spell(true) { ID = 1090 }); //
                    client.AddSpell(new Spell(true) { ID = 1095 }); //
                    client.AddSpell(new Spell(true) { ID = 1195 }); //
                    client.AddSpell(new Spell(true) { ID = 1020 }); //
                    client.AddSpell(new Spell(true) { ID = 1040 }); //
                    client.AddSpell(new Spell(true) { ID = 3060 }); //
                    client.AddSpell(new Spell(true) { ID = 1350 }); //
                    client.AddSpell(new Spell(true) { ID = 1280 }); //
                }
                else if (client.Entity.Class == 142)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1005 }); //
                    client.AddSpell(new Spell(true) { ID = 1085 }); //
                    client.AddSpell(new Spell(true) { ID = 1090 }); //
                    client.AddSpell(new Spell(true) { ID = 1095 }); //
                    client.AddSpell(new Spell(true) { ID = 1195 }); //
                    client.AddSpell(new Spell(true) { ID = 1020 }); //
                    client.AddSpell(new Spell(true) { ID = 1040 }); //
                    client.AddSpell(new Spell(true) { ID = 3060 }); //
                }
                else if (client.Entity.Class == 11 || client.Entity.Class == 51 || client.Entity.Class == 71 ||
                    client.Entity.Class == 81)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1005 }); //
                    client.AddSpell(new Spell(true) { ID = 1085 }); //
                    client.AddSpell(new Spell(true) { ID = 1090 }); //
                    client.AddSpell(new Spell(true) { ID = 1095 }); //
                    client.AddSpell(new Spell(true) { ID = 1195 }); //
                    client.AddSpell(new Spell(true) { ID = 1015 }); //
                    client.AddSpell(new Spell(true) { ID = 1040 }); //
                    client.AddSpell(new Spell(true) { ID = 3060 }); //
                    client.AddSpell(new Spell(true) { ID = 1320 }); //
                }
                else if (client.Entity.Class == 61)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1005 }); //
                    client.AddSpell(new Spell(true) { ID = 1085 }); //
                    client.AddSpell(new Spell(true) { ID = 1090 }); //
                    client.AddSpell(new Spell(true) { ID = 1095 }); //
                    client.AddSpell(new Spell(true) { ID = 1195 }); //
                    client.AddSpell(new Spell(true) { ID = 1015 }); //
                    client.AddSpell(new Spell(true) { ID = 1040 }); //
                    client.AddSpell(new Spell(true) { ID = 3060 }); //
                    client.AddSpell(new Spell(true) { ID = 1320 }); //
                    client.AddSpell(new Spell(true) { ID = 1350 }); //DivineHare
                    client.AddSpell(new Spell(true) { ID = 1280 }); //Water Elf
                }
                else if (client.Entity.Class == 21)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1005 }); //
                    client.AddSpell(new Spell(true) { ID = 1085 }); //
                    client.AddSpell(new Spell(true) { ID = 1090 }); //
                    client.AddSpell(new Spell(true) { ID = 1095 }); //
                    client.AddSpell(new Spell(true) { ID = 1195 }); //
                    client.AddSpell(new Spell(true) { ID = 3060 }); //
                }
                else if (client.Entity.Class == 132)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1005 }); //
                    client.AddSpell(new Spell(true) { ID = 1085 }); //
                    client.AddSpell(new Spell(true) { ID = 1090 }); //
                    client.AddSpell(new Spell(true) { ID = 1095 }); //
                    client.AddSpell(new Spell(true) { ID = 1195 }); //
                    client.AddSpell(new Spell(true) { ID = 1020 }); //
                    client.AddSpell(new Spell(true) { ID = 1040 }); //
                    client.AddSpell(new Spell(true) { ID = 3060 }); //
                    client.AddSpell(new Spell(true) { ID = 1025 }); //
                }
            }

            #endregion
            #region Water-Water

            if (client.Entity.FirstRebornClass == 135 && client.Entity.SecondRebornClass == 135)
            {
                if (client.Entity.Class == 11 || client.Entity.Class == 21 || client.Entity.Class == 41 ||
                    client.Entity.Class == 51 || client.Entity.Class == 71 || client.Entity.Class == 81)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1005 }); //
                    client.AddSpell(new Spell(true) { ID = 1085 }); //
                    client.AddSpell(new Spell(true) { ID = 1090 }); //
                    client.AddSpell(new Spell(true) { ID = 1095 }); //
                    client.AddSpell(new Spell(true) { ID = 1195 }); //
                    client.AddSpell(new Spell(true) { ID = 3090 }); //
                }
                else if (client.Entity.Class == 61)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1005 }); //
                    client.AddSpell(new Spell(true) { ID = 1085 }); //
                    client.AddSpell(new Spell(true) { ID = 1090 }); //
                    client.AddSpell(new Spell(true) { ID = 1095 }); //
                    client.AddSpell(new Spell(true) { ID = 1195 }); //
                    client.AddSpell(new Spell(true) { ID = 3090 }); //
                    client.AddSpell(new Spell(true) { ID = 1350 }); //DivineHare
                    client.AddSpell(new Spell(true) { ID = 1280 }); //Water Elf
                }
                else if (client.Entity.Class == 132)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 3090 }); //
                    client.AddSpell(new Spell(true) { ID = 30000 }); //Azure Shield
                }
                else if (client.Entity.Class == 142)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1050 }); //
                    client.AddSpell(new Spell(true) { ID = 1075 }); //Invisibility
                    client.AddSpell(new Spell(true) { ID = 1055 }); //
                    client.AddSpell(new Spell(true) { ID = 1175 }); //
                    client.AddSpell(new Spell(true) { ID = 3090 }); //
                }
            }

            #endregion
            #region Water-Nin

            if (client.Entity.FirstRebornClass == 135 && client.Entity.SecondRebornClass == 55)
            {
                client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                client.AddSpell(new Spell(true) { ID = 1005 }); //
                client.AddSpell(new Spell(true) { ID = 1085 }); //
                client.AddSpell(new Spell(true) { ID = 1090 }); //
                client.AddSpell(new Spell(true) { ID = 1095 }); //
                client.AddSpell(new Spell(true) { ID = 1195 }); //
                client.AddSpell(new Spell(true) { ID = 6001 }); //
            }

            #endregion
            #region Water-Monk

            if (client.Entity.FirstRebornClass == 135 && client.Entity.SecondRebornClass == 65)
            {
                client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                client.AddSpell(new Spell(true) { ID = 1005 }); //Cure
                client.AddSpell(new Spell(true) { ID = 1085 }); //Star of Accuracy
                client.AddSpell(new Spell(true) { ID = 1090 }); //Magic Shield
                client.AddSpell(new Spell(true) { ID = 1095 }); //Stigma
                client.AddSpell(new Spell(true) { ID = 1195 }); //Meditation
                client.AddSpell(new Spell(true) { ID = 10400 }); //Serenity
            }

            #endregion
            #region Water-Pirate

            if (client.Entity.FirstRebornClass == 135 && client.Entity.SecondRebornClass == 75)
            {
                client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                client.AddSpell(new Spell(true) { ID = 1005 }); //Cure
                client.AddSpell(new Spell(true) { ID = 1085 }); //Star of Accuracy
                client.AddSpell(new Spell(true) { ID = 1090 }); //Magic Shield
                client.AddSpell(new Spell(true) { ID = 1095 }); //Stigma
                client.AddSpell(new Spell(true) { ID = 1195 }); //Meditation
                client.AddSpell(new Spell(true) { ID = 11070 }); //Gale Bomb
            }

            #endregion
            #region Water-KungFu
            if (client.Entity.FirstRebornClass == 135 && client.Entity.SecondRebornClass == 85)
            {
                client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                client.AddSpell(new Spell(true) { ID = 1005 }); //Cure
                client.AddSpell(new Spell(true) { ID = 1085 }); //Star of Accuracy
                client.AddSpell(new Spell(true) { ID = 1090 }); //Magic Shield
                client.AddSpell(new Spell(true) { ID = 1095 }); //Stigma
                client.AddSpell(new Spell(true) { ID = 1195 }); //Meditation
            }
            #endregion
            #endregion
            #region Monk2
            #region Monk-Arch

            if (client.Entity.FirstRebornClass == 65 && client.Entity.SecondRebornClass == 45)
            {
                client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                client.AddSpell(new Spell(true) { ID = 5002 }); //Poisonous Arrows
                client.AddSpell(new Spell(true) { ID = 10400 }); //Serenity
            }

            #endregion
            #region Monk-Fire

            if (client.Entity.FirstRebornClass == 65 && client.Entity.SecondRebornClass == 145)
            {
                if (client.Entity.Class == 11 || client.Entity.Class == 21 || client.Entity.Class == 41 ||
                    client.Entity.Class == 61 || client.Entity.Class == 71 || client.Entity.Class == 81)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1000 }); //Thunder
                    client.AddSpell(new Spell(true) { ID = 1001 }); //Fire
                    client.AddSpell(new Spell(true) { ID = 1005 }); //Cure
                    client.AddSpell(new Spell(true) { ID = 1195 }); //Meditation
                    client.AddSpell(new Spell(true) { ID = 10400 }); //Serenity
                }
                else if (client.Entity.Class == 142)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 10400 }); //Serenity
                }
                else if (client.Entity.Class == 132)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1120 }); //Fire Circle
                    client.AddSpell(new Spell(true) { ID = 10400 }); //Serenity
                }
                else if (client.Entity.Class == 51)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1085 }); //Star of Accuracy
                    client.AddSpell(new Spell(true) { ID = 1090 }); //Magic Shield
                    client.AddSpell(new Spell(true) { ID = 1095 }); //Stigma
                    client.AddSpell(new Spell(true) { ID = 1005 }); //Cure
                    client.AddSpell(new Spell(true) { ID = 1195 }); //Meditation
                    client.AddSpell(new Spell(true) { ID = 10400 }); //Serenity
                }
            }

            #endregion
            #region Monk-Tro

            if (client.Entity.FirstRebornClass == 65 && client.Entity.SecondRebornClass == 15)
            {
                client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                client.AddSpell(new Spell(true) { ID = 10400 }); //Serenity
                client.AddSpell(new Spell(true) { ID = 1190 }); //SpiritHealing
                client.AddSpell(new Spell(true) { ID = 1110 }); //Cyclone
                client.AddSpell(new Spell(true) { ID = 1270 }); //Robot
            }

            #endregion
            #region Monk-War

            if (client.Entity.FirstRebornClass == 65 && client.Entity.SecondRebornClass == 25)
            {
                if (client.Entity.Class == 41 || client.Entity.Class == 142)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 10400 }); //Serenity
                    client.AddSpell(new Spell(true) { ID = 1020 }); //Shield
                    client.AddSpell(new Spell(true) { ID = 1040 }); //Roar
                    client.AddSpell(new Spell(true) { ID = 3060 }); //Reflect
                }
                else if (client.Entity.Class == 11 || client.Entity.Class == 51 || client.Entity.Class == 61 ||
                         client.Entity.Class == 71 || client.Entity.Class == 81)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 10400 }); //Serenity
                    client.AddSpell(new Spell(true) { ID = 1020 }); //Shield
                    client.AddSpell(new Spell(true) { ID = 1040 }); //Roar
                    client.AddSpell(new Spell(true) { ID = 3060 }); //Reflect
                    client.AddSpell(new Spell(true) { ID = 1320 }); //Flying Moon (XP)
                    client.AddSpell(new Spell(true) { ID = 1015 }); //Accuracy
                }
                else if (client.Entity.Class == 21)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 10400 }); //Serenity
                    client.AddSpell(new Spell(true) { ID = 3060 }); //Reflect
                }
                else if (client.Entity.Class == 132)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 10400 }); //Serenity
                    client.AddSpell(new Spell(true) { ID = 1020 }); //Shield
                    client.AddSpell(new Spell(true) { ID = 1040 }); //Roar
                    client.AddSpell(new Spell(true) { ID = 3060 }); //Reflect
                    client.AddSpell(new Spell(true) { ID = 1025 }); //Superman
                }
            }

            #endregion
            #region Monk-Water

            if (client.Entity.FirstRebornClass == 65 && client.Entity.SecondRebornClass == 135)
            {
                if (client.Entity.Class == 11 || client.Entity.Class == 21 || client.Entity.Class == 41 ||
                    client.Entity.Class == 51 || client.Entity.Class == 61)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 10400 }); //Serenity
                    client.AddSpell(new Spell(true) { ID = 1020 }); //Shield
                    client.AddSpell(new Spell(true) { ID = 1040 }); //Roar
                    client.AddSpell(new Spell(true) { ID = 1280 }); //Water Elf
                    client.AddSpell(new Spell(true) { ID = 1195 }); //Meditation
                    client.AddSpell(new Spell(true) { ID = 1015 }); //Accuracy
                    client.AddSpell(new Spell(true) { ID = 1350 }); //DivineHare
                    client.AddSpell(new Spell(true) { ID = 1095 }); //Stigma
                }
                else if (client.Entity.Class == 132)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 3090 }); //Pervade
                    client.AddSpell(new Spell(true) { ID = 10400 }); //Serenity
                }
                else if (client.Entity.Class == 142)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 10400 }); //Serenity
                    client.AddSpell(new Spell(true) { ID = 1280 }); //Water Elf
                    client.AddSpell(new Spell(true) { ID = 1175 }); //dvanced Cure
                    client.AddSpell(new Spell(true) { ID = 1075 }); //Invisibility 
                    client.AddSpell(new Spell(true) { ID = 1055 }); //Healing Rain
                    client.AddSpell(new Spell(true) { ID = 1050 }); //XP Revive
                    client.AddSpell(new Spell(true) { ID = 1350 }); //Divine Hare
                }
                else if (client.Entity.Class == 71 || client.Entity.Class == 81)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 10400 }); //Serenity
                    client.AddSpell(new Spell(true) { ID = 1020 }); //Shield
                    client.AddSpell(new Spell(true) { ID = 1040 }); //Roar
                    client.AddSpell(new Spell(true) { ID = 1195 }); //Meditation
                    client.AddSpell(new Spell(true) { ID = 1015 }); //Accuracy
                    client.AddSpell(new Spell(true) { ID = 1095 }); //Stigma
                }
            }

            #endregion
            #region Monk-Nin

            if (client.Entity.FirstRebornClass == 65 && client.Entity.SecondRebornClass == 55)
            {
                if (client.Entity.Class == 51)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 10400 }); //Serenity
                    client.AddSpell(new Spell(true) { ID = 6002 }); //PoisonStar
                    client.AddSpell(new Spell(true) { ID = 6001 }); //Toxic Fog
                }
                else if (client.Entity.Class == 11 || client.Entity.Class == 132 || client.Entity.Class == 21 ||
                         client.Entity.Class == 41 || client.Entity.Class == 61 || client.Entity.Class == 142 ||
                         client.Entity.Class == 71 || client.Entity.Class == 81)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 10400 }); //Serenity
                    client.AddSpell(new Spell(true) { ID = 6001 }); //Toxic Fog
                }
            }

            #endregion
            #region Monk-Monk

            if (client.Entity.FirstRebornClass == 65 && client.Entity.SecondRebornClass == 65)
            {
                if (client.Entity.Class == 11 || client.Entity.Class == 21 || client.Entity.Class == 41 ||
                    client.Entity.Class == 51 || client.Entity.Class == 132 || client.Entity.Class == 142 ||
                    client.Entity.Class == 71 || client.Entity.Class == 81)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 10400 }); //Serenity
                }
                else if (client.Entity.Class == 61)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 10405 }); //soulshackle
                    client.AddSpell(new Spell(true) { ID = 10400 }); //Serenity
                }
            }

            #endregion
            #region Monk-Pirate

            if (client.Entity.FirstRebornClass == 65 && client.Entity.SecondRebornClass == 75)
            {
                client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                client.AddSpell(new Spell(true) { ID = 10400 }); //Serenity
                client.AddSpell(new Spell(true) { ID = 11070 }); //Gale Bomb
            }

            #endregion
            #region Monk-KungFu
            if (client.Entity.FirstRebornClass == 65 && client.Entity.SecondRebornClass == 85)
            {
                client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                client.AddSpell(new Spell(true) { ID = 10400 }); //Serenity
            }
            #endregion
            #endregion
            #region Pirate2
            #region Pirate-Arch

            if (client.Entity.FirstRebornClass == 75 && client.Entity.SecondRebornClass == 45)
            {
                client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                client.AddSpell(new Spell(true) { ID = 11070 }); //Gale Bomb
                client.AddSpell(new Spell(true) { ID = 5002 }); //Poisonous Arrows
            }

            #endregion
            #region Pirate-Fire

            if (client.Entity.FirstRebornClass == 75 && client.Entity.SecondRebornClass == 145)
            {
                if (client.Entity.Class == 11 || client.Entity.Class == 21 | client.Entity.Class == 41 ||
                    client.Entity.Class == 61 || client.Entity.Class == 71 || client.Entity.Class == 51 ||
                    client.Entity.Class == 81)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1000 }); //Thunder
                    client.AddSpell(new Spell(true) { ID = 1001 }); //Fire
                    client.AddSpell(new Spell(true) { ID = 1005 }); //Cure
                    client.AddSpell(new Spell(true) { ID = 1195 }); //Meditation
                    client.AddSpell(new Spell(true) { ID = 11070 }); //Gale Bomb
                }
                else if (client.Entity.Class == 142)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 11070 }); //Gale Bomb
                }
                else if (client.Entity.Class == 132)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1120 }); //Fire Circle
                    client.AddSpell(new Spell(true) { ID = 11070 }); //Gale Bomb
                }
            }

            #endregion
            #region Pirate-Tro

            if (client.Entity.FirstRebornClass == 75 && client.Entity.SecondRebornClass == 15)
            {
                client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                client.AddSpell(new Spell(true) { ID = 11070 }); //Gale Bomb
                client.AddSpell(new Spell(true) { ID = 1190 }); //SpiritHealing
                client.AddSpell(new Spell(true) { ID = 1110 }); //Cyclone
                client.AddSpell(new Spell(true) { ID = 1270 }); //Robot
            }

            #endregion
            #region Pirate-War

            if (client.Entity.FirstRebornClass == 75 && client.Entity.SecondRebornClass == 25)
            {
                if (client.Entity.Class == 41 || client.Entity.Class == 142)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 11070 }); //Gale Bomb
                    client.AddSpell(new Spell(true) { ID = 1020 }); //Shield
                    client.AddSpell(new Spell(true) { ID = 1040 }); //Roar
                    client.AddSpell(new Spell(true) { ID = 3060 }); //Reflect
                }
                else if (client.Entity.Class == 11 || client.Entity.Class == 51 || client.Entity.Class == 61 ||
                         client.Entity.Class == 71 || client.Entity.Class == 81)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 11070 }); //Gale Bomb
                    client.AddSpell(new Spell(true) { ID = 1020 }); //Shield
                    client.AddSpell(new Spell(true) { ID = 1040 }); //Roar
                    client.AddSpell(new Spell(true) { ID = 3060 }); //Reflect
                    client.AddSpell(new Spell(true) { ID = 1320 }); //Flying Moon (XP)
                    client.AddSpell(new Spell(true) { ID = 1015 }); //Accuracy
                }
                else if (client.Entity.Class == 21)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 11070 }); //Gale Bomb
                    client.AddSpell(new Spell(true) { ID = 3060 }); //Reflect
                }
                else if (client.Entity.Class == 132)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 11070 }); //Gale Bomb
                    client.AddSpell(new Spell(true) { ID = 1020 }); //Shield
                    client.AddSpell(new Spell(true) { ID = 1040 }); //Roar
                    client.AddSpell(new Spell(true) { ID = 3060 }); //Reflect
                    client.AddSpell(new Spell(true) { ID = 1025 }); //Superman
                }
            }

            #endregion
            #region Pirate-Water

            if (client.Entity.FirstRebornClass == 75 && client.Entity.SecondRebornClass == 135)
            {
                if (client.Entity.Class == 11 || client.Entity.Class == 21 || client.Entity.Class == 41 ||
                    client.Entity.Class == 51 || client.Entity.Class == 61 || client.Entity.Class == 71 ||
                    client.Entity.Class == 81)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 11070 }); //Gale Bomb
                    client.AddSpell(new Spell(true) { ID = 1020 }); //Shield
                    client.AddSpell(new Spell(true) { ID = 1040 }); //Roar
                    client.AddSpell(new Spell(true) { ID = 1280 }); //Water Elf
                    client.AddSpell(new Spell(true) { ID = 1195 }); //Meditation
                    client.AddSpell(new Spell(true) { ID = 1015 }); //Accuracy
                    client.AddSpell(new Spell(true) { ID = 1350 }); //DivineHare
                    client.AddSpell(new Spell(true) { ID = 1095 }); //Stigma
                }
                else if (client.Entity.Class == 132)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 3090 }); //Pervade
                    client.AddSpell(new Spell(true) { ID = 11070 }); //Gale Bomb
                }
                else if (client.Entity.Class == 142)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 11070 }); //Gale Bomb
                    client.AddSpell(new Spell(true) { ID = 1280 }); //Water Elf
                    client.AddSpell(new Spell(true) { ID = 1175 }); //dvanced Cure
                    client.AddSpell(new Spell(true) { ID = 1075 }); //Invisibility 
                    client.AddSpell(new Spell(true) { ID = 1055 }); //Healing Rain
                    client.AddSpell(new Spell(true) { ID = 1050 }); //XP Revive
                    client.AddSpell(new Spell(true) { ID = 1350 }); //Divine Hare
                }
            }

            #endregion
            #region Pirate-Nin

            if (client.Entity.FirstRebornClass == 75 && client.Entity.SecondRebornClass == 55)
            {
                if (client.Entity.Class == 51)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 11070 }); //Gale Bomb
                    client.AddSpell(new Spell(true) { ID = 6002 }); //PoisonStar
                }
                else if (client.Entity.Class == 11 || client.Entity.Class == 132 || client.Entity.Class == 21 ||
                         client.Entity.Class == 41 || client.Entity.Class == 51 || client.Entity.Class == 61 ||
                         client.Entity.Class == 71 || client.Entity.Class == 81)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 11070 }); //Gale Bomb
                    client.AddSpell(new Spell(true) { ID = 6001 }); //Toxic Fog
                }
            }

            #endregion
            #region Pirate-Monk

            if (client.Entity.FirstRebornClass == 75 && client.Entity.SecondRebornClass == 65)
            {
                if (client.Entity.Class == 11 || client.Entity.Class == 21 || client.Entity.Class == 41 ||
                    client.Entity.Class == 51 || client.Entity.Class == 132 || client.Entity.Class == 142 ||
                    client.Entity.Class == 81)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 11070 }); //Gale Bomb
                }
                else if (client.Entity.Class == 61)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 10405 }); //soulshackle
                    client.AddSpell(new Spell(true) { ID = 10400 }); //Serenity
                }
                else if (client.Entity.Class == 71)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 11070 }); //Gale Bomb
                    client.AddSpell(new Spell(true) { ID = 10400 }); //Serenity
                }
            }

            #endregion
            #region Pirate-Pirate

            if (client.Entity.FirstRebornClass == 75 && client.Entity.SecondRebornClass == 75)
            {
                if (client.Entity.Class == 11 || client.Entity.Class == 21 || client.Entity.Class == 41 ||
                    client.Entity.Class == 51 || client.Entity.Class == 132 || client.Entity.Class == 142 ||
                    client.Entity.Class == 61 || client.Entity.Class == 81)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 11070 }); //Gale Bomb
                }
                else if (client.Entity.Class == 71)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 11070 }); //Gale Bomb
                    client.AddSpell(new Spell(true) { ID = 11040 }); //Scurvy Bomb
                }
            }

            #endregion
            #region Pirate-KungFu
            if (client.Entity.FirstRebornClass == 75 && client.Entity.SecondRebornClass == 85)
            {
                client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                client.AddSpell(new Spell(true) { ID = 11070 }); //Gale Bomb
            }
            #endregion
            #endregion
            #region KungFuKing
            #region KungFu-Arch

            if (client.Entity.FirstRebornClass == 85 && client.Entity.SecondRebornClass == 45)
            {
                client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                client.AddSpell(new Spell(true) { ID = 5002 }); //Poisonous Arrows
                client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                client.AddSpell(new Spell(true) { ID = 11620 }); //PathOfShadow
                client.AddSpell(new Spell(true) { ID = 11610 }); //Blade Furry
                client.AddSpell(new Spell(true) { ID = 11660 }); //Mortal Wound
                client.AddSpell(new Spell(true) { ID = 11650 }); //Blistering Wave
                client.AddSpell(new Spell(true) { ID = 11670 }); //SpiritFocus
                client.AddSpell(new Spell(true) { ID = 11590 }); //Kinetic Spark
                client.AddSpell(new Spell(true) { ID = 11600 }); //Dagger Storm
            }

            #endregion
            #region KungFu-Fire

            if (client.Entity.FirstRebornClass == 85 && client.Entity.SecondRebornClass == 145)
            {
                if (client.Entity.Class == 11 || client.Entity.Class == 21 | client.Entity.Class == 41 ||
                    client.Entity.Class == 61 || client.Entity.Class == 71 || client.Entity.Class == 51 ||
                    client.Entity.Class == 71)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1000 }); //Thunder
                    client.AddSpell(new Spell(true) { ID = 1001 }); //Fire
                    client.AddSpell(new Spell(true) { ID = 1005 }); //Cure
                    client.AddSpell(new Spell(true) { ID = 1195 }); //Meditation
                }
                else if (client.Entity.Class == 142)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                }
                else if (client.Entity.Class == 132)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1120 }); //Fire Circle
                }
            }

            #endregion
            #region KungFu-Tro

            if (client.Entity.FirstRebornClass == 85 && client.Entity.SecondRebornClass == 15)
            {
                client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                client.AddSpell(new Spell(true) { ID = 1190 }); //SpiritHealing
                client.AddSpell(new Spell(true) { ID = 1110 }); //Cyclone
                client.AddSpell(new Spell(true) { ID = 1270 }); //Robot
            }

            #endregion
            #region KungFu-War

            if (client.Entity.FirstRebornClass == 85 && client.Entity.SecondRebornClass == 25)
            {
                if (client.Entity.Class == 41 || client.Entity.Class == 142)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1020 }); //Shield
                    client.AddSpell(new Spell(true) { ID = 1040 }); //Roar
                    client.AddSpell(new Spell(true) { ID = 3060 }); //Reflect
                }
                else if (client.Entity.Class == 11 || client.Entity.Class == 51 || client.Entity.Class == 61 ||
                         client.Entity.Class == 71 || client.Entity.Class == 81)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1020 }); //Shield
                    client.AddSpell(new Spell(true) { ID = 1040 }); //Roar
                    client.AddSpell(new Spell(true) { ID = 3060 }); //Reflect
                    client.AddSpell(new Spell(true) { ID = 1320 }); //Flying Moon (XP)
                    client.AddSpell(new Spell(true) { ID = 1015 }); //Accuracy
                }
                else if (client.Entity.Class == 21)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 3060 }); //Reflect
                }
                else if (client.Entity.Class == 132)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1020 }); //Shield
                    client.AddSpell(new Spell(true) { ID = 1040 }); //Roar
                    client.AddSpell(new Spell(true) { ID = 3060 }); //Reflect
                    client.AddSpell(new Spell(true) { ID = 1025 }); //Superman
                }
            }

            #endregion
            #region KungFu-Water

            if (client.Entity.FirstRebornClass == 85 && client.Entity.SecondRebornClass == 135)
            {
                if (client.Entity.Class == 11 || client.Entity.Class == 21 || client.Entity.Class == 41 ||
                    client.Entity.Class == 51 || client.Entity.Class == 61 || client.Entity.Class == 71 ||
                    client.Entity.Class == 81)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1020 }); //Shield
                    client.AddSpell(new Spell(true) { ID = 1040 }); //Roar
                    client.AddSpell(new Spell(true) { ID = 1280 }); //Water Elf
                    client.AddSpell(new Spell(true) { ID = 1195 }); //Meditation
                    client.AddSpell(new Spell(true) { ID = 1015 }); //Accuracy
                    client.AddSpell(new Spell(true) { ID = 1350 }); //DivineHare
                    client.AddSpell(new Spell(true) { ID = 1095 }); //Stigma
                }
                else if (client.Entity.Class == 132)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 3090 }); //Pervade
                }
                else if (client.Entity.Class == 142)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 1280 }); //Water Elf
                    client.AddSpell(new Spell(true) { ID = 1175 }); //dvanced Cure
                    client.AddSpell(new Spell(true) { ID = 1075 }); //Invisibility 
                    client.AddSpell(new Spell(true) { ID = 1055 }); //Healing Rain
                    client.AddSpell(new Spell(true) { ID = 1050 }); //XP Revive
                    client.AddSpell(new Spell(true) { ID = 1350 }); //Divine Hare
                }
            }

            #endregion
            #region KungFu-Nin

            if (client.Entity.FirstRebornClass == 85 && client.Entity.SecondRebornClass == 55)
            {
                if (client.Entity.Class == 51)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 6002 }); //PoisonStar
                }
                else if (client.Entity.Class == 11 || client.Entity.Class == 132 || client.Entity.Class == 21 ||
                         client.Entity.Class == 41 || client.Entity.Class == 51 || client.Entity.Class == 61 ||
                         client.Entity.Class == 71 || client.Entity.Class == 81)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 6001 }); //Toxic Fog
                }
            }

            #endregion
            #region KungFu-Monk

            if (client.Entity.FirstRebornClass == 85 && client.Entity.SecondRebornClass == 65)
            {
                if (client.Entity.Class == 11 || client.Entity.Class == 21 || client.Entity.Class == 41 ||
                    client.Entity.Class == 51 || client.Entity.Class == 132 || client.Entity.Class == 142 ||
                    client.Entity.Class == 71)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                }
                else if (client.Entity.Class == 61)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 10405 }); //soulshackle
                    client.AddSpell(new Spell(true) { ID = 10400 }); //Serenity
                }
                else if (client.Entity.Class == 71)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 10400 }); //Serenity
                }
            }

            #endregion
            #region KungFu-Pirate

            if (client.Entity.FirstRebornClass == 85 && client.Entity.SecondRebornClass == 75)
            {
                if (client.Entity.Class == 11 || client.Entity.Class == 21 || client.Entity.Class == 41 ||
                    client.Entity.Class == 51 || client.Entity.Class == 132 || client.Entity.Class == 142 ||
                    client.Entity.Class == 61 || client.Entity.Class == 71)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 11070 }); //Gale Bomb
                }
                else if (client.Entity.Class == 81)
                {
                    client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                    client.AddSpell(new Spell(true) { ID = 12280 }); //DragonRoar
                    client.AddSpell(new Spell(true) { ID = 12300 }); //DragonFury
                    client.AddSpell(new Spell(true) { ID = 12120 }); //SpeedKick
                    client.AddSpell(new Spell(true) { ID = 12130 }); //ViolentKick
                    client.AddSpell(new Spell(true) { ID = 12140 }); //StormKick
                    client.AddSpell(new Spell(true) { ID = 12160 }); //CrackingSwip
                    client.AddSpell(new Spell(true) { ID = 12170 }); //SplittingSwipe
                    client.AddSpell(new Spell(true) { ID = 12200 }); //DragonSwing
                    client.AddSpell(new Spell(true) { ID = 12240 }); //DragonPunch
                    client.AddSpell(new Spell(true) { ID = 12350 }); //DragonSlash
                    client.AddSpell(new Spell(true) { ID = 12270 }); //DragonFlow
                    client.AddSpell(new Spell(true) { ID = 12290 }); //DragonCyclone
                    client.AddSpell(new Spell(true) { ID = 12320 }); //AirKick
                    client.AddSpell(new Spell(true) { ID = 12330 }); //AirSweep
                    client.AddSpell(new Spell(true) { ID = 12340 }); //AirRaid
                }
            }

            #endregion
            #region KungFu-KungFu
            if (client.Entity.FirstRebornClass == 85 && client.Entity.SecondRebornClass == 85)
            {

                client.AddSpell(new Spell(true) { ID = 9876 }); //Bless
                client.AddSpell(new Spell(true) { ID = 12280 }); //DragonRoar
                client.AddSpell(new Spell(true) { ID = 12300 }); //DragonFury
                client.AddSpell(new Spell(true) { ID = 12120 }); //SpeedKick
                client.AddSpell(new Spell(true) { ID = 12130 }); //ViolentKick
                client.AddSpell(new Spell(true) { ID = 12140 }); //StormKick
                client.AddSpell(new Spell(true) { ID = 12160 }); //CrackingSwip
                client.AddSpell(new Spell(true) { ID = 12170 }); //SplittingSwipe
                client.AddSpell(new Spell(true) { ID = 12200 }); //DragonSwing
                client.AddSpell(new Spell(true) { ID = 12240 }); //DragonPunch
                client.AddSpell(new Spell(true) { ID = 12350 }); //DragonSlash
                client.AddSpell(new Spell(true) { ID = 12270 }); //DragonFlow
                client.AddSpell(new Spell(true) { ID = 12290 }); //DragonCyclone
                client.AddSpell(new Spell(true) { ID = 12320 }); //AirKick
                client.AddSpell(new Spell(true) { ID = 12330 }); //AirSweep
                client.AddSpell(new Spell(true) { ID = 12340 }); //AirRaid
            }

            #endregion
            #endregion
            #endregion

            #region Low level items

            for (byte i = 1; i < 9; i++)
            {
                if (i != 7)
                {
                    ConquerItem item = client.Equipment.TryGetItem(i);
                    if (item != null && item.ID != 0)
                    {
                        try
                        {
                            //client.UnloadItemStats(item, false);
                            Database.ConquerItemInformation cii =
                                new Conquer_Online_Server.Database.ConquerItemInformation(item.ID, item.Plus);
                            item.ID =
                                cii.LowestID(
                                    Network.PacketHandler.ItemMinLevel(Network.PacketHandler.ItemPosition(item.ID)));
                            item.Mode = Conquer_Online_Server.Game.Enums.ItemMode.Update;
                            item.Send(client);
                            client.LoadItemStats();
                            client.ReshareClan();
                            if (client.Team != null)
                                foreach (var teammate in client.Team.Teammates)
                                    teammate.ReshareClan();
                            Database.ConquerItemTable.UpdateItemID(item, client);
                        }
                        catch
                        {
                            Console.WriteLine("Reborn item problem: " + item.ID);
                        }
                    }
                }
            }
            ConquerItem hand = client.Equipment.TryGetItem(5);
            if (hand != null)
            {
                client.Equipment.Remove(5);
                client.CalculateStatBonus();
                client.CalculateHPBonus();
            }
            hand = client.Equipment.TryGetItem(25);
            if (hand != null)
            {
                client.Equipment.Remove(25);
                client.CalculateStatBonus();
                client.CalculateHPBonus();
            }
            client.LoadItemStats();
            client.ReshareClan();
            if (client.Team != null)
                foreach (var teammate in client.Team.Teammates)
                    teammate.ReshareClan();
            client.SendScreen(client.Entity.SpawnPacket, false);

            #endregion

            Database.DataHolder.GetStats(client.Entity.Class, client.Entity.Level, client);
            client.CalculateStatBonus();
            client.CalculateHPBonus();
            client.GemAlgorithm();
            client.SendStatMessage();
            //Database.ReincarnationTable.NewReincarnated(client.Entity);
            Network.PacketHandler.WorldMessage(client.Entity.Name + " has got Reincarnation! Congratulations!");
        }
    }

    #endregion

    public class Reincarnate
    {
        public Game.ConquerStructures.Inventory Inventory;
        public Game.ConquerStructures.Equipment Equipment;
        public Entity Entity;
        public byte Class;
        public byte First;
        public byte Second;
        private Interfaces.ISkill[] Skills;
        private Interfaces.IProf[] Profs;
        private Dictionary<ushort, Interfaces.ISkill> Learn = null;
        private Dictionary<ushort, Interfaces.ISkill> WontLearn = null;
        //Client.GameState[] States;

        public Reincarnate(Entity _Entity, byte _class)
        {
            Entity = _Entity;
            Class = _class;
            First = Entity.SecondRebornClass;
            Second = Entity.Class;
            Learn = new Dictionary<ushort, Interfaces.ISkill>();
            WontLearn = new Dictionary<ushort, Interfaces.ISkill>();
            Skills = new Interfaces.ISkill[Entity.Owner.Spells.Values.Count];
            Entity.Owner.Spells.Values.CopyTo(Skills, 0);
            Profs = new Interfaces.IProf[Entity.Owner.Proficiencies.Values.Count];
            Entity.Owner.Proficiencies.Values.CopyTo(Profs, 0);
            //States = new Client.GameState[Program.SafeReturn().Count];
            //Program.SafeReturn().Values.CopyTo(States, 0);
            doIt();
        }

        private void doIt()
        {
            #region Reincarnate

            Database.ReincarnationTable.NewReincarnated(Entity);
            Game.Features.Reincarnation.ReincarnateInfo info = new Game.Features.Reincarnation.ReincarnateInfo();
            info.UID = Entity.UID;
            info.Level = Entity.Level;
            info.Experience = Entity.Experience;
            Kernel.ReincarnatedCharacters.Add(info.UID, info);
            Entity.FirstRebornClass = First;
            Entity.SecondRebornClass = Second;
            Entity.Class = Class;
            Entity.Atributes = 182;
            Entity.Level = 15;
            Entity.Experience = 0;

            #endregion

            #region Low level items

            for (byte i = 1; i < 9; i++)
            {
                if (i != 7)
                {
                    ConquerItem item = Entity.Owner.Equipment.TryGetItem(i);
                    if (item != null && item.ID != 0)
                    {
                        try
                        {
                            Database.ConquerItemInformation cii =
                                new Conquer_Online_Server.Database.ConquerItemInformation(item.ID, item.Plus);
                            item.ID =
                                cii.LowestID(
                                    Network.PacketHandler.ItemMinLevel(Network.PacketHandler.ItemPosition(item.ID)));
                            item.Mode = Conquer_Online_Server.Game.Enums.ItemMode.Update;
                            item.Send(Entity.Owner);
                            Database.ConquerItemTable.UpdateItemID(item, Entity.Owner);
                        }
                        catch
                        {
                            Console.WriteLine("Reborn item problem: " + item.ID);
                        }
                    }
                }
            }
            Entity.Owner.LoadItemStats();
            ConquerItem hand = Entity.Owner.Equipment.TryGetItem(5);
            if (hand != null)
            {
                Entity.Owner.Equipment.Remove(5);
                CalculateStatBonus();
                CalculateHPBonus();
                Entity.Owner.Screen.Reload(null);
            }
            else
                Entity.Owner.SendScreen(Entity.Owner.Entity.SpawnPacket, false);

            #endregion

            foreach (Interfaces.ISkill s in Skills)
                Entity.Owner.Spells.Remove(s.ID);

            switch (First)
            {
                #region KungFuKing
                case 85:
                    {
                        switch (Second)
                        {
                            case 81: // ana zh2t we da5el anam lma tegy a3melha anta
                                //Add(Enums.SkillIDs.SpeedKick);
                                //Add(Enums.SkillIDs.ViolentKick);
                                //Add(Enums.SkillIDs.StormKick);
                                //Add(Enums.SkillIDs.CrackingSwip);
                                //Add(Enums.SkillIDs.SplittingSwipe);
                                //Add(Enums.SkillIDs.DragonSwing);
                                //Add(Enums.SkillIDs.DragonPunch);
                                //Add(Enums.SkillIDs.DragonSlash);
                                //Add(Enums.SkillIDs.DragonFlow);
                                Add(Enums.SkillIDs.DragonRoar);
                                //Add(Enums.SkillIDs.DragonCyclone);
                                //Add(Enums.SkillIDs.AirKick);
                                //Add(Enums.SkillIDs.AirSweep);
                                //Add(Enums.SkillIDs.AirRaid);
                                break;
                            default:
                                //WontAdd(Enums.SkillIDs.SpeedKick);
                                //WontAdd(Enums.SkillIDs.ViolentKick);
                                //WontAdd(Enums.SkillIDs.StormKick);
                                //WontAdd(Enums.SkillIDs.CrackingSwip);
                                //WontAdd(Enums.SkillIDs.SplittingSwipe);
                                //WontAdd(Enums.SkillIDs.DragonSwing);
                                //WontAdd(Enums.SkillIDs.DragonPunch);
                                //WontAdd(Enums.SkillIDs.DragonSlash);
                                //WontAdd(Enums.SkillIDs.DragonFlow);
                                ////WontAdd(Enums.SkillIDs.DragonRoar);
                                //WontAdd(Enums.SkillIDs.DragonCyclone);
                                //WontAdd(Enums.SkillIDs.AirKick);
                                //WontAdd(Enums.SkillIDs.AirSweep);
                                //WontAdd(Enums.SkillIDs.AirRaid);
                                break;
                        }
                        break;
                    }
                #endregion
                #region Trojan

                case 15:
                    {
                        switch (Second)
                        {
                            case 11:
                                Add(Enums.SkillIDs.CruelShade);
                                break;
                            default:
                                WontAdd(Enums.SkillIDs.Accuracy);
                                break;
                        }
                        break;
                    }

                #endregion

                #region Warrior

                case 25:
                    {
                        switch (Second)
                        {
                            case 11:
                                Add(Enums.SkillIDs.IronShirt);
                                WontAdd(Enums.SkillIDs.Shield);
                                WontAdd(Enums.SkillIDs.SuperMan);
                                break;
                            case 21:
                                Add(Enums.SkillIDs.Reflect);
                                break;
                            case 132:
                                WontAdd(Enums.SkillIDs.FlyingMoon);
                                WontAdd(Enums.SkillIDs.Shield);
                                break;
                            case 142:
                                WontAdd(Enums.SkillIDs.Accuracy);
                                WontAdd(Enums.SkillIDs.FlyingMoon);
                                WontAdd(Enums.SkillIDs.SuperMan);
                                break;
                            default:
                                WontAdd(Enums.SkillIDs.Accuracy);
                                WontAdd(Enums.SkillIDs.FlyingMoon);
                                WontAdd(Enums.SkillIDs.SuperMan);
                                WontAdd(Enums.SkillIDs.Shield);
                                break;
                        }
                        break;
                    }

                #endregion

                #region Archer

                case 45:
                    {
                        switch (Second)
                        {
                            case 41:
                                break;
                            default:
                                WontAdd(Enums.SkillIDs.Scatter);
                                WontAdd(Enums.SkillIDs.XPFly);
                                WontAdd(Enums.SkillIDs.AdvancedFly);
                                WontAdd(Enums.SkillIDs.ArrowRain);
                                WontAdd(Enums.SkillIDs.Intensify);
                                WontAdd(Enums.SkillIDs.RapidFire);
                                break;
                        }
                        break;
                    }

                #endregion

                #region Ninja

                case 55:
                    {
                        switch (Second)
                        {
                            case 51:
                                break;
                            default:
                                WontAdd(Enums.SkillIDs.PoisonStar);
                                WontAdd(Enums.SkillIDs.ShurikenVortex);
                                WontAdd(Enums.SkillIDs.FatalStrike);
                                WontAdd(Enums.SkillIDs.TwofoldBlades);
                                WontAdd(Enums.SkillIDs.ArcherBane);
                                WontAdd(Enums.SkillIDs.TwilightDance);
                                WontAdd(Enums.SkillIDs.SuperTwofoldBlade);
                                WontAdd(Enums.SkillIDs.FatalSpin);
                                break;
                        }
                        break;
                    }

                #endregion

                #region Monk

                case 65:
                    {
                        switch (Second)
                        {
                            case 61:
                                break;
                            default:
                                WontAdd(Enums.SkillIDs.Oblivion);
                                WontAdd(Enums.SkillIDs.RadiantPalm);
                                WontAdd(Enums.SkillIDs.TyrantAura);
                                WontAdd(Enums.SkillIDs.DeathBlow);
                                WontAdd(Enums.SkillIDs.DeflectionAura);
                                WontAdd(Enums.SkillIDs.TripleAttack);
                                break;
                        }
                        break;
                    }

                #endregion

                #region Water

                case 135:
                    {
                        switch (Second)
                        {
                            case 132:
                                Add(Enums.SkillIDs.Pervade);
                                break;
                            case 142:
                                WontAdd(Enums.SkillIDs.Nectar);
                                WontAdd(Enums.SkillIDs.HealingRain);
                                break;
                            default:
                                WontAdd(Enums.SkillIDs.Nectar);
                                WontAdd(Enums.SkillIDs.Lightning);
                                WontAdd(Enums.SkillIDs.Volcano);
                                WontAdd(Enums.SkillIDs.AdvancedCure);
                                WontAdd(Enums.SkillIDs.SpeedLightning);
                                WontAdd(Enums.SkillIDs.HealingRain);
                                break;
                        }
                        break;
                    }

                #endregion

                #region Fire

                case 145:
                    {
                        switch (Second)
                        {
                            case 142:
                                Add(Enums.SkillIDs.Dodge);
                                break;
                            default:
                                if (Second != 132)
                                    WontAdd(Enums.SkillIDs.FireCircle);

                                WontAdd(Enums.SkillIDs.Tornado);
                                WontAdd(Enums.SkillIDs.FireMeteor);
                                WontAdd(Enums.SkillIDs.FireOfHell);
                                WontAdd(Enums.SkillIDs.FireRing);
                                WontAdd(Enums.SkillIDs.Volcano);
                                WontAdd(Enums.SkillIDs.Lightning);
                                WontAdd(Enums.SkillIDs.SpeedLightning);
                                break;
                        }
                        break;
                    }

                #endregion
            }


            Add(Enums.SkillIDs.Bless);

            #region Re-Learn Profs

            foreach (Interfaces.IProf Prof in Profs)
            {
                if (Prof == null)
                    continue;

                Prof.Available = false;
                Prof.PreviousLevel = Prof.Level;
                Prof.Level = 0;
                Prof.Experience = 0;
                Entity.Owner.Proficiencies.Add(Prof.ID, Prof);
                Prof.Send(Entity.Owner);
            }

            #endregion

            #region Re-Learn Skills

            foreach (Interfaces.ISkill Skill in Skills)
            {
                if (Skill == null)
                    continue;

                Skill.Available = false;
                Skill.PreviousLevel = Skill.Level;
                Skill.Level = 0;
                Skill.Experience = 0;

                if (!WontLearn.ContainsKey(Skill.ID))
                {
                    Entity.Owner.Spells.Add(Skill.ID, Skill);
                    Skill.Send(Entity.Owner);
                }
            }

            #endregion

            #region Learn Skills

            foreach (Interfaces.ISkill L in Learn.Values)
            {
                if (L == null)
                    continue;

                L.Available = false;
                Entity.Owner.Spells.Add(L.ID, L);
                L.Send(Entity.Owner);
            }

            #endregion

            #region Remove Skills

            foreach (Interfaces.ISkill L in WontLearn.Values)
            {
                if (L == null)
                    continue;

                L.Available = false;
                Entity.Owner.Spells.Remove(L.ID);
                Database.SkillTable.DeleteSpell(Entity.Owner, L.ID);
                L.Send(Entity.Owner);
            }

            #endregion

            Database.DataHolder.GetStats(Entity.Class, Entity.Level, Entity.Owner);
            Entity.Owner.CalculateStatBonus();
            Entity.Owner.CalculateHPBonus();
            //Samak Database.SkillTable.SaveSpells(Entity.Owner);
            //Samak Database.SkillTable.SaveProficiencies(Entity.Owner);
            Kernel.SendWorldMessage(
                new Conquer_Online_Server.Network.GamePackets.Message(
                    "Congratulations, " + Entity.Name + " reincarnated!", System.Drawing.Color.White,
                    Network.GamePackets.Message.TopLeft), Program.GamePool);
        }

        private void Add(Enums.SkillIDs S)
        {
            Interfaces.ISkill New = new Network.GamePackets.Spell(true);
            New.ID = (ushort)S;
            New.Level = 0;
            New.Experience = 0;
            New.PreviousLevel = 0;
            Learn.Add(New.ID, New);
        }

        private void WontAdd(Enums.SkillIDs S)
        {
            Interfaces.ISkill New = new Network.GamePackets.Spell(true);
            New.ID = (ushort)S;
            New.Level = 0;
            New.Experience = 0;
            New.PreviousLevel = 0;
            WontLearn.Add(New.ID, New);
        }

        private void WontAdd2(Enums.SkillIDs S)
        {
            Interfaces.ISkill New = new Network.GamePackets.Spell(true);
            New.ID = (ushort)S;
            New.Level = 0;
            New.Experience = 0;
            New.PreviousLevel = 0;
            WontLearn.Add(New.ID, New);
        }

        private int StatHP;

        public void CalculateHPBonus()
        {
            switch (Entity.Class)
            {
                case 11:
                    Entity.MaxHitpoints = (uint)(StatHP * 1.05F);
                    break;
                case 12:
                    Entity.MaxHitpoints = (uint)(StatHP * 1.08F);
                    break;
                case 13:
                    Entity.MaxHitpoints = (uint)(StatHP * 1.10F);
                    break;
                case 14:
                    Entity.MaxHitpoints = (uint)(StatHP * 1.12F);
                    break;
                case 15:
                    Entity.MaxHitpoints = (uint)(StatHP * 1.15F);
                    break;
                default:
                    Entity.MaxHitpoints = (uint)StatHP;
                    break;
            }
            Entity.MaxHitpoints += Entity.ItemHP;
            Entity.Hitpoints = Math.Min(Entity.Hitpoints, Entity.MaxHitpoints);
        }

        public void CalculateStatBonus()
        {
            byte ManaBoost = 5;
            const byte HitpointBoost = 24;
            sbyte Class = (sbyte)(Entity.Class / 10);
            if (Class == 13 || Class == 14)
                ManaBoost += (byte)(5 * (Entity.Class - (Class * 10)));
            StatHP = (ushort)((Entity.Strength * 3) +
                                     (Entity.Agility * 3) +
                                     (Entity.Spirit * 3) +
                                     (Entity.Vitality * HitpointBoost));
            Entity.MaxMana = (ushort)((Entity.Spirit * ManaBoost) + Entity.ItemMP);
            Entity.Mana = Math.Min(Entity.Mana, Entity.MaxMana);
        }

        public void GemAlgorithm()
        {
            Entity.MaxAttack = Entity.Strength + Entity.BaseMaxAttack;
            Entity.MinAttack = Entity.Strength + Entity.BaseMinAttack;
            Entity.MagicAttack = Entity.ItemHP;
            Entity.MagicAttack = Entity.BaseMagicAttack;
            
            if (Entity.Gems[0] != 0)
            {
                Entity.MagicAttack += (uint)Math.Floor(Entity.MagicAttack * (double)(Entity.Gems[0] * 5));
            }
            if (Entity.Gems[1] != 0)
            {
                Entity.MaxAttack += (uint)Math.Floor(Entity.MaxAttack * (double)(Entity.Gems[1] * 5 ));
                Entity.MinAttack += (uint)Math.Floor(Entity.MinAttack * (double)(Entity.Gems[1] * 5));
               Entity.ItemHP += (uint)Math.Floor(Entity.ItemHP * (double)(Entity.Gems[1] * 5));
                
            }
        }

    }
}