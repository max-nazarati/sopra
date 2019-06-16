using System.ComponentModel;
using Microsoft.Xna.Framework;

namespace KernelPanic.Data
{
    internal enum RelativePosition
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,
        CenterLeft,
        CenterRight,
        CenterTop,
        CenterBottom,
        Center
    }
    
    internal static class RelativePositionImpl
    {
        internal static RelativePosition MirrorVertical(this RelativePosition position)
        {
            switch (position)
            {
                case RelativePosition.TopLeft:
                    return RelativePosition.BottomLeft;
                case RelativePosition.TopRight:
                    return RelativePosition.BottomRight;
                case RelativePosition.BottomLeft:
                    return RelativePosition.TopLeft;
                case RelativePosition.BottomRight:
                    return RelativePosition.TopRight;
                case RelativePosition.CenterTop:
                    return RelativePosition.CenterBottom;
                case RelativePosition.CenterBottom:
                    return RelativePosition.CenterTop;
                default:
                    return position;
            }
        }
        
        internal static Vector2 RectangleOrigin(this RelativePosition position, Vector2 rectangle)
        {
            switch (position)
            {
                case RelativePosition.TopLeft:
                    return Vector2.Zero;
                case RelativePosition.TopRight:
                    return new Vector2(rectangle.X, 0);
                case RelativePosition.BottomLeft:
                    return new Vector2(0, rectangle.Y);
                case RelativePosition.BottomRight:
                    return new Vector2(rectangle.X, rectangle.Y);
                case RelativePosition.Center:
                    return rectangle * 0.5f;
                case RelativePosition.CenterLeft:
                    return new Vector2(0, rectangle.Y / 2);
                case RelativePosition.CenterRight:
                    return new Vector2(rectangle.X, rectangle.Y / 2);
                case RelativePosition.CenterTop:
                    return new Vector2(rectangle.X / 2, 0);
                case RelativePosition.CenterBottom:
                    return new Vector2(rectangle.X / 2, rectangle.Y);
                default:
                    throw new InvalidEnumArgumentException(nameof(position), (int) position, typeof(RelativePosition));
            }
        }
    }
}