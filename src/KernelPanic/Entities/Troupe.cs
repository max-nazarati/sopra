using Microsoft.Xna.Framework;

namespace KernelPanic
{
    internal class Troupe : Unit
    {
        private Troupe(int price, int speed, int life, int attackStrength, Sprite sprite) : base(price, speed, life, attackStrength, sprite)
        {
        }

        internal static Troupe Create(Point position, Color color, SpriteManager spriteManager)
        {
            var sprite = spriteManager.CreateColoredSquare(color).Sprite;
            sprite.DestinationRectangle = new Rectangle(position, new Point(Grid.KachelSize));
            return new Troupe(1, 1, 1, 1, sprite);
        }

        internal override void Update(GameTime gameTime, Matrix invertedViewMatrix)
        {
            // throw new System.NotImplementedException();
        }
    }
}
