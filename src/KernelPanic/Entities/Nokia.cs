namespace KernelPanic.Entities
{
    internal sealed class Nokia : Troupe
    {
        internal Nokia(SpriteManager spriteManager)
            : base(30, 2, 100, 15, spriteManager.CreateNokia(), spriteManager)
        {
        }
    }
}