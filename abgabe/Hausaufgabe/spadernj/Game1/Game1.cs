using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game1
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public sealed class Game1 : Game
    {
        private GraphicsDeviceManager mGraphics;
        private SpriteBatch mSpriteBatch;

        private Texture2D mBackgroundTexture;
        private Texture2D mLogoTexture;
        private const float LogoScale = 0.2f;

        private SoundEffect mSoundHit;
        private SoundEffect mSoundMiss;

        private Vector2 mLogoPosition;

        private float LogoSize => mLogoTexture.Width * LogoScale;

        private Rectangle LogoRect
        {
            get
            {
                var offset = LogoSize / 2;
                var size = (int) Math.Ceiling(LogoSize);
                var x = mLogoPosition.X - offset;
                var y = mLogoPosition.Y - offset;
                return new Rectangle((int) x, (int) y, size, size);
            }
        }

        
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
            IsMouseVisible = true;
            Window.AllowUserResizing = true;

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

            mBackgroundTexture = Content.Load<Texture2D>("Background");
            mLogoTexture = Content.Load<Texture2D>("Unilogo");
            mSoundHit = Content.Load<SoundEffect>("sound_hit");
            mSoundMiss = Content.Load<SoundEffect>("sound_miss");
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

            // Handle mouse-clicks.
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                if (LogoRect.Contains(Mouse.GetState().Position))
                {
                    mSoundHit.Play();
                }
                else
                {
                    mSoundMiss.Play();
                }
            }

            // Move the logo.
            mLogoPosition = CalculatePosition(gameTime.TotalGameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// Calculates the mid-point of the logo based upon a time so that the logo moves on an arc around the window's middle.
        /// </summary>
        /// <param name="ts">The time for which the game is running, upon this value the logo's position is chosen on its arc.</param>
        /// <returns>The logo's calculates mid-point.</returns>
        private Vector2 CalculatePosition(TimeSpan ts)
        {
            // The current window dimensions
            var winWidth = Window.ClientBounds.Width;
            var winHeight = Window.ClientBounds.Height;

            // The factors for the current position on the circle/ellipsis.
            var factorX = Math.Sin(ts.TotalSeconds);
            var factorY = Math.Cos(ts.TotalSeconds);

            // A border which the logo won't touch.
            var border = 50;

            // Calculate the logos mid-point for the case where the coordinate-system's origin is in the middle.
            var x = (winWidth - LogoSize - border) * 0.5 * factorX;
            var y = (winHeight - LogoSize - border) * 0.5 * factorY;

            // Adjust the calculated point for the origin (0,0) in the upper left corner.
            return new Vector2((float)(x + winWidth * 0.5), (float)(y + winHeight * 0.5));
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            var logoOrigin = new Vector2(mLogoTexture.Width / 2f, mLogoTexture.Height / 2f);

            mSpriteBatch.Begin();
            mSpriteBatch.Draw(mBackgroundTexture, new Rectangle(new Point(0, 0), Window.ClientBounds.Size), Color.White);
            mSpriteBatch.Draw(mLogoTexture, mLogoPosition, null, Color.White, 0f, logoOrigin, LogoScale, SpriteEffects.None, 1);
            mSpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
