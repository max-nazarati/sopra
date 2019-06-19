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

        protected SpriteManager SpriteManager { get; }

        public bool mDidDie;

        protected Entity(int price, Sprite sprite, SpriteManager spriteManager)
        {
            Price = price;
            Sprite = sprite;
            Sprite.SetOrigin(RelativePosition.Center);
            SpriteManager = spriteManager;
        }

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

        public int Price { get; }
        public Currency Currency => Currency.Bitcoin;

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
    }
}
