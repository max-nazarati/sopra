using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Autofac.Core.Lifetime;
using KernelPanic.Input;
using KernelPanic.Sprites;
using KernelPanic.Table;
using Microsoft.Xna.Framework;

namespace KernelPanic.Entities
{
    internal abstract class Troupe : Unit
    {
        private Vector2 mLastMovement;
        protected Troupe(int price, int speed, int life, int attackStrength, Sprite sprite, SpriteManager spriteManager)
            : base(price, speed, life, attackStrength, sprite, spriteManager)
        {
            mLastMovement = new Vector2(0, 0);
        }

        protected override void CalculateMovement(PositionProvider positionProvider, GameTime gameTime, InputManager inputManager)
        {
            base.CalculateMovement(positionProvider, gameTime, inputManager);
            Vector2 movementDirection = positionProvider.GetVector(Grid.CoordinatePositionFromScreen(Sprite.Position));
            if (movementDirection.X is float.NaN || movementDirection.Y is float.NaN)
            {
                movementDirection = mLastMovement;
            }
            else
            {
                mLastMovement = movementDirection;
            }
            MoveTarget = Sprite.Position + movementDirection;

            List<Point> baseHitBox = new List<Point>();
            foreach (var baseGrid in positionProvider.Target.GetHitBox())
            {
                baseHitBox.Add(baseGrid.ToPoint());
            }

            if (baseHitBox.Contains(Grid.CoordinatePositionFromScreen(Sprite.Position)))
            {
                DidDie();
            }
        }
    }
}
