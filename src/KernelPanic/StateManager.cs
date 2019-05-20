using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    internal sealed class StateManager
    {
        private Stack<State> _stateStack;
        public Game1 Game { get; }

        public StateManager(Game1 game, GraphicsDeviceManager gameGraphics, ContentManager gameContent)
        {
            Game = game;
            var graphics = gameGraphics;
            var content = gameContent;
            _stateStack = new Stack<State>();
            _stateStack.Push(new StartMenuState(this, graphics, content));
        }

        public void AddState(State newState)
        {
            _stateStack.Push(newState);
        }

        public void RemoveState()
        {
            _stateStack.Pop();
        }

        public void Update()
        {
            _stateStack.Peek().Update();
        }

        public int Count()
        {
            return _stateStack.Count();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _stateStack.Peek().Draw(spriteBatch);
        }

        public State CheckState()
        {
            return _stateStack.Peek();
        }
    }
}
