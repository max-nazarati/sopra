using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using KernelPanic.Entities.Projectiles;
using Microsoft.Xna.Framework;

namespace KernelPanic.Entities.Buildings
{
    // This is instantiated via black magic originating from Building.Create.
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    internal sealed class CdThrower : StrategicTower
    {
        protected override bool WantsRotation => true;

        internal bool ShootsBoomerang { private get; set; }

        internal CdThrower(SpriteManager spriteManager, SoundManager sounds)
            : base(20, 4, TimeSpan.FromSeconds(2), spriteManager.CreateCdThrower(), spriteManager, sounds)
        {
        }

        protected override IEnumerable<Projectile> CreateProjectiles(Vector2 direction)
        {
            yield return new Disc(direction,
                Sprite.Position,
                Radius,
                7,
                1,
                ShootsBoomerang,
                SpriteManager.CreateCdProjectile());
        }
    }
}