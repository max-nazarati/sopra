using System.Collections.Generic;
using KernelPanic.Data;
using KernelPanic.Entities.Buildings;
using KernelPanic.Events;
using KernelPanic.Input;
using KernelPanic.Sprites;
using KernelPanic.Table;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic.Entities.Projectiles
{
    internal class Projectile : IGameObject
    {
        private readonly float mRadius;
        internal int Damage { get; }

        protected ImageSprite Sprite { get; }

        protected Vector2 StartPoint { private get; set; }
        protected Vector2 MoveVector { get; set; }
        private readonly HashSet<Unit> mHitUnits = new HashSet<Unit>();

        internal bool SingleTarget { private get; set; }

        internal Tower Origin { get; }
        
        internal Projectile(Tower origin, Vector2 direction, ImageSprite sprite, float offset = 0)
        {
            // calling this with direction == Vector.Zero will not work -> game crash (out of bound, entityG.)
            Origin = origin;
            mRadius = origin.Radius;
            Damage = origin.Damage;

            direction.Normalize();
            StartPoint = origin.Sprite.Position + offset * direction;
            MoveVector = direction * origin.Speed;

            Sprite = sprite;
            Sprite.Position = StartPoint;
            Sprite.Rotation = direction.Angle(0.5);
        }

        public Rectangle Bounds => Sprite.Bounds;

        public int? DrawLevel => 2;    // Above all units and buildings.
        
        public bool WantsRemoval { get; protected set; }

        public virtual void Update(PositionProvider positionProvider, InputManager inputManager, GameTime gameTime)
        {
            Sprite.Position += MoveVector;

            if (Vector2.DistanceSquared(Sprite.Position, StartPoint) >= mRadius * mRadius)
            {
                RadiusReached();
            }
        }

        /// <summary>
        /// Called when this <see cref="Projectile"/> has reached its radius. The default implementation flags this
        /// projectile for removal.
        /// </summary>
        internal virtual void RadiusReached()
        {
            WantsRemoval = true;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Sprite.Draw(spriteBatch, gameTime);
        }

        internal void Hit(Unit unit, PositionProvider positionProvider)
        {
            if (!unit.WantsRemoval || mHitUnits.Add(unit))
            {
                // The unit is still alive and wasn't hit before by this projectile.
                HandleHit(unit, positionProvider.Owner);
            }
        }

        /// <summary>
        /// Used to deal damage to <paramref name="unit"/> and possible set the <see cref="WantsRemoval"/> flag or react
        /// in other ways to a hit. Called only when <paramref name="unit"/> was not hit before by this <see cref="Projectile"/>.
        /// </summary>
        /// <param name="unit">The <see cref="Unit"/> hit by this <see cref="Projectile"/>.</param>
        /// <param name="owner">To determine who owns the <paramref name="unit"/>.</param>
        private /*virtual*/ void HandleHit(Unit unit, Owner owner)
        {
            EventCenter.Default.Send(Event.DamagedUnit(owner, this, unit));
            if (unit.DealDamage(Damage))
                EventCenter.Default.Send(Event.KilledUnit(owner, this, unit));

            if (SingleTarget)
                WantsRemoval = true;
        }

        protected void ClearHits()
        {
            mHitUnits.Clear();
        }
    }
}
