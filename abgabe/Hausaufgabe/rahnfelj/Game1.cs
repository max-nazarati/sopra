/*
 * Author: Jens Rahnfeld
 */

using System;
using System.CodeDom;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace rahnfelj
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        readonly GraphicsDeviceManager mMGraphics;
        SpriteBatch mSpriteBatch;
        private float mBackgroundWidthScale;
        private float mBackgroundHeightScale;
        private Texture2D mBackground;
        private Texture2D mLogoTexture;
        private Sprite mLogoSprite;
        private float mLogoCenterPositionX;
        private float mLogoCenterPositionY;
        private float mLogoPolarCoordinatesRadius;
        private float mLogoPolarCoordinateDegree;
        private SoundEffect mLogoSoundHit;
        private SoundEffectInstance mLogoSoundInstanceHit;
        private SoundEffect mLogoSoundMiss;
        private SoundEffectInstance mLogoSoundInstanceMiss;
        private MouseState mCurrentMouseState;

        public Game1()
        {
            mMGraphics = new GraphicsDeviceManager(this);
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
            mMGraphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
            mMGraphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
            mMGraphics.ApplyChanges();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            mSpriteBatch = new SpriteBatch(mMGraphics.GraphicsDevice);

            // initialize background
            mBackground = Content.Load<Texture2D>("Background");
            mBackgroundWidthScale = (float) mMGraphics.PreferredBackBufferWidth / mBackground.Width;
            mBackgroundHeightScale = (float) mMGraphics.PreferredBackBufferHeight / mBackground.Height;

            // initialize Uni-logo sprite
            mLogoTexture = Content.Load<Texture2D>("Unilogo");
            mLogoPolarCoordinatesRadius = (float) Math.Min(mMGraphics.PreferredBackBufferWidth, mMGraphics.PreferredBackBufferHeight) / 4;
            mLogoPolarCoordinateDegree = 0;
            float logoWidthScale = 0.2f * (float)Math.Min(mMGraphics.PreferredBackBufferWidth, mMGraphics.PreferredBackBufferHeight) / mLogoTexture.Width;
            float logoHeightScale = 0.2f * (float)Math.Min(mMGraphics.PreferredBackBufferWidth, mMGraphics.PreferredBackBufferHeight) / mLogoTexture.Height;
            mLogoCenterPositionX =
                (float) (mMGraphics.PreferredBackBufferWidth - mLogoTexture.Width * logoWidthScale) / 2;
            mLogoCenterPositionY =
                (float) (mMGraphics.PreferredBackBufferHeight - mLogoTexture.Height * logoHeightScale) / 2;
            float logoPositionX = (float) (mLogoCenterPositionX +mLogoPolarCoordinatesRadius * Math.Cos(mLogoPolarCoordinateDegree));
            float logoPositionY = (float) (mLogoCenterPositionY + mLogoPolarCoordinatesRadius * Math.Sin(mLogoPolarCoordinateDegree));
            mLogoSprite = new Sprite(mLogoTexture, logoPositionX, logoPositionY, logoWidthScale, logoHeightScale, 0, Color.White);

            // load sound files
            mLogoSoundHit = Content.Load<SoundEffect>("Logo_hit");
            mLogoSoundInstanceHit = mLogoSoundHit.CreateInstance();
            mLogoSoundMiss = Content.Load<SoundEffect>("Logo_miss");
            mLogoSoundInstanceMiss = mLogoSoundMiss.CreateInstance();
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

            // rotate Uni-logo sprite using polar coordinates
            mLogoPolarCoordinateDegree += 0.05f;
            float logoNewPositionX = (float)(mLogoCenterPositionX + mLogoPolarCoordinatesRadius * Math.Cos(mLogoPolarCoordinateDegree));
            float logoNewPositionY = (float)(mLogoCenterPositionY + mLogoPolarCoordinatesRadius * Math.Sin(mLogoPolarCoordinateDegree));
            mLogoSprite.SetPositionX(logoNewPositionX);
            mLogoSprite.SetPositionY(logoNewPositionY);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        [Obsolete]
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            mSpriteBatch.Begin();
            mSpriteBatch.Draw(mBackground, new Vector2(0, 0), color: Color.White,
                scale: new Vector2(mBackgroundWidthScale, mBackgroundHeightScale));
            mLogoSprite.Draw(mSpriteBatch);
            mSpriteBatch.End();

            mCurrentMouseState = Mouse.GetState();
            if (mCurrentMouseState.LeftButton == ButtonState.Pressed & mLogoSprite.TouchesSprite(mCurrentMouseState.X, mCurrentMouseState.Y))
            {
                mLogoSoundHit.Play();
            }
            if (mCurrentMouseState.LeftButton == ButtonState.Pressed & !mLogoSprite.TouchesSprite(mCurrentMouseState.X, mCurrentMouseState.Y))
            {
                mLogoSoundMiss.Play();
            }

            base.Draw(gameTime);
        }
    }
}
