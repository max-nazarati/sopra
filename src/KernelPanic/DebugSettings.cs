using KernelPanic.Input;
using Microsoft.Xna.Framework.Input;

namespace KernelPanic
{
    internal static class DebugSettings
    {
        internal static bool VisualizeAStar { get; private set; }
        internal static bool VisualizeHeatMap { get; private set; }

        internal static void Update(InputManager inputManager)
        {
            if (inputManager.KeyPressed(Keys.H))
                VisualizeHeatMap = !VisualizeHeatMap;

            if (inputManager.KeyPressed(Keys.A))
                VisualizeAStar = !VisualizeAStar;
        }
    }
}
