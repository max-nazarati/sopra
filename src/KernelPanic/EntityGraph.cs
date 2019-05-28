using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    internal sealed class EntityGraph
    {
        private readonly List<Unit> mUnits;
        private int? mActiveUnit;

        public EntityGraph()
        {
            mUnits = new List<Unit>();
            mActiveUnit = null;
        }

        public void AddUnit(Unit unit)
        {
            mUnits.Add(unit);
        }

        public void Update(Matrix viewMatrix)
        {
            var i = 0;
            foreach (var Object in mUnits)
            {
                Object.Update(viewMatrix);
                // check if a new unit has been selected
                if (Object.mSelected)
                {
                    if (mActiveUnit is int active)
                    {
                        if (i != active)
                        {
                            mUnits[active].mSelected = false;
                        }
                    }
                    mActiveUnit = i;
                    //break;
                }
                i++;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var Object in mUnits)
            {
                Object.Draw(spriteBatch);
            }
        }
    }
}
