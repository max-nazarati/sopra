using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using KernelPanic.Entities.Buildings;
using KernelPanic.Entities.Projectiles;
using KernelPanic.Input;
using KernelPanic.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using KernelPanic.Table;

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
        }
        
        protected override void CompleteClone()
        {
            base.CompleteClone();
            Cooldown = new CooldownComponent(Cooldown.Cooldown, false);
            Cooldown.CooledDown += component => AbilityStatus = AbilityState.Ready;
        }

        #region Ability 

        protected override void IndicateAbility(PositionProvider positionProvider, InputManager inputManager)
        {
            // find nearest Tower in Range
            mAbilityTargetOne = null;
            mAbilityTargetTwo = null;
            double shortestDistance = mAbilityRange + 1;
            double secondShortestDistance = mAbilityRange + 2;
            foreach (var tower in positionProvider.NearEntities<Tower>(Sprite.Position, mAbilityRange))
            {
                var distance = Vector2.Distance(tower.Sprite.Position, Sprite.Position);
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
                var empOne = new Emp(first, TimeSpan.FromSeconds(EmpDuration * mEmpDurationAmplifier), mEmpSprite.Clone());
                positionProvider.AddProjectile(empOne);
            }

            if (mAbilityTargetTwo is Tower second && TargetsTwoTower)
            {
                var empTwo = new Emp(second, TimeSpan.FromSeconds(EmpDuration * mEmpDurationAmplifier), mEmpSprite.Clone());
                positionProvider.AddProjectile(empTwo);
            }
        }

        protected override void ContinueAbility(PositionProvider positionProvider, GameTime gameTime)
        {
            AbilityStatus = AbilityState.Finished;
        }

        protected override void UpdateCooldown(GameTime gameTime, PositionProvider positionProvider)
        {
            var currTile = positionProvider.RequireTile(this);
            if (InBase(currTile, positionProvider))
            {
                base.UpdateCooldown(gameTime, positionProvider);
            }
        }

        #endregion Ability

        private static bool InBase(TileIndex tile, PositionProvider positionProvider)
        {
            var inBase = positionProvider.Grid.LaneSide == Lane.Side.Left ? positionProvider.Grid.LaneRectangle.Width - tile.Column == 2
                                                                            && tile.Row < Grid.LaneWidthInTiles :
                tile.Column == 1 && tile.Row > Grid.LaneWidthInTiles;
            return inBase;
        }

        protected override void AutonomousAttack(InputManager inputManager, PositionProvider positionProvider)
        {
            var currTile = positionProvider.RequireTile(this);

            if (!Cooldown.Ready)
            {
                // wait for the ability to be ready again
                if (InBase(currTile, positionProvider))
                {
                    return;
                }

                // walk to the own base to refill
                var basePositionX = positionProvider.Grid.LaneSide == Lane.Side.Left ? positionProvider.Grid.LaneRectangle.Width - 2 : 1;
                var basePositionY = positionProvider.Grid.LaneSide == Lane.Side.Left ? Grid.LaneWidthInTiles - 1 : 1;
                var translation = positionProvider.Grid.LaneSide == Lane.Side.Left ? -1 : 1;
                var basePosition = new Point[Grid.LaneWidthInTiles];
                for (var i = 0; i < Grid.LaneWidthInTiles; i++)
                {
                    basePosition[i] = new Point(basePositionX, basePositionY + i * translation);
                }
                mAStar = positionProvider.MakePathFinding(this, basePosition);
                mTarget = new TileIndex(mAStar.Path[mAStar.Path.Count - 1], 1);
                ShouldMove = true;

            }
            else
            {
                if (positionProvider.NearEntities<Tower>(this, mAbilityRange - 100).Any())
                {
                    IndicateAbility(positionProvider, inputManager);
                    StartAbility(positionProvider, inputManager);
                }
                else
                {
                    base.AutonomousAttack(inputManager, positionProvider);
                }
                // Other strategy: search for a tower on adjacent tiles
                // var neighbours = GetNeighbours(positionProvider);
            }
        }

        
        #region Draw
        
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