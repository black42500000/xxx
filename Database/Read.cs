namespace Conquer_Online_Server.Database
{
    using Conquer_Online_Server;
    using System;
    using System.IO;
    using System.Runtime.InteropServices;

    public class Read : IDisposable
    {
        public uint aCount;
        public int Count = 0;
        private int curent_count = -1;
        private string[] items;
        private string location = "";
        private StreamReader SR;

        public Read(string file)
        {
            this.location = file;
        }

        public void Dispose()
        {
            this.location = string.Empty;
            this.SR = null;
            this.items = null;
            this.Count = 0;
        }

        public string[] OutBase()
        {
            return this.items;
        }

        public byte ReadByte(byte add_def)
        {
            this.curent_count++;
            if (this.curent_count < this.Count)
            {
                if (this.items[this.curent_count] == null)
                {
                    return add_def;
                }
                return byte.Parse(this.items[this.curent_count]);
            }
            return add_def;
        }

        public bool Reader(bool useinvalid = true)
        {
            string[] strArray = null;
            int num;
            bool flag = true;
            if (flag == File.Exists(this.location))
            {
                try
                {
                    using (this.SR = File.OpenText(this.location))
                    {
                        this.Count = int.Parse(this.SR.ReadLine().Split(new char[] { '=' })[1]);
                        strArray = new string[this.Count];
                        num = 0;
                        while (num < this.Count)
                        {
                            strArray[num] = this.SR.ReadLine();
                            num++;
                        }
                        if (this.Count == 0)
                        {
                            return false;
                        }
                        this.SR.Close();
                    }
                }
                catch (Exception exception)
                {
                    flag = false;
                    Conquer_Online_Server.Console.WriteLine(exception.ToString());
                }
            }
            else if (useinvalid)
            {
                flag = false;
                strArray = null;
                Conquer_Online_Server.Console.WriteLine("Invalid Reader " + this.location + " location");
            }
            this.items = new string[this.Count];
            for (num = 0; num < this.Count; num++)
            {
                this.items[num] = strArray[num];
            }
            return flag;
        }

        public string ReadString(string add_def)
        {
            this.curent_count++;
            if (this.curent_count < this.Count)
            {
                if (this.items[this.curent_count] == null)
                {
                    return add_def;
                }
                return this.items[this.curent_count];
            }
            return add_def;
        }

        public ushort ReadUInt16(ushort add_def)
        {
            this.curent_count++;
            if (this.curent_count < this.Count)
            {
                if (this.items[this.curent_count] == null)
                {
                    return add_def;
                }
                return ushort.Parse(this.items[this.curent_count]);
            }
            return add_def;
        }

        public uint ReadUInt32(uint add_def)
        {
            this.curent_count++;
            if (this.curent_count < this.Count)
            {
                if (this.items[this.curent_count] == null)
                {
                    return add_def;
                }
                return uint.Parse(this.items[this.curent_count]);
            }
            return add_def;
        }

        public ulong ReadUInt64(ulong add_def)
        {
            this.curent_count++;
            if (this.curent_count < this.Count)
            {
                if (this.items[this.curent_count] == null)
                {
                    return add_def;
                }
                return ulong.Parse(this.items[this.curent_count]);
            }
            return add_def;
        }

        public bool UseRead()
        {
            this.aCount++;
            return (this.items.Length >= this.aCount);
        }
    }
}

