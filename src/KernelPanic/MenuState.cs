using System;
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
        
        public static MenuState CreateMainMenu(Action exitAction, Point screenSize, GameStateManager stateManager)
        {
            var playButton = CreateButton(screenSize, stateManager.Sprite, "SPIELEN", 200);
            playButton.Clicked += _ => stateManager.Push(new InGameState(stateManager));

            var optionsButton = CreateButton(screenSize, stateManager.Sprite, "OPTIONEN", 325);
            optionsButton.Clicked += _ => stateManager.Push(CreateOptionsMenu(screenSize, stateManager));

            var quitButton = CreateButton(screenSize, stateManager.Sprite, "BEENDEN", 450);
            quitButton.Clicked += _ => exitAction();
            
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
            return new StaticComponent(sprites.CreateMenuBackground(screenSize));
        }

        private static Button CreateButton(Point screenSize, SpriteManager sprites, string title, int position)
        {
            var button = new Button(sprites) {Title = title};
            button.Sprite.X = screenSize.X / 2.0f - button.Sprite.Width / 2.0f;
            button.Sprite.Y = position;
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
            spriteBatch.Begin();
            foreach (var component in mComponents)
            {
                component.Draw(spriteBatch, gameTime);
            }
            spriteBatch.End();
        }
    }
}
