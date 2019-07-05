using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using KernelPanic.Data;
using KernelPanic.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace KernelPanic.Entities
{
    // This is instantiated via black magic originating from Building.Create.
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    internal sealed class WifiRouter : StrategicTower
    {
        [JsonIgnore] private readonly List<WifiProjectile> mProjectiles = new List<WifiProjectile>();

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
                mProjectiles.Add(new WifiProjectile(direction, Sprite.Position, Radius, Sprite.Rotation
                    , 40, 3, 1, spriteManager.CreateWifiProjectile()));
                // sounds.PlaySound(SoundManager.Sound.Shoot1);
                
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
            base.Update(positionProvider, gameTime, inputManager);

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
                Sprite.Rotation = (float) (gameTime.TotalGameTime.TotalSeconds % (2 * Math.PI));
            }

            FireTimer.Update(gameTime);
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