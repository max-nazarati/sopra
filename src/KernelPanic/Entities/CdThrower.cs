using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;

namespace KernelPanic.Entities
{
    // This is instantiated via black magic originating from Building.Create.
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    internal sealed class CdThrower : StrategicTower
    {
        protected override bool WantsRotation => true;

        internal bool ShootsBoomerang { private get; set; }

        internal CdThrower(SpriteManager spriteManager, SoundManager sounds)
            : base(20, 4, TimeSpan.FromSeconds(2), spriteManager.CreateCdThrower(), spriteManager, sounds)
        {
        }

        protected override Projectile CreateProjectile(Vector2 direction)
        {
            return new Projectile(direction,
                Sprite.Position,
                Radius,
                Sprite.Rotation,
                40,
                7,
                1,
                SpriteManager.CreateCdProjectile());
        }
    }
}