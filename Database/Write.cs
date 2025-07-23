namespace Conquer_Online_Server.Database
{
    using Conquer_Online_Server;
    using System;
    using System.IO;

    public class Write : IDisposable
    {
        private int Count = 0;
        private string[] Items;
        private string location = "";
        private StreamWriter SW;

        public Write(string loc)
        {
            this.location = loc;
        }

        public Conquer_Online_Server.Database.Write Add(string[] data, int count)
        {
            this.Count = count;
            this.Items = new string[count];
            for (int i = 0; i < count; i++)
            {
                this.Items[i] = data[i];
            }
            return this;
        }

        public void ChangeLocation(string loc)
        {
            this.location = loc;
        }

        public void Dispose()
        {
            this.Items = null;
            this.Count = 0;
            this.location = string.Empty;
            this.SW = null;
        }

        public Conquer_Online_Server.Database.Write Execute(Conquer_Online_Server.Database.Mode mod)
        {
            int num;
            StreamWriter writer2;
            bool flag = true;
            string str = "";
            if (mod == Conquer_Online_Server.Database.Mode.Open)
            {
                if (flag == File.Exists(this.location))
                {
                    File.WriteAllText(this.location, string.Empty);
                    new FileInfo(this.location).Open(FileMode.Truncate).Close();
                    using (writer2 = this.SW = File.AppendText(this.location))
                    {
                        this.SW.WriteLine("Count=" + this.Count);
                        for (num = 0; num < this.Count; num++)
                        {
                            this.SW.WriteLine(this.Items[num]);
                        }
                        this.SW.Close();
                    }
                }
                else
                {
                    Conquer_Online_Server.Console.WriteLine(str = "Write new Reader " + this.location + " location");
                    using (writer2 = this.SW = File.AppendText(this.location))
                    {
                        this.SW.WriteLine("Count=" + this.Count);
                        for (num = 0; num < this.Count; num++)
                        {
                            this.SW.WriteLine(this.Items[num]);
                        }
                        this.SW.Close();
                    }
                }
            }
            else
            {
                using (writer2 = this.SW = File.AppendText(this.location))
                {
                    this.SW.WriteLine("Count=" + this.Count);
                    for (num = 0; num < this.Count; num++)
                    {
                        this.SW.WriteLine(this.Items[num]);
                    }
                    this.SW.Close();
                }
            }
            return this;
        }
    }
}

