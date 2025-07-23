/*using System.Collections.Generic;
using System.Linq;

namespace Conquer_Online_Server.Game
{
    public class GuildAdvertise
    {
        public static SafeDictionary<uint, Database.GuildAd> GuildsAds = new SafeDictionary<uint, Database.GuildAd>();
        public static List<Database.GuildAd> GuildsList = new List<Database.GuildAd>(0x3e8);

        public static void Sort(uint uint_0)
        {
            SortedDictionary<ulong, GClass186<uint, Database.GuildAd>> source = new SortedDictionary<ulong, GClass186<uint, Database.GuildAd>>();
            foreach (Database.GuildAd class2 in GuildsAds.Values)
            {
                GClass186<uint, Database.GuildAd> class3;
                if (source.ContainsKey(class2.AdvertsMoney))
                {
                    class3 = source[class2.AdvertsMoney];
                    class3.dictionary_0.Add(class2.GuildID, class2);
                }
                else
                {
                    class3 = new GClass186<uint, Database.GuildAd>
                    {
                        dictionary_0 = new Dictionary<uint, Database.GuildAd>()
                    };
                    class3.dictionary_0.Add(class2.GuildID, class2);
                    source.Add(class2.AdvertsMoney, class3);
                }
            }
            SafeDictionary<uint, Database.GuildAd> class4 = new SafeDictionary<uint, Database.GuildAd>(1000);
            class4.Clear();
            int num = 0;
            foreach (KeyValuePair<ulong, GClass186<uint, Database.GuildAd>> pair in source.Reverse<KeyValuePair<ulong, GClass186<uint, Database.GuildAd>>>())
            {
                foreach (KeyValuePair<uint, Database.GuildAd> pair2 in pair.Value.dictionary_0)
                {
                    try
                    {
                        int num2 = pair2.Value.int_0;
                        pair2.Value.int_0 = num;
                        int num3 = pair2.Value.int_1;
                        pair2.Value.int_1 = num2;
                        class4.Add(pair2.Key, pair2.Value);
                        num++;
                    }
                    catch
                    {
                    }
                }
            }
            GuildsAds = class4;
            lock (GuildsList)
            {
                GuildsList.Clear();
                GuildsList = null;
                GuildsList = GuildsAds.Values.ToList<Database.GuildAd>();
            }
            source.Clear();
            source = null;
        }
    }

    public sealed class GClass186<T, U>
    {
        public Dictionary<T, U> dictionary_0;
    }
}
*/