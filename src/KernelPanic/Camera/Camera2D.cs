using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace KernelPanic
{
    [DataContract]
    internal sealed class Camera2D : ICamera
    {
        [DataMember]
        private Vector2 mPosition;
        // private readonly float mRotation;
        [DataMember]
        private float mZoom;
        [DataMember]
        private readonly Vector2 mOrigin;

        internal Camera2D(Point viewportSize)
        {
            // mRotation = 0;
            mZoom = 1;
            mOrigin = new Vector2(viewportSize.X / 2f, viewportSize.Y / 2f);
            mPosition.X = 290;
            mPosition.Y = 1150;
        }

        /// <inheritdoc />
        public Matrix Transformation => GetViewMatrix();

        /// <inheritdoc />
        public void Apply(sbyte xMovement, sbyte yMovement, sbyte scaling)
        {
            PosX += xMovement * 10 / mZoom;
            PosY += yMovement * 10 / mZoom;
            Zoom += scaling * 0.1f / mZoom;
        }

        /// <summary>
        /// Asks if the camera should be moved and reacts accordingly
        /// </summary>
        private void MoveCamera()
        {
            // Movement via Dragging (with InputManager) disables the other inputs
            if (InputManager.Default.MouseDown(InputManager.MouseButton.Middle))
            {
                PosX -= InputManager.Default.MouseMovement.X;
                PosY -= InputManager.Default.MouseMovement.Y;
            }
            else
            {
                // Movement wia Button
                if (InputManager.Default.KeyDown(Keys.W) || InputManager.Default.MouseAtTopScreenBorder())
                {
                    // mPosition += new Vector2(0, -600) * deltaTime;
                    PosY += -10 / mZoom;
                }
                if (InputManager.Default.KeyDown(Keys.A) || InputManager.Default.MouseAtLeftScreenBorder())
                {
                    // mPosition += new Vector2(-600, 0) * deltaTime;
                    PosX += -10 / mZoom;
                }
                if (InputManager.Default.KeyDown(Keys.S) || InputManager.Default.MouseAtBottomScreenBorder())
                {
                    // mPosition += new Vector2(0, 600) * deltaTime;
                    PosY += 10 / mZoom;
                }
                if (InputManager.Default.KeyDown(Keys.D) || InputManager.Default.MouseAtRightScreenBorder())
                {
                    // mPosition += new Vector2(600, 0) * deltaTime;
                    PosX += 10 / mZoom;
                }
            }
        }

        /// <summary>
        /// Asks if the camera should be zoomed and reacts accordingly
        /// </summary>
        private void ZoomCamera()
        {
            if (InputManager.Default.ScrolledDown())
            {
                Zoom -= 0.1f * mZoom;
            }
            else if (InputManager.Default.ScrolledUp())
            {
                Zoom += 0.1f * mZoom;
            }
        }

        internal void Update()
        {
            MoveCamera();
            ZoomCamera();
        }

        internal Matrix GetViewMatrix()
        {
            return
                Matrix.CreateTranslation(new Vector3(-mPosition, 0.0f)) *
                Matrix.CreateTranslation(new Vector3(-mOrigin, 0.0f)) *
                // Matrix.CreateRotationZ(mRotation) *
                Matrix.CreateScale(mZoom, mZoom, 1) *
                Matrix.CreateTranslation(new Vector3(mOrigin, 0.0f));
        }

        private float Zoom
        {
            get => mZoom;
            set
            {
                if (value > 0.1 && value < 6)
                {
                    mZoom = value;
                }
            }
        }


        private float PosX
        {
            get => mPosition.X;
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
            get => mPosition.Y;
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
