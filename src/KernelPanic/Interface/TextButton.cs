using KernelPanic.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic.Interface
{
    internal class TextButton : Button
    {
        internal override Sprite Sprite { get; }
        
        private readonly TextSprite mTitleSprite;

        internal TextButton(SpriteManager sprites, int width = 250, int height = 70)
        {
            (Sprite, mBackground, mTitleSprite) = sprites.CreateTextButton(width, height);
        }

        internal string Title
        {
            get => mTitleSprite.Text;
            set => mTitleSprite.Text = value;
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            mTitleSprite.TextColor = ViewEnabled ? Color.Black : Color.DarkGray;
            base.Draw(spriteBatch, gameTime);
        }
    }
}
