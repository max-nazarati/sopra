using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    public class SpriteManager
    {
        
        public ContentManager ContentManager { get; protected set; }
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
