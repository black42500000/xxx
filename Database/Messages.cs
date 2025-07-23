using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Database
{
    class Messagess
    {
        public static string Sys = "";
        public static string Sys2 = "";
        public static string Sys3 = "";
        public static string Sys4 = "";
        public static string Sys5 = "";
        public static string Sys6 = "";
        public static string Sys7 = "";
        public static string Sys8 = "";
        public static string Sys9 = "";

        public static void Load()
        {
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("systemmessage");
            Database.MySqlReader r = new Database.MySqlReader(cmd);
            if (r.Read())
            {
                Sys = r.ReadString("Sys");
                Sys2 = r.ReadString("Sys2");
                Sys3 = r.ReadString("Sys3");
                Sys4 = r.ReadString("Sys4");
                Sys5 = r.ReadString("Sys5");
                Sys6 = r.ReadString("Sys6");
                Sys7 = r.ReadString("Sys7");
                Sys8 = r.ReadString("Sys8");
                Sys9 = r.ReadString("Sys9");

            }
            Console.WriteLine("Messages Inilitized.");
            r.Close();
            r.Dispose();
        }
    }
}
