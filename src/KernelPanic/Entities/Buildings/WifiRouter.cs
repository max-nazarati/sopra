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
    internal sealed class WifiRouter : StrategicTower
    {
        protected override bool WantsRotation => true;

        internal WifiRouter(SpriteManager spriteManager)
            : base(80, 3, 1,3, TimeSpan.FromSeconds(1), spriteManager.CreateWifiRouter(), spriteManager)
        {
        }

        protected override IEnumerable<Projectile> CreateProjectiles(Vector2 direction)
        {
            direction = Vector2.Normalize(direction);
            EventCenter.Default.Send(Event.ProjectileShot(this));
            for (var i = 0; i < 3; ++i)
            {
                yield return new Projectile(this, direction, SpriteManager.CreateWifiProjectile(), 4 * i)
                {
                    SingleTarget = true
                };
            }
        }
    }
}