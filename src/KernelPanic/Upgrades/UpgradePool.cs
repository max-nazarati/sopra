using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using KernelPanic.Data;
using KernelPanic.Input;
using KernelPanic.Interface;
using KernelPanic.Players;
using KernelPanic.Purchasing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace KernelPanic.Upgrades
{
    public sealed class UpgradePool: IPositioned, IBounded
    {
        private const int ButtonWidth = 400;
        private const int ButtonPadding = 20;

        /// <summary>
        /// Stores the <see cref="PurchaseButton{TButton,TResource,TAction}"/> for an <see cref="Upgrade"/> together
        /// with the upgrades <see cref="Upgrade.Id"/>.
        ///
        /// <para>
        /// This second piece of information is required because once the
        /// <see cref="Upgrade"/> is embedded inside the <see cref="SinglePurchasableAction{TResource}"/> we can't
        /// access it or its properties any more.
        /// </para>
        /// </summary>
        private struct UpgradeReference
        {
            internal Upgrade.Id Id { get; }
            internal PurchaseButton<TextButton, Upgrade, SinglePurchasableAction<Upgrade>> Button { get; }

            internal UpgradeReference(Upgrade upgrade, Player player, SpriteManager spriteManager)
            {
                Id = upgrade.Kind;

                var action = new SinglePurchasableAction<Upgrade>(upgrade);
                var button = new TextButton(spriteManager, ButtonWidth) {Title = upgrade.Label};
                action.Purchased += (buyer, resource) => buyer.AddUpgrade(resource);
                Button = new PurchaseButton<TextButton, Upgrade, SinglePurchasableAction<Upgrade>>(player, action, button);
            }
        }
        
        /// <summary>
        /// Stores all available upgrades. The first/outermost level is organized by price, this data-layout is
        /// reused by the indexer; so something which costs one EP, is somewhere in the list at index 0.
        /// </summary>
        private UpgradeReference[][] mUpgrades;

        [JsonProperty]
        private readonly Player mPlayer;

        private readonly SpriteManager mSpriteManager;

        [JsonProperty]
        private readonly Dictionary<Upgrade.Id, Player> mBoughtUpgrades = new Dictionary<Upgrade.Id, Player>();

        /// <summary>
        /// Creates a new <see cref="UpgradePool"/> with all available upgrades.
        /// </summary>
        /// <param name="player">The player who buys an upgrade when it is clicked.</param>
        /// <param name="spriteManager">The <see cref="SpriteManager"/>.</param>
        internal UpgradePool(Player player, SpriteManager spriteManager)
        {
            mPlayer = player;
            mSpriteManager = spriteManager;
            Initialize();
        }

        private void Initialize()
        {
            mUpgrades = Upgrade.Matrix.Select(upgrades =>
                upgrades.Select(upgrade =>
                {
                    var reference = new UpgradeReference(upgrade, mPlayer, mSpriteManager);
                    reference.Button.Action.Purchased += RememberUpgrade;
                    return reference;
                }).ToArray()
            ).ToArray();
        }

        private void RememberUpgrade(Player buyer, Upgrade upgrade)
        {
            mBoughtUpgrades.Add(upgrade.Kind, buyer);
        }

        #region Serialization

        [OnDeserialized]
        private void AfterDeserialization(StreamingContext context)
        {
            Initialize();

            foreach (var upgradeId in mBoughtUpgrades.Keys)
            {
                this[upgradeId].IsPurchased = true;
            }
        }

        #endregion

        #region Layout

        [JsonIgnore]
        public Vector2 Position
        {
            get => mUpgrades[0][0].Button.Position;
            set => LayOut(value);
        }

        [JsonIgnore]
        public Vector2 Size
        {
            get
            {
                var width = mUpgrades.Length * ButtonWidth + (mUpgrades.Length - 1) * ButtonPadding;
                var height = mUpgrades.Max(upgrades =>
                    upgrades.Sum(upgrade => upgrade.Button.Size.Y) + (upgrades.Length - 1) * ButtonPadding
                );
                return new Vector2(width, height);
            }
        }

        [JsonIgnore]
        public Rectangle Bounds => KernelPanic.Data.Bounds.ContainingRectangle(Position, Size);

        /// <summary>
        /// Performs a simple layout of the buttons currently in <see cref="mUpgrades"/>. For each price there is a
        /// column, and all the columns are aligned at the top.
        /// </summary>
        /// <param name="start">The position of the first button.</param>
        private void LayOut(Vector2 start)
        {
            var x = start.X;
            foreach (var upgrades in mUpgrades)
            {
                var y = start.Y;
                foreach (var upgrade in upgrades)
                {
                    upgrade.Button.Position = new Vector2(x, y);
                    y += upgrade.Button.Size.Y + ButtonPadding;
                }

                // Assume that all buttons share the same width.
                x += upgrades[0].Button.Size.X + ButtonPadding;
            }
        }

        #endregion

        /// <summary>
        /// Returns the <see cref="SinglePurchasableAction{Upgrade}"/> which corresponds to
        /// this <see cref="Upgrade.Id"/>.
        /// </summary>
        /// <param name="id">The id for which to retrieve the purchasable action.</param>
        internal SinglePurchasableAction<Upgrade> this[Upgrade.Id id] =>
            mUpgrades[Upgrade.IdPrice(id) - 1].First(upgrade => upgrade.Id == id).Button.Action;

        internal void Update(InputManager inputManager, GameTime gameTime)
        {
            foreach (var upgrades in mUpgrades)
            {
                foreach (var upgrade in upgrades)
                {
                    upgrade.Button.Update(inputManager, gameTime);
                }
            }
        }

        internal void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (var upgrades in mUpgrades)
            {
                foreach (var upgrade in upgrades)
                {
                    // tint button
                    // gray: if player hasn't got enough resources
                    // blue: player already bought the upgrade
                    // red: the other player bought the upgrade
                    var tintColor = mBoughtUpgrades.TryGetValue(upgrade.Id, out var buyer)
                        ? buyer.Select(Color.SteelBlue, Color.IndianRed)
                        : Color.Gray;
                    upgrade.Button.DrawTint(spriteBatch, gameTime, tintColor);
                }
            }
        }
    }
}