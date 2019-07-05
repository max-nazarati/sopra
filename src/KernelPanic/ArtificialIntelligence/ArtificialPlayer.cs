using System;
using System.Collections.Generic;
using KernelPanic.Entities;
using KernelPanic.Players;
using KernelPanic.Purchasing;
using KernelPanic.Table;
using KernelPanic.Upgrades;
using Microsoft.Xna.Framework;

namespace KernelPanic.ArtificialIntelligence
{
    
    internal sealed class ArtificialPlayer : Player
    {
        private AttackPlanner mAttackPlanner;
        private DefencePlanner mDefencePlanner;
        private UpgradePlanner mUpgradePlanner;
        private int[] mDefenceData;
        private int[] mAttackData;
        private int mAttackMoney;
        private int mDefenceMoney;

        private int[] mOwnTroupeAmount;

        private Planner[] mPlanners;

        internal ArtificialPlayer(Lane defendingLane, Lane attackingLane, int bitcoins, SpriteManager spriteManager, SoundManager soundManager) : base(defendingLane, attackingLane, bitcoins)
        {
            mPlanners = new Planner[] {mAttackPlanner, mDefencePlanner, mUpgradePlanner};
            mOwnTroupeAmount = new int[5]; // amount of different troupes in the game            
        }

        

        internal void InitializePlanners(
            Dictionary<Type, PurchasableAction<Unit>> unitBuyingActions,
            BuildingBuyer buildingBuyer,
            Func<Upgrade.Id, SinglePurchasableAction<Upgrade>> upgradeLookup)
        {
            mAttackPlanner = new AttackPlanner(this, unitBuyingActions);
            mDefencePlanner = new DefencePlanner(this, buildingBuyer);
            mUpgradePlanner = new UpgradePlanner(this, upgradeLookup);
        }

        #region Data
        
        private void SetData()
        {
            SetMoney();
            SetAttackData();
            SetDefenceData();
        }

        private void SetMoney()
        {
            // for now we are only splitting the money equal between attack and defence
            mAttackMoney = (int)(Bitcoins * 0.5);
            mDefenceMoney = (int)(Bitcoins * 0.5);
        }

        private void SetAttackData()
        {
            // Data Format is:
            // Bitcoin (own), Bug (own), Trojaner, Nokia, Thunderbird, Settings, Firefox, Bluescreen, Cable(enemy), Mauszeigersch., CD-Werfer, Antivirus, Lüftung, WiFi, Shockfield

            var data = new[] {mAttackMoney, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
            mAttackData = data;
        }

        private void SetDefenceData()
        {
            // Data Format is:
            // Bitcoin (own), Bug (enemy), Trojaner, Nokia, Thunderbird, Settings, Firefox, Bluescreen, Cable(own), Mauszeigersch., CD-Werfer, Antivirus, Lüftung, WiFi, Shockfield
            var data = new[] {mDefenceMoney, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
            mDefenceData = data;

        }

        #endregion

        public void Update(GameTime gameTime)
        {
            SetData();
            
            mAttackPlanner.Update(mAttackData, gameTime);
            mDefencePlanner.Update(mDefenceData, gameTime);
            mUpgradePlanner.Update();
        }
    }
}