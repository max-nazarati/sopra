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
        
        internal Emp(Tower origin, ImageSprite sprite, TimeSpan timeToLive) : base(origin, Vector2.Zero, 0, sprite, 0)
        {
            mTimeToLive = timeToLive;
        }
        
        internal void Hit(Building building)
        {
            HandleHit(building);
        }
        
        private void HandleHit(Building building)
        {
            // building.HitByEmp();
        }

        public override void Update(PositionProvider positionProvider, InputManager inputManager, GameTime gameTime)
        {
            mTimeToLive -= gameTime.ElapsedGameTime.Duration();
            if (mTimeToLive <= TimeSpan.Zero)
            {
                WantsRemoval = true;
            }
        }
    }
}