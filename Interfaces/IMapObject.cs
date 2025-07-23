using System;

namespace Conquer_Online_Server.Interfaces
{
    public interface IMapObject
    {
        ushort X { get; }
        ushort Y { get; }
        ushort MapID { get; }
        uint UID { get; }
        Client.GameClient Owner { get; }
        Game.MapObjectType MapObjType { get; }
        void SendSpawn(Client.GameClient client);
        void SendSpawn(Client.GameClient client, bool checkScreen);
    }
}
