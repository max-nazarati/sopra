using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    internal sealed class Board
    {
        internal Lane LeftLane { get; }
        internal Lane RightLane { get; }

        internal Board(SpriteManager content)
        {
            LeftLane = new Lane(Grid.LaneSide.Left, content);
            RightLane = new Lane(Grid.LaneSide.Right, content);
        }

        internal void Update(GameTime gameTime, Matrix invertedViewMatrix)
        {
            LeftLane.Update(gameTime, invertedViewMatrix);
            RightLane.Update(gameTime, invertedViewMatrix);
        }

        internal void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            LeftLane.Draw(spriteBatch, gameTime);
            RightLane.Draw(spriteBatch, gameTime);
        }
        
    /*    
        public DrawMinimap(SpriteBatch spriteBatch, Rectangle rectangle)
        {
            
        }
        */
    }
}