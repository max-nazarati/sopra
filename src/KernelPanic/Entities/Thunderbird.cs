namespace KernelPanic.Entities
{
    internal sealed class Thunderbird : Troupe
    {
        internal Thunderbird(SpriteManager spriteManager)
            : base(15, 3, 15, 3, spriteManager.CreateThunderbird(), spriteManager)
        {
        }
    }
}