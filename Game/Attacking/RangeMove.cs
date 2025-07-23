using System;
using System.Collections.Generic;
namespace Conquer_Online_Server.Game.Attacking
{
    public class RangeMove
    {
        public class Coords
        {
            public int X;
            public int Y;
        }
        public List<RangeMove.Coords> MoveCoords(ushort X, ushort Y, ushort XX, ushort YY)
        {
            List<RangeMove.Coords> list = new List<RangeMove.Coords>();
            this.GetAngle(X, Y, XX, YY);
            byte b = (byte)this.GetDistance(X, Y, XX, YY);
            int num = (int)(XX - X);
            int num2 = (int)(YY - Y);
            float num3 = (float)X;
            float num4 = (float)Y;
            int num5;
            if (Math.Abs(num) > Math.Abs(num2))
            {
                num5 = Math.Abs(num);
            }
            else
            {
                num5 = Math.Abs(num2);
            }
            float num6 = (float)num / (float)num5;
            float num7 = (float)num2 / (float)num5;
            list.Add(new RangeMove.Coords
            {
                X = (int)Math.Round((double)num3),
                Y = (int)Math.Round((double)num4)
            });
            for (int i = 0; i < (int)b; i++)
            {
                num3 += num6;
                num4 += num7;
                list.Add(new RangeMove.Coords
                {
                    X = (int)Math.Round((double)num3),
                    Y = (int)Math.Round((double)num4)
                });
            }
            return list;
        }
        public Enums.ConquerAngle GetAngle(ushort X, ushort Y, ushort X2, ushort Y2)
        {
            double x = (double)(X2 - X);
            double y = (double)(Y2 - Y);
            double num = Math.Atan2(y, x);
            if (num < 0.0)
            {
                num += 6.2831853071795862;
            }
            double d = 360.0 - num * 180.0 / 3.1415926535897931;
            byte b = (byte)(7.0 - Math.Floor(d) / 45.0 % 8.0 - 1.0);
            return (Enums.ConquerAngle)(b % 8);
        }
        public short GetDistance(ushort X, ushort Y, ushort X2, ushort Y2)
        {
            short num = 0;
            short num2 = 0;
            if (X >= X2)
            {
                num = (short)(X - X2);
            }
            else
            {
                if (X2 >= X)
                {
                    num = (short)(X2 - X);
                }
            }
            if (Y >= Y2)
            {
                num2 = (short)(Y - Y2);
            }
            else
            {
                if (Y2 >= Y)
                {
                    num2 = (short)(Y2 - Y);
                }
            }
            if (num > num2)
            {
                return num;
            }
            return num2;
        }
        public bool InRange(ushort X, ushort Y, byte Range, List<RangeMove.Coords> bas)
        {
            foreach (RangeMove.Coords current in bas)
            {
                byte b = (byte)this.GetDistance(X, Y, (ushort)current.X, (ushort)current.Y);
                if (b <= Range)
                {
                    return true;
                }
            }
            return false;
        }
    }
}