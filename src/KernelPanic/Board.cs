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
            mLeftLane = new Lane(Grid.LaneSide.Left, new EntityGraph(), content);
            mRightLane = new Lane(Grid.LaneSide.Right, new EntityGraph(), content);
        }

        internal void DrawLane(SpriteBatch spriteBatch, Matrix viewMatrix, GameTime gameTime)
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