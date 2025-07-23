using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Conquer_Online_Server.Network.GamePackets
{
    public class GuildCommand : Writer, Interfaces.IPacket
    {
        public const uint
        JoinRequest = 1,
        InviteRequest = 2,
        Quit = 3,
        Info = 6,
        Allied = 7,
        Neutral1 = 8,
        Enemied = 9,
        Neutral2 = 10,
        DonateSilvers = 11,
        Refresh = 12,
        Disband = 19,
        DonateConquerPoints = 20,
        SetRequirement = 24,
        Bulletin = 27,
        Discharge = 30,
        SendRequest = 28,
        AcceptRequest = 29,
        Promote = 37;
        private byte[] Buffer;
        byte buff = 25;
        public GuildCommand(bool Create)
        {
            if (Create)
            {
                Buffer = new byte[8 + 28];
                WriteUInt16(28, 0, Buffer);
                WriteUInt16(1107, 2, Buffer);
            }
        }
        public List<string> Positions = new List<string>();
        public void Set()
        {
            byte[] Saved = Buffer;
            int len = 0;
            foreach (var Pos in Positions)
                len += Pos.Length;

            this.Buffer = new byte[28 + 16 + 8 + len];
            if (Saved != null) Saved.CopyTo(Buffer, 0);
            WriteUInt16((ushort)(Buffer.Length - 8), 0, Buffer);
            WriteUInt16(1107, 2, Buffer);
            Buffer[24] = (byte)(Positions.Count);
            byte start = 25;
            foreach (var Pos in Positions)
            {
                Buffer[start] = (byte)Pos.Length;
                WriteString(Pos, (start + 1), Buffer);
                start += (byte)(Pos.Length + 1);
            }

        }
        public uint Type
        {
            get { return BitConverter.ToUInt32(Buffer, 4); }
            set { WriteUInt32(value, 4, Buffer); }
        }
        public uint RequiredLevel
        {
            get { return BitConverter.ToUInt32(Buffer, 12); }
            set { Writer.WriteUInt32(value, 12, Buffer); }
        }
        public uint RequiredMetempsychosis
        {
            get { return BitConverter.ToUInt32(Buffer, 16); }
            set { Writer.WriteUInt32(value, 16, Buffer); }
        }
        public uint RequiredProfession
        {
            get { return BitConverter.ToUInt32(Buffer, 20); }
            set { Writer.WriteUInt32(value, 20, Buffer); }
        }
        public uint dwParam
        {
            get { return BitConverter.ToUInt32(Buffer, 8); }
            set { WriteUInt32(value, 8, Buffer); }
        }

        public byte dwinfo
        {
            get { return Buffer[24]; }
            set { Buffer[24] = value; }
        }
        //mostafashawky424@yahoo.com

        //01229828573

        //www.egystyle.com/vb

        public string Name
        {
            get
            {
                return Encoding.ASCII.GetString(Buffer, 26, Buffer[25]);
            }
            set
            {
                byte[] Saved = Buffer;
                this.Buffer = new byte[28 + 8 + 16 + value.Length];
                buff = (byte)(28 + 8 + 16 + value.Length);
                if (Saved != null) Saved.CopyTo(Buffer, 0);
                WriteUInt16((ushort)(Buffer.Length - 8), 0, Buffer);
                WriteUInt16(1107, 2, Buffer);
                Buffer[25] = (byte)value.Length;
                WriteString(value, 26, Buffer);
            }
        }

        public string Name1
        {
            get
            {
                return Encoding.ASCII.GetString(Buffer, 39, Buffer[38]);
            }
            set
            {
                byte[] Saved = Buffer;
                this.Buffer = new byte[buff + value.Length];
                if (Saved != null) Saved.CopyTo(Buffer, 0);
                WriteUInt16((ushort)(Buffer.Length - 8), 0, Buffer);
                WriteUInt16(1107, 2, Buffer);
                Buffer[38] = (byte)value.Length;
                WriteString(value, 39, Buffer);
            }
        }

        public void Deserialize(byte[] Data)
        {
            Buffer = Data;
        }
        public byte[] ToArray()
        {
            return Buffer;
        }
        public void Send(Client.GameClient client)
        {
            client.Send(Buffer);
        }

        public static uint GuildRequirements { get; set; }

        public uint dwParam4 { get; set; }

        public uint dwParam3 { get; set; }

        public uint dwParam2 { get; set; }
    }
}