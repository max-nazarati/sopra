using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    public class Lane
    {
        private readonly Grid mGrid;

        public Lane(Grid.LaneSide laneSide, ContentManager content)
        {
            this.mGrid = laneSide == Grid.LaneSide.Left ? new Grid(content, laneSide, 
                new Rectangle(0, 0, 16, 42)) : new Grid(content, laneSide, 
                new Rectangle(15, 0, 16, 42));
        }

        public void Update()
        {
            
        }

        public void Draw(SpriteBatch spriteBatch, Matrix viewMatrix, GameTime gameTime)
        {
            mGrid.Draw(spriteBatch, viewMatrix, gameTime);
        }
        
/*
        public void DrawMinimap(SpriteBatch spriteBatch, Rectangle rectangle)
        {
            
        }
        
        */
    }
}
