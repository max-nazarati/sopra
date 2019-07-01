using KernelPanic.Sprites;

namespace KernelPanic.Entities
{
    internal sealed class Cable : Building
    {
        
        // todo: correct price
        // TODO LOAD CABLE INTO CONTENT
        /* public Cable(SpriteManager spriteManager, SoundManager soundManager) : base(1, spriteManager.CreateCable(), spriteManager)
        {
            //
        }
        */

        public Cable(int price, Sprite sprite, SpriteManager spriteManager) : base(price, sprite, spriteManager)
        {
        }
    }
}