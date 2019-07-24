using System.Collections;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace KernelPanic.Input
{
    /// <summary>
    /// Keeps track of the current and previous mouse and keyboard state.
    /// </summary>
    internal sealed class RawInputState
    {
        internal bool IsActive { get; private set; }
        internal Viewport Viewport { get; private set; }
        internal MouseState CurrentMouse { get; private set; }
        internal MouseState PreviousMouse { get; private set; }
        internal KeyboardState CurrentKeyboard { get; private set; }
        internal KeyboardState PreviousKeyboard { get; private set; }

        private Hashtable ClaimedOperations { get; } = new Hashtable();
        
        internal Keys mPlaceTower, mCameraUp = Keys.W, mCameraDown = Keys.S, mCameraLeft = Keys.A
            , mCameraRight = Keys.D, mSellTower = Keys.X;

        internal void Update(bool isActive, Viewport viewport)
        {
            IsActive = isActive;
            Viewport = viewport;
            PreviousMouse = CurrentMouse;
            CurrentMouse = Mouse.GetState();
            PreviousKeyboard = CurrentKeyboard;
            CurrentKeyboard = Keyboard.GetState();
            
            ClaimedOperations.Clear();
        }

        internal bool KeyUsed(Keys key)
        {
            return key == mPlaceTower || key == mCameraUp || key == mCameraLeft
                     || key == mCameraDown || key == mCameraRight;
        }

        internal bool MouseInWindow =>
            Viewport.Bounds.Contains(CurrentMouse.Position);

        internal void Claim(object value) => ClaimedOperations[value] = null;
        internal bool IsClaimed(object value) => ClaimedOperations.Contains(value);
    }
}
