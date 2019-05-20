using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace KernelPanic
{
    internal sealed class GameState : State
    {
        //private readonly Texture2D _background;
        private readonly List<Unit> mMobileObjectsList = new List<Unit>();
        internal GameState(StateManager stateManager, GraphicsDeviceManager graphics, ContentManager content) : base(stateManager, graphics, content)
        {
            var box1 = new Texture2D(graphics.GraphicsDevice, 1, 1);
            box1.SetData(new[] { Color.Green });
            //_background = Content.Load<Texture2D>("GameBackground");
            Unit logo = new Unit(0, 0, 200, 200);
            Unit box = new Unit(400, 400, 100, 100);
            mMobileObjectsList.Add(logo);
            mMobileObjectsList.Add(box);
        }

        internal override void Update()
        {
            base.Update();

            if(mKeyboardState.IsKeyDown(Keys.Escape) && mOldKeyboardState.IsKeyUp(Keys.Escape))
            {
                mSManager.AddState(new StartMenuState(mSManager, mGraphics, mContent));
                //Game.SetGameState(new StartMenuState(Game, Graphics, Content));
            }

            foreach (Unit obj in mMobileObjectsList)
            {
                obj.Update();
            }
        }
        internal override void Draw(SpriteBatch spriteBatch)
        {
            //StateManager.Draw(gameTime, spriteBatch);
            /*spriteBatch.Begin();
            spriteBatch.Draw(_background, Vector2.Zero, null, Color.White);
            spriteBatch.End();
            
            foreach (Unit obj in mMobileObjectsList)
            {
                obj.Draw(spriteBatch);
            }
            */
        }
    }
}
