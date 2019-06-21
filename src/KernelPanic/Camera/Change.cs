namespace KernelPanic.Camera
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
}