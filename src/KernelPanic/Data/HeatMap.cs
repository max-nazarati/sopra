﻿using System;
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
        internal ObstacleMatrix ObstacleMatrix { get; }

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
        /// <param name="obstacleMatrix"></param>
        public HeatMap(ObstacleMatrix obstacleMatrix)
        {
            ObstacleMatrix = obstacleMatrix;
            mMap = new float[obstacleMatrix.Rows, obstacleMatrix.Columns];
        }

        internal void SetZero()
        {
            for (var i = 0; i < mMap.GetLength(0); ++i)
            {
                for (var j = 0; j < mMap.GetLength(1); ++j)
                {
                    mMap[i, j] = default(float);
                }
            }
        }

        /// <summary>
        /// Checks if this grid point is not blocked and therefore walkable.
        /// Blocked grid points have a value of less than one.
        /// </summary>
        /// <param name="point">the point (x, y)</param>
        /// <returns><c>false</c> if it is blocked, <c>true</c> otherwise.</returns>
        private bool IsWalkable(Point point) => !ObstacleMatrix[point];

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
        internal RelativePosition Gradient(Point point)
        {
            if (!(this[point] is float heatHere) || heatHere <= 0)
                return RelativePosition.Center;

            float LookupHeat(int xOffset, int yOffset)
            {
                var offsetPoint = point + new Point(xOffset, yOffset);
                var maybeHeat = this[offsetPoint];
                return IsWalkable(offsetPoint) && maybeHeat is float heat ? heat : heatHere;
            }

            var heatUp = LookupHeat(0, -1);
            var heatDown = LookupHeat(0, 1);
            var heatLeft = LookupHeat(-1, 0);
            var heatRight = LookupHeat(1, 0);

            var gradient = new Vector2(heatLeft - heatRight, heatUp - heatDown);
            AdjustGradientToWalls(point, ref gradient);
            return RoundToOctant2(gradient);
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

            var blockedX =
                    gradient.X < 0 && Blocked(-1, 0) ||
                    gradient.X > 0 && Blocked(1, 0);
            if (blockedX)
            {
                gradient.X = 0;
                if (Math.Abs(gradient.Y) < 0.0001)
                    gradient.Y = 1;
                return;
            } 
            
            var blockedY =
                    gradient.Y < 0 && Blocked(0, -1) ||
                    gradient.Y > 0 && Blocked(0, 1);
            if (blockedY)
            {
                gradient.Y = 0;
                if (Math.Abs(gradient.X) < 0.0001)
                    gradient.X = 1;
                return;
            }

            
            var blockedDiagonal =
                    gradient.X < 0 && gradient.Y < 0 && Blocked(-1, -1) ||
                    gradient.X < 0 && gradient.Y > 0 && Blocked(-1, 1) ||
                    gradient.X > 0 && gradient.Y < 0 && Blocked(1, -1) ||
                    gradient.X > 0 && gradient.Y > 0 && Blocked(1, 1);
            if (blockedDiagonal)
            { 
                // Set one coordinate to zero. Which one doesn't matter (at least I think so).
                gradient.X = 0;
            }
        }

        /// <summary>
        /// Modifies <paramref name="gradient"/> so that it points at an angle which is a multiple of 45° (π/4).
        /// </summary>
        /// <param name="gradient">The gradient to adjust.</param>
        private static RelativePosition RoundToOctant2(Vector2 gradient)
        {
            const float octant = (float) (2 * Math.PI / 8);
            var index = RelativePosition.CenterRight;
            var angle = gradient.Angle();

            if (angle < 0)
                angle += 2 * (float) Math.PI;

            angle -= octant / 2;
            if (!(angle > 0))
                return index;

            ++index;
            while (angle > octant)
            {
                angle -= octant;

                if ((int)++index == 9)
                    index = RelativePosition.CenterRight;
            }

            return index;
        }

        internal void SetCost(Point point, float cost) => this[point] = cost;

        internal float? this[Point point]
        {
            get => Contains(point) ? (float?) mMap[point.Y, point.X] : null;
            private set
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
            foreach (var (point, value) in AllPoints)
            {
                var (col, row) = point;
                if (col == 0 && row > 0)
                    builder.AppendLine();
                if (col == 0)
                    builder.Append($"[{row:D2}]  ");

                if (!IsWalkable(point))
                {
                    builder.Append(" XX ");
                    continue;
                }

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
            var visualization = TileVisualizer.FullTile(ObstacleMatrix.SubTileCount, grid, spriteManager);
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
            visualization.Append(ObstacleMatrix);
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
