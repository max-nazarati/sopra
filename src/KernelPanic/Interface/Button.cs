using KernelPanic.Input;
using KernelPanic.Sprites;
using Microsoft.Xna.Framework;

namespace KernelPanic.Interface
{
    internal class Button : InterfaceComponent
    {
        internal delegate void Delegate(Button sender, InputManager inputManager);

        internal event Delegate Clicked;
        protected ImageSprite mBackground;

        internal override Sprite Sprite { get; }

        public override void Update(InputManager inputManager, GameTime gameTime)
        {
            inputManager.RegisterClickTarget(Sprite.Bounds, localInputManager =>
            {
                if (Enabled)
                    Clicked?.Invoke(this, localInputManager);
            });
        }
    }

}
