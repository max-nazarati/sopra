using System.Collections.Generic;
// using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    internal sealed class UnitManager
    {
        private readonly List<Unit> mUnits;
        private int? mActiveUnit;

        public UnitManager()
        {
            mUnits = new List<Unit>();
            mActiveUnit = null;
        }

        public void AddUnit(Unit unit)
        {
            mUnits.Add(unit);
        }

        public void Update()
        {
            var i = 0;
            foreach (var Object in mUnits)
            {
                Object.Update();
                // check if a new unit has been selected
                if (Object.mSelected)
                {
                    if (mActiveUnit != null)
                    {
                        mUnits[mActiveUnit.Value].mSelected = false;
                    }
                    mActiveUnit = i;
                    break;
                }
                i++;
            }
        }

        /*public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var Object in mUnits)
            {
                Object.Draw(spriteBatch);
            }
        }*/
    }
}
