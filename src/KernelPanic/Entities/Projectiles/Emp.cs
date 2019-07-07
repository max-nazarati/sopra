using KernelPanic.Sprites;
using Microsoft.Xna.Framework;

namespace KernelPanic.Entities.Projectiles
{
    internal class Emp : Projectile
    {
        public Emp(Vector2 startPoint, ImageSprite sprite) : base(Vector2.Zero, startPoint, 1, 0, 0, sprite)
        {
        }
    }
}