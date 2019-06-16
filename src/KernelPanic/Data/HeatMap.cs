using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace KernelPanic.Data
{
    class HeatMap
    {
        public const int Blocked = -1;

        public double[,] mMap;
        private int mWidth;
        private int mHeight;

        public HeatMap(int width, int height)
        {
            mMap = new double[height, width];
            mWidth = width;
            mHeight = height;
        }

        public bool IsWalkable(Point point)
        {
            if (point.X >= mWidth || point.Y >= mHeight) return false;
            if (point.X < 0 || point.Y < 0) return false;
            return mMap[point.Y, point.X] != Blocked;
        }

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
            else heatUp = mMap[point.Y, point.X];

            if (IsWalkable(left)) heatLeft = mMap[point.Y, point.X - 1];
            else heatUp = mMap[point.Y, point.X];

            if (IsWalkable(right)) heatRight = mMap[point.Y, point.X + 1];
            else heatUp = mMap[point.Y, point.X];

            grad = new Vector2((float)(heatLeft - heatRight), (float)(heatUp - heatDown));
            return grad;
        }

        public Vector2 NormalizedGradient(Point point)
        {
            Vector2 grad = Gradient(point);
            if (grad != Vector2.Zero) grad.Normalize();
            return grad;
        }

        public int Width { get => mWidth;}
        public int Height { get => mHeight;}
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
