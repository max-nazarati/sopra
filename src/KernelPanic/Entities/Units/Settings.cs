using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using KernelPanic.Entities.Buildings;
using KernelPanic.Events;
using KernelPanic.Input;
using KernelPanic.Sprites;
using KernelPanic.Table;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace KernelPanic.Entities.Units
{
    // This is instantiated via black magic originating from Unit.Create.
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    internal sealed class Settings : Hero
    {
        private const int AbilityRange = Grid.KachelSize * 3;
        private const int HealValue = -5;

        private Sprite mIndicator;
        private Sprite mTroupeMarker;
        private List<Troupe> mTroupesInRange;

        [JsonProperty]
        private float mAbilityRangeAmplifier = 1;

        private static Point HitBoxSize => new Point(56, 59);

        internal Settings(SpriteManager spriteManager)
            : base(50, 4, 25, 0, TimeSpan.FromSeconds(1), HitBoxSize, spriteManager.CreateSettings(), spriteManager)
        {
            mTroupesInRange = new List<Troupe>();
            mTroupeMarker = spriteManager.CreateTroupeMarker();
            RecreateIndicator();
        }

        [OnDeserialized]
        private void AfterDeserialization(StreamingContext context)
        {
            RecreateIndicator();
        }

        private void RecreateIndicator()
        {
            mIndicator = SpriteManager.CreateHealIndicator(AbilityRange * mAbilityRangeAmplifier);
        }

        protected override void CompleteClone()
        {
            base.CompleteClone(); 
            Cooldown = new CooldownComponent(Cooldown.Cooldown, false);
            Cooldown.CooledDown += component => AbilityStatus = AbilityState.Ready;
            mIndicator = mIndicator.Clone();
            mTroupeMarker = mTroupeMarker.Clone();
            mTroupesInRange = new List<Troupe>();
        }

        internal void AmplifyAbilityRange(float scale)
        {
            mAbilityRangeAmplifier += scale;
            RecreateIndicator();
        }

        internal void DecreaseHealCooldown(float multiplier)
        {
            Cooldown.Cooldown = new TimeSpan((long) (Cooldown.Cooldown.Ticks * multiplier));
        }

        /// <summary>
        /// This ability is passive, we need a complete different structure here:
        /// indicate before drawing or using; use whenever ready
        /// </summary>
        /// <param name="positionProvider"></param>
        /// <param name="gameTime"></param>
        /// <param name="inputManager"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        protected override void UpdateAbility(PositionProvider positionProvider,
            GameTime gameTime,
            InputManager inputManager)
        {
            switch (AbilityStatus)
            {
                case AbilityState.Ready:
                    // start whenever ready
                    IndicateAbility(positionProvider, inputManager);
                    StartAbility(positionProvider, inputManager);
                    break;

                case AbilityState.CoolingDown:
                    // wait to be ready
                    Cooldown.Update(gameTime);
                    if (Selected)
                    {
                        // update the targets when selected so drawing doesnt have to do this
                        IndicateAbility(positionProvider, inputManager);    
                    }

                    // AbilityStatus = AbilityState.Ready; // Cooldown, my archenemy. why do you not obey me
                    break;

                case AbilityState.Indicating:
                    throw new ArgumentOutOfRangeException();
                case AbilityState.Starting:
                    throw new ArgumentOutOfRangeException();
                case AbilityState.Active:
                    throw new ArgumentOutOfRangeException();
                case AbilityState.Finished:
                    throw new ArgumentOutOfRangeException();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void TryActivateAbility(InputManager inputManager, bool button = false)
        {
        }

        
        protected override void StartAbility(PositionProvider positionProvider, InputManager inputManager)
        {
            if (mTroupesInRange.Count == 0)
            {
                // if the CoolDown should only be used when there is a troupe in Range, remove this if
            }
            else 
            {
                EventCenter.Default.Send(Event.HeroAbility(this));
                foreach (var troupe in mTroupesInRange)
                    troupe.DealDamage(HealValue, positionProvider);
            }

            // Ability was successfully cast;
            AbilityStatus = AbilityState.CoolingDown;
            Cooldown.Reset();
        }
        
        protected override void IndicateAbility(PositionProvider positionProvider, InputManager inputManager)
        {
            // reset
            mTroupesInRange.Clear();
            
            // find all Troupes in Range
            foreach (var troupe in positionProvider.NearEntities<Troupe>(Sprite.Position, AbilityRange * mAbilityRangeAmplifier))
            {
                mTroupesInRange.Add(troupe);
            }
        }

        /// <summary>
        /// try not to die while attacking.
        /// sets mTarget and ShouldMove
        /// </summary>
        protected override void AutonomousAttack(InputManager inputManager, PositionProvider positionProvider)
        {
            var startPoint = positionProvider.RequireTile(Sprite.Position).ToPoint();
            var neighbours = GetNeighbours(positionProvider);
            
            #region calculate a heuristic for all neighbours and choose the best
            var bestPoint = startPoint;
            var bestValue = 0f;
            foreach (var point in neighbours)
            {
                var currentValue = PointHeuristic(point, positionProvider);
                if (currentValue <= bestValue)
                    continue;

                bestPoint = point;
                bestValue = currentValue;
            }
            #endregion

            #region set the optimum as walking target
            mAStar = positionProvider.MakePathFinding(this, new[] {bestPoint});
            if (mAStar.Path != null)
            {
                if (mAStar.Path[mAStar.Path.Count - 1] is Point target)
                {
                    mTarget = new TileIndex(target, 1);
                }
            }

            ShouldMove = true;
            #endregion
        }

        private float PointHeuristic(Point point, PositionProvider positionProvider)
        {
            point *= new Point(Grid.KachelSize);
            var result = 0f;

            // every unit counted positive
            foreach (var troupe in positionProvider.NearEntities<Troupe>(point.ToVector2(),
                AbilityRange * mAbilityRangeAmplifier))
            {
                var tile = positionProvider.RequireTile(troupe).BaseTile;
                var heat = positionProvider.TroupeData.TileHeat(tile.ToPoint());
                result += 10; // just setting a base value
                var factor = 1 + troupe.MaximumLife - troupe.RemainingLife;
                result += factor * (heat ?? 1 ) < 10 ? 20 : (heat ?? 1 ) < 15 ? 16 : (heat ?? 1 ) < 20 ? 12 : (heat ?? 1 ) < 25 ? 8 : (heat ?? 1 ) < 30 ? 4 : (heat ?? 1 ) < 40 ? 2 : 1;
            }

            // every tower is negative
            foreach (var tower in positionProvider.NearEntities<Tower>(point.ToVector2(), 3 * AbilityRange))
            {
                if (Vector2.DistanceSquared(tower.Bounds.Center.ToVector2(), Bounds.Center.ToVector2()) < tower.Radius * tower.Radius)
                {
                    result /= 1.5f;
                    result -= 1;
                }
            }
            return result;
        }

        protected override void DrawAbility(SpriteBatch spriteBatch, GameTime gameTime)
        {
            mIndicator.Position = Sprite.Position;
            mIndicator.Draw(spriteBatch, gameTime);
            foreach (var troupe in mTroupesInRange)
            {
                mTroupeMarker.Position = troupe.Sprite.Position;
                mTroupeMarker.Draw(spriteBatch, gameTime);
            }
        }
        
    }
}