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

        public Button(string title, float x, float y, SpriteManager sprites)
        {
            var texture = sprites.LoadImage("Papier");
            var background = new ImageSprite(texture, 0, 0)
            {
                DestinationRectangle = new Rectangle(0, 0, 250, 70)
            };
            mTitleSprite = new TextSprite(sprites.LoadFont("buttonFont"),
                title,
                background.Width / 2,
                background.Height / 2);
            mTitleSprite.Origin = new Vector2(mTitleSprite.Width / 2, mTitleSprite.Height / 2);

            var sprite = new CompositeSprite(x, y);
            sprite.Children.Add(background);
            sprite.Children.Add(mTitleSprite);
            Sprite = sprite;
        }

        public string Title
        {
            get => mTitleSprite.Text;
            set => mTitleSprite.Text = value ?? throw new ArgumentNullException();
        }

        public override void Update(GameTime gameTime)
        {
            if (Enabled && ContainsMouse() && InputManager.Default.MousePressed(InputManager.MouseButton.Left))
                Clicked?.Invoke(this);
        }
    }
}
