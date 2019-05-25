using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
// LD
namespace KernelPanic
{
    public class StateManager
    {
        private readonly Stack<GState> mStateStack = new Stack<GState>();
        internal Game1 Game { get; }

        internal StateManager(Game1 game, GraphicsDeviceManager gameGraphics, ContentManager gameContent)
        {
            Game = game;
            var graphics = gameGraphics;
            var content = gameContent;
            mStateStack.Push(new StartMenuState(this, graphics, content));
        }

        internal void AddState(GState newState)
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
        internal GState CheckState()
        {
            return mStateStack.Peek();
        }
    }
}
