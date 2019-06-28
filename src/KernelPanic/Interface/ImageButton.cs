using System;
using System.Collections.Generic;
using KernelPanic.Input;
using KernelPanic.Sprites;

namespace KernelPanic.Interface
{
    internal sealed class ImageButton : Button
    {
        internal override Sprite Sprite { get; }

        internal ImageButton(SpriteManager sprites, ImageSprite sprite, int width, int height)
        {
            (Sprite, mBackground) = sprites.CreateImageButton(sprite, width, height);
        }
    }
}
