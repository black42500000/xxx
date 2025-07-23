﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Game.ConquerStructures.Society
{
    public class TradePartner : Interfaces.IKnownPerson
    {
        public uint ID
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }
        public bool IsOnline
        {
            get
            {
                return Kernel.GamePool.ContainsKey(ID);
            }
        }
        public Client.GameClient Client
        {
            get
            {
                return Kernel.GamePool[ID];
            }
        }

        public bool StillOnProbation
        {
            get
            {
                return ProbationStartedOn.AddDays(3) >= DateTime.Now;
            }
        }
        public DateTime ProbationStartedOn
        {
            get;
            set;
        }
    }
}