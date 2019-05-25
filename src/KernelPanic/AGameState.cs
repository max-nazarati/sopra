using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    abstract class AGameState
    {
        protected GameStateManager GameStateManager { get; set; }
        //public Camera2D Camera { get; protected set; }

        public bool IsOverlay { get; protected set; }
        public abstract void Draw(SpriteBatch spriteBatch, GameTime gameTime, bool isOverlay);
        public abstract void Update(GameTime gameTime, bool isOverlay);

    }
}
