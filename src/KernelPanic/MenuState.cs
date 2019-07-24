using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using KernelPanic.Camera;
using KernelPanic.Data;
using KernelPanic.Events;
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

        internal static MenuState CreateMainMenu(GameStateManager stateManager, InputManager inputManager)
        {
            var playButton = CreateButton(stateManager.Sprite, "Spielen", 100);
            playButton.Clicked += (button, input) => stateManager.Push(CreatePlayMenu(stateManager));

            var techDemo = CreateButton(stateManager.Sprite, "Tech-Demo", 200);
            techDemo.Clicked += (button, input) => InGameState.PushTechDemo(stateManager);

            var optionsButton = CreateButton(stateManager.Sprite, "Optionen", 300);
            optionsButton.Clicked += (button, input) =>
                stateManager.Push(CreateOptionsMenu(stateManager, inputManager));
            
            var instructionsButton = CreateButton(stateManager.Sprite, "Anleitung", 400);
            instructionsButton.Clicked += (button, input) => stateManager
                .Push(CreateInstructionsMenu(stateManager));
            
            var achievementsButton = CreateButton(stateManager.Sprite, "Achievements",500);
            achievementsButton.Clicked += (button, input) => stateManager
                .Push(CreateAchievementsMenu(stateManager));
            
            var statisticsButton = CreateButton(stateManager.Sprite, "Statistiken", 600);
            statisticsButton.Clicked += (button, input) => stateManager
                .Push(CreateStatisticsMenu(stateManager));

            var creditsButton = CreateButton(stateManager.Sprite, "Credits", 700);
            creditsButton.Clicked += (button, input) => stateManager
                .Push(CreateCreditsMenu(stateManager));
            
            var quitButton = CreateButton( stateManager.Sprite, "Beenden", 800);
            quitButton.Clicked += (button, input) => stateManager.ExitAction();

            return new MenuState(stateManager, stateManager.ExitAction)
            {
                mComponents = new InterfaceComponent[]
                {
                    CreateBackground(stateManager.Sprite),
                    playButton,
                    techDemo,
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

                    // We abuse that the pressed state looks like the disabled state for still receiving click events
                    // but giving the player a visual indication.
                    loadGameButton.Enabled = true;
                    loadGameButton.ViewPressed = !exists;

                    selectedButton = btn;
                    selectedSlot = slot;
                };

                components.Add(button);
            }

            newGameButton.Clicked += (button, input) => InGameState.PushGameStack(selectedSlot, stateManager);
            loadGameButton.Clicked += LoadGameCallback(() => selectedSlot, stateManager);
            backButton.Clicked += stateManager.PopOnClick;

            return new MenuState(stateManager) { mComponents = components.ToArray() };
        }

        private static Button.Delegate LoadGameCallback(Func<int> slotAccessor, GameStateManager gameStateManager)
        {
            return (button, inputManager) =>
            {
                if (button.ViewPressed)
                {
                    EventCenter.Default.Send(Event.LoadEmptySlot());
                    return;
                }

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

        private static void TurnMusicOnOff(TextButton musicOnOffButton)
        {
            switch (musicOnOffButton.Title)
            {
                case "an":
                    musicOnOffButton.Title = "aus";
                    EventCenter.Default.Send(Event.PlayMusic(musicOnOffButton));
                    break;
                case "aus":
                    musicOnOffButton.Title = "an";
                    EventCenter.Default.Send(Event.PlayMusic(musicOnOffButton));
                    break;
                case "Zacharias":
                    EventCenter.Default.Send(Event.PlayMusic(musicOnOffButton));
                    break;
                default:
                    Console.WriteLine("No valid button title for musicOnOffButton.");
                    break;
            }
        }

        private static void TurnSoundsOnOff(TextButton soundOnOffButton)
        {
            switch (soundOnOffButton.Title)
            {
                case "an":
                    soundOnOffButton.Title = "aus";
                    EventCenter.Default.Send(Event.PlaySounds(soundOnOffButton));
                    break;
                case "aus":
                    soundOnOffButton.Title = "an";
                    EventCenter.Default.Send(Event.PlaySounds(soundOnOffButton));
                    break;
                default:
                    Console.WriteLine("No valid button title for musicOnOffButton.");
                    break;
            }
        }

        private static void ChangeSoundVolume(TextButton volumeButton)
        {
            switch (volumeButton.Title)
            {
                case "Mittel":
                    volumeButton.Title = "Hoch";
                    EventCenter.Default.Send(Event.ChangeSoundVolume());
                    break;
                case "Hoch":
                    volumeButton.Title = "Niedrig";
                    EventCenter.Default.Send(Event.ChangeSoundVolume());
                    break;
                case "Niedrig":
                    volumeButton.Title = "Mittel";
                    EventCenter.Default.Send(Event.ChangeSoundVolume());
                    break;
                default:
                    Console.WriteLine("No valid button title for VolumeButton.");
                    break;
            }
        }
        
        private static void ChangeScreenSize(TextButton button, GraphicsDeviceManager graphics)
        {
            graphics.ToggleFullScreen();
            button.Title = graphics.IsFullScreen ? "an" : "aus";
        }

        private static MenuState CreateOptionsMenu(GameStateManager stateManager, InputManager inputManager)
        {
            var musicButton = CreateButton(stateManager.Sprite, "Hintergrundmusik", 200, 150);
            var musicOnOffButton = CreateButton(stateManager.Sprite, "aus", 200, -150);
            musicOnOffButton.Clicked += (button, input) => TurnMusicOnOff(musicOnOffButton);

            var effectsButton = CreateButton(stateManager.Sprite, "Soundeffekte", 325, 150);
            var effectsOnOffButton = CreateButton(stateManager.Sprite, "aus", 325, -150);
            effectsOnOffButton.Clicked += (button, input) => TurnSoundsOnOff(effectsOnOffButton);

            var volumeButton = CreateButton(stateManager.Sprite, "Lautstärke", 450, 150);
            var volumeRegulatorButton = CreateButton(stateManager.Sprite, "Mittel",450, -150);
            volumeRegulatorButton.Clicked += (button, input) => ChangeSoundVolume(volumeRegulatorButton);
            
            var keyInputsButton = CreateButton(stateManager.Sprite, "Steuerung", 575, 150);
            keyInputsButton.Clicked += (button, input) => stateManager
                .Push(CreateKeyInputMenu(stateManager, inputManager));

            var fullscreen = CreateButton(stateManager.Sprite, "Fullscreen", 700, 150);
            var fullScreenWindowButton = CreateButton(stateManager.Sprite, "aus",700, -150);
            fullScreenWindowButton.Clicked += (button, input) => ChangeScreenSize(fullScreenWindowButton, stateManager.GraphicsDeviceManager);
            
            var backButton = CreateButton(stateManager.Sprite, "Zurück", 800);
            backButton.Clicked += stateManager.PopOnClick;

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
                    keyInputsButton,
                    backButton,
                    fullscreen,
                    fullScreenWindowButton
                }
            };
        }

        private static MenuState CreateKeyInputMenu(GameStateManager stateManager, InputManager inputManager)
        {
            var backButton = CreateButton(stateManager.Sprite, "Zurück", 800);
            backButton.Clicked += stateManager.PopOnClick;
            var title = stateManager.Sprite.CreateText("Tastaturbelegung");
            var screenMid = stateManager.Sprite.ScreenSize.X / 2f;
            title.X = screenMid + title.Width/2;
            title.Y = 100;
            title.SetOrigin(RelativePosition.TopRight);

            var placeTowerKey = CreateButton(stateManager.Sprite, inputManager.mInputState.mPlaceTower.ToString(), 200, 250);
            var placeTower = CreateButton(stateManager.Sprite, "Turm platzieren", 200,600);
            placeTowerKey.Clicked += (button, input) => inputManager.ChangeKey(InputManager.MacroKeys.TowerPlacement, (TextButton)button);

            var cameraUpKey = CreateButton(stateManager.Sprite, inputManager.mInputState.mCameraUp.ToString(), 300, 250);
            var cameraUp = CreateButton(stateManager.Sprite, "Kamera hoch", 300,600);
            cameraUpKey.Clicked += (button, input) => inputManager.ChangeKey(InputManager.MacroKeys.CameraUp, (TextButton)button);
            
            var cameraLeftKey = CreateButton(stateManager.Sprite, inputManager.mInputState.mCameraLeft.ToString(), 400, 250);
            var cameraLeft = CreateButton(stateManager.Sprite, "Kamera links", 400,600);
            cameraLeftKey.Clicked += (button, input) => inputManager.ChangeKey(InputManager.MacroKeys.CameraLeft, (TextButton)button);
            
            var cameraDownKey = CreateButton(stateManager.Sprite, inputManager.mInputState.mCameraDown.ToString(), 500, 250);
            var cameraDown = CreateButton(stateManager.Sprite, "Kamera runter", 500,600);
            cameraDownKey.Clicked += (button, input) => inputManager.ChangeKey(InputManager.MacroKeys.CameraDown, (TextButton)button);
            
            var cameraRightKey = CreateButton(stateManager.Sprite, inputManager.mInputState.mCameraRight.ToString(), 600, 250);
            var cameraRight = CreateButton(stateManager.Sprite, "Kamera rechts", 600,600);
            cameraRightKey.Clicked += (button, input) => inputManager.ChangeKey(InputManager.MacroKeys.CameraRight, (TextButton) button);
            
            var selectTower1Key = CreateButton(stateManager.Sprite, inputManager.mInputState.mTowerOne.ToString(), 700, 250);
            var selectTower1 = CreateButton(stateManager.Sprite, "Kabel", 700,600);
            selectTower1Key.Clicked += (button, input) => inputManager.ChangeKey(InputManager.MacroKeys.Tower1, (TextButton)button);
            
            var selectTower2Key = CreateButton(stateManager.Sprite, inputManager.mInputState.mTowerTwo.ToString(), 200, -600);
            var selectTower2 = CreateButton(stateManager.Sprite, "Mausschütze", 200,-250);
            selectTower2Key.Clicked += (button, input) => inputManager.ChangeKey(InputManager.MacroKeys.Tower2, (TextButton)button);
            
            var selectTower3Key = CreateButton(stateManager.Sprite, inputManager.mInputState.mTowerThree.ToString(), 300, -600);
            var selectTower3 = CreateButton(stateManager.Sprite, "Ventilator", 300,-250);
            selectTower3Key.Clicked += (button, input) => inputManager.ChangeKey(InputManager.MacroKeys.Tower3, (TextButton)button);
            
            var selectTower4Key = CreateButton(stateManager.Sprite, inputManager.mInputState.mTowerFour.ToString(), 400, -600);
            var selectTower4 = CreateButton(stateManager.Sprite, "Antivirus", 400,-250);
            selectTower4Key.Clicked += (button, input) => inputManager.ChangeKey(InputManager.MacroKeys.Tower4, (TextButton)button);
            
            var selectTower5Key = CreateButton(stateManager.Sprite, inputManager.mInputState.mTowerFive.ToString(), 500, -600);
            var selectTower5 = CreateButton(stateManager.Sprite, "Wifi-Router", 500,-250);
            selectTower5Key.Clicked += (button, input) => inputManager.ChangeKey(InputManager.MacroKeys.Tower5, (TextButton)button);
            
            var selectTower6Key = CreateButton(stateManager.Sprite, inputManager.mInputState.mTowerSix.ToString(), 600, -600);
            var selectTower6 = CreateButton(stateManager.Sprite, "Schockfeld", 600,-250);
            selectTower6Key.Clicked += (button, input) => inputManager.ChangeKey(InputManager.MacroKeys.Tower6, (TextButton)button);
            
            var selectTower7Key = CreateButton(stateManager.Sprite, inputManager.mInputState.mTowerSeven.ToString(), 700, -600);
            var selectTower7 = CreateButton(stateManager.Sprite, "CD-Thrower", 700,-250);
            selectTower7Key.Clicked += (button, input) => inputManager.ChangeKey(InputManager.MacroKeys.Tower7, (TextButton)button);
            
            return new MenuState(stateManager)
            {
                mComponents = new InterfaceComponent[]
                {
                    CreateBackgroundWithoutText(stateManager.Sprite),
                    new StaticComponent(title),
                    placeTower,
                    placeTowerKey,
                    cameraDown,
                    cameraDownKey,
                    cameraLeft,
                    cameraLeftKey,
                    cameraRight,
                    cameraRightKey,
                    cameraUp,
                    cameraUpKey,
                    backButton,
                    selectTower1,
                    selectTower1Key,
                    selectTower2,
                    selectTower2Key,
                    selectTower3,
                    selectTower3Key,
                    selectTower4,
                    selectTower4Key,
                    selectTower5,
                    selectTower5Key,
                    selectTower6,
                    selectTower6Key,
                    selectTower7,
                    selectTower7Key
                }
            };
        }

        private static MenuState CreateBuildingIntroPage(GameStateManager stateManager)
        {
            var backButton = CreateButton(stateManager.Sprite, "Zurück", 900);
            backButton.Clicked += stateManager.PopOnClick;
            var title = stateManager.Sprite.CreateText("Gebäudebeschreibung");
            var screenMid = stateManager.Sprite.ScreenSize.X / 2f;
            title.X = screenMid + title.Width / 2;
            title.Y = 50;
            title.SetOrigin(RelativePosition.TopRight);

            var description = stateManager.Sprite.CreateText("" +
                "Verteidige deine Basis mit:\n\n" +
                "    -Kabel: effektiv eine Mauer.\n" +
                "    -Mauszeigerschütze: der schwächste Turm der dir zur Verfügung steht.\n" +
                "    -Lüftung: macht gegnerische Einheiten in ihrer Umgebung langsamer.\n" +
                "    -Antivirusprogramm: ein stärkerer Turm.\n" +
                "    -Wifi-Router: schießt in Wellen statt mit Projektilien.\n" +
                "    -Schockfeld: verursacht Schaden an die Einheiten die darüber laufen.\n" +
                "    -CD-Werfer: ein Turm der CDs schießt, nach einem Upgrade können diese auch zurückkehrend Schaden verursachen.\n\n");

            description.X = 20;
            description.Y = 100;
            return new MenuState(stateManager)
            {
                mComponents = new InterfaceComponent[]
                {
                    CreateBackgroundWithoutText(stateManager.Sprite),
                    new StaticComponent(title),
                    new StaticComponent(description),
                    backButton
                }
            };
        }

        private static MenuState CreateControlsPage(GameStateManager stateManager)
        {
            var backButton = CreateButton(stateManager.Sprite, "Zurück", 900);
            backButton.Clicked += stateManager.PopOnClick;
            var title = stateManager.Sprite.CreateText("Steuerung");
            var screenMid = stateManager.Sprite.ScreenSize.X / 2f;
            title.X = screenMid + title.Width / 2;
            title.Y = 50;
            title.SetOrigin(RelativePosition.TopRight);

            //"      "
            var instructions = stateManager.Sprite.CreateText("" +
                "- Mauszeiger Bewegung / WASD:   Kamera-Bewegung.\n" +
                "- Mittlerer Maus-Click:                      Erster Click Hero-Fähigkeit togglen, zweiter Click Fähigkeit ausführen.\n" +
                "- Rechter Maus-Click:                      Bewegungsziel für den ausgewählten Hero angeben / un-togglet Hero-Fähigkeit.\n" +
                "- Linker Maus-Click:                         Objekte kaufen / Objekte (un-)auswählen / GUI Buttons drücken /\n" +
                "                                                                  Bau-Modus mit click auf die entsprechende Gebäude-Taste verlassen.\n" +
                "- Scroll-Wheel:                                 Kamera-Zoom.\n\n" +
                "- Esc-Taste:                      Bau-Modus verlassen / ins Pause Menu kommen oder Pause-Menu verlassen /\n" +
                "                                               ins vorherige Menu-Screen gelangen / aus Main-Menu das Spiel beenden.\n" +
                "- Q-Taste:                         Alternative zu mittlerem Maus-Click.\n" +
                "- E-Taste:                         Un-togglet Hero-Fähigkeit.\n" +
                "- Zahlen 1-7:                     Gebäude-Auswahl zum bauen.\n");


            instructions.Y = title.Y + 50;

            return new MenuState(stateManager)
            {
                mComponents = new InterfaceComponent[]
                {
                    CreateBackgroundWithoutText(stateManager.Sprite),
                    new StaticComponent(title),
                    new StaticComponent(instructions),
                    backButton
                }
            };
        }

        private static MenuState CreateUnitIntroPage(GameStateManager stateManager)
        {
            var backButton = CreateButton(stateManager.Sprite, "Zurück", 900);
            backButton.Clicked += stateManager.PopOnClick;
            var title = stateManager.Sprite.CreateText("Spielobjekt Beschreibung");
            var screenMid = stateManager.Sprite.ScreenSize.X / 2f;
            title.X = screenMid + title.Width / 2;
            title.Y = 50;
            title.SetOrigin(RelativePosition.TopRight);

            var description = stateManager.Sprite.CreateText("" +
                "Greife gegnerische Basis an mit:\n\n" +
                "    -Bug: schwach, aber schnell und billig.\n" +
                "    -Virus: stärker als der Bug aber immer noch nicht zu teuer.\n" +
                "    -Trojan: kann mehr Schaden ertragen, beim Tod spawnt er Bugs an seiner Position.\n" +
                "    -Thunderbird: fliegende Einheit die sich über Türme und Kabeln bewegen kann.\n" +
                "    -Nokia: sehr robuste Einheit die viel Schaden verursachen kann, wenn sie genug Zeite hatte um das Ziel zu erreichen.\n\n" +
                "Die obige Einheiten spawnen am Anfang der nächsten Welle, es gibt jedoch noch Helden,\n" +
                "die man selbst auf dem Spielfeld unabhängig von dem Wellen-Zycklus kaufen und bewegen kann:\n\n" +
                "    -Firefox: ein Held der über gewisse Abstände springen kann um einen kürzeren Weg zum Ziel zu finden.\n" +
                "    -Settings: ein Held der Einheiten in seiner Nähe heilen kann.\n" +
                "    -Bluescreen: ein Held der gegnerische Türme temporär ausschalten kann.\n\n");

            description.X = 20;
            description.Y = 100;
            return new MenuState(stateManager)
            {
                mComponents = new InterfaceComponent[]
                {
                    CreateBackgroundWithoutText(stateManager.Sprite),
                    new StaticComponent(title),
                    new StaticComponent(description),
                    backButton
                }
            };
        }

        private static MenuState CreateInstructionsMenu(GameStateManager stateManager)
        {
            var unitsButton = CreateButton(stateManager.Sprite, "Einheitenbeschreibung", 800, -350, 350);
            unitsButton.Clicked += (button, input) => stateManager.Push(CreateUnitIntroPage(stateManager));

            var buildingsButton = CreateButton(stateManager.Sprite, "Gebäudebeschreibung", 800, 350, 350);
            buildingsButton.Clicked += (button, input) => stateManager.Push(CreateBuildingIntroPage(stateManager));

            var controlsButton = CreateButton(stateManager.Sprite, "Steuerung", 800);
            controlsButton.Clicked += (button, input) => stateManager.Push(CreateControlsPage(stateManager));

            var backButton = CreateButton(stateManager.Sprite, "Zurück", 900);
            backButton.Clicked += stateManager.PopOnClick;
            var title = stateManager.Sprite.CreateText("Spielanleitung");
            var screenMid = stateManager.Sprite.ScreenSize.X / 2f;
            title.X = screenMid + title.Width / 2;
            title.Y = 50;
            title.SetOrigin(RelativePosition.TopRight);

            var description = stateManager.Sprite.CreateText("" +
                "Ziel: zerstöre die gegnerische Basis.\n\n" +
                "Um das zu machen bekommst du regelmäßig Bitcoins, womit du sowohl neue Einheiten und Türme kaufen\n" +
                "als auch Türme verbessern kannst.\n\n" +
                "Sachen kauft man aus den 2 Menus die man im Spiel sieht, Türme verbessert man einzeln indem man auf ein Turm\n" +
                "klickt und ein Update kauft. Bei den Türmen kann man so auch eine Strategie angeben.\n\n" +
                "Außerdem bekommt man Erfahrungspunkte (EP) am Ende jeder gegnerischen Welle. Damit kann man die Upgrades,\n" +
                "die in der Mitte des Spielfeldes sind, bekommen. Man muss aber schnell sein, denn sie mit dem Gegner geteilt sind\n" +
                "und nur einer von euch jedes von ihnen bekommen kann.\n");

            description.X = 20;
            description.Y = 100;
            return new MenuState(stateManager)
            {
                mComponents = new InterfaceComponent[]
                {
                    CreateBackgroundWithoutText(stateManager.Sprite),
                    new StaticComponent(title),
                    new StaticComponent(description),
                    backButton,
                    unitsButton,
                    buildingsButton,
                    controlsButton
                }
            };
        }

        private static MenuState CreateStatisticsMenu(GameStateManager stateManager)
        {
            var backButton = CreateButton(stateManager.Sprite, "Zurück", 600);
            backButton.Clicked += stateManager.PopOnClick;

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
            backButton.Clicked += stateManager.PopOnClick;

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
            // TODO: button clicked should call the event....
            // zachariasButton.Clicked += EventCenter.Default.Send(Event.PlayMusic(zachariasButton));
            // EventCenter.Default.Send(Event.PlayMusic(zachariasButton));
            zachariasButton.Clicked += (button, input) => TurnMusicOnOff(zachariasButton);


            // zachariasButton.Clicked

            var melissaButton = CreateButton(stateManager.Sprite, "Melissa", 450);
            // melissaButton.Clicked

            var jensButton = CreateButton(stateManager.Sprite, "Jens", 550);
            // jensButton.Clicked

            var zoeButton = CreateButton( stateManager.Sprite, "Zoe", 650);
            // zoeButton.Clicked
            
            var backButton = CreateButton(stateManager.Sprite, "Zurück", 750);
            backButton.Clicked += stateManager.PopOnClick;
            // TODO stop secret song
            
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
            Table.Board.GameState result, InputManager inputManager)
        {
            var mainMenuButton = CreateButton(stateManager.Sprite, "Hauptmenü", 575);
            mainMenuButton.Clicked += (button, input) =>
                stateManager.Restart(CreateMainMenu(stateManager, inputManager));

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
            InGameState inGameState, InputManager inputManager)
        {
            var backButton = CreateButton(stateManager.Sprite, "Weiter Spielen", 200);
            backButton.Clicked += stateManager.PopOnClick;

            var optionsButton = CreateButton(stateManager.Sprite, "Optionen", 325);
            optionsButton.Clicked += (button, input) =>
                stateManager.Push(CreateOptionsMenu(stateManager, inputManager));

            var saveButton = CreateButton(stateManager.Sprite, "Speichern", 450);
            saveButton.Clicked += (button, input) => StorageManager.SaveGame(inGameState);

            var mainMenuButton = CreateButton(stateManager.Sprite, "Hauptmenü", 575);
            mainMenuButton.Clicked += (button, input) =>
                stateManager.Restart(CreateMainMenu(stateManager, inputManager));

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

        private static TextButton CreateButton(SpriteManager sprites, string title, int positionY, int shiftPositionX = 0, int width = 250)
        {
            var button = new TextButton(sprites, width) {Title = title};
            button.Clicked += (button2, input) => EventCenter.Default.Send(Event.ButtonClicked());
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
