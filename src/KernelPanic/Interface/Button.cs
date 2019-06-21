using KernelPanic.Input;
using KernelPanic.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic.Interface
{
    internal abstract class Button : InterfaceComponent
    {
        internal delegate void Delegate(Button sender, InputManager inputManager);

        internal event Delegate Clicked;
        protected ImageSprite mBackground;
        private bool mMouseDown;

        protected bool ViewEnabled => !mMouseDown && Enabled;

        public override void Update(InputManager inputManager, GameTime gameTime)
        {
            // Because InputManager.MouseDown is non-claiming we can use it without further precautions.
            mMouseDown = inputManager.MouseDown(InputManager.MouseButton.Left) &&
                         Sprite.Bounds.Contains(inputManager.TranslatedMousePosition);

            inputManager.RegisterClickTarget(Sprite.Bounds, localInputManager =>
            {
                if (Enabled)
                    Clicked?.Invoke(this, localInputManager);
            });
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            mBackground.TintColor = ViewEnabled ? Color.White : Color.Gray;
            base.Draw(spriteBatch, gameTime);
        }
    }

}
