using KernelPanic.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace KernelPanic.Table
{
    internal sealed class Board
    {
        internal Lane LeftLane => PlayerA.DefendingLane;
        internal Lane RightLane => PlayerA.AttackingLane;

        [JsonProperty]
        internal Player PlayerA { get; }
        [JsonProperty]
        internal Player PlayerB { get; }

        internal static Rectangle Bounds
        {
            get
            {
                var bounds = Rectangle.Union(Lane.LeftBounds, Lane.RightBounds);
                bounds.Inflate(100, 100);
                return bounds;
            }
        }

        internal Board(SpriteManager content, SoundManager sounds)
        {
            var leftLane = new Lane(Lane.Side.Left, content, sounds);
            var rightLane = new Lane(Lane.Side.Right, content, sounds);
            
            PlayerA = new Player(leftLane, rightLane);
            PlayerB = new Player(rightLane, leftLane);
        }

        [JsonConstructor]
        private Board(Player playerA, Player playerB)
        {
            PlayerA = playerA;
            PlayerB = playerB;
        }

        internal void Update(GameTime gameTime, InputManager inputManager)
        {
            PlayerA.AttackingLane.Update(gameTime, inputManager, new Owner(PlayerA, PlayerB));
            PlayerA.DefendingLane.Update(gameTime, inputManager, new Owner(PlayerB, PlayerA));
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