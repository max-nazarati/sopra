using Microsoft.Xna.Framework;

namespace KernelPanic
{
    internal interface IPositioned
    {
        Vector2 Position { get; set; }

        Vector2 Size { get; }
    }
}
