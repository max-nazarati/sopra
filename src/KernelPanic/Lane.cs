using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    internal sealed class Lane
    {
        private readonly Grid mGrid;

        internal EntityGraph EntityGraph { get; }
        private Base mTarget;

        // private UnitSpawner mUnitSpawner;
        // private BuildingSpawner mBuildingSpawner;

        public Lane(Grid.LaneSide laneSide, SpriteManager sprites)
        {
            EntityGraph = new EntityGraph();
            mTarget = new Base();
            mGrid = new Grid(sprites, laneSide);
        }

        public void Update(GameTime gameTime, Matrix invertedViewMatrix)
        {
            EntityGraph.Update(gameTime, invertedViewMatrix);
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            mGrid.Draw(spriteBatch, gameTime);
            EntityGraph.Draw(spriteBatch, gameTime);
        }
        
/*
        public void DrawMinimap(SpriteBatch spriteBatch, Rectangle rectangle)
        {
            
        }
        
        */
    }
}
