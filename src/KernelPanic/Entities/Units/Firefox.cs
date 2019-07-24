using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using KernelPanic.Sprites;
using Microsoft.Xna.Framework;
using System.Runtime.Serialization;
using KernelPanic.Data;
using KernelPanic.Entities.Buildings;
using KernelPanic.Events;
using KernelPanic.Input;
using KernelPanic.Table;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic.Entities.Units
{
    
    [DataContract]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    internal sealed class Firefox : Hero
    {
        private Stack<Vector2> mAbility = new Stack<Vector2>();
        private Vector2 mAbilityTarget;
        private Vector2? mJumpTarget;
        private readonly ImageSprite mIndicator;
        private const int JumpDuration = 10;
        private const int JumpSegmentLength = 30;
        private HashSet<Building> mJumpedBuildings;

        private static Point HitBoxSize => new Point(56, 29);

        internal Firefox(SpriteManager spriteManager)
            : base(50, 6, 30, 10, TimeSpan.FromSeconds(5), HitBoxSize, spriteManager.CreateFirefox(), spriteManager)
        {
            mIndicator = spriteManager.CreateJumpIndicator();
        }

        protected override void CompleteClone()
        {
            base.CompleteClone();
            mAbility = new Stack<Vector2>(mAbility);
            Cooldown = new CooldownComponent(Cooldown.Cooldown, false);
            Cooldown.CooledDown += component => AbilityStatus = AbilityState.Ready;
        }

        internal override void UpdateInformation()
        {
            base.UpdateInformation();
            mInfoText.Text += $"\nStärke: {AttackStrength}";
        }

        #region Ability 

        protected override void IndicateAbility(PositionProvider positionProvider, InputManager inputManager)
        {
            mAbilityTarget = inputManager.TranslatedMousePosition;
            base.IndicateAbility(positionProvider, inputManager);
            
        }

        protected override void StartAbility(PositionProvider positionProvider, InputManager inputManager)
        {
            // AbilityState, CoolDown, sound...
            base.StartAbility(positionProvider, inputManager);
            ShouldMove = false;

            // Achievement Data
            mJumpedBuildings = new HashSet<Building>();

            // calculate the jump direction
            var mouse = mJumpTarget ?? inputManager.TranslatedMousePosition;
            var direction = mouse - Sprite.Position;
            direction.Normalize();

            // we have _10_ Parts with a distance of _30_ each
            var jumpSegment = direction * JumpSegmentLength;
            for (var _ = 0; _ < JumpDuration; _++)
            {
                mAbility.Push(jumpSegment);
            }

            CorrectJump(direction, JumpDuration, positionProvider);
        }

        private void CorrectJump(Vector2 direction, int duration, PositionProvider positionProvider)
        {
            bool EntityIsBuilding(IGameObject gameObject) => gameObject is Building && !(gameObject is ShockField);

            var jumpFrame = direction * JumpSegmentLength;
            var goal = Sprite.Position + jumpFrame * duration;

            // jump was too short
            while (positionProvider.HasEntityAt(goal, EntityIsBuilding))
            {
                mAbility.Push(jumpFrame);
                goal += jumpFrame;
            }

            // jump was too long
            while (!positionProvider.Grid.Contains(goal) || positionProvider.HasEntityAt(goal, EntityIsBuilding))
            {
                if (mAbility.Count == 0)
                {
                    // Firefox might recognize itself as unity and tries to pop from empty stack
                    return;
                }
                goal -= mAbility.Pop();
            }

            // yet another error fixed :)
            var cancelJump = false;
            try
            {
                var _ = (positionProvider.RequireTile(goal).ToPoint());
            }
            catch (InvalidOperationException)
            {
                cancelJump = true;
            }
            if (cancelJump)
            {
                mAbility = new Stack<Vector2>();
            }
        }

        protected override void ContinueAbility(PositionProvider positionProvider, GameTime gameTime)
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

            #region Achievement
            bool EntityIsBuilding(IGameObject gameObject) => gameObject is Building;
            // TODO we are accessing entity graph twice here... makes me kinda sad
            if (positionProvider.HasEntityAt(Sprite.Position, EntityIsBuilding))
            {
                // TODO we might get more towers here than we want...
                var buildings = positionProvider.EntitiesAt<Building>(this);
                foreach (var building in buildings)
                {
                    mJumpedBuildings.Add(building);
                }
            }
            #endregion

            // set the correct animation
            if (!(Sprite is AnimatedSprite animated))
                return;
            if (jumpDistance.X is float x)
            {
                // choose correct movement direction based on x value or direction of idle animation
                animated.MovementDirection = (animated.Effect == SpriteEffects.None && (int)x == 0) || x < 0
                    ? AnimatedSprite.Direction.Left
                    : AnimatedSprite.Direction.Right;
            }
        }

        protected override void FinishAbility(PositionProvider positionProvider)
        {
            base.FinishAbility(positionProvider);
            // send one Event for every building we jumped
            for (var i = 0; i < mJumpedBuildings.Count; i++)
            {
                EventCenter.Default.Send(Event.FirefoxJumped(positionProvider.Owner, this));
            }
        }

        #endregion Ability

        #region KI

        protected override void AutonomousAttack(InputManager inputManager, PositionProvider positionProvider)
        {
            // moving
            base.AutonomousAttack(inputManager, positionProvider);
            // jumping
            SmartJump(inputManager, positionProvider);
        }

        private void SmartJump(InputManager inputManager, PositionProvider positionProvider)
        {
            // check invariants for jumping
            if (MoveTarget == null || !Cooldown.Ready) return;
            if (!(mAStar.Path is List<Point> path) || path.Count < 2) return;

            // iterate over the path and note the distances
            double[] distance = new double[path.Count];
            for (int i = 0; i < path.Count; i++)
            {
                var posX = Grid.KachelSize * path[i].X;
                var posY = Grid.KachelSize * path[i].Y;
                var diff = Sprite.Position - new Vector2(posX, posY);
                var dist = Math.Sqrt(Math.Pow(diff.X, 2) + Math.Pow(diff.Y, 2));
                distance[i] = dist;
            }

            var cSharpHasNoElseAfterAForLoopLikeWtfIsThisAnApesLanguage = false;
            // look for a good jump (aka a small distance but high number in the path)
            // -> lets just jump to the furthest target with small enough distance for a jump
            // for (int i = 6; i < distance.Length - 1; i++)
            for (int i = distance.Length - 1; i >= 6; i--)
            {
                // TODO find a good check if we should wait before jumping
                //      so we dont waste it.
                
                if (distance[i] < 300) // TODO this distance is hardcoded and therefore bad
                {
                    mJumpTarget = positionProvider.Grid.GetTile(new TileIndex(path[i], 1)).Position;
                    cSharpHasNoElseAfterAForLoopLikeWtfIsThisAnApesLanguage = true;
                    break;
                }
            }

            if (!cSharpHasNoElseAfterAForLoopLikeWtfIsThisAnApesLanguage) return;
            TryActivateAbility(inputManager, true);
            StartAbility(positionProvider, inputManager);
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