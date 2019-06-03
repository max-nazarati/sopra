using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    internal sealed class EntityGraph
    {
        private readonly List<Entity> mEntities;
        private int? mActiveUnit;

        private Quadtree mQuadtree;

        public EntityGraph()
        {
            mEntities = new List<Entity>();
            mActiveUnit = null;
            mQuadtree = new Quadtree(1, new Rectangle(0, 0, 1000, 1000));
        }

        public void Add(Entity entity)
        {
            mEntities.Add(entity);
        }

        public bool HasEntityAt(Vector2 point)
        {
            // TODO: Implement this.
            return false;
        }

        public void Update(GameTime gameTime, Matrix invertedViewMatrix)
        {
            var i = 0;
            foreach (var Object in mEntities)
            {
                Object.Update(gameTime, invertedViewMatrix);
                // check if a new unit has been selected
                if (Object.Selected)
                {
                    if (mActiveUnit is int active)
                    {
                        if (i != active)
                        {
                            mEntities[active].Selected = false;
                        }
                    }
                    mActiveUnit = i;
                    //break;
                }
                i++;
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (var Object in mEntities)
            {
                Object.Draw(spriteBatch, gameTime);
            }
        }
    }
}
