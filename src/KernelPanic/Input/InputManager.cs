using System;
using System.Collections.Generic;
using System.ComponentModel;
using KernelPanic.Camera;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace KernelPanic.Input
{
    /// <summary>
    /// Handles all input coming from the mouse and the keyboard including de-bouncing.
    /// </summary>
    public sealed class InputManager
    {
        // private const double MaximumDoubleClickDelay = 330; // You have 333ms to enter your double click
        // private double mTimeLastClick = MaximumDoubleClickDelay; // Left MouseButton (init is for 'reset')
        // private int mDoubleClickFrameCount; 
        
        /// <summary>
        /// The width/height of the stripe around the screen border in which the camera is moved.
        /// </summary>
        internal const int ScreenBorderDistance = 100;

        private readonly RawInputState mInputState;
        private readonly ICamera mCamera;
        private readonly List<ClickTarget> mClickTargets;

        /// <summary>
        /// Left, Middle and Right MouseButton
        /// </summary>
        internal enum MouseButton : byte
        {
            Left, Middle, Right
        }

        internal InputManager(List<ClickTarget> clickTargets, ICamera camera, RawInputState inputState)
        {
            mCamera = camera;
            mInputState = inputState;
            mClickTargets = clickTargets;

            UpdateCamera();
        }

        internal bool IsActive => mInputState.IsActive;

        #region Claiming
        
        /* internal bool IsClaimed(Keys key) => mInputState.IsClaimed(key); */
        
        internal bool IsClaimed(MouseButton mouseButton) => mInputState.IsClaimed(mouseButton);

        #endregion

        #region Keys

        /// <summary>
        /// checks if any of the Keyboard Buttons has been pressed (at this exact moment)
        /// </summary>
        /// <param name="keys">Keys that should be checked</param>
        /// <returns>True if any of the keys was pressed</returns>
        internal bool KeyPressed(params Keys[] keys)
        {
            return KeyChanged(keys, true);
        }

        /* TODO uncomment this
        /// <summary>
        /// checks if any of the Keyboard Buttons has been released (at this exact moment)
        /// </summary>
        /// <param name="keys">Keys that should be checked</param>
        /// <returns>True if any of the keys was released</returns>
        internal bool KeyReleased(params Keys[] keys)
        {
            return KeyChanged(keys, true);
        }
        */

        private bool KeyChanged(IEnumerable<Keys> keys, bool currentDown)
        {
            foreach (var key in keys)
            {
                if (mInputState.IsClaimed(key) ||
                    mInputState.PreviousKeyboard.IsKeyDown(key) == currentDown ||
                    mInputState.CurrentKeyboard.IsKeyDown(key) != currentDown)
                {
                    continue;
                }

                mInputState.Claim(key);
                return true;
            }

            return false;
        }

        /// <summary>
        /// checks if any of the Keyboard Buttons is currently being pressed
        /// </summary>
        /// <param name="keys">Keys that should be checked</param>
        /// <returns>True if any of the keys was down</returns>
        internal bool KeyDown(params Keys[] keys)
        {
            foreach (var key in keys)
            {
                if (mInputState.CurrentKeyboard.IsKeyDown(key))
                {
                    return true;
                }
            }

            return false;
        }

        /* TODO uncomment this
        /// <summary>
        /// checks if any of the Keyboard Buttons is currently not being pressed
        /// (this is not equal to '!KeyDown(keys)'
        /// </summary>
        /// <param name="keys">Keys that should be checked</param>
        /// <returns>True if any of the keys was up</returns>
        public bool KeyUp(params Keys[] keys)
        {
            foreach (Keys key in keys)
            {
                if (mInputState.CurrentKeyboard.IsKeyUp(key))
                {
                    return true;
                }
            }

            return false;
        }
        */

        #endregion

        #region Mouse

        private Point MousePosition => mInputState.CurrentMouse.Position;

        internal Vector2 TranslatedMousePosition =>
            Vector2.Transform(MousePosition.ToVector2(), mCamera.InverseTransformation);

        /*
        /// <summary>
        /// calculates the X and Y difference since the last update
        /// </summary>
        /// <returns>Tuple with X and Y distance</returns>
        internal Point MouseMovement =>
            new Point(mInputState.CurrentMouse.X - mInputState.PreviousMouse.X, mInputState.CurrentMouse.Y - mInputState.PreviousMouse.Y);
        */
        internal void RegisterClickTarget(Rectangle position, Action<InputManager> action) =>
            mClickTargets.Add(new ClickTarget(position, action));

        /// <summary>
        /// checks if any of the MouseButtons has been pressed (at this exact moment => de-bounced)
        /// </summary>
        /// <param name="mouseButtons">enum object: Left, Middle, Right</param>
        /// <returns>true if any of the buttons is freshly pressed</returns>
        internal bool MousePressed(params MouseButton[] mouseButtons)
        {
            return MouseChanged(mouseButtons, ButtonState.Pressed);
        }

        /// <summary>
        /// checks if any of the MouseButtons has been released (at this exact moment)
        /// </summary>
        /// <param name="mouseButtons"></param>
        /// <returns></returns>
        internal bool MouseReleased(params MouseButton[] mouseButtons)
        {
            return MouseChanged(mouseButtons, ButtonState.Released);
        }

        private bool MouseChanged(IEnumerable<MouseButton> mouseButtons, ButtonState expectedCurrent)
        {
            foreach (var mouseButton in mouseButtons)
            {
                bool Changed(Func<MouseState, ButtonState> view)
                {
                    if (mInputState.IsClaimed(mouseButton) ||
                        view(mInputState.PreviousMouse) == expectedCurrent ||
                        view(mInputState.CurrentMouse) != expectedCurrent)
                    {
                        return false;
                    }

                    mInputState.Claim(mouseButton);
                    return true;
                }
                
                switch (mouseButton)
                {
                    case MouseButton.Left:
                        if (Changed(state => state.LeftButton))
                            return true;
                        break;
                    case MouseButton.Middle:
                        if (Changed(state => state.MiddleButton))
                            return true;
                        break;
                    case MouseButton.Right:
                        if (Changed(state => state.RightButton))
                            return true;
                        break;
                    default:
                        throw new InvalidEnumArgumentException(nameof(mouseButtons),
                            (int) mouseButton,
                            typeof(MouseButton));
                }
            }

            return false;
        }

        /// <summary>
        /// checks if any of the MouseButtons is currently down
        /// </summary>
        /// <param name="mouseButtons"></param>
        /// <returns></returns>
        internal bool MouseDown(params MouseButton[] mouseButtons)
        {
            var down = false;
            foreach (var mouseButton in mouseButtons)
            {
                switch (mouseButton)
                {
                    case MouseButton.Left:
                        down |= mInputState.CurrentMouse.LeftButton == ButtonState.Pressed;
                        break;
                    case MouseButton.Middle:
                        down |= mInputState.CurrentMouse.MiddleButton == ButtonState.Pressed;
                        break;
                    case MouseButton.Right:
                        down |= mInputState.CurrentMouse.RightButton == ButtonState.Pressed;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(mouseButtons));
                }
            }

            return down;
        }

#if false // Uncomment when used.
        public Point LatestMouseLeftClickPosition { get; private set; }
        public Point LatestMouseMiddleClickPosition { get; private set; }
        public Point LatestMouseRightClickPosition { get; private set; }

        /// <summary>
        /// Checking for and updating the newest mouse Click positions
        /// </summary>
        private void UpdateMouseClickPosition()
        {
            if (MousePressed(MouseButton.Left))
            {
                LatestMouseLeftClickPosition = new Point(mInputState.CurrentMouse.X, mInputState.CurrentMouse.Y);
            }
            
            if (MousePressed(MouseButton.Middle))
            {
                LatestMouseMiddleClickPosition = new Point(mInputState.CurrentMouse.X, mInputState.CurrentMouse.Y);
            }
            
            if (MousePressed(MouseButton.Right))
            {
                LatestMouseRightClickPosition = new Point(mInputState.CurrentMouse.X, mInputState.CurrentMouse.Y);
            }
        }
#endif

        #endregion

        #region Scroll Wheel

        /// <summary>
        /// calculate how far the scroll wheel got turned
        /// </summary>
        /// <returns>negative value for zooming out</returns>
        private int ScrollWheelMovement()
        {
            return mInputState.CurrentMouse.ScrollWheelValue - mInputState.PreviousMouse.ScrollWheelValue;
        }

        /// <summary>
        /// Checking for 'Zoom Out' since last Update
        /// </summary>
        /// <returns></returns>
        private bool ScrolledDown()
        {
            return ScrollWheelMovement() < -5;
        }

        /// <summary>
        /// Checking for 'Zoom In' since last Update
        /// </summary>
        /// <returns></returns>
        private bool ScrolledUp()
        {
            return ScrollWheelMovement() > 5;
        }

        #endregion

        /* TODO uncomment this
        /// <summary>
        /// TODO
        /// </summary>
        /// <returns></returns>
        public Tuple<bool, Tuple<int, int>, Tuple<int, int>> MouseDragged(MouseButton mouseButton) // TODO implement this iff we need a rectangular selection
        {
            var startPointXy = new Tuple<int, int> (mInputState.CurrentMouse.X, mInputState.CurrentMouse.Y);
            var endPointXy = new Tuple<int, int>(mInputState.CurrentMouse.X, mInputState.CurrentMouse.Y);

            var result = new Tuple<bool, Tuple<int, int>, Tuple<int, int>>(false, startPointXy, endPointXy);
            return result;
        }
        */

        #region Camera

        /// <summary>
        /// Updates the camera's translation based on the current mouse/keyboard state. 
        /// </summary>
        private void UpdateCamera()
        {
            var xLeft = KeyDown(Keys.A);
            var xRight = KeyDown(Keys.D);
            var yUp = KeyDown(Keys.W);
            var yDown = KeyDown(Keys.S);

            bool mouseXLeft = false, mouseXRight = false, mouseYUp = false, mouseYDown = false;

            if (mInputState.MouseInWindow)
            {
                // Only react to mouse-near-window-border when the mouse is actually inside the window.
                mouseXLeft = MousePosition.X <= ScreenBorderDistance;
                mouseXRight = mInputState.Viewport.Width - ScreenBorderDistance <= MousePosition.X;
                mouseYUp = MousePosition.Y <= ScreenBorderDistance;
                mouseYDown = mInputState.Viewport.Height - ScreenBorderDistance <= MousePosition.Y;
            }

            // This function returns the value to be used for the apply call. If any of the first two arguments is true,
            // the arguments mouseDir1 and mouseDir2 aren't looked at, because keyboard input has preference over mouse
            // input.
            Change ChooseDirection(bool isKeyboard, bool dir1, bool dir2, bool? altDir1 = null, bool? altDir2 = null)
            {
                if (!dir1 && !dir2 && altDir1 is bool b1T && altDir2 is bool b2T)
                    return ChooseDirection(false, b1T, b2T);
                if (dir1 && !dir2)
                    return new Change(-1, isKeyboard, !isKeyboard);
                if (!dir1 && dir2)
                    return new Change(1, isKeyboard, !isKeyboard);

                return Change.None;
            }

            mCamera.Update(mInputState.Viewport.Bounds.Size,
                ChooseDirection(true, xLeft, xRight, mouseXLeft, mouseXRight),
                ChooseDirection(true, yUp, yDown, mouseYUp, mouseYDown),
                ChooseDirection(false, ScrolledUp(), ScrolledDown())
            );
        }

        #endregion
    }
}
