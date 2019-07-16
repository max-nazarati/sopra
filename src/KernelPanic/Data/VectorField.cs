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

        internal int Width => mRelativeField.GetLength(1);
        internal int Height => mRelativeField.GetLength(0);

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
        internal VectorField(HeatMap heatMap, IEnumerable<Point> target)
        {
            HeatMap = heatMap;
            mRelativeField = new RelativePosition[heatMap.Height, heatMap.Width];

            foreach (var (column, row) in target)
                mRelativeField[row, column] = RelativePosition.CenterRight;
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
        
        internal static VectorField GetVectorFieldThunderbird(Point size, Lane.Side laneSide)
        {
            const RelativePosition left = RelativePosition.CenterLeft;
            const RelativePosition right = RelativePosition.CenterRight;
            const RelativePosition up = RelativePosition.CenterTop;
            const RelativePosition down = RelativePosition.CenterBottom;
            var thunderBirdField = new RelativePosition[size.Y, size.X];

            for (var row = 0; row < size.Y; ++row)
            {
                for (var col = 0; col < size.X; ++col)
                {
                    switch (laneSide)
                    {
                        case Lane.Side.Right when row + col < 18:
                            // distance to left, top
                            thunderBirdField[row, col] = left;
                            break;
                        case Lane.Side.Right when size.Y - row + col < 18:
                            // distance to left, bottom
                            thunderBirdField[row, col] = right;
                            break;
                        case Lane.Side.Right:
                            thunderBirdField[row, col] = up;
                            break;
                        case Lane.Side.Left when row + (size.X - 1) - col < 18:
                            // distance to right, top
                            thunderBirdField[row, col] = left;
                            break;
                        case Lane.Side.Left when size.Y - row + (size.X - 1) - col < 18:
                            // distance to right, bottom
                            thunderBirdField[row, col] = right;
                            break;
                        case Lane.Side.Left:
                            thunderBirdField[row, col] = down;
                            break;
                        default:
                            throw new InvalidEnumArgumentException(nameof(laneSide), (int)laneSide, typeof(Lane.Side));
                    }
                }
            }
            var res = new VectorField(thunderBirdField);
            return res;
        }

        public RelativePosition this[Point point] => mRelativeField[point.Y, point.X];

        internal Visualizer Visualize(Grid grid, SpriteManager spriteManager)
        {
            var visualizer = new ArrowVisualizer(HeatMap?.ObstacleMatrix.SubTileCount ?? 1, grid, spriteManager);
            visualizer.Append(mRelativeField);
            return visualizer;
        }
    }
}
