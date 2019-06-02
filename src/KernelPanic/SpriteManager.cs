using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    public class SpriteManager
    {
        internal static SpriteManager mSprites;
        public ContentManager ContentManager { get; set; }
        public GraphicsDevice GraphicsDevice { get; set; }

        internal static SpriteManager Default => mSprites ?? (mSprites = new SpriteManager());
        public Texture2D LoadImage(string textureName)
        {
            return ContentManager.Load<Texture2D>(textureName);
        }
        public SpriteFont LoadFont(string fontName)
        {
            return ContentManager.Load<SpriteFont>(fontName);
        }
        public SpriteManager()
        {

        }
        public SpriteManager(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            ContentManager = contentManager;
            GraphicsDevice = graphicsDevice;
        }
    }
}
