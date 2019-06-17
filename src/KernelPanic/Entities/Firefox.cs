using System;
using System.Collections.Generic;
using KernelPanic.Sprites;
using Microsoft.Xna.Framework;
using System.Runtime.Serialization;
using KernelPanic.Input;
using KernelPanic.Table;

namespace KernelPanic.Entities
{
    [DataContract]
    internal sealed class Firefox : Hero
    {
        private Stack<Vector2> mAbility = new Stack<Vector2>();
        
        public Firefox(int price, int speed, int life, int attackStrength, Sprite sprite) : base(price, speed, life, attackStrength, sprite)
        {
            Cooldown = new CooldownComponent(new TimeSpan(0, 0, 5));
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
        

        protected override void ActivateAbility(InputManager inputManager)
        {
            AbilityActive = true;
            mShouldMove = false;
            Cooldown.Reset();
            
            // calculate the jump direction
            var mouse = inputManager.TranslatedMousePosition;
            var direction = mouse - Sprite.Position;
            direction.Normalize();
            direction *= 30;
            
            for (var _ = 0; _ < 10; _++)
            {
                mAbility.Push(direction);
            }
        }
        
        protected override void ContinueAbility(GameTime gameTime)
        {
            Console.WriteLine(this + " JUST USED HIS ABILITY! (method of class Firefox)  [TIME:] " + gameTime.TotalGameTime);
            if (mAbility.Count == 0)
            {
                AbilityActive = false;
                mShouldMove = true;
                return;
            }

            var jumpDistance = mAbility.Pop();
            Sprite.Position += jumpDistance;
        }
    }
}