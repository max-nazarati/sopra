using System.Collections.Generic;
using KernelPanic.Data;
using Microsoft.Xna.Framework;

namespace KernelPanic.PathPlanning
{
    internal abstract class PathPlanner
    {
        private readonly PriorityQueue mQueue = new PriorityQueue();
        protected readonly HashSet<Point> Explored = new HashSet<Point>();

        protected abstract bool IsWalkable(Point point);
        protected abstract double EstimateCost(Point point);
        protected abstract double CostIncrease { get; }

        /// <summary>
        /// Enumerates through the neighbours of a node. Only walkable nodes (see <see cref="IsWalkable"/>) are yielded.
        /// </summary>
        /// <param name="node">The neighbours of this node are returned.</param>
        /// <returns>An enumeration of all walkable neighbour nodes.</returns>
        private IEnumerable<Node> EnumerateNeighbours(Node node)
        {
            var cost = node.Cost + CostIncrease;

            Node CreateNode(int xOffset, int yOffset)
            {
                var point = node.Position + new Point(xOffset, yOffset);
                return IsWalkable(point) ? new Node(point, node, cost, EstimateCost(point)) : null;
            }

            if (CreateNode(0, -1) is Node up) yield return up;
            if (CreateNode(0, 1) is Node down) yield return down;
            if (CreateNode(-1, 0) is Node left) yield return left;
            if (CreateNode(1, 0) is Node right) yield return right;
        }

        private bool ExpandNode(Node node)
        {
            if (!Explored.Add(node.Position))
                return false;

            foreach (var neighbour in EnumerateNeighbours(node))
                mQueue.Insert(neighbour);
            return true;
        }

        protected IEnumerable<Node> Run(IEnumerable<Point> start)
        {
            mQueue.Clear();
            Explored.Clear();

            foreach (var point in start)
                mQueue.Insert(new Node(point, null, 0, EstimateCost(point)));

            while (!mQueue.IsEmpty())
            {
                var node = mQueue.RemoveMin();
                if (ExpandNode(node))
                    yield return node;
            }
        }
    }
}