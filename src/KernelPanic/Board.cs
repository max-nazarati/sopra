using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    public class Board
    {
        private readonly Lane mLeftLane, mRightLane;
        public SpriteManager Sprite { get; }

        public Board(SpriteManager spriteManager)
        {
            Sprite = spriteManager;
            mLeftLane = new Lane(Grid.LaneSide.Left, new EntityGraph(), Sprite);
            mRightLane = new Lane(Grid.LaneSide.Right, new EntityGraph(), Sprite);
        }

        public void DrawLane(SpriteBatch spriteBatch, Matrix viewMatrix, GameTime gameTime)
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