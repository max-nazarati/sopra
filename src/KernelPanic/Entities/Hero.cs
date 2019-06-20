#define DEBUG
// #undef DEBUG

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using KernelPanic.Data;
using KernelPanic.Input;
using KernelPanic.Interface;
using KernelPanic.PathPlanning;
using KernelPanic.Sprites;
using KernelPanic.Table;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace KernelPanic.Entities
{
    [DataContract]
    [KnownType(typeof(Firefox))]
    internal class Hero : Unit
    {
        #region MemberVariables
        
        protected enum AbilityState
        {
            Ready = 0, // default value since it has the value '0'
            Indicating, Starting, Active, Finished, CoolingDown
        }

        [DataMember]
        protected CooldownComponent Cooldown { get; set; }
        private AStar mAStar; // save the AStar for path-drawing
        private Point mTarget; // the target we wish to move to
        private Visualizer mPathVisualizer;
        protected AbilityState AbilityStatus;
        
        #endregion

        #region Konstruktor / Create

        /// <summary>
        /// Convenience function for creating a Hero. The sprite is automatically scaled to the size of one tile.
        /// </summary>
        /// <param name="position">The point where to position this troupe.</param>
        /// <param name="sprite">The sprite to display.</param>
        /// <param name="spriteManager"></param>
        /// <returns>A new Troupe</returns>
        private static Hero Create(Point position, Sprite sprite, SpriteManager spriteManager)
        {
            sprite.Position = position.ToVector2();
            sprite.ScaleToWidth(Grid.KachelSize);
            return new Hero(10, 1, 100, 1, sprite, spriteManager);
        }

        protected Hero(int price, int speed, int life, int attackStrength, Sprite sprite, SpriteManager spriteManager)
            : base(price, speed, life, attackStrength, sprite, spriteManager)
        {
            // TODO set mTarget to the position itself so heroes spawn non moving
            // Kind of done... Hero starts moving when the first target command is set... :)
            // mTarget = Sprite.Position;
            ShouldMove = false;
        }

        #endregion

        #region Movement
        protected override void CalculateMovement(PositionProvider positionProvider, GameTime gameTime, InputManager inputManager)
        {
            UpdateTarget(positionProvider, gameTime, inputManager);
            
            // set the start Position for the AStar (something like the current position should do great)
            var start = Sprite.Position;
            var startPoint = new Point((int)start.X, (int)start.Y) / new Point(100, 100);
            
            // set the target Position for the AStar (latest updated target should be saved in mTarget
            var target = mTarget / new Point(100, 100);

            // calculate the path
            mAStar = positionProvider.MakePathFinding(this, startPoint, target);
            mPathVisualizer = positionProvider.Visualize(mAStar);
            var path = mAStar.Path;

            if (path.Count == 0) // there is no path to be found
            {
                target = FindNearestWalkableField(target); // new Point(100, 100);
                // Console.WriteLine("This is the debug message you are looking for" + mTarget);
                mAStar = positionProvider.MakePathFinding(this, startPoint, target);
                mPathVisualizer = positionProvider.Visualize(mAStar);
                path = mAStar.Path;
            }
            
            // TODO get the next position of the path (should be at path[0]; something is ****ed up tho)...
            // ... setting it to 0 makes the firefox disappear (thus making me cry T_T) ...
            // ... firefox walks to neighboured field for now

            Vector2? movement = Sprite.Position * 100;
            if (path.Count > 1)
            {
                var x = path[1].X;
                var y = path[1].Y;
                // + (100, 100) translates the target to the point it should be
                movement = positionProvider.GridCoordinate(new Vector2(x * 100, y * 100) + new Vector2(100, 100));
            }
            else // stop moving if target is reached;
            {
                ShouldMove = false;
            }
            
            // MoveTarget will be used by the Update Function (of the base class 'unit') to move the object
            MoveTarget = movement;
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
            // only check for new target of selected and Right Mouse Button was pressed
            if (!Selected) return;
            if (!inputManager.MousePressed(InputManager.MouseButton.Right)) return;

            var mouse = inputManager.TranslatedMousePosition;
            if (positionProvider.GridCoordinate(mouse) == null) return;
            mTarget = new Point((int)mouse.X, (int)mouse.Y);
            ShouldMove = true;
        }
        
        protected Point FindNearestWalkableField(Point target)
        {
            var result = mAStar.FindNearestField();
            return result ?? new Point((int)Sprite.Position.X, (int)Sprite.Position.Y);
        }
        
        #endregion Movement

        #region Ability
        
        
        /*
        protected virtual bool AbilityAvailable()
        {
            return Cooldown.Enabled;
        }
        */
        
        
        // protected bool AbilityActive { /*protected*/ private get; set; }

        protected virtual void ActivateAbility(InputManager inputManager)
        {
            AbilityStatus = AbilityState.Active; 
            ShouldMove = false;
            Cooldown.Reset();
        }

        protected virtual void IndicateAbility(InputManager inputManager)
        {
            // just quit indicating when not selected anymore
            if (!Selected)
            {
                AbilityStatus = AbilityState.Ready;
                return;
            }
            
            if (inputManager.KeyPressed(Keys.Q, Keys.E))
            {
                if (inputManager.KeyPressed(Keys.Q))
                {
                    AbilityStatus = AbilityState.Starting;
                }

                else if (inputManager.KeyPressed(Keys.E))
                {
                    AbilityStatus = AbilityState.Ready;
                }
            }
            else
            {
                AbilityStatus = AbilityState.Indicating;
            }
        }

        protected virtual void InitializeAbility()
        {
            #region DEBUG
#if DEBUG
            Console.WriteLine("Ability is now initialized.");
#endif
            #endregion
        }
        
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
                    Console.WriteLine("NotActive");
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
#endif
            #endregion
            
            SwitchAbilityState:
            switch (AbilityStatus)
            {
                case AbilityState.Ready:
                    // Here we should check if we should start to indicate the ability

                    if (CheckAbilityStart(inputManager))
                    {
                        AbilityStatus = AbilityState.Indicating;
                    }
                    break;
                
                case AbilityState.Indicating:
                    // sets the next AbilityState if wanted.
                    IndicateAbility(inputManager);
                    break;

                case AbilityState.Starting:
                    // initialize the ability
                    InitializeAbility();
                    break;
                
                case AbilityState.Active:
                    // take one action per update cycle until the ability is finished
                    ContinueAbility(gameTime);
                    break;

                case AbilityState.Finished:
                    // finally cleaning up has to be done and starting to cool down
                    ShouldMove = true;
                    AbilityStatus = AbilityState.CoolingDown;
                    break;

                case AbilityState.CoolingDown:
                    Cooldown.Update(gameTime);
                    if (true)// if (Cooldown.Enabled)
                    {
                        AbilityStatus = AbilityState.Ready;
                    }
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        
        protected virtual bool CheckAbilityStart(InputManager inputManager)
        {
             
            return Selected  && inputManager.KeyPressed(Keys.Q);
        }
        
        protected virtual void ContinueAbility(GameTime gameTime)
        {
            Console.WriteLine(this + " JUST USED HIS ABILITY! (virtual method of class Hero)  [TIME:] " + gameTime.TotalGameTime);
            AbilityStatus = AbilityState.Finished;
        }
        
        protected virtual void DrawAbility(SpriteBatch spriteBatch, GameTime gameTime)
        {
            
        }
        
        #endregion Ability

        #region Update

        internal override void Update(PositionProvider positionProvider, GameTime gameTime, InputManager inputManager)
        {
            // Check if we still want to move to the same target, etc.
            // also sets mAStar to the current version.
            UpdateTarget(positionProvider, gameTime, inputManager);
            
            // also updates the cooldown
            UpdateAbility(positionProvider, gameTime, inputManager);

            // base.Update checks for ShouldMove
            base.Update(positionProvider, gameTime, inputManager);
        }
        
        #endregion

        #region Draw

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            DrawAStarPath(spriteBatch, gameTime);
            base.Draw(spriteBatch, gameTime);
            DrawAbility(spriteBatch, gameTime);
        }

        private void DrawAStarPath(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (Selected)
            {
                mPathVisualizer?.Draw(spriteBatch, gameTime);
            }
        }

        #endregion

        #region Actions

        protected override IEnumerable<IAction> Actions =>
            base.Actions.Extend(new AbilityAction(this, SpriteManager));

        private sealed class AbilityAction : BaseAction<TextButton>
        {
            internal AbilityAction(Hero hero, SpriteManager sprites) : base(new TextButton(sprites) {Title = "Fähigkeit"})
            {
                Provider.Clicked += (button, inputManager) => hero.IndicateAbility(inputManager);
            }

            public override void MoveTo(Vector2 position)
            {
                Provider.Sprite.Position = position;
            }
        }

        #endregion
    }
}
