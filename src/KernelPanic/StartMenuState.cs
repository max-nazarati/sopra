﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace KernelPanic
{
    // OLD
    internal sealed class StartMenuState : GState
    {
        private Texture2D MenuBackgroundTexture { get; }
        private readonly SpriteFont mTitleFont;
        private readonly List<Button> mButtonList = new List<Button>();
        //private ContentManager Content;

        private int Width { get; }
        private int Height { get; }
        public StartMenuState(StateManager stateManager, GraphicsDeviceManager graphics, ContentManager contentManager) :base(stateManager, graphics, contentManager)
        {
            mContent = contentManager;
            MenuBackgroundTexture = new Texture2D(mGraphics.GraphicsDevice, 1, 1);
            MenuBackgroundTexture.SetData(new[] { Color.Black });

            var buttonFont = mContent.Load <SpriteFont>("ButtonFont");
            mTitleFont = mContent.Load<SpriteFont>("StartMenuTitle");
            Height = graphics.PreferredBackBufferHeight;
            Width = graphics.PreferredBackBufferWidth;
            var buttonBackground = new Texture2D(graphics.GraphicsDevice, 1, 1);
            buttonBackground.SetData(new[] { Color.DarkGray });
            mButtonList.Add(new Button(buttonFont, "PLAY", Width / 2, Height / 4, 200, Color.LightGray, Color.Black, graphics));
            //mButtonList.Add(new Button(buttonFont, "OPTIONS", Width / 2, (int)(Height / 3.5), 200, Color.LightGray, Color.Black, graphics));
            mButtonList.Add(new Button(buttonFont, "QUIT", Width / 2, (int)(Height / 3.1), 200, Color.LightGray, Color.Black, graphics));
        }

        internal override void Update()
        {
            mOldKeyboardState = mKeyboardState;
            mOldMouseState = mMouseState;
            base.Update();

            if (mKeyboardState.IsKeyDown(Keys.Escape) && !mOldKeyboardState.IsKeyDown(Keys.Escape))
            {
                
            }
            foreach (var btn in mButtonList)
            {
                if (!btn.ContainsMouse(mMouseState)) continue;
                if (mMouseState.LeftButton != ButtonState.Pressed ||
                    mOldMouseState.LeftButton != ButtonState.Released) continue;
                
                if (btn.Text == "PLAY")
                {
                    if (mSManager.Count() == 1)
                    {
                        mSManager.RemoveState();
                        mSManager.AddState(new GameState(mSManager, mGraphics, mContent));
                        //Game.SetGameState(new GameState(Game, _graphics, Content));
                    } else
                    {
                        mSManager.RemoveState();
                    }
                }
                else if (btn.Text == "QUIT")
                {
                    mSManager.Game.Exit();
                }
            }
        }

        internal override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(MenuBackgroundTexture, new Rectangle(0, 0, Width, Height), Color.White);
            spriteBatch.DrawString(mTitleFont, "MAIN MENU", new Vector2((int)(Width / 2.5), (int)(Height / 6.0)), Color.Yellow);
            spriteBatch.End();
            foreach(var btn in mButtonList)
            {
                btn.Draw(spriteBatch);
            }
        }
    }
}
