using System;
using System.Linq;
using System.Linq.Expressions;
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

        public static MenuState CreateMainMenu(Action exitAction, Point screenSize, GameStateManager stateManager)
        {
            var playButton = CreateButton(screenSize, stateManager.Sprite, "Spielen", 100);
            playButton.Clicked += _ => stateManager.Push(CreatePlayMenu(screenSize, stateManager));


            var optionsButton = CreateButton(screenSize, stateManager.Sprite, "Optionen", 225);
            optionsButton.Clicked += _ => stateManager.Push(CreateOptionsMenu(screenSize, stateManager));
            
            var instructionsButton = CreateButton(screenSize, stateManager.Sprite, "Anleitung", 350);
            instructionsButton.Clicked += _ => stateManager.Push(CreateInstructionsMenu(screenSize, stateManager));
            
            var achievementsButton = CreateButton(screenSize, stateManager.Sprite, "Achievements", 475);
            achievementsButton.Clicked += _ => stateManager.Push(CreateAchievementsMenu(screenSize, stateManager));
            
            var statisticsButton = CreateButton(screenSize, stateManager.Sprite, "Statistiken", 600);
            statisticsButton.Clicked += _ => stateManager.Push(CreateStatisticsMenu(screenSize, stateManager));

            var creditsButton = CreateButton(screenSize, stateManager.Sprite, "Credits", 725);
            creditsButton.Clicked += _ => stateManager.Push(CreateCreditsMenu(screenSize, stateManager));
            
            var quitButton = CreateButton(screenSize, stateManager.Sprite, "Beenden", 850);
            quitButton.Clicked += _ => exitAction();
            
            return new MenuState(stateManager)
            {
                mComponents = new InterfaceComponent[]
                {
                    CreateBackground(screenSize, stateManager.Sprite),
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

        public static MenuState CreatePlayMenu(Point screenSize, GameStateManager stateManager)
        {
            var newGameButton = CreateButton(screenSize, stateManager.Sprite, "Neues Spiel",450);
            newGameButton.Clicked += _ => stateManager.Push(new InGameState(stateManager));

            var loadGameButton = CreateButton(screenSize, stateManager.Sprite, "Spiel laden", 575);
            // TODO: Load XML Files from here.
            
            var backButton = CreateButton(screenSize, stateManager.Sprite, "Zurück", 700);
            backButton.Clicked += _ => stateManager.Pop();

            return new MenuState(stateManager)
            {
                mComponents = new InterfaceComponent[]
                {
                    CreateBackground(screenSize, stateManager.Sprite),
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

        private static MenuState CreateOptionsMenu(Point screenSize, GameStateManager stateManager)
        {
            var musicButton = CreateButton(screenSize, stateManager.Sprite, "Hintergrundmusik", 200, 150);
            var musicOnOffButton = CreateButton(screenSize, stateManager.Sprite, "an", 200, -150);
            musicOnOffButton.Clicked += _=> TurnSoundsOnOff(musicOnOffButton);
            
            var effectsButton = CreateButton(screenSize, stateManager.Sprite, "Soundeffekte", 325, 150);
            var effectsOnOffButton = CreateButton(screenSize, stateManager.Sprite, "an", 325, -150);
            effectsOnOffButton.Clicked += _ => TurnSoundsOnOff(effectsOnOffButton);

            var volumeButton = CreateButton(screenSize, stateManager.Sprite, "Lautstärke", 450, 150);
            var volumeRegulatorButton = CreateButton(screenSize, stateManager.Sprite, "Mittel",450, -150);
            
            var backButton = CreateButton(screenSize, stateManager.Sprite, "Zurück", 600);
            backButton.Clicked += _ => stateManager.Pop();

            return new MenuState(stateManager)
            {
                mComponents = new InterfaceComponent[]
                {
                    CreateBackground(screenSize, stateManager.Sprite),
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

        public static MenuState CreateInstructionsMenu(Point screenSize, GameStateManager stateManager)
        {
            // TODO: Write Game Instructions.
            var backButton = CreateButton(screenSize, stateManager.Sprite, "Zurück", 600);
            backButton.Clicked += _ => stateManager.Pop();
            
            return new MenuState(stateManager)
            {
                mComponents = new InterfaceComponent[]
                {
                    CreateBackground(screenSize, stateManager.Sprite),
                    backButton
                }
            };
        }

        public static MenuState CreateStatisticsMenu(Point screenSize, GameStateManager stateManager)
        {
            // TODO: Collecting and processing game statistics. 
            var backButton = CreateButton(screenSize, stateManager.Sprite, "Zurück", 600);
            backButton.Clicked += _ => stateManager.Pop();
            
            return new MenuState(stateManager)
            {
                mComponents = new InterfaceComponent[]
                {
                    CreateBackground(screenSize, stateManager.Sprite),
                    backButton
                }
            };   
        }

        public static MenuState CreateAchievementsMenu(Point screenSize, GameStateManager stateManager)
        {
            // TODO: Create List with all Achievements with true/false flag.
            var backButton = CreateButton(screenSize, stateManager.Sprite, "Zurück", 600);
            backButton.Clicked += _ => stateManager.Pop();
            
            return new MenuState(stateManager)
            {
                mComponents = new InterfaceComponent[]
                {
                    CreateBackground(screenSize, stateManager.Sprite),
                    backButton
                }
            };
        } 
        
        /*
         * Connect current results of not yet integrated tasks for presentation
         * at sprint meeting with your Button.
         */
        public static MenuState CreateCreditsMenu(Point screenSize, GameStateManager stateManager)
        {
            var janekButton = CreateButton(screenSize, stateManager.Sprite, "Janek", 100);
            // janekButton.Clicked

            var johannesButton = CreateButton(screenSize, stateManager.Sprite, "Johannes", 200);
            // johannesButton.Clicked

            var maxButton = CreateButton(screenSize, stateManager.Sprite, "Max", 300);
            // maxButton.Clicked

            var zachariasButton = CreateButton(screenSize, stateManager.Sprite, "Zacharias", 400);
            // zachariasButton.Clicked

            var melissaButton = CreateButton(screenSize, stateManager.Sprite, "Melissa", 500);
            // melissaButton.Clicked

            var jensButton = CreateButton(screenSize, stateManager.Sprite, "Jens", 600);
            // jensButton.Clicked

            var zoeButton = CreateButton(screenSize, stateManager.Sprite, "Zoë", 700);
            // zoeButton.Clicked
            
            var backButton = CreateButton(screenSize, stateManager.Sprite, "Zurück", 800);
            backButton.Clicked += _ => stateManager.Pop();
            
            return new MenuState(stateManager)
            {
                mComponents = new InterfaceComponent[]
                {
                    CreateBackground(screenSize, stateManager.Sprite),
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
        
       
       public static MenuState CreatePauseMenu(Point screenSize, GameStateManager stateManager, InGameState inGameState)
       {
           var backButton = CreateButton(screenSize, stateManager.Sprite, "WEITER SPIELEN", 450);
           backButton.Clicked += _ => stateManager.Pop();

           var optionsButton = CreateButton(screenSize, stateManager.Sprite, "OPTIONEN", 325);
           optionsButton.Clicked += _ => CreateOptionsMenu(screenSize, stateManager);

           var saveButton = CreateButton(screenSize, stateManager.Sprite, "SPEICHERN", 450);

           var mainMenuButton = CreateButton(screenSize, stateManager.Sprite, "HAUPTMENU", 575);
           // TODO: exitAction? mainMenuButton.Clicked += _ => CreateMainMenu(,screenSize, stateManager);
           
           return new MenuState(stateManager)
           {
               mComponents = new InterfaceComponent[]
               {
                   CreateBackground(screenSize, stateManager.Sprite),
                   optionsButton,
                   saveButton,
                   mainMenuButton,
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

        private static Button CreateButton(Point screenSize, SpriteManager sprites, string title, int positionY, int shiftPositionX = 0)
        {
            // TODO: Change Text Color on Buttons
            var button = new Button(title, 0, positionY, sprites);
            button.Sprite.X = screenSize.X / 2.0f - button.Sprite.Width / 2.0f - shiftPositionX;
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
