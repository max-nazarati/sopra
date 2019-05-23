using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    internal sealed class Tower
    {
        private readonly ContentManager mContent;
        private readonly Texture2D mTower;
        private readonly int mPosX, mPosY;
        private float mRotation;
        private float mTimer = 3f;
        private const float Timer = 3f;
        private readonly List<Projectile> mProjektilListe = new List<Projectile>();
        internal Tower(ContentManager content, int x, int y)
        {
            mPosX = x;
            mPosY = y;
            mContent = content;
            mTower = mContent.Load<Texture2D>("tower");
        }

        internal void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(mTower, new Vector2(mPosX + 25, mPosY + 25), null, Color.White, mRotation,
                new Vector2(mTower.Width / 2f, mTower.Height / 2f), 0.08f, SpriteEffects.None, 0);
            foreach (var projectile in mProjektilListe)
            {
                projectile.Draw(spriteBatch);
            }
        }

        internal void Update(GameTime gameTime, Matrix viewMatrix)
        {
            // Mouse coordinates in World coordinates
            var relativeMouseVector = Vector2.Transform(InputManager.Default.MousePosition.ToVector2(), Matrix.Invert(viewMatrix));
            
            // distance between tower and mouse cursor
            var distance = (int) Math.Sqrt((int) Math.Pow(relativeMouseVector.X - mPosX, 2) +
                                           (int) Math.Pow(relativeMouseVector.Y - mPosY, 2));
            

            // elapsed gameTime since the last shoot
            var elapsed = (float) gameTime.ElapsedGameTime.TotalSeconds;
            mTimer -= elapsed;
            
            // only shoots, if cursor is less then 300 pixel away
            if (distance < 300)
            {
                // calculates the Rotation, so that the tower attacks the mouse Cursor
                mRotation =
                    (float)Math.Atan2(mPosY + 25 - relativeMouseVector.Y,
                        mPosX + 25 - relativeMouseVector.X) - (float)Math.PI / 2;
                if (mTimer < 0)
                {
                    mProjektilListe.Add(new Projectile(mContent, new Vector2(
                        (float)Math.Sin(mRotation % (3.14159265f * 2)),
                        -(float)Math.Cos(mRotation % (3.14159265f * 2))), new Vector2(mPosX, mPosY)));
                    SoundManager.Instance.PlaySound("shoot");
                    if (mProjektilListe.Count > 5)
                    {
                        mProjektilListe.RemoveAt(0);
                    }

                    mTimer = Timer;
                }
            }
            else
            {
                // Rotate animation, if no enemy is in range
                mRotation = ((float)Math.Sin((gameTime.TotalGameTime.TotalSeconds*10) % (2*Math.PI)) / 2);
            }

            foreach (var projectile in mProjektilListe)
            {
                projectile.Update();
            }
        }
    }
}