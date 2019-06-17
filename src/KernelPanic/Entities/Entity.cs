using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using KernelPanic.Data;
using KernelPanic.Input;
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

        protected Entity(int price, Sprite sprite)
        {
            Price = price;
            Sprite = sprite;
            Sprite.SetOrigin(RelativePosition.Center);
        }

        public bool Selected { get; set; }

        public Rectangle Bounds => Sprite.Bounds;

        internal virtual void Update(PositionProvider positionProvider, GameTime gameTime, InputManager inputManager, QuadTree<Entity> quadtree)
        {
            // TODO: Display the actions if this entity is selected.
        }
        
        internal virtual void Update(PositionProvider positionProvider, GameTime gameTime, InputManager inputManager)
        {
            // TODO: Display the actions if this entity is selected.
        }

        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Sprite.Draw(spriteBatch, gameTime);
        }

        public int Price { get; }
        public Currency Currency => Currency.Bitcoin;

        #region Actions
        
        protected virtual IEnumerable<IAction> Actions => Enumerable.Empty<IAction>();

        protected interface IAction : IBounded, IDrawable, IUpdatable
        {
        }

        protected class BaseAction<T> : IAction where T: IBounded, IDrawable, IUpdatable
        {
            protected T Provider { get; }

            protected BaseAction(T provider)
            {
                Provider = provider;
            }

            public Rectangle Bounds => Provider.Bounds;
            public void Draw(SpriteBatch spriteBatch, GameTime gameTime) => Provider.Draw(spriteBatch, gameTime);
            public void Update(GameTime gameTime) => Provider.Update(gameTime);
        }

        #endregion
    }
}
