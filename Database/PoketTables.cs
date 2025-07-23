using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Database
{
    public class PoketTables
    {
        public static void LoadTables()
        {
            string[] TDs = System.IO.File.ReadAllLines("PokerTables.txt");
            foreach (string Tinfo in TDs)
            {
                //T.Id + " " + T.BetType + " " + T.Map + " " + T.Nomber + " " + T.X + " " + T.Y;
                string[] DR = Tinfo.Split(' ');
                Game.PokerTable T = new Game.PokerTable();
                T.Id = Convert.ToUInt32(DR[0]);
                T.Nomber = (byte)Convert.ToInt16(DR[3]);
                T.BetType = (byte)Convert.ToUInt32(DR[1]);
                T.X = (ushort)Convert.ToUInt32(DR[4]);
                T.Y = (ushort)Convert.ToUInt32(DR[5]);
                bool FoundMinBet = false;
                try
                {
                    T.MinLimit = (ushort)Convert.ToUInt32(DR[6]); FoundMinBet = true;
                }
                catch { }
                T.Map = Convert.ToUInt32(DR[2]);
                if (!FoundMinBet)
                {
                    if (T.Nomber > 19)
                    {
                        T.MinLimit = 10000;
                        T.FreeBet = false;
                    }
                    if (T.Nomber > 25) T.MinLimit = 5000;
                    if (T.Nomber > 30) T.MinLimit = 2000;
                    if (T.Nomber > 35) T.MinLimit = 1000;
                }
                if (!Conquer_Online_Server.Kernel.PokerTables.ContainsKey(T.Id))
                    Conquer_Online_Server.Kernel.PokerTables.Add(T.Id, T);
            }
            Console.WriteLine("Loaded " + Conquer_Online_Server.Kernel.PokerTables.Count+" Poker Tables Loaded");
        }
    }
}
