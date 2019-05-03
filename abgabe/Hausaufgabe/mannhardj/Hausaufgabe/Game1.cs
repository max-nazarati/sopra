using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Hausaufgabe
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch mSpriteBatch;
        Texture2D mBackground;
        Texture2D mUnilogo;
        private SoundEffect mHit;
        private SoundEffect mMiss;
        private double mLogoPositionY;
        private double mLogoPositionX;
        private int mOriginalLogoSize;
        private int mCurrentLogoSize;
        private int mWindowSizeX;
        private int mWindowSizeY;
        private float mScale;
        private MouseState mMouseState;
        private Rectangle mLogoRectangle;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
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
            this.IsMouseVisible = true;

            // TODO: Add your initialization logic here
            mLogoPositionX = 100;
            mLogoPositionY = 100;
            mOriginalLogoSize = 893;
            mScale = 0.3f;
            mWindowSizeX = 1200;
            mWindowSizeY = 1000;
            graphics.PreferredBackBufferWidth = mWindowSizeX;  // set this value to the desired width of your window
            graphics.PreferredBackBufferHeight = mWindowSizeY;   // set this value to the desired height of your window
            graphics.ApplyChanges();

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
            mBackground = Content.Load<Texture2D>("Background");
            mUnilogo = Content.Load<Texture2D>("Unilogo");

            mHit = Content.Load<SoundEffect>("Logo_hit");
            mMiss = Content.Load<SoundEffect>("Logo_miss");
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
            mMouseState = Mouse.GetState();
            mCurrentLogoSize = (int)(mOriginalLogoSize * mScale);

            if (mMouseState.LeftButton == ButtonState.Pressed)
            {
                mLogoRectangle = new Rectangle((int)mLogoPositionX, (int)mLogoPositionY, mCurrentLogoSize,mCurrentLogoSize);
                if (mLogoRectangle.Contains(mMouseState.Position))
                {
                    mHit.Play();
                }
                else
                {
                    mMiss.Play();
                }
            }

            var factorX = Math.Sin(gameTime.TotalGameTime.TotalSeconds);
            var factorY = Math.Cos(gameTime.TotalGameTime.TotalSeconds);

            mLogoPositionX = mWindowSizeX * 0.5 + factorX * (mWindowSizeX-mCurrentLogoSize * 1.1) / 2 - mCurrentLogoSize * 0.5;
            mLogoPositionY = mWindowSizeY * 0.5 + factorY * (mWindowSizeY-mCurrentLogoSize * 1.1) / 2 - mCurrentLogoSize * 0.5;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            mSpriteBatch.Begin();
            mSpriteBatch.Draw(mBackground, new Vector2(0, 0), Color.White);
            mSpriteBatch.Draw(mUnilogo, new Vector2((float)mLogoPositionX, (float)mLogoPositionY), null, Color.White, 0f,
                Vector2.Zero, mScale, SpriteEffects.None, 0f);
            mSpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}