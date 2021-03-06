﻿using System;
using System.Collections.Generic;
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
using KernelPanic.Table;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace KernelPanic.Entities
{
    [DataContract]
    internal abstract class Building : Entity
    {
        protected Building(int price, Sprite sprite, SpriteManager spriteManager)
            : base(price, new Point(Grid.KachelSize), sprite, spriteManager)
        {
            BitcoinWorth = (int) (price * 0.8f);
            sprite.ScaleToWidth(Grid.KachelSize);
        }

        /// <summary>
        /// Creates an object of type <typeparamref name="TBuilding"/> using reflection. <typeparamref name="TBuilding"/>
        /// should have a one-argument constructor which takes a <see cref="SpriteManager"/>.
        /// 
        /// <para>
        /// Prefer the explicit constructor if possible.
        /// </para>
        /// </summary>
        /// <param name="spriteManager">The <see cref="SpriteManager"/> passed to the constructor.</param>
        /// <typeparam name="TBuilding">The type of <see cref="Unit"/> to create.</typeparam>
        /// <returns>A new instance of <typeparamref name="TBuilding"/>.</returns>
        internal static TBuilding Create<TBuilding>(SpriteManager spriteManager) where TBuilding : Building
        {
            const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.CreateInstance |
                                              BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

            return (TBuilding) Activator.CreateInstance(typeof(TBuilding),
                bindingFlags,
                null,
                new object[] {spriteManager},
                null);
        }

        internal Building Clone() => Clone<Building>();

        public override Rectangle Bounds =>
            new Rectangle(
                (Sprite.Position - 0.5f * new Vector2(Grid.KachelSize)).ToPoint(),
                new Point(Grid.KachelSize));

        public override int? DrawLevel => 0;    // Buildings have the lowest level.

        internal int BitcoinWorth { get; set; }

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
                        Sprite.TintColor = Color.Black;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }
            }
        }

        internal override void UpdateInformation()
        {
            mInfoText.Text = $"Wert: {BitcoinWorth}";
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
            public int Price => -1 * mBuilding.BitcoinWorth;

            void IUpdatable.Update(InputManager inputManager, GameTime gameTime) => mButton.Update(inputManager, gameTime);

            void IDrawable.Draw(SpriteBatch spriteBatch, GameTime gameTime) =>
                mButton.Draw(spriteBatch, gameTime);
        }

        #endregion
    }

    [JsonConverter(typeof(StringEnumConverter))]
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
