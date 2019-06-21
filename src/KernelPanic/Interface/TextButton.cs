using KernelPanic.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic.Interface
{
    internal sealed class TextButton : Button
    {
        internal override Sprite Sprite { get; }
        
        private readonly TextSprite mTitleSprite;

        internal TextButton(SpriteManager sprites)
        {
            (Sprite, mBackground, mTitleSprite) = sprites.CreateTextButton();
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
