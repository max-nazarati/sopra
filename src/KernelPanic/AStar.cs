using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;

namespace KernelPanic
{
    /*
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
        private List<Point> mNeighbours;
        private bool mIsStart;
        private bool mIsGoal;

        public void CalculateKey()
        {
            mKey = mEstimatedCost + mCost;
        }

        public Point Position => mPosition;
        public double Key => mKey;

        public Node(Point coordinate, Node parent, double cost, double estimatedCost, List<Point> neighbours,
        bool start=false, bool goal=false)
        {
            mPosition = coordinate;
            mParent = parent;
            mCost = cost;
            mEstimatedCost = estimatedCost;
            CalculateKey();
            mIsStart = start;
            mIsGoal = goal;
            mNeighbours = neighbours;
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
        public void Insert(Node item, double newKey)
        {
            var queueIndex = mItems.Count;
            mItems.Add(item);
            DecreaseKey(queueIndex, newKey);
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
        }

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
        private const bool GRID_BLOCKED = true;

        private List<Point> mCoordinateList;
        private List<List<bool>> mMap;
        private PriorityQueue mHeap = new PriorityQueue();
        private Point mTarget;
        private Point mStart;

        public AStar(List<Point> coordinateList, Point start, Point target)
        {
            // LoadListIntoQueue(coordinateList, start);
            mTarget = target;
            mStart = start;
        }

        private double EuclidHeuristic(Point point) => Math.Sqrt(Math.Pow(point.X, 2) + Math.Pow(point.Y, 2));

        private double ManhattenHeuristic(Point point) => Math.Abs(mTarget.X - point.X) + Math.Abs(mTarget.Y - point.Y);

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
        }

        public void createNeighbours(Node node)
        {
            List<Point> neighbours = new List<Point>();
            var x = node.Position.X;
            var y = node.Position.Y;
            Point up = new Point(x, y + 1);
            Point left = new Point(x - 1, y);
            Point down = new Point(x, y + 1);
            Point right = new Point(x + 1, y);
            if (mCoordinateList.Contains(up))
            {
                neighbours.Add(up);
            }
        }
        
        public void ExpandNode(Node node)
        {
            List<Point> neighbours = new List<Point>();
            
            
        }
        public List<Node> FindPath()
        {
            Node startNode = new Node(mStart, null, 0, ManhattenHeuristic(mStart), true, false);
            double heuristicValue = startNode.Key;
            mHeap.Insert(startNode, heuristicValue);

            while (!mHeap.IsEmpty())
            {
                Node heapNode = mHeap.RemoveMin();
                
            }
        }
        
    }
    */
}