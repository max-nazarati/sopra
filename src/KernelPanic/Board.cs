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

        internal void Update(GameTime gameTime, Matrix invertedViewMatrix)
        {
            mLeftLane.Update(gameTime, invertedViewMatrix);
            mRightLane.Update(gameTime, invertedViewMatrix);
        }
        
        internal void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            mLeftLane.Draw(spriteBatch, gameTime);
            mRightLane.Draw(spriteBatch, gameTime);
        }
        
    /*    
        public DrawMinimap(SpriteBatch spriteBatch, Rectangle rectangle)
        {
            
        }
        */
    }
}