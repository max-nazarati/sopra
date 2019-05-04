using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace ClickingExperiment
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public sealed class Game1 : Game
    {
        readonly GraphicsDeviceManager mGraphics;
        SpriteBatch mSpriteBatch;
        private Texture2D mBackground;
        private Texture2D mLogo;
        private Point mMouseGoalPoint = new Point(-1, -1);
        private Texture2D mSquareTexture2D;

        private List<Texture2D> mTexture2List = new List<Texture2D>();
        private IDictionary<Texture2D, Tuple<Rectangle, bool>> mTextureRectangleDictionary = new Dictionary<Texture2D, Tuple<Rectangle, bool>>();

        private MouseState mMouseState;
        private MouseState mOldMouseState;

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
            mBackground = Content.Load<Texture2D>("Background");

            mLogo = new Texture2D(mGraphics.GraphicsDevice, 1, 1);
            mLogo = Content.Load<Texture2D>("Unilogo");

            mSquareTexture2D = new Texture2D(mGraphics.GraphicsDevice, 1, 1);
            mSquareTexture2D.SetData(new[] { Color.Green });

            mTexture2List.Add(mLogo);
            mTextureRectangleDictionary.Add(mLogo, Tuple.Create(new Rectangle(new Point(100, 100), new Point(200, 200)), false));

            mTexture2List.Add(mSquareTexture2D);
            mTextureRectangleDictionary.Add(mSquareTexture2D, Tuple.Create(new Rectangle(new Point(400, 200), new Point(100, 100)), false));
        }


        private bool CheckClickedRectangle()
        {
            return (mTextureRectangleDictionary[mTexture2List[mTexture2List.Count - 1]].Item2);
        }
        private void SelectRectangle(Point clickPoint)
        {
            for (int i = mTexture2List.Count - 1; i > -1; i--)
            {
                if (mTextureRectangleDictionary[mTexture2List[i]].Item1.Contains(clickPoint) && 
                    !mTextureRectangleDictionary[mTexture2List[mTexture2List.Count - 1]].Item2)
                {
                    Texture2D tmp = mTexture2List[i];
                    mTexture2List.RemoveAt(i);
                    mTexture2List.Add(tmp);
                    mTextureRectangleDictionary[mTexture2List[mTexture2List.Count - 1]] = Tuple.Create(mTextureRectangleDictionary[tmp].Item1, true);
                }
                else
                {
                    mTextureRectangleDictionary[mTexture2List[i]] = Tuple.Create(mTextureRectangleDictionary[mTexture2List[i]].Item1, false);

                }
            }
        }

        private Point MoveToClick(Rectangle rectangle, Point target, int speed)
        {
            Vector2 direction = new Vector2(target.X - (rectangle.Width / 2) - rectangle.X,
                target.Y - (rectangle.Height / 2) - rectangle.Y);
            direction.Normalize();
            Vector2 normalizedDirection = direction;

            if (normalizedDirection.Length() >= 0.99)
            {
                rectangle.X += (int)(normalizedDirection.X * speed);
                rectangle.Y += (int)(normalizedDirection.Y * speed);
            }

            if (Math.Abs(rectangle.X + (rectangle.Width / 2) - target.X) <= 4 &&
                Math.Abs(rectangle.Y + (rectangle.Height / 2) - target.Y) <= 4)
            {
                mMouseGoalPoint = new Point(-1, -1);
            }

            return rectangle.Location;
        }

        private Point JumpToClick(Rectangle rectangle, MouseState mouseState)
        {
            return new Point(mouseState.X - rectangle.Width / 2, mouseState.Y - rectangle.Height / 2);
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

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                // cancels latest rectangle selection
                SelectRectangle(new Point(-1, -1));
                mMouseGoalPoint = new Point(-1, -1);
            }

            mMouseState = Mouse.GetState();
            if (mMouseState.LeftButton == ButtonState.Pressed && mOldMouseState.LeftButton == ButtonState.Released && !CheckClickedRectangle())
            {
                SelectRectangle(mMouseState.Position);
                System.Threading.Thread.Sleep(200);
            }
            else if (mMouseState.LeftButton == ButtonState.Pressed && CheckClickedRectangle())
            {
                mMouseGoalPoint = new Point(-1, -1);
                Tuple<Rectangle, bool> tmpTuple = mTextureRectangleDictionary[mTexture2List[mTexture2List.Count - 1]];
                Rectangle tmpRectangle = tmpTuple.Item1;

                tmpRectangle.Location = JumpToClick(tmpRectangle, mMouseState);
                mTextureRectangleDictionary[mTexture2List[mTexture2List.Count - 1]] =
                    Tuple.Create(tmpRectangle, true);
            }
            else if (mMouseState.RightButton == ButtonState.Pressed &&
                     mOldMouseState.RightButton == ButtonState.Released && CheckClickedRectangle())
            {
                mMouseGoalPoint = mMouseState.Position;
            }

            if (!mMouseGoalPoint.Equals(new Point(-1, -1)) && CheckClickedRectangle())
            {
                Tuple<Rectangle, bool> tmpTuple = mTextureRectangleDictionary[mTexture2List[mTexture2List.Count - 1]];
                Rectangle tmpRectangle = tmpTuple.Item1;

                tmpRectangle.Location = MoveToClick(tmpRectangle, mMouseGoalPoint, 5);
                mTextureRectangleDictionary[mTexture2List[mTexture2List.Count - 1]] =
                    Tuple.Create(tmpRectangle, true);
            }

            mOldMouseState = mMouseState;


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
            mSpriteBatch.Draw(mBackground, new Rectangle(0, 0, 800, 480), Color.White);
            foreach (Texture2D element in mTexture2List)
            {
                mSpriteBatch.Draw(element, mTextureRectangleDictionary[element].Item1, Color.White);
            }
            mSpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

