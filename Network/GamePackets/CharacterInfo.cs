using System;

namespace Conquer_Online_Server.Network.GamePackets
{
    public class CharacterInfo : Writer, Interfaces.IPacket
    {
        Client.GameClient client;
        public CharacterInfo(Client.GameClient _client)
        {
            client = _client;
        }
        public void Deserialize(byte[] buffer)
        {
            throw new NotImplementedException();
        }
       /* public byte[] ToArray()
        {
            byte[] Packet = new byte[132 + client.Entity.Spouse.Length + client.Entity.Name.Length];
            WriteUInt16((ushort)(Packet.Length - 8), 0, Packet);
            WriteUInt16(1006, 2, Packet);
            WriteUInt32((uint)Time32.timeGetTime().GetHashCode(), 4, Packet);
            WriteUInt32(client.Entity.UID, 8, Packet);
            WriteUInt32(client.Entity.Mesh, 14, Packet);
            WriteUInt16(client.Entity.HairStyle, 18, Packet);
            WriteUInt32(client.Entity.Money, 20, Packet);
            WriteUInt32((uint)client.Entity.ConquerPoints, 24, Packet);
            WriteUInt64(client.Entity.Experience, 28, Packet);
            WriteUInt16(client.Entity.Strength, 56, Packet);
            WriteUInt16(client.Entity.Agility, 58, Packet);
            WriteUInt16(client.Entity.Vitality, 60, Packet);
            WriteUInt16(client.Entity.Spirit, 62, Packet);
            WriteUInt16(client.Entity.Atributes, 64, Packet);
            WriteUInt32(client.Entity.Hitpoints, 66, Packet);
            WriteUInt16(client.Entity.Mana, 70, Packet);
            WriteUInt16(client.Entity.PKPoints, 72, Packet);
            Packet[74] = client.Entity.Level;
            Packet[75] = client.Entity.Class;
            Packet[76] = client.Entity.FirstRebornClass;
            Packet[77] = client.Entity.SecondRebornClass;
            Packet[79] = client.Entity.Reborn;
            WriteUInt32(client.Entity.QuizPoints, 81, Packet);
            WriteUInt16(client.Entity.EnlightenPoints, 85, Packet);
            WriteUInt16(0/*enlightened time left, 87, Packet);
            WriteUInt16((UInt16)client.Entity.MyTitle, 97, Packet);
            Packet[119] = 1;
            WriteUInt16((ushort)client.Entity.CountryID, 120, Packet);
            WriteStringList(new System.Collections.Generic.List<string>() { client.Entity.Name, "", client.Entity.Spouse }, 122, Packet);
            return Packet;
        }*/
        public byte[] ToArray()
        {
            byte[] level = new byte[132 + 4 + this.client.Entity.Spouse.Length + this.client.Entity.Name.Length + 2];
            Writer.WriteUInt16((ushort)((int)level.Length - 8), 0, level);
            Writer.WriteUInt16(1006, 2, level);
            WriteUInt32((uint)client.Entity.BoundCps, 99 + num, Packet);
            Writer.WriteUInt32(this.client.Entity.UID, 4 + 4, level);
            Writer.WriteUInt32(this.client.Entity.Mesh, 10 + 4, level);
            Writer.WriteUInt16(this.client.Entity.HairStyle, 14 + 4, level);
            Writer.WriteUInt32(this.client.Entity.Money, 16 + 4, level);
            Writer.WriteUInt32(this.client.Entity.ConquerPoints, 20 + 4, level);
            Writer.WriteUInt64(this.client.Entity.Experience, 24 + 4, level);
            Writer.WriteUInt16(this.client.Entity.Strength, 52 + 4, level);
            Writer.WriteUInt16(this.client.Entity.Agility, 54 + 4, level);
            Writer.WriteUInt16(this.client.Entity.Vitality, 56 + 4, level);
            Writer.WriteUInt16(this.client.Entity.Spirit, 58 + 4, level);
            Writer.WriteUInt16(this.client.Entity.Atributes, 64, level);
            Writer.WriteUInt16((ushort)this.client.Entity.Hitpoints, 66 + 4, level);
            Writer.WriteUInt16(this.client.Entity.Mana, 68, level);
            Writer.WriteUInt16(this.client.Entity.PKPoints, 70, level);
            level[74] = this.client.Entity.Level;
            level[75] = this.client.Entity.Class;
            level[76] = this.client.Entity.FirstRebornClass;
            level[77] = this.client.Entity.SecondRebornClass;
            level[79] = this.client.Entity.Reborn;
            Writer.WriteUInt32(this.client.Entity.QuizPoints, 81, level);
            Writer.WriteUInt16(this.client.Entity.EnlightenPoints, 89, level);
            Writer.WriteUInt16(this.client.Entity.TitleActivated, 101, level); 
            Writer.WriteUInt16(0, 87, level);
            Writer.WriteUInt16(this.client.Entity.VIPLevel, 90, level);

           // Writer.WriteUInt16(this.client.Entity.TitleActivated, 101, level);
            //Writer.WriteUInt32(this.client.Entity.BoundCps, 103, level);
            level[119] = 1;
            Writer.WriteUInt16((ushort)this.client.Entity.CountryID, 120, level);
            Writer.WriteByte(3, 122, level);
            level[123] = (byte)this.client.Entity.Name.Length;
            Writer.WriteString(this.client.Entity.Name, 124, level);
            Writer.WriteByte((byte)this.client.Entity.Spouse.Length, 125 + this.client.Entity.Name.Length, level);
            Writer.WriteString(this.client.Entity.Spouse, 126 + this.client.Entity.Name.Length, level);
            byte[] numArray = level;
            return numArray;
        }
        public void Send(Client.GameClient client)
        {
            client.Send(ToArray());
        }

        public byte[] Packet { get; set; }

        public int num { get; set; }
    }
}
