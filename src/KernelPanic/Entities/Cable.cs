using KernelPanic.Sprites;

namespace KernelPanic.Entities
{
    internal sealed class Cable : Building
    {
        // todo: correct price
        public Cable(SpriteManager spriteManager, SoundManager soundManager) : base(1, spriteManager.CreateCable(),
            spriteManager)
        {
            
        } 
    }
}