using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    internal sealed class EntityGraph
    {
        private readonly List<Entity> mEntities;
        private int? mActiveUnit;

        public EntityGraph()
        {
            mEntities = new List<Entity>();
            mActiveUnit = null;
        }

        public void Add(Unit unit)
        {
            mEntities.Add(unit);
        }

        public void Update(Matrix viewMatrix)
        {
            var i = 0;
            foreach (var Object in mEntities)
            {
                Object.Update(viewMatrix);
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
