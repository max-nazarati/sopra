using System;
using KernelPanic.Sprites;
using KernelPanic.Table;


namespace KernelPanic.Entities
{
    internal sealed class Trojan : Troupe
    {
        private Action<Unit> mSpawnChildrenAction;

        internal Trojan(SpriteManager spriteManager)
            : base(20, 3, 500, 6, spriteManager.CreateTrojan(), spriteManager)
        {
        }
        public void Kill()
        {

        }
        public void WillSpawn(Action<Unit> unit)
        {
        }
    }
}
