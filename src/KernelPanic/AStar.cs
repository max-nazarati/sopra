using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
  
    /// <summary>
    /// Class representing a priority queue.
    /// The left child of i is 2i+1 and the right one 2i+2.
    /// The parent of i is (i-1)//2
    /// </summary>
    public class Node
    {
        public double mKey;
        private Point mPosition;
        private Node mParent;
        private double mCost;
        private double mEstimatedCost;
        // private List<Point> mNeighbours;
        private bool mIsStart;
        private bool mIsGoal;

        public void CalculateKey()
        {
            mKey = mEstimatedCost + mCost;
        }

        public Point Position { get => mPosition; set => mPosition = value; }

        public double Cost { get => mCost; set => mCost = value; }
        public double EstimatedCost { get => mEstimatedCost; set => mEstimatedCost = value; }
        public double Key => mKey;

        public Node(Point coordinate, Node parent, double cost, double estimatedCost,
        bool start=false, bool goal=false)
        {
            mPosition = coordinate;
            mParent = parent;
            mCost = cost;
            mEstimatedCost = estimatedCost;
            CalculateKey();
            mIsStart = start;
            mIsGoal = goal;
            // mNeighbours = neighbours;
        }

    }

    class PriorityQueue
    {
        private List<Node> mItems;
        private int mCount;
        public PriorityQueue()
        {
            mItems = new List<Node>();
        }
        
        /// <summary>
        /// Insert a new Element into the Queue
        /// </summary>
        /// <param name="item"></param>
        /// <param name="newKey"></param>
        public void Insert(Node item)
        {
            var queueIndex = mItems.Count;
            mItems.Add(item);
            // DecreaseKey(queueIndex, newKey);
            // heapify
            while (queueIndex > 0 && mItems[queueIndex].Key < mItems[Parent(queueIndex)].Key)
            {
                Swap(queueIndex, Parent(queueIndex));
                queueIndex = Parent(queueIndex);
            }

            mCount++;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="removeIndex"></param>
        /// <returns></returns>
        private Node Remove(int currentIndex)
        {
            mCount--;
            var item = mItems[currentIndex];
            var lastIndex = mItems.Count - 1;
            Swap(currentIndex, lastIndex);

            while (mItems[currentIndex].Key > mItems[Left(currentIndex)].Key ||
                   mItems[currentIndex].Key > mItems[Right(currentIndex)].Key)
            {
                if (Right(currentIndex) > mItems.Count &&
                    mItems[Left(currentIndex)].Key <= mItems[Right(currentIndex)].Key)
                {
                    Swap(currentIndex, Left(currentIndex));
                    currentIndex = Right(currentIndex);
                    continue;
                }

                if (Right(currentIndex) > mItems.Count &&
                    mItems[Left(currentIndex)].Key >= mItems[Right(currentIndex)].Key)
                {
                    Swap(currentIndex, Right(currentIndex));
                    currentIndex = Left(currentIndex);
                    continue;
                }

                return item;
            }

            return item;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Node RemoveMin()
        {
            if (mItems.Count > 0)
            {
                return Remove(0);
            }
            throw new Exception("cant Remove from an empty Priority Queue");
        }

        /// <summary>
        /// swap the array position of two elements in the Q
        /// </summary>
        /// <param name="pos1"></param>
        /// <param name="pos2"></param>
        private void Swap(int pos1, int pos2)
        {
            var temp = mItems[pos2];
            mItems[pos2] = mItems[pos1];
            mItems[pos1] = temp;
        }

        /*
        /// <summary>
        /// decreasing the key value is increasing its priority
        /// </summary>
        /// <param name="queueIndex"></param>
        /// <param name="newKey"></param>
        private void DecreaseKey(int queueIndex, double newKey)
        {
            mItems[queueIndex].Key = newKey;
            // heapify
            while (queueIndex > 0 && mItems[queueIndex].Key < mItems[Parent(queueIndex)].Key)
            {
                Swap(queueIndex, Parent(queueIndex));
                queueIndex = Parent(queueIndex);
            }
        } */

        private static int Parent(int currentIndex)
        {
            return currentIndex / 2;
        }
        private static int Left(int currentIndex)
        {
            return currentIndex * 2;
        }
        private static int Right(int currentIndex)
        {
            return currentIndex * 2 + 1;
        }

        public bool IsEmpty() => mCount == 0;
    }
    public class AStar
    {
        private List<Point> mCoordinateList;
        private List<Point> mExploredNodes;
        private PriorityQueue mHeap = new PriorityQueue();
        private Point mTarget;
        private Point mStart;

        public AStar(List<Point> coordinateList, Point start, Point target)
        {
            // LoadListIntoQueue(coordinateList, start);
            mTarget = target;
            mStart = start;
        }

        // private double EuclidHeuristic(Point point) => Math.Sqrt(Math.Pow(point.X, 2) + Math.Pow(point.Y, 2));

        private double ManhattenHeuristic(Point point) => Math.Abs(mTarget.X - point.X) + Math.Abs(mTarget.Y - point.Y);

        /*
        void LoadListIntoQueue(List<Point> coordinateList, Point start)
        {
            foreach (var point in coordinateList)
            {
                var newItem = new Node(point, 100);
                if (point == start)
                {
                    newItem.Key = 1;
                    createNeighbours();
                }
                mHeap.Insert(newItem, newItem.Key);
            }
        } */

        public bool IsStartPosition(Point position) => position == mStart;

        public bool IsTargetPosition(Point position) => position == mTarget;

        public List<Node> CreateNeighbours(Node node)
        {
            List<Node> neighbours = new List<Node>();
            var x = node.Position.X;
            var y = node.Position.Y;
            Point up = new Point(x, y + 1);
            Point left = new Point(x - 1, y);
            Point down = new Point(x, y + 1);
            Point right = new Point(x + 1, y);
            double estimatedCost;
            double cost = node.Cost + 1;
            bool isStartNode = false;
            bool isTargetNode = false;

            if (mCoordinateList.Contains(up))
            {
                estimatedCost = ManhattenHeuristic(up);
                isStartNode = IsStartPosition(up);
                isTargetNode = IsTargetPosition(up);
                Node nodeUp = new Node(up, node, cost, estimatedCost, isStartNode, isTargetNode);
                neighbours.Add(nodeUp);
            }

            if (mCoordinateList.Contains(down))
            {
                estimatedCost = ManhattenHeuristic(down);
                isStartNode = IsStartPosition(down);
                isTargetNode = IsTargetPosition(down);
                Node nodeDown = new Node(down, node, cost, estimatedCost, isStartNode, isTargetNode);
                neighbours.Add(nodeDown);
            }

            if (mCoordinateList.Contains(left))
            {
                estimatedCost = ManhattenHeuristic(left);
                isStartNode = IsStartPosition(left);
                isTargetNode = IsTargetPosition(left);
                Node nodeLeft = new Node(left, node, cost, estimatedCost, isStartNode, isTargetNode);
                neighbours.Add(nodeLeft);
            }

            if (mCoordinateList.Contains(right))
            {
                estimatedCost = ManhattenHeuristic(right);
                isStartNode = IsStartPosition(right);
                isTargetNode = IsTargetPosition(right);
                Node nodeRight = new Node(right, node, cost, estimatedCost, isStartNode, isTargetNode);
                neighbours.Add(nodeRight);
            }

            return neighbours;
        }
        
        public void ExpandNode(Node node)
        {
            if (mExploredNodes.Contains(node.Position)) return;
            List<Node> neighbours = CreateNeighbours(node);
            foreach (var neighbour in neighbours)
            {
                mHeap.Insert(neighbour);
            }
            mExploredNodes.Add(node.Position);
        }
        public Node FindTarget()
        {
            var startNode = new Node(mStart, null, 0, ManhattenHeuristic(mStart), true, false);
            double heuristicValue = startNode.Key;
            mHeap.Insert(startNode);

            while (!mHeap.IsEmpty())
            {
                var heapNode = mHeap.RemoveMin();
                if (IsTargetPosition(heapNode.Position)) return heapNode;
                ExpandNode(heapNode);
            }

            return null;
        }
        
    }
 
}