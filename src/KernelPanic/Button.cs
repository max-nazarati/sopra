using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace KernelPanic
{
    class Button
    {
        public SpriteFont PFont { get; }
        public string PText { get; }
        public int XCoord { get; }
        public int YCoord { get; }
        public Color PTextColor { get;}
        public Color PBackgroundColor { get; set; }
        private Rectangle _mButtonRectangle;
        private readonly Texture2D _ButtonBackgroundTexture;


        public Button(SpriteFont font, string text, int x, int y, int width, Color backgroundColor, Color textColor, GraphicsDeviceManager graphics)
        {
            PFont = font;
            PText = text;
            PTextColor = textColor;
            PBackgroundColor = backgroundColor;
            XCoord = (int)(x - width/1.5);
            YCoord = y;
            _mButtonRectangle = new Rectangle(new Point(XCoord, YCoord), new Point(width, (int)(font.MeasureString(text).Y)));
            _ButtonBackgroundTexture = new Texture2D(graphics.GraphicsDevice, 1, 1);
            _ButtonBackgroundTexture.SetData(new[] { PBackgroundColor});
        }
        public bool ContainsMouse(MouseState mouseState)
        {
            return _mButtonRectangle.Contains(mouseState.Position);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(_ButtonBackgroundTexture, _mButtonRectangle, PBackgroundColor);
            spriteBatch.DrawString(PFont, PText, new Vector2(XCoord, YCoord), PTextColor);
            spriteBatch.End();
        }

    }
}
