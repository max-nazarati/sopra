
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    internal class Hero : Unit
    {
        // public CooldownComponent Cooldown { get; set; }
        private PositionProvider mPositionProvider;

        /// <summary>
        /// Convenience function for creating a Hero. The sprite is automatically scaled to the size of one tile.
        /// </summary>
        /// <param name="position">The point where to position this troupe.</param>
        /// <param name="sprite">The sprite to display.</param>
        /// <returns>A new Troupe</returns>
        private static Hero Create(Point position, Sprite sprite)
        {
            sprite.Position = position.ToVector2();
            sprite.ScaleToWidth(Grid.KachelSize);
            return new Hero(10, 1, 1, 1, sprite);
        }

        public Hero(int price, int speed, int life, int attackStrength, Sprite sprite) : base(price, speed, life, attackStrength, sprite)
        {
        }

        public bool AbilityAvailable()
        {
            return false;
        }

        public void ActivateAbility()
        {
            throw new NotImplementedException();
        }


        internal override void Update(PositionProvider positionProvider, GameTime gameTime, InputManager inputManager)
        {
            base.Update(positionProvider, gameTime, inputManager);
            var startX = Sprite.Position.X;
            var startY = Sprite.Position.Y;
            var start = new Point((int)startX, (int)startY);
            var path = positionProvider.MakePathFinding(start, new Point(5, 10));
            mPositionProvider = positionProvider;
        }
        
        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch, gameTime);
            mPositionProvider.DrawAStar(spriteBatch, gameTime);
        }
    }
}
