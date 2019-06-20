using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using KernelPanic.PathPlanning;
using KernelPanic.Table;
using Microsoft.Xna.Framework;

namespace KernelPanic.Data
{
    class HeatMap
    {
        public const int Blocked = -1;

        public double[,] mMap;
        private int mWidth;
        private int mHeight;

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
            mMap = new double[height, width];
            mWidth = width;
            mHeight = height;
        }

        /// <summary>
        /// check if a grid of the heat map is blocked (= -1) or not
        /// </summary>
        /// <param name="point">the point (x, y)</param>
        /// <returns>true if heatMap[y, x] != -1 (blocked)</returns>
        public bool IsWalkable(Point point)
        {
            if (point.X >= mWidth || point.Y >= mHeight) return false;
            if (point.X < 0 || point.Y < 0) return false;
            return mMap[point.Y, point.X] != Blocked;
        }

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
        public Vector2 Gradient(Point point)
        {
            Vector2 grad;
            Point up = new Point(point.X, point.Y - 1);
            Point down = new Point(point.X, point.Y + 1);
            Point left = new Point(point.X - 1, point.Y);
            Point right = new Point(point.X + 1, point.Y);
            double heatUp = 0;
            double heatDown = 0;
            double heatLeft = 0;
            double heatRight = 0;

            if (!IsWalkable(point)) return new Vector2(Single.NaN, Single.NaN);

            if (IsWalkable(up)) heatUp = mMap[point.Y - 1, point.X];
            else heatUp = mMap[point.Y, point.X];

            if (IsWalkable(down)) heatDown = mMap[point.Y + 1, point.X];
            else heatDown = mMap[point.Y, point.X];

            if (IsWalkable(left)) heatLeft = mMap[point.Y, point.X - 1];
            else heatLeft = mMap[point.Y, point.X];

            if (IsWalkable(right)) heatRight = mMap[point.Y, point.X + 1];
            else heatRight = mMap[point.Y, point.X];

            grad = new Vector2((float)(heatLeft - heatRight), (float)(heatUp - heatDown));
            return grad;
        }

        public Vector2 NormalizedGradient(Point point)
        {
            Vector2 grad = Gradient(point);
            if (grad != Vector2.Zero) grad.Normalize();
            return grad;
        }

        public void Set(Point point, int value)
        {
            if (point.X < 0 || point.X >= mWidth) return;
            if (point.Y < 0 || point.Y >= mHeight) return;
            mMap[point.Y, point.X] = value;
        }

        public double Get(Point point)
        {
            if (point.X < 0 || point.X >= mWidth) return double.NaN;
            if (point.Y < 0 || point.Y >= mHeight) return double.NaN;
            return mMap[point.Y, point.X];
        }


        public String ToString()
        {
            string result = "";
            for (int y=0; y < mHeight; y++)
            {
                for (int x = 0; x < mWidth; x++)
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
                        default: break;
                    }
                }
                if (y != mHeight - 1) result += "\n";
            }

            return result;
        }

        public int Width { get => mWidth;}
        public int Height { get => mHeight;}

        internal Visualizer CreateVisualization(Grid grid, SpriteManager spriteManager, bool drawBorderOnly=true)
        {
            var visualization = new Visualizer(grid, spriteManager, drawBorderOnly);
            for (int x = 0; x < mWidth; x++)
            {
                for (int y = 0; y < mHeight; y++)
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

    class VectorField
    {
        private Vector2[,] mVectorField;
        private int mHeight;
        private int mWidth;
        public VectorField(int width, int height)
        {
            mVectorField = new Vector2[height, width];
            mHeight = height;
            mWidth = width;
        }

        /// <summary>
        /// Update vector field given a heat map, e.g.:
        /// Let
        /// [ 4][ 3][ 2][ 2]
        /// [ 3][-1][ 1][ 1]
        /// [ 2][ 1][ 0][ 0]
        /// [ 2][ 1][ 0][ 0]
        /// denote the heat map, then the updated vector field is
        /// [(1,1)][(1,0)][(0,1)][(0,1)]
        /// [(0,1)][ None][(0,1)][(0,1)]
        /// [(1, 0)][(1, 0)][ None][ None]
        /// [(1, 0)][(1, 0)][ None][ None]
        /// with each square additionally normalized
        /// </summary>
        /// <param name="map"></param>
        public void Update(HeatMap map)
        {
            if (map.Width > mWidth || map.Height > mHeight) return;
            for (int i = 0; i < map.Width; i++)
            {
                for (int j = 0; j < map.Height; j++)
                {
                    mVectorField[j, i] = map.NormalizedGradient(new Point(i, j));
                }
            }
        }

        public Vector2 Vector(Point point)
        {
            if (point.X >= mWidth || point.Y >= mHeight) return new Vector2(Single.NaN, Single.NaN);
            if (point.X < 0 || point.Y < 0) return new Vector2(Single.NaN, Single.NaN);
            return mVectorField[point.Y, point.X];
        }
    }
}
