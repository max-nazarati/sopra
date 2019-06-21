using Microsoft.Xna.Framework;

namespace KernelPanic.Camera
{
    /// <summary>
    /// A static camera doesn't transform and won't change on <see cref="Update"/>.
    /// </summary>
    internal sealed class StaticCamera : ICamera
    {
        public Matrix Transformation => Matrix.Identity;
        public Matrix InverseTransformation => Matrix.Identity;

        public void Update(Point viewportSize, Change x, Change y, Change scrollVertical)
        {
            // A static camera never changes.
        }
    }
}
