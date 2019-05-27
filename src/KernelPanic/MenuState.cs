using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    [DataContract]
    sealed class MenuState : AGameState
    {
        private MenuState(GameStateManager mngr, bool isOverlay)
        {
            GameStateManager = mngr;
            IsOverlay = isOverlay;
        }

        private InterfaceComponent[] mComponents;
        /*
        private MenuState()
        {

        }
        
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
        public static MenuState CreateMainMenu(GameStateManager stateManager, bool isOverlay)
        {
            MenuState menu = new MenuState(stateManager, isOverlay) {mComponents = new InterfaceComponent[4]};
            menu.mComponents[0] = new InterfaceComponent
            {
                Sprite = new ImageSprite(stateManager.Sprite.LoadImage("Base"), 0, 0, 1920, 1080)
            };
            Button playBtn = new Button
            {
                Sprite = new CompositeSprite(stateManager.Sprite.LoadImage("Papier"),
                    stateManager.Sprite.LoadFont("ButtonFont"),
                    "PLAY",
                    800,
                    200,
                    200,
                    100)
            };
            playBtn.Clicked += () =>
            {
                if (InputManager.Default.MousePressed(InputManager.MouseButton.Left))
                {
                    if (playBtn.Sprite.Container.Contains(InputManager.Default.MousePosition))
                    {
                        menu.GameStateManager.Pop();
                    }
                }
            };
            menu.mComponents[1] = playBtn;

            Button optionsBtn = new Button
            {
                Sprite = new CompositeSprite(stateManager.Sprite.LoadImage("Papier"),
                    stateManager.Sprite.LoadFont("ButtonFont"),
                    "OPTIONS",
                    800,
                    325,
                    200,
                    100)
            };
            optionsBtn.Clicked += () =>
            {
                if (InputManager.Default.MousePressed(InputManager.MouseButton.Left))
                {
                    if (optionsBtn.Sprite.Container.Contains(InputManager.Default.MousePosition))
                    {
                        menu.GameStateManager.Push(CreateOptionsMenu(stateManager));
                    }
                }
            };
            menu.mComponents[2] = optionsBtn;

            Button quitBtn = new Button();
            quitBtn.Sprite = new CompositeSprite(stateManager.Sprite.LoadImage("Papier"),
                    stateManager.Sprite.LoadFont("ButtonFont"), "QUIT", 800, 450, 200, 100);
            quitBtn.Clicked += () =>
            {
                if (InputManager.Default.MousePressed(InputManager.MouseButton.Left))
                {
                    if (quitBtn.Sprite.Container.Contains(InputManager.Default.MousePosition))
                    {
                        menu.GameStateManager.Game.Exit();
                    }
                }
            };
            menu.mComponents[3] = quitBtn;
            return menu;
        }

        private static MenuState CreateOptionsMenu(GameStateManager stateManager)
        {
            MenuState menu = new MenuState(stateManager, false);

            menu.mComponents = new InterfaceComponent[4];
            menu.mComponents[0] = new InterfaceComponent();
            menu.mComponents[0].Sprite = new ImageSprite(stateManager.Sprite.LoadImage("Base"), 0, 0, 1920, 1080);
            Button x = new Button();
            x.Sprite = new CompositeSprite(stateManager.Sprite.LoadImage("Papier"),
                stateManager.Sprite.LoadFont("ButtonFont"), "OPTION 1", 800, 200, 200, 100);
            menu.mComponents[1] = x;

            menu.mComponents[2] = new Button();
            menu.mComponents[2].Sprite = new CompositeSprite(stateManager.Sprite.LoadImage("Papier"),
                stateManager.Sprite.LoadFont("ButtonFont"), "OPTION 2", 800, 325, 200, 100);

            Button btn3 = new Button();
            btn3.Sprite = new CompositeSprite(stateManager.Sprite.LoadImage("Papier"),
                stateManager.Sprite.LoadFont("ButtonFont"), "BACK", 800, 450, 200, 100);
            btn3.Clicked += () =>
            {
                if (InputManager.Default.MousePressed(InputManager.MouseButton.Left))
                {
                    if (btn3.Sprite.Container.Contains(InputManager.Default.MousePosition))
                    {
                        menu.GameStateManager.Pop();
                    }
                }
            };
            menu.mComponents[3] = btn3;

            return menu;
        }
       
        public override void Update(GameTime gameTime, bool isOverlay)
        {
            foreach(InterfaceComponent component in mComponents)
            {
                component.Update(gameTime);
            }
        }
        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime, bool isOverlay)
        {
            foreach (InterfaceComponent component in mComponents)
            {
                component.Sprite.Draw(spriteBatch, gameTime);
            }
        }
    }
}
