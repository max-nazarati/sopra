using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace KernelPanic
{
    internal sealed class SoundManager
    {
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

#if false // Currently not used.
        private SoundEffect mWalking, mShooting, mBuildTower;

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
#endif
    }
}