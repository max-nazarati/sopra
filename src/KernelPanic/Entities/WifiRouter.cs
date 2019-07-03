using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;

namespace KernelPanic.Entities
{
    // This is instantiated via black magic originating from Building.Create.
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    internal sealed class WifiRouter : StrategicTower
    {
        internal WifiRouter(SpriteManager spriteManager, SoundManager sounds)
            : base(40, 3, TimeSpan.FromSeconds(1), spriteManager.CreateWifiRouter(), spriteManager, sounds)
        {
            FireTimer.CooledDown += timer =>
            {
                if (!mInRange)
                {
                    // If the cursor isn't in the range do nothing for now but keep the timer enabled.
                    // If it is enabled it keeps calling this callback.
                    timer.Enabled = true;
                    return;
                }

                var direction = new Vector2(
                    (float) Math.Sin(Sprite.Rotation % (Math.PI * 2)),
                    -(float) Math.Cos(Sprite.Rotation % (Math.PI * 2)));
                Projectiles.Add(new WifiProjectile(direction, Sprite.Position, Radius, Sprite.Rotation
                    , 40, 3, 1, spriteManager.CreateWifiProjectile()));

                // sounds.PlaySound(SoundManager.Sound.Shoot1);
                timer.Reset();
            };

            mRadiusSprite = spriteManager.CreateTowerRadiusIndicator(Radius);
        }
    }
}