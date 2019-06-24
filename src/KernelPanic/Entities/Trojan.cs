using System;
using KernelPanic.Sprites;
using KernelPanic.Table;


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
            // TODO: Spawn children.
        }

        /// <inheritdoc />
        public override void WillSpawn(Action<Unit> spawnAction)
        {
            mSpawnChildrenAction = spawnAction;
        }
    }
}
