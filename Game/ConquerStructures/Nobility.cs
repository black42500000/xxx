using System;
using System.Linq;
using System.Collections.Generic;
using Conquer_Online_Server.Network.GamePackets;
using System.IO;
using KinSocket;

namespace Conquer_Online_Server.Game.ConquerStructures
{
    public class Nobility
    {
        public static ulong MaxDonation = 0;
        public static ulong MaxDonation1 = 0;
        public static ulong MaxDonation2 = 0;
        public static Conquer_Online_Server.Network.GamePackets.nobility.NobilityIcon.NobilityRanks GetFemale(Conquer_Online_Server.Network.GamePackets.nobility.NobilityIcon.NobilityRanks rank)
        {
            switch (rank)
            {
                case Conquer_Online_Server.Network.GamePackets.nobility.NobilityIcon.NobilityRanks.Knight:
                    return Conquer_Online_Server.Network.GamePackets.nobility.NobilityIcon.NobilityRanks.Lady;

                case Conquer_Online_Server.Network.GamePackets.nobility.NobilityIcon.NobilityRanks.Lady:
                case Conquer_Online_Server.Network.GamePackets.nobility.NobilityIcon.NobilityRanks.Baroness:
                case Conquer_Online_Server.Network.GamePackets.nobility.NobilityIcon.NobilityRanks.Countess:
                case Conquer_Online_Server.Network.GamePackets.nobility.NobilityIcon.NobilityRanks.Dutchess:
                    return rank;

                case Conquer_Online_Server.Network.GamePackets.nobility.NobilityIcon.NobilityRanks.Baron:
                    return Conquer_Online_Server.Network.GamePackets.nobility.NobilityIcon.NobilityRanks.Baroness;

                case Conquer_Online_Server.Network.GamePackets.nobility.NobilityIcon.NobilityRanks.Earl:
                    return Conquer_Online_Server.Network.GamePackets.nobility.NobilityIcon.NobilityRanks.Countess;

                case Conquer_Online_Server.Network.GamePackets.nobility.NobilityIcon.NobilityRanks.Duke:
                    return Conquer_Online_Server.Network.GamePackets.nobility.NobilityIcon.NobilityRanks.Dutchess;

                case Conquer_Online_Server.Network.GamePackets.nobility.NobilityIcon.NobilityRanks.Prince:
                    return Conquer_Online_Server.Network.GamePackets.nobility.NobilityIcon.NobilityRanks.Princess;

                case Conquer_Online_Server.Network.GamePackets.nobility.NobilityIcon.NobilityRanks.King:
                    return Conquer_Online_Server.Network.GamePackets.nobility.NobilityIcon.NobilityRanks.Queen;
            }
            return rank;
        }

        public static ulong GetMinimumDonation(Conquer_Online_Server.Network.GamePackets.nobility.NobilityIcon.NobilityRanks Rank)
        {
            switch (Rank)
            {
                case Conquer_Online_Server.Network.GamePackets.nobility.NobilityIcon.NobilityRanks.Knight:
                case Conquer_Online_Server.Network.GamePackets.nobility.NobilityIcon.NobilityRanks.Lady:
                    return 0x1c9c380L;

                case Conquer_Online_Server.Network.GamePackets.nobility.NobilityIcon.NobilityRanks.Baron:
                case Conquer_Online_Server.Network.GamePackets.nobility.NobilityIcon.NobilityRanks.Baroness:
                    return 0x5f5e100L;

                case Conquer_Online_Server.Network.GamePackets.nobility.NobilityIcon.NobilityRanks.Earl:
                case Conquer_Online_Server.Network.GamePackets.nobility.NobilityIcon.NobilityRanks.Countess:
                    return 0xbebc200L;
            }
            return MinimumDonationFor(Rank);
        }

        public static SafeDictionary<uint, NobilityInformation> Board = new SafeDictionary<uint, NobilityInformation>(10000);
        public static List<NobilityInformation> BoardList = new List<NobilityInformation>(10000);
        public static ulong MinimumDonationFor(Conquer_Online_Server.Network.GamePackets.nobility.NobilityIcon.NobilityRanks Rank)
        {
            ulong num = 0L;
            for (int c = 0; c < BoardList.Count; c++)
            {


                sbyte place = 0;
                Conquer_Online_Server.Network.GamePackets.nobility.NobilityIcon.NobilityRanks commoner = Conquer_Online_Server.Network.GamePackets.nobility.NobilityIcon.NobilityRanks.Commoner;
                uint identifier = BoardList[c].EntityUID;
                ulong donation = BoardList[c].Donation;
                GetNobilityStats(identifier, donation, ref place, ref commoner);
                if ((commoner == Rank) && ((donation < num) || (num == 0L)))
                {
                    num = donation;
                }

            }
            return (num + ((ulong)1L));
        }
        public static void GetNobilityStats(uint Identifier, ulong Donation, ref sbyte Place, ref Conquer_Online_Server.Network.GamePackets.nobility.NobilityIcon.NobilityRanks Rank)
        {
            sbyte place = 0;
            Conquer_Online_Server.Network.GamePackets.nobility.NobilityIcon.NobilityRanks commoner = Conquer_Online_Server.Network.GamePackets.nobility.NobilityIcon.NobilityRanks.Commoner;
            for (int c = 0; c < 50; c++)
            {

                if (BoardList[c].EntityUID == Identifier)
                {
                    break;
                }
                if (place < 50)
                {
                    place = (sbyte)(place + 1);
                }
            }

            Place = (place < 50) ? place : ((sbyte)(-1));
            if (Donation == 0L)
            {
                Place = -1;
            }
            commoner = GetRanking(Donation, place);
            Rank = commoner;
        }
        public static Conquer_Online_Server.Network.GamePackets.nobility.NobilityIcon.NobilityRanks GetRanking(ulong Donation, sbyte Place)
        {
            if ((Donation != 0L) && (Place >= 0))
            {
                if ((Place < Database.rates.king) && (Donation > 0L))
                {
                    return Conquer_Online_Server.Network.GamePackets.nobility.NobilityIcon.NobilityRanks.King;
                }
                if ((Place < 0x12) && (Donation > 0L))
                {
                    return Conquer_Online_Server.Network.GamePackets.nobility.NobilityIcon.NobilityRanks.Prince;
                }
                if ((Place < 0x35) && (Donation > 0L))
                {
                    return Conquer_Online_Server.Network.GamePackets.nobility.NobilityIcon.NobilityRanks.Duke;
                }
                if (Donation >= 0xbebc200L)
                {
                    return Conquer_Online_Server.Network.GamePackets.nobility.NobilityIcon.NobilityRanks.Earl;
                }
                if (Donation >= 0x5f5e100L)
                {
                    return Conquer_Online_Server.Network.GamePackets.nobility.NobilityIcon.NobilityRanks.Baron;
                }
                if (Donation >= 0x1c9c380L)
                {
                    return Conquer_Online_Server.Network.GamePackets.nobility.NobilityIcon.NobilityRanks.Knight;
                }
            }
            return Conquer_Online_Server.Network.GamePackets.nobility.NobilityIcon.NobilityRanks.Commoner;
        }
        public static void Handle(NobilityInfo information, Client.GameClient client)
        {
            switch (information.Type)
            {

                case NobilityInfo.Donate:
                    {

                        if (client.Trade.InTrade)
                            return;
                        if (client.Entity.MapID == 3031)
                            return;
                        uint silvers = information.dwParam;
                        bool newDonator = false;
                        if (client.NobilityInformation.Donation == 0)
                            newDonator = true;
                        if (client.Entity.Money < information.dwParam)
                        {
                            ulong cps = silvers / 20000;
                            if (client.Entity.ConquerPoints >= cps)
                            {
                                if (client.Entity.NobalityDonation == client.NobilityInformation.Donation)
                                {
                                    client.Entity.ConquerPoints -= (uint)cps;
                                    Database.EntityTable.UpdateCps(client);
                                    client.NobilityInformation.Donation += silvers;
                                    client.Entity.NobalityDonation += silvers;                                   
                                }
                                else
                                {
                                    if (client.Entity.NobalityDonation > client.NobilityInformation.Donation)
                                    {
                                        client.Entity.ConquerPoints -= (uint)cps;
                                        Database.EntityTable.UpdateCps(client);
                                        client.Entity.NobalityDonation += silvers;
                                        client.NobilityInformation.Donation = client.Entity.NobalityDonation;                                        
                                    }
                                    else
                                    {
                                        if (client.Entity.NobalityDonation < client.NobilityInformation.Donation)
                                        {
                                            client.Entity.ConquerPoints -= (uint)cps;
                                            Database.EntityTable.UpdateCps(client);
                                            client.NobilityInformation.Donation += silvers;
                                            client.Entity.NobalityDonation = client.NobilityInformation.Donation;                                            
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (client.Entity.Money >= silvers)
                            {
                                if (client.Entity.NobalityDonation == client.NobilityInformation.Donation)
                                {
                                    client.Entity.Money -= silvers;
                                    client.NobilityInformation.Donation += silvers;
                                    client.Entity.NobalityDonation += silvers;                                    
                                }
                                else
                                {
                                    if (client.Entity.NobalityDonation > client.NobilityInformation.Donation)
                                    {
                                        client.Entity.Money -= silvers;
                                        client.Entity.NobalityDonation += silvers;
                                        client.NobilityInformation.Donation = client.Entity.NobalityDonation;                                        
                                    }
                                    else
                                    {
                                        if (client.Entity.NobalityDonation < client.NobilityInformation.Donation)
                                        {
                                            client.Entity.Money -= silvers;
                                            client.NobilityInformation.Donation += silvers;
                                            client.Entity.NobalityDonation = client.NobilityInformation.Donation;                                           
                                        }
                                    }
                                }
                            }
                        }

                        if (!Board.ContainsKey(client.Entity.UID) && newDonator)
                        {
                            Board.Add(client.Entity.UID, client.NobilityInformation);
                            try
                            {
                                Database.NobilityTable.InsertNobilityInformation(client.NobilityInformation);
                            }
                            catch
                            {
                                Database.NobilityTable.UpdateNobilityInformation(client.NobilityInformation);
                            }
                        }
                        else
                        {
                            Database.NobilityTable.UpdateNobilityInformation(client.NobilityInformation);
                        }
                        Sort(client.Entity.UID);

                        Network.GamePackets.NpcReply npc = new Network.GamePackets.NpcReply(6, "You've donated " + silvers + " Silver. Your total donation is " + client.NobilityInformation.Donation + " silver. You rank at " + client.NobilityInformation.Position + " place in the Donation Ranking.");
                        npc.OptionID = 255;
                        client.Send(npc.ToArray());
                        //return;

                        break;
                    }
                #region List
                case NobilityInfo.List:
                    {
                        bool Again = false;
                        byte Count = 0;
                        MemoryStream strm = new MemoryStream();
                        BinaryWriter wtr = new BinaryWriter(strm);
                        wtr.Write((ushort)0);
                        wtr.Write((ushort)2064);
                        wtr.Write((uint)NobilityInfo.List);
                        wtr.Write((ushort)information.wParam1);
                        wtr.Write((ushort)5);
                        wtr.Write((uint)0);
                        wtr.Write((uint)0);
                        wtr.Write((uint)0);
                        for (int i = (int)(information.wParam1 * 10); i < information.wParam1 * 10 + 10; i++)
                        {
                            if (BoardList.Count > i)
                            {
                                Count++;
                                wtr.Write((uint)BoardList[i].EntityUID);
                                wtr.Write((uint)BoardList[i].Gender);
                                wtr.Write((uint)BoardList[i].Mesh);
                                for (int c = 0; c < 20; c++)
                                {
                                    if (BoardList[i].Name.Length > c)
                                    {
                                        wtr.Write((byte)(BoardList[i].Name[c]));
                                    }
                                    else
                                    {
                                        wtr.Write((byte)(0));
                                    }
                                }
                                wtr.Write((ulong)BoardList[i].Donation);
                                wtr.Write((uint)BoardList[i].Rank);
                                wtr.Write((uint)BoardList[i].Position);
                                if (i == 1 && Again == false || i == 11 && Again == false || i == 21 && Again == false || i == 31 && Again == false || i == 41 && Again == false)
                                {
                                    i -= 2;
                                    Count = 0;
                                    Again = true;
                                }
                            }
                        }
                        Again = false;
                        int packetlength = (int)strm.Length;
                        strm.Position = 0;
                        wtr.Write((ushort)packetlength);
                        strm.Position = strm.Length;
                        wtr.Write(System.Text.Encoding.UTF7.GetBytes("TQServer"));
                        strm.Position = 0;
                        byte[] buf = new byte[strm.Length];
                        strm.Read(buf, 0, buf.Length);
                        Network.Writer.WriteUInt32(Count, 12, buf);
                        client.Send(buf);
                        break;
                    }
                case NobilityInfo.NextRank:
                    {
                        try
                        {
                            uint[] offset = { (uint)(30000000 - client.NobilityInformation.Donation), (uint)(100000000 - client.NobilityInformation.Donation), 0, (uint)(200000000 - client.NobilityInformation.Donation), 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                            MemoryStream strm = new MemoryStream();
                            BinaryWriter wtr = new BinaryWriter(strm);
                            wtr.Write((ushort)0);
                            wtr.Write((ushort)2064);
                            wtr.Write((uint)4);
                            wtr.Write((uint)offset[(byte)client.NobilityInformation.Rank]);
                            wtr.Write((uint)0);
                            for (byte Rank = (byte)client.NobilityInformation.Rank; Rank < 12; Rank++)
                            {
                                if (Rank == 2 || Rank == 4)
                                {
                                    continue;
                                }
                                wtr.Write((uint)0);
                                wtr.Write((uint)Rank);
                                wtr.Write((uint)offset[(byte)Rank]);
                                wtr.Write((uint)0);
                            }
                            int packetlength = (int)strm.Length;
                            strm.Position = 0;
                            wtr.Write((ushort)packetlength);
                            strm.Position = strm.Length;
                            wtr.Write(System.Text.Encoding.UTF7.GetBytes("TQServer"));
                            strm.Position = 0;
                            byte[] buf = new byte[strm.Length];
                            strm.Read(buf, 0, buf.Length);
                            client.Send(buf);
                            wtr.Close();
                            strm.Close();
                        }
                        catch (Exception e)
                        {
                            Program.WriteLine(e); Program.SaveException(e);
                        }
                        break;
                    }
                #endregion

            }
        }
        public static void Donate(ulong silvers, Client.GameClient client)
        {
            bool newDonator = false;
            client.NobilityInformation.Gender = (byte)(client.Entity.Body % 10);
            if (client.NobilityInformation.Donation == 0)
                newDonator = true;

            client.NobilityInformation.Donation += silvers;

            if (!Board.ContainsKey(client.Entity.UID) && client.NobilityInformation.Donation == silvers && newDonator)
            {
                Board.Add(client.Entity.UID, client.NobilityInformation);
                try
                {
                    Database.NobilityTable.InsertNobilityInformation(client.NobilityInformation);
                }
                catch
                {
                    Database.NobilityTable.UpdateNobilityInformation(client.NobilityInformation);
                }
            }
            else
            {
                Database.NobilityTable.UpdateNobilityInformation(client.NobilityInformation);
            }
            Sort(client.Entity.UID);
        }
        public static void Sort(uint updateUID)
        {
            SortedDictionary<ulong, SortEntry<uint, NobilityInformation>> sortdict = new SortedDictionary<ulong, SortEntry<uint, NobilityInformation>>();

            foreach (NobilityInformation info in Board.Values)
            {
                if (sortdict.ContainsKey(info.Donation))
                {
                    SortEntry<uint, NobilityInformation> entry = sortdict[info.Donation];
                    entry.Values.Add(info.EntityUID, info);
                }
                else
                {
                    SortEntry<uint, NobilityInformation> entry = new SortEntry<uint, NobilityInformation>();
                    entry.Values = new Dictionary<uint, NobilityInformation>();
                    entry.Values.Add(info.EntityUID, info);
                    sortdict.Add(info.Donation, entry);
                }
            }

            SafeDictionary<uint, NobilityInformation> sortedBoard = new SafeDictionary<uint, NobilityInformation>(1000000);
            sortedBoard.Clear();
            int Place = 0;
            foreach (KeyValuePair<ulong, SortEntry<uint, NobilityInformation>> entries in sortdict.Reverse())
            {
                foreach (KeyValuePair<uint, NobilityInformation> value in entries.Value.Values)
                {
                    Client.GameClient client = null;
                    try
                    {
                        int previousPlace = value.Value.Position;
                        value.Value.Position = Place;
                        NobilityRank Rank = NobilityRank.Serf;

                        if (Place > 100)
                        {
                            if (value.Value.Donation >= 200000000)
                            {
                                Rank = NobilityRank.Earl;
                            }
                            else if (value.Value.Donation >= 100000000)
                            {
                                Rank = NobilityRank.Baron;
                            }
                            else if (value.Value.Donation >= 30000000)
                            {
                                Rank = NobilityRank.Knight;
                            }
                        }
                        else if (Place < Database.rates.king)
                        {
                            Rank = NobilityRank.King;
                            if (Place < (Database.rates.king - (Database.rates.king - 1)))
                            {
                                MaxDonation = value.Value.Donation;
                            }
                        }
                        else if (Place < Database.rates.prince)
                        {
                            Rank = NobilityRank.Prince;
                            if (Place < (Database.rates.king + 2))
                            {
                                MaxDonation1 = value.Value.Donation;
                            }
                        }
                        else
                        {

                            Rank = NobilityRank.Duke;
                            if (Place < (Database.rates.prince + 2))
                            {
                                MaxDonation2 = value.Value.Donation;
                            }
                        }
                        var oldRank = value.Value.Rank;
                        value.Value.Rank = Rank;
                        if (Kernel.GamePool.TryGetValue(value.Key, out client))
                        {
                            bool updateTheClient = false;
                            if (oldRank != Rank)
                            {
                                updateTheClient = true;
                                if (Rank == NobilityRank.Baron)
                                {
                                    Kernel.SendWorldMessage(new Message("Congratulation! " + client.Entity.Name + " Donation To Baron in Nobility Rank.", System.Drawing.Color.White, Message.TopLeft), Program.GamePool);
                                }
                                if (Rank == NobilityRank.Earl)
                                {
                                    Kernel.SendWorldMessage(new Message("Congratulation! " + client.Entity.Name + " Donation To Earl in Nobility Rank.", System.Drawing.Color.White, Message.TopLeft), Program.GamePool);
                                }
                                if (Rank == NobilityRank.Duke)
                                {
                                    Kernel.SendWorldMessage(new Message("Congratulation! " + client.Entity.Name + " Donation To Duke in Nobility Rank.", System.Drawing.Color.White, Message.Center), Program.GamePool);
                                }
                                if (Rank == NobilityRank.Prince)
                                {
                                    Kernel.SendWorldMessage(new Message("Congratulation! " + client.Entity.Name + " Donation To Prince in Nobility Rank.", System.Drawing.Color.White, Message.Center), Program.GamePool);
                                }
                                if (Rank == NobilityRank.King)
                                {
                                    Kernel.SendWorldMessage(new Message("Congratulation! " + client.Entity.Name + " has become the new King/Queen in " + Database.rates.servername + "!", System.Drawing.Color.White, Message.Center), Program.GamePool);
                                }
                                if (Rank == NobilityRank.Knight)
                                {
                                    Kernel.SendWorldMessage(new Message("Congratulation! " + client.Entity.Name + " Donation To Knight in Nobility Rank.", System.Drawing.Color.White, Message.TopLeft), Program.GamePool);
                                }
                            }
                            else
                            {
                                if (previousPlace != Place)
                                {
                                    updateTheClient = true;
                                }
                            }
                            if (updateTheClient || client.Entity.UID == updateUID)
                            {
                                NobilityInfo update = new NobilityInfo(true);
                                update.Type = NobilityInfo.Icon;
                                update.dwParam = value.Key;
                                update.UpdateString(value.Value);
                                client.SendScreen(update, true);
                                client.Entity.NobilityRank = value.Value.Rank;
                            }
                        }
                        sortedBoard.Add(value.Key, value.Value);
                        Place++;
                    }
                    catch { }
                }

            }

            Board = sortedBoard;

            lock (BoardList)
            {
                BoardList = Board.Values.ToList();
            }

        }
    }
    public class NobilityInformation
    {
        public string Name;
        public uint EntityUID;
        public uint Mesh;
        public ulong Donation;
        public byte Gender;
        public int Position;
        public bool rank1;
        public bool rank2;
        public bool rank3;
        public NobilityRank Rank;
    }

    public enum NobilityRank : byte
    {
        Serf = 0,
        Knight = 1,
        Baron = 3,
        Earl = 5,
        Duke = 7,
        Prince = 9,
        King = 12
    }
}
