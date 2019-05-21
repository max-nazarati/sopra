using System;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace TowerPlacementTest
{
    internal sealed class SoundManager
    {
        private SoundEffect mShoot, mPlacement;

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
            // mBuildTower = content.Load<SoundEffect>("");
        }
#if false
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
                    mPlacement.Play(0.5f, 1f, 0.5f);
                    break;
                case "shoot":
                    mShoot.Play();
                    break;
                default:
                    Console.WriteLine("No valid sound name. Try 'shoot' or 'placement' ");
                    break;
            }
        }
#endif
    }
}