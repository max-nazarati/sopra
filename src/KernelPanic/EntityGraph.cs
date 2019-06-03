using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    internal sealed class EntityGraph
    {
        private Quadtree mQuadtree;

        public EntityGraph()
        {
            mQuadtree = new Quadtree(1, new Rectangle(0, 0, 1000, 1000));
        }

        public void Add(Entity entity)
        {
            mQuadtree.Add(entity);
        }

        public bool HasEntityAt(Vector2 point)
        {
            // TODO: Implement this.
            return false;
        }

        public void Update(PositionProvider positionProvider, GameTime gameTime, Matrix invertedViewMatrix)
        {
            foreach (var entity in mQuadtree)
            {
                entity.Update(positionProvider, gameTime, invertedViewMatrix);
            }

            mQuadtree.Rebuild();
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (var Object in mQuadtree)
            {
                Object.Draw(spriteBatch, gameTime);
            }
        }
    }
}
