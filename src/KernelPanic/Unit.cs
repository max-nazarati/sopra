using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


namespace KernelPanic
{
    internal sealed class Unit : Entity
    {
        /* To select a unit, left-click on it.
         * Further left-clicks place the unit on different positions.
         * Hold left-mouse button to drag the unit around.
         * Right-clicks make the unit float to a different position.
         * Use Space to unselect the unit.
         */

        public bool mSelected;
        private Point? mMovementGoal;

        internal Unit(int x, int y, int width, int height) : base(x, y, width, height)
        {
        }

        private void MoveToClick(Point target, int speed)
        {
            var direction = new Vector2(target.X - mContainerRectangle.Width / 2 - mContainerRectangle.X,
                target.Y - mContainerRectangle.Height / 2 - mContainerRectangle.Y);
            direction.Normalize();
            var normalizedDirection = direction;

            if (normalizedDirection.Length() >= 0.99)
            {
                mContainerRectangle.X += (int)(normalizedDirection.X * speed);
                mContainerRectangle.Y += (int)(normalizedDirection.Y * speed);
            }

            if (Math.Abs(mContainerRectangle.X + mContainerRectangle.Width / 2 - target.X) <= 4 &&
                Math.Abs(mContainerRectangle.Y + mContainerRectangle.Height / 2 - target.Y) <= 4)
            {
                mMovementGoal = new Point(-1, -1);
            }
        }

        private void JumpToClick(Point position)
        {
            mContainerRectangle.Location = new Point(position.X - mContainerRectangle.Width / 2, 
                position.Y - mContainerRectangle.Height / 2);
        }

        internal override void Update()
        {
            base.Update();

            var input = InputManager.Default;

            if (!mSelected)
            {
                mSelected = input.MousePressed(InputManager.MouseButton.Left);
                return;
            }

            if (input.MousePressed(InputManager.MouseButton.Right))
            {
                mMovementGoal = input.MousePosition;
                return;
            }

            if (input.MousePressed(InputManager.MouseButton.Left))
            {
                JumpToClick(input.MousePosition);
                return;
            }

            if (mMovementGoal is Point goal)
            {
                MoveToClick(goal, 10);
            }
            else if (input.KeyPressed(Keys.Space))
            {
                mSelected = false;
                mMovementGoal = null;
            }
        }
    }
}
