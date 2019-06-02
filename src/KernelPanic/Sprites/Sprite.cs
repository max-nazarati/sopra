using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    public abstract class Sprite
    {
        private Color mColor = Color.White;
        public Color Color
        {
            get
            {
                return mColor;
            }
            set
            {
                mColor = value;
            }
        }
        public float Rotation { get; set; } = 0.0f;
        public float Scale { get; set; } = 1.0f;
        public Vector2 Origin { get; set; } = Vector2.Zero;

        public abstract float UnscaledWidth { get; }
        public abstract float UnscaledHeight { get; }
        public virtual Vector2 UnscaledSize => new Vector2(UnscaledWidth, UnscaledHeight);

        public float Width => Scale * UnscaledWidth;
        public float Height => Scale * UnscaledHeight;
        public Vector2 Size => Scale * UnscaledSize; 
        
        public float X { get; set; }
        public float Y { get; set; }

        public Vector2 Position
        {
            get => new Vector2(X, Y);
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }

        protected Sprite(float x, float y)
        {
            X = x;
            Y = y;
        }

        internal virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Draw(spriteBatch, gameTime, Vector2.Zero, Rotation, Scale);   
        }

        internal abstract void Draw(SpriteBatch spriteBatch, GameTime gameTime, Vector2 offset, float rotation, float scale);
    }
}
