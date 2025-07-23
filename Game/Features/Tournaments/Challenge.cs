using System;
using System.Collections.Generic;
using System.Text;
using Conquer_Online_Server.Client;
using Conquer_Online_Server.Database;
using Conquer_Online_Server.Network.GamePackets;
using Conquer_Online_Server.Interfaces;
using System.IO;

namespace Conquer_Online_Server.Game
{
    public class Challenge
    {
        public const int Rounds = 10;

        public GameClient PlayerOne, PlayerTwo;
        public Map FightLocation;
        public uint Wager;
        public bool Inside;
        private bool done;

        public Challenge(GameClient challenger, GameClient challenged)
        {
            PlayerOne = challenger;
            PlayerTwo = challenged;
            if (PlayerOne.Challenge == null && PlayerTwo.Challenge == null)
            {
                if (PlayerTwo.WarehousePW.Equals(string.Empty))
                {
                    PlayerTwo.MessageBox("You have been summoned to fight against someone, but for that you need to set a warehouse password first.", null, null);
                    PlayerOne.Send(PlayerTwo.Entity.Name + " doesn't have a warehouse password set. The fight cannot happen.");
                    return;
                }
                PlayerOne.Challenge = PlayerTwo.Challenge = this;
                PlayerOne.ChallengeScore = PlayerTwo.ChallengeScore = 0;

                FightLocation = Kernel.Maps[700].MakeDynamicMap();

                if (MonsterInformation.MonsterInformations.ContainsKey(MonsterInformation.ReviverID))
                {
                    MonsterInformation mi = MonsterInformation.MonsterInformations[MonsterInformation.ReviverID];
                    for (int i = -1; i < 1; i++)
                    {
                        for (int j = -1; j < 1; j++)
                        {
                            Entity reviver = new Entity(EntityFlag.Monster, false);
                            mi.RespawnTime = 0;
                            mi.BoundX = (ushort)(40 + i * -20);
                            mi.BoundY = (ushort)(40 + j * -20);

                            reviver.MapObjType = MapObjectType.Monster;
                            reviver.MonsterInfo = mi.Copy();
                            reviver.MonsterInfo.Owner = reviver;
                            reviver.Name = mi.Name;
                            reviver.MinAttack = mi.MinAttack;
                            reviver.MaxAttack = reviver.MagicAttack = mi.MaxAttack;
                            reviver.Hitpoints = reviver.MaxHitpoints = mi.Hitpoints;
                            reviver.Body = mi.Mesh;
                            reviver.Level = mi.Level;
                            reviver.UID = FightLocation.EntityUIDCounter.Next;
                            reviver.MapID = FightLocation.ID;
                            reviver.SendUpdates = true;
                            reviver.X = mi.BoundX;
                            reviver.Y = mi.BoundY;

                            FightLocation.AddEntity(reviver);
                        }
                    }
                }
            }
        }

        public static void Create(GameClient challenger, GameClient challenged)
        {
            Challenge challange = new Challenge(challenger, challenged);
        }

        private void VerifyPasswords()
        {
            if (!PlayerOne.WarehousePW.Equals(string.Empty))
            {
                PlayerOne.VerifiedChallenge = false;
                PlayerOne.VerifyChallengeCount = 0;
                PlayerOne.QCorrect = p =>
                {
                    p.VerifiedChallenge = true;
                    if (p.Challenge.PlayerOne.VerifiedChallenge && p.Challenge.PlayerTwo.VerifiedChallenge)
                        p.Challenge.Import();
                };
                PlayerOne.QWrong = p =>
                {
                    p.VerifyChallengeCount++;
                    if (p.VerifyChallengeCount >= 3)
                    {
                        p.Send("Failed to authenticate! The challenge is destroyed.");
                        p.Challenge.Crash();
                        return;
                    }
                   // p.Question("What's your warehouse password?", PlayerOne.WarehousePW);

                };
                //PlayerOne.Question("What's your warehouse password?", PlayerOne.WarehousePW);
            }
            else
                PlayerOne.VerifiedChallenge = true;
            if (!PlayerTwo.WarehousePW.Equals(string.Empty))
            {
                PlayerTwo.VerifiedChallenge = false;
                PlayerTwo.VerifyChallengeCount = 0;
                PlayerTwo.QCorrect = p =>
                {
                    p.VerifiedChallenge = true;
                    if (p.Challenge.PlayerOne.VerifiedChallenge && p.Challenge.PlayerTwo.VerifiedChallenge)
                        p.Challenge.Import();
                };
                PlayerTwo.QWrong = p =>
                {
                    p.VerifyChallengeCount++;
                    if (p.VerifyChallengeCount >= 3)
                    {
                        p.Send("Failed to authenticate! The challenge is destroyed.");
                        p.Challenge.Crash();
                        return;
                    }
                    //p.Question("What's your warehouse password?", PlayerTwo.WarehousePW);

                };
                //PlayerTwo.Question("What's your warehouse password?", PlayerTwo.WarehousePW);
            }
            else
                PlayerTwo.VerifiedChallenge = true;
            if (PlayerOne.VerifiedChallenge && PlayerTwo.VerifiedChallenge)
                Import();
        }

        private void Crash()
        {
            PlayerOne = PlayerTwo = null;
            FightLocation.Dispose();
        }

        public void Import()
        {
            if (PlayerOne.Entity.ConquerPoints >= Wager && PlayerTwo.Entity.ConquerPoints >= Wager)
            {
                if (PlayerOne.Online && PlayerTwo.Online)
                {
                    injectPlayer(PlayerOne);
                    injectPlayer(PlayerTwo);

                    Inside = true;

                    sendScores();
                }
            }
            else
            {
                if (PlayerOne.Entity.ConquerPoints < Wager)
                {
                    PlayerOne.Send("You don't have enough money to fight this fight!");
                }
                else
                {
                    PlayerTwo.Send("You don't have enough money to fight this fight!");
                }
            }
        }
        private void injectPlayer(GameClient player)
        {
            Time32 now = Time32.Now;
            var coordinates = FightLocation.RandomCoordinates();
            player.Entity.Teleport(FightLocation.BaseID, FightLocation.ID, coordinates.Item1, coordinates.Item2);
            player.Time(10);
            player.CantAttack = now.AddSeconds(10);
            player.Entity.BringToLife();
            player.Entity.ConquerPoints -= Wager;
            player.Entity.PrevPKMode = player.Entity.PKMode;
            player.Entity.PKMode = Game.Enums.PKMode.PK;
            player.Send(new Data(true) { UID = player.Entity.UID, ID = Data.ChangePKMode, dwParam = (uint)player.Entity.PKMode });
            player.Entity.OnDeath = p => 
            {
                if (p != null && p.Owner != null && p.Owner.Challenge != null)
                {
                    p.Owner.ChallengeScore++;
                    p.Owner.Challenge.sendScores();
                    p.Owner.Challenge.processScores();
                }
            };
        }

        public void End(GameClient loser)
        {
            end(Opponent(loser), loser);
        }
        public GameClient Opponent(GameClient player)
        {
            if (PlayerOne.Account.EntityID == player.Account.EntityID)
                return PlayerTwo;
            return PlayerOne;
        }
        private void processScores()
        {
            float scoreOne = PlayerTwo.ChallengeScore,
                scoreTwo = PlayerOne.ChallengeScore;
            if (scoreOne + scoreTwo != Rounds)
            {
                if (scoreOne - scoreTwo >= Rounds / 2 + 1)
                {
                    end(PlayerOne, PlayerTwo);
                }
                else if (scoreTwo - scoreOne >= Rounds / 2 + 1)
                {
                    end(PlayerTwo, PlayerOne);
                }
            }
            else
            {
                if (scoreOne > scoreTwo)
                {
                    end(PlayerOne, PlayerTwo);
                }
                else
                {
                    end(PlayerTwo, PlayerOne);
                }
            }
        }
        private void sendScores()
        {
            sendInside(new Message("----- 1 vs 1 -----", Message.FirstRightCorner));
            sendInside(new Message("-*_*- Scores -*_*-", Message.ContinueRightCorner));
            sendInside(new Message(PlayerOne.Entity.Name + ": " + PlayerTwo.ChallengeScore, Message.ContinueRightCorner));
            sendInside(new Message(PlayerTwo.Entity.Name + ": " + PlayerOne.ChallengeScore, Message.ContinueRightCorner));
        }
        private void sendInside(IPacket packet)
        {
            PlayerOne.Send(packet); 
            PlayerTwo.Send(packet);
        }
        private void end(GameClient winner, GameClient loser)
        {
            if (!done)
            {
                done = true;
                File.AppendAllText("challenge.txt", winner.Entity.Name + " vs " + loser.Entity.Name + " for " + Wager + ". Winner: " + winner.Entity.Name + "\r\n");
                winner.ChallengeScore = loser.ChallengeScore = 0;

                winner.Entity.ConquerPoints += Wager * 2;
                winner.Entity.Teleport(1002, 400, 400);
                loser.Entity.Teleport(1002, 400, 400);
                winner.Challenge = loser.Challenge = null;

                winner.Entity.PKMode = winner.Entity.PrevPKMode;
                winner.Send(new Data(true) { UID = winner.Entity.UID, ID = Data.ChangePKMode, dwParam = (uint)winner.Entity.PKMode });
                loser.Entity.PKMode = loser.Entity.PrevPKMode;
                loser.Send(new Data(true) { UID = loser.Entity.UID, ID = Data.ChangePKMode, dwParam = (uint)loser.Entity.PKMode });

                var sign = new Game.Arena.ArenaSignup();
                sign.Stats = loser.ArenaStatistic;
                sign.DialogID = Game.Arena.ArenaSignup.MainIDs.Dialog;
                sign.OptionID = Game.Arena.ArenaSignup.DialogButton.Win;
                sign.Stats = winner.ArenaStatistic;
                winner.Send(sign.BuildPacket());
                sign.OptionID = Game.Arena.ArenaSignup.DialogButton.Lose;
                loser.Send(sign.BuildPacket());

                FightLocation.Dispose();
            }
        }

        public void Send()
        {
            PlayerTwo.MessageBox(PlayerOne.Entity.Name + " has challenged you to a 1vs1 fight! The wager is: " + Wager + " ConquerPoints! Winner gets it all. Do you accept!",
                p => { p.Challenge.VerifyPasswords(); },
                p => { p.Challenge.PlayerOne.Send("The challanged has refused.", Message.Talk); }, 60);
        }
    }
}
