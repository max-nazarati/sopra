using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace KernelPanic
{
    public class GameState : State
    {
        private readonly Texture2D _background;
        private readonly List<Unit> _mobileObjectsList = new List<Unit>();
        public GameState(Game1 game, GraphicsDeviceManager graphics, ContentManager content) : base(game, graphics, content)
        {
            var box1 = new Texture2D(graphics.GraphicsDevice, 1, 1);
            box1.SetData(new[] { Color.Green });
            _background = Content.Load<Texture2D>("GameBackground");
            Unit logo = new Unit(0, 0, 200, 200, Content.Load<Texture2D>("Unilogo"), Graphics);
            Unit box = new Unit(400, 400, 100, 100, box1, Graphics);
            _mobileObjectsList.Add(logo);
            _mobileObjectsList.Add(box);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if(KeyboardState.IsKeyDown(Keys.Escape) && OldKeyboardState.IsKeyUp(Keys.Escape))
            {
                Game.SetGameState(new StartMenuState(Game, Graphics, Content));
            }

            foreach (Unit obj in _mobileObjectsList)
            {
                obj.Update(gameTime);
            }
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(_background, Vector2.Zero, null, Color.White);
            spriteBatch.End();
            foreach (Unit obj in _mobileObjectsList)
            {
                obj.Draw(spriteBatch);
            }
        }
    }
}
