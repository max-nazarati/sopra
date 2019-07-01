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
        private readonly Player mPlayerA;
        private readonly Player mPlayerB;
        private readonly PlayTime mPlayTime;

        private readonly Sprite mSprite;
        private readonly TextSprite mTextA;
        private readonly TextSprite mTextAMoney;
        private readonly TextSprite mTextAExperience;
        private readonly TextSprite mTextB;
        private readonly TextSprite mTextBMoney;
        private readonly TextSprite mTextBExperience;
        private readonly TextSprite mTextTime;
        private readonly ImageButton mPauseButton;

        internal bool Pause;

        private static Point PowerIndicatorSize => new Point(100, 30);
        private static Point ClockSize => new Point(100, 20);

        public ScoreOverlay(Player player1, Player player2, SpriteManager spriteManager)
        {
            mPlayerA = player1;
            mPlayerB = player2;
            mPlayTime = new PlayTime();

            (mSprite, mTextA, mTextAMoney, mTextAExperience, mTextB, mTextBMoney, mTextBExperience, mTextTime) =
                spriteManager.CreateScoreDisplay(PowerIndicatorSize, ClockSize);
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
            mPlayTime.Update(gameTime);
            mTextA.Text = "A: " + mPlayerA.Base.Power + "%";
            mTextAMoney.Text = mPlayerA.Bitcoins + "$";
            mTextAExperience.Text = mPlayerA.ExperiencePoints + " EP";
            mTextB.Text = "B: " + mPlayerB.Base.Power + "%";
            mTextBMoney.Text = mPlayerB.Bitcoins + "$";
            mTextBExperience.Text = mPlayerB.ExperiencePoints + " EP";
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
