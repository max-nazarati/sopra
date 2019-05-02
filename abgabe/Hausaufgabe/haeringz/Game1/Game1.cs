using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game1
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        private Texture2D BackgroundTexture;
        private Texture2D uniLogoTexture;
        private Vector2 uniLogoCenter;
        private Vector2 uniLogoOffset;
        private float uniLogoScale;
        // Variables controlling the size of the moving circle
        private float uniLogoMoveRadius;
        private float uniLogoDegree;
        private float uniLogoTurningDegree;


        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        
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
            // TODO: Add your initialization logic here
            this.IsMouseVisible = true;
            uniLogoCenter = new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2);
            uniLogoMoveRadius = graphics.PreferredBackBufferWidth / 6;
            uniLogoScale = 0.2f;
            uniLogoDegree = 0f;
            uniLogoTurningDegree = (float)Math.PI / 120f;

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

            // TODO: use this.Content to load your game content here
            uniLogoTexture = Content.Load<Texture2D>("Unilogo");
            BackgroundTexture = Content.Load<Texture2D>("Background");
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

            // update the Angel from 0 to 2pi
            uniLogoDegree += uniLogoTurningDegree;
            if (uniLogoDegree % 2 * Math.PI < 0.001f) { uniLogoDegree = 0; }

            // calculate the position difference to the center
            uniLogoOffset = new Vector2(
                (float)Math.Cos(uniLogoDegree) * uniLogoMoveRadius,
                (float)Math.Sin(uniLogoDegree) * uniLogoMoveRadius
                );
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
            spriteBatch.Begin();
            spriteBatch.Draw(BackgroundTexture, new Vector2(0, 0), color:Color.White);
            spriteBatch.End();

            spriteBatch.Begin();
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
