using Microsoft.Xna.Framework;

namespace KernelPanic
{
    public delegate void ClickedDelegate();

    public sealed class Button :UIComponent
    {
        public event ClickedDelegate Clicked;

        /*
        public void ButtonDelegate()
        {

        }
        */
        public override void Update(GameTime gameTime)
        {
            Clicked?.Invoke();
        }

        // old stuff below
        /*
        private SpriteFont Font { get; }
        public string Text { get; }
        private int X { get; }
        private int Y { get; }
        private Color TextColor { get;}
        private Color BackgroundColor { get; }
        private Rectangle mButtonRectangle;
        private readonly Texture2D mButtonBackgroundTexture;


        public Button(SpriteFont font, string text, int x, int y, int width, Color backgroundColor, Color textColor, IGraphicsDeviceService graphics)
        {
            Font = font;
            Text = text;
            TextColor = textColor;
            BackgroundColor = backgroundColor;
            X = (int)(x - width/1.5);
            Y = y;
            mButtonRectangle = new Rectangle(new Point(X, Y), new Point(width, (int)font.MeasureString(text).Y));
            mButtonBackgroundTexture = new Texture2D(graphics.GraphicsDevice, 1, 1);
            mButtonBackgroundTexture.SetData(new[] { BackgroundColor });
        }
        public bool ContainsMouse(MouseState mouseState)
        {
            return mButtonRectangle.Contains(mouseState.Position);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(mButtonBackgroundTexture, mButtonRectangle, BackgroundColor);
            spriteBatch.DrawString(Font, Text, new Vector2(X, Y), TextColor);
            spriteBatch.End();
        }
        */
    }
    
}
