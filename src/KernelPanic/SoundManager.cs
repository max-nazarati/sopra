using System;
using System.Diagnostics;
using KernelPanic.Entities.Buildings;
using KernelPanic.Entities.Units;
using KernelPanic.Events;
using KernelPanic.Interface;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace KernelPanic
{
    internal sealed class SoundManager
    {
        private enum Sound
        {
            TowerPlacement,
            Shoot1,
            DiscShoot,
            CursorShoot,
            WifiShoot,
            AntivirusShoot,
            ElectroShock,
            BluescreenAbility,
            MoneyEarned,
            ButtonClick,
            SwooshFirefox
        }

        private enum Music
        {
            Soundtrack1,
            Soundtrack2
        }
        
        private readonly (Sound sound, SoundEffect soundEffect)[] mSounds;
        private readonly (Music music, Song song)[] mSongs;

        private float mVolume;
        private bool mPlaySounds;

        internal SoundManager(ContentManager contentManager)
        {
            mVolume = 0.8f;
            mPlaySounds = true;
            
            (Sound, SoundEffect) SoundEffect(Sound sound, string name) => (sound, contentManager.Load<SoundEffect>(name));
            (Music, Song) Song(Music music, string name) => (music, contentManager.Load<Song>(name));
            
            mSounds = new[]
            {
                SoundEffect(Sound.Shoot1, "sounds/shoot"),
                SoundEffect(Sound.TowerPlacement, "sounds/TowerPlacement"),
                SoundEffect(Sound.DiscShoot, "sounds/DiscShoot"),
                SoundEffect(Sound.CursorShoot, "sounds/CursorShoot"),
                SoundEffect(Sound.BluescreenAbility, "sounds/BluescreenAbility"),
                SoundEffect(Sound.WifiShoot, "sounds/WifiShoot"),
                SoundEffect(Sound.ElectroShock, "sounds/ElectroShock"),
                SoundEffect(Sound.AntivirusShoot, "sounds/AntivirusShoot"),
                SoundEffect(Sound.MoneyEarned, "sounds/MoneyEarned"),
                SoundEffect(Sound.ButtonClick, "sounds/ButtonClick"),
                SoundEffect(Sound.SwooshFirefox, "sounds/SwooshFirefox")
            };
            Array.Sort(mSounds);

            mSongs = new[]
            {
                Song(Music.Soundtrack1, "sounds/Soundtrack1"),
                Song(Music.Soundtrack2, "sounds/Soundtrack2")
            };
            Array.Sort(mSongs);
            
            var eventCenter = EventCenter.Default;
            eventCenter.Subscribe(Event.Id.BuildingPlaced, PlaySound);
            eventCenter.Subscribe(Event.Id.BuildingSold, PlaySound);
            eventCenter.Subscribe(Event.Id.ProjectileShot, PlaySound);
            eventCenter.Subscribe(Event.Id.ButtonClicked, PlaySound);
            eventCenter.Subscribe(Event.Id.FirefoxJump, PlaySound);
            eventCenter.Subscribe(Event.Id.PlayMusic, PlayMusic);
            eventCenter.Subscribe(Event.Id.PlaySounds, PlaySounds);
            eventCenter.Subscribe(Event.Id.ChangeSoundVolume, ChangeSoundVolume);
            eventCenter.Subscribe(Event.Id.HeroAbility, PlaySound);
        }
        
        private SoundEffect Lookup(Sound sound)
        {
            var (realSound, soundEffect) = mSounds[(int) sound];
            Trace.Assert(realSound == sound, $"{nameof(mSounds)} not ordered correctly");
            return soundEffect;
        }

        private Song Lookup(Music music)
        {
            var (realSong, song) = mSongs[(int) music];
            Trace.Assert(realSong == music, $"{nameof(mSongs)} not ordered correctly");
            return song;
        }

        /// <summary>
        /// Plays background music when called
        /// </summary>
        private void PlayMusic(Event e)
        {
            switch (e.Get<TextButton>(Event.Key.Button).Title)
            {
                case "an":
                    var values = Enum.GetValues(typeof(Music));
                    var random = new Random();
                    var randomBar = (Music)values.GetValue(random.Next(values.Length));
                    MediaPlayer.Play(Lookup(randomBar));
                    break;
                case "aus":
                    MediaPlayer.Stop();
                    break;
            }
        }

        internal void PlaySounds(Event e)
        {
            switch (e.Get<TextButton>(Event.Key.Button).Title)
            {
                case "an":
                    mPlaySounds = true;
                    break;
                case "aus":
                    mPlaySounds = false;
                    break;
            }
        }

        private void ChangeSoundVolume(Event e)
        {
            switch (mVolume)
            {
                case 0.8f:
                    mVolume = 1.4f;
                    MediaPlayer.Volume = mVolume;
                    break;
                case 1.4f:
                    mVolume = 0.4f;
                    MediaPlayer.Volume = mVolume;
                    break;
                case 0.4f:
                    mVolume = 0.8f;
                    MediaPlayer.Volume = mVolume;
                    break;
            }
        }

        private void PlaySound(Event e)
        {
            if (!mPlaySounds) return;
            switch (e.Kind)
            {
                case Event.Id.BuildingPlaced:
                    Lookup(Sound.TowerPlacement).Play(0.1f * mVolume,1,0);
                    break;
                case Event.Id.BuildingSold:
                    Lookup(Sound.MoneyEarned).Play(0.5f * mVolume,1,0);
                    break;
                case Event.Id.ProjectileShot:
                    switch (e.Get<Tower>(Event.Key.Tower))
                    {
                        case CdThrower _:
                            Lookup(Sound.DiscShoot).Play(0.3f * mVolume,0,0);
                            break;
                        case ShockField _:
                            Lookup(Sound.ElectroShock).Play(0.2f * mVolume,0,0);
                            break;
                        case CursorShooter _:
                            Lookup(Sound.CursorShoot).Play(0.1f * mVolume,0.3f,0);
                            break;
                        case Antivirus _:
                            Lookup(Sound.AntivirusShoot).Play(0.3f * mVolume,0,0);
                            break;
                        case WifiRouter _:
                            Lookup(Sound.WifiShoot).Play(0.3f * mVolume,0,0);
                            break;
                    }
                    break;
                case Event.Id.ButtonClicked:
                    Lookup(Sound.ButtonClick).Play(0.25f * mVolume,1,0);
                    break;
                case Event.Id.HeroAbility:
                    switch (e.Get<Hero>(Event.Key.Hero))
                    {
                        case Firefox _:
                            Lookup(Sound.SwooshFirefox).Play(0.3f * mVolume, 0, 0);
                            break;
                        case Bluescreen _:
                            Lookup(Sound.BluescreenAbility).Play(0.03f * mVolume, 0, 0);
                            break;
                    }

                    break;
            }
        }
    }
}