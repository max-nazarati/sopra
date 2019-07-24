using System;
using System.Diagnostics;
using KernelPanic.Entities.Buildings;
using KernelPanic.Entities.Units;
using KernelPanic.Events;
using KernelPanic.Options;
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
            SettingsAbility,
            MoneyEarned,
            ButtonClick,
            SwooshFirefox
        }

        private enum Music
        {
            Soundtrack1,
            Soundtrack2,
            SecretSong
        }
        
        private readonly (Sound sound, SoundEffect soundEffect)[] mSounds;
        private readonly (Music music, Song song)[] mSongs;

        private float mVolume;
        private bool mPlayMusic;
        private bool mPlaySounds;
        private Music mActiveSoundtrack = Music.Soundtrack2;

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
                SoundEffect(Sound.SettingsAbility, "sounds/SettingsAbility"),
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
                Song(Music.Soundtrack2, "sounds/Soundtrack2"),
                Song(Music.SecretSong, "sounds/CreditsSoundtrack")
            };
            Array.Sort(mSongs);
            
            var eventCenter = EventCenter.Default;
            eventCenter.Subscribe(Event.Id.BuildingPlaced, PlaySound);
            eventCenter.Subscribe(Event.Id.BuildingSold, PlaySound);
            eventCenter.Subscribe(Event.Id.ProjectileShot, PlaySound);
            eventCenter.Subscribe(Event.Id.ButtonClicked, PlaySound);
            eventCenter.Subscribe(Event.Id.FirefoxJump, PlaySound);
            eventCenter.Subscribe(Event.Id.HeroAbility, PlaySound);

            MediaPlayer.ActiveSongChanged += delegate
            {
                if (MediaPlayer.State == MediaState.Stopped)
                    ChangeSong();
            };
        }

        internal void PlaySecretSong(bool play = true)
        {
            if (play)
            {
                MediaPlayer.Play(Lookup(Music.SecretSong));
            }
            else if (mPlayMusic)
            {
                ChangeSong();
            }
            else
            {
                MediaPlayer.Stop();
            }
        }

        private void ChangeSong()
        {
            mActiveSoundtrack = mActiveSoundtrack == Music.Soundtrack1 ? Music.Soundtrack2 : Music.Soundtrack1;
            MediaPlayer.Play(Lookup(mActiveSoundtrack));
        }

        internal void Update(OptionsData settings)
        {
            MediaPlayer.Volume = settings.MusicVolume;
            if (settings.PlayBackgroundMusic)
            {
                mPlayMusic = true;
                if (MediaPlayer.State == MediaState.Stopped)
                    ChangeSong();
                else
                    MediaPlayer.Resume();
            }
            else
            {
                mPlayMusic = false;
                MediaPlayer.Pause();
            }

            mPlaySounds = settings.PlaySoundEffects;
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
                            Lookup(Sound.ElectroShock).Play(0.1f * mVolume,0,0);
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
                        case Settings _:
                            Lookup(Sound.SettingsAbility).Play(0.2f * mVolume, 0, 0);
                            break;
                    }
                    break;
            }
        }
    }
}