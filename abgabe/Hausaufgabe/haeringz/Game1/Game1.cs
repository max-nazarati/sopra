using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace Game1
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        private Texture2D backgroundTexture;

        private Texture2D uniLogoTexture;
        private Vector2 uniLogoCenter;
        private Vector2 uniLogoOffset;
        private float uniLogoScale;
        private Rectangle logoRectangle;


        // Variables controlling the size of the moving circle
        private float uniLogoMoveRadius;
        private float uniLogoDegree;
        private float uniLogoTurningDegree;

        // Sound
        private bool overlappingSound;
        SoundEffect mLogoHit;
        SoundEffect mLogoMiss;
        SoundEffectInstance mLogoHitInstance;
        SoundEffectInstance mLogoMissInstance;

        // Mouse
        private MouseState prevMouseState;
        private MouseState currentMouseState;


        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        /*
        public class Circle : Rectangle
        {

        }
        */

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Window.AllowUserResizing = true; 
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // general stuff
            this.IsMouseVisible = true;

            // Background
            // backgroundScale = new Vector2(1000000 / graphics.PreferredBackBufferWidth, 1 / graphics.PreferredBackBufferHeight);

            // uniLogo
            uniLogoCenter = new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2);
            uniLogoMoveRadius = graphics.PreferredBackBufferWidth / 6;
            uniLogoScale = 0.2f;
            uniLogoDegree = 0f;
            uniLogoTurningDegree = (float)Math.PI / 240f;
            logoRectangle = new Rectangle((int)uniLogoCenter.X, (int)uniLogoCenter.Y, 100, 100);

            overlappingSound = true;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Textures
            uniLogoTexture = Content.Load<Texture2D>("Unilogo");
            backgroundTexture = Content.Load<Texture2D>("Background");
            // Sound
            mLogoHit = Content.Load<SoundEffect>("hitmarker");
            mLogoMiss = Content.Load<SoundEffect>("miss");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // If you are creating your texture (instead of loading it with
            // Content.Load) then you must Dispose of it
        }



        void InitializeSoundInstanceHit()
        {
            if (mLogoHitInstance is null)
            {
                mLogoHitInstance = mLogoHit.CreateInstance();
            }
        }

        void InitializeSoundInstanceMiss()
        {
            if (mLogoMissInstance is null)
            {
                mLogoMissInstance = mLogoMiss.CreateInstance();
            }
        }

        void playSoundHit()
        {
            if (overlappingSound)
            {
                mLogoHit.Play();
            }
            else
            {
                InitializeSoundInstanceHit();
                if (mLogoHitInstance.State == SoundState.Stopped)
                {
                    mLogoHitInstance.Play();
                }
            }
        }

        void playSoundMiss()
        {
            if (overlappingSound)
            {
                mLogoMiss.Play();
            }
            else
            {
                InitializeSoundInstanceMiss();
                if(mLogoMissInstance.State == SoundState.Stopped)
                {
                    mLogoMissInstance.Play();
                }
            }
        }

        void updateLogoRectangle()
        {
            logoRectangle.X = ((int)uniLogoCenter.X) + ((int)uniLogoOffset.X) - ((int)(uniLogoTexture.Width * uniLogoScale / 2f));
            logoRectangle.Y = ((int)uniLogoCenter.Y) + ((int)uniLogoOffset.Y) - ((int)(uniLogoTexture.Height * uniLogoScale / 2f));
            logoRectangle.Width = (int)(uniLogoTexture.Width * uniLogoScale);
            logoRectangle.Height = (int)(uniLogoTexture.Height * uniLogoScale);
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

            // update the Angel from 0 to 2pi
            uniLogoDegree += uniLogoTurningDegree;
            if (uniLogoDegree >= 2 * Math.PI ) { uniLogoDegree = 0; }  // avoiding silly jumps when overflowing
            // calculate the position difference to the center
            uniLogoOffset = new Vector2(
                (float)Math.Cos(uniLogoDegree) * uniLogoMoveRadius,
                (float)Math.Sin(uniLogoDegree) * uniLogoMoveRadius
                );
            



            prevMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

            // change the sound Mode with RightMouseButton

            if (currentMouseState.RightButton == ButtonState.Pressed &&
                prevMouseState.RightButton != ButtonState.Pressed)
            {
                overlappingSound = !overlappingSound;
            }



            if (currentMouseState.LeftButton == ButtonState.Pressed && prevMouseState.LeftButton != ButtonState.Pressed)
            {
                updateLogoRectangle();  // updating the hitbox only when needed
                if (logoRectangle.Contains(currentMouseState.Position)){playSoundHit();}else{playSoundMiss();}
            }

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
            //spriteBatch.Begin();
            // 
            //spriteBatch.Draw(backgroundTexture, null, null);
            //spriteBatch.End();

            spriteBatch.Begin();

            //spriteBatch.Draw(backgroundTexture, color: Color.White, scale: new Vector2(backgroundTexture.Width / graphics.PreferredBackBufferWidth, backgroundTexture.Heigth / graphics.PreferredBackBufferheigth));
            spriteBatch.Draw(
                backgroundTexture,
                new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight),
                new Rectangle(0,0, backgroundTexture.Width, backgroundTexture.Height),
                Color.White);


            spriteBatch.Draw(uniLogoTexture,
                uniLogoCenter + uniLogoOffset,
                null,
                color:Color.Black,
                0f,
                new Vector2(uniLogoTexture.Width / 2, uniLogoTexture.Height / 2),
                uniLogoScale,
                SpriteEffects.None,
                0f);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
