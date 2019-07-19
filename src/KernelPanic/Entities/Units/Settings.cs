using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using KernelPanic.Entities.Buildings;
using KernelPanic.Input;
using KernelPanic.Sprites;
using KernelPanic.Table;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace KernelPanic.Entities.Units
{
    // This is instantiated via black magic originating from Unit.Create.
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    internal sealed class Settings : Hero
    {
        private readonly ImageSprite mIndicator;
        private const int AbilityRange = Grid.KachelSize * 3;
        private float mAbilityRangeAmplifier = 1;
        private readonly List<Troupe> mTroupesInRange;
        private const int HealValue = -5;
        private readonly ImageSprite mTroupeMarker;

        private static Point HitBoxSize => new Point(56, 59);

        // Heilt alle zwei Sekunden, Truppen im Radius von 1 Kachel um 2 LP
        internal Settings(SpriteManager spriteManager)
            : base(50, 4, 25, 0, TimeSpan.FromSeconds(1), HitBoxSize, spriteManager.CreateSettings(), spriteManager)
        {
            mIndicator = spriteManager.CreateHealIndicator(AbilityRange * mAbilityRangeAmplifier);
            mTroupesInRange = new List<Troupe>();
            mTroupeMarker = spriteManager.CreateTroupeMarker();
        }

        protected override void CompleteClone()
        {
            base.CompleteClone(); 
            Cooldown = new CooldownComponent(Cooldown.Cooldown, false);
            Cooldown.CooledDown += component => AbilityStatus = AbilityState.Ready;
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

        protected override void UpdateCooldown(GameTime gameTime, PositionProvider positionProvider)
        {
            if (/*TODO ask if we are back in base */ true) // ask for base tile hit
            {
                base.UpdateCooldown(gameTime, positionProvider);
            }
        }

        protected override void TryActivateAbility(InputManager inputManager, bool button = false)
        {
            // TODO show the cooldown and disable the click ability (or make it active i dunno, im just a comment not a cop)
            Console.WriteLine("TODO: settings ability is passive");
        }

        
        protected override void StartAbility(PositionProvider positionProvider, InputManager inputManager)
        {
            if (mTroupesInRange.Count == 0)
            {
                // if the CoolDown should only be used when there is a troupe in Range, remove this if
            }
            else foreach (var troupe in mTroupesInRange)
            {
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
        
        internal void AmplifyAbilityRange(float scale)
        {
            mAbilityRangeAmplifier += scale;
            // mIndicator = spriteManager.CreateHealIndicator(AbilityRange * mAbilityRangeAmplifier);
            // we are scaling bc we dont know the spriteManager in this context
            // seems to be doing fine, just had to figure out that we need a factor 2 bc of radius vs diameter
            mIndicator.ScaleToWidth(AbilityRange * mAbilityRangeAmplifier * 2);
        }

        protected override float PointHeuristic(Point point, PositionProvider positionProvider)
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
                var factor = troupe.MaximumLife - troupe.RemainingLife;
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
            if (mTroupesInRange != null)
            {
                foreach (var troupe in mTroupesInRange)
                {
                    mTroupeMarker.Position = troupe.Sprite.Position;
                    mTroupeMarker.Draw(spriteBatch, gameTime);
                }
            }
        }
        
    }
}