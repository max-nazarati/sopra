using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using KernelPanic.Input;
using KernelPanic.Sprites;
using KernelPanic.Table;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic.Entities.Units
{
    // This is instantiated via black magic originating from Unit.Create.
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    internal sealed class Settings : Hero
    {
        private readonly ImageSprite mIndicator;
        private const int AbilityRange = Grid.KachelSize * 5;
        private float mAbilityRangeAmplifier = 1;
        private readonly List<Troupe> mTroupesInRange;
        private const int HealValue = -2;
        private readonly ImageSprite mTroupeMarker;

        // Heilt alle zwei Sekunden, Truppen im Radius von 1 Kachel um 2 LP
        internal Settings(SpriteManager spriteManager)
            : base(50, 4, 25, 0, TimeSpan.FromSeconds(1), spriteManager.CreateSettings(), spriteManager)
        {
            mIndicator = spriteManager.CreateHealIndicator(AbilityRange * mAbilityRangeAmplifier);
            mTroupesInRange = new List<Troupe>();
            mTroupeMarker = spriteManager.CreateTroupeMarker();
        }
        
        protected override void CompleteClone()
        {
            base.CompleteClone(); 
            Cooldown = new CooldownComponent(TimeSpan.FromSeconds(1), false);
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

        protected override void TryActivateAbility(InputManager inputManager, bool button = false)
        {
            // TODO show the cooldown and disable the click ability (or make it active i dunno, im just a comment not a cop)
            Console.WriteLine("TODO: settings ability is passive");
        }

        
        protected override void StartAbility(PositionProvider positionProvider, InputManager inputManager)
        {
            if (mTroupesInRange.Count == 0)
            {
                // if the CoolDown should only be used when there is a troupe in Range, remove the line below
                AbilityStatus = AbilityState.CoolingDown;

                return;
            }
            foreach (var troupe in mTroupesInRange)
            {
                troupe.DealDamage(HealValue);
            }

            // Ability was successfully cast;
            AbilityStatus = AbilityState.CoolingDown;
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