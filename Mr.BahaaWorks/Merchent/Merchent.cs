namespace Conquer_Online_Server.Network
{
    using Conquer_Online_Server.Client;
    using Conquer_Online_Server.Database;
    using Conquer_Online_Server.Network.GamePackets;
    using System;
    using System.Drawing;

    public class MerchantTable
    {
        public static System.Collections.Generic.Dictionary<uint, MerchantInfo> MerchantCharacters = new System.Collections.Generic.Dictionary<uint, MerchantInfo>();
        public static void CancelMerch(GameClient client)
        {
            if (client.Entity.Merchant > 0)
            {
                MySqlCommand command = new MySqlCommand(MySqlCommandType.DELETE);
                command.Delete("merchant", "UID", (long)client.Entity.UID).Execute();
            }
            else
            {
                client.Send(new Message("You already not merchant!", Color.Red, 0x7d5));
            }
        }

        public static void InsertMerch(GameClient client)
        {
            if (client.Entity.Merchant == 0)
            {
                client.Entity.MerchantDate = DateTime.Now;
                client.Entity.Merchant = 1;
                MySqlCommand command = new MySqlCommand(MySqlCommandType.INSERT);
                command.Insert("merchant").Insert("UID", (long)client.Account.EntityID).Insert("Merchant", (long)client.Entity.Merchant).Insert("Date", client.Entity.MerchantDate.Ticks).Execute();
            }
            else
            {
                client.Send(new Message("You already merchant!", Color.Red, 0x7d5));
            }
        }

        public static void LoadMerchant(GameClient client)
        {
            MySqlCommand command = new MySqlCommand(MySqlCommandType.SELECT);
            command.Select("merchant").Where("UID", (long)client.Entity.UID);
            MySqlReader reader = new MySqlReader(command);
            while (reader.Read())
            {
                client.Entity.Merchant = reader.ReadUInt16("Merchant");
                client.Entity.MerchantDate = DateTime.FromBinary(reader.ReadInt64("Date"));
                if ((DateTime.Now > client.Entity.MerchantDate.AddDays(1.0)) && (client.Entity.Merchant == 1))
                {
                    client.Entity.Merchant = 0xff;
                    UpdateMerchant(client);
                }
                MerchantInfo info = new MerchantInfo
                {
                    UID = client.Entity.UID,

                };
                if (!MerchantCharacters.ContainsKey(info.UID))
                {
                    MerchantCharacters.Add(info.UID, info);
                }
            }
            //  Reader.Close();
        }

        public static void UpdateMerchant(GameClient client)
        {
            client.Entity.Merchant = 0xff;
            MySqlCommand command = new MySqlCommand(MySqlCommandType.SELECT);
            command.Select("merchant").Where("UID", (long)client.Entity.UID).Set("Merchant", (long)0xffL).Set("Date", (long)0L).Execute();
        }
    }
    public class MerchantInfo
    {
        public ulong Experience;
        public byte Level;
        public uint UID;
    }
}