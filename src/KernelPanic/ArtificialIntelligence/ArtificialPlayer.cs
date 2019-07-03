using System;
using System.Collections;
using System.Collections.Generic;
using KernelPanic.Entities;
using KernelPanic.Players;
using KernelPanic.Purchasing;
using KernelPanic.Table;
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
            mDefencePlanner = new DefencePlanner(this, spriteManager, soundManager);
            mUpgradePlanner = new UpgradePlanner(this, spriteManager);
            mPlanners = new Planner[] {mAttackPlanner, mDefencePlanner, mUpgradePlanner};
            mOwnTroupeAmount = new int[5]; // amount of different troupes in the game            
        }

        #region Data

        internal void InitializeAttackPlanner(Dictionary<Type, PurchasableAction<Unit>> actions, SpriteManager spriteManager)
        {
            mAttackPlanner = new AttackPlanner(this, actions, spriteManager);
        }

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
            mDefenceMoney = (int)(Bitcoins * 0.5);;
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