﻿using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Conquer_Online_Server.Network.Sockets
{
    public class ServerSocket
    {
        public event Action<ClientWrapper> OnClientConnect, OnClientDisconnect;
        public event Action<byte[], int, ClientWrapper> OnClientReceive;

        private ConcurrentDictionary<int, int> BruteforceProtection;
        private const int TimeLimit = 1000 * 15; // 1 connection every 10 seconds for one ip
        private object SyncRoot;

        private Socket Connection;
        private ushort port;
        private string ipString;
        private bool enabled;
        private Thread thread;
        public ServerSocket()
        {
            this.Connection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.SyncRoot = new object();
            thread = new Thread(doSyncAccept);
            thread.Start();
        }

        public void Enable(ushort port, string ip)
        {
            this.ipString = ip;
            this.port = port;
            this.Connection.Bind(new IPEndPoint(IPAddress.Parse(ipString), this.port));
            this.Connection.Listen((int)SocketOptionName.MaxConnections);
            this.enabled = true;
            //this.Connection.BeginAccept(doAsyncAccept, null);
            BruteforceProtection = new ConcurrentDictionary<int, int>();
        }

        public bool PrintoutIPs = false;
        private void doSyncAccept()
        {
            while (true)
            {
                if (this.enabled)
                {
                    try
                    {
                        processSocket(this.Connection.Accept());
                    }
                    catch { }
                }
                Thread.Sleep(1);
            }
        }
        private void doAsyncAccept(IAsyncResult res)
        {
            try
            {
                Socket socket = this.Connection.EndAccept(res);
                processSocket(socket);
                this.Connection.BeginAccept(doAsyncAccept, null);
            }
            catch
            {

            }
        }

        private void processSocket(Socket socket)
        {
            try
            {
                string ip = (socket.RemoteEndPoint as IPEndPoint).Address.ToString();
                int ipHash = ip.GetHashCode();
                if (!Program.ALEXPC)
                {
                    int time = Time32.Now.GetHashCode();
                    int oldValue;
                    if (!BruteforceProtection.TryGetValue(ipHash, out oldValue))
                    {
                        BruteforceProtection[ipHash] = time;
                    }
                    else
                    {
                        if (time - oldValue < TimeLimit)
                        {
                            if (PrintoutIPs) Console.WriteLine("Dropped connection: " + ip);
                            socket.Disconnect(false);
                            socket.Close();
                            return;
                        }
                        else
                        {
                            BruteforceProtection[ipHash] = time;
                            if (PrintoutIPs) Console.WriteLine("Allowed connection: " + ip);
                        }
                    }
                }
                ClientWrapper wrapper = new ClientWrapper();
                wrapper.Create(socket, this, OnClientReceive);
                wrapper.Alive = true;
                wrapper.IP = ip;
                if (this.OnClientConnect != null) this.OnClientConnect(wrapper);
            }
            catch
            {

            }
        }

        public void Reset()
        {
            this.Disable();
            this.Enable();
        }

        public void Disable()
        {
            this.enabled = false;
            this.Connection.Close(1);
        }

        public void Enable()
        {
            if (!this.enabled)
            {
                this.Connection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.Connection.Bind(new IPEndPoint(IPAddress.Parse(ipString), this.port));
                this.Connection.Listen((int)SocketOptionName.MaxConnections);
                this.enabled = true;
                //this.Connection.BeginAccept(doAsyncAccept, null);
            }
        }

        public void InvokeDisconnect(ClientWrapper Client)
        {
            if (this.OnClientDisconnect != null)
                this.OnClientDisconnect(Client);
        }

        public bool Enabled
        {
            get
            {
                return this.enabled;
            }
        }
    }
}
