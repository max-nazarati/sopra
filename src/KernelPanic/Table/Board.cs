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

        private Sprites.CompositeSprite mBase;

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

            var leftBase = content.CreateLeftBase();
            var rightBase =  content.CreateRightBase();
            leftBase.ScaleToWidth(Lane.RightBounds.Left - Lane.LeftBounds.Right);
            rightBase.ScaleToWidth(Lane.RightBounds.Left - Lane.LeftBounds.Right);
            leftBase.Position = new Vector2(0, 0);
            rightBase.Position = new Vector2(0, Lane.LeftBounds.Bottom);
            mBase = new Sprites.CompositeSprite
            {
                X = Lane.LeftBounds.Right,
                Children = { leftBase, rightBase}
            };

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
            mBase.Draw(spriteBatch, gameTime);
        }

        /*    
        public DrawMinimap(SpriteBatch spriteBatch, Rectangle rectangle)
        {
            
        }
        */
    }
}