namespace KernelPanic.Entities.Units
{
    internal sealed class Bug : Troupe
    {
        internal Bug(SpriteManager spriteManager)
            : base(2, 7, 4, 1, spriteManager.CreateBug(), spriteManager)
        {
        }
    }
}