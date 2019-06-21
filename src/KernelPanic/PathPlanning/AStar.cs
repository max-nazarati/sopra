using System;
using System.Collections.Generic;
using System.Linq;
using KernelPanic.Data;
using KernelPanic.Table;
using Microsoft.Xna.Framework;

namespace KernelPanic.PathPlanning
{
    internal sealed class AStar
    {
        private readonly ObstacleMatrix mObstacles;
        private List<Point> mExploredNodes;
        private PriorityQueue mHeap = new PriorityQueue();
        private readonly Point mTarget;
        private readonly Point mStart;
        private readonly int mCountLimit;
        
        // debugging the path visually
        private List<Point> mPath;

        /// <summary>
        /// /// start and target as Point
        /// </summary>
        /// <param name="start"></param>
        /// <param name="target"></param>
        /// <param name="obstacles"></param>
        /// <param name="tileCount"></param>
        internal AStar(Point start, Point target, ObstacleMatrix obstacles, int tileCount)
        {
            mObstacles = obstacles;
            mExploredNodes = new List<Point>();
            mTarget = target;
            mStart = start;
            mCountLimit = tileCount;
        }
        
        public List<Point> Path => mPath;

        private bool IsStartPosition(Point position) => position == mStart;

        private bool IsTargetPosition(Point position) => position == mTarget;

        private IEnumerable<Node> CreateNeighbours(Node node)
        {
            return node.EnumerateNeighbours(0.5, EuclidHeuristic).Where(n => !mObstacles[n.Position]);
        }

        private void ExpandNode(Node node)
        {
            if (mExploredNodes.Contains(node.Position))
                return;
            foreach (var neighbour in CreateNeighbours(node))
            {
                if (!mExploredNodes.Contains(neighbour.Position)) mHeap.Insert(neighbour);
                // Console.WriteLine(neighbour.Position.ToString());
            }
            mExploredNodes.Add(node.Position);
            // Console.WriteLine(mExploredNodes.Count);
            // Console.WriteLine(mHeap.Count);
            // Console.WriteLine(node.Position.ToString());
            // Console.WriteLine(node.Position.ToString());
        }

        private Node FindTarget()
        {
            Reset();
            var startNode = new Node(mStart, null, 0, EuclidHeuristic(mStart));
            // double heuristicValue = startNode.Key;
            mHeap.Insert(startNode);

            var count = 0;
            while (!mHeap.IsEmpty() && count < mCountLimit)
            {
                count++;
                var heapNode = mHeap.RemoveMin();
                if (IsTargetPosition(heapNode.Position)) return heapNode;
                ExpandNode(heapNode);
                // Console.Write("I am currently expanding: ");
                // Console.WriteLine(heapNode.Position);
            }

            // Console.WriteLine("Couldn't find a path");
            return null;
        }

        internal void CalculatePath()
        {
            Reset();
            var path = new List<Point>();
            var goalNode = FindTarget();
            if (goalNode == null)
            {
                // Console.Write("Goal Node was null: ");
                // Console.WriteLine("why tho");
                return;
            }
            path.Add(goalNode.Position);
            var currentNode = goalNode;
            var currentPosition = goalNode.Position;

            while (!IsStartPosition(currentPosition))
            {
                currentNode = currentNode.Parent;
                currentPosition = currentNode.Position;
                path.Add(currentPosition);
            }
            path.Reverse();
            mPath = path;
        }

        private void Reset()
        {
            // mExploredNodes.Clear();
            mExploredNodes = new List<Point>();
            mHeap = new PriorityQueue();
            mPath = new List<Point>();
        }

        public Point? FindNearestField()
        {
            /*
            Console.WriteLine("----------- ExploredNodes:-----------");
            foreach (var VARIABLE in mExploredNodes)
            {
                Console.WriteLine(VARIABLE);
            }
            Console.WriteLine("-------------------------------------");
            if (!(mExploredNodes.Count > 0))
            {
                return new Point(0, 0);
            }
            */

            double bestDistance = EuclidHeuristic(mExploredNodes[0]);
            Point result = mExploredNodes[0];
            foreach (var point in mExploredNodes)
            {
                var currentDistance = EuclidHeuristic(point);
                if (currentDistance < bestDistance)
                {
                    bestDistance = currentDistance;
                    result = point;
                }
            }

            return result;
        }

        #region Heuristics

        private double EuclidHeuristic(Point point) => Math.Sqrt(Math.Pow(point.X - mTarget.X, 2) + Math.Pow(point.Y - mTarget.Y, 2));
        
        // private double ManhattenHeuristic(Point point) => Math.Abs(mTarget.X - point.X) + Math.Abs(mTarget.Y - point.Y);

        #endregion

        #region Visualization

        internal Visualizer CreateVisualization(Grid grid, SpriteManager spriteManager)
        {
            var visualization = new Visualizer(grid, spriteManager);
            visualization.Append(mObstacles);
            visualization.Append(mExploredNodes, Color.Yellow);
            visualization.Append(mPath, Color.Green);
            visualization.Append(new[] {mStart}, Color.Turquoise);
            visualization.Append(new[] {mTarget}, Color.Blue);
            return visualization;
        }

        #endregion
    }
    
}