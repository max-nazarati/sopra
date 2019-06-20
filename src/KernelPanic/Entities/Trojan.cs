using System;
using KernelPanic.Sprites;
using KernelPanic.Table;
using Microsoft.Xna.Framework;


namespace KernelPanic.Entities
{
    internal sealed class Trojan : Troupe
    {
        private Action<Unit> mSpawnChildrenAction;

        internal Trojan(SpriteManager spriteManager)
            : base(20, 3, 30, 6, spriteManager.CreateTrojan(), spriteManager)
        {
        }
        public void Kill()
        {

        }
        public void WillSpawn(Action<Unit> unit)
        {
        }

        /// <summary>
        /// Convenience function for creating a Troupe. The sprite is automatically scaled to the size of one tile.
        /// </summary>
        /// <param name="position">The point where to position this troupe.</param>
        /// <param name="sprite">The sprite to display.</param>
        /// <returns>A new Troupe</returns>
        private static Trojan Create(Point position, SpriteManager spriteManager)
        {
            return new Trojan(spriteManager) {Sprite = {Position = position.ToVector2()}};
        }
    }
}
