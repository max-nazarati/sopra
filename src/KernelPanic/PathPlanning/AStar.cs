using System;
using System.Collections.Generic;
using System.Linq;
using KernelPanic.Data;
using KernelPanic.Table;
using Microsoft.Xna.Framework;

namespace KernelPanic.PathPlanning
{
    internal sealed class AStar : PathPlanner
    {
        private readonly ObstacleMatrix mObstacles;
        private readonly Point mTarget;
        private readonly Point mStart;
        internal List<Point> Path { get; private set; }

        internal AStar(Point start, Point target, ObstacleMatrix obstacles)
        {
            mObstacles = obstacles;
            mTarget = target;
            mStart = start;
        }

        private Node FindTarget()
        {
            return Run(new[] {mStart}).FirstOrDefault(node => node.Position == mTarget);
        }

        internal void CalculatePath()
        {
            var goalNode = FindTarget();
            if (goalNode == null)
                return;

            var path = new List<Point> {goalNode.Position};
            var currentNode = goalNode;
            var currentPosition = goalNode.Position;

            while (currentPosition != mStart)
            {
                currentNode = currentNode.Parent;
                currentPosition = currentNode.Position;
                path.Add(currentPosition);
            }

            path.Reverse();
            Path = path;
        }

        public Point? FindNearestField()
        {
            var result = (Point?) null;
            var bestDistance = double.PositiveInfinity;
            foreach (var point in mExplored)
            {
                var currentDistance = EstimateCost(point);
                if (currentDistance >= bestDistance)
                    continue;

                result = point;
                bestDistance = currentDistance;
            }

            return result;
        }

        #region Overrides

        protected override bool IsWalkable(Point point) => !mObstacles[point];
        protected override double CostIncrease => 0.5;
        protected override double EstimateCost(Point point)
        {
            // Euclidean heuristic.
            return Math.Sqrt(Math.Pow(point.X - mTarget.X, 2) + Math.Pow(point.Y - mTarget.Y, 2));
            
            // Manhattan heuristic.
            // return Math.Abs(mTarget.X - point.X) + Math.Abs(mTarget.Y - point.Y);
        }

        #endregion

        #region Visualization

        internal Visualizer CreateVisualization(Grid grid, SpriteManager spriteManager)
        {
            var visualization = TileVisualizer.Border(grid, spriteManager);
            visualization.Append(mObstacles);
            visualization.Append(mExplored, Color.Yellow);
            visualization.Append(Path, Color.Green);
            visualization.Append(new[] {mStart}, Color.Turquoise);
            visualization.Append(new[] {mTarget}, Color.Blue);
            return visualization;
        }

        #endregion
    }
    
}