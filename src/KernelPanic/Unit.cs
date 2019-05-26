﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;


namespace KernelPanic
{
    internal class Unit : Entity
    {
        public int AttackStrength { get; set; }
        public Point? MoveTarget { get; set; }
        public int RemainingLife { get; set; }
        public int Speed { get; set; }

        public Unit(int param) : base(param)
        {

        }

        public void DealDamage(int dmg)
        {

        }
        public void Kill()
        {

        }
        public int MaximumLife()
        {
            return -1;
        }
        public void WillSpawn(Action<Unit> unit)
        {

        }

        private int mCosts;
        /// <summary>
        /// Below, redundant stuff, perhaps?
        /// </summary>
        /// 
        /* To select a unit, left-click on it.
         * Further left-clicks place the unit on different positions.
         * Right-clicks make the unit float to a different position.
         * Use Space to unselect the unit.
         */

        public bool mSelected;
        private Point? mMovementGoal;

        internal Unit(int x, int y, int width, int height, Texture2D texture) : base(x, y, width, height, texture)
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
                mMovementGoal = null;
            }
        }

        private void JumpToClick(Point position)
        {
            mContainerRectangle.Location = new Point(position.X - mContainerRectangle.Width / 2, 
                position.Y - mContainerRectangle.Height / 2);
        }


        internal void Update(Matrix viewMatrix)
        {
            base.Update();
            var input = InputManager.Default;
            Vector2 vector = Vector2.Transform(input.MousePosition.ToVector2(), Matrix.Invert(viewMatrix));
            Point position = new Point((int)vector.X, (int)vector.Y);
            if (!mSelected)
            {
                if (mContainerRectangle.Contains(position))
                {
                    // select object
                    mSelected = input.MousePressed(InputManager.MouseButton.Left);
                }
            }
            else if (input.MousePressed(InputManager.MouseButton.Right))
            {
                mMovementGoal = position;
            }
            else if (input.MousePressed(InputManager.MouseButton.Left))
            {
                JumpToClick(position);
                mMovementGoal = null;
            }
            else if (mMovementGoal is Point goal)
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
