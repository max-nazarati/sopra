using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace KernelPanic
{
    class StartMenuState : State
    {
        private Texture2D MenuBackgroundTexture { get; }
        private readonly SpriteFont mTitleFont;
        private readonly List<Button> mButtonList = new List<Button>();
        //private ContentManager Content;
        private readonly GraphicsDeviceManager mGraphics;
        private int Width { get; }
        private int Height { get; }
        public StartMenuState(StateManager stateManager, GraphicsDeviceManager graphics, ContentManager contentManager) :base(stateManager, graphics, contentManager)
        {
            mContent = contentManager;
            mGraphics = graphics;
            MenuBackgroundTexture = new Texture2D(mGraphics.GraphicsDevice, 1, 1);
            MenuBackgroundTexture.SetData(new[] { Color.Black });

            var buttonFont = mContent.Load <SpriteFont>("ButtonFont");
            mTitleFont = mContent.Load<SpriteFont>("StartMenuTitle");
            Height = graphics.PreferredBackBufferHeight;
            Width = graphics.PreferredBackBufferWidth;
            var buttonBackground = new Texture2D(graphics.GraphicsDevice, 1, 1);
            buttonBackground.SetData(new[] { Color.DarkGray });
            mButtonList.Add(new Button(buttonFont, "PLAY", Width / 2, Height / 4, 200, Color.LightGray, Color.Black, graphics));
            mButtonList.Add(new Button(buttonFont, "OPTIONS", Width / 2, (int)(Height / 3.5), 200, Color.LightGray, Color.Black, graphics));
            mButtonList.Add(new Button(buttonFont, "QUIT", Width / 2, y: (int)(Height / 3.1), 200, Color.LightGray, Color.Black, graphics));
        }

        internal override void Update()
        {
            mOldKeyboardState = mKeyboardState;
            mOldMouseState = mMouseState;
            base.Update();

            if (mKeyboardState.IsKeyDown(Keys.Escape) && !mOldKeyboardState.IsKeyDown(Keys.Escape))
            {
                
            }
            foreach (Button btn in mButtonList)
            {
                if (btn.ContainsMouse(mMouseState))
                {
                    if (mMouseState.LeftButton == ButtonState.Pressed && mOldMouseState.LeftButton == ButtonState.Released)
                    {
                        if (btn.PText == "PLAY")
                        {
                            if (SManager.Count() == 1)
                            {
                                SManager.RemoveState();
                                SManager.AddState(new GameState(SManager, mGraphics, mContent));
                                //Game.SetGameState(new GameState(Game, _graphics, Content));
                            } else
                            {
                                SManager.RemoveState();
                            }
                        }
                        else if (btn.PText == "QUIT")
                        {
                            SManager.Game.Exit();
                        }
                    }
                }
            }
        }

        internal override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(MenuBackgroundTexture, new Rectangle(0, 0, Width, Height), Color.White);
            spriteBatch.DrawString(mTitleFont, "MAIN MENU", new Vector2((int)(Width / 2.5), y: (int)(Height / 6.0)), Color.Yellow);
            spriteBatch.End();
            foreach(Button btn in mButtonList)
            {
                btn.Draw(spriteBatch);
            }
        }
    }
}
