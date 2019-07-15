using System.Collections.Generic;
using KernelPanic.Camera;
using KernelPanic.Events;
using KernelPanic.Input;
using KernelPanic.Tracking;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    internal sealed class Game1 : Game
    {
        private readonly GraphicsDeviceManager mGraphics;
        private SpriteBatch mSpriteBatch;
        private GameStateManager mGameStateManager;
        private readonly RawInputState mInputState;
        private SoundManager mSoundManager;

        public Game1()
        {
            Content.RootDirectory = "Content";

            mGraphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width,
                PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height
            };
            
            // No mGraphics.ApplyChanges() required as explained here:
            // https://stackoverflow.com/a/11287316/1592765

            mInputState = new RawInputState();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                mGameStateManager?.Dispose();
                mGameStateManager = null;
            }

            base.Dispose(disposing);
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
            mSoundManager = new SoundManager(Content);
            EventCenter.Default.Subscribe(Event.Id.AchievementUnlocked, @event =>
            {
                var achievement = @event.Get<Achievement>(Event.Key.Achievement);
                var screen = MenuState.CreateAchievementDisplay(achievement, mGameStateManager);
                mGameStateManager.Push(screen);
            });

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            mSpriteBatch = new SpriteBatch(mGraphics.GraphicsDevice);
            mGameStateManager =
                new GameStateManager(Exit, new SpriteManager(Content, GraphicsDevice), mGraphics);
            InGameState.PushGameStack(0, mGameStateManager);
            // SoundManager.Instance.PlayBackgroundMusic();
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
            mInputState.Update(IsActive, GraphicsDevice.Viewport);

            if (!DebugSettings.GamePaused)
            {
                mGameStateManager.Update(mInputState, gameTime);
                EventCenter.Default.Run();
            }

            DebugSettings.Update(new InputManager(new List<ClickTarget>(), new StaticCamera(), mInputState));
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.MintCream);
            mGameStateManager.Draw(mSpriteBatch, gameTime);
            base.Draw(gameTime);
        }
    }
}
