using System;
using System.IO;
using System.Text;


namespace Conquer_Online_Server.Database
{
    public class AccountTable
    {
        public enum AccountState : byte
        {
            PMManger = 8,  
            NotActivated = 100,
            ProjectManager = 4,
            GameHelper = 5,
            MrBahaa = 6,
            GameMaster = 3,
            Player = 2,
            Banned = 1,
            DoesntExist = 0
        }
        public string Username;
        public string Password;
        public string Email;
        public string IP;
        public DateTime LastCheck;
        public AccountState State;
        public uint EntityID;
        public int RandomKey;
        public string EarthAdress;

        public bool exists = false;
        public AccountTable(string username)
        {
            if (username == null) return;
            this.Username = username;
            this.Password = "";
            this.IP = "";
            this.LastCheck = DateTime.Now;
            this.State = AccountState.DoesntExist;
            this.EntityID = 0;
            using(var cmd = new MySqlCommand( MySqlCommandType.SELECT).Select("accounts").Where("Username", username))
            using (var reader = new MySqlReader(cmd))
            {
                if (reader.Read())
                {
                    exists = true;
                    this.Password = reader.ReadString("Password");
                    this.IP =  reader.ReadString("Ip");
                    this.EntityID =  reader.ReadUInt32("EntityID");
                    this.LastCheck = DateTime.FromBinary(reader.ReadInt64("LastCheck"));
                    this.State = (AccountState) reader.ReadInt32("State");
                    this.Email = reader.ReadString("Email");
                }
            }
        }

        public uint GenerateKey(int randomKey = 0)
        {
            if(randomKey == 0)
                RandomKey = Kernel.Random.Next(11, 253) % 100 + 1;
            return (uint)
                        (Username.GetHashCode() *
                        Password.GetHashCode() *
                        RandomKey);
        }

        public bool MatchKey(uint key)
        {
            return key == GenerateKey(RandomKey);
        }

        public void SetCurrentIP(string ip)
        {
            var loc = IPtoLocation.GetLocation(ip);
            IP = ip;

            if (loc == null)
            {
                EarthAdress = "N/A no. N/A, N/A, N/A, N/A";
            }
            else
            {
                EarthAdress = loc.city + ", " + loc.regionName + ", " + loc.countryName;
            }
        }

        public void Save()
        {
            using (var cmd = new MySqlCommand(MySqlCommandType.UPDATE))
                cmd.Update("accounts").Set("Password", Password).Set("Ip", IP).Set("EntityID", EntityID)
                    .Where("Username", Username).Execute();
        }
        public void Insert()
        {
            using (var cmd = new MySqlCommand(MySqlCommandType.INSERT))
                cmd.Insert("accounts").Insert("Username", Username)
                    .Insert("Password", Password).Insert("State", (int)State)
                    .Execute();
            exists = true;
        }
        public void SaveState()
        {
            using (var cmd = new MySqlCommand(MySqlCommandType.UPDATE))
                cmd.Update("accounts").Set("State", (int)State)
                    .Where("Username", Username).Execute();
        }
    }
}
