using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace KernelPanic
{
    internal sealed class Lane
    {
        internal EntityGraph EntityGraph { get; }
        internal Base Target { get; }

        private readonly Grid mGrid;
        private readonly SpriteManager mSpriteManager;

        // private UnitSpawner mUnitSpawner;
        // private BuildingSpawner mBuildingSpawner;

        private AStar mAStar;

        public Lane(Grid.LaneSide laneSide, SpriteManager sprites)
        {
            EntityGraph = new EntityGraph(sprites);
            mGrid = new Grid(sprites, laneSide);
            Target = new Base();
            mSpriteManager = sprites;
            InitAStar(sprites);

            // mTile = Grid.CreateTile(sprites);
            // mAStar = new AStar(mGrid.CoordSystem, mGrid.CoordSystem.First(), mGrid.CoordSystem.Last());
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

            var positionProvider = new PositionProvider(mGrid, EntityGraph);
            EntityGraph.Update(positionProvider, gameTime, invertedViewMatrix);
            
            mAStar.Update();
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            mGrid.Draw(spriteBatch, gameTime);
            mAStar.Draw(spriteBatch, gameTime);
            EntityGraph.Draw(spriteBatch, gameTime);
        }
        
        /*
        public void DrawMinimap(SpriteBatch spriteBatch, Rectangle rectangle)
        {    
        }
        */

        
        // -------------------------- A STAR DEBUG ----------------------------------------------------------------------
        private void InitAStar(SpriteManager sprite, int obstacleEnv=2)
        {
            // set start and target
            Point start = new Point(0, 0);
            Point target = new Point(0, 15);
            
            mAStar = new AStar(mGrid.CoordSystem, start, target, sprite);
            mAStar.ChangeObstacleEnvironment(obstacleEnv);
       
            mAStar.CalculatePath();
        }
        // ------------------------------------------------------------------------------------------------------------
}
}
