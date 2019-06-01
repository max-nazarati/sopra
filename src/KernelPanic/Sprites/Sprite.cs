using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    public abstract class Sprite
    {
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

        /// <summary>
        /// Modifies <see cref="Scale"/> so that <see cref="Width"/> equals <paramref name="wantedWidth"/>.
        /// </summary>
        /// <param name="wantedWidth">The width to scale to</param>
        internal void ScaleToWidth(float wantedWidth) => Scale = wantedWidth / UnscaledWidth;

        /// <summary>
        /// Modifies <see cref="Scale"/> so that <see cref="Height"/> equals <paramref name="wantedHeight"/>.
        /// </summary>
        /// <param name="wantedHeight">The height to scale to</param>
        internal void ScaleToHeight(float wantedHeight) => Scale = wantedHeight / UnscaledHeight;

        internal virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Draw(spriteBatch, gameTime, Vector2.Zero, Rotation, Scale);   
        }

        internal abstract void Draw(SpriteBatch spriteBatch, GameTime gameTime, Vector2 offset, float rotation, float scale);
    }
}
