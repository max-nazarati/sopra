using Autofac.Util;
using KernelPanic.Camera;
using KernelPanic.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    internal abstract class AGameState : Disposable
    {
        internal GameStateManager GameStateManager { get; }
        internal virtual bool IsOverlay => false;
        internal ICamera Camera { get; }

        protected AGameState(ICamera camera, GameStateManager gameStateManager)
        {
            GameStateManager = gameStateManager;
            Camera = camera;
        }

        public abstract void Draw(SpriteBatch spriteBatch, GameTime gameTime);

        public abstract void Update(InputManager inputManager, GameTime gameTime);
    }
}
