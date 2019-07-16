using System;
using System.Collections.Generic;
using KernelPanic.Entities;
using KernelPanic.Entities.Units;
using KernelPanic.Players;
using KernelPanic.Purchasing;
using Microsoft.Xna.Framework;

namespace KernelPanic.ArtificialIntelligence
{
    internal sealed class AttackPlanner : Planner
    {
        private readonly Dictionary<Type, PurchasableAction<Unit>> mActions;
        private DecisionTreeClassifier mOffenseDecisionMaker;

        #region Konstruktor

        public AttackPlanner(Player player, Dictionary<Type, PurchasableAction<Unit>> actions) : base(player)
        {
            mActions = actions;
            mOffenseDecisionMaker = new DecisionTreeClassifier();
            mOffenseDecisionMaker.ReaderCsv("sopra_offense_train.csv");
            mOffenseDecisionMaker.TrainModel();
            BuyEntity<Firefox>();
            BuyEntity<Settings>();
            BuyEntity<Bluescreen>();
            BuyEntity<Bug>();
        }
        
        #endregion

        private void BuyEntity<T>(int amount=1) where T : Unit
        {
            for (var i = 0; i < amount; i++)
            {
                mActions[typeof(T)].TryPurchase(mPlayer);
            }
        }

        #region Update

        public void BuySingleUnit(string choice)
        {
            switch (choice)
            {
                case "Bug":
                    BuyEntity<Bug>();
                    break;
                case "Virus":
                    BuyEntity<Virus>();
                    break;
                case "Trojaner":
                    BuyEntity<Trojan>();
                    break;
                case "Nokia":
                    BuyEntity<Nokia>();
                    break;
                case "Thunderbird":
                    BuyEntity<Thunderbird>();
                    break;
                case "Settings":
                    BuyEntity<Settings>();
                    break;
                case "Firefox":
                    BuyEntity<Firefox>();
                    break;
                case "Bluescreen":
                    BuyEntity<Bluescreen>();
                    break;
            }
        }

        public void Update(int[] attackData, GameTime gameTime)
        {
            base.Update();
            var choiceEncoded = mOffenseDecisionMaker.Predict(Array.ConvertAll<int, double>(attackData, x => (double)x));
            var choice = mOffenseDecisionMaker.Revert(choiceEncoded);
                // Console.WriteLine(String.Join(",", defenceData.Select(p => p.ToString()).ToArray()));
            BuySingleUnit(choice);
            // TroupeParade();
        }
        
        #endregion
        
    }
}