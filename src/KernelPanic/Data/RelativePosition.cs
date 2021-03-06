using System.ComponentModel;
using Microsoft.Xna.Framework;

namespace KernelPanic.Data
{
    internal enum RelativePosition
    {
        // Important: This ordering shouldn't be changed because HeatMap.RoundToOctant relies on it!
        Center,
        CenterRight,
        BottomRight,
        CenterBottom,
        BottomLeft,
        CenterLeft,
        TopLeft,
        CenterTop,
        TopRight
    }
    
    internal static class RelativePositionImpl
    {
        /// <summary>
        /// Calculates the origin of the rectangle at (0,0) with size <paramref name="rectangleSize"/> such that the
        /// origin is at the position indicated by <paramref name="position"/>.
        /// </summary>
        /// <param name="position">Where to put the origin.</param>
        /// <param name="rectangleSize">The size of the rectangle.</param>
        /// <returns>The coordinates of the origin.</returns>
        /// <exception cref="InvalidEnumArgumentException">
        /// If <paramref name="position"/> is not one of the listed values of <see cref="RelativePosition"/>.
        /// </exception>
        internal static Vector2 RectangleOrigin(this RelativePosition position, Vector2 rectangleSize)
        {
            switch (position)
            {
                case RelativePosition.TopLeft:
                    return Vector2.Zero;
                case RelativePosition.TopRight:
                    return new Vector2(rectangleSize.X, 0);
                case RelativePosition.BottomLeft:
                    return new Vector2(0, rectangleSize.Y);
                case RelativePosition.BottomRight:
                    return new Vector2(rectangleSize.X, rectangleSize.Y);
                case RelativePosition.Center:
                    return rectangleSize * 0.5f;
                case RelativePosition.CenterLeft:
                    return new Vector2(0, rectangleSize.Y / 2);
                case RelativePosition.CenterRight:
                    return new Vector2(rectangleSize.X, rectangleSize.Y / 2);
                case RelativePosition.CenterTop:
                    return new Vector2(rectangleSize.X / 2, 0);
                case RelativePosition.CenterBottom:
                    return new Vector2(rectangleSize.X / 2, rectangleSize.Y);
                default:
                    throw new InvalidEnumArgumentException(nameof(position), (int) position, typeof(RelativePosition));
            }
        }

        /// <summary>
        /// Calculates the point on the border of <paramref name="rectangle"/> indicated by <paramref name="position"/>.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <param name="position">The position of the point.</param>
        /// <returns>Returns the point as a <see cref="Vector2"/> because centers might lie between points.</returns>
        internal static Vector2 At(this Rectangle rectangle, RelativePosition position)
        {
            return rectangle.Location.ToVector2() + position.RectangleOrigin(rectangle.Size.ToVector2());
        }
    }
}