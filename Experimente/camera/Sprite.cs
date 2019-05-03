using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace rahnfelj
{
    class Sprite
    {
        private Texture2D mTexture;
        private float mPositionX = 0;
        private float mPositionY = 0;
        private float mScaleX = 1;
        private float mScaleY = 1;
        private float mRotation = 0;
        private Color mColor;
        private float mSizeX;
        private float mSizeY;

        public Sprite(Texture2D texture, float positionX, float positionY, float scaleX, float scaleY, float rotation, Color color)
        {
            this.mTexture = texture;
            this.mPositionX = positionX;
            this.mPositionY = positionY;
            this.mScaleX = scaleX;
            this.mScaleY = scaleY;
            this.mRotation = rotation;
            this.mColor = color;
            this.mSizeX = texture.Width * scaleX;
            this.mSizeY = texture.Height * scaleY;
        }
        /// <summary>
        /// check if given coordinates intersect with coordinates of the sprite object
        /// </summary>
        /// <param name="x">x coordinate to be checked</param>
        /// <param name="y">y coordinate to be checked</param>
        /// <returns>true if coordinates have intersection point with sprite object</returns>
        public Boolean TouchesSprite(float x, float y)
        {
            Boolean result = false;
            if (this.mPositionX < x & x < this.mPositionX + mSizeX)
            {
                if (this.mPositionY < y & y < this.mPositionY + mSizeY)
                {
                    result = true;
                }
            }

            return result;
        }

        [Obsolete]
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this.mTexture, new Vector2(this.mPositionX, this.mPositionY), color: this.mColor,
                scale: new Vector2(this.mScaleX, this.mScaleY), rotation: this.mRotation);
        }

        // setters and getters
        public Texture2D GetTexture() => mTexture;
        public float PositionX() => mPositionX;
        public float PositionY() => mPositionY;
        public float ScaleX() => mScaleX;
        public float ScaleY() => mScaleY;
        public float GetRotation() => mRotation;
        public Color GetColor() => mColor;
        public float GetSizeX() => mSizeX;
        public float GetSizeY() => mSizeY;

        // setters
        public void SetTexture(Texture2D texture) => this.mTexture = texture;
        public void SetPositionX(float x) => this.mPositionX = x;
        public void SetPositionY(float y) => this.mPositionY = y;
        public void SetScaleX(float x) => this.mScaleX = x;
        public void SetScaleY(float y) => this.mScaleY = y;

        public void SetRotation(float rotation) => this.mRotation = rotation;
        public void SetColor(Color color) => this.mColor = color;
        public void SetSizeX(float x) => this.mSizeX = x;
        public void SetSizeY(float y) => this.mSizeY = y;
    }
}
