using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    internal sealed class Game1 : Game
    {
        private SpriteBatch mSpriteBatch;
        private Board mBoard;

        private readonly GraphicsDeviceManager mGraphics;

        private GameStateManager mGameStateManager;
        //private StateManager mStateManager;
        //private readonly List<State> mStateList = new List<State>();

        private EntityGraph mEntityGraph;
        private CollisionManager mCollisionManager;
        private Unit mUnit1;
        private Unit mUnit2;
        private CooldownComponent mCoolDown;

        public Game1()
        {
            Content.RootDirectory = "Content";

            mGraphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1920,
                PreferredBackBufferHeight = 1080
            };

            // No mGraphics.ApplyChanges() required as explained here:
            // https://stackoverflow.com/a/11287316/1592765
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
            IsMouseVisible = true;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            /*
            List<Point> map = new List<Point>();
            for (int i = 0; i < 500; i++)
            {
                for (int j = 0; j < 500; j++)
                {
                    Point testPoint = new Point(i, j);
                    map.Add(testPoint);
                }
            }

            AStar testAStar = new AStar(map, new Point(0, 0), new Point(10, 10));
            List<Point> path = testAStar.FindPath();
            for (int i = path.Count - 1; i > 0; i--)
            {
                Console.WriteLine(path[i].ToString());
            }
            */

            // Create a new SpriteBatch, which can be used to draw textures.
            mSpriteBatch = new SpriteBatch(GraphicsDevice);
            mGameStateManager = new GameStateManager(Exit, new SpriteManager(Content, GraphicsDevice));
            mGameStateManager.Push(new InGameState(mGameStateManager));

            // TODO: use this.Content to load your game content here
            SoundManager.Instance.Init(Content);
            SoundManager.Instance.PlayBackgroundMusic();

            // mWorld2 = new Grid(Content, Grid.LaneSide.Left, new Rectangle(0, 0, 20, 50));
            // mWorld2 = new Grid(Content, Grid.LaneSide.Left, new Rectangle(0, 0, 16, 42));
            // mWorld3 = new Grid(Content, Grid.LaneSide.Right, new Rectangle(30, 0, 20, 50));
            // mWorld3 = new Grid(Content, Grid.LaneSide.Right, new Rectangle(32, 0, 16, 42));
            
            /*mBoard = new Board(Content);

            // testing movable objects and collision
            mEntityGraph = new EntityGraph();
            mCollisionManager = new CollisionManager();
            Texture2D texture = new Texture2D(mGraphics.GraphicsDevice, 1, 1);
            texture.SetData(new[] { Color.Green });
            mUnit1 = new Unit(0, 0, 100, 100, texture);
            Texture2D texture2 = new Texture2D(mGraphics.GraphicsDevice, 1, 1);
            texture2.SetData(new[] { Color.Red });
            mUnit2 = new Unit(200, 200, 100, 100, texture2);
            mEntityGraph.Add(mUnit1);
            mEntityGraph.Add(mUnit2);
            mCollisionManager.CreatedObject(mUnit1);
            mCollisionManager.CreatedObject(mUnit2);
            // testing cooldown component
            mCoolDown = new CooldownComponent(new TimeSpan(0, 0, 5));
            mCoolDown.CooledDown += mUnit1.CooledDownDelegate;
            */
            // Testing Storage Manager
            StorageManager storageManager = new StorageManager();
            InGameState testSaveState = new InGameState(mGameStateManager);
            storageManager.SaveGame("testSave.xml", testSaveState);
            var testLoadState = storageManager.LoadGame("testSave.xml");
            
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
            // List <Point> path = testAStar.FindPath();
            // TODO: Add your update logic here
           /* if (mGameStateManager.Empty())
            {
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || InputManager.Default.KeyPressed(Keys.Escape))
                    mGameStateManager.Push(MenuState.CreateMainMenu(Exit, GraphicsDevice.Viewport.Bounds.Size, mGameStateManager));
                mCamera.Update();

                Console.WriteLine("fps: " + 1 / (float) gameTime.ElapsedGameTime.TotalSeconds);

                mEntityGraph.Update(mCamera.GetViewMatrix());
                mCollisionManager.Update();
                /* if (mStateList != null)
                 {
                     // Console.WriteLine(mStateList);
                 }// puts comment end here
            }
            else
            {
                mGameStateManager.Update(gameTime, false);
            }

            mCoolDown.Update(gameTime);
            */
            InputManager.Default.Update(gameTime);
            mGameStateManager.Update(gameTime, false);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.MintCream);
            mGameStateManager.Draw(mSpriteBatch, gameTime);
            /*if (!mGameStateManager.Empty())
            {
                mSpriteBatch.Begin(transformMatrix: mGameStateManager.Active.Camera?.GetViewMatrix());
                mGameStateManager.Draw(mSpriteBatch, gameTime);
                mSpriteBatch.End();
            }
            else
            {
                GraphicsDevice.Clear(Color.MintCream);

                // TODO: Add your drawing code here

                var viewMatrix = mCamera.GetViewMatrix();

                mSpriteBatch.Begin(transformMatrix: viewMatrix);

                // mWorld.Draw(mSpriteBatch, mCamera);
                // mWorld2.Draw(mSpriteBatch, mCamera.GetViewMatrix(), gameTime);
                // mWorld3.Draw(mSpriteBatch, mCamera.GetViewMatrix(), gameTime);
                
                mBoard.Draw(mSpriteBatch, viewMatrix, gameTime);
                
                mEntityGraph.Draw(mSpriteBatch);

                mSpriteBatch.End();

                //mStateManager.Draw(mSpriteBatch);
            }*/
            base.Draw(gameTime);
        }
    }
}
