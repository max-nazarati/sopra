using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    public class SpriteManager
    {
        public ContentManager ContentManager { get; private set; }
        public GraphicsDevice GraphicsDevice { get; private set; }
        public Texture2D LoadImage(string textureName)
        {
            return ContentManager.Load<Texture2D>(textureName);
        }
        public SpriteFont LoadFont(string fontName)
        {
            return ContentManager.Load<SpriteFont>(fontName);
        }
        public SpriteManager(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            ContentManager = contentManager;
            GraphicsDevice = graphicsDevice;
        }
    }
}
