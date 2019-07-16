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
    internal sealed class CursorShooter : StrategicTower
    {
        protected override bool WantsRotation => true;
        
        internal CursorShooter(SpriteManager spriteManager)
            : base(40, 4, 2, 10,TimeSpan.FromSeconds(1), spriteManager.CreateCursorShooter(), spriteManager)
        {
        }

        protected override IEnumerable<Projectile> CreateProjectiles(Vector2 direction)
        {
            EventCenter.Default.Send(Event.ProjectileShot(this));
            yield return new Projectile(this, direction, SpriteManager.CreateCursorProjectile())
            {
                SingleTarget = true
            };
        }
    }
}