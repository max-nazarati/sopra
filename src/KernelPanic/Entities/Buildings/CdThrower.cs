using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using KernelPanic.Entities.Projectiles;
using KernelPanic.Events;
using Microsoft.Xna.Framework;

namespace KernelPanic.Entities.Buildings
{
    // This is instantiated via black magic originating from Building.Create.
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    internal sealed class CdThrower : StrategicTower
    {
        protected override bool WantsRotation => true;

        internal bool ShootsBoomerang { get; set; }

        internal CdThrower(SpriteManager spriteManager)
            : base(20, 4, 5, 7,TimeSpan.FromSeconds(2), spriteManager.CreateCdThrower(), spriteManager)
        {
        }

        protected override IEnumerable<Projectile> CreateProjectiles(Vector2 direction)
        {
            EventCenter.Default.Send(Event.ProjectileShot(this));
            yield return new Disc(this, direction, SpriteManager.CreateCdProjectile());
        }
    }
}