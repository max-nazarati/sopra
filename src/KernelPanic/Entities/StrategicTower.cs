﻿using System;
using System.Collections.Generic;
using System.Linq;
using KernelPanic.Data;
using KernelPanic.Input;
using KernelPanic.Interface;
using KernelPanic.Players;
using KernelPanic.Sprites;
using KernelPanic.Table;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace KernelPanic.Entities
{
    internal abstract class StrategicTower : Tower
    {
        [JsonProperty]
        private TowerStrategy mStrategy = TowerStrategy.First;

        [JsonIgnore] protected List<Projectile> Projectiles { get; private set; } = new List<Projectile>();

        /// <summary>
        /// <c>true</c> if the tower should be rotated in the direction of the unit.
        /// </summary>
        protected abstract bool WantsRotation { get; }
        
        protected Unit NextTarget { get; private set; }

        protected StrategicTower(int price, float radius, TimeSpan cooldown, Sprite sprite, SpriteManager spriteManager, SoundManager sounds)
            : base(price, radius, cooldown, sprite, spriteManager, sounds)
        {
            FireTimer.CooledDown += ShootNow;
        }

        protected override void CompleteClone()
        {
            base.CompleteClone();
            Projectiles = new List<Projectile>();
            FireTimer.CooledDown += ShootNow;
        }

        #region Shooting

        protected abstract Projectile CreateProjectile(Vector2 direction);
        
        private void ShootNow(CooldownComponent timer)
        {
            if (NextTarget == null)
                return;

            var direction = new Vector2(
                (float) Math.Sin(Sprite.Rotation % (Math.PI * 2)),
                -(float) Math.Cos(Sprite.Rotation % (Math.PI * 2)));
            Projectiles.Add(CreateProjectile(direction));
            timer.Reset();
        }

        #endregion

        #region Actions

        protected override IEnumerable<IAction> Actions(Player owner) =>
            base.Actions(owner).Extend(
                new StrategyAction(this, TowerStrategy.First, SpriteManager),
                new StrategyAction(this, TowerStrategy.Strongest, SpriteManager),
                new StrategyAction(this, TowerStrategy.Weakest, SpriteManager)
            );

        private sealed class StrategyAction : IAction
        {
            public Button Button { get; }
            private readonly Func<bool> mIsCurrentStrategy;

            internal StrategyAction(StrategicTower tower, TowerStrategy strategy, SpriteManager spriteManager)
            {
                Button = new TextButton(spriteManager) {Title = strategy.ToString()};
                Button.Clicked += (button, input) => tower.mStrategy = strategy;
                mIsCurrentStrategy = () => tower.mStrategy == strategy;
            }

            void IUpdatable.Update(InputManager inputManager, GameTime gameTime)
            {
                Button.Enabled = !mIsCurrentStrategy();
                Button.Update(inputManager, gameTime);
            }

            void IDrawable.Draw(SpriteBatch spriteBatch, GameTime gameTime) =>
                Button.Draw(spriteBatch, gameTime);
        }

        #endregion

        #region Updating

        internal override void Update(PositionProvider positionProvider, GameTime gameTime, InputManager inputManager)
        {
            base.Update(positionProvider, gameTime, inputManager);
            ChooseTarget(positionProvider);
            RotateToTarget();

            FireTimer.Update(gameTime);

            foreach (var projectile in Projectiles)
            {
                projectile.Update(positionProvider);
            }

            Projectiles.RemoveAll(projectile => projectile.mHasHit);
        }

        private void RotateToTarget()
        {
            if (NextTarget == null || !WantsRotation)
                return;

            Sprite.Rotation = (Sprite.Position - NextTarget.Sprite.Position).Angle(-0.5);
        }

        private void ChooseTarget(PositionProvider positionProvider)
        {
            // Are smaller or larger values better for the current strategy?
            var (ordering, identity) = PreferredStrategyOrdering();

            // An explicit "no unit found" element because Aggregate throws if the enumerable is empty.
            (Unit unit, int measure) noUnit = (null, identity);
            
            // Get all near entities, skipping those for which StrategyMeasure returns null.
            var measuredUnits = positionProvider
                    .NearEntities<Unit>(this, Radius)
                    .SelectMany(unit =>
                        StrategyMeasure(positionProvider, unit) is int measure
                            ? new[] {(unit: unit, measure: measure)}
                            : Enumerable.Empty<(Unit unit, int measure)>()
                    );

            // Remember the target we had before, in case there is no better target don't switch.
            var currentTarget = NextTarget;

            // Determine the best target.
            NextTarget = measuredUnits.Aggregate(noUnit, (a, b) =>
            {
                var comparison = a.measure.CompareTo(b.measure);
                return comparison == 0
                    ? a.unit == currentTarget ? a : b
                    : comparison == ordering ? a : b;
            }).unit;

            if (NextTarget != null)
            {
                FireTimer.Enabled = true;
            }
        }

        private (int ordering, int identity) PreferredStrategyOrdering()
        {
            switch (mStrategy)
            {
                case TowerStrategy.First:
                    return (-1, int.MaxValue);    // Smaller values are better, start with the largest possible.
                case TowerStrategy.Strongest:
                    return (1, int.MinValue);     // Larger values are better, start with the smallest possible.
                case TowerStrategy.Weakest:
                    return (-1, int.MaxValue);    // Smaller values are better, start with the largest possible.
                default:
                    throw new InvalidOperationException("Invalid Strategy " + mStrategy);
            }
        }

        private int? StrategyMeasure(PositionProvider positionProvider, Unit unit)
        {
            switch (mStrategy)
            {
                case TowerStrategy.First:
                    var tile = positionProvider.Grid.TileFromWorldPoint(unit.Sprite.Position);
                    var heat = tile is TileIndex tileIndex ? positionProvider.TileHeat(tileIndex.ToPoint()) : null;
                    return heat;

                case TowerStrategy.Strongest:
                    return unit.RemainingLife;

                case TowerStrategy.Weakest:
                    return unit.RemainingLife;

                default:
                    throw new InvalidOperationException("Invalid Strategy " + mStrategy);
            }
        }

        #endregion

        internal override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch, gameTime);

            foreach (var projectile in Projectiles)
            {
                projectile.Draw(spriteBatch, gameTime);
            }
        }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    internal enum TowerStrategy { First, Strongest, Weakest };
}