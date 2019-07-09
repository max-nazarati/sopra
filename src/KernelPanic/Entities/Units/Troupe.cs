using KernelPanic.Input;
using KernelPanic.Sprites;
using KernelPanic.Waves;
using Microsoft.Xna.Framework;

namespace KernelPanic.Entities.Units
{
    internal abstract class Troupe : Unit
    {
        protected Vector2 mLastMovement;

        protected Troupe(int price, int speed, int life, int attackStrength, Sprite sprite, SpriteManager spriteManager)
            : base(price, speed, life, attackStrength, sprite, spriteManager)
        {
            mLastMovement = new Vector2(0, 0);
            Speed = speed;
        }

        internal WaveReference Wave { get; set; }

        internal override void UpdateInformation()
        {
            base.UpdateInformation();
            mInfoText.Text += $"\nStärke: {AttackStrength}";
        }

        protected override void CalculateMovement(PositionProvider positionProvider, GameTime gameTime, InputManager inputManager)
        {
            if (MoveTarget != null)
                return;

            var currentTile = positionProvider.RequireTile(this);
            var movement = positionProvider.RelativeMovement(currentTile.ToPoint());
            MoveTarget = Sprite.Position + movement;
        }
        
        internal new Troupe Clone() => Clone<Troupe>();
    }
}
