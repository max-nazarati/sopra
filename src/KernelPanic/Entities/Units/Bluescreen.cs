using System;
using System.Diagnostics.CodeAnalysis;
using KernelPanic.Entities.Projectiles;
using KernelPanic.Input;
using KernelPanic.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic.Entities.Units
{
    // This is instantiated via black magic originating from Unit.Create.
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    internal sealed class Bluescreen : Hero
    {
        private readonly ImageSprite mIndicatorRange;
        private ImageSprite mIndicatorTarget;
        private ImageSprite mEmpSprite;
        private Vector2? mAbilityTargetOne;
        private Vector2? mAbilityTargetTwo;
        private TimeSpan mAbilityDurationTotal;
        private TimeSpan mAbilityDurationLeft;
        private readonly int mAbilityRange;
        private Emp[] mEmps;
        
        internal bool TargetsTwoTower { private get; set; } = true;

        internal Bluescreen(SpriteManager spriteManager)
            : base(50, 9, 15, 0, TimeSpan.FromSeconds(5), spriteManager.CreateBluescreen(), spriteManager)
        {
            mAbilityRange = 1000;
            mIndicatorRange = spriteManager.CreateEmpIndicatorRange(mAbilityRange);
            mIndicatorTarget = spriteManager.CreateEmpIndicatorTarget();
            mEmpSprite = spriteManager.CreateEmp();
            mAbilityDurationTotal = TimeSpan.FromSeconds(2);
            mAbilityDurationLeft = TimeSpan.Zero;
            mEmps = new Emp[2];
        }
        
        #region Ability 

        private static double Distance(Vector2 a, Vector2 b)
        {
            return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }
            
        protected override void IndicateAbility(PositionProvider positionProvider, InputManager inputManager)
        {
            // find nearest Tower in Range
            mAbilityTargetOne = null;
            mAbilityTargetTwo = null;
            double shortestDistance = mAbilityRange + 1;
            double secondShortestDistance = mAbilityRange + 2;
            foreach (var building in positionProvider.NearEntities<Building>(Sprite.Position, mAbilityRange))
            {
                var distance = Distance(building.Sprite.Position, Sprite.Position);
                if (distance < shortestDistance)
                {
                    // shift the old closest turret to the second place
                    secondShortestDistance = shortestDistance;
                    mAbilityTargetTwo = mAbilityTargetOne;
                    
                    // set the new closest turret
                    shortestDistance = distance;
                    mAbilityTargetOne = building.Sprite.Position;
                }
                else if (distance < secondShortestDistance)
                {
                    // just replace the second place
                    secondShortestDistance = distance;
                    mAbilityTargetTwo = building.Sprite.Position;
                }
            }

            base.IndicateAbility(positionProvider, inputManager);
            
        }

        protected override void StartAbility(PositionProvider positionProvider, InputManager inputManager)
        {
            // debug
            base.StartAbility(positionProvider, inputManager);
            mAbilityDurationLeft = mAbilityDurationTotal;
            if (mAbilityTargetOne is Vector2 first)
            {
                mEmps[0] = new Emp(first, mEmpSprite);
            }

            if (mAbilityTargetTwo is Vector2 second)
            { 
                mEmps[1] = new Emp(second, mEmpSprite);
            }
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
        
        protected override void FinishAbility()
        {
            base.FinishAbility();
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
            mIndicatorRange.Position = Sprite.Position;
            mIndicatorRange.Draw(spriteBatch, gameTime);

            if (mAbilityTargetOne != null)
            {
                mIndicatorTarget.Position = (Vector2) mAbilityTargetOne;
                mIndicatorTarget.Draw(spriteBatch, gameTime);
            }

            if (mAbilityTargetTwo != null && TargetsTwoTower)
            {
                mIndicatorTarget.Position = (Vector2) mAbilityTargetTwo;
                mIndicatorTarget.Draw(spriteBatch, gameTime);
            }
        }
    }
}