using KernelPanic.Input;
using KernelPanic.Sprites;
using KernelPanic.Waves;
using Microsoft.Xna.Framework;

namespace KernelPanic.Entities.Units
{
    internal abstract class Troupe : Unit
    {
        protected Troupe(int price, int speed, int life, int attackStrength, Sprite sprite, SpriteManager spriteManager)
            : base(price, speed, life, attackStrength, sprite, spriteManager)
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

        protected override void CalculateMovement(PositionProvider positionProvider, GameTime gameTime, InputManager inputManager)
        {
            if (MoveTarget == null)
                MoveTarget = Sprite.Position + positionProvider.TroupeData.RelativeMovement(this);
        }

        internal new Troupe Clone() => Clone<Troupe>();
    }
}
