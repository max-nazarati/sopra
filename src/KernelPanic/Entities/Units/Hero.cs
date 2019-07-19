// #define DEBUG
#undef DEBUG

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using KernelPanic.Data;
using KernelPanic.Events;
using KernelPanic.Input;
using KernelPanic.Interface;
using KernelPanic.PathPlanning;
using KernelPanic.Players;
using KernelPanic.Sprites;
using KernelPanic.Table;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace KernelPanic.Entities.Units
{
    [DataContract]
    internal abstract class Hero : Unit
    {
        #region MemberVariables

        #region Enums
        
        protected enum AbilityState
        {
            Ready = 0, // default value since it has the value '0'
            Indicating, Starting, Active, Finished, CoolingDown
        }

        protected enum Strategy
        {
            Human = 0,
            Attack,
            Econ
        }
        
        #endregion

        [DataMember]
        protected internal CooldownComponent Cooldown { get; set; }

        private TileIndex? mTarget;
        protected AStar mAStar; // save the AStar for path-drawing
        private Visualizer mPathVisualizer;

        internal double RemainingCooldownTime => Cooldown.RemainingCooldown.TotalSeconds;
        protected AbilityState AbilityStatus { get; set; }
        protected Strategy StrategyStatus { get; set; }

        #endregion

        #region Konstruktor / Create

        protected Hero(int price, int speed, int life, int attackStrength, TimeSpan coolDown, Point hitBoxSize, Sprite sprite, SpriteManager spriteManager)
            : base(price, speed, life, attackStrength, hitBoxSize, sprite, spriteManager)
        {
            ShouldMove = false;
            Cooldown = new CooldownComponent(coolDown, false);
            Cooldown.CooledDown += component => AbilityStatus = AbilityState.Ready;
        }

        #endregion

        #region Movement

        protected override void CalculateMovement(Vector2? projectionStart,
            PositionProvider positionProvider,
            InputManager inputManager)
        {
            if (projectionStart != null)
                return;

            if (MoveTarget == null && mTarget is TileIndex tileTarget)
            {
                MoveTargetReached(positionProvider, tileTarget);
            }

            if (MoveTarget != null && mTarget == null)
            {
                // We have a MoveTarget but we don't have a tile target, that means we are doing the last steps in the
                // direction of the enemy base. We don't want to allow any more changes to the path at this point.
                return;
            }

            UpdateTarget(positionProvider, inputManager);

            if (!(mTarget?.ToPoint() is Point target))
            {
                return;
            }

            // calculate the path
            mAStar = positionProvider.MakePathFinding(this, new[] {target});
            mPathVisualizer = positionProvider.Visualize(mAStar);
            var path = mAStar.Path;
            if (path == null || path.Count == 0) // there is no path to be found
            {
                target = FindNearestWalkableField(target);
                mAStar = positionProvider.MakePathFinding(this, new[] {target});
                mPathVisualizer = positionProvider.Visualize(mAStar);
                path = mAStar.Path;
            }

            var nextTarget = new TileIndex(path.Count > 2 ? path[1] : target, 1);
            MoveTarget = positionProvider.Grid.GetTile(nextTarget).Position;
        }
        
        private void MoveTargetReached(PositionProvider positionProvider, TileIndex tileTarget)
        {
            var troupeData = positionProvider.TroupeData;
            if (troupeData.Target.Contains(tileTarget.ToPoint()))
                MoveTarget = Sprite.Position + troupeData.RelativeMovement(this);

            mAStar = null;
            mTarget = null;
            mPathVisualizer = null;
        }

        /// <summary>
        /// This method should check for a new target to walk to and saves it in mTarget
        /// also sets mShouldMove true
        /// </summary>
        /// <param name="positionProvider"></param>
        /// <param name="inputManager"></param>
        private void UpdateTarget(PositionProvider positionProvider, InputManager inputManager)
        {
            if (StrategyStatus == Strategy.Attack)
            {
                AttackBase(inputManager, positionProvider);
                return;
            }

            if (StrategyStatus == Strategy.Econ)
            {
                SlowPush(inputManager, positionProvider);
                return;
            }

            // only check for new target of selected and Right Mouse Button was pressed
            if (!Selected) return;
            if (!inputManager.MousePressed(InputManager.MouseButton.Right)) return;

            var mouse = inputManager.TranslatedMousePosition;
            if (!(positionProvider.Grid.TileFromWorldPoint(mouse) is TileIndex target))
                return;

            ShouldMove = true;
            mTarget = target;
        }

        private Point FindNearestWalkableField(Point target)
        {
            var result = mAStar.FindNearestField();
            return result ?? new Point((int)Sprite.Position.X, (int)Sprite.Position.Y);
        }
        
        #endregion Movement

        #region Ability

        protected virtual void UpdateAbility(PositionProvider positionProvider, GameTime gameTime, InputManager inputManager)
        {
            #region DEBUG
#if DEBUG
            if (!Selected) { goto SwitchAbilityState; }
            Console.WriteLine("################################################################################");
            Console.WriteLine("[TIME:] " + gameTime.TotalGameTime);
            Console.WriteLine(this + " WANTS TO UPDATED HIS ABILITY!");
            Console.Write("CURRENT ABILITY STATE IS: ");

            switch (AbilityStatus)
            {
                case AbilityState.Ready:
                    Console.WriteLine("Ready");
                    break;

                case AbilityState.Indicating:
                    Console.WriteLine("Indicating");
                    break;

                case AbilityState.Active:
                    Console.WriteLine("Active");
                    break;

                case AbilityState.Starting:
                    Console.WriteLine("Starting");
                    break;

                case AbilityState.Finished:
                    Console.WriteLine("Finished");
                    break;

                case AbilityState.CoolingDown:
                    Console.WriteLine("CoolingDown");
                    break;
            }

            Console.WriteLine("################################################################################"+'\n'); 

            SwitchAbilityState:
#endif
            #endregion
            
            switch (AbilityStatus)
            {
                case AbilityState.Ready:
                    // Here we should check if we should start to indicate the ability
                    TryActivateAbility(inputManager);
                    if (AbilityStatus == AbilityState.Indicating)
                    {
                        goto case AbilityState.Indicating;
                    }
                    break;
                
                case AbilityState.Indicating:
                    // sets the next AbilityState if wanted.
                    IndicateAbility(positionProvider, inputManager);
                    break;

                case AbilityState.Starting:
                    // initialize the ability
                    StartAbility(positionProvider, inputManager);
                    break;
                
                case AbilityState.Active:
                    // take one action per update cycle until the ability is finished
                    ContinueAbility(gameTime);
                    break;

                case AbilityState.Finished:
                    // finally cleaning up has to be done and starting to cool down
                    FinishAbility();
                    break;

                case AbilityState.CoolingDown:
                    UpdateCooldown(gameTime, positionProvider);
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected virtual void UpdateCooldown(GameTime gameTime, PositionProvider positionProvider)
        {
            Cooldown.Update(gameTime);
        }

        protected virtual void TryActivateAbility(InputManager inputManager, bool button = false)
        {
            if (CheckAbilityStart(inputManager, button))
                AbilityStatus = AbilityState.Indicating;
        }

        protected virtual bool CheckAbilityStart(InputManager inputManager, bool button = false)
        {
            return Selected  && Cooldown.Ready && (inputManager.KeyPressed(Keys.Q) || inputManager.MousePressed(InputManager.MouseButton.Middle) || button);
        }

        protected virtual void IndicateAbility(PositionProvider positionProvider, InputManager inputManager)
        {
            // just quit indicating when not selected anymore
            if (!Selected)
            {
                AbilityStatus = AbilityState.Ready;
                return;
            }
            
            if (inputManager.KeyPressed(Keys.Q) || inputManager.MousePressed(InputManager.MouseButton.Middle))
            {
                AbilityStatus = AbilityState.Starting;
            }
            
            else if (inputManager.KeyPressed(Keys.E) || inputManager.MousePressed(InputManager.MouseButton.Right))
            {
                AbilityStatus = AbilityState.Ready;
            }
            else
            {
                AbilityStatus = AbilityState.Indicating;
            }
        }

        protected virtual void StartAbility(PositionProvider positionProvider, InputManager inputManager)
        {
            #region DEBUG
#if DEBUG
            Console.WriteLine("Ability is now initialized.");
#endif
            #endregion
            
            AbilityStatus = AbilityState.Active;
            Cooldown.Reset();
            
            // For sound
            EventCenter.Default.Send(Event.HeroAbility(this));
        }
        
        protected virtual void ContinueAbility(GameTime gameTime)
        {
            // Console.WriteLine(this + " JUST USED HIS ABILITY! (virtual method of class Hero)  [TIME:] " + gameTime.TotalGameTime);
            AbilityStatus = AbilityState.Finished;
        }

        protected virtual void FinishAbility()
        {
            ShouldMove = true;
            AbilityStatus = AbilityState.CoolingDown;
            Cooldown.Reset();
        }
        
        #endregion Ability

        #region KI

        internal override void AttackBase(InputManager inputManager, PositionProvider positionProvider)
        {
            var grid = positionProvider.Grid;
            var basePosition = Base.TargetPoints(grid.LaneRectangle.Size, grid.LaneSide);
            // TODO: is there a function for the calculation below?
            //       something like Grid.WorldPositionFromTile(basePosition);
            mAStar = positionProvider.MakePathFinding(this, basePosition);
            mTarget = new TileIndex(mAStar.Path[mAStar.Path.Count - 1], 1);
            ShouldMove = true;
        }

        /// <summary>
        /// try not to die while attacking.
        /// sets mTarget and ShouldMove
        /// </summary>
        protected virtual void SlowPush(InputManager inputManager, PositionProvider positionProvider)
        {
            #region get all translation for the 8 adjacent tiles
            var startPoint = positionProvider.RequireTile(Sprite.Position).ToPoint();
            var neighboursPosition = new List<(int, int)>()
            {
                         (-1, -1),  (0, -1), (1, -1),
                         (-1,  0),           (1,  0), // (0, 0) is the point itself
                         (-1,  1),  (0, 1),  (1,  1),

                // increases the radius by 1:
                (-2, -2), (-1, -2), (0, -2), (1, -2), (2, -2),
                (-2, -1),                             (2, -1),
                (-2,  0),                             (2,  0),
                (-2,  1),                             (2,  1),
                (-2,  2), (-1,  2), (0,  2), (1,  2), (2,  2)
            };
            #endregion

            #region add the points of the adjacent neighbours into a list (if they are on the lane)
            var neighbours = new List<Point>();
            foreach (var (x, y) in neighboursPosition)
            {
                try
                {
                    neighbours.Add(
                        positionProvider.RequireTile(
                            new Vector2(Sprite.Position.X + x * Grid.KachelSize,
                                        Sprite.Position.Y + y * Grid.KachelSize)
                            ).ToPoint());
                }
                catch (InvalidOperationException)
                {
                    // just dont add the point
                }
            }
            #endregion

            #region calculate a heuristic for all neighbours and choose the best
            var bestPoint = startPoint;
            var bestValue = 0f;
            foreach (var point in neighbours)
            {
                var currentValue = PointHeuristic(point, positionProvider);
                if (currentValue <= bestValue)
                    continue;

                bestPoint = point;
                bestValue = currentValue;
            }
            #endregion
            
            #region set the optimum as walking target
            mAStar = positionProvider.MakePathFinding(this, new[] {bestPoint});
            if (mAStar.Path != null)
            {
                if (mAStar.Path[mAStar.Path.Count - 1] is Point target)
                {
                    mTarget = new TileIndex(target, 1);
                }
            }
            
            ShouldMove = true;
            #endregion
        }

        protected virtual float PointHeuristic(Point point, PositionProvider positionProvider)
        {
            return 0;
        }

        #endregion

        #region Update

        public override void Update(PositionProvider positionProvider, InputManager inputManager, GameTime gameTime)
        {
            if (Selected)
            {
                if (inputManager.KeyPressed(Keys.Space))
                {
                    StrategyStatus = StrategyStatus == Strategy.Human ? Strategy.Attack : Strategy.Human;
                }

                if (inputManager.KeyPressed(Keys.O))
                {
                    StrategyStatus = StrategyStatus == Strategy.Human ? Strategy.Econ : Strategy.Human;
                }
            }
            // also updates the cooldown
            UpdateAbility(positionProvider, gameTime, inputManager);

            // base.Update checks for ShouldMove
            base.Update(positionProvider, inputManager, gameTime);
            
        }
        
        #endregion

        #region Draw

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            DrawAStarPath(spriteBatch, gameTime);
            base.Draw(spriteBatch, gameTime);
            if (!Selected)
            {
                return;
            }
            DrawAbility(spriteBatch, gameTime);
        }

        protected virtual void DrawAbility(SpriteBatch spriteBatch, GameTime gameTime)
        {
        }
        
        private void DrawAStarPath(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (Selected && DebugSettings.VisualizeAStar)
            {
                mPathVisualizer?.Draw(spriteBatch, gameTime);
            }
        }

        #endregion

        #region Actions

        protected override IEnumerable<IAction> Actions(Player owner) =>
            base.Actions(owner).Extend(new AbilityAction(this, SpriteManager));
        
        private sealed class AbilityAction : IAction
        {
            public Button Button { get; }

            internal AbilityAction(Hero hero, SpriteManager sprites)
            {
                Button = new AnimatedButton(sprites, hero) {Title = "Fähigkeit"};
                Button.Clicked += (button, inputManager) => hero.TryActivateAbility(inputManager, true);
            }

            void IUpdatable.Update(InputManager inputManager, GameTime gameTime) => 
                Button.Update(inputManager, gameTime);

            void IDrawable.Draw(SpriteBatch spriteBatch, GameTime gameTime) =>
                Button.Draw(spriteBatch, gameTime);
        }

        #endregion
    }
}
