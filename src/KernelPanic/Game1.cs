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
        readonly GraphicsDeviceManager graphics;
        private SpriteBatch mSpriteBatch;
        private SoundManager mMusic;
        private Grid mWorld;
        private Camera2D mCamera;
        private StateManager stateManager;
        private List<State> stateList = new List<State>();
        public Game1()
        {
            Content.RootDirectory = "Content";
            graphics = new GraphicsDeviceManager(this);

            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            graphics.ApplyChanges();
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
            this.IsMouseVisible = true;
            mCamera = new Camera2D(GraphicsDevice.Viewport);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            mSpriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            mMusic = new SoundManager("testSoundtrack", Content);
            mMusic.Init();
            mMusic.Play();

            mWorld = new Grid(Content, 20, 5, false);
            stateManager = new StateManager(this, graphics, Content);
            stateList.Add(new StartMenuState(stateManager, graphics, Content));
            stateList.Add(new GameState(stateManager, graphics, Content));
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
                Exit();
            // TODO: Add your update logic here
            mCamera.Update(gameTime);
            Console.WriteLine(1 / (float)gameTime.ElapsedGameTime.TotalSeconds);

            stateManager.Update(gameTime);
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
            var viewMatrix = mCamera.GetViewMatrix();
            mSpriteBatch.Begin(transformMatrix: viewMatrix);
            mWorld.Draw(mSpriteBatch, mCamera);
            mSpriteBatch.End();

            stateManager.Draw(gameTime, mSpriteBatch);

            base.Draw(gameTime);
        }
    }
}
