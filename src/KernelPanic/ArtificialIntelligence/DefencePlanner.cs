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
        
        #endregion
        
        // private int mTimer = 0; // this can prob be deleted in the future (usage: TroupeParade)
        
        #endregion
        public DefencePlanner(Player player, SpriteManager sprites) : base(player, sprites)
        {
            #region Initializing Member

            // mCable = new PurchasableAction<Entity>(new Cable(sprites));

            #endregion

            #region initializing Purchases

            // mCable.Purchased += EntityBought;

            #endregion
        }


        public void Update(int[] defenceData, GameTime gameTime)
        {
            base.Update();
            // PurchasableAction<Entity> turret = new PurchasableAction<Entity>(new StrategicTower());
        }
        
    }
}