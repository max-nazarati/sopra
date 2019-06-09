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
        private readonly Rectangle mMaximumRectangle;

        private Matrix mTransformation;
        private Matrix mInverseTransformation;

        internal Camera2D(Rectangle maximumRectangle, Point viewportSize)
        {
            mMaximumRectangle = maximumRectangle;
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

            mX += x.Direction * 10 / mZoom;
            mY += y.Direction * 10 / mZoom;
            mZoom *= Clamp((float) Math.Pow(1.5, scrollVertical.Direction), 0.2f, 2.0f);

            // We should not let more than the maximum rectangle become visible.
            mX = Clamp(mX,
                mMaximumRectangle.X,
                (mMaximumRectangle.X + mMaximumRectangle.Width) * mZoom - viewportSize.X);
            mY = Clamp(mY,
                mMaximumRectangle.Y,
                (mMaximumRectangle.Y + mMaximumRectangle.Height) * mZoom - viewportSize.Y);
            
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

        /// <summary>
        /// Clamps the given value to the provided bounds. If <paramref name="min"/> is greater than
        /// <paramref name="max"/> the returned value is undefined.
        /// </summary>
        /// <param name="value">Value to clamp</param>
        /// <param name="min">Lower bound</param>
        /// <param name="max">Upper bound</param>
        /// <returns>
        /// If <paramref name="value"/> lies inside [<paramref name="min"/>, <paramref name="max"/>]
        /// <paramref name="value"/> will be returned, otherwise the nearest bound.
        /// </returns>
        private static float Clamp(float value, float min, float max) => Math.Max(Math.Min(value, max), min);
    }
}
