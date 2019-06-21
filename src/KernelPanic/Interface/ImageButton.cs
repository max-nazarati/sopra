using System;
using System.Collections.Generic;
using KernelPanic.Input;
using KernelPanic.Sprites;

namespace KernelPanic.Interface
{
    class ImageButton : Button
    {
        internal delegate void Delegate(ImageButton sender, InputManager inputManager);

        internal event Delegate Clicked;
        internal override Sprite Sprite { get; }
        internal ImageSprite mDisplayedImage;

        internal ImageButton(SpriteManager sprites, ImageSprite sprite, int width, int height)
        {
            (Sprite, mBackground, mDisplayedImage) = sprites.CreateImageButton(sprite, width, height);
        }
    }
}
