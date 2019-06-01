using System;
using Microsoft.Xna.Framework;

namespace KernelPanic
{
    public sealed class Button : InterfaceComponent
    {
        public delegate void Delegate(Button sender);
        public event Delegate Clicked;

        private readonly TextSprite mTitleSprite;

        public override Sprite Sprite { get; }

        internal Button(SpriteManager sprites)
        {
            (Sprite, mTitleSprite) = sprites.CreateButton();
        }

        internal string Title
        {
            get => mTitleSprite.Text;
            set => mTitleSprite.Text = value;
        }

        public override void Update(GameTime gameTime)
        {
            if (Enabled && ContainsMouse() && InputManager.Default.MousePressed(InputManager.MouseButton.Left))
                Clicked?.Invoke(this);
        }
    }
}
