using System;
using KernelPanic.Sprites;
using KernelPanic.Interface;
using KernelPanic.Input;
using KernelPanic.Players;
using KernelPanic.Waves;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic.Hud
{
    internal sealed class ScoreOverlay
    {
        private readonly PlayerIndexed<Player> mPlayers;
        private readonly PlayTime mPlayTime;

        private readonly Sprite mSprite;
        private readonly PlayerIndexed<TextSprite> mPowerTexts;
        private readonly PlayerIndexed<TextSprite> mMoneyTexts;
        private readonly PlayerIndexed<TextSprite> mExperienceTexts;
        private readonly TextSprite mTextTime;
        private readonly TextSprite mDefeatedWavesByHuman, mDefeatedWavesByComputer;
        private readonly WaveManager mWaveManager;
        private readonly ImageButton mPauseButton;

        internal bool Pause { get; set; }

        private const float Scale = 1.5f;
        private static Point PowerIndicatorSize => new Point((int)(60*Scale), (int)(28 *Scale));
        private static Point ClockSize => new Point((int)(80 * Scale), (int)(18 * Scale));

        public ScoreOverlay(WaveManager waveManager, SpriteManager spriteManager, TimeSpan time)
        {
            mPlayers = waveManager.Players;
            mPlayTime = new PlayTime(time);
            mWaveManager = waveManager;

            var sprites = spriteManager.CreateScoreDisplay();
            mSprite = sprites.Main;
            mPowerTexts = new PlayerIndexed<TextSprite>(sprites.Left, sprites.Right);
            mMoneyTexts = new PlayerIndexed<TextSprite>(sprites.LeftMoney, sprites.RightMoney);
            mExperienceTexts = new PlayerIndexed<TextSprite>(sprites.LeftEP, sprites.RightEP);
            mTextTime = sprites.Clock;
            mTextTime.Text = mPlayTime.Time;

            mDefeatedWavesByHuman = sprites.DefeatedWavesByHuman;
            mDefeatedWavesByHuman.Text = waveManager.mByHumanDefeatedWaves.ToString();
            
            mDefeatedWavesByComputer = sprites.DefeatedWavesByComputer;
            mDefeatedWavesByComputer.Text = waveManager.mByComputerDefeatedWaves.ToString();

            var sprite = spriteManager.CreatePause();
            mPauseButton = new ImageButton(spriteManager, sprite, 40, 40);
            mPauseButton.Sprite.X = spriteManager.ScreenSize.X-50-mPauseButton.Bounds.Width;
            mPauseButton.Sprite.Y = 50;
            Pause = false;
            mPauseButton.Clicked += (button, input) => Pause = true;
        }

        public TimeSpan Time => mPlayTime.Overall;

        public void Update(InputManager inputManager, GameTime gameTime)
        {
            mPowerTexts.A.Text = mPlayers.A.Base.Power + "%";
            mPowerTexts.B.Text = mPlayers.B.Base.Power + "%";
            mMoneyTexts.A.Text = mPlayers.A.Bitcoins + "";
            mMoneyTexts.B.Text = mPlayers.B.Bitcoins + "";
            mExperienceTexts.A.Text = mPlayers.A.ExperiencePoints + " EP";
            mExperienceTexts.B.Text = mPlayers.B.ExperiencePoints + " EP";

            mPlayTime.Update(gameTime);
            mTextTime.Text = mPlayTime.Time;

            mDefeatedWavesByHuman.Text = mWaveManager.mByHumanDefeatedWaves.ToString();
            mDefeatedWavesByComputer.Text = mWaveManager.mByComputerDefeatedWaves.ToString();

            mPauseButton.Update(inputManager, gameTime);
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            mSprite.Draw(spriteBatch, gameTime);
            mPauseButton.Draw(spriteBatch, gameTime);
        }
    }
}
