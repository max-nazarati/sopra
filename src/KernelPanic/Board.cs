using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    public class Board
    {
        private readonly Lane mLeftLane, mRightLane;

        public Board(ContentManager content)
        {
            mLeftLane = new Lane(Grid.LaneSide.Left, new EntityGraph(), content);
            mRightLane = new Lane(Grid.LaneSide.Right, new EntityGraph(), content);
        }

        public void DrawLane(SpriteBatch spriteBatch, Matrix viewMatrix, GameTime gameTime)
        {
            mLeftLane.Draw(spriteBatch, viewMatrix, gameTime);
            mRightLane.Draw(spriteBatch, viewMatrix, gameTime);
        }

        public void Update(GameTime gameTime)
        {
            mLeftLane.Update();
            mRightLane.Update();
        }
    /*    
        public DrawMinimap(SpriteBatch spriteBatch, Rectangle rectangle)
        {
            
        }
        */
    }
}