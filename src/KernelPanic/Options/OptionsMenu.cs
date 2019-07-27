using System;
using KernelPanic.Input;
using KernelPanic.Interface;
using Microsoft.Xna.Framework;

namespace KernelPanic.Options
{
    internal sealed class OptionsMenu : MenuState
    {
        public OptionsMenu(GameStateManager gameStateManager) : base(gameStateManager, CloseOptionsMenu(gameStateManager))
        {
            Components = InterfaceComponents(gameStateManager);
        }

        private static Action CloseOptionsMenu(GameStateManager gameStateManager)
        {
            return () =>
            {
                gameStateManager.Settings.Save();
                gameStateManager.Pop();
            };
        }

        private static InterfaceComponent[] InterfaceComponents(GameStateManager stateManager)
        {
            var settings = stateManager.Settings;

            var musicButton = CreateButton(stateManager.Sprite, "Hintergrundmusik", 100, 150);
            var musicOnOffButton = CreateButton(stateManager.Sprite, YesNoTitle(settings.PlayBackgroundMusic), 100, -150);
            musicOnOffButton.Clicked += (button, input) =>
            {
                settings.PlayBackgroundMusic = !settings.PlayBackgroundMusic;
                musicOnOffButton.Title = YesNoTitle(settings.PlayBackgroundMusic);
            };

            var effectsButton = CreateButton(stateManager.Sprite, "Soundeffekte", 210, 150);
            var effectsOnOffButton = CreateButton(stateManager.Sprite, YesNoTitle(settings.PlaySoundEffects), 210, -150);
            effectsOnOffButton.Clicked += (button, input) =>
            {
                settings.PlaySoundEffects = !settings.PlaySoundEffects;
                effectsOnOffButton.Title = YesNoTitle(settings.PlaySoundEffects);
            };

            var volumeButton = CreateButton(stateManager.Sprite, "Lautstärke", 320, 150);
            var volumeRegulatorButton = CreateButton(stateManager.Sprite, InitialSoundVolumeTitle(settings.MusicVolume), 320, -150);
            volumeRegulatorButton.Clicked += (button, input) => ChangeSoundVolume(volumeRegulatorButton, settings);

            var fullscreen = CreateButton(stateManager.Sprite, "Fullscreen", 430,150);
            var fullScreenWindowButton = CreateButton(stateManager.Sprite, YesNoTitle(settings.IsFullscreen), 430, -150);
            fullScreenWindowButton.Clicked += (button, input) =>
            {
                settings.IsFullscreen = !settings.IsFullscreen;
                fullScreenWindowButton.Title = YesNoTitle(settings.IsFullscreen);
            };

            var invertScroll = CreateButton(stateManager.Sprite, "Scroll invertieren", 540, 150);
            var invertScrollButton = CreateButton(stateManager.Sprite, YesNoTitle(settings.ScrollInverted), 540, -150);
            invertScrollButton.Clicked += (button, input) =>
            {
                settings.ScrollInverted = !settings.ScrollInverted;
                invertScrollButton.Title = YesNoTitle(settings.ScrollInverted);
            };

            var keyInputsButton = CreateButton(stateManager.Sprite, "Tastaturbelegung", 650);
            keyInputsButton.Clicked += (button, input) => stateManager.Push(new ControlsMenu(stateManager));

            var backButton = CreateButton(stateManager.Sprite, "Zurück", 800);
            var backAction = CloseOptionsMenu(stateManager);
            backButton.Clicked += delegate { backAction(); };

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
                fullScreenWindowButton,
                invertScroll,
                invertScrollButton
            };
        }

        private static string YesNoTitle(bool value) => value ? "an" : "aus";

        private static string InitialSoundVolumeTitle(float volume)
        {
            return volume < 0.4f ? "Leise" : volume > 0.6f ? "Laut" : "Mittel";
        }

        private static void ChangeSoundVolume(TextButton volumeButton, OptionsData settings)
        {
            switch (volumeButton.Title)
            {
                case "Mittel":
                    volumeButton.Title = "Laut";
                    settings.MusicVolume = 1.0f;
                    break;
                case "Laut":
                    volumeButton.Title = "Leise";
                    settings.MusicVolume = 0.1f;
                    break;
                case "Leise":
                    volumeButton.Title = "Mittel";
                    settings.MusicVolume = 0.5f;
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
