using System;
using System.Diagnostics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace KernelPanic
{
    internal sealed class SoundManager
    {
        public enum Sound
        {
            TowerPlacement,
            Shoot1
        }

        public enum Music
        {
            BackgroundMusic1,
            // MenuMusic1
        }
        
        private readonly (Sound sound, SoundEffect soundEffect)[] mSounds;
        private readonly (Music music, Song song)[] mSongs;

        internal SoundManager(ContentManager contentManager)
        {
            (Sound, SoundEffect) SoundEffect(Sound sound, string name) => (sound, contentManager.Load<SoundEffect>(name));
            (Music, Song) Song(Music music, string name) => (music, contentManager.Load<Song>(name));
            
            mSounds = new[]
            {
                SoundEffect(Sound.Shoot1, "shoot"),
                SoundEffect(Sound.TowerPlacement, "placement"),
            };
            Array.Sort(mSounds);

            mSongs = new[]
            {
                Song(Music.BackgroundMusic1, "testSoundtrack"),
                // Song(Music.MenuMusic1, "placeholder")
            };
            Array.Sort(mSongs);
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
        internal void PlaySong(Music music)
        {
            MediaPlayer.Play(Lookup(music));
        }

        internal void StopMusic()
        {
            MediaPlayer.Stop();
        }

        /// <summary>
        /// plays the sound according to the given string
        /// <param name="sound"><see cref="Sound"/> to play.</param>
        /// </summary>
        public void PlaySound(Sound sound)
        {
            Lookup(sound).Play(0.4f, 1f, 1f);
        }
    }
}