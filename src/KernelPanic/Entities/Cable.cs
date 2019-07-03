namespace KernelPanic.Entities
{
    internal sealed class Cable : Building
    {
        // ReSharper disable once UnusedParameter.Local – the parameters are required for Building.Create to work!
        public Cable(SpriteManager spriteManager, SoundManager soundManager)
            : base(10, spriteManager.CreateCable(), spriteManager)
        {
        }
    }
}
