using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    internal sealed class Lane
    {
        
        private readonly Grid mGrid;
        public SpriteManager Sprite { get; }

        public EntityGraph EntityGraph { get; private set; }
        // private BuildingSpawner mBuildingSpawner;
        // private EntityGraph mEntityGraph;
        private Base mBase;
        // private UnitSpawner mUnitSpawner;

        public Lane(Grid.LaneSide laneSide, EntityGraph entityGraph, SpriteManager spriteManager)
        {
            Sprite = spriteManager;
            mGrid = laneSide == Grid.LaneSide.Left ? new Grid(Sprite, laneSide, 
                new Rectangle(0, 0, 16, 42)) : new Grid(Sprite, laneSide, 
                new Rectangle(32, 0, 16, 42));
            EntityGraph = entityGraph;
            mBase = new Base();
            InitAStar(content);
        }

        public void Update()
        {
        }

        public void Draw(SpriteBatch spriteBatch, Matrix viewMatrix, GameTime gameTime)
        {
            mGrid.Draw(spriteBatch, viewMatrix, gameTime);
            
            DrawPath(spriteBatch);
        }
        
/*
        public void DrawMinimap(SpriteBatch spriteBatch, Rectangle rectangle)
        {
            
        }
        
        */

        // ------------------------------------------------------------------------------------------------------------
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
        

        // ------------------------------------------------------------------------------------------------------------
    }
}
