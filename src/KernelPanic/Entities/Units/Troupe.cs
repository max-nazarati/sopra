using KernelPanic.Input;
using KernelPanic.Sprites;
using KernelPanic.Waves;
using Microsoft.Xna.Framework;

namespace KernelPanic.Entities.Units
{
    internal abstract class Troupe : Unit
    {
        protected Troupe(int price, int speed, int life, int attackStrength, Point hitBoxSize, Sprite sprite, SpriteManager spriteManager)
            : base(price, speed, life, attackStrength, hitBoxSize, sprite, spriteManager)
        {
            Speed = speed;
        }

        internal WaveReference Wave { get; set; }
        internal abstract bool IsSmall { get; }

        internal override void UpdateInformation()
        {
            base.UpdateInformation();
            mInfoText.Text += $"\nStärke: {AttackStrength}";
        }

        protected override void CalculateMovement(Vector2? projectionStart, PositionProvider positionProvider, InputManager inputManager)
        {
            var move = GetNextMoveVector(positionProvider);
            MoveVector = move;
        }

        protected override void DoMove(PositionProvider positionProvider,
            InputManager inputManager,
            GameTime gameTime)
        {
            CalculateMovement(null, positionProvider, inputManager);
            if (MoveVector is Vector2 theMove)
            {
                theMove.Normalize();
                var modifiedSpeed = mSlowedDown ? Speed / 2f : Speed;
                mSlowedDown = false;
                var distance = modifiedSpeed * gameTime.ElapsedGameTime.Milliseconds * 0.06f;
                Sprite.Position += theMove * distance;
            }
        }

        internal new Troupe Clone() => Clone<Troupe>();
    }
}
