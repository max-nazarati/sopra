using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace KernelPanic
{
    internal sealed class Lane
    {
        private readonly Grid mGrid;

        internal EntityGraph EntityGraph { get; }
        private Base mTarget;
        private SpriteManager mSpriteManager;

        // private UnitSpawner mUnitSpawner;
        // private BuildingSpawner mBuildingSpawner;

        public Lane(Grid.LaneSide laneSide, SpriteManager sprites)
        {
            EntityGraph = new EntityGraph();
            mTarget = new Base();
            mGrid = new Grid(sprites, laneSide);
            mSpriteManager = sprites;
        }

        public void Update(GameTime gameTime, Matrix invertedViewMatrix)
        {
            var input = InputManager.Default;
            var mouse = Vector2.Transform(input.MousePosition.ToVector2(), invertedViewMatrix);
            if (input.KeyPressed(Keys.T))
            {
                // It seems can't use pattern matching here because of compiler-limitations.
                var gridPoint = mGrid.GridPointFromWorldPoint(mouse, 2);
                if (gridPoint != null)
                {
                    var (position, size) = gridPoint.Value;
                    if (!EntityGraph.HasEntityAt(position))
                        EntityGraph.Add(Tower.Create(position, size, mSpriteManager));
                }
            }

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
