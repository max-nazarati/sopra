using System;
using Microsoft.Xna.Framework;


namespace KernelPanic
{
    internal abstract class Unit : Entity
    {
        public Point? MoveTarget { get; set; }

        public int Speed { get; set; }
        public int AttackStrength { get; set; }
        public int MaximumLife { get; set; }
        public int RemainingLife { get; set; }

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
    }
}
