using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace KernelPanic
{
    /// <summary>
    /// Keeps track of the current and previous mouse and keyboard state.
    /// </summary>
    internal sealed class RawInputState
    {
        internal Viewport Viewport { get; private set; }
        internal MouseState CurrentMouse { get; private set; }
        internal MouseState PreviousMouse { get; private set; }
        internal KeyboardState CurrentKeyboard { get; private set; }
        internal KeyboardState PreviousKeyboard { get; private set; }

        internal void Update(Viewport viewport)
        {
            Viewport = viewport;
            PreviousMouse = CurrentMouse;
            CurrentMouse = Mouse.GetState();
            PreviousKeyboard = CurrentKeyboard;
            CurrentKeyboard = Keyboard.GetState();
        }
    }
}
