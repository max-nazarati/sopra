using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace KernelPanic
{
    public class SoundManager
    {
        private Song mSong;
        private string mSongName;
        private ContentManager Content;
        public SoundManager(string songName, ContentManager content)
        {
            Content = content;
            mSongName = songName;
        }

        public void Init()
        {
            mSong = Content.Load<Song>(mSongName);
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.2f;
        }

        /*
        public void Pause()
        {
            MediaPlayer.Pause();
        }
        */

        public void Play()
        {
            MediaPlayer.Play(mSong);
        }
    }
}