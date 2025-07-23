using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace Conquer_Online_Server.Game
{
   public class Quarantine
    {
        public static ConcurrentDictionary<uint, Client.GameClient> White =
            new ConcurrentDictionary<uint, Client.GameClient>();
        public static ConcurrentDictionary<uint, Client.GameClient> Black =
            new ConcurrentDictionary<uint, Client.GameClient>();

        public static int BlackScore, WhiteScore = 0;
        public static bool Started = false;
        public static ushort Map = 1844;
        public static ushort X = 119;
        public static ushort Y = 159;


    }
}
