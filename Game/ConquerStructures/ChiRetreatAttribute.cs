using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Conquer_Online_Server.Game.ConquerStructures
{
    public struct ChiRetreatAttribute
    {
        public Enums.ChiAttribute Type;
        public ushort Value;


        public ChiRetreatAttribute(Enums.ChiAttribute type, ushort value)
        {
            Type = type;
            Value = value;

        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write((byte)Type);
            writer.Write(Value);

        }

        public void Deserialize(BinaryReader reader)
        {
            Type = (Enums.ChiAttribute)reader.ReadByte();
            Value = reader.ReadUInt16();


        }

        public static implicit operator int(ChiRetreatAttribute attribute)
        {
            return (int)attribute.Type * 10000 + attribute.Value;
        }
    }
    public class ChiRetreatStructure
    {
        public Enums.ChiPowerType Power;
        public ChiRetreatAttribute[] Attributes;
        public DateTime EndTime;

        public ChiRetreatStructure(Enums.ChiPowerType power = Enums.ChiPowerType.None)
        {
            Power = power;
            Attributes = new ChiRetreatAttribute[4];
            EndTime = DateTime.Now;
        }

        public ChiRetreatStructure Deserialize(BinaryReader reader)
        {
            Power = (Enums.ChiPowerType)reader.ReadByte();
            for (int i = 0; i < Attributes.Length; i++)
            {
                Attributes[i].Deserialize(reader);
            }
            EndTime = DateTime.FromBinary(reader.ReadInt64());
            return this;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write((byte)Power);
            for (int i = 0; i < Attributes.Length; i++)
            {
                Attributes[i].Serialize(writer);
            }
            writer.Write(EndTime.ToBinary());
        }
    }
}