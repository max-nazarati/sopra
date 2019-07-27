namespace KernelPanic.Camera
{
    /// <summary>
    /// Describes a possible change in a camera's translation. The actual interpretation is up to the camera.
    /// </summary>
    internal struct Change
    {
        /// <summary>
        /// Creates a new object and initializes it with the given values.
        /// </summary>
        /// <param name="direction"><see cref="Direction"/></param>
        internal Change(sbyte direction)
        {
            Direction = direction;
        }

        /// <summary>
        /// Returns a value indicating no change.
        /// </summary>
        internal static Change None => new Change(0);

        /// <summary>
        /// The direction of this change, either <c>-1</c>, <c>0</c> or <c>1</c>. Zero means “no change“, the meaning
        /// of the other numbers depends on the context.
        /// </summary>
        internal sbyte Direction { get; }

        /// <summary>
        /// Returns <c>true</c> if no change occured.
        /// </summary>
        internal bool IsNone => Direction == 0;
    }
}