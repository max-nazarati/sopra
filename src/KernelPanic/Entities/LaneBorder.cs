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
        internal readonly RelativePosition mSide;
        public Rectangle Bounds { get; }
        internal bool IsTargetBorder { get; }

        private LaneBorder(Rectangle bounds, bool targetBorder, RelativePosition side)
        {
            Bounds = bounds;
            IsTargetBorder = targetBorder;
            mSide = side;
        }

        internal static IEnumerable<LaneBorder> Borders(Rectangle rectangle, int width, bool outside, Lane.Side? targetSide = null)
        {
            var ifOutside = outside ? 1 : 0;
            var ifInside = outside ? 0 : 1;
            var plusMinusMul = outside ? 1 : -1;

            var top = new Rectangle(
                rectangle.X - ifOutside * width,
                rectangle.Y - ifOutside * width,
                rectangle.Size.X + 2 * ifOutside * width,
                width);

            var left = new Rectangle(
                rectangle.X - ifOutside * width,
                rectangle.Y + ifInside * width,
                width,
                rectangle.Height - 2 * ifInside * width);

            var bottom = top;
            bottom.Offset(0, rectangle.Height + plusMinusMul * width);

            var right = left;
            right.Offset(rectangle.Width + plusMinusMul * width, 0);

            return new[]
            {
                new LaneBorder(top, false, outside ? RelativePosition.CenterBottom : RelativePosition.CenterTop),
                new LaneBorder(left, targetSide == Lane.Side.Left, outside ? RelativePosition.CenterRight : RelativePosition.CenterLeft),
                new LaneBorder(bottom, false, outside ? RelativePosition.CenterTop : RelativePosition.CenterBottom),
                new LaneBorder(right, targetSide == Lane.Side.Right, outside ? RelativePosition.CenterLeft : RelativePosition.CenterRight)
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