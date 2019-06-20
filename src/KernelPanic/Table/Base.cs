using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace KernelPanic.Table
{
    internal sealed class Base
    {
        public int Power { get; set; }
        private Vector2 mPosition;

        public Base(Vector2 position = new Vector2())
        {
            Power = 50;
            mPosition = position;
        }

        // for testing purposes
        public List<Vector2> GetHitBox()
        {
            List<Vector2> hitBox = new List<Vector2>();
            hitBox.Add(Position);
            hitBox.Add(new Vector2(Position.X + 1, Position.Y));
            hitBox.Add(new Vector2(Position.X - 1, Position.Y));
            hitBox.Add(new Vector2(Position.X, Position.Y + 1));
            hitBox.Add(new Vector2(Position.X, Position.Y - 1));

            return hitBox;
        }

        public Vector2 Position { get => mPosition; set => mPosition = value;}
    }
}