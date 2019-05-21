using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TowerPlacementTest
{
    internal sealed class Tower
    {
        private readonly ContentManager mContent;
        private readonly Texture2D mTower;
        private readonly int mPosX, mPosY;
        private float mRotation;
        private float mTimer = 1f;
        private const float Timer = 1f;
        private readonly List<Projectile> mProjektilListe = new List<Projectile>();
        internal Tower(ContentManager content, int x, int y)
        {
            this.mPosX = x;
            this.mPosY = y;
            this.mContent = content;
            mTower = mContent.Load<Texture2D>("tower");
        }

        internal void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(mTower, new Vector2(mPosX + 25, mPosY + 25), null, Color.White, mRotation,
                new Vector2(mTower.Width / 2, mTower.Height / 2), 0.08f, SpriteEffects.None, 0);
            foreach (var projectile in mProjektilListe)
            {
                projectile.Draw(spriteBatch);
            }
        }

        internal void Update(GameTime gameTime)
        {
            mRotation += 0.01f;
            var elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            mTimer -= elapsed;
            if (mTimer < 0)
            {
                mProjektilListe.Add(new Projectile(mContent, new Vector2((float)Math.Sin(mRotation % (3.14159265f * 2)),
                    -(float)Math.Cos(mRotation % (3.14159265f * 2))), new Vector2(mPosX + 25, mPosY + 25)));
                SoundManager.Instance.PlaySound("shoot");
                if (mProjektilListe.Count > 50)
                {
                    mProjektilListe.RemoveAt(0);
                }
                mTimer = Timer;
            }

            foreach (var projectile in mProjektilListe)
            {
                projectile.Update();
            }

        }

    }
}
