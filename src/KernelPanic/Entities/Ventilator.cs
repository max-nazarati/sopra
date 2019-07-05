using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using KernelPanic.Input;
using Microsoft.Xna.Framework;

namespace KernelPanic.Entities
{
    // This is instantiated via black magic originating from Building.Create.
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    internal sealed class Ventilator : Tower
    {
        private readonly List<Unit> mSlowedDownUnits;
        private List<Unit> mSlowedDownUnitsOld;

        internal Ventilator(SpriteManager spriteManager, SoundManager sounds)
            : base(20, 2, TimeSpan.Zero, spriteManager.CreateVentilator(), spriteManager, sounds)
        {
            mSlowedDownUnits = new List<Unit>();
            mSlowedDownUnitsOld = new List<Unit>();
        }

        internal override void Update(PositionProvider positionProvider, GameTime gameTime, InputManager inputManager)
        {
            base.Update(positionProvider, gameTime, inputManager);
            mSlowedDownUnits.Clear();
            foreach (var entity in positionProvider.NearEntities<Unit>(Sprite.Position, 200))
            {
                if (!(Vector2.Distance(Sprite.Position, entity.Sprite.Position) <= 200)) continue;
                entity.mIsSlower = true;
                mSlowedDownUnits.Add(entity);
            }

            foreach (var unit in mSlowedDownUnitsOld)
            {
                if (!mSlowedDownUnits.Contains(unit))
                {
                    unit.mIsSlower = false;
                }
            }
            mSlowedDownUnitsOld = new List<Unit>(mSlowedDownUnits);
        }
    }
}