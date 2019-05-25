using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    class TextSprite : Sprite
    {
        public SpriteFont Font { get; set; }
        public string Text { get; set; }
        public TextSprite(SpriteFont font, string text, int x, int y, int width, int height) : base(x, y, width, height)
        {
            Font = font;
            Text = text;
        }
        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(Font, Text, Container.Location.ToVector2(), Color.White);
            spriteBatch.End();
        }
    }
}
