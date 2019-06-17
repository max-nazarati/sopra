using System;
using System.Runtime.Serialization;
using KernelPanic.Input;
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
        protected CooldownComponent Cooldown { get; set; }
        
        
        private AStar mAStar; // save the AStar for path-drawing
        private Point mTarget; // the target we wish to move to

        private Visualizer mPathVisualizer;
        // protected InputManager mInputManager;

        /// <summary>
        /// Convenience function for creating a Hero. The sprite is automatically scaled to the size of one tile.
        /// </summary>
        /// <param name="position">The point where to position this troupe.</param>
        /// <param name="sprite">The sprite to display.</param>
        /// <returns>A new Troupe</returns>
        private static Hero Create(Point position, Sprite sprite)
        {
            sprite.Position = position.ToVector2();
            sprite.ScaleToWidth(Grid.KachelSize);
            return new Hero(10, 1, 1, 1, sprite);
        }

        public Hero(int price, int speed, int life, int attackStrength, Sprite sprite) : base(price, speed, life, attackStrength, sprite)
        {
            // TODO set mTarget to the position itself so heroes spawn non moving
            // Kind of done... Hero starts moving when the first target command is set... :)
            // mTarget = Sprite.Position;
            mShouldMove = false;
        }
        
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
                mShouldMove = false;
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
            mShouldMove = true;
        }
        
        #endregion Movement

        #region Ability
        protected virtual bool AbilityAvailable()
        {
            return Cooldown.Enabled;
        }

        protected virtual void ActivateAbility(InputManager inputManager)
        {
            AbilityActive = true;
            mShouldMove = false;
            Cooldown.Reset();
        }

        protected bool AbilityActive { get; set; }
        
        protected virtual void UpdateAbility(PositionProvider positionProvider, GameTime gameTime, InputManager inputManager)
        {
            // Console.WriteLine(this + " JUST UPDATED HIS ABILITY!");
            if (AbilityActive)
            {
                ContinueAbility(gameTime);
            }
            else
            {
                if (CheckAbilityStart(inputManager))
                {
                    ActivateAbility(inputManager);
                    ContinueAbility(gameTime);
                }
            }
        }

        protected virtual bool CheckAbilityStart(InputManager inputManager)
        {
            // return false if not ready, selected or not activated by pressing 'q': 
            return Selected && AbilityAvailable() && inputManager.KeyPressed(Keys.Q);
        }
        
        protected virtual void ContinueAbility(GameTime gameTime)
        {
            Console.WriteLine(this + " JUST USED HIS ABILITY! (virtual method of class Hero)  [TIME:] " + gameTime.TotalGameTime);
            AbilityActive = false;
            // ShouldMove = true;
        }
        
        protected virtual void DrawAbility(SpriteBatch spriteBatch, GameTime gameTime)
        {
            
        }
        
        #endregion Ability

        internal override void Update(PositionProvider positionProvider, GameTime gameTime, InputManager inputManager)
        {
            mInputManager = inputManager;
            // Check if we still want to move to the same target, etc.
            // also sets mAStar to the current version.
            UpdateTarget(positionProvider, gameTime, inputManager);
            
            Cooldown.Update(gameTime);
            UpdateAbility(positionProvider, gameTime, inputManager);

            // base.Update checks for mShouldMove
            base.Update(positionProvider, gameTime, inputManager);
            
        }
        
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
    }
}
