using System;
using Microsoft.Xna.Framework;


namespace KernelPanic
{
    internal abstract class Unit : Entity
    {
        private Vector2? MoveTarget { get; set; }

        private int Speed { get; set; }
        private int AttackStrength { get; set; }
        private int MaximumLife { get; set; }
        private int RemainingLife { get; set; }

        private Vector2? MoveVector
        {
            get
            {
                if (!(MoveTarget is Vector2 target))
                    return null;

                return Vector2.Normalize(target - Sprite.Position) * Speed;
            }
        }

        protected Unit(int price, int speed, int life, int attackStrength, Sprite sprite) : base(price, sprite)
        {
            Speed = speed;
            MaximumLife = life;
            RemainingLife = life;
            AttackStrength = attackStrength;
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
        }

        /// <summary>
        /// Called when this unit is spawned, the passed action can be used to spawn further units when something
        /// special happens. The spawned units are automatically associated with the correct wave.
        /// </summary>
        /// <param name="spawnAction">To be used to spawn further units for this wave later on.</param>
        public virtual void WillSpawn(Action<Unit> spawnAction)
        {
        }

        internal override void Update(PositionProvider positionProvider, GameTime gameTime, InputManager inputManager)
        {
            base.Update(positionProvider, gameTime, inputManager);

            if (Selected)
            {
                if (inputManager.MousePressed(InputManager.MouseButton.Right))
                {
                    var mouse = inputManager.TranslatedMousePosition;
                    if (positionProvider.GridCoordinate(mouse) != null)
                        MoveTarget = mouse;
                }
            }

            if (MoveVector is Vector2 movement)
            {
                Sprite.Position += movement;
            }
        }
    }
}
