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
        readonly GraphicsDeviceManager mGraphics;
        SpriteBatch mSpriteBatch;
        private float mBackgroundWidthScale;
        private float mBackgroundHeightScale;
        private Texture2D mBackground;
        private Texture2D mLogoTexture;
        private Sprite mLogoSprite;
        private float mLogoCenterPositionX;
        private float mLogoCenterPositionY;
        private float mLogoPoloarCoordinatesRadius;
        private float mLogoPolarCoordinateDegree;
        private SoundEffect mLogoSoundHit;
        private SoundEffectInstance mLogoSoundInstanceHit;
        private SoundEffect mLogoSoundMiss;
        private SoundEffectInstance mLogoSoundInstanceMiss;
        private MouseState mCurrentMouseState;
        private Boolean mMouseMiddleButtonPressed;
        private float mMousePressedX;
        private float mMousePressedY;
        private float mMouseAccelerationX = 0;
        private float mMouseAccelerationY = 0;
        private int mMouseLastScrollValue = 0;
        private KeyboardState mCurrentKeyboardState;
        private Camera mCam;

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
            this.IsMouseVisible = true;
            mGraphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
            mGraphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
            mGraphics.ApplyChanges();

            float windowWidth = mGraphics.PreferredBackBufferWidth;
            float windowHeight = mGraphics.PreferredBackBufferHeight;
            mCam = new Camera(Matrix.CreateTranslation(0, 0, 0),
                GraphicsDevice.Viewport,
                windowWidth,
                windowHeight,
                0.2f,
                5,
                1);

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

            // initialize background
            mBackground = Content.Load<Texture2D>("Background");
            mBackgroundWidthScale = (float) mGraphics.PreferredBackBufferWidth / mBackground.Width;
            mBackgroundHeightScale = (float) mGraphics.PreferredBackBufferHeight / mBackground.Height;

            // initialize Uni-logo sprite
            mLogoTexture = Content.Load<Texture2D>("Unilogo");
            mLogoPoloarCoordinatesRadius = (float) Math.Min(mGraphics.PreferredBackBufferWidth, mGraphics.PreferredBackBufferHeight) / 4;
            mLogoPolarCoordinateDegree = 0;
            float logoWidthScale = 0.2f * (float)Math.Min(mGraphics.PreferredBackBufferWidth, mGraphics.PreferredBackBufferHeight) / mLogoTexture.Width;
            float logoHeightScale = 0.2f * (float)Math.Min(mGraphics.PreferredBackBufferWidth, mGraphics.PreferredBackBufferHeight) / mLogoTexture.Height;
            mLogoCenterPositionX =
                (float) (mGraphics.PreferredBackBufferWidth - mLogoTexture.Width * logoWidthScale) / 2;
            mLogoCenterPositionY =
                (float) (mGraphics.PreferredBackBufferHeight - mLogoTexture.Height * logoHeightScale) / 2;
            float logoPositionX = (float) (mLogoCenterPositionX +mLogoPoloarCoordinatesRadius * Math.Cos(mLogoPolarCoordinateDegree));
            float logoPositionY = (float) (mLogoCenterPositionY + mLogoPoloarCoordinatesRadius * Math.Sin(mLogoPolarCoordinateDegree));
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
            float logoNewPositionX = (float)(mLogoCenterPositionX + mLogoPoloarCoordinatesRadius * Math.Cos(mLogoPolarCoordinateDegree));
            float logoNewPositionY = (float)(mLogoCenterPositionY + mLogoPoloarCoordinatesRadius * Math.Sin(mLogoPolarCoordinateDegree));
            mLogoSprite.SetPositionX(logoNewPositionX);
            mLogoSprite.SetPositionY(logoNewPositionY);

            mCurrentMouseState = Mouse.GetState();
            mCurrentKeyboardState = Keyboard.GetState();

            if (mCurrentMouseState.LeftButton == ButtonState.Pressed & mLogoSprite.TouchesSprite(mCurrentMouseState.X, mCurrentMouseState.Y))
            {
                mLogoSoundHit.Play();
            }
            if (mCurrentMouseState.LeftButton == ButtonState.Pressed & !mLogoSprite.TouchesSprite(mCurrentMouseState.X, mCurrentMouseState.Y))
            {
                mLogoSoundMiss.Play();
            }

            if ((mCurrentMouseState.X >= mGraphics.PreferredBackBufferWidth - 10 &
                 mCurrentMouseState.X <= mGraphics.PreferredBackBufferWidth) |
                mCurrentKeyboardState.IsKeyDown(Keys.Right))
            {
                mCam.MoveRight(20);
            }
            if ((mCurrentMouseState.X <= 10 & mCurrentMouseState.X >= 0) | mCurrentKeyboardState.IsKeyDown(Keys.Left))
            {
                mCam.MoveLeft(20);
            }
            if ((mCurrentMouseState.Y <= 10 && mCurrentMouseState.Y >= 0) | mCurrentKeyboardState.IsKeyDown(Keys.Up))
            {
                mCam.MoveUp(20);
            }
            if ((mCurrentMouseState.Y >= mGraphics.PreferredBackBufferHeight - 50 &
                mCurrentMouseState.Y <= mGraphics.PreferredBackBufferHeight) |
                mCurrentKeyboardState.IsKeyDown(Keys.Down))
            {
                mCam.MoveDown(20);
            }

            if (mCurrentMouseState.MiddleButton == ButtonState.Pressed && !mMouseMiddleButtonPressed)
            {
                mMouseMiddleButtonPressed = true;
                mMousePressedX = mCurrentMouseState.X;
                mMousePressedY = mCurrentMouseState.Y;
            }

            if (mCurrentMouseState.MiddleButton == ButtonState.Released && mMouseMiddleButtonPressed)
            {
                mMouseMiddleButtonPressed = false;
                float diffX = mCurrentMouseState.X - mMousePressedX;
                float diffY = mCurrentMouseState.Y - mMousePressedY;
                mMouseAccelerationX = 0.5f * diffX;
                mMouseAccelerationY = 0.5f * diffY;
            }

            if (mCurrentMouseState.ScrollWheelValue < mMouseLastScrollValue)
            {
                mCam.ZoomOut(1.5f);
                mMouseLastScrollValue = mCurrentMouseState.ScrollWheelValue;
            }

            if (mCurrentMouseState.ScrollWheelValue > mMouseLastScrollValue)
            {
                mCam.ZoomIn(1.5f);
                mMouseLastScrollValue = mCurrentMouseState.ScrollWheelValue;
            }

            mCam.TransformCamera(Matrix.CreateTranslation(mMouseAccelerationX, mMouseAccelerationY, 0));
            mMouseAccelerationX /= 1.5f;
            mMouseAccelerationY /= 1.5f;

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

            mSpriteBatch.Begin(SpriteSortMode.BackToFront,
                BlendState.AlphaBlend,
                null,
                null,
                null,
                null,
                transformMatrix: mCam.TransformMatrix);
            mSpriteBatch.Draw(mBackground, new Vector2(0, 0), color: Color.White,
                scale: new Vector2(mBackgroundWidthScale, mBackgroundHeightScale));
            mLogoSprite.Draw(mSpriteBatch);
            mSpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
