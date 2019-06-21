using System.Runtime.Serialization;
using KernelPanic.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace KernelPanic.Table
{
    [DataContract]
    internal sealed class Board
    {
        [DataMember]
        internal Lane LeftLane { get; private set; }
        [DataMember]
        internal Lane RightLane { get; private set; }

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
            LeftLane = new Lane(Lane.Side.Left, content, sounds);
            RightLane = new Lane(Lane.Side.Right, content, sounds);
        }

        [JsonConstructor]
        internal Board(Lane leftLane, Lane rightLane)
        {
            LeftLane = leftLane;
            RightLane = rightLane;
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