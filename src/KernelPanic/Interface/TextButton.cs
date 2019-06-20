using System;
using System.Collections.Generic;
using System.Linq;
using KernelPanic.Sprites;
using KernelPanic.Input;
using Microsoft.Xna.Framework;

namespace KernelPanic.Interface
{
    internal class TextButton : Button
    {
        internal delegate void Delegate(TextButton sender, InputManager inputManager);

        internal event Delegate Clicked;
        internal override Sprite Sprite { get; }
        private readonly TextSprite mTitleSprite;

        internal TextButton(SpriteManager sprites)
        {
            (Sprite, mBackground, mTitleSprite) = sprites.CreateTextButton();
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
                    Clicked?.Invoke(this, localInputManager);
            });
        }
    }
}
