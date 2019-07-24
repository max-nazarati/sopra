using System.Collections.Generic;
using System.ComponentModel;
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
        /// The <see cref="VectorField"/> for non-small troupes. <see cref="Thunderbird"/> uses
        /// <see cref="mThunderbirdVectorField"/>.
        /// </summary>
        private readonly VectorField mVectorField;

        /// <summary>
        /// The <see cref="VectorField"/> for <see cref="Thunderbird"/>.
        /// </summary>
        private readonly VectorField mThunderbirdVectorField;

        /// <summary>
        /// The target of all troupes.
        /// </summary>
        internal IReadOnlyCollection<Point> Target { get; }

        private readonly Grid mGrid;

        internal TroupePathData(Lane lane, IEnumerable<Building> initialBuildings)
        {
            mGrid = lane.Grid;
            Target = lane.TargetPoints;

            BuildingMatrix = new ObstacleMatrix(mGrid);
            var spawnPoints = lane.SpawnPoints;
            foreach (var point in spawnPoints)
                BuildingMatrix[point] = true;
            if (initialBuildings != null)
                BuildingMatrix.Raster(initialBuildings);

            RelativePosition spawnDirection, targetDirection;
            switch (mGrid.LaneSide)
            {
                case Lane.Side.Left:
                    spawnDirection = RelativePosition.CenterLeft;
                    targetDirection = RelativePosition.CenterRight;
                    break;
                case Lane.Side.Right:
                    spawnDirection = RelativePosition.CenterRight;
                    targetDirection = RelativePosition.CenterLeft;
                    break;
                default:
                    throw new InvalidEnumArgumentException(nameof(mGrid.LaneSide),
                        (int) mGrid.LaneSide,
                        typeof(Lane.Side));
            }

            var heatMap = new HeatMap(BuildingMatrix);
            mVectorField = new VectorField(heatMap, mGrid.TileCutout, spawnPoints, spawnDirection, Target, targetDirection);
            mThunderbirdVectorField = VectorField.GetVectorFieldThunderbird(mGrid.LaneRectangle.Size, mGrid.LaneSide);
        }

        internal Vector2 RelativeMovement(Unit unit, Vector2? position = null)
        {
            var subTiles = unit is Troupe troupe && troupe.IsSmall ? 2 : 1;
            var maybeTile = mGrid.TileFromWorldPoint(position ?? unit.Sprite.Position, subTiles, true);
            if (!(maybeTile is TileIndex tile))
                return Vector2.Zero;

            var size = mGrid.GetTile(tile).Size;
            var vector = RelativeMovement(tile.BaseTile, SelectVectorField(unit));
            return vector * size;
        }

        private Vector2 RelativeMovement(TileIndex tile, VectorField vectorField)
        {
            var rectangle = new Rectangle(new Point(-1), new Point(2));
            return rectangle.At(vectorField[tile.ToPoint(), mGrid.LaneSide]);
        }

        private VectorField SelectVectorField(Unit unit)
        {
            return unit is Thunderbird ? mThunderbirdVectorField : mVectorField;
        }

        internal int? TileHeat(Point point, Unit unit = null)
        {
            var vectorField = unit == null ? mVectorField : SelectVectorField(unit);
            return (int?) vectorField.HeatMap[point];
        }

        internal void Update()
        {
            if (!BuildingMatrix.WasUpdated)
                return;

            BreadthFirstSearch.UpdateHeatMap(mVectorField.HeatMap, Target);
            mVectorField.Update();
        }

        internal void Visualize(SpriteManager spriteManager, SpriteBatch spriteBatch, GameTime gameTime)
        {
            VectorField Select(DebugSettings.TroupeDataVisualization visualization)
            {
                switch (visualization)
                {
                    case DebugSettings.TroupeDataVisualization.Normal:
                        return mVectorField;
                    case DebugSettings.TroupeDataVisualization.Thunderbird:
                        return mThunderbirdVectorField;

                    case DebugSettings.TroupeDataVisualization.None:
                        goto default;
                    default:
                        return null;
                }
            }
            
            if (DebugSettings.VisualizeHeatMap)
                mVectorField.HeatMap.Visualize(mGrid, spriteManager).Draw(spriteBatch, gameTime);

            if (Select(DebugSettings.VectorFieldVisualization) is VectorField vectorField2)
                vectorField2.Visualize(mGrid, spriteManager).Draw(spriteBatch, gameTime);

            if (!DebugSettings.VisualizeHeatMap && DebugSettings.VectorFieldVisualization == DebugSettings.TroupeDataVisualization.None)
            {
                return;
            }

            var tileVisualizer = TileVisualizer.Border(1, mGrid, spriteManager);
            tileVisualizer.Append(Target, Color.Blue);
            tileVisualizer.Draw(spriteBatch, gameTime);
        }
    }
}