using Microsoft.Xna.Framework;

namespace KernelPanic
{
    /// <summary>
    /// Describes a possible change in a camera's translation. The actual interpretation is up to the camera.
    /// </summary>
    internal struct Change
    {
        /// <summary>
        /// Creates a new object and initializes it with the given values..
        /// </summary>
        /// <param name="direction"><see cref="Direction"/></param>
        /// <param name="viaKeyboard"><see cref="ViaKeyboard"/></param>
        /// <param name="viaMouse"><see cref="ViaMouse"/></param>
        internal Change(sbyte direction, bool viaKeyboard, bool viaMouse)
        {
            ViaMouse = viaMouse;
            Direction = direction;
            ViaKeyboard = viaKeyboard;
        }

        /// <summary>
        /// Returns a value indicating no change.
        /// </summary>
        internal static Change None => new Change(0, false, false);

        /// <summary>
        /// The direction of this change, either <c>-1</c>, <c>0</c> or <c>1</c>. Zero means “no change“, the meaning
        /// of the other numbers depends on the context.
        /// </summary>
        internal sbyte Direction { get; }
        
        /// <summary>
        /// <c>true</c> if this change was triggered by some mouse-event, <c>false</c> otherwise.
        /// </summary>
        /*internal*/ private bool ViaMouse { get; }
        
        /// <summary>
        /// <c>true</c> if this change was triggered by some key-press, <c>false</c> otherwise.
        /// </summary>
        /*internal*/ private bool ViaKeyboard { get; }

        /// <summary>
        /// Returns <c>true</c> if no change occured.
        /// </summary>
        internal bool IsNone => Direction == 0;
    }

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
