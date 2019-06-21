using System;
using System.Collections.Generic;
using KernelPanic.Sprites;
using Microsoft.Xna.Framework;
using System.Runtime.Serialization;
using KernelPanic.Input;
using KernelPanic.Table;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace KernelPanic.Entities
{
    [DataContract]
    internal sealed class Firefox : Hero
    {
        private Stack<Vector2> mAbility = new Stack<Vector2>();
        private Vector2 mAbilityTarget;
        private readonly ImageSprite mIndicator;

        private Firefox(int price, int speed, int life, int attackStrength, Sprite sprite, SpriteManager spriteManager)
            : base(price, speed, life, attackStrength, sprite, spriteManager)
        {
            Cooldown.Reset(new TimeSpan(0, 0, 5));
            mIndicator = spriteManager.CreateJumpIndicator();
        }

        internal Firefox(SpriteManager spriteManager) : this(0, 0, 0, 0, spriteManager.CreateFirefox(), spriteManager)
        {
            
        }

        private static Firefox Create(Point position, Sprite sprite, SpriteManager spriteManager)
        {
            sprite.Position = position.ToVector2();
            return new Firefox(10, 2, 100, 1, sprite, spriteManager);
        }

        internal static Firefox CreateFirefox(Point position, SpriteManager spriteManager) =>
            Create(position, spriteManager.CreateFirefox(), spriteManager);


        #region Ability 

        protected override void IndicateAbility(InputManager inputManager)
        {
            mAbilityTarget = inputManager.TranslatedMousePosition;
            base.IndicateAbility(inputManager);
            
        }

        protected override void StartAbility(PositionProvider positionProvider, InputManager inputManager)
        {
            // debug
            base.StartAbility(positionProvider, inputManager);

            var jumpDuration = 10;
            var jumpSegmentLength = 30;
            
            // calculate the jump direction
            var mouse = inputManager.TranslatedMousePosition;
            var direction = mouse - Sprite.Position;
            direction.Normalize();
            var jumpSegment = direction * jumpSegmentLength;


            for (var _ = 0; _ < jumpDuration; _++)
            {
                mAbility.Push(jumpSegment);
            }

            CorrectJump(direction, jumpDuration, positionProvider);
        }

        
        private void CorrectJump(Vector2 jumpSegment, int duration, PositionProvider positionProvider)
        {
            var jump = jumpSegment * duration;
            var goal = Sprite.Position + jump;
            if (positionProvider.Contains(goal)) // goal of jump is on the lane?
            {
                /*
                if (true) // goal of jump is a turret
                {
                    return;
                }
                */
            }

            // jump was too long
            var count = 0;
            while (!positionProvider.Contains(goal)) // this if is not working atm, bois aint stealing
            {
                
                count += 1;
                Console.WriteLine("Me and the bois stealing from  your stack.");
                goal -= jumpSegment;
            } 

            for (var _ = 0; _ <= count; _++)
            {
                // mAbility.Clear();
                mAbility.Pop();
            }
            Console.WriteLine("Stack size of jump: " + mAbility.Count +'\n');
            // jump was too short
            // goal += direction;

        }

        protected override void ContinueAbility(GameTime gameTime)
        {
            if (mAbility.Count == 0)
            {
                AbilityStatus = AbilityState.Finished;
                ShouldMove = true;
                Console.WriteLine(this + " JUST USED HIS ABILITY! (method of class Firefox)  [TIME:] " + gameTime.TotalGameTime);
                return;
            }

            var jumpDistance = mAbility.Pop();
            Sprite.Position += jumpDistance;
        }
        #endregion Ability
        
        #region Draw

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch, gameTime);
        }
        
        protected override void DrawAbility(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.DrawAbility(spriteBatch, gameTime);
            if (AbilityStatus == AbilityState.Indicating)
            {
                DrawIndicator(spriteBatch, gameTime);
            }
        }
        
        private void DrawIndicator(SpriteBatch spriteBatch, GameTime gameTime)
        {
            var direction = (mAbilityTarget - Sprite.Position);
            direction.Normalize();
            var rotation = -(float)(Math.Atan2(direction.X, direction.Y) + Math.PI);

            mIndicator.Position = Sprite.Position;
            mIndicator.Rotation = rotation;
            mIndicator.Draw(spriteBatch, gameTime);
            

        }
        
        #endregion
    }
}