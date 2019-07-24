using System.Collections.Generic;
using KernelPanic.Data;
using KernelPanic.Input;
using KernelPanic.Table;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic.Entities
{
    internal struct LaneBorder : IGameObject
    {
        internal readonly RelativePosition mDirectionToLane;
        public Rectangle Bounds { get; }
        internal bool IsTargetBorder { get; private set; }
        internal bool IsOutside { get; private set; }

        private LaneBorder(Rectangle bounds, RelativePosition directionToLane) : this()
        {
            Bounds = bounds;
            mDirectionToLane = directionToLane;
        }

        internal static IEnumerable<LaneBorder> Borders(Rectangle rectangle, int width, bool outside, Lane.Side? targetSide = null)
        {
            var ifOutside = outside ? 1 : 0;
            var plusMinusMul = outside ? 1 : -1;

            var top = new Rectangle(
                rectangle.X - ifOutside * width,
                rectangle.Y - ifOutside * width,
                rectangle.Size.X + 2 * ifOutside * width,
                width);

            var left = new Rectangle(
                rectangle.X - ifOutside * width,
                rectangle.Y,
                width,
                rectangle.Height);

            var bottom = top;
            bottom.Offset(0, rectangle.Height + plusMinusMul * width);

            var right = left;
            right.Offset(rectangle.Width + plusMinusMul * width, 0);

            return new[]
            {
                new LaneBorder(top, outside ? RelativePosition.CenterBottom : RelativePosition.CenterTop)
                {
                    IsOutside = outside
                },
                new LaneBorder(left, outside ? RelativePosition.CenterRight : RelativePosition.CenterLeft)
                {
                    IsOutside = outside,
                    IsTargetBorder = targetSide == Lane.Side.Left
                },
                new LaneBorder(bottom, outside ? RelativePosition.CenterTop : RelativePosition.CenterBottom)
                {
                    IsOutside = outside
                },
                new LaneBorder(right, outside ? RelativePosition.CenterLeft : RelativePosition.CenterRight)
                {
                    IsOutside = outside,
                    IsTargetBorder = targetSide == Lane.Side.Right
                }
            };
        }

        int? IGameObject.DrawLevel => null;
        bool IGameObject.WantsRemoval
        {
            get => false;
            set { }
        }

        void IGameObject.Update(PositionProvider positionProvider, InputManager inputManager, GameTime gameTime)
        {
            // Nothing to do.
        }

        void IDrawable.Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            // Nothing to do.
        }
    }
}