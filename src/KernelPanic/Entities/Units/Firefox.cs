using System;
using System.Collections.Generic;
using KernelPanic.Sprites;
using Microsoft.Xna.Framework;
using System.Runtime.Serialization;
using KernelPanic.Data;
using KernelPanic.Input;
using KernelPanic.Table;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic.Entities.Units
{
    [DataContract]
    internal sealed class Firefox : Hero
    {
        private Stack<Vector2> mAbility = new Stack<Vector2>();
        private Vector2 mAbilityTarget;
        private Vector2? mJumpTarget;
        private readonly ImageSprite mIndicator;
        private const int JumpDuration = 10;
        private const int JumpSegmentLength = 30;

        internal Firefox(SpriteManager spriteManager)
            : base(50, 6, 30, 10, TimeSpan.FromSeconds(5), spriteManager.CreateFirefox(), spriteManager)
        {
            mIndicator = spriteManager.CreateJumpIndicator();
        }

        protected override void CompleteClone()
        {
            base.CompleteClone();
            mAbility = new Stack<Vector2>(mAbility);
            Cooldown = new CooldownComponent(TimeSpan.FromSeconds(5), false);
            Cooldown.CooledDown += component => AbilityStatus = AbilityState.Ready;
        }

        internal override void UpdateInformation()
        {
            base.UpdateInformation();
            InfoText.Text += $"\nStärke: {AttackStrength}";
        }

        #region Ability 

        protected override void IndicateAbility(PositionProvider positionProvider, InputManager inputManager)
        {
            mAbilityTarget = inputManager.TranslatedMousePosition;
            base.IndicateAbility(positionProvider, inputManager);
            
        }

        protected override void StartAbility(PositionProvider positionProvider, InputManager inputManager)
        {
            // debug
            base.StartAbility(positionProvider, inputManager);

            // calculate the jump direction
            var mouse = mJumpTarget ?? inputManager.TranslatedMousePosition;
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
            while (!positionProvider.Grid.Contains(goal) || positionProvider.HasEntityAt(goal))
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

        #region KI

        internal override void AttackBase(InputManager inputManager, PositionProvider positionProvider, Point basePosition)
        {
            // moving
            base.AttackBase(inputManager, positionProvider, basePosition);
            // jump if possible
            if (MoveTarget != null && Cooldown.Ready)
            {
                // just jump the next steps
                if (AStar.Path is List<Point> path && path.Count >= 2)
                {
                    mJumpTarget = positionProvider.Grid.GetTile(new TileIndex(path[2], 1)).Position;
                    TryActivateAbility(inputManager, true);
                    StartAbility(positionProvider, inputManager);
                }
            }
        }

        #endregion
        
        #region Draw

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
            var direction = mAbilityTarget - Sprite.Position;
            direction.Normalize();
            var rotation = direction.Angle(0.5);

            mIndicator.Position = Sprite.Position;
            mIndicator.Rotation = rotation;
            mIndicator.ScaleToHeight(JumpDuration * JumpSegmentLength);
            mIndicator.Draw(spriteBatch, gameTime);
            
        }
        
        #endregion
    }
}