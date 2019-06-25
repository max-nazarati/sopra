using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using KernelPanic.Data;
using KernelPanic.Input;
using KernelPanic.Sprites;
using Microsoft.Xna.Framework;
using KernelPanic.Interface;
using KernelPanic.Purchasing;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic.Entities
{
    [DataContract]
    [KnownType(typeof(Tower))]
    internal abstract class Building : Entity
    {
        protected Building(int price, Sprite sprite, SpriteManager spriteManager) : base(price, sprite, spriteManager)
        {
            BitcoinWorth = price;
            sprite.ScaleToWidth(Table.Grid.KachelSize);
        }

        private int BitcoinWorth { get; set; }

        internal State StateProperty { get; set; }
        
        internal enum State
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
            Valid
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

            public SellAction(Building building, Player owner, SpriteManager spriteManager)
            {
                var action = new PurchasableAction<SellAction>(this);
                var button = new TextButton(spriteManager) {Title = "Verkaufen"};
                mButton = new PurchaseButton<TextButton, SellAction>(owner, action, button);
                action.Purchased += (player, theAction) => Console.WriteLine("Sold building " + building);

                // You get 80% of the buildings worth back when selling.
                Price = (int) (building.BitcoinWorth * -0.8);
            }

            public int Price { get; }
            public Currency Currency => Currency.Bitcoin;

            void IUpdatable.Update(InputManager inputManager, GameTime gameTime) =>
                mButton.Update(inputManager, gameTime);

            void IDrawable.Draw(SpriteBatch spriteBatch, GameTime gameTime) =>
                mButton.Draw(spriteBatch, gameTime);
        }

        #endregion
    }
}
