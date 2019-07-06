using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using KernelPanic.Entities.Projectiles;
using Microsoft.Xna.Framework;

namespace KernelPanic.Entities.Buildings
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

        protected override IEnumerable<Projectile> CreateProjectiles(Vector2 direction)
        {
            var image = SpriteManager.CreateCursorShooter();
            image.ScaleToWidth(20);
            yield return new Projectile(direction, Sprite.Position, Radius, 15, 2, image)
            {
                SingleTarget = true
            };
        }
    }
}