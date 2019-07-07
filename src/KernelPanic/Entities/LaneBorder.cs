using System.Collections.Generic;
using KernelPanic.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic.Entities
{
    internal struct LaneBorder : IGameObject
    {
        public Rectangle Bounds { get; }

        private LaneBorder(Rectangle bounds)
        {
            Bounds = bounds;
        }

        internal static IEnumerable<LaneBorder> Borders(Rectangle rectangle, int width, bool outside)
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

            return new[] {new LaneBorder(top), new LaneBorder(left), new LaneBorder(bottom), new LaneBorder(right)};
        }

        int? IGameObject.DrawLevel => null;
        bool IGameObject.WantsRemoval => false;

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