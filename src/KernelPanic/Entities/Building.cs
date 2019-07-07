using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.Serialization;
using KernelPanic.Data;
using KernelPanic.Events;
using KernelPanic.Input;
using KernelPanic.Sprites;
using Microsoft.Xna.Framework;
using KernelPanic.Interface;
using KernelPanic.Players;
using KernelPanic.Purchasing;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic.Entities
{
    [DataContract]
    internal abstract class Building : Entity
    {
        [SuppressMessage("ReSharper", "UnusedParameter.Local", Justification =
            "Ensures that all buildings take a SoundManager in their constructor, which is required for Building.Create to work."
        )]
        protected Building(int price, Sprite sprite, SpriteManager spriteManager, SoundManager soundManager)
            : base(price, sprite, spriteManager)
        {
            BitcoinWorth = price;
            sprite.ScaleToWidth(Table.Grid.KachelSize);
        }

        /// <summary>
        /// Creates an object of type <typeparamref name="TBuilding"/> using reflection. <typeparamref name="TBuilding"/>
        /// should have a two-argument constructor which takes a <see cref="SpriteManager"/> and a <see cref="SoundManager"/>.
        /// 
        /// <para>
        /// Prefer the explicit constructor if possible.
        /// </para>
        /// </summary>
        /// <param name="spriteManager">The <see cref="SpriteManager"/> passed to the constructor.</param>
        /// <param name="soundManager"></param>
        /// <typeparam name="TBuilding">The type of <see cref="Unit"/> to create.</typeparam>
        /// <returns>A new instance of <typeparamref name="TBuilding"/>.</returns>
        internal static TBuilding Create<TBuilding>(SpriteManager spriteManager, SoundManager soundManager) where TBuilding : Building
        {
            const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.CreateInstance |
                                              BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

            return (TBuilding) Activator.CreateInstance(typeof(TBuilding),
                bindingFlags,
                null,
                new object[] {spriteManager, soundManager},
                null);
        }

        internal Building Clone() => Clone<Building>();

        public override int? DrawLevel => 0;    // Buildings have the lowest level.

        private int BitcoinWorth { get; set; }

        private BuildingState mBuildingState;

        internal BuildingState State
        {
            get => mBuildingState;
            set
            {
                mBuildingState = value;
                switch (value)
                {
                    case BuildingState.Active:
                        Sprite.TintColor = Color.White;
                        break;
                    case BuildingState.Inactive:
                        Sprite.TintColor = Color.Gray;
                        break;
                    case BuildingState.Invalid:
                        Sprite.TintColor = Color.Red;
                        break;
                    case BuildingState.Valid:
                        Sprite.TintColor = Color.Green;
                        break;
                    case BuildingState.Disabled:
                        Sprite.TintColor = Color.Chocolate;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }
            }
        }

        #region Actions

        protected override IEnumerable<IAction> Actions(Player owner) =>
            base.Actions(owner).Extend(
                new SellAction(this, owner, SpriteManager)
            );

        private sealed class SellAction : IAction, IPriced
        {
            public Button Button => mButton.Button;

            private readonly PurchaseButton<TextButton, SellAction> mButton;
            private readonly Building mBuilding;

            public SellAction(Building building, Player owner, SpriteManager spriteManager)
            {
                var action = new PurchasableAction<SellAction>(this);
                var button = new TextButton(spriteManager) {Title = "Verkaufen"};
                mButton = new PurchaseButton<TextButton, SellAction>(owner, action, button);
                action.Purchased += (player, theAction) =>
                {
                    building.WantsRemoval = true;
                    EventCenter.Default.Send(Event.BuildingSold(player, building));
                };
                mBuilding = building;
            }

            public Currency Currency => Currency.Bitcoin;

            // You get 80% of the buildings worth back when selling.
            public int Price => (int) (mBuilding.BitcoinWorth * -0.8);

            void IUpdatable.Update(InputManager inputManager, GameTime gameTime) =>
                mButton.Update(inputManager, gameTime);

            void IDrawable.Draw(SpriteBatch spriteBatch, GameTime gameTime) =>
                mButton.Draw(spriteBatch, gameTime);
        }

        #endregion

        public void HitByEmp()
        {
            throw new NotImplementedException();
        }
    }

    internal enum BuildingState
    {
        /// <summary>
        /// The building is able to act, that means it is able to attack enemies.
        /// </summary>
        Active,

        /// <summary>
        /// The building has been bought and is waiting to become active, that is when no enemies are at its position.
        /// </summary>
        Inactive,

        /// <summary>
        /// Used during selection of a new place for a building when the current position is not allowed.
        /// </summary>
        Invalid,

        /// <summary>
        /// Used during selection of a new place for a building when the current position is allowed.
        /// </summary>
        Valid,
        
        /// <summary>
        /// Used when hit by the Bluescreen Ability, the building is currently not able to attack
        /// </summary>
        Disabled
    }
}
