using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer_Online_Server.Game.ConquerStructures.Society;
using Conquer_Online_Server.GamePackets;

namespace Conquer_Online_Server.Network.Test
{
    public class Advertise2
    {
        public static void Execute2(Client.GameClient client, byte[] packet)
        {
            try
            {
                Advertise_client[] _clientArray = Advertise.Top8.Values.ToArray<Advertise_client>();
                _clientArray = (from guil in _clientArray orderby guil.Rank ascending select guil).ToArray();
                uint all_advertise = (ushort)_clientArray.Length;

                uint Page = Conquer_Online_Server.BitConverter.ToUInt32(packet, 4);

                if (Page > all_advertise) return;
                ushort x_count = (ushort)(all_advertise - Page);

                ushort count = (ushort)Math.Min((int)x_count, 4);
                if (count <= 2)
                {
                    Advertise adv = new Advertise(count);
                    adv.AllRegistred = (ushort)_clientArray.Length;
                    adv.AtCount = (ushort)Page;
                    for (uint x = Page; x < Page + count; x++) // 3'alat ya 3am el index kda 3'alat
                    {
                        var element = _clientArray[x];
                        adv.Aprend(element);
                    }
                    client.Send(adv.ToArray());
                    // kda sa7 ya 7oyb ;) htshta3'al m3ak
                }
                else
                {
                    // na2es el 7eta de
                    ushort Other_count = (ushort)(count - 2);
                    ushort countx = (ushort)(count - Other_count);
                    Advertise adv = new Advertise(countx);
                    adv.AllRegistred = (ushort)_clientArray.Length;
                    adv.AtCount = (ushort)Page;
                    for (uint x = Page; x < Page + countx; x++)
                    {
                        var element = _clientArray[x];
                        adv.Aprend(element);
                    }
                    client.Send(adv.ToArray());

                    adv = new Advertise(Other_count);
                    adv.AllRegistred = (ushort)_clientArray.Length;
                    adv.AtCount = (ushort)Page;
                    for (uint x = countx; x < Page + Other_count; x++)
                    {
                        var element = _clientArray[x];
                        adv.Aprend(element);
                    }
                    client.Send(adv.ToArray());
                    // kda htshta3'al m3ak kois gedan 23mel test w 2oly
                }

            }
            catch (Exception e) { Console.WriteLine(e.ToString()); }
        }
        public static void Execute(Client.GameClient client, byte[] packet)
        {
            try
            {
                Advertise_client[] _clientArray = Advertise.Top8.Values.ToArray<Advertise_client>();
                _clientArray = (from guil in _clientArray orderby guil.Rank ascending select guil).ToArray();
                uint all_advertise = (ushort)_clientArray.Length;
                uint Receive_count = Conquer_Online_Server.BitConverter.ToUInt32(packet, 4);
                if (Receive_count > all_advertise) return;
                ushort x_count = (ushort)(all_advertise - Receive_count);
                ushort count = (ushort)Math.Min((int)x_count, 4);
                uint num = Math.Max(0, Receive_count);
                if (count <= 2)
                {
                    Advertise adv = new Advertise(count);
                    adv.AllRegistred = (ushort)_clientArray.Length;
                    adv.AtCount = (ushort)Receive_count;
                    adv.First = 1;
                    for (ushort x = 0; x < count; x++)
                    {
                        var element = _clientArray[x + num];
                        adv.Aprend(element);
                    }
                    client.Send(adv.ToArray());
                }
                else
                {
                    ushort Other_count = (ushort)(count - 2);
                    ushort countx = (ushort)(count - Other_count);

                    Advertise adv = new Advertise(countx);
                    adv.AllRegistred = (ushort)_clientArray.Length;
                    adv.AtCount = (ushort)Receive_count;
                    adv.First = 1;
                    for (ushort x = 0; x < countx; x++)
                    {
                        var element = _clientArray[x + num];
                        adv.Aprend(element);
                    }
                    client.Send(adv.ToArray());

                    adv = new Advertise(Other_count);
                    adv.AllRegistred = (ushort)_clientArray.Length;
                    adv.AtCount = (ushort)Receive_count;
                    for (ushort x = 0; x < Other_count; x++)
                    {
                        var element = _clientArray[x + countx + num];
                        adv.Aprend(element);
                    }
                    client.Send(adv.ToArray());
                    return;
                }

            }
            catch (Exception e) { Console.WriteLine(e.ToString()); }
        }
    }
}
