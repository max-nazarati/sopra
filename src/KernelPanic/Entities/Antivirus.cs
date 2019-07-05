using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;

namespace KernelPanic.Entities
{
    // This is instantiated via black magic originating from Building.Create.
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    internal sealed class Antivirus : StrategicTower
    {
        internal Antivirus(SpriteManager spriteManager , SoundManager sounds)
            : base(30, 5, TimeSpan.FromSeconds(3), spriteManager.CreateAntivirus(), spriteManager, sounds)
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
                Projectiles.Add(new Projectile(direction, Sprite.Position, Radius, Sprite.Rotation,20
                    , 15, 2, spriteManager.CreateCursorShooter()));
                sounds.PlaySound(SoundManager.Sound.Shoot1);

                // SoundManager.Instance.PlaySound("shoot");
                // TODO implement updated SoundManager
                
                if (Projectiles.Count > 5)
                {
                    Projectiles.RemoveAt(0);
                }

                timer.Reset();
            };

            mRadiusSprite = spriteManager.CreateTowerRadiusIndicator(Radius);
        }
    }
}