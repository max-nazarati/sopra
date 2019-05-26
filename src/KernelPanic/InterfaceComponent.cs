using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    public class InterfaceComponent
    {
        public bool Enabled { get; }
        public SpriteManager SpriteManager { get; set; }
        public Sprite Sprite { get; set; }

        public virtual void Update(GameTime gameTime)
        {

        }
        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {

        }
    }
}
