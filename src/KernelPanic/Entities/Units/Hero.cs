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

        #endregion

        [DataMember]
        protected internal CooldownComponent Cooldown { get; set; }

        protected TileIndex? mTarget;
        protected AStar mAStar; // save the AStar for path-drawing
        private Lazy<Visualizer> mPathVisualizer;

        [DataMember]
        protected AbilityState AbilityStatus { get; set; }

        [DataMember]
        internal bool IsAutonomous { private get; set; }

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

        protected override void DidDie(PositionProvider positionProvider)
        {
            base.DidDie(positionProvider);
            positionProvider.Owner[this].UpdateHeroCount(GetType(), -1);
        }

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
            mPathVisualizer = new Lazy<Visualizer>(() => positionProvider.Visualize(mAStar), false);
            var path = mAStar.Path;
            if (path == null || path.Count == 0) // there is no path to be found
            {
                target = FindNearestWalkableField();
                mAStar = positionProvider.MakePathFinding(this, new[] {target});
                mPathVisualizer = new Lazy<Visualizer>(() => positionProvider.Visualize(mAStar), false);
                path = mAStar.Path;
            }

            if (path == null)
            {
                MoveTarget = null;
                MoveVector = Vector2.Zero;
                return;
            }

            var nextTarget = new TileIndex(path.Count > 2 ? path[1] : target, 1);
            MoveTarget = positionProvider.Grid.GetTile(nextTarget).Position;
            MoveVector = MoveTarget - Sprite.Position;
        }
        
        private void MoveTargetReached(PositionProvider positionProvider, TileIndex tileTarget)
        {
            var troupeData = positionProvider.TroupeData;
            if (troupeData.Target.Contains(tileTarget.ToPoint()))
                MoveTarget = Sprite.Position + troupeData.RelativeMovement(this);

            mAStar = null;
            mTarget = null;
            MoveVector = null;
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
            if (IsAutonomous)
            {
                AutonomousAttack(inputManager, positionProvider);
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

        private Point FindNearestWalkableField()
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
                    ContinueAbility(positionProvider);
                    break;

                case AbilityState.Finished:
                    // finally cleaning up has to be done and starting to cool down
                    FinishAbility(positionProvider);
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
        
        protected virtual void ContinueAbility(PositionProvider positionProvider)
        {
            // Console.WriteLine(this + " JUST USED HIS ABILITY! (virtual method of class Hero)  [TIME:] " + gameTime.TotalGameTime);
            AbilityStatus = AbilityState.Finished;
        }

        protected virtual void FinishAbility(PositionProvider positionProvider)
        {
            ShouldMove = true;
            AbilityStatus = AbilityState.CoolingDown;
            Cooldown.Reset();
        }
        
        #endregion Ability

        #region KI

        protected virtual void AutonomousAttack(InputManager inputManager, PositionProvider positionProvider)
        {
            var grid = positionProvider.Grid;
            var basePosition = Base.TargetPoints(grid.LaneRectangle.Size, grid.LaneSide);
            // TODO: is there a function for the calculation below?
            //       something like Grid.WorldPositionFromTile(basePosition);
            mAStar = positionProvider.MakePathFinding(this, basePosition);
            if (mAStar.Path == null)
            {
                return;
            }
            mTarget = new TileIndex(mAStar.Path[mAStar.Path.Count - 1], 1);
            ShouldMove = true;
        }

        /// <summary>
        /// A List of the world positions of adjacent Tiles
        /// </summary>
        /// <param name="positionProvider"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        protected List<Point> GetNeighbours(PositionProvider positionProvider, int distance = 2)
        {
            var neighboursPosition = new List<(int, int)>()
            {
                (-1, -1), (0, -1), (1, -1),
                (-1,  0),          (1,  0), // (0, 0) is the point itself
                (-1,  1), (0,  1), (1,  1)
            };
            if (distance >= 2)
            {   // increases the radius by 1:
                neighboursPosition.AddRange(new[]
                {
                    (-2, -2), (-1, -2), (0, -2), (1, -2), (2, -2),
                    (-2, -1), (2, -1),
                    (-2, 0), (2, 0),
                    (-2, 1), (2, 1),
                    (-2, 2), (-1, 2), (0, 2), (1, 2), (2, 2)
                });
            }
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

            return neighbours;
        }

        #endregion

        #region Update

        public override void Update(PositionProvider positionProvider, InputManager inputManager, GameTime gameTime)
        {
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
                mPathVisualizer?.Value?.Draw(spriteBatch, gameTime);
            }
        }

        #endregion

        #region Actions

        protected override IEnumerable<IAction> Actions(Player owner) =>
            base.Actions(owner).Extend(new AbilityAction(this, SpriteManager));

        private sealed class AbilityAction : IAction
        {
            private Hero Hero { get; }
            public Button Button { get; }
            private readonly ImageSprite mOverlay;

            internal AbilityAction(Hero hero, SpriteManager sprites)
            {
                Hero = hero;
                Button = new TextButton(sprites) {Title = "Fähigkeit"};
                Button.Clicked += (button, inputManager) => hero.TryActivateAbility(inputManager, true);
                mOverlay = sprites.CreateColoredRectangle(1, 1, new[] {new Color(0.8f, 0.8f, 0.8f, 0.5f)});
            }

            void IUpdatable.Update(InputManager inputManager, GameTime gameTime)
            {
                var cooldown = Hero.Cooldown;
                var width = (int) ((float) cooldown.RemainingCooldown.Ticks / cooldown.Cooldown.Ticks * Button.Size.X);
                var size = new Point(width, (int) Button.Sprite.Height);
                Button.Update(inputManager, gameTime);
                Button.Enabled = cooldown.Ready;
                mOverlay.DestinationRectangle = new Rectangle(Button.Position.ToPoint(), size);
            }

            void IDrawable.Draw(SpriteBatch spriteBatch, GameTime gameTime)
            {
                Button.Draw(spriteBatch, gameTime);
                mOverlay.Draw(spriteBatch, gameTime);
            }
        }

        #endregion
    }
}
