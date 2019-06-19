using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using KernelPanic.Data;
using KernelPanic.Sprites;
using Microsoft.Xna.Framework;

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

        protected override IEnumerable<IAction> Actions =>
            base.Actions.Extend(new SellAction(this, SpriteManager));

        private sealed class SellAction : BaseAction<PurchaseButton<SellAction, PurchasableAction<SellAction>>>, IPriced
        {
            private readonly Building mBuilding;

            public SellAction(Building building, SpriteManager spriteManager) : base(CreateButton(spriteManager))
            {
                mBuilding = building;
                Provider.Action.ResetResource(this);
                Provider.Action.Purchased += (player, action) => Console.WriteLine("Sold building " + building);
            }

            private static PurchaseButton<SellAction, PurchasableAction<SellAction>> CreateButton(SpriteManager spriteManager)
            {
                var action = new PurchasableAction<SellAction>();
                var button = new PurchaseButton<SellAction, PurchasableAction<SellAction>>(null, action, spriteManager)
                {
                    Button = { Title = "Verkaufen" },
                    PossiblyEnabled = false // We don't have enough information to actually do something.
                };
                return button;
            }

            public Currency Currency => Currency.Bitcoin;
            public int Price =>
                // You get 80% of the buildings worth back when selling.
                (int) (mBuilding.BitcoinWorth * -0.8);

            public override void MoveTo(Vector2 position)
            {
                Provider.Button.Sprite.Position = position;
            }
        }

        #endregion
    }
}
