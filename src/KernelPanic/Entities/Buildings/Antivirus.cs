using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using KernelPanic.Entities.Projectiles;
using KernelPanic.Events;
using Microsoft.Xna.Framework;

namespace KernelPanic.Entities.Buildings
{
    // This is instantiated via black magic originating from Building.Create.
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    internal sealed class Antivirus : StrategicTower
    {
        protected override bool WantsRotation => false;

        internal Antivirus(SpriteManager spriteManager)
            : base(50, 5.5f, 9, 15,TimeSpan.FromSeconds(2), spriteManager.CreateAntivirus(), spriteManager)
        {
        }

        protected override IEnumerable<Projectile> CreateProjectiles(Vector2 direction)
        {
            var image = SpriteManager.CreateUmbrellaProjectile();
            EventCenter.Default.Send(Event.ProjectileShot(this));
            yield return new Projectile(this, direction, image)
            {
                SingleTarget = true
            };
        }
    }
}