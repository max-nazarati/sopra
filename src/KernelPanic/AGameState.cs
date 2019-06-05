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
        internal virtual Camera2D Camera => null;

        protected AGameState(GameStateManager gameStateManager)
        {
            GameStateGameStateManager = gameStateManager;
        }
        
        public abstract void Draw(SpriteBatch spriteBatch, GameTime gameTime);
        public abstract void Update(GameTime gameTime);
    }
}
