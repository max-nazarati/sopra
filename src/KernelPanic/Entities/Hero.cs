using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Diagnostics;
using KernelPanic.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic.Entities
{
    [DataContract]
    [KnownType(typeof(Firefox))]
    internal class Hero : Unit
    {
        // public CooldownComponent Cooldown { get; set; }
        
        // save the position provider so the AStar can be debugged in the draw method
        private PositionProvider mPositionProvider;
        
        private Point mTarget;

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
        }

        public bool AbilityAvailable()
        {
            return false;
        }

        public void ActivateAbility()
        {
            throw new NotImplementedException();
        }

        protected override void CalculateMovement(PositionProvider positionProvider, GameTime gameTime, InputManager inputManager)
        {
            UpdateTarget(positionProvider, gameTime, inputManager);
            
            // set the start Position for the AStar (something like the current position should do great)
            var start = Sprite.Position;
            var startPoint = new Point((int)start.X, (int)start.Y) / new Point(100, 100);
            
            // set the target Position for the Astar (latest updated target should be saved in mTarget
            var target = mTarget / new Point(100, 100);
            
            // calculate the path
            var path = positionProvider.MakePathFinding(startPoint, target);
            
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
            
            // MoveTarget will be used by the Update Function (of the base class 'unit') to move the object
            MoveTarget = movement;
        }

        /// <summary>
        /// This method should check for a new target to walk to and saves it in mTarget
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
        }
        
        internal override void Update(PositionProvider positionProvider, GameTime gameTime, InputManager inputManager)
        {
            // Check if we still want to move to the same target, etc.
            UpdateTarget(positionProvider, gameTime, inputManager);
            
            base.Update(positionProvider, gameTime, inputManager);
            mPositionProvider = positionProvider;
        }
        
        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch, gameTime);
            if (Selected)
            {
                mPositionProvider?.Draw(spriteBatch, gameTime);    
            }
            
        }
    }
}
