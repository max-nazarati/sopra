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
        private Texture2D mMTexture;
        private float mMPositionX = 0;
        private float mMPositionY = 0;
        private float mMScaleX = 1;
        private float mMScaleY = 1;
        private float mMRotation = 0;
        private Color mMColor;
        private float mMSizeX;
        private float mMSizeY;

        public Sprite(Texture2D texture, float positionX, float positionY, float scaleX, float scaleY, float rotation, Color color)
        {
            this.mMTexture = texture;
            this.mMPositionX = positionX;
            this.mMPositionY = positionY;
            this.mMScaleX = scaleX;
            this.mMScaleY = scaleY;
            this.mMRotation = rotation;
            this.mMColor = color;
            this.mMSizeX = texture.Width * scaleX;
            this.mMSizeY = texture.Height * scaleY;
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
            if (this.mMPositionX < x & x < this.mMPositionX + mMSizeX)
            {
                if (this.mMPositionY < y & y < this.mMPositionY + mMSizeY)
                {
                    result = true;
                }
            }

            return result;
        }

        [Obsolete]
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this.mMTexture, new Vector2(this.mMPositionX, this.mMPositionY), color: this.mMColor,
                scale: new Vector2(this.mMScaleX, this.mMScaleY), rotation: this.mMRotation);
        }

        // getters
        public Texture2D GetTexture() => mMTexture;
        public float GetPositionX() => mMPositionX;
        public float GetPositionY() => mMPositionY;
        public float GetScaleX() => mMScaleX;
        public float GetScaleY() => mMScaleY;
        public float GetRotation() => mMRotation;
        public Color GetColor() => mMColor;
        public float GetSizeX() => mMSizeX;
        public float GetSizeY() => mMSizeY;

        // setters
        public void SetTexture(Texture2D texture) => this.mMTexture = texture;
        public void SetPositionX(float x) => this.mMPositionX = x;
        public void SetPositionY(float y) => this.mMPositionY = y;
        public void SetScaleX(float x) => this.mMScaleX = x;
        public void SetScaleY(float y) => this.mMScaleY = y;

        public void SetRotation(float rotation) => this.mMRotation = rotation;
        public void SetColor(Color color) => this.mMColor = color;
        public void SetSizeX(float x) => this.mMSizeX = x;
        public void SetSizeY(float y) => this.mMSizeY = y;
    }
}
