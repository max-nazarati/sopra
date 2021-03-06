﻿using System.Collections.Generic;
using KernelPanic.Data;
using Microsoft.Xna.Framework;

namespace KernelPanic.PathPlanning
{
    internal sealed class BreadthFirstSearch : PathPlanner
    {
        private readonly ObstacleMatrix mObstacleMatrix;
        private readonly List<Point> mUnwantedPoints;

        private BreadthFirstSearch(ObstacleMatrix obstacleMatrix, List<Point> unwantedPoints)
        {
            mObstacleMatrix = obstacleMatrix;
            mUnwantedPoints = unwantedPoints;
        }

        internal static void UpdateHeatMap(HeatMap map, IEnumerable<Point> goalPoints, List<Point> unwantedPoints = null)
        {
            map.SetZero();
            var breadthFirstSearch = new BreadthFirstSearch(map.ObstacleMatrix, unwantedPoints);
            foreach (var node in breadthFirstSearch.Run(goalPoints))
            {
                map.SetCost(node.Position, (float) node.Cost);
            }
        }

        private const int UnwantedCost = 2;
        private const int UnwantedEstimation = 1;

        protected override bool IsWalkable(Point point) => !mObstacleMatrix[point];
        protected override double EstimateCost(Point point) =>
            mUnwantedPoints?.BinarySearch(point, new PointComparer()) >= 0 ? UnwantedEstimation : 0;
        protected override double CostIncrease(Point point) =>
            mUnwantedPoints?.BinarySearch(point, new PointComparer()) >= 0 ? UnwantedCost : 1;
    }
}
