using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Database
{
    class HelpDesk
    {
        public static string Register = "";
        public static string Vote = "";
        public static string ChatBox = "";
        public static string Purchase = "";
        public static string Facebook = "";
        public static string ChangePass = "";
        public static string Yahoo = "";
        public static uint Number;
        public static void MrBahaaLoading()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("helpdesk");
            Conquer_Online_Server.Database.MySqlReader r = new Conquer_Online_Server.Database.MySqlReader(cmd);
            if (r.Read())
            {
                Register = r.ReadString("Register");
                Vote = r.ReadString("Vote");
                ChatBox = r.ReadString("ChatBox");
                Purchase = r.ReadString("Purchase");
                Facebook = r.ReadString("Facebook");
                ChangePass = r.ReadString("ChangePass");
                Yahoo = r.ReadString("Yahoo");
                Number = r.ReadUInt32("Number");

            }
            Console.WriteLine("HELPDESK Table Loaded.");
            r.Close();
            r.Dispose();
        }

        public static string BuyCPs { get; set; }
    }
}
