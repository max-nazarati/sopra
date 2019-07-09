using KernelPanic.Input;
using KernelPanic.Sprites;
using KernelPanic.Waves;
using Microsoft.Xna.Framework;

namespace KernelPanic.Entities.Units
{
    internal abstract class Troupe : Unit
    {
        protected Vector2 LastMovement;

        protected Troupe(int price, int speed, int life, int attackStrength, Sprite sprite, SpriteManager spriteManager)
            : base(price, speed, life, attackStrength, sprite, spriteManager)
        {
            LastMovement = new Vector2(0, 0);
            Speed = speed;
        }

        internal WaveReference Wave { get; set; }

        internal override void UpdateInformation()
        {
            base.UpdateInformation();
            InfoText.Text += $"\nStärke: {AttackStrength}";
        }

        protected override void CalculateMovement(PositionProvider positionProvider, GameTime gameTime, InputManager inputManager)
        {
            var currentTile = positionProvider.RequireTile(this);
            var movementDirection = positionProvider.MovementVector(currentTile.ToPoint());
            // var shift = positionProvider.MovememtShift(currentTile.ToPoint());
            if (movementDirection.X is float.NaN || movementDirection.Y is float.NaN)
            {
                movementDirection = LastMovement;
            }
            else
            {
                LastMovement = movementDirection;
            }

            MoveTarget = Sprite.Position + Speed * movementDirection;// + shift;
        }
        
        internal new Troupe Clone() => Clone<Troupe>();
    }
}
