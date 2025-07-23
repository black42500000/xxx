using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer_Online_Server.Interfaces;

namespace Conquer_Online_Server.Network.GamePackets
{
    #region Class Level
    public class SubClassShowFull : Writer, Interfaces.IPacket
    {
        public const byte
        SwitchSubClass = 0,
        ActivateSubClass = 1,
        LearnSubClass = 4,
        Effect = 5,
        ShowGUI = 7;

        byte[] Buffer;
        public SubClassShowFull(bool Create)
        {
            if (Create)
            {
                Buffer = new byte[8 + 30];
                WriteUInt16(30, 0, Buffer);
                WriteUInt16(2320, 2, Buffer);
                WriteUInt32((uint)Time32.timeGetTime().GetHashCode(), 4, Buffer);
            }
        }
        public ushort Study
        {
            get
            {
                return Conquer_Online_Server.BitConverter.ToUInt16(this.Buffer, 10);
            }
            set
            {
                Conquer_Online_Server.Network.Writer.WriteUInt16(value, 10, this.Buffer);
            }
        }

        public ushort StudyReceive
        {
            get
            {
                return Conquer_Online_Server.BitConverter.ToUInt16(this.Buffer, 0x12);
            }
            set
            {
                Conquer_Online_Server.Network.Writer.WriteUInt16(value, 0x12, this.Buffer);
            }
        }  
        public ushort ID
        {
            get { return BitConverter.ToUInt16(Buffer, 8); }
            set { WriteUInt16(value, 8, Buffer); }
        }

        public byte Class
        {
            get { return Buffer[10]; }
            set { Buffer[10] = value; }
        }

        public byte Level
        {
            get { return Buffer[11]; }
            set { Buffer[11] = value; }
        }

        public void Deserialize(byte[] buffer)
        {
            this.Buffer = buffer;
        }

        public byte[] ToArray()
        {
            return Buffer;
        }

        public void Send(Client.GameClient c)
        {
            c.Send(Buffer);
        }
    }
    #endregion
    #region Class Send
    public class Game_SubClassInfo : IPacket
    {
        private Byte[] mData;
        private Int32 Offset = 30;

        public Game_SubClassInfo(Game.Entity c, Game_SubClass.Types Type)
        {
            this.mData = new Byte[30 + (c.SubClasses.Classes.Count * 3) + 8];
            Writer.WriteUInt16((UInt16)(mData.Length - 8), 0, mData);
            Writer.WriteUInt16((UInt16)2320, 2, mData);
           Writer.WriteUInt32((uint)Time32.timeGetTime().GetHashCode(), 4, mData);
            Writer.WriteByte((Byte)Type, 8, mData);
            Writer.WriteUInt16((UInt16)c.SubClasses.StudyPoints, 10, mData);
            Writer.WriteUInt16((UInt16)c.SubClasses.Classes.Count, 26, mData);

            foreach (Game.SubClass subc in c.SubClasses.Classes.Values)
            {
                Writer.WriteByte((Byte)subc.ID, Offset, mData); Offset++;
                Writer.WriteByte((Byte)subc.Phase, Offset, mData); Offset++;
                Writer.WriteByte((Byte)subc.Level, Offset, mData); Offset++;
            }
        }
        public void Deserialize(byte[] buffer)
        {
            this.mData = buffer;
        }

        public byte[] ToArray()
        {
            return mData;
        }

        public void Send(Client.GameClient c)
        {
            c.Send(mData);
        }

    }
    public class Game_SubClass : IPacket
    {
        private Byte[] mData;

        public Game_SubClass()
        {
            this.mData = new Byte[29 + 8];
            Writer.WriteUInt16((UInt16)(mData.Length - 8), 0, mData);
            Writer.WriteUInt16((UInt16)2320, 2, mData);
            //Writer.WriteUInt32((uint)Time32.timeGetTime().GetHashCode(), 4, mData);
        }

        public Types Type
        {
            get { return (Types)BitConverter.ToUInt16(mData, 4); }
            set { Writer.WriteUInt16((Byte)value, 4, mData); }
        }
        public ID ClassId
        {
            get { return (ID)mData[6]; }
            set { mData[6] = (Byte)value; }
        }
        public Byte Phase
        {
            get { return mData[7]; }
            set { mData[7] = value; }
        }
        public void Deserialize(byte[] buffer)
        {
            this.mData = buffer;
        }

        public byte[] ToArray()
        {
            return mData;
        }

        public void Send(Client.GameClient c)
        {
            c.Send(mData);
        }

        public enum Types : ushort
        {
            Switch = 0,
            Activate = 1,
            Upgrade = 2,
            Learn = 4,
            MartialPromoted = 5,
            Show = 6,
            Info = 7
        }
        public enum ID : byte
        {
            None = 0,
            MartialArtist = 1,
            Warlock = 2,
            ChiMaster = 3,
            Sage = 4,
            Apothecary = 5,
            Performer = 6,
            Wrangler = 9
        }
    }
    public class SubClass : Writer, Interfaces.IPacket
    {
        public const byte
        SwitchSubClass = 0,
        ActivateSubClass = 1,
        ShowSubClasses = 7,
        MartialPromoted = 5,
        LearnSubClass = 4;
        Game.Entity Owner = null;

        byte[] Buffer;
        byte Type;
        public SubClass(Game.Entity E) { Owner = E; Type = 7; }

        public void Deserialize(byte[] buffer)
        {
            this.Buffer = buffer;
        }

        public byte[] ToArray()
        {
            Buffer = new byte[8 + 30 + (Owner.SubClasses.Classes.Count * 3)];
            WriteUInt16((ushort)(Buffer.Length - 8), 0, Buffer);
            WriteUInt16(2320, 2, Buffer);
            Writer.WriteUInt32((uint)Time32.timeGetTime().GetHashCode(), 4, Buffer);
            WriteUInt16((ushort)Type, 8, Buffer);
            WriteUInt16(Owner.SubClasses.StudyPoints, 10, Buffer);
            WriteUInt16((ushort)Owner.SubClasses.Classes.Count, 26, Buffer);
            int Position = 30;
            if (Owner.SubClasses.Classes.Count > 0)
            {
                Game.SubClass[] Classes = new Game.SubClass[Owner.SubClasses.Classes.Count];
                Owner.SubClasses.Classes.Values.CopyTo(Classes, 0);
                foreach (Game.SubClass Class in Classes)
                {
                    WriteByte(Class.ID, Position, Buffer); Position++;
                    WriteByte(Class.Phase, Position, Buffer); Position++;
                    WriteByte(Class.Level, Position, Buffer); Position++;
                }
            }
            WriteString("TQServer", (Buffer.Length - 8), Buffer);
            return Buffer;
        }

        public void Send(Client.GameClient c)
        {
            c.Send(Buffer);
        }
    }
    #endregion
}
