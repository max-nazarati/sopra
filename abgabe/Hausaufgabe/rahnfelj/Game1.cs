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
        SpriteBatch mMSpriteBatch;
        private float mMBackgroundWidthScale;
        private float mMBackgroundHeightScale;
        private Texture2D mMBackground;
        private Texture2D mMLogoTexture;
        private Sprite mMLogoSprite;
        private float mMLogoCenterPositionX;
        private float mMLogoCenterPositionY;
        private float mMLogoPolarCoordinatesRadius;
        private float mMLogoPolarCoordinateDegree;
        private SoundEffect mMLogoSoundHit;
        private SoundEffectInstance mMLogoSoundInstanceHit;
        private SoundEffect mMLogoSoundMiss;
        private SoundEffectInstance mMLogoSoundInstanceMiss;
        private MouseState mMCurrentMouseState;

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
            mMSpriteBatch = new SpriteBatch(mMGraphics.GraphicsDevice);

            // initialize background
            mMBackground = Content.Load<Texture2D>("Background");
            mMBackgroundWidthScale = (float) mMGraphics.PreferredBackBufferWidth / mMBackground.Width;
            mMBackgroundHeightScale = (float) mMGraphics.PreferredBackBufferHeight / mMBackground.Height;

            // initialize Uni-logo sprite
            mMLogoTexture = Content.Load<Texture2D>("Unilogo");
            mMLogoPolarCoordinatesRadius = (float) Math.Min(mMGraphics.PreferredBackBufferWidth, mMGraphics.PreferredBackBufferHeight) / 4;
            mMLogoPolarCoordinateDegree = 0;
            float logoWidthScale = 0.2f * (float)Math.Min(mMGraphics.PreferredBackBufferWidth, mMGraphics.PreferredBackBufferHeight) / mMLogoTexture.Width;
            float logoHeightScale = 0.2f * (float)Math.Min(mMGraphics.PreferredBackBufferWidth, mMGraphics.PreferredBackBufferHeight) / mMLogoTexture.Height;
            mMLogoCenterPositionX =
                (float) (mMGraphics.PreferredBackBufferWidth - mMLogoTexture.Width * logoWidthScale) / 2;
            mMLogoCenterPositionY =
                (float) (mMGraphics.PreferredBackBufferHeight - mMLogoTexture.Height * logoHeightScale) / 2;
            float logoPositionX = (float) (mMLogoCenterPositionX +mMLogoPolarCoordinatesRadius * Math.Cos(mMLogoPolarCoordinateDegree));
            float logoPositionY = (float) (mMLogoCenterPositionY + mMLogoPolarCoordinatesRadius * Math.Sin(mMLogoPolarCoordinateDegree));
            mMLogoSprite = new Sprite(mMLogoTexture, logoPositionX, logoPositionY, logoWidthScale, logoHeightScale, 0, Color.White);

            // load sound files
            mMLogoSoundHit = Content.Load<SoundEffect>("Logo_hit");
            mMLogoSoundInstanceHit = mMLogoSoundHit.CreateInstance();
            mMLogoSoundMiss = Content.Load<SoundEffect>("Logo_miss");
            mMLogoSoundInstanceMiss = mMLogoSoundMiss.CreateInstance();
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
            mMLogoPolarCoordinateDegree += 0.05f;
            float logoNewPositionX = (float)(mMLogoCenterPositionX + mMLogoPolarCoordinatesRadius * Math.Cos(mMLogoPolarCoordinateDegree));
            float logoNewPositionY = (float)(mMLogoCenterPositionY + mMLogoPolarCoordinatesRadius * Math.Sin(mMLogoPolarCoordinateDegree));
            mMLogoSprite.SetPositionX(logoNewPositionX);
            mMLogoSprite.SetPositionY(logoNewPositionY);

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

            mMSpriteBatch.Begin();
            mMSpriteBatch.Draw(mMBackground, new Vector2(0, 0), color: Color.White,
                scale: new Vector2(mMBackgroundWidthScale, mMBackgroundHeightScale));
            mMLogoSprite.Draw(mMSpriteBatch);
            mMSpriteBatch.End();

            mMCurrentMouseState = Mouse.GetState();
            if (mMCurrentMouseState.LeftButton == ButtonState.Pressed & mMLogoSprite.TouchesSprite(mMCurrentMouseState.X, mMCurrentMouseState.Y))
            {
                mMLogoSoundHit.Play();
            }
            if (mMCurrentMouseState.LeftButton == ButtonState.Pressed & !mMLogoSprite.TouchesSprite(mMCurrentMouseState.X, mMCurrentMouseState.Y))
            {
                mMLogoSoundMiss.Play();
            }

            base.Draw(gameTime);
        }
    }
}
