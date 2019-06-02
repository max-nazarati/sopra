using System.Linq;
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
        
        private readonly AStar mAStar;
        private readonly ImageSprite mTile;

        public Lane(Grid.LaneSide laneSide, SpriteManager sprites)
        {
            EntityGraph = new EntityGraph();
            mTarget = new Base();
            mGrid = new Grid(sprites, laneSide);
            mSpriteManager = sprites;
            mTile = Grid.CreateTile(sprites);
            mAStar = new AStar(mGrid.CoordSystem, mGrid.CoordSystem.First(), mGrid.CoordSystem.Last());
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
            DrawPath(spriteBatch, gameTime);
        }
        
/*
        public void DrawMinimap(SpriteBatch spriteBatch, Rectangle rectangle)
        {
            
        }
        
        */

        private void NewStart(Point start)
        {
            mAStar.SetStart(start);
        }

        private void NewTarget(Point target)
        {
            mAStar.SetTarget(target);
        }
        
        private void DrawPath(SpriteBatch spriteBatch, GameTime gameTime)
        {
            /*
            // var path = mAStar.FindPath();
            var path = new List<Point>();
            path.Add(new Point(0, 0));
            path.Add(new Point(0, 1));
            path.Add(new Point(1, 1));
            path.Add(new Point(1, 2));
            path.Add(new Point(2, 2));
            path.Add(new Point(2, 3));
            path.Add(new Point(3, 3));
            path.Add(new Point(3, 4));
            path.Add(new Point(4, 4));

            foreach (var point in path)
            {
                DrawTile(spriteBatch, point, gameTime);
            }
            */
        }

        private void DrawTile(SpriteBatch spriteBatch, Point point, GameTime gameTime)
        {
            mTile.Position = Grid.ScreenPositionFromCoordinate(point).ToVector2();
            mTile.Draw(spriteBatch, gameTime);
        }
    }
}
