using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace KernelPanic.Data
{
    /// <summary>
    /// Class representing a priority queue.
    /// The left child of i is 2i+1 and the right one 2i+2.
    /// The parent of i is (i-1)//2
    /// </summary>
    internal sealed class Node
    {
        internal Point Position { get; } // set; }

        internal Node Parent { get; } // set => mParent = value; }
        internal double Cost { get; } // set; }

        private double EstimatedCost { get; }  //set; }

        internal double Key { get; }

        internal Node(Point coordinate, Node parent, double cost, double estimatedCost)
        {
            Position = coordinate;
            Parent = parent;
            Cost = cost;
            EstimatedCost = estimatedCost;
            Key = cost + estimatedCost;
        }
    }

    sealed class PriorityQueue
    {
        private readonly List<Node> mItems;
        private int mCount;
        public PriorityQueue()
        {
            mItems = new List<Node>();
        }

        public void Clear()
        {
            mItems.Clear();
            mCount = 0;
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
        
        // left child of 'i' is at '(2 * i) + 1'
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
}