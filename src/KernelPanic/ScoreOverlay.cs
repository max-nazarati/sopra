using System;
using KernelPanic.Sprites;
using KernelPanic.Interface;
using KernelPanic.Input;
using KernelPanic.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    internal sealed class ScoreOverlay
    {
        private readonly PlayerOwned<Player> mPlayers;
        private readonly PlayTime mPlayTime;

        private readonly Sprite mSprite;
        private readonly PlayerOwned<TextSprite> mPowerTexts;
        private readonly PlayerOwned<TextSprite> mMoneyTexts;
        private readonly PlayerOwned<TextSprite> mExperienceTexts;
        private readonly TextSprite mTextTime;
        private readonly ImageButton mPauseButton;

        internal bool Pause;

        private static Point PowerIndicatorSize => new Point(100, 30);
        private static Point ClockSize => new Point(100, 20);

        public ScoreOverlay(PlayerOwned<Player> players, SpriteManager spriteManager)
        {
            mPlayers = players;
            mPlayTime = new PlayTime();

            var sprites = spriteManager.CreateScoreDisplay(PowerIndicatorSize, ClockSize);
            mSprite = sprites.Main;
            mPowerTexts = new PlayerOwned<TextSprite>(sprites.Left, sprites.Right);
            mMoneyTexts = new PlayerOwned<TextSprite>(sprites.LeftMoney, sprites.RightMoney);
            mExperienceTexts = new PlayerOwned<TextSprite>(sprites.LeftEP, sprites.RightEP);
            mTextTime = sprites.Clock;
            mTextTime.Text = mPlayTime.Time;

            var sprite = spriteManager.CreatePause();
            mPauseButton = new ImageButton(spriteManager, sprite, 40, 40);
            mPauseButton.Sprite.X = spriteManager.ScreenSize.X/2-20;
            mPauseButton.Sprite.Y = 20;
            Pause = false;
            mPauseButton.Clicked += (button, input) => Pause = true;
        }

        public TimeSpan Time => mPlayTime.Overall;

        public void Update(InputManager inputManager, GameTime gameTime)
        {
            mPowerTexts.A.Text = "A: " + mPlayers.A.Base.Power + "%";
            mPowerTexts.B.Text = "B: " + mPlayers.B.Base.Power + "%";
            mMoneyTexts.A.Text = mPlayers.A.Bitcoins + "$";
            mMoneyTexts.B.Text = mPlayers.B.Bitcoins + "$";
            mExperienceTexts.A.Text = mPlayers.A.ExperiencePoints + " EP";
            mExperienceTexts.B.Text = mPlayers.B.ExperiencePoints + " EP";

            mPlayTime.Update(gameTime);
            mTextTime.Text = mPlayTime.Time;

            mPauseButton.Update(inputManager, gameTime);
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            mSprite.Draw(spriteBatch, gameTime);
            mPauseButton.Draw(spriteBatch, gameTime);
        }
    }
}
