using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;

namespace KernelPanic.Entities
{
    // This is instantiated via black magic originating from Building.Create.
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    internal sealed class CursorShooter : StrategicTower
    {
        protected override bool WantsRotation => true;

        internal CursorShooter(SpriteManager spriteManager, SoundManager sounds)
            : base(20, 4, TimeSpan.FromSeconds(1), spriteManager.CreateCursorShooter(), spriteManager, sounds)
        {
        }

        protected override Projectile CreateProjectile(Vector2 direction)
        {
            return new Projectile(direction,
                Sprite.Position,
                Radius,
                Sprite.Rotation,
                20,
                10,
                2,
                SpriteManager.CreateCursorProjectile());
        }
    }
}