using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using KernelPanic.Camera;
using KernelPanic.Data;
using KernelPanic.Input;
using KernelPanic.Interface;
using KernelPanic.Serialization;
using KernelPanic.Sprites;
using KernelPanic.Tracking;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace KernelPanic
{
    [SuppressMessage("ReSharper", "StringLiteralTypo", Justification = "The Strings are in German")]
    internal sealed class MenuState : AGameState
    {
        private IReadOnlyCollection<InterfaceComponent> mComponents;
        private readonly Action mEscapeAction;

        private MenuState(GameStateManager gameStateManager, Action escapeAction = null)
            : base(new StaticCamera(), gameStateManager)
        {
            mEscapeAction = escapeAction;
        }

        internal static MenuState CreateMainMenu(GameStateManager stateManager, SoundManager soundManager)
        {
            var playButton = CreateButton(stateManager.Sprite, "Spielen", 100);
            playButton.Clicked += (button, input) => stateManager.Push(CreatePlayMenu(stateManager));
            
            var optionsButton = CreateButton(stateManager.Sprite, "Optionen", 200);
            optionsButton.Clicked += (button, input) =>
                stateManager.Push(CreateOptionsMenu(stateManager, soundManager));
            
            var instructionsButton = CreateButton(stateManager.Sprite, "Anleitung", 300);
            instructionsButton.Clicked += (button, input) => stateManager
                .Push(CreateInstructionsMenu(stateManager));
            
            var achievementsButton = CreateButton(stateManager.Sprite, "Achievements",400);
            achievementsButton.Clicked += (button, input) => stateManager
                .Push(CreateAchievementsMenu(stateManager));
            
            var statisticsButton = CreateButton(stateManager.Sprite, "Statistiken", 500);
            statisticsButton.Clicked += (button, input) => stateManager
                .Push(CreateStatisticsMenu(stateManager));

            var creditsButton = CreateButton(stateManager.Sprite, "Credits", 600);
            creditsButton.Clicked += (button, input) => stateManager
                .Push(CreateCreditsMenu(stateManager));
            
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

        private static MenuState CreatePlayMenu(GameStateManager stateManager, bool hasError = false)
        {
            var newGameButton = CreateButton(stateManager.Sprite, "Neues Spiel",600, 150);
            var loadGameButton = CreateButton(stateManager.Sprite, "Spiel laden", 600, -150);
            var backButton = CreateButton(stateManager.Sprite, "Zurück", 700);

            newGameButton.Enabled = false;
            loadGameButton.Enabled = false;
            
            var components = new List<InterfaceComponent>
            {
                CreateBackgroundWithoutText(stateManager.Sprite), newGameButton, loadGameButton, backButton
            };

            if (hasError)
            {
                var errorSprite = stateManager.Sprite.CreateText();
                errorSprite.Text = "Es ist ein Fehler aufgetreten, bitte versuche es erneut.";
                errorSprite.TextColor = Color.Red;
                errorSprite.SetOrigin(RelativePosition.CenterTop);
                errorSprite.X = stateManager.Sprite.ScreenSize.X / 2f;
                components.Add(new StaticComponent(errorSprite));
            }

            var positionY = 0;
            var selectedSlot = 0;
            Button selectedButton = null;

            foreach (var slot in StorageManager.Slots)
            {
                positionY += 100;

                var info = StorageManager.LoadInfo(slot, stateManager);
                var exists = info.HasValue;
                var button = CreateButton(stateManager.Sprite, info?.Label ?? "leer", positionY);

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

            newGameButton.Clicked += (button, input) => InGameState.PushGameStack(selectedSlot, stateManager);
            loadGameButton.Clicked += LoadGameCallback(() => selectedSlot, stateManager);
            backButton.Clicked += (button, input) => stateManager.Pop();

            return new MenuState(stateManager) { mComponents = components.ToArray() };
        }

        private static Button.Delegate LoadGameCallback(Func<int> slotAccessor, GameStateManager gameStateManager)
        {
            return (button, inputManager) =>
            {
                try
                {
                    InGameState.PushGameStack(slotAccessor(),
                        gameStateManager,
                        StorageManager.LoadGame(slotAccessor(), gameStateManager));
                }
                catch (Exception e)
                {
                    Console.WriteLine("Couldn't load slot {0}", slotAccessor());
                    Console.WriteLine(e);
                    gameStateManager.Switch(CreatePlayMenu(gameStateManager, true));
                }
            };
        }

        private static void TurnSoundsOnOff(TextButton soundOnOffButton, SoundManager soundManager)
        {
            switch (soundOnOffButton.Title)
            {
                case "an":
                    soundOnOffButton.Title = "aus"; 
                    soundManager.StopMusic();
                    break;
                case "aus":
                    soundOnOffButton.Title = "an";
                    soundManager.PlaySong(SoundManager.Music.BackgroundMusic1);
                    break;
                default:
                    Console.WriteLine("No valid button title for musicOnOffButton.");
                    break;
            }
        }
        
        private static void ChangeScreenSize(TextButton button, GraphicsDeviceManager graphics)
        {
            graphics.ToggleFullScreen();
            button.Title = graphics.IsFullScreen ? "an" : "aus";
        }

        private static MenuState CreateOptionsMenu(GameStateManager stateManager, SoundManager soundManager)
        {
            var musicButton = CreateButton(stateManager.Sprite, "Hintergrundmusik", 200, 150);
            var musicOnOffButton = CreateButton(stateManager.Sprite, "aus", 200, -150);
            musicOnOffButton.Clicked += (button, input) => TurnSoundsOnOff(musicOnOffButton, soundManager);
            
            var effectsButton = CreateButton(stateManager.Sprite, "Soundeffekte", 325, 150);
            var effectsOnOffButton = CreateButton(stateManager.Sprite, "aus", 325, -150);
            effectsOnOffButton.Clicked += (button, input) => TurnSoundsOnOff(effectsOnOffButton, soundManager);

            var volumeButton = CreateButton(stateManager.Sprite, "Lautstärke", 450, 150);
            var volumeRegulatorButton = CreateButton(stateManager.Sprite, "Mittel",450, -150);
            
            var fullscreen = CreateButton(stateManager.Sprite, "Fullscreen", 575, 150);
            var fullScreenWindowButton = CreateButton(stateManager.Sprite, "aus",575, -150);
            fullScreenWindowButton.Clicked += (button, input) => ChangeScreenSize(fullScreenWindowButton, stateManager.GraphicsDeviceManager);
            
            var backButton = CreateButton(stateManager.Sprite, "Zurück", 800);

            backButton.Clicked += (button, input) => stateManager.Pop();

            return new MenuState(stateManager)
            {
                mComponents = new InterfaceComponent[]
                {
                    CreateBackgroundWithoutText(stateManager.Sprite),
                    musicButton,
                    musicOnOffButton,
                    effectsButton,
                    effectsOnOffButton,
                    volumeButton,
                    volumeRegulatorButton,
                    backButton,
                    fullscreen,
                    fullScreenWindowButton
                }
            };
        }

        private static MenuState CreateInstructionsMenu(GameStateManager stateManager)
        {
            // TODO: Write Game Instructions.
            var backButton = CreateButton(stateManager.Sprite, "Zurück", 800);
            backButton.Clicked += (button, input) => stateManager.Pop();
            
            return new MenuState(stateManager)
            {
                mComponents = new InterfaceComponent[]
                {
                    CreateBackgroundWithoutText(stateManager.Sprite),
                    backButton
                }
            };
        }

        private static MenuState CreateStatisticsMenu(GameStateManager stateManager)
        {
            var backButton = CreateButton(stateManager.Sprite, "Zurück", 600);
            backButton.Clicked += (button, input) => stateManager.Pop();

            var resetButton = CreateButton(stateManager.Sprite, "Zurücksetzen", 800);
            resetButton.Clicked += (button, input) =>
            {
                // Reset the statistics and recreate the statistics menu.
                stateManager.Statistics.Reset();
                stateManager.Switch(CreateStatisticsMenu(stateManager));
            };

            var components = new List<InterfaceComponent>
            {
                CreateBackgroundWithoutText(stateManager.Sprite),
                backButton,
                resetButton
            };

            var y = 50f;
            var screenMid = stateManager.Sprite.ScreenSize.X / 2f;
            foreach (var (title, value) in stateManager.Statistics.UserRepresentation)
            {
                var titleText = stateManager.Sprite.CreateText(title);
                var valueText = stateManager.Sprite.CreateText(value);
                
                titleText.SetOrigin(RelativePosition.TopRight);
                valueText.SetOrigin(RelativePosition.TopLeft);

                // TODO: Better layout.
                titleText.X = screenMid - 10;
                valueText.X = screenMid + 10;
                titleText.Y = valueText.Y = y;
                y += titleText.Height + 20;

                components.Add(new StaticComponent(titleText));
                components.Add(new StaticComponent(valueText));
            }

            return new MenuState(stateManager) {mComponents = components};
        }

        private static MenuState CreateAchievementsMenu(GameStateManager stateManager)
        {
            const int hPadding = 10;
            const int vPadding = 8;

            var screenMid = stateManager.Sprite.ScreenSize.X / 2f;
            return CreatePaged(6, stateManager.AchievementPool.UserRepresentation, stateManager,
                element =>
                {
                    var titleText = stateManager.Sprite.CreateText(element.Title);
                    titleText.SetOrigin(RelativePosition.TopRight);
                    titleText.X = -hPadding;

                    var descText = stateManager.Sprite.CreateText(element.Description);
                    descText.SetOrigin(RelativePosition.TopRight);
                    descText.X = -hPadding;
                    descText.Y = titleText.Height + vPadding;

                    var valueText = stateManager.Sprite.CreateText(element.Value);
                    valueText.SetOrigin(RelativePosition.CenterLeft);
                    valueText.X = hPadding;
                    valueText.Y = (descText.Bounds.Bottom - titleText.Bounds.Top) / 2f;

                    return new StaticComponent(new CompositeSprite
                    {
                        Origin = new Vector2(-screenMid, 0),
                        Children = {titleText, descText, valueText}
                    });
                });
        }

        /// <summary>
        /// Creates a <see cref="MenuState"/> for an “Achievement unlocked” screen.
        /// </summary>
        /// <param name="achievement">The unlocked <see cref="Achievement"/>.</param>
        /// <param name="stateManager"></param>
        /// <returns>A new <see cref="MenuState"/>.</returns>
        internal static MenuState CreateAchievementDisplay(Achievement achievement, GameStateManager stateManager)
        {
            var screenMid = stateManager.Sprite.ScreenSize.X / 2f;
            var titleSprite = stateManager.Sprite.CreateText(achievement.Title());
            titleSprite.ScaleToHeight(100);
            titleSprite.SetOrigin(RelativePosition.CenterTop);
            titleSprite.X = screenMid;
            titleSprite.Y = 200;
            
            var descriptionSprite = stateManager.Sprite.CreateText(achievement.Description());
            descriptionSprite.SetOrigin(RelativePosition.CenterTop);
            descriptionSprite.X = screenMid;
            descriptionSprite.Y = 400;

            var backButton = CreateButton(stateManager.Sprite, "Ok", 800);
            backButton.Clicked += (button, input) => stateManager.Pop();

            return new MenuState(stateManager)
            {
                mComponents = new InterfaceComponent[]
                {
                    CreateBackgroundWithoutText(stateManager.Sprite),
                    new StaticComponent(titleSprite),
                    new StaticComponent(descriptionSprite),
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
                    CreateBackgroundWithoutText(stateManager.Sprite),
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

        public static MenuState CreateGameOverScreen(GameStateManager stateManager,
            Table.Board.GameState result,
            SoundManager soundManager)
        {
            var mainMenuButton = CreateButton(stateManager.Sprite, "Hauptmenü", 575);
            mainMenuButton.Clicked += (button, input) =>
                stateManager.Restart(CreateMainMenu(stateManager, soundManager));

            var text = stateManager.Sprite.CreateText(result == Table.Board.GameState.AWon
                ? "Du hast gewonnen!"
                : "Game over!");
            text.ScaleToWidth(400);
            text.Position = new Vector2(10, 10);

            return new MenuState(stateManager)
            {
                mComponents = new InterfaceComponent[]
                {
                    CreateBackgroundWithoutText(stateManager.Sprite),
                    mainMenuButton,
                    new StaticComponent(text)
                }
            };
        }

        public static MenuState CreatePauseMenu(GameStateManager stateManager,
            InGameState inGameState,
            SoundManager soundManager)
        {
            var backButton = CreateButton(stateManager.Sprite, "Weiter Spielen", 200);
            backButton.Clicked += (button, input) => stateManager.Pop();

            var optionsButton = CreateButton(stateManager.Sprite, "Optionen", 325);
            optionsButton.Clicked += (button, input) =>
                stateManager.Push(CreateOptionsMenu(stateManager, soundManager));

            var saveButton = CreateButton(stateManager.Sprite, "Speichern", 450);
            saveButton.Clicked += (button, input) => StorageManager.SaveGame(inGameState);

            var mainMenuButton = CreateButton(stateManager.Sprite, "Hauptmenü", 575);
            mainMenuButton.Clicked += (button, input) =>
                stateManager.Restart(CreateMainMenu(stateManager, soundManager));

            return new MenuState(stateManager)
            {
                mComponents = new InterfaceComponent[]
                {
                    CreateBackgroundWithoutText(stateManager.Sprite),
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

        private static StaticComponent CreateBackgroundWithoutText(SpriteManager sprites)
        {
            return new StaticComponent(sprites.CreateMenuBackgroundWithoutText());
        }

        private static MenuState CreatePaged<T>(int elementsPerPage,
            IReadOnlyList<T> elements,
            GameStateManager stateManager,
            Func<T, InterfaceComponent> viewFunc,
            int currentPage = 0)
        {
            const float padding = 50;        // Padding between elements.
            const float screenBorder = 50;   // Distance from the screen border.

            var spriteManager = stateManager.Sprite;
            var screenSize = spriteManager.ScreenSize;
            var screenMid = screenSize.X / 2.0f;
            var bottomRowY = screenSize.Y - screenBorder;

            var backButton = new TextButton(spriteManager)
            {
                Title = "Zurück",
                Sprite = {X = screenMid, Y = bottomRowY}
            };
            backButton.Sprite.SetOrigin(RelativePosition.CenterBottom);
            backButton.Clicked += stateManager.PopOnClick;
            var backButtonHalfWidth = backButton.Bounds.Width / 2.0f;

            var prevButton = new TextButton(spriteManager)
            {
                Title = "<",
                Enabled = currentPage > 0,
                Sprite = {X = screenMid - backButtonHalfWidth - padding, Y = bottomRowY}
            };
            prevButton.Sprite.SetOrigin(RelativePosition.BottomRight);
            prevButton.Clicked += delegate
            {
                stateManager.Switch(CreatePaged(elementsPerPage,
                    elements,
                    stateManager,
                    viewFunc,
                    currentPage - 1));
            };

            var nextButton = new TextButton(spriteManager)
            {
                Title = ">",
                Enabled = currentPage * elementsPerPage + elementsPerPage < elements.Count,
                Sprite = {X = screenMid + backButtonHalfWidth + padding, Y = bottomRowY}
            };
            nextButton.Sprite.SetOrigin(RelativePosition.BottomLeft);
            nextButton.Clicked += delegate
            {
                stateManager.Switch(CreatePaged(elementsPerPage,
                    elements,
                    stateManager,
                    viewFunc,
                    currentPage + 1));
            };

            var components = new List<InterfaceComponent>(elementsPerPage + 4)
            {
                CreateBackgroundWithoutText(spriteManager),
                backButton,
                prevButton,
                nextButton
            };

            var y = screenBorder;
            var indexRange = Enumerable.Range(currentPage * elementsPerPage,
                    Math.Min(elementsPerPage, elements.Count - currentPage * elementsPerPage));
            components.AddRange(indexRange.Select(index =>
            {
                var component = viewFunc(elements[index]);
                component.Position = new Vector2(component.Position.X, y);
                y += component.Bounds.Height + padding;
                return component;
            }));

            return new MenuState(stateManager) {mComponents = components};
        }

        private static TextButton CreateButton(SpriteManager sprites, string title, int positionY, int shiftPositionX = 0)
        {
            var button = new TextButton(sprites) {Title = title};
            button.Sprite.X = sprites.ScreenSize.X / 2.0f - button.Sprite.Width / 2.0f - shiftPositionX;
            button.Sprite.Y = positionY;
            
            return button;
        }

        public override void Update(InputManager inputManager, GameTime gameTime, SoundManager soundManager)
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
