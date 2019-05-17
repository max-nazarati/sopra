using System.Net.Mime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
            this.Content = content;
            this.mSongName = songName;
        }

        public void Init()
        {
            //mSong = Content.Load<Song>(mSongName);
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.2f;
        }

        public void Pause()
        {
            MediaPlayer.Pause();
        }

        public void Play()
        {
            //MediaPlayer.Play(mSong);
        }
    }
}