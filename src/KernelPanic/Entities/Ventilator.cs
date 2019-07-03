using System;
using System.Collections.Generic;
using KernelPanic.Data;
using KernelPanic.Input;
using KernelPanic.Sprites;
using Microsoft.Xna.Framework;

namespace KernelPanic.Entities
{
    internal class Ventilator : StrategicTower
    {
        private List<Unit> mSlowedDownUnits, mSlowedDownUnitsOld;
        
        internal Ventilator(SpriteManager spriteManager
            , SoundManager sounds) : base(20, 2, new TimeSpan(0,0,0)
            ,  spriteManager.CreateVentilator(), spriteManager, sounds)
        {
            mSlowedDownUnits = new List<Unit>();
            mSlowedDownUnitsOld = new List<Unit>();
            mRadiusSprite = spriteManager.CreateTowerRadiusIndicator(Radius);
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