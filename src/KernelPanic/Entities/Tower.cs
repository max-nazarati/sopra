using System.Runtime.Serialization;
using System;
using System.Collections.Generic;
using KernelPanic.Data;
using KernelPanic.Input;
using KernelPanic.Sprites;
using KernelPanic.Table;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace KernelPanic.Entities
{
    internal abstract class Tower : Building
    {
        [DataMember] protected float Radius { get; set; }
        [DataMember] protected CooldownComponent FireTimer { get; }
        [JsonIgnore] protected List<Projectile> Projectiles { get; private set; } = new List<Projectile>();

        protected Sprite mRadiusSprite;
        protected bool mInRange;

        internal Tower(int price, float radius, TimeSpan cooldown, Sprite sprite, SpriteManager spriteManager, SoundManager sounds)
            : base(price, sprite, spriteManager)
        {
            FireTimer = new CooldownComponent(cooldown);
            Radius = radius * Grid.KachelSize;
            sprite.ScaleToHeight(64);
            sprite.SetOrigin(RelativePosition.Center);
        }

        [OnDeserialized]
        private void AfterDeserialization(StreamingContext context)
        {
            mRadiusSprite = SpriteManager.CreateTowerRadiusIndicator(Radius);
        }

        protected override void CompleteClone()
        {
            base.CompleteClone();
            mRadiusSprite = mRadiusSprite.Clone();
            Projectiles = new List<Projectile>();
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch, gameTime);

            foreach (var projectile in Projectiles)
            {
                projectile.Draw(spriteBatch, gameTime);
            }
            
            if (!Selected)
                return;
            
            mRadiusSprite.Position = Sprite.Position;
            mRadiusSprite.Draw(spriteBatch, gameTime);
            
        }

        protected Vector2? Target(PositionProvider positionProvider)
        {
            var target = (Vector2?) null;
            var minDistance = 1000;
            foreach (var entity in positionProvider.NearEntities<Unit>(this, Radius))
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

            foreach (var projectile in Projectiles)
            {
                projectile.Update(positionProvider);
            }

            Projectiles.RemoveAll(projectile => projectile.mHasHit);
        }
    }
}