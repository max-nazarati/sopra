using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using KernelPanic.Entities.Projectiles;
using Microsoft.Xna.Framework;

namespace KernelPanic.Entities.Buildings
{
    // This is instantiated via black magic originating from Building.Create.
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    internal sealed class CursorShooter : StrategicTower
    {
        protected override bool WantsRotation => true;

        internal CursorShooter(SpriteManager spriteManager, SoundManager sounds)
            : base(20, 4, 2, TimeSpan.FromSeconds(1), spriteManager.CreateCursorShooter(), spriteManager, sounds)
        {
        }

        protected override IEnumerable<Projectile> CreateProjectiles(Vector2 direction)
        {
            yield return new Projectile(this, direction, 10, SpriteManager.CreateCursorProjectile())
            {
                SingleTarget = true
            };
        }
    }
}