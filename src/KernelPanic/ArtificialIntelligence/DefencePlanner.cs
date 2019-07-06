using System;
using KernelPanic.Entities;
using KernelPanic.Entities.Buildings;
using KernelPanic.Players;
using Microsoft.Xna.Framework;

namespace KernelPanic.ArtificialIntelligence
{
    internal sealed class DefencePlanner : Planner
    {
        private readonly SpriteManager mSpriteManager;
        private readonly SoundManager mSoundManager;
        private bool mFirstTime = true;

        public DefencePlanner(Player player, SpriteManager spriteManager, SoundManager soundManager) : base(player)
        {
            mSpriteManager = spriteManager;
            mSoundManager = soundManager;
        }

        private void BuyBuilding<T>(Point tile) where T : Building
        {
            if (!BuildingBuyer.Buy(mPlayer, Building.Create<T>(mSpriteManager, mSoundManager), tile, mSoundManager))
                Console.WriteLine("Wanted to build " + typeof(T) + " at " + tile + " which is not possible.");
        }

        public void Update(int[] defenceData, GameTime gameTime)
        {
            base.Update();
            BuySingleTower();
        }

        private void BuySingleTower()
        {
            if (!mFirstTime) return;
            BuyBuilding<CursorShooter>(new Point(5));
            mFirstTime = false;
        }
    }
}