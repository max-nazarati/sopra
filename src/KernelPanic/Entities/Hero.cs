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
            
            // var startVectorNullable = positionProvider.GridCoordinate(Sprite.Position);
            // var startVector = startVectorNullable ?? new Vector2(0, 0);
            // var startVector = new Vector2(2, 2);
            // var startX = startVector.X;
            // var startY = startVector.Y;
            // var start = new Point((int)startX, (int)startY);


            // TODO 1 set the start Position for the AStar (something like the current position should do great)
            // DONE!
            // var start = new Point(0, 0); // hardcoding/debugging bc smth doesnt work
            var start = Sprite.Position;
            var startPoint = new Point((int)start.X, (int)start.Y) / new Point(100, 100);
            
            
            // var startVectorNullable = positionProvider.GridCoordinate(Sprite.Position);
            // var startVector = startVectorNullable ?? new Vector2(0, 0);
            // var startVector = new Vector2(2, 2);
            // var startX = startVector.X;
            // var startY = startVector.Y;
            // var start = new Point((int)startX, (int)startY);
            
            
            
            // TODO 2 set the target Position for the Astar (latest updated target should be saved in mTarget
            // DONE!
            // var target = new Point(6, 9); // hardcoding/debugging bc smth doesnt work
            var target = mTarget / new Point(100, 100);
            

            // TODO 3 calculate the path
            // DONE!
            var path = positionProvider.MakePathFinding(startPoint, target);
            
            
            // TODO 4 get the next position of the path (should be at path[0]; indexing is ****ed up tho
            // The way it is right now, firefox walks to the field on the upper left of the actual target (directly)

            Vector2? movement = Sprite.Position * 100;
            if (path.Count > 0)
            {
                var x = path[path.Count-1].X;
                var y = path[path.Count-1].Y;
                movement = positionProvider.GridCoordinate(new Vector2(x * 100, y * 100));
            }
            MoveTarget = movement;
            

            // TODO 5 and finally save the found position at Movetarget
            // MoveTarget = new Vector2(x, y); // DEBUG
            
            
        }

        /// <summary>
        /// This method should check for a new target to walk to
        /// TODO save the target properly in mTarget
        /// </summary>
        /// <param name="positionProvider"></param>
        /// <param name="gameTime"></param>
        /// <param name="inputManager"></param>
        private void UpdateTarget(PositionProvider positionProvider, GameTime gameTime, InputManager inputManager)
        {
            // if target is null, set it to current position
            // mTarget = mTarget ??  new Point((int)Sprite.Position.X, (int)Sprite.Position.Y); // TODO Grid coordinate
            if (!Selected) return;
            if (!inputManager.MousePressed(InputManager.MouseButton.Right)) return;

            var mouse = inputManager.TranslatedMousePosition;
            if (positionProvider.GridCoordinate(mouse) == null) return;
            var targetX = positionProvider.GridCoordinate(mouse);
            // Console.WriteLine(targetX);
            mTarget = new Point((int)mouse.X, (int)mouse.Y);
            // Console.WriteLine(mTarget);
            // mTarget = new Point(500, 500); // debug
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
