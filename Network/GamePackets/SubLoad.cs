using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Network.GamePackets
{
    class SubLoad
    {
        public static void SubClass(Client.GameClient client)
        {
            client.Entity.Block += 15 * 100;
            foreach (Conquer_Online_Server.Game.SubClass class3 in client.Entity.SubClasses.Classes.Values)
            {
                if (class3.ID == 1)
                {
                    if (class3.Phase == 1)
                    {
                        client.Entity.CriticalStrike += 100;
                    }
                    if (class3.Phase == 2)
                    {
                        client.Entity.CriticalStrike += 200;
                    }
                    if (class3.Phase == 3)
                    {
                        client.Entity.CriticalStrike += 300;
                    }
                    if (class3.Phase == 4)
                    {
                        client.Entity.CriticalStrike += 400;
                    }
                    if (class3.Phase == 5)
                    {
                        client.Entity.CriticalStrike += 600;
                    }
                    if (class3.Phase == 6)
                    {
                        client.Entity.CriticalStrike += 800;
                    }
                    if (class3.Phase == 7)
                    {
                        client.Entity.CriticalStrike += 0x3e8;
                    }
                    if (class3.Phase == 8)
                    {
                        client.Entity.CriticalStrike += 0x4b0;
                    }
                    if (class3.Phase == 9)
                    {
                        client.Entity.CriticalStrike += 0x5dc;
                    }
                }
                if (class3.ID == 2)
                {
                    if (class3.Phase == 1)
                    {
                        client.Entity.SkillCStrike = (ushort)(client.Entity.SkillCStrike + 100);
                    }
                    if (class3.Phase == 2)
                    {
                        client.Entity.SkillCStrike = (ushort)(client.Entity.SkillCStrike + 200);
                    }
                    if (class3.Phase == 3)
                    {
                        client.Entity.SkillCStrike = (ushort)(client.Entity.SkillCStrike + 300);
                    }
                    if (class3.Phase == 4)
                    {
                        client.Entity.SkillCStrike = (ushort)(client.Entity.SkillCStrike + 400);
                    }
                    if (class3.Phase == 5)
                    {
                        client.Entity.SkillCStrike = (ushort)(client.Entity.SkillCStrike + 600);
                    }
                    if (class3.Phase == 6)
                    {
                        client.Entity.SkillCStrike = (ushort)(client.Entity.SkillCStrike + 800);
                    }
                    if (class3.Phase == 7)
                    {
                        client.Entity.SkillCStrike = (ushort)(client.Entity.SkillCStrike + 0x3e8);
                    }
                    if (class3.Phase == 8)
                    {
                        client.Entity.SkillCStrike = (ushort)(client.Entity.SkillCStrike + 0x4b0);
                    }
                    if (class3.Phase == 9)
                    {
                        client.Entity.SkillCStrike = (ushort)(client.Entity.SkillCStrike + 0x5dc);
                    }
                }
                if (class3.ID == 3)
                {
                    if (class3.Phase == 1)
                    {
                        client.Entity.Immunity += 100;
                    }
                    if (class3.Phase == 2)
                    {
                        client.Entity.Immunity += 200;
                    }
                    if (class3.Phase == 3)
                    {
                        client.Entity.Immunity += 300;
                    }
                    if (class3.Phase == 4)
                    {
                        client.Entity.Immunity += 400;
                    }
                    if (class3.Phase == 5)
                    {
                        client.Entity.Immunity += 600;
                    }
                    if (class3.Phase == 6)
                    {
                        client.Entity.Immunity += 800;
                    }
                    if (class3.Phase == 7)
                    {
                        client.Entity.Immunity += 0x3e8;
                    }
                    if (class3.Phase == 8)
                    {
                        client.Entity.Immunity += 0x4b0;
                    }
                    if (class3.Phase == 9)
                    {
                        client.Entity.Immunity += 0x5dc;
                    }
                }
                if (class3.ID == 4)
                {
                    if (class3.Phase == 1)
                    {
                        client.Entity.Penetration = (ushort)(client.Entity.Penetration + 100);
                    }
                    if (class3.Phase == 2)
                    {
                        client.Entity.Penetration = (ushort)(client.Entity.Penetration + 200);
                    }
                    if (class3.Phase == 3)
                    {
                        client.Entity.Penetration = (ushort)(client.Entity.Penetration + 300);
                    }
                    if (class3.Phase == 4)
                    {
                        client.Entity.Penetration = (ushort)(client.Entity.Penetration + 400);
                    }
                    if (class3.Phase == 5)
                    {
                        client.Entity.Penetration = (ushort)(client.Entity.Penetration + 600);
                    }
                    if (class3.Phase == 6)
                    {
                        client.Entity.Penetration = (ushort)(client.Entity.Penetration + 800);
                    }
                    if (class3.Phase == 7)
                    {
                        client.Entity.Penetration = (ushort)(client.Entity.Penetration + 0x3e8);
                    }
                    if (class3.Phase == 8)
                    {
                        client.Entity.Penetration = (ushort)(client.Entity.Penetration + 0x4b0);
                    }
                    if (class3.Phase == 9)
                    {
                        client.Entity.Penetration = (ushort)(client.Entity.Penetration + 0x5dc);
                    }
                }
                if (class3.ID == 5)
                {
                    if (class3.Phase == 1)
                    {
                        client.Entity.Detoxication = (ushort)(client.Entity.Detoxication + 8);
                    }
                    if (class3.Phase == 2)
                    {
                        client.Entity.Detoxication = (ushort)(client.Entity.Detoxication + 0x10);
                    }
                    if (class3.Phase == 3)
                    {
                        client.Entity.Detoxication = (ushort)(client.Entity.Detoxication + 0x18);
                    }
                    if (class3.Phase == 4)
                    {
                        client.Entity.Detoxication = (ushort)(client.Entity.Detoxication + 0x20);
                    }
                    if (class3.Phase == 5)
                    {
                        client.Entity.Detoxication = (ushort)(client.Entity.Detoxication + 40);
                    }
                    if (class3.Phase == 6)
                    {
                        client.Entity.Detoxication = (ushort)(client.Entity.Detoxication + 0x30);
                    }
                    if (class3.Phase == 7)
                    {
                        client.Entity.Detoxication = (ushort)(client.Entity.Detoxication + 0x38);
                    }
                    if (class3.Phase == 8)
                    {
                        client.Entity.Detoxication = (ushort)(client.Entity.Detoxication + 0x40);
                    }
                    if (class3.Phase == 9)
                    {
                        client.Entity.Detoxication = (ushort)(client.Entity.Detoxication + 0x48);
                    }
                }
                if (class3.ID == 6)
                {
                    if (class3.Phase == 1)
                    {
                        client.Entity.BaseMaxAttack += 100;
                        client.Entity.BaseMinAttack += 100;
                        client.Entity.BaseMagicAttack += 100;
                    }
                    if (class3.Phase == 2)
                    {
                        client.Entity.BaseMaxAttack += 200;
                        client.Entity.BaseMinAttack += 200;
                        client.Entity.BaseMagicAttack += 200;
                    }
                    if (class3.Phase == 3)
                    {
                        client.Entity.BaseMaxAttack += 300;
                        client.Entity.BaseMinAttack += 300;
                        client.Entity.BaseMagicAttack += 300;
                    }
                    if (class3.Phase == 4)
                    {
                        client.Entity.BaseMaxAttack += 400;
                        client.Entity.BaseMinAttack += 400;
                        client.Entity.BaseMagicAttack += 400;
                    }
                    if (class3.Phase == 5)
                    {
                        client.Entity.BaseMaxAttack += 500;
                        client.Entity.BaseMinAttack += 500;
                        client.Entity.BaseMagicAttack += 500;
                    }
                    if (class3.Phase == 6)
                    {
                        client.Entity.BaseMaxAttack += 600;
                        client.Entity.BaseMinAttack += 600;
                        client.Entity.BaseMagicAttack += 600;
                    }
                    if (class3.Phase == 7)
                    {
                        client.Entity.BaseMaxAttack += 700;
                        client.Entity.BaseMinAttack += 700;
                        client.Entity.BaseMagicAttack += 700;
                    }
                    if (class3.Phase == 8)
                    {
                        client.Entity.BaseMaxAttack += 800;
                        client.Entity.BaseMinAttack += 800;
                        client.Entity.BaseMagicAttack += 800;
                    }
                    if (class3.Phase == 9)
                    {
                        client.Entity.BaseMaxAttack += 0x3e8;
                        client.Entity.BaseMinAttack += 0x3e8;
                        client.Entity.BaseMagicAttack += 0x3e8;
                    }
                }
                if (class3.ID == 9)
                {
                    if (class3.Phase == 1)
                    {
                        client.Entity.ItemHP = (ushort)(client.Entity.ItemHP + 100);
                    }
                    if (class3.Phase == 2)
                    {
                        client.Entity.ItemHP = (ushort)(client.Entity.ItemHP + 200);
                    }
                    if (class3.Phase == 3)
                    {
                        client.Entity.ItemHP = (ushort)(client.Entity.ItemHP + 300);
                    }
                    if (class3.Phase == 4)
                    {
                        client.Entity.ItemHP = (ushort)(client.Entity.ItemHP + 400);
                    }
                    if (class3.Phase == 5)
                    {
                        client.Entity.ItemHP = (ushort)(client.Entity.ItemHP + 500);
                    }
                    if (class3.Phase == 6)
                    {
                        client.Entity.ItemHP = (ushort)(client.Entity.ItemHP + 600);
                    }
                    if (class3.Phase == 7)
                    {
                        client.Entity.ItemHP = (ushort)(client.Entity.ItemHP + 800);
                    }
                    if (class3.Phase == 8)
                    {
                        client.Entity.ItemHP = (ushort)(client.Entity.ItemHP + 0x3e8);
                    }
                    if (class3.Phase == 9)
                    {
                        client.Entity.ItemHP = (ushort)(client.Entity.ItemHP + 0x4b0);
                    }
                }
            }
        }
    }
}
