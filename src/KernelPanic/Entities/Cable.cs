using System.Diagnostics.CodeAnalysis;

namespace KernelPanic.Entities
{
    internal sealed class Cable : Building
    {
        // The parameters are required for Building.Create to work!
        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        public Cable(SpriteManager spriteManager, SoundManager soundManager)
            : base(10, spriteManager.CreateCable(), spriteManager)
        {
        }
    }
}
