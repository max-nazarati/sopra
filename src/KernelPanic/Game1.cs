using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace KernelPanic
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>

    public class Game1 : Game
    {
        readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _mSpriteBatch;

        private State _currentState;
        private State _nextState;
        private StateManager stateManager;
        private readonly List<State> _gameStateList = new List<State>();
        public void SetGameState(State gameState)
        {
            _nextState = _gameStateList.Find(state => state.GetType().Name == gameState.GetType().Name);
        }
        

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.ApplyChanges();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            IsMouseVisible = true;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _mSpriteBatch = new SpriteBatch(GraphicsDevice);

          /*  _gameStateList.Add(new StartMenuState(this, _graphics, Content));
            _gameStateList.Add(new GameState(this, _graphics, Content));

            _currentState = _gameStateList.Find(state => state.GetType().Name == "StartMenuState");*/

            stateManager = new StateManager(this, _graphics, Content);
            stateManager.AddState(new StartMenuState(stateManager, _graphics, Content));
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (_nextState != null)
            {
                _currentState = _nextState;
                _nextState = null;
            }
            //_currentState.Update(gameTime);
            stateManager.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // _currentState.Draw(gameTime, _mSpriteBatch);
            stateManager.Draw(gameTime, _mSpriteBatch);
       
            base.Draw(gameTime);
        }
    }
}
