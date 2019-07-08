// #define DEBUG
#undef DEBUG

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using KernelPanic.Data;
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
    [KnownType(typeof(Firefox))]
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
            Attack
        }
        
        #endregion

        [DataMember]
        protected CooldownComponent Cooldown { get; set; }
        internal AStar mAStar; // save the AStar for path-drawing
        private Point? mTarget; // the target we wish to move to
        private Visualizer mPathVisualizer;
        internal double RemainingCooldownTime => Cooldown.RemainingCooldown.TotalSeconds;
        protected AbilityState AbilityStatus { get; set; }
        protected Strategy StrategyStatus { get; set; }

        #endregion

        #region Konstruktor / Create

        protected Hero(int price, int speed, int life, int attackStrength, TimeSpan coolDown, Sprite sprite, SpriteManager spriteManager)
            : base(price, speed, life, attackStrength, sprite, spriteManager)
        {
            // TODO set mTarget to the position itself so heroes spawn non moving
            // Kind of done... Hero starts moving when the first target command is set... :)
            // mTarget = Sprite.Position;
            ShouldMove = false;
            Cooldown = new CooldownComponent(coolDown, false);
            Cooldown.CooledDown += component => AbilityStatus = AbilityState.Ready;
        }

        #endregion

        #region Movement

        protected override void CalculateMovement(PositionProvider positionProvider, GameTime gameTime, InputManager inputManager)
        {
            UpdateTarget(positionProvider, gameTime, inputManager);

            if (!(mTarget?.ToVector2() is Vector2 targetVector))
            {
                return;
            }
            
            // set the start Position for the AStar (something like the current position should do great)
            var start = Sprite.Position;
            var startPoint = positionProvider.RequireTile(start).ToPoint();
            
            // set the target Position for the AStar (latest updated target should be saved in mTarget
            var target = positionProvider.RequireTile(targetVector).ToPoint();

            // calculate the path
            mAStar = positionProvider.MakePathFinding(this, startPoint, target);
            mPathVisualizer = positionProvider.Visualize(mAStar);
            var path = mAStar.Path;
            if (path == null || path.Count == 0) // there is no path to be found
            {
                target = FindNearestWalkableField(target);
                mAStar = positionProvider.MakePathFinding(this, startPoint, target);
                mPathVisualizer = positionProvider.Visualize(mAStar);
                path = mAStar.Path;
            }

            if (path.Count > 2)
            {
                MoveTarget = positionProvider.Grid.GetTile(new TileIndex(path[1], 1)).Position;
            }
            else
            {
                MoveTarget = positionProvider.Grid.GetTile(new TileIndex(target, 1)).Position;
                MoveTargetReached += MoveTargetReachedHandler;
            }
        }

        private void MoveTargetReachedHandler(Vector2 target)
        {
            mAStar = null;
            mTarget = null;
            mPathVisualizer = null;
            MoveTargetReached -= MoveTargetReachedHandler;
        }

        /// <summary>
        /// This method should check for a new target to walk to and saves it in mTarget
        /// also sets mShouldMove true
        /// </summary>
        /// <param name="positionProvider"></param>
        /// <param name="gameTime"></param>
        /// <param name="inputManager"></param>
        private void UpdateTarget(PositionProvider positionProvider, GameTime gameTime, InputManager inputManager)
        {
            if (StrategyStatus == Strategy.Attack)
            {
                AttackBase(inputManager, positionProvider, positionProvider.Target.Position);
                return;
            }
            // only check for new target of selected and Right Mouse Button was pressed
            if (!Selected) return;
            if (!inputManager.MousePressed(InputManager.MouseButton.Right)) return;

            var mouse = inputManager.TranslatedMousePosition;
            if (positionProvider.Grid.GridPointFromWorldPoint(mouse)?.Position == null) return;
            mTarget = new Point((int)mouse.X, (int)mouse.Y);
            ShouldMove = true;
            MoveTargetReached -= MoveTargetReachedHandler;
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
                    Cooldown.Update(gameTime);
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
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
            ShouldMove = false;
            Cooldown.Reset();
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

        internal override void AttackBase(InputManager inputManager, PositionProvider positionProvider, Point basePosition)
        {
            var startPoint = positionProvider.RequireTile(this).ToPoint();
            mTarget = positionProvider.RequireTile(basePosition.ToVector2()).ToPoint();
            mAStar = positionProvider.MakePathFinding(this, startPoint, basePosition);
            ShouldMove = true;
        }

        #endregion
        
        #region Update

        public override void Update(PositionProvider positionProvider, InputManager inputManager, GameTime gameTime)
        {
            if (inputManager.KeyPressed(Keys.Space))
            {
                StrategyStatus = StrategyStatus == Strategy.Attack ? Strategy.Human : Strategy.Attack;
            }
            // also updates the cooldown
            UpdateAbility(positionProvider, gameTime, inputManager);
            
            // Check if we still want to move to the same target, etc.
            // also sets mAStar to the current version.
            UpdateTarget(positionProvider, gameTime, inputManager);

            // base.Update checks for ShouldMove
            base.Update(positionProvider, inputManager, gameTime);
            
        }
        
        #endregion

        #region Draw

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            DrawAStarPath(spriteBatch, gameTime);
            base.Draw(spriteBatch, gameTime);
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
