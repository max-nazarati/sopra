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
        private readonly SpriteFont _titleFont;
        private readonly List<Button> _buttonList = new List<Button>();
        //private ContentManager Content;
        private readonly GraphicsDeviceManager _graphics;
        private int Width { get; }
        private int Height { get; }
        public StartMenuState(StateManager stateManager, GraphicsDeviceManager graphics, ContentManager contentManager) :base(stateManager, graphics, contentManager)
        {
            Content = contentManager;
            _graphics = graphics;
            MenuBackgroundTexture = new Texture2D(_graphics.GraphicsDevice, 1, 1);
            MenuBackgroundTexture.SetData(new[] { Color.Black });

            var buttonFont = Content.Load <SpriteFont>("ButtonFont");
            _titleFont = Content.Load<SpriteFont>("StartMenuTitle");
            Height = graphics.PreferredBackBufferHeight;
            Width = graphics.PreferredBackBufferWidth;
            var buttonBackground = new Texture2D(graphics.GraphicsDevice, 1, 1);
            buttonBackground.SetData(new[] { Color.DarkGray });
            _buttonList.Add(new Button(buttonFont, "PLAY", Width / 2, Height / 4, 200, Color.LightGray, Color.Black, graphics));
            _buttonList.Add(new Button(buttonFont, "OPTIONS", Width / 2, (int)(Height / 3.5), 200, Color.LightGray, Color.Black, graphics));
            _buttonList.Add(new Button(buttonFont, "QUIT", Width / 2, y: (int)(Height / 3.1), 200, Color.LightGray, Color.Black, graphics));
        }

        public override void Update(GameTime gameTime)
        {
            OldKeyboardState = KeyboardState;
            OldMouseState = MouseState;
            base.Update(gameTime);

            if (KeyboardState.IsKeyDown(Keys.Escape) && !OldKeyboardState.IsKeyDown(Keys.Escape))
            {
                
            }
            foreach (Button btn in _buttonList)
            {
                if (btn.ContainsMouse(MouseState))
                {
                    if (MouseState.LeftButton == ButtonState.Pressed && OldMouseState.LeftButton == ButtonState.Released)
                    {
                        if (btn.PText == "PLAY")
                        {
                            if (SManager.Count() == 1)
                            {
                                SManager.RemoveState();
                                SManager.AddState(new GameState(SManager, _graphics, Content));
                                //Game.SetGameState(new GameState(Game, _graphics, Content));
                            } else
                            {
                                SManager.RemoveState();
                            }
                        }
                        else if (btn.PText == "QUIT")
                        {
                            SManager._game.Exit();
                        }
                    }
                }
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(MenuBackgroundTexture, new Rectangle(0, 0, Width, Height), Color.White);
            spriteBatch.DrawString(_titleFont, "MAIN MENU", new Vector2((int)(Width / 2.5), y: (int)(Height / 6.0)), Color.Yellow);
            spriteBatch.End();
            foreach(Button btn in _buttonList)
            {
                btn.Draw(gameTime, spriteBatch);
            }
        }
    }
}
