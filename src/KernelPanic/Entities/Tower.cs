using System.Runtime.Serialization;
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
    internal class Tower : Building
    {
        [DataMember]
        private readonly float mRadius;
        [DataMember]
        private readonly CooldownComponent mFireTimer;
        [JsonIgnore]
        private List<Projectile> mProjectiles = new List<Projectile>();
        private Sprite mRadiusSprite;
        private bool mInRange;

        internal Tower(SpriteManager sprites, SoundManager sounds)
            : this(0, 0, new TimeSpan(), sprites.CreateTower(), sprites, sounds)
        {
        }

        internal Tower(int price, float radius, TimeSpan cooldown, Sprite sprite, SpriteManager sprites, SoundManager sounds)
            : base(price, sprite, sprites)
        {
            mFireTimer = new CooldownComponent(cooldown);
            mRadius = radius;

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
                mProjectiles.Add(new Projectile(direction, Sprite.Position, mRadius, sprites));
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

        [OnDeserialized]
        private void AfterDeserialization(StreamingContext context)
        {
            mRadiusSprite = SpriteManager.CreateTowerRadiusIndicator(mRadius);
        }

        internal static Tower Create(Vector2 position, float size, SpriteManager sprites, SoundManager sounds)
        {
            var sprite = sprites.CreateTower();
            sprite.Position = position;
            sprite.ScaleToHeight(size);
            sprite.SetOrigin(RelativePosition.Center);
            return new Tower(15, 300, new TimeSpan(0, 0, 1), sprite, sprites, sounds);
        }

        // Only for demonstration purposes.
        internal static Tower CreateStrategic(Vector2 position, float size, SpriteManager sprites, SoundManager sounds)
        {
            var sprite = sprites.CreateTower();
            sprite.Position = position;
            sprite.ScaleToHeight(size);
            sprite.SetOrigin(RelativePosition.Center);
            return new StrategicTower(15, 150, new TimeSpan(0, 0, 3), sprite, sprites, sounds);
        }
        
        internal static Tower CreateCdThrower(Vector2 position, float size, SpriteManager sprites, SoundManager sounds)
        {
            var sprite = sprites.CreateCDThrower();
            sprite.Position = position;
            sprite.ScaleToHeight(size);
            sprite.SetOrigin(RelativePosition.Center);
            return new CdThrower(15, 150, new TimeSpan(0, 0, 3), sprite, sprites, sounds);
        }
        
        internal static Tower CreateAntivirus(Vector2 position, float size, SpriteManager sprites, SoundManager sounds)
        {
            var sprite = sprites.CreateAntivirus();
            sprite.Position = position;
            sprite.ScaleToHeight(size);
            sprite.SetOrigin(RelativePosition.Center);
            return new Antivirus(15, 150, new TimeSpan(0, 0, 3), sprite, sprites, sounds);
        }

        protected override void CompleteClone()
        {
            mRadiusSprite = mRadiusSprite.Clone();
            mProjectiles = new List<Projectile>(mProjectiles);
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
        
        private Vector2? Target(PositionProvider positionProvider)
        {
            var target = (Vector2?) null;
            var minDistance = 1000;
            foreach (var entity in positionProvider.NearEntities<Unit>(this, mRadius))
            {
                var distance = (int)Vector2.Distance(entity.Sprite.Position, Sprite.Position);
                if (distance >= minDistance) continue;
                minDistance = distance;
                target = entity.Sprite.Position;
            }

            return target;
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
            foreach (var projectile in new List<Projectile>(mProjectiles))
            {
                if (projectile.mHasHit)
                    mProjectiles.Remove(projectile);
                else
                    projectile.Update(positionProvider);
            }   
        }
    }
}