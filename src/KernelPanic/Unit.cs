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

        private bool _selected;
        private Point _movementGoal = new Point(-1, -1);

        public Unit(int x, int y, int width, int height, Texture2D texture, GraphicsDeviceManager graphics) : base(x, y,
            width, height, texture, graphics)
        {
        }

        private bool LeftClick()
        {
            if (ContainerRectangle.Contains(MouseState.Position))
            {
                if (MouseState.LeftButton == ButtonState.Pressed && 
                    OldMouseState.LeftButton == ButtonState.Released)
                {
                    return true;
                }
            }
            return false;
        }

        private void MoveToClick(Point target, int speed)
        {
            Vector2 direction = new Vector2(target.X - (ContainerRectangle.Width / 2) - ContainerRectangle.X,
                target.Y - (ContainerRectangle.Height / 2) - ContainerRectangle.Y);
            direction.Normalize();
            Vector2 normalizedDirection = direction;

            if (normalizedDirection.Length() >= 0.99)
            {
                ContainerRectangle.X += (int)(normalizedDirection.X * speed);
                ContainerRectangle.Y += (int)(normalizedDirection.Y * speed);
            }

            if (Math.Abs(ContainerRectangle.X + (ContainerRectangle.Width / 2) - target.X) <= 4 &&
                Math.Abs(ContainerRectangle.Y + (ContainerRectangle.Height / 2) - target.Y) <= 4)
            {
                _movementGoal = new Point(-1, -1);
            }
        }

        private void JumpToClick(MouseState mouseState)
        {
            ContainerRectangle.Location = new Point(mouseState.X - ContainerRectangle.Width / 2, 
                mouseState.Y - ContainerRectangle.Height / 2);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_selected)
            {
                if (MouseState.RightButton == ButtonState.Pressed &&
                    OldMouseState.RightButton == ButtonState.Released)
                {
                    _movementGoal = MouseState.Position;
                    return;
                }
                else if (MouseState.LeftButton == ButtonState.Pressed)
                {
                    JumpToClick(MouseState);
                    return;
                }
                else if (_movementGoal != new Point(-1, -1))
                {
                    MoveToClick(_movementGoal, 10);
                }

                if (KeyboardState.IsKeyDown(Keys.Space) &&
                    OldKeyboardState.IsKeyUp(Keys.Space))
                {
                    _selected = false;
                    _movementGoal = new Point(-1, -1);
                    return;
                }
            }
            if (LeftClick() && !_selected)
            {
                _selected = true;
            }
        }
    }
}
