using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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

            // mTile and Init AStar is for debugging and showing the Pathfinding
            // mTile = content.Load<Texture2D>("LaneTile");
            InitAStar(content);
        }

        public void Update()
        {
            mAStar.UpdateStartAndTarget();
            // mPath = mAStar.FindPath();
        }

        public void Draw(SpriteBatch spriteBatch, Matrix viewMatrix, GameTime gameTime)
        {
            mGrid.Draw(spriteBatch, viewMatrix, gameTime);
            // mPath = mAStar.FindPath();
            // DrawPath(spriteBatch);
            mAStar.DrawPath(spriteBatch);

        }
        
/*
        public void DrawMinimap(SpriteBatch spriteBatch, Rectangle rectangle)
        {
            
        }
        
        */

        // ------------------------------------------------------------------------------------------------------------
        private AStar mAStar;
        /*
        private readonly Texture2D mTile;
        private List<Point> mPath;
        */

        private void InitAStar(ContentManager content)
        {
            // set start and target
            Point start = new Point(0, 0);
            Point target = new Point(1, 10);

            // write every coordinate in the grid on the console
            foreach (var point in mGrid.CoordSystem)
            {
                Console.WriteLine(point);
            }
            
            mAStar = new AStar(mGrid.CoordSystem, start, target, content);
            mAStar.CalculatePath();
            // mAStar = new AStar(mGrid.CoordSystem, mGrid.CoordSystem[0], mGrid.CoordSystem[mGrid.CoordSystem.Count - 1]);
            // mAStar.SetStart(new Point(0, 0));
            // mAStar.SetTarget(new Point(10, 20));
            // mPath = mAStar.FindPath();
        }

        /*
        private void DrawPath(SpriteBatch spriteBatch)
        {
            foreach (var point in mPath)
            {
                DrawTile(spriteBatch, point);
            }

        }
        private void DrawTile(SpriteBatch spriteBatch, Point point)
        {
            var pos = Grid.ScreenPositionFromCoordinate(point);
            spriteBatch.Draw(mTile, new Rectangle(pos.X, pos.Y, (100), (100)), Color.Red);
        }
        */
        // ------------------------------------------------------------------------------------------------------------
    }
}
