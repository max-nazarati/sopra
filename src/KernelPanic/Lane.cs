using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    internal sealed class Lane
    {
        private readonly Grid mGrid;

        public EntityGraph EntityGraph { get; private set; }
        // private BuildingSpawner mBuildingSpawner;
        // private EntityGraph mEntityGraph;
        private Base mBase;
        // private UnitSpawner mUnitSpawner;

        public Lane(Grid.LaneSide laneSide, EntityGraph entityGraph, ContentManager content)
        {
            mGrid = laneSide == Grid.LaneSide.Left ? new Grid(content, laneSide, 
                new Rectangle(0, 0, 16, 42)) : new Grid(content, laneSide, 
                new Rectangle(32, 0, 16, 42));
            EntityGraph = entityGraph;
            mBase = new Base();
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
