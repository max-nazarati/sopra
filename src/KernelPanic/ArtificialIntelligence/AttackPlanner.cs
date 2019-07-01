using KernelPanic.Entities;
using KernelPanic.Players;
using KernelPanic.Purchasing;
using Microsoft.Xna.Framework;

namespace KernelPanic.ArtificialIntelligence
{
    internal sealed class AttackPlanner : Planner
    {
        #region Member

        #region UnitPurchaser

        private readonly PurchasableAction<Entity> mBug;
        private readonly PurchasableAction<Entity> mVirus;
        private readonly PurchasableAction<Entity> mTrojan;
        private readonly PurchasableAction<Entity> mThunderbird;
        private readonly PurchasableAction<Entity> mNokia;
        private readonly PurchasableAction<Entity> mFirefox;
        private readonly PurchasableAction<Entity> mSettings;
        private readonly PurchasableAction<Entity> mBluescreen;
        
        #endregion
        
        private int mTimer = 0; // this can prob be deleted in the future (usage: TroupeParade)
        
        #endregion

        #region Konstruktor
        
        public AttackPlanner(Player player, SpriteManager sprites) : base(player, sprites)
        {
            #region Initializing Member

            mBug = new PurchasableAction<Entity>(new Bug(sprites));
            mVirus = new PurchasableAction<Entity>(new Virus(sprites));
            mTrojan = new PurchasableAction<Entity>(new Trojan(sprites));
            mThunderbird = new PurchasableAction<Entity>(new Thunderbird(sprites));
            mNokia = new PurchasableAction<Entity>(new Nokia(sprites));
            mFirefox = new PurchasableAction<Entity>(new Firefox(sprites));
            mSettings = new PurchasableAction<Entity>(new Settings(sprites));
            mBluescreen = new PurchasableAction<Entity>(new Bluescreen(sprites));

            #endregion

            #region initializing Purchases

            mBug.Purchased += EntityBought;
            mVirus.Purchased += EntityBought;
            mTrojan.Purchased += EntityBought;
            mThunderbird.Purchased += EntityBought;
            mNokia.Purchased += EntityBought;
            mFirefox.Purchased += EntityBought;
            mSettings.Purchased += EntityBought;
            mBluescreen.Purchased += EntityBought;
            
            #endregion
        }
        
        #endregion

        #region Spawn Functions
        
        private void TroupeParade(int interval = 200)
        {
            if (mTimer == interval)
            {
                BuyEntity(mThunderbird);
                // BuyThunderbird();
                mTimer++;
                return;
            }
            if (mTimer == 2 * interval)
            {
                BuyEntity(mBug);
                // BuyBug();
                mTimer++;
                return;
            }
            if (mTimer == 3 * interval)
            {
                BuyEntity(mVirus);
                // BuyVirus();
                mTimer++;
                return;
            }
            if (mTimer == 4 * interval)
            {
                BuyEntity(mNokia);
                mTimer++;
                return;
            }
            if (mTimer >= 5 * interval)
            {
                BuyEntity(mTrojan);
                // BuyTrojan();
                mTimer = 0;
                return;
            }

            mTimer++;
        }
        
        #endregion

        #region Update

        public void Update(int[] attackData, GameTime gameTime)
        {
            base.Update();
            TroupeParade();
        }
        
        #endregion
        
    }
}