namespace Conquer_Online_Server.Game
{
    using System;

    public interface AttJiangHu
    {
        DateTime CountDownEnd { get; set; }

        string JiangName { get; set; }

        uint FreeCourse { get; set; }

        uint FreeTimeTodey { get; set; }

        uint FreeTimeTodeyUsed { get; set; }

        ushort Inner_Strength { get; set; }

        byte Level { get; set; }

        Conquer_Online_Server.Game.JiangHu.OldStar MyOldStar { get; set; }

        bool OnJiangHu { get; set; }

        string OwnName { get; set; }

        byte Rank { get; set; }

        ushort RoundBuyPoints { get; set; }

        byte Stage { get; set; }

        Game.JiangHu.JiangStages[] Stagers { get; set; }

        byte Star { get; set; }

        DateTime StartCountDwon { get; set; }

        byte Talent { get; set; }

        uint Time { get; }

        uint UID { get; set; }
    }
}

