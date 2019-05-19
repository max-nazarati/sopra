using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace KernelPanic
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    internal sealed class Game1 : Game
    {
        readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _mSpriteBatch;
        private SoundManager _mMusic;
        private Grid _mWorld;
        private Camera2D _mCamera;
        private StateManager _stateManager;
        public readonly List<State> _stateList = new List<State>();
        public Game1()
        {
            Content.RootDirectory = "Content";
            _graphics = new GraphicsDeviceManager(this);

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
            // TODO: Add your initialization logic here
            IsMouseVisible = true;
            _mCamera = new Camera2D(GraphicsDevice.Viewport);
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

            // TODO: use this.Content to load your game content here
            _mMusic = new SoundManager("testSoundtrack", Content);
            _mMusic.Init();
            _mMusic.Play();

            _mWorld = new Grid(Content, 20, 5, false);
            _stateManager = new StateManager(this, _graphics, Content);
            _stateList.Add(new StartMenuState(_stateManager, _graphics, Content));
            _stateList.Add(new GameState(_stateManager, _graphics, Content));
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                _stateManager.AddState(new StartMenuState(_stateManager, _graphics, Content));
            // TODO: Add your update logic here
            _mCamera.Update(gameTime);
            Console.WriteLine(1 / (float)gameTime.ElapsedGameTime.TotalSeconds);
            InputManager.Default.Update();

            _stateManager.Update(gameTime);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.MintCream);

            // TODO: Add your drawing code here
            var viewMatrix = _mCamera.GetViewMatrix();
            _mSpriteBatch.Begin(transformMatrix: viewMatrix);
            _mWorld.Draw(_mSpriteBatch, _mCamera);
            _mSpriteBatch.End();

            _stateManager.Draw(gameTime, _mSpriteBatch);

            base.Draw(gameTime);
        }
    }
}
