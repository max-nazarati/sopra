using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
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
        
        public SpriteManager Sprite { get; }
        private readonly Stack<AGameState> mGameStates = new Stack<AGameState>();

        internal Action ExitAction { get; }

        public GameStateManager(Action exitAction, SpriteManager sprites)
        {
            Sprite = sprites;
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
            mGameStates.Push(newGameState);
        }
        public void Update(RawInputState rawInput, GameTime gameTime)
        {
            foreach (var state in ActiveStates())
            {
                var input = new InputManager(state.Camera, rawInput);
                state.Update(input, gameTime);
            }
        }
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (var state in ActiveStates().Reverse())
            {
                spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: state.Camera.Transformation);
                state.Draw(spriteBatch, gameTime);
                spriteBatch.End();
            }
        }

        private IEnumerable<AGameState> ActiveStates()
        {
            foreach (var state in mGameStates)
            {
                yield return state;
                if (!state.IsOverlay)
                    break;
            }
        }
    }
}
