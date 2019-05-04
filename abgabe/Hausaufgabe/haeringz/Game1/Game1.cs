using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game1
{
    /// <summary>
    ///     This is the main type for your game.
    /// </summary>
    public sealed class Game1 : Game
    {
        private Texture2D mBackgroundTexture;
        private MouseState mCurrentMouseState;

        private readonly GraphicsDeviceManager mGraphics;
        private Rectangle mLogoRectangle;
        private SoundEffect mLogoHit;
        private SoundEffectInstance mLogoHitInstance;
        private SoundEffect mLogoMiss;
        private SoundEffectInstance mLogoMissInstance;

        // Sound
        private bool mOverlappingSound;

        // Mouse
        private MouseState mPrevMouseState;
        private SpriteBatch mSpriteBatch;
        private Vector2 mUniLogoCenter;
        private float mUniLogoDegree;


        // moving circle
        private float mUniLogoMoveRadius;
        private Vector2 mUniLogoOffset;
        private float mUniLogoScale;

        private Texture2D mUniLogoTexture;
        private float mUniLogoTurningDegree;


        public Game1()
        {
            mGraphics = new GraphicsDeviceManager(game: this);
            Content.RootDirectory = "Content";
            Window.AllowUserResizing = true;
        }

        /// <summary>
        ///     Allows the game to perform any initialization it needs to before starting to run.
        ///     This is where it can query for any required services and load any non-graphic
        ///     related content.  Calling base.Initialize will enumerate through any components
        ///     and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // general stuff
            IsMouseVisible = true;

            // Background
            // backgroundScale = new Vector2(1000000 / graphics.PreferredBackBufferWidth, 1 / graphics.PreferredBackBufferHeight);

            // uniLogo
            mUniLogoCenter = new Vector2(x: mGraphics.PreferredBackBufferWidth / 2f, y: mGraphics.PreferredBackBufferHeight / 2f);
            mUniLogoMoveRadius = mGraphics.PreferredBackBufferWidth / 6f;
            mUniLogoScale = 0.2f;
            mUniLogoDegree = 0f;
            mUniLogoTurningDegree = (float) Math.PI / 240f;
            mLogoRectangle = new Rectangle((int) mUniLogoCenter.X, (int) mUniLogoCenter.Y, width: 100, height: 100);

            mOverlappingSound = true;
            base.Initialize();
        }

        /// <summary>
        ///     LoadContent will be called once per game and is the place to load
        ///     all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            mSpriteBatch = new SpriteBatch(GraphicsDevice);

            // Textures
            mUniLogoTexture = Content.Load<Texture2D>("Unilogo");
            mBackgroundTexture = Content.Load<Texture2D>("Background");
            // Sound
            mLogoHit = Content.Load<SoundEffect>("hitmarker");
            mLogoMiss = Content.Load<SoundEffect>("miss");
        }

        /// <summary>
        ///     UnloadContent will be called once per game and is the place to unload
        ///     game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // If you are creating your texture (instead of loading it with
            // Content.Load) then you must Dispose of it
        }


        private void InitializeSoundInstanceHit()
        {
            if (mLogoHitInstance is null) mLogoHitInstance = mLogoHit.CreateInstance();
        }

        private void InitializeSoundInstanceMiss()
        {
            if (mLogoMissInstance is null) mLogoMissInstance = mLogoMiss.CreateInstance();
        }

        private void PlaySoundHit()
        {
            if (mOverlappingSound)
            {
                mLogoHit.Play();
            }
            else
            {
                InitializeSoundInstanceHit();
                if (mLogoHitInstance.State == SoundState.Stopped) mLogoHitInstance.Play();
            }
        }

        private void PlaySoundMiss()
        {
            if (mOverlappingSound)
            {
                mLogoMiss.Play();
            }
            else
            {
                InitializeSoundInstanceMiss();
                if (mLogoMissInstance.State == SoundState.Stopped) mLogoMissInstance.Play();
            }
        }

        private void UpdateLogoRectangle()
        {
            mLogoRectangle.X = (int) mUniLogoCenter.X + (int) mUniLogoOffset.X -
                              (int) (mUniLogoTexture.Width * mUniLogoScale / 2f);
            mLogoRectangle.Y = (int) mUniLogoCenter.Y + (int) mUniLogoOffset.Y -
                              (int) (mUniLogoTexture.Height * mUniLogoScale / 2f);
            mLogoRectangle.Width = (int) (mUniLogoTexture.Width * mUniLogoScale);
            mLogoRectangle.Height = (int) (mUniLogoTexture.Height * mUniLogoScale);
        }


        /// <summary>
        ///     Allows the game to run logic such as updating the world,
        ///     checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // update the Angel from 0 to 2pi
            mUniLogoDegree += mUniLogoTurningDegree;
            if (mUniLogoDegree >= 2 * Math.PI) mUniLogoDegree = 0;
            // calculate the position difference to the center
            mUniLogoOffset = new Vector2(
                (float) Math.Cos(mUniLogoDegree) * mUniLogoMoveRadius,
                (float) Math.Sin(mUniLogoDegree) * mUniLogoMoveRadius
            );

            mPrevMouseState = mCurrentMouseState;
            mCurrentMouseState = Mouse.GetState();

            // change the sound Mode with RightMouseButton
            if (mCurrentMouseState.RightButton == ButtonState.Pressed &&
                mPrevMouseState.RightButton != ButtonState.Pressed)
            {
                mOverlappingSound = !mOverlappingSound;
            }
                


            if (mCurrentMouseState.LeftButton == ButtonState.Pressed && mPrevMouseState.LeftButton != ButtonState.Pressed)
            {
                UpdateLogoRectangle(); // updating the hit box only when needed
                if (mLogoRectangle.Contains(mCurrentMouseState.Position))
                    PlaySoundHit();
                else
                    PlaySoundMiss();
            }

            base.Update(gameTime);
        }

        /// <summary>
        ///     This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            mSpriteBatch.Begin();
            mSpriteBatch.Draw(
                mBackgroundTexture,
                new Rectangle(x: 0, y: 0, mGraphics.PreferredBackBufferWidth, mGraphics.PreferredBackBufferHeight),
                new Rectangle(x: 0, y: 0, mBackgroundTexture.Width, mBackgroundTexture.Height),
                Color.White);


            if (mUniLogoTexture != null)
                mSpriteBatch.Draw(mUniLogoTexture,
                    position: mUniLogoCenter + mUniLogoOffset,
                    sourceRectangle: null,
                    Color.Black,
                    rotation: 0f,
                    
                    origin: new Vector2(x: mUniLogoTexture.Width / 2f, y: mUniLogoTexture.Height / 2f),
                    mUniLogoScale,
                    SpriteEffects.None,
                    layerDepth: 0f);
            mSpriteBatch.End();
            base.Draw(gameTime);
        }
    }
}