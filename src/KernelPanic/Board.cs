using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    internal sealed class Board
    {
        internal Lane LeftLane { get; }
        internal Lane RightLane { get; }

        internal static Rectangle Bounds
        {
            get
            {
                var bounds = Rectangle.Union(Lane.LeftBounds, Lane.RightBounds);
                bounds.Inflate(100, 100);
                return bounds;
            }
        }

        internal Board(SpriteManager content)
        {
            LeftLane = new Lane(Grid.LaneSide.Left, content);
            RightLane = new Lane(Grid.LaneSide.Right, content);
        }

        internal void Update(GameTime gameTime, InputManager inputManager)
        {
            LeftLane.Update(gameTime, inputManager);
            RightLane.Update(gameTime, inputManager);
        }

        internal void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            LeftLane.Draw(spriteBatch, gameTime);
            RightLane.Draw(spriteBatch, gameTime);
        }

        /*    
        public DrawMinimap(SpriteBatch spriteBatch, Rectangle rectangle)
        {
            
        }
        */
    }
}