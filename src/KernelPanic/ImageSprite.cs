using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    sealed class ImageSprite : Sprite
    {
        /*
        public new float Height { get; set; }
        public new float Width { get; set; }
        */
        private Texture2D Texture { get; }
        public ImageSprite(Texture2D texture, int x, int y, int width, int height) : base(x, y, width, height)
        {
            Texture = texture;
        }
        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(Texture, Container, Color.White);
            spriteBatch.End();
        }
    }
}
