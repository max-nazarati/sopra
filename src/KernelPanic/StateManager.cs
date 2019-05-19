using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    internal sealed class StateManager
    {
        private Stack<State> _stateStack = new Stack<State>();
        private  GraphicsDeviceManager _graphics;
        private  ContentManager _content;
        public Game1 _game { get; set; }

        public StateManager(Game1 game, GraphicsDeviceManager graphics, ContentManager content)
        {
            this._game = game;
            this._graphics = graphics;
            this._content = content;
            this._stateStack = new Stack<State>();
            _stateStack.Push(new StartMenuState(this, _graphics, _content));
        }

        public void AddState(State newState)
        {
            _stateStack.Push(newState);
        }

        public void RemoveState()
        {
            _stateStack.Pop();
        }

        public void Update(GameTime gameTime)
        {
            _stateStack.Peek().Update(gameTime);
        }

        public int Count()
        {
            return _stateStack.Count();
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _stateStack.Peek().Draw(gameTime, spriteBatch);
        }
    }
}
