using Microsoft.Xna.Framework;

namespace KernelPanic.Camera
{
    internal interface ICamera
    {
        /// <summary>
        /// The camera's current transformation for turning world-coordinates into window-coordinates.
        /// </summary>
        Matrix Transformation { get; }
        
        /// <summary>
        /// The camera's current inverse transformation for turning window-coordinates into world-coordinates.
        /// </summary>
        Matrix InverseTransformation { get; }

        /// <summary>
        /// Updates the camera's <see cref="Transformation"/> and in turn <see cref="InverseTransformation"/>.
        /// Implementations may choose to ignore parts of the input to disallow changing the camera.
        /// </summary>
        /// <param name="viewportSize"></param>
        /// <param name="x"><paramref name="x.Direction"/> is negative if moved to the left and positive if moved to the right.</param>
        /// <param name="y"><paramref name="y.Direction"/> is negative if moved to the top and positive if moved to the bottom.</param>
        /// <param name="scrollVertical"><paramref name="scrollVertical.Direction"/> is negative if scrolled up and positive if scrolled down.</param>
        void Update(Point viewportSize, Change x, Change y, Change scrollVertical);
    }
}
