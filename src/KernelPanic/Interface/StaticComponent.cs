using Microsoft.Xna.Framework;

namespace KernelPanic
{
    internal sealed class StaticComponent : InterfaceComponent
    {
        public override Sprite Sprite { get; }
        
        public StaticComponent(Sprite sprite)
        {
            Sprite = sprite;
        }

        public override void Update(GameTime gameTime, Matrix invertedViewMatrix)
        {
            // Nothing to update.
        }
    }
}
