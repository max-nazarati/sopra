using System;
using KernelPanic.Entities.Units;
using KernelPanic.Input;
using KernelPanic.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic.Interface
{
    internal class AnimatedButton : TextButton
    {
        private readonly ImageSprite ButtonOverlay;
        private readonly Hero mHero;
        internal AnimatedButton(SpriteManager sprites, Hero hero, int width = 250, int height = 70) : base(sprites, width, height)
        {
            mHero = hero;
            ButtonOverlay = sprites.CreateAntivirus();
        }
        
        public override void Update(InputManager inputManager, GameTime gameTime)
        {
            base.Update(inputManager, gameTime);
            var width = (int)(50 * mHero.RemainingCooldownTime);
            ButtonOverlay.DestinationRectangle = new Rectangle((int)Position.X, (int)Position.Y, width, 70);
        }
        
        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch, gameTime);
            ButtonOverlay.Draw(spriteBatch, gameTime);
        }
    }
}