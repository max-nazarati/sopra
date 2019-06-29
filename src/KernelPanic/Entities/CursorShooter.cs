using System;
using KernelPanic.Sprites;
using Microsoft.Xna.Framework;

namespace KernelPanic.Entities
{
    internal class CursorShooter : StrategicTower
    {
        internal CursorShooter(int price, float radius, TimeSpan cooldown, Sprite sprite, SpriteManager sprites
            , SoundManager sounds) : base(price, radius, cooldown, sprite, sprites, sounds)
        {
            mFireTimer.CooledDown += timer =>
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
                mProjectiles.Add(new Projectile(direction, Sprite.Position-Sprite.Origin, mRadius, 70
                    , sprites.CreateCursorProjectile()));
                sounds.PlaySound(SoundManager.Sound.Shoot1);

                // SoundManager.Instance.PlaySound("shoot");
                // TODO implement updated SoundManager
                
                if (mProjectiles.Count > 5)
                {
                    mProjectiles.RemoveAt(0);
                }

                timer.Reset();
            };

            mRadiusSprite = sprites.CreateTowerRadiusIndicator(radius);
        }

        internal CursorShooter(SpriteManager spriteManager, SoundManager soundManager)
            : base(spriteManager, soundManager)
        {
        }
    }
}