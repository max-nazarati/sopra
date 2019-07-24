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
        private int mWaveAmount;
        protected override bool WantsRotation => true;

        internal WifiRouter(SpriteManager spriteManager)
            : base(80, 3.0f, 1,3, TimeSpan.FromSeconds(2), spriteManager.CreateWifiRouter(), spriteManager)
        {
            mWaveAmount = 2;
        }

        protected override IEnumerable<Projectile> CreateProjectiles(Vector2 direction)
        {
            direction = Vector2.Normalize(direction);
            EventCenter.Default.Send(Event.ProjectileShot(this));
            for (var i = 0; i < mWaveAmount; ++i)
            {
                yield return new Wifi(this, direction, SpriteManager.CreateWifiProjectile(), this, 16 * i)
                {
                    SingleTarget = false
                };
            }
        }

        internal void IncreaseWaveCount(int amount = 2)
        {
            mWaveAmount += amount;
        }
    }
}