using System;
using System.Collections.Generic;
using System.Linq;
using KernelPanic.Data;
using Microsoft.Xna.Framework;

namespace KernelPanic.PathPlanning
{
    /// <summary>
    /// Supporting functions shared between <see cref="AStar"/> and <see cref="BreadthFirstSearch"/>.
    /// </summary>
    internal static class PathPlanner
    {
        /// <summary>
        /// Enumerates through the neighbours of a node. The resulting nodes can be further filtered using
        /// <see cref="Enumerable.Where{Node}(System.Collections.Generic.IEnumerable{Node},System.Func{Node,bool})"/>.
        /// </summary>
        /// <param name="node">The neighbours of this node are returned.</param>
        /// <param name="costIncrease">The value by which the cost of the new nodes is higher than the cost of <paramref name="node"/>.</param>
        /// <param name="costEstimation">Used to perform an estimation based on the new nodes point. If it is <c>null</c>, zero is used.</param>
        /// <returns></returns>
        internal static IEnumerable<Node> EnumerateNeighbours(this Node node,
            double costIncrease,
            Func<Point, double> costEstimation)
        {
            var cost = node.Cost + costIncrease;

            Node CreateNode(int xOffset, int yOffset)
            {
                var point = node.Position + new Point(xOffset, yOffset);
                var estimatedCost = costEstimation?.Invoke(point) ?? 0;
                return new Node(point, node, cost, estimatedCost);
            }

            yield return CreateNode(0, -1);
            yield return CreateNode(0, 1);
            yield return CreateNode(-1, 0);
            yield return CreateNode(1, 0);
        }
    }
}