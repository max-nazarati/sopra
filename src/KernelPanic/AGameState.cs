using System.Runtime.Serialization;
using KernelPanic.Camera;
using KernelPanic.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    [DataContract]
    [KnownType(typeof(InGameState))]
    [KnownType(typeof(MenuState))]
    internal abstract class AGameState
    {
        protected GameStateManager GameStateManager { get; }
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
