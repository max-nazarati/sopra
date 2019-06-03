using System;
 using Microsoft.Xna.Framework;
 using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace KernelPanic
{
    internal sealed class Lane
    {
        
        private readonly Grid mGrid;
        internal EntityGraph EntityGraph { get; }
        private readonly Base mTarget;
        private readonly SpriteManager mSpriteManager;

        // private UnitSpawner mUnitSpawner;
        // private BuildingSpawner mBuildingSpawner;

        private AStar mAStar;

        public Lane(Grid.LaneSide laneSide, SpriteManager sprites)
        {
            /*
            mGrid = laneSide == Grid.LaneSide.Left ? new Grid(content, laneSide, 
                new Rectangle(0, 0, 16, 42)) : new Grid(content, laneSide, 
                new Rectangle(32, 0, 16, 42));
            EntityGraph = entityGraph;
            mBase = new Base();

            // mTile and Init AStar is for debugging and showing the Pathfinding
            // mTile = content.Load<Texture2D>("LaneTile");
            
            */
            EntityGraph = new EntityGraph();
            mTarget = new Base();
            mGrid = new Grid(sprites, laneSide);
            mSpriteManager = sprites;
            InitAStar(sprites);

            // mTile = Grid.CreateTile(sprites);
            // mAStar = new AStar(mGrid.CoordSystem, mGrid.CoordSystem.First(), mGrid.CoordSystem.Last());

        }

        public void Update(GameTime gameTime, Matrix invertedViewMatrix)
        {
            mAStar.UpdateStartAndTarget();

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
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            mGrid.Draw(spriteBatch, gameTime);
            EntityGraph.Draw(spriteBatch, gameTime);
            mAStar.DrawPath(spriteBatch, gameTime);
        }
        
/*
        public void DrawMinimap(SpriteBatch spriteBatch, Rectangle rectangle)
        {
            
        }
        
        */

        // -------------------------- A STAR DEBUG ----------------------------------------------------------------------

        
        // private void InitAStar(SpriteManager content)
        private void InitAStar(SpriteManager sprite)
        {
            // set start and target
            Point start = new Point(0, 0);
            Point target = new Point(0, 10);

            // write every coordinate in the grid on the console
            foreach (var point in mGrid.CoordSystem)
            {
                Console.WriteLine(point);
            }
            
            mAStar = new AStar(mGrid.CoordSystem, start, target, sprite);
            mAStar.CalculatePath();
            // mAStar = new AStar(mGrid.CoordSystem, mGrid.CoordSystem[0], mGrid.CoordSystem[mGrid.CoordSystem.Count - 1]);
            // mAStar.SetStart(new Point(0, 0));
            // mAStar.SetTarget(new Point(10, 20));
            // mPath = mAStar.FindPath();
        }
        // ------------------------------------------------------------------------------------------------------------
}
}
