using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer_Online_Server.Database;

namespace Conquer_Online_Server.Game.Tournaments
{
    public enum Tournamet_Types
    {
        LeveL100,
        LeveL119,
        LeveL129,
        LeveL130
    }
    public class Team_client
    {
        public uint Points = 0;
        public uint UID = 0;
        public ushort Avatar = 0;
        public ushort Mesh = 0;
        public string Name = "";
        public ushort Postion = 0;
        public byte MyTitle = 0;

        public Team_client(Client.GameClient client)
        {
            this.UID = client.Entity.UID;
            this.Avatar = client.Entity.Face;
            this.Mesh = client.Entity.Body;
            this.Name = client.Entity.Name;

        }
        public Team_client(uint _uid, ushort _avatar, ushort _mesh, string _name, uint _points, ushort Position, byte Tytle)
        {
            this.MyTitle = Tytle;
            this.Postion = Position;
            this.Points = _points;
            this.UID = _uid;
            this.Avatar = _avatar;
            this.Mesh = _mesh;
            this.Name = _name;
        }
    }
    public class TeamTournament
    {
        public static Dictionary<uint, Team_client> Team_PK_Tournament = new Dictionary<uint, Team_client>(500);
        public static Dictionary<uint, Team_client> Top8 = new Dictionary<uint, Team_client>(10);

        public static void DeleteTabelInstances()
        {
            foreach (Team_client client in Top8.Values)
            {
                MySqlCommand cmd = new MySqlCommand(MySqlCommandType.DELETE);
                cmd.Delete("teampk", "UID", client.UID).Execute();
            }
        }
        public static void LoadTop8()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT);
            cmd.Select("teampk");
            MySqlReader r = new MySqlReader(cmd);
            while (r.Read())
            {
                Team_client client = new Team_client(r.ReadUInt32("UID"), r.ReadUInt16("Avatar"), r.ReadUInt16("Mesh"), r.ReadString("Name"), r.ReadUInt32("Points"), r.ReadUInt16("Postion"), r.ReadByte("MyTitle"));
                if (!Top8.ContainsKey(client.UID))
                    Top8.Add(client.UID, client);
            }
            r.Close();
            r.Dispose();
        }
        public static void SaveTop8(Client.GameClient client)
        {
            //Conquer.Database.Elitepk.Insert((int)client.Entity.UID, (int)client.Entity.Face, (string)client.Entity.Name, (int)client.Entity.Mesh, (int)client.Entity.Points, (int)client.Entity.Postion, (long)client.Entity.MyTitle);
            Team_client clients = new Team_client(
                (uint)client.Entity.UID
                , (ushort)client.Entity.Face
                , (ushort)client.Entity.Body
                , (string)client.Entity.Name
                , (uint)client.Entity.Points
                , (ushort)Program.TeamRank
                , (byte)client.Entity.MyTitle
                    );
            if (!Top8.ContainsKey(clients.UID))
                Top8.Add(clients.UID, clients);
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.INSERT);
            cmd.Insert("teampk")
                .Insert("UID", clients.UID).Insert("Avatar", clients.Avatar)
                .Insert("Mesh", clients.Mesh).Insert("Name", clients.Name)
                .Insert("Points", clients.Points).Insert("Postion", Program.TeamRank)
          .Insert("MyTitle", clients.MyTitle);
            cmd.Execute();
            // Conquer.Database.Elitepk.Insert((int)clients.UID, (int)clients.Avatar, (string)clients.Name, (int)clients.Mesh, (int)clients.Points, (int)Program.EliteRank, (long)clients.MyTitle);
        }
        //public EliteTournament() { LoadTop8(); }
        public static void Open()
        {
            if (!Start)
            {
                DeleteTabelInstances();
                Start = true;
                //CalculateTime = DateTime.Now;
                //StartTimer = DateTime.Now;
                //SendInvitation();
                Team_PK_Tournament.Clear();
                Top8.Clear();
            }
        }
        public static void TeamPkHema()
        {
            if (DateTime.Now.Minute == DateTime.Now.Minute)
            {
                if (DateTime.Now.Minute == DateTime.Now.Minute)
                {
                    DeleteTabelInstances();
                    Team_PK_Tournament.Clear();
                    Top8.Clear();
                }
            }
        }
        public void Open(int hour, int minute)
        {
            if (DateTime.Now.Minute == minute && DateTime.Now.Hour == hour)
            {
                if (!Start)
                {
                    DeleteTabelInstances();
                    Start = true;
                    //CalculateTime = DateTime.Now;
                    //StartTimer = DateTime.Now;
                    //SendInvitation();
                    Team_PK_Tournament.Clear();
                    Top8.Clear();
                }
            }
        }
        /*public static void SendInvitation()
        {
            Client.GameClient[] client = Conquer_Online_Server.ServerBase.Kernel.GamePool.Values.ToArray();
            foreach (Client.GameClientclientss in client)
            {
                Network.GamePackets.NpcReply npc = new Network.GamePackets.NpcReply(6, "The Elite Tournament has Started! You Wana Join?");
                npc.OptionID = 249;
                clientss.Send(npc.ToArray());
            }
        }*/
        //private static DateTime CalculateTime;

        public static void ObtinedReward(Client.GameClient client)
        {
            if (Kernel.GamePool.ContainsKey(client.Entity.UID))
            {
                switch (Program.TeamRank)
                {
                    case 1:
                        {
                            //client.Entity.MyTitle = (byte)top_typ.Team_PK_Champion_High_;
                            client.Entity.ConquerPoints += 1000000;
                            SaveTop8(client);
                            Kernel.SendWorldMessage(new Network.GamePackets.Message("Congratulations, " + client.Entity.Name + " has Won TeamPk Tourment and Get [1st] rank and 250k cps!", System.Drawing.Color.White, Network.GamePackets.Message.Center), Program.GamePool);
                            break;
                        }
                    case 2:
                        {
                            //client.Entity.MyTitle = (byte)top_typ.Elite_PK_2nd_Place_High_;
                            client.Entity.ConquerPoints += 1000000 - 300000;
                            SaveTop8(client);
                            Kernel.SendWorldMessage(new Network.GamePackets.Message("Congratulations, " + client.Entity.Name + " has got [2nd] rank in TeamPk Tourment and " + (250000 - 30000) + " cps!", System.Drawing.Color.White, Network.GamePackets.Message.Center), Program.GamePool);
                            break;
                        }
                    case 3:
                        {
                            // client.Entity.MyTitle = (byte)top_typ.Elite_PK_3rd_Place__High_;
                            client.Entity.ConquerPoints += 1000000 - 500000;
                            SaveTop8(client);
                            Kernel.SendWorldMessage(new Network.GamePackets.Message("Congratulations, " + client.Entity.Name + " has got [3rd] rank in TeamPk Tourment and " + (250000 - 50000) + " cps!", System.Drawing.Color.White, Network.GamePackets.Message.Center), Program.GamePool);
                            break;
                        }
                    default:
                        {
                            //client.Entity.MyTitle = (byte)top_typ.Elite_PK_Top_8_High_;
                            client.Entity.ConquerPoints += 1000000 - 700000;
                            SaveTop8(client);
                            Kernel.SendWorldMessage(new Network.GamePackets.Message("Congratulations, " + client.Entity.Name + " has got [" + Program.TeamRank + "Th] rank in TeamPk Tourment and " + (250000 - 70000) + " cps!", System.Drawing.Color.White, Network.GamePackets.Message.World), Program.GamePool);
                            break;
                        }

                }//TQ
                //Console.WriteLine("a7a7");
            }
        }
        public static DateTime StartTimer;
        public static bool Start = false;
        // private static ushort Mapid = 7777;

        public static void AddMap(Client.GameClient client)
        {

            //client.elitepoints = 0;
            client.Entity.Teleport(7778, 150, 162);
        }

    }
    public class Skill_client
    {
        public uint Points = 0;
        public uint UID = 0;
        public ushort Avatar = 0;
        public ushort Mesh = 0;
        public string Name = "";
        public ushort Postion = 0;
        public byte MyTitle = 0;

        public Skill_client(Client.GameClient client)
        {
            this.UID = client.Entity.UID;
            this.Avatar = client.Entity.Face;
            this.Mesh = client.Entity.Body;
            this.Name = client.Entity.Name;

        }
        public Skill_client(uint _uid, ushort _avatar, ushort _mesh, string _name, uint _points, ushort Position, byte Tytle)
        {
            this.MyTitle = Tytle;
            this.Postion = Position;
            this.Points = _points;
            this.UID = _uid;
            this.Avatar = _avatar;
            this.Mesh = _mesh;
            this.Name = _name;
        }
    }
    public class SkillTournament
    {
        public static Dictionary<uint, Skill_client> Skill_PK_Tournament = new Dictionary<uint, Skill_client>(500);
        public static Dictionary<uint, Skill_client> Top8 = new Dictionary<uint, Skill_client>(10);
        public static void DeleteTabelInstances()
        {
            foreach (Skill_client client in Top8.Values)
            {
                MySqlCommand cmd = new MySqlCommand(MySqlCommandType.DELETE);
                cmd.Delete("skillpk", "UID", client.UID).Execute();
            }
        }
        public static void LoadTop8()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT);
            cmd.Select("skillpk");
            MySqlReader r = new MySqlReader(cmd);
            while (r.Read())
            {
                Skill_client client = new Skill_client(r.ReadUInt32("UID"), r.ReadUInt16("Avatar"), r.ReadUInt16("Mesh"), r.ReadString("Name"), r.ReadUInt32("Points"), r.ReadUInt16("Postion"), r.ReadByte("MyTitle"));
                if (!Top8.ContainsKey(client.UID))
                    Top8.Add(client.UID, client);
            }
            r.Close();
            r.Dispose();
        }
        public static void SaveTop8(Client.GameClient client)
        {
            //Conquer.Database.Elitepk.Insert((int)client.Entity.UID, (int)client.Entity.Face, (string)client.Entity.Name, (int)client.Entity.Mesh, (int)client.Entity.Points, (int)client.Entity.Postion, (long)client.Entity.MyTitle);
            Skill_client clients = new Skill_client(
                (uint)client.Entity.UID
                , (ushort)client.Entity.Face
                , (ushort)client.Entity.Body
                , (string)client.Entity.Name
                , (uint)client.Entity.Points
                , (ushort)Program.SkillPkRank
                , (byte)client.Entity.MyTitle
                    );
            if (!Top8.ContainsKey(clients.UID))
                Top8.Add(clients.UID, clients);
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.INSERT);
            cmd.Insert("skillpk")
                .Insert("UID", clients.UID).Insert("Avatar", clients.Avatar)
                .Insert("Mesh", clients.Mesh).Insert("Name", clients.Name)
                .Insert("Points", clients.Points).Insert("Postion", Program.SkillPkRank)
          .Insert("MyTitle", clients.MyTitle);
            cmd.Execute();
            // Conquer.Database.Elitepk.Insert((int)clients.UID, (int)clients.Avatar, (string)clients.Name, (int)clients.Mesh, (int)clients.Points, (int)Program.EliteRank, (long)clients.MyTitle);
        }
        //public EliteTournament() { LoadTop8(); }
        public static void Open()
        {
            if (!Start)
            {
                DeleteTabelInstances();
                Start = true;
                //CalculateTime = DateTime.Now;
                //StartTimer = DateTime.Now;
                //SendInvitation();
                Skill_PK_Tournament.Clear();
                Top8.Clear();
            }
        }
        public static void SkillPkHema()
        {
            if (DateTime.Now.Minute == DateTime.Now.Minute)
            {
                if (DateTime.Now.Minute == DateTime.Now.Minute)
                {
                    DeleteTabelInstances();
                    Skill_PK_Tournament.Clear();
                    Top8.Clear();
                }
            }
        }
        public void Open(int hour, int minute)
        {
            if (DateTime.Now.Minute == minute && DateTime.Now.Hour == hour)
            {
                if (!Start)
                {
                    DeleteTabelInstances();
                    Start = true;
                    //CalculateTime = DateTime.Now;
                    //StartTimer = DateTime.Now;
                    //SendInvitation();
                    Skill_PK_Tournament.Clear();
                    Top8.Clear();
                }
            }
        }
        /*public static void SendInvitation()
        {
            Client.GameClient[] client = Conquer_Online_Server.ServerBase.Kernel.GamePool.Values.ToArray();
            foreach (Client.GameClientclientss in client)
            {
                Network.GamePackets.NpcReply npc = new Network.GamePackets.NpcReply(6, "The Elite Tournament has Started! You Wana Join?");
                npc.OptionID = 249;
                clientss.Send(npc.ToArray());
            }
        }*/
        //private static DateTime CalculateTime;

        public static void ObtinedReward(Client.GameClient client)
        {
            if (Kernel.GamePool.ContainsKey(client.Entity.UID))
            {
                switch (Program.SkillPkRank)
                {
                    case 1:
                        {
                            //client.Entity.MyTitle = (byte)top_typ.Elite_PK_Champion_High_;
                            client.Entity.ConquerPoints += 1000000;
                            Kernel.SendWorldMessage(new Network.GamePackets.Message("Congratulations, " + client.Entity.Name + " has Won SkillPk Tourment and Get [1st] rank and 250k cps!", System.Drawing.Color.White, Network.GamePackets.Message.Center), Program.GamePool);
                            SaveTop8(client);
                            break;
                        }
                    case 2:
                        {
                            //client.Entity.MyTitle = (byte)top_typ.Elite_PK_2nd_Place_High_;
                            client.Entity.ConquerPoints += 1000000 - 400000;
                            SaveTop8(client);
                            Kernel.SendWorldMessage(new Network.GamePackets.Message("Congratulations, " + client.Entity.Name + " has got [2nd] rank in SkillPk Tourment and 210k cps!", System.Drawing.Color.White, Network.GamePackets.Message.Center), Program.GamePool);
                            break;
                        }
                    case 3:
                        {
                            //client.Entity.MyTitle = (byte)top_typ.Elite_PK_3rd_Place__High_;
                            client.Entity.ConquerPoints += 1000000 - 500000;
                            SaveTop8(client);
                            Kernel.SendWorldMessage(new Network.GamePackets.Message("Congratulations, " + client.Entity.Name + " has got [3rd] rank in SkillPk Tourment and 200k cps!", System.Drawing.Color.White, Network.GamePackets.Message.Center), Program.GamePool);
                            break;
                        }
                    default:
                        {
                            //client.Entity.MyTitle = (byte)top_typ.Elite_PK_Top_8_High_;
                            client.Entity.ConquerPoints += 1000000 - 700000;
                            SaveTop8(client);
                            Kernel.SendWorldMessage(new Network.GamePackets.Message("Congratulations, " + client.Entity.Name + " has got [" + Program.SkillPkRank + "Th] rank in SkillPk Tourment and 5kk cps!", System.Drawing.Color.White, Network.GamePackets.Message.World), Program.GamePool);
                            break;
                        }

                }
                //Console.WriteLine("a7a7");
            }
        }
        public static DateTime StartTimer;
        public static bool Start = false;
        // private static ushort Mapid = 7779;

        public static void AddMap(Client.GameClient client)
        {

            //client.elitepoints = 0;
            client.Entity.Teleport(7779, 150, 162);
        }

    }
}
