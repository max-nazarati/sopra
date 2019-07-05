using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Autofac.Core.Lifetime;
using KernelPanic.Input;
using KernelPanic.Sprites;
using KernelPanic.Table;
using KernelPanic.Waves;
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
            Speed = speed;
        }

        internal WaveReference Wave { get; set; }

        protected override void CalculateMovement(PositionProvider positionProvider, GameTime gameTime, InputManager inputManager)
        {
            var currentTile = positionProvider.RequireTile(this);
            var movementDirection = positionProvider.MovementVector(currentTile.ToPoint());
            if (movementDirection.X is float.NaN || movementDirection.Y is float.NaN)
            {
                movementDirection = mLastMovement;
            }
            else
            {
                mLastMovement = movementDirection;
            }
            MoveTarget = Sprite.Position + Speed * movementDirection;
        }
        
        internal new Troupe Clone() => Clone<Troupe>();
    }
}
