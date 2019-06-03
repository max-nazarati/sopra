using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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

        // private List<Point> mNeighbours;
        private bool mIsStart;
        private bool mIsGoal;


        public Point Position { get => mPosition; set => mPosition = value; }
        public Node Parent { get => mParent; set => mParent = value; }
        public double Cost { get => mCost; set => mCost = value; }
        public double EstimatedCost { get; set; }

        public double Key => mKey;

        public Node(Point coordinate, Node parent, double cost, double estimatedCost,
        bool start=false, bool goal=false)
        {
            mPosition = coordinate;
            mParent = parent;
            mCost = cost;
            EstimatedCost = estimatedCost;
            mKey = cost + estimatedCost;
            mIsStart = start;
            mIsGoal = goal;
            // mNeighbours = neighbours;
        }

    }

    sealed class PriorityQueue
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
        /// <param name="currentIndex"></param>
        /// <returns></returns>
        private Node Remove(int currentIndex)
        {
            // assert to check for indexError
            var lastIndex = mItems.Count - 1;
            if (currentIndex > lastIndex)
            {
                return null;
            }
            mCount--;

            // swap the item we want to last place for easier removal
            var item = mItems[currentIndex];
            Swap(currentIndex, lastIndex);
            mItems.RemoveAt(lastIndex);
            lastIndex--;

            // as long as the current item has both children
            while (Right(currentIndex) <= lastIndex)
            {
                // if left child is smaller, swap with left child
                if (mItems[Left(currentIndex)].Key <= mItems[Right(currentIndex)].Key)
                {
                    if (mItems[currentIndex].Key > mItems[Left(currentIndex)].Key)
                    {
                        Swap(currentIndex, Left(currentIndex));
                    }
                    currentIndex = Left(currentIndex);

                }
                // if right child is smaller, swap with right child
                else
                {
                    if (mItems[currentIndex].Key > mItems[Right(currentIndex)].Key)
                    {
                        Swap(currentIndex, Right(currentIndex));
                    }
                    currentIndex = Right(currentIndex);
                }
            }
            // maybe there is still a left child but not a right child:
            if (Left(currentIndex) <= lastIndex)
            {
                // if left child is smaller than the current, swap those
                if (mItems[currentIndex].Key > mItems[Left(currentIndex)].Key)
                {
                    Swap(currentIndex, Left(currentIndex));
                }
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

        /*
        public Node GetMin()
        {
            if (mItems.Count > 0) return mItems[0];
            throw new Exception("test");
        }
        */

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

        // parent of 'i' is at '(i-1)/2'
        private static int Parent(int currentIndex)
        {
            return (currentIndex - 1) / 2;
        }
        // right child of 'i' is at '(2 * i) + 1'
        private static int Left(int currentIndex)
        {
            return (currentIndex * 2) + 1;
        }
        // right child of 'i' is at '(2 * i) + 2' 
        private static int Right(int currentIndex)
        {
            return (currentIndex * 2) + 2;
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

        // debugging the path visually
        private List<Point> mPath;
        private readonly Texture2D mTile;



        public AStar(List<Point> coordinateList, Point start, Point target, ContentManager content)
        {
            mCoordinateList = coordinateList;
            mExploredNodes = new List<Point>();
            mTarget = target;
            mStart = start;

            // debug visually (only reason for imcludijng contentmanager)
            mTile = content.Load<Texture2D>("LaneTile");
        }

        private double EuclidHeuristic(Point point) => Math.Sqrt(Math.Pow(point.X - mTarget.X, 2) + Math.Pow(point.Y - mTarget.Y, 2));
        
        // private double ManhattenHeuristic(Point point) => Math.Abs(mTarget.X - point.X) + Math.Abs(mTarget.Y - point.Y);

        public void SetStart(Point start)
        {
            if (mCoordinateList.Contains(start))
            {
                mStart = start;
            }
        }
        
        public void SetTarget(Point target)
        {
            if (mCoordinateList.Contains(target))
            {
                mTarget = target;
            }
        }

        public bool IsStartPosition(Point position) => position == mStart;

        public bool IsTargetPosition(Point position) => position == mTarget;

        public List<Node> CreateNeighbours(Node node)
        {
            var neighbours = new List<Node>();
            var x = node.Position.X;
            var y = node.Position.Y;
            Point up = new Point(x, y - 1);
            Point left = new Point(x - 1, y);
            Point down = new Point(x, y + 1);
            Point right = new Point(x + 1, y);
            double estimatedCost;
            double cost = node.Cost + 1;
            bool isStartNode = false;
            bool isTargetNode = false;

            if (mCoordinateList.Contains(up))
            {
                estimatedCost = EuclidHeuristic(up);
                isStartNode = IsStartPosition(up);
                isTargetNode = IsTargetPosition(up);
                Node nodeUp = new Node(up, node, cost, estimatedCost, isStartNode, isTargetNode);
                neighbours.Add(nodeUp);
            }

            if (mCoordinateList.Contains(down))
            {
                estimatedCost = EuclidHeuristic(down);
                isStartNode = IsStartPosition(down);
                isTargetNode = IsTargetPosition(down);
                Node nodeDown = new Node(down, node, cost, estimatedCost, isStartNode, isTargetNode);
                neighbours.Add(nodeDown);
            }

            if (mCoordinateList.Contains(left))
            {
                estimatedCost = EuclidHeuristic(left);
                isStartNode = IsStartPosition(left);
                isTargetNode = IsTargetPosition(left);
                Node nodeLeft = new Node(left, node, cost, estimatedCost, isStartNode, isTargetNode);
                neighbours.Add(nodeLeft);
            }

            if (mCoordinateList.Contains(right))
            {
                estimatedCost = EuclidHeuristic(right);
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
                if (!mExploredNodes.Contains(neighbour.Position)) mHeap.Insert(neighbour);
                // Console.WriteLine(neighbour.Position.ToString());
            }
            mExploredNodes.Add(node.Position);
            // Console.WriteLine(mExploredNodes.Count);
            // Console.WriteLine(mHeap.Count);
            // Console.WriteLine(node.Position.ToString());
            // Console.WriteLine(node.Position.ToString());
        }
        public Node FindTarget()
        {

            var startNode = new Node(mStart, null, 0, EuclidHeuristic(mStart), true, false);
            // double heuristicValue = startNode.Key;
            mHeap.Insert(startNode);
            
            while (!mHeap.IsEmpty())
            {
                var heapNode = mHeap.RemoveMin();
                if (IsTargetPosition(heapNode.Position)) return heapNode;
                ExpandNode(heapNode);
                Console.Write("I am currently expanding: ");
                Console.WriteLine(heapNode.Position);
            }

            Console.WriteLine("Couldn't find a path");
            return null;
        }

        public List<Point> FindPath()
        {
            // Reset();
            List<Point> path = new List<Point>();
            Node goalNode = FindTarget();
            if (goalNode == null)
            {
                Console.Write("Goal Node was null: ");
                Console.WriteLine("why tho");
                return path;
            }
            path.Add(goalNode.Position);
            Node currenNode = goalNode;
            Point currentPosition = goalNode.Position;
            int count = 0;
            while (!IsStartPosition(currentPosition) && count < 1000)
            {
                count++;
                currenNode = currenNode.Parent;
                currentPosition = currenNode.Position;
                path.Add(currentPosition);
            }

            return path;
        }

        public void CalculatePath()
        {
            // Reset();
            List<Point> path = new List<Point>();
            Node goalNode = FindTarget();
            if (goalNode == null)
            {
                Console.Write("Goal Node was null: ");
                Console.WriteLine("why tho");
                return;
            }
            path.Add(goalNode.Position);
            Node currenNode = goalNode;
            Point currentPosition = goalNode.Position;
            int count = 0;
            while (!IsStartPosition(currentPosition) && count < 1000)
            {
                count++;
                currenNode = currenNode.Parent;
                currentPosition = currenNode.Position;
                path.Add(currentPosition);
            }
            path.Reverse();
            mPath = path;
        }

        public void Reset()
        {
            mExploredNodes.Clear();
            mHeap = new PriorityQueue();
        }

        public void UpdateStartAndTarget()
        {
            if (InputManager.Default.KeyPressed(Keys.Up, Keys.Left, Keys.Down, Keys.Right))
            {
                if (InputManager.Default.KeyPressed(Keys.Up))
                {
                    if (InputManager.Default.KeyDown(Keys.LeftShift))
                    {
                        SetStart(new Point(mStart.X, mStart.Y - 1));
                    }
                    else
                    {
                        SetTarget(new Point(mTarget.X, mTarget.Y - 1));
                    }
                    
                }
                if (InputManager.Default.KeyPressed(Keys.Left))
                {
                    if (InputManager.Default.KeyDown(Keys.LeftShift))
                    {
                        SetStart(new Point(mStart.X - 1, mStart.Y));
                    }
                    else
                    {
                        SetTarget(new Point(mTarget.X - 1, mTarget.Y));
                    }

                }
                if (InputManager.Default.KeyPressed(Keys.Down))
                {
                    if (InputManager.Default.KeyDown(Keys.LeftShift))
                    {
                        SetStart(new Point(mStart.X, mStart.Y + 1));
                    }
                    else
                    {
                        SetTarget(new Point(mTarget.X, mTarget.Y + 1));
                    }
                }
                if (InputManager.Default.KeyPressed(Keys.Right))
                {
                    if (InputManager.Default.KeyDown(Keys.LeftShift))
                    {
                        SetStart(new Point(mStart.X + 1, mStart.Y));
                    }
                    else
                    {
                        SetTarget(new Point(mTarget.X + 1, mTarget.Y));
                    }
                }
                Reset();
                CalculatePath();
            }
        }

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
        public void DrawPath(SpriteBatch spriteBatch)
        {
            if (mPath == null)
            {
                mPath = FindPath();
            }
            foreach (var point in mPath)
            {
                DrawTile(spriteBatch, point);
            }

        }
        private void DrawTile(SpriteBatch spriteBatch, Point point)
        {
            var pos = Grid.ScreenPositionFromCoordinate(point);
            spriteBatch.Draw(mTile, new Rectangle(pos.X, pos.Y, (100), (100)), Color.Red);
        }
    }
 
}