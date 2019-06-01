using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    internal class Board
    {
        private readonly Lane mLeftLane, mRightLane;

        internal Board(SpriteManager content)
        {
            mLeftLane = new Lane(Grid.LaneSide.Left, content);
            mRightLane = new Lane(Grid.LaneSide.Right, content);
        }

        internal void Draw(SpriteBatch spriteBatch, Matrix viewMatrix, GameTime gameTime)
        {
            mLeftLane.Draw(spriteBatch, viewMatrix, gameTime);
            mRightLane.Draw(spriteBatch, viewMatrix, gameTime);
        }
        
    /*    
        public DrawMinimap(SpriteBatch spriteBatch, Rectangle rectangle)
        {
            
        }
        */
    }
}