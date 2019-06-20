using System.Runtime.Serialization;
using KernelPanic.Input;
using KernelPanic.Sprites;
using KernelPanic.Table;
using Microsoft.Xna.Framework;

namespace KernelPanic.Entities
{
    internal abstract class Troupe : Unit
    {
        protected Troupe(int price, int speed, int life, int attackStrength, Sprite sprite, SpriteManager spriteManager)
            : base(price, speed, life, attackStrength, sprite, spriteManager)
        {
        }

        protected override void CalculateMovement(PositionProvider positionProvider, GameTime gameTime, InputManager inputManager)
        {
            base.CalculateMovement(positionProvider, gameTime, inputManager);
            Vector2 movementDirection = positionProvider.GetVector(new Point((int)Sprite.Position.X, (int)Sprite.Position.Y));
            MoveTarget = Sprite.Position + movementDirection;
        }
    }
}
