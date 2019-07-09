using KernelPanic.Sprites;

namespace KernelPanic.Interface
{
    internal sealed class ImageButton : Button
    {
        internal override Sprite Sprite { get; }

        internal ImageButton(SpriteManager sprites, ImageSprite sprite, int width, int height)
        {
            (Sprite, Background) = sprites.CreateImageButton(sprite, width, height);
        }
    }
}
