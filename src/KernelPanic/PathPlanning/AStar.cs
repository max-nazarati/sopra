using System;
using System.Collections.Generic;
using KernelPanic.Data;
using KernelPanic.Input;
using KernelPanic.Table;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace KernelPanic.PathPlanning
{
    internal sealed class AStar
    {
        private readonly List<Point> mCoordinateList;
        private List<Point> mExploredNodes;
        private PriorityQueue mHeap = new PriorityQueue();
        private readonly Point mTarget;
        private readonly Point mStart;
        private readonly int mCountLimit;

        // mainly debugging reasons for now
        private List<Point> mBlocked;
        private List<Point> mWalkable;
        private readonly ObstacleMatrix mObstacles;
        
        // debugging the path visually
        private List<Point> mPath;

        /// <summary>
        /// /// start and target as Point
        /// </summary>
        /// <param name="coordinateList"></param>
        /// <param name="start"></param>
        /// <param name="target"></param>
        /// <param name="obstacles"></param>
        internal AStar(List<Point> coordinateList, Point start, Point target, ObstacleMatrix obstacles)
        {
            mCoordinateList = coordinateList;
            mExploredNodes = new List<Point>();
            mTarget = target;
            mStart = start;

            mWalkable = new List<Point>(); // = coordinateList;
            mBlocked = new List<Point>();
            mObstacles = obstacles;
            mCountLimit = coordinateList.Count;
        }
        
        public List<Point> Path => mPath;

        private bool IsStartPosition(Point position) => position == mStart;

        private bool IsTargetPosition(Point position) => position == mTarget;

        private IEnumerable<Node> CreateNeighbours(Node node)
        {
            var cost = node.Cost + 0.5; // if we put an higher value as summand we get a 'dumb' search

            Node CreateNode(int xOffset, int yOffset)
            {
                var point = node.Position + new Point(xOffset, yOffset);
                if (mObstacles[point])
                    return null;

                var estimatedCost = EuclidHeuristic(point);
                var isStartNode = IsStartPosition(point);
                var isGoalNode = IsTargetPosition(point);
                return new Node(point, node, cost, estimatedCost, isStartNode, isGoalNode);
            }

            if (CreateNode(0, -1) is Node up)
                yield return up;
            if (CreateNode(0, 1) is Node down)
                yield return down;
            if (CreateNode(-1, 0) is Node left)
                yield return left;
            if (CreateNode(1, 0) is Node right)
                yield return right;
        }

        private void ExpandNode(Node node)
        {
            if (mExploredNodes.Contains(node.Position)) return;
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
            var startNode = new Node(mStart, null, 0, EuclidHeuristic(mStart), true);
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
        
        #region update
        /*
        private void SetStart(Point start)
        {
            if (mWalkable.Contains(start))
            {
                mStart = start;
            }
        }

        private void SetTarget(Point target)
        {
            if (mWalkable.Contains(target))
            {
                mTarget = target;
            }
        }
        
        public void Update(InputManager inputManager)
        {
            UpdateObstacles(inputManager);
            UpdateStartAndTarget(inputManager);
            CalculatePath();
        }

        private void UpdateStartAndTarget(InputManager inputManager)
        {
            if (!inputManager.KeyPressed(Keys.Up, Keys.Left, Keys.Down, Keys.Right)) return;
            if (inputManager.KeyPressed(Keys.Up))
            {
                if (inputManager.KeyDown(Keys.LeftShift, Keys.RightShift))
                {
                    SetStart(new Point(mStart.X, mStart.Y - 1));
                }
                else
                {
                    SetTarget(new Point(mTarget.X, mTarget.Y - 1));
                }
                    
            }
            if (inputManager.KeyPressed(Keys.Left))
            {
                if (inputManager.KeyDown(Keys.LeftShift, Keys.RightShift))
                {
                    SetStart(new Point(mStart.X - 1, mStart.Y));
                }
                else
                {
                    SetTarget(new Point(mTarget.X - 1, mTarget.Y));
                }

            }
            if (inputManager.KeyPressed(Keys.Down))
            {
                if (inputManager.KeyDown(Keys.LeftShift, Keys.RightShift))
                {
                    SetStart(new Point(mStart.X, mStart.Y + 1));
                }
                else
                {
                    SetTarget(new Point(mTarget.X, mTarget.Y + 1));
                }
            }
            if (inputManager.KeyPressed(Keys.Right))
            {
                if (inputManager.KeyDown(Keys.LeftShift, Keys.RightShift))
                {
                    SetStart(new Point(mStart.X + 1, mStart.Y));
                }
                else
                {
                    SetTarget(new Point(mTarget.X + 1, mTarget.Y));
                }
            }
            CalculatePath();
        }
        */
        #endregion update
        
        #region Heuristics

        private double EuclidHeuristic(Point point) => Math.Sqrt(Math.Pow(point.X - mTarget.X, 2) + Math.Pow(point.Y - mTarget.Y, 2));
        
        // private double ManhattenHeuristic(Point point) => Math.Abs(mTarget.X - point.X) + Math.Abs(mTarget.Y - point.Y);

        #endregion
        
        #region Test

        /*
        public void test1()
        {
            PriorityQueue queue = new PriorityQueue();

            Node node1 = new Node(new Point(0, 0), null, 0, EuclidHeuristic(new Point(0, 0)));
            Node node2 = new Node(new Point(0, 1), null, 1, EuclidHeuristic(new Point(0, 1)));
            Node node3 = new Node(new Point(1, 0), null, 1, EuclidHeuristic(new Point(1, 0)));
            Node node4 = new Node(new Point(1, 1), null, 2, EuclidHeuristic(new Point(1, 1)));
            Node node5 = new Node(new Point(2, 0), null, 2, EuclidHeuristic(new Point(2, 0)));
            Node node6 = new Node(new Point(0, 2), null, 2, EuclidHeuristic(new Point(0, 2)));
            Node node7 = new Node(new Point(2, 1), null, 3, EuclidHeuristic(new Point(2, 1)));
            Node node8 = new Node(new Point(1, 2), null, 3, EuclidHeuristic(new Point(1, 2)));
            Node node9 = new Node(new Point(2, 2), null, 4, EuclidHeuristic(new Point(2, 2)));

            queue.Insert(node1);
            queue.Insert(node2);
            queue.Insert(node3);
            queue.RemoveMin();
            queue.Insert(node4);
            queue.Insert(node5);
            queue.RemoveMin();
            queue.Insert(node6);
            queue.Insert(node7);
            queue.Insert(node8);
            queue.Insert(node9);
        }*/

        #endregion
        
        #region ObstacleEnvironment
        private void UpdateObstacles(InputManager inputManager)
        {
            if (inputManager.KeyPressed(Keys.D1, Keys.D2, Keys.D3, Keys.D0))
            {
                // mBlocked.Clear();
                if (inputManager.KeyPressed(Keys.D1))
                {
                    ChangeObstacleEnvironment(1);
                }

                else if (inputManager.KeyPressed(Keys.D2))
                {
                    ChangeObstacleEnvironment(2);
                }

                else if (inputManager.KeyPressed(Keys.D3))
                {
                    ChangeObstacleEnvironment(3);
                }
                else
                {
                    ChangeObstacleEnvironment(0);
                }
            }
        }

        private List<Point> CreateObstacleEnvironment(int testEnvironment = 1)
        {
            // create a list with obstacles
            List<Point> blocked = new List<Point>();
            if (testEnvironment == 1) // debug test a local minimum
            {
                blocked.Add(new Point(0, 5));
                blocked.Add(new Point(1, 5));
                blocked.Add(new Point(2, 5));
                blocked.Add(new Point(3, 5));
                blocked.Add(new Point(4, 5));
                blocked.Add(new Point(5, 5));
                blocked.Add(new Point(5, 4));
                blocked.Add(new Point(5, 3));
                blocked.Add(new Point(5, 2));
            }
            
            else if (testEnvironment == 2) // debug test a way deeper local minimum
            {
                blocked.Add(new Point(0, 13));
                blocked.Add(new Point(1, 13));
                blocked.Add(new Point(2, 13));
                blocked.Add(new Point(3, 13));
                blocked.Add(new Point(4, 13));
                blocked.Add(new Point(5, 13));
                blocked.Add(new Point(5, 12));
                blocked.Add(new Point(5, 11));
                blocked.Add(new Point(5, 10));
                blocked.Add(new Point(5, 9));
                blocked.Add(new Point(5, 8));
                blocked.Add(new Point(5, 7));
                blocked.Add(new Point(5, 6));
                blocked.Add(new Point(5, 5));
                blocked.Add(new Point(5, 4));
                blocked.Add(new Point(5, 3));
                blocked.Add(new Point(5, 2));
            }
            
            else if (testEnvironment == 3) // debug test a impossible field
            {
                blocked.Add(new Point(0, 11));
                blocked.Add(new Point(1, 11));
                blocked.Add(new Point(2, 11));
                blocked.Add(new Point(3, 11));
                blocked.Add(new Point(4, 11));
                blocked.Add(new Point(5, 11));
                blocked.Add(new Point(6, 11));
                blocked.Add(new Point(7, 11));
                blocked.Add(new Point(8, 11));
                blocked.Add(new Point(9, 11));
            }

            return blocked;
        }

        internal void ChangeObstacleEnvironment(int environment)
        {
            mWalkable = new List<Point>();
            mBlocked = CreateObstacleEnvironment(environment);
            foreach (var point in mCoordinateList)
            {
                if (!mBlocked.Contains(point)) { mWalkable.Add(point); }
            }
        }

        #endregion ObstacleEnvironment

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