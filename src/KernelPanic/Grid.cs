using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace KernelPanic
{
    internal sealed class Grid
    {
        private readonly int mRows, mColumns;
        private readonly float mScale;
        private readonly ContentManager mContent;
        private readonly bool mLeft;
        private Texture2D mKacheln, mBase, mSäule, mMauer, mBorder;

        private Color mBorderColor = Color.Red;

        internal Grid(ContentManager content, int rows, int columns, bool left)
        {
            this.mContent = content;
            this.mRows = rows;
            this.mColumns = columns;
            this.mLeft = left;
            mScale = 0.5F;
        }

        internal void Draw(SpriteBatch spriteBatch, Camera2D camera)
        {
            mKacheln = mContent.Load<Texture2D>("Kachel3");
            mBase = mContent.Load<Texture2D>("Papier");
            mSäule = mContent.Load<Texture2D>("Saeule");
            mMauer = mContent.Load<Texture2D>("Mauer");
            mBorder = mContent.Load<Texture2D>("Border");
            DrawFields(spriteBatch);
            DrawBorder(spriteBatch, camera);
        }

        private void DrawFields(SpriteBatch spriteBatch)
        {
            for (var i = 0; i < mColumns; i++)
            {
                for (var j = 0; j < mRows; j++)
                {
                    if (mLeft)
                    {
                        // draws left lane
                        spriteBatch.Draw(mKacheln,
                            new Rectangle((int)(mScale * mKacheln.Width * i),
                                (int)(mScale * mKacheln.Height * j),
                                (int)(mScale * 200),
                                (int)(mScale * 200)),
                            null,
                            Color.AliceBlue);
                        // draws left corners
                        if (j < mColumns || j > mRows - mColumns - 1)
                        {
                            spriteBatch.Draw(mKacheln,
                                new Rectangle((int) (mScale * mKacheln.Width * i + (mColumns * mScale * mKacheln.Width)),
                                    (int) (mScale * mKacheln.Height * j),
                                    (int) (mScale * 200),
                                    (int) (mScale * 200)),
                                null,
                                Color.AliceBlue);
                        }
                    }
                    else
                    {
                        // draws right lane
                        spriteBatch.Draw(mKacheln,
                            new Rectangle((int) (mScale * mKacheln.Width * i) + (int) (4000 * mScale),
                                (int) (mScale * mKacheln.Height * j),
                                (int) (mScale * 200),
                                (int) (mScale * 200)),
                            null,
                            Color.AliceBlue);
                        // draws right corners
                        if (j < mColumns || j > mRows - mColumns - 1)
                        {
                            spriteBatch.Draw(mKacheln,
                                new Rectangle(
                                    (int) (mScale * mKacheln.Width * i + 4000 * mScale -
                                           (mColumns * mScale * mKacheln.Width)),
                                    (int) (mScale * mKacheln.Height * j),
                                    (int) (mScale * 200),
                                    (int) (mScale * 200)),
                                null,
                                Color.AliceBlue);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// change the color of the selected square on doubleClick
        /// mainly for testing purpose
        /// </summary>
        private void UpdateColor()
        {
            if (!InputManager.Default.DoubleClick()) return;
            if (mBorderColor == Color.Green)
            {
                mBorderColor = Color.Red;
            }
            else
            {
                mBorderColor = Color.Green;
            }
        }

        private void DrawBorder(SpriteBatch spriteBatch, Camera2D camera)
        {
            UpdateColor();
            var mouseState = Mouse.GetState();
            var mouseX = (int)((mouseState.X + camera.mPosition.X)/camera.mZoom);
            var mouseY = (int)((mouseState.Y + camera.mPosition.Y)/camera.mZoom);
            var posX = (int) ((mouseX / 50) * 50);
            var posY = (int) ((mouseY / 50) * 50);
            Console.WriteLine(posX*camera.mZoom);
            spriteBatch.Draw(mBorder, new Rectangle(posX, posY, 50, 50), null, mBorderColor);
        }
    }
}