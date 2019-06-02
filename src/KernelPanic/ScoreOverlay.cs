using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    sealed class ScoreOverlay
    {
        private readonly Player mPlayerA;
        private readonly Player mPlayerB;
        private PlayTime mPlayTime;
        
        private Sprite mSprite;
        private TextSprite mTextA, mTextB, mTextTime;
        
        private static Point PowerIndicatorSize => new Point(100, 30);
        private static Point ClockSize => new Point(100, 20);

        private readonly Point mScreenSize = SpriteManager.Default.GraphicsDevice.Viewport.Bounds.Size;

        public ScoreOverlay(Player player1, Player player2)
        {
            mPlayerA = player1;
            mPlayerB = player2;
            mPlayTime = new PlayTime();

            (mSprite, mTextA, mTextB, mTextTime) =
                SpriteManager.Default.CreateScoreDisplay(mScreenSize, PowerIndicatorSize, ClockSize);
            mTextTime.Text = mPlayTime.Time;
        }

        public TimeSpan Time => mPlayTime.Overall;

        public void Update(GameTime gameTime)
        {
            mPlayTime.Update(gameTime);
            mTextA.Text = "A: " + mPlayerA.Base.Power + "%";
            mTextB.Text = "B: " + mPlayerA.Base.Power + "%";
            mTextTime.Text = mPlayTime.Time;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            mSprite.Draw(spriteBatch, gameTime);
        }
    }
}
