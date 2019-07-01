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

        private readonly PurchasableAction<Entity> mCable;
        private readonly PurchasableAction<Entity> mCursorShooter;
        private readonly PurchasableAction<Entity> mCdThrower;
        private readonly PurchasableAction<Entity> mAntiVirus;
        private readonly PurchasableAction<Entity> mWifiRouter;
        private readonly PurchasableAction<Entity> mVentilator;
        private readonly PurchasableAction<Entity> mShockfield;
        
        #endregion

        #endregion
        public DefencePlanner(Player player, SpriteManager sprites, SoundManager soundManager) : base(player, sprites)
        {
            #region Initializing Member

            // mCable = new PurchasableAction<Entity>(new Cable(sprites, soundManager));  // TODO LOAD CABLE INTO CONTENT
            mCursorShooter = new PurchasableAction<Entity>(new CursorShooter(sprites, soundManager));
            mCdThrower = new PurchasableAction<Entity>(new CdThrower(sprites, soundManager));
            mAntiVirus = new PurchasableAction<Entity>(new Antivirus(sprites, soundManager));
            mWifiRouter = new PurchasableAction<Entity>(new WifiRouter(sprites, soundManager));
            mVentilator = new PurchasableAction<Entity>(new Ventilator(sprites, soundManager));
            mShockfield = new PurchasableAction<Entity>(new ShockField(sprites, soundManager));

            #endregion

            #region initializing Purchases

            // mCable.Purchased += EntityBought;  // TODO LOAD CABLE INTO CONTENT
            mCursorShooter.Purchased += EntityBought;
            mCdThrower.Purchased += EntityBought;
            mAntiVirus.Purchased += EntityBought;
            mWifiRouter.Purchased += EntityBought;
            mVentilator.Purchased += EntityBought;
            mShockfield.Purchased += EntityBought;

            #endregion
        }


        public void Update(int[] defenceData, GameTime gameTime)
        {
            base.Update();
            // mWifiRouter.TryPurchase(Player);
        }
        
    }
}