using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;

namespace KernelPanic.Entities
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

        protected override Projectile CreateProjectile(Vector2 direction)
        {
            return new WifiProjectile(direction,
                Sprite.Position,
                Radius,
                Sprite.Rotation,
                40,
                3,
                1,
                SpriteManager.CreateWifiProjectile());
        }
    }
}