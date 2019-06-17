using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    [DataContract]
    internal sealed class GameStateManager
    {
        /*
        public AGameState ActiveState { get; }
        public InputManager Input { get; }
        //public Settings Settings { get; }
        public SoundManager Sounds { get; }
        public void Switch(AGameState newGameState)
        {
            mGameStates.Pop();
            mGameStates.Push(newGameState);
        }
        */

        /// <summary>
        /// Stores a <see cref="AGameState"/> together with the current quad-tree of click targets.
        /// </summary>
        private sealed class GameStateInfo
        {
            internal AGameState State { get; }
            internal Quadtree<ClickTarget> ClickTargets { get; set; }

            internal GameStateInfo(AGameState state)
            {
                State = state;
                ClickTargets = Quadtree<ClickTarget>.Empty;
            }
        }

        public SpriteManager Sprite { get; }
        public SoundManager Sound { get; }
        private readonly Stack<GameStateInfo> mGameStates = new Stack<GameStateInfo>();

        internal Action ExitAction { get; }

        public GameStateManager(Action exitAction, SpriteManager sprites, SoundManager sounds)
        {
            Sprite = sprites;
            Sound = sounds;
            ExitAction = exitAction;
        }

        public void Pop()
        {
            if (mGameStates.Count > 0)
            {
                mGameStates.Pop();
            }
        }

        public void Push(AGameState newGameState)
        {
            mGameStates.Push(new GameStateInfo(newGameState));
        }

        /// <summary>
        /// Clears the stack and pushes the given <see cref="AGameState"/>.
        /// </summary>
        /// <param name="newGameState">The new state.</param>
        internal void Restart(AGameState newGameState)
        {
            mGameStates.Clear();
            Push(newGameState);
        }

        public void Update(RawInputState rawInput, GameTime gameTime)
        {
            foreach (var info in ActiveStates())
            {
                var state = info.State;
                var newClickTargets = new List<ClickTarget>();
                var input = new InputManager(newClickTargets, state.Camera, rawInput);

                if (!rawInput.IsClaimed(InputManager.MouseButton.Left))
                {
                    var possibleTargets = info.ClickTargets.EntitiesAt(input.TranslatedMousePosition);
                    var maybeTarget = possibleTargets.FirstOrDefault();
                    if (maybeTarget != null && input.MousePressed(InputManager.MouseButton.Left))
                        maybeTarget.Action();
                }

                // Do the actual update.
                state.Update(input, gameTime);

                // The call to Update filled newClickTargets via the reference in the InputManager.
                info.ClickTargets = Quadtree<ClickTarget>.Create(newClickTargets);
            }
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
