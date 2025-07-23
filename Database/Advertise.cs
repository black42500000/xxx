/*namespace Conquer_Online_Server.Database
{
    public class Advertise
    {
        public static void Load()
        {

            using (var cmd = new MySqlCommand(MySqlCommandType.SELECT).Select("adverts"))
            using (var reader = new MySqlReader(cmd))
            {
                while (reader.Read())
                {
                    GuildAd class4 = new GuildAd
                    {
                        GuildID = reader.ReadUInt16("GuildID"),
                        Message = reader.ReadString("Message"),
                        AdvertsMoney = reader.ReadUInt16("AdvertsMoney"),
                        Date = reader.ReadUInt16("date"),
                        level = reader.ReadByte("level"),
                        reborn = reader.ReadByte("reborn")
                    };
                    Game.GuildAdvertise.GuildsAds.Add(class4.GuildID, class4);
                }
            }
            Game.GuildAdvertise.Sort(0);
            Console.WriteLine("Guild Adverts information loaded. Count " + Game.GuildAdvertise.GuildsList.Count);
        }

        public static void Insert(uint AdvertsMoney, uint GuildID, string Message, ulong date, byte reborn, byte level)
        {
            using (var cmd = new MySqlCommand(MySqlCommandType.INSERT).Insert("Adverts")
              .Insert("AdvertsMoney", (long)AdvertsMoney).Insert("Message", Message)
              .Insert("GuildID", (long)GuildID).Insert("reborn", (long)reborn).Insert("level", (long)level)
              .Insert("date", date))
                cmd.Execute();

            GuildAd Guild = new GuildAd
            {
                GuildID = GuildID,
                Message = Message,
                AdvertsMoney = AdvertsMoney,
                Date = date,
                reborn = reborn,
                level = level
            };
            Game.GuildAdvertise.GuildsAds.Add(Guild.GuildID, Guild);
            Game.GuildAdvertise.Sort(0);
        }

        public static void Remove(uint GuildID)
        {
            using (var command = new MySqlCommand(MySqlCommandType.DELETE))
                command.Delete("adverts", "GuildID", (long)GuildID).Execute();
        }

        public static void Update(uint AdvertsMoney, uint GuildID, string Message, byte reborn, byte level)
        {
            using (var cmd = new MySqlCommand(MySqlCommandType.UPDATE))
                cmd.Update("Adverts").Set("AdvertsMoney", (long)AdvertsMoney)
                    .Set("Message", Message)
                    .Set("reborn", (long)reborn)
                    .Set("level", (long)level)
                    .Where("GuildID", GuildID).Execute();
            Game.GuildAdvertise.Sort(0);
        }
    }
    public sealed class GuildAd
    {
        public byte byte_0;
        public byte level;
        public byte reborn;
        public int int_0;
        public int int_1;
        public string string_0;
        public string Message;
        public uint GuildID;
        public ulong AdvertsMoney;
        public ulong Date;
    }
}
*/