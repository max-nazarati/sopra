using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using KernelPanic.Entities.Projectiles;
using Microsoft.Xna.Framework;

namespace KernelPanic.Entities.Buildings
{
    // This is instantiated via black magic originating from Building.Create.
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    internal sealed class WifiRouter : StrategicTower
    {
        protected override bool WantsRotation => true;

        internal WifiRouter(SpriteManager spriteManager, SoundManager sounds)
            : base(40, 3, TimeSpan.FromSeconds(1), spriteManager.CreateWifiRouter(), spriteManager, sounds)
        {
        }

        protected override IEnumerable<Projectile> CreateProjectiles(Vector2 direction)
        {
            direction = Vector2.Normalize(direction);
            for (var i = 0; i < 3; ++i)
            {
                yield return new Projectile(direction,
                    Sprite.Position + 4 * i * direction,
                    Radius,
                    3,
                    1,
                    SpriteManager.CreateWifiProjectile())
                {
                    SingleTarget = true
                };
            }
        }
    }
}