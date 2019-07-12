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
        private readonly Point[] mTargets;
        private readonly Point mStart;
        internal List<Point> Path { get; private set; }

        internal AStar(Point start, Point target, ObstacleMatrix obstacles) : this(start, new []{target}, obstacles)
        {
        }

        internal AStar(Point start, Point[] targets, ObstacleMatrix obstacles)
        {
            mObstacles = obstacles;
            mTargets = targets;
            mStart = start;
        }

        private Node FindTarget()
        {
            return Run(new[] {mStart}).FirstOrDefault(node => mTargets.Contains(node.Position));
        }

        internal bool CalculatePath()
        {
            var goalNode = FindTarget();
            if (goalNode == null)
                return false;

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
            return true;
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
            return mTargets.Select(target =>
            {
                var x = point.X - target.X;
                var y = point.Y - target.Y;
                return x * x + y * y;
            }).Min();
            
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
            visualization.Append(mTargets, Color.Blue);
            return visualization;
        }

        #endregion
    }
    
}