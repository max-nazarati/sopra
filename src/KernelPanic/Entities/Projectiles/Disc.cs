using System;
using KernelPanic.Entities.Buildings;
using KernelPanic.Sprites;
using Microsoft.Xna.Framework;

namespace KernelPanic.Entities.Projectiles
{
    internal sealed class Disc : Projectile
    {
        private bool Boomerang { get; set; }

        internal Disc(CdThrower cdThrower, Vector2 direction, ImageSprite sprite)
            : base(cdThrower, direction, sprite)
        {
            Boomerang = cdThrower.ShootsBoomerang;
        }

        internal override void RadiusReached()
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