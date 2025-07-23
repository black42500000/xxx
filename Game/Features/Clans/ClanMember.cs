using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server
{
    public class ClanMember
    {
        public UInt32 Identifier, Donation;
        public String Name , LeaderName;
        public Clan.Ranks Rank;
        public Byte Level, Class;
        public uint UID = 0;
        public bool IsOnline
        {
            get
            {
                return Kernel.GamePool.ContainsKey(UID);
            }
        }
        public Client.GameClient Client
        {
            get
            {
                return Kernel.GamePool[UID];
            }
        }
    }
}
