using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conquer_Online_Server.Database;
using Conquer_Online_Server.Interfaces;
using Conquer_Online_Server.Network.GamePackets;


namespace Conquer_Online_Server.Game
{
    public class Calculations
    {
        public static double PointDirecton(double x1, double y1, double x2, double y2)
        {
            double direction = 0;

            double AddX = x2 - x1;
            double AddY = y2 - y1;
            double r = (double)Math.Atan2(AddY, AddX);

            if (r < 0) r += (double)Math.PI * 2;

            direction = 360 - (r * 180 / (double)Math.PI);
            return direction;
        }
        public static double PointDirectonRad(double x1, double y1, double x2, double y2)
        {
            double AddX = x2 - x1;
            double AddY = y2 - y1;
            double r = (double)Math.Atan2(AddY, AddX);

            return r;
        }
        public static double PointDirecton2(double x1, double y1, double x2, double y2)
        {
            double direction = 0;

            double AddX = x2 - x1;
            double AddY = y2 - y1;
            double r = (double)Math.Atan2(AddY, AddX);

            direction = (r * 180 / (double)Math.PI);
            return direction;
        }
        public static double RadianToDegree(double r)
        {
            if (r < 0) r += (double)Math.PI * 2;

            double direction = 360 - (r * 180 / (double)Math.PI);
            return direction;
        }
        public static double DegreeToRadian(double degr)
        {
            return degr * Math.PI / 180;
        }
        public static int PointDistance(double x1, double y1, double x2, double y2)
        {
            return (int)Math.Sqrt(((x1 - x2) * (x1 - x2)) + ((y1 - y2) * (y1 - y2)));
        }
        public static bool InBox(double x1, double y1, double x2, double y2, byte Range)
        {
            return (Math.Max(Math.Abs(x1 - x2), Math.Abs(y1 - y2)) <= Range);
        }
        public static uint Percent(Entity attacked, float percent)
        {
            return (uint)(attacked.Hitpoints * percent);
        }
        public static uint Percent(Entity attacked, double percent)
        {
            return (uint)(attacked.Hitpoints * percent);
        }
        public static uint Percent(int target, float percent)
        {
            return (uint)(target * percent);
        }
        public static void IsBreaking(Client.GameClient client, ushort oldX, ushort oldY)
        {
            if (Conquer_Online_Server.Game.GuildWar.RightGate.Mesh == (270 + Conquer_Online_Server.Game.GuildWar.RightGate.Mesh % 10) && oldX >= Conquer_Online_Server.Game.GuildWar.RightGate.X && client.Entity.X <= Conquer_Online_Server.Game.GuildWar.RightGate.X && client.Entity.Y < Conquer_Online_Server.Game.GuildWar.LeftGate.Y)
            {
                client.Entity.X = oldX;
                client.Entity.Y = oldY;
                client.Disconnect();
                return;
            }

            if (Conquer_Online_Server.Game.GuildWar.LeftGate.Mesh == (240 + Conquer_Online_Server.Game.GuildWar.LeftGate.Mesh % 10) && oldY >= Conquer_Online_Server.Game.GuildWar.LeftGate.Y && client.Entity.Y <= Conquer_Online_Server.Game.GuildWar.LeftGate.Y && client.Entity.X < Conquer_Online_Server.Game.GuildWar.RightGate.X)
            {
                client.Entity.X = oldX;
                client.Entity.Y = oldY;
                client.Disconnect();
                return;
            }
        }
    }
}
