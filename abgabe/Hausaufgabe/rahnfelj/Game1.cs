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
        readonly GraphicsDeviceManager _mGraphics;
        SpriteBatch _mSpriteBatch;
        private float _mBackgroundWidthScale;
        private float _mBackgroundHeightScale;
        private Texture2D _mBackground;
        private Texture2D _mLogoTexture;
        private Sprite _mLogoSprite;
        private float _mLogoCenterPositionX;
        private float _mLogoCenterPositionY;
        private float _mLogoPoloarCoordinatesRadius;
        private float _mLogoPolarCoordinateDegree;
        private SoundEffect _mLogoSoundHit;
        private SoundEffectInstance _mLogoSoundInstanceHit;
        private SoundEffect _mLogoSoundMiss;
        private SoundEffectInstance _mLogoSoundInstanceMiss;
        private MouseState _mCurrentMouseState;

        public Game1()
        {
            _mGraphics = new GraphicsDeviceManager(this);
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
            _mGraphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
            _mGraphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
            _mGraphics.ApplyChanges();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _mSpriteBatch = new SpriteBatch(_mGraphics.GraphicsDevice);

            // initialize background
            _mBackground = Content.Load<Texture2D>("Background");
            _mBackgroundWidthScale = (float) _mGraphics.PreferredBackBufferWidth / _mBackground.Width;
            _mBackgroundHeightScale = (float) _mGraphics.PreferredBackBufferHeight / _mBackground.Height;

            // initialize Uni-logo sprite
            _mLogoTexture = Content.Load<Texture2D>("Unilogo");
            _mLogoPoloarCoordinatesRadius = (float) Math.Min(_mGraphics.PreferredBackBufferWidth, _mGraphics.PreferredBackBufferHeight) / 4;
            _mLogoPolarCoordinateDegree = 0;
            float logoWidthScale = 0.2f * (float)Math.Min(_mGraphics.PreferredBackBufferWidth, _mGraphics.PreferredBackBufferHeight) / _mLogoTexture.Width;
            float logoHeightScale = 0.2f * (float)Math.Min(_mGraphics.PreferredBackBufferWidth, _mGraphics.PreferredBackBufferHeight) / _mLogoTexture.Height;
            _mLogoCenterPositionX =
                (float) (_mGraphics.PreferredBackBufferWidth - _mLogoTexture.Width * logoWidthScale) / 2;
            _mLogoCenterPositionY =
                (float) (_mGraphics.PreferredBackBufferHeight - _mLogoTexture.Height * logoHeightScale) / 2;
            float logoPositionX = (float) (_mLogoCenterPositionX +_mLogoPoloarCoordinatesRadius * Math.Cos(_mLogoPolarCoordinateDegree));
            float logoPositionY = (float) (_mLogoCenterPositionY + _mLogoPoloarCoordinatesRadius * Math.Sin(_mLogoPolarCoordinateDegree));
            _mLogoSprite = new Sprite(_mLogoTexture, logoPositionX, logoPositionY, logoWidthScale, logoHeightScale, 0, Color.White);

            // load sound files
            _mLogoSoundHit = Content.Load<SoundEffect>("Logo_hit");
            _mLogoSoundInstanceHit = _mLogoSoundHit.CreateInstance();
            _mLogoSoundMiss = Content.Load<SoundEffect>("Logo_miss");
            _mLogoSoundInstanceMiss = _mLogoSoundMiss.CreateInstance();
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
            _mLogoPolarCoordinateDegree += 0.05f;
            float logoNewPositionX = (float)(_mLogoCenterPositionX + _mLogoPoloarCoordinatesRadius * Math.Cos(_mLogoPolarCoordinateDegree));
            float logoNewPositionY = (float)(_mLogoCenterPositionY + _mLogoPoloarCoordinatesRadius * Math.Sin(_mLogoPolarCoordinateDegree));
            _mLogoSprite.SetPositionX(logoNewPositionX);
            _mLogoSprite.SetPositionY(logoNewPositionY);

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

            _mSpriteBatch.Begin();
            _mSpriteBatch.Draw(_mBackground, new Vector2(0, 0), color: Color.White,
                scale: new Vector2(_mBackgroundWidthScale, _mBackgroundHeightScale));
            _mLogoSprite.Draw(_mSpriteBatch);
            _mSpriteBatch.End();

            _mCurrentMouseState = Mouse.GetState();
            if (_mCurrentMouseState.LeftButton == ButtonState.Pressed & _mLogoSprite.TouchesSprite(_mCurrentMouseState.X, _mCurrentMouseState.Y))
            {
                _mLogoSoundHit.Play();
            }
            if (_mCurrentMouseState.LeftButton == ButtonState.Pressed & !_mLogoSprite.TouchesSprite(_mCurrentMouseState.X, _mCurrentMouseState.Y))
            {
                _mLogoSoundMiss.Play();
            }

            base.Draw(gameTime);
        }
    }
}
