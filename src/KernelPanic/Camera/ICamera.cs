using Microsoft.Xna.Framework;

namespace KernelPanic
{
    internal interface ICamera
    {
        /// <summary>
        /// The camera's current transformation for turning window-coordinates into world-coordinates.
        /// </summary>
        Matrix Transformation { get; }
        
        /// <summary>
        /// The camera's current inverse transformation for turning world-coordinates into window-coordinates.
        /// </summary>
        Matrix InverseTransformation { get; }

        /// <summary>
        /// Updates the camera's <see cref="Transformation"/>. Implementations may choose to ignore parts of the
        /// input to disallow changing the camera. The parameters are either <c>-1</c>, <c>0</c> or <c>1</c>.
        /// </summary>
        /// <param name="xMovement">negative if moved to the left, positive if moved to the right.</param>
        /// <param name="yMovement">negative if moved to the top, positive if moved to the bottom.</param>
        /// <param name="scaling">negative if zoomed out, positive if zoomed in.</param>
        void Apply(sbyte xMovement, sbyte yMovement, sbyte scaling);
    }
}
