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
            BuyEntity<Thunderbird>();
            // FastWave();
            // FastAndFurious();
            mOffenseDecisionMaker = new DecisionTreeClassifier();
            mOffenseDecisionMaker.ReaderCsv("sopra_offense_train.csv");
            mOffenseDecisionMaker.TrainModel();
            BuyEntity<Firefox>();
            BuyEntity<Settings>();
            BuyEntity<Bluescreen>();
        }
        
        #endregion

        private void BuyEntity<T>(int amount=1) where T : Unit
        {
            for (var i = 0; i < amount; i++)
            {
                mActions[typeof(T)].TryPurchase(mPlayer);
            }
        }
        
        #region Spawn Functions
        
        private void FastWave()
        {
            BuyEntity<Bug>();
        }
        
        private void FastAndFurious()
        {
            BuyEntity<Bug>(15);
            BuyEntity<Virus>(10);
            BuyEntity<Thunderbird>(5);
        }
        
        #endregion

        #region Update

        public void Update(int[] attackData, GameTime gameTime)
        {
            base.Update();
            // TroupeParade();
        }
        
        #endregion
        
    }
}