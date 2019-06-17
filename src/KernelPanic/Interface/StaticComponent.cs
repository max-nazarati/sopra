using KernelPanic.Input;
using KernelPanic.Sprites;
using Microsoft.Xna.Framework;

namespace KernelPanic.Interface
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
