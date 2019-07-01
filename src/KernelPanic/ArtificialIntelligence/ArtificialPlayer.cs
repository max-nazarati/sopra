using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace KernelPanic.ArtificialIntelligence
{
    
    internal sealed class ArtificialPlayer
    {
        private Player mPlayer;
        private AttackPlanner mAttackPlanner;
        private DefencePlanner mDefencePlanner;
        private int[] mDefenceData;
        private int[] mAttackData;
        private UpgradePlanner mUpgradePlanner;

        private Planner[] mPlanners;
        
        public ArtificialPlayer(Player player, SpriteManager spriteManager, SoundManager soundManager)
        {
            mPlayer = player;
            mAttackPlanner = new AttackPlanner(player, spriteManager);
            mDefencePlanner = new DefencePlanner(player, spriteManager, soundManager);
            mUpgradePlanner = new UpgradePlanner(player, spriteManager);
            mPlanners = new Planner[] {mAttackPlanner, mDefencePlanner, mUpgradePlanner};
        }

        #region Data

        private void SetData()
        {
            SetAttackData();
            SetDefenceData();
        }
        
        private void SetAttackData()
        {
            var bitcoins = mPlayer.Bitcoins;
            var attackMoney = (int)(bitcoins * 0.5);
            // Data Format is:
            // Bitcoin (own), Bug (own), Trojaner, Nokia, Thunderbird, Settings, Firefox, Bluescreen, Cable(enemy), Mauszeigersch., CD-Werfer, Antivirus, Lüftung, WiFi, Shockfield
            var data = new[] {attackMoney, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
            mAttackData = data;
        }

        private void SetDefenceData()
        {
            var bitcoins = mPlayer.Bitcoins;
            var defenceMoney = (int)(bitcoins * 0.5);
            // Data Format is:
            // Bitcoin (own), Bug (enemy), Trojaner, Nokia, Thunderbird, Settings, Firefox, Bluescreen, Cable(own), Mauszeigersch., CD-Werfer, Antivirus, Lüftung, WiFi, Shockfield
            var data = new[] {defenceMoney, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
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