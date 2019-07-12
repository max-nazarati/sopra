using System;
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
        private readonly DecisionTreeClassifier mDefenseDecisionMaker;
        private int mBuildX;
        private int mBuildY;
        private readonly int mBoundX = 18;
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
            var choiceEncoded = mDefenseDecisionMaker.Predict(Array.ConvertAll<int, double>(defenceData, x => x));
            var choice = mDefenseDecisionMaker.Revert(choiceEncoded);
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
            }
        }

        private void UpdateBuildPosition()
        {
            if (mBuildX < 17 && mBuildForward) mBuildX += 1;
            else if (mBuildX > 8 && !mBuildForward) mBuildX -= 1;
            else
            {
                mBuildForward = !mBuildForward;
                mBuildY += 2;
            }
        }
    }
}