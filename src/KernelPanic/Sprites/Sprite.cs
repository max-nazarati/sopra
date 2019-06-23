using System.Runtime.Serialization;
using System.ComponentModel;
using KernelPanic.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic.Sprites
{
    [DataContract]
    [KnownType(typeof(AnimatedSprite))]
    [KnownType(typeof(ImageSprite))]
    public abstract class Sprite: IBounded, IPositioned
    {
        #region Properties

        internal float Rotation { get; set; }
        /* internal */ private float Scale { get; set; } = 1.0f;
        
        /// <summary>
        /// The origin of a sprite is used as the center for rotation and as a offset for <see cref="Position"/>.
        /// Due to the way MonoGame drawing works, it has to be calculated with respect to the <see cref="UnscaledSize"/>.
        /// </summary>
        internal Vector2 Origin { get; set; } = Vector2.Zero;

        /* internal */ protected abstract float UnscaledWidth { get; }
        /* internal */ protected abstract float UnscaledHeight { get; }
        /* internal */ protected virtual Vector2 UnscaledSize => new Vector2(UnscaledWidth, UnscaledHeight);

        internal float Width => Scale * UnscaledWidth;
        internal float Height => Scale * UnscaledHeight;
        public Vector2 Size => Scale * UnscaledSize;

        internal float X { get; set; }
        internal float Y { get; set; }

        public Vector2 Position
        {
            get => new Vector2(X, Y);
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }

        /// <inheritdoc />
        // TODO: This doesn't factor in rotation which might distort the surrounding rectangle.
        //       But maybe we don't even want this but the current behaviour.
        public virtual Rectangle Bounds =>
            KernelPanic.Data.Bounds.ContainingRectangle(Position - Scale * Origin, Size);

        #endregion

        #region Smart Property Modification

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

        #endregion

        #region Drawing

        internal void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            DrawChild(this, spriteBatch, gameTime, Vector2.Zero, 0f, 1f);
        }

        /// <summary>
        /// <para>
        /// This function handles the actual drawing. The passed parameters are the <paramref name="position"/>,
        /// <paramref name="rotation"/> and <paramref name="scale"/> to be used for the drawing including this sprites
        /// values.
        /// </para>
        /// <para>
        /// The handling of <see cref="Origin"/> is at the implementers discretion. For example for
        /// <see cref="PatternSprite"/> it is a negative offset and <see cref="ImageSprite"/> uses it directly in its
        /// drawing call.
        /// </para>
        /// </summary>
        /// <param name="spriteBatch">The sprite batch into which is drawn.</param>
        /// <param name="gameTime">The current <see cref="GameTime"/>.</param>
        /// <param name="position">The position at which to draw.</param>
        /// <param name="rotation"></param>
        /// <param name="scale"></param>
        protected abstract void Draw(SpriteBatch spriteBatch,
            GameTime gameTime,
            Vector2 position,
            float rotation,
            float scale);

        protected static void DrawChild(
            Sprite sprite,
            SpriteBatch spriteBatch,
            GameTime gameTime,
            Vector2 position,
            float rotation,
            float scale)
        {
            sprite.Draw(spriteBatch,
                gameTime,
                position + sprite.Position,
                rotation + sprite.Rotation,
                scale * sprite.Scale);
        }

        #endregion
    }
}
