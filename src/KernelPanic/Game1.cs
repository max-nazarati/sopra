using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace KernelPanic
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    internal sealed class Game1 : Game
    {
        private SpriteBatch mSpriteBatch;
        // private Grid mWorld;
        private Grid mWorld2;
        private Grid mWorld3;
        private Camera2D mCamera;

        private readonly GraphicsDeviceManager mGraphics;

        private GameStateManager mGameStateManager;
        //private StateManager mStateManager;
        //private readonly List<State> mStateList = new List<State>();

        private UnitManager mUnitManager;
        private CollisionManager mCollisionManager;
        private Unit mUnit;

        public Game1()
        {
            Content.RootDirectory = "Content";
            mGraphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1920,
                PreferredBackBufferHeight = 1080
            };

            // No mGraphics.ApplyChanges() required as explained here:
            // https://stackoverflow.com/a/11287316/1592765
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

            mGameStateManager = new GameStateManager(this, Content);

            // TODO: use this.Content to load your game content here
            SoundManager.Instance.Init(Content);
            SoundManager.Instance.PlayBackgroundMusic();

            // mWorld2 = new Grid(Content, Grid.LaneSide.Left, new Rectangle(0, 0, 20, 50));
            mWorld2 = new Grid(Content, Grid.LaneSide.Left, new Rectangle(0, 0, 16, 42));
            // mWorld3 = new Grid(Content, Grid.LaneSide.Right, new Rectangle(30, 0, 20, 50));
            mWorld3 = new Grid(Content, Grid.LaneSide.Right, new Rectangle(15, 0, 16, 42));

            mUnitManager = new UnitManager();
            mCollisionManager = new CollisionManager();
            mUnit = new Unit(0, 0, 100, 100);
            mUnitManager.AddUnit(mUnit);
            mCollisionManager.CreatedObject(mUnit);
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
            // TODO: Add your update logic here
            if (mGameStateManager.Empty())
            {
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || InputManager.Default.KeyPressed(Keys.Escape))
                    mGameStateManager.Push(MenuState.CreateMainMenu(mGameStateManager, false));
                mCamera.Update();

                Console.WriteLine("fps: " + 1 / (float) gameTime.ElapsedGameTime.TotalSeconds);

                /* if (mStateList != null)
                 {
                     // Console.WriteLine(mStateList);
                 }*/


                mUnitManager.Update();
                mCollisionManager.Update();
            }
            else
            {
                mGameStateManager.Update(gameTime, false);
            }

            InputManager.Default.Update(gameTime);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            if (!mGameStateManager.Empty())
            {
                mGameStateManager.Draw(mSpriteBatch, gameTime);
            }
            else
            {
                GraphicsDevice.Clear(Color.MintCream);

                // TODO: Add your drawing code here

                var viewMatrix = mCamera.GetViewMatrix();

                mSpriteBatch.Begin(transformMatrix: viewMatrix);

                //mWorld.Draw(mSpriteBatch, mCamera);
                mWorld2.Draw(mSpriteBatch, mCamera.GetViewMatrix(), gameTime);
                mWorld3.Draw(mSpriteBatch, mCamera.GetViewMatrix(), gameTime);

                mSpriteBatch.End();

                //mStateManager.Draw(mSpriteBatch);
            }
            base.Draw(gameTime);
        }
    }
}
