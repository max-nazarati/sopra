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
        private CompositeSprite mASide;
        private CompositeSprite mBSide;
        private CompositeSprite mClock;
        private PlayTime mPlayTime;
        private static int mClockWidth = 100;
        private static int mClockHeight = 20;
        private static int mPowerIndicatorWidth = 100;
        private static int mPowerIndicatorHeight = 30;

        private Vector2 mScreenSize = new Vector2(SpriteManager.Default.GraphicsDevice.Viewport.Bounds.Size.X,
            SpriteManager.Default.GraphicsDevice.Viewport.Bounds.Size.Y);
        public ScoreOverlay(Player player1, Player player2)
        {
            mPlayerA = player1;
            mPlayerB = player2;
            mPlayTime = new PlayTime();
            var texture = SpriteManager.Default.LoadImage("Papier");
            var font = SpriteManager.Default.LoadFont("ScoreOverlayFont");

            var background1 = new ImageSprite(texture, 0, 0)
            {
                DestinationRectangle = new Rectangle(0, 0, mPowerIndicatorWidth, mPowerIndicatorHeight)
            };
            var text1 = new TextSprite(font, "BaseA: " + mPlayerA.Base.Power, background1.Width / 2, background1.Height / 2);
            text1.Origin = new Vector2(text1.Width / 2, text1.Height / 2);
            text1.Color = Color.Black;
            background1.Color = Color.LightBlue;
            mASide = new CompositeSprite(mScreenSize.X/2 - (mPowerIndicatorWidth / 2 + mClockWidth), 0);
            mASide.Children.Add(background1);
            mASide.Children.Add(text1);

            var background2 = new ImageSprite(texture, 0, 0)
            {
                DestinationRectangle = new Rectangle(0, 0, mPowerIndicatorWidth, mPowerIndicatorHeight)
            };
            background2.Color = Color.LightSalmon;
            var text2 = new TextSprite(font, "BaseB: " + mPlayerB.Base.Power, background2.Width / 2, background2.Height / 2);
            text2.Origin = new Vector2(text2.Width / 2, text2.Height / 2);
            text2.Color = Color.Black;
            mBSide = new CompositeSprite(mScreenSize.X/2 + mClockWidth/2, 0);
            mBSide.Children.Add(background2);
            mBSide.Children.Add(text2);

            var clockBackground = new ImageSprite(texture, 0, 0)
            {
                DestinationRectangle = new Rectangle(0, 0, mClockWidth, mClockHeight)
            };
            var time = new TextSprite(font, mPlayTime.Time, clockBackground.Width / 2, clockBackground.Height / 2);
            time.Origin = new Vector2(time.Width / 2, time.Height / 2);
            time.Color = Color.Black;
            mClock = new CompositeSprite(mScreenSize.X/2 - mClockWidth / 2, 0);
            mClock.Children.Add(clockBackground);
            mClock.Children.Add(time);
        }

        public TimeSpan Time
        {
            get
            {
                return mPlayTime.Overall;
            }
        }

        public void Update(GameTime gameTime)
        {
            mPlayTime.Update(gameTime);
            var tmpText = (TextSprite)mClock.Children[1];
            tmpText.Text = mPlayTime.Time;
            tmpText.Origin = new Vector2(tmpText.Width / 2, tmpText.Height / 2);
            mClock.Children[1] = tmpText;
        }
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            mASide.Draw(spriteBatch, gameTime);
            mBSide.Draw(spriteBatch, gameTime);
            mClock.Draw(spriteBatch, gameTime);

        }
    }
}
