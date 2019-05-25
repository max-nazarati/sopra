using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    abstract class AGameState
    {
        public GameStateManager GameStateManager { get; protected set; }
        public Camera2D Camera { get; protected set; }

        public bool IsOverlay { get; protected set; }
        public AGameState()
        {

        }
        public abstract void Draw(SpriteBatch spriteBatch, GameTime gameTime, bool isOverlay);
        public abstract void Update(GameTime gameTime, bool isOverlay);

    }
}
