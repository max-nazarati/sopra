using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using KernelPanic.Data;
using KernelPanic.Input;
using KernelPanic.Interface;
using KernelPanic.Players;
using KernelPanic.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic.Entities
{
    [DataContract]
    [KnownType(typeof(Unit))]
    [KnownType(typeof(Building))]
    internal abstract class Entity : IPriced, IBounded
    {
        internal Sprite Sprite { get; private set; }

        protected SpriteManager SpriteManager { get; }

        protected Entity(int price, Sprite sprite, SpriteManager spriteManager)
        {
            Price = price;
            Sprite = sprite;
            Sprite.SetOrigin(RelativePosition.Center);
            SpriteManager = spriteManager;
        }

        #region Cloning

        /// <summary>
        /// Can be overridden by subclasses to perform a deeper copy on selected properties.
        /// </summary>
        protected virtual void CompleteClone()
        {
            Sprite = Sprite.Clone();
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
        protected internal T Clone<T>() where T: Entity
        {
            var copy = (T) MemberwiseClone();
            copy.CompleteClone();
            return copy;
        }

        #endregion

        #region Removal

        /// <summary>
        /// If this flag is <c>true</c> this entity should be removed from the <see cref="EntityGraph"/>.
        /// </summary>
        internal bool WantsRemoval { get; private set; }

        /// <summary>
        /// Flags this entity for removal.
        /// </summary>
        protected void SetWantsRemoval()
        {
            WantsRemoval = true;
        }

        #endregion

        public bool Selected { get; set; }

        public Rectangle Bounds => Sprite.Bounds;

        internal virtual void AttackBase(InputManager inputManager, PositionProvider positionProvider, Point basePosition)
        {
            // do nothing (Troupes walk to the base anyways
        }
            
        internal virtual void Update(PositionProvider positionProvider, GameTime gameTime, InputManager inputManager)
        {
            if (!Selected)
                return;

            if (mStoredActions == null)
                mStoredActions = Actions(positionProvider.Owner[this]).ToArray();

            PositionActions(action => action.Update(inputManager, gameTime));
        }

        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Sprite.Draw(spriteBatch, gameTime);
        }

        internal void DrawActions(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (var action in mStoredActions)
            {
                action.Draw(spriteBatch, gameTime);
            }
        }

        #region IPriced

        public int Price { get; }
        public Currency Currency => Currency.Bitcoin;

        #endregion

        #region Actions

        private IAction[] mStoredActions;

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
