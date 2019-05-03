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
    class Camera
    {

        private Matrix mTransformMatrix;
        private Matrix mInverseTransformMatrix;
        private float mViewportWidth;
        private float mViewportHeight;
        private float mMaxZoom;
        private float mMinZoom;
        private float mZoom;
        private float mWindowWidth;
        private float mWindowHeight;

        public Camera(Matrix transformMatrix, Viewport viewport,
            float windowWidth,
            float windowHeight,
            float minZoom = 0.1f,
            float maxZoom = 10,
            float zoom = 1)
        {
            this.mTransformMatrix = transformMatrix * Matrix.CreateScale(zoom, zoom, 1);
            this.mViewportWidth = viewport.Width;
            this.mViewportHeight = viewport.Height;
            this.mMaxZoom = maxZoom;
            this.mMinZoom = minZoom;
            this.mZoom = zoom;
            this.mWindowWidth = windowWidth;
            this.mWindowHeight = windowHeight;
        }

        public void MoveUp(float y) => this.mTransformMatrix *= Matrix.CreateTranslation(0, y, 0);
        public void MoveDown(float y) => this.mTransformMatrix *= Matrix.CreateTranslation(0, -y, 0);
        public void MoveRight(float x) => this.mTransformMatrix *= Matrix.CreateTranslation(-x, 0, 0);
        public void MoveLeft(float x) => this.mTransformMatrix *= Matrix.CreateTranslation(x, 0, 0);

        public void ZoomIn(float zoom)
        {
            float resultZoom = this.mZoom * zoom;
            if (resultZoom >= this.mMinZoom & resultZoom <= this.mMaxZoom)
            {
                this.mZoom *= zoom;
            }
        }

        public void ZoomOut(float zoom)
        {
            float resultZoom = this.mZoom * (1 / zoom);
            if (resultZoom >= this.mMinZoom & resultZoom <= this.mMaxZoom)
            {
                this.mZoom /= zoom;
            }
        }

        public void UpdateInverseTransformMatrix() => this.mInverseTransformMatrix = Matrix.Invert(this.mTransformMatrix);
        public void TransformCamera(Matrix transformMatrix) => this.mTransformMatrix *= transformMatrix;
        public Matrix TransformMatrix
        {
            get => this.mTransformMatrix * Matrix.CreateScale(this.mZoom, this.mZoom, 1) *
                   Matrix.CreateTranslation(new Vector3(this.mViewportWidth * 0.5f, this.mViewportHeight * 0.5f, 0));
            set => this.mTransformMatrix = value;
        }
        public Matrix InverseTransformMatrix
        {
            get => this.mInverseTransformMatrix;
            set => this.mInverseTransformMatrix = value;
        }
        public float MaxZoom
        {
            get => this.mMaxZoom;
            set => this.mMaxZoom = value; 
        }
        public float MinZoom
        {
            get => this.mMinZoom;
            set => this.mMinZoom = value;
        }

        public float CurrentZoom
        {
            get => this.mZoom;
            set => this.mZoom = value;
        }
    }
}
