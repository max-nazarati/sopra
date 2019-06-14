using KernelPanic.Sprites;
using Microsoft.Xna.Framework;
using System.Runtime.Serialization;

namespace KernelPanic.Entities
{
    [DataContract]
    internal sealed class Firefox : Hero
    {
        public Firefox(int price, int speed, int life, int attackStrength, Sprite sprite) : base(price, speed, life, attackStrength, sprite)
        {
        }

        private static Firefox Create(Point position, Sprite sprite)
        {
            sprite.Position = position.ToVector2();
            sprite.ScaleToWidth(Grid.KachelSize);
            return new Firefox(10, 2, 1, 1, sprite);
        }

        internal static Firefox CreateFirefox(Point position, SpriteManager spriteManager) =>
            Create(position, spriteManager.CreateFirefox());
        
        internal static Firefox CreateFirefoxJump(Point position, SpriteManager spriteManager) =>
            Create(position, spriteManager.CreateFirefoxJump());
        
        protected override void UpdateAbility()
        {
        }
    }
}