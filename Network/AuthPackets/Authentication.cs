﻿using System;
using System.IO;
using System.Text;
using Conquer_Online_Server.Network.Cryptography;

namespace Conquer_Online_Server.Network.AuthPackets
{
    public class Authentication : Interfaces.IPacket
    {
        public string Username;
        public string Password;
        public string Server;
        

        public Authentication()
        {
        }
        public void Deserialize(byte[] buffer)
        {
            if (buffer.Length == 312)
            {
                ushort length = BitConverter.ToUInt16(buffer, 0);

                if (length == 312)
                {

                    ushort type = BitConverter.ToUInt16(buffer, 2);
                    byte[] temp = new byte[16];
                    if (type == 1542)
                    {
                        MemoryStream MS = new MemoryStream(buffer);
                        BinaryReader BR = new BinaryReader(MS);

                        BR.ReadUInt16();
                        BR.ReadUInt16();
                        Username = Encoding.Default.GetString(BR.ReadBytes(32));
                        Username = Username.Replace("\0", "");
                        BR.ReadBytes(36);
                        Password = Encoding.Default.GetString(BR.ReadBytes(32));
                        Password = Password.Replace("\0", "");
                        BR.ReadBytes(32);
                        Server = Encoding.Default.GetString(BR.ReadBytes(32));
                        Server = Server.Replace("\0", "");
                        BR.Close();
                        MS.Close();
                    }
                }
            }
        }

        public byte[] ToArray()
        {
            throw new NotImplementedException();
        }
        public void Send(Client.GameClient client)
        {
            throw new NotImplementedException();
        }
    }
}
