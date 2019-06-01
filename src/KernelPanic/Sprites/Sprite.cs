using System;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    enum RelativePosition
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,
        Center,
        CenterLeft,
        CenterRight,
        CenterTop,
        CenterBottom
    }
    
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
            switch (origin)
            {
                case RelativePosition.TopLeft:
                    Origin = Vector2.Zero;
                    break;
                case RelativePosition.TopRight:
                    Origin = new Vector2(Width, 0);
                    break;
                case RelativePosition.BottomLeft:
                    Origin = new Vector2(0, Height);
                    break;
                case RelativePosition.BottomRight:
                    Origin = new Vector2(Width, Height);
                    break;
                case RelativePosition.Center:
                    Origin = new Vector2(Width / 2, Height / 2);
                    break;
                case RelativePosition.CenterLeft:
                    Origin = new Vector2(0, Height / 2);
                    break;
                case RelativePosition.CenterRight:
                    Origin = new Vector2(Width, Height / 2);
                    break;
                case RelativePosition.CenterTop:
                    Origin = new Vector2(Width / 2, 0);
                    break;
                case RelativePosition.CenterBottom:
                    Origin = new Vector2(Width / 2, Height);
                    break;
                default:
                    throw new InvalidEnumArgumentException(nameof(origin), (int)origin, typeof(RelativePosition));
            }
        }
        
        internal virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Draw(spriteBatch, gameTime, Vector2.Zero, Rotation, Scale);   
        }

        internal abstract void Draw(SpriteBatch spriteBatch, GameTime gameTime, Vector2 offset, float rotation, float scale);
    }
}
