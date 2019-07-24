using KernelPanic.Input;
using Microsoft.Xna.Framework.Input;

namespace KernelPanic
{
    internal static class DebugSettings
    {
        private static bool sVisualizeAStar;
        private static TroupeDataVisualization sVectorFieldVisualization;
        private static bool sVisualizeHeatMap;
        private static bool sGamePaused;
        private static bool sShowHitBoxes;

        internal static bool VisualizeAStar => sVisualizeAStar;
        internal static TroupeDataVisualization VectorFieldVisualization => sVectorFieldVisualization;
        public static bool VisualizeHeatMap => sVisualizeHeatMap;
        internal static bool GamePaused => sGamePaused;
        internal static bool ShowHitBoxes => sShowHitBoxes;

        internal static void Update(InputManager inputManager)
        {
            void Toggles(Keys key, ref bool b)
            {
                if (inputManager.KeyPressed(key))
                    b = !b;
            }

            void Advances(Keys key, ref TroupeDataVisualization value, TroupeDataVisualization maximum)
            {
                if (inputManager.KeyPressed(key))
                    value = value == maximum ? TroupeDataVisualization.None : value + 1;
            }

            Toggles(Keys.G, ref sVisualizeAStar);
            Toggles(Keys.H, ref sVisualizeHeatMap);
            Advances(Keys.V, ref sVectorFieldVisualization, TroupeDataVisualization.Thunderbird);
            Toggles(Keys.P, ref sGamePaused);
            Toggles(Keys.B, ref sShowHitBoxes);
        }

        internal enum TroupeDataVisualization
        {
            None, Normal, Thunderbird
        }
    }
}
