using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    [DataContract]
    internal sealed class MenuState : AGameState
    {
        private InterfaceComponent[] mComponents;
        
        private MenuState(GameStateManager gameStateManager) : base(gameStateManager)
        {
        }

        /*
        public static MenuState CreateAchievementsMenu(GameStateManager stateManager)
        {
            return new MenuState();
        }
        public static MenuState CreateCreditsMenu(GameStateManager stateManager)
        {
            return new MenuState();
        }
        public static MenuState CreateInstructionsMenu(GameStateManager stateManager)
        {
            return new MenuState();
        }
        public static MenuState CreatePauseMenu(GameStateManager stateManager, InGameState inGameState)
        {
            return new MenuState();
        }
        public static MenuState CreatePlayMenu(GameStateManager stateManager)
        {
            return new MenuState();
        }
        public static MenuState CreateStatisticsMenu(GameStateManager stateManager)
        {
            return new MenuState();
        }
        */
        
        public static MenuState CreateMainMenu(Point screenSize, GameStateManager stateManager)
        {
            var playButton = CreateButton(screenSize, stateManager.Sprite, "SPIELEN", 200);
            playButton.Clicked += _ => stateManager.Pop();  // TODO: Push `InGameState` when it exists.

            var optionsButton = CreateButton(screenSize, stateManager.Sprite, "OPTIONEN", 325);
            optionsButton.Clicked += _ => stateManager.Push(CreateOptionsMenu(screenSize, stateManager));

            var quitButton = CreateButton(screenSize, stateManager.Sprite, "BEENDEN", 450);
            quitButton.Clicked += _ => stateManager.Game.Exit();
            
            return new MenuState(stateManager)
            {
                mComponents = new InterfaceComponent[]
                {
                    CreateBackground(screenSize, stateManager.Sprite),
                    playButton,
                    optionsButton,
                    quitButton
                }
            };
        }

        private static MenuState CreateOptionsMenu(Point screenSize, GameStateManager stateManager)
        {
            var backButton = CreateButton(screenSize, stateManager.Sprite, "ZURÜCK", 450);
            backButton.Clicked += _ => stateManager.Pop();

            return new MenuState(stateManager)
            {
                mComponents = new InterfaceComponent[]
                {
                    CreateBackground(screenSize, stateManager.Sprite),
                    CreateButton(screenSize, stateManager.Sprite, "OPTION 1", 200),
                    CreateButton(screenSize, stateManager.Sprite, "OPTION 2", 325),
                    backButton
                }
            };
        }

        private static StaticComponent CreateBackground(Point screenSize, SpriteManager sprites)
        {
            var texture = sprites.LoadImage("Base");
            var fullRows = screenSize.Y / texture.Height;
            var fullCols = screenSize.X / texture.Width;
            var bottomRem = screenSize.Y - fullRows * texture.Height;
            var rightRem = screenSize.X - fullCols * texture.Width;
            
            var fullTile = new ImageSprite(texture, 0, 0);
            var pattern = new PatternSprite(fullTile, 0, 0, fullRows, fullCols);

            var bottomTile = new ImageSprite(texture, 0, 0)
            {
                SourceRectangle = new Rectangle(0, 0, texture.Width, bottomRem)
            };
            var rightTile = new ImageSprite(texture, 0, 0)
            {
                SourceRectangle = new Rectangle(0, 0, rightRem, texture.Height)
            };
            var cornerTile = new ImageSprite(texture, pattern.Width, pattern.Height)
            {
                SourceRectangle = new Rectangle(0, 0, rightRem, bottomRem)
            };

            return new StaticComponent(new CompositeSprite(0, 0)
            {
                Children =
                {
                    pattern,
                    new PatternSprite(bottomTile, 0, pattern.Height, 1, fullCols),
                    new PatternSprite(rightTile, pattern.Width, 0, fullRows, 1),
                    cornerTile
                }
            });
        }

        private static Button CreateButton(Point screenSize, SpriteManager sprites, string title, int position)
        {
            var button = new Button(title, 0, position, sprites);
            button.Sprite.X = screenSize.X / 2.0f - button.Sprite.Width / 2.0f;
            return button;
        }
        
        public override void Update(GameTime gameTime, bool isOverlay)
        {
            foreach(var component in mComponents)
            {
                component.Update(gameTime);
            }
        }
        
        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime, bool isOverlay)
        {
            foreach (var component in mComponents)
            {
                component.Draw(spriteBatch, gameTime);
            }
        }
    }
}
