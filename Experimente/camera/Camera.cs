using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace rahnfelj
{
    class Camera
    {

        private Matrix mTransformMatrix;
        private Matrix mInverseTransformMatrix;

        public Camera(Matrix transformMatrix)
        {
            this.mTransformMatrix = transformMatrix;
        }

        public void MoveUp(float y) => this.mTransformMatrix *= Matrix.CreateTranslation(0, y, 0);
        public void MoveDown(float y) => this.mTransformMatrix *= Matrix.CreateTranslation(0, -y, 0);
        public void MoveRight(float x) => this.mTransformMatrix *= Matrix.CreateTranslation(-x, 0, 0);
        public void MoveLeft(float x) => this.mTransformMatrix *= Matrix.CreateTranslation(x, 0, 0);
        public void Zoom(float zoom) => this.mTransformMatrix *= Matrix.CreateScale(zoom, zoom, 1);
        public void UpdateInverseTransformMatrix() => this.mInverseTransformMatrix = Matrix.Invert(this.mTransformMatrix);
        public void TransformCamera(Matrix transformMatrix) => this.mTransformMatrix *= transformMatrix;
        public Matrix TransformMatrix { get => this.mTransformMatrix; set => this.mTransformMatrix = value; }
        public Matrix InverseTransformMatrix
        {
            get => this.mInverseTransformMatrix;
            set => this.mInverseTransformMatrix = value;
        }
    }
}
