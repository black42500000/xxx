using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer_Online_Server;
using Conquer_Online_Server.Network.GamePackets;

namespace Conquer_Online_Server.Game.Attacking
{
    public class GemEffect
    {
        public static void Effect(Entity client)
        {


            for (uint i = 1; i < 12; i++)
            {
                if (i != ConquerItem.Bottle)
                {
                    ConquerItem item = client.Owner.Equipment.TryGetItem(i);
                    if (item != null && item.ID != 0)
                    {
                        if (item.SocketOne != 0)
                        {
                            switch (item.SocketOne)
                            {
                                #region VioletGem Check
                                case Enums.Gem.SuperVioletGem:
                                    {
                                        if (Kernel.Rate(5, 300))
                                        {
                                            _String str = new _String(true);
                                            str.UID = client.UID;
                                            str.TextsCount = 1;
                                            str.Type = _String.Effect;
                                            str.Texts.Add("purpleray");
                                            client.Owner.SendScreen(str, true);
                                            goto jump;
                                        }
                                        break;
                                    }
                                #endregion
                                #region PhoenixGem Check
                                case Enums.Gem.SuperPhoenixGem:
                                    {
                                        if (Kernel.Rate(5, 300))
                                        {
                                            _String str = new _String(true);
                                            str.UID = client.UID;
                                            str.TextsCount = 1;
                                            str.Type = _String.Effect;
                                            str.Texts.Add("phoenix");
                                            client.Owner.SendScreen(str, true);
                                            goto jump;
                                        }
                                        break;
                                    }
                                #endregion
                                #region DragonGem Check
                                case Enums.Gem.SuperDragonGem:
                                    {
                                        if (Kernel.Rate(5, 300))
                                        {
                                            _String str = new _String(true);
                                            str.UID = client.UID;
                                            str.TextsCount = 1;
                                            str.Type = _String.Effect;
                                            str.Texts.Add("goldendragon");
                                            client.Owner.SendScreen(str, true);
                                            goto jump;
                                        }
                                        break;
                                    }
                                #endregion
                                #region RainbowGem Check
                                case Enums.Gem.SuperRainbowGem:
                                    {
                                        if (Kernel.Rate(5, 300))
                                        {
                                            _String str = new _String(true);
                                            str.UID = client.UID;
                                            str.TextsCount = 1;
                                            str.Type = _String.Effect;
                                            str.Texts.Add("rainbow");
                                            client.Owner.SendScreen(str, true);
                                            goto jump;
                                        }
                                        break;
                                    }
                                #endregion
                                #region MoonGem Check
                                case Enums.Gem.SuperMoonGem:
                                    {
                                        if (Kernel.Rate(5, 300))
                                        {
                                            _String str = new _String(true);
                                            str.UID = client.UID;
                                            str.TextsCount = 1;
                                            str.Type = _String.Effect;
                                            str.Texts.Add("moon");
                                            client.Owner.SendScreen(str, true);
                                            goto jump;
                                        }
                                        break;
                                    }
                                #endregion
                                #region FuryGem Check
                                case Enums.Gem.SuperFuryGem:
                                    {
                                        if (Kernel.Rate(5, 300))
                                        {
                                            _String str = new _String(true);
                                            str.UID = client.UID;
                                            str.TextsCount = 1;
                                            str.Type = _String.Effect;
                                            str.Texts.Add("fastflash");
                                            client.Owner.SendScreen(str, true);
                                            goto jump;
                                        }
                                        break;
                                    }
                                #endregion
                                #region KylinGem Check
                                case Enums.Gem.SuperKylinGem:
                                    {
                                        if (Kernel.Rate(5, 300))
                                        {
                                            _String str = new _String(true);
                                            str.UID = client.UID;
                                            str.TextsCount = 1;
                                            str.Type = _String.Effect;
                                            str.Texts.Add("goldenkylin");
                                            client.Owner.SendScreen(str, true);
                                            goto jump;
                                        }
                                        break;
                                    }
                                #endregion
                                #region TortoiseGem Check
                                case Enums.Gem.SuperTortoiseGem:
                                    {
                                        if (Kernel.Rate(5, 400))
                                        {
                                            _String str = new _String(true);
                                            str.UID = client.UID;
                                            str.TextsCount = 1;
                                            str.Type = _String.Effect;
                                            str.Texts.Add("recovery");
                                            client.Owner.SendScreen(str, true);
                                            goto jump;
                                        }
                                        break;
                                    }
                                #endregion
                            }
                        }
                        if (item.SocketTwo != 0)
                        {
                            switch (item.SocketTwo)
                            {

                                #region VioletGem Check
                                case Enums.Gem.SuperVioletGem:
                                    {
                                        if (Kernel.Rate(5, 200))
                                        {
                                            _String str = new _String(true);
                                            str.UID = client.UID;
                                            str.TextsCount = 1;
                                            str.Type = _String.Effect;
                                            str.Texts.Add("purpleray");
                                            client.Owner.SendScreen(str, true);
                                            goto jump;
                                        }
                                        break;
                                    }
                                #endregion
                                #region PhoenixGem Check
                                case Enums.Gem.SuperPhoenixGem:
                                    {
                                        if (Kernel.Rate(5, 200))
                                        {
                                            _String str = new _String(true);
                                            str.UID = client.UID;
                                            str.TextsCount = 1;
                                            str.Type = _String.Effect;
                                            str.Texts.Add("phoenix");
                                            client.Owner.SendScreen(str, true);
                                            goto jump;
                                        }
                                        break;
                                    }
                                #endregion
                                #region DragonGem Check
                                case Enums.Gem.SuperDragonGem:
                                    {
                                        if (Kernel.Rate(5, 200))
                                        {
                                            _String str = new _String(true);
                                            str.UID = client.UID;
                                            str.TextsCount = 1;
                                            str.Type = _String.Effect;
                                            str.Texts.Add("goldendragon");
                                            client.Owner.SendScreen(str, true);
                                            goto jump;
                                        }
                                        break;
                                    }
                                #endregion
                                #region RainbowGem Check
                                case Enums.Gem.SuperRainbowGem:
                                    {
                                        if (Kernel.Rate(5, 200))
                                        {
                                            _String str = new _String(true);
                                            str.UID = client.UID;
                                            str.TextsCount = 1;
                                            str.Type = _String.Effect;
                                            str.Texts.Add("rainbow");
                                            client.Owner.SendScreen(str, true);
                                            goto jump;
                                        }
                                        break;
                                    }
                                #endregion
                                #region MoonGem Check
                                case Enums.Gem.SuperMoonGem:
                                    {
                                        if (Kernel.Rate(5, 200))
                                        {
                                            _String str = new _String(true);
                                            str.UID = client.UID;
                                            str.TextsCount = 1;
                                            str.Type = _String.Effect;
                                            str.Texts.Add("moon");
                                            client.Owner.SendScreen(str, true);
                                            goto jump;
                                        }
                                        break;
                                    }
                                #endregion
                                #region FuryGem Check
                                case Enums.Gem.SuperFuryGem:
                                    {
                                        if (Kernel.Rate(5, 200))
                                        {
                                            _String str = new _String(true);
                                            str.UID = client.UID;
                                            str.TextsCount = 1;
                                            str.Type = _String.Effect;
                                            str.Texts.Add("fastflash");
                                            client.Owner.SendScreen(str, true);
                                            goto jump;
                                        }
                                        break;
                                    }
                                #endregion
                                #region KylinGem Check
                                case Enums.Gem.SuperKylinGem:
                                    {
                                        if (Kernel.Rate(5, 200))
                                        {
                                            _String str = new _String(true);
                                            str.UID = client.UID;
                                            str.TextsCount = 1;
                                            str.Type = _String.Effect;
                                            str.Texts.Add("goldenkylin");
                                            client.Owner.SendScreen(str, true);
                                            goto jump;
                                        }
                                        break;
                                    }
                                #endregion
                                #region TortoiseGem Check
                                case Enums.Gem.SuperTortoiseGem:
                                    {
                                        if (Kernel.Rate(5, 300))
                                        {
                                            _String str = new _String(true);
                                            str.UID = client.UID;
                                            str.TextsCount = 1;
                                            str.Type = _String.Effect;
                                            str.Texts.Add("recovery");
                                            client.Owner.SendScreen(str, true);
                                            goto jump;
                                        }
                                        break;
                                    }
                                #endregion
                            }
                        }
                    }
                }
            }
        jump:
            return;
        }
    }
}