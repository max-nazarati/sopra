using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KernelPanic
{
    class GridCalculator
    {
        // member variables
        private int rectangleHeightInPixel;
        private int rectangleHeightInFields;
        private int rectangleWidthInPixel;
        private int rectangleWidthInFields;
        private int laneWidthInPixel;
        private int laneWidthInFields;
        private int pixelPerField;

        // member methods
        public int GetLaneWidthInPixel()
        {
            return laneWidthInPixel;
        }

        private Tuple<int, int> CoordinateFromScreenPosition()
        {
            return new Tuple<int, int>(-1, -1);
        }

        private Tuple<int, int> ScreenPositionFromCoordinate()
        {
            return new Tuple<int, int>(-1, -1);
        }

        /*
        public void UnusedMethodsReSharper()
        {
            InputManager.Default.KeyPressed();
            InputManager.Default.KeyReleased();
            InputManager.Default.KeyUp();
            InputManager.Default.MouseReleased();
            InputManager.Default.MouseDragged(InputManager.MouseButton.Left);
        }
        */
    }
}
