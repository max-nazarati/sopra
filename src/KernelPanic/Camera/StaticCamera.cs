using Microsoft.Xna.Framework;

namespace KernelPanic
{
    /// <summary>
    /// A static camera doesn't transform and won't change on <see cref="Apply"/>.
    /// </summary>
    internal sealed class StaticCamera : ICamera
    {
        public Matrix Transformation => Matrix.Identity;
        public Matrix InverseTransformation => Matrix.Identity;

        public void Apply(Change x, Change y, Change scrollVertical)
        {
            // A static camera never changes.
        }
    }
}
