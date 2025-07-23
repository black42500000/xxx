namespace Conquer_Online_Server.Database
{
    using Conquer_Online_Server;
    using System;
    using System.Runtime.InteropServices;
    using System.Collections.Generic;
    using Conquer_Online_Server.Game;

    public class Houses
    {
        public static SafeDictionary<ushort, DMapInformation> HouseInfo = new SafeDictionary<ushort, DMapInformation>(280);
        public static void Load()
        {
            MySqlCommand command = new MySqlCommand(MySqlCommandType.SELECT);
            command.Select("house");
            MySqlReader reader = new MySqlReader(command);
            while (reader.Read())
            {
                MapsTable.MapInformation information = new MapsTable.MapInformation();
                {
                    information.ID = reader.ReadUInt16("id");
                    information.BaseID = reader.ReadUInt16("mapdoc");
                    information.Status = reader.ReadUInt32("type");
                    information.Weather = reader.ReadUInt32("weather");
                    information.Owner = reader.ReadUInt32("owner");
                    information.HouseLevel = reader.ReadUInt32("HouseLevel");
                    information.Box = reader.ReadUInt32("Box");
                    information.BoxX = reader.ReadUInt32("BoxX");
                    information.BoxY = reader.ReadUInt32("BoxY");
                };
                MapsTable.MapInformations.Add(information.ID, information);
            }
            Conquer_Online_Server.Console.WriteLine("Houses Loaded informations loaded.");
        }

        public static void LoadHouse(Client.GameClient client)
        {
            try
            {
                MySqlCommand command = new MySqlCommand(MySqlCommandType.SELECT);
                command.Select("house").Where("id", (long)client.Account.EntityID);
                MySqlReader reader = new MySqlReader(command);
                while (reader.Read())
                {
                    MapsTable.MapInformation information = new MapsTable.MapInformation
                    {
                        ID = ((ushort)client.Entity.UID),
                        BaseID = reader.ReadUInt16("mapdoc"),
                        Status = 7,
                        Weather = 0,
                        Owner = client.Entity.UID,
                        HouseLevel = reader.ReadUInt32("HouseLevel")

                    };
                    MapsTable.MapInformations.Add(information.ID, information);
                    if (!Kernel.Maps.ContainsKey(information.BaseID))
                    {
                        new Map(information.BaseID, DMaps.MapPaths[information.BaseID]);
                    }

                }
            }
            catch (Exception exception)
            {
                Program.SaveException(exception);
            }
        }

        public static void recreatehouse(Client.GameClient client)
        {
            MySqlCommand command = new MySqlCommand(MySqlCommandType.SELECT);
            command.Select("house");
            MySqlReader reader = new MySqlReader(command);
            while (reader.Read())
            {
                MapsTable.MapInformation information = new MapsTable.MapInformation
                {
                    ID = reader.ReadUInt16("id"),
                    BaseID = reader.ReadUInt16("mapdoc"),
                    Status = 7,
                    Weather = 0,
                    Owner = reader.ReadUInt32("owner"),
                    HouseLevel = reader.ReadUInt32("HouseLevel")
                };
                MapsTable.MapInformations.Add(information.ID, information);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DMapInformation
        {
            public ushort ID;
            public ushort BaseID;
            public uint Status;
            public uint Weather;
            public uint Owner;
            public uint HouseLevel;
        }
    }
}
namespace Conquer_Online_Server.Game
{
    using Conquer_Online_Server.Client;
    using Conquer_Online_Server.Database;
    using System;

    internal class House
    {
        public static void AddBox(GameClient client)
        {
            MapsTable.MapInformations.Remove(((ushort)client.Entity.UID));
            Kernel.Maps.Remove(((ushort)client.Entity.UID));
            MapsTable.MapInformation information = new MapsTable.MapInformation
            {
                ID = (ushort)client.Entity.UID,
                BaseID = 0x44b,
                Status = 7,
                Weather = 0,
                Owner = client.Entity.UID,
                HouseLevel = 2,
                Box = 1,
                BoxX = client.Entity.X,
                BoxY = client.Entity.Y
            };
            MapsTable.MapInformations.Add(information.ID, information);
            new MySqlCommand(MySqlCommandType.UPDATE).Update("house")
                .Set("Box", "1").Set("BoxX", (long)client.Entity.X)
                .Set("BoxY", (long)client.Entity.Y)
                .Where("id", (long)client.Entity.UID).Execute();
        }
        public static void createhouse(GameClient client)
        {
            MySqlCommand command = new MySqlCommand(MySqlCommandType.INSERT);
            command.Insert("house").Insert("id", (long)client.Entity.UID)
                .Insert("mapdoc", "1098").Insert("owner", (long)client.Entity.UID)
                .Insert("HouseLevel", "1");
            command.Execute();
            MapsTable.MapInformation information = new MapsTable.MapInformation
            {
                ID = (ushort)client.Entity.UID,
                BaseID = 1098,
                Status = 7,
                Weather = 0,
                Owner = client.Entity.UID,
                HouseLevel = 1
            };
            MapsTable.MapInformations.Add(information.ID, information);
        }
        public static void UpgradeHouse(GameClient client, ushort _base, byte level)
        {
            //1098 level 1
            //1099 level 2
            //2080 level 3
            //1765 level 4
            //3024 level 5
            MapsTable.MapInformations.Remove((ushort)client.Entity.UID);
            Kernel.Maps.Remove(((ushort)client.Entity.UID));
            MapsTable.MapInformation information = new MapsTable.MapInformation
            {
                ID = (ushort)client.Entity.UID,
                BaseID = _base,
                Status = 7,
                Weather = 0,
                Owner = client.Entity.UID,
                HouseLevel = level
            };
            MapsTable.MapInformations.Add(information.ID, information);
            new MySqlCommand(MySqlCommandType.UPDATE).Update("house").Set("HouseLevel", level)
                .Set("mapdoc", _base).Where("id", (long)client.Entity.UID).Execute();
        }
    }
}
