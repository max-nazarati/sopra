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

        public static MenuState CreateMainMenu(GameStateManager stateManager)
        {
            var playButton = CreateButton(stateManager.Sprite, "Spielen", 100);
            playButton.Clicked += _ => stateManager.Push(CreatePlayMenu( stateManager));


            var optionsButton = CreateButton( stateManager.Sprite, "Optionen", 225);
            optionsButton.Clicked += _ => stateManager.Push(CreateOptionsMenu( stateManager));
            
            var instructionsButton = CreateButton(stateManager.Sprite, "Anleitung", 350);
            instructionsButton.Clicked += _ => stateManager.Push(CreateInstructionsMenu( stateManager));
            
            var achievementsButton = CreateButton(stateManager.Sprite, "Achievements",475);
            achievementsButton.Clicked += _ => stateManager.Push(CreateAchievementsMenu( stateManager));
            
            var statisticsButton = CreateButton(stateManager.Sprite, "Statistiken", 600);
            statisticsButton.Clicked += _ => stateManager.Push(CreateStatisticsMenu(stateManager));

            var creditsButton = CreateButton(stateManager.Sprite, "Credits", 725);
            creditsButton.Clicked += _ => stateManager.Push(CreateCreditsMenu(stateManager));
            
            var quitButton = CreateButton( stateManager.Sprite, "Beenden", 850);
            quitButton.Clicked += _ => stateManager.ExitAction();

            return new MenuState(stateManager)
            {
                mComponents = new InterfaceComponent[]
                {
                    CreateBackground(stateManager.Sprite),
                    playButton,
                    optionsButton,
                    instructionsButton,
                    achievementsButton,
                    statisticsButton,
                    creditsButton,
                    quitButton
                }
            };
        }

        public static MenuState CreatePlayMenu(GameStateManager stateManager)
        {
            var newGameButton = CreateButton(stateManager.Sprite, "Neues Spiel",450);
            newGameButton.Clicked += _ => stateManager.Push(new InGameState(stateManager));

            var loadGameButton = CreateButton(stateManager.Sprite, "Spiel laden", 575);
            // TODO: Load XML Files from here.
            
            var backButton = CreateButton(stateManager.Sprite, "Zurück", 700);

            backButton.Clicked += _ => stateManager.Pop();

            return new MenuState(stateManager)
            {
                mComponents = new InterfaceComponent[]
                {
                    CreateBackground(stateManager.Sprite),
                    newGameButton,
                    loadGameButton,
                    backButton
                }
            };
        }

        public static Button TurnSoundsOnOff(Button soundOnOffButton)
        {
            switch (soundOnOffButton.Title) // TODO: Interact with SoundManager
            {
                case "an":
                    soundOnOffButton.Title = "aus";
                    break;
                case "aus":
                    soundOnOffButton.Title = "an";
                    break;
                default:
                    Console.WriteLine("No valid button title for musicOnOffButton.");
                    break;
            }

            return soundOnOffButton;
        }

        private static MenuState CreateOptionsMenu(GameStateManager stateManager)
        {
            var musicButton = CreateButton(stateManager.Sprite, "Hintergrundmusik", 200, 150);
            var musicOnOffButton = CreateButton(stateManager.Sprite, "an", 200, -150);
            musicOnOffButton.Clicked += _=> TurnSoundsOnOff(musicOnOffButton);
            
            var effectsButton = CreateButton(stateManager.Sprite, "Soundeffekte", 325, 150);
            var effectsOnOffButton = CreateButton(stateManager.Sprite, "an", 325, -150);
            effectsOnOffButton.Clicked += _ => TurnSoundsOnOff(effectsOnOffButton);

            var volumeButton = CreateButton(stateManager.Sprite, "Lautstärke", 450, 150);
            var volumeRegulatorButton = CreateButton(stateManager.Sprite, "Mittel",450, -150);
            
            var backButton = CreateButton(stateManager.Sprite, "Zurück", 600);

            backButton.Clicked += _ => stateManager.Pop();

            return new MenuState(stateManager)
            {
                mComponents = new InterfaceComponent[]
                {
                    CreateBackground(stateManager.Sprite),
                    musicButton,
                    musicOnOffButton,
                    effectsButton,
                    effectsOnOffButton,
                    volumeButton,
                    volumeRegulatorButton,
                    backButton
                }
            };
        }

        public static MenuState CreateInstructionsMenu(GameStateManager stateManager)
        {
            // TODO: Write Game Instructions.
            var backButton = CreateButton(stateManager.Sprite, "Zurück", 600);
            backButton.Clicked += _ => stateManager.Pop();
            
            return new MenuState(stateManager)
            {
                mComponents = new InterfaceComponent[]
                {
                    CreateBackground(stateManager.Sprite),
                    backButton
                }
            };
        }

        public static MenuState CreateStatisticsMenu(GameStateManager stateManager)
        {
            // TODO: Collecting and processing game statistics. 
            var backButton = CreateButton(stateManager.Sprite, "Zurück", 600);
            backButton.Clicked += _ => stateManager.Pop();
            
            return new MenuState(stateManager)
            {
                mComponents = new InterfaceComponent[]
                {
                    CreateBackground(stateManager.Sprite),
                    backButton
                }
            };   
        }

        public static MenuState CreateAchievementsMenu(GameStateManager stateManager)
        {
            // TODO: Create List with all Achievements with true/false flag.
            var backButton = CreateButton(stateManager.Sprite, "Zurück", 600);
            backButton.Clicked += _ => stateManager.Pop();
            
            return new MenuState(stateManager)
            {
                mComponents = new InterfaceComponent[]
                {
                    CreateBackground(stateManager.Sprite),
                    backButton
                }
            };
        } 
        
        /*
         * Connect current results of not yet integrated tasks for presentation
         * at sprint meeting with your Button.
         */
        public static MenuState CreateCreditsMenu(GameStateManager stateManager)
        {
            var janekButton = CreateButton(stateManager.Sprite, "Janek", 100);
            // janekButton.Clicked

            var johannesButton = CreateButton(stateManager.Sprite, "Johannes", 200);
            // johannesButton.Clicked

            var maxButton = CreateButton(stateManager.Sprite, "Max", 300);
            // maxButton.Clicked

            var zachariasButton = CreateButton(stateManager.Sprite, "Zacharias", 400);
            // zachariasButton.Clicked

            var melissaButton = CreateButton(stateManager.Sprite, "Melissa", 500);
            // melissaButton.Clicked

            var jensButton = CreateButton(stateManager.Sprite, "Jens", 600);
            // jensButton.Clicked

            var zoeButton = CreateButton( stateManager.Sprite, "Zoë", 700);
            // zoeButton.Clicked
            
            var backButton = CreateButton(stateManager.Sprite, "Zurück", 800);
            backButton.Clicked += _ => stateManager.Pop();
            
            return new MenuState(stateManager)
            {
                mComponents = new InterfaceComponent[]
                {
                    CreateBackground(stateManager.Sprite),
                    janekButton,
                    johannesButton,
                    maxButton,
                    zachariasButton,
                    melissaButton,
                    jensButton,
                    zoeButton,
                    backButton
                }
            };
        } 
        
       
       public static MenuState CreatePauseMenu(GameStateManager stateManager, InGameState inGameState)
       {
           var backButton = CreateButton(stateManager.Sprite, "WEITER SPIELEN", 450);
           backButton.Clicked += _ => stateManager.Pop();

           var optionsButton = CreateButton(stateManager.Sprite, "OPTIONEN", 325);
           optionsButton.Clicked += _ => CreateOptionsMenu(stateManager);

           var saveButton = CreateButton(stateManager.Sprite, "SPEICHERN", 450);

           var mainMenuButton = CreateButton(stateManager.Sprite, "HAUPTMENU", 575);
           // TODO: exitAction? mainMenuButton.Clicked += _ => CreateMainMenu(,screenSize, stateManager);
           
           return new MenuState(stateManager)
           {
               mComponents = new InterfaceComponent[]
               {
                   CreateBackground(stateManager.Sprite),
                   optionsButton,
                   saveButton,
                   mainMenuButton,
                   backButton
               }
           }; 
       }
       

        private static StaticComponent CreateBackground(SpriteManager sprites)
        {
            return new StaticComponent(sprites.CreateMenuBackground());
        }
        
        private static Button CreateButton(SpriteManager sprites, string title, int positionY, int shiftPositionX = 0)
        {
            // TODO: Change Text Color on Buttons
            var button = new Button(sprites) {Title = title};
            button.Sprite.X = sprites.ScreenSize.X / 2.0f - button.Sprite.Width / 2.0f - shiftPositionX;
            button.Sprite.Y = positionY;
            
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
