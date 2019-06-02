using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    internal sealed class ImageSprite : Sprite
    {
        private Texture2D Texture { get; }
        
        public Rectangle? SourceRectangle { get; set; }
        public Rectangle? DestinationRectangle { get; set; }

        public ImageSprite(Texture2D texture, float x, float y) : base(x, y)
        {
            Texture = texture;
        }

        public override float UnscaledWidth => DestinationRectangle?.Width ?? SourceRectangle?.Width ?? Texture.Width;
        public override float UnscaledHeight => DestinationRectangle?.Height ?? SourceRectangle?.Height ?? Texture.Height;

        internal override void Draw(SpriteBatch spriteBatch,
            GameTime gameTime,
            Vector2 offset,
            float rotation,
            float scale)
        {
            if (DestinationRectangle is Rectangle destinationRectangle)
            {
                destinationRectangle.Offset(offset);
                destinationRectangle.Width = (int) (destinationRectangle.Width * scale);
                destinationRectangle.Height = (int) (destinationRectangle.Height * scale);
                spriteBatch.Draw(Texture,
                    destinationRectangle,
                    SourceRectangle,
                    Color,
                    rotation,
                    Origin,
                    SpriteEffects.None,
                    1.0f);
            }
            else
            {
                spriteBatch.Draw(Texture,
                    Position + offset,
                    SourceRectangle,
                    Color,
                    Rotation,
                    Origin,
                    Scale,
                    SpriteEffects.None,
                    1.0f);
            }
        }
    }
}
