using System;
using Accord.Math;
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
        private Vector2? mAbilityTarget;
        private TimeSpan mAbilityDurationTotal;
        private TimeSpan mAbilityDurationLeft;
        private readonly int mAbilityRange;

        internal Bluescreen(SpriteManager spriteManager)
            : base(50, 9, 15, 0, TimeSpan.FromSeconds(5), spriteManager.CreateBluescreen(), spriteManager)
        {
            mAbilityRange = 1000;
            mIndicatorRange = spriteManager.CreateEmpIndicatorRange(mAbilityRange);
            mIndicatorTarget = spriteManager.CreateEmpIndicatorTarget();
            mAbilityDurationTotal = TimeSpan.FromSeconds(2);
            mAbilityDurationLeft = TimeSpan.Zero;
        }
        
        #region Ability 

        private static double Distance(Vector2 a, Vector2 b)
        {
            return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }
            
        protected override void IndicateAbility(PositionProvider positionProvider, InputManager inputManager)
        {
            // find nearest Tower in Range
            mAbilityTarget = null;
            double shortestDistance = mAbilityRange + 1;
            foreach (var building in positionProvider.NearEntities<Building>(Sprite.Position, mAbilityRange))
            {
                var distance = Distance(building.Sprite.Position, Sprite.Position);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    mAbilityTarget = building.Sprite.Position;
                }
            }

            base.IndicateAbility(positionProvider, inputManager);
            
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

            if (mAbilityTarget != null)
            {
                mIndicatorTarget.Position = (Vector2) mAbilityTarget;
                mIndicatorTarget.Draw(spriteBatch, gameTime);
            }
        }
    }
}