using System.Collections.Generic;
using KernelPanic.Data;
using Microsoft.Xna.Framework;

namespace KernelPanic.PathPlanning
{
    internal sealed class BreadthFirstSearch : PathPlanner
    {
        private readonly ObstacleMatrix mObstacleMatrix;

        private BreadthFirstSearch(ObstacleMatrix obstacleMatrix)
        {
            mObstacleMatrix = obstacleMatrix;
        }

        internal static void UpdateHeatMap(HeatMap map, IEnumerable<Point> goalPoints)
        {
            foreach (var node in new BreadthFirstSearch(map.ObstacleMatrix).Run(goalPoints))
            {
                map.SetCost(node.Position, (float) node.Cost);
            }
        }

        protected override bool IsWalkable(Point point) => !mObstacleMatrix[point];
        protected override double EstimateCost(Point point) => 0;
        protected override double CostIncrease => 1;
    }
}
