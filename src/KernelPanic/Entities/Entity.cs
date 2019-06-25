using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using KernelPanic.Data;
using KernelPanic.Input;
using KernelPanic.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;


namespace KernelPanic.Entities
{
    [DataContract]
    [KnownType(typeof(Unit))]
    [KnownType(typeof(Building))]
    internal abstract class Entity : IPriced, IBounded
    {
        internal Sprite Sprite { get; private set; }

        protected SpriteManager SpriteManager { get; }

        public bool mDidDie;

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
        protected T Clone<T>() where T: Entity
        {
            var copy = (T) MemberwiseClone();
            copy.CompleteClone();
            return copy;
        }

        #endregion

        public bool Selected { get; set; }

        public Rectangle Bounds => Sprite.Bounds;

        internal virtual void Update(PositionProvider positionProvider, GameTime gameTime, InputManager inputManager)
        {
            if (Selected)
            {
                PositionActions(action => action.Update(inputManager, gameTime));
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Sprite.Draw(spriteBatch, gameTime);

            if (Selected)
            {
                PositionActions(action => action.Draw(spriteBatch, gameTime));
            }
        }

        #region IPriced

        public int Price { get; }
        public Currency Currency => Currency.Bitcoin;

        #endregion

        #region Actions

        private void PositionActions(Action<IAction> body)
        {
            const int actionSpacer = 15;
            var bounds = Sprite.Bounds;
            var position = bounds.Location + new Point(bounds.Width + actionSpacer, -actionSpacer);
            foreach (var action in Actions)
            {
                position.Y += actionSpacer;
                action.MoveTo(position.ToVector2());
                body(action);
                position.Y += action.Bounds.Height;
            }
        }
        
        protected virtual IEnumerable<IAction> Actions => Enumerable.Empty<IAction>();

        protected interface IAction : IBounded, IDrawable, IUpdatable
        {
            void MoveTo(Vector2 position);
        }

        protected abstract class BaseAction<T> : IAction where T: IBounded, IDrawable, IUpdatable
        {
            protected T Provider { get; }

            protected BaseAction(T provider)
            {
                Provider = provider;
            }

            public Rectangle Bounds => Provider.Bounds;
            public abstract void MoveTo(Vector2 position);
            public void Draw(SpriteBatch spriteBatch, GameTime gameTime) => Provider.Draw(spriteBatch, gameTime);
            public void Update(InputManager inputManager, GameTime gameTime) => Provider.Update(inputManager, gameTime);
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
