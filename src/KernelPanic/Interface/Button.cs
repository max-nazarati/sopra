using Microsoft.Xna.Framework;

namespace KernelPanic
{
    public sealed class Button : InterfaceComponent
    {
        internal delegate void Delegate(Button sender);

        internal event Delegate Clicked;

        private readonly TextSprite mTitleSprite;

        internal override Sprite Sprite { get; }

        internal Button(SpriteManager sprites)
        {
            (Sprite, mTitleSprite) = sprites.CreateButton();
        }

        internal string Title
        {
            get => mTitleSprite.Text;
            set => mTitleSprite.Text = value;
        }

        internal override void Update(GameTime gameTime, InputManager inputManager)
        {
            if (Enabled && inputManager.MousePressed(InputManager.MouseButton.Left) && ContainsMouse(inputManager))
                Clicked?.Invoke(this);
        }
    }
}
