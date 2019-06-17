using KernelPanic.Input;
using KernelPanic.Sprites;
using Microsoft.Xna.Framework;

namespace KernelPanic.Interface
{
    internal sealed class Button : InterfaceComponent
    {
        internal delegate void Delegate(Button sender);

        internal event Delegate Clicked;

        private readonly ImageSprite mBackground;
        private readonly TextSprite mTitleSprite;

        internal override Sprite Sprite { get; }

        internal Button(SpriteManager sprites)
        {
            (Sprite, mBackground, mTitleSprite) = sprites.CreateButton();
        }

        internal string Title
        {
            get => mTitleSprite.Text;
            set => mTitleSprite.Text = value;
        }

        internal override bool Enabled
        {
            get => base.Enabled;
            set
            {
                mBackground.TintColor = value ? Color.White : Color.Gray;
                mTitleSprite.TextColor = value ? Color.Black : Color.DarkGray;
                base.Enabled = value;
            }
        }

        public override void Update(InputManager inputManager, GameTime gameTime)
        {
            inputManager.RegisterClickTarget(Sprite.Bounds, localInputManager =>
            {
                if (Enabled)
                    Clicked?.Invoke(this);
            });
        }
    }
}
