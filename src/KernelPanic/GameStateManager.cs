using System;
using System.Collections.Generic;
using System.Linq;
using Autofac.Util;
using KernelPanic.Data;
using KernelPanic.Input;
using KernelPanic.Tracking;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    internal sealed class GameStateManager : Disposable
    {
        /// <summary>
        /// Stores a <see cref="AGameState"/> together with the current quad-tree of click targets.
        /// </summary>
        private sealed class GameStateInfo
        {
            internal AGameState State { get; }
            internal QuadTree<ClickTarget> ClickTargets { get; set; }

            internal GameStateInfo(AGameState state)
            {
                State = state;
                ClickTargets = QuadTree<ClickTarget>.Empty;
            }
        }

        public SpriteManager Sprite { get; }
        internal GraphicsDeviceManager GraphicsDeviceManager { get; }

        public SoundManager Sound { get; }
        private readonly Stack<GameStateInfo> mGameStates = new Stack<GameStateInfo>();

        internal Action ExitAction { get; }

        internal Statistics Statistics { get; } = new Statistics();

        internal AchievementPool AchievementPool { get; } = AchievementPool.LoadGlobal();

        public GameStateManager(Action exitAction, SpriteManager sprites, SoundManager sounds, GraphicsDeviceManager graphics)
        {
            Sprite = sprites;
            Sound = sounds;
            GraphicsDeviceManager = graphics;
            ExitAction = exitAction;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Statistics.Dispose();
                
                // TODO: Enable when we can load them.
                // AchievementPool.Dispose();
            }

            base.Dispose(disposing);
        }

        private void Clear()
        {
            foreach (var info in mGameStates)
            {
                info.State.Dispose();
            }
            
            mGameStates.Clear();
        }

        public void Pop()
        {
            if (mGameStates.Count <= 0)
                return;

            mGameStates.Pop().State.Dispose();
        }

        public void Push(AGameState newGameState)
        {
            mGameStates.Push(new GameStateInfo(newGameState));
        }

        internal void Switch(AGameState newGameState)
        {
            Pop();
            Push(newGameState);
        }

        /// <summary>
        /// Clears the stack and pushes the given <see cref="AGameState"/>.
        /// </summary>
        /// <param name="newGameState">The new state.</param>
        internal void Restart(AGameState newGameState)
        {
            Clear();
            Push(newGameState);
        }

        public void Update(RawInputState rawInput, GameTime gameTime, SoundManager soundManager)
        {
            foreach (var info in ActiveStates())
            {
                var state = info.State;
                var newClickTargets = new List<ClickTarget>();
                var input = new InputManager(newClickTargets, state.Camera, rawInput);

                if (InvokeClickTargets(input, info.ClickTargets) is object requiredClaim)
                    rawInput.Claim(requiredClaim);

                // Do the actual update.
                state.Update(input, gameTime, soundManager);

                // The call to Update filled newClickTargets via the reference in the InputManager.
                info.ClickTargets = QuadTree<ClickTarget>.Create(newClickTargets);
            }
        }

        private static object InvokeClickTargets(InputManager inputManager, QuadTree<ClickTarget> targets)
        {
            if (inputManager.IsClaimed(InputManager.MouseButton.Left))
                return null;

            var maybeTarget = targets.EntitiesAt(inputManager.TranslatedMousePosition).FirstOrDefault();
            if (maybeTarget == null)
                return null;

            if (inputManager.MouseReleased(InputManager.MouseButton.Left))
                maybeTarget.Action(inputManager);
            return InputManager.MouseButton.Left;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (var state in ActiveStates().Select(i => i.State).Reverse())
            {
                spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: state.Camera.Transformation);
                state.Draw(spriteBatch, gameTime);
                spriteBatch.End();
            }
        }

        /// <summary>
        /// Enumerates from top to bottom through all currently visible states.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{AGameState}"/>.</returns>
        private IEnumerable<GameStateInfo> ActiveStates()
        {
            foreach (var info in mGameStates)
            {
                yield return info;
                if (!info.State.IsOverlay)
                    break;
            }
        }
    }
}
