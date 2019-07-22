using KernelPanic.Input;
using KernelPanic.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic.Interface
{
    internal abstract class Button : InterfaceComponent, IButtonLike
    {
        internal delegate void Delegate(Button sender, InputManager inputManager);

        internal event Delegate Clicked;
        protected ImageSprite mBackground;
        private bool mMouseDown;

        internal bool ViewPressed { get; set; }

        protected bool ViewEnabled => !ViewPressed && !mMouseDown && Enabled;

        public override void Update(InputManager inputManager, GameTime gameTime)
        {
            MouseOver = Sprite.Bounds.Contains(inputManager.TranslatedMousePosition);
            // Because InputManager.MouseDown is non-claiming we can use it without further precautions.
            mMouseDown = inputManager.MouseDown(InputManager.MouseButton.Left) &&
                         MouseOver;

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

        public void DrawTint(SpriteBatch spriteBatch, GameTime gameTime, Color tintColor)
        {
            mBackground.TintColor = ViewEnabled ? Color.White : tintColor;
            base.Draw(spriteBatch, gameTime);
        }

        Button IButtonLike.Button => this;

        public bool MouseOver { get; private set; }
    }

    internal interface IButtonLike : IUpdatable, IDrawable
    {
        Button Button { get; }
    }
}
