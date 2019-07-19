using System;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using KernelPanic.Events;
using KernelPanic.Input;
using Microsoft.Xna.Framework;

namespace KernelPanic.Entities.Buildings
{
    // This is instantiated via black magic originating from Building.Create.
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    internal sealed class ShockField : Tower
    {
        internal ShockField(SpriteManager spriteManager)
            : base(100, 1, 2, 0,TimeSpan.FromSeconds(3), spriteManager.CreateShockField(), spriteManager)
        {
            FireTimer.Enabled = true;
        }

        internal override void UpdateInformation()
        {
            base.UpdateInformation();
            mInfoText.Text += $"\nStärke: {Damage}";
        }

        public override void Update(PositionProvider positionProvider, InputManager inputManager, GameTime gameTime)
        {
            base.Update(positionProvider, inputManager, gameTime);
            if (FireTimer.RemainingCooldown.Milliseconds != 0) return;
            foreach(var unit in positionProvider.NearEntities<Unit>(this, Radius))
            {
                if (!Bounds.Intersects(unit.Bounds))
                    continue;

                unit.DealDamage(Damage, positionProvider);
                EventCenter.Default.Send(Event.ProjectileShot(this));
                FireTimer.Reset();
            }
        }
    }
}
