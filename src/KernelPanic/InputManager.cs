﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace KernelPanic
{
    /// <summary>
    /// Handles all input coming from the mouse and the keyboard including de-bouncing.
    /// </summary>
    public sealed class InputManager
    {
        // private const double MaximumDoubleClickDelay = 330; // You have 333ms to enter your double click
        // private double mTimeLastClick = MaximumDoubleClickDelay; // Left MouseButton (init is for 'reset')
        // private int mDoubleClickFrameCount; 
        private const int ScreenBorderDistance = 100;

        private readonly RawInputState mInputState;
        private readonly ICamera mCamera;

        /// <summary>
        /// Left, Middle and Right MouseButton
        /// </summary>
        internal enum MouseButton : byte
        {
            Left, Middle, Right
        }

        internal InputManager(ICamera camera, RawInputState inputState)
        {
            mCamera = camera;
            mInputState = inputState;

            UpdateCamera();
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

        private Point MousePosition => mInputState.CurrentMouse.Position;

        internal Vector2 TranslatedMousePosition =>
            Vector2.Transform(MousePosition.ToVector2(), mCamera.InverseTransformation);

        /// <summary>
        /// checks if any of the Keyboard Buttons has been pressed (at this exact moment)
        /// </summary>
        /// <param name="keys">Keys that should be checked</param>
        /// <returns>True if any of the keys was pressed</returns>
        public bool KeyPressed(params Keys[] keys)
        {
            foreach (Keys key in keys)
            {
                if (mInputState.CurrentKeyboard.IsKeyDown(key) && mInputState.PreviousKeyboard.IsKeyUp(key))
                {
                    return true;
                }
            }

            return false;
        }


        /* TODO uncomment this
        /// <summary>
        /// checks if any of the Keyboard Buttons has been released (at this exact moment)
        /// </summary>
        /// <param name="keys">Keys that should be checked</param>
        /// <returns>True if any of the keys was released</returns>
        public bool KeyReleased(params Keys[] keys)
        {
            foreach (Keys key in keys)
            {
                if (mInputState.CurrentKeyboard.IsKeyUp(key) && mInputState.PreviousKeyboard.IsKeyDown(key))
                {
                    return true;
                }
            }

            return false;
        }
        */

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

        /// <summary>
        /// calculates the X and Y difference since the last update
        /// </summary>
        /// <returns>Tuple with X and Y distance</returns>
        internal Point MouseMovement =>
            new Point(mInputState.CurrentMouse.X - mInputState.PreviousMouse.X, mInputState.CurrentMouse.Y - mInputState.PreviousMouse.Y);

        /// <summary>
        /// checks if any of the MouseButtons has been pressed (at this exact moment => de-bounced)
        /// </summary>
        /// <param name="mouseButtons">enum object: Left, Middle, Right</param>
        /// <returns>true if any of the buttons is freshly pressed</returns>
        internal bool MousePressed(params MouseButton[] mouseButtons)
        {
            var pressed = false;
            foreach (var mouseButton in mouseButtons)
            {
                switch (mouseButton)
                {
                    case MouseButton.Left:
                        pressed |= mInputState.CurrentMouse.LeftButton == ButtonState.Pressed &&
                                   mInputState.PreviousMouse.LeftButton != ButtonState.Pressed;
                        break;
                    case MouseButton.Middle:
                        pressed |= mInputState.CurrentMouse.MiddleButton == ButtonState.Pressed &&
                                   mInputState.PreviousMouse.MiddleButton != ButtonState.Pressed;
                        break;
                    case MouseButton.Right:
                        pressed |= mInputState.CurrentMouse.RightButton == ButtonState.Pressed &&
                                   mInputState.PreviousMouse.RightButton != ButtonState.Pressed;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(mouseButtons));
                }
            }

            return pressed;
        }

        /* TODO uncomment this
        /// <summary>
        /// checks if any of the MouseButtons has been released (at this exact moment)
        /// </summary>
        /// <param name="mouseButtons"></param>
        /// <returns></returns>
        public bool MouseReleased(params MouseButton[] mouseButtons)
        {
            bool released = false;
            foreach (var mouseButton in mouseButtons)
            {
                switch (mouseButton)
                {
                    case MouseButton.Left:
                        released |= mInputState.CurrentMouse.LeftButton != ButtonState.Pressed &&
                                    mInputState.PreviousMouse.LeftButton == ButtonState.Pressed;
                        break;
                    case MouseButton.Middle:
                        released |= mInputState.CurrentMouse.MiddleButton != ButtonState.Pressed &&
                                    mInputState.PreviousMouse.MiddleButton == ButtonState.Pressed;
                        break;
                    case MouseButton.Right:
                        released |= mInputState.CurrentMouse.RightButton != ButtonState.Pressed &&
                                    mInputState.PreviousMouse.RightButton == ButtonState.Pressed;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(mouseButtons));
                }
            }

            return released;
        }
        */

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

        /// <summary>
        /// Updates the camera's translation based on the current mouse/keyboard state. 
        /// </summary>
        private void UpdateCamera()
        {
            var xLeft = KeyDown(Keys.A);
            var xRight = KeyDown(Keys.D);
            var yUp = KeyDown(Keys.W);
            var yDown = KeyDown(Keys.S);

            var mouseXLeft = MousePosition.X <= ScreenBorderDistance;
            var mouseXRight = mInputState.Viewport.Width - ScreenBorderDistance <= MousePosition.X;
            var mouseYUp = MousePosition.Y <= ScreenBorderDistance;
            var mouseYDown = mInputState.Viewport.Height - ScreenBorderDistance <= MousePosition.Y;

            // This function returns the value to be used for the apply call. If any of the first two arguments is true,
            // the arguments mouseDir1 and mouseDir2 aren't looked at, because keyboard input has preference over mouse
            // input.
            Change ChooseDirection(bool isKeyboard, bool dir1, bool dir2, bool? altDir1 = null, bool? altDir2 = null)
            {
                if (dir1 && !dir2)
                    return new Change(-1, isKeyboard, !isKeyboard);
                if (!dir1 && dir2)
                    return new Change(1, isKeyboard, !isKeyboard);
                if (!dir1 && !dir2 && altDir1 is bool b1T && altDir2 is bool b2T)
                    return ChooseDirection(false, b1T, b2T);

                return Change.None;
            }

            mCamera.Update(
                ChooseDirection(true, xLeft, xRight, mouseXLeft, mouseXRight),
                ChooseDirection(true, yUp, yDown, mouseYUp, mouseYDown),
                ChooseDirection(false, ScrolledUp(), ScrolledDown())
            );
        }
    }
}
