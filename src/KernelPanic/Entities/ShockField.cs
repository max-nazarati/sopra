
using KernelPanic.Sprites;

namespace KernelPanic.Entities
{
    internal sealed class ShockField : Building
    {
        // todo: correct price
        public ShockField(SpriteManager spriteManager, SoundManager soundManager) : base(1, spriteManager.CreateShockField(), spriteManager)
        {
            // throw new System.NotImplementedException();
        }
    }
}
