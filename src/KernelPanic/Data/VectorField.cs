using System.Collections.Generic;
using System.ComponentModel;
using KernelPanic.PathPlanning;
using KernelPanic.Table;
using Microsoft.Xna.Framework;

namespace KernelPanic.Data
{
    internal sealed class VectorField
    {
        internal HeatMap HeatMap { get; }

        private readonly RelativePosition[,] mRelativeField;

        /// <summary>
        /// Creates a <see cref="VectorField"/> from a <see cref="HeatMap"/>.
        /// </summary>
        /// <example>
        /// Let
        /// <code>
        /// [ 4][ 3][ 2][ 2]
        /// [ 3][-1][ 1][ 1]
        /// [ 2][ 1][ 0][ 0]
        /// [ 2][ 1][ 0][ 0]
        /// </code>
        /// denote the heat map, the the resulting vector field is
        /// <code>
        /// [(1,1)][(1,0)][(0,1)][(0,1)]
        /// [(0,1)][ None][(0,1)][(0,1)]
        /// [(1,0)][(1,0)][ None][ None]
        /// [(1,0)][(1,0)][ None][ None]
        /// </code>
        /// with each square additionally normalized.
        /// </example>
        /// <param name="heatMap">The heat map.</param>
        /// <param name="spawn"></param>
        /// <param name="spawnDirection"></param>
        /// <param name="target"></param>
        /// <param name="targetDirection"></param>
        internal VectorField(HeatMap heatMap, IEnumerable<Point> spawn, RelativePosition spawnDirection, IEnumerable<Point> target, RelativePosition targetDirection)
        {
            HeatMap = heatMap;
            mRelativeField = new RelativePosition[heatMap.Height, heatMap.Width];

            foreach (var (column, row) in spawn)
                mRelativeField[row, column] = spawnDirection;
            foreach (var (column, row) in target)
                mRelativeField[row, column] = targetDirection;
        }

        private VectorField(RelativePosition[,] vectorField)
        {
            mRelativeField = vectorField;
        }

        internal void Update()
        {
            for (var row = 0; row < HeatMap.Height; ++row)
            {
                for (var col = 0; col < HeatMap.Width; ++col)
                {
                    var relative = HeatMap.Gradient(new Point(col, row));
                    if (relative != RelativePosition.Center)
                        mRelativeField[row, col] = relative;
                }
            }
        }

        #region Thunderbird's vector field

        internal static VectorField GetVectorFieldThunderbird(Point size, Lane.Side laneSide)
        {
            const RelativePosition topMovement = RelativePosition.CenterLeft;
            const RelativePosition botMovement = RelativePosition.CenterRight;
            var thunderBirdField = new RelativePosition[size.Y, size.X];

            // Fill top and bottom.
            for (var row = 0; row < Grid.LaneWidthInTiles; ++row)
            {
                for (var col = 0; col < size.X; ++col)
                {
                    thunderBirdField[row, col] = topMovement;
                    thunderBirdField[size.Y - 1 - row, col] = botMovement;
                }
            }

            int middleOffset;
            RelativePosition verticalMovement;
            switch (laneSide)
            {
                case Lane.Side.Left:
                    middleOffset = 0;
                    verticalMovement = RelativePosition.CenterBottom;
                    FillLeftTriangles(size, verticalMovement, thunderBirdField);
                    break;
                case Lane.Side.Right:
                    middleOffset = size.X - Grid.LaneWidthInTiles;
                    verticalMovement = RelativePosition.CenterTop;
                    FillRightTriangles(size, verticalMovement, thunderBirdField);
                    break;
                default:
                    throw new InvalidEnumArgumentException(nameof(laneSide), (int) laneSide, typeof(Lane.Side));
            }

            // Fill the vertical part.
            for (var row = Grid.LaneWidthInTiles; row < size.Y - Grid.LaneWidthInTiles; ++row)
            {
                for (var col = 0; col < Grid.LaneWidthInTiles; ++col)
                {
                    thunderBirdField[row, col + middleOffset] = verticalMovement;
                }
            }

            return new VectorField(thunderBirdField);
        }

        private static void FillLeftTriangles(Point size, RelativePosition movement, RelativePosition[,] field)
        {
            for (var row = 0; row < Grid.LaneWidthInTiles; ++row)
            {
                for (var col = 0; col <= row; ++col)
                {
                    field[row, col] = movement;
                    field[size.Y - 2 - row, col] = movement;
                }
            }
        }

        private static void FillRightTriangles(Point size, RelativePosition movement, RelativePosition[,] field)
        {
            for (var row = 0; row < Grid.LaneWidthInTiles; ++row)
            {
                for (var col = 0; col <= row; ++col)
                {
                    field[row + 1, size.X - 1 - col] = movement;
                    field[size.Y - 1 - row, size.X - 1 - col] = movement;
                }
            }
        }

        #endregion

        public RelativePosition this[Point point] => mRelativeField[point.Y, point.X];

        internal Visualizer Visualize(Grid grid, SpriteManager spriteManager)
        {
            var visualizer = new ArrowVisualizer(HeatMap?.ObstacleMatrix.SubTileCount ?? 1, grid, spriteManager);
            visualizer.Append(mRelativeField);
            return visualizer;
        }
    }
}
