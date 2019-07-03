using System;
using KernelPanic.Input;
using KernelPanic.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic.Entities
{
    internal sealed class Bluescreen : Hero
    {
        private readonly ImageSprite mIndicatorRange;
        private ImageSprite mIndicatorTarget;
        private Vector2 mAbilityTarget;
        private TimeSpan mAbilityDurationTotal;
        private TimeSpan mAbilityDurationLeft;
        
        internal Bluescreen(SpriteManager spriteManager)
            : base(50, 9, 15, 0, TimeSpan.FromSeconds(5), spriteManager.CreateBluescreen(), spriteManager)
        {
            var mAbilityRange = 1000;
            mIndicatorRange = spriteManager.CreateEmpIndicatorRange(mAbilityRange);
            mIndicatorTarget = spriteManager.CreateEmpIndicatorTarget();
            mAbilityDurationTotal = TimeSpan.FromSeconds(2);
            mAbilityDurationLeft = TimeSpan.Zero;
        }
        
        #region Ability 

        protected override void IndicateAbility(InputManager inputManager)
        {
            // find nearest Tower in Range
            mAbilityTarget = Sprite.Position;
            base.IndicateAbility(inputManager);
            
        }

        protected override void StartAbility(PositionProvider positionProvider, InputManager inputManager)
        {
            // debug
            base.StartAbility(positionProvider, inputManager);
            mAbilityDurationLeft = mAbilityDurationTotal;
        }

        protected override void ContinueAbility(GameTime gameTime)
        {
            mAbilityDurationLeft -= gameTime.ElapsedGameTime;
            if (mAbilityDurationLeft > TimeSpan.Zero)
            {
                
            }
            else
            {
                AbilityStatus = AbilityState.Finished;
            }

        }
        #endregion Ability

        protected override void DrawAbility(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.DrawAbility(spriteBatch, gameTime);
            if (AbilityStatus == AbilityState.Indicating)
            {
                DrawIndicator(spriteBatch, gameTime);
            }
        }

        private void DrawIndicator(SpriteBatch spriteBatch, GameTime gameTime)
        {
            // var direction = mAbilityTarget - Sprite.Position;
            // direction.Normalize();
            // var rotation = direction.Angle(0.5);

            mIndicatorRange.Position = Sprite.Position;
            // mIndicator.Rotation = rotation;
            // mIndicator.ScaleToHeight(JumpDuration * JumpSegmentLength);
            mIndicatorRange.Draw(spriteBatch, gameTime);
        }
    }
}