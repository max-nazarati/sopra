using System;
using System.Diagnostics.CodeAnalysis;
using KernelPanic.Entities.Buildings;
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
        private readonly int mAbilityRange;
        private readonly ImageSprite mIndicatorRange;
        private readonly ImageSprite mIndicatorTarget;
        private readonly ImageSprite mEmpSprite;
        private Tower mAbilityTargetOne;
        private Tower mAbilityTargetTwo;

        private Emp[] mEmps;

        private static Point HitBoxSize => new Point(64, 64);

        #region Upgrades
        internal bool TargetsTwoTower { private get; set; }
        private const double EmpDuration = 5;
        internal float mEmpDurationAmplifier = 1;
        
        #endregion
        
        internal Bluescreen(SpriteManager spriteManager)
            : base(50, 6, 15, 0, TimeSpan.FromSeconds(1), HitBoxSize, spriteManager.CreateBluescreen(), spriteManager)
        {
            mAbilityRange = 1000;
            mIndicatorRange = spriteManager.CreateEmpIndicatorRange(mAbilityRange);
            mIndicatorTarget = spriteManager.CreateEmpIndicatorTarget();
            mEmpSprite = spriteManager.CreateEmp();

            mEmps = new Emp[2];
        }
        
        protected override void CompleteClone()
        {
            base.CompleteClone();
            mEmps = new Emp[2];
            Cooldown = new CooldownComponent(Cooldown.Cooldown, false);
            Cooldown.CooledDown += component => AbilityStatus = AbilityState.Ready;
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
            foreach (var tower in positionProvider.NearEntities<Tower>(Sprite.Position, mAbilityRange))
            {
                var distance = Distance(tower.Sprite.Position, Sprite.Position);
                if (distance < shortestDistance)
                {
                    // shift the old closest turret to the second place
                    secondShortestDistance = shortestDistance;
                    mAbilityTargetTwo = mAbilityTargetOne;
                    
                    // set the new closest turret
                    shortestDistance = distance;
                    mAbilityTargetOne = tower;
                }
                else if (distance < secondShortestDistance)
                {
                    // just replace the second place
                    secondShortestDistance = distance;
                    mAbilityTargetTwo = tower;
                }
            }

            base.IndicateAbility(positionProvider, inputManager);
            
        }

        protected override void StartAbility(PositionProvider positionProvider, InputManager inputManager)
        {
            // debug
            base.StartAbility(positionProvider, inputManager);
            
            if (mAbilityTargetOne is Tower first)
            {
                var empOne = new Emp(first, TimeSpan.FromSeconds(EmpDuration * mEmpDurationAmplifier), mEmpSprite);
                positionProvider.AddProjectile(empOne);
            }

            if (mAbilityTargetTwo is Tower second && TargetsTwoTower)
            {
                var empTwo = new Emp(second, TimeSpan.FromSeconds(EmpDuration * mEmpDurationAmplifier), mEmpSprite);
                positionProvider.AddProjectile(empTwo);
            }
        }

        protected override void ContinueAbility(PositionProvider positionProvider, GameTime gameTime)
        {
            AbilityStatus = AbilityState.Finished;
        }
        
        protected override void FinishAbility(PositionProvider positionProvider)
        {
            base.FinishAbility(positionProvider);
            // Projectiles in mEmp will clear themselves and should not be deleted here
        }
        
        #endregion Ability

        #region Update

        public override void Update(PositionProvider positionProvider, InputManager inputManager, GameTime gameTime)
        {
            base.Update(positionProvider, inputManager, gameTime);
            if (mEmps[0] is Emp empOne)
            {
                empOne.Update(positionProvider, inputManager, gameTime);
            }
            if (mEmps[1] is Emp empTwo)
            {
                empTwo.Update(positionProvider, inputManager, gameTime);
            }
        }

        #endregion
        
        #region Draw
        
        protected override void DrawAbility(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.DrawAbility(spriteBatch, gameTime);
            if (AbilityStatus == AbilityState.Indicating)
            {
                DrawIndicator(spriteBatch, gameTime);
            }
            
            // Drawing the Emps needs to get moved, so the emp doesnt die together with the bluescreen
            if (mEmps[0] is Emp empOne)
            {
                empOne.Draw(spriteBatch, gameTime);
            }
            if (mEmps[1] is Emp empTwo)
            {
                empTwo.Draw(spriteBatch, gameTime);
            }
        }

        private void DrawIndicator(SpriteBatch spriteBatch, GameTime gameTime)
        {
            mIndicatorRange.Position = Sprite.Position;
            mIndicatorRange.Draw(spriteBatch, gameTime);

            if (mAbilityTargetOne != null)
            {
                mIndicatorTarget.Position = mAbilityTargetOne.Sprite.Position;
                mIndicatorTarget.Draw(spriteBatch, gameTime);
            }

            if (mAbilityTargetTwo != null && TargetsTwoTower)
            {
                mIndicatorTarget.Position = mAbilityTargetTwo.Sprite.Position;
                mIndicatorTarget.Draw(spriteBatch, gameTime);
            }
        }
        
        #endregion
    }
}