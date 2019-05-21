using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    internal sealed class StateManager
    {
        private readonly Stack<State> mStateStack = new Stack<State>();
        public Game1 Game { get; }

        public StateManager(Game1 game, GraphicsDeviceManager gameGraphics, ContentManager gameContent)
        {
            Game = game;
            var graphics = gameGraphics;
            var content = gameContent;
            mStateStack.Push(new StartMenuState(this, graphics, content));
        }

        public void AddState(State newState)
        {
            mStateStack.Push(newState);
        }

        public void RemoveState()
        {
            mStateStack.Pop();
        }

        public void Update()
        {
            mStateStack.Peek().Update();
        }

        public int Count()
        {
            return mStateStack.Count;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            mStateStack.Peek().Draw(spriteBatch);
        }

        public State CheckState()
        {
            return mStateStack.Peek();
        }
    }
}
