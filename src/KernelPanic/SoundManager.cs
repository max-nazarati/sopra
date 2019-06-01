using System;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace KernelPanic
{
    internal sealed class SoundManager
    {
        private SoundEffect mShoot, mPlacement;
        private Song mBackgroundSong1;

        private SoundManager()
        {
        }

        internal static SoundManager Instance { get; } = new SoundManager();

        /// <summary>
        /// loads content
        /// </summary>
        /// <param name="content">ContentManager object</param>
        public void Init(ContentManager content)
        {
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.2f;

            // Uncomment if these are used.
            mPlacement = content.Load<SoundEffect>("placement");
            mShoot = content.Load<SoundEffect>("shoot");
            mBackgroundSong1 = content.Load<Song>("testSoundtrack");
            // mBuildTower = content.Load<SoundEffect>("");
        }
#if true
        /// <summary>
        /// Plays background music when called
        /// </summary>
        internal void PlayBackgroundMusic()
        {
            MediaPlayer.Play(mBackgroundSong1);
        }
#endif
#if true // Currently not used.

        /// <summary>
        /// plays the sound according to the given string
        /// </summary>
        /// <param name="actionName">Sound to play. walk, shoot, buildTower</param>
        internal void PlaySound(string actionName)
        {
            switch (actionName)
            {
                case "placement":
                    mPlacement.Play(0.4f, 1f, 0.5f);
                    break;
                case "shoot":
                    mShoot.Play(0.4f, 1f, 1f);
                    break;
                default:
                    Console.WriteLine("No valid sound name. Try 'shoot' or 'placement' ");
                    break;
            }
        }
#endif
    }
}