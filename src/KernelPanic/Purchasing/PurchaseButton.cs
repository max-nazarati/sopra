using System;
using KernelPanic.Data;
using KernelPanic.Input;
using KernelPanic.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using KernelPanic.Sprites;

namespace KernelPanic.Purchasing
{
    internal class PurchaseButton<TButton, TResource, TAction>: IPositioned, IBounded, IDrawable, IUpdatable, IButtonLike
        where TResource: IPriced
        where TAction: PurchasableAction<TResource>
        where TButton: Button
    {
        /// <summary>
        /// The button which is drawn and which is used to handle the click events.
        /// Use <see cref="PossiblyEnabled"/> to influence whether this button is enabled besides the ability to
        /// for <see cref="Player"/> to afford <see cref="Action"/>.
        /// </summary>
        internal TButton Button { get; }
        
        /// <summary>
        /// The player who buys <see cref="Action"/> when the button is clicked.
        /// </summary>
        /*internal*/ private Player Player { get; }

        /// <summary>
        /// The action which is bought when <see cref="Button"/> is clicked.
        /// </summary>
        internal TAction Action { get; }

        /// <summary>
        /// Set this to <code>false</code> to keep the button disabled
        /// although the player is able to pay for <see cref="Action"/>.
        /// </summary>
        internal bool PossiblyEnabled { /*internal*/ private get; set; } = true;

        #region IPositioned/IBounded

        public Vector2 Position
        {
            get => Button.Position;
            set => Button.Position = value;
        }

        public Vector2 Size => Button.Size;

        /// <inheritdoc />
        public Rectangle Bounds => Button.Bounds;

        #endregion

        /// <summary>
        /// Creates a <code>PurchaseButton</code> so that a click on it purchases
        /// <paramref name="action"/> for <paramref name="player"/>.
        /// </summary>
        /// <param name="player">The player who purchases the action.</param>
        /// <param name="action">The action which can be purchased.</param>
        /// <param name="button"></param>
        internal PurchaseButton(Player player, TAction action, TButton button)
        {
            Player = player;
            Action = action;
            Button = button;
            Button.Clicked += Purchase;
        }

        public void Update(InputManager inputManager, GameTime gameTime)
        {
            Button.Update(inputManager, gameTime);
            Button.Enabled = PossiblyEnabled && Action.Available(Player);
        }

        /// <inheritdoc />
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Button?.Draw(spriteBatch, gameTime);
        }

        private void Purchase(Button sender, InputManager inputManager)
        {
            if (!Action.TryPurchase(Player))
                throw new InvalidOperationException($"Player {Player} was not able to purchase {Action}");
        }

        Button IButtonLike.Button => ((IButtonLike) Button).Button;
    }

    internal sealed class PurchaseButton<TButton, TResource> : PurchaseButton<TButton, TResource, PurchasableAction<TResource>>
        where TResource : class, IPriced
        where TButton : Button
    {
        /// <summary>
        /// Creates a <code>PurchaseButton</code> so that a click on it purchases
        /// <paramref name="action"/> for <paramref name="player"/>.
        /// </summary>
        /// <param name="player">The player who purchases the action.</param>
        /// <param name="action">The action which can be purchased.</param>
        /// <param name="button"></param>
        internal PurchaseButton(Player player, PurchasableAction<TResource> action, TButton button)
            : base(player, action, button)
        {
        }
    }
}