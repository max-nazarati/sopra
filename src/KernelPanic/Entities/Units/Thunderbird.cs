using System.Diagnostics.CodeAnalysis;
using KernelPanic.Input;
using Microsoft.Xna.Framework;

namespace KernelPanic.Entities.Units
{
    // This is instantiated via black magic originating from Unit.Create.
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    internal sealed class Thunderbird : Troupe
    {
        private Rectangle mHitBox;

        public override Rectangle Bounds
        {
            get
            {
                mHitBox.X = Sprite.Bounds.X + 5;
                mHitBox.Y = Sprite.Bounds.Y + 2;
                return mHitBox;
            }
        }

        internal Thunderbird(SpriteManager spriteManager)
            : base(15, 2, 15, 3, spriteManager.CreateThunderbird(), spriteManager)
        {
            mHitBox = Sprite.Bounds;
            mHitBox.Width = 53;
            mHitBox.Height = 55;
        }
        
        protected override void CalculateMovement(PositionProvider positionProvider, GameTime gameTime, InputManager inputManager)
        {
            var currentTile = positionProvider.RequireTile(this);
            var movement = positionProvider.RelativeMovementThunderbird(currentTile.ToPoint());
            MoveTarget = Sprite.Position + movement;
        }
    }
}