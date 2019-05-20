using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace Collision
{
    sealed class UnitManager
    {
        private readonly List<Unit> mUnits;
        private int mActiveUnit;

        public UnitManager()
        {
            mUnits = new List<Unit>();
            mActiveUnit = -1;
        }

        public void AddUnit(Unit Object)
        {
            mUnits.Add(Object);
        }

        public void Update()
        {
            var i = 0;
            foreach (var Object in mUnits)
            {
                if (Object.Update())
                {
                    if (mActiveUnit >= 0)
                    {
                        mUnits[mActiveUnit].Deselect();
                    }
                    mActiveUnit = i;
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
