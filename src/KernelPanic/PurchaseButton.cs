using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    internal sealed class PurchaseButton<TResource, TAction>: IDrawable
        where TResource: class, IPriced
        where TAction: PurchasableAction<TResource>
    {
        /// <summary>
        /// The button which is drawn and which is used to handle the click events.
        /// Use <see cref="PossiblyEnabled"/> to influence whether this button is enabled besides the ability to
        /// for <see cref="Player"/> to afford <see cref="Action"/>.
        /// </summary>
        internal Button Button { get; }

        /// <summary>
        /// The player who buys <see cref="Action"/> when the button is clicked.
        /// </summary>
        internal Player Player { get; }

        /// <summary>
        /// The action which is bought when <see cref="Button"/> is clicked.
        /// </summary>
        internal TAction Action { get; }

        /// <summary>
        /// Set this to <code>false</code> to keep the button disabled
        /// although the player is able to pay for <see cref="Action"/>.
        /// </summary>
        internal bool PossiblyEnabled { get; set; } = true;

        /// <summary>
        /// Creates a <code>PurchaseButton</code> so that a click on it purchases
        /// <paramref name="action"/> for <paramref name="player"/>.
        /// </summary>
        /// <param name="player">The player who purchases the action.</param>
        /// <param name="action">The action which can be purchased.</param>
        /// <param name="spriteManager">The sprite manager.</param>
        internal PurchaseButton(Player player, TAction action, SpriteManager spriteManager)
        {
            Player = player;
            Action = action;
            Button = new Button(spriteManager);
            Button.Clicked += Purchase;
        }
        
        public void Update(GameTime gameTime, InputManager inputManager)
        {
            Button.Update(gameTime, inputManager);
            Button.Enabled = PossiblyEnabled && Action.Available(Player);
        }

        /// <inheritdoc />
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Button?.Draw(spriteBatch, gameTime);
        }

        private void Purchase(Button sender)
        {
            if (!Action.TryPurchase(Player))
                throw new InvalidOperationException($"Player {Player} was not able to purchase {Action}");
        }
    }
}