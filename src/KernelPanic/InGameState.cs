using System;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    [DataContract]
    internal sealed class InGameState : AGameState
    {
        [DataMember]
        private Camera2D mCamera;

        public InGameState(Camera2D camera, GameStateManager gameStateManager) : base(gameStateManager)
        {
            mCamera = camera;
        }

        public override void Update(GameTime gameTime, bool isOverlay)
        {
            throw new NotImplementedException();
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime, bool isOverlay)
        {
            throw new NotImplementedException();
        }
    }
}
