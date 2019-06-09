using System;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

namespace KernelPanic
{
    [DataContract]
    internal sealed class Camera2D : ICamera
    {
        [DataMember]
        private Vector2 mPosition;
        [DataMember]
        private float mZoom;

        private Matrix mTransformation;
        private Matrix mInverseTransformation;

        internal Camera2D(Point viewportSize)
        {
            mZoom = 1;
            mPosition.X = 290;
            mPosition.Y = 1150;
            RecalculateTransformations(viewportSize);
        }

        /// <inheritdoc />
        public Matrix Transformation => mTransformation;

        /// <inheritdoc />
        public Matrix InverseTransformation => mInverseTransformation;

        /// <inheritdoc />
        public void Update(Point viewportSize, Change x, Change y, Change scrollVertical)
        {
            if (x.Direction == 0 && y.Direction == 0 && scrollVertical.Direction == 0)
                return;

            PosX += x.Direction * 10 / mZoom;
            PosY += y.Direction * 10 / mZoom;
            Zoom *= (float) Math.Pow(1.5, scrollVertical.Direction);
            RecalculateTransformations(viewportSize);
        }

        private void RecalculateTransformations(Point viewportSize)
        {
            // We recalculate the transformation and its inverse after each update, since the update occurs only once
            // during an update-cycle and in each update-cycle both will be used.
            var origin = new Vector3(0.5f * viewportSize.ToVector2(), 0);
            mTransformation =
                Matrix.CreateTranslation(new Vector3(-mPosition, 0.0f)) *
                Matrix.CreateTranslation(-origin) *
                Matrix.CreateScale(mZoom, mZoom, 1) *
                Matrix.CreateTranslation(origin);

            Matrix.Invert(ref mTransformation, out mInverseTransformation);
        }

        private float Zoom
        {
            get => mZoom;
            set => mZoom = Math.Max(Math.Min(value, 2f), 0.2f);  // Zoom should stay in [0.2, 2.0].
        }


        private float PosX
        {
            get => mPosition.X;
            set
            {
                if (mPosition.X >= -1050 && mPosition.X <= 1150)
                {
                    mPosition.X = value;
                }
                if (mPosition.X < -1050)
                {
                    mPosition.X = -1050;
                }

                if (mPosition.X > 1150)
                {
                    mPosition.X = 1150;
                }
            }
        }

        private float PosY
        {
            get => mPosition.Y;
            set
            {
                if (mPosition.Y >= -1000 && mPosition.Y <= 1500)
                {
                    mPosition.Y = value;
                }
                if (mPosition.Y < -1000)
                {
                    mPosition.Y = -1000;
                }

                if (mPosition.Y > 1500)
                {
                    mPosition.Y = 1500;
                }
            }
        }
    }
}
