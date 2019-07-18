using System;
using System.Diagnostics.CodeAnalysis;
using KernelPanic.Data;
using KernelPanic.Input;
using KernelPanic.Sprites;
using Microsoft.Xna.Framework;

namespace KernelPanic.Entities.Buildings
{
    // This is instantiated via black magic originating from Building.Create.
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    internal sealed class Ventilator : Tower
    {
        internal Ventilator(SpriteManager spriteManager)
            : base(50, 2, 0, 0,TimeSpan.Zero, spriteManager.CreateVentilator(), spriteManager)
        {
            // The fire timer is not used by the Ventilator.
            FireTimer.Enabled = false;
        }

        public override void Update(PositionProvider positionProvider, InputManager inputManager, GameTime gameTime)
        {
            base.Update(positionProvider, inputManager, gameTime);
            Sprite.Rotation = (float) (gameTime.TotalGameTime.TotalSeconds * 5 % (2 * Math.PI));

            if (State != BuildingState.Active)
                return;

            foreach (var unit in positionProvider.NearEntities<Unit>(this, Radius))
            {
                unit.SlowDownForFrame();
            }
        }
    }
}