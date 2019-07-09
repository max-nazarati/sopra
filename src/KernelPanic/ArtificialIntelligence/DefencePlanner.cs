using System;
using System.Collections.Generic;
using KernelPanic.Entities;
using KernelPanic.Entities.Buildings;
using KernelPanic.Players;
using Microsoft.Xna.Framework;

namespace KernelPanic.ArtificialIntelligence
{
    internal sealed class DefencePlanner : Planner
    {
        private readonly SpriteManager mSpriteManager;
        private readonly SoundManager mSoundManager;
        private bool mFirstTime = true;
        private DecisionTreeClassifier mDefenseDecisionMaker;
        private int mBuildX;
        private int mBuildY;
        private int mBoundX = 18;
        private bool mBuildForward;

        public DefencePlanner(Player player, SpriteManager spriteManager, SoundManager soundManager) : base(player)
        {
            mBuildX = 9;
            mBuildY = 9;
            mBuildForward = true;
            mDefenseDecisionMaker = new DecisionTreeClassifier();
            mDefenseDecisionMaker.ReaderCsv("sopra_defense_train.csv");
            mDefenseDecisionMaker.TrainModel();
            mSpriteManager = spriteManager;
            mSoundManager = soundManager;
        }

        private void BuyBuilding<T>(Point tile) where T : Building
        {
            if (!BuildingBuyer.Buy(mPlayer, Building.Create<T>(mSpriteManager, mSoundManager), tile, mSoundManager))
                Console.WriteLine("Wanted to build " + typeof(T) + " at " + tile + " which is not possible.");
        }

        public void Update(int[] defenceData, GameTime gameTime)
        {
            base.Update();
            int choiceEncoded = mDefenseDecisionMaker.Predict(Array.ConvertAll<int, double>(defenceData, x => x));
            string choice = mDefenseDecisionMaker.Revert(choiceEncoded);
            BuySingleTower(choice);
        }

        private void BuySingleTower(string choice)
        {
            // if (!mFirstTime) return;
            switch (choice)
            {
                case "Kabel":
                    if (mBuildX < mBoundX && mBuildY < 18)
                    {
                        BuyBuilding<Cable>(new Point(mBuildX, mBuildY));
                        UpdateBuildPosition();
                    }
                    break;
                case "Mauszeigerschütze":
                    if (mBuildX < mBoundX && mBuildY < 18)
                    {
                        BuyBuilding<CursorShooter>(new Point(mBuildX, mBuildY));
                        UpdateBuildPosition();
                    }
                    break;
                case "CD-Werfer":
                    if (mBuildX < mBoundX && mBuildY < 18)
                    {
                        BuyBuilding<CdThrower>(new Point(mBuildX, mBuildY));
                        UpdateBuildPosition();
                    }
                    break;
                case "Antivirusprogramm":
                    if (mBuildX < mBoundX && mBuildY < 18)
                    {
                        BuyBuilding<Antivirus>(new Point(mBuildX, mBuildY));
                        UpdateBuildPosition();
                    }
                    break;
                case "Lüftung":
                    if (mBuildX < mBoundX && mBuildY < 18)
                    {
                        BuyBuilding<Ventilator>(new Point(mBuildX, mBuildY));
                        UpdateBuildPosition();
                    }
                    break;
                case "Wifi-Router":
                    if (mBuildX < mBoundX && mBuildY < 18)
                    {
                        BuyBuilding<WifiRouter>(new Point(mBuildX, mBuildY));
                        UpdateBuildPosition();
                    }
                    break;
                case "Schockfeld":
                    if (mBuildX < mBoundX && mBuildY < 18)
                    {
                        BuyBuilding<ShockField>(new Point(mBuildX, mBuildY));
                        UpdateBuildPosition();
                    }
                    break;
                default: break;
            }
            // 2nd wall
            /*
            BuyBuilding<CdThrower>(new Point(9, 29));
            BuyBuilding<CdThrower>(new Point(10, 29));
            BuyBuilding<CdThrower>(new Point(11, 29));
            BuyBuilding<CdThrower>(new Point(12, 29));
            BuyBuilding<CdThrower>(new Point(13, 29));
            BuyBuilding<CdThrower>(new Point(14, 29));
            BuyBuilding<CdThrower>(new Point(15, 29));
            BuyBuilding<CdThrower>(new Point(16, 29));
            BuyBuilding<CdThrower>(new Point(17, 29));
            */
            // first wall
            /*
            BuyBuilding<CdThrower>(new Point(8, 31));
            BuyBuilding<CdThrower>(new Point(9, 31));
            BuyBuilding<CdThrower>(new Point(10, 31));
            BuyBuilding<CdThrower>(new Point(11, 31));
            BuyBuilding<CdThrower>(new Point(12, 31));
            BuyBuilding<CdThrower>(new Point(13, 31));
            BuyBuilding<CdThrower>(new Point(14, 31));
            BuyBuilding<CdThrower>(new Point(15, 31));
            BuyBuilding<CdThrower>(new Point(16, 31));
            */
            // anti aircraft defence
            /*
            BuyBuilding<CdThrower>(new Point(7, 37));
            BuyBuilding<CdThrower>(new Point(8, 37));
            BuyBuilding<CdThrower>(new Point(9, 37));
            BuyBuilding<CdThrower>(new Point(10, 37));
            BuyBuilding<CdThrower>(new Point(11, 37));
            BuyBuilding<CdThrower>(new Point(12, 37));
            BuyBuilding<CdThrower>(new Point(13, 37));
            */
            // mFirstTime = false;
        }

        private void UpdateBuildPosition()
        {
            if (mBuildX < 17 && mBuildForward) mBuildX += 1;
            else if (mBuildX > 8 && !mBuildForward) mBuildX -= 1;
            else
            {
                mBuildForward = mBuildForward ? false : true;
                mBuildY += 2;
            }
        }
    }
}