using KernelPanic.PathPlanning;
using KernelPanic.Table;
using Microsoft.Xna.Framework;

namespace KernelPanic.Data
{
    internal sealed class VectorField
    {
        internal HeatMap HeatMap { get; }
    
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
            HeatMap = heatMap;
            mVectorField = new Vector2[heatMap.Height, heatMap.Width];
            for (var row = 0; row < heatMap.Height; ++row)
            {
                for (var col = 0; col < heatMap.Width; ++col)
                {
                    mVectorField[row, col] = heatMap.Gradient(new Point(col, row));
                }
            }
        }

        private VectorField(Vector2[,] vectorField)
        {
            mVectorField = vectorField;
        }
        
        internal static VectorField GetVectorFieldThunderbird(VectorField vectorField, Lane.Side laneSide)
        {
            var left = new Vector2(-1, 0);
            var right = new Vector2(1, 0);
            var up = new Vector2(0, -1);
            var down = new Vector2(0, 1);

            var laneWidth = 10;
            var thunderBirdField = new Vector2[vectorField.Height, vectorField.Width];
            for (var row = 0; row < vectorField.Height; ++row)
            {
                for (var col = 0; col < vectorField.Width; ++col)
                {
                    if (laneSide == Lane.Side.Right)
                    {
                        if (row < laneWidth && col <= 1)
                        {
                            // go upwards to hit the base tile
                            thunderBirdField[row, col] = up;
                        }
                        else if (row + col < 18)
                        {
                            // distance to left, top
                            thunderBirdField[row, col] = left;
                        }

                        else if (vectorField.Height - row + col < 18)
                        {
                            // distance to left, bottom
                            thunderBirdField[row, col] = right;
                        }
                        else
                        {
                            thunderBirdField[row, col] = up;
                        }
                    }

                    if (laneSide == Lane.Side.Left)
                    {
                        if (row > vectorField.Height - laneWidth && col >= vectorField.Width - 1)
                        {
                            // go downwards to hit the base tile
                            thunderBirdField[row, col] = down;
                        }
                        else if (row + (vectorField.Width - 1) - col < 18)
                        {
                            // distance to right, top
                            thunderBirdField[row, col] = left;
                        }

                        else if (vectorField.Height - row + (vectorField.Width - 1) - col < 18)
                        {
                            // distance to right, bottom
                            thunderBirdField[row, col] = right;
                        }

                        else
                        {
                            thunderBirdField[row, col] = down;
                        }
                        
                    }

                }
            }
            var res = new VectorField(thunderBirdField);
            return res;
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
