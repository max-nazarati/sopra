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
        private readonly PurchasableAction<Unit> mBug;
        private readonly PurchasableAction<Unit> mVirus;
        private readonly PurchasableAction<Unit> mTrojan;
        private readonly PurchasableAction<Unit> mThunderbird;
        private readonly PurchasableAction<Unit> mNokia;
        private readonly PurchasableAction<Unit> mFirefox;
        private readonly PurchasableAction<Unit> mSettings;
        private readonly PurchasableAction<Unit> mBluescreen;
        
        private readonly Player mPlayer;
        private int mTimer = 0;
        

        public AttackPlanner(Player player, SpriteManager sprites)
        {
            mPlayer = player;
            
            mBug = new PurchasableAction<Unit>(new Bug(sprites));
            mVirus = new PurchasableAction<Unit>(new Virus(sprites));
            mTrojan = new PurchasableAction<Unit>(new Trojan(sprites));
            mThunderbird = new PurchasableAction<Unit>(new Thunderbird(sprites));
            mNokia = new PurchasableAction<Unit>(new Nokia(sprites));
            mFirefox = new PurchasableAction<Unit>(new Firefox(sprites));
            mSettings = new PurchasableAction<Unit>(new Settings(sprites));
            mBluescreen = new PurchasableAction<Unit>(new Bluescreen(sprites));
            mBug.Purchased += UnitBought;
            mVirus.Purchased += UnitBought;
            mTrojan.Purchased += UnitBought;
            mThunderbird.Purchased += UnitBought;
            mNokia.Purchased += UnitBought;
            mFirefox.Purchased += UnitBought;
            mSettings.Purchased += UnitBought;
            mBluescreen.Purchased += UnitBought;
        }

        #region Buy Troupes
        
        private static void UnitBought(Player buyer, Unit unit)
        {
            buyer.AttackingLane.UnitSpawner.Register(unit.Clone());
        }

        private void BuyUnit(PurchasableAction<Unit> unit, int amount=1)
        {
            for (var i = 0; i < amount; i++)
            {
                unit.TryPurchase(mPlayer);
            }
        }
        
        #endregion

        #region Spawn Functions
        
        private void TroupeParade(int interval = 200)
        {
            if (mTimer == interval)
            {
                BuyUnit(mThunderbird);
                // BuyThunderbird();
                mTimer++;
                return;
            }
            if (mTimer == 2 * interval)
            {
                BuyUnit(mBug);
                // BuyBug();
                mTimer++;
                return;
            }
            if (mTimer == 3 * interval)
            {
                BuyUnit(mVirus);
                // BuyVirus();
                mTimer++;
                return;
            }
            if (mTimer == 4 * interval)
            {
                BuyUnit(mNokia);
                mTimer++;
                return;
            }
            if (mTimer >= 5 * interval)
            {
                BuyUnit(mTrojan);
                // BuyTrojan();
                mTimer = 0;
                return;
            }

            mTimer++;
        }
        
        #endregion
        
        public void Update(int[] attackData, GameTime gameTime)
        {
            base.Update();
            TroupeParade();
        }
    }
}