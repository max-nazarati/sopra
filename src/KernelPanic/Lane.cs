using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace KernelPanic
{
    internal sealed class Lane
    {
        
        private readonly Grid mGrid;
        public SpriteManager Sprite { get; }

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
            InitAStar(Sprite.ContentManager);
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
            DrawPath(spriteBatch);
        }
        
/*
        public void DrawMinimap(SpriteBatch spriteBatch, Rectangle rectangle)
        {
            
        }
        
        */

        private AStar mAStar;
        private Texture2D mTile;

        private void InitAStar(ContentManager content)
        {
            mAStar = new AStar(mGrid.CoordSystem, mGrid.CoordSystem[0], mGrid.CoordSystem[mGrid.CoordSystem.Count - 1]);
            var tile = content.Load<Texture2D>("LaneTile");
            var mTile = new ImageSprite(tile, 0, 0) {Scale = (float) 100};
        }

        private void NewStart(Point start)
        {
            mAStar.SetStart(start);
        }

        private void NewTarget(Point target)
        {
            mAStar.SetTarget(target);
        }
        
        private void DrawPath(SpriteBatch spriteBatch)
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
                DrawTile(spriteBatch, point);
            }
            */
            
        }
        private void DrawTile(SpriteBatch spriteBatch, Point point)
        {
            var pos = Grid.ScreenPositionFromCoordinate(point);
            spriteBatch.Draw(mTile, new Rectangle(pos.X, pos.Y, (100), (100)), Color.Red);
        }
     
    }
}
