using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
// ReSharper disable All

namespace hausaufgabe
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public sealed class Game1 : Game
    {
        GraphicsDeviceManager mGraphics;
        SpriteBatch mSpriteBatch;

        Texture2D mTextureBackground;
        Texture2D mTextureUnilogo;

        SoundEffect mSoundMiss;
        SoundEffect mSoundHit;

        Rectangle mPositionUnilogo;
        float mAngleUnilogo;
        Point mCenter;

        MouseState mCurrentMouseState;
        MouseState mPreviousMouseState;


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
            // Position Unilogo in middle of screen
            mPositionUnilogo = new Rectangle(x: 400, y: 240, width: 150, height: 150);

            mCenter = new Point(400, 240);


            // Initialize Tracking Mouse Curser
            IsMouseVisible = true;
            mPreviousMouseState = Mouse.GetState();

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

            mTextureBackground = Content.Load<Texture2D>("Background");
            mTextureUnilogo = Content.Load<Texture2D>("Unilogo");

            mSoundHit = Content.Load<SoundEffect>("hit");
            mSoundMiss = Content.Load<SoundEffect>("miss");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            mCurrentMouseState = Mouse.GetState();
            var mousePosition = new Point(mCurrentMouseState.X, mCurrentMouseState.Y);

            // Playing a different Sound deppending on weather mouse is clicking on Unilogo or not 
            if (mCurrentMouseState.LeftButton == ButtonState.Pressed &
                               mPreviousMouseState.LeftButton == ButtonState.Released)
            {
                if (mPositionUnilogo.Contains(mousePosition))
                {
                    mSoundHit.Play();
                }
                else
                {
                    mSoundMiss.Play();
                }
            }

            mPreviousMouseState = mCurrentMouseState;


            // Calculation of current position in circle
            mAngleUnilogo += 0.01f; //

            mPositionUnilogo.X = (int)(mCenter.X - 75 + Math.Cos(mAngleUnilogo % 180) * 150);
            mPositionUnilogo.Y = (int)(mCenter.Y - 75 + Math.Sin(mAngleUnilogo % 180) * 150);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {

            mSpriteBatch.Begin();
            mSpriteBatch.Draw(texture: mTextureBackground, destinationRectangle: new Rectangle(x: 0, y: 0, width: 800, height: 480), color: Color.White);
            mSpriteBatch.Draw(texture: mTextureUnilogo, destinationRectangle: mPositionUnilogo, color: Color.White);
            mSpriteBatch.End();

            base.Draw(gameTime: gameTime);
        }
    }
}