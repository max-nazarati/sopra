using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using KernelPanic.Events;
using KernelPanic.Input;
using KernelPanic.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace KernelPanic.Entities
{
    [DataContract]
    internal abstract class Unit : Entity
    {
        [DataMember]
        protected Vector2? MoveTarget { get; set; }

        /// <summary>
        /// The speed (GS) of this unit.
        /// </summary>
        [DataMember]
        protected int Speed { get; set; }

        /// <summary>
        /// The AS of this Unit. This is the damage dealt to the enemy's base if reached.
        /// </summary>
        [DataMember]
        internal int AttackStrength { get; set; }

        /// <summary>
        /// Stores the initial/maximum LP. Kept to ensure,
        /// that <see cref="RemainingLife"/> won't increase above the maximum life.
        /// </summary>
        [DataMember]
        internal int MaximumLife { get; set; }

        /// <summary>
        /// Stores the current/remaining LP. If this goes to zero or below, this unit is considered to be dead.
        /// </summary>
        [DataMember]
        internal int RemainingLife { get; set; }

        protected bool ShouldMove { get; set; } // should the basic movement take place this cycle? 

        protected delegate void MoveTargetReachedDelegate(Vector2 target);
        protected event MoveTargetReachedDelegate MoveTargetReached;

        protected Unit(int price, int speed, int life, int attackStrength, Sprite sprite, SpriteManager spriteManager)
            : base(price, sprite, spriteManager)
        {
            Speed = speed;
            MaximumLife = life;
            RemainingLife = life;
            AttackStrength = attackStrength;
            ShouldMove = true;
        }

        /// <summary>
        /// Creates an object of type <typeparamref name="TUnit"/> using reflection. <typeparamref name="TUnit"/> should
        /// have a one-argument constructor which takes a <see cref="SpriteManager"/>.
        ///
        /// <para>
        /// Prefer the explicit constructor if possible.
        /// </para>
        /// </summary>
        /// <param name="spriteManager">The <see cref="SpriteManager"/> passed to the constructor.</param>
        /// <typeparam name="TUnit">The type of <see cref="Unit"/> to create.</typeparam>
        /// <returns>A new instance of <typeparamref name="TUnit"/>.</returns>
        internal static TUnit Create<TUnit>(SpriteManager spriteManager) where TUnit : Unit
        {
            const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.CreateInstance |
                                              BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

            return (TUnit) Activator.CreateInstance(typeof(TUnit),
                bindingFlags,
                null,
                new object[] {spriteManager},
                null);
        }

        internal Unit Clone() => Clone<Unit>();

        public override int? DrawLevel => 1;    // Units are between buildings and projectiles.

        #region Taking damage

        /// <summary>
        /// <para>
        /// Subtracts the damage from the remaining life and calls <see cref="DidDie"/> if the result is zero or less.
        /// </para>
        /// <para>
        /// This function can be used to increase this units health by passing a negative value for
        /// <paramref name="damage"/>. <see cref="RemainingLife"/> won't rise above <see cref="MaximumLife"/>.
        /// </para>
        /// </summary>
        /// <param name="damage">The number of life-points to subtract.</param>
        /// <returns><c>true</c> if this <see cref="Unit"/> died, <c>false</c> otherwise.</returns>
        public bool DealDamage(int damage)
        {
            RemainingLife = Math.Min(MaximumLife, RemainingLife - damage);
            if (RemainingLife > 0)
                return false;

            DidDie();
            return true;
        }

        /// <summary>
        /// Can be overriden to act when this unit dies.
        /// </summary>
        protected virtual void DidDie()
        {
            WantsRemoval = true;
        }

        #endregion

        private bool mSlowedDown;

        /// <summary>
        /// Slows this unit for the next frame.
        /// </summary>
        internal void SlowDownForFrame()
        {
            mSlowedDown = true;
        }

        protected abstract void CalculateMovement(PositionProvider positionProvider, GameTime gameTime, InputManager inputManager);

        public override void Update(PositionProvider positionProvider, InputManager inputManager, GameTime gameTime)
        {
            base.Update(positionProvider, inputManager, gameTime);

            CalculateMovement(positionProvider, gameTime, inputManager);

            var actualSpeed = mSlowedDown ? Speed / 2 : Speed;
            mSlowedDown = false;

            var move = (Vector2?) null;
            if (ShouldMove && MoveTarget is Vector2 target)
            {
                var remainingDistance = Vector2.Distance(Sprite.Position, target);
                if (remainingDistance < 0.1)
                {
                    MoveTargetReached?.Invoke(target);
                    MoveTarget = null;
                }
                else
                {
                    var theMove = Vector2.Normalize(target - Sprite.Position) * Math.Min(actualSpeed, remainingDistance);
                    Sprite.Position += theMove;
                    CheckBaseReached(positionProvider);
                    move = theMove;
                }
            }

            if (!(Sprite is AnimatedSprite animated))
                return;

            if (move?.X is float x)
            {
                // choose correct movement direction based on x value or direction of idle animation
                animated.MovementDirection = (animated.Effect == SpriteEffects.None && (int)x == 0) || x < 0
                    ? AnimatedSprite.Direction.Left
                    : AnimatedSprite.Direction.Right;
            }
            else
                animated.MovementDirection = AnimatedSprite.Direction.Standing;
        }

        private void CheckBaseReached(PositionProvider positionProvider)
        {
            if (!positionProvider.Target.HitBox.Any(p => positionProvider.TileBounds(p).Intersects(Sprite.Bounds)))
                return;

            EventCenter.Default.Send(Event.DamagedBase(positionProvider.Owner, this));
            positionProvider.DamageBase(AttackStrength);
            WantsRemoval = true;
        }
    }
}
