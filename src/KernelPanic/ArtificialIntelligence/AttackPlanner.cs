using System;
using System.Collections.Generic;
using KernelPanic.Entities;
using KernelPanic.Interface;
using KernelPanic.Purchasing;
using Microsoft.Xna.Framework;

namespace KernelPanic.ArtificialIntelligence
{
    internal sealed class AttackPlanner : Planner
    {
        private readonly PurchasableAction<Unit> mBuyBug;
        private readonly PurchasableAction<Unit> mBuyVirus;
        private readonly PurchasableAction<Unit> mBuyTrojan;
        private readonly PurchasableAction<Unit> mBuyThunderbird;
        
        private readonly Player mPlayer;
        private int mTimer = 0;
        

        public AttackPlanner(Player player, SpriteManager sprites)
        {
            mPlayer = player;
            
            mBuyBug = new PurchasableAction<Unit>(new Bug(sprites));
            mBuyVirus = new PurchasableAction<Unit>(new Virus(sprites));
            mBuyTrojan = new PurchasableAction<Unit>(new Trojan(sprites));
            mBuyThunderbird = new PurchasableAction<Unit>(new Thunderbird(sprites));
        }

        #region Buy Troupes
        
        private static void UnitBought(Player buyer, Unit unit)
        {
            buyer.AttackingLane.UnitSpawner.Register(unit.Clone());
        }
        
        private void BuyVirus(int amount = 1)
        {
            for (var i = 0; i < amount; i++)
            {
                mBuyVirus.TryPurchase(mPlayer);
                mBuyVirus.Purchased += UnitBought;
            }
        }
        
        private void BuyThunderbird(int amount = 1)
        {
            for (var i = 0; i < amount; i++)
            {
                mBuyThunderbird.TryPurchase(mPlayer);
                mBuyThunderbird.Purchased += UnitBought;
            }
        }
        
        private void BuyBug(int amount=1)
        {
            for (var i = 0; i < amount; i++)
            {
                mBuyBug.TryPurchase(mPlayer);
                mBuyBug.Purchased += UnitBought;
            }
        }

        private void BuyTrojan(int amount=1)
        {
            for (var i = 0; i < amount; i++)
            {
                mBuyTrojan.TryPurchase(mPlayer);
                mBuyTrojan.Purchased += UnitBought;
            }
        }

        #endregion
        
        public void Update(int[] attackData, GameTime gameTime)
        {
            base.Update();
            if (mTimer == 60)
            {
                BuyThunderbird();
                mTimer++;
                return;
            }
            if (mTimer == 120)
            {
                BuyBug();
                mTimer++;
                return;
            }
            if (mTimer == 180)
            {
                BuyVirus();
                mTimer++;
                return;
            }
            if (mTimer >= 240)
            {
                BuyTrojan();
                mTimer = 0;
                return;
            }

            mTimer++;
        }
    }
}