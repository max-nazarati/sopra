using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    [DataContract]
    sealed class InGameState : AGameState
    {
        [DataMember]
        private Camera2D mCamera;

        public InGameState(Camera2D camera)
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
