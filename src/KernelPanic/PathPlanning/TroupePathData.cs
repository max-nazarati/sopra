using System.Collections.Generic;
using System.Linq;
using KernelPanic.Data;
using KernelPanic.Entities;
using KernelPanic.Entities.Units;
using KernelPanic.Table;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic.PathPlanning
{
    internal sealed class TroupePathData
    {
        /// <summary>
        /// Tracks the buildings with one element per tile.
        /// </summary>
        internal ObstacleMatrix BuildingMatrix { get; }

        /// <summary>
        /// The <see cref="HeatMap"/> generated from <see cref="BuildingMatrix"/> used for calculating
        /// <see cref="mVectorField"/>.
        /// </summary>
        private readonly HeatMap mHeatMap;

        /// <summary>
        /// The <see cref="VectorField"/> for non-small troupes. <see cref="Thunderbird"/> uses
        /// <see cref="mThunderbirdVectorField"/>.
        /// </summary>
        private readonly VectorField mVectorField;

        /// <summary>
        /// The <see cref="VectorField"/> for small troupes.
        /// </summary>
        private readonly VectorField mSmallVectorField;

        /// <summary>
        /// The <see cref="VectorField"/> for <see cref="Thunderbird"/>.
        /// </summary>
        private readonly VectorField mThunderbirdVectorField;

        /// <summary>
        /// Used in <see cref="Update"/>. Stored globally to avoid costly re-allocations each time it is called.
        /// </summary>
        private readonly List<Point> mLargeUnits = new List<Point>();

        /// <summary>
        /// The target of all troupes.
        /// </summary>
        private readonly ICollection<Point> mTarget;

        private readonly Grid mGrid;

        internal TroupePathData(ICollection<Point> target, Grid grid, IEnumerable<Building> initialBuildings)
        {
            mTarget = target;
            mGrid = grid;

            BuildingMatrix = new ObstacleMatrix(grid);

            if (initialBuildings != null)
                BuildingMatrix.Raster(initialBuildings);

            mHeatMap = new HeatMap(BuildingMatrix);
            var smallHeatMap = new HeatMap(BuildingMatrix);

            mVectorField = new VectorField(mHeatMap);
            mSmallVectorField = new VectorField(smallHeatMap);
            mThunderbirdVectorField = VectorField.GetVectorFieldThunderbird(grid.LaneRectangle.Size, grid.LaneSide);
        }

        internal Vector2 RelativeMovement(Troupe troupe, Vector2? position = null)
        {
            var maybeTile = mGrid.TileFromWorldPoint(position ?? troupe.Sprite.Position, troupe.IsSmall ? 2 : 1);
            if (!(maybeTile is TileIndex tile))
                return Vector2.Zero;

            var size = mGrid.GetTile(tile).Size;
            var vector = RelativeMovement(tile.Rescaled(1).First(), SelectVectorField(troupe));
            return vector * size;
        }

        private static Vector2 RelativeMovement(TileIndex tile, VectorField vectorField)
        {
            var rectangle = new Rectangle(new Point(-1), new Point(2));
            return rectangle.At(vectorField[tile.ToPoint()]);
        }

        private VectorField SelectVectorField(Unit unit)
        {
            switch (unit)
            {
                case Thunderbird _:
                    return mThunderbirdVectorField;
                case Troupe troupe when troupe.IsSmall:
                    return mSmallVectorField;
                default:
                    return mVectorField;
            }
        }

        internal int? TileHeat(Point point)
        {
            return (int?) mHeatMap[point];
        }

        internal void Update(EntityGraph entityGraph, bool buildingsChanged)
        {
            if (buildingsChanged)
            {
                BreadthFirstSearch.UpdateHeatMap(mHeatMap, mTarget);
                mVectorField.Update();
            }
            
            var hadLargeUnits = mLargeUnits.Count > 0;
            UpdateLargeUnits(entityGraph);

            // If there was no change in the buildings and the number of large units is and was zero, we can skip this
            // update.
            if (!buildingsChanged && !hadLargeUnits && mLargeUnits.Count == 0)
                return;

            BreadthFirstSearch.UpdateHeatMap(mSmallVectorField.HeatMap, mTarget, mLargeUnits);
            mSmallVectorField.Update();
        }

        private void UpdateLargeUnits(EntityGraph entityGraph)
        {
            var largeUnits = entityGraph.Entities<Troupe>()
                .SelectMany(
                    troupe => troupe.IsSmall || troupe is Thunderbird
                                ? Enumerable.Empty<TileIndex>()
                                : ProjectMovement(troupe, 2),
                    (troupe, index) => index.ToPoint());
            mLargeUnits.Clear();
            mLargeUnits.AddRange(largeUnits);
            mLargeUnits.Sort(new PointComparer());
        }

        private IEnumerable<TileIndex> ProjectMovement(Entity troupe, int depth)
        {
            if (!(mGrid.TileFromWorldPoint(troupe.Sprite.Position) is TileIndex tile))
                yield break;

            yield return tile;

            while (depth-- > 0)
            {
                var movement = RelativeMovement(tile, mVectorField);
                tile.Row += (int) movement.Y;
                tile.Column += (int) movement.X;
                yield return tile;
            }
        }

        internal void Visualize(SpriteManager spriteManager, SpriteBatch spriteBatch, GameTime gameTime)
        {
            VectorField Select(DebugSettings.TroupeDataVisualization visualization)
            {
                switch (visualization)
                {
                    case DebugSettings.TroupeDataVisualization.Normal:
                        return mVectorField;
                    case DebugSettings.TroupeDataVisualization.Small:
                        return mSmallVectorField;
                    case DebugSettings.TroupeDataVisualization.Thunderbird:
                        return mThunderbirdVectorField;

                    case DebugSettings.TroupeDataVisualization.None:
                        goto default;
                    default:
                        return null;
                }
            }
            
            if (Select(DebugSettings.HeatMapVisualization) is VectorField vectorField1)
                vectorField1.HeatMap.Visualize(mGrid, spriteManager).Draw(spriteBatch, gameTime);

            if (Select(DebugSettings.VectorFieldVisualization) is VectorField vectorField2)
                vectorField2.Visualize(mGrid, spriteManager).Draw(spriteBatch, gameTime);

            if (DebugSettings.HeatMapVisualization == DebugSettings.TroupeDataVisualization.None ||
                DebugSettings.VectorFieldVisualization == DebugSettings.TroupeDataVisualization.None)
            {
                return;
            }

            var tileVisualizer = TileVisualizer.Border(1, mGrid, spriteManager);
            tileVisualizer.Append(mTarget, Color.Blue);
            tileVisualizer.Draw(spriteBatch, gameTime);
        }
    }
}