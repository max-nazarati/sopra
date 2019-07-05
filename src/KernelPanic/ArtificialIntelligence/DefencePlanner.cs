using System;
using System.Collections.Generic;
using KernelPanic.Entities;
using KernelPanic.Players;
using KernelPanic.Purchasing;
using Microsoft.Xna.Framework;

namespace KernelPanic.ArtificialIntelligence
{
    internal sealed class DefencePlanner : Planner
    {
        
        private readonly BuildingBuyer mBuildingBuyer;
        private bool mFirstTime = true;
        
        public DefencePlanner(Player player, BuildingBuyer buildingBuyer) : base(player)
        {
            mBuildingBuyer = buildingBuyer;
        }

        private void BuyBuilding<T>(Point point) where T : Building
        {
            // mActions[typeof(T)].TryPurchase(mPlayer);

        }

        public void Update(int[] defenceData, GameTime gameTime)
        {
            base.Update();
            // BuySingleTower();
        }

        private void BuySingleTower()
        {
            if (!mFirstTime) return;
            BuyBuilding<CursorShooter>(new Point(5));
            mFirstTime = false;
        }
    }
}