using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace KernelPanic
{
    class Button
    {
        private SpriteFont PFont { get; }
        public string PText { get; }
        private int XCoord { get; }
        private int YCoord { get; }
        private Color PTextColor { get;}
        private Color PBackgroundColor { get; }
        private Rectangle mMButtonRectangle;
        private readonly Texture2D mButtonBackgroundTexture;


        public Button(SpriteFont font, string text, int x, int y, int width, Color backgroundColor, Color textColor, GraphicsDeviceManager graphics)
        {
            PFont = font;
            PText = text;
            PTextColor = textColor;
            PBackgroundColor = backgroundColor;
            XCoord = (int)(x - width/1.5);
            YCoord = y;
            mMButtonRectangle = new Rectangle(new Point(XCoord, YCoord), new Point(width, (int)(font.MeasureString(text).Y)));
            mButtonBackgroundTexture = new Texture2D(graphics.GraphicsDevice, 1, 1);
            mButtonBackgroundTexture.SetData(new[] { PBackgroundColor});
        }
        public bool ContainsMouse(MouseState mouseState)
        {
            return mMButtonRectangle.Contains(mouseState.Position);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(mButtonBackgroundTexture, mMButtonRectangle, PBackgroundColor);
            spriteBatch.DrawString(PFont, PText, new Vector2(XCoord, YCoord), PTextColor);
            spriteBatch.End();
        }

    }
}
