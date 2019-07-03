using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;

namespace KernelPanic.Entities
{
    // This is instantiated via black magic originating from Building.Create.
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    internal sealed class Antivirus : StrategicTower
    {
        protected override bool WantsRotation => false;

        internal Antivirus(SpriteManager spriteManager, SoundManager sounds)
            : base(30, 5, TimeSpan.FromSeconds(3), spriteManager.CreateAntivirus(), spriteManager, sounds)
        {
        }

        protected override Projectile CreateProjectile(Vector2 direction)
        {
            return new Projectile(direction,
                Sprite.Position,
                Radius,
                20,
                15,
                2,
                SpriteManager.CreateCursorShooter());
        }
    }
}