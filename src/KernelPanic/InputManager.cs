using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace KernelPanic
{
    /// <summary>
    /// singleton class to handle the Input for the whole game
    /// </summary>
    public sealed class InputManager
    {
        private KeyboardState mCurrentKeyboardState;
        private KeyboardState mPreviousKeyboardState;
        private static InputManager sInstance;

        public static InputManager Default
        {
            get
            {
                if (sInstance == null)
                {
                    sInstance = new InputManager();
                }

                return sInstance;
            }
        }

        /// <summary>
        /// updates the Key states, should be used frequently in the main.
        /// </summary>
        internal void Update()
        {
            mPreviousKeyboardState = mCurrentKeyboardState;
            mCurrentKeyboardState = Keyboard.GetState();
        }

        /// <summary>
        /// checks if a list of Keyboard Buttons have been pressed (at this exact moment)
        /// </summary>
        /// <param name="keys">Keys that should be checked</param>
        /// <returns>True if any of the keys was pressed</returns>
        private bool KeyPressed(params Keys[] keys)
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

        /// <summary>
        /// checks if a list of Keyboard Buttons have been released (at this exact moment)
        /// </summary>
        /// <param name="keys">Keys that should be checked</param>
        /// <returns>True if any of the keys was released</returns>
        private bool KeyReleased(params Keys[] keys)
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

        /// <summary>
        /// checks if a list of Keyboard Buttons are currently being pressed
        /// </summary>
        /// <param name="keys">Keys that should be checked</param>
        /// <returns>True if any of the keys was down</returns>
        private bool KeyDown(params Keys[] keys)
        {
            foreach (Keys key in keys)
            {
                if (mCurrentKeyboardState.IsKeyDown(key))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// checks if a list of Keyboard Buttons are currently not being pressed
        /// </summary>
        /// <param name="keys">Keys that should be checked</param>
        /// <returns>True if any of the keys was up</returns>
        private bool KeyUp(params Keys[] keys)
        {
            return !KeyDown(keys);
        }
    }
}
