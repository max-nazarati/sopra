using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace KernelPanic
{
  
    /// <summary>
    /// Class representing a priority queue.
    /// The left child of i is 2i+1 and the right one 2i+2.
    /// The parent of i is (i-1)//2
    /// </summary>
    public sealed class Node
    {
        // private List<Point> mNeighbours;
        private bool mIsStart;
        private bool mIsGoal;


        internal Point Position { get; } // set; }

        internal Node Parent { get; } // set => mParent = value; }
        internal double Cost { get; } // set; }

        private double EstimatedCost { get; }  //set; }

        internal double Key { get; }

        internal Node(Point coordinate, Node parent, double cost, double estimatedCost,
        bool start=false, bool goal=false)
        {
            Position = coordinate;
            Parent = parent;
            Cost = cost;
            EstimatedCost = estimatedCost;
            Key = cost + estimatedCost;
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

        // mainly debugging reasons for now
        private List<Point> mBlocked;
        private List<Point> mWalkable;
        
        // debugging the path visually
        private List<Point> mPath;
        private ImageSprite mTile;



        internal AStar(List<Point> coordinateList, Point start, Point target, SpriteManager sprite)
        {
            mCoordinateList = coordinateList;
            mExploredNodes = new List<Point>();
            mTarget = target;
            mStart = start;

            // debug visually (only reason for imcludijng contentmanager)
            // mTile = content.Load<Texture2D>("LaneTile");
            mTile = Grid.CreateTile(sprite);
            mWalkable = new List<Point>();// = coordinateList;
            mBlocked = new List<Point>();
        }
        
        public void Update()
        {
            UpdateObstacles();
            UpdateStartAndTarget();
            CalculatePath();
        }
        
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            DrawObstacles(spriteBatch, gameTime);
            DrawExplored(spriteBatch, gameTime);
            DrawPath(spriteBatch, gameTime);
            DrawStartAndTarget(spriteBatch, gameTime);
            
        }

        private void UpdateObstacles()
        {
            if (InputManager.Default.KeyPressed(Keys.D1, Keys.D2, Keys.D3, Keys.D0))
            {
                // mBlocked.Clear();
                if (InputManager.Default.KeyPressed(Keys.D1))
                {
                    ChangeObstacleEnvironment(1);
                }

                else if (InputManager.Default.KeyPressed(Keys.D2))
                {
                    ChangeObstacleEnvironment(2);
                }

                else if (InputManager.Default.KeyPressed(Keys.D3))
                {
                    ChangeObstacleEnvironment(3);
                }
                else
                {
                    ChangeObstacleEnvironment(0);
                }
            }
        }

        private void UpdateStartAndTarget()
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
                CalculatePath();
            }
        }

        private double EuclidHeuristic(Point point) => Math.Sqrt(Math.Pow(point.X - mTarget.X, 2) + Math.Pow(point.Y - mTarget.Y, 2));
        
        // private double ManhattenHeuristic(Point point) => Math.Abs(mTarget.X - point.X) + Math.Abs(mTarget.Y - point.Y);

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

        private bool IsStartPosition(Point position) => position == mStart;

        private bool IsTargetPosition(Point position) => position == mTarget;

        private List<Node> CreateNeighbours(Node node)
        {
            var neighbours = new List<Node>();
            var x = node.Position.X;
            var y = node.Position.Y;
            var up = new Point(x, y - 1);
            var left = new Point(x - 1, y);
            var down = new Point(x, y + 1);
            var right = new Point(x + 1, y);
            double estimatedCost;
            var cost = node.Cost + 0.5; // if we put an higher value as summand we get a 'dumb' search
            bool isStartNode; // = false;
            bool isTargetNode; // = false;

            if (mWalkable.Contains(up))
            {
                estimatedCost = EuclidHeuristic(up);
                isStartNode = IsStartPosition(up);
                isTargetNode = IsTargetPosition(up);
                Node nodeUp = new Node(up, node, cost, estimatedCost, isStartNode, isTargetNode);
                neighbours.Add(nodeUp);
            }

            if (mWalkable.Contains(down))
            {
                estimatedCost = EuclidHeuristic(down);
                isStartNode = IsStartPosition(down);
                isTargetNode = IsTargetPosition(down);
                Node nodeDown = new Node(down, node, cost, estimatedCost, isStartNode, isTargetNode);
                neighbours.Add(nodeDown);
            }

            if (mWalkable.Contains(left))
            {
                estimatedCost = EuclidHeuristic(left);
                isStartNode = IsStartPosition(left);
                isTargetNode = IsTargetPosition(left);
                Node nodeLeft = new Node(left, node, cost, estimatedCost, isStartNode, isTargetNode);
                neighbours.Add(nodeLeft);
            }

            if (mWalkable.Contains(right))
            {
                estimatedCost = EuclidHeuristic(right);
                isStartNode = IsStartPosition(right);
                isTargetNode = IsTargetPosition(right);
                Node nodeRight = new Node(right, node, cost, estimatedCost, isStartNode, isTargetNode);
                neighbours.Add(nodeRight);
            }

            return neighbours;
        }

        private void ExpandNode(Node node)
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

        private Node FindTarget()
        {
            Reset();
            var startNode = new Node(mStart, null, 0, EuclidHeuristic(mStart), true);
            // double heuristicValue = startNode.Key;
            mHeap.Insert(startNode);
            
            while (!mHeap.IsEmpty())
            {
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
            // Reset();
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
            var count = 0;
            while (!IsStartPosition(currentPosition) && count < 1000)
            {
                count++;
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

        private void DrawStartAndTarget(SpriteBatch spriteBatch, GameTime gameTime)
        {
            DrawTile(spriteBatch, mStart, gameTime, Color.Firebrick);
            DrawTile(spriteBatch, mTarget, gameTime, Color.Firebrick);
        }

        private void DrawExplored(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (mExploredNodes == null) { return; }

            foreach (var point in mExploredNodes)
            {
                DrawTile(spriteBatch, point, gameTime, Color.Pink);
            }
        }

        private void DrawPath(SpriteBatch spriteBatch, GameTime gameTime)
        {
            // Console.WriteLine("drawing the path");
            if (mPath == null) { return; }
            
            foreach (var point in mPath)
            {
                DrawTile(spriteBatch, point, gameTime, Color.Red); 
            }
        }

        private void DrawObstacles(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (mBlocked == null) { return; }

            foreach (var point in mBlocked)
            {
                DrawTile(spriteBatch, point, gameTime, Color.DarkSlateGray);
            }
        }
        
        private void DrawTile(SpriteBatch spriteBatch, Point point, GameTime gameTime, Color  color)
        {
            mTile.Position = Grid.ScreenPositionFromCoordinate(point).ToVector2();
            mTile.TintColor = color;
            mTile.Draw(spriteBatch, gameTime);
        }
    }
}