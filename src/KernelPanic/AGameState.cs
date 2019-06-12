using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    [DataContract]
    [KnownType(typeof(InGameState))]
    [KnownType(typeof(MenuState))]
    internal abstract class AGameState
    {
        protected GameStateManager GameStateGameStateManager { get; }
        internal virtual bool IsOverlay => false;
        internal ICamera Camera { get; set; }

        protected AGameState(ICamera camera, GameStateManager gameStateManager)
        {
            GameStateGameStateManager = gameStateManager;
            Camera = camera;
        }
        
        public abstract void Draw(SpriteBatch spriteBatch, GameTime gameTime);
        public abstract void Update(InputManager inputManager, GameTime gameTime);
    }
}
