using System;
using System.Runtime.Serialization;
using KernelPanic.Input;
using KernelPanic.Sprites;
using Microsoft.Xna.Framework;


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

        protected bool ShouldMove; // should the basic movement take place this cycle? 

        protected virtual Vector2? MoveVector
        {
            get
            {
                if (!(MoveTarget is Vector2 target))
                    return null;

                return Vector2.Normalize(target - Sprite.Position) * Speed;
            }
        }

        protected Unit(int price, int speed, int life, int attackStrength, Sprite sprite, SpriteManager spriteManager)
            : base(price, sprite, spriteManager)
        {
            Speed = speed;
            MaximumLife = life;
            RemainingLife = life;
            AttackStrength = attackStrength;
            ShouldMove = true;
            mDidDie = false;
        }

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
            mDidDie = true;
        }

        /// <summary>
        /// Called when this unit is spawned, the passed action can be used to spawn further units when something
        /// special happens. The spawned units are automatically associated with the correct wave.
        /// </summary>
        /// <param name="spawnAction">To be used to spawn further units for this wave later on.</param>
        public virtual void WillSpawn(Action<Unit> spawnAction)
        {
        }

        protected virtual void CalculateMovement(PositionProvider positionProvider, GameTime gameTime, InputManager inputManager)
        {
            if (Selected)
            {
                if (inputManager.MousePressed(InputManager.MouseButton.Right))
                {
                    var mouse = inputManager.TranslatedMousePosition;
                    if (positionProvider.GridCoordinate(mouse) != null)
                        MoveTarget = mouse;
                }
            }
        }

        internal override void Update(PositionProvider positionProvider, GameTime gameTime, InputManager inputManager)
        {
            base.Update(positionProvider, gameTime, inputManager);

            CalculateMovement(positionProvider, gameTime, inputManager);
            if (Sprite is AnimatedSprite animation)
            {
                // children - classes want to know if movement is allowed(mShouldMove)
                if (ShouldMove && MoveVector is Vector2 movement)
                {
                    Sprite.Position += movement;
                    // choose correct movement animation
                    if (Math.Abs(movement.X) >= Math.Abs(movement.Y))
                    {
                        if (movement.X > 0)
                        {
                            animation.mMovement = AnimatedSprite.Movement.Right;
                        }
                        else
                        {
                            animation.mMovement = AnimatedSprite.Movement.Left;
                        }
                    }
                    else
                    {
                        if (movement.Y > 0)
                        {
                            animation.mMovement = AnimatedSprite.Movement.Down;
                        }
                        else
                        {
                            animation.mMovement = AnimatedSprite.Movement.Up;
                        }
                    }
                }
                else
                {
                    animation.mMovement = AnimatedSprite.Movement.Standing;
                }
            }

            // children-classes want to know if movement is allowed (mShouldMove) 
            /*if (ShouldMove && MoveVector is Vector2 movement)
            {
                Sprite.Position += movement;
            }*/

        }
        
    }
}
