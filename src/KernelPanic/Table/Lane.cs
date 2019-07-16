using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using KernelPanic.Entities;
using KernelPanic.Entities.Buildings;
using KernelPanic.Input;
using KernelPanic.PathPlanning;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace KernelPanic.Table
{
    internal sealed class Lane
    {
        /// <summary>
        /// Left and Right lane
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enum Side
        {
            /// <summary>
            /// Describes the lane which is shaped like an opening bracket ›[‹.
            /// </summary>
            Left,

            /// <summary>
            /// Describes the lane which is shaped like an closing bracket ›]‹.
            /// </summary>
            Right
        }

        #region Properties

        private readonly SpriteManager mSpriteManager;

        [JsonProperty]
        internal Base Target { get; /* required for deserialization */ private set; }

        [JsonProperty]
        private readonly Side mLaneSide;

        internal EntityGraph EntityGraph { get; private set; }
        internal UnitSpawner UnitSpawner { get; private set; }
        internal BuildingSpawner BuildingSpawner { get; private set; }
        internal Grid Grid { get; private set; }

        private TroupePathData mTroupeData;

        #endregion

        #region Size, position and bound functions

        private static Rectangle LaneBoundsInTiles(Side laneSide) =>
            new Rectangle(laneSide == Side.Left ? 0 : 30, 0, 18, 42);

        private static Rectangle LaneBoundsInPixel(Side laneSide)
        {
            var bounds = LaneBoundsInTiles(laneSide);
            bounds.X *= Grid.KachelSize;
            bounds.Y *= Grid.KachelSize;
            bounds.Width *= Grid.KachelSize;
            bounds.Height *= Grid.KachelSize;
            return bounds;
        }

        internal static Rectangle LeftBounds => LaneBoundsInPixel(Side.Left);
        internal static Rectangle RightBounds => LaneBoundsInPixel(Side.Right);

        internal bool Contains(Vector2 point) => Grid.Contains(point);

        private List<LaneBorder> LaneBorders
        {
            get
            {
                var borders = new List<LaneBorder>(8);
                borders.AddRange(LaneBorder.Borders(Grid.Bounds, Grid.KachelSize, true));
                borders.AddRange(LaneBorder.Borders(Grid.PixelCutout, Grid.KachelSize, false));
                return borders;
            }
        }

        #endregion

        #region Constructors

        public Lane(Side laneSide, SpriteManager sprites) : this(sprites)
        {
            mLaneSide = laneSide;
            Target = new Base(LaneBoundsInTiles(laneSide).Size, laneSide);
            Initialize();
        }

        internal Lane(SpriteManager sprites)
        {
            mSpriteManager = sprites;
        }

        /// <summary>
        /// Performs the initialization common to deserialized lanes and lanes created at runtime.
        /// </summary>
        /// <param name="entities">Entities to be added to the <see cref="EntityGraph"/>.</param>
        private void Initialize(IReadOnlyCollection<Entity> entities = null)
        {
            Grid = new Grid(LaneBoundsInTiles(mLaneSide), mSpriteManager, mLaneSide);
            EntityGraph = new EntityGraph(LaneBorders);
            UnitSpawner = new UnitSpawner(Grid, EntityGraph);
            BuildingSpawner = new BuildingSpawner(Grid, EntityGraph.Add, 
                entities?.OfType<Building>().Where(building => building.State == BuildingState.Inactive));

            if (entities?.Count > 0)
            {
                EntityGraph.Add(entities);
            }

            mTroupeData = new TroupePathData(Target.HitBox, Grid, entities?.OfType<Building>());
            mTroupeData.Update(EntityGraph, true);
        }

        #endregion

        #region Update

        internal void Update(GameTime gameTime, InputManager inputManager, Owner owner)
        {
            var buildingsChanged = false;
            
            // Block new buildings in the obstacle matrix.
            foreach (var position in BuildingSpawner.NewBuildings())
            {
                buildingsChanged = true;
                mTroupeData.BuildingMatrix[position] = true;
            }

            // Unblock sold buildings in the obstacle matrix.
            IEnumerable<Building> buildings = EntityGraph.Entities<Building>();
            foreach (var building in buildings.Where(building => building.WantsRemoval))
            {
                buildingsChanged = true;
                Vector2 position = building.Sprite.Position;
                TileIndex tileIndex = Grid.TileFromWorldPoint(position).GetValueOrDefault();
                mTroupeData.BuildingMatrix[tileIndex.ToPoint()] = false;
            }

            mTroupeData.Update(EntityGraph, buildingsChanged);

            var positionProvider = new PositionProvider(Target, owner, Grid, EntityGraph, mTroupeData, mSpriteManager);
            EntityGraph.Update(positionProvider, gameTime, inputManager);
            UnitSpawner.Update(gameTime);
            BuildingSpawner.Update(positionProvider);
        }

        #endregion

        #region Drawing

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Grid.Draw(spriteBatch, gameTime);
            mTroupeData.Visualize(mSpriteManager, spriteBatch, gameTime);
            EntityGraph.Draw(spriteBatch, gameTime);
        }

        #endregion

        #region Serialization

        /// <summary>
        /// Stores/receives the entities during serialization. Don't use it outside of this!
        /// </summary>
        [JsonProperty]
        internal List<Entity> mEntitiesSerializing;

        [OnSerializing]
        private void BeforeSerialization(StreamingContext context)
        {
            // Store the current entities.
            mEntitiesSerializing = new List<Entity>(EntityGraph.AllEntities);
        }

        [OnSerialized]
        private void AfterSerialization(StreamingContext context)
        {
            // Reset this secondary storage.
            mEntitiesSerializing = null;
        }

        [OnDeserialized]
        private void AfterDeserialization(StreamingContext context)
        {
            Initialize(mEntitiesSerializing);
            mEntitiesSerializing = null;
        }

        #endregion
    }
}
