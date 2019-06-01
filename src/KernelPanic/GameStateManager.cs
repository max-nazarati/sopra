using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

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
        public Game1 Game { get; }
        private readonly Stack<AGameState> mGameStates = new Stack<AGameState>();

        public GameStateManager(Game1 game, ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            Sprite = new SpriteManager(contentManager, graphicsDevice);
            Game = game;
        }
        
        internal AGameState Active => mGameStates.Peek();

        public void Pop()
        {
            if (mGameStates.Count() > 0)
            {
                mGameStates.Pop();
            }
        }
        public void Push(AGameState newGameState)
        {
            mGameStates.Push(newGameState);
        }
        public void Update(GameTime gameTime, bool isOverlay)
        {

            mGameStates.Peek().Update(gameTime, mGameStates.Peek().IsOverlay);

        }
        public bool Empty()
        {
            return mGameStates.Count() < 1;
        }
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (!Empty())
            {
                AGameState x = mGameStates.Peek();
                x.Draw(spriteBatch, gameTime, mGameStates.Peek().IsOverlay);
            }

        }
    }
}
