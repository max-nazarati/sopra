using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using KernelPanic.Data;
using KernelPanic.Input;
using KernelPanic.Interface;
using KernelPanic.Players;
using KernelPanic.Purchasing;
using KernelPanic.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic.Entities
{
    [DataContract]
    [DebuggerDisplay("at {Sprite.Position}")]
    internal abstract class Entity : IPriced, IGameObject
    {
        internal Sprite Sprite { get; private set; }

        internal readonly TextSprite mInfoText;

        protected SpriteManager SpriteManager { get; }

        private Sprite mHitBoxSprite;

        protected Entity(int price, Point hitBoxSize, Sprite sprite, SpriteManager spriteManager)
        {
            Price = price;
            Sprite = sprite;
            Sprite.SetOrigin(RelativePosition.Center);
            mInfoText = spriteManager.CreateText($"Preis: {price}");
            mInfoText.SetOrigin(RelativePosition.CenterRight);
            mInfoText.TextColor = Color.White;
            SpriteManager = spriteManager; 
            mHitBoxSprite = spriteManager.CreateHitBoxBorder(hitBoxSize);
        }

        #region Cloning

        /// <summary>
        /// Can be overridden by subclasses to perform a deeper copy on selected properties.
        /// </summary>
        protected virtual void CompleteClone()
        {
            Sprite = Sprite.Clone();
            mHitBoxSprite = mHitBoxSprite?.Clone();
        }

        /// <summary>
        /// Creates a new object from this object. <see cref="CompleteClone"/> is called before returning it.
        /// </summary>
        /// <example>
        /// When creating a clone function for a new subclass called <c>TheSubclass</c> use it like this
        /// <code>
        /// internal TheSubclass Clone() => Clone&lt;TheSubclass&gt;();
        /// </code>
        /// </example>
        /// <typeparam name="T">Should be the type of <c>this</c>.</typeparam>
        /// <returns>A copy of the object.</returns>
        protected T Clone<T>() where T: Entity
        {
            var copy = (T) MemberwiseClone();
            copy.CompleteClone();
            return copy;
        }

        #endregion

        public abstract int? DrawLevel { get; }

        /// <summary>
        /// If this flag is <c>true</c> this entity should be removed from the <see cref="EntityGraph"/>.
        /// </summary>
        public bool WantsRemoval { get; protected set; }

        public bool Selected { get; set; }

        public abstract Rectangle Bounds { get; }

        public abstract void Update(PositionProvider positionProvider, InputManager inputManager, GameTime gameTime);

        internal void UpdateOverlay(Player owner, InputManager inputManager, GameTime gameTime)
        {
            if (mStoredActions == null)
            {
                mStoredActions = Actions(owner).ToArray();
                mDrawActions = owner.Select(true, false);
            }

            if (!mDrawActions)
                return;

            PositionActions(action => action.Update(inputManager, gameTime));
            UpdateInformation();
        }

        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Sprite.Draw(spriteBatch, gameTime);

            if (!DebugSettings.ShowHitBoxes)
                return;

            mHitBoxSprite.Position = Bounds.Location.ToVector2();
            mHitBoxSprite.Draw(spriteBatch, gameTime);
        }

        internal virtual void UpdateInformation()
        {
            mInfoText.Text = $"Preis: {Price}";
        }

        internal virtual void DrawActions(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (!mDrawActions)
                return;

            foreach (var action in mStoredActions)
            {
                action.Draw(spriteBatch, gameTime);
            }

            
            mInfoText.Draw(spriteBatch, gameTime);
        }

        #region IPriced

        public int Price { get; }
        public Currency Currency => Currency.Bitcoin;

        #endregion

        #region Actions

        private IAction[] mStoredActions;
        private bool mDrawActions;

        /// <summary>
        /// Enumerates through all actions supported by this entity. Defaults to no actions.
        /// <para>
        /// Can be overriden in subclasses to add additional actions e.g. using
        /// <see cref="EnumerableExtensions.Extend{IAction}"/>.
        /// </para>
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        protected virtual IEnumerable<IAction> Actions(Player owner) =>
            Enumerable.Empty<IAction>();

        protected interface IAction : IButtonLike
        {
        }

        /// <summary>
        /// Modifies the actions in <see cref="mStoredActions"/> to be stacked vertically aligned with this entity's
        /// bounds. To avoid a second loop, <paramref name="body"/> is called with each action after positioning.
        /// </summary>
        /// <param name="body">Called with each action in <see cref="mStoredActions"/>.</param>
        private void PositionActions(Action<IAction> body)
        {
            const int padding = 15;
            var bounds = Sprite.Bounds;
            var position = (bounds.Location + new Point(bounds.Width + padding, -padding)).ToVector2();
            foreach (var action in mStoredActions)
            {
                position.Y += padding;
                action.Button.Sprite.Position = position;
                body(action);
                position.Y += action.Button.Sprite.Height;
            }
            mInfoText.Position = new Vector2(Sprite.Position.X - Sprite.Width, Sprite.Position.Y);
        }

        #endregion

        #region Serializing

        /// <summary>
        /// Stores/receives the entity's position during serialization. Don't use it outside of this!
        /// </summary>
        [DataMember]
        private Vector2 mPositionSerializing;

        [OnSerializing]
        private void BeforeSerialization(StreamingContext context)
        {
            mPositionSerializing = Sprite.Position;
        }

        [OnDeserialized]
        private void AfterDeserialization(StreamingContext context)
        {
            Sprite.Position = mPositionSerializing;
        }

        #endregion
    }
}
