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

        protected override void CalculateMovement(Vector2? projectionStart,
            PositionProvider positionProvider,
            InputManager inputManager)
        {
            if (projectionStart == null && MoveTarget != null)
                return;

            var relativeMovement = positionProvider.TroupeData.RelativeMovement(this, projectionStart);
            MoveTarget = (projectionStart ?? Sprite.Position) + relativeMovement;
        }

        internal new Troupe Clone() => Clone<Troupe>();
    }
}
