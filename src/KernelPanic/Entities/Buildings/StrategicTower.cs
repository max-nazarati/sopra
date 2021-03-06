﻿using System;
using System.Collections.Generic;
using System.Linq;
using KernelPanic.Data;
using KernelPanic.Entities.Projectiles;
using KernelPanic.Input;
using KernelPanic.Interface;
using KernelPanic.Players;
using KernelPanic.Purchasing;
using KernelPanic.Sprites;
using KernelPanic.Table;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace KernelPanic.Entities.Buildings
{
    internal abstract class StrategicTower : Tower
    {
        [JsonProperty]
        private TowerStrategy mStrategy = TowerStrategy.First;

        private TowerLevel mLevel = TowerLevel.Zero;
        private ImageSprite[] mLevelSprites;

        /// <summary>
        /// <c>true</c> if the tower should be rotated in the direction of the unit.
        /// </summary>
        [JsonIgnore]
        protected abstract bool WantsRotation { get; }

        [JsonIgnore]
        private Unit NextTarget { get; set; }

        protected StrategicTower(int price,
            float radius,
            int damage,
            int speed,
            TimeSpan cooldown,
            Sprite sprite,
            SpriteManager spriteManager)
            : base(price, radius, damage, (int) (1.5f * speed), cooldown, sprite, spriteManager)
        {
            FireTimer.CooledDown += ShootNow;
            mLevelSprites = new ImageSprite[3];
        }

        protected override void CompleteClone()
        {
            base.CompleteClone();
            FireTimer.CooledDown += ShootNow;
            var levelSprites = new ImageSprite[3];
            for (var i = 0; i <mLevelSprites.Length; i++)
            {
                if (mLevelSprites[i] is ImageSprite sprite)
                {
                    levelSprites[i] = (ImageSprite)sprite.Clone();
                }
            }
            mLevelSprites = levelSprites;
        }

        #region Shooting

        protected abstract IEnumerable<Projectile> CreateProjectiles(Vector2 direction);
        
        private void ShootNow(CooldownComponent timer)
        {
            if (NextTarget == null || State != BuildingState.Active)
                return;

            foreach (var projectile in CreateProjectiles(NextTarget.Sprite.Position - Sprite.Position))
            {
                FireAction(projectile);
            }
            timer.Reset();
        }

        private void UpdateLevel(SpriteManager spriteManager)
        {
            switch (mLevel)
            {
                // Damage increases from e.g. 100 to 175 to 225
                case TowerLevel.Zero:
                    break;
                case TowerLevel.One:
                {
                    Damage += Damage;
                    BitcoinWorth += BitcoinWorth;

                    var badge = spriteManager.CreateTowerLevelOne();
                    badge.Position = Sprite.Position;
                    badge.X -= 40;
                    badge.Y -= 40;
                    mLevelSprites[1] = badge;
                    // mLevelSprites.TintColor = Color.Gold;
                    break;
                }
                case TowerLevel.Two:
                {
                    Damage += (int) (0.5f * Damage);
                    BitcoinWorth += (int) (0.5f * BitcoinWorth);

                    var badge = spriteManager.CreateTowerLevelTwo();
                    badge.Position = Sprite.Position;
                    badge.X -= 20;
                    badge.Y -= 40;
                    mLevelSprites[2] = badge;
                    // mLevelSprites.TintColor = Color.Orange;
                    break;
                }

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch, gameTime);
            for (var i = 0; i < mLevelSprites.Length; i++)
            {
                if (mLevelSprites[i] != null)
                {
                    mLevelSprites[i].Draw(spriteBatch, gameTime);
                }
            }
        }

        #region Actions

        protected override IEnumerable<IAction> Actions(Player owner) =>
            new IAction[]
            {
                new ImprovementAction(this, SpriteManager, owner),
                new StrategyAction(this, TowerStrategy.First, SpriteManager),
                new StrategyAction(this, TowerStrategy.Strongest, SpriteManager),
                new StrategyAction(this, TowerStrategy.Weakest, SpriteManager),
            }.Concat(base.Actions(owner));
        
        private sealed class ImprovementAction : IAction, IPriced
        {
            public Button Button => mButton.Button;

            private readonly PurchaseButton<TextButton, ImprovementAction> mButton;
            private readonly Func<bool> mIsFinalLevel;
            private readonly Tower mTower;
            private readonly Player mOwner;

            internal ImprovementAction(StrategicTower tower, SpriteManager spriteManager, Player owner)
            {
                var action = new PurchasableAction<ImprovementAction>(this);
                var button = new TextButton(spriteManager) {Title = "Verbessern"};
                mOwner = owner;
                mButton = new PurchaseButton<TextButton, ImprovementAction>(owner, action, button);
                action.Purchased += (player, theAction) =>
                {
                    switch (tower.mLevel)
                    {
                        case TowerLevel.Zero:
                            tower.mLevel = TowerLevel.One;
                            break;
                        case TowerLevel.One:
                            tower.mLevel = TowerLevel.Two;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    tower.UpdateLevel(spriteManager);
                };
                mIsFinalLevel = () => tower.mLevel == TowerLevel.Two;
                mTower = tower;
            }

            void IUpdatable.Update(InputManager inputManager, GameTime gameTime)
            {
                Button.Enabled = !mIsFinalLevel() && (mOwner.Bitcoins >= Price);
                Button.Update(inputManager, gameTime);
            }

            void IDrawable.Draw(SpriteBatch spriteBatch, GameTime gameTime) =>
                Button.Draw(spriteBatch, gameTime);

            public Currency Currency => Currency.Bitcoin;

            // Improving costs 100% of Tower value.
            public int Price => mTower.Price;
        }


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

        public override void Update(PositionProvider positionProvider, InputManager inputManager, GameTime gameTime)
        {
            base.Update(positionProvider, inputManager, gameTime);

            if (State != BuildingState.Active)
                return;

            ChooseTarget(positionProvider);
            RotateToTarget();
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
                            ? new[] {(unit, measure)}
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
                    var heat = tile is TileIndex tileIndex ? positionProvider.TroupeData.TileHeat(tileIndex.ToPoint()) : null;
                    return heat;

                case TowerStrategy.Strongest:
                    return unit.RemainingLife;

                case TowerStrategy.Weakest:
                    return unit.RemainingLife;

                default:
                    throw new InvalidOperationException("Invalid Strategy " + mStrategy);
            }
        }

        internal override void UpdateInformation()
        {
            base.UpdateInformation();
            mInfoText.Text += $"\nStärke: {Damage}";
        }

        #endregion
    }

    [JsonConverter(typeof(StringEnumConverter))]
    internal enum TowerStrategy { First, Strongest, Weakest };

    internal enum TowerLevel {Zero,  One, Two}
}