using System;
using System.Collections.Generic;
using KernelPanic.Sprites;
using Microsoft.Xna.Framework;
using System.Runtime.Serialization;

namespace KernelPanic.Entities
{
    [DataContract]
    internal sealed class Firefox : Hero
    {
        private Stack<Vector2> mAbility = new Stack<Vector2>();
        
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
        
        protected override void UpdateAbility(PositionProvider positionProvider, GameTime gameTime, InputManager inputManager)
        {
            base.UpdateAbility(positionProvider, gameTime, inputManager);
        }

        protected override void ActivateAbility()
        {
            AbilityActive = true;
            ShouldMove = false;
            for (var _ = 0; _ < 5; _++)
            {
                mAbility.Push(new Vector2(100, 0));
            }
        }
        
        protected override void ContinueAbility(GameTime gameTime)
        {
            Console.WriteLine(this + " JUST USED HIS ABILITY! (method of class Firefox)  [TIME:] " + gameTime.TotalGameTime);
            if (mAbility.Count == 0)
            {
                AbilityActive = false;
                ShouldMove = true;
                return;
            }

            var jumpDistance = mAbility.Pop();
            Sprite.Position += jumpDistance;
        }
    }
}