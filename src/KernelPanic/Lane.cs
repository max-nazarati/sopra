using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace KernelPanic
{
    [DataContract]
    internal sealed class Lane
    {
        [DataMember]
        internal EntityGraph EntityGraph { get; set; }
        internal Base Target { get; }

        [DataMember(Name = "Grid")]
        private readonly Grid mGrid;
        private readonly SpriteManager mSpriteManager;

        // private UnitSpawner mUnitSpawner;
        // private BuildingSpawner mBuildingSpawner;

        private AStar mAStar;

        private static Rectangle LaneBoundsInTiles(Grid.LaneSide laneSide) =>
            new Rectangle(laneSide == Grid.LaneSide.Left ? 0 : 32, 0, 16, 42);

        private static Rectangle LaneBoundsInPixel(Grid.LaneSide laneSide)
        {
            var bounds = LaneBoundsInTiles(laneSide);
            bounds.X *= Grid.KachelSize;
            bounds.Y *= Grid.KachelSize;
            bounds.Width *= Grid.KachelSize;
            bounds.Height *= Grid.KachelSize;
            return bounds;
        }

        internal static Rectangle LeftBounds => LaneBoundsInPixel(Grid.LaneSide.Left);
        internal static Rectangle RightBounds => LaneBoundsInPixel(Grid.LaneSide.Right);

        public Lane(Grid.LaneSide laneSide, SpriteManager sprites)
        {
            EntityGraph = new EntityGraph(sprites);
            mGrid = new Grid(LaneBoundsInTiles(laneSide), sprites, laneSide);
            Target = new Base();
            mSpriteManager = sprites;
            InitAStar(sprites);

            // mTile = Grid.CreateTile(sprites);
            // mAStar = new AStar(mGrid.CoordSystem, mGrid.CoordSystem.First(), mGrid.CoordSystem.Last());
        }

        public void Update(GameTime gameTime, InputManager inputManager)
        {
            var mouse = inputManager.TranslatedMousePosition;
            if (inputManager.KeyPressed(Keys.T))
            {
                // It seems we can't use pattern matching here because of compiler-limitations.
                var gridPoint = mGrid.GridPointFromWorldPoint(mouse, 2);
                if (gridPoint != null)
                {
                    var (position, size) = gridPoint.Value;
                    if (!EntityGraph.HasEntityAt(position))
                        EntityGraph.Add(Tower.Create(position, size, mSpriteManager));
                }
            }


            var positionProvider = new PositionProvider(mGrid, EntityGraph, mSpriteManager);
            EntityGraph.Update(positionProvider, gameTime, inputManager);
            mAStar.Update(inputManager);
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
