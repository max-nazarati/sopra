using System;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using KernelPanic.Input;
using Microsoft.Xna.Framework;

namespace KernelPanic.Entities.Buildings
{
    // This is instantiated via black magic originating from Building.Create.
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    internal sealed class ShockField : Tower
    {
        private List<Unit> mDamagedUnits;
        internal ShockField(SpriteManager spriteManager)
            : base(1, 1, 2, 0,TimeSpan.FromSeconds(3), spriteManager.CreateShockField(), spriteManager)
        {
            // The fire timer is not used by the Shockfield.
            mDamagedUnits = new List<Unit>();
            FireTimer.Enabled = false;
        }

        internal override void UpdateInformation()
        {
            base.UpdateInformation();
            mInfoText.Text += $"\nStärke: {Damage}";
        }

        protected override void CompleteClone()
        {
            base.CompleteClone();
            mDamagedUnits = new List<Unit>();
        }

        public override void Update(PositionProvider positionProvider, InputManager inputManager, GameTime gameTime)
        {
            base.Update(positionProvider, inputManager, gameTime);

            foreach(var unit in positionProvider.NearEntities<Unit>(this, Radius))
            {
                if (this.Bounds.Intersects(unit.Bounds) && !mDamagedUnits.Contains(unit))
                {
                    unit.DealDamage(Damage);
                    mDamagedUnits.Add(unit);
                }
            }
        }
    }
}
