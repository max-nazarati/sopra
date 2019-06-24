using KernelPanic.PathPlanning;
using KernelPanic.Table;
using Microsoft.Xna.Framework;

namespace KernelPanic.Data
{
    internal sealed class VectorField
    {
        private readonly Vector2[,] mVectorField;
        private int Height => mVectorField.GetLength(0);
        private int Width => mVectorField.GetLength(1);

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
        internal VectorField(HeatMap heatMap)
        {
            mVectorField = new Vector2[heatMap.Height, heatMap.Width];
            for (var row = 0; row < heatMap.Height; ++row)
            {
                for (var col = 0; col < heatMap.Width; ++col)
                {
                    mVectorField[row, col] = heatMap.NormalizedGradient(new Point(col, row));
                }
            }
        }

        public Vector2 this[Point point]
        {
            get
            {
                if (point.X >= Width || point.Y >= Height) return new Vector2(float.NaN);
                if (point.X < 0 || point.Y < 0) return new Vector2(float.NaN);
                return mVectorField[point.Y, point.X];
            }
        }

        internal Visualizer Visualize(Grid grid, SpriteManager spriteManager)
        {
            var visualizer = new ArrowVisualizer(grid, spriteManager);
            visualizer.Append(mVectorField);
            return visualizer;
        }
    }
}
