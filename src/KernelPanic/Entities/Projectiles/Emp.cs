using KernelPanic.Entities.Buildings;
using KernelPanic.Sprites;
using Microsoft.Xna.Framework;

namespace KernelPanic.Entities.Projectiles
{
    internal sealed class Emp : Projectile
    {
        internal Emp(Tower origin, ImageSprite sprite) : base(origin, Vector2.Zero, 0, sprite, 0)
        {
        }
    }
}