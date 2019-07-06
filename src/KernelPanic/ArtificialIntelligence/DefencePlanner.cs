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
            // 2nd wall
            BuyBuilding<CdThrower>(new Point(9, 29));
            BuyBuilding<CdThrower>(new Point(10, 29));
            BuyBuilding<CdThrower>(new Point(11, 29));
            BuyBuilding<CdThrower>(new Point(12, 29));
            BuyBuilding<CdThrower>(new Point(13, 29));
            BuyBuilding<CdThrower>(new Point(14, 29));
            BuyBuilding<CdThrower>(new Point(15, 29));
            BuyBuilding<CdThrower>(new Point(16, 29));
            BuyBuilding<CdThrower>(new Point(17, 29));

            // first wall
            BuyBuilding<CdThrower>(new Point(8, 31));
            BuyBuilding<CdThrower>(new Point(9, 31));
            BuyBuilding<CdThrower>(new Point(10, 31));
            BuyBuilding<CdThrower>(new Point(11, 31));
            BuyBuilding<CdThrower>(new Point(12, 31));
            BuyBuilding<CdThrower>(new Point(13, 31));
            BuyBuilding<CdThrower>(new Point(14, 31));
            BuyBuilding<CdThrower>(new Point(15, 31));
            BuyBuilding<CdThrower>(new Point(16, 31));
            
            // anti aircraft defende
            BuyBuilding<CdThrower>(new Point(4, 37));
            BuyBuilding<CdThrower>(new Point(5, 37));
            BuyBuilding<CdThrower>(new Point(6, 37));
            BuyBuilding<CdThrower>(new Point(7, 37));
            BuyBuilding<CdThrower>(new Point(8, 37));
            BuyBuilding<CdThrower>(new Point(9, 37));
            BuyBuilding<CdThrower>(new Point(10, 37));
            BuyBuilding<CdThrower>(new Point(11, 37));
            BuyBuilding<CdThrower>(new Point(12, 37));
            BuyBuilding<CdThrower>(new Point(13, 37));
            
            mFirstTime = false;
        }
    }
}