using System.Runtime.Serialization;
using System;
using KernelPanic.Data;
using KernelPanic.Sprites;
using KernelPanic.Table;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic.Entities
{
    internal abstract class Tower : Building
    {
        [DataMember] protected float Radius { get; set; }
        [DataMember] protected CooldownComponent FireTimer { get; private set; }

        private Sprite mRadiusSprite;

        internal Tower(int price, float radius, TimeSpan cooldown, Sprite sprite, SpriteManager spriteManager, SoundManager sounds)
            : base(price, sprite, spriteManager)
        {
            FireTimer = new CooldownComponent(cooldown);
            Radius = radius * Grid.KachelSize;
            sprite.ScaleToHeight(64);
            sprite.SetOrigin(RelativePosition.Center);

            mRadiusSprite = spriteManager.CreateTowerRadiusIndicator(Radius);
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
            FireTimer = new CooldownComponent(FireTimer.TimeSpan, !FireTimer.Ready) { Enabled = FireTimer.Enabled };
        }

        internal override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch, gameTime);
            FireTimer.Update(gameTime);

            if (Selected)
                mRadiusSprite.Draw(spriteBatch, gameTime);
        }
    }
}