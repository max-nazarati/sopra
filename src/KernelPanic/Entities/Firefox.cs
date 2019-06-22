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
        private const int JumpDuration = 10;
        private const int JumpSegmentLength = 30;

        internal Firefox(SpriteManager spriteManager)
            : base(50, 6, 30, 10, TimeSpan.FromSeconds(5), spriteManager.CreateFirefox(), spriteManager)
        {
            mIndicator = spriteManager.CreateJumpIndicator();
        }

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

            // calculate the jump direction
            var mouse = inputManager.TranslatedMousePosition;
            var direction = mouse - Sprite.Position;
            direction.Normalize();
            var jumpSegment = direction * JumpSegmentLength;

            for (var _ = 0; _ < JumpDuration; _++)
            {
                mAbility.Push(jumpSegment);
            }

            CorrectJump(direction, JumpDuration, positionProvider);
        }

        
        private void CorrectJump(Vector2 direction, int duration, PositionProvider positionProvider)
        {
            var jumpFrame = direction * JumpSegmentLength;
            var goal = Sprite.Position + jumpFrame * duration;
            
            // jump was too short
            while (positionProvider.HasEntityAt(goal))
            {
                mAbility.Push(jumpFrame);
                goal += jumpFrame;
            }
            
            // jump was too long
            while (!positionProvider.Contains(goal) || positionProvider.HasEntityAt(goal))
            {
                goal -= mAbility.Pop();
            }
        }

        protected override void ContinueAbility(GameTime gameTime)
        {
            if (mAbility.Count == 0)
            {
                AbilityStatus = AbilityState.Finished;
                ShouldMove = true;
                // Console.WriteLine(this + " JUST USED HIS ABILITY! (method of class Firefox)  [TIME:] " + gameTime.TotalGameTime);
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
            mIndicator.ScaleToHeight(JumpDuration * JumpSegmentLength);
            mIndicator.Draw(spriteBatch, gameTime);
            
        }
        
        #endregion
    }
}