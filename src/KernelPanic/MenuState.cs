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

        private MenuState(GameStateManager gameStateManager) : base(new StaticCamera(), gameStateManager)
        {
        }

        public static MenuState CreateMainMenu(GameStateManager stateManager)
        {
            var playButton = CreateButton(stateManager.Sprite, "Spielen", 100);
            playButton.Clicked += _ => stateManager.Push(CreatePlayMenu(stateManager));
            
            var optionsButton = CreateButton( stateManager.Sprite, "Optionen", 200);
            optionsButton.Clicked += _ => stateManager.Push(CreateOptionsMenu(stateManager));
            
            var instructionsButton = CreateButton(stateManager.Sprite, "Anleitung", 300);
            instructionsButton.Clicked += _ => stateManager.Push(CreateInstructionsMenu(stateManager));
            
            var achievementsButton = CreateButton(stateManager.Sprite, "Achievements",400);
            achievementsButton.Clicked += _ => stateManager.Push(CreateAchievementsMenu(stateManager));
            
            var statisticsButton = CreateButton(stateManager.Sprite, "Statistiken", 500);
            statisticsButton.Clicked += _ => stateManager.Push(CreateStatisticsMenu(stateManager));

            var creditsButton = CreateButton(stateManager.Sprite, "Credits", 600);
            creditsButton.Clicked += _ => stateManager.Push(CreateCreditsMenu(stateManager));
            
            var quitButton = CreateButton( stateManager.Sprite, "Beenden", 700);
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
            var savedGame1 = CreateButton(stateManager.Sprite, "leer", 100);
            var savedGame2 = CreateButton(stateManager.Sprite, "leer", 200);
            var savedGame3 = CreateButton(stateManager.Sprite, "leer", 300);
            var savedGame4 = CreateButton(stateManager.Sprite, "leer", 400);
            var savedGame5 = CreateButton(stateManager.Sprite, "leer", 500);

            var newGameButton = CreateButton(stateManager.Sprite, "Neues Spiel",600, 150);
            newGameButton.Clicked += _ => InGameState.PushGameStack(stateManager);

            var loadGameButton = CreateButton(stateManager.Sprite, "Spiel laden", 600, -150);
            loadGameButton.Clicked += _ => stateManager.Push(new StorageManager().LoadGame("testSave.xml", stateManager));

            var backButton = CreateButton(stateManager.Sprite, "Zurück", 700);

            backButton.Clicked += _ => stateManager.Pop();

            return new MenuState(stateManager)
            {
                mComponents = new InterfaceComponent[]
                {
                    CreateBackground(stateManager.Sprite),
                    savedGame1,
                    savedGame2,
                    savedGame3,
                    savedGame4,
                    savedGame5,
                    newGameButton,
                    loadGameButton,
                    backButton
                }
            };
        }

        public static Button TurnSoundsOnOff(Button soundOnOffButton)
        {
            switch (soundOnOffButton.Title) // TODO: when SoundManager is updated: Interaction with SoundManager
            {
                case "an":
                    soundOnOffButton.Title = "aus";
                    break;
                case "aus":
                    soundOnOffButton.Title = "an";
                    SoundManager.Instance.PlayBackgroundMusic();
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
            var musicOnOffButton = CreateButton(stateManager.Sprite, "aus", 200, -150);
            musicOnOffButton.Clicked += _=> TurnSoundsOnOff(musicOnOffButton);
            
            var effectsButton = CreateButton(stateManager.Sprite, "Soundeffekte", 325, 150);
            var effectsOnOffButton = CreateButton(stateManager.Sprite, "aus", 325, -150);
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
            var janekButton = CreateButton(stateManager.Sprite, "Janek", 50);
            // janekButton.Clicked

            var johannesButton = CreateButton(stateManager.Sprite, "Johannes", 150);
            // johannesButton.Clicked

            var maxButton = CreateButton(stateManager.Sprite, "Max", 250);
            // maxButton.Clicked

            var zachariasButton = CreateButton(stateManager.Sprite, "Zacharias", 350);
            // zachariasButton.Clicked

            var melissaButton = CreateButton(stateManager.Sprite, "Melissa", 450);
            // melissaButton.Clicked

            var jensButton = CreateButton(stateManager.Sprite, "Jens", 550);
            // jensButton.Clicked

            var zoeButton = CreateButton( stateManager.Sprite, "Zoe", 650);
            // zoeButton.Clicked
            
            var backButton = CreateButton(stateManager.Sprite, "Zurück", 750);
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
           var backButton = CreateButton(stateManager.Sprite, "Weiter Spielen", 200);
           backButton.Clicked += _ => stateManager.Pop();

           var optionsButton = CreateButton(stateManager.Sprite, "Optionen", 325);
           optionsButton.Clicked += _ => stateManager.Push(CreateOptionsMenu(stateManager));

           var saveButton = CreateButton(stateManager.Sprite, "Speichern", 450);
           saveButton.Clicked += _ =>
           {
               
               new StorageManager().SaveGame("testSave.xml", inGameState);
               // TODO: change name on Button in CreatePlayMenu
           };

               var mainMenuButton = CreateButton(stateManager.Sprite, "Hauptmenü", 575);
           mainMenuButton.Clicked += _ => stateManager.Push(CreateMainMenu(stateManager));
           
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

        public override void Update(InputManager inputManager, GameTime gameTime)
        {
            foreach(var component in mComponents)
            {
                component.Update(gameTime, inputManager);
            }
        }
        
        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (var component in mComponents)
            {
                component.Draw(spriteBatch, gameTime);
            }
        }
    }
}
