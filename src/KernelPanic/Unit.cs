using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace KernelPanic
{
    public class Unit : Entity
    {
        /* To select a unit, left-click on it
         * Further lef-clicks place the unit on different positions
         * Hold left-mouse button to drag the unit around
         * Right-clicks make the unit float to a different position
         * Spacebar to unselect unit
         */

        private bool mSelected;
        private Point mMovementGoal = new Point(-1, -1);

        public Unit(int x, int y, int width, int height, Texture2D texture) : base(x, y,
            width, height, texture)
        {
        }

        private bool LeftClick()
        {
            if (mContainerRectangle.Contains(mMouseState.Position))
            {
                if (mMouseState.LeftButton == ButtonState.Pressed && 
                    mOldMouseState.LeftButton == ButtonState.Released)
                {
                    return true;
                }
            }
            return false;
        }

        private void MoveToClick(Point target, int speed)
        {
            Vector2 direction = new Vector2(target.X - (mContainerRectangle.Width / 2) - mContainerRectangle.X,
                target.Y - (mContainerRectangle.Height / 2) - mContainerRectangle.Y);
            direction.Normalize();
            Vector2 normalizedDirection = direction;

            if (normalizedDirection.Length() >= 0.99)
            {
                mContainerRectangle.X += (int)(normalizedDirection.X * speed);
                mContainerRectangle.Y += (int)(normalizedDirection.Y * speed);
            }

            if (Math.Abs(mContainerRectangle.X + (mContainerRectangle.Width / 2) - target.X) <= 4 &&
                Math.Abs(mContainerRectangle.Y + (mContainerRectangle.Height / 2) - target.Y) <= 4)
            {
                mMovementGoal = new Point(-1, -1);
            }
        }

        private void JumpToClick(MouseState mouseState)
        {
            mContainerRectangle.Location = new Point(mouseState.X - mContainerRectangle.Width / 2, 
                mouseState.Y - mContainerRectangle.Height / 2);
        }

        public override void Update()
        {
            base.Update();

            if (mSelected)
            {
                if (mMouseState.RightButton == ButtonState.Pressed &&
                    mOldMouseState.RightButton == ButtonState.Released)
                {
                    mMovementGoal = mMouseState.Position;
                    return;
                }
                else if (mMouseState.LeftButton == ButtonState.Pressed)
                {
                    JumpToClick(mMouseState);
                    return;
                }
                else if (mMovementGoal != new Point(-1, -1))
                {
                    MoveToClick(mMovementGoal, 10);
                }

                if (mKeyboardState.IsKeyDown(Keys.Space) &&
                    mOldKeyboardState.IsKeyUp(Keys.Space))
                {
                    mSelected = false;
                    mMovementGoal = new Point(-1, -1);
                    return;
                }
            }
            if (LeftClick() && !mSelected)
            {
                mSelected = true;
            }
        }
    }
}
