using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    public class SpriteManager
    {
        private ContentManager ContentManager { get; }
        public Texture2D LoadImage(string textureName)
        {
            return ContentManager.Load<Texture2D>(textureName);
        }
        public SpriteFont LoadFont(string fontName)
        {
            return ContentManager.Load<SpriteFont>(fontName);
        }
        public SpriteManager(ContentManager contentManager)
        {
            ContentManager = contentManager;
        }
    }
}
