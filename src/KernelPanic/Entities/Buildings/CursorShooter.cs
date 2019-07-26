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
    internal sealed class CursorShooter : StrategicTower
    {
        protected override bool WantsRotation => true;
        private bool mDoubleClick;

        internal CursorShooter(SpriteManager spriteManager)
            : base(20, 4, 2, 10,TimeSpan.FromSeconds(1), spriteManager.CreateCursorShooter(), spriteManager)
        {
        }

        protected override IEnumerable<Projectile> CreateProjectiles(Vector2 direction)
        {
            EventCenter.Default.Send(Event.ProjectileShot(this));
            if (mDoubleClick)
            {
                for (var i = -0.1; i < 0.2; i += 0.2)
                {
                    var x = (float) (Math.Cos(i) * direction.X - Math.Sin(i) * direction.Y);
                    var y = (float) (Math.Sin(i) * direction.X + Math.Cos(i) * direction.Y);


                    var newDirection = new Vector2(x, y);
                    yield return new Projectile(this, newDirection, SpriteManager.CreateCursorProjectile(), 30)
                    {
                        SingleTarget = true
                    };
                }
            }
            else
            {
                yield return new Projectile(this, direction, SpriteManager.CreateCursorProjectile())
                {
                    SingleTarget = true
                };
            }
        }

        internal void ActivateDoubleClick()
        {
            mDoubleClick = true;
        }
    }
}