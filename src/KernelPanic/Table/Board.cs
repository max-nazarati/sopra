using System.Runtime.Serialization;
using KernelPanic.Data;
using KernelPanic.Input;
using KernelPanic.Players;
using KernelPanic.Sprites;
using KernelPanic.Upgrades;
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
        internal Player PlayerA { get; /* required for deserialization */ private set; }
        [JsonProperty]
        internal Player PlayerB { get; /* required for deserialization */ private set; }

        [JsonProperty]
        internal UpgradePool mUpgradePool;

        private readonly Sprite mBase;

        internal enum GameState
        {
            Playing,
            AWon,
            BWon
        }

        internal static Rectangle Bounds
        {
            get
            {
                var bounds = Rectangle.Union(Lane.LeftBounds, Lane.RightBounds);
                bounds.Inflate(100, 100);
                return bounds;
            }
        }

        #region Constructing a Board

        internal Board(SpriteManager content, SoundManager sounds, bool deserializing = false)
        {
            mBase = CreateBase(content);
            if (deserializing)
            {
                // If this is object is deserialized the other properties are set automatically later.
                return;
            }

            var leftLane = new Lane(Lane.Side.Left, content, sounds);
            var rightLane = new Lane(Lane.Side.Right, content, sounds);
            
            PlayerA = new Player(leftLane, rightLane);
            PlayerB = new Player(rightLane, leftLane, false);

            mUpgradePool = new UpgradePool(PlayerA, content);
            LayOutUpgradePool();
        }

        [OnDeserialized]
        private void AfterDeserialization(StreamingContext context)
        {
            LayOutUpgradePool();
        }

        private void LayOutUpgradePool()
        {
            var centerLeft = Lane.LeftBounds.At(RelativePosition.CenterRight);
            var centerRight = Lane.RightBounds.At(RelativePosition.CenterLeft);
            var middle = centerLeft + (centerRight - centerLeft) / 2;
            mUpgradePool.Position = middle - mUpgradePool.Size / 2;
        }

        private static Sprite CreateBase(SpriteManager spriteManager)
        {
            var position = Lane.LeftBounds.At(RelativePosition.TopRight);
            var size = new Vector2(Lane.RightBounds.Left - Lane.LeftBounds.Right, Lane.LeftBounds.Height);
            return spriteManager.CreateBases(position, size);
        }

        #endregion

        internal void Update(GameTime gameTime, InputManager inputManager)
        {
            PlayerA.AttackingLane.Update(gameTime, inputManager, new Owner(PlayerA, PlayerB));
            PlayerA.DefendingLane.Update(gameTime, inputManager, new Owner(PlayerB, PlayerA));
            PlayerA.Update(gameTime);
            PlayerB.Update(gameTime);
            mUpgradePool.Update(inputManager, gameTime);
        }

        internal GameState CheckGameState()
        {
            if (PlayerB.Base.Power <= 0)
            {
                return GameState.AWon;
            }
            if (PlayerA.Base.Power <= 0)
            {
                return GameState.BWon;
            }
            return GameState.Playing;
        }

        internal void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            LeftLane.Draw(spriteBatch, gameTime);
            RightLane.Draw(spriteBatch, gameTime);
            mUpgradePool.Draw(spriteBatch, gameTime);
            mBase.Draw(spriteBatch, gameTime);
        }
    }
}