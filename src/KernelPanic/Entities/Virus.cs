namespace KernelPanic.Entities
{
    internal sealed class Virus : Troupe
    {
        internal Virus(SpriteManager spriteManager)
            : base(3, 5, 10, 2, spriteManager.CreateVirus(), spriteManager)
        {
        }
    }
}