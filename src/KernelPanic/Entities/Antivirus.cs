using System;
using KernelPanic.Sprites;
using Microsoft.Xna.Framework;

namespace KernelPanic.Entities
{
    internal class Antivirus : StrategicTower
    {
        internal Antivirus(Sprite sprite, SpriteManager spriteManager
            , SoundManager sounds) : base(price:30, radius:5, cooldown: TimeSpan.FromSeconds(3), sprite: sprite, spriteManager: spriteManager, sounds: sounds)
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
                Console.WriteLine(direction);
                mProjectiles.Add(new Projectile(direction, Sprite.Position, Radius, Sprite.Rotation,20
                    , 15, 2, spriteManager.CreateCursorShooter()));
                sounds.PlaySound(SoundManager.Sound.Shoot1);

                // SoundManager.Instance.PlaySound("shoot");
                // TODO implement updated SoundManager
                
                if (mProjectiles.Count > 5)
                {
                    mProjectiles.RemoveAt(0);
                }

                timer.Reset();
            };

            mRadiusSprite = spriteManager.CreateTowerRadiusIndicator(Radius);
        }

        internal Antivirus(SpriteManager spriteManager, SoundManager soundManager) : base(spriteManager, soundManager)
        {
        }
    }
}