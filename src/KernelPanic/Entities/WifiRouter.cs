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
    internal class WifiRouter : StrategicTower
    {
        [JsonIgnore] private List<WifiProjectile> mProjectiles = new List<WifiProjectile>();
        internal WifiRouter(int price, float radius, TimeSpan cooldown, Sprite sprite, SpriteManager sprites
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
                Console.WriteLine(direction);
                mProjectiles.Add(new WifiProjectile(direction, Sprite.Position, mRadius, Sprite.Rotation, 40
                    , sprites.CreateWifiProjectile()));
                // sounds.PlaySound(SoundManager.Sound.Shoot1);
                
                if (mProjectiles.Count > 5)
                {
                    mProjectiles.RemoveAt(0);
                }

                timer.Reset();
            };

            mRadiusSprite = sprites.CreateTowerRadiusIndicator(radius);
        }

        internal WifiRouter(SpriteManager spriteManager, SoundManager soundManager)
            : base(spriteManager, soundManager)
        {
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
            base.Update(positionProvider, gameTime, inputManager);

            if (Target(positionProvider) is Vector2 target && Vector2.Distance(target, Sprite.Position) <= mRadius)
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

            mFireTimer.Update(gameTime);
            foreach (var projectile in new List<WifiProjectile>(mProjectiles))
            {
                if (projectile.mHasHit && projectile.mHasHit2 && projectile.mHasHit3)
                    mProjectiles.Remove(projectile);
                else
                    projectile.Update(positionProvider);
            }
        }
    }
}