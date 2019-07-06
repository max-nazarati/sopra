using System;
using KernelPanic.Sprites;
using Microsoft.Xna.Framework;

namespace KernelPanic.Entities.Projectiles
{
    internal sealed class Disc : Projectile
    {
        private bool Boomerang { get; set; }

        internal Disc(Vector2 direction, Vector2 startPoint, float radius, int speed, int damage, bool boomerang, ImageSprite sprite)
            : base(direction, startPoint, radius, speed, damage, sprite)
        {
            Boomerang = boomerang;
        }

        protected override void RadiusReached()
        {
            if (!Boomerang)
            {
                WantsRemoval = true;
                return;
            }

            ClearHits();
            MoveVector *= -1;
            StartPoint = Sprite.Position;
            Boomerang = false;
        }
    }
}