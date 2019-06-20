using KernelPanic.Input;
using KernelPanic.Sprites;
using Microsoft.Xna.Framework;

namespace KernelPanic.Interface
{
    internal class Button : InterfaceComponent
    {
        

        protected ImageSprite mBackground;

        internal override Sprite Sprite { get; }

        public override void Update(InputManager inputManager, GameTime gameTime)
        {
            
        }
    }
}
