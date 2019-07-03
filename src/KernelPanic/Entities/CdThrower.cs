using System;
using System.Collections.Generic;
using KernelPanic.Data;
using KernelPanic.Input;
using KernelPanic.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace KernelPanic.Entities
{
    internal class CdThrower : StrategicTower
    {
        [JsonIgnore] private readonly List<Projectile> mProjectiles = new List<Projectile>();
        internal CdThrower(SpriteManager spriteManager
            , SoundManager sounds) : base(price:20, radius: 4, cooldown: TimeSpan.FromSeconds(2)
            , sprite: spriteManager.CreateCdThrower(), spriteManager: spriteManager, sounds: sounds)
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
                mProjectiles.Add(new Projectile(direction, Sprite.Position, Radius, Sprite.Rotation,40
                    , 7, 1, spriteManager.CreateCdProjectile()));
                sounds.PlaySound(SoundManager.Sound.Shoot1);

                if (mProjectiles.Count > 5)
                {
                    mProjectiles.RemoveAt(0);
                }

                timer.Reset();
            };

            mRadiusSprite = spriteManager.CreateTowerRadiusIndicator(Radius);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch, gameTime);

            foreach (var projectile in mProjectiles)
            {
                projectile.Draw(spriteBatch, gameTime);
            }
            
            if (!Selected)
                return;
            
            mRadiusSprite.Position = Sprite.Position;
            mRadiusSprite.Draw(spriteBatch, gameTime);
        }

        internal override void Update(PositionProvider positionProvider, GameTime gameTime, InputManager inputManager) 
        {
            if (Target(positionProvider) is Vector2 target && Vector2.Distance(target, Sprite.Position) <= Radius)
            {
                // Turn into the direction of the target.
                mInRange = true;
                Sprite.Rotation = (Sprite.Position - target).Angle(-0.5);
            }
            else
            {
                // If no unit is in range we wiggle the tower.
                mInRange = false;
                Sprite.Rotation = (float) Math.Sin(gameTime.TotalGameTime.TotalSeconds * 10 % (2 * Math.PI)) / 2;
            }

            FireTimer.Update(gameTime);
            foreach (var projectile in new List<Projectile>(mProjectiles))
            {
                projectile.Update(positionProvider);
            }
        }
    }
}