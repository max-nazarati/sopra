using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace KernelPanic
{
    /// <summary>
    /// singleton class to handle the Input for the whole game
    /// </summary>
    internal sealed class InputManager
    {
        private KeyboardState mCurrentKeyboardState, mPreviousKeyboardState;
        private MouseState mCurrentMouseState, mPreviousMouseState;
        private static InputManager sInstance;
        private const double MaximumDoubleClickDelay = 330; // You have 333ms to enter your double click
        private double mTimeLastClick = MaximumDoubleClickDelay; // Left MouseButton (init is for 'reset')
        // private int mDoubleClickFrameCount; 
        private readonly Tuple<int, int> mScreenSizeTuple = new Tuple<int, int> (1920, 1080);
        private const int ScreenBorderDistance = 100;

        /// <summary>
        /// Left, Middle and Right MouseButton
        /// </summary>
        internal enum MouseButton : byte
        {
            Left, Middle, Right
        }

        internal static InputManager Default => sInstance ?? (sInstance = new InputManager());

        /// <summary>
        /// updates the Input states, should be called in the main update function.
        /// </summary>
        internal void Update(GameTime gameTime)
        {
            // updating the keyboard
            mPreviousKeyboardState = mCurrentKeyboardState;
            mCurrentKeyboardState = Keyboard.GetState();

            // updating the mouse
            mPreviousMouseState = mCurrentMouseState;
            mCurrentMouseState = Mouse.GetState();

            DoubleClickUpdate(gameTime);
            
            // Uncomment when used.
            // UpdateMouseClickPosition();
        }

        /// <summary>
        /// Updates the mRecentDoubleClicked Variable
        /// </summary>
        private void DoubleClickUpdate(GameTime gameTime)
        {
            // time has past :)
            mTimeLastClick += gameTime.ElapsedGameTime.Milliseconds;

            // check for Input to reset timer or mark recent double click as success
            if (MousePressed(MouseButton.Left))
            {
                if (mTimeLastClick < MaximumDoubleClickDelay)
                {
                    mTimeLastClick = MaximumDoubleClickDelay; // 'resetting' the double click
                    DoubleClick = true;
                    return;
                }
                DoubleClick = false;
                mTimeLastClick = 0;
            }
            else
            {
                DoubleClick = false;
            }
        }

        /// <summary>
        /// TODO make as get function
        /// </summary>
        /// <returns></returns>
        internal bool DoubleClick { get; private set; }

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
                LatestMouseLeftClickPosition = new Point(mCurrentMouseState.X, mCurrentMouseState.Y);
            }
            
            if (MousePressed(MouseButton.Middle))
            {
                LatestMouseMiddleClickPosition = new Point(mCurrentMouseState.X, mCurrentMouseState.Y);
            }
            
            if (MousePressed(MouseButton.Right))
            {
                LatestMouseRightClickPosition = new Point(mCurrentMouseState.X, mCurrentMouseState.Y);
            }
        }
#endif

        internal Point MousePosition => mCurrentMouseState.Position;

        /// <summary>
        /// checks if any of the Keyboard Buttons has been pressed (at this exact moment)
        /// </summary>
        /// <param name="keys">Keys that should be checked</param>
        /// <returns>True if any of the keys was pressed</returns>
        public bool KeyPressed(params Keys[] keys)
        {
            foreach (Keys key in keys)
            {
                if (mCurrentKeyboardState.IsKeyDown(key) && mPreviousKeyboardState.IsKeyUp(key))
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
                if (mCurrentKeyboardState.IsKeyUp(key) && mPreviousKeyboardState.IsKeyDown(key))
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
                if (mCurrentKeyboardState.IsKeyDown(key))
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
                if (mCurrentKeyboardState.IsKeyUp(key))
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
            new Point(mCurrentMouseState.X - mPreviousMouseState.X, mCurrentMouseState.Y - mPreviousMouseState.Y);

        /// <summary>
        /// checks if any of the MouseButtons has been pressed (at this exact moment => de-bounced)
        /// </summary>
        /// <param name="mouseButtons">enum object: Left, Middle, Right</param>
        /// <returns>true if any of the buttons is freshly pressed</returns>
        public bool MousePressed(params MouseButton[] mouseButtons)
        {
            var pressed = false;
            foreach (var mouseButton in mouseButtons)
            {
                switch (mouseButton)
                {
                    case MouseButton.Left:
                        pressed |= mCurrentMouseState.LeftButton == ButtonState.Pressed &&
                                   mPreviousMouseState.LeftButton != ButtonState.Pressed;
                        break;
                    case MouseButton.Middle:
                        pressed |= mCurrentMouseState.MiddleButton == ButtonState.Pressed &&
                                   mPreviousMouseState.MiddleButton != ButtonState.Pressed;
                        break;
                    case MouseButton.Right:
                        pressed |= mCurrentMouseState.RightButton == ButtonState.Pressed &&
                                   mPreviousMouseState.RightButton != ButtonState.Pressed;
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
                        released |= mCurrentMouseState.LeftButton != ButtonState.Pressed &&
                                    mPreviousMouseState.LeftButton == ButtonState.Pressed;
                        break;
                    case MouseButton.Middle:
                        released |= mCurrentMouseState.MiddleButton != ButtonState.Pressed &&
                                    mPreviousMouseState.MiddleButton == ButtonState.Pressed;
                        break;
                    case MouseButton.Right:
                        released |= mCurrentMouseState.RightButton != ButtonState.Pressed &&
                                    mPreviousMouseState.RightButton == ButtonState.Pressed;
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
                        down |= mCurrentMouseState.LeftButton == ButtonState.Pressed;
                        break;
                    case MouseButton.Middle:
                        down |= mCurrentMouseState.MiddleButton == ButtonState.Pressed;
                        break;
                    case MouseButton.Right:
                        down |= mCurrentMouseState.RightButton == ButtonState.Pressed;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(mouseButtons));
                }
            }

            return down;
        }

        /// <summary>
        /// TODO WIP should this even be here or in camera?
        /// </summary>
        /// <returns></returns>
        internal bool MouseAtLeftScreenBorder()
        {
            return mCurrentMouseState.X < ScreenBorderDistance
                   && mCurrentMouseState.X > 0;
        }

        /// <summary>
        /// TODO WIP should this even be here or in camera?
        /// </summary>
        /// <returns></returns>
        internal bool MouseAtTopScreenBorder()
        {
            return mCurrentMouseState.Y < ScreenBorderDistance
                   && mCurrentMouseState.Y > 0;
        }

        /// <summary>
        /// TODO WIP should this even be here or in camera?
        /// </summary>
        /// <returns></returns>
        internal bool MouseAtRightScreenBorder()
        {
            return mCurrentMouseState.X > mScreenSizeTuple.Item1 - ScreenBorderDistance
                   && mCurrentMouseState.X < mScreenSizeTuple.Item1;
        }

        /// <summary>
        /// TODO WIP should this even be here or in camera?
        /// </summary>
        /// <returns></returns>
        internal bool MouseAtBottomScreenBorder()
        {
            return mCurrentMouseState.Y > mScreenSizeTuple.Item2 - ScreenBorderDistance
                   && mCurrentMouseState.Y < mScreenSizeTuple.Item2;
        }

        /// <summary>
        /// calculate how far the scroll wheel got turned
        /// </summary>
        /// <returns>negative value for zooming out</returns>
        private int ScrollWheelMovement()
        {
            return mCurrentMouseState.ScrollWheelValue - mPreviousMouseState.ScrollWheelValue;
        }

        /// <summary>
        /// Checking for 'Zoom Out' since last Update
        /// </summary>
        /// <returns></returns>
        internal bool ScrolledDown()
        {
            return ScrollWheelMovement() < -5;
        }

        /// <summary>
        /// Checking for 'Zoom In' since last Update
        /// </summary>
        /// <returns></returns>
        internal bool ScrolledUp()
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
            var startPointXy = new Tuple<int, int> (mCurrentMouseState.X, mCurrentMouseState.Y);
            var endPointXy = new Tuple<int, int>(mCurrentMouseState.X, mCurrentMouseState.Y);

            var result = new Tuple<bool, Tuple<int, int>, Tuple<int, int>>(false, startPointXy, endPointXy);
            return result;
        }
        */

    }
}
