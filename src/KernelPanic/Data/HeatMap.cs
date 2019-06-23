using KernelPanic.PathPlanning;
using KernelPanic.Table;
using Microsoft.Xna.Framework;

namespace KernelPanic.Data
{
    internal sealed class HeatMap
    {
        private readonly float[,] mMap;

        public int Width => mMap.GetLength(1);
        public int Height => mMap.GetLength(0);

        /// <summary>
        /// Heatmap which displays the distance of each grid to
        /// the goal grid (heat = 0). We denote heat = -1 for a
        /// non-walkable square, e.g. for the initial lane:
        /// [13][12][11][12][13][14]
        /// [12][11][10][11][12][13]
        /// [11][10][ 9][10][11][12]
        /// [10][ 9][ 8][-1][-1][-1]
        /// [ 9][ 8][ 7][-1][-1][-1]
        /// [ 8][ 7][ 6][-1][-1][-1]
        /// [ 7][ 6][ 5][-1][-1][-1]
        /// [ 6][ 5][ 4][-1][-1][-1]
        /// [ 5][ 4][ 3][ 2][ 1][ 1]
        /// [ 4][ 3][ 2][ 1][ 0][ 0]
        /// [ 4][ 3][ 2][ 1][ 0][ 0]
        /// </summary>
        /// <param name="width">number of columns</param>
        /// <param name="height">number of rows</param>
        public HeatMap(int width, int height)
        {
            mMap = new float[height, width];
        }

        /// <summary>
        /// Checks if this grid point is not blocked and therefore walkable.
        /// Blocked grid points have a value of less than one.
        /// </summary>
        /// <param name="point">the point (x, y)</param>
        /// <returns><c>false</c> if it is blocked, <c>true</c> otherwise.</returns>
        public bool IsWalkable(Point point) => 0 <= this[point];

        /// <summary>
        /// Calculates the gradient, given point (x, y) i,e.:
        /// grad(x, y) = (h_y(x-1)-h_y(x+1), h_x(y-1)-h_x(y+1))
        /// where h_y(x-1) = heat[x-1, y] if defined else heat[x, y]
        /// and   h_x(y-1) = heat[x, y-1] if defined else heat[x, y]
        /// e.g.:
        /// [ 1][ 2][ 3]
        /// [ 4][ 5][ 6]
        /// [ 7][ 8][ 9]
        /// then grad(1, 1) = (4-6, 2-8) = (-2, -6)
        /// </summary>
        /// <param name="point">(x, y) point of the gradient to be computed</param>
        /// <returns></returns>
        private Vector2 Gradient(Point point)
        {
            if (!(this[point] is float hereHeat))
                return new Vector2(float.NaN);

            float LookupHeat(int xOffset, int yOffset)
            {
                var maybeHeat = this[point + new Point(xOffset, yOffset)];
                return maybeHeat is float heat2 && heat2 >= 0 ? heat2 : hereHeat;
            }

            var heatUp = LookupHeat(0, -1);
            var heatDown = LookupHeat(0, 1);
            var heatLeft = LookupHeat(-1, 0);
            var heatRight = LookupHeat(1, 0);

            var gradient = new Vector2(heatLeft - heatRight, heatUp - heatDown);
            return AdjustGradientToWalls(point, gradient);
        }

        private Vector2 AdjustGradientToWalls(Point point, Vector2 grad)
        {
            Vector2 adjustedGrad = grad;
            if (grad.X > 0 && !IsWalkable(new Point(point.X + 1, point.Y))) adjustedGrad = new Vector2(0, grad.Y);
            if (grad.X < 0 && !IsWalkable(new Point(point.X - 1, point.Y))) adjustedGrad = new Vector2(0, grad.Y);
            if (grad.Y > 0 && !IsWalkable(new Point(point.X, point.Y + 1))) adjustedGrad = new Vector2(grad.X, 0);
            if (grad.Y < 0 && !IsWalkable(new Point(point.X, point.Y - 1))) adjustedGrad = new Vector2(grad.X, 0);
             
            return adjustedGrad;
        }

        public Vector2 NormalizedGradient(Point point)
        {
            Vector2 grad = Gradient(point);
            if (grad != Vector2.Zero) grad.Normalize();
            return grad;
        }

        internal void Block(Point point) => this[point] = -1;

        internal void SetCost(Point point, float cost) => this[point] = cost;

        private float? this[Point point]
        {
            get => Contains(point) ? (float?) mMap[point.Y, point.X] : null;
            set
            {
                if (Contains(point) && value is float val)
                    mMap[point.Y, point.X] = val;
            }
        }

        private bool Contains(Point point) =>
            0 <= point.X && point.X < Width && 0 <= point.Y && point.Y < Height;

        public override string ToString()
        {
            string result = "";
            for (int y=0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    result += mMap[y, x].ToString();
                    switch (mMap[y, x].ToString().Length)
                    {
                        case 1:
                            result += "   ";
                            break;
                        case 2:
                            result += "  ";
                            break;
                        case 3:
                            result += " ";
                            break;
                    }
                }
                if (y != Height - 1) result += "\n";
            }

            return result;
        }

        internal Visualizer CreateVisualization(Grid grid, SpriteManager spriteManager, bool drawBorderOnly=true)
        {
            var visualization = new Visualizer(grid, spriteManager, drawBorderOnly);
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Color color;
                    if ((int)mMap[y, x] == 0)
                    {
                        color = Color.Red;
                    }
                    else if (IsWalkable(new Point(x, y)))
                    {
                        color = new Color(255 - 4*(int)mMap[y, x], 255 - 4 * (int)mMap[y, x], 0);
                    }
                    else
                    {
                        color = Color.White;
                    }
                    visualization.Append(new Point[] {new Point(x, y)}, color);
                }
            }

            return visualization;
        }
    }

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
        /// [(1, 0)][(1, 0)][ None][ None]
        /// [(1, 0)][(1, 0)][ None][ None]
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
    }
}
