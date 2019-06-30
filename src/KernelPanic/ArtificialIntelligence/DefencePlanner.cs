using System;
using System.Collections.Generic;
using KernelPanic.Entities;
using KernelPanic.Purchasing;
using Microsoft.Xna.Framework;

namespace KernelPanic.ArtificialIntelligence
{
    internal sealed class DefencePlanner : Planner
    {
        
        #region Member

        #region UnitPurchaser

        private readonly PurchasableAction<Entity> mWifiRouter;
        
        #endregion

        #endregion
        public DefencePlanner(Player player, SpriteManager sprites, SoundManager soundManager) : base(player, sprites)
        {
            #region Initializing Member

            mWifiRouter = new PurchasableAction<Entity>(new WifiRouter(sprites, soundManager));

            #endregion

            #region initializing Purchases

            mWifiRouter.Purchased += EntityBought;

            #endregion
        }


        public void Update(int[] defenceData, GameTime gameTime)
        {
            base.Update();
            // mWifiRouter.TryPurchase(Player);
        }
        
    }
}