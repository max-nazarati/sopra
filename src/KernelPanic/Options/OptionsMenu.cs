using System;
using KernelPanic.Input;
using KernelPanic.Interface;
using Microsoft.Xna.Framework;

namespace KernelPanic.Options
{
    internal sealed class OptionsMenu : MenuState
    {
        public OptionsMenu(GameStateManager gameStateManager) : base(gameStateManager)
        {
            Components = InterfaceComponents(gameStateManager);
        }

        private static InterfaceComponent[] InterfaceComponents(GameStateManager stateManager)
        {
            var settings = stateManager.Settings;

            var musicButton = CreateButton(stateManager.Sprite, "Hintergrundmusik", 200, 150);
            var musicOnOffButton = CreateButton(stateManager.Sprite, "aus", 200, -150);
            musicOnOffButton.Clicked += (button, input) =>
            {
                settings.PlayBackgroundMusic = !settings.PlayBackgroundMusic;
                musicOnOffButton.Title = settings.PlayBackgroundMusic ? "an" : "aus";
            };

            var effectsButton = CreateButton(stateManager.Sprite, "Soundeffekte", 325, 150);
            var effectsOnOffButton = CreateButton(stateManager.Sprite, "aus", 325, -150);
            effectsOnOffButton.Clicked += (button, input) =>
            {
                settings.PlaySoundEffects = !settings.PlaySoundEffects;
                effectsOnOffButton.Title = settings.PlaySoundEffects ? "an" : "aus";
            };

            var volumeButton = CreateButton(stateManager.Sprite, "Lautstärke", 450, 150);
            var volumeRegulatorButton = CreateButton(stateManager.Sprite, "Mittel", 450, -150);
            volumeRegulatorButton.Clicked += (button, input) => ChangeSoundVolume(volumeRegulatorButton, settings);

            var fullscreen = CreateButton(stateManager.Sprite, "Fullscreen", 700, 150);
            var fullScreenWindowButton = CreateButton(stateManager.Sprite, "aus", 700, -150);
            fullScreenWindowButton.Clicked += (button, input) =>
            {
                settings.IsFullscreen = !settings.IsFullscreen;
                fullScreenWindowButton.Title = settings.IsFullscreen ? "an" : "aus";
            };

            var keyInputsButton = CreateButton(stateManager.Sprite, "Steuerung", 575, 150);
            keyInputsButton.Clicked += (button, input) => stateManager.Push(new ControlsMenu(stateManager));

            var backButton = CreateButton(stateManager.Sprite, "Zurück", 800);
            backButton.Clicked += stateManager.PopOnClick;

            return new InterfaceComponent[]
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
            };
        }

        private static void ChangeSoundVolume(TextButton volumeButton, OptionsData settings)
        {
            switch (volumeButton.Title)
            {
                case "Mittel":
                    volumeButton.Title = "Hoch";
                    settings.MusicVolume = 1.0f;
                    break;
                case "Hoch":
                    volumeButton.Title = "Niedrig";
                    settings.MusicVolume = 0.2f;
                    break;
                case "Niedrig":
                    volumeButton.Title = "Mittel";
                    settings.MusicVolume = 0.6f;
                    break;
                default:
                    Console.WriteLine("No valid button title for VolumeButton.");
                    break;
            }
        }

        public override void Update(InputManager inputManager, GameTime gameTime)
        {
            base.Update(inputManager, gameTime);

            GameStateManager.SoundManager.Update(GameStateManager.Settings);
            if (GameStateManager.Settings.IsFullscreen != GameStateManager.GraphicsDeviceManager.IsFullScreen)
                GameStateManager.GraphicsDeviceManager.ToggleFullScreen();
        }
    }
}
