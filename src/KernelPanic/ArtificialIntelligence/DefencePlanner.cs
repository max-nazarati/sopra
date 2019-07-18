using System;
using KernelPanic.Entities;
using KernelPanic.Entities.Buildings;
using KernelPanic.Events;
using KernelPanic.Players;
using Microsoft.Xna.Framework;

namespace KernelPanic.ArtificialIntelligence
{
    internal sealed class DefencePlanner : Planner
    {
        private readonly SpriteManager mSpriteManager;
        private readonly DecisionTreeClassifier mDefenseDecisionMaker;
        private int mBuildX;
        private int mBuildY;
        private readonly int mBoundX = 18;
        private bool mBuildForward;

        public DefencePlanner(Player player, SpriteManager spriteManager) : base(player)
        {
            mBuildX = 8;
            mBuildY = 9;
            mBuildForward = true;
            mDefenseDecisionMaker = new DecisionTreeClassifier();
            mDefenseDecisionMaker.ReaderCsv("sopra_defense_train.csv");
            mDefenseDecisionMaker.TrainModel();
            var eventCenter = EventCenter.Default;
            eventCenter.Subscribe(Event.Id.BuildingPlaced,
                e =>
                {
                    UpdateBuildPosition();
                },
                e => !e.IsActivePlayer(Event.Key.Buyer));
            mSpriteManager = spriteManager;
        }

        private void BuyBuilding<T>(Point tile) where T : Building
        {
            if (!BuildingBuyer.Buy(mPlayer, Building.Create<T>(mSpriteManager), tile))
                Console.WriteLine("Wanted to build " + typeof(T) + " at " + tile + " which is not possible.");
        }

        public void Update(int[] defenceData, GameTime gameTime)
        {
            base.Update();
            var choiceEncoded = mDefenseDecisionMaker.Predict(Array.ConvertAll<int, double>(defenceData, x => (double)x));
            var choice = mDefenseDecisionMaker.Revert(choiceEncoded);
            // Console.WriteLine(String.Join(",", defenceData.Select(p => p.ToString()).ToArray()));
            BuySingleTower(choice);
        }

        public void BuyRandomTower(GameTime gameTime)
        {
            string[] choices = {"Kabel", "Mauszeigerschütze", "CD-Werfer", "Antivirusprogramm", "Lüftung", "Wifi-Router", "Schockfeld"};
            Random numberGenerator = new Random();
            int number = numberGenerator.Next(0, 6);
            string choice = choices[number];
            BuySingleTower(choice);
        }

        private void BuySingleTower(string choice)
        {
            // if (!mFirstTime) return;
            switch (choice)
            {
                case "Kabel":
                    BuyBuilding<Cable>(new Point(mBuildX, mBuildY));
                    break;
                case "Mauszeigerschütze":
                    BuyBuilding<CursorShooter>(new Point(mBuildX, mBuildY));
                    break;
                case "CD-Werfer":
                    BuyBuilding<CdThrower>(new Point(mBuildX, mBuildY));
                    break;
                case "Antivirusprogramm":
                    BuyBuilding<Antivirus>(new Point(mBuildX, mBuildY));
                    break;
                case "Lüftung":
                    BuyBuilding<Ventilator>(new Point(mBuildX, mBuildY));
                    break;
                case "Wifi-Router":
                    BuyBuilding<WifiRouter>(new Point(mBuildX, mBuildY));
                    break;
                case "Schockfeld":
                    BuyBuilding<ShockField>(new Point(mBuildX, mBuildY));
                    break;
            }
        }

        private void UpdateBuildPosition()
        {
            if (mBuildX < 16 && mBuildForward) mBuildX += 1;
            else if (mBuildX > 9 && !mBuildForward) mBuildX -= 1;
            else
            {
                if (mBuildForward)
                {
                    mBuildForward = false;
                    mBuildX++;
                }
                else
                {
                    mBuildForward = true;
                    mBuildX--;
                }
                mBuildY += 2;
            }
        }
    }
}