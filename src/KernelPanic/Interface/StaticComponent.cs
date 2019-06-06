using Microsoft.Xna.Framework;

namespace KernelPanic
{
    internal sealed class StaticComponent : InterfaceComponent
    {
        internal override Sprite Sprite { get; }
        
        public StaticComponent(Sprite sprite)
        {
            Sprite = sprite;
        }

        internal override void Update(GameTime gameTime, InputManager inputManager)
        {
            // Nothing to update.
        }
    }
}
