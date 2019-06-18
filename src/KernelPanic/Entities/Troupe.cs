using System.Runtime.Serialization;
﻿using KernelPanic.Sprites;
using KernelPanic.Table;
using Microsoft.Xna.Framework;

namespace KernelPanic.Entities
{
    [DataContract]
    internal class Troupe : Unit
    {
        private Troupe(int price, int speed, int life, int attackStrength, Sprite sprite, SpriteManager spriteManager)
            : base(price, speed, life, attackStrength, sprite, spriteManager)
        {
        }

        /// <summary>
        /// Convenience function for creating a Troupe. The sprite is automatically scaled to the size of one tile.
        /// </summary>
        /// <param name="position">The point where to position this troupe.</param>
        /// <param name="sprite">The sprite to display.</param>
        /// <returns>A new Troupe</returns>
        private static Troupe Create(Point position, Sprite sprite, SpriteManager spriteManager)
        {
            sprite.Position = position.ToVector2();
            sprite.ScaleToWidth(Grid.KachelSize);
            return new Troupe(10, 1, 1, 1, sprite, spriteManager);
        }

        internal static Troupe CreateTrojan(Point position, SpriteManager spriteManager) =>
            Create(position, spriteManager.CreateTrojan(), spriteManager);
    }
}
