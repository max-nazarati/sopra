using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using KernelPanic.Events;
using KernelPanic.Input;
using KernelPanic.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using KernelPanic.Data;


namespace KernelPanic.Entities
{
    [DataContract]
    internal abstract class Unit : Entity
    {
        private ImageSprite mHealthBar;
        private ImageSprite mDamageBar;

        [DataMember] protected internal Vector2? MoveTarget { get; protected set; }

        /// <summary>
        /// The speed (GS) of this unit.
        /// </summary>
        [DataMember]
        internal float Speed { get; set; }

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

        [DataMember]
        private Point mHitBoxSize;

        public override Rectangle Bounds
        {
            get
            {
                var (width, height) = mHitBoxSize;
                var x = (int) (Sprite.X - width / 2f);
                var y = (int) (Sprite.Y - height / 2f);
                return new Rectangle(x, y, width, height);
            }
        }

        protected Unit(int price, int speed, int life, int attackStrength, Point hitBoxSize, Sprite sprite, SpriteManager spriteManager)
            : base(price, hitBoxSize, sprite, spriteManager)
        {
            Speed = speed;
            MaximumLife = life;
            RemainingLife = life;
            AttackStrength = attackStrength;
            ShouldMove = true;
            mHitBoxSize = hitBoxSize;
            mHealthBar = spriteManager.CreateColoredRectangle(1, 1, new[] { new Color(0.0f, 1.0f, 0.0f, 1.0f) });
            mHealthBar.SetOrigin(RelativePosition.TopLeft);
            mDamageBar = spriteManager.CreateColoredRectangle(1, 1, new[] { new Color(1.0f, 0.0f, 0.0f, 1.0f) });
            mDamageBar.SetOrigin(RelativePosition.TopRight);
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

            return (TUnit)Activator.CreateInstance(typeof(TUnit),
                bindingFlags,
                null,
                new object[] { spriteManager },
                null);
        }

        internal Unit Clone() => Clone<Unit>();

        protected override void CompleteClone()
        {
            base.CompleteClone();
            mHealthBar = (ImageSprite)mHealthBar.Clone();
            mDamageBar = (ImageSprite)mDamageBar.Clone();
        }

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
        /// <param name="positionProvider"></param>
        /// <returns><c>true</c> if this <see cref="Unit"/> died, <c>false</c> otherwise.</returns>
        public bool DealDamage(int damage, PositionProvider positionProvider)
        {
            RemainingLife = Math.Min(MaximumLife, RemainingLife - damage);
            if (RemainingLife > 0)
                return false;

            DidDie(positionProvider);
            return true;
        }

        /// <summary>
        /// Can be overriden to act when this unit dies.
        /// </summary>
        /// <param name="positionProvider"></param>
        protected virtual void DidDie(PositionProvider positionProvider)
        {
            WantsRemoval = true;
        }

        #endregion

        #region Movement

        private bool mSlowedDown;
        private Vector2 mLastPosition;

        /// <summary>
        /// Slows this unit for the next frame.
        /// </summary>
        internal void SlowDownForFrame()
        {
            mSlowedDown = true;
        }

        /// <summary>
        /// Calculates the relative movement this unit should complete next. Overrides of this method can look at
        /// <see cref="MoveTarget"/> which is set to <c>null</c> when the last requested movement is completed.
        /// <para>
        /// If the parameter <paramref name="projectionStart"/> is not <c>null</c> this calculation should be based on
        /// its value.
        /// </para>
        /// </summary>
        /// <param name="projectionStart">An alternative to the units current position.</param>
        /// <param name="positionProvider">The current <see cref="PositionProvider"/>.</param>
        /// <param name="inputManager">The <see cref="InputManager"/> associated with this update cycle.</param>
        protected abstract void CalculateMovement(Vector2? projectionStart,
            PositionProvider positionProvider,
            InputManager inputManager);

        private Vector2? PerformMove(Vector2 target,
            PositionProvider positionProvider,
            InputManager inputManager)
        {
            var initialTarget = target;
            var targetMove = target - Sprite.Position;
            var targetDistance = targetMove.Length();

            if (targetDistance < 0.01)
            {
                MoveTarget = null;
                return null;
            }

            var actualSpeed = mSlowedDown ? Speed / 2 : Speed;
            mSlowedDown = false;

            var theMove = Vector2.Zero;

            while (true)
            {
                var normalizedMove = targetMove / targetDistance;

                if (targetDistance > actualSpeed)
                    return theMove + normalizedMove * actualSpeed;

                theMove += targetMove;

                MoveTarget = null;
                CalculateMovement(target, positionProvider, inputManager);
                if (!(MoveTarget is Vector2 projectedTarget))
                {
                    MoveTarget = initialTarget;
                    return theMove;
                }

                targetMove = projectedTarget - target;
                targetDistance = targetMove.Length();
                target = projectedTarget;
            }
        }

        internal void DamageBase(PositionProvider positionProvider)
        {
            EventCenter.Default.Send(Event.DamagedBase(positionProvider.Owner, this));
            positionProvider.Target.Power = Math.Max(0, positionProvider.Target.Power - AttackStrength);
            WantsRemoval = true;
        }

        internal bool ResetMovement()
        {
            if (Sprite.Position == mLastPosition)
                return false;

            Sprite.Position = mLastPosition;
            return true;
        }

        internal void SetInitialPosition(Vector2 position)
        {
            mLastPosition = position;
            Sprite.Position = position;
        }

        #endregion

        internal override void UpdateInformation()
        {
            base.UpdateInformation();
            mInfoText.Text += $"\nLeben: {RemainingLife}";
        }

        public override void Update(PositionProvider positionProvider, InputManager inputManager, GameTime gameTime)
        {
            base.Update(positionProvider, inputManager, gameTime);


            CalculateMovement(null, positionProvider, inputManager);

            mLastPosition = Sprite.Position;
            var move = ShouldMove && MoveTarget is Vector2 target
                ? PerformMove(target, positionProvider, inputManager)
                : null;

            if (move is Vector2 theMove)
            {
                Sprite.Position += theMove;
            }
            
            UpdateHealthBar();

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

        private void UpdateHealthBar()
        {
            const int length = 50;
            const int height = 3;
            mHealthBar.DestinationRectangle = new Rectangle((int) (Sprite.Position.X - length / 2.0),
                (int) (Sprite.Y - Sprite.Height / 1.5),
                (int) (length * (RemainingLife * 1.0f / MaximumLife)),
                height);

            mDamageBar.DestinationRectangle = new Rectangle((int) (Sprite.Position.X + length / 2.0),
                (int) (Sprite.Y - Sprite.Height / 1.5),
                length,
                height);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch, gameTime);
            mDamageBar.Draw(spriteBatch, gameTime);
            mHealthBar.Draw(spriteBatch, gameTime);
        }
    }
}
