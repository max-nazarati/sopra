using System.Diagnostics.CodeAnalysis;
using KernelPanic.Input;
using Microsoft.Xna.Framework;

namespace KernelPanic.Entities
{
    // This is instantiated via black magic originating from Unit.Create.
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    internal sealed class Thunderbird : Troupe
    {
        internal Thunderbird(SpriteManager spriteManager)
            : base(15, 3, 15, 3, spriteManager.CreateThunderbird(), spriteManager)
        {
        }
        
        protected override void CalculateMovement(PositionProvider positionProvider, GameTime gameTime, InputManager inputManager)
        {
            var currentTile = positionProvider.RequireTile(this);
            var movementDirection = positionProvider.MovementVectorThunderbird(currentTile.ToPoint());
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
    }
}