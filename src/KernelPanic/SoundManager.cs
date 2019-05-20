using System;
using System.Net.Mime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace KernelPanic
{
    internal sealed class SoundManager
    {
        private SoundEffect mWalking, mShooting, mBuildTower;
        private Song mBackgroundSong1;

        // singleton pattern
        private static readonly SoundManager sInstance = new SoundManager();

        static SoundManager()
        {
        }

        private SoundManager()
        {
        }

        internal static SoundManager Instance => sInstance;

        /// <summary>
        /// loads content
        /// </summary>
        /// <param name="content">ContentManager object</param>
        public void Init(ContentManager content)
        {
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.2f;
            // mWalking = content.Load<SoundEffect>("");
            // mShooting = content.Load<SoundEffect>("");
            // mBuildTower = content.Load<SoundEffect>("");
            mBackgroundSong1 = content.Load<Song>("testSoundtrack");
        }

        /// <summary>
        /// Plays background music when called
        /// </summary>
        internal void PlayBackgroundMusic()
        {
            MediaPlayer.Play(mBackgroundSong1);
        }
        /// <summary>
        /// plays the sound according to the given string
        /// </summary>
        /// <param name="actionName">Sound to play. walk, shoot, buildTower</param>
        internal void PlaySound(string actionName)
        {
            switch (actionName)
            {
                case "walking":
                    mWalking.Play(1f, 1f, 1f);
                    break;
                case "shoot":
                    mShooting.Play();
                    break;
                case "buildTower":
                    mBuildTower.Play();
                    break;
                default:
                    Console.WriteLine("No valid sound name. Try 'walk', 'shoot' or 'buildTower' ");
                    break;
            }
        }
    }
}