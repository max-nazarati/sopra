using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace KernelPanic
{
    internal sealed class Camera2D
    {
        internal Vector2 mPosition;
        private readonly float mRotation;
        private int mOldMouseX, mOldMouseY;
        internal float mZoom;
        private readonly Vector2 mOrigin;

        private int mScrollValue, mPreviousScrollValue;
        internal Camera2D(Viewport viewport)
        {
            mRotation = 0;
            mZoom = 1;
            mOrigin = new Vector2(viewport.Width / 2f, viewport.Height / 2f);
            mPosition.X = 290;
            mPosition.Y = 1150;
        }

        internal void Update(GameTime gameTime)
        {
            var mouseState = Mouse.GetState();
            mScrollValue = mouseState.ScrollWheelValue;
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var keyboardState = Keyboard.GetState();

            // movement
            if (keyboardState.IsKeyDown(Keys.W))
                mPosition -= new Vector2(0, 600) * deltaTime;

            if (keyboardState.IsKeyDown(Keys.S))
               mPosition += new Vector2(0, 600) * deltaTime;

            if (keyboardState.IsKeyDown(Keys.A))
                mPosition -= new Vector2(600, 0) * deltaTime;

            if (keyboardState.IsKeyDown(Keys.D))
                mPosition += new Vector2(600, 0) * deltaTime;

            if (mScrollValue + 5 < mPreviousScrollValue)
            {
                Zoom -= 0.1f * mZoom;
            }
            else if (mScrollValue - 5 > mPreviousScrollValue)
            {
                Zoom += 0.1f * mZoom;
            }

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                if (mouseState.Position.X != mOldMouseX)
                {
                    PosX -= (mouseState.X - mOldMouseX) / mZoom;
                }

                if (mouseState.Position.Y != 0)
                {
                    mPosition.Y -= (mouseState.Y - mOldMouseY) / mZoom;
                }
            }

            if (mouseState.X < 400 && mouseState.X > 0)
            {
                PosX -= 10/mZoom;
            }
            if (mouseState.X > 1520 && mouseState.X < 1920)
            {
                PosX += 10/mZoom;
            }
            if (mouseState.Y < 200 && mouseState.Y > 0)
            {
                PosY -= 10/mZoom;
            }
            if (mouseState.Y > 880 && mouseState.Y < 1080)
            {
                PosY += 10/mZoom;
            }

            mOldMouseX = mouseState.X;
            mOldMouseY = mouseState.Y;
            mPreviousScrollValue = mScrollValue;
        }

        internal Matrix GetViewMatrix()
        {
            return
                Matrix.CreateTranslation(new Vector3(-mPosition, 0.0f)) *
                Matrix.CreateTranslation(new Vector3(-mOrigin, 0.0f)) *
                Matrix.CreateRotationZ(mRotation) *
                Matrix.CreateScale(mZoom, mZoom, 1) *
                Matrix.CreateTranslation(new Vector3(mOrigin, 0.0f));
        }

        private float Zoom
        {
            get => this.mZoom;
            set
            {
                if (value > 0.8 && value < 6)
                {
                    mZoom = value;
                }
            }
        }

        private float PosX
        {
            get => this.mPosition.X;
            set
            {
                if (mPosition.X >= -1050 && mPosition.X <= 1150)
                {
                    mPosition.X = value;
                }
                if (mPosition.X < -1050)
                {
                    mPosition.X = -1050;
                }

                if (mPosition.X > 1150)
                {
                    mPosition.X = 1150;
                }
            }
        }

        private float PosY
        {
            get => this.mPosition.Y;
            set
            {
                if (mPosition.Y >= -1000 && mPosition.Y <= 1500)
                {
                    mPosition.Y = value;
                }
                if (mPosition.Y < -1000)
                {
                    mPosition.Y = -1000;
                }

                if (mPosition.Y > 1500)
                {
                    mPosition.Y = 1500;
                }
            }
        }
    }
}
