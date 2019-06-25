using System;
using System.Linq;
using System.Runtime.Serialization;
using KernelPanic.Input;
using KernelPanic.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace KernelPanic.Entities
{
    [DataContract]
    [KnownType(typeof(Troupe))]
    [KnownType(typeof(Hero))]
    internal abstract class Unit : Entity
    {
        [DataMember]
        protected Vector2? MoveTarget { get; set; }

        [DataMember]
        private int Speed { get; set; }
        [DataMember]
        private int AttackStrength { get; set; }
        [DataMember]
        private int MaximumLife { get; set; }
        [DataMember(Name = "HP")]
        private int RemainingLife { get; set; }

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

        internal Unit Clone() => Clone<Unit>();

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
        public void DealDamage(int damage)
        {
            RemainingLife = Math.Min(MaximumLife, RemainingLife - damage);
            if (RemainingLife <= 0)
                DidDie();
        }

        /// <summary>
        /// Can be overriden to act when this unit dies.
        /// </summary>
        protected virtual void DidDie()
        {
            SetWantsRemoval();
        }

        #endregion

        /// <summary>
        /// Called when this unit is spawned, the passed action can be used to spawn further units when something
        /// special happens. The spawned units are automatically associated with the correct wave.
        /// </summary>
        /// <param name="spawnAction">To be used to spawn further units for this wave later on.</param>
        public virtual void WillSpawn(Action<Unit> spawnAction)
        {
        }

        protected abstract void CalculateMovement(PositionProvider positionProvider, GameTime gameTime, InputManager inputManager);

        internal override void Update(PositionProvider positionProvider, GameTime gameTime, InputManager inputManager)
        {
            base.Update(positionProvider, gameTime, inputManager);

            CalculateMovement(positionProvider, gameTime, inputManager);

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
                    var theMove = Vector2.Normalize(target - Sprite.Position) * Math.Min(Speed, remainingDistance);
                    Sprite.Position += theMove;
                    CheckBaseReached(positionProvider);
                    move = theMove;
                }
            }
            
            if (!(Sprite is AnimatedSprite animatedSprite))
                return;

            if (move?.X is float x)
                animatedSprite.MovementDirection =
                    x > 0 ? AnimatedSprite.Direction.Right : AnimatedSprite.Direction.Left;
            else
                animatedSprite.MovementDirection = AnimatedSprite.Direction.Standing;
        }

        private void CheckBaseReached(PositionProvider positionProvider)
        {
            if (!positionProvider.Target.HitBox.Any(p => positionProvider.TileBounds(p).Intersects(Sprite.Bounds)))
                return;
                
            DidDie();
            positionProvider.DamageBase(AttackStrength);
        }
    }
}
