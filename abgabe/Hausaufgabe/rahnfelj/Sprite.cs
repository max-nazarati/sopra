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
        private Texture2D _mTexture;
        private float _mPositionX = 0;
        private float _mPositionY = 0;
        private float _mScaleX = 1;
        private float _mScaleY = 1;
        private float _mRotation = 0;
        private Color _mColor;
        private float _mSizeX;
        private float _mSizeY;

        public Sprite(Texture2D texture, float positionX, float positionY, float scaleX, float scaleY, float rotation, Color color)
        {
            this._mTexture = texture;
            this._mPositionX = positionX;
            this._mPositionY = positionY;
            this._mScaleX = scaleX;
            this._mScaleY = scaleY;
            this._mRotation = rotation;
            this._mColor = color;
            this._mSizeX = texture.Width * scaleX;
            this._mSizeY = texture.Height * scaleY;
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
            if (this._mPositionX < x & x < this._mPositionX + _mSizeX)
            {
                if (this._mPositionY < y & y < this._mPositionY + _mSizeY)
                {
                    result = true;
                }
            }

            return result;
        }

        [Obsolete]
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this._mTexture, new Vector2(this._mPositionX, this._mPositionY), color: this._mColor,
                scale: new Vector2(this._mScaleX, this._mScaleY), rotation: this._mRotation);
        }

        // getters
        public Texture2D GetTexture() => _mTexture;
        public float GetPositionX() => _mPositionX;
        public float GetPositionY() => _mPositionY;
        public float GetScaleX() => _mScaleX;
        public float GetScaleY() => _mScaleY;
        public float GetRotation() => _mRotation;
        public Color GetColor() => _mColor;
        public float GetSizeX() => _mSizeX;
        public float GetSizeY() => _mSizeY;

        // setters
        public void SetTexture(Texture2D texture) => this._mTexture = texture;
        public void SetPositionX(float x) => this._mPositionX = x;
        public void SetPositionY(float y) => this._mPositionY = y;
        public void SetScaleX(float x) => this._mScaleX = x;
        public void SetScaleY(float y) => this._mScaleY = y;

        public void SetRotation(float rotation) => this._mRotation = rotation;
        public void SetColor(Color color) => this._mColor = color;
        public void SetSizeX(float x) => this._mSizeX = x;
        public void SetSizeY(float y) => this._mSizeY = y;
    }
}
