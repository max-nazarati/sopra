using System.Collections.Generic;
using KernelPanic.Data;
using KernelPanic.Input;
using KernelPanic.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic.Entities.Projectiles
{
    internal class Projectile : IGameObject
    {
        private readonly float mRadius;
        private readonly int mDamage;

        protected ImageSprite Sprite { get; }

        protected Vector2 StartPoint { private get; set; }
        protected Vector2 MoveVector { get; set; }
        private readonly HashSet<Unit> mHitUnits = new HashSet<Unit>();

        internal bool SingleTarget { private get; set; }

        public Projectile(Vector2 direction, Vector2 startPoint, float radius, int speed, int damage, ImageSprite sprite)
        {
            StartPoint = startPoint;
            mRadius = radius;
            mDamage = damage;

            MoveVector = Vector2.Normalize(direction) * speed;

            Sprite = sprite;
            Sprite.Position = startPoint;
            Sprite.Rotation = direction.Angle(0.5);
        }

        public Rectangle Bounds => Sprite.Bounds;

        public int DrawLevel => 2;    // Above all units and buildings.
        
        public bool WantsRemoval { get; protected set; }

        public void Update(PositionProvider positionProvider, InputManager inputManager, GameTime gameTime)
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
        protected virtual void RadiusReached()
        {
            WantsRemoval = true;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Sprite.Draw(spriteBatch, gameTime);
        }

        internal void Hit(Unit unit)
        {
            if (!unit.WantsRemoval || mHitUnits.Add(unit))
            {
                // The unit is still alive and wasn't hit before by this projectile.
                HandleHit(unit);
            }
        }

        /// <summary>
        /// Used to deal damage to <paramref name="unit"/> and possible set the <see cref="WantsRemoval"/> flag or react
        /// in other ways to a hit. Called only when <paramref name="unit"/> was not hit before by this <see cref="Projectile"/>.
        /// </summary>
        /// <param name="unit">The <see cref="Unit"/> hit by this <see cref="Projectile"/>.</param>
        private /*virtual*/ void HandleHit(Unit unit)
        {
            unit.DealDamage(mDamage);

            if (SingleTarget)
                WantsRemoval = true;
        }

        protected void ClearHits()
        {
            mHitUnits.Clear();
        }
    }
}
