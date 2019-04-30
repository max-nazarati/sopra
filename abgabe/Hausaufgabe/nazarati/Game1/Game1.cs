using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System;

namespace Game1
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public sealed class Game1 : Game
    {
        readonly GraphicsDeviceManager mGraphics;
        SpriteBatch mSpriteBatch;
        private Texture2D mBackground;
        private Texture2D mLogo;
        private double mLogoTopLeftCornerX;
        private double mLogoTopLeftCornerY;
        private Rectangle mLogoContainerRectangle;
        private double mTime;

        private MouseState mMouseState;
        private MouseState mOldMouseState;

        private SoundEffect mLogoClick;
        private SoundEffect mLogoMissClick;

        public Game1()
        {
            mGraphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            IsMouseVisible = true;

        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            mSpriteBatch = new SpriteBatch(mGraphics.GraphicsDevice);
            mBackground = Content.Load<Texture2D>("Background");
            mLogo = Content.Load<Texture2D>("Unilogo");

            mLogoClick = Content.Load<SoundEffect>("Logo_hit");
            mLogoMissClick = Content.Load<SoundEffect>("Logo_miss");
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            mTime = gameTime.TotalGameTime.TotalSeconds * 2;

            mLogoTopLeftCornerX = (Window.ClientBounds.Width / 2 - mLogo.Width / 8) + Math.Cos(mTime) * mLogo.Width / 8;
            mLogoTopLeftCornerY = (Window.ClientBounds.Height / 2 - mLogo.Height / 8) + Math.Sin(mTime) * mLogo.Height / 8;
            mMouseState = Mouse.GetState();

            mLogoContainerRectangle = new Rectangle((int)mLogoTopLeftCornerX, (int)mLogoTopLeftCornerY, mLogo.Width / 4, mLogo.Height / 4);
            if (mMouseState.LeftButton == ButtonState.Pressed && mOldMouseState.LeftButton == ButtonState.Released)
            {
                if (mLogoContainerRectangle.Contains(mMouseState.Position))
                {
                    mLogoClick.Play();
                }
                else
                {
                    mLogoMissClick.Play();
                }
            }

            mOldMouseState = mMouseState;  // This way only clicks and no holding is registered
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            mSpriteBatch.Begin();
            mSpriteBatch.Draw(mBackground, new Rectangle(0, 0, 800, 480), Color.White);
            mSpriteBatch.Draw(mLogo, mLogoContainerRectangle, Color.White);
            mSpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
