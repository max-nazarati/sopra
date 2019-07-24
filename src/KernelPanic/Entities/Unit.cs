using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization;
using Accord.Math;
using KernelPanic.Events;
using KernelPanic.Input;
using KernelPanic.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using KernelPanic.Data;
using KernelPanic.Entities.Units;


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

        [DataMember] // TODO does this fix #270
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
        private Vector2? mLastMoveTarget;

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

            var remainingSpeed = mSlowedDown ? Speed / 2 : Speed;
            mSlowedDown = false;

            var accumulatedMovement = Vector2.Zero;

            while (true)
            {
                var normalizedMove = targetMove / targetDistance;

                // Movement can be completed, speed will get completely used
                if (targetDistance > remainingSpeed)
                    return accumulatedMovement + normalizedMove * remainingSpeed;

                accumulatedMovement += targetMove;
                remainingSpeed -= targetDistance;

                // we exceeded our target, there is still speed left:
                // calculate a new position we want to reach, but now from the previous target as startPoint
                MoveTarget = null;
                CalculateMovement(target, positionProvider, inputManager);

                // We couldn't find a new target (we possibly reached the enemy base)
                if (!(MoveTarget is Vector2 projectedTarget))
                {
                    MoveTarget = initialTarget;
                    return accumulatedMovement;
                }

                // set the new place we want to reach and the vector to get there
                targetMove = projectedTarget - target;
                targetDistance = targetMove.Length();
                target = projectedTarget;
            }
        }

        internal void DamageBase(PositionProvider positionProvider)
        {
            EventCenter.Default.Send(Event.DamagedBase(positionProvider.Owner, this));
            positionProvider.Target.Power = Math.Max(0, positionProvider.Target.Power - AttackStrength);
            positionProvider.Owner[this].UpdateHeroCount(GetType(), -1);
            WantsRemoval = true;
        }

        internal virtual bool ResetMovement()
        {
            if (Sprite.Position == mLastPosition)
                return false;

            Sprite.Position = mLastPosition;
            MoveTarget = mLastMoveTarget;
            return true;
        }

        internal bool MovementResettable => Sprite.Position != mLastPosition;

        internal virtual void SetInitialPosition(Vector2 position)
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
            CalculateMovement(null, positionProvider, inputManager);

            mLastPosition = Sprite.Position;
            mLastMoveTarget = MoveTarget;
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

        #region Flocking

        /// <summary>
        /// adapted version of this tutorial
        /// https://gamedevelopment.tutsplus.com/tutorials/3-simple-rules-of-flocking-behaviors-alignment-cohesion-and-separation--gamedev-3444
        /// </summary>
        /// <param name="positionProvider"></param>
        /// <returns></returns>
        protected Vector2? GetNextMoveVector(PositionProvider positionProvider)
        {
            const int neighbourhoodRadius = 200;

            const float vectorWeight = 90 / 100f; // VectorField (Heatmap)
            const float alignmentWeight = 10 / 100f;
            const float cohesionWeight = 20 / 100f;
            const float separationWeight = 60 / 100f;
            const float obstacleWeight = 10 / 100f;
            const float borderWeight = 30 / 100f;

            #region Get Enumerators

            // Iterate over the Entity Graph only one time and get EVERYTHING
            var rect = new Rectangle(Sprite.Position.ToPoint(), new Point(neighbourhoodRadius));
            // TODO is this rectangle centered?
            var completeNeighbourhood = positionProvider.EntitiesAt(rect);

            // Create explicitly forced Lists to iterate over, depending on which unit we are
            var neighbourTroupes = new List<Troupe>();
            var neighbourBuilding = new List<Building>();
            var neighbourBorder = new List<LaneBorder>();

            if (this is Bug || this is Virus)
            {
                foreach (var thing in completeNeighbourhood)
                {
                    switch (thing)
                    {
                        case Bug bug:
                            neighbourTroupes.Add(bug);
                            continue;
                        case Virus virus:
                            neighbourTroupes.Add(virus);
                            continue;
                        case Building building:
                            neighbourBuilding.Add(building);
                            continue;
                        case LaneBorder laneBorder:
                            if (!laneBorder.IsTargetBorder)
                            {
                                neighbourBorder.Add(laneBorder);
                            }
                            continue;
                        default:
                            continue;
                    }
                }
            }
            else if (this is Nokia || this is Trojan)
            {
                foreach (var thing in completeNeighbourhood)
                {
                    switch (thing)
                    {
                        case Trojan trojan:
                            neighbourTroupes.Add(trojan);
                            continue;
                        case Nokia nokia:
                            neighbourTroupes.Add(nokia);
                            continue;
                        case Building building:
                            neighbourBuilding.Add(building);
                            continue;
                        case LaneBorder laneBorder:
                            if (!laneBorder.IsTargetBorder)
                            {
                                neighbourBorder.Add(laneBorder);
                            }
                            continue;
                        default:
                            continue;
                    }
                }
            }
            else if (this is Thunderbird)
            {
                foreach (var thing in completeNeighbourhood)
                {
                    switch (thing)
                    {
                        case Thunderbird thunderbird:
                            neighbourTroupes.Add(thunderbird);
                            continue;
                        case LaneBorder laneBorder:
                            if (!laneBorder.IsTargetBorder)
                            {
                                neighbourBorder.Add(laneBorder);
                            }
                            continue;
                        default:
                            continue;
                    }
                }
            }

            #endregion
            
            // var move = Vector2.Zero;
            var vector = positionProvider.TroupeData.RelativeMovement(this, Sprite.Position);
            if (float.IsNaN(vector.X) || vector == Vector2.Zero)
            {
                vector = Vector2.Zero;
            }
            else
            {
                vector.Normalize();
            }
            
            // using the 3 different lists to iterate over
            var alignment = ComputeFlockingAlignment(positionProvider, neighbourTroupes);
            // var cohesion = ComputeFlockingCohesion(neighbourTroupes);
            var separation = ComputeFlockingSeparation(neighbourTroupes);
            var obstacle = ComputeFlockingBuilding(neighbourBuilding);
            // var border = ComputeFlockingBorder(neighbourBorder);

            // putting all pieces together with their individual weight
            var move = Vector2.Zero;

            move += vectorWeight * vector;
            move += alignmentWeight * alignment;
            // move += cohesionWeight * cohesion;
            move += separationWeight * separation;
            move += obstacleWeight * obstacle;
            // move = borderWeight * border;

            move.Normalize();
            move *= Speed;
            return float.IsNaN(move.X) ? Vector2.Zero : move;

        }

        /// <summary>
        /// Alignment is a behavior that causes a particular Troupe to line up with Troupes close by
        /// </summary>
        /// <returns>A normalized vector or zero</returns>
        private Vector2 ComputeFlockingAlignment(PositionProvider positionProvider, IEnumerable<Troupe> neighbourhood)
        {
            var result = Vector2.Zero;
            var neighbourCount = 0;

            foreach (var unit in neighbourhood)
            {
                result += positionProvider.TroupeData.RelativeMovement(unit, unit.Sprite.Position);
                neighbourCount++;
            }

            if (neighbourCount == 0)
            {
                return result;
            }

            // Console.WriteLine("Alignment: " + result);
            result.Normalize();
            // Console.WriteLine("Alignment Normalized: " + result);

            // NaN after Normalize?
            return float.IsNaN(result.X) ? Vector2.Zero : result;
        }

        /// <summary>
        /// Cohesion is a behavior that causes agents to steer towards the "center of mass" - that is,
        /// the average position of the agents within a certain radius
        /// </summary>
        /// <returns>A normalized vector or zero</returns>
        private Vector2 ComputeFlockingCohesion(IEnumerable<Troupe> neighbourhood)
        {
            var center = Vector2.Zero;
            var neighbourCount = 0;

            foreach (var unit in neighbourhood)
            {
                center += unit.Sprite.Position;
                neighbourCount++;
            }

            if (neighbourCount == 0)
            {
                return Vector2.Zero;
            }

            center.X /= neighbourCount;
            center.Y /= neighbourCount;

            // we dont want the center of mass but our direction to it
            var result = center - Sprite.Position;
            // Console.WriteLine("Cohesion: " + result);
            result.Normalize();
            // Console.WriteLine("Cohesion Normalized: " + result);

            // NaN after Normalize
            return float.IsNaN(result.X) ? Vector2.Zero : result;
        }

        /// <summary>
        /// Separation is the behavior that causes an agent to steer away from all of its neighbors
        /// Should return a negative value, so we move away from the other units
        /// </summary>
        /// <returns>A normalized negative vector, or zero</returns>
        private Vector2 ComputeFlockingSeparation(IEnumerable<Troupe> neighbourhood)
        {
            var result = Vector2.Zero;
            var neighbourCount = 0;

            foreach (var unit in neighbourhood)
            {
                result += unit.Sprite.Position - Sprite.Position;
                neighbourCount++;
            }

            if (neighbourCount == 0)
            {
                return result;
            }

            // Console.WriteLine("Separation: " + result);
            result.Normalize();
            // Console.WriteLine("Separation Normalized: " + result);
            // NaN after Normalize
            return float.IsNaN(result.X) ? Vector2.Zero : -1 * result;
        }

        private Vector2 ComputeFlockingBuilding(IEnumerable<Building> neighbourhood)
        {
            var result = Vector2.Zero;
            if (this is Thunderbird)
            {
                return result;
            }

            Building closestBuilding = null;
            var minDistanceSq = float.NaN;
            foreach (var building in neighbourhood)
            {
                var distanceSq = Vector2.DistanceSquared(Sprite.Position, building.Sprite.Position);
                if (distanceSq < minDistanceSq || float.IsNaN(minDistanceSq))
                {
                    closestBuilding = building;
                    minDistanceSq = distanceSq;
                }
            }

            // can only be NaN if we never found a building
            if (float.IsNaN(minDistanceSq))
            {
                return result;
            }
            result = Sprite.Position - closestBuilding.Sprite.Position;
            result.Normalize();
            // return result; // dont need to return negative, we directly calculated reversed
            return float.IsNaN(result.X) ? Vector2.Zero : result;
        }

        /// <summary>
        /// This function does not check for IsTargetBorder
        /// </summary>
        /// <param name="neighbourhood"></param>
        /// <returns></returns>
        private Vector2 ComputeFlockingBorder(IEnumerable<LaneBorder> neighbourhood)
        {
            var closestBorderPosition = new Vector2(float.NaN);
            var minDistance = float.NaN;
            foreach (var border in neighbourhood)
            {
                // corner point of the rectangle
                var point1 = new Vector2(border.Bounds.X, border.Bounds.Y);
                var point2 = new Vector2(border.Bounds.X + border.Bounds.Width, border.Bounds.Y + border.Bounds.Height);
                var distance = 0f;
                var horizontal = false;

                var x = Sprite.Position.X;
                var y = Sprite.Position.Y;
                if (x > point1.X && x < point2.X)
                {
                    distance = Math.Min(y - point1.Y, y - point2.Y);
                }
                if (y > point1.Y && y < point2.Y)
                {
                    distance = Math.Min(x - point1.X, x - point2.X);
                    horizontal = true;
                }

                if (distance < minDistance || float.IsNaN(minDistance))
                {
                    minDistance = distance;
                    if (horizontal)
                    {
                        closestBorderPosition.X = distance;
                        closestBorderPosition.Y = 0;
                    }
                    else
                    {
                        closestBorderPosition.X = 0;
                        closestBorderPosition.Y = distance;
                    }
                }

                return closestBorderPosition;
            }

            // probably could not find a border close by
            if (float.IsNaN(closestBorderPosition.X) || float.IsNaN(closestBorderPosition.Y))
            {
                return Vector2.Zero;
            }

            // now that we found the closest, calculate the distance from there to this
            var result = Sprite.Position - closestBorderPosition; 
            return result;
        }

        #endregion


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
