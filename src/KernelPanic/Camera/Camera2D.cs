using System;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

namespace KernelPanic
{
    [DataContract]
    internal sealed class Camera2D : ICamera
    {
        [DataMember]
        private float mX;
        [DataMember]
        private float mY;
        [DataMember]
        private float mZoom = 1;
        [DataMember]
        private readonly Rectangle mBoundingBox;

        private Matrix mTransformation;
        private Matrix mInverseTransformation;

        private const float MinZoom = 0.2f;
        private const float MaxZoom = 2.0f;
        private const float Movement = 10f;

        /// <summary>
        /// Creates a new camera at (0,0) which moves only inside <paramref name="boundingBox"/> with a viewport of size
        /// <paramref name="viewportSize"/>.
        /// </summary>
        /// <param name="boundingBox">No area outside this rectangle defined in world-coordinates is viewable using this camera.</param>
        /// <param name="viewportSize">The current size of the viewport.</param>
        internal Camera2D(Rectangle boundingBox, Point viewportSize)
        {
            mBoundingBox = boundingBox;
            RecalculateTransformations(viewportSize);
        }

        /// <inheritdoc />
        public Matrix Transformation => mTransformation;

        /// <inheritdoc />
        public Matrix InverseTransformation => mInverseTransformation;

        /// <inheritdoc />
        public void Update(Point viewportSize, Change x, Change y, Change scrollVertical)
        {
            var newZoom = MathHelper.Clamp((float) Math.Pow(1.5, scrollVertical.Direction) * mZoom, MinZoom, MaxZoom);
            if (Math.Abs(newZoom - mZoom) > 0.1f)
            {
                // When zooming we have to recalculate immediately, otherwise
                // the calculations to stay in the bounding box aren't correct.
                mZoom = newZoom;
                RecalculateTransformations(viewportSize);
            }

            if (x.IsNone && y.IsNone)
                return;

            // Turn the corners of the camera's bounds into screen coordinates.
            var windowUpperLeft = Vector2.Transform(Vector2.Zero, InverseTransformation);
            var windowLowerRight = Vector2.Transform(viewportSize.ToVector2(), InverseTransformation);

            // Calculate the ranges in which the movement has to lie.
            var minDifference = mBoundingBox.Location.ToVector2() - windowUpperLeft;
            var maxDifference = mBoundingBox.Location.ToVector2() + mBoundingBox.Size.ToVector2() - windowLowerRight;

            // Update the camera position.
            mX += MathHelper.Clamp(x.Direction * Movement / mZoom, minDifference.X, maxDifference.X);
            mY += MathHelper.Clamp(y.Direction * Movement / mZoom, minDifference.Y, maxDifference.Y);

            // We recalculate the transformation and its inverse after each update, since the update occurs only once
            // during an update-cycle and in each update-cycle both will be used.
            RecalculateTransformations(viewportSize);
        }

        private void RecalculateTransformations(Point viewportSize)
        {
            var position = new Vector3(mX, mY, 0);
            var origin = new Vector3(0.5f * viewportSize.ToVector2(), 0);
            mTransformation =
                Matrix.CreateTranslation(-position) *
                Matrix.CreateTranslation(-origin) *
                Matrix.CreateScale(mZoom, mZoom, 1) *
                Matrix.CreateTranslation(origin);

            Matrix.Invert(ref mTransformation, out mInverseTransformation);
        }
    }
}
