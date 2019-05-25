using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    public class Sprite
    {
        public float Angle { get; set; }
        public float Scale { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Height { get; }
        public int Width { get; }
        public Rectangle Container { get; private set; }

        protected Sprite(int x, int y, int width, int height)
        {
            Container = new Rectangle(new Point(x, y), new Point(width, height));
        }
        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {

        }
        /*
        protected void Draw(SpriteBatch spriteBatch, GameTime gameTime, Point position)
        {

        }
        */
    }
}
