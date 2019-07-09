using KernelPanic.Input;
using Microsoft.Xna.Framework.Input;

namespace KernelPanic
{
    internal static class DebugSettings
    {
        private static bool _sVisualizeAStar;
        private static bool _sVisualizeHeatMap;
        private static bool _sVisualizeVectors;

        internal static bool VisualizeAStar => _sVisualizeAStar;
        internal static bool VisualizeHeatMap => _sVisualizeHeatMap;
        internal static bool VisualizeVectors => _sVisualizeVectors;

        internal static void Update(InputManager inputManager)
        {
            void Toggles(Keys key, ref bool b)
            {
                if (inputManager.KeyPressed(key))
                    b = !b;
            }

            Toggles(Keys.H, ref _sVisualizeHeatMap);
            Toggles(Keys.G, ref _sVisualizeAStar);
            Toggles(Keys.V, ref _sVisualizeVectors);
        }
    }
}
