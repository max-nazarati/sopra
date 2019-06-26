using System;
using KernelPanic.Sprites;
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
        private readonly TextSprite mTextB;
        private readonly TextSprite mTextBMoney;
        private readonly TextSprite mTextTime;

        private static Point PowerIndicatorSize => new Point(100, 30);
        private static Point ClockSize => new Point(100, 20);

        public ScoreOverlay(Player player1, Player player2, SpriteManager spriteManager)
        {
            mPlayerA = player1;
            mPlayerB = player2;
            mPlayTime = new PlayTime();

            (mSprite, mTextA, mTextAMoney, mTextB, mTextBMoney, mTextTime) =
                spriteManager.CreateScoreDisplay(PowerIndicatorSize, ClockSize);
            mTextTime.Text = mPlayTime.Time;
        }

        public TimeSpan Time => mPlayTime.Overall;

        public void Update(GameTime gameTime)
        {
            mPlayTime.Update(gameTime);
            mTextA.Text = "A: " + mPlayerA.Base.Power + "%";
            mTextAMoney.Text = mPlayerA.Bitcoins + "$";
            mTextB.Text = "B: " + mPlayerB.Base.Power + "%";
            mTextBMoney.Text = mPlayerB.Bitcoins + "$";
            mTextTime.Text = mPlayTime.Time;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            mSprite.Draw(spriteBatch, gameTime);
        }
    }
}
