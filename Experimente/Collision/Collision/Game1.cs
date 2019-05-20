using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Collision
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public sealed class Game1 : Game
    {
        readonly GraphicsDeviceManager mGraphics;
        SpriteBatch mSpriteBatch;
        private Texture2D mSquareTexture2D;

        private Unit mSquareUnit;
        private Unit mSquareUnit2;

        private CollisionManager mCollisionManager;
        private UnitManager mUnitManager;
        

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
            base.Initialize();
            IsMouseVisible = true;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            mSpriteBatch = new SpriteBatch(mGraphics.GraphicsDevice);
            mCollisionManager = new CollisionManager();
            mUnitManager = new UnitManager();

            mSquareTexture2D = new Texture2D(mGraphics.GraphicsDevice, 1, 1);
            mSquareTexture2D.SetData(new[] { Color.Green });
            mSquareUnit = new Unit(200, 200, 100, 100, mSquareTexture2D);
            mSquareUnit2 = new Unit(50, 50, 100, 100, mSquareTexture2D);
            mCollisionManager.CreatedObject(mSquareUnit);
            mCollisionManager.CreatedObject(mSquareUnit2);
            mUnitManager.AddUnit(mSquareUnit);
            mUnitManager.AddUnit(mSquareUnit2);

        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            /* 
             * ESC for unselecting a rectangle
             * Left Click for selecting, placing and dragging a rectangle
             * Right Click for telling a rectangle to move to specified point
             */

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                Exit();
            mUnitManager.Update();
            mCollisionManager.Update();
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
            mUnitManager.Draw(mSpriteBatch);
            mSquareUnit2.Draw(mSpriteBatch);
            mSquareUnit.Draw(mSpriteBatch);
            mSpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
