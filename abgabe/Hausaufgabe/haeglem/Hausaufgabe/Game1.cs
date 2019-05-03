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
        private Texture2D mBackgroundTexture;
        private Texture2D mLogoTexture;
        private Rectangle mLogoRectangle;
        private Point mCenter;
        private double mLogoAngle;
        private double mIncrementX;
        private double mIncrementY;
        private SoundEffect mHit;
        private SoundEffect mMiss;


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
            IsMouseVisible = true;
            mCenter = new Point(Window.ClientBounds.Width / 2, Window.ClientBounds.Height / 2);
            mLogoRectangle = new Rectangle(mCenter.X+150-75, mCenter.Y-75, 100, 100);
            mLogoAngle = 0d;
            mIncrementX = 0d;
            mIncrementY = 0d;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            mSpriteBatch = new SpriteBatch(graphics.GraphicsDevice);

            mBackgroundTexture = Content.Load<Texture2D>("Background");
            mLogoTexture = Content.Load<Texture2D>("Unilogo");

            mHit = Content.Load<SoundEffect>("Logo_hit");
            mMiss = Content.Load<SoundEffect>("Logo_miss");
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
            if (Mouse.GetState().LeftButton == ButtonState.Pressed && Window.ClientBounds.Contains(Mouse.GetState().Position))
            {
                if (mLogoRectangle.Contains(Mouse.GetState().Position))
                {
                    mHit.Play();
                }
                else
                {
                    mMiss.Play();
                }
            }
            mLogoAngle += 0.05d;
            mIncrementX = Math.Cos(mLogoAngle) * 100;
            mIncrementY = Math.Sin(mLogoAngle) * 100;
            mLogoRectangle.X = (int) (mCenter.X + mIncrementX - 75);
            mLogoRectangle.Y = (int) (mCenter.Y + mIncrementY - 75);
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
            mSpriteBatch.Draw(mBackgroundTexture, new Rectangle(new Point(0, 0), Window.ClientBounds.Size), Color.White);
            mSpriteBatch.Draw(mLogoTexture, mLogoRectangle, Color.White);
            mSpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
