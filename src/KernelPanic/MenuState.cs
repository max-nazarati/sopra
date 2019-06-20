using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using KernelPanic.Camera;
using KernelPanic.Input;
using KernelPanic.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace KernelPanic
{
    [DataContract]
    internal sealed class MenuState : AGameState
    {
        private InterfaceComponent[] mComponents;
        private Action mEscapeAction;

        private MenuState(GameStateManager gameStateManager, Action escapeAction = null)
            : base(new StaticCamera(), gameStateManager)
        {
            mEscapeAction = escapeAction;
        }

        private static MenuState CreateMainMenu(GameStateManager stateManager)
        {
            var playButton = CreateButton(stateManager.Sprite, "Spielen", 100);
            playButton.Clicked += (button, input) => stateManager.Push(CreatePlayMenu(stateManager));
            
            var optionsButton = CreateButton( stateManager.Sprite, "Optionen", 200);
            optionsButton.Clicked += (button, input) => stateManager.Push(CreateOptionsMenu(stateManager));
            
            var instructionsButton = CreateButton(stateManager.Sprite, "Anleitung", 300);
            instructionsButton.Clicked += (button, input) => stateManager.Push(CreateInstructionsMenu(stateManager));
            
            var achievementsButton = CreateButton(stateManager.Sprite, "Achievements",400);
            achievementsButton.Clicked += (button, input) => stateManager.Push(CreateAchievementsMenu(stateManager));
            
            var statisticsButton = CreateButton(stateManager.Sprite, "Statistiken", 500);
            statisticsButton.Clicked += (button, input) => stateManager.Push(CreateStatisticsMenu(stateManager));

            var creditsButton = CreateButton(stateManager.Sprite, "Credits", 600);
            creditsButton.Clicked += (button, input) => stateManager.Push(CreateCreditsMenu(stateManager));
            
            var quitButton = CreateButton( stateManager.Sprite, "Beenden", 700);
            quitButton.Clicked += (button, input) => stateManager.ExitAction();

            return new MenuState(stateManager, stateManager.ExitAction)
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

        private static MenuState CreatePlayMenu(GameStateManager stateManager)
        {
            var newGameButton = CreateButton(stateManager.Sprite, "Neues Spiel",600, 150);
            var loadGameButton = CreateButton(stateManager.Sprite, "Spiel laden", 600, -150);
            var backButton = CreateButton(stateManager.Sprite, "Zurück", 700);

            newGameButton.Enabled = false;
            loadGameButton.Enabled = false;

            var components = new List<InterfaceComponent>
            {
                CreateBackground(stateManager.Sprite), newGameButton, loadGameButton, backButton
            };

            var positionY = 0;
            var selectedSlot = 0;
            Button selectedButton = null;

            foreach (var slot in StorageManager.Slots)
            {
                positionY += 100;

                var info = StorageManager.LoadStorageInfo(slot, stateManager);
                var button = CreateButton(stateManager.Sprite,
                    info?.Timestamp.ToString(CultureInfo.CurrentCulture) ?? "leer",
                    positionY);

                var exists = info.HasValue;
                button.Clicked += (btn, input) =>
                {
                    if (selectedButton is Button oldSelection)
                        oldSelection.Enabled = true;

                    btn.Enabled = false;
                    newGameButton.Enabled = true;
                    loadGameButton.Enabled = exists;
                    selectedButton = btn;
                    selectedSlot = slot;
                };

                components.Add(button);
            }

            newGameButton.Clicked += (button, input) => InGameState.PushGameStack(stateManager);
            loadGameButton.Clicked += (button, input) =>
                InGameState.PushGameStack(stateManager, StorageManager.LoadGame(selectedSlot, stateManager));
            backButton.Clicked += (button, input) => stateManager.Pop();

            return new MenuState(stateManager) { mComponents = components.ToArray() };
        }

        private static Button TurnSoundsOnOff(Button soundOnOffButton)
        {
            switch (soundOnOffButton.Title) // TODO: when SoundManager is updated: Interaction with SoundManager
            {
                case "an":
                    soundOnOffButton.Title = "aus";
                    // sounds.StopMusic();
                    break;
                case "aus":
                    soundOnOffButton.Title = "an";
                    // sounds.PlaySong(SoundManager.Music.BackgroundMusic1);
                    // TODO implement updated SoundManager
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
            musicOnOffButton.Clicked += (button, input) => TurnSoundsOnOff(musicOnOffButton);
            
            var effectsButton = CreateButton(stateManager.Sprite, "Soundeffekte", 325, 150);
            var effectsOnOffButton = CreateButton(stateManager.Sprite, "aus", 325, -150);
            effectsOnOffButton.Clicked += (button, input) => TurnSoundsOnOff(effectsOnOffButton);

            var volumeButton = CreateButton(stateManager.Sprite, "Lautstärke", 450, 150);
            var volumeRegulatorButton = CreateButton(stateManager.Sprite, "Mittel",450, -150);
            
            var backButton = CreateButton(stateManager.Sprite, "Zurück", 600);

            backButton.Clicked += (button, input) => stateManager.Pop();

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

        private static MenuState CreateInstructionsMenu(GameStateManager stateManager)
        {
            // TODO: Write Game Instructions.
            var backButton = CreateButton(stateManager.Sprite, "Zurück", 600);
            backButton.Clicked += (button, input) => stateManager.Pop();
            
            return new MenuState(stateManager)
            {
                mComponents = new InterfaceComponent[]
                {
                    CreateBackground(stateManager.Sprite),
                    backButton
                }
            };
        }

        private static MenuState CreateStatisticsMenu(GameStateManager stateManager)
        {
            // TODO: Collecting and processing game statistics. 
            var backButton = CreateButton(stateManager.Sprite, "Zurück", 600);
            backButton.Clicked += (button, input) => stateManager.Pop();
            
            return new MenuState(stateManager)
            {
                mComponents = new InterfaceComponent[]
                {
                    CreateBackground(stateManager.Sprite),
                    backButton
                }
            };   
        }

        private static MenuState CreateAchievementsMenu(GameStateManager stateManager)
        {
            // TODO: Create List with all Achievements with true/false flag.
            var backButton = CreateButton(stateManager.Sprite, "Zurück", 600);
            backButton.Clicked += (button, input) => stateManager.Pop();
            
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
        private static MenuState CreateCreditsMenu(GameStateManager stateManager)
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
            backButton.Clicked += (button, input) => stateManager.Pop();
            
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
           backButton.Clicked += (button, input) => stateManager.Pop();

           var optionsButton = CreateButton(stateManager.Sprite, "Optionen", 325);
           optionsButton.Clicked += (button, input) => stateManager.Push(CreateOptionsMenu(stateManager));

           var saveButton = CreateButton(stateManager.Sprite, "Speichern", 450);
           saveButton.Clicked += (button, input) => StorageManager.SaveGame(StorageManager.Debug.NextSaveSlot, inGameState);

           var mainMenuButton = CreateButton(stateManager.Sprite, "Hauptmenü", 575);
           mainMenuButton.Clicked += (button, input) => stateManager.Restart(CreateMainMenu(stateManager));
           
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
                component.Update(inputManager, gameTime);
            }

            if (!inputManager.KeyPressed(Keys.Escape))
                return;

            if (mEscapeAction != null)
                mEscapeAction();
            else
                GameStateManager.Pop();
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
