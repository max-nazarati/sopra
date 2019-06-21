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

        public override void Update(InputManager inputManager, GameTime gameTime)
        {
            // Nothing to update.
        }
    }
}
