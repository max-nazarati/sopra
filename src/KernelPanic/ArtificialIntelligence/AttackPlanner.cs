using System;
using System.Collections.Generic;
using KernelPanic.Entities;
using KernelPanic.Players;
using KernelPanic.Purchasing;
using Microsoft.Xna.Framework;

namespace KernelPanic.ArtificialIntelligence
{
    internal sealed class AttackPlanner : Planner
    {
        private readonly Dictionary<Type, PurchasableAction<Unit>> mActions;

        private int mTimer = 0; // this can prob be deleted in the future (usage: TroupeParade)

        #region Konstruktor
        
        public AttackPlanner(Player player, Dictionary<Type, PurchasableAction<Unit>> actions) : base(player)
        {
            mActions = actions;
            FastWave();
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
        
        private void TroupeParade(int interval = 200)
        {
            if (mTimer == interval)
            {
                BuyEntity<Thunderbird>();
                mTimer++;
                return;
            }
            if (mTimer == 2 * interval)
            {
                BuyEntity<Bug>();
                mTimer++;
                return;
            }
            if (mTimer == 3 * interval)
            {
                BuyEntity<Virus>();
                mTimer++;
                return;
            }
            if (mTimer == 4 * interval)
            {
                BuyEntity<Nokia>();
                mTimer++;
                return;
            }
            if (mTimer >= 5 * interval)
            {
                BuyEntity<Trojan>();
                mTimer = 0;
                return;
            }

            mTimer++;
        }

        private void FastWave()
        {
            BuyEntity<Bug>(1);
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