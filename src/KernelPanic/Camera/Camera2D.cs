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
        [DataMember]
        private readonly Vector2 mOrigin;

        private Matrix mTransformation;
        private Matrix mInverseTransformation;

        internal Camera2D(Point viewportSize)
        {
            // mRotation = 0;
            mZoom = 1;
            mOrigin = new Vector2(viewportSize.X / 2f, viewportSize.Y / 2f);
            mPosition.X = 290;
            mPosition.Y = 1150;
            RecalculateTransformations();
        }

        /// <inheritdoc />
        public Matrix Transformation => mTransformation;

        /// <inheritdoc />
        public Matrix InverseTransformation => mInverseTransformation;

        /// <inheritdoc />
        public void Apply(Change x, Change y, Change scrollVertical)
        {
            if (x.Direction == 0 && y.Direction == 0 && scrollVertical.Direction == 0)
                return;

            PosX += x.Direction * 10 / mZoom;
            PosY += y.Direction * 10 / mZoom;
            Zoom += scrollVertical.Direction * 0.1f / mZoom;
            RecalculateTransformations();
        }

        private void RecalculateTransformations()
        {
            // We recalculate the transformation and its inverse after each update, since the update occurs only once
            // during an update-cycle and in each update-cycle both will be used.

            mTransformation =
                Matrix.CreateTranslation(new Vector3(-mPosition, 0.0f)) *
                Matrix.CreateTranslation(new Vector3(-mOrigin, 0.0f)) *
                Matrix.CreateScale(mZoom, mZoom, 1) *
                Matrix.CreateTranslation(new Vector3(mOrigin, 0.0f));

            Matrix.Invert(ref mTransformation, out mInverseTransformation);
        }

        private float Zoom
        {
            get => mZoom;
            set
            {
                if (value > 0.1 && value < 6)
                {
                    mZoom = value;
                }
            }
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
