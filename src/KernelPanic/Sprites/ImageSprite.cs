using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic.Sprites
{
    internal sealed class ImageSprite : Sprite
    {
        private Texture2D Texture { get; }
        public Rectangle? SourceRectangle { /*internal*/ private get; set; }

        /// <summary>
        /// Draw this sprite into this exact screen rectangle. This can be used to distort the image.
        /// If this is set <see cref="Sprite.Position"/> will not be used during drawing.
        /// </summary>
        public Rectangle? DestinationRectangle { /*internal*/ private get; set; }

        internal override Color TintColor { get; set; } = Color.White;

        public SpriteEffects SpriteEffect { private get; set; } = SpriteEffects.None;

        public ImageSprite(Texture2D texture)
        {
            Texture = texture;
        }

        protected override float UnscaledWidth => DestinationRectangle?.Width ?? SourceRectangle?.Width ?? Texture.Width;
        protected override float UnscaledHeight => DestinationRectangle?.Height ?? SourceRectangle?.Height ?? Texture.Height;

        protected override void Draw(SpriteBatch spriteBatch,
            GameTime gameTime,
            Vector2 position,
            float rotation,
            float scale)
        {
            if (DestinationRectangle is Rectangle destinationRectangle)
            {
                destinationRectangle.Offset(position);
                destinationRectangle.Width = (int) (destinationRectangle.Width * scale);
                destinationRectangle.Height = (int) (destinationRectangle.Height * scale);
                spriteBatch.Draw(Texture,
                    destinationRectangle,
                    SourceRectangle,
                    TintColor,
                    rotation,
                    Origin,
                    SpriteEffect,
                    1.0f);
            }
            else
            {
                spriteBatch.Draw(Texture,
                    position,
                    SourceRectangle,
                    TintColor,
                    rotation,
                    Origin,
                    scale,
                    SpriteEffect,
                    1.0f);
            }
        }
    }
}
