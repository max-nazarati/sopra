using System;
using System.Diagnostics.CodeAnalysis;
using KernelPanic.Input;
using Microsoft.Xna.Framework;

namespace KernelPanic.Entities
{
    // This is instantiated via black magic originating from Building.Create.
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    internal sealed class Ventilator : Tower
    {
        internal Ventilator(SpriteManager spriteManager, SoundManager sounds)
            : base(20, 2, TimeSpan.Zero, spriteManager.CreateVentilator(), spriteManager, sounds)
        {
            // The fire timer is not used by the Ventilator.
            FireTimer.Enabled = false;
        }

        public override void Update(PositionProvider positionProvider, InputManager inputManager, GameTime gameTime)
        {
            base.Update(positionProvider, inputManager, gameTime);

            foreach (var unit in positionProvider.NearEntities<Unit>(this, Radius))
            {
                unit.SlowDownForFrame();
            }
        }
    }
}