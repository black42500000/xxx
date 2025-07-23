
namespace Conquer_Online_Server.Network.GamePackets
{
    using Conquer_Online_Server;
    using Conquer_Online_Server.Client;
    using Conquer_Online_Server.Interfaces;
    using Conquer_Online_Server.Network;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Text;

    public class Message : IPacket
    {
        public string __Message;
        public string _From;
        public string _To;
        public const uint Agate = 0x843;
        public const uint study = 2024;
        public const uint BroadcastMessage = 0x9c4;
        public const uint Center = 0x7db;
        public uint ChatType;
        public const uint Clan = 0x7d6;
        public System.Drawing.Color Color;
        public const uint ContinueRightCorner = 0x83d;
        public const uint Dialog = 0x835;
        public const uint FirstRightCorner = 0x83c;
        public const uint Friend = 0x7d9;
        public const uint Guild = 0x7d4;
        public const uint GuildAnnouncement = 0x83f;
        public const uint HawkMessage = 0x838;
        public uint Mesh;

        public uint MessageUID1;
        public uint MessageUID2;
        public const uint Monster = 0xa28;
        public const uint PopUP = 0x834;
        public const uint Qualifier = 0x7e6;
        public const uint Service = 0x7de;
        public const uint SlideFromRight = 0x186a0;
        public const uint SlideFromRightRedVib = 0xf4240;
        public const uint System = 0x7d7;
        public const uint SystemWhisper = 0x83e;
        public const uint Talk = 0x7d0;
        public const uint Team = 0x7d3;
        public const uint Tip = 0x7df;
        public const uint TopLeft = 0x7dc;
        public const uint Website = 0x839;
        public const uint ArenaQualifier = 2022;
        public const uint Whisper = 0x7d1;
        public const uint WhiteVibrate = 0x989680;
        public const uint World = 0x7e5;
        // private string p;
        // private uint p_2;
        public const uint CHISistem = 2024,
            SistemaVioleta = 2115,
                    Middle = 2115,
                    kimo = 2016;
        public const uint TradeBoard = 0x899,
            FriendBoard = 0x89a,
            TeamBoard = 0x89b,
            GuildBoard = 0x89c,
            OthersBoard = 0x89d;



        public const uint MsgMessageBoard = 1111;
        private string Mesage;
        private global::System.Drawing.Color color;
        private int p;
        //   private string Param;
        //   private string p_2;
        // private string Words;
        //   private COServer.Network.MsgMessageBoard.Channel Channel;
        //   private int p_3;



        public Message()
        {
            this.MessageUID1 = 0;
            this.MessageUID2 = 0;
            this.Mesh = 0;
        }

        public Message(string _Message, System.Drawing.Color _Color, uint _ChatType)
        {
            this.MessageUID1 = 0;
            this.MessageUID2 = 0;
            this.Mesh = 0;
            this.__Message = _Message;
            this._To = "ALL";
            this._From = "SYSTEM";
            this.Color = _Color;
            this.ChatType = _ChatType;
        }

        public Message(string _Message, string __To, System.Drawing.Color _Color, uint _ChatType)
        {
            this.MessageUID1 = 0;
            this.MessageUID2 = 0;
            this.Mesh = 0;
            this.__Message = _Message;
            this._To = __To;
            this._From = "SYSTEM";
            this.Color = _Color;
            this.ChatType = _ChatType;
        }

        public Message(string _Message, string __To, string __From, System.Drawing.Color _Color, uint _ChatType)
        {
            this.MessageUID1 = 0;
            this.MessageUID2 = 0;
            this.Mesh = 0;
            this.__Message = _Message;
            this._To = __To;
            this._From = __From;
            this.Color = _Color;
            this.ChatType = _ChatType;
        }

        public Message(string __To, string __From, string _Message, uint _ChatType, System.Drawing.Color _Color)
        {
            this.MessageUID1 = 0;
            this.MessageUID2 = 0;
            this.Mesh = 0;
            this.__Message = _Message;
            this._To = "ALL";
            this._From = __From;
            this.Color = _Color;
            this.ChatType = _ChatType;
        }

        public Message(string _Message, uint _ChatType)
        {
            this.Mesh = 0;
            this.__Message = _Message;
            this._To = "ALL";
            this._From = "SYSTEM";
            this.ChatType = _ChatType;
            this.Color = Color.White;
        }

        public Message(string Mesage, global::System.Drawing.Color color, int p)
        {
            // TODO: Complete member initialization            
            this.MessageUID1 = 0;
            this.MessageUID2 = 0;
            this.Mesh = 0;
            this.__Message = Mesage;
            this._To = "ALL";
            this._From = "SYSTEM";
            this.Mesage = Mesage;
            this.color = color;
            this.p = p;
        }

        public void Deserialize(byte[] buffer)
        {
            this.Color = Color.FromArgb(Conquer_Online_Server.BitConverter.ToInt32(buffer, 4 + 4));
            this.ChatType = Conquer_Online_Server.BitConverter.ToUInt32(buffer, 8 + 4);
            this.MessageUID1 = Conquer_Online_Server.BitConverter.ToUInt32(buffer, 12 + 4);
            this.MessageUID2 = Conquer_Online_Server.BitConverter.ToUInt32(buffer, 0x10 + 4);
            this.Mesh = Conquer_Online_Server.BitConverter.ToUInt32(buffer, 20 + 4);
            this._From = Encoding.UTF7.GetString(buffer, 0x1a, buffer[0x19 + 4]);
            this._To = Encoding.UTF7.GetString(buffer, 0x1b + 4 + this._From.Length, buffer[0x1a + 4 + this._From.Length]);
            this.__Message = Encoding.UTF7.GetString(buffer, (0x1d + 4 + this._From.Length) + this._To.Length, buffer[(0x1c + 4 + this._From.Length) + this._To.Length]);
        }

        public void Send(GameClient client)
        {
            client.Send(this.ToArray());
        }

        public byte[] ToArray()
        {
            byte[] buffer = new byte[(((0x20 + 4 + this._From.Length) + this._To.Length) + this.__Message.Length) + 9];
            Writer.WriteUInt16((ushort)(buffer.Length - 8), 0, buffer);
            Writer.WriteUInt16(0x3ec, 2, buffer);
            Writer.WriteUInt32((uint)this.Color.ToArgb(), 4 + 4, buffer);
            Writer.WriteUInt32(this.ChatType, 8 + 4, buffer);
            Writer.WriteUInt32(this.MessageUID1, 12 + 4, buffer);
            Writer.WriteUInt32(this.MessageUID2, 0x10 + 4, buffer);
            Writer.WriteUInt32(this.Mesh, 20 + 4, buffer);
            Writer.WriteStringList(new List<string> { this._From, this._To, "", this.__Message }, 0x18 + 4, buffer);
            return buffer;
        }

        public class MessageBoard
        {
            private const Int32 TITLE_SIZE = 44;
            private const Int32 LIST_SIZE = 10;

            private static List<MessageInfo> TradeBoard = new List<MessageInfo>();
            private static List<MessageInfo> FriendBoard = new List<MessageInfo>();
            private static List<MessageInfo> TeamBoard = new List<MessageInfo>();
            private static List<MessageInfo> SynBoard = new List<MessageInfo>();
            private static List<MessageInfo> OtherBoard = new List<MessageInfo>();
            private static List<MessageInfo> SystemBoard = new List<MessageInfo>();

            public struct MessageInfo
            {
                public String Author;
                public String Words;
                public String Date;
            };

            public static void Add(String Author, String Words, ushort Channel)
            {
                MessageInfo Info = new MessageInfo();
                Info.Author = Author;
                Info.Words = Words;
                Info.Date = DateTime.Now.ToString("yyyyMMddHHmmss");

                switch (Channel)
                {
                    case 2201:
                        TradeBoard.Add(Info);
                        break;
                    case 2202:
                        FriendBoard.Add(Info);
                        break;
                    case 2203:
                        TeamBoard.Add(Info);
                        break;
                    case 2204:
                        SynBoard.Add(Info);
                        break;
                    case 2205:
                        OtherBoard.Add(Info);
                        break;
                    case 2206:
                        SystemBoard.Add(Info);
                        break;
                }
            }

            public static void Delete(MessageInfo Message, ushort Channel)
            {
                switch (Channel)
                {
                    case 2201:
                        if (TradeBoard.Contains(Message))
                            TradeBoard.Remove(Message);
                        break;
                    case 2202:
                        if (FriendBoard.Contains(Message))
                            FriendBoard.Remove(Message);
                        break;
                    case 2203:
                        if (TeamBoard.Contains(Message))
                            TeamBoard.Remove(Message);
                        break;
                    case 2204:
                        if (SynBoard.Contains(Message))
                            SynBoard.Remove(Message);
                        break;
                    case 2205:
                        if (OtherBoard.Contains(Message))
                            OtherBoard.Remove(Message);
                        break;
                    case 2206:
                        if (SystemBoard.Contains(Message))
                            SystemBoard.Remove(Message);
                        break;
                }
            }

            public static String[] GetList(UInt16 Index, ushort Channel)
            {
                MessageInfo[] Board = null;
                switch (Channel)
                {
                    case 2201:
                        Board = TradeBoard.ToArray();
                        break;
                    case 2202:
                        Board = FriendBoard.ToArray();
                        break;
                    case 2203:
                        Board = TeamBoard.ToArray();
                        break;
                    case 2204:
                        Board = SynBoard.ToArray();
                        break;
                    case 2205:
                        Board = OtherBoard.ToArray();
                        break;
                    case 2206:
                        Board = SystemBoard.ToArray();
                        break;
                    default:
                        return null;
                }

                if (Board.Length == 0)
                    return null;

                if ((Index / 8 * LIST_SIZE) > Board.Length)
                    return null;

                String[] List = null;

                Int32 Start = (Board.Length - ((Index / 8 * LIST_SIZE) + 1));

                if (Start < LIST_SIZE)
                    List = new String[(Start + 1) * 3];
                else
                    List = new String[LIST_SIZE * 3];

                Int32 End = (Start - (List.Length / 3));

                Int32 x = 0;
                for (Int32 i = Start; i > End; i--)
                {
                    List[x + 0] = Board[i].Author;
                    if (Board[i].Words.Length > TITLE_SIZE)
                        List[x + 1] = Board[i].Words.Remove(TITLE_SIZE, Board[i].Words.Length - TITLE_SIZE);
                    else
                        List[x + 1] = Board[i].Words;
                    List[x + 2] = Board[i].Date;
                    x += 3;
                }
                return List;
            }

            public static String GetWords(String Author, ushort Channel)
            {
                MessageInfo[] Board = null;
                switch (Channel)
                {
                    case 2201:
                        Board = TradeBoard.ToArray();
                        break;
                    case 2202:
                        Board = FriendBoard.ToArray();
                        break;
                    case 2203:
                        Board = TeamBoard.ToArray();
                        break;
                    case 2204:
                        Board = SynBoard.ToArray();
                        break;
                    case 2205:
                        Board = OtherBoard.ToArray();
                        break;
                    case 2206:
                        Board = SystemBoard.ToArray();
                        break;
                    default:
                        return "";
                }

                foreach (MessageInfo Info in Board)
                {
                    if (Info.Author == Author)
                        return Info.Words;
                }
                return "";
            }

            public static MessageInfo GetMsgInfoByAuthor(String Author, ushort Channel)
            {
                MessageInfo[] Board = null;
                switch (Channel)
                {
                    case 2201:
                        Board = TradeBoard.ToArray();
                        break;
                    case 2202:
                        Board = FriendBoard.ToArray();
                        break;
                    case 2203:
                        Board = TeamBoard.ToArray();
                        break;
                    case 2204:
                        Board = SynBoard.ToArray();
                        break;
                    case 2205:
                        Board = OtherBoard.ToArray();
                        break;
                    case 2206:
                        Board = SystemBoard.ToArray();
                        break;
                    default:
                        return new MessageInfo();
                }

                foreach (MessageInfo Info in Board)
                {
                    if (Info.Author == Author)
                        return Info;
                }
                return new MessageInfo();
            }
        }


    }
}

