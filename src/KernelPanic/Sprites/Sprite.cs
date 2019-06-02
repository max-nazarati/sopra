using System.ComponentModel;
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

        /// <summary>
        /// Sets the origin based on the current <see cref="Width"/> and <see cref="Height"/>.
        /// </summary>
        /// <remarks>
        /// If <see cref="Width"/> or <see cref="Height"/> change, <see cref="Origin"/> is not update.
        /// You can either call this function again or update it yourself.
        /// </remarks>
        /// <param name="origin">The relative position to set the origin to</param>
        /// <exception cref="InvalidEnumArgumentException">If <paramref name="origin"/> isn't one of the listed enum values.</exception>
        internal void SetOrigin(RelativePosition origin)
        {
            Origin = origin.RectangleOrigin(UnscaledSize);
        }
        
        internal virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Draw(spriteBatch, gameTime, -Origin, Rotation, Scale);   
        }

        internal abstract void Draw(SpriteBatch spriteBatch, GameTime gameTime, Vector2 offset, float rotation, float scale);
    }
}
