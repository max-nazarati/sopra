using System;
using KernelPanic.Input;
using Microsoft.Xna.Framework.Input;

namespace KernelPanic
{
    internal static class DebugSettings
    {
        private static bool sVisualizeAStar;
        private static bool sVisualizeHeatMap;
        private static bool sVisualizeVectors;

        internal static bool VisualizeAStar => sVisualizeAStar;
        internal static bool VisualizeHeatMap => sVisualizeHeatMap;
        internal static bool VisualizeVectors => sVisualizeVectors;

        internal static void Update(InputManager inputManager)
        {
            void Toggles(Keys key, ref bool b)
            {
                if (inputManager.KeyPressed(key))
                    b = !b;
            }

            Toggles(Keys.H, ref sVisualizeHeatMap);
            Toggles(Keys.G, ref sVisualizeAStar);
            Toggles(Keys.V, ref sVisualizeVectors);
        }
    }
}
