using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    class CompositeSprite : Sprite
    {
        private List<Sprite> mChildren = new List<Sprite>();
        /*
        public new int Height { get; set; }
        public new int Width { get; set; }
        */
        public CompositeSprite(Texture2D texture, SpriteFont font, string text, int x, int y, int width, int height) : base(x, y, width, height)
        {
            mChildren.Add(new ImageSprite(texture, x, y, width, height));
            mChildren.Add(new TextSprite(font, text, x, (int)(y + height/2 - font.MeasureString(text).Y/2), width, height));
        }
        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach(Sprite child in mChildren)
            {
                child.Draw(spriteBatch, gameTime);
            }
        }
    }
}
