﻿using Conquer_Online_Server.Database;
using System;
using Conquer_Online_Server.Game;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class SobNpcSpawn : Writer, Interfaces.IPacket, Interfaces.INpc, Interfaces.ISobNpc, Interfaces.IMapObject
    {
        private byte[] Buffer;

        public SobNpcSpawn()
        {
            Buffer = new byte[90];
            WriteUInt16(82, 0, Buffer);
            WriteUInt16(1109, 2, Buffer);
            ShowName = false;
        }

        public uint UID
        {
            get { return BitConverter.ToUInt32(Buffer, 4); }
            set { WriteUInt32(value, 4, Buffer); }
        }

        public uint MaxHitpoints
        {
            get { return BitConverter.ToUInt32(Buffer, 12); }
            set { WriteUInt32(value, 12, Buffer); }
        }

        public uint Hitpoints
        {
            get { return BitConverter.ToUInt32(Buffer, 16); }
            set
            {
                WriteUInt32(value, 16, Buffer);
            }
        }

        public ushort X
        {
            get { return BitConverter.ToUInt16(Buffer, 20); }
            set { WriteUInt16(value, 20, Buffer); }
        }

        public ushort Y
        {
            get { return BitConverter.ToUInt16(Buffer, 22); }
            set { WriteUInt16(value, 22, Buffer); }
        }

        public ushort Mesh
        {
            get { return BitConverter.ToUInt16(Buffer, 24); }
            set { WriteUInt16(value, 24, Buffer); }
        }

        public Conquer_Online_Server.Game.Enums.NpcType Type
        {
            get { return (Conquer_Online_Server.Game.Enums.NpcType)Buffer[26]; }
            set { Buffer[26] = (byte)value; }
        }

        public ushort Sort
        {
            get { return BitConverter.ToUInt16(Buffer, 28); }
            set { WriteUInt16(value, 28, Buffer); }
        }

        public bool SpawnOnMinutes
        {
            get;
            set;
        }
        public byte BeforeHour
        {
            get;
            set;
        }

        public bool ShowName
        {
            get { return Buffer[36] == 1; }
            set { Buffer[36] = value == true ? (byte)1 : (byte)0; }
        }

        public string LoweredName;
        private string _Name;
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
                LoweredName = value.ToLower();
                byte[] buffer = new byte[90];
                Buffer.CopyTo(buffer, 0);
                WriteUInt16((ushort)(buffer.Length - 8), 0, buffer);
                WriteStringWithLength(value, 37, buffer);
                Buffer = buffer;
            }
        }

        public ushort MapID { get; set; }

        public Conquer_Online_Server.Game.MapObjectType MapObjType
        {
            get
            {
                if (MaxHitpoints == 0)
                    return Conquer_Online_Server.Game.MapObjectType.Npc;
                return Conquer_Online_Server.Game.MapObjectType.SobNpc;
            }
        }

        public void Die(Game.Entity killer)
        {
            if (MapID == 2075)//island
            {
                if (UID != 815)
                {
                    if (Hitpoints != 0 || Mesh != 252 && Mesh != 282)
                    {
                        if (Mesh == 241)
                            Mesh = (ushort)(250 + Mesh % 10);
                        else
                            Mesh = (ushort)(280 + Mesh % 10);

                        Update upd = new Update(true);
                        upd.UID = UID;
                        upd.Append(Update.Mesh, Mesh);
                        killer.Owner.SendScreen(upd, true);
                        Hitpoints = 0;
                    }
                    Attack attack = new Attack(true);
                    attack.Attacker = killer.UID;
                    attack.Attacked = UID;
                    attack.AttackType = Network.GamePackets.Attack.Kill;
                    attack.X = X;
                    attack.Y = Y;
                    killer.Owner.Send(attack);
                    killer.KOCount++;
                }
            }
            else
            {
                Attack attack = new Attack(true);
                attack.Attacker = killer.UID;
                attack.Attacked = UID;
                attack.AttackType = Network.GamePackets.Attack.Kill;
                attack.X = X;
                attack.Y = Y;
                killer.Owner.Send(attack);
                Hitpoints = MaxHitpoints;
                Update upd = new Update(true);
                upd.UID = UID;
                upd.Append(Update.Hitpoints, MaxHitpoints);
                killer.Owner.SendScreen(upd, true);
            }  
            if (MapID == 2072)
            {
                if (UID != 818)
                {
                    if (Hitpoints != 0 || Mesh != 251 && Mesh != 281)
                    {
                        if (Mesh == 241)
                            Mesh = (ushort)(250 + Mesh % 10);
                        else
                            Mesh = (ushort)(280 + Mesh % 10);

                        Update upd = new Update(true);
                        upd.UID = UID;
                        upd.Append(Update.Mesh, Mesh);
                        killer.Owner.SendScreen(upd, true);
                        Hitpoints = 0;
                    }
                    Attack attack = new Attack(true);
                    attack.Attacker = killer.UID;
                    attack.Attacked = UID;
                    attack.AttackType = Network.GamePackets.Attack.Kill;
                    attack.X = X;
                    attack.Y = Y;
                    killer.Owner.Send(attack);
                    killer.KOCount++;
                }
            }
            else
            {
                Attack attack = new Attack(true);
                attack.Attacker = killer.UID;
                attack.Attacked = UID;
                attack.AttackType = Network.GamePackets.Attack.Kill;
                attack.X = X;
                attack.Y = Y;
                killer.Owner.Send(attack);
                Hitpoints = MaxHitpoints;
                Update upd = new Update(true);
                upd.UID = UID;
                upd.Append(Update.Hitpoints, MaxHitpoints);
                killer.Owner.SendScreen(upd, true);
            }  
            if (UID == 123456)
            {
                Hitpoints = 0;
                Program.World.PoleDomination.KillPole();
                return;
            }
            if (MapID == CaptureTheFlag.MapID)
            {
                if (Program.World.CTF.Bases.ContainsKey(UID))
                {
                    var _base = Program.World.CTF.Bases[UID];
                    _base.Capture();
                }
            }
            else if (MapID == 1038)
            {
                if (UID != 810)
                {
                    if (Hitpoints != 0 || Mesh != 251 && Mesh != 281)
                    {
                        if (Mesh == 241)
                            Mesh = (ushort)(250 + Mesh % 10);
                        else
                            Mesh = (ushort)(280 + Mesh % 10);

                        Update upd = new Update(true);
                        upd.UID = UID;
                        upd.Append(Update.Mesh, Mesh);
                        killer.Owner.SendScreen(upd, true);
                        Hitpoints = 0;
                    }
                    Attack attack = new Attack(true);
                    attack.Attacker = killer.UID;
                    attack.Attacked = UID;
                    attack.AttackType = Network.GamePackets.Attack.Kill;
                    attack.X = X;
                    attack.Y = Y;
                    killer.Owner.Send(attack);
                    killer.KOCount++;
                }
            }
            else
            {
                Attack attack = new Attack(true);
                attack.Attacker = killer.UID;
                attack.Attacked = UID;
                attack.AttackType = Network.GamePackets.Attack.Kill;
                attack.X = X;
                attack.Y = Y;
                killer.Owner.Send(attack);
                Hitpoints = MaxHitpoints;
                Update upd = new Update(true);
                upd.UID = UID;
                upd.Append(Update.Hitpoints, MaxHitpoints);
                killer.Owner.SendScreen(upd, true);
            }
        }
        private Client.GameClient owner_null = null;
        public bool _isprize;
        public Client.GameClient Owner
        {
            get
            {
                return owner_null;
            }
            set
            {
                owner_null = value;
            }
        }
        public void SendSpawn(Client.GameClient client, bool checkScreen)
        {
            if (client.Screen.Add(this) || !checkScreen)
            {
                client.Send(Buffer);
            }
        }
        public void SendSpawn(Client.GameClient client)
        {
            SendSpawn(client, false);
        }

        public byte[] ToArray()
        {
            return Buffer;
        }
        public void Deserialize(byte[] buffer)
        {
            Buffer = buffer;
        }
        public void Send(Client.GameClient client)
        {
            SendSpawn(client, false);
        }
    }
}
