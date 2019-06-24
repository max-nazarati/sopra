using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        internal Vector2 Gradient(Point point)
        {
            if (!(this[point] is float heatHere) || heatHere < 0)
                return new Vector2(float.NaN);

            float LookupHeat(int xOffset, int yOffset)
            {
                var maybeHeat = this[point + new Point(xOffset, yOffset)];
                return maybeHeat is float heat2 && heat2 >= 0 ? heat2 : heatHere;
            }

            var heatUp = LookupHeat(0, -1);
            var heatDown = LookupHeat(0, 1);
            var heatLeft = LookupHeat(-1, 0);
            var heatRight = LookupHeat(1, 0);

            var gradient = new Vector2(heatLeft - heatRight, heatUp - heatDown);
            RoundToOctant(ref gradient);
            AdjustGradientToWalls(point, ref gradient);
            return gradient;
        }

        /// <summary>
        /// Adjusts <paramref name="gradient"/> so that
        /// <list type="number">
        /// <item>
        /// <description>it doesn't point in the direction of a blocked tile</description>
        /// </item>
        /// <item>
        /// <description>we don't get zero-vectors after ensuring 1).</description>
        /// </item>
        /// </list>
        /// The second point can occur when placing a tower and both ways around it have the same distance. This is
        /// detected by making a change because of 1) in one dimension and the other dimension already being zero. To
        /// handle this case we just set the second dimension to one.
        /// </summary>
        /// <param name="point">The tile at which <paramref name="gradient"/> is.</param>
        /// <param name="gradient">The gradient to adjust.</param>
        private void AdjustGradientToWalls(Point point, ref Vector2 gradient)
        {
            bool Blocked(int xOffset, int yOffset) =>
                !IsWalkable(new Point(point.X + xOffset, point.Y + yOffset));

            var blockedX = gradient.X < 0 && Blocked(-1, 0) || gradient.X > 0 && Blocked(1, 0);
            var blockedY = gradient.Y < 0 && Blocked(0, -1) || gradient.Y > 0 && Blocked(0, 1);

            if (blockedX)
            {
                gradient.X = 0;
                if (Math.Abs(gradient.Y) < 0.0001)
                    gradient.Y = 1;
            } 
            else if (blockedY)
            {
                gradient.Y = 0;
                if (Math.Abs(gradient.X) < 0.0001)
                    gradient.X = 1;
            }
        }

        /// <summary>
        /// Modifies <paramref name="gradient"/> so that it points at an angle which is a multiple of 45° (π/4).
        /// </summary>
        /// <param name="gradient">The gradient to adjust.</param>
        private static void RoundToOctant(ref Vector2 gradient)
        {
            const float octant = (float) (2 * Math.PI / 8);
            var index = 0;
            var angle = gradient.Angle();

            if (angle < 0)
                angle += 2 * (float) Math.PI;

            angle -= octant / 2;
            if (angle > 0)
            {
                ++index;
                while (angle > octant)
                {
                    angle -= octant;
                    index++;
                }
            }

            angle = index * octant;
            gradient.X = (float) Math.Cos(angle);
            gradient.Y = (float) Math.Sin(angle);
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

        #region Visualization

        public override string ToString()
        {
            var builder = new StringBuilder();
            foreach (var ((col, row), value) in AllPoints)
            {
                if (col == 0 && row > 0)
                    builder.AppendLine();
                if (col == 0)
                    builder.Append($"[{row:D2}]  ");
                if (value < 100 && value >= 0)
                    builder.Append(' ');
                if (value < 10)
                    builder.Append(' ');
                builder.Append(value).Append(' ');
            }
            return builder.ToString();
        }

        internal Visualizer Visualize(Grid grid, SpriteManager spriteManager)
        {
            var visualization = TileVisualizer.FullTile(grid, spriteManager);
            foreach (var (point, value) in AllPoints)
            {
                Color color;
                if (value == 0)
                    color = Color.Red;
                else if (IsWalkable(point))
                    color = new Color(255 - 4 * value, 255 - 4 * value, 0);
                else
                    color = Color.White;
                    
                visualization.Append(new [] {point}, color);
            }
            return visualization;
        }

        private IEnumerable<(Point Point, int Value)> AllPoints
        {
            get
            {
                var width = Width;
                return Enumerable.Range(0, Height).SelectMany(row =>
                    Enumerable.Range(0, width).Select(col => (new Point(col, row), (int) mMap[row, col])));
            }
        }

        #endregion
    }
}
