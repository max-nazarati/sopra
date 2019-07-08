using KernelPanic.Entities.Buildings;
using System;
using KernelPanic.Input;
using KernelPanic.Sprites;
using Microsoft.Xna.Framework;

namespace KernelPanic.Entities.Projectiles
{
    internal sealed class Emp : Projectile
    {
        private TimeSpan mTimeToLive;
        private Tower mHitTower;
        
        internal Emp(Tower origin, TimeSpan timeToLive, ImageSprite sprite) : base(origin, Vector2.One, sprite, 0)
        {
            mTimeToLive = timeToLive;
        }

        internal void Hit(Tower tower)
        {
            tower.HitByEmp();
            mHitTower = tower;
        }

        public override void Update(PositionProvider positionProvider, InputManager inputManager, GameTime gameTime)
        {
            mTimeToLive -= gameTime.ElapsedGameTime.Duration();
            if (mTimeToLive <= TimeSpan.Zero)
            {
                WantsRemoval = true;
                mHitTower.EmpIsGone();
            }
        }
    }
}