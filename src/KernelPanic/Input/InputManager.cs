using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using KernelPanic.Camera;
using KernelPanic.Options;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace KernelPanic.Input
{
    /// <summary>
    /// Handles all input coming from the mouse and the keyboard including de-bouncing.
    /// </summary>
    internal sealed class InputManager
    {
        /// <summary>
        /// The width/height of the stripe around the screen border in which the camera is moved.
        /// </summary>
        internal const int ScreenBorderDistance = 100;

        private readonly RawInputState mInputState;
        private readonly ICamera mCamera;
        private readonly List<ClickTarget> mClickTargets;

        private readonly OptionsData mSettings;

        /// <summary>
        /// Left, Middle and Right MouseButton
        /// </summary>
        internal enum MouseButton : byte
        {
            Left, Middle, Right
        }

        internal InputManager(OptionsData settings, List<ClickTarget> clickTargets, ICamera camera, RawInputState inputState, GameTime gameTime)
        {
            mCamera = camera;
            mSettings = settings;
            mInputState = inputState;
            mClickTargets = clickTargets;

            UpdateCamera(gameTime);
        }

        internal bool IsActive => mInputState.IsActive;

        #region Claiming
        
        /* internal bool IsClaimed(Keys key) => mInputState.IsClaimed(key); */
        
        internal bool IsClaimed(MouseButton mouseButton) => mInputState.IsClaimed(mouseButton);

        #endregion

        #region Keys

        internal bool AnyKeyPressed => mInputState.CurrentKeyboard.GetPressedKeys().Any();

        internal Keys PressedKey(Func<Keys, bool> predicate)
        {
            var key = mInputState.CurrentKeyboard.GetPressedKeys().FirstOrDefault(predicate);
            if (key != Keys.None)
                mInputState.Claim(key);
            return key;
        }

        /// <summary>
        /// Checks if the given key was pressed in this frame (de-bounced).
        /// </summary>
        /// <param name="key">Key that should be checked.</param>
        /// <returns><c>true</c> if the key became pressed.</returns>
        internal bool KeyPressed(Keys key)
        {
            key = mSettings.KeyMap[key];
            if (mInputState.IsClaimed(key) || mInputState.PreviousKeyboard.IsKeyDown(key) || !mInputState.CurrentKeyboard.IsKeyDown(key))
            {
                return false;
            }

            mInputState.Claim(key);
            return true;
        }

        /// <summary>
        /// Checks if the given key is in a pressed state.
        /// </summary>
        /// <param name="key">Key that should be checked.</param>
        /// <returns><c>true</c> if the key is down.</returns>
        private bool KeyDown(Keys key)
        {
            return mInputState.CurrentKeyboard.IsKeyDown(mSettings.KeyMap[key]);
        }

        #endregion

        #region Mouse

        private Point MousePosition => mInputState.CurrentMouse.Position;

        private Vector2? mLazyTranslatedMousePosition;
        internal Vector2 TranslatedMousePosition
        {
            get
            {
                if (mLazyTranslatedMousePosition is Vector2 position)
                    return position;
                position = Vector2.Transform(MousePosition.ToVector2(), mCamera.InverseTransformation);
                mLazyTranslatedMousePosition = position;
                return position;
            }
        }

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
            return mSettings.ScrollInverted ? ScrollWheelMovement() < 0 : ScrollWheelMovement() > 0;
        }

        /// <summary>
        /// Checking for 'Zoom In' since last Update
        /// </summary>
        /// <returns></returns>
        private bool ScrolledUp()
        {
            return mSettings.ScrollInverted ? ScrollWheelMovement() > 0 : ScrollWheelMovement() < 0;
        }

        #endregion

        #region Camera

        /// <summary>
        /// Updates the camera's translation based on the current mouse/keyboard state. 
        /// </summary>
        /// <param name="gameTime"></param>
        private void UpdateCamera(GameTime gameTime)
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
            Change ChooseDirection(bool dir1, bool dir2, bool? altDir1 = null, bool? altDir2 = null)
            {
                if (!dir1 && !dir2 && altDir1 is bool b1T && altDir2 is bool b2T)
                    return ChooseDirection(b1T, b2T);
                if (dir1 && !dir2)
                    return new Change(-1);
                if (!dir1 && dir2)
                    return new Change(1);

                return Change.None;
            }

            mCamera.Update(mInputState.Viewport.Bounds.Size,
                ChooseDirection(xLeft, xRight, mouseXLeft, mouseXRight),
                ChooseDirection(yUp, yDown, mouseYUp, mouseYDown),
                ChooseDirection(ScrolledUp(), ScrolledDown()),
                gameTime
            );
        }

        #endregion
    }
}
